using MultiplayerGameFramework.Interfaces.Config;
using MultiplayerGameFramework.Interfaces.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameFramework.Implementation.Server
{
    public class ServerConnectionCollection : IServerConnectionCollection<IServerType, IServerPeer>
    {
        private List<IServerPeer> _servers;

        public ServerConnectionCollection()
        {
            _servers = new List<IServerPeer>();
        }

        public void Connect(IServerPeer peer)
        {
            _servers.Add(peer);
        }

        public void Disconnect(IServerPeer peer)
        {
            _servers.Remove(peer);
            peer.Disconnect();
        }

        public void Clear()
        {
            _servers.Clear();
        }

        public List<T> GetPeers<T>() where T : class, IServerPeer
        {
            return new List<T>(_servers.Cast<T>());
        }

        public Dictionary<IServerType, List<IServerPeer>> GetServers()
        {
            var retValue = new Dictionary<IServerType, List<IServerPeer>>();

            // Sort Servers by type and add it to a list for that type
            foreach (IServerPeer server in _servers)
            {
                if (server.ServerType != null)
                {
                    if (!retValue.ContainsKey(server.ServerType))
                    {
                        retValue.Add(server.ServerType, new List<IServerPeer>());
                    }
                    retValue[server.ServerType].Add(server);
                }
            }
            return retValue;
        }

        public List<T> GetServerByType<T>(IServerType type) where T : class, IServerPeer
        {
            var retValue = new List<T>();

            // sort servers by type and add it to the list if the type matches
            foreach (IServerPeer server in _servers)
            {
                if (server.ServerType == type && server is T)
                {
                    retValue.Add(server as T);
                }
            }
            return retValue;

        }
    }
}
