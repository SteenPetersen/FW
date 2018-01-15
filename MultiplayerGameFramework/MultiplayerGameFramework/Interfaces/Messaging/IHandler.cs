

using MultiplayerGameFramework.Implementation.Messaging;

namespace MultiplayerGameFramework.Interfaces.Messaging
{
    public interface IHandler<T>
    {
        MessageType Type { get; }
        // what code should it look for if its going to handle that message
        byte Code { get; }
        // if it has NO subcode it is going to handle any message if it does its going to look for that specific subcode
        int? SubCode { get; }
        bool HandleMessage(IMessage message, T peer);
    }
}
