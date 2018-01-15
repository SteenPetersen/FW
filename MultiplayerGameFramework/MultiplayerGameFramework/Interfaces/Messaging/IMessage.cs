using MultiplayerGameFramework.Implementation.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameFramework.Interfaces.Messaging
{
    public interface IMessage
    {
        MessageType Type { get; }
        byte Code { get; }
        // nullable as we dont always want it to have a specific subcode
        int? SubCode { get;  }
        // Dictionary to make sure we dont have duplicate parammeters
        Dictionary<byte, object> Parameters { get; }
    }
}
