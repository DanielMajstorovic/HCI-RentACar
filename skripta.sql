CREATE DATABASE  IF NOT EXISTS `rentacar` /*!40100 DEFAULT CHARACTER SET utf8mb3 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `rentacar`;
-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: localhost    Database: rentacar
-- ------------------------------------------------------
-- Server version	8.0.36

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `administrator`
--

DROP TABLE IF EXISTS `administrator`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `administrator` (
  `KORISNIK_idKORISNIK` int NOT NULL,
  PRIMARY KEY (`KORISNIK_idKORISNIK`),
  CONSTRAINT `fk_ADMINISTRATOR_KORISNIK1` FOREIGN KEY (`KORISNIK_idKORISNIK`) REFERENCES `korisnik` (`idKORISNIK`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `administrator`
--

LOCK TABLES `administrator` WRITE;
/*!40000 ALTER TABLE `administrator` DISABLE KEYS */;
/*!40000 ALTER TABLE `administrator` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `agent`
--

DROP TABLE IF EXISTS `agent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `agent` (
  `KORISNIK_idKORISNIK` int NOT NULL,
  `Plata` decimal(6,2) DEFAULT NULL,
  PRIMARY KEY (`KORISNIK_idKORISNIK`),
  CONSTRAINT `fk_AGENT_KORISNIK1` FOREIGN KEY (`KORISNIK_idKORISNIK`) REFERENCES `korisnik` (`idKORISNIK`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `agent`
--

LOCK TABLES `agent` WRITE;
/*!40000 ALTER TABLE `agent` DISABLE KEYS */;
/*!40000 ALTER TABLE `agent` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `iznajmljivanje`
--

DROP TABLE IF EXISTS `iznajmljivanje`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `iznajmljivanje` (
  `idIZNAJMLJIVANJE` int NOT NULL AUTO_INCREMENT,
  `AGENT_KORISNIK_idKORISNIK` int NOT NULL,
  `VOZILO_KATEGORIJA_VOZILA_idKATEGORIJA_VOZILA` int NOT NULL,
  `VOZILO_idVOZILO` int NOT NULL,
  `Cijena` decimal(6,2) NOT NULL,
  `DatumIznajmljivanja` date NOT NULL,
  `DatumVracanja` date DEFAULT NULL,
  `Klijent_idKlijent` int NOT NULL,
  `DodatniOpis` varchar(512) DEFAULT NULL,
  `StatusIznajmljivanja_idStatusIznajmljivanja` int NOT NULL,
  PRIMARY KEY (`idIZNAJMLJIVANJE`),
  KEY `fk_IZNAJMLJIVANJE_AGENT1_idx` (`AGENT_KORISNIK_idKORISNIK`),
  KEY `fk_IZNAJMLJIVANJE_VOZILO1_idx` (`VOZILO_idVOZILO`),
  KEY `fk_IZNAJMLJIVANJE_Klijent1_idx` (`Klijent_idKlijent`),
  KEY `fk_IZNAJMLJIVANJE_StatusIznajmljivanja1_idx` (`StatusIznajmljivanja_idStatusIznajmljivanja`),
  CONSTRAINT `fk_IZNAJMLJIVANJE_AGENT1` FOREIGN KEY (`AGENT_KORISNIK_idKORISNIK`) REFERENCES `agent` (`KORISNIK_idKORISNIK`),
  CONSTRAINT `fk_IZNAJMLJIVANJE_Klijent1` FOREIGN KEY (`Klijent_idKlijent`) REFERENCES `klijent` (`idKlijent`) ON DELETE CASCADE,
  CONSTRAINT `fk_IZNAJMLJIVANJE_StatusIznajmljivanja1` FOREIGN KEY (`StatusIznajmljivanja_idStatusIznajmljivanja`) REFERENCES `statusiznajmljivanja` (`idStatusIznajmljivanja`),
  CONSTRAINT `fk_IZNAJMLJIVANJE_VOZILO1` FOREIGN KEY (`VOZILO_idVOZILO`) REFERENCES `vozilo` (`idVOZILO`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `iznajmljivanje`
--

LOCK TABLES `iznajmljivanje` WRITE;
/*!40000 ALTER TABLE `iznajmljivanje` DISABLE KEYS */;
/*!40000 ALTER TABLE `iznajmljivanje` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `kategorija_vozila`
--

DROP TABLE IF EXISTS `kategorija_vozila`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `kategorija_vozila` (
  `idKATEGORIJA_VOZILA` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`idKATEGORIJA_VOZILA`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `kategorija_vozila`
--

LOCK TABLES `kategorija_vozila` WRITE;
/*!40000 ALTER TABLE `kategorija_vozila` DISABLE KEYS */;
/*!40000 ALTER TABLE `kategorija_vozila` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `klijent`
--

DROP TABLE IF EXISTS `klijent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `klijent` (
  `idKlijent` int NOT NULL AUTO_INCREMENT,
  `Ime` varchar(45) NOT NULL,
  `Prezime` varchar(45) NOT NULL,
  `BrojTelefona` varchar(45) NOT NULL,
  `Email` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`idKlijent`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `klijent`
--

LOCK TABLES `klijent` WRITE;
/*!40000 ALTER TABLE `klijent` DISABLE KEYS */;
/*!40000 ALTER TABLE `klijent` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `korisnik`
--

DROP TABLE IF EXISTS `korisnik`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `korisnik` (
  `idKORISNIK` int NOT NULL AUTO_INCREMENT,
  `KorisnickoIme` varchar(64) NOT NULL,
  `Lozinka` varchar(64) NOT NULL,
  `EPosta` varchar(128) NOT NULL,
  `TipNaloga` varchar(64) NOT NULL,
  `MJESTO_idMJESTO` int NOT NULL,
  `Tema` int DEFAULT NULL,
  `Jezik` int DEFAULT NULL,
  PRIMARY KEY (`idKORISNIK`),
  UNIQUE KEY `KorisnickoIme_UNIQUE` (`KorisnickoIme`),
  KEY `fk_KORISNIK_MJESTO1_idx` (`MJESTO_idMJESTO`),
  CONSTRAINT `fk_KORISNIK_MJESTO1` FOREIGN KEY (`MJESTO_idMJESTO`) REFERENCES `mjesto` (`idMJESTO`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `korisnik`
--

LOCK TABLES `korisnik` WRITE;
/*!40000 ALTER TABLE `korisnik` DISABLE KEYS */;
/*!40000 ALTER TABLE `korisnik` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `mjesto`
--

DROP TABLE IF EXISTS `mjesto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `mjesto` (
  `idMJESTO` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(45) NOT NULL,
  PRIMARY KEY (`idMJESTO`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `mjesto`
--

LOCK TABLES `mjesto` WRITE;
/*!40000 ALTER TABLE `mjesto` DISABLE KEYS */;
/*!40000 ALTER TABLE `mjesto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `opcija`
--

DROP TABLE IF EXISTS `opcija`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `opcija` (
  `idOPCIJA` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(45) NOT NULL,
  `Opis` varchar(45) DEFAULT NULL,
  `Cijena` decimal(5,2) NOT NULL,
  PRIMARY KEY (`idOPCIJA`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `opcija`
--

LOCK TABLES `opcija` WRITE;
/*!40000 ALTER TABLE `opcija` DISABLE KEYS */;
/*!40000 ALTER TABLE `opcija` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `opcija_na_iznajmljivanju`
--

DROP TABLE IF EXISTS `opcija_na_iznajmljivanju`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `opcija_na_iznajmljivanju` (
  `OPCIJA_idOPCIJA` int NOT NULL,
  `IZNAJMLJIVANJE_idIZNAJMLJIVANJE` int NOT NULL,
  `idOPCIJA_NA_IZNAJMLJIVANJU` int NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`idOPCIJA_NA_IZNAJMLJIVANJU`),
  KEY `fk_OPCIJA_NA_IZNAJMLJIVANJU_IZNAJMLJIVANJE1_idx` (`IZNAJMLJIVANJE_idIZNAJMLJIVANJE`),
  KEY `fk_OPCIJA_NA_IZNAJMLJIVANJU_OPCIJA1` (`OPCIJA_idOPCIJA`),
  CONSTRAINT `fk_OPCIJA_NA_IZNAJMLJIVANJU_IZNAJMLJIVANJE1` FOREIGN KEY (`IZNAJMLJIVANJE_idIZNAJMLJIVANJE`) REFERENCES `iznajmljivanje` (`idIZNAJMLJIVANJE`) ON DELETE CASCADE,
  CONSTRAINT `fk_OPCIJA_NA_IZNAJMLJIVANJU_OPCIJA1` FOREIGN KEY (`OPCIJA_idOPCIJA`) REFERENCES `opcija` (`idOPCIJA`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `opcija_na_iznajmljivanju`
--

LOCK TABLES `opcija_na_iznajmljivanju` WRITE;
/*!40000 ALTER TABLE `opcija_na_iznajmljivanju` DISABLE KEYS */;
/*!40000 ALTER TABLE `opcija_na_iznajmljivanju` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `problem`
--

DROP TABLE IF EXISTS `problem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `problem` (
  `idPROBLEM` int NOT NULL AUTO_INCREMENT,
  `OpisProblema` varchar(256) NOT NULL,
  `DatumKreiranja` datetime NOT NULL,
  `ADMINISTRATOR_KORISNIK_idKORISNIK` int DEFAULT NULL,
  `AGENT_KORISNIK_idKORISNIK` int NOT NULL,
  `PovratneInformacije` varchar(256) DEFAULT NULL,
  `DatumObrade` datetime DEFAULT NULL,
  PRIMARY KEY (`idPROBLEM`),
  KEY `fk_PROBLEM_ADMINISTRATOR1_idx` (`ADMINISTRATOR_KORISNIK_idKORISNIK`),
  KEY `fk_PROBLEM_AGENT1_idx` (`AGENT_KORISNIK_idKORISNIK`),
  CONSTRAINT `fk_PROBLEM_ADMINISTRATOR1` FOREIGN KEY (`ADMINISTRATOR_KORISNIK_idKORISNIK`) REFERENCES `administrator` (`KORISNIK_idKORISNIK`),
  CONSTRAINT `fk_PROBLEM_AGENT1` FOREIGN KEY (`AGENT_KORISNIK_idKORISNIK`) REFERENCES `agent` (`KORISNIK_idKORISNIK`) ON DELETE CASCADE,
  CONSTRAINT `FK_Problem_AgentKorisnik` FOREIGN KEY (`AGENT_KORISNIK_idKORISNIK`) REFERENCES `korisnik` (`idKORISNIK`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `problem`
--

LOCK TABLES `problem` WRITE;
/*!40000 ALTER TABLE `problem` DISABLE KEYS */;
/*!40000 ALTER TABLE `problem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `statusiznajmljivanja`
--

DROP TABLE IF EXISTS `statusiznajmljivanja`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `statusiznajmljivanja` (
  `idStatusIznajmljivanja` int NOT NULL AUTO_INCREMENT,
  `NazivStatusa` varchar(64) NOT NULL,
  PRIMARY KEY (`idStatusIznajmljivanja`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `statusiznajmljivanja`
--

LOCK TABLES `statusiznajmljivanja` WRITE;
/*!40000 ALTER TABLE `statusiznajmljivanja` DISABLE KEYS */;
/*!40000 ALTER TABLE `statusiznajmljivanja` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `telefon`
--

DROP TABLE IF EXISTS `telefon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `telefon` (
  `Telefon` varchar(20) NOT NULL,
  `KORISNIK_idKORISNIK` int NOT NULL,
  PRIMARY KEY (`KORISNIK_idKORISNIK`,`Telefon`),
  KEY `fk_TELEFON_KORISNIK1_idx` (`KORISNIK_idKORISNIK`),
  CONSTRAINT `fk_TELEFON_KORISNIK1` FOREIGN KEY (`KORISNIK_idKORISNIK`) REFERENCES `korisnik` (`idKORISNIK`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `telefon`
--

LOCK TABLES `telefon` WRITE;
/*!40000 ALTER TABLE `telefon` DISABLE KEYS */;
/*!40000 ALTER TABLE `telefon` ENABLE KEYS */;
UNLOCK TABLES;


