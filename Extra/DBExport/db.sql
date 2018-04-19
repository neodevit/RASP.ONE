-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: 192.168.1.69    Database: SassoSec
-- ------------------------------------------------------
-- Server version	5.5.5-10.0.32-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `50_RULES`
--

DROP TABLE IF EXISTS `50_RULES`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `50_RULES` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `NOME` varchar(50) NOT NULL,
  `DESCRIZIONE` varchar(255) NOT NULL,
  `UserIns` varchar(100) NOT NULL,
  `DataIns` datetime NOT NULL,
  `UserMod` varchar(100) NOT NULL,
  `DataMod` datetime NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `50_RULES`
--

LOCK TABLES `50_RULES` WRITE;
/*!40000 ALTER TABLE `50_RULES` DISABLE KEYS */;
INSERT INTO `50_RULES` VALUES (1,'prova','bla bla','','0000-00-00 00:00:00','','0000-00-00 00:00:00');
/*!40000 ALTER TABLE `50_RULES` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `51_RULES_ITEM`
--

DROP TABLE IF EXISTS `51_RULES_ITEM`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `51_RULES_ITEM` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `ID_RULE` int(11) NOT NULL,
  `Tipo` int(11) NOT NULL,
  `ID_Componente` int(11) NOT NULL,
  `POS` int(11) NOT NULL,
  `NOME` varchar(50) NOT NULL,
  `Condizione` int(11) NOT NULL,
  `Valore` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `51_RULES_ITEM`
--

LOCK TABLES `51_RULES_ITEM` WRITE;
/*!40000 ALTER TABLE `51_RULES_ITEM` DISABLE KEYS */;
INSERT INTO `51_RULES_ITEM` VALUES (1,1,0,5,1,'LUCE CANCELLO',1,NULL),(2,1,0,6,2,'PIR CANCELLO',4,'15000'),(3,1,1,5,3,'LUCE CANCELLO',0,NULL),(4,1,2,0,4,'',0,'5'),(5,1,1,5,5,'LUCE CANCELLO',1,NULL);
/*!40000 ALTER TABLE `51_RULES_ITEM` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `60_GPIO_PIN`
--

DROP TABLE IF EXISTS `60_GPIO_PIN`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `60_GPIO_PIN` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Tipo` int(11) NOT NULL,
  `GPIO` int(11) NOT NULL,
  `NUM` int(11) NOT NULL,
  `NomeGPIO` varchar(20) NOT NULL,
  `NomeNUM` varchar(20) DEFAULT NULL,
  `Descrizione` varchar(255) DEFAULT NULL,
  `UserIns` varchar(100) NOT NULL,
  `DataIns` datetime NOT NULL,
  `UserMod` varchar(100) NOT NULL,
  `DataMod` datetime NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `60_GPIO_PIN`
--

LOCK TABLES `60_GPIO_PIN` WRITE;
/*!40000 ALTER TABLE `60_GPIO_PIN` DISABLE KEYS */;
INSERT INTO `60_GPIO_PIN` VALUES (1,0,5,29,'GPIO 5',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(2,0,6,31,'GPIO 6',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(3,0,12,32,'GPIO 12',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(4,0,13,33,'GPIO 13',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(5,0,16,36,'GPIO 16',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(6,0,17,11,'GPIO 17',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(7,4,18,12,'GPIO 18 ','PCM CLK',NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(8,0,19,35,'GPIO 19',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(9,0,20,38,'GPIO 20',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(10,0,21,40,'GPIO 21',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(11,0,22,15,'GPIO 22',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(12,0,23,16,'GPIO 23',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(13,0,24,18,'GPIO 24',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(14,0,25,22,'GPIO 25',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(15,0,26,37,'GPIO 26',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(16,0,27,13,'GPIO 27',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(17,0,4,7,'GPIO 4',NULL,NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(18,1,0,1,'','3.3 V','Power 3.3 V','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(19,1,0,2,'','5 V','Power 5 V','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(20,4,2,3,'GPIO 2','SDA1 I2C',NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(21,1,0,4,'','5 V','Power 5 V','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(22,4,3,5,'GPIO 3','SCL1 I2C',NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(23,2,0,6,'','GND','Ground','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(24,4,14,8,'GPIO 14','UART T0 TXD',NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(25,2,0,9,'','GND',NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(26,4,15,10,'GPIO 15','UART T0 RXD',NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(27,2,0,14,'','GND','Ground','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(28,1,0,17,'','3.3 V','Power 3.3 V','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(29,4,10,19,'GPIO 10','SPI0 MOSI',NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(30,2,0,20,'','GND','Ground','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(31,4,9,21,'GPIO 9','SPI0 MISO',NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(32,4,11,23,'GPIO 11','SPI0 SCLK',NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(33,4,8,24,'GPIO 8','SPI0 CE0 N',NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(34,2,0,25,'','GND','Ground','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(35,4,7,26,'GPIO 7','SPI0 CE1 N',NULL,'','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(36,3,0,27,'','ID SD','I2C ID EEPROM','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(37,3,0,28,'','ID SC','I2C ID EEPROM','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(38,2,0,30,'','GND','Ground','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(39,2,0,34,'','GND','Ground','','0000-00-00 00:00:00','','0000-00-00 00:00:00'),(40,2,0,39,'','GND','Ground','','0000-00-00 00:00:00','','0000-00-00 00:00:00');
/*!40000 ALTER TABLE `60_GPIO_PIN` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `70_COMPONENTE`
--

DROP TABLE IF EXISTS `70_COMPONENTE`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `70_COMPONENTE` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Enabled` tinyint(4) NOT NULL,
  `Trusted` tinyint(1) NOT NULL,
  `Stato` int(11) DEFAULT NULL,
  `IDComponenteTipo` int(11) NOT NULL,
  `Nome` varchar(50) NOT NULL,
  `HostName` varchar(100) DEFAULT NULL,
  `Descrizione` varchar(255) DEFAULT NULL,
  `PositionLeft` double NOT NULL,
  `PositionTop` double NOT NULL,
  `PositionBottom` double NOT NULL,
  `PositionRight` double NOT NULL,
  `Node_Num` int(11) NOT NULL,
  `Node_Pin` int(11) NOT NULL,
  `Value` varchar(255) DEFAULT NULL,
  `repeat` tinyint(4) DEFAULT NULL,
  `repeatTime_sec` int(11) DEFAULT NULL,
  `IPv4` varchar(20) DEFAULT NULL,
  `IPv6` varchar(50) DEFAULT NULL,
  `BlueTooth` varchar(45) DEFAULT NULL,
  `HWAddress` varchar(30) DEFAULT NULL,
  `OSVersion` varchar(45) DEFAULT NULL,
  `NodeSWVersion` varchar(20) DEFAULT NULL,
  `SystemProductName` varchar(45) DEFAULT NULL,
  `SystemID` varchar(45) DEFAULT NULL,
  `Options` varchar(255) DEFAULT NULL,
  `UserIns` varchar(100) NOT NULL,
  `DataIns` datetime NOT NULL,
  `UserMod` varchar(100) NOT NULL,
  `DataMod` datetime NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `70_COMPONENTE`
--

LOCK TABLES `70_COMPONENTE` WRITE;
/*!40000 ALTER TABLE `70_COMPONENTE` DISABLE KEYS */;
INSERT INTO `70_COMPONENTE` VALUES (1,1,1,0,3,'NODO 2','','',356.03466796875,196.00473022460938,0,0,3,0,'',0,0,'192.168.1.102','','','','','','','','','Fabio','2018-03-16 23:25:18','Fabio','2018-04-14 02:01:30'),(2,1,1,NULL,3,'NODO 1','sasso_nodo_1','',400.030029296875,442.00628662109375,0,0,1,0,'',NULL,NULL,'192.168.1.101','','','\0R;\0L(\0w\0??\0o?\0Z&\0??	\0','10.0.16299.309','1.0.0.0','Raspberry Pi 3','22745ab9-2f91-48f7-0201-cde4504d2038','','Fabio','2018-03-16 23:29:28','sasso_nodo_1','2018-03-29 13:02:19'),(3,1,1,0,4,'CENTRALE ','LAPTOP-6M48D644','',307.032958984375,272.0330810546875,0,0,2,0,'',0,0,'192.168.1.10','','','\0?n\0/ \0f(\0%W\0f\0\0\0B,\0','10.0.16299.371','1.0.0.0','HP Pavilion Notebook','\0?n\0/ \0f(\0%W\0f\0\0\0B,\049\0??\0?\r\06	','','Fabio','2018-03-17 02:49:53','Fabio','2018-04-19 20:45:12'),(5,1,1,2,1,'LUCE CANCELLO','','',370.032958984375,429.0189208984375,0,0,1,26,'1',0,0,'192.168.1.101','','','','','','','','1','Fabio','2018-03-17 15:32:10','Fabio','2018-04-19 20:45:40'),(6,1,1,1,2,'PIR CANCELLO',NULL,'',365.02197265625,456.0189208984375,0,0,1,18,'2',0,0,'192.168.1.101','','','','','','','','1','Fabio','2018-03-17 15:39:19','Fabio','2018-04-19 20:45:44'),(7,1,1,NULL,1,'Luce Corridoio 1','','',383.018798828125,209.0299072265625,0,0,2,5,'1',NULL,NULL,'192.168.1.102','','','','','','','','0','Fabio','2018-03-17 17:56:12','Fabio','2018-03-28 22:49:07'),(18,1,1,0,2,'PIR ',NULL,'',274.0078125,406.00469970703125,0,0,4,6,'2',0,0,'192.168.1.104','','','','','','','','0','Fabio','2018-03-19 13:50:27','Fabio','2018-04-14 02:13:46'),(19,1,1,0,5,'Ingresso Locale',NULL,'',352.0125732421875,315.00628662109375,0,0,0,0,'http://192.168.1.2:88/cgi-bin/CGIProxy.fcgi?cmd=snapPicture2&usr=admin&pwd=',0,0,'192.168.1.2','','','','','','','','','Fabio','2018-03-24 03:57:39','Fabio','2018-04-08 21:34:41'),(20,1,1,0,5,'PORTA CASA','','',278.0361328125,202.0330810546875,0,0,0,0,'http://192.168.1.6/cgi-bin/videostream.cgi?user=admin&pwd=',0,0,'192.168.1.3','','','','','','','','','Fabio','2018-03-24 04:21:15','Fabio','2018-04-15 01:01:41'),(26,1,1,NULL,3,'NODO 4','','',131.031494140625,269.0157470703125,0,0,4,0,'',NULL,NULL,'192.168.1.104','','','','','','','','','Fabio','2018-03-25 15:58:36','Fabio','2018-03-28 14:03:45'),(27,1,1,NULL,3,'NODE 5','','',176.0283203125,335.0204772949219,0,0,5,0,'',NULL,NULL,'192.168.1.105','','','','','','','','','Fabio','2018-03-25 16:10:55','Fabio','2018-03-28 13:52:19'),(28,1,1,NULL,1,'Luce Corridoio 2',NULL,'',415.00634765625,207.0330810546875,0,0,2,6,'1',NULL,NULL,'192.168.1.102','','','','','','','','0','Fabio','2018-03-25 16:14:39','Fabio','2018-03-28 22:49:16'),(29,1,1,NULL,1,'Luce Corridoio 3','','',443.010986328125,207.02203369140625,0,0,2,26,'1',NULL,NULL,'192.168.1.102','','','','','','','','0','Fabio','2018-03-25 16:20:29','Fabio','2018-03-28 22:49:23'),(30,1,1,NULL,1,'Luce ingresso Locale',NULL,'',297.025146484375,339.017333984375,0,0,4,12,'1',NULL,NULL,'192.168.1.104','','','','','','','','0','Fabio','2018-03-25 16:43:36','Fabio','2018-03-28 22:48:18'),(31,1,1,1,7,'temperatura sala','','',320.02362060546875,298.00146484375,0,0,1,21,'23',1,300,'192.168.1.101','','','','','','','','1','Fabio','2018-04-06 09:17:51','Fabio','2018-04-19 20:36:24'),(32,1,1,0,3,'nodo 6',NULL,'',335.0078125,404.00946044921875,0,0,6,0,'',0,0,'192.168.1.106','','','','','','','','1','Fabio','2018-04-14 02:13:16','Fabio','2018-04-14 02:13:54'),(33,1,1,2,9,'cancello locale',NULL,'',303.02197265625,402.02520751953125,0,0,1,23,'',0,0,'192.168.1.101','','','','','','','','1','Fabio','2018-04-14 02:17:00','Fabio','2018-04-15 12:15:06'),(34,1,1,1,8,'umidità sala','','',383.17889404296875,289.6075744628906,0,0,1,21,'23,2',0,0,'192.168.1.101','','','','','','','','1','Fabio','2018-04-17 09:54:39','Fabio','2018-04-19 20:53:25');
/*!40000 ALTER TABLE `70_COMPONENTE` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'SassoSec'
--
/*!50003 DROP PROCEDURE IF EXISTS `SP30_SetPIR` */;
ALTER DATABASE `SassoSec` CHARACTER SET latin1 COLLATE latin1_general_ci ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `SP30_SetPIR`(INOUT `ID` INT(11), IN `Enabled` BIT, IN `Stato` INT(11), IN `Nome` VARCHAR(100), IN `Descrizione` VARCHAR(255), IN `Utente` VARCHAR(100))
    MODIFIES SQL DATA
    SQL SECURITY INVOKER
BEGIN

IF EXISTS( SELECT * FROM `30_PIR` WHERE `ID` = ID) THEN

		UPDATE `30_PIR`
		   SET `Enabled` = Enabled
			  ,`Stato` = Stato
			  ,`Nome` = Nome
			  ,`Descrizione` = Descrizione
			  ,`UserMod` = Utente
              ,`DataMod` = NOW()
		 WHERE `ID` = ID;
ELSE
		INSERT INTO 30_PIR
				    (`Enabled`
				    ,`Stato`
				    ,`Nome`
				    ,`Descrizione`
				    ,`UserIns`
				    ,`DataIns`
                    ,`UserMod`
				    ,`DataMod`)
			 VALUES
				    (Enabled
				    ,Stato
				    ,Nome
				    ,Descrizione
				    ,Utente
				    ,NOW()
                    ,Utente
                    ,NOW());
		select LAST_INSERT_ID() into ID;
END IF; 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
ALTER DATABASE `SassoSec` CHARACTER SET utf8 COLLATE utf8_general_ci ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-04-19 21:14:59
