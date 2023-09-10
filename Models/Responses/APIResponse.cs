
using System.Text.Json.Serialization;

namespace HanumPay.Models.Responses;

public class APIResponse<TData> {
    [JsonPropertyName("message")]
    public required string Code { get; set; }

    public TData? Data { get; set; } = default;

    public static APIResponse<TData> FromData(TData data) {
        return new APIResponse<TData> {
            Code = "SUCCESS",
            Data = data
        };
    }

    public static APIResponse<TData> FromError(string code) {
        return new APIResponse<TData> {
            Code = code
        };
    }
}

public class APIPagenationResponse {
    public required int Page { get; set; }
    public required int Limit { get; set; }
    public required int Total { get; set; }
    public int TotalPage => (int)Math.Ceiling((double)Total / Limit);
}