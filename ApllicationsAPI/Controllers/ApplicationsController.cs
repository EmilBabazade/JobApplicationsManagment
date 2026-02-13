using ApllicationsAPI.Models;
using ApllicationsAPI.Models.Data;
using ApplicationsAPI.Protos;
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

        public ApplicationsController(ApplicationDbContext context, DocumentSearch.DocumentSearchClient documentClient,
            PersonSearch.PersonSearchClient personSearchClient)
        {
            _context = context;
            _documentClient = documentClient;
            _personSearchClient = personSearchClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
        {
            return await _context.Applications.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationDetailsDTO>> GetApplication(Guid id)
        {
            var application = await _context.Applications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            var documents = await _documentClient.GetAsync(new DocumentSearchRequest
            {
                PersonId = application.PersonId.ToString()
            });

            var person = await _personSearchClient.GetByIdAsync(new GetByIdRequest
            {
                Id = application.PersonId.ToString()
            });

            return new ApplicationDetailsDTO(application, documents, person);
        }

        [HttpPatch("{id}/PatchIncorrectData")]
        public async Task<IActionResult> PatchApplication(Guid id, PatchApplicationRequest requestData)
        {
            return await Patch(id, application => application.PatchApplication(requestData));
        }

        [HttpPatch("{id}/NextStep")]
        public async Task<IActionResult> NextStep(Guid id)
        {
            return await Patch(id, application => application.NextStep());
        }

        [HttpPatch("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            return await Patch(id, application => application.Reject());
        }

        [HttpPatch("{id}/SetAppointment")]
        public async Task<IActionResult> SetAppointment(Guid id, TimeSlot timeSlot)
        {
            return await Patch(id, application => application.SetAppointment(timeSlot));
        }

        private async Task<IActionResult> Patch(Guid id, Action<Application> action)
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
        public async Task<ActionResult<Application>> PostApplication(CreateApplicationRequest requestData)
        {
            var application = new Application(requestData);
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

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

        private bool ApplicationExists(Guid id)
        {
            return _context.Applications.Any(e => e.Id == id);
        }
    }
}