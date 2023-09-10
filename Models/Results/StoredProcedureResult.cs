
namespace HanumPay.Models.Results;

public class StoredProcedureResult<TValue> {
    static readonly Dictionary<string, string> ErrorMessages = new() {
        {"USER_NOT_FOUND", "해당 사용자가 존재하지 않습니다."},
        {"NOT_A_PERSONAL_BALANCE", "해당 잔고는 개인잔고가 아닙니다."},
        {"BOOTH_BALANCE_NOT_FOUND", "해당 부스 잔고가 존재하지 않습니다."},
        {"NOT_A_BOOTH_OPERATIONAL_BALANCE", "해당 잔고는 부스 잔고가 아닙니다."},
        {"PAYMENT_RECORD_NOT_FOUND", "해당 결제내역이 존재하지 않습니다."},
        {"PAYMENT_ALREADY_CANCELLED", "이미 결제가 취소되었습니다."},
        {"PAYMENT_CANCELLATION_STATUS_NOT_UPDATED", "결제 취소 상태를 업데이트하지 못했습니다."},
        {"SENDER_ID_EQUALS_RECEIVER_ID", "송금자와 수신자가 일치합니다."},
        {"INVALID_TRANSFER_AMOUNT", "송금액이 올바른지 확인하십시오."},
        {"INVALID_SENDER_ID", "송금자ID가 잘못되었습니다."},
        {"INSUFFICIENT_SENDER_BALANCE", "송금자의 잔액이 부족합니다."},
        {"INVALID_RECEIVER_ID", "수신자ID가 잘못되었습니다."},
        {"SENDER_BALANCE_NOT_UPDATED", "송금자 금액을 업데이트하지 못했습니다."},
        {"RECEIVER_BALANCE_NOT_UPDATED", "수신자 금액을 업데이트하지 못했습니다."},
    };

    private TValue? _data;

    public bool Success { get; set; } = true;
    public string? ErrorCode { get; set; } = null;
    public TValue Data {
        get => Success ? _data! : throw new InvalidOperationException("Data is not available when Success is false");
        set => _data = value;
    }
    public string? ErrorMessage => ErrorCode != null ? ErrorMessages.GetValueOrDefault(ErrorCode, $"({ErrorCode}) 정의되지 않은 오류입니다.") : null;

    public static StoredProcedureResult<TValue> FromError(string errorCode) => new() {
        Success = false,
        ErrorCode = errorCode
    };

    public static StoredProcedureResult<TValue> FromData(TValue value) => new() {
        Success = true,
        Data = value
    };
}