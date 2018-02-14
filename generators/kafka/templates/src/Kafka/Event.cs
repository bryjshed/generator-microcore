using System;

namespace <%= namespace %>
{
    /// <summary>
    /// The event to be 
    /// </summary>
    public class KafkaEvent<E>
    {
        /// <summary>
        /// Event Id.
        /// </summary>
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// The user Id for the event.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Event timestamp.
        /// </summary>
        public DateTime Date { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicKey { get; set; }

        /// <summary>
        /// The entity.
        /// </summary>
        public E Entity { get; set; }

        /// <summary>
        /// Constructor for an event.
        /// </summary>
        public KafkaEvent(string topicKey, string userId, E entity)
        {
            TopicKey = topicKey;
            UserId = userId;
            Entity = entity;
        }
    }
}