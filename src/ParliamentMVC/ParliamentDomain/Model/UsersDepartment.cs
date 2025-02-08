using System;
using System.Collections.Generic;

namespace ParliamentDomain.Model;

public partial class UsersDepartment : Entity
{
    public int WorkerId { get; set; }

    public int DepartmentId { get; set; }

    public string Position { get; set; } = null!;

    public DateOnly JoinedAt { get; set; }
}
