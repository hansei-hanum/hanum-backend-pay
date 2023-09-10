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

-- Dumping structure for procedure hanum.transaction
DELIMITER //
CREATE PROCEDURE `transaction`(
	IN `sender_id` BIGINT UNSIGNED,
	IN `receiver_id` BIGINT UNSIGNED,
	IN `transfer_amount` BIGINT UNSIGNED,
	IN `message` VARCHAR(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
	OUT `transaction_id` BIGINT UNSIGNED,
	OUT `transaction_time` DATETIME,
	OUT `sender_amount` BIGINT UNSIGNED,
	OUT `receiver_amount` BIGINT UNSIGNED
)
    COMMENT '계좌 금액 송금'
BEGIN
    -- 송금자와 수신자의 ID가 동일한지 여부 확인
    IF sender_id = receiver_id THEN
        -- (HWR1101) 송금자와 수신자가 동일합니다.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'HWR1101';
    END IF;
    
    -- 송금액이 있는지 여부 확인
    IF transfer_amount <= 0 THEN
    	  -- (HWR1102) 송금액이 올바른지 확인하십시오.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'HWR1102';
    END IF;
    
    -- 트랜잭션 시작
    START transaction;

    -- 송금자와 수신자의 잔액을 잠금
    SELECT amount INTO sender_amount FROM `balances` WHERE id = sender_id FOR UPDATE;
    SELECT amount INTO receiver_amount FROM `balances` WHERE id = receiver_id FOR UPDATE;

    -- 환전소가 아닐때
    IF sender_id IS NOT NULL THEN
        -- 송금자 잔액이 NULL 여부 판단
        IF sender_amount IS NULL THEN
            ROLLBACK;  -- 송금자를 찾을수 없어 트랜잭션 롤백
            -- (HWR1001) 송금자ID가 잘못되었습니다.
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'HWR1001';
            
        END IF;
    
        -- 송금자의 잔액 확인 및 송금 가능 여부 판단
        IF sender_amount < transfer_amount THEN
            ROLLBACK;  -- 잔액 부족으로 트랜잭션 롤백
            -- (HWR1005) 송금자의 잔액이 부족합니다.
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'HWR1005';
        END IF;
    END IF;
    
    -- 수신자 잔액이 NULL 여부 판단
    IF receiver_amount IS NULL THEN
        ROLLBACK;  -- 수신자를 찾을수 없어 트랜잭션 롤백
        -- (HWR1002) 수신자ID가 잘못되었습니다.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'HWR1002';
    END IF;

    -- 송금자의 잔액을 송금액 만큼 감소
	 if sender_id IS NOT NULL THEN
        UPDATE `balances` SET amount = amount - transfer_amount WHERE id = sender_id;
    
	     -- 금액 무결성 검증
        IF ROW_COUNT() < 1 THEN
            ROLLBACK; -- 변경된 행이 없다면 트랜젝션 롤백
            -- (HWR1203) 트랜잭션에 실패하였습니다. (송금자 금액을 업데이트하지 못했습니다.)
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'HWR1203';
        END IF;
    END IF;
    
    -- 수신자의 잔액을 송금액 만큼 증가
    UPDATE `balances` SET amount = amount + transfer_amount WHERE id = receiver_id;
      
    -- 금액 무결성 검증
    IF ROW_COUNT() < 1 THEN
        ROLLBACK; -- 변경된 행이 없다면 트랜젝션 롤백
        -- (HWR1204) 트랜잭션에 실패하였습니다. (수신자 금액을 업데이트하지 못했습니다.)
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'HWR1204';
    END IF;
     
	 -- 트랜잭션 로그 추가
	 INSERT INTO `transactions` (`sender_id`, `receiver_id`, `amount`, `comment`)
	     VALUES (sender_id, receiver_id, transfer_amount, message);
	 
	 -- [SET OUT PARAMETER]
	 -- 트랜잭션 고유번호 설정
	 SET transaction_id := LAST_INSERT_ID();
	 -- 트랜잭션 시간 설정
	 SELECT `time` INTO transaction_time FROM `transactions` WHERE id = transaction_id;
	 -- 송수신자 잔액
    SELECT amount INTO sender_amount FROM `balances` WHERE id = sender_id;
    SELECT amount INTO receiver_amount FROM `balances` WHERE id = receiver_id;
	 
    -- 트랜잭션 커밋
    COMMIT;
END//
DELIMITER ;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
