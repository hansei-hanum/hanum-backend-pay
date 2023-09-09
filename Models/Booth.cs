using System;
using System.Collections.Generic;

namespace HanumPay.Models;

/// <summary>
/// 어울림한마당 부스
/// </summary>
public partial class Booth
{
    /// <summary>
    /// 부스 고유번호
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// 부스 키
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// 부스 구분
    /// </summary>
    public string Class { get; set; } = null!;

    /// <summary>
    /// 부스명
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 부스 위치
    /// </summary>
    public string Location { get; set; } = null!;

    /// <summary>
    /// 부스 생성 날짜
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 부스 수정 날짜
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
