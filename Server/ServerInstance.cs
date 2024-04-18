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

namespace NowPlaying.Server
{
    class ServerInstance
    {

        public static string pageDataBase =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>Ludere</title>" +
            "  </head>" +
            "  <body>" +
            "    <h1>Hello World</h1>" +
            "  </body>" +
            "</html>";
        private HttpListener? Server;
        private Thread? serverThread;
        private volatile bool IsRunning;

        private static HttpListener CreateServerListener(string prefix)
        {
            HttpListener server = new();
            server.Prefixes.Add(prefix);
            return server;
        }

        public void Start()
        {
            if (IsRunning) return;
            serverThread = new Thread(new ThreadStart(RunServerConnectionAsync))
            {
                IsBackground = true
            };
            IsRunning = true;
            serverThread.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            Server?.Stop();
        }

        private async void RunServerConnectionAsync()
        {
            string protocol = "http://";
            string host = "localhost";
            int port = 5000;
            Server = CreateServerListener($"{protocol}{host}:{port}/");
            Server.Start();
            await HandleConnection();

        }

        private async Task HandleConnection()
        {
            while (IsRunning) 
            {
                Byte[] data;
                HttpListenerContext ctx;
                try
                {
                    if (Server == null) return;
                    ctx = await Server.GetContextAsync();
                }
                catch (Exception)
                {
                    return;
                }

                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                if (req.Url!.AbsolutePath == "/favicon.ico") return;
                
                if (req.Url!.AbsolutePath == "/nowplaying")
                {

                }
                else
                {
                    data = Encoding.UTF8.GetBytes(pageDataBase);
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    await resp.OutputStream.WriteAsync(data, CancellationToken.None);
                }

                resp.Close();
            }
        }
    }
}


