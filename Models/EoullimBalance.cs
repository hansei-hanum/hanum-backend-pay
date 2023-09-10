using System;
using System.Collections.Generic;

namespace HanumPay.Models;

/// <summary>
/// 한세어울림한마당 잔고
/// </summary>
public partial class EoullimBalance
{
    /// <summary>
    /// 잔고 고유 ID
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// 사용자 ID
    /// </summary>
    public ulong? UserId { get; set; }

    /// <summary>
    /// 부스 ID
    /// </summary>
    public ulong? BoothId { get; set; }

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

    public virtual EoullimBooth? Booth { get; set; }

    public virtual ICollection<EoullimPayment> EoullimPaymentBoothBalances { get; set; } = new List<EoullimPayment>();

    public virtual ICollection<EoullimPayment> EoullimPaymentUserBalances { get; set; } = new List<EoullimPayment>();

    public virtual ICollection<EoullimTransaction> EoullimTransactionReceivers { get; set; } = new List<EoullimTransaction>();

    public virtual ICollection<EoullimTransaction> EoullimTransactionSenders { get; set; } = new List<EoullimTransaction>();

    public virtual User? User { get; set; }
}
