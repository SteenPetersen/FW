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
    // do something whenever we are trying to forward an even with no handler existing for it.
    public class ErrorEventForwardHandler : ServerHandler, IDefaultEventHandler<IServerPeer>
    {
        public  ILogger Log { get; set; }

        public ErrorEventForwardHandler(ILogger log)
        {
            Log = log;
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
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
            Log.ErrorFormat("No existing Event Handler. {0}-{1}", message.Code, message.SubCode);
            return true;
        }
    }
}
