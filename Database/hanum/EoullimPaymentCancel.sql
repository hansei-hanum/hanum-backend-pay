-- --------------------------------------------------------
-- Host:                         hanum.c8oqu7y0ih5p.ap-northeast-2.rds.amazonaws.com
-- Server version:               10.6.14-MariaDB - managed by https://aws.amazon.com/rds/
-- Server OS:                    Linux
-- HeidiSQL Version:             12.5.0.6677
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- Dumping structure for procedure hanum.EoullimPaymentCancel
DELIMITER //
CREATE PROCEDURE `EoullimPaymentCancel`(
	IN `paymentId` BIGINT UNSIGNED,
	IN `message` VARCHAR(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
	OUT `transactionId` BIGINT UNSIGNED,
	OUT `transactionTime` DATETIME,
	OUT `userId` BIGINT UNSIGNED,
	OUT `boothId` BIGINT UNSIGNED,
	OUT `userBalanceId` BIGINT UNSIGNED,
	OUT `boothBalanceId` BIGINT UNSIGNED,
	OUT `userBalanceAmount` BIGINT UNSIGNED,
	OUT `boothBalanceAmount` BIGINT UNSIGNED,
	OUT `paidAmount` BIGINT UNSIGNED,
	OUT `refundedAmount` BIGINT UNSIGNED
)
    COMMENT '한세어울림한마당 결제 취소'
BEGIN
    DECLARE paymentStatus ENUM('paid','refunded');
    
    -- 트랜잭션 시작
    START transaction;
    
	 -- 결제 내역을 잠금
    SELECT `status`, `userId`, `boothId`, `userBalanceId`, `boothBalanceId`, `paidAmount`
	     INTO paymentStatus, userId, boothId, userBalanceId, boothBalanceId, paidAmount
		  FROM `EoullimPayments`
		  WHERE id = paymentId FOR UPDATE;
    
    -- 결제내역 존재 여부 확인
    IF paymentStatus IS NULL THEN
        -- 해당 결제내역이 존재하지 않습니다.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'PAYMENT_RECORD_NOT_FOUND';
    END IF;
    
    -- 이미 취소되었는지 여부 확인
    IF paymentStatus = 'refunded' THEN
        -- 이미 결제가 취소되었습니다.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'PAYMENT_ALREADY_CANCELLED';
    END IF;
    
    -- 트랜잭션 호출
    CALL EoullimTransaction(
	     boothBalanceId,
		  userBalanceId,
		  paidAmount,
		  message,
		  
		  transactionId,
		  transactionTime,
		  userBalanceAmount,
		  boothBalanceAmount
	 );
	 
	 -- 결제 취소 설정
    UPDATE `EoullimPayments`
	     SET
	         `status` = 'refunded',
      		`refundTransactionId` = transactionId,
      		`refundComment` = message,
      		`refundedTime` = transactionTime,
      		`refundedAmount` = paidAmount
        WHERE `id` = paymentId;
    
	 -- 무결성 검증
    IF ROW_COUNT() < 1 THEN
        ROLLBACK;
        -- 결제 취소 상태를 업데이트하지 못했습니다.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'PAYMENT_CANCELLATION_STATUS_NOT_UPDATED';
    END IF;
    
    -- 결제취소금액 설정
    SET refundedAmount := paidAmount;
    
    -- 트랜잭션 커밋
    COMMIT;
END//
DELIMITER ;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
