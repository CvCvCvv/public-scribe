namespace CommandsReceiver.Application.Abstractions
{
    public interface IRabbitMqService
    {
        void SendMessage(object obj);
        void SendMessage(string message);
        int CountInQueue();
    }
}
