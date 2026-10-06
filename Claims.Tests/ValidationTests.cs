using Microsoft.AspNetCore.Mvc.Testing;
using Claims.Domain.Models.Insurance;
using Claims.Domain.Enums;
using Claims.Domain.Exceptions;
using Xunit;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Claims.Infrastructure.Services.Validation;
using Claims.Infrastructure.DataContext;

namespace Claims.Tests
{
    /// <summary>
    /// Cover validation tests for the validation service in the Claims API involving managing claims and covers.
    /// </summary>
    public class ValidationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly DateTime StartCoverDateTime = DateTime.Parse("2026-11-05T18:27:56.113Z");
        private readonly DateTime EndCoverDateTime = DateTime.Parse("2026-12-05T18:27:56.113Z");

        private const int MaxDamageCost = 100000;
        private const int MaxCoverDurationDays = 365;

        private readonly WebApplicationFactory<Program> application;

        public ValidationTests(WebApplicationFactory<Program> application)
        {
            this.application = application;
        }

        [Fact]
        public async Task ValidateClaim_FailsDueToExceededDamageCost()
        {
            string expectedMessage = $"Damage cost exceeds the maximum - {MaxDamageCost}";

            Claim claimToValidate = new Claim
            {
                CoverId = 1,
                Created = DateTime.UtcNow,
                Name = "Test Claim",
                Type = ClaimType.Fire,
                DamageCost = 100000000.00m
            };

            using (IServiceScope scope = application.Services.CreateScope())
            {
                IValidationService validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();

                BadValidationException exception = await Assert.ThrowsAsync<BadValidationException>(async () => await validationService.ValidateClaim(claimToValidate));

                Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
                Assert.Equal(expectedMessage, exception.Message);
            }
        }

        [Theory]
        [InlineData(7)]
        [InlineData(-7)]
        public async Task ValidateClaim_FailsDueToCreatedBeingOutsideCoverPeriod(int monthsToAdd)
        {
            string expectedMessage = $"Claim date is outside the cover period - {StartCoverDateTime} to {EndCoverDateTime}";

            Claim claimToValidate = new Claim
            {
                CoverId = 1,
                Created = DateTime.UtcNow.AddMonths(monthsToAdd),
                Name = "Test Claim",
                Type = ClaimType.Fire,
                DamageCost = 100.00m
            };

            using (IServiceScope scope = application.Services.CreateScope())
            {
                MainContext context = scope.ServiceProvider.GetRequiredService<MainContext>();
                await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

                context.Covers.Add(new Cover
                {
                    StartDate = StartCoverDateTime,
                    EndDate = EndCoverDateTime,
                    Type = CoverType.Yacht,
                    Premium = 1000.00m
                });

                await context.SaveChangesAsync(TestContext.Current.CancellationToken);

                IValidationService validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();

                BadValidationException exception = await Assert.ThrowsAsync<BadValidationException>(async () => await validationService.ValidateClaim(claimToValidate));

                Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
                Assert.Equal(expectedMessage, exception.Message);
            }
        }

        [Fact]
        public async Task ValidateCover_FailsDueToStartDateInThePast()
        {
            DateTime pastStartDate = StartCoverDateTime.AddMonths(-6);
            string expectedMessage = $"Cover start date is in the past - {pastStartDate}";

            Cover coverToValidate = new Cover
            {
                StartDate = pastStartDate,
                EndDate = EndCoverDateTime,
                Type = CoverType.Yacht,
                Premium = 1000.00m
            };

            using (IServiceScope scope = application.Services.CreateScope())
            {
                IValidationService validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();

                BadValidationException exception = Assert.Throws<BadValidationException>(() => validationService.ValidateCover(coverToValidate));
                Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
                Assert.Equal(expectedMessage, exception.Message);
            }
        }

        [Fact]
        public async Task ValidateCover_FailsDueToPeriodBeingTooLong()
        {
            DateTime longEndDate = EndCoverDateTime.AddYears(1);
            string expectedMessage = $"Cover duration exceeds the maximum - {MaxCoverDurationDays} days";

            Cover coverToValidate = new Cover
            {
                StartDate = StartCoverDateTime,
                EndDate = longEndDate,
                Type = CoverType.Yacht,
                Premium = 1000.00m
            };

            using (IServiceScope scope = application.Services.CreateScope())
            {
                IValidationService validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();

                BadValidationException exception = Assert.Throws<BadValidationException>(() => validationService.ValidateCover(coverToValidate));
                Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
                Assert.Equal(expectedMessage, exception.Message);
            }
        }
    }
}
