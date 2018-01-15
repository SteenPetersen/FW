using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiplayerGameFramework.Interfaces.Server;

namespace MultiplayerGameFramework.Interfaces.Support
{
    public interface IAfterServerRegistration
    {
        void AfterRegister(IServerPeer serverPeer);
    }
}
