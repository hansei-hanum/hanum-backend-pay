﻿using System;
using System.Collections.Generic;

namespace Hanum.Pay.Models;

public partial class User {
    public ulong Id { get; set; }

    public string Phone { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Profile { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual EoullimBalance? EoullimBalance { get; set; }

    public virtual ICollection<EoullimPayment> EoullimPayments { get; set; } = new List<EoullimPayment>();

    public virtual ICollection<VerificationKey> VerificationKeys { get; set; } = new List<VerificationKey>();
}
