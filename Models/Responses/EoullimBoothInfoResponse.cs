
/// <summary>
/// 한세어울림한마당 부스정보
/// </summary>
public class EoullimBoothInfoResponse {
    /// <summary>
    /// 부스고유번호
    /// </summary>
    public ulong Id { get; set; }
    /// <summary>
    /// 부스명
    /// </summary>
    public required string Name { get; set; }
}