
using System.ComponentModel.DataAnnotations;

namespace HanumPay.Models.Requests;

/// <summary>
/// 한세어울림한마당환전충전요청
/// </summary>
public class EoullimExchangeTransferRequest {
    /// <summary>
    /// 사용자고유번호
    /// </summary>
    [Required]
    [Range(1, ulong.MaxValue)]
    public ulong UserId { get; set; }

    /// <summary>
    /// 충전금액
    /// </summary>
    [Required]
    [Range(1, ulong.MaxValue)]
    public ulong Amount { get; set; }

    /// <summary>
    /// 충전메시지
    /// </summary>
    [MaxLength(24)]
    public string? Message { get; set; }
}