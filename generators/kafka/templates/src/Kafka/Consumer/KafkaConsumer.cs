using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace <%= namespace %>
{

    /// <summary>
    /// The kafka consumer.
    /// </summary>
    public abstract class KafkaConsumer : IKafkaConsumer
    {
        /// <summary>
        /// Consumer logger.
        /// </summary>
        protected readonly ILogger _logger;

        private readonly KafkaConfig _serviceConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaConsumer"/> class.
        /// </summary>
        /// <param name="logger">The log handler for the controller.</param>
        /// <param name="serviceConfig">The service config settings <see cref="KafkaConfig"/></param>
        public KafkaConsumer(
            ILogger<KafkaConsumer> logger,
            KafkaConfig serviceConfig)
        {
            _logger = logger;
            _serviceConfig = serviceConfig;
        }

        /// <summary>
        /// Iniilizes the consumer.
        /// </summary>
        public virtual async Task Run(CancellationToken cancelToken = default(CancellationToken))
        {
            _logger.LogInformation($"Starting kafka consumer.");
            // Create the consumer
            using (var consumer = new Consumer<Null, string>(CreateConfiguration(_serviceConfig), null, new StringDeserializer(Encoding.UTF8)))
            {
                consumer.OnError += (_, e) =>
                {
                    if (e.HasError && !e.Reason.Contains("Receive failed: Disconnected"))
                    {
                        _logger.LogError("Kafka consuming error {error}", e.Reason);
                    }
                };
                    
                if (_serviceConfig.EnableAutoCommit)
                {
                    // Subscribe to the OnMessage event
                    consumer.OnMessage += async (obj, msg) =>
                    {
                        try
                        {
                            //Process event.
                            await ProcessEvent(msg);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("There was an error processing kafka message. {Error}", ex);
                        }
                    };

                    // Subscribe to the Kafka topic
                    consumer.Subscribe(_serviceConfig.ConsumedTopics);

                    // Poll for messages
                    while (!cancelToken.IsCancellationRequested)
                    {
                        // Blocks indefinitely until a new event is ready.
                        consumer.Poll(TimeSpan.FromMilliseconds(100));
                    }
                }
                else
                {
                    // Subscribe to the Kafka topic
                    consumer.Subscribe(_serviceConfig.ConsumedTopics);

                    while (!cancelToken.IsCancellationRequested)
                    {
                        Message<Null, string> msg;
                        if (!consumer.Consume(out msg, TimeSpan.FromMilliseconds(100)))
                        {
                            continue;
                        }
                        try
                        {
                            await ProcessEvent(msg);
                            var committedOffsets = consumer.CommitAsync(msg).Result;
                            _logger.LogInformation($"Committed offset: {committedOffsets}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("There was an error processing kafka message. {Error}", ex);
                        }

                    }
                }
            }
            _logger.LogInformation($"The kafka consumer has been stopped.");
        }

        /// <summary>
        /// The kafka consumer event proccessing method.
        /// </summary>
        /// <param name="message">The event message to processes.</param>
        protected abstract Task ProcessEvent(Message<Null, string> message);

        /// <summary>
        /// Used for creating the configuration dictionary used by the kafka consumer.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>The configuration setup on startup.</returns>
        protected virtual Dictionary<string, object> CreateConfiguration(KafkaConfig configuration)
        {
            return new Dictionary<string, object>
            {
                { "group.id", configuration.GroupId },
                { "enable.auto.commit", configuration.EnableAutoCommit },
                { "auto.commit.interval.ms", configuration.AutoCommitIntervalMs },
                { "statistics.interval.ms", configuration.StatisticsIntervalMs },
                { "bootstrap.servers", configuration.BrokerList },
                { "default.topic.config", configuration.TopicConfigs }
            };
        }
    }
}