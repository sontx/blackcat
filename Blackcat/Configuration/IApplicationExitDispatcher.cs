using System;

namespace Blackcat.Configuration
{
    public interface IApplicationExitDispatcher
    {
        event EventHandler Exit;
    }
}