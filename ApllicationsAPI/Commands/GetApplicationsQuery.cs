using ApllicationsAPI.Commands.Common;
using ApllicationsAPI.Models.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApllicationsAPI.Commands;

public record GetApplicationsQuery() : IRequest<IEnumerable<ApplicationDTO>>;

public class GetApplicationsHandler(ApplicationDbContext dbContext) : IRequestHandler<GetApplicationsQuery, IEnumerable<ApplicationDTO>>
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<IEnumerable<ApplicationDTO>> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
    {
        // TODO: use dappr to get data!
        var applications = await _dbContext.Applications.AsNoTracking().ToListAsync();
        var dtos = applications.Select(x => new ApplicationDTO(x));
        return dtos;
    }
}
