namespace ChatR.Interface.Observer
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent;
    }
}
