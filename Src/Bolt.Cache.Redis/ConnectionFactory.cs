using System.Configuration;
using StackExchange.Redis;

namespace Bolt.Cache.Redis
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IConnectionSettings _settings;
        private ConnectionMultiplexer _instance;
        private static volatile object _syncLock = new object();

        public ConnectionFactory(IConnectionSettings settings)
        {
            _settings = settings;
        }

        public ConnectionMultiplexer Create()
        {
            if (_instance != null && _instance.IsConnected) return _instance;

            lock (_syncLock)
            {
                if (_instance != null && _instance.IsConnected) return _instance;

                if (_instance != null)
                {
                    _instance.Dispose();
                }

                var connectionStringSetting = ConfigurationManager.ConnectionStrings[_settings.ConnectionStringOrName];
                
                _instance = ConnectionMultiplexer.Connect(connectionStringSetting == null ? _settings.ConnectionStringOrName : connectionStringSetting.ConnectionString);
            }

            return _instance;
        }

        public void Dispose()
        {
            if (_instance != null)
            {
                _instance.Dispose();
            }
        }
    }
}