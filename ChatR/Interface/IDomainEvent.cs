namespace ChatR.Interface
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
