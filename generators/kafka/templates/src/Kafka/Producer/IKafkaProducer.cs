using System.Collections.Generic;

namespace <%= namespace %>
{
    /// <summary>
    /// Interface for handling publishing of events using Kafka as messaging infrastructure.
    /// </summary>
    public interface IKafkaProducer
    {
        /// <summary>
        /// Publishes specific event type.
        /// </summary>
        /// <param name="kafkaEvent">The event to be published to kafka.</param>
        void Publish<E>(KafkaEvent<E> kafkaEvent);

        /// <summary>
        /// Publishes a list of events.
        /// </summary>
        /// <param name="kafkaEvents">The list of events to be published to kafka.</param>
        void Publish<E>(List<KafkaEvent<E>> kafkaEvents);
    }
}