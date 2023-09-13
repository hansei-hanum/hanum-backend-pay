using HanumPay.Models.Responses;

namespace Models.Responses;

/// <summary>
/// 한세어울림한마당 부스결제내역
/// </summary>
public class EoullimBoothPaymentDetailResponse : APIPagenationResponse {
    /// <summary>
    /// 잔액
    /// </summary>
    public required ulong BalanceAmount { get; set; }
    /// <summary>
    /// 결제내역
    /// </summary>
    public required List<EoullimBoothPayment> Payments { get; set; }
}

/// <summary>
/// 한세어울림한마당 결제내역
/// </summary>
public partial class EoullimBoothPayment : EoullimPayment {
    public required string UserName { get; set; }
}
