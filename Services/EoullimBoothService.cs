using Microsoft.EntityFrameworkCore;

using Hanum.Core.Helpers;
using Hanum.Core.Models;
using Hanum.Pay.Contexts;
using Hanum.Pay.Contracts.Services;
using Hanum.Pay.Exceptions;
using Hanum.Pay.Models.DTO.Requests;
using Hanum.Pay.Models.DTO.Responses;

namespace Hanum.Pay.Services;

public class EoullimBoothService(ILogger<EoullimBoothService> logger, HanumContext context) : IEoullimBoothService {
    public async Task<DbOffsetBasedPaginationResult<EoullimBoothPayment>> GetPaymentDetailAsync(ulong boothId, int page = 1, int limit = 20) {
        return await context.EoullimPayments.Where(p => p.BoothId == boothId)
            .ToOffsetPagination(
                p => new EoullimBoothPayment {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserName = p.User.Name,
                    BoothId = p.BoothId,
                    PaidAmount = p.PaidAmount,
                    RefundedAmount = p.RefundedAmount,
                    PaymentComment = p.PaymentComment,
                    RefundComment = p.RefundComment,
                    Status = p.Status,
                    PaidTime = p.PaidTime,
                    RefundedTime = p.RefundedTime,
                },
                p => p,
                page, limit
            );
    }

    public async Task<ulong> GetBalanceAmountAsync(ulong boothId) {
        return await (
            from b in context.EoullimBalances
            where b.BoothId == boothId
            select b.Amount
        ).FirstOrDefaultAsync();
    }

    public async Task<EoullimBoothRefundResponse> RefundAsync(ulong boothId, EoullimBoothRefundRequest refundRequest) {
        var paymentInfo = await (
            from p in context.EoullimPayments
            where p.Id == refundRequest.PaymentId && p.BoothId == boothId
            select new {
                p.UserId,
                p.PaidAmount,
            }
        ).FirstOrDefaultAsync() ?? throw new ArgumentException("결제정보가 존재하지 않습니다.", nameof(refundRequest));

        var refundResult = await context.EoullimPaymentCancelAsync(
            paymentId: refundRequest.PaymentId,
            message: refundRequest.Message
        );

        if (!refundResult.Success) {
            logger.LogWarning("환불실패: {ErrorMessage} [결제: {PaymentId}, 부스: {BoothId}, 사용자: {UserId}, 금액: {Amount}]",
                refundResult.ErrorMessage ?? "Unknown", refundRequest.PaymentId, boothId, paymentInfo.UserId, paymentInfo.PaidAmount);

            throw new DbTransctionException(refundResult.ErrorCode!);
        }

        var transaction = refundResult.Data.Transaction;

        return new() {
            PaymentId = refundResult.Data.Id,
            UserId = refundResult.Data.UserId,
            BoothId = refundResult.Data.BoothId,
            PaidAmount = refundResult.Data.PaidAmount,
            RefundedAmount = refundResult.Data.RefundedAmount,
            BalanceAmount = transaction.ReceiverAmount,
            Transaction = new() {
                Id = transaction.Id,
                SenderId = transaction.SenderId,
                ReceiverId = transaction.ReceiverId,
                TransferAmount = transaction.TransferAmount,
                Message = transaction.Message,
                Time = transaction.Time
            }
        };
    }

    public async Task<EoullimBoothInfo?> GetBoothInfoAsync(ulong boothId) {
        var boothInfo = await (
            from b in context.EoullimBooths
            where b.Id == boothId
            select new EoullimBoothInfo {
                Id = b.Id,
                Name = b.Name
            }
        ).FirstOrDefaultAsync();

        if (boothInfo is null) {
            logger.LogWarning("부스정보조회실패: 부스정보가 존재하지 않음 [부스: {BoothId}]", boothId);
            return null;
        }

        return boothInfo;
    }

    public async Task<EoullimBoothDetailResponse?> GetBoothDetailAsync(ulong boothId) {
        var boothInfo = await (
            from b in context.EoullimBooths
            where b.Id == boothId
            select new EoullimBoothDetailResponse {
                Id = b.Id,
                Name = b.Name,
                Classification = b.Class,
                Location = b.Location,
            }
        ).FirstOrDefaultAsync();

        if (boothInfo is null) {
            logger.LogWarning("부스정보조회실패: 부스정보가 존재하지 않음 [부스: {BoothId}]", boothId);
            return null;
        }

        return boothInfo;
    }
}
