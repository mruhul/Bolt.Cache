namespace Bolt.Cache.Redis
{
    public interface IConnectionSettings
    {
        string ConnectionStringOrName { get; }
        int Database { get; }
    }
}