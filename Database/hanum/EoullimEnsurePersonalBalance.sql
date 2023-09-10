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

-- Dumping structure for function hanum.EoullimEnsurePersonalBalance
DELIMITER //
CREATE FUNCTION `EoullimEnsurePersonalBalance`(`personalUserId` BIGINT UNSIGNED,
	`message` VARCHAR(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
) RETURNS bigint(20) unsigned
    COMMENT '한세어울림한마당 개인 사용자 잔고 ID 보장 조회'
BEGIN
    DECLARE balanceId BIGINT UNSIGNED;
    DECLARE balanceType ENUM('personal', 'booth');

    -- 잔고가 존재하지 않으면 초기화하고, 있다면 무시
	 INSERT IGNORE INTO `EoullimBalances` (`userId`, `amount`, `type`, `comment`)
	     VALUES (personalUserId, 0, 'personal', message);
	    
    -- 추가된 잔고 고유번호
    SET balanceId = LAST_INSERT_ID();
    
    -- 새로운 레코드가 삽입되지 않았다면 레코드의 ID를 가져옴
    IF balanceId = 0 THEN
        SELECT `id`, `type` INTO balanceId, balanceType FROM `EoullimBalances` WHERE userId = personalUserId LIMIT 1;
	
		 -- 사용자 존재 여부 확인
		 IF balanceId = 0 THEN
		     -- 해당 사용자가 존재하지 않습니다.
	        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'USER_NOT_FOUND';
		 END IF;
		 
	    -- 개안잔고 여부 조회
	    IF balanceType != 'personal' THEN
	    	  -- 해당 잔고는 개인잔고가 아닙니다.
	        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'NOT_A_PERSONAL_BALANCE';
	    END IF;
    END IF;
    
    -- 고유번호 반환
    RETURN balanceId;
END//
DELIMITER ;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
