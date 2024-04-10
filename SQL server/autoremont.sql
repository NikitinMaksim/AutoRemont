/*
Navicat MySQL Data Transfer

Source Server         : localhost_3306
Source Server Version : 50525
Source Host           : localhost:3306
Source Database       : autoremont

Target Server Type    : MYSQL
Target Server Version : 50525
File Encoding         : 65001

Date: 2023-06-18 18:07:43
*/

CREATE DATABASE IF NOT EXISTS autoremont CHARACTER SET utf8 COLLATE utf8_general_ci;

USE autoremont;

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for authorization
-- ----------------------------
DROP TABLE IF EXISTS `authorization`;
CREATE TABLE `authorization` (
`ID_of_user`  int(11) NOT NULL AUTO_INCREMENT ,
`Login`  varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL ,
`Password`  varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL ,
`ID_of_worker`  int(11) NOT NULL ,
`Level_of_access`  tinyint(4) NULL DEFAULT NULL ,
PRIMARY KEY (`ID_of_user`, `Login`, `Password`),
FOREIGN KEY (`ID_of_worker`) REFERENCES `workers` (`ID_of_worker`) ON DELETE CASCADE ON UPDATE CASCADE,
INDEX `FK_ID_of_worker` (`ID_of_worker`) USING BTREE 
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=5

;

-- ----------------------------
-- Records of authorization
-- ----------------------------
BEGIN;
INSERT INTO `authorization` VALUES ('1', 'Admin', 'Password', '4', '0'), ('2', 'User1', 'User1Pass', '1', '1'), ('3', 'User2', 'User2Pass', '5', '2'), ('4', 'User3', 'User3pass', '3', '3');
COMMIT;

-- ----------------------------
-- Table structure for orders
-- ----------------------------
DROP TABLE IF EXISTS `orders`;
CREATE TABLE `orders` (
`ID_of_order`  int(11) NOT NULL AUTO_INCREMENT ,
`Date_of_complition`  date NOT NULL ,
`Name_of_client`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Surname_of_client`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Phone_number_of_client`  varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Is_order_complete`  bit(1) NOT NULL ,
`Car_number`  varchar(15) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL ,
PRIMARY KEY (`ID_of_order`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=17

;

-- ----------------------------
-- Records of orders
-- ----------------------------
BEGIN;
INSERT INTO `orders` VALUES ('14', '2023-06-15', 'Иван', 'Иванов', '+1(111) 111-1111', '\0', 'о777оо777rus'), ('15', '2023-06-30', 'Эдуард', 'Жуков', '+2(222) 222-2222', '', 'э111ээ111rus');
COMMIT;

-- ----------------------------
-- Table structure for orders-services
-- ----------------------------
DROP TABLE IF EXISTS `orders-services`;
CREATE TABLE `orders-services` (
`ID_of_order`  int(11) NULL DEFAULT NULL ,
`ID_of_service`  int(11) NULL DEFAULT NULL ,
FOREIGN KEY (`ID_of_order`) REFERENCES `orders` (`ID_of_order`) ON DELETE CASCADE ON UPDATE NO ACTION,
FOREIGN KEY (`ID_of_service`) REFERENCES `services` (`ID_of_service`) ON DELETE CASCADE ON UPDATE NO ACTION,
INDEX `FK_IF_of_service` (`ID_of_service`) USING BTREE ,
INDEX `FK_ID_of_order` (`ID_of_order`) USING BTREE 
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci

;

-- ----------------------------
-- Records of orders-services
-- ----------------------------
BEGIN;
INSERT INTO `orders-services` VALUES ('14', '1'), ('14', '4'), ('15', '1');
COMMIT;

-- ----------------------------
-- Table structure for orders-workers
-- ----------------------------
DROP TABLE IF EXISTS `orders-workers`;
CREATE TABLE `orders-workers` (
`ID_of_order`  int(11) NOT NULL ,
`ID_of_responsible`  int(11) NOT NULL ,
PRIMARY KEY (`ID_of_order`, `ID_of_responsible`),
FOREIGN KEY (`ID_of_responsible`) REFERENCES `workers` (`ID_of_worker`) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (`ID_of_order`) REFERENCES `orders` (`ID_of_order`) ON DELETE CASCADE ON UPDATE CASCADE,
INDEX `FK_ID_of_responsible` (`ID_of_responsible`) USING BTREE 
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci

;

-- ----------------------------
-- Records of orders-workers
-- ----------------------------
BEGIN;
INSERT INTO `orders-workers` VALUES ('14', '1'), ('15', '2');
COMMIT;

-- ----------------------------
-- Table structure for services
-- ----------------------------
DROP TABLE IF EXISTS `services`;
CREATE TABLE `services` (
`ID_of_service`  int(11) NOT NULL AUTO_INCREMENT ,
`Name_of_service`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL ,
`Price`  int(7) NOT NULL ,
`Recommended_time_days`  tinyint(4) NOT NULL ,
PRIMARY KEY (`ID_of_service`, `Name_of_service`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=6

;

-- ----------------------------
-- Records of services
-- ----------------------------
BEGIN;
INSERT INTO `services` VALUES ('1', 'Замена шины', '1000', '1'), ('2', 'Замена лобового стекла', '4500', '1'), ('4', 'Замена масла', '500', '1'), ('5', 'Замена руля', '2000', '2');
COMMIT;

-- ----------------------------
-- Table structure for storage
-- ----------------------------
DROP TABLE IF EXISTS `storage`;
CREATE TABLE `storage` (
`ID_of_part`  int(11) NOT NULL AUTO_INCREMENT ,
`Name_of_part`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL ,
`Remaining_amount`  int(11) NULL DEFAULT NULL ,
PRIMARY KEY (`ID_of_part`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=13

;

-- ----------------------------
-- Records of storage
-- ----------------------------
BEGIN;
INSERT INTO `storage` VALUES ('1', 'Гайки', '50'), ('2', 'Болты', '50'), ('3', 'Машинное масло(Литр)', '15'), ('7', 'Карбюратор', '5'), ('8', 'Лобовое стекло', '5'), ('9', 'Шины', '20'), ('10', 'Руль', '10'), ('11', 'Педаль', '5');
COMMIT;

-- ----------------------------
-- Table structure for storage-service
-- ----------------------------
DROP TABLE IF EXISTS `storage-service`;
CREATE TABLE `storage-service` (
`ID_of_link`  int(10) UNSIGNED NOT NULL AUTO_INCREMENT ,
`ID_of_service`  int(11) NOT NULL ,
`ID_of_part`  int(11) NOT NULL ,
`Required_amount`  tinyint(3) UNSIGNED NULL DEFAULT NULL ,
PRIMARY KEY (`ID_of_link`),
FOREIGN KEY (`ID_of_part`) REFERENCES `storage` (`ID_of_part`) ON DELETE NO ACTION ON UPDATE NO ACTION,
FOREIGN KEY (`ID_of_service`) REFERENCES `services` (`ID_of_service`) ON DELETE NO ACTION ON UPDATE NO ACTION,
INDEX `FK_ID_of_service` (`ID_of_service`) USING BTREE ,
INDEX `FK_ID_of_part` (`ID_of_part`) USING BTREE 
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=7

;

-- ----------------------------
-- Records of storage-service
-- ----------------------------
BEGIN;
INSERT INTO `storage-service` VALUES ('1', '1', '9', '1'), ('2', '2', '2', '5'), ('3', '2', '8', '1'), ('5', '5', '10', '1'), ('6', '4', '3', '3');
COMMIT;

-- ----------------------------
-- Table structure for workers
-- ----------------------------
DROP TABLE IF EXISTS `workers`;
CREATE TABLE `workers` (
`ID_of_worker`  int(11) NOT NULL AUTO_INCREMENT ,
`Profession`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL ,
`Name`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL ,
`Surname`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL ,
`Salary`  int(11) NOT NULL ,
PRIMARY KEY (`ID_of_worker`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=6

;

-- ----------------------------
-- Records of workers
-- ----------------------------
BEGIN;
INSERT INTO `workers` VALUES ('1', 'Механик', 'Иван', 'Иванов', '30000'), ('2', 'Механик', 'Сергей', 'Сергеевич', '30000'), ('3', 'Бухгалтер', 'Ольга', 'Сергеевна', '40000'), ('4', 'Администратор', 'Максим', 'Никитин', '40000'), ('5', 'Кассир', 'Наталья', 'Викторовна', '25000');
COMMIT;

-- ----------------------------
-- Auto increment value for authorization
-- ----------------------------
ALTER TABLE `authorization` AUTO_INCREMENT=5;

-- ----------------------------
-- Auto increment value for orders
-- ----------------------------
ALTER TABLE `orders` AUTO_INCREMENT=17;

-- ----------------------------
-- Auto increment value for services
-- ----------------------------
ALTER TABLE `services` AUTO_INCREMENT=6;

-- ----------------------------
-- Auto increment value for storage
-- ----------------------------
ALTER TABLE `storage` AUTO_INCREMENT=13;

-- ----------------------------
-- Auto increment value for storage-service
-- ----------------------------
ALTER TABLE `storage-service` AUTO_INCREMENT=7;

-- ----------------------------
-- Auto increment value for workers
-- ----------------------------
ALTER TABLE `workers` AUTO_INCREMENT=6;
SET FOREIGN_KEY_CHECKS=1;
