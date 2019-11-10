namespace Blackcat.Configuration
{
    public interface IConfigLoader
    {
        T Get<T>() where T : class;
    }
}