using System;
using System.Collections.Generic;

namespace HanumPay.Models;

/// <summary>
/// 한세어울림한마당 이체 내역
/// </summary>
public partial class EoullimTransaction
{
    /// <summary>
    /// 트랜잭션 고유 ID
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// 송금자 ID, 환전소의 경우 NULL로 설정
    /// </summary>
    public ulong? SenderId { get; set; }

    /// <summary>
    /// 수신자 ID
    /// </summary>
    public ulong ReceiverId { get; set; }

    /// <summary>
    /// 송금액
    /// </summary>
    public ulong Amount { get; set; }

    /// <summary>
    /// 송금 메모
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// 트랜잭션 시간
    /// </summary>
    public DateTime Time { get; set; }

    public virtual EoullimBalance Receiver { get; set; } = null!;

    public virtual EoullimBalance? Sender { get; set; }
}
