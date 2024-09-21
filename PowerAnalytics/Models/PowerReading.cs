namespace PowerAnalytics.Models;

public class PowerReading
{
    public int Id { get; set; }
    public DateTime LoggedAt { get; set; }
    public double Value { get; set; }
}