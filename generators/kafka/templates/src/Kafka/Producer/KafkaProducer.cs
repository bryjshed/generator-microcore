using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace <%= namespace %>
{
    /// <summary>
    /// Handles publishing of events using Kafka as messaging infrastructure.
    /// </summary>
    public class KafkaProducer : IKafkaProducer
    {
        private readonly ILogger _logger;

        private readonly KafkaConfig _serviceConfig;

        private readonly Producer<Null, string> _producer;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaProducer"/> class.
        /// </summary>
        /// <param name="logger">The log handler for the controller.</param>
        /// <param name="serviceConfig">The service config settings <see cref="KafkaProducer"/></param>
        public KafkaProducer(
            ILogger<KafkaProducer> logger,
            KafkaConfig serviceConfig)
        {
            _logger = logger;
            _serviceConfig = serviceConfig;
            if(string.IsNullOrWhiteSpace(_serviceConfig.BrokerList))
            {
                throw new NotSupportedException("The broker list cannot be empty.");
            }
            var config = new Dictionary<string, object> { { "bootstrap.servers", _serviceConfig.BrokerList } };
            _producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
            _producer.OnError += (_, e) =>
            {
                if (e.HasError && !e.Reason.Contains("Receive failed: Disconnected"))
                {
                    _logger.LogError("Kafka Producer error {error}", e.Reason);
                }
            };
        }

        /// <summary>
        /// Publishes specific event topic.
        /// </summary>
        /// <param name="kafkaEvent">The event to be published to kafka.</param>
        public void Publish<E>(KafkaEvent<E> kafkaEvent)
        {
            if (string.IsNullOrWhiteSpace(kafkaEvent.TopicKey))
            {
                throw new NullTopicException("No topic was set.");
            }
            if(!_serviceConfig.ProducedTopics.ContainsKey(kafkaEvent.TopicKey))
            {
                throw new NullTopicException("The topic does not exist in the topic dictionary. Update the appsettings file.");
            }
            _logger.LogInformation("Creating a new event {@Event}", kafkaEvent);

            var jsonString = JsonConvert.SerializeObject(kafkaEvent);
            var deliveryReport = _producer.ProduceAsync(_serviceConfig.ProducedTopics[kafkaEvent.TopicKey], null, jsonString);
            _logger.LogInformation("Event has been processed");
        }

        /// <summary>
        /// Publishes a list of events.
        /// </summary>
        /// <param name="kafkaEvents">The list of events to be published to kafka.</param>
        public void Publish<E>(List<KafkaEvent<E>> kafkaEvents)
        {
            _logger.LogInformation("Creating a new event {@Event}", kafkaEvents);
            //Only used for testing and picking a random topic.
            foreach (var e in kafkaEvents)
            {
                Publish(e);
            }
            _logger.LogInformation("Events have been processed.");
        }
    }
}