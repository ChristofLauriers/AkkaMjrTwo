using System;
using System.Text;
using Akka.Actor;
using Akka.Serialization;

namespace AkkaMjrTwo.Infrastructure.Akka
{
    public class EventSerializer : Serializer
    {
        public override bool IncludeManifest => true;
        public override int Identifier => 800;
        
        public EventSerializer(ExtendedActorSystem system)
            : base(system)
        { }


        public override object FromBinary(byte[] bytes, Type type)
        {
            return JsonEventSerializer.Deserialize(bytes, type);
        }

        public override byte[] ToBinary(object obj)
        {
            var json = JsonEventSerializer.Serialize(obj);
            var bytes = Encoding.UTF8.GetBytes(json);
            return bytes;
        }
    }
}
