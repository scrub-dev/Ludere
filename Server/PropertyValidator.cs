using Microsoft.Graph.Security.Labels.RetentionLabels.Item.RetentionEventType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlaying.Server
{
    internal class PropertyValidator
    {
        const int UPPER_PORT_RANGE = 65535;
        const int LOWER_PORT_RANGE = 1000;

        const int LOWER_REFRESH_INTERVAL_RANGE = 1;
        const int UPPER_REFRESH_INTERVAL_RANGE = 600;

        public static bool ArePortsSame(int port1, int port2) => port1 == port2;
        public static bool IsPortInValidRange(int port) => port >= LOWER_PORT_RANGE && port <= UPPER_PORT_RANGE;
        public static bool IsRefreshIntervalValid(int interval) => interval >= LOWER_REFRESH_INTERVAL_RANGE && interval <= UPPER_REFRESH_INTERVAL_RANGE;
        
    }


}
