using System.Data;
using System.Data.Common;
using Hanum.Pay.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Hanum.Pay.Contexts;

static class HanumEoullimContextExtension {
    static IEnumerable<MySqlParameter> AddOutputParameters(
        this DbParameterCollection collection, params (string name, MySqlDbType type)[] parameters) {
        foreach (var (name, type) in parameters) {
            var param = new MySqlParameter(name, type) { Direction = ParameterDirection.Output };
            collection.Add(param);
            yield return param;
        }
    }

    static IEnumerable<MySqlParameter> AddInputParameters(
        this DbParameterCollection collection, params (string name, object? value)[] parameters) {
        foreach (var (name, value) in parameters) {
            var param = new MySqlParameter(name, value ?? DBNull.Value);
            collection.Add(param);
            yield return param;
        }
    }

    static object? EnsureNull(object? value) => value == DBNull.Value ? null : value;

    /// <summary>
    /// 한세어울림한마당 부스결제
    /// </summary>
    /// <param name="context">DB 컨텍스트</param>
    /// <param name="userId">결제자 ID</param>
    /// <param name="boothId">부스 ID</param>
    /// <param name="transferAmount">결제 금액</param>
    /// <param name="message">메시지</param>
    /// <returns>결제 결과</returns>
    public static async Task<StoredProcedureResult<EoullimPaymentResult>> EoullimPayment(
        this HanumContext context, ulong userId, ulong boothId, ulong transferAmount, string? message = null) {
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "EoullimPayment";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddInputParameters(
            ("@userId", userId),
            ("@boothId", boothId),
            ("@transferAmount", transferAmount),
            ("@message", message)
        ).Count();

        var parameters = command.Parameters.AddOutputParameters(
            ("@transactionId", MySqlDbType.UInt64),
            ("@transactionTime", MySqlDbType.DateTime),
            ("@userBalanceAmount", MySqlDbType.UInt64),
            ("@boothBalanceAmount", MySqlDbType.UInt64),
            ("@userBalanceId", MySqlDbType.UInt64),
            ("@boothBalanceId", MySqlDbType.UInt64),
            ("@paymentId", MySqlDbType.UInt64)
        ).ToArray();

        await context.Database.OpenConnectionAsync();

        try {
            await command.ExecuteNonQueryAsync();

            return StoredProcedureResult<EoullimPaymentResult>.FromData(new() {
                Id = (ulong)parameters[6].Value!,
                UserId = userId,
                BoothId = boothId,
                Transaction = new() {
                    Id = (ulong)parameters[0].Value!,
                    Time = (DateTime)parameters[1].Value!,
                    SenderId = (ulong?)EnsureNull(parameters[4].Value)!,
                    ReceiverId = (ulong)parameters[5].Value!,
                    SenderAmount = (ulong?)EnsureNull(parameters[2].Value!),
                    ReceiverAmount = (ulong)parameters[3].Value!,
                    TransferAmount = transferAmount,
                    Message = message
                }
            });
        } catch (MySqlException ex) when (ex.SqlState == "45000") {
            return StoredProcedureResult<EoullimPaymentResult>.FromError(ex.Message);
        } finally {
            await context.Database.CloseConnectionAsync();
        }
    }

