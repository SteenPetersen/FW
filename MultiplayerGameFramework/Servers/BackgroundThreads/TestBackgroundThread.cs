using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Logging;
using MultiplayerGameFramework;
using MultiplayerGameFramework.Implementation.Messaging;
using MultiplayerGameFramework.Interfaces.Client;
using MultiplayerGameFramework.Interfaces.Messaging;
using MultiplayerGameFramework.Interfaces.Support;


// video 20

namespace Servers.BackgroundThreads
{
    public class TestBackgroundThread : IBackgroundThread
    {
        private bool isRunning;
        public ILogger Log { get; set; }
        public IConnectionCollection<IClientPeer> ConnectionCollection;

        public TestBackgroundThread(IConnectionCollection<IClientPeer> connectionCollection, ILogger log) // Include all IoC objects this thread needs i.e. Iregion, IStats, etc...
        {
            Log = log;
            ConnectionCollection = connectionCollection;
        }

        public void Setup(IServerApplication server)
        {
            // do nothing in this setup, normally used for setting up one time things in th background thread it starts.
        }

        public void Run(object threadContext)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            isRunning = true;

            while (isRunning)
            {
                try
                {
                    // check to see if there are any players - we need a list of the players. if we have no players sleep for 1 second and try again.
                    if (ConnectionCollection.GetPeers<IClientPeer>().Count <= 0)
                    {
                        Thread.Sleep(1000);
                        timer.Restart();
                        continue;
                    }
                    if (timer.Elapsed < TimeSpan.FromMilliseconds(5000)) // run every 5000ms - 5 seconds
                    {
                        if (5000 - timer.ElapsedMilliseconds > 0)
                        {
                            Thread.Sleep(5000 - (int)timer.ElapsedMilliseconds);
                        }

                        continue;
                    }

                    Update(timer.Elapsed);
                    // Restart the timer so that, just in case it takes longer than 100ms itll start over as soon as the process finishes.
                    timer.Restart();
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(string.Format("Exception occurid in the TestBackgroundThread.Run - {0}",
                        e.StackTrace));
                    throw;
                }
            }
        }

        public void Stop()
        {
            isRunning = false;
        }

        public void Update(TimeSpan elapsed)
        {
            // Do the update here.
            Parallel.ForEach(ConnectionCollection.GetPeers<IClientPeer>(), SendUpdate);
        }

        public void SendUpdate(IClientPeer instance)
        {
            if (instance != null)
            {
                Log.DebugFormat("Sending test message to peer");
                instance.SendMessage(new Event(2, 3, new Dictionary<byte, object>()));
            }
        }
    }
}
