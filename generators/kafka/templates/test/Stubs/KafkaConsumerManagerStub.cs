using System.Collections.Generic;
using System.Threading;

namespace <%= namespace %>.Tests
{
    public class KafkaConsumerManagerStub : IKafkaConsumerManager
    {
        private CancellationTokenSource _cancelToken;

        private IList<object> _eventList;

        public KafkaConsumerManagerStub()
        {
            _cancelToken = new CancellationTokenSource();
            _eventList = new List<object>();
        }

        public string GetConsumerTaskStatus()
        {
            return string.Empty;
        }

        public void StartConsumer()
        {
           
        }

        public void StopConsumer()
        {
            _cancelToken.Cancel();
        }
    }
}