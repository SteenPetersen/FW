using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using log4net.Core;
using MGF_Photon.Implementation.Data;
using MGF_Photon.Implementation.Operation;
using MGF_Photon.Implementation.Operation.Data;
using MGF_Photon.Implementation.Server;
using MultiplayerGameFramework.Implementation.Messaging;
using MultiplayerGameFramework.Interfaces.Config;
using MultiplayerGameFramework.Interfaces.Messaging;
using MultiplayerGameFramework.Interfaces.Server;
using Photon.SocketServer;
using ErrorCode = MGF_Photon.Implementation.Codes.ErrorCode;
using ILogger = ExitGames.Logging.ILogger;

namespace MGF_Photon.Implementation.Handler
{
    public class HandleServerRegistration : ServerHandler
    {
        private readonly IServerType _serverType;
        public ILogger Log { get; set; }

        public HandleServerRegistration(ILogger log, IServerType serverType)
        {
            Log = log;
            _serverType = serverType;
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return 0; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, IServerPeer serverPeer)
        {
            var peer = serverPeer as PhotonServerPeer;
            if (peer != null)
            {
                return OnHandleMessage(message, peer);
            }

            return false;
        }

        protected bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            OperationResponse operationResponse;
            // we are already registered, tell the subserver it tried to reiogster more than once
            if (serverPeer.Registered)
            {
                operationResponse = new OperationResponse(message.Code) { ReturnCode = (short)ErrorCode.InternalServerError, DebugMessage = "Already Registered"};
            }
            else
            {
                var registerRequest = new RegisterSubServer(serverPeer.Protocol, message);

                // Register sub server operation is bad, something is missing, etc....
                if (!registerRequest.IsValid)
                {
                    string msg = registerRequest.GetErrorMessage();
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Invalid Register request {0}", msg);
                    }

                    operationResponse = new OperationResponse(message.code) {DebugMessage = msg, ReturnCode = (short)ErrorCode.OperationInvalid};
                }
                else
                {
                    // valid message, not registered process the registration.
                    XmlSerializer mySerializer = new XmlSerializer(typeof(RegisterSubServerData));
                    StringReader inStream = new StringReader(registerRequest.RegisterSubServerOperation);
                    var registerData = (RegisterSubServerData) mySerializer.Deserialize((inStream));

                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Received register request: Address ={0}, Udpport={1}, Tcpport={2}, Type={3}",
                            registerData.GameServerAddress, registerData.UdpPort, registerData.TcpPort,
                            registerData.ServerType);
                    }

                    var serverData = serverPeer.ServerData<ServerData>();
                    if (serverData == null)
                    {
                        // Autofac Doesnt have a reference to serverData so it doesnt exist in the server Iserverdata list
                        Log.DebugFormat("ServerData is null...");
                    }

                    if (registerData.UdpPort.HasValue)
                    {
                        serverData.UdpAddress = registerData.GameServerAddress + ":" + registerData.UdpPort;
                    }

                    if (registerData.TcpPort.HasValue)
                    {
                        serverData.TcpAddress = registerData.GameServerAddress + ":" + registerData.TcpPort;
                    }

                    // setting server ID
                    serverData.ServerId = registerData.ServerId;
                    // setting server type
                    serverData.ServerType = registerData.ServerType;
                    // looking up the server type for the server peer
                    serverPeer.ServerType = _serverType.GetServerType(registerData.ServerType);
                    // setting application name
                    serverData.ApplicationName = registerData.ServerName;

                    operationResponse = new OperationResponse(message.Code);

                    serverPeer.Registered = true;
                }
            }

            serverPeer.SendOperationResponse(operationResponse, new SendParameters());
            return true;
        }
    }
}
