using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace <%= namespace %>
{
    /// <summary>
    /// Extension methods used for Kafka middleware setup.
    /// </summary>
    public static class KafkaServiceExtension
    {
        /// <summary>
        /// Extension method used for setting up kafka in the middleware.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="Configuration">The configuration for settings.</param>
        public static IServiceCollection AddKafkaServices<E>(this IServiceCollection services, IConfiguration Configuration) where E : KafkaConsumer
        {
            var kafkaConfig = new KafkaConfig();
            Configuration.GetSection("Kafka").Bind(kafkaConfig);
            services.AddSingleton<KafkaConfig>(kafkaConfig);

            services.AddSingleton<IKafkaConsumerManager, KafkaConsumerManager>();
            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            services.AddTransient<IKafkaConsumer, E>();
            return services;
        }

        /// <summary>
        /// Starts the kafka consumer.
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure an application's request pipeline</param>
        public static IApplicationBuilder StartKafkaConsumer(this IApplicationBuilder app)
        {
            var kafkaManager = app.ApplicationServices.GetService<IKafkaConsumerManager>();
            kafkaManager.StartConsumer();
            return app;
        }
    }
}