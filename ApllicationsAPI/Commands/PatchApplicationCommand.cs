using ApllicationsAPI.Commands.Common;
using ApllicationsAPI.Models;
using ApllicationsAPI.Models.Data;
using MediatR;

namespace ApllicationsAPI.Commands;

public record PatchApplicationCommand(
    Guid Id,
    Guid PersonId,
    TimeSlot? Appointment,
    Seniority Seniority,
    IEnumerable<Skill> Skills,
    Status Status
) : IRequest<ApplicationDTO>;

public class PatchApplicationHandler(
    ApplicationDbContext dbContext,
    HelperRepo helperRepo
) : IRequestHandler<PatchApplicationCommand, ApplicationDTO>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly HelperRepo _helperRepo = helperRepo;

    public async Task<ApplicationDTO> Handle(PatchApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _helperRepo.FindByIdAsync(request.Id, cancellationToken);

        application.PatchApplication(request.PersonId, request.Appointment, request.Seniority, request.Skills, request.Status);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ApplicationDTO(application);
    }
}