using NowPlaying.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WebSocketSharp.Server;

namespace NowPlaying.Server
{
    class ServerInstance
    {
        private Action<string>? _UpdateStateOutputFunc;

        private HttpListener? Server;
        private Thread? serverThread;
        private volatile bool IsServerRunning;

        private Thread? webSocketThread;
        private WebSocketServer?  WSServer;
        private volatile bool IsWSRunning;

        public void SetStateOutputFunc(Action<string> UpdateStateOutputFunc)
        {
            _UpdateStateOutputFunc = UpdateStateOutputFunc;
        }
        private void SetStateOutput(string s)
        {
            _UpdateStateOutputFunc?.Invoke(s);
        }

        private static HttpListener CreateServerListener(string prefix)
        {
            HttpListener server = new();
            server.Prefixes.Add(prefix);
            return server;
        }
        public void Start()
        {
            if (!IsServerRunning) 
            {
                serverThread = new Thread(new ThreadStart(RunServerConnectionAsync))
                {
                    IsBackground = true
                };
                serverThread.Start();
            }

            if (!IsWSRunning) 
            {
                webSocketThread = new Thread(new ThreadStart(RunWebSocketServer))
                {
                    IsBackground = true
                };
                webSocketThread.Start();
            }
        }
        public void Stop()
        {
            IsServerRunning = false;
            Server?.Stop();
            Server = null;

            IsWSRunning = false;
            WSServer?.Stop();
            WSServer = null;

            SetStateOutput(NowPlaying.Server.Server.OFFLINE);
        }
        private void RunServerConnectionAsync()
        {
            if (IsServerRunning) return;
            string protocol = "http://";
            string host = "localhost";
            int port = Settings.Default.SourcePort;
            Server = CreateServerListener($"{protocol}{host}:{port}/");
            Server.Start();
            IsServerRunning = true;
            Server.BeginGetContext(Context, null);
        }
        private void Context(IAsyncResult ar)
        {
            if (Server == null) return;
            HttpListenerContext ctx;

            try{ 
                ctx = Server.EndGetContext(ar);
            }
            catch (Exception) { return; }

            Server.BeginGetContext(Context, null);
            byte[] data = [];
            ctx.Response.ContentType = "text/html";
            ctx.Response.ContentEncoding = Encoding.UTF8;

            if (ctx.Request.Url!.AbsolutePath == "/callback")
            {
                CredentialManager.HandleAuthorizationCallback([.. ctx.Request.Url!.Query.ToString().Split("&")]);
                data = Encoding.UTF8.GetBytes(ResponseBuilder.CALLBACK_PAGE);
                ctx.Response.ContentLength64 = data.LongLength;
                ctx.Response.OutputStream.WriteAsync(data, CancellationToken.None);
            }
            else
            {
                data = Encoding.UTF8.GetBytes(ResponseBuilder.GenerateNowPlayingPage());
                ctx.Response.ContentLength64 = data.LongLength;
                ctx.Response.OutputStream.WriteAsync(data, CancellationToken.None);
            }

            ctx.Response.OutputStream.Close();
        }

        #region Web Socket Stuff
        private void RunWebSocketServer()
        {
            if (IsWSRunning) return;
            WSServer = CreateWebSocketServer();
            WSServer.Start();
            IsWSRunning = true;
            while (IsWSRunning)
            {
                JSONObjects.SpotifyNowPlaying? apiResult = new();
                int? sessionCount = WSServer?.WebSocketServices.SessionCount;
                if (sessionCount != null && sessionCount > 0) SetStateOutput(NowPlaying.Server.Server.CLIENTS(sessionCount));
                else SetStateOutput(NowPlaying.Server.Server.RUNNING);

                if(Settings.Default.SpotifyAccessToken is not null) apiResult = CredentialManager.GetSpotifyNowPlaying();
                else SetStateOutput(NowPlaying.Server.Server.CUSTOM("Running: Please Authorize"));

                WSServer?.WebSocketServices.Broadcast($"<h1>{DateTime.Now}</h1>");
                Thread.Sleep(Settings.Default.ServerRefreshInterval * 1000);
            }
            WSServer?.Stop();
        }
        private WebSocketServer CreateWebSocketServer()
        {
            WebSocketServer wssv = new(Settings.Default.SocketPort);
            wssv.AddWebSocketService("/", () => new WSBehavior());
            return wssv;
        }
        #endregion
    }
}
