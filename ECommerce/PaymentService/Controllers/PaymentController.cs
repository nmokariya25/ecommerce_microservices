using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;
using PaymentService.MQ;

namespace PaymentService.Controllers
{
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ECom_PaymentDBContext _context;
        private readonly IRabitMQProducer _rabitMQProducer;

        public PaymentController(ECom_PaymentDBContext context, IRabitMQProducer rabitMQProducer)
        {
            this._context = context;
            this._rabitMQProducer = rabitMQProducer;
        }

        [HttpGet]
        [Route("payments")]
        public ActionResult<List<Payment>> Get()
        {
            return Ok(_context.Payments.ToList());
        }

        [HttpPost]
        [Route("payments")]
        public ActionResult<Payment> ProcessPayment([FromBody] Payment payment)
        {
            bool paymentStatus = true;
            // get the live payment status
            if (paymentStatus)
                payment.Status = "PAID";
            else
                payment.Status = "FAILED";
            _context.Payments.Add(payment);
            _context.SaveChanges();
            if (payment.Id == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");

            _rabitMQProducer.SendMessage(payment);

            return Ok(payment);
        }
    }
}
