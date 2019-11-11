namespace Blackcat.Configuration
{
    public interface IConfigLoader
    {
        SaveMode SaveMode { get; set; }

        T Get<T>() where T : class;
    }
}