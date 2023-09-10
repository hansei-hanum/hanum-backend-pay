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

-- Dumping structure for function hanum.ensure_personal_balance
DELIMITER //
CREATE FUNCTION `ensure_personal_balance`(`personal_id` BIGINT UNSIGNED,
	`message` VARCHAR(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
) RETURNS bigint(20) unsigned
    COMMENT '개인 고객의 계정 연동 잔고를 보장함 (단일 커서 RESET 필요)'
BEGIN
    DECLARE balance_id BIGINT UNSIGNED;
    DECLARE balance_type ENUM('personal', 'business');

    -- 잔고가 존재하지 않으면 초기화하고, 있다면 무시
	 INSERT IGNORE INTO `balances` (`user_id`, `amount`, `type`, `comment`, `label`)
	     VALUES (personal_id, 0, 'personal', message, (SELECT `name` FROM `users` WHERE id = personal_id));
	    
    -- 추가된 잔고 고유번호
    SET balance_id = LAST_INSERT_ID();
    
    -- 새로운 레코드가 삽입되지 않았다면 레코드의 ID를 가져옴
    IF balance_id = 0 THEN
        SELECT `id`, `type` INTO balance_id, balance_type FROM `balances` WHERE user_id = personal_id;
	
		 -- 사용자 존재 여부 확인
		 IF balance_id = 0 THEN
		     -- (HWR2201) 해당 사용자가 존재하지 않습니다.
	        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'HWR2201';
		 END IF;
		 
	    -- 개안잔고 여부 조회
	    IF balance_type != 'personal' THEN
	    	  -- (HWR2111) 해당 잔고는 개인잔고가 아닙니다.
	        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'HWR2111';
	    END IF;
    END IF;
    
    -- 고유번호 반환
    RETURN balance_id;
END//
DELIMITER ;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
