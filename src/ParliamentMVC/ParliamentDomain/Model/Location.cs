using System;
using System.Collections.Generic;

namespace ParliamentDomain.Model;

public partial class Location : Entity
{
    public string Address { get; set; } = null!;

    public int ContactId { get; set; }

    public string? GoogleMapsLink { get; set; }

    public virtual Contact Contact { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
