
using System.ComponentModel.DataAnnotations;

namespace Hanum.Pay.Models.Requests;

/// <summary>
/// 한세어울림한마당환불요청
/// </summary>
public class EoullimBoothRefundRequest {
    /// <summary>
    /// 결제고유번호
    /// </summary>
    [Required]
    [Range(1, ulong.MaxValue)]
    public ulong PaymentId { get; set; }
    /// <summary>
    /// 환불메시지
    /// </summary>
    [MaxLength(24)]
    public string? Message { get; set; }
}