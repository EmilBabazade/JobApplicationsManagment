using System.Text.Json.Serialization;

namespace Messages;

public class RejectionMessage : BaseMessage
{
    [JsonInclude]
    public Guid ApplicationId { get; private set; }
    [JsonInclude]
    public Guid PersonId { get; private set; }
    public RejectionMessage(Guid applicationId, Guid personId) : base()
    {
        ApplicationId = applicationId;
        PersonId = personId;
    }
}