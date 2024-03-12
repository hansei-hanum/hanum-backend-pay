namespace Hanum.Pay.Models.DTO.Responses;

/// <summary>
/// 한세어울림한마당 부스결제내역
/// </summary>
public class EoullimBoothPaymentDetailResponse : APIPagenationResponse {
    /// <summary>
    /// 부스정보
    /// </summary>
    public required EoullimBoothDetail BoothInfo { get; set; }
    /// <summary>
    /// 잔액
    /// </summary>
    public required ulong BalanceAmount { get; set; }
    /// <summary>
    /// 결제내역
    /// </summary>
    public required IEnumerable<EoullimBoothPayment> Payments { get; set; }
}

/// <summary>
/// 한세어울림한마당 결제내역
/// </summary>
public partial class EoullimBoothPayment : EoullimPayment {
    /// <summary>
    /// 사용자이름
    /// </summary>
    public required string UserName { get; set; }
}
