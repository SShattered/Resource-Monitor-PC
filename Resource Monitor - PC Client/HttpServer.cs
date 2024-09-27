using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Resource_Monitor___PC_Client
{
    public class HttpServer
    {
        private HttpListener server;
        private ResourceRepository _repository;

        public HttpServer(ResourceRepository repository) 
        {
            server = new HttpListener();
            _repository = repository;
            Init();
        }

        void Init()
        {
            server.Prefixes.Add("http://+:8989/");
        }

        public void Start()
        {
            server.Start();
            server.BeginGetContext(new AsyncCallback(Listener), server);
        }

        void Listener(IAsyncResult ar)
        {
            var ctx = server.EndGetContext(ar);
            server.BeginGetContext(new AsyncCallback(Listener), server);

            Random random = new Random();
            var buf = Encoding.ASCII.GetBytes(_repository.GetDashboardJson());
            ctx.Response.ContentType = "text/plain";

            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
            ctx.Response.OutputStream.Close();
        }

    }
}
