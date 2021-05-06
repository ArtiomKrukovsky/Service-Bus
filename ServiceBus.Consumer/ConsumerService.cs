using Microsoft.Extensions.Hosting;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using ServiceBus.Consumer.Models;

namespace ServiceBus.Consumer
{
    public class ConsumerService : BackgroundService
    {
        private readonly ISubscriptionClient _subscriptionClient;

        public ConsumerService(ISubscriptionClient subscriptionClient)
        {
            _subscriptionClient = subscriptionClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _subscriptionClient.RegisterMessageHandler((message, token) =>
            {
                // convert message to the object
                var objAsString = Encoding.UTF8.GetString(message.Body);
                var order = JsonConvert.DeserializeObject<Order>(objAsString);

                // do smth with data
                Console.WriteLine($"New order with name {order.ProductName} and id {order.Id}");
                return _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }, new MessageHandlerOptions(args => Task.CompletedTask)
            {
                AutoComplete = false,
                MaxConcurrentCalls = 1
            });

            return Task.CompletedTask;
        }
    }
}
