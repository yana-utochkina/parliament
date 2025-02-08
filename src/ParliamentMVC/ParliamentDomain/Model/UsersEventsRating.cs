using System;
using System.Collections.Generic;

namespace ParliamentDomain.Model;

public partial class UsersEventsRating : Entity
{
    public int EventId { get; set; }

    public int UserId { get; set; }

    public short? Rating { get; set; }
}
