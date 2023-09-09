using System;
using System.Collections.Generic;

namespace HanumPay.Models;

/// <summary>
/// 계좌
/// </summary>
public partial class Balance
{
    /// <summary>
    /// 잔고 고유 ID
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// 사용자 ID, 비즈니스의 경우 NULL
    /// </summary>
    public ulong? UserId { get; set; }

    /// <summary>
    /// 잔고 정산 후 총 잔액
    /// </summary>
    public ulong Amount { get; set; }

    /// <summary>
    /// 잔고 분류
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// 잔고 메모
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// 잔고 이름
    /// </summary>
    public string Label { get; set; } = null!;

    public virtual ICollection<Transaction> TransactionReceivers { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionSenders { get; set; } = new List<Transaction>();

    public virtual User? User { get; set; }
}
