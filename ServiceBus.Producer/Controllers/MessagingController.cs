using Microsoft.AspNetCore.Mvc;
using ServiceBus.Producer.Models;
using ServiceBus.Producer.Requests;
using ServiceBus.Producer.Services;
using System.IO;
using System.Threading.Tasks;

namespace ServiceBus.Producer.Controllers
{
    public class MessagingController : ControllerBase
    {
        private readonly IMessagePublisher _messagePublisher;

        public MessagingController(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        [HttpPost]
        [Route("publish/text")]
        public async Task<IActionResult> PublishText()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var bodyAsText = await reader.ReadToEndAsync();
                await _messagePublisher.Publish(bodyAsText);
            };

            return Ok();
        }

        [HttpPost]
        [Route("publish/order")]
        public async Task<IActionResult> PublishOrder([FromBody] CreateOrderRequest request)
        {
            var order = new Order
            {
                Id = request.Id,
                ProductName = request.ProductName
            };

            await _messagePublisher.Publish(order);
            return Ok();
        }
    }
}
