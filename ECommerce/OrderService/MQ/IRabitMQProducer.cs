namespace OrderService.MQ
{
    public interface IRabitMQProducer
    {
        public void SendMessage<T>(T message);
    }
}
