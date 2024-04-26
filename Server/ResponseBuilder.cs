using NowPlaying.Properties;
using NowPlaying.Server.JSONObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlaying.Server
{
    internal class ResponseBuilder
    {
        private static string WebSocketScript()
        {
            string webSocketString = $"let ws = new WebSocket('ws://localhost:{Settings.Default.SocketPort}');\r\n";
            string onErrorString   = "\r\n";
            string onCloseString   = "ws.onclose = (e) => {setTimeout(connect, 5000)}\r\n";
            string onMessageString = "ws.onmessage = (e) => {document.getElementById(\"Content\").innerHTML= e.data; console.log(e)} \r\n \r\n";
            string onOpenString    = "ws.onopen = () => console.log('Connected')\r\n";
            string output = "const connect = () => {"
                + webSocketString
                + onErrorString 
                + onCloseString
                + onMessageString
                + onOpenString 
                + "}\r\nconnect()";
            return output;
        }
        public static string GenerateNowPlayingPage()
        {
            return ""
                + "<!DOCTYPE>\r\n<html>"
                + "<head>\r\n<title>Ludere</title>\r\n</head>"
                + "<body>"
                + $"<script>{WebSocketScript()}</script>"
                + "<div id=\"Content\"></div>"
                + "</body>"
                + "</html>";

        }
    
    public static string CALLBACK_PAGE =
""" 
<!DOCTYPE>
<html>
  <head>
    <title>Ludere</title>
  </head>
  <body>
    <script>
        window.onload = (event) => {
            window.close();
        };
    </script>
    <h1>Callback</h1>
  </body>
</html>
""";

        public static string WebSocketResponse(SpotifyNowPlaying spnpObj)
        {
            return WrapDiv("");
        }


        private static string WrapDiv(string toBeWrapped, string style = "")
        {
            return $"<div style=\"{style}\">" + toBeWrapped + "</div>";
        }


    }
}
