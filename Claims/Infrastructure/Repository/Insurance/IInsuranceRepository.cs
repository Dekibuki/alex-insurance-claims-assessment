using Claims.Domain.Models.Insurance;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Infrastructure.Repository.Insurance
{
    /// <summary>
    /// Repository interface for managing claims and covers in the database.
    /// Currently provides all CRUD operations for claims, and read operations for covers.
    /// </summary>
    public interface IInsuranceRepository
    {
        public Task<List<Claim>> GetAllClaimsAsync();

        public Task<Claim?> GetClaimByIdAsync(int id);

        public Task AddClaimAsync(Claim item);

        public Task DeleteClaimByIdAsync(int id);

        public Task<List<Cover>> GetAllCoversAsync();

        public Task<Cover?> GetCoverByIdAsync(int id);

        public Task AddCoverAsync(Cover item);

        public Task DeleteCoverByIdAsync(int id);
    }
}
