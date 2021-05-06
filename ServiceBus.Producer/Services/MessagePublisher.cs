using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Producer.Services
{
    public class MessagePublisher : IMessagePublisher
    {
        public readonly ITopicClient _topicClient;

        public MessagePublisher(ITopicClient topicClient)
        {
            _topicClient = topicClient;
        }

        public async Task Publish<T>(T obj)
        {
            // convert the object to transformable type
            var objAsText = JsonConvert.SerializeObject(obj);
            var message = new Message(Encoding.UTF8.GetBytes(objAsText));

            // add property value uses by filter
            message.UserProperties[Constants.Constants.MessageType] = typeof(T).Name;

            // send the message to the topic
            await _topicClient.SendAsync(message);
        }

        public async Task Publish(string raw)
        {
            var message = new Message(Encoding.UTF8.GetBytes(raw));

            message.UserProperties[Constants.Constants.MessageType] = "raw";

            await _topicClient.SendAsync(message);
        }
    }
}
