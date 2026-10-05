using Claims.Domain.Enums;
using Claims.Domain.Models.Insurance;
using Claims.Infrastructure.Repository.Insurance;
using Claims.Infrastructure.Services.Audit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Claims.Controllers
{
    /// <summary>
    /// Controller class for managing covers.
    /// </summary>
    [ApiController]
    [Route("api/covers")]
    [Produces("application/json")]
    public class CoversController : ControllerBase
    {
        private readonly IInsuranceRepository insuranceRepository;
        private readonly ILogger<CoversController> _logger;
        private readonly IAuditService auditer;

        public CoversController(IInsuranceRepository claimsRepository, IAuditService auditer, ILogger<CoversController> logger)
        {
            this.insuranceRepository = claimsRepository;
            _logger = logger;
            this.auditer = auditer;
        }

        // POST: api/covers/compute
        [HttpPost("compute")]
        [SwaggerOperation(Summary = "An API endpoint for computing cover premium. Takes start and end dates along with cover type as input.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Premium computed successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while computing premium.")]
        public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            return Ok(ComputePremium(startDate, endDate, coverType));
        }

        // GET: api/covers
        [HttpGet]
        [SwaggerOperation(Summary = "An API endpoint for retrieving all covers.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Covers retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while retreiving covers list.")]
        public async Task<ActionResult<List<Cover>>> GetAsync()
        {
            List<Cover> results = await insuranceRepository.GetAllCoversAsync();
            return Ok(results);
        }

        // GET: api/covers/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "An API endpoint for retrieving a cover. Takes a Cover Id as input.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Cover retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid id.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while retrieving cover.")]
        public async Task<ActionResult<Cover>> GetAsync(string id)
        {
            Cover? result = await insuranceRepository.GetCoverByIdAsync(id);
            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/covers
        [HttpPost]
        [SwaggerOperation(Summary = "An API endpoint for creating a new cover. Takes a Cover object as input.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Cover created successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid cover data.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while creating cover.")]
        public async Task<ActionResult<string>> CreateAsync(Cover cover)
        {
            cover.Id = Guid.NewGuid().ToString();
            cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

            await insuranceRepository.AddCoverAsync(cover);
            await auditer.AuditCover(cover.Id, "POST");
            return Ok(cover.Id);
        }

        // DELETE: api/covers/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "An API endpoint for retrieving a claim. Takes a Claim Id as input.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Claim retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid id.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while retrieving claim.")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            await auditer.AuditCover(id, "DELETE");
            await insuranceRepository.DeleteCoverByIdAsync(id);
            return Ok();
        }

        private decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            var multiplier = 1.3m;
            if (coverType == CoverType.Yacht)
            {
                multiplier = 1.1m;
            }

            if (coverType == CoverType.PassengerShip)
            {
                multiplier = 1.2m;
            }

            if (coverType == CoverType.Tanker)
            {
                multiplier = 1.5m;
            }

            var premiumPerDay = 1250 * multiplier;
            var insuranceLength = (endDate - startDate).TotalDays;
            var totalPremium = 0m;

            for (var i = 0; i < insuranceLength; i++)
            {
                if (i < 30) totalPremium += premiumPerDay;
                if (i < 180 && coverType == CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
                else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
                if (i < 365 && coverType != CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
                else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;
            }

            return totalPremium;
        }
    }
}
