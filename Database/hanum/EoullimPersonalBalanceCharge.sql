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

-- Dumping structure for procedure hanum.EoullimPersonalBalanceCharge
DELIMITER //
CREATE PROCEDURE `EoullimPersonalBalanceCharge`(
	IN `userId` BIGINT UNSIGNED,
	IN `transferAmount` BIGINT UNSIGNED,
	IN `message` VARCHAR(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
	OUT `transactionId` BIGINT UNSIGNED,
	OUT `transactionTime` DATETIME,
	OUT `senderAmount` BIGINT UNSIGNED,
	OUT `receiverAmount` BIGINT UNSIGNED,
	OUT `balanceId` BIGINT UNSIGNED,
	OUT `totalExchangeAmount` BIGINT UNSIGNED
)
    COMMENT '한세어울림한마당 잔고 충전'
BEGIN
    -- 개인잔고 고유번호 조회 및 개인잔고 타입 보장
    SET balanceId := EoullimEnsurePersonalBalance(userId, NULL);
    
    -- 트랜잭션 호출
    CALL EoullimTransaction(
	     NULL,
		  balanceId,
		  transferAmount,
		  message,
		  
		  transactionId,
		  transactionTime,
		  senderAmount,
		  receiverAmount
	 );
	 
	 -- 환전 총액 조회
	 SELECT SUM(amount) INTO totalExchangeAmount FROM `EoullimTransactions` WHERE senderId IS NULL;
END//
DELIMITER ;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
