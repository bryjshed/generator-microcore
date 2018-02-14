using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace <%= namespace %>
{
    /// <summary>
    /// The consumer used for the <%= appname.toLowerCase() %> service.
    /// </summary>
    public class <%= appname %>KafkaConsumer : KafkaConsumer
    {
        private readonly IServiceScopeFactory _serviceFactory;

        // Track worker threads for fast processing
        private readonly SemaphoreSlim _maxThread;

        /// <summary>
        /// The <%= appname.toLowerCase() %> kafka consumer constructor.
        /// </summary>
        /// <param name="logger">The log handler for the service.</param>
        /// <param name="serviceConfig">Kafka consumer configuration.</param>
        /// <param name="serviceFactory">The service collection factory.</param>
        public <%= appname %>KafkaConsumer(
            ILogger<<%= appname %>KafkaConsumer> logger,
            KafkaConfig serviceConfig,
            IServiceScopeFactory serviceFactory) : base(logger, serviceConfig)
        {
            _serviceFactory = serviceFactory;
            // Max threads allowed.
            _maxThread = new SemaphoreSlim(serviceConfig.ConsumerThreadCount);
        }

        /// <summary>
        /// Process the consumer event for <%= appname %> service.
        /// </summary>
        /// <param name="message">The event message to processes.</param>
        protected override async Task ProcessEvent(Message<Null, string> message)
        {
            _logger.LogInformation("<%= appname %> Consumer processing offset {offset} and topic {topic}.", message.Offset, message.Topic);
            using (var scope = _serviceFactory.CreateScope())
            {
                try
                {
                    // Need to fetch from the service collection since we don't want to overload the db context
                    var service = scope.ServiceProvider.GetService<I<%= appname %>Service>();

                    switch (message.Topic)
                    {
                        <% for(var i=0; i<kafkaConsumerTopics.length; i++) { %>
                        case "<%= kafkaConsumerTopics[i] %>":
                        // Add code to processes Event here.

                        break;
                        <% } %>   
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("There was a fetal {error}", ex);
                }
            }
        }

        private void ProcessWorker(Action action)
        {
            _maxThread.Wait();
            Task.Factory.StartNew(action
                , TaskCreationOptions.LongRunning)
            .ContinueWith((task) => _maxThread.Release());
        }
    }
}