using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PowerAnalytics.Data;
using PowerAnalytics.DTOs;
using PowerAnalytics.Models;
using PowerAnalytics.Services;

namespace PowerAnalytics.UnitTests;

[TestFixture]
public class ServiceTests
{
    IPowerAnalyticsService _sut;
    PowerAnalyticsDbContext _context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PowerAnalyticsDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new PowerAnalyticsDbContext(options);
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<PowerReading, PowerReadingDto>().ReverseMap();
        });
        var mapper = config.CreateMapper();

        _sut = new PowerAnalyticsService(_context, mapper);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetPowerReadingsAsync_ShouldReturnEmptyList_WhenNoReadingsExist()
    {
        // Arrange
        // No setup needed as we're testing the empty state

        // Act
        var result = await _sut.GetPowerReadingsAsync(new PowerReadingFilter());

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetPowerReadingsAsync_ShouldReturnSingleReading_WhenSingleReadingExists()
    {
        // Arrange
        var reading = new PowerReading()
        {
            LoggedAt = DateTime.UtcNow,
            Value = 100_914
        };
        _context.Add(reading);
        _context.SaveChanges();

        // Act
        var result = await _sut.GetPowerReadingsAsync(new PowerReadingFilter());

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetPowerReadingsAsync_ShouldReturnList_WhenManyReadingsExist()
    {
        // Arrange
        var reading1 = new PowerReading() { LoggedAt = DateTime.UtcNow, Value = 100_914 };
        var reading2 = new PowerReading() { LoggedAt = DateTime.UtcNow, Value = 111_111 };
        var readings = new List<PowerReading>() { reading1, reading2 };
        _context.AddRange(readings);
        _context.SaveChanges();

        // Act
        var result = await _sut.GetPowerReadingsAsync(new PowerReadingFilter());

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetPowerReadingsAsync_ShouldReturnSubsetList_WhenReadingsFilteredByStartDate()
    {
        // Arrange
        var reading1 = new PowerReading() { LoggedAt = DateTime.UtcNow, Value = 100_914 };
        var reading2 = new PowerReading() { LoggedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(10)), Value = 111_111 };
        var reading3 = new PowerReading() { LoggedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(20)), Value = 111_222 };
        var readings = new List<PowerReading>() { reading1, reading2, reading3 };
        _context.AddRange(readings);
        _context.SaveChanges();
        var filter = new PowerReadingFilter
        {
            StartDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(11))
        };

        // Act
        var result = await _sut.GetPowerReadingsAsync(filter);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetPowerReadingsAsync_ShouldReturnSubsetList_WhenReadingsFilteredByEndDate()
    {
        // Arrange
        var reading1 = new PowerReading() { LoggedAt = DateTime.UtcNow, Value = 100_914 };
        var reading2 = new PowerReading() { LoggedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(10)), Value = 111_111 };
        var reading3 = new PowerReading() { LoggedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(20)), Value = 111_222 };
        var readings = new List<PowerReading>() { reading1, reading2, reading3 };
        _context.AddRange(readings);
        _context.SaveChanges();
        var filter = new PowerReadingFilter
        {
            EndDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(11))
        };

        // Act
        var result = await _sut.GetPowerReadingsAsync(filter);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetPowerReadingsAsync_ShouldReturnSubsetList_WhenReadingsFilteredByMinValue()
    {
        // Arrange
        var reading1 = new PowerReading() { LoggedAt = DateTime.UtcNow, Value = 100_914 };
        var reading2 = new PowerReading() { LoggedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(10)), Value = 111_111 };
        var reading3 = new PowerReading() { LoggedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(20)), Value = 111_222 };
        var readings = new List<PowerReading>() { reading1, reading2, reading3 };
        _context.AddRange(readings);
        _context.SaveChanges();
        var filter = new PowerReadingFilter
        {
            MinValue = 101_000
        };

        // Act
        var result = await _sut.GetPowerReadingsAsync(filter);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetPowerReadingsAsync_ShouldReturnSubsetList_WhenReadingsFilteredByMaxValue()
    {
        // Arrange
        var reading1 = new PowerReading() { LoggedAt = DateTime.UtcNow, Value = 100_914 };
        var reading2 = new PowerReading() { LoggedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(10)), Value = 111_111 };
        var reading3 = new PowerReading() { LoggedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(20)), Value = 111_222 };
        var readings = new List<PowerReading>() { reading1, reading2, reading3 };
        _context.AddRange(readings);
        _context.SaveChanges();
        var filter = new PowerReadingFilter
        {
            MaxValue = 101_000
        };

        // Act
        var result = await _sut.GetPowerReadingsAsync(filter);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetPowerReadingsAsync_ShouldReturnSubsetList_WhenReadingsFilteredByDateAndValue()
    {
        // Arrange
        var reading1 = new PowerReading() { LoggedAt = DateTime.UtcNow, Value = 100_914 };
        var reading2 = new PowerReading() { LoggedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(10)), Value = 111_111 };
        var reading3 = new PowerReading() { LoggedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(20)), Value = 111_222 };
        var readings = new List<PowerReading>() { reading1, reading2, reading3 };
        _context.AddRange(readings);
        _context.SaveChanges();
        var filter = new PowerReadingFilter
        {
            MinValue = 101_000,
            StartDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(11))
        };

        // Act
        var result = await _sut.GetPowerReadingsAsync(filter);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(reading2.LoggedAt, Is.EqualTo(result.Single().LoggedAt));
            Assert.That(reading2.Value, Is.EqualTo(result.Single().Value));
        });
    }

    [Test]
    public async Task CreatePowerReadingAsync_SingleReading_ShouldReturnCreatedReading()
    {
        // Arrange
        var reading = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 100_914 };

        // Act
        var result = await _sut.CreatePowerReadingsAsync(new List<PowerReadingDto>() { reading });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].LoggedAt, Is.EqualTo(reading.LoggedAt));
            Assert.That(result[0].Value, Is.EqualTo(reading.Value));
        });
    }

    [Test]
    public async Task CreatePowerReadingAsync_SingleReading_ShouldPersistReadingToDatabase()
    {
        // Arrange
        var reading = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 100_914 };

        // Act
        _ = await _sut.CreatePowerReadingsAsync(new List<PowerReadingDto>() { reading });

        // Assert
        var actualReadings = await _context.PowerReadings.ToListAsync();
        Assert.That(actualReadings, Is.Not.Empty);
        Assert.That(actualReadings, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(actualReadings[0].Id, Is.EqualTo(1));
            Assert.That(actualReadings[0].LoggedAt, Is.EqualTo(reading.LoggedAt));
            Assert.That(actualReadings[0].Value, Is.EqualTo(reading.Value));
        });
    }

    [Test]
    public async Task CreatePowerReadingAsync_ShouldReturnCreatedReadings_WhenManyReadingsExist()
    {
        // Arrange
        var reading1 = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 100_914 };
        var reading2 = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 100_999 };
        var expectedReadings = new List<PowerReadingDto>() { reading1, reading2 };

        // Act
        var actualReadings = await _sut.CreatePowerReadingsAsync(expectedReadings);

        // Assert
        Assert.That(actualReadings, Is.Not.Empty);
        Assert.That(actualReadings, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task CreatePowerReadingAsync_ManyReadings_ShouldPersistReadingsToDatabase()
    {
        // Arrange
        var reading1 = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 100_914 };
        var reading2 = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 100_999 };
        var expectedReadings = new List<PowerReadingDto>() { reading1, reading2 };

        // Act
        _ = await _sut.CreatePowerReadingsAsync(expectedReadings);

        // Assert
        var actualReadings = await _context.PowerReadings.ToListAsync();
        Assert.That(actualReadings, Is.Not.Empty);
        Assert.That(actualReadings, Has.Count.EqualTo(2));
    }

    [Test]
    public void CreatePowerReadingAsync_NullReadingsList_ShouldReturnArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _sut.CreatePowerReadingsAsync(null!));
    }

    [Test]
    public void CreatePowerReadingAsync_ShouldReturnArgumentException_WhenNoReadingsExist()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _sut.CreatePowerReadingsAsync(new List<PowerReadingDto>()));
    }

    [Test]
    public async Task GetPowerReadingByIdAsync_ShouldReturnReading_WhenReadingMatchExists()
    {
        // Arrange
        var reading = new PowerReading()
        {
            LoggedAt = DateTime.UtcNow,
            Value = 100_914
        };
        _context.Add(reading);
        _context.SaveChanges();

        // Act
        var result = await _sut.GetPowerReadingByIdAsync(1);

        // Assert
        Assert.That(result!.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task GetPowerReadingByIdAsync_ShouldReturnNull_WhenNoReadingMatchExists()
    {
        // Arrange
        // No setup needed as we're testing the empty state

        // Act
        var result = await _sut.GetPowerReadingByIdAsync(1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UpdatePowerReadingAsync_ShouldReturnUpdatedReading_WhenReadingIsUpdated()
    {
        // Arrange
        var reading = new PowerReading()
        {
            LoggedAt = DateTime.UtcNow,
            Value = 100_914
        };
        _context.Add(reading);
        _context.SaveChanges();
        var updateReading = new PowerReadingDto()
        {
            Id = 1,
            LoggedAt = DateTime.UtcNow,
            Value = 45
        };

        // Act
        var result = await _sut.UpdatePowerReadingAsync(updateReading);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(updateReading.Id));
            Assert.That(result!.LoggedAt, Is.EqualTo(updateReading.LoggedAt));
            Assert.That(result!.Value, Is.EqualTo(updateReading.Value));
        });
    }

    [Test]
    public async Task UpdatePowerReadingAsync_ShouldReturnNull_WhenNoReadingMatchExists()
    {
        // Arrange
        var updateReading = new PowerReadingDto()
        {
            Id = 1,
            LoggedAt = DateTime.UtcNow,
            Value = 45
        };

        // Act
        var result = await _sut.UpdatePowerReadingAsync(updateReading);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeletePowerReadingAsync_ShouldDeleteReading_WhenReadingExists()
    {
        var reading = new PowerReading()
        {
            LoggedAt = DateTime.UtcNow,
            Value = 100_914
        };
        _context.Add(reading);
        _context.SaveChanges();

        var result = await _sut.DeletePowerReadingAsync(1);
        Assert.That(result);
        Assert.That(_context.PowerReadings, Is.Empty);
    }

    [Test]
    public async Task DeletePowerReadingAsync_ShouldReturnFalse_WhenNoReadingMatchExists()
    {
        var reading = new PowerReading()
        {
            LoggedAt = DateTime.UtcNow,
            Value = 100_914
        };
        _context.Add(reading);
        _context.SaveChanges();

        var result = await _sut.DeletePowerReadingAsync(10);
        Assert.Multiple(() =>
        {
            Assert.That(!result);
            Assert.That(_context.PowerReadings, Is.Not.Empty);
        });
    }
}