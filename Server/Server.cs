using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlaying.Server
{
    class Server()
    {
        public static string OFFLINE = "Offline";
        public static string RUNNING = "Running";
        public static string STARTING = "Starting";
        public static string WAITING = "Waiting for Connection...";
        public static string CONNECTED = "Connected";


        public static string ERROR(string s) { return string.Format("ERROR: {0}", s); }
        public static string CLIENTS(int? i) { return (i == 1) ? "" : string.Format("Sources Connected: {0}", i); }

        protected Action<string>? UpdateServerStateFunc;
        private readonly ServerInstance _serverInstance = new();

        public void SetStateOutput(Action<string> UpdateServerState)
        {
            UpdateServerStateFunc = UpdateServerState;
            _serverInstance.SetStateOutputFunc(UpdateServerState);
        }
        public void Start() {
            _serverInstance.Start();
        }
        public void Stop() {
            _serverInstance.Stop();
        }
    }
}
