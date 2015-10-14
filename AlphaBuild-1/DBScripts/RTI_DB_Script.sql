CREATE DATABASE `rti` /*!40100 DEFAULT CHARACTER SET utf8 */;

/* Cond_Archive */

CREATE TABLE `cond_archive` (
  `dataID` int(11) NOT NULL,
  `cond` int(11) DEFAULT NULL,
  `temp` int(11) DEFAULT NULL,
  `measurment_date` varchar(45) DEFAULT NULL,
  `sourceID` int(32) DEFAULT NULL,
  PRIMARY KEY (`dataID`),
  KEY `fk_water_data_sources1_idx` (`sourceID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/* Sources */

CREATE TABLE `sources` (
  `sources_sourceID` int(11) NOT NULL,
  `river` varchar(128) DEFAULT NULL,
  `city` varchar(255) DEFAULT NULL,
  `state` varchar(5) DEFAULT NULL,
  `country` varchar(45) DEFAULT NULL,
  `region` varchar(45) DEFAULT NULL,
  `measurement_location` varchar(255) DEFAULT NULL,
  `exact_lat` varchar(255) DEFAULT NULL,
  `exact_lng` varchar(255) DEFAULT NULL,
  `street_number` varchar(255) DEFAULT NULL,
  `street_name` varchar(255) DEFAULT NULL,
  `street_lat` varchar(255) DEFAULT NULL,
  `street_lng` varchar(255) DEFAULT NULL,
  `feature_class` varchar(255) DEFAULT NULL,
  `miles_from_site` varchar(255) DEFAULT NULL,
  `post_code` varchar(15) DEFAULT NULL,
  `place_name` varchar(255) DEFAULT NULL,
  `county_number` varchar(255) DEFAULT NULL,
  `county_name` varchar(255) DEFAULT NULL,
  `state_name` varchar(255) DEFAULT NULL,
  `agency` varchar(255) DEFAULT NULL,
  `agency_id` varchar(255) DEFAULT NULL,
  `full_site_name` varchar(255) DEFAULT NULL,
  `unique_site_name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`sources_sourceID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


/* Customer */

CREATE TABLE `customer` (
  `customerID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(45) DEFAULT NULL,
  `company` varchar(45) DEFAULT NULL,
  `plant` varchar(45) DEFAULT NULL,
  `city` varchar(45) DEFAULT NULL,
  `state` varchar(45) DEFAULT NULL,
  `acid_price` decimal(4,2) DEFAULT NULL,
  `caustic_price` decimal(4,2) DEFAULT NULL,
  `demand` int(11) DEFAULT NULL,
  `num_trains` int(10) unsigned DEFAULT NULL,
  `date_added` varchar(45) DEFAULT NULL,
  `notes` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`customerID`)
) ENGINE=InnoDB AUTO_INCREMENT=119 DEFAULT CHARSET=utf8;

/* Customer_Water */

