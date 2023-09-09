namespace Models.Responses;

/// <summary>
/// 잔고 정보 응답 DTO
/// </summary>
public class BalanceInfoResponse {
    /// <summary>
    /// 잔고 ID
    /// </summary>
    public ulong Id { get; set; }
    /// <summary>
    /// 잔고 이름
    /// </summary>
    public required string Label { get; set; }
    /// <summary>
    /// 잔고 종류 (personal/business)
    /// </summary>
    public required string Type { get; set; }
}