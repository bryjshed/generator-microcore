using System.Threading;
using System.Threading.Tasks;

namespace <%= namespace %>
{
    /// <summary>
    /// The kafka consumer interface.
    /// </summary>
    public interface IKafkaConsumer
    {
        /// <summary>
        /// Iniilizes the consumer.
        /// </summary>
        /// <param name="cancelToken">The cancellation token used to stop the kafka consumer.</param>
        Task Run(CancellationToken cancelToken = default(CancellationToken));
    }
}