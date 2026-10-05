using Claims.Domain.Enums;
using Claims.Domain.Models.Audit;
using Claims.Domain.Models.Insurance;
using Claims.Infrastructure.Repository.Insurance;
using Claims.Infrastructure.Services.Audit;
using Claims.Infrastructure.Services.Validation;
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
        private readonly IAuditService auditService;
        private readonly IValidationService validationService;

        private readonly AuditQueue auditQueue;

        public CoversController(
            IInsuranceRepository claimsRepository,
            IAuditService auditer, ILogger<CoversController> logger,
            IValidationService validationService,
            AuditQueue auditQueue)
        {
            this.insuranceRepository = claimsRepository;
            _logger = logger;
            this.auditService = auditer;
            this.validationService = validationService;
            this.auditQueue = auditQueue;
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
            return Ok(auditService.ComputePremium(startDate, endDate, coverType));
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
        public async Task<ActionResult<Cover>> GetAsync(int id)
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
        public async Task<ActionResult<int>> CreateAsync(Cover cover)
        {
            if (validationService.ValidateCover(cover))
            {
                cover.Premium = auditService.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

                await insuranceRepository.AddCoverAsync(cover);
                await auditQueue.EnqueueAsync(new AuditMessage
                {
                    Type = AuditType.Cover,
                    EntityId = cover.Id,
                    HttpRequestType = "POST",
                    Created = DateTime.UtcNow
                });

                return Ok(cover.Id);
            }
            else
            {
                return BadRequest("Invalid cover data.");
            }
        }

        // DELETE: api/covers/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "An API endpoint for retrieving a claim. Takes a Claim Id as input.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Claim retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid id.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while retrieving claim.")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await auditQueue.EnqueueAsync(new AuditMessage
            {
                Type = AuditType.Cover,
                EntityId = id,
                HttpRequestType = "DELETE",
                Created = DateTime.UtcNow
            });

            await insuranceRepository.DeleteCoverByIdAsync(id);
            return Ok();
        }
    }
}
