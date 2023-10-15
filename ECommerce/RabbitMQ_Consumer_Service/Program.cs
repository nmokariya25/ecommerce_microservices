using Newtonsoft.Json;
using OrderService.Models;
using PaymentService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Channels;

//Here we specify the Rabbit MQ Server. we use rabbitmq docker image and use it
var factory = new ConnectionFactory
{
    HostName = "localhost"
};
//Create the RabbitMQ connection using connection factory details as i mentioned above
var connection = factory.CreateConnection();
//Here we create channel with session and model
using
var channelOrderPayment = connection.CreateModel();
//declare the queue after mentioning name and a few property related to that
channelOrderPayment.QueueDeclare("ecom_order_payment", exclusive: false);
//Set Event object which listen message from chanel which is sent by producer
var consumerOrderPayment = new EventingBasicConsumer(channelOrderPayment);
consumerOrderPayment.Received += async (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var orderObj = JsonConvert.DeserializeObject<Order>(message);
    // Initiate Payment request
    if (orderObj != null)
    {
        using (var client = new HttpClient())
        {
            Console.WriteLine($"Order message received: {message}");
            client.BaseAddress = new Uri("http://localhost:5014");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsJsonAsync("payments", new Payment
            {
                OrderId = orderObj.Id,
                Status = orderObj.Status
            });
            if (response.IsSuccessStatusCode)
            {
                Console.Write($"Payment service has been called for Order: {message}");
            }

        }
    }
    // 
};
//read the message
channelOrderPayment.BasicConsume(queue: "ecom_order_payment", autoAck: true, consumer: consumerOrderPayment);


using
var channelPaymentOrder = connection.CreateModel();
//declare the queue after mentioning name and a few property related to that
channelPaymentOrder.QueueDeclare("ecom_payment_order", exclusive: false);
//Set Event object which listen message from chanel which is sent by producer
var consumerPaymentOrder = new EventingBasicConsumer(channelPaymentOrder);
consumerPaymentOrder.Received += async (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var paymentObj = JsonConvert.DeserializeObject<Payment>(message);
    // Initiate Payment request
    if (paymentObj != null)
    {
        using (var client = new HttpClient())
        {
            Console.WriteLine($"Payment message received: {message}");
            client.BaseAddress = new Uri("http://localhost:5096");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PutAsJsonAsync("orders", new Order
            {
                Id = paymentObj.OrderId,
                Status = paymentObj.Status
            });
            if (response.IsSuccessStatusCode)
            {
                Console.Write($"Payment service has been called for Order: {message}");
            }

        }
    }
    // 
};
//read the message
channelPaymentOrder.BasicConsume(queue: "ecom_payment_order", autoAck: true, consumer: consumerPaymentOrder);
Console.ReadKey();


Console.ReadKey();

