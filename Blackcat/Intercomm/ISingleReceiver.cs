namespace Blackcat.Intercomm
{
    /// <summary>
    /// Supports receiving from a single <see cref="ISender"/>
    /// </summary>
    public interface ISingleReceiver : IReceiver, ISession
    {
    }
}