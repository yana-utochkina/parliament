using System;
using System.Collections.Generic;

namespace ParliamentDomain.Model;

public partial class Event : Entity
{
    public int? LocationId { get; set; }

    public int? DepartmentId { get; set; }

    public string Title { get; set; } = null!;

    public string AccessType { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Description { get; set; } = null!;

    public virtual Department? Department { get; set; }

    public virtual Location? Location { get; set; }
}
