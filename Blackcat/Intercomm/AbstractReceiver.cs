using System;

namespace Blackcat.Intercomm
{
    public abstract class AbstractReceiver : AbstractIntercomm, IReceiver
    {
        public Func<IProtocol, ISession> SessionFactory { get; set; }

        public AbstractReceiver()
        {
            SessionFactory = DefaultSessionFactory;
        }

        private ISession DefaultSessionFactory(IProtocol protocol)
        {
            return new SessionImpl(protocol);
        }
    }
}