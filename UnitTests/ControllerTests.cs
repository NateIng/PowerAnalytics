using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PowerAnalytics.DTOs;
using PowerAnalytics.Models;
using PowerAnalytics.Services;
using PowerReadingsApi.Controllers;

namespace PowerAnalytics.UnitTests;

[TestFixture]
public class ControllerTests
{
    [Test]
    public async Task GetPowerReadings_ShouldReturnEmptyList_WhenNoReadingsExist()
    {
        // Arrange
        var mockService = new Mock<IPowerAnalyticsService>();
        mockService
            .Setup(s => s.GetPowerReadingsAsync(It.IsAny<PowerReadingFilter>()))
            .ReturnsAsync(new List<PowerReadingDto>());
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.GetPowerReadings(new PowerReadingFilter());

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var value = okResult!.Value as IEnumerable<PowerReadingDto>;
        Assert.That(value, Is.Empty);
    }

    [Test]
    public async Task GetPowerReadings_ShouldReturnList_WhenReadingsExist()
    {
        // Arrange
        var readings = new List<PowerReadingDto>()
        {
            new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 1 },
            new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 2 },
        };
        var mockService = new Mock<IPowerAnalyticsService>();
        mockService
            .Setup(s => s.GetPowerReadingsAsync(It.IsAny<PowerReadingFilter>()))
            .ReturnsAsync(readings);
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.GetPowerReadings(new PowerReadingFilter());

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var value = okResult!.Value as IEnumerable<PowerReadingDto>;
        Assert.That(value!.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetPowerReading_ShouldReturnNotFound_WhenNoReadingMatchExists()
    {
        // Arrange
        var mockService = new Mock<IPowerAnalyticsService>();
        mockService
            .Setup(s => s.GetPowerReadingByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(null as PowerReadingDto);
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.GetPowerReading(3);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
            Assert.That(result.Value, Is.Null);
        });
    }

    [Test]
    public async Task GetPowerReading_ShouldReturnReading_WhenReadingMatchExists()
    {
        // Arrange
        var expectedReading = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 1 };
        var mockService = new Mock<IPowerAnalyticsService>();
        mockService
            .Setup(s => s.GetPowerReadingByIdAsync(1))
            .ReturnsAsync(expectedReading);
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.GetPowerReading(1);

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okObject = (result.Result as OkObjectResult);
        Assert.That(okObject!.Value, Is.EqualTo(expectedReading));
    }

    [Test]
    public async Task CreatePowerReadings_ShouldReturnNewReading_WhenNewReadingCreated()
    {
        // Arrange
        var createReading = new PowerReadingDto()
        {
            LoggedAt = DateTime.UtcNow,
            Value = 10
        };
        var newReadings = new PowerReadingDto[] { createReading };
        var returnReading = new PowerReadingDto() 
        {
            LoggedAt = createReading.LoggedAt, 
            Value = createReading.Value, 
            Id = 1
        };
        var returnReadings = new List<PowerReadingDto>() { returnReading };
        var mockService = new Mock<IPowerAnalyticsService>();
        mockService
            .Setup(s => s.CreatePowerReadingsAsync(newReadings))
            .ReturnsAsync(returnReadings);
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.CreatePowerReadings(newReadings);

        // Assert
        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var actualList = (result.Result as CreatedAtActionResult)!.Value as IList<PowerReadingDto>;
        CollectionAssert.AreEquivalent(actualList, returnReadings);
    }

    [Test]
    public async Task CreatePowerReadings_ShouldReturnNewReadingList_WhenManyReadingsCreated()
    {
        // Arrange
        var createReading1 = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 10 };
        var createReading2 = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 20 };
        var newReadings = new PowerReadingDto[] { createReading1, createReading2 };
        var returnReading1 = new PowerReadingDto()
        {
            LoggedAt = createReading1.LoggedAt,
            Value = createReading1.Value,
            Id = 1
        };
        var returnReading2 = new PowerReadingDto()
        {
            LoggedAt = createReading2.LoggedAt,
            Value = createReading2.Value,
            Id = 2
        };
        var returnReadings = new List<PowerReadingDto>() { returnReading1, returnReading2 };
        var mockService = new Mock<IPowerAnalyticsService>();
        mockService
            .Setup(s => s.CreatePowerReadingsAsync(newReadings))
            .ReturnsAsync(returnReadings);
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.CreatePowerReadings(newReadings);

        // Assert
        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var actualList = (result.Result as CreatedAtActionResult)!.Value as IList<PowerReadingDto>;
        CollectionAssert.AreEquivalent(actualList, returnReadings);
    }

    [Test]
    public async Task CreatePowerReadings_ShouldReturnBadRequest_WhenEmptyReadingListProvided()
    {
        // Arrange
        var mockService = new Mock<IPowerAnalyticsService>();
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.CreatePowerReadings(new PowerReadingDto[] { });

        // Assert
        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
    }


    [Test]
    public async Task CreatePowerReadings_ShouldReturnBadRequest_WhenNullReadingListProvided()
    {
        // Arrange
        var mockService = new Mock<IPowerAnalyticsService>();
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.CreatePowerReadings(null!);

        // Assert
        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdatePowerReadings_ShouldReturnUpdatedReading_WhenMatchingReadingExists()
    {
        // Arrange
        var updateReading = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 20, Id = 1 };
        var returnReading = new PowerReadingDto() { LoggedAt = updateReading.LoggedAt, Value = updateReading.Value, Id = updateReading.Id };
        var mockService = new Mock<IPowerAnalyticsService>();
        mockService
            .Setup(s => s.UpdatePowerReadingAsync(updateReading))
            .ReturnsAsync(returnReading);
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.UpdatePowerReading(updateReading.Id.Value, updateReading);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var actualReading = (result as OkObjectResult)!.Value as PowerReadingDto;
        Assert.Multiple(() =>
        {
            Assert.That(actualReading?.Id, Is.EqualTo(returnReading.Id));
            Assert.That(actualReading?.LoggedAt, Is.EqualTo(returnReading.LoggedAt));
            Assert.That(actualReading?.Value, Is.EqualTo(returnReading.Value));
        });
    }

    [Test]
    public async Task UpdatePowerReadings_ShouldReturnBadRequest_WhenProvidedIdsConflict()
    {
        // Arrange
        var updateReading = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 20, Id = 1 };
        var mockService = new Mock<IPowerAnalyticsService>();
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.UpdatePowerReading(2, updateReading);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task UpdatePowerReadings_ShouldReturnNotFound_WhenNoReadingMatchExists()
    {
        // Arrange
        var updateReading = new PowerReadingDto() { LoggedAt = DateTime.UtcNow, Value = 20, Id = 1 };
        var mockService = new Mock<IPowerAnalyticsService>();
        mockService
            .Setup(s => s.UpdatePowerReadingAsync(updateReading))
            .ReturnsAsync(null as PowerReadingDto);
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.UpdatePowerReading(updateReading.Id.Value, updateReading);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task DeletePowerReading_ShouldReturnNoContent_WhenReadingDeleted()
    {
        // Arrange
        var mockService = new Mock<IPowerAnalyticsService>();
        mockService
            .Setup(s => s.DeletePowerReadingAsync(It.IsAny<int>()))
            .ReturnsAsync(true);
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.DeletePowerReading(1);

        // Assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task DeletePowerReading_ShouldReturnNotFound_WhenNoReadingMatchExists()
    {
        // Arrange
        var mockService = new Mock<IPowerAnalyticsService>();
        mockService
            .Setup(s => s.DeletePowerReadingAsync(It.IsAny<int>()))
            .ReturnsAsync(false);
        var controller = new PowerAnalyticsController(mockService.Object);

        // Act
        var result = await controller.DeletePowerReading(1);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }
}
