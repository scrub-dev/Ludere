using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlaying.Server
{
    class Server
    {
        private readonly ServerInstance _serverInstance;
        public Server() {
            _serverInstance = new();
        }
        public void Start() {
            _serverInstance.Start();
        }
        public void Stop() {
            _serverInstance.Stop();
        }
    }
}
