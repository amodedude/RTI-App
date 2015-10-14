alter table rti.customer_water add(firstSourcePercentage int);

UPDATE `rti`.`customer_water` SET `firstSourcePercentage`='100' WHERE `customer_customerID`='1' and
`first_sourceID`='2427' and`second_sourceID`='0';


ALTER TABLE rti.vessel DROP FOREIGN KEY fk_vessel_vessel_historical1;
ALTER TABLE rti.vessel DROP FOREIGN KEY fk_vessel_train1; 
ALTER TABLE `rti`.`train` 
CHANGE COLUMN `trainID` `trainID` INT(11) NOT NULL AUTO_INCREMENT ;
ALTER TABLE rti.vessel add CONSTRAINT `fk_vessel_train1` FOREIGN KEY (`train_trainID`) REFERENCES 
`train` (`trainID`) ON DELETE NO ACTION ON UPDATE NO ACTION;
alter table rti.vessel add column Salt_Split double default 0;
