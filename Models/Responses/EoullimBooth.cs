namespace Hanum.Pay.Models.Responses;

/// <summary>
/// 한세어울림한마당부스정보
/// </summary>
public class EoullimBoothInfo {
    /// <summary>
    /// 부스고유번호
    /// </summary>
    public ulong Id { get; set; }
    /// <summary>
    /// 부스명
    /// </summary>
    public required string Name { get; set; }
}

/// <summary>
/// 한세어울림한마당부스상세정보
/// </summary>
public class EoullimBoothDetail : EoullimBoothInfo {
    /// <summary>
    /// 부스구분
    /// </summary>
    public required string Classification { get; set; }
    /// <summary>
    /// 부스위치
    /// </summary>
    public required string Location { get; set; }
}