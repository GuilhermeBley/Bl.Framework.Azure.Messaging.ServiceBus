using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Bl.Framework.Azure.Messaging.ServiceBus
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();
            
            var azureServiceBusConnectionString = configurationRoot.GetConnectionString("AzServiceBus");
            


            
        }
    }
}
