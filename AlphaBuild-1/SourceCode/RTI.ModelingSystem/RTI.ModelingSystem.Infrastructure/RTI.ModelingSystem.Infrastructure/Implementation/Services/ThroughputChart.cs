// -----------------------------------------------------------------------
// <copyright file="ThroughputChart.cs" company="RTI">
// RTI
// </copyright>
// <summary>Throughput Chart</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Models;

    #endregion Usings

    /// <summary>
	/// ThroughputChart class
	/// </summary>
	public class ThroughputChart
	{
		/// <summary>
		/// data To Send
		/// </summary>
		private PriceData dataToSend = new PriceData();

		/// <summary>
		/// calculation Run
		/// </summary>
		private bool calculationRun = false;

		/// <summary>
		/// train Resin Amount
		/// </summary>
		private double trainResinAmount;

        /// <summary>
        /// train Anion Resin Amount
        /// </summary>
        private double trainAnionResinAmount;

        /// <summary>
        /// train pounds of Chemical (Caustic)
        /// </summary>
        private double lbsAnionChemical;

		/// <summary>
		/// selected Train GPM
		/// </summary>
		private double selectedTrainGPM;

		/// <summary>
		/// current Salt Split Value
		/// </summary>
		private double currentSaltSplitValue;

		/// <summary>
		/// with Cleaning
		/// </summary>
		private bool withCleaning = false;

		/// <summary>
		/// with And Without Cleaning
		/// </summary>
		private bool withAndWithoutCleaning = true;

		/// <summary>
		/// train Gpm value
		/// </summary>
		private double[] trainGPM;

		/// <summary>
		/// train GPM Values
		/// </summary>
		public string[] trainGPMValues;

		/// <summary>
		/// Run Model which calculates the required computations
		/// </summary>
		/// <param name="startingSaltSplit">starting Salt Split</param>
		/// <param name="currentSaltSplit">current Salt Split</param>
		/// <param name="resinLifeExpectancy">resin Life Expectancy</param>
		/// <param name="simulationConfidence">simulation Confidence</param>
		/// <param name="numberSimulationIterations">number Simulation Iterations</param>
		/// <param name="simulationMethod">simulation Method</param>
		/// <param name="standardDeviationThreshold">standard Deviation Threshold</param>
		/// <param name="resinAge">resin Age parameter</param>
		/// <param name="replacementLevel">replacement Level</param>
		/// <param name="rtiCleaningLevel">rti Cleaning Level</param>
		/// <param name="donotReplaceResin">dont Replace Resin</param>
		/// <returns>Returns the Price data</returns>
        public PriceData RunModel(double startingSaltSplit, double currentSaltSplit, double resinLifeExpectancy, int simulationConfidence, int numberSimulationIterations, string simulationMethod, int standardDeviationThreshold, double resinAge, double replacementLevel, double rtiCleaningLevel, bool donotReplaceResin, List<double> regenTimes, int selectedTrainId, List<train> trains, List<vessel> vessels)
		{
			try
			{
				var trainList = dataToSend.TrainList;
				var numberOfTrains = dataToSend.NumberTrains;
				if (calculationRun)
				{
					dataToSend = new PriceData();
				}
				dataToSend.TrainList = trainList;
				dataToSend.NumberTrains = numberOfTrains;
				double numberOfWeeks = Convert.ToDouble(resinLifeExpectancy);
				var standardDevData = ProcessData.WeeklyWeightedAverageCondstdDev;
				RandomNumberSimulation conductivity = new RandomNumberSimulation();
				double roundedAgeValues = Convert.ToDouble(string.Format("{0:0.00}", resinAge));
				Dictionary<int, double> grainForeCast = conductivity.BeginCalculation(standardDevData, simulationConfidence, numberSimulationIterations, simulationMethod, standardDeviationThreshold, roundedAgeValues);
				int grainCount = grainForeCast.Count;
				int numberOfLoops = Convert.ToInt16(resinLifeExpectancy);
				if (numberOfLoops >= grainCount)
				{
					int weeksToAdd = Convert.ToInt32(numberOfLoops - grainCount);
					int lastGrainWeek = 0;
					if (grainForeCast != null && grainForeCast.Count > 0)
					{
						lastGrainWeek = grainForeCast.Last().Key + 1;
					}
					int grainIndx = 0;
					for (int loop = 0; loop <= weeksToAdd; loop++)
					{
						double grainToAdd = 0;
						if (grainForeCast != null && grainForeCast.Count > 0)
						{
							grainToAdd = grainForeCast.ElementAt(grainIndx).Value;
							grainForeCast.Add(lastGrainWeek, grainToAdd);
							lastGrainWeek++;
							grainIndx++;
						}
					}
				}
				dataToSend.GrainForcast = grainForeCast;
				double rtiCleaningeffectivness = 28.0;
                ThroughputBuilder tp = new ThroughputBuilder();
                Dictionary<DateTime, Tuple<int, double, string>> throughputCleanPrediction  = new Dictionary<DateTime,Tuple<int,double,string>>();
                Dictionary<DateTime, Tuple<int, double, string>> throughputNoCleanPrediction = new Dictionary<DateTime, Tuple<int, double, string>>();
                List<Tuple<double, double, double, double, double, double>> beforeConditions = new List<Tuple<double, double, double, double, double, double>>();
                List<Tuple<double, double, double, double, double, double>> afterConditions = new List<Tuple<double, double, double, double, double, double>>();
                List<Dictionary<DateTime, Tuple<int, double, string>>> throughputCleanPredictionAllTrains = new List<Dictionary<DateTime, Tuple<int, double, string>>>();
                List<Dictionary<DateTime, Tuple<int, double, string>>> throughputNoCleanPredictionAllTrains = new List<Dictionary<DateTime,Tuple<int,double,string>>>();
                List<List<Tuple<double, double, double, double, double, double>>> beforeConditionsAllTrains = new List<List<Tuple<double,double,double,double,double,double>>>();
                List<List<Tuple<double, double, double, double, double, double>>> afterConditionsAllTrains = new List<List<Tuple<double, double, double, double, double, double>>>();

                List<double> reginWeek_before = new List<double>();
                List<double> hoursWeek_before = new List<double>();
                List<double> reginTime_before = new List<double>();
                List<double> tpWeek_before = new List<double>();
                List<double> ssWeek_before = new List<double>();
                List<double> reginWeek_after = new List<double>();
                List<double> hoursWeek_after = new List<double>();
                List<double> reginTime_after = new List<double>();
                List<double> tpWeek_after = new List<double>();
                List<double> ssWeek_after = new List<double>();
                List<double> beforeSaltSplit = new List<double>();
                List<double> afterSaltSplit = new List<double>();
                List<int> weeks = new List<int>();
                List<Tuple<int, double>> regweekToSendNormal = new List<Tuple<int, double>>();
                List<Tuple<int, double>> regweekToSendClean = new List<Tuple<int, double>>();

                List<double> grainsWeek_before_AllTrains = new List<double>();
                List<double> reginWeek_before_AllTrains = new List<double>();
                List<double> hoursWeek_before_AllTrains = new List<double>();
                List<double> reginTime_before_AllTrains = new List<double>();
                List<double> tpWeek_before_AllTrains = new List<double>();
                List<double> ssWeek_before_AllTrains = new List<double>();

                List<double> grainsWeek_after_AllTrains = new List<double>();
                List<double> reginWeek_after_AllTrains = new List<double>();
                List<double> hoursWeek_after_AllTrains = new List<double>();
                List<double> reginTime_after_AllTrains = new List<double>();
                List<double> tpWeek_after_AllTrains = new List<double>();
                List<double> ssWeek_after_AllTrains = new List<double>();

                List<double> beforeSaltSplit_AllTrains = new List<double>();
                List<double> afterSaltSplit_AllTrains = new List<double>();
                List<int> weeks_AllTrains = new List<int>();
                List<Tuple<int, double>> regweekToSendNormal_AllTrains = new List<Tuple<int, double>>();
                List<Tuple<int, double>> regweekToSendClean_AllTrains = new List<Tuple<int, double>>();

                List<DateTime> dateWeekTpPair_AllTrains = new List<DateTime>();
                List<Tuple<double, double, string>> beforeWeekTP_Pair_AllTrains = new List<Tuple<double,double,string>>();
                List<Tuple<double, double, string>> afterWeekTP_Pair_AllTrains = new List<Tuple<double,double,string>>();


                int weekNumber = 0;
                double numRegensBefore = 0;
                double numRegensAfter = 0;

                if (selectedTrainId != 0)
                {
                    throughputCleanPrediction = tp.ThroughputBuild(replacementLevel, rtiCleaningLevel, currentSaltSplit, grainForeCast, trainAnionResinAmount, rtiCleaningeffectivness, selectedTrainGPM, resinAge, startingSaltSplit, numberOfWeeks, true, donotReplaceResin, regenTimes.Average(), selectedTrainId, trains.Count);
                    afterConditions = tp.AfterSystemConditions;
                    throughputNoCleanPrediction = tp.ThroughputBuild(replacementLevel, rtiCleaningLevel, currentSaltSplit, grainForeCast, trainAnionResinAmount, rtiCleaningeffectivness, selectedTrainGPM, resinAge, startingSaltSplit, numberOfWeeks, false, donotReplaceResin, regenTimes.Average(), selectedTrainId, trains.Count);
                    beforeConditions = tp.BeforeSystemConditions;

                    foreach (var week in beforeConditions)
                    {
                        weekNumber++;
                        weeks.Add(weekNumber);
                        reginWeek_before.Add(week.Item2);
                        hoursWeek_before.Add(week.Item3);
                        reginTime_before.Add(week.Item4);
                        tpWeek_before.Add(week.Item5);
                        ssWeek_before.Add(week.Item6);
                        beforeSaltSplit.Add(week.Item6);
                        Tuple<int, double> reginsPerWeeK = new Tuple<int, double>(weekNumber, week.Item2);
                        regweekToSendNormal.Add(reginsPerWeeK);
                    }
                    weekNumber = 0;
                    foreach (var week in afterConditions)
                    {
                        weekNumber++;
                        Tuple<int, double> reginsPerWeeK = new Tuple<int, double>(weekNumber, week.Item2);
                        afterSaltSplit.Add(week.Item6);
                        regweekToSendClean.Add(reginsPerWeeK);
                    }
                }
                else // If selected train == 0 (all trains) 
                {
                    // Perform the simulation for each train in the system
                    for (int train = 1; train <= trains.Count; train++) // for each train in the system
                    {
                        int trainID = Convert.ToInt32(trains.ElementAt(train-1).trainID);
                        selectedTrainGPM = Convert.ToDouble(trains.ElementAt(train - 1).gpm);
                        double regenTime = Convert.ToInt32(trains.ElementAt(train - 1).regen_duration);
                        double amtAnionResin = 0;
                        List<double> resinAges = new List<double>();
                        var train_vessels = vessels.Where(ves => ves.train_trainID == trainID);
                        int count = 0;
                        foreach (var vessel in train_vessels) // for each vessel in the current train
                        {
                            if (count % 2 != 0) // If it is an anion vessel
                            {
                                amtAnionResin += Convert.ToInt32(vessel.size);
                                DateTime purchaseDate = DateTime.Parse(vessel.date_replaced);
                                TimeSpan span = DateTime.Today - purchaseDate;
                                resinAges.Add(span.TotalDays);
                            }
                            count++;
                        }

                        double resAge = resinAges.Count() != 0 ? resinAges.Average() / 7 : 0;
                        throughputCleanPrediction = tp.ThroughputBuild(replacementLevel, rtiCleaningLevel, currentSaltSplit, grainForeCast, amtAnionResin, rtiCleaningeffectivness, selectedTrainGPM, resAge, startingSaltSplit, numberOfWeeks, true, donotReplaceResin, regenTime, trainID, trains.Count);
                        throughputCleanPredictionAllTrains.Add(throughputCleanPrediction);
                        afterConditions = tp.AfterSystemConditions;
                        afterConditionsAllTrains.Add(afterConditions);
                        throughputNoCleanPrediction = tp.ThroughputBuild(replacementLevel, rtiCleaningLevel, currentSaltSplit, grainForeCast, amtAnionResin, rtiCleaningeffectivness, selectedTrainGPM, resAge, startingSaltSplit, numberOfWeeks, false, donotReplaceResin, regenTime, trainID, trains.Count);
                        throughputNoCleanPredictionAllTrains.Add(throughputNoCleanPrediction);
                        beforeConditions = tp.BeforeSystemConditions;
                        beforeConditionsAllTrains.Add(beforeConditions);
                        numRegensBefore += beforeConditions.Average(regens => regens.Item2);
                        numRegensAfter += afterConditions.Average(regens => regens.Item2);
                        beforeConditions = new List<Tuple<double, double, double, double, double, double>>(); // Reset the before conditions 
                        afterConditions = new List<Tuple<double, double, double, double, double, double>>(); // Reset the after conditions 
                        throughputCleanPrediction = new Dictionary<DateTime, Tuple<int, double, string>>(); // Reset the before conditions
                        throughputNoCleanPrediction = new Dictionary<DateTime, Tuple<int, double, string>>(); // Reset the after conditions 
                        tp = new ThroughputBuilder(); // Reset the TP builder class 
                    }


                    int beforeMaxLength = beforeConditionsAllTrains.Select(len => len.Count()).ToList().Max();
                    int afterMaxLength = afterConditionsAllTrains.Select(len => len.Count()).ToList().Max();
                    List<Tuple<int, double, double, double, double, double>>[] before_conditions = new List<Tuple<int, double, double, double, double, double>>[beforeMaxLength];
                    List<Tuple<int, double, double, double, double, double>>[] after_conditions = new List<Tuple<int, double, double, double, double, double>>[afterMaxLength];

                    foreach (var trains_beforeConditions in beforeConditionsAllTrains) // Cycle through all of the before system conditions for each train
                    {
                        foreach (var week in trains_beforeConditions) // Go through each week's before conditions for each train in the system and add to an array of lists
                        {
                            Tuple<int, double, double, double, double, double> thisWeeksBeforeConditions = new Tuple<int,double,double,double,double,double>(weekNumber+1, week.Item2, week.Item3, week.Item4, week.Item5, week.Item6);
                            
                            if (before_conditions[weekNumber] == null) // Initialize the array object if it is empty
                            before_conditions[weekNumber] = new List<Tuple<int, double, double, double, double, double>>();

                            before_conditions[weekNumber].Add(thisWeeksBeforeConditions);
                            weekNumber++;
                        }
                        weekNumber = 0;
                    }
                    weekNumber = 0;
                    foreach (var trains_afterConditions in afterConditionsAllTrains) // Cycle through all of the before system conditions for each train 
                    {
                        foreach (var week in trains_afterConditions) // Go through each week's after conditions for each train in the system and add to an array of lists
                        {
                            Tuple<int, double, double, double, double, double> thisWeeksAfterConditions = new Tuple<int, double, double, double, double, double>(weekNumber+1, week.Item2, week.Item3, week.Item4, week.Item5, week.Item6);

                            if (after_conditions[weekNumber] == null) // Initialize the array object if it is empty
                            after_conditions[weekNumber] = new List<Tuple<int, double, double, double, double, double>>();

                            after_conditions[weekNumber].Add(thisWeeksAfterConditions);
                            weekNumber++;
                        }
                        weekNumber = 0;
                    }
  
                    foreach (var weekly_conditions in before_conditions) // Average together the data for each of the trains before conditions
                    {
                        weekNumber++;
                        weeks_AllTrains.Add(weekNumber + 1);

                        double _grainWeek = weekly_conditions.Select(grains => grains.Item1).ToList().Average();
                        double _reginWeek = weekly_conditions.Select(regenPerWeek => regenPerWeek.Item2).ToList().Average();                           
                        double _hoursWeek = weekly_conditions.Select(hrPerRun => hrPerRun.Item3).ToList().Average();
                        double _reginTime = weekly_conditions.Select(regenTime => regenTime.Item4).ToList().Average();
                        double _tpWeek = weekly_conditions.Select(throughput => throughput.Item5).ToList().Average();
                        double _ssWeek = weekly_conditions.Select(SaltSplit => SaltSplit.Item6).ToList().Average();
                        double _beforeSS = weekly_conditions.Select(SaltSplit => SaltSplit.Item6).ToList().Average();

                        grainsWeek_before_AllTrains.Add(_grainWeek);
                        reginWeek_before_AllTrains.Add(_reginWeek);
                        hoursWeek_before_AllTrains.Add(_hoursWeek);
                        reginTime_before_AllTrains.Add(_reginTime);
                        tpWeek_before_AllTrains.Add(_tpWeek);
                        ssWeek_before_AllTrains.Add(_ssWeek);
                        beforeSaltSplit_AllTrains.Add(_beforeSS);

                        Tuple<int, double> reginsPerWeeK = new Tuple<int, double>(weekNumber, weekly_conditions.Select(regenPerWeek => regenPerWeek.Item2).ToList().Average());
                        Tuple<double, double, double, double, double, double> beforeCondition = new Tuple<double,double,double,double,double,double>(_grainWeek, _reginWeek, _hoursWeek, _reginTime, _tpWeek, _ssWeek);
                        regweekToSendNormal_AllTrains.Add(reginsPerWeeK);
                        beforeConditions.Add(beforeCondition);
                    }

                    // Use the new average value from each of the train simulations BEFORE
                    weeks = weeks_AllTrains;
                    reginWeek_before = reginWeek_before_AllTrains;
                    hoursWeek_before = hoursWeek_before_AllTrains;
                    reginTime_before = reginTime_before_AllTrains;
                    tpWeek_before = tpWeek_before_AllTrains;
                    ssWeek_before = ssWeek_before_AllTrains;

                    weekNumber = 0;
                    foreach (var weekly_conditions in after_conditions) // Average together the data for each of the trains after conditions 
                    {
                        weekNumber++;

                        double _grainWeek = weekly_conditions.Select(grains => grains.Item1).ToList().Average();
                        double _reginWeek = weekly_conditions.Select(regenPerWeek => regenPerWeek.Item2).ToList().Average();
                        double _hoursWeek = weekly_conditions.Select(hrPerRun => hrPerRun.Item3).ToList().Average();
                        double _reginTime = weekly_conditions.Select(regenTime => regenTime.Item4).ToList().Average();
                        double _tpWeek = weekly_conditions.Select(throughput => throughput.Item5).ToList().Average();
                        double _ssWeek = weekly_conditions.Select(SaltSplit => SaltSplit.Item6).ToList().Average();
                        double _afterSS = weekly_conditions.Select(SaltSplit => SaltSplit.Item6).ToList().Average();

                        grainsWeek_after_AllTrains.Add(_grainWeek);
                        reginWeek_after_AllTrains.Add(_reginWeek);
                        hoursWeek_after_AllTrains.Add(_hoursWeek);
                        reginTime_after_AllTrains.Add(_reginTime);
                        tpWeek_after_AllTrains.Add(_tpWeek);
                        ssWeek_after_AllTrains.Add(_ssWeek);
                        afterSaltSplit_AllTrains.Add(_afterSS);


                        Tuple<int, double> reginsPerWeeK = new Tuple<int, double>(weekNumber, weekly_conditions.Select(regenPerWeek => regenPerWeek.Item2).ToList().Average());
                        Tuple<double, double, double, double, double, double> afterCondition = new Tuple<double, double, double, double, double, double>(_grainWeek, _reginWeek, _hoursWeek, _reginTime, _tpWeek, _ssWeek);
                        regweekToSendClean_AllTrains.Add(reginsPerWeeK);
                        afterConditions.Add(afterCondition);
                    }

                    // Use the new average value from each of the train simulations AFTER
                    reginWeek_after = reginWeek_after_AllTrains;
                    hoursWeek_after = hoursWeek_after_AllTrains;
                    reginTime_after = reginTime_after_AllTrains;
                    tpWeek_after = tpWeek_after_AllTrains;
                    ssWeek_after = ssWeek_after_AllTrains;

                    regweekToSendNormal = regweekToSendNormal_AllTrains;
                    regweekToSendClean = regweekToSendClean_AllTrains;

                    beforeMaxLength = throughputNoCleanPredictionAllTrains.Select(len => len.Count()).ToList().Max();
                    afterMaxLength = throughputCleanPredictionAllTrains.Select(len => len.Count()).ToList().Max();
                    List<Dictionary<DateTime, Tuple<int, double, string>>>[] before = new List<Dictionary<DateTime, Tuple<int, double, string>>>[beforeMaxLength];
                    List<Dictionary<DateTime, Tuple<int, double, string>>>[] after = new List<Dictionary<DateTime, Tuple<int, double, string>>>[afterMaxLength];

                    weekNumber = 0;
                    foreach (var trains_beforeConditions in throughputNoCleanPredictionAllTrains) // Cycle through all of the before system conditions for each train
                    {
                        foreach (var week in trains_beforeConditions) // Go through each week's before conditions for each train in the system and add to an array of lists
                        {
                            Dictionary<DateTime, Tuple<int, double, string>> dictionary = new Dictionary<DateTime, Tuple<int, double, string>>();
                            Tuple<int, double, string> thisWeeksBeforeConditions = new Tuple<int, double, string>(week.Value.Item1, week.Value.Item2, week.Value.Item3);
                            dictionary.Add(week.Key, thisWeeksBeforeConditions);

                            if (before[weekNumber] == null) // Initialize the array object if it is empty
                                before[weekNumber] = new List<Dictionary<DateTime, Tuple<int, double, string>>>();


                            before[weekNumber].Add(dictionary);
                            weekNumber++;
                        }
                        weekNumber = 0;
                    }

                    weekNumber = 0;
                    foreach (var trains_afterConditions in throughputCleanPredictionAllTrains) // Cycle through all of the before system conditions for each train
                    {
                        foreach (var week in trains_afterConditions) // Go through each week's before conditions for each train in the system and add to an array of lists
                        {
                            Dictionary<DateTime, Tuple<int, double, string>> dictionary = new Dictionary<DateTime, Tuple<int, double, string>>();
                            Tuple<int, double, string> thisWeeksAfterConditions = new Tuple<int, double, string>(week.Value.Item1, week.Value.Item2, week.Value.Item3);
                            dictionary.Add(week.Key, thisWeeksAfterConditions);

                            if (after[weekNumber] == null) // Initialize the array object if it is empty
                                after[weekNumber] = new List<Dictionary<DateTime, Tuple<int, double, string>>>();


                            after[weekNumber].Add(dictionary);
                            weekNumber++;
                        }
                        weekNumber = 0;
                    }

                    weekNumber = 0;
                    foreach (var weekly_conditions in before) // Average together the data for each of the trains before conditions
                    {
                        List<int> grains = new List<int>();
                        List<double> throughputVal = new List<double>();
                        List<string> cleaOrReplace = new List<string>();
                        int len = weekly_conditions.Count()-1;
                        for (int trn = 0; trn <= len; trn++)
                        {
                            grains.Add(weekly_conditions.ElementAt(trn).Values.ElementAt(0).Item1);
                            throughputVal.Add(weekly_conditions.ElementAt(trn).Values.ElementAt(0).Item2);
                            cleaOrReplace.Add(weekly_conditions.ElementAt(trn).Values.ElementAt(0).Item3);   
                        }

                        //dateWeekTpPair_AllTrains = weekly_conditions
                        //weekNumber++;
                        //weeks_AllTrains.Add(weekNumber + 1);
                        //grainsWeek_AllTrains.Add(weekly_conditions.Select(grains => grains.Item1).ToList().Average());
                        //reginWeek_AllTrains.Add(weekly_conditions.Select(regenPerWeek => regenPerWeek.Item2).ToList().Average());
                        //hoursWeek_AllTrains.Add(weekly_conditions.Select(hrPerRun => hrPerRun.Item3).ToList().Average());
                        //reginTime_AllTrains.Add(weekly_conditions.Select(regenTime => regenTime.Item4).ToList().Average());
                        //tpWeek_AllTrains.Add(weekly_conditions.Select(throughput => throughput.Item5).ToList().Average());
                        //Tuple<int, double> reginsPerWeeK = new Tuple<int, double>(weekNumber, weekly_conditions.Select(regenPerWeek => regenPerWeek.Item2).ToList().Average());
                        //regweekToSendNormal_AllTrains.Add(reginsPerWeeK);
                        //if (grains.First() == 120 || weekNumber == 80)
                        //{
                        //    // stop here
                        //}

                        Tuple<int, double, string> thisWeeksBeforeConditions = new Tuple<int, double, string>(grains.First(), throughputVal.Average(), cleaOrReplace.First());
                        //dictionary.Add(weekly_conditions.ElementAt(0).Keys.ElementAt(0), thisWeeksAfterConditions);

                        throughputNoCleanPrediction.Add(weekly_conditions.ElementAt(0).Keys.ElementAt(0), thisWeeksBeforeConditions);
                        //.Select(x => tranform(x))
                        //         .Where(y => y != null) //Check for nulls
                        //         .ToList();

                        weekNumber++;
                    }

                    weekNumber = 0;
                    foreach (var weekly_conditions in after) // Average together the data for each of the trains before conditions
                    {
                        List<int> grains = new List<int>();
                        List<double> throughputVal = new List<double>();
                        List<string> cleaOrReplace = new List<string>();
                        int len = weekly_conditions.Count() - 1;
                        for (int trn = 0; trn <= len; trn++)
                        {
                            grains.Add(weekly_conditions.ElementAt(trn).Values.ElementAt(0).Item1);
                            throughputVal.Add(weekly_conditions.ElementAt(trn).Values.ElementAt(0).Item2);
                            cleaOrReplace.Add(weekly_conditions.ElementAt(trn).Values.ElementAt(0).Item3);
                        }
                        
                        Tuple<int, double, string> thisWeeksAfterConditions = new Tuple<int, double, string>(grains.First(), throughputVal.Average(), cleaOrReplace.First());
                        throughputCleanPrediction.Add(weekly_conditions.ElementAt(0).Keys.ElementAt(0), thisWeeksAfterConditions);


                        weekNumber++;
                    }
                }

				dataToSend.RegensPerWeekNormalOps = regweekToSendNormal;
				dataToSend.CleanThroughput = throughputCleanPrediction;
				dataToSend.NormalOpsThroughput = throughputNoCleanPrediction;
				double averageTPBefore = 0;
                if (tpWeek_before != null && tpWeek_before.Count > 0)
				{
                    averageTPBefore = Math.Round(tpWeek_before.Average(), 0);
				}
                if (reginWeek_before != null && reginWeek_before.Count > 0 && selectedTrainId != 0)
				{
                    dataToSend.RegensPerWeekAverageBefore = Math.Round(reginWeek_before.Average(), 2);
				}
                else
                {
                    dataToSend.RegensPerWeekAverageBefore = Math.Round(numRegensBefore, 2);
                }
                if (hoursWeek_before != null && hoursWeek_before.Count > 0)
				{
                    dataToSend.HoursPerRunAverageBefore = Math.Round(hoursWeek_before.Average(), 2);
				}
                if (reginTime_before != null && reginTime_before.Count > 0)
				{
                    dataToSend.RegenTimeAverageBefore = Math.Round(reginTime_before.Average(), 2);
				}
				dataToSend.ThroughputAverageBefore = averageTPBefore;
                if (ssWeek_before != null && ssWeek_before.Count > 0)
				{
                    dataToSend.NumberRegens = Math.Round(ssWeek_before.Average());
				}
				if (withCleaning || withAndWithoutCleaning)
				{
                    //reginWeek_after.Clear();
                    //hoursWeek_after.Clear();
                    //reginTime_after.Clear();
                    //tpWeek_after.Clear();
                    //ssWeek_after.Clear();
					foreach (var week in afterConditions)
					{
						reginWeek_after.Add(week.Item2);
                        hoursWeek_after.Add(week.Item3);
                        reginTime_after.Add(week.Item4);
                        tpWeek_after.Add(week.Item5);
                        ssWeek_after.Add(week.Item6);
					}
                    if (reginWeek_after != null && reginWeek_after.Count > 0 && selectedTrainId != 0)
					{
                        dataToSend.RegensPerWeekAverageAfter = Math.Round(reginWeek_after.Average(), 2);
					}
                    else
                    {
                        dataToSend.RegensPerWeekAverageAfter = Math.Round(numRegensAfter, 2);
                    }
                    if (hoursWeek_after != null && hoursWeek_after.Count > 0)
					{
                        dataToSend.HoursPerRunAverageAfter = Math.Round(hoursWeek_after.Average(), 2);
					}
                    if (reginTime_after != null && reginTime_after.Count > 0)
					{
                        dataToSend.RegenTimeAverageAfter = Math.Round(reginTime_after.Average(), 2);
					}
                    if (tpWeek_after != null && tpWeek_after.Count > 0)
					{
                        dataToSend.ThroughputAverageAfter = Math.Round(tpWeek_after.Average(), 0);
					}
				}


                //// Clear previous data used in ALL TRAINS calculation
                //if (selectedTrainId != 0)
                //{
                //    tp = new ThroughputBuilder();
                //    throughputCleanPrediction = new Dictionary<DateTime, Tuple<int, double, string>>();
                //    throughputNoCleanPrediction = new Dictionary<DateTime, Tuple<int, double, string>>();
                //    beforeConditions = new List<Tuple<double, double, double, double, double, double>>();
                //    afterConditions = new List<Tuple<double, double, double, double, double, double>>();
                //    regweekToSendClean = new List<Tuple<int, double>>();
                //    regweekToSendNormal = new List<Tuple<int, double>>();

                //    throughputCleanPrediction = tp.ThroughputBuild(replacementLevel, rtiCleaningLevel, currentSaltSplit, grainForeCast, trainAnionResinAmount, rtiCleaningeffectivness, selectedTrainGPM, resinAge, startingSaltSplit, numberOfWeeks, true, donotReplaceResin, regenTimes.Average(), selectedTrainId, trains.Count);
                //    afterConditions = tp.AfterSystemConditions;
                //    throughputNoCleanPrediction = tp.ThroughputBuild(replacementLevel, rtiCleaningLevel, currentSaltSplit, grainForeCast, trainAnionResinAmount, rtiCleaningeffectivness, selectedTrainGPM, resinAge, startingSaltSplit, numberOfWeeks, false, donotReplaceResin, regenTimes.Average(), selectedTrainId, trains.Count);
                //    beforeConditions = tp.BeforeSystemConditions;

                //    weeks.Clear();
                //    reginWeek.Clear();
                //    hoursWeek.Clear();
                //    reginTime.Clear();
                //    tpWeek.Clear();
                //    ssWeek.Clear();
                //    beforeSaltSplit.Clear();
                //    afterSaltSplit.Clear();

                //    foreach (var week in beforeConditions)
                //    {
                //        weekNumber++;
                //        weeks.Add(weekNumber);
                //        reginWeek.Add(week.Item2);
                //        hoursWeek.Add(week.Item3);
                //        reginTime.Add(week.Item4);
                //        tpWeek.Add(week.Item5);
                //        ssWeek.Add(week.Item6);
                //        beforeSaltSplit.Add(week.Item6);
                //        Tuple<int, double> reginsPerWeeK = new Tuple<int, double>(weekNumber, week.Item2);
                //        regweekToSendNormal.Add(reginsPerWeeK);
                //    }
                //    weekNumber = 0;
                //    foreach (var week in afterConditions)
                //    {
                //        weekNumber++;
                //        Tuple<int, double> reginsPerWeeK = new Tuple<int, double>(weekNumber, week.Item2);
                //        afterSaltSplit.Add(week.Item6);
                //        regweekToSendClean.Add(reginsPerWeeK);
                //    }
                //}
				
				Dictionary<int, Tuple<double?, double?>> saltSplitData = new Dictionary<int, Tuple<double?, double?>>();
				int length;
				if (beforeSaltSplit.Count < afterSaltSplit.Count)
				{
					length = afterSaltSplit.Count;
				}
				else
				{
					length = beforeSaltSplit.Count;
				}
				for (int i = 0; i < length; i++)
				{
					int aftercount = afterSaltSplit.Count;
					int beforecount = beforeSaltSplit.Count;
					double? before = 0;
					double? after = 0;
					int week = 0;
					bool shouldAdd = true;

					if (beforecount > aftercount)
					{
						if (i < aftercount)
						{
							before = beforeSaltSplit.ElementAt(i);
							after = afterSaltSplit.ElementAt(i);
							week = i + 1;
						}
						else
						{
							shouldAdd = false;
						}
					}
					else if (beforecount < aftercount)
					{
						if (i < beforecount)
						{
							before = beforeSaltSplit.ElementAt(i);
							after = afterSaltSplit.ElementAt(i);
							week = i + 1;
						}
						else
						{
							shouldAdd = false;
						}
					}
					else if (beforecount == aftercount)
					{
						before = beforeSaltSplit.ElementAt(i);
						after = afterSaltSplit.ElementAt(i);
						week = i + 1;
					}
					Tuple<double?, double?> temp = new Tuple<double?, double?>(before, after);
					if (shouldAdd)
					{
                        saltSplitData.Add(week + ((throughputCleanPrediction.Values.ElementAt(0).Item1) - 1), temp);
					}
				}

                dataToSend.RegensPerWeekNormalOps = regweekToSendNormal;
				dataToSend.RegensPerWeekClean = regweekToSendClean;
				dataToSend.CleanThroughput = throughputCleanPrediction;
				dataToSend.NormalOpsThroughput = throughputNoCleanPrediction;
				dataToSend.SaltSplit = saltSplitData;
				dataToSend.NumberWeeks = numberOfWeeks;
				return dataToSend;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Loads the Train Data
		/// </summary>
		/// <param name="numberTrains">number of Trains</param>
		/// <param name="dbTrainGpm">db Train Gpm</param>
		public void LoadTrainData(int numberTrains, string[] dbTrainGpm)
		{
			try
			{
				int size = dbTrainGpm.Count();
				trainGPM = new double[size];
				for (int i = 0; i < size; i++)
				{
					trainGPM[i] = Convert.ToDouble(dbTrainGpm[i]);
				}
				DataTable trainList = new DataTable();
				trainList.Columns.Add("Train#", typeof(long));
				trainList.Columns.Add("Name", typeof(string));
				for (int max = 0; max < numberTrains; max++)
				{
					DataRow row = trainList.NewRow();
					string trainName;
					if (max != 0)
					{
						trainName = "Train# " + max.ToString();
					}
					else
					{
						trainName = "All Trains (System)";
					}
					row["Train#"] = max;
					row["Name"] = trainName;
					trainList.Rows.Add(row);
				}
				dataToSend.TrainList = trainList;
				dataToSend.NumberTrains = numberTrains;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Loads the Model
		/// </summary>
		/// <param name="customerId">customer Identifier</param>
		/// <param name="currentSaltSplit">current Salt Split</param>
		/// <param name="trainRepository">train Repository</param>
		/// <param name="predictiveRepository">predictive Repository</param>
		/// <param name="conductivityS1">conductivity S1</param>
		/// <param name="conductivityS2">conductivity S2</param>
		/// <param name="sourceOnePercent">source One Percent</param>
		/// <param name="startingSaltSplit">starting Salt Split</param>
		/// <param name="resinLifeExpectancy">resin Life Expectancy</param>
		/// <param name="simulationConfidence">simulation Confidence</param>
		/// <param name="numberSimulationIterations">number of Simulation Iterations</param>
		/// <param name="simulationMethod">simulation Method</param>
		/// <param name="standardDevThreshold">standardDev Threshold</param>
		/// <param name="resinAge">resin Age parameter</param>
		/// <param name="replacementLevel">replacement Level</param>
		/// <param name="rtiCleaningLevel">rti Cleaning Level</param>
		/// <param name="selectedTrain">selected Train</param>
		/// <param name="donotReplaceResin">donot Replace Resin</param>
		/// <returns>Returns the price data</returns>
		public PriceData LoadModel(string customerId, double currentSaltSplit, IRepository<train> trainRepository, IPredictiveModelRepository predictiveRepository, List<string>[] conductivityS1, List<string>[] conductivityS2, double sourceOnePercent, double startingSaltSplit, double resinLifeExpectancy, int simulationConfidence, int numberSimulationIterations, string simulationMethod, int standardDevThreshold, double resinAge, double replacementLevel, double rtiCleaningLevel, string selectedTrain, bool donotReplaceResin)
		{
			try
			{
				ProcessData process = new ProcessData();
				process.Compute(conductivityS1, conductivityS2, sourceOnePercent);
				currentSaltSplitValue = currentSaltSplit;
				List<vessel> lstCustomerVessels = new List<vessel>();
				lstCustomerVessels = predictiveRepository.GetCustomerVessels(Convert.ToInt64(customerId));
				List<BedNum_Interval> intervalList = new List<BedNum_Interval>();
				List<double> vesselSizeList = new List<double>();
				List<double> lbsChemicalList = new List<double>();
				TimeSpan intervalSum = new TimeSpan();
				List<double> replacementPlan = new List<double>();
				int selectedTrainId;
				if (selectedTrain == "0")
				{
					selectedTrainId = 0;
				}
				else
				{
					selectedTrainId = Convert.ToInt16(selectedTrain);
					lstCustomerVessels = lstCustomerVessels.Where(item => selectedTrain == Convert.ToString(item.train_trainID)).ToList();
				}
				if (lstCustomerVessels != null)
				{
					foreach (var vesselList in lstCustomerVessels)
					{
						int trainID = Convert.ToInt32(vesselList.train_trainID);
						if (trainID == selectedTrainId)
						{
							DateTime purchase_date = Convert.ToDateTime(vesselList.date_replaced, new CultureInfo("en-US", true));
							TimeSpan interval = DateTime.Today - purchase_date;
                            BedNum_Interval bednum_interval = new BedNum_Interval();
                            bednum_interval.bed_number = vesselList.bed_number;
                            bednum_interval.span = interval;
							intervalList.Add(bednum_interval);
							replacementPlan.Add(Convert.ToInt32(vesselList.replacement_plan));
                            double lbsChemical = Convert.ToDouble(vesselList.lbs_chemical);
							double vesselSize = Convert.ToDouble(vesselList.size);
							vesselSizeList.Add(vesselSize);
							lbsChemicalList.Add(lbsChemical);
						}
						if (selectedTrainId == 0)
						{
							DateTime purchase_date = Convert.ToDateTime(vesselList.date_replaced, new CultureInfo("en-US", true));
							TimeSpan interval = DateTime.Today - purchase_date;
                            BedNum_Interval bednum_interval = new BedNum_Interval();
                            bednum_interval.bed_number = vesselList.bed_number;
                            bednum_interval.span = interval;
                            intervalList.Add(bednum_interval);
							replacementPlan.Add(Convert.ToInt32(vesselList.replacement_plan));
							double lbsChemical = Convert.ToDouble(vesselList.lbs_chemical);
							double vesselSize = Convert.ToDouble(vesselList.size);
							vesselSizeList.Add(vesselSize);
							lbsChemicalList.Add(lbsChemical);
						}
					}
				}

                    int anionCount = 0;
                    foreach (var span in intervalList)
                    {
                        // Ensure we are only summing the ANION resin age only
                        if (span.bed_number == "2")
                        {
                            intervalSum += span.span;
                            anionCount++;
                        }
                    }


                double average = intervalSum.TotalMilliseconds / anionCount;
				TimeSpan averageResinAge = new TimeSpan();
				if (!double.IsNaN(average))
				{
					averageResinAge = TimeSpan.FromMilliseconds(average);
				}
				double size = 0.0;
				if (vesselSizeList != null && vesselSizeList.Count > 0)
				{
					size = vesselSizeList.Sum();
				}

				trainResinAmount = size;
				bool cation = true;
				foreach (var vesselSize in vesselSizeList)
				{
					if (cation)
					{
						dataToSend.AmountCation += vesselSize;
					}
					else
					{
						dataToSend.AmountAnion += vesselSize;
					}
					cation = !cation;
				}

                trainAnionResinAmount = dataToSend.AmountAnion; // Set the amount of resin for just the ANION

				cation = true;

				foreach (var chemAmt in lbsChemicalList)
				{
					if (cation)
					{
						dataToSend.AcidUsage = chemAmt;
					}
					else
					{
						dataToSend.CausticUsage = chemAmt;
					}
					cation = !cation;
				}

                lbsAnionChemical = dataToSend.CausticUsage; // Set the amount of Caustic 

				int customerCustomerId = Convert.ToInt16(customerId);
				List<train> trains = trainRepository.GetAll().Where(p => p.customer_customerID == customerCustomerId).ToList();
				trainGPMValues = new string[trains.Count];
				trainGPM = new double[trains.Count];
				for (int i = 0; i < trains.Count; i++)
				{
					trainGPMValues[i] = trains[i].gpm;
					trainGPM[i] = Convert.ToDouble(trains[i].gpm);
				}

				if (selectedTrainId == 0 && trainGPM != null && trainGPM.Length > 0)
				{
					selectedTrainGPM = trainGPM.Average();
				}
				else if (trainGPM != null && trainGPM.Length > 0)
				{
					selectedTrainGPM = trainGPM[0];
				}

				this.LoadTrainData(2, trainGPMValues);
                
                List<double> regenTimes = new List<double>();
                foreach (var train in trains)
                {
                    regenTimes.Add(Convert.ToInt32(train.regen_duration));
                }

				PriceData priceData = this.RunModel(startingSaltSplit, currentSaltSplit, resinLifeExpectancy, simulationConfidence, numberSimulationIterations, simulationMethod, standardDevThreshold, resinAge, replacementLevel, rtiCleaningLevel, donotReplaceResin, regenTimes, selectedTrainId, trains, lstCustomerVessels);
				return priceData;
			}
			catch
			{
				throw;
			}
		}

	}
}
