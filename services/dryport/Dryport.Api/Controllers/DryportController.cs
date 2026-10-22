using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dryport.Application.Services;
using Dryport.Application.DTOs;
using Dryport.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

// Placeholder for EF DbContext to allow compilation of Controller concepts
namespace Dryport.Infrastructure.Persistence
{
    public interface IDryportDbContext
    {
        IQueryable<ContainerRecord> Containers { get; }
        IQueryable<DemurrageAlert> DemurrageAlerts { get; }
        IQueryable<DemurrageRule> DemurrageRules { get; }
        
        void Add<T>(T entity) where T : class;
        Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default);
    }
}

namespace Dryport.Api.Controllers
{
    using Dryport.Infrastructure.Persistence;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class DryportController : ControllerBase
    {
        private readonly IDryportDbContext _dbContext;
        private readonly IDemurrageService _demurrageService;
        private readonly ILogger<DryportController> _logger;

        public DryportController(
            IDryportDbContext dbContext,
            IDemurrageService demurrageService,
            ILogger<DryportController> logger)
        {
            _dbContext = dbContext;
            _demurrageService = demurrageService;
            _logger = logger;
        }

        [HttpGet("containers/demurrage")]
        public async Task<IActionResult> GetContainersDemurrageOverview()
        {
            try
            {
                var currentDate = DateTime.UtcNow;

                // In a real EF scenario, use .Include() and .ToListAsync()
                var containers = _dbContext.Containers
                    .Where(c => c.Status == ContainerStatus.Arrived || c.Status == ContainerStatus.InStorage)
                    .ToList();

                var response = containers.Select(c => new ContainerDemurrageDto
                {
                    ContainerId = c.Id,
                    ContainerNumber = c.ContainerNumber,
                    Status = c.Status.ToString(),
                    DaysInPort = (currentDate - c.ArrivalDate).Days,
                    FreeDays = c.DemurrageRule?.FreeDays ?? 0,
                    AccruedDemurrage = _demurrageService.CalculateDemurrage(c, currentDate),
                    Currency = c.DemurrageRule?.Currency ?? "USD"
                }).OrderByDescending(c => c.AccruedDemurrage).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting demurrage data");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("containers/receive")]
        public async Task<IActionResult> ReceiveContainer([FromBody] ReceiveContainerDto request)
        {
            try
            {
                var ruleExists = _dbContext.DemurrageRules.Any(r => r.Id == request.DemurrageRuleId);
                if (!ruleExists)
                {
                    return NotFound($"Rule ID {request.DemurrageRuleId} not found.");
                }

                var newContainer = new ContainerRecord
                {
                    Id = Guid.NewGuid(),
                    ContainerNumber = request.ContainerNumber,
                    Size = request.Size,
                    Type = request.Type,
                    ArrivalDate = DateTime.UtcNow,
                    Status = ContainerStatus.Arrived,
                    ClientId = request.ClientId,
                    DemurrageRuleId = request.DemurrageRuleId,
                    AccruedDemurrage = 0
                };

                _dbContext.Add(newContainer);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("New container registered: {ContainerNumber}", request.ContainerNumber);

                return CreatedAtAction(nameof(GetContainersDemurrageOverview), new { id = newContainer.Id }, newContainer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering container");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
