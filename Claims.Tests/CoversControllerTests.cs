using Azure;
using Claims.Domain.Enums;
using Claims.Domain.Models.Insurance;
using Claims.Infrastructure.DataContext;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Claims.Tests
{
    public class CoversControllerTests
    {
        private readonly WebApplicationFactory<Program> application;

        private readonly DateTime StartCoverDateTime = DateTime.Parse("2026-11-05T18:27:56.113Z");
        private readonly DateTime EndCoverDateTime = DateTime.Parse("2026-12-05T18:27:56.113Z");

        public CoversControllerTests()
        {
            this.application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                { });
        }

        [Fact]
        public async Task CreateCover()
        {
            await InitializeTestingData();

            Cover coverToInsert = new Cover
            {
                StartDate = DateTime.Now.AddMonths(1),
                EndDate = DateTime.Now.AddMonths(2),
                Type = CoverType.Yacht,
                Premium = 1000
            };

            HttpClient client = application.CreateClient();

            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/covers",
                coverToInsert,
                TestContext.Current.CancellationToken);


            string json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());

            int? coverId = JsonSerializer.Deserialize<int?>(json, options);

            response.EnsureSuccessStatusCode();
            Assert.NotNull(coverId);
        }

        [Fact]
        public async Task GetCovers()
        {
            await InitializeTestingData();

            HttpClient client = application.CreateClient();

            HttpResponseMessage response = await client.GetAsync("api/covers", TestContext.Current.CancellationToken);

            string json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            response.EnsureSuccessStatusCode();

            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());

            List<Cover>? covers = JsonSerializer.Deserialize<List<Cover>>(json, options);

            Assert.NotNull(covers);
            Assert.NotEmpty(covers);
        }


        [Fact]
        public async Task GetCoverById()
        {
            await InitializeTestingData();

            HttpClient client = application.CreateClient();

            int coverId = 1;

            HttpResponseMessage response = await client.GetAsync($"api/covers/{coverId}", TestContext.Current.CancellationToken);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());

            Cover? cover = JsonSerializer.Deserialize<Cover>(json, options);

            Assert.NotNull(cover);
            Assert.Equal(coverId, cover.Id);
        }

        [Fact]
        public async Task DeleteCover()
        {
            await InitializeTestingData();

            HttpClient client = application.CreateClient();

            int coverToBeDeletedId = 1;

            HttpResponseMessage response = await client.DeleteAsync(
                $"api/covers/{coverToBeDeletedId}",
                TestContext.Current.CancellationToken);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(CoverType.Yacht, 41250)]
        [InlineData(CoverType.PassengerShip, 45000)]
        [InlineData(CoverType.ContainerShip, 48750)]
        [InlineData(CoverType.BulkCarrier, 48750)]
        [InlineData(CoverType.Tanker, 56250)]
        public async Task ComputePremium(CoverType coverType, decimal premium)
        {
            await InitializeTestingData();

            HttpClient client = application.CreateClient();

            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/covers/compute",
                new
                {
                    startDate = StartCoverDateTime,
                    endDate = EndCoverDateTime,
                    type = coverType.ToString()
                },
                TestContext.Current.CancellationToken);

            string json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            response.EnsureSuccessStatusCode();

            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());

            decimal? premiumResult = JsonSerializer.Deserialize<decimal>(json);

            Assert.Equal(premium, premiumResult);
        }

        private async Task InitializeTestingData()
        {
            using (IServiceScope scope = application.Services.CreateScope())
            {
                MainContext context = scope.ServiceProvider.GetRequiredService<MainContext>();
                await context.Database.EnsureCreatedAsync();

                context.Covers.Add(new Cover { StartDate = StartCoverDateTime, EndDate = EndCoverDateTime, Type = CoverType.Tanker, Premium = 1000 });
                context.Covers.Add(new Cover { StartDate = StartCoverDateTime, EndDate = EndCoverDateTime, Type = CoverType.Yacht, Premium = 2000 });
                await context.SaveChangesAsync();
            }
        }
    }
}
