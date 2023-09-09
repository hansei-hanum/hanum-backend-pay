namespace Models.Responses;

/// <summary>
/// 결제 응답 DTO
/// </summary>
public class PaymentResponse {
    /// <summary>
    /// 결제 트랜잭션 정보
    /// </summary>
    public class TransactionInfo {
        /// <summary>
        /// 트랜잭션 ID
        /// </summary>
        public ulong Id { get; set; }
        /// <summary>
        /// 송금자 ID
        /// </summary>
        public ulong? SenderId { get; set; }
        /// <summary>
        /// 수신자 ID
        /// </summary>
        public ulong ReceiverId { get; set; }
        /// <summary>
        /// 송금 금액
        /// </summary>
        public ulong TransferAmount { get; set; }
        /// <summary>
        /// 트랜잭션 시각
        /// </summary>
        public DateTime Time { get; set; }
    }

    /// <summary>
    /// 송금자 잔고
    /// </summary>
    public ulong BalanceAmount { get; set; }
    /// <summary>
    /// 결제 트랜잭션 정보
    /// </summary>
    public required TransactionInfo Transaction { get; set; }
}