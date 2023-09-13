using HanumPay.Models.Responses;

namespace Models.Responses;

public class EoullimBoothRankResponse : APIPagenationResponse {
    /// <summary>
    /// 부스순위
    /// </summary>
    public required List<EoullimBoothRankItem> Ranks { get; set; }
    /// <summary>
    /// 정렬방식
    /// </summary>
    public required bool Descending { get; set; }
}

public class EoullimBoothRankItem {
    /// <summary>
    /// 부스고유번호
    /// </summary>
    public required ulong Id { get; set; }
    /// <summary>
    /// 부스순위
    /// </summary>
    public required int Rank { get; set; }
    /// <summary>
    /// 부스명
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// 부스잔액
    /// </summary>
    public required ulong Amount { get; set; }
}