namespace PaymentService.MQ
{
    public interface IRabitMQProducer
    {
        public void SendMessage<T>(T message);
    }
}
