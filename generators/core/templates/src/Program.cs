using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
<%_ if(addDatabase && database === 'dynamodb') { _%>
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
<%_ } _%>
<%_ if(addDatabase && database !== 'dynamodb') { _%>
using Microsoft.EntityFrameworkCore;
<%_ } _%>
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace <%= namespace %>
{
    /// <summary>
    /// The main class for the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Represents a set of key/value application configuration properties.
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();

        /// <summary>
        /// The main method of the program.
        /// </summary>
        /// <param name="args">The command-line arguments to the program.</param>
        public static int Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var level = new LoggingLevelSwitch();
            if (Enum.TryParse(Configuration["Logging:LogLevel:Default"], out LogEventLevel logLevel))
            {
                level.MinimumLevel = logLevel;
            }
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(level)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", env)
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {Environment} {SourceContext} {Level} {Message}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Starting service...");

                var host = BuildWebHost(args);

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    try
                    {
                        // Run startup code here
                        <%_ if(addDatabase) { _%>
                        <%_ if(database === 'dynamodb') { _%>
                        <%_ if(createModel) { _%>
                        var db = services.GetRequiredService<ProvisionDynamodb>();
                        db.CreateTable(
                            $"{$"{env.ToLower()}-<%= appname.toLowerCase() %>-"}<%= modelName.toLowerCase() %>",
                            new ProvisionedThroughput(5,5),
                            new ProvisionedThroughput(5,5));
                        <%_ } _%>
                        <%_ } else { _%>
                        services.GetService<<%= appname %>Context>().Database.Migrate();
                        <%_ } _%>
                        <%_ } _%>
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred seeding the DB.");
                    }
                }

                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Creates and instances of IWebHost
        /// </summary>
        public static IWebHost BuildWebHost(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://*:<%= portNumber %>;";

            return WebHost.CreateDefaultBuilder(args)
                .UseUrls(urls)
                .UseEnvironment(env)
                .UseConfiguration(Configuration)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
        }
    }
}
