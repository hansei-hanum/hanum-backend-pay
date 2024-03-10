
using System.ComponentModel.DataAnnotations;

namespace Hanum.Pay.Models.Requests;

/// <summary>
/// 한세어울림한마당결제요청
/// </summary>
public class EoullimPaymentRequest {
    /// <summary>
    /// 부스고유번호
    /// </summary>
    [Required]
    [Range(1, ulong.MaxValue)]
    public ulong BoothId { get; set; }

    /// <summary>
    /// 결제금액
    /// </summary>
    [Required]
    [Range(1, ulong.MaxValue)]
    public ulong Amount { get; set; }
}