namespace HanumPay.Models.Responses;

/// <summary>
/// 한세어울림한마당 사용자잔액조회응답
/// </summary>
public class EoullimBalanceAmountResponse {
    /// <summary>
    /// 잔액
    /// </summary>
    public required ulong BalanceAmount { get; set; }
}