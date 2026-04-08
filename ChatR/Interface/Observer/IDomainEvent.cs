namespace ChatR.Interface.Observer
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
