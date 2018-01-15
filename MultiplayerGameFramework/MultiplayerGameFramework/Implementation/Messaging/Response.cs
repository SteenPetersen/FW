using MultiplayerGameFramework.Interfaces.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameFramework.Implementation.Messaging
{
    public class Response : IMessage
    {
        private readonly byte _code;
        private readonly Dictionary<byte, object> _parameters;
        private readonly int? _subcode;

        // Photon Specific
        private readonly string _debugmessage; // message that is returned
        private readonly short _returnCode; // code to tell the client what happened

        public Response(byte code, int? subcode, Dictionary<byte, object> parameters)
        {
            _code = code;
            _subcode = subcode;
            _parameters = parameters;
        }

        public Response(byte code, int? subCode, Dictionary<byte, object> parameters, string debugMessage, short returnCode)
            : this(code, subCode, parameters)
        {
            _debugmessage = debugMessage;
            _returnCode = returnCode;
        }

        public byte Code
        {
            get
            {
                return _code;
            }
        }

        public Dictionary<byte, object> Parameters
        {
            get
            {
                return _parameters;
            }
        }

        public int? SubCode
        {
            get
            {
                return _subcode;
            }
        }

        public MessageType Type
        {
            get
            {
                return MessageType.Response;
            }
        }

        public string DebugMessage
        {
            get
            {
                return _debugmessage;
            }
        }


        public short ReturnCode
        {
            get
            {
                return _returnCode;
            }
        }


    }
}
