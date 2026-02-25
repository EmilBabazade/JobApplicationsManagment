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
        private readonly IMediator _mediator;

        public ApplicationsController(IMediator mediator)
        {
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
        public async Task<ActionResult<ApplicationDTO>> PatchApplication(PatchApplicationCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPatch("{id}/NextStep")]
        public async Task<ActionResult<ApplicationDTO>> NextStep(NextStepCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPatch("{id}/Reject")]
        public async Task<ActionResult<ApplicationDTO>> Reject(Guid id)
        {
            var command = new RejectCommand(id);
            return await _mediator.Send(command);
        }

        [HttpPatch("{id}/SetAppointment")]
        public async Task<ActionResult<ApplicationDTO>> SetAppointment(SetAppointmentCommand command)
        {
            return await _mediator.Send(command);
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
            var command = new DeleteApplicationCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}