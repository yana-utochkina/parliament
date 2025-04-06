using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ParliamentInfrastructure.Controllers;

[Route("api/chart")]
[ApiController]
public class ChartController : ControllerBase
{
    private record CountByMonthResponseItem(string  Month, int Count);

    private readonly ParliamentDbContext parliamentDbContext;

    public ChartController(ParliamentDbContext parliamentDbContext)
    {
        this.parliamentDbContext = parliamentDbContext;
    }

    [HttpGet("countByMonth/{departmentId}")]
    public async Task<JsonResult> GetCountByMonthAsync(int departmentId, CancellationToken cancellationToken)
    {
        var responseItems = await parliamentDbContext.Events
            .Where(e => e.DepartmentId == departmentId)
            .GroupBy(e => new { e.StartDate.Year, e.StartDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new CountByMonthResponseItem(
                $"{g.Key.Year}-{g.Key.Month:D2}",
                g.Count()))
            .ToListAsync(cancellationToken);

        return new JsonResult(responseItems);
    }


}
