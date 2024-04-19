using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlaying.Server
{
    internal class ResponseBuilder
    {
        public static string DEFAULT_CLIENT_PAGE =
""" 
<!DOCTYPE>
<html>
  <head>
    <title>Ludere</title>
  </head>
  <body>
    <script>
        const ws = new WebSocket('ws:localhost:5001')
        ws.onopen = () => {
            console.log("ws open")
        }

        ws.onmessage = (message) => {
            console.log(message)
        }
    </script>
    <h1>Hello World</h1>
  </body>
</html>
""";
    }
}
