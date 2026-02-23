using ApllicationsAPI.Commands;
using ApllicationsAPI.Commands.Common;
using ApllicationsAPI.Models;
using ApllicationsAPI.Models.Data;
using ApplicationsAPI.Protos;
using MassTransit;
using MediatR;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApllicationsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DocumentSearch.DocumentSearchClient _documentClient;
        private readonly PersonSearch.PersonSearchClient _personSearchClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMediator _mediator;

        public ApplicationsController(ApplicationDbContext context, DocumentSearch.DocumentSearchClient documentClient,
            PersonSearch.PersonSearchClient personSearchClient, IPublishEndpoint publishEndpoint, IMediator mediator)
        {
            _context = context;
            _documentClient = documentClient;
            _personSearchClient = personSearchClient;
            _publishEndpoint = publishEndpoint;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationDTO>>> GetApplications()
        {
            var res = await _mediator.Send(new GetApplicationsQuery());
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationDetailsDTO>> GetApplication(Guid id)
        {
            var application = await _mediator.Send(new GetApplicationByIdQuery(id));
            if (application == null)
            {
                return NotFound();
            }
            return application;
        }

        [HttpPatch("{id}/PatchIncorrectData")]
        public async Task<IActionResult> PatchApplication(Guid id, PatchApplicationRequest requestData)
        {
            return await Patch(id, application => application.PatchApplication(requestData));
        }

        [HttpPatch("{id}/NextStep")]
        public async Task<IActionResult> NextStep(Guid id, NextStepRequest requestData)
        {
            return await Patch(
                id, 
                application => application.NextStep(requestData),
                async application =>
                {
                    if(application.Appointment != null)
                    {
                        var message = new AppointmentSetMessage(application.Id, application.PersonId, application.Appointment.Start, application.Appointment.End);
                        await _publishEndpoint.Publish(message);
                    }
                });
        }

        [HttpPatch("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            return await Patch(id, 
                application => application.Reject(),
                async application =>
                {
                    var message = new RejectionMessage(application.Id, application.PersonId);
                    await _publishEndpoint.Publish(message);
                });
        }

        [HttpPatch("{id}/SetAppointment")]
        public async Task<IActionResult> SetAppointment(Guid id, TimeSlot timeSlot)
        {
            return await Patch(id, 
                application => application.SetAppointment(timeSlot),
                async application =>
                {
                    if (application.Appointment != null)
                    {
                        var message = new AppointmentSetMessage(application.Id, application.PersonId, application.Appointment.Start, application.Appointment.End);
                        await _publishEndpoint.Publish(message);
                    }
                });
        }

        private async Task<IActionResult> Patch(Guid id, Action<Application> action, Func<Application, Task>? postSaveAction = null)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            try
            {
                action(application);
                await _context.SaveChangesAsync();
                if (postSaveAction != null)
                {
                    await postSaveAction(application);
                }
            }
            catch (AlreadyAcceptedException)
            {
                return BadRequest("Employee is already hired!");
            }
            catch (AlreadyRejectedException)
            {
                return BadRequest("Employee is already rejected");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Application>> PostApplication(CreateApplicationCommand requestData)
        {
            var application = await _mediator.Send(requestData);
            return CreatedAtAction("GetApplication", new { id = application.Id }, application);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(Guid id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}