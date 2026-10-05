using Claims.Domain.Models.Insurance;

namespace Claims.Infrastructure.Services.Validation
{
    /// <summary>
    /// Validation interface for checking if a claim or cover can be correctly added.
    /// </summary>
    public interface IValidationService
    {
        public Task<bool> ValidateClaim(Claim claim);

        public bool ValidateCover(Cover cover);
    }
}
