using ApllicationsAPI.Commands.Common;
using ApllicationsAPI.Models.Data;
using MediatR;

namespace ApllicationsAPI.Commands;

public record DeleteApplicationCommand(Guid Id) : IRequest;

public class DeleteApplicationHandler(ApplicationDbContext dbContext, HelperRepo helperRepo) : IRequestHandler<DeleteApplicationCommand>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly HelperRepo _helperRepo = helperRepo;

    public async Task Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _helperRepo.FindByIdAsync(request.Id, cancellationToken);

        _dbContext.Applications.Remove(application);
        await _dbContext.SaveChangesAsync();
    }
}