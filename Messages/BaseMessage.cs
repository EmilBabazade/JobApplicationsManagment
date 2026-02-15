using System.Text.Json.Serialization;

namespace Messages;

public abstract class BaseMessage
{
    [JsonInclude]
    public Guid Id { get; private set; }
    [JsonInclude]
    public DateTimeOffset TimeStamp { get; private set; }

    protected BaseMessage()
    {
        Id = Guid.NewGuid();
        TimeStamp = DateTime.UtcNow;
    }
}
