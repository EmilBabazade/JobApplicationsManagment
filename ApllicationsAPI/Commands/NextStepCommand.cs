using ApllicationsAPI.Models;
using ApllicationsAPI.Models.Data;
using MassTransit;
using MediatR;
using Messages;

namespace ApllicationsAPI.Commands;

public record NextStepCommand(Guid id, TimeSlot? Appointment) : IRequest;

public class NextStepHandler(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint) : IRequestHandler<NextStepCommand>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Handle(NextStepCommand request, CancellationToken cancellationToken)
    {
        var application = await _dbContext.Applications.FindAsync(request.id, cancellationToken);
        if (application == null)
        {
            throw new NotFoundException();
        }

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
    }
}