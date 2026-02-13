using Grpc.Core;

namespace PersonsService.Services;

public class PersonSearchService(ILogger<PersonSearchService> logger) : PersonSearch.PersonSearchBase
{
    private readonly List<Person> _persons = [
        new Person() {
            Id = "11111111-1111-1111-1111-111111111111",
            Address = "Stabu iela 16",
            Email = "bob@gmail.com",
            LastName = "bobson",
            Name = "bob",
            Phone = "+3712356434"
        }
    ];

    public override async Task<Person> GetById(GetByIdRequest request, ServerCallContext context)
    {
        return _persons.Find(x => x.Id == request.Id);
    }
}