using ApllicationsAPI.Models;
using ApllicationsAPI.Models.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApllicationsAPI.Commands;

public record GetApplicationsQuery() : IRequest<IEnumerable<ApplicationDTO>>;

public class GetApplicationsHandler(ApplicationDbContext dbContext) : IRequestHandler<GetApplicationsQuery, IEnumerable<ApplicationDTO>>
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<IEnumerable<ApplicationDTO>> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
    {
        // TODO: use dappr to get data!
        var applications = await _dbContext.Applications.AsNoTracking().ToListAsync();
        var dtos = applications.Select(x => new ApplicationDTO(x));
        return dtos;
    }
}

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