    /// <summary>
    /// 한세어울림한마당 부스환불
    /// 부스고유번호 검증이 없으므로 주의하여 사용하십시오.
    /// </summary>
    /// <param name="context">DB 컨텍스트</param>
    /// <param name="paymentId">결제 ID</param>
    /// <param name="message">메시지</param>
    /// <returns>환불 결과</returns>
    public static async Task<StoredProcedureResult<EoullimRefundResult>> EoullimPaymentCancel(
        this HanumContext context, ulong paymentId, string? message = null) {
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "EoullimPaymentCancel";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddInputParameters(
            ("@paymentId", paymentId),
            ("@message", message)
        ).Count();

        var parameters = command.Parameters.AddOutputParameters(
            ("@transactionId", MySqlDbType.UInt64),
            ("@transactionTime", MySqlDbType.DateTime),
            ("@userId", MySqlDbType.UInt64),
            ("@boothId", MySqlDbType.UInt64),
            ("@userBalanceId", MySqlDbType.UInt64),
            ("@boothBalanceId", MySqlDbType.UInt64),
            ("@userBalanceAmount", MySqlDbType.UInt64),
            ("@boothBalanceAmount", MySqlDbType.UInt64),
            ("@paidAmount", MySqlDbType.UInt64),
            ("@refundedAmount", MySqlDbType.UInt64)
        ).ToArray();

        await context.Database.OpenConnectionAsync();

        try {
            await command.ExecuteNonQueryAsync();

            return StoredProcedureResult<EoullimRefundResult>.FromData(new() {
                Id = paymentId,
                UserId = (ulong)parameters[2].Value!,
                BoothId = (ulong)parameters[3].Value!,
                PaidAmount = (ulong)parameters[8].Value!,
                RefundedAmount = (ulong)parameters[9].Value!,
                Transaction = new() {
                    Id = (ulong)parameters[0].Value!,
                    Time = (DateTime)parameters[1].Value!,
                    SenderId = (ulong?)EnsureNull(parameters[4].Value)!,
                    ReceiverId = (ulong)parameters[5].Value!,
                    SenderAmount = (ulong?)EnsureNull(parameters[6].Value!),
                    ReceiverAmount = (ulong)parameters[7].Value!,
                    TransferAmount = (ulong)parameters[9].Value!,
                    Message = message
                }
            });
        } catch (MySqlException ex) when (ex.SqlState == "45000") {
            return StoredProcedureResult<EoullimRefundResult>.FromError(ex.Message);
        } finally {
            await context.Database.CloseConnectionAsync();
        }
    }

    /// <summary>
    /// 한세어울림한마당 잔고충전
    /// </summary>
    /// <param name="context">DB 컨텍스트</param>
    /// <param name="userId">충전자 ID</param>
    /// <param name="transferAmount">충전 금액</param>
    /// <param name="message">메시지</param>
    /// <returns>충전 결과</returns>
    public static async Task<StoredProcedureResult<EoullimExchangeTransferResult>> EoullimPersonalBalanceCharge(
        this HanumContext context, ulong userId, ulong transferAmount, string? message = null) {
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "EoullimPersonalBalanceCharge";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddInputParameters(
            ("@userId", userId),
            ("@transferAmount", transferAmount),
            ("@message", message)
        ).Count();

        var parameters = command.Parameters.AddOutputParameters(
            ("@transactionId", MySqlDbType.UInt64),
            ("@transactionTime", MySqlDbType.DateTime),
            ("@senderAmount", MySqlDbType.UInt64),
            ("@receiverAmount", MySqlDbType.UInt64),
            ("@balanceId", MySqlDbType.UInt64),
            ("@totalExchangeAmount", MySqlDbType.UInt64)
        ).ToArray();

        await context.Database.OpenConnectionAsync();

        try {
            await command.ExecuteNonQueryAsync();

            return StoredProcedureResult<EoullimExchangeTransferResult>.FromData(new() {
                TotalExchangeAmount = (ulong)parameters[5].Value!,
                Transaction = new() {
                    Id = (ulong)parameters[0].Value!,
                    Time = (DateTime)parameters[1].Value!,
                    SenderId = null,
                    ReceiverId = (ulong)parameters[4].Value!,
                    SenderAmount = null,
                    ReceiverAmount = (ulong)parameters[3].Value!,
                    TransferAmount = transferAmount,
                    Message = message
                }
            });
        } catch (MySqlException ex) when (ex.SqlState == "45000") {
            return StoredProcedureResult<EoullimExchangeTransferResult>.FromError(ex.Message);
        } finally {
            await context.Database.CloseConnectionAsync();
        }
    }
}
