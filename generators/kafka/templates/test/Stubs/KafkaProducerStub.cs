using System;
using System.Collections.Generic;

namespace <%= namespace %>.Tests
{
    public class KafkaProducerStub : IKafkaProducer
    {
        public void Publish<E>(KafkaEvent<E> kafkaEvent)
        {

        }

        public void Publish<E>(List<KafkaEvent<E>> kafkaEvents)
        {

        }
    }
}