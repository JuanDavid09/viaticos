using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Viaticos.Infrastructure.Persistence;

namespace Viaticos.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ViaticosDbContext _dbContext;

    public HealthController(ViaticosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

        if (!canConnect)
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { status = "unhealthy", database = "disconnected" });

        return Ok(new { status = "healthy", database = "connected", health = "/health" });
    }
}
