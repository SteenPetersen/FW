using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameFramework
{
    public class PeerInfo
    {

        public IPEndPoint MasterEndPoint { get; set; }
        public int ConnectRetryIntervalSeconds { get; set; }

        public bool IsSiblingConnection { get; set; }
        public int MaxTries { get; set; }
        public int NumTries { get; set; }
        public string ApplicationName { get; set; }

        public PeerInfo(string ipAddress, int ipPort, int connectRetryIntervalSeconds, bool isSiblingConnection, int maxTries, string applcationName)
        {
            MasterEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), ipPort);
            ConnectRetryIntervalSeconds = connectRetryIntervalSeconds;
            IsSiblingConnection = isSiblingConnection;
            MaxTries = maxTries;
            NumTries = 0;
            ApplicationName = applcationName;
        }

    }
}
