using MultiplayerGameFramework.Interfaces.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameFramework
{
    public interface IServerApplication
    {
        byte SubCodeParameterCode { get; }
        string BinaryPath { get; }
        string ApplicationName { get; }

        void Setup();
        void TearDown();
        void OnServerConnectionFailed(int erroCode, string errorMessage, object state);
        void AfterServerRegistration(IServerPeer serverpeer);
        void ConnectToPeer(PeerInfo peerInfo);
    }
}
