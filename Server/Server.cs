using NowPlaying.Properties;
using NowPlaying.Utility;
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
        public static string CLIENTS(int? i) { return (i == 1) ? "Source Connected" : string.Format("Sources Connected: {0}", i); }

        protected Action<string>? UpdateServerStateFunc;
        private readonly ServerInstance _serverInstance = new();

        public void SetStateOutput(Action<string> UpdateServerState)
        {
            UpdateServerStateFunc = UpdateServerState;
            _serverInstance.SetStateOutputFunc(UpdateServerState);
        }
        public void Start() {
            if (ValidateSettings()) _serverInstance.Start();
            else return;
        }
        public void Stop() {
            _serverInstance.Stop();
        }

        private bool ValidateSettings()
        {
            if (!PropertyValidator.IsPropertySet(Settings.Default.SpotifyClientID)) {
                Util.TimedSetErrorUIWithSetAfter("Missing Client ID", 3000, UpdateServerStateFunc!, "Offline");
                return false;
            }
            if (!PropertyValidator.IsPropertySet(Settings.Default.SpotifyClientSecret))
            {
                Util.TimedSetErrorUIWithSetAfter("Missing Client Secret", 3000, UpdateServerStateFunc!, "Offline");
                return false;
            }
            if (Settings.Default.SourcePort < PropertyValidator.LOWER_PORT_RANGE || Settings.Default.SourcePort > PropertyValidator.UPPER_PORT_RANGE)
            {
                Util.TimedSetErrorUIWithSetAfter("Invalid Host Port", 3000, UpdateServerStateFunc!, "Offline");
                return false;
            }
            if (Settings.Default.SocketPort < PropertyValidator.LOWER_PORT_RANGE || Settings.Default.SocketPort > PropertyValidator.UPPER_PORT_RANGE)
            {
                Util.TimedSetErrorUIWithSetAfter("Invalid Socket Port", 3000, UpdateServerStateFunc!, "Offline");
                return false;
            }
            if(Settings.Default.SocketPort == Settings.Default.SourcePort)
            {
                Util.TimedSetErrorUIWithSetAfter("Invalid Ports (Identical)", 3000, UpdateServerStateFunc!, "Offline");
                return false;
            }

            return true;
        }
    }
}
