using Azure.Messaging.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bl.Framework.Azure.Messaging.ServiceBus.Bus
{
    internal class Consumer : IDisposable, IAsyncDisposable
    {

        private readonly ServiceBusClient _client;
        private readonly string _queueName;

        public Consumer(string connectionString, string queueName)
        {
            _client = new ServiceBusClient(connectionString);
            _queueName = queueName;
        }

        public void Dispose()
            => DisposeAsync().GetAwaiter().GetResult();

        public async ValueTask DisposeAsync()
        {
            await _client.DisposeAsync();
        }

        public void CreateThreadConsumer(Action<ServiceBusReceivedMessage> action, CancellationToken cancellationToken = default)
        {
            var thread = new Thread(new ThreadStart(()=>ConsumeTaskMethod(action, cancellationToken)));

            thread.IsBackground = true;
            thread.Start();
        }

        private void ConsumeTaskMethod(Action<ServiceBusReceivedMessage> act, CancellationToken cancellationToken)
        {
            ServiceBusReceiver receiver = null;

            try
            {
                receiver = _client.CreateReceiver(_queueName);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var message = receiver.ReceiveMessageAsync().GetAwaiter().GetResult();

                    act(message);

                    Thread.Sleep(50);
                }
            }
            finally
            {
                if (receiver != null)
                    receiver.DisposeAsync().GetAwaiter().GetResult();
            }
        }
    }
}
