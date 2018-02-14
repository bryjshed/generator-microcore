using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace <%= namespace %>
{
    /// <summary>
    /// Manages the kafka consumer.
    /// </summary>
    public class KafkaConsumerManager : IKafkaConsumerManager
    {
        private readonly ILogger _logger;

        private readonly IServiceScopeFactory _serviceFactory;

        private readonly KafkaConfig _serviceConfig;

        private CancellationTokenSource _cancelToken;

        private Task _runningConsumer;

        // Use a shared locked to prevent consumer start and stopping from happening at the same time.
        private Object consumerLock = new Object();

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaConsumerManager"/> class.
        /// </summary>
        /// <param name="logger">The log handler for the controller.</param>
        /// <param name="serviceFactory">Create an Microsoft.Extensions.DependencyInjection.IServiceScope which
        ///     contains an System.IServiceProvider used to resolve dependencies from a newly
        ///     created scope.</param>
        /// <param name="serviceConfig">The service config settings <see cref="KafkaConfig"/></param>
        public KafkaConsumerManager(
            ILogger<KafkaConsumerManager> logger,
            IServiceScopeFactory serviceFactory,
            KafkaConfig serviceConfig)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
            _serviceConfig = serviceConfig;
        }

        /// <summary>
        /// Starts a single kafka consumer on a new thread.
        /// </summary>
        public void StartConsumer()
        {
            if (IsConsumerRunning())
            {
                throw new NotSupportedException("The consumer is already running.");
            }
            lock (consumerLock)
            {
                _cancelToken = new CancellationTokenSource();
                CancellationToken token = _cancelToken.Token;
                _runningConsumer = new Task(async () =>
                {
                    using (var scope = _serviceFactory.CreateScope())
                    {
                        // Need to fetch from the service collection since this is a new thread and scope.
                        var kafkaConsumer = scope.ServiceProvider.GetService<IKafkaConsumer>();
                        await kafkaConsumer.Run(token);
                    }
                }, token, TaskCreationOptions.LongRunning);
                _runningConsumer.Start();
                _logger.LogInformation("Starting consumer.");
            }
        }

        private bool IsConsumerRunning()
        {
            return (_runningConsumer != null) && (_runningConsumer.IsCompleted == false ||
                           _runningConsumer.Status == TaskStatus.Running ||
                           _runningConsumer.Status == TaskStatus.WaitingToRun ||
                           _runningConsumer.Status == TaskStatus.WaitingForActivation);
        }

        /// <summary>
        /// Request canceling of the kafka consumer.
        /// </summary>
        public void StopConsumer()
        {
            lock (consumerLock)
            {
                if (IsConsumerRunning())
                {
                    _cancelToken.Cancel();
                    _logger.LogInformation("Sending a cancel request to stop the consumer.");
                }
            }
        }

        /// <summary>
        /// Gets the status of the kafka consumer task.
        /// </summary>
        public string GetConsumerTaskStatus()
        {
            if (_runningConsumer != null)
            {
                return _runningConsumer.Status.ToString();
            }
            return null;
        }
    }
}