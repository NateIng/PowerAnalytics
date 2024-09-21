namespace PowerAnalytics.Models;

public class PowerReadingFilter
{
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? MinValue { get; set; }

    public int? MaxValue { get; set; }
}
