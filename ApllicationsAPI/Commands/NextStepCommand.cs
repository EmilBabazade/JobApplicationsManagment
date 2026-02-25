using ApllicationsAPI.Commands.Common;
using ApllicationsAPI.Models;
using ApllicationsAPI.Models.Data;
using MassTransit;
using MediatR;
using Messages;

namespace ApllicationsAPI.Commands;

public record NextStepCommand(Guid id, TimeSlot? Appointment) : IRequest<ApplicationDTO>;

public class NextStepHandler(
    ApplicationDbContext dbContext, 
    IPublishEndpoint publishEndpoint,
    HelperRepo helperRepo
) : IRequestHandler<NextStepCommand, ApplicationDTO>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly HelperRepo _helperRepo = helperRepo;

    public async Task<ApplicationDTO> Handle(NextStepCommand request, CancellationToken cancellationToken)
    {
        var application = await _helperRepo.FindByIdAsync(request.id, cancellationToken);

        try
        {
            application.NextStep(request.Appointment);
        }
        catch (AlreadyAcceptedException)
        {
            throw new BadRequestException("Employee is already hired!");
        }
        catch (AlreadyRejectedException)
        {
            throw new BadRequestException("Employee is already rejected");
        }
        await _dbContext.SaveChangesAsync(cancellationToken);

        if(application.Appointment != null)
        {
            var message = new AppointmentSetMessage(application.Id, application.PersonId, application.Appointment.Start, application.Appointment.End);
            await _publishEndpoint.Publish(message);
        }

        return new ApplicationDTO(application);
    }
}