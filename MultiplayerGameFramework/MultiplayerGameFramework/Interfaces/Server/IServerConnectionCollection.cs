using MultiplayerGameFramework.Interfaces.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameFramework.Interfaces.Server
{
    public interface IServerConnectionCollection<TServerType, TPeer> : IConnectionCollection<TPeer>
    {
        // get all servers that are associated with this connectioncollection
        Dictionary<TServerType, List<TPeer>> GetServers();
        List<T> GetServerByType<T>(TServerType type) where T : class, TPeer;
    }

}
