using System.Collections.Generic;
using System.Threading.Tasks;

namespace <%= namespace %>
{
    /// <summary>
    /// The interface for the kafka consumer manager.
    /// </summary>
    public interface IKafkaConsumerManager
    {
        /// <summary>
        /// Starts a single kafka consumer on a new thread.
        /// </summary>
        void StartConsumer();

        /// <summary>
        /// Request canceling of the kafka consumer.
        /// </summary>
        void StopConsumer();

        /// <summary>
        /// Gets the status of the kafka consumer task.
        /// </summary>
        string GetConsumerTaskStatus();
    }
}