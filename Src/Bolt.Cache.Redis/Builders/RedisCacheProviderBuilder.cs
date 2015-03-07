using System;
using Bolt.Cache.Redis.Configs;
using Bolt.Serializer;

namespace Bolt.Cache.Redis.Builders
{
    public class RedisCacheProviderBuilder
    {
        private const string DefaultName = "Redis";
        private const int DefaultOrder = 5;
        private const string DefaultSettingsSectionName = "Bolt.Cache.Redis.Settings";

        private IConnectionSettings _connectionSettings;
        private string _settingsSectionName;
        private IConnectionFactory _connectionFactory;
        private ISerializer _serializer;

        private RedisCacheProviderBuilder() { }

        public static RedisCacheProviderBuilder New()
        {
            return new RedisCacheProviderBuilder();
        }

        public RedisCacheProviderBuilder SettingsSectionName(string sectionName)
        {
            _settingsSectionName = sectionName;
            return this;
        }

        public RedisCacheProviderBuilder ConnectionSettings(IConnectionSettings settings)
        {
            _connectionSettings = settings;
            return this;
        }

        public RedisCacheProviderBuilder ConnectionFactory(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            return this;
        }

        public RedisCacheProviderBuilder Serializer(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        public ICacheProvider Build()
        {
            if(_serializer == null) throw new Exception("Serializer is required. Please provide an instance of ISerializer");

            if (_connectionSettings == null)
            {
                _connectionSettings = ConnectionSettingsSection.Instance(_settingsSectionName ?? DefaultSettingsSectionName);
            }

            if (_connectionFactory == null)
            {
                _connectionFactory = new ConnectionFactory(_connectionSettings);
            }

            return new RedisCacheProvider(DefaultName, DefaultOrder, _connectionSettings,_connectionFactory,_serializer);
        }
    }
}
