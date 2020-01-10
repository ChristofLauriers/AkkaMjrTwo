using System.IO;

namespace AkkaMjrTwo.Infrastructure.Akka
{
    public static class ConfigurationLoader
    {
        public static global::Akka.Configuration.Config Load() => LoadConfig("akka.conf");

        private static global::Akka.Configuration.Config LoadConfig(string configFile)
        {
            if (File.Exists(configFile))
            {
                string config = File.ReadAllText(configFile);
                return global::Akka.Configuration.ConfigurationFactory.ParseString(config);
            }

            return global::Akka.Configuration.Config.Empty;
        }
    }
}
