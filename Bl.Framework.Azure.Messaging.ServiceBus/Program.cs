using Bl.Framework.Azure.Messaging.ServiceBus.Bus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bl.Framework.Azure.Messaging.ServiceBus
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

            var azureServiceBusConnectionString = configurationRoot.GetConnectionString("AzServiceBus");

            Console.Write("Type message: ");
            var messageLine = Console.ReadLine();

            Console.Write("Type message quantity (number 1 - 100): ");
            var readCountText = Console.ReadLine();

            Console.Write("Type consumer quantity (number 1 - 100): ");
            var consumerQttText = Console.ReadLine();

            int.TryParse(readCountText, out int readCount);
            int.TryParse(consumerQttText, out int consumerQtt);

            readCount = readCount < 1 || readCount > 100 ? 10 : readCount;
            consumerQtt = consumerQtt < 1 || consumerQtt > 100 ? 10 : consumerQtt;

            var messagesConsole = new ConcurrentQueue<string>();

            using (var consumer = new Consumer(azureServiceBusConnectionString, "queue-example"))
            {
                using (var pub = new Publisher(azureServiceBusConnectionString, "queue-example"))
                {
                    for (int idxMessageQtt = 0; idxMessageQtt < readCount; idxMessageQtt++)
                        await pub.Publish(string.Concat($"{idxMessageQtt} - ", messageLine));
                }

                for(int idxQttConsu = 0; idxQttConsu < consumerQtt; idxQttConsu++)
                    consumer.CreateThreadConsumer(
                        (m) =>
                        {
                            messagesConsole.Enqueue(Encoding.UTF8.GetString(m.Body.ToArray()));
                        });

                while (true)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    messagesConsole.TryDequeue(out string messageToWrite);

                    if (messageToWrite != null)
                        Console.WriteLine(messageToWrite);
                }
            }
        }
    }
}
