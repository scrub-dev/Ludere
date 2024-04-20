using NowPlaying.Properties;
using System;
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

        private DateTime CurrentTokenValidUntil = new (DateTime.Now.AddMinutes(-1).Ticks);
        private string? AccessToken;
        

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

        private bool HandleAccessTokenRefresh()
        {
            if (DateTime.Now > CurrentTokenValidUntil || AccessToken == null)
            {
                string clientID = Settings.Default.SpotifyClientID;
                string clientSecret = Settings.Default.SpotifyClientSecret;
                (string? accessToken, DateTime? expiryTime, string? errorString) = CredentialManager.GetAccessToken(clientID, clientSecret);

                if(accessToken == null)
                {
                    SetStateOutput(NowPlaying.Server.Server.ERROR(errorString!));
                    return false;
                }
                else
                {
                    CurrentTokenValidUntil = (DateTime)expiryTime!;
                    AccessToken = accessToken;
                }
            }
            return true;
        }

        public void Start()
        {
            if (!HandleAccessTokenRefresh()) return;

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
            int port = 5000;
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

            byte[] data = Encoding.UTF8.GetBytes(ResponseBuilder.DEFAULT_CLIENT_PAGE);
            ctx.Response.ContentType = "text/html";
            ctx.Response.ContentEncoding = Encoding.UTF8;
            ctx.Response.ContentLength64 = data.LongLength;

            ctx.Response.OutputStream.WriteAsync(data, CancellationToken.None); 
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
                if(!HandleAccessTokenRefresh()) break;

                int? sessionCount = WSServer?.WebSocketServices.SessionCount;
                if (sessionCount != null && sessionCount > 0) SetStateOutput(NowPlaying.Server.Server.CLIENTS(sessionCount));
                else SetStateOutput(NowPlaying.Server.Server.RUNNING);
                
                WSServer?.WebSocketServices.Broadcast("HelloWorld");
                Thread.Sleep(Settings.Default.ServerRefreshInterval * 1000);
            }
            WSServer?.Stop();
        }
        private WebSocketServer CreateWebSocketServer()
        {
            WebSocketServer wssv = new(5001);
            wssv.AddWebSocketService("/", () => new WSBehavior());
            return wssv;
        }
        #endregion
    }
}
