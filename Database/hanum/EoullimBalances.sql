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

-- Dumping structure for table hanum.EoullimBalances
CREATE TABLE IF NOT EXISTS `EoullimBalances` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT COMMENT '잔고 고유 ID',
  `userId` bigint(20) unsigned DEFAULT NULL COMMENT '사용자 ID',
  `boothId` bigint(20) unsigned DEFAULT NULL COMMENT '부스 ID',
  `amount` bigint(20) unsigned NOT NULL DEFAULT 0 COMMENT '잔고 정산 후 총 잔액',
  `type` enum('personal','booth') NOT NULL DEFAULT 'personal' COMMENT '잔고 분류',
  `comment` varchar(24) DEFAULT NULL COMMENT '잔고 메모',
  PRIMARY KEY (`id`),
  UNIQUE KEY `user_id` (`userId`) USING BTREE,
  UNIQUE KEY `booth_id` (`boothId`) USING BTREE,
  CONSTRAINT `BOOTH_ID_FK` FOREIGN KEY (`boothId`) REFERENCES `EoullimBooths` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `USER_ID_FK` FOREIGN KEY (`userId`) REFERENCES `users` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin COMMENT='한세어울림한마당 잔고';

-- Data exporting was unselected.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
