-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema subwaytalent-dev
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `subwaytalent-dev` ;

-- -----------------------------------------------------
-- Schema subwaytalent-dev
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `subwaytalent-dev` DEFAULT CHARACTER SET utf8 ;
USE `subwaytalent-dev` ;

-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`event`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`event` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`event` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(50) NULL DEFAULT NULL,
  `type_id` INT(11) NULL DEFAULT NULL,
  `dateCreated` DATETIME NULL DEFAULT NULL,
  `dateStarted` DATETIME NULL DEFAULT NULL,
  `dateEnd` DATETIME NULL DEFAULT NULL,
  `description` VARCHAR(200) NULL DEFAULT NULL,
  `location` VARCHAR(50) NULL DEFAULT NULL,
  `picture` VARCHAR(500) NULL DEFAULT NULL,
  `status` TINYINT(4) NULL DEFAULT '1' COMMENT '0-Cancelled, 1 - Confirmed',
  `title` VARCHAR(45) NULL DEFAULT NULL,
  `longitude` VARCHAR(50) NULL DEFAULT NULL,
  `latitude` VARCHAR(50) NULL DEFAULT NULL,
  `delete_reason` VARCHAR(1000) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  FULLTEXT INDEX `Name` (`Name` ASC, `description` ASC, `title` ASC, `location` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`event_invites`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`event_invites` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`event_invites` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `event_id` INT(11) NULL DEFAULT NULL,
  `user_id` VARCHAR(30) NULL DEFAULT NULL,
  `status_id` TINYINT(4) NULL DEFAULT '0' COMMENT '1 - accepted, 0 - pending, 2 rejected',
  `date_created` DATETIME NULL DEFAULT NULL,
  `lastupdate_date` DATETIME NULL DEFAULT NULL,
  `user_rating` VARCHAR(5) NULL DEFAULT NULL,
  `comments` VARCHAR(400) NULL DEFAULT NULL,
  `performed` TINYINT(4) NULL DEFAULT '1',
  `updated_by` VARCHAR(30) NULL DEFAULT NULL,
  `performed_comment` VARCHAR(200) NULL DEFAULT NULL,
  `rated_by` VARCHAR(30) NULL DEFAULT NULL,
  `rated_date` DATETIME NULL DEFAULT NULL,
  `user_rate` VARCHAR(30) NULL DEFAULT NULL,
  `payment_status` TINYINT(4) NULL DEFAULT '0' COMMENT '(0) - pending, (1) completed, (2) created, (3) planerpaid, (4) exceptions',
  `payment_date_update` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`event_planner`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`event_planner` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`event_planner` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `event_id` INT(11) NULL DEFAULT NULL,
  `user_id` VARCHAR(30) NULL DEFAULT NULL,
  `planner_payment_id` INT(11) NULL DEFAULT NULL,
  `payment_status` TINYINT(4) NULL DEFAULT '0' COMMENT '0-pending, 1-paid, 2-exception',
  `payment_date_update` DATETIME NULL DEFAULT NULL,
  `transaction_auth_id` VARCHAR(200) NULL DEFAULT NULL,
  `transaction_id_completed` VARCHAR(200) NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`event_planner_feedbacks`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`event_planner_feedbacks` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`event_planner_feedbacks` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `event_planner_id` INT(11) NULL DEFAULT NULL,
  `comments` VARCHAR(400) NULL DEFAULT NULL,
  `user_rating` VARCHAR(5) NULL DEFAULT NULL,
  `rated_by` VARCHAR(30) NULL DEFAULT NULL,
  `rated_date` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`event_preferred_genre`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`event_preferred_genre` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`event_preferred_genre` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `event_id` INT(11) NULL DEFAULT NULL,
  `genre_id` INT(11) NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`event_preferred_skills`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`event_preferred_skills` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`event_preferred_skills` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `event_id` INT(11) NULL DEFAULT NULL,
  `skill_id` INT(11) NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`event_types`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`event_types` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`event_types` (
  `id` INT(11) NOT NULL,
  `type_name` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  FULLTEXT INDEX `eventType_idx` (`type_name` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`genre`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`genre` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`genre` (
  `Id` INT(11) NOT NULL,
  `Name` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  FULLTEXT INDEX `genreName` (`Name` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`payment_exceptions`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`payment_exceptions` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`payment_exceptions` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `message` VARCHAR(2000) NULL DEFAULT NULL,
  `event_planner_id` INT(11) NULL DEFAULT NULL,
  `date_created` DATETIME NULL DEFAULT NULL,
  `event_invites_id` INT(11) NULL DEFAULT NULL,
  `stack_trace` VARCHAR(2000) NULL DEFAULT NULL,
  `payment_event` VARCHAR(45) NULL DEFAULT NULL,
  `other_info` VARCHAR(2000) NULL DEFAULT NULL,
  `user_id` VARCHAR(30) NULL DEFAULT NULL COMMENT 'the one who initiates the pay event.',
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`payment_methods`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`payment_methods` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`payment_methods` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `payment_name` VARCHAR(50) NULL DEFAULT NULL,
  `payment_processor` VARCHAR(50) NULL DEFAULT NULL,
  `date_created` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`planner_payments`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`planner_payments` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`planner_payments` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `user_id` VARCHAR(45) NULL DEFAULT NULL,
  `refresh_token` VARCHAR(1000) NULL DEFAULT NULL,
  `date_created` DATETIME NULL DEFAULT NULL,
  `payment_method_id` INT(11) NULL DEFAULT NULL,
  `payment_instrument_id` VARCHAR(100) NULL DEFAULT NULL COMMENT 'unique identifier for the payment used like email, id, phone number etc.',
  `masked_card_no` VARCHAR(30) NULL DEFAULT NULL,
  `card_type` VARCHAR(20) NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`skills`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`skills` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`skills` (
  `Id` INT(11) NOT NULL,
  `Name` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  FULLTEXT INDEX `skillName` (`Name` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`subway_settings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`subway_settings` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`subway_settings` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `setting_name` VARCHAR(45) NULL DEFAULT NULL,
  `setting_value` VARCHAR(45) NULL DEFAULT NULL,
  `date_created` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 7
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`talent_genres`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`talent_genres` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`talent_genres` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `user_id` VARCHAR(30) NULL DEFAULT NULL,
  `genre_id` INT(11) NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`talent_skills`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`talent_skills` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`talent_skills` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `user_id` VARCHAR(30) NULL DEFAULT NULL,
  `skill_id` INT(11) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`us_cities`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`us_cities` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`us_cities` (
  `us_cities_id` INT(11) NOT NULL AUTO_INCREMENT,
  `city` VARCHAR(255) NULL DEFAULT NULL,
  `state_id` VARCHAR(5) NULL DEFAULT NULL,
  `state_name` VARCHAR(255) NULL DEFAULT NULL,
  `zip` VARCHAR(4000) NULL DEFAULT NULL,
  `latitude` DECIMAL(15,10) NULL DEFAULT NULL,
  `longitude` DECIMAL(15,10) NULL DEFAULT NULL,
  PRIMARY KEY (`us_cities_id`),
  INDEX `State_name_index` (`state_name` ASC))
ENGINE = InnoDB
AUTO_INCREMENT = 38184
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`user_account`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`user_account` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`user_account` (
  `userId` VARCHAR(30) NOT NULL,
  `firstName` VARCHAR(100) NULL DEFAULT NULL,
  `lastName` VARCHAR(100) NULL DEFAULT NULL,
  `email` VARCHAR(100) NULL DEFAULT NULL,
  `birthday` DATETIME NULL DEFAULT NULL,
  `facebookuser` TINYINT(4) NULL DEFAULT '0',
  `userType` VARCHAR(2) NULL DEFAULT 'T' COMMENT 'Talent - T\nPlanner - P',
  `timezone` VARCHAR(10) NULL DEFAULT NULL,
  `rate` VARCHAR(30) NULL DEFAULT NULL,
  `location` VARCHAR(255) NULL DEFAULT NULL,
  `bio` VARCHAR(1000) NULL DEFAULT NULL,
  `rating` VARCHAR(5) NULL DEFAULT NULL,
  `profilePic` VARCHAR(500) NULL DEFAULT NULL,
  `mobileNumber` VARCHAR(15) NULL DEFAULT NULL,
  `gender` CHAR(1) NULL DEFAULT NULL COMMENT 'M - Male ,  F - Female',
  `talentName` VARCHAR(100) NULL DEFAULT NULL,
  `profilePicTalent` VARCHAR(500) NULL DEFAULT NULL,
  `allow_notif` TINYINT(4) NULL DEFAULT '1',
  `allow_email` TINYINT(4) NULL DEFAULT '1',
  `cityStateId` INT(11) NULL DEFAULT NULL,
  `ratingTalent` VARCHAR(5) NULL DEFAULT NULL,
  PRIMARY KEY (`userId`),
  UNIQUE INDEX `id_UNIQUE` (`userId` ASC),
  FULLTEXT INDEX `idx_userAccount` (`firstName` ASC, `lastName` ASC, `email` ASC, `location` ASC, `talentName` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COMMENT = 'user info';


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`user_devices`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`user_devices` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`user_devices` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `user_id` VARCHAR(30) NULL DEFAULT NULL,
  `device_id` VARCHAR(100) NULL DEFAULT NULL,
  `device_type` TINYINT(4) NULL DEFAULT NULL COMMENT '1 - IOS, 2 - Android',
  `date_created` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`user_external_media`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`user_external_media` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`user_external_media` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `user_id` VARCHAR(30) NULL DEFAULT NULL,
  `name` VARCHAR(100) NULL DEFAULT NULL,
  `url` VARCHAR(200) NULL DEFAULT NULL,
  `thumbnail_url` VARCHAR(200) NULL DEFAULT NULL,
  `type` CHAR(1) NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`user_files`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`user_files` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`user_files` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `user_id` VARCHAR(30) NULL DEFAULT NULL,
  `file_type` CHAR(2) NULL DEFAULT NULL COMMENT 'P - Picture, V-Videos',
  `file_path` VARCHAR(500) NULL DEFAULT NULL,
  `name` VARCHAR(100) NULL DEFAULT NULL,
  `thumbnail_path` VARCHAR(500) NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`user_help`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`user_help` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`user_help` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `message` VARCHAR(1500) NULL DEFAULT NULL,
  `senderId` VARCHAR(30) NULL DEFAULT NULL,
  `senderName` VARCHAR(100) NULL DEFAULT NULL,
  `senderEmail` VARCHAR(100) NULL DEFAULT NULL,
  `date_created` DATETIME NULL DEFAULT NULL,
  `subject` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`user_login`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`user_login` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`user_login` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `userid` VARCHAR(30) NULL DEFAULT NULL,
  `LastLoggedInDate` DATETIME NULL DEFAULT NULL,
  `Password` VARCHAR(20) NULL DEFAULT NULL,
  `LockedOutDate` DATETIME NULL DEFAULT NULL,
  `role_name` VARCHAR(125) NULL DEFAULT 'USER',
  PRIMARY KEY (`Id`),
  INDEX `fk_user_login_idx` (`userid` ASC),
  CONSTRAINT `fk_user_login`
    FOREIGN KEY (`userid`)
    REFERENCES `subwaytalent-dev`.`user_account` (`userId`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 18
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`user_notifications`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`user_notifications` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`user_notifications` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `user_id` VARCHAR(30) NULL DEFAULT NULL,
  `event_id` INT(11) NULL DEFAULT NULL,
  `date_created` DATETIME NULL DEFAULT NULL,
  `status_id` TINYINT(4) NULL DEFAULT NULL COMMENT 'Status [0-pending,1-accepted,2-rejected,3-requested]',
  `updated_by` VARCHAR(30) NULL DEFAULT NULL,
  `notif_type` TINYINT(4) NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `subwaytalent-dev`.`user_tokens`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `subwaytalent-dev`.`user_tokens` ;

CREATE TABLE IF NOT EXISTS `subwaytalent-dev`.`user_tokens` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `user_id` VARCHAR(30) NULL DEFAULT NULL,
  `auth_token` VARCHAR(250) NULL DEFAULT NULL,
  `issued_on` DATETIME NULL DEFAULT NULL,
  `expires_on` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


--
-- Table structure for table `document_template`
--

DROP TABLE IF EXISTS `document_template`;

CREATE TABLE `document_template` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `docu_name` varchar(125) NOT NULL,
  `content` text,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8;



USE `subwaytalent-dev` ;

-- -----------------------------------------------------
-- procedure spSubway_AcceptRejectTalentInvite
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AcceptRejectTalentInvite`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AcceptRejectTalentInvite`(
	talentid varchar(30),
    eventid int,
    statusid tinyint,
    updatedBy varchar(30)
)
BEGIN

	declare result tinyint;
    declare existingReq tinyint;
	set result = 0;
    set existingReq = 0;

	select count(1) into result from user_account
							where userId = talentid;

	if(result = 0) then
		begin 
			select 0;
        end;
	else
		begin
			-- send request or invited, insert record
			if(statusid = 3 or statusid = 0) then
				begin
                
					select count(1) into existingReq 
                    from event_invites
					where user_id = talentid
                    and event_id = eventid;
					
                    if(existingReq = 0) then
						begin
							
							declare userRate varchar(30);            
							set userRate = (select rate from user_account 
											where userId = talentid);
                                            
							insert into event_invites(event_id,user_id,status_id,
													  date_created,lastupdate_date,
                                                      user_rate)
							values(eventid,talentid,statusid,
								   UTC_TIMESTAMP(),UTC_TIMESTAMP(),
                                   userRate);
						end;
					end if;
				end;
			else
				begin
					update event_invites
					set status_id = statusid,
                    updated_by = updatedBy,
                    lastupdate_date = UTC_TIMESTAMP()
					
					where event_id = eventid
					and user_id = talentid;
				end;
			end if; 
            
            select 1;
		end;        
	end if;	
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddDeviceID
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddDeviceID`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddDeviceID`(
	userIdParam varchar(30),
    deviceID	varchar(100),
    deviceType	tinyint
)
BEGIN

	insert into user_devices(user_id,device_id,device_type,date_created)
    values(userIdParam,deviceID,deviceType,UTC_TIMESTAMP());

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddEvent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddEvent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddEvent`(

  Name varchar(50),
  type_id int(11),
  dateCreated datetime,
  dateStarted datetime,
  dateEnd datetime,
  description varchar(200),
  location varchar(50),
  picture varchar(500),
  status varchar(45),
  title varchar(45),
  longitude varchar(50),
  latitude varchar(50)

)
BEGIN

INSERT INTO event(Name,type_id,dateCreated,dateStarted,dateEnd,
					description, location,picture,status,title,
                    longitude,latitude)
VALUES(Name,type_id,dateCreated,dateStarted,
		dateEnd,description,location,picture,
        status,title,longitude,latitude);

select last_insert_id();

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddEventPlanner
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddEventPlanner`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddEventPlanner`(
	 event_id int(11),
	 userId varchar(30)
)
BEGIN

	declare plannerPaymentId int;
    set plannerPaymentId = 0;
    
    select id into plannerPaymentId
    from planner_payments
    where user_id = userId
    limit 1;
    
    if(plannerPaymentId = 0) then
		begin			
            select 0;
        end;
	else
		begin
			insert into event_planner(event_id,user_id,planner_payment_id)
			values(event_id,userId,plannerPaymentId);          
            
			select 1;
        end;
	end if;

	

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddGenreToEvent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddGenreToEvent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddGenreToEvent`(
	eventId int,
    genreId int
)
BEGIN

	insert into event_preferred_genre(event_id,genre_id)
    values(eventId,genreId);

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddHelp
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddHelp`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddHelp`(
	userId varchar(30),
    sender_Name varchar(100),
    sender_Email varchar(100),
    subjectParam varchar(100),
    messageParam varchar(1500)
)
BEGIN

	insert into user_help(message,senderId,senderName,senderEmail,date_created,subject)
    values(messageParam, userId, sender_Name, sender_Email, UTC_TIMESTAMP(),subjectParam);


END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddNotification
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddNotification`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddNotification`(
	userIdParam varchar(30),
    eventId int,
    statusId tinyint,
    updatedBy varchar(30),
    notifType tinyint
)
BEGIN
	
     declare statExists tinyint;
     set statExists = 0;
    
     select count(1) into statExists
     from user_notifications
     where user_id = userIdParam
     and event_id = eventId
     and status_id = statusId
     and updated_by = updatedBy
     and notif_type = notifType;
    
     if(statExists = 0) then
	 	begin   
			insert into user_notifications(user_id, event_id, status_id, date_created, 
            updated_by,notif_type)
			values(userIdParam, eventId, statusId, UTC_TIMESTAMP(),updatedBy,notifType);
         end;
	 end if;
    
     select statExists;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddPaymentException
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddPaymentException`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddPaymentException`(
	messageParam varchar(2000),
    eventPlannerId int,
    eventInvitesId int,
    stackTrace varchar(2000),
    eventName varchar(45),
    otherInfo varchar(2000),
    userId varchar(30)
)
BEGIN

	insert into payment_exceptions(message,event_planner_id,date_created,
									event_invites_id,stack_trace,
                                    payment_event, other_info, user_id)
	values(messageParam,eventPlannerId,UTC_TIMESTAMP(),eventInvitesId,
									stackTrace,eventName,otherInfo,userId);

	select last_insert_id();
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddPaymentTransactionAuthId
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddPaymentTransactionAuthId`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddPaymentTransactionAuthId`(
	transactionAuthId varchar(200),
    transactionIdCompleted varchar(200),
    eventPlannerId int,
    paymentStatus tinyint
)
BEGIN

	update event_planner
    set transaction_auth_id = transactionAuthId,
    payment_status = paymentStatus,
    payment_date_update = UTC_TIMESTAMP(),
    transaction_id_completed = transactionIdCompleted
    
    where id = eventPlannerId;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddPlannerPaymentMethod
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddPlannerPaymentMethod`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddPlannerPaymentMethod`(
	userId varchar(30),
    refreshToken varchar(1000),
    paymentMethodId int,
    paymentInstrumentId varchar(100),
    maskedCardNo varchar(30),
    cardType varchar(20)
    
)
BEGIN

	declare plannerPaymentId int;
     
	delete from planner_payments
    where user_id = userId;

	insert into planner_payments(user_id,refresh_token,payment_method_id,
								date_created,payment_instrument_id,masked_card_no,
                                card_type)
    values(userId,refreshToken,paymentMethodId,UTC_TIMESTAMP(),
			paymentInstrumentId,maskedCardNo,cardType);
    
    set plannerPaymentId = (select last_insert_id());
    
    
    update event_planner
    set planner_payment_id = plannerPaymentId
    where user_id = userId
    and payment_status = 0;
    
	
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddSettings
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddSettings`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddSettings`(
	settingName varchar(45),
    settingValue varchar(45)
)
BEGIN

	declare result tinyint;
    set result = 0;
    
	select count(1) into result 
	from subway_settings
	where setting_name = settingName;
    
    if(result = 0) then 
		begin     
			insert into subway_settings(setting_name,setting_value,date_created)
			values(settingName,settingValue, UTC_TIMESTAMP());           
            select 1;
        end;
	else
		begin			
            select 0;
        end;
	end if;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddSkillsToEvent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddSkillsToEvent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddSkillsToEvent`(
	eventId int,
    skillId int
)
BEGIN

	insert into event_preferred_skills(event_id,skill_id)
    values(eventId,skillId);

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddTalentGenre
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddTalentGenre`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddTalentGenre`(
	user_id varchar(30),
	genre_id int
)
BEGIN	
	insert into talent_genres(user_id,genre_id)
    values(user_id,genre_id);
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddTalentSkill
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddTalentSkill`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddTalentSkill`(
	user_id varchar(30),
	skill_id int
)
BEGIN	
	insert into talent_skills(user_id,skill_id)
    values(user_id,skill_id);
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddTalentToEvent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddTalentToEvent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddTalentToEvent`(
	userIdParam	varchar(30),
    eventid	int
)
BEGIN

	declare result tinyint;    
    set result =0;

	select count(1) into result from event_invites
							where event_id = eventid
                            and user_id = userIdParam;
                            
	if (result = 0) then
		begin
        
			declare userRate varchar(30);
            
            set userRate = (select rate from user_account 
							where userId = userIdParam);
        
           insert into event_invites(event_id, user_id,date_created,user_rate)
		   values(eventid,userIdParam,UTC_TIMESTAMP(),userRate);
		end ;	
	end if;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddUser
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddUser`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddUser`(
	userid varchar(100),       
	firstname varchar(100),
	lastname varchar(100),  
	email varchar(100),
	password  varchar(20),
	birthday datetime,       
	lastloggedindate datetime,
	lockedoutdate datetime,
	facebookuser tinyint
)
BEGIN


insert into user_account(userid,firstname,lastname,email,birthday,facebookuser)
values(userid,firstname,lastname,email,birthday,facebookuser);

insert into user_login(userid,lastloggedindate,password,lockedoutdate)
values(userid,lastloggedindate,password,lockedoutdate);





END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddUserExternalMedia
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddUserExternalMedia`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddUserExternalMedia`(
	userId	varchar(30),
    name	varchar(100),
    url	varchar(200),
    thumbUrl varchar(200),
	media_type	char(1)
)
BEGIN

	insert into user_external_media(user_id,name,url,thumbnail_url,type)
    values(userId,name,url,thumbUrl,media_type);

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddUserFile
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddUserFile`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddUserFile`(
	user_id	varchar(30),
    file_path	varchar(500),
    file_type	char(2),
    name varchar(100),
    thumb_path	varchar(500)
)
BEGIN

	insert into user_files(user_id,file_path,file_type,name,thumbnail_path)
    values(user_id,file_path,file_type,name,thumb_path);

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_AddUserToken
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_AddUserToken`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_AddUserToken`(
	userIdParam varchar(30),
    token varchar(255),
    issuedOn datetime,
    expiresOn datetime
)
BEGIN

	insert into user_tokens(user_id, auth_token, issued_on, expires_on)
    values(userIdParam,token,issuedOn,expiresOn);
    


END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_ChangePassword
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_ChangePassword`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_ChangePassword`(
	oldPassword varchar(20),
    newPassword varchar(20),
    user_id	varchar(30)
)
BEGIN

	declare userFound	int;
    set userFound = 0;
    
    select count(1) into userFound from user_login
    where userid = user_id
    and password = oldPassword;
    
    if(userFound >0) then
		begin
			update user_login
            set password = newPassword
            where userid = user_id;
            
            select 1;
		end;
    else
		begin
			select 0;
		end;
	end if;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_ChangeSettings
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_ChangeSettings`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_ChangeSettings`(
	user_id varchar(30),
    allowNotif tinyint,
    allowEmail tinyint
)
BEGIN

	update user_account
    set allow_notif = allowNotif,
    allow_email = allowEmail
    
    where userId = user_id;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_DeleteEvent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_DeleteEvent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_DeleteEvent`(
	eventIdParam int
)
BEGIN

	delete from user_notifications
    where event_id = eventIdParam;
    

	delete from event_preferred_genre
    where event_id = eventIdParam;
    
	delete from event_preferred_skills
    where event_id = eventIdParam;
    
    delete from event_invites
     where event_id = eventIdParam;
     
     
     delete from  event_planner_feedbacks
     where event_planner_id in (select id from event_planner
								where event_id = eventIdParam);
                                
     delete from event_planner
     where event_id = eventIdParam;
     
     delete from event
     where id = eventIdParam;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_DeleteEventPreferredSkillsGenre
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_DeleteEventPreferredSkillsGenre`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_DeleteEventPreferredSkillsGenre`(
	eventId int
)
BEGIN

	delete from event_preferred_genre
    where event_id = eventId;
    
    delete from event_preferred_skills
    where event_id = eventId;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_DeleteExternalMedia
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_DeleteExternalMedia`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_DeleteExternalMedia`(
	userId	varchar(30),
    media_type	char(1)
)
BEGIN

	delete from user_external_media
    where user_id = userId
    and type = media_type;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_DeleteTalentGenres
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_DeleteTalentGenres`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_DeleteTalentGenres`(
	userid	varchar(30)
)
BEGIN
	delete from talent_genres
    where user_id = userid;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_DeleteTalentSkills
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_DeleteTalentSkills`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_DeleteTalentSkills`(
	userid	varchar(30)
)
BEGIN
	delete from talent_skills
    where user_id = userid;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_DeleteTokenByUserId
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_DeleteTokenByUserId`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_DeleteTokenByUserId`(
	userIdParam varchar(30)
)
BEGIN

	delete from user_tokens
    where user_id = userIdParam;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_DeleteUserFile
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_DeleteUserFile`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_DeleteUserFile`(
	userId varchar(30),
    file_id	int,
    fileName varchar(100)
	
)
BEGIN

	declare fileFound int;
    set fileFound = 0;
    
    

	-- if there was no id passed
	if(file_id = 0) then
		begin
			
			 delete from user_files
			 where user_id = userId
			 and name = fileName;
             
             select 1;
        end;
	else
		begin
			
            select count(1) into fileFound from user_files
            where id = file_id;
           
            
            if(fileFound > 0) then
				begin
					select 1;
				end;
            else
				begin
					select 0;
				end;
            end if;
            
			delete from user_files
			where id = file_id;
        end;
	end if;
    
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_DeleteUserNotifications
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_DeleteUserNotifications`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_DeleteUserNotifications`(
	notificationId int
)
BEGIN

	delete from user_notifications
    where id = notificationId;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_DeleteUserToken
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_DeleteUserToken`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_DeleteUserToken`(
	token varchar(255)
)
BEGIN
	delete from user_tokens
    where auth_token = token;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_DropTalentToEvent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_DropTalentToEvent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_DropTalentToEvent`(
	eventId int,
	talentId varchar(30),
    comments varchar(200),
    performed_flg tinyint,
    updatedBy varchar(30)
)
BEGIN
	
	update event_invites
	set status_id = 4,
	performed = performed_flg,
	performed_comment = comments,
	updated_by = updatedBy,
	lastupdate_date = UTC_TIMESTAMP()

	where user_id = talentId
	and event_id = eventId;            
         
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetAllEventTypes
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetAllEventTypes`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetAllEventTypes`()
BEGIN
	select  
		id as Id,
        type_name as Name
    from event_types;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetAllEvents
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetAllEvents`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetAllEvents`(
	userid	varchar(30)
)
BEGIN

	

	if(userid is null or userid='') then
		select distinct e.*, 
		et.type_name as typeName, 
		et.Id as typeId

		from event as e
		inner join event_types as et
		on e.type_id = et.id
        inner join event_planner as ep
        on e.id = ep.event_id
		left outer join event_invites as ei
		on e.id = ei.event_id
		
		where e.status = 1        
        order by e.dateCreated desc;
		
	else 
		select distinct e.*, 
		et.type_name as typeName, 
		et.Id as typeId
        
		from event as e
		inner join event_types as et
		on e.type_id = et.id
        inner join event_planner as ep
        on e.id = ep.event_id
		left outer join event_invites as ei
		on e.id = ei.event_id
        -- and ei.user_id <> userid
		
		where e.status = 1
		-- and ei.user_id <> userid
		-- or ei.user_id is null
        and e.id not in (select event_id from event_invites where user_id = userid)
        and e.id not in (select event_id from event_planner where user_id = userid)
        
        order by e.dateCreated desc;
	end if;
    
	

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetAllGenres
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetAllGenres`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetAllGenres`()
BEGIN
	select * from genre;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetAllSkills
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetAllSkills`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetAllSkills`()
BEGIN

	select * from skills;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetAllUsers
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetAllUsers`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetAllUsers`(
	user_id varchar(30)
)
BEGIN

select ua.*, uc.*
from user_account as ua
inner join talent_genres as tg
on ua.userId = tg.user_id
left outer join us_cities as uc
on ua.cityStateId = uc.us_cities_id

where ua.userId <> user_id
and rate is not null
and rate <> ''

union

select ua.*, uc.*
from user_account as ua
inner join talent_skills as ts
on ua.userId = ts.user_id
left outer join us_cities as uc
on ua.cityStateId = uc.us_cities_id

where ua.userId <> user_id
and rate is not null
and rate <> '';

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetCitiesByStateId
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetCitiesByStateId`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetCitiesByStateId`(
	stateId varchar(4)
)
BEGIN

	select us_cities_id, city 
    from us_cities
    where state_id = stateId
    
    order by city;
    

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetDoneEvents
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetDoneEvents`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetDoneEvents`()
BEGIN


	select 
    pm.payment_name,
    pm.payment_processor,
    e.Id as eventId,
    ep.id as eventPlannerId,
    ep.payment_status,
    ep.transaction_auth_id,
    ep.transaction_id_completed,
    pp.*
    from event_planner as ep
    inner join event as e
    on ep.event_id = e.Id
    inner join planner_payments as pp
    on pp.id = ep.planner_payment_id
    inner join payment_methods as pm
    on pm.id = pp.payment_method_id
    
    
    where e.status = 1
    and ep.payment_status in (0,2)  -- only pending and created
    and (select count(1) from event_invites where event_id = e.Id) > 0
    and e.dateEnd < Now();
    

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetEventDetails
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetEventDetails`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetEventDetails`(
	eventId int
)
BEGIN

	select e.*, et.type_name
    from event as e
    inner join event_Types as et
    on e.type_id = et.Id    
    where e.Id = eventId;
    
    select ua.*,uc.* from 
    event_planner as ep
    inner join user_account as ua
    on ep.user_id = ua.userId    
    left outer join us_cities as uc
    on ua.cityStateId = uc.us_cities_id
    where event_id = eventId;
    
    select g.* from 
    event_preferred_genre as eg
    inner join genre as g
    on g.Id = eg.genre_id    
    where event_id = eventId;
    
    select s.* from 
    event_preferred_skills as es
    inner join skills as s
    on s.Id = es.skill_id
    where event_id = eventId;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetEventPreferredGenres
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetEventPreferredGenres`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetEventPreferredGenres`(
	event_id int
)
BEGIN

	select g.id, g.name from event_preferred_genre as eg
    inner join genre as g
    on eg.genre_id = g.id
    
    where eg.event_id = event_id;  
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetEventPreferredSkills
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetEventPreferredSkills`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetEventPreferredSkills`(
	event_id int
)
BEGIN

	select g.id, g.name from event_preferred_skills as es
    inner join skills as g
    on es.skill_id = g.id
    
    where es.event_id = event_id;  
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetEventsPlanner
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetEventsPlanner`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetEventsPlanner`(
	userid varchar(30)
)
BEGIN

	select e.*,
    et.type_name as typeName, 
    et.Id as typeId
    
    from event as e
    inner join event_planner as ep
    on e.id = ep.event_id
    inner join event_types as et
    on e.type_id = et.id
    
    where ep.user_id = userid;
    -- and e.dateStarted >= NOW();


END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetEventsTalent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetEventsTalent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetEventsTalent`(
	userid varchar(30),
    inviteStatus tinyint
)
BEGIN	

	select e.*, 
    et.type_name as typeName, 
    et.Id as typeId
    
    from event as e
    inner join event_invites as ei
    on e.id = ei.event_id
    inner join event_types as et
    on e.type_id = et.id
    
    where ei.user_id = userid
	and ei.status_id = inviteStatus;
    

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetExternalMedia
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetExternalMedia`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetExternalMedia`(
	userId	varchar(30)
)
BEGIN

	select * from user_external_media
    where user_id = userId;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetLatLongByCityStateId
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetLatLongByCityStateId`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetLatLongByCityStateId`(
	cityStateList varchar(100)
)
BEGIN
	
	select * from us_cities as uc
    
    where find_in_set(uc.us_cities_id,cityStateList);

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetPaymentMethods
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetPaymentMethods`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetPaymentMethods`()
BEGIN

	select * from payment_methods;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetPlannerPendingPayments
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetPlannerPendingPayments`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetPlannerPendingPayments`(
	userId varchar(30)
)
BEGIN

	
    select 
    e.*,
    ei.user_rate,    
    ei.payment_status,
    ei.payment_date_update, 
    ei.user_rating,
    ua.*,
    et.type_name as typeName, 
    et.Id as typeId
    
    from event_planner as ep
    inner join event as e
    on ep.event_id = e.id
    inner join event_invites as ei
    on ei.event_id = e.id
    inner join event_types as et
    on e.type_id = et.id  
    inner join user_account as ua
    on ua.userId = ei.user_id
    
    where ep.user_id = userId
	and ei.status_id = 1;
    -- and ei.payment_status <> 1;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetPlannerTotals
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetPlannerTotals`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetPlannerTotals`(
	userId	varchar(30)
)
BEGIN
	
    declare inviteCount	int;
    declare bookedCount int;
    declare closedCount int;

	-- invited talents
	set inviteCount = (select count(1)
						from event_invites as ei
						inner join event_planner as ep
						on ei.event_id = ep.event_id
						inner join event as e
                        on e.Id = ep.event_id
                        
						where ep.user_id = userId
						and ei.status_id = 0
                        and e.status =1);
	-- accepted talents
	set bookedCount = (select count(1)
						from event_invites as ei
						inner join event_planner as ep
						on ei.event_id = ep.event_id
						inner join event as e
                        on e.Id = ep.event_id
                        
						where ep.user_id = userId
						and ei.status_id = 1
                        and e.status =1);
	
    set closedCount = (select count(1) 
						from event_planner as ep
                        inner join event as e
                        on ep.event_id = e.Id
                        
                        where e.status = 0);
					
	select inviteCount as invitecount, 
		   bookedCount as bookedcount,
           closedCount as closecount;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetPlannersByEvent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetPlannersByEvent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetPlannersByEvent`(
	eventId int
)
BEGIN

	select ua.*,uc.* from event_planner as ep
    inner join user_account as ua
    on ep.user_id = ua.userid
    left outer join us_cities as uc 
    on uc.us_cities_id = ua.cityStateId
    
    where ep.event_id = eventId;
   

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetSettings
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetSettings`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetSettings`(
	
)
BEGIN

	select * from subway_settings;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetStates
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetStates`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetStates`(
	
)
BEGIN

	select distinct state_id, state_name 
    from us_cities order by state_name asc;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetTalentInvites
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetTalentInvites`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetTalentInvites`(
	userid varchar(30)
)
BEGIN
	select 
     et.id as typeId, et.type_name as typeName,
     e.Id as eventId,
     ei.status_id as talentStatus
    
    
    from event as e
    inner join event_invites as ei
    on e.id = ei.event_id
    inner join event_types as et
    on e.type_id = et.id
    
    where ei.user_id = userid
    -- pending/invite and requested only
    and ei.status_id in (0,3)
	-- and e.dateStarted >= NOW()
    
    order by et.id;
 -- group by e.type_id;	

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetTalentInvitesPlanners
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetTalentInvitesPlanners`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetTalentInvitesPlanners`(
	userid varchar(30),
    eventTypeId int
)
BEGIN

	select ua.*
    
    from event as e
    inner join event_invites as ei
    on e.id = ei.event_id
    inner join event_types as et
    on e.type_id = et.id
    inner join event_planner as ep
    on ep.event_id = e.Id
    inner join user_account as ua
    on ua.userid = ep.user_id
    
    where ei.user_id = userid
    and ei.status_id = 0
    and et.id = eventTypeId;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetTalentPendingPayments
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetTalentPendingPayments`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetTalentPendingPayments`(
	userId varchar(30)
)
BEGIN

	select e.*, 
    et.type_name as typeName, 
    et.Id as typeId,
    ei.user_rate,
    ei.payment_status,
    ei.payment_date_update,    
    ua.*,
    epf.user_rating as plannerRating
    
    from event as e
    inner join event_invites as ei
    on e.id = ei.event_id
    inner join event_types as et
    on e.type_id = et.id   
    inner join event_planner as ep
    on ep.event_id = e.id
    left join event_planner_feedbacks as epf
    on epf.event_planner_id = ep.id
    and epf.rated_by = ei.user_id
    inner join user_account as ua
    on ep.user_id = ua.userId
   
    where ei.user_id = userId
    and ei.status_id = 1;
    -- and ei.payment_status <> 1;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetTalentTotals
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetTalentTotals`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetTalentTotals`(
	userId varchar(30)
    
)
BEGIN

	declare leadCount	int;
    declare bookedCount int;
    declare closedCount int;

	-- requested
	set leadCount = (select count(1)
						from event_invites as ei					
						inner join event as e
                        on e.Id = ei.event_id
                        
						where user_id = userId
						and ei.status_id = 3
                        and e.status = 1);
    -- accepted                    
	set bookedCount = (select count(1)
						from event_invites as ei					
						inner join event as e
                        on e.Id = ei.event_id					

						where user_id = userId
						and ei.status_id = 1
						and e.status = 1);

	set closedCount = (select count(1)
						from event_invites as ei 					
						inner join event as e
                        on e.Id = ei.event_id
                        
						where user_id = userId
						and e.status = 0);
                        
                        
                        
                        
	select leadCount as leadcount, 
		   bookedCount as bookedcount,
           closedCount as closecount; 
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetTalentsByEvent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetTalentsByEvent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetTalentsByEvent`(
	eventId int,
    inviteStatus tinyint,
    allTalent tinyint
)
BEGIN

	declare invStatus tinyint;
    set invStatus = inviteStatus;
    
    
    if(allTalent = 1) then
		begin
			select ua.*, uc.*, ei.status_id,ei.user_rate,
            ei.payment_status,
            ei.payment_date_update
            
			from event_invites as ei
			inner join user_account as ua
			on ei.user_id = ua.userid
			left outer join us_cities as uc
			on ua.cityStateId = uc.us_cities_id
			
			where ei.event_id = eventId;
		end;
	else
		begin
			-- if invite status is 0(pending) also include the requested(3)
			if(invStatus = 0) then
				begin
					select ua.*, uc.*, ei.status_id,ei.user_rate,
                    ei.payment_status,
					ei.payment_date_update
					from event_invites as ei
					inner join user_account as ua
					on ei.user_id = ua.userid
					left outer join us_cities as uc
					on ua.cityStateId = uc.us_cities_id
					
					where ei.event_id = eventId
					and ei.status_id in (0,3);
				end;
			else
				begin
					select ua.*, uc.*, ei.status_id,ei.user_rate,
                    ei.payment_status,
					ei.payment_date_update
                    
					from event_invites as ei
					inner join user_account as ua
					on ei.user_id = ua.userid
					left outer join us_cities as uc
					on ua.cityStateId = uc.us_cities_id
					
					where ei.event_id = eventId
					and ei.status_id = inviteStatus;
				end;
			end if;
		end;
	end if;
    -- and ua.userType = "T";

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetUserComments
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetUserComments`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetUserComments`(
	userIdParam varchar(30),
    userType char(1)
)
BEGIN

	if(userType = 'P') then
		begin
			select pf.* , e.Name,
            concat(ua.firstname,' ', ua.lastName) as ratedBy_name,
            ua.profilePicTalent as profilePicture
            
            from event_planner as ep
            inner join event_planner_feedbacks as pf
            on ep.id = pf.event_planner_id
            inner join event as e
            on ep.event_id = e.id
            left outer join user_account as ua
            on ua.userId = pf.rated_by
            
            where ep.user_id = userIdParam
            and pf.comments is not null
            order by pf.rated_date desc;
            
        end;
    elseif (userType = 'T') then
		begin
			select ei.comments,ei.user_rating
				  ,ei.rated_by
                  ,ei.rated_date
                  ,e.Name
                  ,concat(ua.firstname,' ', ua.lastName) as ratedBy_name
                  ,ua.profilePic as profilePicture
                  
            from event_invites as ei
            inner join event as e
            on e.id = ei.event_id
			left outer join user_account as ua
            on ua.userId = ei.rated_by
            
            where user_id = userIdParam
            and comments is not null
            order by rated_date desc;
        end;
	end if;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetUserDetails
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetUserDetails`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetUserDetails`(userid varchar(30))
BEGIN

	select * from user_login as a
    inner join user_account as b
    on a.userid = b.userId
    left outer join us_cities as uc
	on b.cityStateId = uc.us_cities_id
    
    where a.userid = userid;
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetUserDevices
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetUserDevices`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetUserDevices`(
	userId	varchar(30)
)
BEGIN

	select * from user_devices
    where user_id = userId;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetUserFiles
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetUserFiles`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetUserFiles`(
	userid	varchar(30)
)
BEGIN

	select * from user_files
    where user_id = userid;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetUserGenres
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetUserGenres`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetUserGenres`(
	user_id	varchar(30)
)
BEGIN
	select g.id, g.name from talent_genres as tg
    inner join genre as g
    on tg.genre_id = g.id
    
    where tg.user_id = user_id;    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetUserNotifications
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetUserNotifications`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetUserNotifications`(
	userId varchar(30)
)
BEGIN

	select *
    from user_notifications
    where user_id = userId
    order by date_created desc;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetUserPaymentMethods
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetUserPaymentMethods`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetUserPaymentMethods`(
	userId varchar(30)
)
BEGIN

	select * from planner_payments as pp
    inner join payment_methods pm
    on pp.payment_method_id = pm.id
    where user_id= userId;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetUserSkills
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetUserSkills`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetUserSkills`(
	user_id	varchar(30)
)
BEGIN
	select s.id, s.name from talent_skills as ts
    inner join skills as s
    on ts.skill_id = s.id
    
    where ts.user_id = user_id;    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetUserToken
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_GetUserToken`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_GetUserToken`(
	tokenId varchar(255),
    expireDateRef datetime
)
BEGIN

	select * from user_tokens
    where auth_token = tokenId
    and expires_on > expireDateRef;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_LoginUser
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_LoginUser`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_LoginUser`(
	user_id varchar(30),
    passwordParam varchar(100)
)
BEGIN
	
	declare userFound tinyint;
    declare loginSuccess tinyint;
    
    set userFound = 0;
    set loginSuccess = 0;
    
    select count(1) into userFound
    from user_login 
    where userid = user_id;
    
	select count(1) into loginSuccess
    from user_login as a
    inner join user_account as b
    where a.userid = user_id
    and a.password = password;
    
    if(loginSuccess > 0) then
		begin
			update user_login
            set LastLoggedInDate = UTC_TIMESTAMP()
            where userid = user_id;
        end;
	end if;
    
    
    select * from user_login as a
    inner join user_account as b
    on a.userid = b.userId
    where a.userid = user_id
    and a.password = passwordParam;   
	
    
    select userFound;    
    
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_GetEmailContents
-- -----------------------------------------------------
USE `subwaytalent-dev`;
DROP procedure IF EXISTS `spSubway_GetEmailContents`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubway_GetEmailContents`(
	documentName	varchar(125)
)
BEGIN

	select * from document_template
    where docu_name = documentName;

END$$

DELIMITER ;



-- -----------------------------------------------------
-- procedure spSubway_RatePlannerToEvent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_RatePlannerToEvent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_RatePlannerToEvent`(
	plannerId varchar(30),   
    eventId	int,
	rating	varchar(5),
    feedback varchar(400),
    updatedBy varchar(30)
)
BEGIN
	declare result tinyint;    
    set result = 0;

	select count(1) into result from event_planner
							where event_id = eventid
                            and user_id = plannerId;
                            
                            
	if (result = 1) then
		begin
			declare ratingExists tinyint;  
            declare eventPlannerId int;
            set ratingExists = 0;
            set eventPlannerId = (select id from event_planner
									where event_id = eventid
									and user_id = plannerId);                              
            
            select count(1) into ratingExists
            from event_planner_feedbacks
            where rated_by = updatedBy
            and event_planner_id = eventPlannerId;
            
            if(ratingExists > 0) then
				begin 
					update event_planner_feedbacks
                    set user_rating = rating,
                    comments = feedback,
                    rated_date = UTC_TIMESTAMP()
                    
                    where event_planner_id = eventPlannerId
                    and rated_by = updatedBy;
                end;
			else
				begin
					insert into event_planner_feedbacks(event_planner_id,comments,
					user_rating,rated_by,rated_date)
					values(eventPlannerId,feedback,rating,updatedBy,UTC_TIMESTAMP());
                end;
			end if;          
            
            
            update user_account
			set rating = (select sum(pf.user_rating)/ count(nullif(pf.user_rating,''))
						 from event_planner as ep
                         inner join event_planner_feedbacks as pf
                         on ep.id = pf.event_planner_id
                         
						 where ep.user_id = plannerId)
			where userId = plannerId;
            
            select 1;
        end;
	else
		begin
			select 0;
        end;
	end if;
	

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_RateTalentToEvent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_RateTalentToEvent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_RateTalentToEvent`(
	talentID	varchar(30),   
    eventId	int,
	rating	varchar(5),
    feedback varchar(400),
    ratedBy varchar(30)
)
BEGIN

	declare result tinyint;    
    set result = 0;

	select count(1) into result from event_invites
							where event_id = eventid
                            and user_id = talentID
                            and status_id = 1;
                            
	if (result = 1) then
		begin
			update event_invites
			set user_rating = rating,
            comments = feedback,
            rated_by = ratedBy,
            rated_date = UTC_TIMESTAMP()
			
			where event_id = eventId
			and user_id = talentID;
            
            update user_account
			set ratingTalent = (select sum(user_rating)/ count(nullif(user_rating,''))
						 from event_invites
						 where user_id = talentID)
			where userId = talentID;
            
            select 1;
        end;
	else
		begin
			select 0;
        end;
	end if;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_RemoveDeviceId
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_RemoveDeviceId`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_RemoveDeviceId`(
	deviceId varchar(100),
    userId varchar(30)
)
BEGIN

	delete from user_devices
    where device_id = deviceId
    and user_id = userId;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_SearchEvents
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_SearchEvents`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_SearchEvents`(
	searchKeyWord	varchar(50),
    genreList	varchar(20),
    skillList varchar(20),
    userid	varchar(30)
)
BEGIN
	  
    declare searchSql varchar(450);
	declare genreSql varchar(200);
    declare skillSql varchar(200);
   
    set searchSql = '';	
    set genreSql = '';   
    set skillSql = '';
   
    if(trim(searchKeyWord) <> '') then
		begin
			set searchSql = concat(' and (match(e.Name,e.Title,e.description,e.location)
			against(concat("',searchKeyWord,'","*") in boolean mode)			
			or match(s.Name) against(concat("',searchKeyWord,'","*") in boolean mode)
			or match(et.type_name) against(concat("',searchKeyWord,'","*") in boolean mode)
			or match(ua.firstname,ua.lastName,ua.email,
			ua.location,ua.talentName) against(concat("',searchKeyWord,'","*") in boolean mode))');
        end;
    end if;
    
	if(trim(genreList) <> '') then
		begin			
			set genreSql = concat(' and (find_in_set(pg.genre_id,"', genreList,'"))');
		end;
    end if;
    
    if(trim(skillList) <> '') then
		begin			
			set skillSql = concat(' and (find_in_set(ps.skill_id,"', skillList,'"))');
		end;
    end if;

	set @sqlTxt	 = concat("select distinct e.id
						
			from event as e
            inner join event_types as et
			on e.type_id = et.id
			left outer join event_preferred_skills as ps
			on e.id = ps.event_id
			left outer join skills as s
			on ps.skill_id = s.id
			inner join event_planner as ep
			on ep.event_id = e.Id
			inner join user_account as ua
			on ua.userId = ep.user_id
			left outer join event_preferred_genre as pg
			on e.id = pg.event_id
            where  e.status = 1 
            and e.id not in (select event_id from event_invites where user_id = '",userid,"') "
            ,searchSql,genreSql,skillSql);
            
    --  select @sqlTxt;       
	  prepare stmts from @sqlTxt;
      execute stmts;
      deallocate prepare stmts;
    

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_SearchTalent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_SearchTalent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_SearchTalent`(
	searchKeyWord	varchar(50),
    genreList	varchar(20),
    skillList varchar(20),
    userid	varchar(30)
)
BEGIN
	
    declare searchSql varchar(450);
	declare genreSql varchar(200);
	declare skillSql varchar(200);
    
    set searchSql = '';	
    set genreSql = ''; 
    set skillSql = '';

	if(trim(searchKeyWord) <> '') then
		begin
			set searchSql = concat(' and (match(ua.firstname,ua.lastName,ua.email,
			ua.location, ua.talentName) 
            against(concat("',searchKeyWord,'","*") in boolean mode) 
			or match(s.Name) against(concat("',searchKeyWord,'","*") in boolean mode)    
			or ua.rate like concat("',searchKeyWord,'","%"))');
        end;
    end if;
    
	if(trim(genreList) <> '') then
		begin			
			set genreSql = concat(' and (find_in_set(tg.genre_id,"', genreList,'"))');
		end;
    end if;
    
    if(trim(skillList) <> '') then
		begin			
			set skillSql = concat(' and (find_in_set(ts.skill_id,"', skillList,'"))');
		end;
    end if;


	set @sqlTxt	 = concat("select distinct ua.userId    
					from user_account as ua
					left outer join talent_skills as ts
					on ua.userId = ts.user_id
					inner join skills as s
					on ts.skill_id = s.Id 
                    left outer join talent_genres as tg
					on ua.userId = tg.user_id
                    where rate is not null
					and rate <> ''"
					,searchSql,genreSql, skillSql);
            
     -- select @sqlTxt;       
	  prepare stmts from @sqlTxt;
      execute stmts;
      deallocate prepare stmts;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_SetProfilePic
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_SetProfilePic`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_SetProfilePic`(
	fileId int,
    user_Id_param	varchar(30),
    perspective	char(1)
    
)
BEGIN

	declare currentPicPath	varchar(200);
    declare fileType	char(2);
    
    set fileType = "TF";
    
    if(perspective = 'p') then 
		begin
			set fileType = "PF";
		end;
	end if;
    
    select profilePicTalent into currentPicPath from user_account 
    where userId = user_Id_param;

	-- update the file type of the current profile pic
	update user_files
    set file_type = "P"
    
    where file_path = currentPicPath
    and user_id = user_Id_param;

	-- set the file type of the new profile pic
	update user_files
    set file_type = fileType
    where id = fileId;
	
    if(perspective = 't') then
		begin
			update user_account
			set profilePicTalent = (select file_path from user_files
									where id = fileId limit 1)
			where userId = user_Id_param;
		end;
	end if;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_UpdateEvent
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_UpdateEvent`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_UpdateEvent`(
  eventId int,
  event_Name varchar(50),
  typeid int(11),
  date_Created datetime,
  date_Started datetime,
  date_End datetime,
  descr varchar(200),
  loc varchar(50),
  pic varchar(500),
  status_id tinyint(45),
  title_name varchar(45),
  lon varchar(50),
  lat varchar(50)
)
BEGIN

	update event
	set name = event_Name,
	type_id = typeid,
	dateCreated = date_Created,
	dateStarted = date_Started,
	dateEnd = date_End,
	description = descr,
	location = loc,
	picture = pic,
	status = status_id,
	title = title_name,
	longitude = lon,
	latitude = lat
	
	where Id = eventId;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_UpdateEventPlannerPayment
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_UpdateEventPlannerPayment`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_UpdateEventPlannerPayment`(
	paymentStatus tinyint,
    eventPlannerId int
)
BEGIN
	
    update event_planner
    set payment_status = paymentStatus,
    payment_date_update = UTC_TIMESTAMP()
    where id = eventPlannerId; 

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_UpdatePassword
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_UpdatePassword`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_UpdatePassword`(
	userIdParam varchar(30),
    passwordParam varchar(20)
)
BEGIN

	update user_login
    set Password = passwordParam
    
    where userid = userIdParam;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_UpdateSettings
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_UpdateSettings`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_UpdateSettings`(
	settingName varchar(45),
    settingValue varchar(45)
)
BEGIN

	declare result tinyint;
    set result = 0;
    
	select count(1) into result 
	from subway_settings
	where setting_name = settingName;
    
    if(result = 1) then 
		begin     
			update subway_settings
            set setting_value = settingValue,
            date_created = UTC_TIMESTAMP()
            where setting_name = settingName;          
            select 1;
        end;
	else
		begin			
            select 0;
        end;
	end if;
END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_UpdateTalentPaymentStatus
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_UpdateTalentPaymentStatus`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_UpdateTalentPaymentStatus`(
	eventPlannerId int,
    statusId tinyint
)
BEGIN

	declare eventId int;
    
    select event_id into eventId 
    from event_planner
    where id = eventPlannerId;
    
    
    update event_invites
    set payment_status = statusId,
	payment_date_update = UTC_Timestamp()
    
    where event_id = eventId
    and status_id = 1;
end$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_UpdateUser
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_UpdateUser`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_UpdateUser`(
	user_id varchar(30),
    bio varchar(1000),
    location varchar(100),
    rate varchar(30),
    rating varchar(5),
    profilePic varchar(500),
    email varchar(100),
    birthday datetime,
    firstname varchar(100),
    lastname varchar(100),
	cityStateId int,
    mobileNumber varchar(15),
    gender char(1),
    talentName varchar(100),
    profile_Pic_Talent varchar(500)
    
)
BEGIN

	update user_account
	set bio = bio,
    location = location,
    rate = rate,
    rating = rating,
    profilePic = profilePic,
    email = email,
    birthday = birthday,
    firstname = firstname,
    lastname = lastname,
    cityStateId = cityStateId,
    mobileNumber = mobileNumber,
    gender = gender,
    talentName = talentName,
	profilePicTalent = profile_Pic_Talent
    
    where userId = user_id;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_UpdateUserFile
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_UpdateUserFile`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_UpdateUserFile`(
	userId varchar(30),   
    fileType char(1),
    fileName varchar(100)
    
)
BEGIN


	update user_files
    set file_type =fileType
    
    
    where user_id = userId
    and name = fileName;

END$$

DELIMITER ;

-- -----------------------------------------------------
-- procedure spSubway_UpdateUserToken
-- -----------------------------------------------------

USE `subwaytalent-dev`;
DROP procedure IF EXISTS `subwaytalent-dev`.`spSubway_UpdateUserToken`;

DELIMITER $$
USE `subwaytalent-dev`$$
CREATE  PROCEDURE `spSubway_UpdateUserToken`(
	token varchar(255),
    userIdParam varchar(30),   
    expiresOn datetime
)
BEGIN

	update user_tokens
    set expires_on = expiresOn
    
    where auth_token = token
    and user_id = userIdParam;

END$$

DELIMITER ;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
