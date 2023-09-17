namespace HanumPay.Models.Responses;

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
    public required List<EoullimBoothPayment> Payments { get; set; }
}

/// <summary>
/// 한세어울림한마당 결제내역
/// </summary>
public partial class EoullimBoothPayment : EoullimPayment {
    /// <summary>
    /// 사용자이름
    /// </summary>
    public required string UserName { get; set; }
    // /// <summary>
    // /// 사용자신원정보
    // /// </summary>
    // public required UserRoleInfo? UserRole { get; set; }
}
