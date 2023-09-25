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

-- Dumping structure for table hanum.EoullimPayments
CREATE TABLE IF NOT EXISTS `EoullimPayments` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT COMMENT '결제고유변호',
  `userId` bigint(20) unsigned NOT NULL COMMENT '결제자',
  `boothId` bigint(20) unsigned NOT NULL COMMENT '결제대상',
  `userBalanceId` bigint(20) unsigned NOT NULL COMMENT '걸제자잔고고유번호',
  `boothBalanceId` bigint(20) unsigned NOT NULL COMMENT '결제대상잔고고유번호',
  `paidAmount` bigint(20) unsigned NOT NULL COMMENT '결제금액',
  `refundedAmount` bigint(20) unsigned DEFAULT NULL COMMENT '결제취소금액',
  `paymentTransactionId` bigint(20) unsigned NOT NULL COMMENT '결제트랜잭션고유번호',
  `refundTransactionId` bigint(20) unsigned DEFAULT NULL COMMENT '환불트랜잭션고유번호',
  `paymentComment` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL COMMENT '결제메시지',
  `refundComment` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL COMMENT '환불메시지',
  `status` enum('paid','refunded') CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '결제상태',
  `paidTime` datetime NOT NULL DEFAULT current_timestamp() COMMENT '결제시간',
  `refundedTime` datetime DEFAULT NULL COMMENT '결제취소시간',
  PRIMARY KEY (`id`),
  KEY `EOULLIM_PAYMENTS_BOOTH_BALANCE_ID_FK` (`boothBalanceId`),
  KEY `EOULLIM_PAYMENTS_BOOTH_ID_FK` (`boothId`),
  KEY `EOULLIM_PAYMENTS_USER_BALANCE_ID_FK` (`userBalanceId`),
  KEY `EOULLIM_PAYMENTS_USER_ID_FK` (`userId`),
  CONSTRAINT `EOULLIM_PAYMENTS_BOOTH_BALANCE_ID_FK` FOREIGN KEY (`boothBalanceId`) REFERENCES `EoullimBalances` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `EOULLIM_PAYMENTS_BOOTH_ID_FK` FOREIGN KEY (`boothId`) REFERENCES `EoullimBooths` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `EOULLIM_PAYMENTS_USER_BALANCE_ID_FK` FOREIGN KEY (`userBalanceId`) REFERENCES `EoullimBalances` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `EOULLIM_PAYMENTS_USER_ID_FK` FOREIGN KEY (`userId`) REFERENCES `users` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='한세어울림한마당 결제내역';

-- Data exporting was unselected.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
