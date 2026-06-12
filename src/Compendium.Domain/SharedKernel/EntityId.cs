namespace Compendium.Domain.SharedKernel;

public abstract record EntityId
{
    protected EntityId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Entity id must be a non-empty GUID.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }

    public override string ToString() => Value.ToString("D");
}
