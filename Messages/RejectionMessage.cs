using System.Text.Json.Serialization;

namespace Messages;

public class RejectionMessage : BaseMessage
{
    [JsonInclude]
    public Guid ApplicationId { get; private set; }
    [JsonInclude]
    public Guid PersonId { get; private set; }
    public RejectionMessage(Guid applicationId, Guid personId) : base(applicationId)
    {
        ApplicationId = applicationId;
        PersonId = personId;
    }

    public override string ToString()
    {
        var str = $"Id: {Id}\nTimeStamp: {TimeStamp}\nApplicationId: {ApplicationId}\nPersonId: {PersonId}\n--------------------------------------------";
        return str;
    }
}