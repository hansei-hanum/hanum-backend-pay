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

-- Dumping structure for procedure hanum.EoullimPayment
CREATE PROCEDURE `EoullimPayment`(
	IN `userId` BIGINT UNSIGNED,
	IN `boothId` BIGINT UNSIGNED,
	IN `transferAmount` BIGINT UNSIGNED,
	IN `message` VARCHAR(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
	OUT `transactionId` BIGINT UNSIGNED,
	OUT `transactionTime` DATETIME,
	OUT `userBalanceAmount` BIGINT UNSIGNED,
	OUT `boothBalanceAmount` BIGINT UNSIGNED,
	OUT `userBalanceId` BIGINT UNSIGNED,
	OUT `boothBalanceId` BIGINT UNSIGNED,
	OUT `paymentId` BIGINT UNSIGNED
)
    COMMENT '한세어울림한마당 결제'
BEGIN
    DECLARE boothBalanceType ENUM('personal', 'booth');
    
    -- 사용자잔고 고유번호 조회
    SET userBalanceId := EoullimEnsurePersonalBalance(userId, NULL);
    
    -- 부스 잔고 id 조회
    SELECT `id`, `type` INTO boothBalanceId, boothBalanceType FROM `EoullimBalances` WHERE EoullimBalances.boothId = boothId;
    
    -- 부스 잔고 존재 여부 확인
    IF boothBalanceType IS NULL THEN
    	  -- 해당 부스 잔고가 존재하지 않습니다.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'BOOTH_BALANCE_NOT_FOUND';
    END IF;
    
    -- 부스 잔고 여부 확인
    IF boothBalanceType != 'booth' THEN
    	  -- 해당 잔고는 부스 잔고가 아닙니다.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'NOT_A_BOOTH_OPERATIONAL_BALANCE';
    END IF;
    
    -- 트랜잭션 호출
    CALL EoullimTransaction(
	     userBalanceId,
		  boothBalanceId,
		  transferAmount,
		  message,
		  
		  transactionId,
		  transactionTime,
		  userBalanceAmount,
		  boothBalanceAmount
	 );
	 
	 -- 결제내역 추가
	 INSERT INTO `EoullimPayments` (`userId`, `boothId`, `paidAmount`, `paymentTransactionId`, `paymentComment`, `status`, `userBalanceId`, `boothBalanceId`, `paidTime`)
	     VALUES (userId, boothId, transferAmount, transactionId, message, 'paid', userBalanceId, boothBalanceId, transactionTime);
	 
	 -- 결제고유번호 설정
	 SET paymentId := LAST_INSERT_ID();
END;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
