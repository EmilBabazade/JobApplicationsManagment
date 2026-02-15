using System.Text.Json.Serialization;

namespace Messages;

public class AppointmentSetMessage : BaseMessage
{
    [JsonInclude]
    public Guid ApplicationId { get; private set; }
    [JsonInclude]
    public Guid PersonId { get; private set; }
    [JsonInclude]
    public DateTimeOffset AppointmentStart { get; private set; }
    [JsonInclude]
    public DateTimeOffset AppointmentEnd { get; private set; }
    public AppointmentSetMessage(Guid applicationId, Guid personId, DateTimeOffset appointmentStart, DateTimeOffset appointmentEnd) : base()
    {
        ApplicationId = applicationId;
        PersonId = personId;
        AppointmentStart = appointmentStart;
        AppointmentEnd = appointmentEnd;
    }

    public override string ToString()
    {
        var str = $"Id: {Id}\nTimeStamp: {TimeStamp}\nApplicationId: {ApplicationId}\nPersonId: {PersonId}\nAppointmentStart: {AppointmentStart}\nAppointmentEnd: {AppointmentEnd}\n--------------------------------------------";
        return str;
    }
}
