CREATE TABLE `Files` (
	`FileId` BIGINT(20) NOT NULL AUTO_INCREMENT,
	`CreationTimeUtc` DATETIME NOT NULL,
	`LastAccessTimeUtc` DATETIME NOT NULL,
	`LastWriteTimeUtc` DATETIME NOT NULL,
	`FileSize` BIGINT(20) NOT NULL DEFAULT '0',
	`Extension` VARCHAR(16) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	`FileName` VARCHAR(256) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	`PathSegment01` VARCHAR(128) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	`PathSegment02` VARCHAR(128) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	`PathSegment03` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment04` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment05` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment06` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment07` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment08` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment09` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment10` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment11` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment12` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment13` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment14` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`PathSegment15` VARCHAR(128) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`DirectoryName` VARCHAR(1024) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	PRIMARY KEY (`FileId`) USING BTREE,
	INDEX `Extension` (`Extension`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
