
using System.ComponentModel.DataAnnotations;

namespace HanumPay.Models.Requests;

/// <summary>
/// 환전 송금 요청 DTO
/// </summary>
public class ExchangeTransferRequest {
    /// <summary>
    /// 개인 잔고 소유자 ID
    /// </summary>
    [Required]
    [Range(1, ulong.MaxValue)]
    public ulong UserId { get; set; }

    /// <summary>
    /// 환전 금액
    /// </summary>
    public ulong Amount { get; set; }
}