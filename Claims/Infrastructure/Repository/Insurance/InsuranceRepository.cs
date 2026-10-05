using Claims.Domain.Models.Insurance;
using Claims.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repository.Insurance
{
    /// <summary>
    /// Repository class for managing claims in the database.
    /// Currently provides all CRUD operations for claims.
    /// </summary>
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly InsuranceContext _claimsContext;

        public InsuranceRepository(InsuranceContext claimsContext)
        {
            _claimsContext = claimsContext;
        }

        public async Task<List<Claim>> GetAllClaimsAsync()
        {
            return await _claimsContext.Claims.ToListAsync();
        }

        public async Task<Claim?> GetClaimByIdAsync(string id)
        {
            return await _claimsContext.Claims
                .Where(claim => claim.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task AddClaimAsync(Claim item)
        {
            _claimsContext.Claims.Add(item);
            await _claimsContext.SaveChangesAsync();
        }

        public async Task DeleteClaimByIdAsync(string id)
        {
            Claim? claim = await GetClaimByIdAsync(id);

            if (claim is not null)
            {
                _claimsContext.Claims.Remove(claim);
                await _claimsContext.SaveChangesAsync();
            }
        }

        public async Task<List<Cover>> GetAllCoversAsync()
        {
            return await _claimsContext.Covers.ToListAsync();
        }

        public async Task<Cover?> GetCoverByIdAsync(string id)
        {
            return await _claimsContext.Covers
                .Where(cover => cover.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task AddCoverAsync(Cover item)
        {
            _claimsContext.Covers.Add(item);
            await _claimsContext.SaveChangesAsync();
        }

        public async Task DeleteCoverByIdAsync(string id)
        {
            Cover? cover = await GetCoverByIdAsync(id);

            if (cover is not null)
            {
                _claimsContext.Covers.Remove(cover);
                await _claimsContext.SaveChangesAsync();
            }
        }
    }
}
