using PowerAnalytics.DTOs;
using PowerAnalytics.Models;

namespace PowerAnalytics.Services;

public interface IPowerAnalyticsService
{
    Task<IList<PowerReadingDto>> GetPowerReadingsAsync(PowerReadingFilter filter);
    Task<PowerReadingDto?> GetPowerReadingByIdAsync(int id);
    Task<IList<PowerReadingDto>> CreatePowerReadingsAsync(IEnumerable<PowerReadingDto> powerReadingDtos);
    Task<PowerReadingDto?> UpdatePowerReadingAsync(PowerReadingDto powerReadingDto);
    Task<bool> DeletePowerReadingAsync(int id);
}
