
namespace Hanum.Pay.Models.DTO.Responses;

/// <summary>
/// 한세어울림한마당 사용자결제내역
/// </summary>
public class EoullimUserPaymentDetailResponse : APIPagenationResponse {
    /// <summary>
    /// 잔액
    /// </summary>
    public required ulong BalanceAmount { get; set; }
    /// <summary>
    /// 결제내역
    /// </summary>
    public required List<EoullimUserPayment> Payments { get; set; }
}

/// <summary>
/// 한세어울림한마당 결제내역
/// </summary>
public partial class EoullimUserPayment : EoullimPayment {
    /// <summary>
    /// 부스이름
    /// </summary>
    public required string BoothName { get; set; }
}
