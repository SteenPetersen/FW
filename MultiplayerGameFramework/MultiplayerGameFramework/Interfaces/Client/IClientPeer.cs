using MultiplayerGameFramework.Interfaces.Messaging;
using System;

namespace MultiplayerGameFramework.Interfaces.Client
{
    public interface IClientPeer
    {
        // to know if this is a proxy class on another server or not
        bool IsProxy { get; set; }
        Guid PeerId { get; set; }
        T ClientData<T>() where T : class, IClientData;
        void Disconnect();
        void SendMessage(IMessage message);

    }
}
