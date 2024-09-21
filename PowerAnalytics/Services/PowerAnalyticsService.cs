using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PowerAnalytics.Data;
using PowerAnalytics.DTOs;
using PowerAnalytics.Models;

namespace PowerAnalytics.Services;

public class PowerAnalyticsService : IPowerAnalyticsService
{
    private readonly PowerAnalyticsDbContext _context;
    private readonly IMapper _mapper;

    public PowerAnalyticsService(PowerAnalyticsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IList<PowerReadingDto>> GetPowerReadingsAsync(PowerReadingFilter filter)
    {
        var query = _context.PowerReadings.AsQueryable();

        if (filter.StartDate.HasValue)
            query = query.Where(r => r.LoggedAt >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(r => r.LoggedAt <= filter.EndDate.Value);

        if (filter.MinValue.HasValue)
            query = query.Where(r => r.Value >= filter.MinValue.Value);

        if (filter.MaxValue.HasValue)
            query = query.Where(r => r.Value <= filter.MaxValue.Value);
        
        var powerReadings = await query.ToListAsync();
        return _mapper.Map<IList<PowerReadingDto>>(powerReadings);
    }

    public async Task<PowerReadingDto?> GetPowerReadingByIdAsync(int id)
    {
        var powerReading = await _context.PowerReadings.FindAsync(id);
        return _mapper.Map<PowerReadingDto>(powerReading);
    }

    public async Task<IList<PowerReadingDto>> CreatePowerReadingsAsync(IEnumerable<PowerReadingDto> powerReadingDtos)
    {
        if (powerReadingDtos == null)
            throw new ArgumentNullException(nameof(powerReadingDtos), "should not be null");

        if (!powerReadingDtos.Any())
            throw new ArgumentException("should not be empty", nameof(powerReadingDtos));

        var powerReadings = _mapper.Map<List<PowerReading>>(powerReadingDtos);
        _context.PowerReadings.AddRange(powerReadings);
        await _context.SaveChangesAsync();
        return _mapper.Map<List<PowerReadingDto>>(powerReadings);
    }

    public async Task<PowerReadingDto?> UpdatePowerReadingAsync(PowerReadingDto powerReadingDto)
    {
        var powerReading = await _context.PowerReadings.FindAsync(powerReadingDto.Id);
        if (powerReading == null) return null;

        _mapper.Map(powerReadingDto, powerReading);
        await _context.SaveChangesAsync();
        return _mapper.Map<PowerReadingDto>(powerReading);
    }

    public async Task<bool> DeletePowerReadingAsync(int id)
    {
        var powerReading = await _context.PowerReadings.FindAsync(id);
        if (powerReading != null)
        {
            _context.PowerReadings.Remove(powerReading);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}