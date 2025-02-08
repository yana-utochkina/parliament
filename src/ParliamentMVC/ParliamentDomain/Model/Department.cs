using System;
using System.Collections.Generic;

namespace ParliamentDomain.Model;

public partial class Department : Entity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int ContactId { get; set; }

    public virtual Contact Contact { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<News> News { get; set; } = new List<News>();
}
