using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Verse;

namespace OpenClaw
{
    public static class OpenClawHttpServer
    {
        private static HttpListener _listener;
        private static Thread _thread;
        private static bool _running;

        public static void Start(int port = 3456)
        {
            if (_running) return;
            _running = true;
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
            _listener.Start();
            _thread = new Thread(ListenLoop) { IsBackground = true };
            _thread.Start();
            Log.Message($"[OpenClaw] HTTP server started on 127.0.0.1:{port}");
        }

        private static void ListenLoop()
        {
            while (_running)
            {
                try
                {
                    var context = _listener.GetContext();
                    HandleRequest(context);
                }
                catch (Exception ex)
                {
                    Log.Error($"[OpenClaw] HTTP error: {ex}");
                }
            }
        }

        private static void HandleRequest(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath.ToLower();
            if (path == "/state")
            {
                var payload = OpenClawStateBuilder.Build();
                RespondJson(context, payload);
                return;
            }
            if (path == "/schema")
            {
                RespondJson(context, OpenClawSchema.Schema);
                return;
            }
            if (path == "/actions" && context.Request.HttpMethod == "POST")
            {
                string body;
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    body = reader.ReadToEnd();
                }
                var response = ActionExecutor.ApplyActions(body);
                RespondJson(context, response);
                return;
            }

            context.Response.StatusCode = 404;
            context.Response.Close();
        }

        private static void RespondJson(HttpListenerContext context, string json)
        {
            var buffer = Encoding.UTF8.GetBytes(json);
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
    }
}
