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

-- Dumping structure for table hanum.EoullimTransactions
CREATE TABLE IF NOT EXISTS `EoullimTransactions` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT COMMENT '트랜잭션 고유 ID',
  `senderId` bigint(20) unsigned DEFAULT NULL COMMENT '송금자 ID, 환전소의 경우 NULL로 설정',
  `receiverId` bigint(20) unsigned NOT NULL COMMENT '수신자 ID',
  `amount` bigint(20) unsigned NOT NULL COMMENT '송금액',
  `comment` varchar(24) DEFAULT NULL COMMENT '송금 메모',
  `time` datetime NOT NULL DEFAULT current_timestamp() COMMENT '트랜잭션 시간',
  PRIMARY KEY (`id`),
  KEY `SENDER_BALANCE_FK` (`senderId`) USING BTREE,
  KEY `RECEIVER_BALANCE_FK` (`receiverId`) USING BTREE,
  CONSTRAINT `RECEIVER_BALANCE_FK` FOREIGN KEY (`receiverId`) REFERENCES `EoullimBalances` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `SENDER_BALANCE_FK` FOREIGN KEY (`senderId`) REFERENCES `EoullimBalances` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='한세어울림한마당 이체 내역';

-- Data exporting was unselected.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
