using MultiplayerGameFramework.Interfaces.Client;
using MultiplayerGameFramework.Interfaces.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameFramework.Implementation.Messaging
{
    // must be abstract because we define methods that we use later on
    public abstract class ClientHandler : IHandler<IClientPeer>
    {
        // made abstract so that we can override them later
        public abstract MessageType Type { get; }
        public abstract byte Code { get; }
        public abstract int? SubCode { get; }


        // tells us wether or not we handled this message
        public bool HandleMessage(IMessage message, IClientPeer peer)
        {
            return OnHandleMessage(message, peer);
        }

        protected abstract bool OnHandleMessage(IMessage message, IClientPeer peer);
    }
}
