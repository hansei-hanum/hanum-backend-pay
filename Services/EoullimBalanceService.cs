
using Hanum.Core.Helpers;
using Hanum.Pay.Contexts;
using Hanum.Pay.Contracts.Services;
using Hanum.Pay.Exceptions;
using Hanum.Pay.Models.DTO.Requests;
using Hanum.Pay.Models.DTO.Responses;
using Microsoft.EntityFrameworkCore;

namespace Hanum.Pay.Services;

public class EoullimBalanceService(ILogger<EoullimBalanceService> logger, HanumContext context) : IEoullimBalanceService {
    public async Task<EoullimExchangeTransferResponse> ExchangeTransferAsync(EoullimExchangeTransferRequest transferRequest) {
        var exchangeResult = await context.EoullimPersonalBalanceChargeAsync(
            userId: transferRequest.UserId,
            transferAmount: transferRequest.Amount,
            message: transferRequest.Message
        );

        if (!exchangeResult.Success) {
            logger.LogWarning("충전실패: {ErrorMessage} [사용자: {UserId}, 충전금액: {Amount}]",
                exchangeResult.ErrorMessage ?? "Unknown", transferRequest.UserId, transferRequest.Amount);

            throw new DbTransctionException(exchangeResult.ErrorCode!);
        }

        var transaction = exchangeResult.Data.Transaction;

        logger.LogInformation("충전성공: [사용자: {UserId}, 충전금액: {Amount}, 잔액: {BalanceAmount}, 누적환전금: {TotalExchangeAmount}]",
            transferRequest.UserId, transferRequest.Amount, transaction.ReceiverAmount, exchangeResult.Data.TotalExchangeAmount);

        return new() {
            TotalExchangeAmount = exchangeResult.Data.TotalExchangeAmount,
            Transaction = new() {
                Id = transaction.Id,
                SenderId = transaction.SenderId,
                ReceiverId = transaction.ReceiverId,
                TransferAmount = transaction.TransferAmount,
                Time = transaction.Time,
                SenderAmount = transaction.SenderAmount,
                ReceiverAmount = transaction.ReceiverAmount,
                Message = transaction.Message,
            }
        };
    }

    public async Task<ulong> GetBalanceAmountAsync(ulong userId) {
        return await (
            from b in context.EoullimBalances
            where b.UserId == userId
            select b.Amount
        ).FirstOrDefaultAsync();
    }

    public async Task<DbOffsetBasedPagenationResult<EoullimUserPayment>> GetPaymentsDetailAsync(ulong userId, int page = 1, int limit = 20) {
        return await context.EoullimPayments.Where(p => p.UserId == userId)
            .OrderByDescending(p => p.Id)
            .ToOffsetPagenation(
                p => new EoullimUserPayment {
                    Id = p.Id,
                    UserId = p.UserId,
                    BoothId = p.BoothId,
                    BoothName = p.Booth.Name,
                    PaidAmount = p.PaidAmount,
                    RefundedAmount = p.RefundedAmount,
                    PaymentComment = p.PaymentComment,
                    RefundComment = p.RefundComment,
                    Status = p.Status,
                    PaidTime = p.PaidTime,
                    RefundedTime = p.RefundedTime
                },
                p => p,
                page,
                limit
            );
    }

    public async Task<EoullimPaymentResponse> PaymentAsync(ulong userId, EoullimPaymentRequest paymentRequest) {
        var paymentResult = await context.EoullimPaymentAsync(
            userId: userId,
            boothId: paymentRequest.BoothId,
            transferAmount: paymentRequest.Amount
        );

        if (!paymentResult.Success) {
            logger.LogWarning("결제실패: {ErrorMessage} [결제자: {UserId}, 부스: {BoothId}, 금액: {Amount}]",
                paymentResult.ErrorMessage ?? "Unknown", userId, paymentRequest.BoothId, paymentRequest.Amount);

            throw new DbTransctionException(paymentResult.ErrorCode!);
        }

        var transaction = paymentResult.Data.Transaction;

        logger.LogInformation("결제성공: [결제자: {UserId}, 부스: {BoothId}, 금액: {Amount}, 잔액: {BalanceAmount}]",
            userId, paymentRequest.BoothId, paymentRequest.Amount, transaction.SenderAmount);

        return new() {
            PaymentId = paymentResult.Data.Id,
            UserId = paymentResult.Data.UserId,
            BoothId = paymentResult.Data.BoothId,
            BalanceAmount = transaction.SenderAmount!.Value,
            Transaction = new() {
                Id = transaction.Id,
                Time = transaction.Time,
                SenderId = transaction.SenderId,
                ReceiverId = transaction.ReceiverId,
                TransferAmount = transaction.TransferAmount,
                Message = transaction.Message,
            }
        };
    }
}