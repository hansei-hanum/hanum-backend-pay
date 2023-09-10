using System;
using System.Collections.Generic;

namespace HanumPay.Models;

public partial class User
{
    public ulong Id { get; set; }

    public string Phone { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Profile { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual EoullimBalance? EoullimBalance { get; set; }

    public virtual ICollection<EoullimPayment> EoullimPayments { get; set; } = new List<EoullimPayment>();
}
