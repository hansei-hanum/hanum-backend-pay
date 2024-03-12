namespace Hanum.Pay.Models.DTO.Responses;

/// <summary>
/// API페이징응답
/// </summary>
public class APIPagenationResponse {
    /// <summary>
    /// 페이지
    /// </summary>
    public required int Page { get; set; }
    /// <summary>
    /// 페이지당 항목수
    /// </summary>
    public required int Limit { get; set; }
    /// <summary>
    /// 전체 항목수
    /// </summary>
    public required int Total { get; set; }
    /// <summary>
    /// 전체 페이지수
    /// </summary>
    public int TotalPage => (int)Math.Ceiling((double)Total / Limit);
}