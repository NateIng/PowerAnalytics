namespace PowerAnalytics.DTOs;

public class PowerReadingDto
{
    public int? Id { get; set; }
    public DateTime LoggedAt { get; set; }
    public double Value { get; set; }
}