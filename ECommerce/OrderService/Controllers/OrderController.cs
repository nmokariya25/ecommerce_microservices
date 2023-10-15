using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.MQ;

namespace OrderService.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly Ecom_OrderDBContext _context;
        private readonly IRabitMQProducer _rabitMQProducer;

        public OrderController(Ecom_OrderDBContext context, IRabitMQProducer rabitMQProducer)
        {
            this._context = context;
            this._rabitMQProducer = rabitMQProducer;
        }

        [HttpGet]
        [Route("orders")]
        public ActionResult<List<Order>> Get()
        {
            return Ok(_context.Orders.ToList());
        }

        [HttpPost]
        [Route("orders")]
        public ActionResult<Order> CreateOrder([FromBody] Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
            if (order.Id == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");

            _rabitMQProducer.SendMessage(order);

            return Ok(order);
        }

        [HttpPut]
        [Route("orders")]
        public ActionResult<Order> UpdateOrder([FromBody] Order order)
        {
            var orderObj = _context.Orders.FirstOrDefault(s => s.Id == order.Id);
            if (orderObj != null)
            {
                orderObj.Status = order.Status;
                _context.Orders.Update(orderObj);
                _context.SaveChanges();
                if (order.Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");

                return Ok(order);
            }

            return BadRequest("Payment has been failed..!!");
        }
    }
}
