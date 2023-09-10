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

-- Dumping structure for procedure hanum.EoullimTransaction
DELIMITER //
CREATE PROCEDURE `EoullimTransaction`(
	IN `senderId` BIGINT UNSIGNED,
	IN `receiverId` BIGINT UNSIGNED,
	IN `transferAmount` BIGINT UNSIGNED,
	IN `message` VARCHAR(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
	OUT `transactionId` BIGINT UNSIGNED,
	OUT `transactionTime` DATETIME,
	OUT `senderAmount` BIGINT UNSIGNED,
	OUT `receiverAmount` BIGINT UNSIGNED
)
    COMMENT '한세어울림한마당 잔고 이체'
BEGIN
    IF senderId = receiverId THEN
        -- 송금자와 수신자가 일치합니다.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'SENDER_ID_EQUALS_RECEIVER_ID';
    END IF;
    
    IF transferAmount <= 0 THEN
    	  -- 송금액이 올바른지 확인하십시오.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'INVALID_TRANSFER_AMOUNT';
    END IF;
    
    -- 트랜잭션 시작
    START transaction;

    -- 송금자와 수신자의 잔액을 잠금
    SELECT amount INTO senderAmount FROM `EoullimBalances` WHERE id = senderId FOR UPDATE;
    SELECT amount INTO receiverAmount FROM `EoullimBalances` WHERE id = receiverId FOR UPDATE;

    -- 환전소(SENDER_ID IS NULL)가 아닐때
    IF senderId IS NOT NULL THEN
        -- 송금자가 존재하는지 여부 판단
        IF senderAmount IS NULL THEN
            ROLLBACK;
            -- 송금자ID가 잘못되었습니다.
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'INVALID_SENDER_ID';
        END IF;
    
        -- 송금자의 잔액 확인, 송금 가능 여부 판단
        IF senderAmount < transferAmount THEN
            ROLLBACK;
            -- 송금자의 잔액이 부족합니다.
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'INSUFFICIENT_SENDER_BALANCE';
        END IF;
    END IF;
    
    -- 수신자 잔액 NULL 여부 판단
    IF receiverAmount IS NULL THEN
        ROLLBACK;
        -- 수신자ID가 잘못되었습니다.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'INVALID_RECEIVER_ID';
    END IF;

	 -- 환전소가 아닐 때
	 IF senderId IS NOT NULL THEN
        -- 송금자의 잔액을 송금액 만큼 감소
        UPDATE `EoullimBalances` SET amount = amount - transferAmount WHERE id = senderId;
    
	     -- 무결성 검증
        IF ROW_COUNT() < 1 THEN
            ROLLBACK;
            -- 송금자 금액을 업데이트하지 못했습니다.
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'SENDER_BALANCE_NOT_UPDATED';
        END IF;
    END IF;
    
    -- 수신자의 잔액을 송금액 만큼 증가
    UPDATE `EoullimBalances` SET amount = amount + transferAmount WHERE id = receiverId;
      
    -- 무결성 검증
    IF ROW_COUNT() < 1 THEN
        ROLLBACK;
        -- 수신자 금액을 업데이트하지 못했습니다.
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'RECEIVER_BALANCE_NOT_UPDATED';
    END IF;
     
	 -- 트랜잭션 로그 추가
	 SET transactionTime := CURRENT_TIMESTAMP();
	 
	 INSERT INTO `EoullimTransactions` (`senderId`, `receiverId`, `amount`, `comment`, `time`)
	     VALUES (senderId, receiverId, transferAmount, message, transactionTime);
	 
	 -- [SET OUT PARAMETER]
	 -- 트랜잭션 고유번호 설정
	 SET transactionId := LAST_INSERT_ID();
	 -- 송수신자 잔액
    SELECT amount INTO senderAmount FROM `EoullimBalances` WHERE id = senderId;
    SELECT amount INTO receiverAmount FROM `EoullimBalances` WHERE id = receiverId;
	 
    -- 트랜잭션 커밋
    COMMIT;
END//
DELIMITER ;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
