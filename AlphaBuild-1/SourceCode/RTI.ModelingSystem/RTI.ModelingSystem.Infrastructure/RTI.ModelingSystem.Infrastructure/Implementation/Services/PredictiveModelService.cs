// -----------------------------------------------------------------------
// <copyright file="PredictiveModelService.cs" company="RTI">
// RTI
// </copyright>
// <summary>Predictive Model Service</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Interfaces.Services;
    using RTI.ModelingSystem.Core.Models;

    #endregion Usings

    /// <summary>
    /// Predictive Model Service
    /// </summary>
    public class PredictiveModelService : IPredictiveModelService
    {
        #region Properties

        /// <summary>
        /// converted data
        /// </summary>
        private static Dictionary<double, double> convertedSaltSplitPoints = new Dictionary<double, double>();

        /// <summary>
        /// modified waterdataRepository
        /// </summary>
        private IRepository<water_data> modifiedwaterdataRepository;

        /// <summary>
        /// modified TrainRepository
        /// </summary>
        private IRepository<train> modifiedTrainRepository;

        /// <summary>
        /// Predictive Repository
        /// </summary>
        private IPredictiveModelRepository predictiveRepository;

        /// <summary>
        /// modified VesselRepository
        /// </summary>
        private IVesselRepository modifiedVesselRepository;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PredictiveModelService" /> class
        /// </summary>
        /// <param name="trainRepository">trainRepository parameter</param>
        /// <param name="waterdataRepository">waterdataRepository parameter</param>
        /// <param name="predictiveRepository">predictiveRepository parameter</param>
        /// <param name="vesselRepository">vesselRepository parameter</param>
        public PredictiveModelService(IRepository<train> trainRepository, IRepository<water_data> waterdataRepository, IPredictiveModelRepository predictiveRepository, IVesselRepository vesselRepository)
        {
            this.modifiedwaterdataRepository = waterdataRepository;
            this.predictiveRepository = predictiveRepository;
            this.modifiedTrainRepository = trainRepository;
            this.modifiedVesselRepository = vesselRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Computes Data Points
        /// </summary>
        /// <param name="numWeeks">number of Weeks</param>
        /// <param name="startingSaltSplit">starting Salt Split</param>
        /// <param name="maximumDegradationSaltSplit">maximum Degradation Salt Split</param>
        /// <returns>Returns dictionary</returns>
        public Dictionary<double, double> ComputeDataPoints(double numberWeeks, double startingSaltSplit, double maximumDegradationSaltSplit)
        {
            try
            {
                PercentOfTimeCalculator(numberWeeks);
                return SaltSplitDegradationCalculator(numberWeeks, maximumDegradationSaltSplit, startingSaltSplit);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// percent Of Time Calculator
        /// </summary>
        /// <param name="numWks">number of Weeks</param>
        private static void PercentOfTimeCalculator(double numWks)
        {
            try
            {
                Dictionary<double, double> percentOfTime = new Dictionary<double, double>();
                for (int wk = 1; wk <= numWks; wk++)
                {
                    double percent = wk / numWks;
                    percentOfTime.Add(wk, percent);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// salt Split Degradation Calculator
        /// </summary>
        /// <param name="numberOfWeeks">number of Weeks</param>
        /// <param name="maximumDegradationSaltSplit">maximum Degradation Salt Split</param>
        /// <param name="startingSaltSplit">Starting Salt Split</param>
        /// <returns>Returns dictionary</returns>
        private Dictionary<double, double> SaltSplitDegradationCalculator(double numberOfWeeks, double maximumDegradationSaltSplit, double startingSaltSplit)
        {
            try
            {
                Dictionary<double, double> saltSplitDegradation = new Dictionary<double, double>();
                Dictionary<double, double> saltSplitCurve = new Dictionary<double, double>();
                double baseCurveFactor = numberOfWeeks / 312;
                double[] degPoly = new double[5];
                degPoly[0] = 1.93647597707001E-10;
                degPoly[1] = -1.71818433081473E-07;
                degPoly[2] = 0.0000450031960953974;
                degPoly[3] = -0.001102430677463;
                degPoly[4] = 0.009638951553683;
                double degradation = 0;
                for (int week = 1; week <= 312; week++)
                {
                    if (week > 6)
                    {
                        degradation = (Math.Pow(week, 4) * degPoly[0]) + (Math.Pow(week, 3) * degPoly[1]) + (Math.Pow(week, 2) * degPoly[2]) + (week * degPoly[3]) + degPoly[4];
                        saltSplitDegradation.Add(week, degradation);
                    }
                    else
                    {
                        if (week != 1)
                        {
                            degradation += 0.0001;
                        }
                        saltSplitDegradation.Add(week, degradation);
                    }

                }
                foreach (var wklyReductionVal in saltSplitDegradation)
                {
                    double finalVal = saltSplitDegradation.Last().Value;
                    double curveValue = ((wklyReductionVal.Value * maximumDegradationSaltSplit * 0.01) / (finalVal != 0 ? finalVal : 1));
                    double weekNum = wklyReductionVal.Key * baseCurveFactor;
                    saltSplitCurve.Add(weekNum, curveValue);

                }
                Dictionary<double, double> curve = CurveSlopeSmoother(saltSplitCurve, startingSaltSplit);
                return curve;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// curve Slope Smoother
        /// </summary>
        /// <param name="inputCurve">input Curve</param>
        /// <param name="startingSaltSplit">Starting SS</param>
        /// <returns>Returns dictionary</returns>
        private static Dictionary<double, double> CurveSlopeSmoother(Dictionary<double, double> inputCurve, double startingSaltSplit)
        {
            try
            {
                Dictionary<double, double> tempCurve = new Dictionary<double, double>();
                Dictionary<double, double> outputCurve = new Dictionary<double, double>();
                List<double> slope = new List<double>();
                double[] degPoly = new double[5];
                degPoly[0] = 1.93647597707001E-10;
                degPoly[1] = -1.71818433081473E-07;
                degPoly[2] = 0.0000450031960953974;
                degPoly[3] = -0.001102430677463;
                degPoly[4] = 0.009638951553683;
                double x_i = inputCurve.Count - 1;
                for (double x = 0; x <= x_i; x++)
                {
                    double accuracy = 0.0000000001;
                    double h = x + accuracy;
                    double fx = (Math.Pow(x, 4) * degPoly[0]) + (Math.Pow(x, 3) * degPoly[1]) + (Math.Pow(x, 2) * degPoly[2]) + (x * degPoly[3]) + degPoly[4];
                    double fx_plus_h = (Math.Pow((x + h), 4) * degPoly[0]) + (Math.Pow((x + h), 3) * degPoly[1]) + (Math.Pow((x + h), 2) * degPoly[2]) + ((x + h) * degPoly[3]) + degPoly[4];
                    double fx_prime = (fx_plus_h - fx) / h;
                    slope.Add(fx_prime);
                }
                double last_positiveValue = 0;
                for (int j = inputCurve.Count - 1; j >= 0; j--)
                {
                    double m = slope[j];
                    double sign = Math.Sign(Convert.ToDecimal(m));
                    if (sign == -1 || sign == 0)
                    {
                        tempCurve.Add(inputCurve.ElementAt(j).Key, last_positiveValue);
                    }
                    else
                    {
                        last_positiveValue = inputCurve.ElementAt(j).Value;
                        tempCurve.Add(inputCurve.ElementAt(j).Key, inputCurve.ElementAt(j).Value);
                    }
                }
                for (int j = inputCurve.Count - 1; j >= 0; j--)
                {
                    outputCurve.Add(tempCurve.ElementAt(j).Key, tempCurve.ElementAt(j).Value);
                }
                return SaltSplitCurveConverter(startingSaltSplit, outputCurve);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Salt Split Curve Converter
        /// </summary>
        /// <param name="initialSaltSplit">initial SaltSplit</param>
        /// <param name="outputCurve">output Curve</param>
        /// <returns>Retusn dictionary</returns>
        private static Dictionary<double, double> SaltSplitCurveConverter(double initialSaltSplit, Dictionary<double, double> outputCurve)
        {
            try
            {
                convertedSaltSplitPoints.Clear();
                foreach (var point in outputCurve)
                {
                    double yValueConverted = initialSaltSplit - (point.Value * initialSaltSplit);
                    convertedSaltSplitPoints.Add(point.Key, yValueConverted);
                }
                return convertedSaltSplitPoints;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Calculates Minimum SaltSplit
        /// </summary>
        /// <param name="id">Customer identifier</param>
        /// <param name="selectedTrainId">selected Train Id</param>
        /// <returns>Returns list</returns>
        public List<double> CalculateMinSaltSplit(long id, double degredation, string selectedTrainId = "0")
        {
            try
            {
                double currentTrain = 0;
                int waterDemand = this.predictiveRepository.GetWaterDemand(id);
                List<train> trainData = new List<train>();
                List<vessel> vesselData = new List<vessel>();
                List<BedNum_Interval> intervalList = new List<BedNum_Interval>();
                List<double> lbsChemicalList = new List<double>();
                TimeSpan intervalSum = new TimeSpan();
                List<double> replacementPlan = new List<double>();
                List<double> output = new List<double>();
                trainData = this.predictiveRepository.GetCustomerTrains(id);
                vesselData = this.predictiveRepository.GetCustomerVessels(id);
                double numberOfTrains;
                if (selectedTrainId == "0")
                    numberOfTrains = trainData.Count;
                else
                    numberOfTrains = 1;

                //// Check if all vessel data has actually been set
                //int _vesselCount = vesselData.Count();
                //int _anionCount = 0;
                //int _cationCount = 0;

                //foreach(var train in trainData){
                //    if (train.num_beds_anion == 2)
                //        _anionCount += 2;
                //    else
                //        _anionCount++;

                //    if (train.num_beds_cation == 2)
                //        _cationCount += 2;
                //    else
                //        _cationCount++;
                //}

                //int totalNumVessels = _anionCount + _cationCount;

                //int loadAllTrains = 0;
                //if (totalNumVessels == _vesselCount)
                //{
                //    loadAllTrains = 1;
                //}

                double numberCubicFeet = 0, numberRegens = 0;
                List<double> numberRegenPerTrain = new List<double>();
                string usingManifold = string.Empty;
                int traincount = 0;
                foreach (var item in trainData)
                {
                    traincount++;
                    usingManifold = item.using_manifold;
                    if (item.trainID == Convert.ToInt32(selectedTrainId))
                    {
                        numberRegenPerTrain.Add(Convert.ToDouble(item.regens_per_month));
                        currentTrain = traincount;
                    }
                    else if (selectedTrainId == "0")
                    {
                        numberRegenPerTrain.Add(Convert.ToDouble(item.regens_per_month));
                        currentTrain = 0;
                    }
                }
                if (usingManifold == "NO")
                {
                    numberRegens = numberRegenPerTrain.Sum(); // Get the total number of regens for the system
                }
                else
                {
                    foreach (var item in vesselData)
                    {
                        if (selectedTrainId != "0")
                        {
                            numberRegenPerTrain.Add(Convert.ToDouble(item.num_regens));
                        }
                        else
                        {
                            if (item.train_trainID == Convert.ToInt32(selectedTrainId))
                            {
                                numberRegenPerTrain.Add(Convert.ToDouble(item.num_regens));
                            }
                        }

                    }
                    numberRegens = numberRegenPerTrain.Sum(); // Get the total number of regens for the system
                }
                List<double> numCuFt = new List<double>();
                int vesselNumber = 0;
                foreach (var item in vesselData)
                {
                    
                    if (selectedTrainId == "0")
                    {
                        
                        //numberCubicFeet = numberCubicFeet + Convert.ToDouble(item.size);
                        DateTime purchasedate = Convert.ToDateTime(item.date_replaced, new CultureInfo("en-US", true));
                        TimeSpan interval = DateTime.Today - purchasedate;
                        BedNum_Interval bed_interval = new BedNum_Interval();
                        bed_interval.bed_number = item.bed_number;
                        bed_interval.span = interval;
                        intervalList.Add(bed_interval);                        
                       
                        double lbsChemical = Convert.ToDouble(item.lbs_chemical);
                        
                        if (vesselNumber % 2 != 0)
                        {  // Only the anion vessels
                            replacementPlan.Add(Convert.ToInt32(item.replacement_plan));
                            numCuFt.Add(Convert.ToDouble(item.size));
                            lbsChemicalList.Add(lbsChemical);
                        }
                    }
                    else
                    {
                        if (item.train_trainID == Convert.ToInt32(selectedTrainId))
                        {
                            
                            //numberCubicFeet = numberCubicFeet + Convert.ToDouble(item.size);
                            DateTime purchasedate = Convert.ToDateTime(item.date_replaced, new CultureInfo("en-US", true));
                            TimeSpan interval = DateTime.Today - purchasedate;
                            BedNum_Interval bed_interval = new BedNum_Interval();
                            bed_interval.bed_number = item.bed_number;
                            bed_interval.span = interval;
                            intervalList.Add(bed_interval); 
                            
                            double lbsChemical = Convert.ToDouble(item.lbs_chemical);
                            
                            if (vesselNumber % 2 != 0)
                            {  // Only the anion vessels
                                replacementPlan.Add(Convert.ToInt32(item.replacement_plan));
                                numCuFt.Add(Convert.ToDouble(item.size));
                                lbsChemicalList.Add(lbsChemical);
                            }
                        }
                    }
                    numberCubicFeet = numCuFt.Sum(); // Use only the number of CubicFt for the Anion Vessel
                    vesselNumber++;
                }

                int num_anions = 0;
                foreach (var span in intervalList)
                {
                    // Ensure we are summing only the ANION vesel purchase dates!
                    if (span.bed_number == "2")
                    {
                        intervalSum += span.span;
                        num_anions++;
                    }
                }

                double average = 0;
                if (intervalList.Count > 0)
                {
                    average = intervalSum.TotalMilliseconds / num_anions;
                }
                else
                {
                    average = intervalSum.TotalMilliseconds;
                }

                if (Double.IsNaN(average))
                    average = 0;
                
                TimeSpan averageResinAge = TimeSpan.FromMilliseconds(average);
                double age = averageResinAge.TotalDays / 7;
                double grainAverage = this.predictiveRepository.GetGrainsWeightTotal(id.ToString());
                            
                
                double minimumSaltSplit = 10.8;
                output.Add(minimumSaltSplit);
                output.Add(age);
                if (replacementPlan.Count > 0)
                    output.Add(replacementPlan.Average());
                else
                    output.Add(0);
                output.Add(currentTrain);
                return output;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// current Salt Split conditions
        /// </summary>
        /// <param name="resinAge">Resin Age parameter</param>
        /// <param name="cleaningEffectiveness">Cleaning Effectiveness</param>
        /// <param name="startingSaltSplit">starting SS</param>
        /// <returns>Returns dictionary</returns>
        public Dictionary<double, double> CurrentSSConditions(double resinAge, double cleaningEffectiveness, double startingSaltSplit, double resinLifeExpectancy)
        {
            try
            {
                Dictionary<double, double> currentSaltSplitConditions = new Dictionary<double, double>();
                double currentSaltSplit = 0;
                double weekNumber = 0;
                foreach (KeyValuePair<double, double> value in convertedSaltSplitPoints)
                {
                    if (resinAge == value.Key)
                    {
                        currentSaltSplit = value.Value;
                        weekNumber = value.Key;
                        break;
                    }
                    else if (Math.Abs(resinAge - value.Key) < 1.5)
                    {
                        currentSaltSplit = Math.Round(value.Value, 2);
                        weekNumber = value.Key;
                        break;
                    }
                    else
                    {
                        currentSaltSplit = Math.Round(convertedSaltSplitPoints.ElementAt(convertedSaltSplitPoints.Count - 1).Value);
                        weekNumber = Math.Round(convertedSaltSplitPoints.ElementAt(convertedSaltSplitPoints.Count - 1).Key);
                    }
                }


                //double effectiveAfterCleaningResinAge = Math.Floor(weekNumber - (weekNumber*cleaningEffectiveness*0.01));
                //double weeKNumber = 312 * (Convert.ToDouble(effectiveAfterCleaningResinAge) / resinLifeExpectancy);
                //double[] degPoly = new double[5];
                //degPoly[0] = 1.93647597707001E-10;
                //degPoly[1] = -1.71818433081473E-07;
                //degPoly[2] = 0.0000450031960953974;
                //degPoly[3] = -0.001102430677463;
                //degPoly[4] = 0.009638951553683;
                //double degradation = ((Math.Pow(weeKNumber, 4) * degPoly[0]) + (Math.Pow(weeKNumber, 3) * degPoly[1]) + (Math.Pow(weeKNumber, 2) * degPoly[2]) + (weeKNumber * degPoly[3]) + degPoly[4]);
                //double saltSplit = (startingSaltSplit * (1 - degradation));

                //double afterCleaningSaltSplit = saltSplit;
                double afterCleaningSaltSplit = currentSaltSplit + (cleaningEffectiveness * 0.01 * currentSaltSplit);

                // Limit the after cleaning saltsplit to the new resin saltsplit level
                afterCleaningSaltSplit = afterCleaningSaltSplit >= startingSaltSplit ? startingSaltSplit : afterCleaningSaltSplit;

                List<double> afterCleaningWeek = FindAfterCleaningWeak(afterCleaningSaltSplit);
				if (!currentSaltSplitConditions.ContainsKey(weekNumber))
				{
					currentSaltSplitConditions.Add(weekNumber, currentSaltSplit);
				}
				if (afterCleaningWeek != null && afterCleaningWeek.Count > 0 && !currentSaltSplitConditions.ContainsKey(afterCleaningWeek[0]))
                {
                    currentSaltSplitConditions.Add(afterCleaningWeek[0], afterCleaningWeek[1]);
                }
                return currentSaltSplitConditions;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Finds the After Cleaning Week
        /// </summary>
        /// <param name="afterCleaningSaltSplit">After Cleaning Salt Split</param>
        /// <returns>Returns list of double</returns>
        public List<double> FindAfterCleaningWeak(double afterCleaningSaltSplit)
        {
            try
            {
                List<double> afterCleaningweek = new List<double>();
                bool isSaltSplitFound = false;
                if (true)
                {
                    double count = 0;
                    while (isSaltSplitFound == false)
                    {
                        count += 1;
                        foreach (var saltSplit in convertedSaltSplitPoints)
                        {
                            double splitLevel = saltSplit.Value;

                            if (splitLevel == afterCleaningSaltSplit)
                            {
                                afterCleaningweek.Add(saltSplit.Key);
                                afterCleaningweek.Add(saltSplit.Value);
                                isSaltSplitFound = true;
                                break;
                            }
                        }
                        if (isSaltSplitFound == true)
                        {
                            break;
                        }
                        foreach (var saltSplit in convertedSaltSplitPoints)
                        {
                            double splitLevel = Math.Round(saltSplit.Value, 2);
                            if (splitLevel == Math.Round(afterCleaningSaltSplit, 2))
                            {
                                afterCleaningweek.Add(saltSplit.Key);
                                afterCleaningweek.Add(saltSplit.Value);
                                isSaltSplitFound = true;
                                break;
                            }
                        }
                        if (isSaltSplitFound == true)
                        {
                            break;
                        }
                        foreach (var saltSplit in convertedSaltSplitPoints)
                        {
                            double splitLevel = Math.Round(saltSplit.Value, 1);

                            if (splitLevel == Math.Round(afterCleaningSaltSplit, 1))
                            {
                                afterCleaningweek.Add(saltSplit.Key);
                                afterCleaningweek.Add(saltSplit.Value);
                                isSaltSplitFound = true;
                                break;
                            }
                        }

                        if (isSaltSplitFound == true)
                        {
                            break;
                        }
                        foreach (var saltSplit in convertedSaltSplitPoints)
                        {
                            double splitLevel = Math.Round(saltSplit.Value, 1);
                            if (Math.Abs(afterCleaningSaltSplit - saltSplit.Value) < .05)
                            {
                                afterCleaningweek.Add(saltSplit.Key);
                                afterCleaningweek.Add(saltSplit.Value);
                                isSaltSplitFound = true;
                                break;
                            }
                        }

                        if (isSaltSplitFound == true)
                        {
                            break;
                        }
                        foreach (var saltSplit in convertedSaltSplitPoints)
                        {
                            double splitLevel = Math.Round(saltSplit.Value, 1);
                            if (Math.Abs(afterCleaningSaltSplit - saltSplit.Value) < .5)
                            {
                                afterCleaningweek.Add(saltSplit.Key);
                                afterCleaningweek.Add(saltSplit.Value);
                                isSaltSplitFound = true;
                                break;
                            }
                        }
                        if (isSaltSplitFound == true)
                        {
                            break;
                        }

                        foreach (var saltSplit in convertedSaltSplitPoints)
                        {
                            double splitLevel = Math.Round(saltSplit.Value, 1);
                            if (Math.Abs(afterCleaningSaltSplit - saltSplit.Value) < 1)
                            {
                                afterCleaningweek.Add(saltSplit.Key);
                                afterCleaningweek.Add(saltSplit.Value);
                                isSaltSplitFound = true;
                                break;
                            }
                        }
                        if (isSaltSplitFound == true)
                        {
                            break;
                        }
                        if (count > 5 * convertedSaltSplitPoints.Count)
                        {
                            break;
                        }
                    }
                }
                return afterCleaningweek;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Thoughput chart
        /// </summary>
        /// <param name="customerId">CustomerId parameter</param>
        /// <param name="Cur_SS">Curent Salt Split</param>
        /// <param name="startingSaltSplit">starting Salt Split</param>
        /// <param name="resinLifeExpectancy">resin Life Expectancy</param>
        /// <param name="simulationconfidence">simulation confidence</param>
        /// <param name="numberSimulationIterations">num of simulation iterations</param>
        /// <param name="simMethod">simulation Method</param>
        /// <param name="standardDeviationThreshold">stdDev threshold</param>
        /// <param name="resinAge">resin Age parameter</param>
        /// <param name="RTIcleaning_effectivness">RTI cleaning effectivness</param>
        /// <param name="replacementLevel">Replacement Level</param>
        /// <param name="rticleaningLevel">RTIcleaning Level</param>
        /// <param name="ReGen_effectivness">ReGeneration effectivness</param>
        /// <param name="selectedTrain">Selected Train</param>
        /// <param name="donotReplaceResin">Dont Replace Resin</param>
        /// <returns>Returns the price data</returns>
        public PriceData Thoughputchart(string customerId, double currentSaltSplit, double startingSaltSplit, double resinLifeExpectancy, int simulationconfidence, int numberSimulationIterations, string simMethod, int standardDeviationThreshold, double resinAge, double replacementLevel, double rticleaningLevel, string selectedTrain, bool donotReplaceResin)
        {
            try
            {
                double[] sourceId = this.predictiveRepository.FetchSourceIdTP(customerId);
                List<water_data> conductivitSource1 = new List<water_data>();
                PriceData priceData = new PriceData();
                if (sourceId != null && sourceId.Length > 0)
                {
                    double sourceIdOne = sourceId[0];
                    conductivitSource1 = (from waterData in this.modifiedwaterdataRepository.GetAll() where waterData.sourceID == sourceIdOne select waterData).ToList();
                }
                if (conductivitSource1.Count > 0)
                {
                    List<water_data> conductivitSource2 = new List<water_data>();
                    if (sourceId != null && sourceId.Length > 1 && sourceId[1] == 0)
                    {
                        conductivitSource2 = conductivitSource1;
                    }
                    else if (sourceId != null && sourceId.Length > 1)
                    {
                        double sourceIdTwo = sourceId[1];
                        conductivitSource2 = (from waterData in this.modifiedwaterdataRepository.GetAll() where waterData.sourceID == sourceIdTwo select waterData).ToList();
                    }
                    List<string>[] conductivitySourceArray1 = new List<string>[5];
                    List<string>[] conductivitySourceArray2 = new List<string>[5];
 
                    conductivitySourceArray1[0] = (from cond in conductivitSource1 
                                                   select Convert.ToString(cond.dataID)).ToList();
                    conductivitySourceArray1[1] = (from cond in conductivitSource1 
                                                   select Convert.ToString(cond.cond)).ToList();
                    conductivitySourceArray1[2] = (from cond in conductivitSource1 
                                                   select Convert.ToString(cond.temp)).ToList();
                    conductivitySourceArray1[3] = (from cond in conductivitSource1 
                                                   select Convert.ToString(cond.measurment_date)).ToList();
                    conductivitySourceArray1[4] = (from cond in conductivitSource1 
                                                   select Convert.ToString(cond.sourceID)).ToList();

                    conductivitySourceArray2[0] = (from cond in conductivitSource2 
                                                   select Convert.ToString(cond.dataID)).ToList();
                    conductivitySourceArray2[1] = (from cond in conductivitSource2 
                                                   select Convert.ToString(cond.cond)).ToList();
                    conductivitySourceArray2[2] = (from cond in conductivitSource2 
                                                   select Convert.ToString(cond.temp)).ToList();
                    conductivitySourceArray2[3] = (from cond in conductivitSource2
                                                   select Convert.ToString(cond.measurment_date)).ToList();
                    conductivitySourceArray2[4] = (from cond in conductivitSource2 
                                                   select Convert.ToString(cond.sourceID)).ToList();
                    double sourceOnePercent = predictiveRepository.GetFirstSourcePercentage(customerId);
                    ThroughputChart tpchart = new ThroughputChart();
                    priceData = tpchart.LoadModel(customerId, currentSaltSplit, this.modifiedTrainRepository, this.predictiveRepository, conductivitySourceArray1, conductivitySourceArray2, sourceOnePercent, startingSaltSplit, resinLifeExpectancy, simulationconfidence, numberSimulationIterations, simMethod, standardDeviationThreshold, resinAge, replacementLevel, rticleaningLevel, selectedTrain, donotReplaceResin);
                    return priceData;
                }
                return priceData;
            }
            catch
            {
                throw;
            }
        }
        #endregion Methods
    }

    // Holds bed number and purchase date time span
    public class BedNum_Interval{
        public string bed_number { get; set; }
        public TimeSpan span { get; set; }
    }

    // Used for testing is all train data has actually been set
    public class Train_BedCount
    {
        public int trainNumber { get; set; }
        public bool dualAnions { get; set; }
        public bool dualCations { get; set; }
    }
}
