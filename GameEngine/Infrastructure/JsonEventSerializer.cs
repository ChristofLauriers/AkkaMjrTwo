using System;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AkkaMjrTwo.GameEngine.Infrastructure
{
    /// <summary>
    /// Serializer supports Weak Schema Versioning.
    /// - Exists on json and instance -> value from json
    /// - Exists on json but not on instance -> NOP
    /// - Exists on instance but not in json -> default value
    /// More info:
    ///   Versioning in an Event Sourced System - book by Greg Young
    ///   https://leanpub.com/esversioning/read#leanpub-auto-weak-schema
    /// </summary>
    public static class JsonEventSerializer
    {
        /// <summary>
        /// https://www.newtonsoft.com/json/help/html/Performance.htm#MemoryUsage
        /// </summary>
        public static object Deserialize(string json, Type type)
        {
            return Deserialize(Encoding.UTF8.GetBytes(json), type);
        }

        public static object Deserialize(byte[] data, Type type)
        {
            object result;

            var settings = GetCommonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Populate;
            settings.ContractResolver = PrivatePropertySetterResolver.Instance;

            using (Stream s = new MemoryStream(data))
            using (var sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                var serializer = JsonSerializer.Create(settings);

                result = serializer.Deserialize(reader, type);
            }
            return result;
        }

        public static string Serialize(object obj, bool indentedFormatting = false)
        {
            var settings = GetCommonSerializerSettings();
            settings.Formatting = indentedFormatting ? Formatting.Indented : Formatting.None;
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.ContractResolver = SuppressItemTypeNameContractResolver.Instance;

            return JsonConvert.SerializeObject(obj, settings);
        }

        private static JsonSerializerSettings GetCommonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }

    #region ContractResolvers

    public class SuppressItemTypeNameContractResolver : DefaultContractResolver
    {
        // As of 7.0.1, Json.NET suggests using a static instance for "stateless" contract resolvers, for performance reasons.
        // http://www.newtonsoft.com/json/help/html/ContractResolver.htm
        // http://www.newtonsoft.com/json/help/html/M_Newtonsoft_Json_Serialization_DefaultContractResolver__ctor_1.htm
        // "Use the parameterless constructor and cache instances of the contract resolver within your application for optimal performance."
        private static readonly SuppressItemTypeNameContractResolver _instance;

        // Using a static constructor enables fairly lazy initialization.  http://csharpindepth.com/Articles/General/Singleton.aspx
        static SuppressItemTypeNameContractResolver() { _instance = new SuppressItemTypeNameContractResolver(); }

        public static SuppressItemTypeNameContractResolver Instance { get { return _instance; } }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);
            if (contract is JsonContainerContract containerContract)
            {
                if (containerContract.ItemTypeNameHandling == null)
                    containerContract.ItemTypeNameHandling = TypeNameHandling.None;
            }
            return contract;
        }
    }

    public class PrivatePropertySetterResolver : DefaultContractResolver
    {
        // As of 7.0.1, Json.NET suggests using a static instance for "stateless" contract resolvers, for performance reasons.
        // http://www.newtonsoft.com/json/help/html/ContractResolver.htm
        // http://www.newtonsoft.com/json/help/html/M_Newtonsoft_Json_Serialization_DefaultContractResolver__ctor_1.htm
        // "Use the parameterless constructor and cache instances of the contract resolver within your application for optimal performance."
        private static readonly PrivatePropertySetterResolver _instance;

        // Using a static constructor enables fairly lazy initialization.  http://csharpindepth.com/Articles/General/Singleton.aspx
        static PrivatePropertySetterResolver() { _instance = new PrivatePropertySetterResolver(); }

        public static PrivatePropertySetterResolver Instance { get { return _instance; } }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.SetMethod != null;
                    prop.Writable = hasPrivateSetter;
                }
            }
            return prop;
        }
    }

    #endregion
}
