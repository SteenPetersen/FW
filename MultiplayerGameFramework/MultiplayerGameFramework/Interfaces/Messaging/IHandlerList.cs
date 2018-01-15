using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameFramework.Interfaces.Messaging
{
    public interface IHandlerList<T>
    {
        // <T> determines wether it is a client or a server
        bool RegisterHandler(IHandler<T> handler);
        bool HandleMessage(IMessage message, T peer);
    }
}
