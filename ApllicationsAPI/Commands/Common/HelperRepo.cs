using ApllicationsAPI.Models;
using ApllicationsAPI.Models.Data;

namespace ApllicationsAPI.Commands.Common;

// THIS IS ONLY FOR FREQUENTLY USED COMMON MEHTODS, NOT FOR REPOSITORY PATTERN!
// JUST USE THIS FOR STUFF YOU DO WITH DB THAT YOU NEED TO DO AGAIN AND AGAIN, DONT PUT EVERYTINHG HERE JUST FOR THE SAKE OF PUTTING THEM HERE

public class HelperRepo(ApplicationDbContext dbContext)
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Application> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var application = await _dbContext.Applications.FindAsync([id], cancellationToken);
        if (application == null)
        {
            throw new NotFoundException();
        }
        return application;
    }
}