

/// <summary>
/// 사용자신원정보
/// </summary>
public class UserRoleInfo {
    /// <summary>
    /// 사용자고유번호
    /// </summary>
    public required ulong? UserId { get; set; }
    /// <summary>
    /// 사용자구분('STUDENT','TEACHER')
    /// </summary>
    public required string Type { get; set; }
    /// <summary>
    /// 사용자과구분('CLOUD_SECURITY','NETWORK_SECURITY','HACKING_SECURITY','METAVERSE_GAME','GAME')
    /// </summary>
    public required string? Department { get; set; }
    /// <summary>
    /// 사용자학년
    /// </summary>
    public required byte? Grade { get; set; }
    /// <summary>
    /// 사용자반
    /// </summary>
    public required byte? Classroom { get; set; }
    /// <summary>
    /// 사용자번호
    /// </summary>
    public required byte? Number { get; set; }
}