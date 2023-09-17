using System;
using System.Collections.Generic;

namespace HanumPay.Models;

public partial class VerificationKey
{
    public string Key { get; set; } = null!;

    public ulong? UserId { get; set; }

    public string Type { get; set; } = null!;

    public string? Department { get; set; }

    public byte? Grade { get; set; }

    public byte? Classroom { get; set; }

    public byte? Number { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ValidUntil { get; set; }

    public DateTime? UsedAt { get; set; }

    public virtual User? User { get; set; }
}
