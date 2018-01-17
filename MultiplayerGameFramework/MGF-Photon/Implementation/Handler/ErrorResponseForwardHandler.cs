﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Logging;
using MultiplayerGameFramework.Implementation.Messaging;
using MultiplayerGameFramework.Interfaces.Messaging;
using MultiplayerGameFramework.Interfaces.Server;

namespace MGF_Photon.Implementation.Handler
{
    public class ErrorResponseForwardHandler : ServerHandler, IDefaultResponseHandler<IServerPeer>
    {

        public ILogger Log { get; set; }

        public ErrorResponseForwardHandler(ILogger log)
        {
            Log = log;
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
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
            Log.ErrorFormat("No existing response Handler. {0}-{1}", message.Code, message.SubCode);
            return true;
        }
    }
}

