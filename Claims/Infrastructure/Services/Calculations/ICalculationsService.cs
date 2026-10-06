using Claims.Domain.Enums;

namespace Claims.Infrastructure.Services.Calculations
{
    /// <summary>
    /// Interface for insurance calculations.
    /// </summary>
    public interface ICalculationsService
    {
        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
    }
}
