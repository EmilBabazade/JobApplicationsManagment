using ApllicationsAPI.Commands.Common;
using ApllicationsAPI.Models;
using ApllicationsAPI.Models.Data;
using MediatR;

namespace ApllicationsAPI.Commands;

public record CreateApplicationCommand(
    Guid PersonId,
    TimeSlot? Appointment,
    Seniority Seniorty,
    IEnumerable<Skill> Skills
) : IRequest<ApplicationDTO>;

public class CreateApplicationHandler(ApplicationDbContext dbContext) : IRequestHandler<CreateApplicationCommand, ApplicationDTO>
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<ApplicationDTO> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = new Application(request.PersonId, request.Seniorty, request.Skills, request.Appointment);
        _dbContext.Applications.Add(application);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new ApplicationDTO(application);
    }
}