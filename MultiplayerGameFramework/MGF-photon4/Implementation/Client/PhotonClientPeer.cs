﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Logging;
using MultiplayerGameFramework;
using MultiplayerGameFramework.Implementation.Messaging;
using MultiplayerGameFramework.Interfaces.Client;
using MultiplayerGameFramework.Interfaces.Messaging;
using Photon.SocketServer;
using Photon.SocketServer.Annotations;
using PhotonHostRuntimeInterfaces;

namespace MGF_Photon.Implementation.Client
{
    public class PhotonClientPeer : ClientPeer, IClientPeer
    {
        public bool IsProxy
        {
            get { return false; }
            set {  /* dont set anything */}
        }

        public Guid PeerId { get; set; }

        public IConnectionCollection<IClientPeer> ConnectionCollection { get; set; }

        protected ILogger Log;
        private readonly IServerApplication _server;
        private readonly IHandlerList<IClientPeer> _handlerList;
        private readonly Dictionary<Type, IClientData> _clientData;

        #region Factory Method

        public delegate PhotonClientPeer ClientPeerFactory(InitRequest initRequest);

        #endregion


        // video 10

        public PhotonClientPeer(InitRequest initRequest,
            ILogger log,
            IServerApplication server,
            IEnumerable<IClientData> clientData,
            IConnectionCollection<IClientPeer> connectionCollection,
            IHandlerList<IClientPeer> handlerList)
            : base(initRequest)
        {
            Log = log;
            _server = server;
            _handlerList = handlerList;
            Log.DebugFormat("Created Client Peer");
            ConnectionCollection = connectionCollection;
            connectionCollection.Connect(this);
            PeerId = Guid.NewGuid();
            _clientData = new Dictionary<Type, IClientData>();
            foreach (var data in clientData)
            {
                _clientData.Add(data.GetType(), data);
            }
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Log.DebugFormat("Handling Operation Request");
            _handlerList.HandleMessage(new Request(operationRequest.OperationCode,
                operationRequest.Parameters.ContainsKey(_server.SubCodeParameterCode)
                    ? (int?)Convert.ToInt32(operationRequest.Parameters[_server.SubCodeParameterCode])
                    : null, operationRequest.Parameters), this);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            ConnectionCollection.Disconnect(this);
            Log.DebugFormat("Client Disconnected {0}", PeerId);
        }

        public void SendMessage(IMessage message)
        {
            if (!message.Parameters.Keys.Contains(_server.SubCodeParameterCode))
            {
                message.Parameters.Add(_server.SubCodeParameterCode, message.SubCode);
            }
            if (message is Event)
            {
                SendEvent(new EventData(message.Code) { Parameters = message.Parameters }, new SendParameters());
            }

            var response = message as Response;
            if (response != null)
            {
                SendOperationResponse(new OperationResponse(response.Code, response.Parameters)
                {
                    DebugMessage = response.DebugMessage,
                    ReturnCode = response.ReturnCode
                }, new SendParameters());
            }
        }

        public T ClientData<T>() where T : class, IClientData
        {
            IClientData result;
            _clientData.TryGetValue(typeof(T), out result);
            if (result != null)
            {
                return result as T;
            }

            return null;
        }
    }
}
