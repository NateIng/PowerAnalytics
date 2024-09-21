using Microsoft.AspNetCore.Mvc;
using PowerAnalytics.DTOs;
using PowerAnalytics.Models;
using PowerAnalytics.Services;

namespace PowerReadingsApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class PowerAnalyticsController : ControllerBase
    {
        private readonly IPowerAnalyticsService _service;

        public PowerAnalyticsController(IPowerAnalyticsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PowerReadingDto>>> GetPowerReadings([FromQuery] PowerReadingFilter filter)
        {
            var powerReadings = await _service.GetPowerReadingsAsync(filter);
            return Ok(powerReadings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PowerReadingDto>> GetPowerReading(int id)
        {
            var powerReading = await _service.GetPowerReadingByIdAsync(id);
            if (powerReading == null) return NotFound();
            return Ok(powerReading);
        }

        [HttpPost]
        public async Task<ActionResult<IList<PowerReadingDto>>> CreatePowerReadings(PowerReadingDto[] powerReadingDtos)
        {
            if (powerReadingDtos == null || !powerReadingDtos.Any())
            {
                return BadRequest("No power readings provided.");
            }

            var createdReadings = await _service.CreatePowerReadingsAsync(powerReadingDtos);

            // Return a 201 Created response with the list of created readings
            return CreatedAtAction(nameof(GetPowerReadings), null, createdReadings);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePowerReading(int id, PowerReadingDto powerReadingDto)
        {
            if (id != powerReadingDto.Id) return BadRequest();

            var updatedPowerReading = await _service.UpdatePowerReadingAsync(powerReadingDto);
            if (updatedPowerReading == null) return NotFound();
            return Ok(updatedPowerReading);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePowerReading(int id)
        {
            if (!await _service.DeletePowerReadingAsync(id)) return NotFound();
            return NoContent();
        }
    }
}
