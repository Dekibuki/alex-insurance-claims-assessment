using Claims.Controllers;
using Claims.Infrastructure.DataContext;
using Claims.Infrastructure.Repository.Audit;
using Claims.Infrastructure.Repository.Insurance;
using Claims.Infrastructure.Services.Audit;
using Claims.Infrastructure.Services.Calculations;
using Claims.Infrastructure.Services.Validation;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

public partial class Program 
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Start Testcontainers for SQL Server and MongoDB
        var sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new MsSqlBuilder()
                    .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                : new()

            ).Build();

        var mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .Build();

        await sqlContainer.StartAsync();
        await mongoContainer.StartAsync();

        // Add services to the container.
        builder.Services
            .AddControllers()
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddDbContext<MainContext>(options =>
            options.UseSqlServer(sqlContainer.GetConnectionString()));

        builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();
        builder.Services.AddScoped<IAuditRepository, AuditRepository>();
        builder.Services.AddScoped<IAuditService, AuditService>();
        builder.Services.AddScoped<IValidationService, ValidationService>();
        builder.Services.AddScoped<ICalculationsService, CalculationsService>();

        builder.Services.AddSingleton<AuditQueue>();
        builder.Services.AddHostedService<AuditWorker>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<MainContext>();
            context.Database.Migrate();
        }

        app.Run();
    }
}
