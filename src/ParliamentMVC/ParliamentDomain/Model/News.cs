using System;
using System.Collections.Generic;

namespace ParliamentDomain.Model;

public partial class News : Entity
{
    public int PublisherId { get; set; }

    public int? DepartmentId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime PublicationDate { get; set; }

    public string Description { get; set; } = null!;

    public virtual Department? Department { get; set; }
}
