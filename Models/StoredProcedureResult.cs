
namespace HanumPay.Models;

public class StoredProcedureResult {
    static readonly Dictionary<string, string> ErrorMessages = new() {
        {"HWR1101", "(HWR1101) 송금자와 수신자가 동일합니다"},
        {"HWR1102", "(HWR1102) 송금액이 올바른지 확인하십시오"},
        {"HWR1001", "(HWR1001) 송금자ID가 잘못되었습니다"},
        {"HWR1005", "(HWR1005) 송금자의 잔액이 부족합니다"},
        {"HWR1002", "(HWR1002) 수신자ID가 잘못되었습니다"},
        {"HWR1203", "(HWR1203) 트랜잭션에 실패하였습니다. (송금자 금액을 업데이트하지 못했습니다."},
        {"HWR1204", "(HWR1204) 트랜잭션에 실패하였습니다. (수신자 금액을 업데이트하지 못했습니다."},
        {"HWR2001", "(HWR2001) 비즈니스 계좌ID를 확인해주십시오"},
        {"HWR2101", "(HWR2101) 해당 계좌는 비즈니스 계좌가 아닙니다"},
        {"HWR2111", "(HWR2111) 해당 잔고는 개인잔고가 아닙니다."},
        {"HWR2201", "(HWR2201) 해당 사용자가 존재하지 않습니다."},
    };

    public bool Success { get; set; } = true;
    public string? ErrorCode { get; set; } = null;

    public string? ErrorMessage => ErrorCode != null ? ErrorMessages.GetValueOrDefault(ErrorCode, "정의되지 않은 오류입니다") : null;
}