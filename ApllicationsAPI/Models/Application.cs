using ApplicationsAPI.Protos;
using Microsoft.EntityFrameworkCore;

namespace ApllicationsAPI.Models;

[Owned]
public class TimeSlot
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}

[Owned]
public class Skill
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public enum Seniority
{
    Junior,
    Middle,
    Senior,
}

public enum Status
{
    HRInterview,
    TechnicalInterview,
    Homework,
    FinalInterview,
    Accepted,
    Rejected,
}

public class CreateApplicationRequest
{
    public Guid PersonId { get; set; }
    public TimeSlot? Appointment { get; set; }
    public Seniority Seniority { get; set; }
    public IEnumerable<Skill> Skills { get; set; }
}

public class PatchApplicationRequest
{
    public Guid PersonId { get; set; }
    public TimeSlot? Appointment { get; set; }
    public Seniority Seniority { get; set; }
    public IEnumerable<Skill> Skills { get; set; }
    public Status Status { get; set; }
}

public class ApplicationDetailsDTO
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public TimeSlot? Appointment { get; set; }
    public Seniority Seniority { get; set; }
    public IEnumerable<Skill> Skills { get; set; }
    public Status Status { get; set; }
    public DocumentList Documents { get; set; }
    public Person Person { get; set; }

    public ApplicationDetailsDTO(Application application, DocumentList documentList, Person person)
    {
        Id = application.Id;
        PersonId = application.PersonId;
        Appointment = application.Appointment;
        Seniority = application.Seniority;
        Skills = application.Skills;
        Status = application.Status;
        Documents = documentList;
        Person = person;
    }
}

public class Application
{
    public Guid Id { get; private set; }
    public Guid PersonId { get; private set; }
    public TimeSlot? Appointment { get; private set; }
    public Seniority Seniority { get; private set; }
    public IEnumerable<Skill> Skills { get; private set; }
    public Status Status { get; private set; }

    public Application()
    { }

    public Application(Guid personId, Seniority seniority, IEnumerable<Skill> skills, TimeSlot? appointment = null)
    {
        Id = Guid.NewGuid();
        PersonId = personId;
        Appointment = appointment;
        Seniority = seniority;
        Skills = skills;
        Status = Status.HRInterview;
    }

    public Application(CreateApplicationRequest createRequest) :
        this(createRequest.PersonId, createRequest.Seniority, createRequest.Skills, createRequest.Appointment)
    {
    }

    /// <summary>
    /// oh shit i need to update a specific thing dumbass user clicked next step twice or some shit idk
    /// in real world you would need to tell api consumer to not use this unless necessary but they would still use this because who cares about domain
    /// and then figure out the business logic themselves later that actually you need to use only for NextStep and UpdateAppointment and this is for just in case
    /// you need to update other shit. Is this ideal ? no - but it is also not ideal to manually update database, better give users a way to un-fuckup
    /// </summary>
    /// <param name="patchApplicationRequest"></param>
    public void PatchApplication(PatchApplicationRequest patchApplicationRequest)
    {
        PersonId = patchApplicationRequest.PersonId;
        Appointment = patchApplicationRequest.Appointment;
        Seniority = patchApplicationRequest.Seniority;
        Skills = patchApplicationRequest.Skills;
        Status = patchApplicationRequest.Status;
    }

    public void SetAppointment(TimeSlot appointment)
    {
        Appointment = appointment;
    }

    public void NextStep(TimeSlot? appointment = null)
    {
        Appointment = appointment;
        Status = Status switch
        {
            Status.HRInterview => Status.TechnicalInterview,
            Status.TechnicalInterview => Status.Homework,
            Status.Homework => Status.FinalInterview,
            Status.FinalInterview => Status.Accepted,
            Status.Rejected => throw new AlreadyRejectedException(),
            Status.Accepted => throw new AlreadyAcceptedException(),
            _ => throw new InvalidDataException("this shit aint in the enum")
        };
    }

    public void Reject()
    {
        Status = Status.Rejected;
    }
}

public class AlreadyAcceptedException : Exception
{
}

public class AlreadyRejectedException : Exception
{
}