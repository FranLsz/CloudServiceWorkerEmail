using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private int ms = 86400000;
        private string dest;
        private string url;

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");

            
                //this.RunAsync(this.cancellationTokenSource.Token).Wait();


                while (true)
                {
                    Thread.Sleep(ms);
                    var c = new Correo()
                    {
                        Asunto = "Mensaje	worker",
                        Destino = dest,
                        Origen = "ftajamar3@tajamar365.com",
                        Contenido = "HOLA"
                        /*Password = "Taj@mar1970",
                        Puerto = "587",
                        Smtp = "smtp.office365.com",
                        Texto = "Notificacion	desde	el	worker",
                        Usuario = "ftajamar3@tajamar365.com",*/
                    };

                    Enviar(c);
                }
            try
            {
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            int.TryParse(ConfigurationManager.AppSettings["tiempo"], out ms);
            url = ConfigurationManager.AppSettings["url"];
            dest = ConfigurationManager.AppSettings["destino"];

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }

        }

        protected async Task Enviar(Correo c)
        {
            string postBody = JsonConvert.SerializeObject(c);
            HttpClient cl = new HttpClient();

            cl.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage wcfResponse =
                await cl.PostAsync(url, new StringContent(postBody, Encoding.UTF8, "application/json"));
        }
    }
}
