using System;
using System.Collections.Generic;

namespace ParliamentDomain.Model;

public partial class UsersEvent : Entity
{
    public int EventId { get; set; }

    public int UserId { get; set; }

    public string Role { get; set; } = null!;

    public short? Rating { get; set; }
}
