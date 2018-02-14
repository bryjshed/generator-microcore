using System;
using System.Threading;
using System.Threading.Tasks;

namespace <%= namespace %>.Tests
{
    public class KafkaConsumerStub : IKafkaConsumer
    {
        public Task Run(CancellationToken cancelToken = default(CancellationToken))
        {
            while(!cancelToken.IsCancellationRequested)
            {
                
            }
            return Task.CompletedTask;
        }
    }
}