--
-- Table structure for table `vozilo`
--

DROP TABLE IF EXISTS `vozilo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vozilo` (
  `idVOZILO` int NOT NULL AUTO_INCREMENT,
  `KATEGORIJA_VOZILA_idKATEGORIJA_VOZILA` int NOT NULL,
  `Marka` varchar(20) NOT NULL,
  `Model` varchar(20) NOT NULL,
  `Godiste` year NOT NULL,
  `Snaga` int NOT NULL,
  `DnevnaTarifa` decimal(5,2) NOT NULL,
  `SedmicnaTarifa` decimal(6,2) NOT NULL,
  PRIMARY KEY (`idVOZILO`),
  KEY `fk_VOZILO_KATEGORIJA_VOZILA1_idx` (`KATEGORIJA_VOZILA_idKATEGORIJA_VOZILA`),
  CONSTRAINT `fk_VOZILO_KATEGORIJA_VOZILA1` FOREIGN KEY (`KATEGORIJA_VOZILA_idKATEGORIJA_VOZILA`) REFERENCES `kategorija_vozila` (`idKATEGORIJA_VOZILA`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vozilo`
--

LOCK TABLES `vozilo` WRITE;
/*!40000 ALTER TABLE `vozilo` DISABLE KEYS */;
/*!40000 ALTER TABLE `vozilo` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-04-09 17:11:15
