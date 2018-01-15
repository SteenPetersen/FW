﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiplayerGameFramework.Interfaces.Messaging;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace MGF_Photon.Implementation.Operation
{
    public class RegisterSubServer : Photon.SocketServer.Rpc.Operation
    {
        public RegisterSubServer(IRpcProtocol protocol, IMessage message)
            : base(protocol, new OperationRequest(message.Code, message.Parameters))
        {

        }

        public RegisterSubServer(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {

        }

        public RegisterSubServer() { }

        [DataMember(Code = 1, IsOptional = false)]

        public string RegisterSubServerOperation { get; set; }
    }
}
