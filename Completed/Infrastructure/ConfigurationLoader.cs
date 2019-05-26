using System.IO;

namespace AkkaMjrTwo.Infrastructure
{
    public static class ConfigurationLoader
    {
        public static Akka.Configuration.Config Load() => LoadConfig("akka.conf");

        private static Akka.Configuration.Config LoadConfig(string configFile)
        {
            if (File.Exists(configFile))
            {
                string config = File.ReadAllText(configFile);
                return Akka.Configuration.ConfigurationFactory.ParseString(config);
            }


            return Akka.Configuration.Config.Empty;
        }
    }
}
