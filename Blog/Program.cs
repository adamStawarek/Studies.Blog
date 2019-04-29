using System;
using Blog.Helpers;
using Blog.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Blog
{
    public class Program
    {

        public static void Main(string[] args)
        {
            //TODO move connection string to appsettings.json
            var logDB = @"Server=DESKTOP-HC7DAS3\ADAMSQL;Database=Blog;Trusted_Connection=True;";
            var logTable = "Logs";
            var sink = new MSSqlServerSink(logDB, logTable, 1, TimeSpan.FromSeconds(1), null, true);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole()
                .WriteTo.Sink(sink)
                .CreateLogger();

            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<BlogContext>();
                    BlogInitializer.Initialize(context);
                    Log.Information("Starting web host");
                }
                catch (Exception ex)
                {
                    Log.Fatal("Host terminated unexpectedly");
                    return;
                }
                finally
                {
                    Log.CloseAndFlush();
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}