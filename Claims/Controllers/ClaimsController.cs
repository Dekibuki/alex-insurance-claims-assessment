using Claims.Domain.Models.Insurance;
using Claims.Infrastructure.Repository.Insurance;
using Claims.Infrastructure.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Claims.Controllers
{
    /// <summary>
    /// Controller class for managing claims.
    /// </summary>
    [ApiController]
    [Route("api/claims")]
    [Produces("application/json")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly IInsuranceRepository claimsRepository;

        private readonly IValidationService validationService;

        public ClaimsController(ILogger<ClaimsController> logger, IInsuranceRepository claimsRepository, IValidationService validationService)
        {
            _logger = logger;
            this.claimsRepository = claimsRepository;
            this.validationService = validationService;
        }

        // GET: api/claims
        [HttpGet]
        [SwaggerOperation(Summary = "An API endpoint for getting a list of all claims.")]
        [SwaggerResponse(StatusCodes.Status200OK, "List of claims resturned successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while retreiving claims list.")]
        public async Task<ActionResult<List<Claim>>> GetAsync()
        {
            List<Claim> claims = await claimsRepository.GetAllClaimsAsync();
            return Ok(claims);
        }

        // POST: api/claims
        [HttpPost]
        [SwaggerOperation(Summary = "An API endpoint for creating a new claim. Takes a Claim object as input.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Claim created successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid claim data.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while creating claim.")]
        public async Task<ActionResult<int>> CreateAsync(Claim claim)
        {
            if (await validationService.ValidateClaim(claim))
            {
                await claimsRepository.AddClaimAsync(claim);
                return Ok(claim.Id);
            }
            else
            {
                return BadRequest("Invalid claim data.");
            }
        }

        // DELETE: api/claims/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "An API endpoint for deleting a claim. Takes a Claim Id as input.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Claim deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid id.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while deleting claim.")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await claimsRepository.DeleteClaimByIdAsync(id);
            return Ok();
        }

        // GET: api/claims/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "An API endpoint for retrieving a claim. Takes a Claim Id as input.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Claim retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid id.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while retrieving claim.")]
        public async Task<ActionResult<Claim?>> GetAsync(int id)
        {
            Claim? claim = await claimsRepository.GetClaimByIdAsync(id);
            if (claim is null)
            {
                return NotFound();
            }
            return Ok(claim);
        }
    }
}
