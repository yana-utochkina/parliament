namespace ParliamentInfrastructure.ViewModels;

public class EventRatingViewModel
{
    public int EventId { get; set; }
    public string? EventTitle { get; set; }
    public DateTime EventDate { get; set; }
    public double? AverageRating { get; set; }
    public int RatingCount { get; set; }
    public short UserRating { get; set; }
}

