using System;
using System.Configuration;

namespace Bolt.Cache.Redis.Configs
{
    public class ConnectionSettingsSection : ConfigurationSection, IConnectionSettings
    {
        public static ConnectionSettingsSection Instance(string sectionName)
        {
            var result = ConfigurationManager.GetSection(sectionName) as ConnectionSettingsSection;

            if (result == null) throw new ConfigurationErrorsException(string.Format("Failed to load ConnectionSettingsSection using section name {0}", sectionName));

            return result;
        }

        [ConfigurationProperty("ConnectionStringOrName", IsRequired = true)]
        public string ConnectionStringOrName { get{ return this["ConnectionStringOrName"] as string;} }

        [ConfigurationProperty("Database", IsRequired = false, DefaultValue = 0)]
        public int Database { get { return Convert.ToInt32(this["Database"]); } }
    }
}