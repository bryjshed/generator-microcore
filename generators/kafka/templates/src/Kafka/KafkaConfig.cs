using System.Collections.Generic;

namespace <%= namespace %>
{
    /// <summary>
    /// Represents the configuration for Kafka.
    /// </summary>
    public class KafkaConfig
    {
        /// <summary>
        /// Gets or sets the EndPoint for the service.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the EnableAutoCommit for the kafka.
        /// </summary>
        public bool EnableAutoCommit { get; set; }

        /// <summary>
        /// Gets or sets the BrokerList for the kafka.
        /// </summary>
        public string BrokerList { get; set; }

        /// <summary>
        /// Gets or sets the AutoCommitIntervalMs for the kafka.
        /// </summary>
        public int AutoCommitIntervalMs { get; set; }

        /// <summary>
        /// Gets or sets the ConsumerThreadCount for the kafka.
        /// </summary>
        public int ConsumerThreadCount { get; set; }

        /// <summary>
        /// Gets or sets the StatisticsIntervalMs for the kafka.
        /// </summary>
        public int StatisticsIntervalMs { get; set; }

        /// <summary>
        /// Gets or sets Topics for kafka.
        /// </summary>
        public IDictionary<string, object> TopicConfigs { get; set; }

        /// <summary>
        /// List of topics to subscribe to.
        /// </summary>
        public IList<string> ConsumedTopics { get; set; }

        /// <summary>
        /// Gets or sets ProduceTopics for kafka.
        /// </summary>
        public IDictionary<string, string> ProducedTopics { get; set; }
    }
}