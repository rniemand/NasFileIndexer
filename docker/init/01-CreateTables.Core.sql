CREATE TABLE `Files` (
	`FileId` BIGINT(20) NOT NULL AUTO_INCREMENT,
	`CreationTimeUtc` DATETIME NOT NULL,
	`LastAccessTimeUtc` DATETIME NULL DEFAULT NULL,
	`LastWriteTimeUtc` DATETIME NULL DEFAULT NULL,
	`FileSize` BIGINT(20) NOT NULL DEFAULT '0',
	`Extension` VARCHAR(16) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	`DirectoryName` VARCHAR(1024) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	`FileName` VARCHAR(256) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	PRIMARY KEY (`FileId`) USING BTREE,
	INDEX `Extension` (`Extension`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
