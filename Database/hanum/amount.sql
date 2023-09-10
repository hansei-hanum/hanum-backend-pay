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

-- Dumping structure for procedure hanum.amount
DELIMITER //
CREATE PROCEDURE `amount`(
	IN `balance_id` BIGINT UNSIGNED,
	IN `record_page` INT UNSIGNED,
	IN `record_count` SMALLINT UNSIGNED,
	OUT `balance_amount` BIGINT UNSIGNED
)
    COMMENT '잔액조회'
BEGIN
    DECLARE offs INT UNSIGNED;
    
    -- 잔액 조회
    SELECT amount INTO balance_amount FROM `balances` WHERE id = balance_id;
    
    -- 송금이력 조회
    IF record_count > 0 THEN
        SET offs = (record_page - 1) * record_count;
        SELECT * FROM `transactions`
		      WHERE sender_id = balance_id OR receiver_id = balance_id
				ORDER BY `time` DESC
				LIMIT record_count OFFSET offs;
    END IF;
END//
DELIMITER ;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
