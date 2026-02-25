using ApllicationsAPI.Commands.Common;
using ApllicationsAPI.Models.Data;
using MassTransit;
using MediatR;
using Messages;

namespace ApllicationsAPI.Commands;

public record RejectCommand(Guid Id) : IRequest<ApplicationDTO>;

public class RejectHandler(
    ApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    HelperRepo helperRepo
) : IRequestHandler<RejectCommand, ApplicationDTO>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly HelperRepo _helperRepo = helperRepo;

    public async Task<ApplicationDTO> Handle(RejectCommand request, CancellationToken cancellationToken)
    {
        var application = await _helperRepo.FindByIdAsync(request.Id, cancellationToken);

        application.Reject();
        await _dbContext.SaveChangesAsync(cancellationToken);

        var message = new RejectionMessage(application.Id, application.PersonId);
        await _publishEndpoint.Publish(message);

        return new ApplicationDTO(application);
    }
}