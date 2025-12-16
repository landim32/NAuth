using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NAuth.API
{
    public class Program
    {
        private const string PFX_CERTIFICATE = "NAuth.API.emagine.pfx";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
#if !DEBUG
                    webBuilder.UseKestrel(options =>
                    {
                        options.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(PFX_CERTIFICATE);
                            if (s == null) {
                                throw new Exception($"Cant find {PFX_CERTIFICATE}.");
                            }
                            using (MemoryStream ms = new MemoryStream())
                            {
                                s.CopyTo(ms);
                                httpsOptions.ServerCertificate = new X509Certificate2(ms.ToArray(), "pikpro6");
                            }
                        });
                    });
#endif
                    webBuilder.UseStartup<Startup>();
                });
    }
}