CREATE TABLE `customer_water` (
  `customer_customerID` int(10) unsigned NOT NULL,
  `first_sourceID` int(11) NOT NULL,
  `second_sourceID` int(11) NOT NULL,
  `firstSourcePercentage` int(11) DEFAULT NULL,
  PRIMARY KEY (`customer_customerID`,`first_sourceID`,`second_sourceID`),
  KEY `fk_customer_water_sources1_idx` (`first_sourceID`),
  KEY `fk_customer_water_sources2_idx` (`second_sourceID`),
  CONSTRAINT `fk_customer_water_customer1` FOREIGN KEY (`customer_customerID`) REFERENCES `customer` (`customerID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_customer_water_sources1` FOREIGN KEY (`first_sourceID`) REFERENCES `sources` (`sources_sourceID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_customer_water_sources2` FOREIGN KEY (`second_sourceID`) REFERENCES `sources` (`sources_sourceID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/* Resin_Products */

CREATE TABLE `resin_products` (
  `resin_product_id` int(11) NOT NULL,
  `manufacturer` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `model_number` varchar(45) DEFAULT NULL,
  `resin_type` varchar(45) DEFAULT NULL,
  `primary_type` varchar(45) DEFAULT NULL,
  `group` varchar(45) DEFAULT NULL,
  `teir` varchar(45) DEFAULT NULL,
  `chemical_structure` varchar(45) DEFAULT NULL,
  `physical_structure` varchar(45) DEFAULT NULL,
  `color` varchar(45) DEFAULT NULL,
  `total_capacity` varchar(45) DEFAULT NULL,
  `salt_split_CAP` varchar(45) DEFAULT NULL,
  `price_per_cuft` varchar(45) DEFAULT NULL,
  `comments` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`resin_product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


/* Train */
CREATE TABLE `train` (
  `trainID` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) DEFAULT NULL,
  `number` int(11) DEFAULT NULL,
  `gpm` varchar(45) DEFAULT NULL,
  `num_beds_cation` int(11) DEFAULT NULL,
  `num_beds_anion` int(11) DEFAULT NULL,
  `using_manifold` varchar(45) DEFAULT NULL,
  `regens_per_month` varchar(45) DEFAULT NULL,
  `regen_duration` varchar(45) DEFAULT NULL,
  `has_mixed_bed` varchar(45) DEFAULT NULL,
  `has_historical_data` varchar(45) DEFAULT NULL,
  `customer_customerID` int(11) NOT NULL,
  PRIMARY KEY (`trainID`,`customer_customerID`)
) ENGINE=InnoDB AUTO_INCREMENT=152 DEFAULT CHARSET=utf8;

/* Vessel Historical */

CREATE TABLE `vessel_historical` (
  `sampleID` int(11) NOT NULL,
  `date` varchar(45) DEFAULT NULL,
  `lbs_chemical` int(11) DEFAULT NULL,
  `throughput` varchar(45) DEFAULT NULL,
  `num_regens` varchar(45) DEFAULT NULL,
  `toc` varchar(45) DEFAULT NULL,
  `vessel_historical_customerID` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`sampleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


/* Vessel */

CREATE TABLE `vessel` (
  `vesselID` int(11) NOT NULL,
  `vessel_number` int(11) DEFAULT NULL,
  `name` varchar(120) DEFAULT NULL,
  `size` varchar(45) DEFAULT NULL,
  `bed_number` varchar(45) DEFAULT NULL,
  `lbs_chemical` varchar(45) DEFAULT NULL,
  `date_replaced` varchar(45) DEFAULT NULL,
  `replacement_plan` varchar(45) DEFAULT NULL,
  `throughput` varchar(45) DEFAULT NULL,
  `num_regens` varchar(45) DEFAULT NULL,
  `toc` varchar(45) DEFAULT NULL,
  `with_degassifier` varchar(45) DEFAULT NULL,
  `with_polisher` varchar(45) DEFAULT NULL,
  `vessel_customerID` varchar(45) DEFAULT NULL,
  `train_trainID` int(11) NOT NULL,
  `resin_data_product_id` int(11) NOT NULL,
  `Salt_Split` double DEFAULT NULL,
  PRIMARY KEY (`vesselID`,`train_trainID`,`resin_data_product_id`),
  KEY `fk_vessel_train1_idx` (`train_trainID`),
  KEY `fk_vessel_resin_products1_idx` (`resin_data_product_id`),
  KEY `fk_vessel_vessel_historical1_idx` (`vesselID`),
  CONSTRAINT `fk_vessel_resin_products1` FOREIGN KEY (`resin_data_product_id`) REFERENCES `resin_products` (`resin_product_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_vessel_train1` FOREIGN KEY (`train_trainID`) REFERENCES `train` (`trainID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='<double-click to overwrite multiple objects>';


/* Water Data */

CREATE TABLE `water_data` (
  `dataID` int(11) NOT NULL,
  `cond` int(11) DEFAULT NULL,
  `temp` int(11) DEFAULT NULL,
  `measurment_date` varchar(45) DEFAULT NULL,
  `sourceID` int(32) DEFAULT NULL,
  PRIMARY KEY (`dataID`),
  KEY `fk_water_data_sources1_idx` (`sourceID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



