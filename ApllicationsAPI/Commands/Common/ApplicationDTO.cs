using ApllicationsAPI.Models;

namespace ApllicationsAPI.Commands.Common;

public class ApplicationDTO
{
    public Guid Id { get; private set; }
    public Guid PersonId { get; private set; }
    public TimeSlot? Appointment { get; private set; }
    public Seniority Seniority { get; private set; }
    public IEnumerable<Skill> Skills { get; private set; }
    public Status Status { get; private set; }
    public ApplicationDTO(Application application)
    {
        Id = application.Id;
        PersonId = application.PersonId;
        Appointment = application.Appointment;
        Seniority = application.Seniority;
        Skills = application.Skills;
        Status = application.Status;
    }

    public ApplicationDTO()
    {
        
    }
}