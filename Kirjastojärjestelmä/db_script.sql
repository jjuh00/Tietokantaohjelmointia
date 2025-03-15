-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema LibraryDB
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `LibraryDB` ;

-- -----------------------------------------------------
-- Schema LibraryDB
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `LibraryDB` DEFAULT CHARACTER SET utf8 ;
USE `LibraryDB` ;

-- -----------------------------------------------------
-- Table `LibraryDB`.`books`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `LibraryDB`.`books` ;

CREATE TABLE IF NOT EXISTS `LibraryDB`.`books` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(45) NOT NULL,
  `author` VARCHAR(45) NOT NULL,
  `isbn` VARCHAR(20) NOT NULL,
  `available` TINYINT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `isbn_UNIQUE` (`isbn` ASC) VISIBLE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `LibraryDB`.`members`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `LibraryDB`.`members` ;

CREATE TABLE IF NOT EXISTS `LibraryDB`.`members` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  `email` VARCHAR(45) NOT NULL,
  `phone` VARCHAR(15) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `LibraryDB`.`transactions`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `LibraryDB`.`transactions` ;

CREATE TABLE IF NOT EXISTS `LibraryDB`.`transactions` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `book_id` INT NOT NULL,
  `member_id` INT NOT NULL,
  `borrow_date` DATE NOT NULL,
  `due_date` DATE NOT NULL,
  `return_date` DATE NULL,
  `available` TINYINT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  CONSTRAINT `book_id`
    FOREIGN KEY (`id`)
    REFERENCES `LibraryDB`.`books` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `member_id`
    FOREIGN KEY (`id`)
    REFERENCES `LibraryDB`.`members` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

-- -----------------------------------------------------
-- Data for table `LibraryDB`.`books`
-- -----------------------------------------------------
START TRANSACTION;
USE `LibraryDB`;
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (1, 'Pikku Prinssi', 'Antoine de Saint-Exupéry', '9789519969851', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (2, 'Harry Potter ja kirottu lapsi', 'J. K. Rowling', '9789513192761', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (3, 'One Piece 2', 'Eiichiro Oda', '9788410258013', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (4, 'Chainsaw Man 4', 'Tatsuki Fujimoto', '9791387500108', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (5, 'Attack on Titan', 'Hajime Isayama', '9781612620244', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (6, 'Sinuhe egyptiläinen', 'Mika Waltari', '9789510098752', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (7, 'Tuntematon sotilas', 'Väinö Linna', '9789510510766', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (8, 'Täällä Pohjantähden alla', 'Väinö Linna', '9789510435724', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (9, 'Häräntappoase', 'Anna-Leena Härkönen', '9789511077299', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (10, 'Työmiehen vaimo', 'Minna Canth', '9786069482711', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (11, 'Kiirastuli', 'Ilkka Remes', '9789510420072', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (12, 'Kahden veren tytär', 'Saara Rostedt', '9789511487319', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (13, 'Puhdistus', 'Sofi Oksanen', '9789510425442', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (14, 'Hytti nro 6', 'Rosa Liksom', '9789510485989', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (15, '6/12', 'Ilkka Remes', '9789519478226', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (16, 'Twisted Lies', 'Ana Huang', '9780349434285', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (17, 'Treasure Island', 'Robert Louis Stevenson', '9780195811391', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (18, 'Mockingjay', 'Suzanne Collions', '9780439023511', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (19, 'Twilight', 'Stephenie Meyer', '9780316160179', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (20, 'The Handmaids Tale', 'Margaret Atwood', '0771008139', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (21, 'Lyhyt historia lähes kaikesta', 'Bill Bryson', '9780309729', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (22, 'The God Equation', 'Michio Kaku', '9780385542746', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (23, 'Olemisen porteilla', 'Kari Enqvist', '9510229156', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (24, 'A Brief History of Time', 'Stephen Hawking', '9780553109535', 1);
INSERT INTO `LibraryDB`.`books` (`id`, `title`, `author`, `isbn`, `available`) VALUES (25, 'Atomic Habits', 'James Clear', '9781847941831', 1);

COMMIT;