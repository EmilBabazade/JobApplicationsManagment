using ApllicationsAPI.Commands.Common;
using ApllicationsAPI.Models;
using ApllicationsAPI.Models.Data;
using MassTransit;
using MediatR;
using Messages;

namespace ApllicationsAPI.Commands;

public record SetAppointmentCommand(
    Guid Id,
    TimeSlot TimeSlot
) : IRequest<ApplicationDTO>;

public class SetAppointmentHandler(
    ApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    HelperRepo helperRepo  
) : IRequestHandler<SetAppointmentCommand, ApplicationDTO>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly HelperRepo _helperRepo = helperRepo;

    public async Task<ApplicationDTO> Handle(SetAppointmentCommand request, CancellationToken cancellationToken)
    {
        var application = await _helperRepo.FindByIdAsync(request.Id, cancellationToken);

        application.SetAppointment(request.TimeSlot);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var message = new AppointmentSetMessage(
            application.Id, 
            application.PersonId, 
            application.Appointment.Start, 
            application.Appointment.End
        );
        await _publishEndpoint.Publish(message);

        return new ApplicationDTO(application);
    }
}