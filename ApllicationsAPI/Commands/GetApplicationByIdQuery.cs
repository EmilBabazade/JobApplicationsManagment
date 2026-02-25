using ApllicationsAPI.Models;
using ApllicationsAPI.Models.Data;
using ApplicationsAPI.Protos;
using MediatR;

namespace ApllicationsAPI.Commands;

public record GetApplicationByIdQuery(Guid Id) : IRequest<ApplicationDetailsDTO>;

public class GetApplicationByIdHandler(
    ApplicationDbContext dbContext,
    DocumentSearch.DocumentSearchClient documentSearchClient,
    PersonSearch.PersonSearchClient personSearchClient
) : IRequestHandler<GetApplicationByIdQuery, ApplicationDetailsDTO>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly DocumentSearch.DocumentSearchClient _documentSearchClient = documentSearchClient;
    private readonly PersonSearch.PersonSearchClient _personSearchClient = personSearchClient;

    public async Task<ApplicationDetailsDTO> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: use dappr for queries
        var application = await _dbContext.Applications.FindAsync([request.Id], cancellationToken);

        if (application == null)
        {
            return null;
        }

        var documents = await _documentSearchClient.GetAsync(new DocumentSearchRequest
        {
            PersonId = application.PersonId.ToString()
        });

        var person = await _personSearchClient.GetByIdAsync(new GetByIdRequest
        {
            Id = application.PersonId.ToString()
        });

        return new ApplicationDetailsDTO(application, documents, person);
    }
}

public class ApplicationDetailsDTO(
    Application application, 
    DocumentList documentList, 
    Person person)
{
    public Guid Id { get; set; } = application.Id;
    public Guid PersonId { get; set; } = application.PersonId;
    public TimeSlot? Appointment { get; set; } = application.Appointment;
    public Seniority Seniority { get; set; } = application.Seniority;
    public IEnumerable<Skill> Skills { get; set; } = application.Skills;
    public Status Status { get; set; } = application.Status;
    public DocumentList Documents { get; set; } = documentList;
    public Person Person { get; set; } = person;
}