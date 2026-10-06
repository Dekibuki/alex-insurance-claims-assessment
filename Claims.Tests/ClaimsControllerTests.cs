using Claims.Domain.Enums;
using Claims.Domain.Models.Insurance;
using Claims.Infrastructure.DataContext;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Claims.Tests
{
    public class ClaimsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> application;

        public ClaimsControllerTests(WebApplicationFactory<Program> application)
        {
            this.application = application;
        }

        [Fact]
        public async Task CreateClaim()
        {
            await InitializeTestingData();

            Claim claimToInsert = new Claim
            {
                CoverId = 1,
                Created = DateTime.UtcNow,
                Name = "Test Claim",
                Type = ClaimType.Fire,
                DamageCost = 2000.00m
            };

            HttpClient client = application.CreateClient();

            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/claims",
                claimToInsert,
                TestContext.Current.CancellationToken);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());

            int? claimId = JsonSerializer.Deserialize<int>(json, options);

            Assert.NotNull(claimId);
        }

        [Fact]
        public async Task GetClaims()
        {
            await InitializeTestingData();

            HttpClient client = application.CreateClient();

            HttpResponseMessage response = await client.GetAsync("api/claims", TestContext.Current.CancellationToken);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());

            List<Claim>? claims = JsonSerializer.Deserialize<List<Claim>>(json, options);

            Assert.NotNull(claims);
            Assert.NotEmpty(claims);
        }


        [Fact]
        public async Task GetClaimById()
        {
            await InitializeTestingData();

            HttpClient client = application.CreateClient();

            int claimId = 1;

            HttpResponseMessage response = await client.GetAsync($"api/claims/{claimId}", TestContext.Current.CancellationToken);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());

            Claim? claim = JsonSerializer.Deserialize<Claim>(json, options);

            Assert.NotNull(claim);
            Assert.Equal(claimId, claim.Id);
        }

        [Fact]
        public async Task DeleteClaim()
        {
            await InitializeTestingData();

            HttpClient client = application.CreateClient();

            int claimToBeDeletedId = 1;

            HttpResponseMessage response = await client.DeleteAsync(
                $"api/claims/{claimToBeDeletedId}",
                TestContext.Current.CancellationToken);

            response.EnsureSuccessStatusCode();
        }

        private async Task InitializeTestingData()
        {
            using (IServiceScope scope = application.Services.CreateScope())
            {
                MainContext context = scope.ServiceProvider.GetRequiredService<MainContext>();
                await context.Database.EnsureCreatedAsync();

                context.Claims.Add(new Claim { CoverId = 2, Created = DateTime.Now, Name = "Random", Type = ClaimType.Fire, DamageCost = 1000 });
                context.Claims.Add(new Claim { CoverId = 3, Created = DateTime.Now, Name = "Random", Type = ClaimType.BadWeather, DamageCost = 1000 });
                await context.SaveChangesAsync();
            }
        }
    }
}
