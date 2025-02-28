namespace ParliamentDomain.Model;

public partial class UsersEventsRole : Entity
{
    public int EventId { get; set; }

    public int UserId { get; set; }

    public string Role { get; set; } = null!;
}
