using System;
using System.Collections.Generic;

namespace ParliamentDomain.Model;

public partial class Contact : Entity
{
    public string? Phone { get; set; }

    public string Email { get; set; } = null!;

    public string? InstagramLink { get; set; }

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}
