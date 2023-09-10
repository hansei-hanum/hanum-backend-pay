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

-- Dumping structure for procedure hanum.personal_exchange
DELIMITER //
CREATE PROCEDURE `personal_exchange`(
	IN `personal_user_id` BIGINT UNSIGNED,
	IN `transfer_amount` BIGINT UNSIGNED,
	IN `message` VARCHAR(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
	OUT `transaction_id` BIGINT UNSIGNED,
	OUT `transaction_time` DATETIME,
	OUT `sender_amount` BIGINT UNSIGNED,
	OUT `receiver_amount` BIGINT UNSIGNED,
	OUT `personal_balance_id` BIGINT UNSIGNED,
	OUT `total_exchange_amount` BIGINT UNSIGNED
)
    COMMENT '개인 잔고로 환전합니다.'
BEGIN
    DECLARE dummy_uint64 BIGINT UNSIGNED;
    
    -- 개인잔고 고유번호 조회 및 개인잔고 타입 보장
    SET personal_balance_id := ensure_personal_balance(personal_user_id, NULL);
    
    -- 트랜잭션 호출
    CALL transaction(
	     NULL,
		  personal_balance_id,
		  transfer_amount,
		  message,
		  
		  transaction_id,
		  transaction_time,
		  sender_amount,
		  receiver_amount
	 );
	 
	 -- 환전 총액 조회
	 SELECT SUM(amount) INTO total_exchange_amount FROM `transactions` WHERE sender_id IS NULL;
END//
DELIMITER ;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
