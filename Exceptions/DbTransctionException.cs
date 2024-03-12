using Hanum.Core.Models;
using Hanum.Pay.Models;

namespace Hanum.Pay.Exceptions;

public class DbTransctionException(string errorCode) : Exception(
    $"DB Transaction Error: {StoredProcedureResult.ErrorMessages.GetValueOrDefault(errorCode, $"({errorCode}) 정의되지 않은 오류입니다.")}") {
    public string ErrorCode { get; } = errorCode;
    public string ErrorMessage => StoredProcedureResult.ErrorMessages.GetValueOrDefault(ErrorCode, $"({ErrorCode}) 정의되지 않은 오류입니다.");
    public HanumStatusCode StatusCode => StoredProcedureResult.ErrorCodes.GetValueOrDefault(ErrorCode, HanumStatusCode.Error);
}