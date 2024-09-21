using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PowerAnalytics.Data;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PowerAnalytics.Models;

namespace PowerAnalytics.IntegrationTests;

[TestFixture]
public class IntegrationTests
{
    private HttpClient _client;
    private PowerAnalyticsDbContext _context;

    [SetUp]
    public void Setup()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PowerAnalyticsDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<PowerAnalyticsDbContext>(options =>
                        options.UseInMemoryDatabase("TestDatabase"));
                });
            });

        _client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<PowerAnalyticsDbContext>();
    }

    [Test]
    public async Task CreatePowerReading_ShouldCreateDbReading_WhenNewReadingIsPosted()
    {
        // Arrange
        var newReading = new
        {
            Value = 123,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/poweranalytics", new object[] { newReading });

        // Assert
        Assert.That(response.IsSuccessStatusCode);

        var powerReadings = await _context.PowerReadings.ToListAsync();
        Assert.That(powerReadings, Has.Count.EqualTo(1));
        Assert.That(powerReadings[0].Value, Is.EqualTo(newReading.Value));
    }
}