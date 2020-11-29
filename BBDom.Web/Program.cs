using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Topshelf;

namespace BBDom.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // NLog: setup the logger first to catch all errors
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            var pathToContentRoot = Directory.GetCurrentDirectory();
            var webHostArgs = args.Where(arg => arg != "--console").ToArray();

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                logger.Info("pathToExe: {0}", pathToExe);
                pathToContentRoot = Path.GetDirectoryName(pathToExe);
            }
            logger.Info("pathToContentRoot: {0}", pathToContentRoot);

            try
            
            {
                logger.Info("init main");
                
                HostFactory.Run(host =>
                {
                    host.Service<WebServer>(service =>
                    {
                        service.ConstructUsing(s => new WebServer());
                        service.WhenStarted(s => { s.Start(pathToContentRoot, webHostArgs); });
                        service.WhenStopped(s => { s.Stop(); });
                    });

                    host.SetDescription("BBDom.Web Service");
                    host.SetDisplayName("BBDom.Web");
                    host.SetServiceName("BBDom.Web");
                });
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }                    
    }

    public class WebServer
    {
        private IHost _webHost;

        public void Start(string pathToContentRoot, string[] args)
        {
            _webHost = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseContentRoot(pathToContentRoot).UseNLog().Build();

            _webHost.StartAsync().GetAwaiter().GetResult();
        }

        public void Stop()
        {
            _webHost.StopAsync().GetAwaiter().GetResult();
            _webHost?.Dispose();
        }
    }

}
