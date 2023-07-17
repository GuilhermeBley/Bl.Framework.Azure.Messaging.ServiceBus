using Azure.Messaging.ServiceBus;
using System;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace Bl.Framework.Azure.Messaging.ServiceBus.Bus
{
    internal class Publisher : IAsyncDisposable, IDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public Publisher(string connectionString, string queueName)
        {
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender(queueName);
        }

        public void Dispose()
            => DisposeAsync().GetAwaiter().GetResult();
        
        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
            await _client.DisposeAsync();
        }

        public async Task Publish(string message, CancellationToken cancellationToken = default)
        {
            await SendMessage(message, cancellationToken).ConfigureAwait(false);
        }

        private async Task SendMessage(string bodyMessage, CancellationToken cancellationToken)
        {
            var message = new ServiceBusMessage(bodyMessage)
            {
                
            };

            await _sender.SendMessageAsync(message, cancellationToken);
        }
    }
}
