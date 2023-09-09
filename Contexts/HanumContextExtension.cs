using System.Data;
using System.Data.Common;
using HanumPay.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace HanumPay.Contexts;

static class HanumContextExtension {
    static IEnumerable<MySqlParameter> AddOutputParameters(this DbParameterCollection collection, params (string name, MySqlDbType type)[] parameters) {
        foreach (var (name, type) in parameters) {
            var param = new MySqlParameter(name, type) { Direction = ParameterDirection.Output };
            collection.Add(param);
            yield return param;
        }
    }

    static object? EnsureNull(object? value) => value == DBNull.Value ? null : value;

    /// <summary>
    /// 송금을 수행합니다.
    /// </summary>
    /// <param name="context">DB Context</param>
    /// <param name="senderId">송금자 ID</param>
    /// <param name="receiverId">수신자 ID</param>
    /// <param name="transferAmount">송금액</param>
    /// <param name="message">송금 메시지</param>
    /// <returns>트랜잭션 결과</returns>
    public static async Task<TransactionResult> TransactionAsync(
        this HanumContext context,
        ulong? senderId,
        ulong receiverId,
        ulong transferAmount,
        string? message = null
    ) {
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "transaction";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new MySqlParameter("@sender_id", senderId ?? (object)DBNull.Value));
        command.Parameters.Add(new MySqlParameter("@receiver_id", receiverId));
        command.Parameters.Add(new MySqlParameter("@transfer_amount", transferAmount));
        command.Parameters.Add(new MySqlParameter("@message", message ?? (object)DBNull.Value));

        var parameters = command.Parameters.AddOutputParameters(
            ("@transaction_id", MySqlDbType.UInt64),
            ("@transaction_time", MySqlDbType.DateTime),
            ("@sender_amount", MySqlDbType.UInt64),
            ("@receiver_amount", MySqlDbType.UInt64)
        ).ToArray();

        await context.Database.OpenConnectionAsync();

        try {
            await command.ExecuteNonQueryAsync();

            return new TransactionResult {
                Id = (ulong)parameters[0].Value!,
                Time = (DateTime)parameters[1].Value!,
                SenderId = senderId,
                SenderAmount = (ulong?)EnsureNull(parameters[2].Value!),
                ReceiverId = receiverId,
                ReceiverAmount = (ulong)parameters[3].Value!,
                TransferAmount = transferAmount
            };
        } catch (MySqlException ex) when (ex.SqlState == "45000") {
            return new TransactionResult {
                Success = false,
                ErrorCode = ex.Message
            };
        } finally {
            await context.Database.CloseConnectionAsync();
        }
    }

    /// <summary>
    /// 개인 -> 비즈니스 송금을 수행합니다.
    /// </summary>
    /// <param name="context">DB Context</param>
    /// <param name="personalUserId">개인 사용자 ID</param>
    /// <param name="businessBalanceId">비즈니스 잔고 ID</param>
    /// <param name="transferAmount">송금액</param>
    /// <param name="message">송금 메시지</param>
    /// <returns>트랜잭션 결과</returns>
    public static async Task<TransactionResult> PersonalPayment(
        this HanumContext context,
        ulong personalUserId,
        ulong businessBalanceId,
        ulong transferAmount,
        string? message = null
    ) {
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "personal_payment";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new MySqlParameter("@personal_user_id", personalUserId));
        command.Parameters.Add(new MySqlParameter("@business_balance_id", businessBalanceId));
        command.Parameters.Add(new MySqlParameter("@transfer_amount", transferAmount));
        command.Parameters.Add(new MySqlParameter("@message", message ?? (object)DBNull.Value));

        var parameters = command.Parameters.AddOutputParameters(
            ("@transaction_id", MySqlDbType.UInt64),
            ("@transaction_time", MySqlDbType.DateTime),
            ("@sender_amount", MySqlDbType.UInt64),
            ("@receiver_amount", MySqlDbType.UInt64),
            ("@personal_balance_id", MySqlDbType.UInt64)
        ).ToArray();

        await context.Database.OpenConnectionAsync();

        try {
            await command.ExecuteNonQueryAsync();

            return new TransactionResult {
                Id = (ulong)parameters[0].Value!,
                Time = (DateTime)parameters[1].Value!,
                SenderId = (ulong)parameters[4].Value!,
                SenderAmount = (ulong?)EnsureNull(parameters[2].Value!),
                ReceiverId = businessBalanceId,
                ReceiverAmount = (ulong)parameters[3].Value!,
                TransferAmount = transferAmount
            };
        } catch (MySqlException ex) when (ex.SqlState == "45000") {
            return new TransactionResult {
                Success = false,
                ErrorCode = ex.Message
            };
        } finally {
            await context.Database.CloseConnectionAsync();
        }
    }

    /// <summary>
    /// 개인 잔고로 환전합니다.
    /// </summary>
    /// <param name="context">DB Context</param>
    /// <param name="personalUserId">개인 사용자 ID</param>
    /// <param name="transferAmount">송금액</param>
    /// <param name="message">송금 메시지</param>
    /// <returns>트랜잭션 결과</returns>
    public static async Task<ExchangeResult> PersonalExchange(
        this HanumContext context,
        ulong personalUserId,
        ulong transferAmount,
        string? message = null
    ) {
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "personal_exchange";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new MySqlParameter("@personal_user_id", personalUserId));
        command.Parameters.Add(new MySqlParameter("@transfer_amount", transferAmount));
        command.Parameters.Add(new MySqlParameter("@message", message ?? (object)DBNull.Value));

        var parameters = command.Parameters.AddOutputParameters(
            ("@transaction_id", MySqlDbType.UInt64),
            ("@transaction_time", MySqlDbType.DateTime),
            ("@sender_amount", MySqlDbType.UInt64),
            ("@receiver_amount", MySqlDbType.UInt64),
            ("@personal_balance_id", MySqlDbType.UInt64),
            ("@total_exchange_amount", MySqlDbType.UInt64)
        ).ToArray();

        await context.Database.OpenConnectionAsync();

        try {
            await command.ExecuteNonQueryAsync();

            return new ExchangeResult {
                Id = (ulong)parameters[0].Value!,
                Time = (DateTime)parameters[1].Value!,
                SenderId = null,
                SenderAmount = (ulong?)EnsureNull(parameters[2].Value!),
                ReceiverId = (ulong)parameters[4].Value!,
                ReceiverAmount = (ulong)parameters[3].Value!,
                TransferAmount = transferAmount,
                TotalExchangeAmount = (ulong)parameters[5].Value!
            };
        } catch (MySqlException ex) when (ex.SqlState == "45000") {
            return new ExchangeResult {
                Success = false,
                ErrorCode = ex.Message
            };
        } finally {
            await context.Database.CloseConnectionAsync();
        }
    }
}
