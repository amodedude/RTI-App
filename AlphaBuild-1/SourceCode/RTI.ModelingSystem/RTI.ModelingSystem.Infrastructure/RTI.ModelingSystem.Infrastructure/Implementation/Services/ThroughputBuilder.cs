// -----------------------------------------------------------------------
// <copyright file="ThroughputBuilder.cs" company="RTI">
// RTI
// </copyright>
// <summary>Throughput Builder</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RTI.ModelingSystem.Core.DBModels;

    #endregion Usings

    public class ThroughputBuilder
    {
        /// <summary>
        /// Before System Conditions
        /// </summary>
        public List<Tuple<double, double, double, double, double, double>> BeforeSystemConditions = new List<Tuple<double, double, double, double, double, double>>();

        /// <summary>
        /// After System Conditions
        /// </summary>
        public List<Tuple<double, double, double, double, double, double>> AfterSystemConditions = new List<Tuple<double, double, double, double, double, double>>();

        /// <summary>
        /// Builds the Throughput
        /// </summary>
        /// <param name="replacementLevel">replacement Level</param>
        /// <param name="rticleaningLevel">rti cleaning Level</param>
        /// <param name="startingSaltSplit">starting Salt Split</param>
        /// <param name="randomGrains">random Grains</param>
        /// <param name="amountOfResin">amount Of Resin</param>
        /// <param name="cleaningEffectivness">cleaning Effectivness</param>
        /// <param name="trainGPM">train GPM</param>
        /// <param name="ageOfResin">age Of Resin</param>
        /// <param name="newResinSaltSplit">new Resin Salt Split</param>
        /// <param name="lifeExpectancy">life Expectancy</param>
        /// <param name="isCleaning">is Cleaning</param>
        /// <param name="isReplacing">is Replacing</param>
        /// <returns>Returns the throughput data</returns>
        public Dictionary<DateTime, Tuple<int, double, string>> ThroughputBuild(double replacementLevel, double rticleaningLevel, double startingSaltSplit, Dictionary<int, double> randomGrains, double amountOfResin, double cleaningEffectivness, double trainGPM, double ageOfResin, double newResinSaltSplit, double lifeExpectancy, bool isCleaning, bool isReplacing, double regenDuration, int selectedTrainId, int numberOfTrains)
        {
            try
            {
                replacementLevel = 10.00;
                rticleaningLevel = 17.00;
                Dictionary<DateTime, Tuple<int, double, string>> tpPrediction = new Dictionary<DateTime, Tuple<int, double, string>>();
                TOCBuilder toc = new TOCBuilder();
                PreviousWeek lastWeeks = new PreviousWeek();
                DateTime date = new DateTime();
                date = DateTime.Now.Date;
                double effectiveResinAge = ageOfResin;
                double origionalCleaningLevel = replacementLevel;
                int simulationDuration = 104;
                int weekNumber = 1;
                int lastCleaningWeek = 0; // Keep track of the last week a cleaning was performed
                double cleaningInterval = 52; // Spacing between cleanings
                bool isAllTrains = selectedTrainId == 0 ? true : false;

                foreach (var week in randomGrains)
                {
                    if (weekNumber <= simulationDuration) // Ensure simulation does not exceed 130 weeks (2.5 years) 
                    {
                        // Calculte data for the very first week in the simulation
                        if (week.Key == randomGrains.FirstOrDefault().Key)
                        {
                            Tuple<int, double, string> weekTpPair = CalculateFirstWeek(toc, isCleaning, startingSaltSplit, amountOfResin, week, trainGPM, ageOfResin, lastWeeks, regenDuration, isAllTrains, numberOfTrains);
                            tpPrediction.Add(date, weekTpPair);
                            lastWeeks.Date = date;
                        }
                        else if (isCleaning)
                        {
                            date = lastWeeks.Date.AddDays(7);
                            effectiveResinAge += 1;

                            // NORMAL OPS
                            if (lastWeeks.SaltSplit > rticleaningLevel && (Convert.ToInt32(week.Key) - lastCleaningWeek <= cleaningInterval))
                            {
                                Tuple<int, double, string> weekTpPair = NormalOpsWeek(isCleaning, trainGPM, lifeExpectancy, toc, effectiveResinAge, amountOfResin, week, randomGrains, lastWeeks, newResinSaltSplit, regenDuration, isAllTrains, numberOfTrains);
                                tpPrediction.Add(date, weekTpPair);
                                lastWeeks.Date = date;
                            }

                            // CLEANING WEEK
                            else if ((lastWeeks.SaltSplit > replacementLevel) && (lastWeeks.SaltSplit < rticleaningLevel) || lastCleaningWeek == 0)
                            {
                                effectiveResinAge = effectiveResinAge - (effectiveResinAge * cleaningEffectivness * 0.01);
                                Tuple<int, double, string> weekTpPair = CleanOpsWeek(toc, effectiveResinAge, amountOfResin, week, cleaningEffectivness, newResinSaltSplit, lastWeeks, trainGPM, regenDuration, lifeExpectancy, isAllTrains, numberOfTrains);
                                //rticleaningLevel = PostCleaningSaltSPlitReduction(rticleaningLevel, replacementLevel, cleaningEffectivness, lastWeeks);
                                tpPrediction.Add(date, weekTpPair);
                                lastWeeks.Date = date;
                                if (lastCleaningWeek != 0)
                                    cleaningInterval = cleaningInterval - cleaningInterval * (rticleaningLevel / 100);
                                lastCleaningWeek = week.Key;
                            }

                            //if (week.Key > 169)
                            //{
                            //    // break
                            //}

                            //// REPLACEMENT WEEK
                            //else
                            //{
                            //    if (isReplacing)
                            //    {
                            //        effectiveResinAge = 1;
                            //        Tuple<int, double, string> weekTpPair = ReplaceOpsWeek(isCleaning, trainGPM, lifeExpectancy, toc, effectiveResinAge, newResinSaltSplit, amountOfResin, week, lastWeeks, hoursPerRun);
                            //        rticleaningLevel = origionalCleaningLevel;   // Restore the cleaning level upon resin replacement
                            //        tpPrediction.Add(date, weekTpPair);
                            //        lastWeeks.Date = date;
                            //    }
                            //    else
                            //    {
                            //        Tuple<int, double, string> weekTpPair = NormalOpsWeek(isCleaning, trainGPM, lifeExpectancy, toc, effectiveResinAge, amountOfResin, week, randomGrains, lastWeeks, newResinSaltSplit, hoursPerRun);
                            //        tpPrediction.Add(date, weekTpPair);
                            //        lastWeeks.Date = date;
                            //    }
                            //}
                        }
                        else  // If not the first week and you are not performing cleanings, calculate data
                        {
                            date = lastWeeks.Date.AddDays(7);
                            effectiveResinAge += 1;

                            // NORMAL OPS
                            if (lastWeeks.SaltSplit > replacementLevel || isReplacing)
                            {
                                Tuple<int, double, string> weekTpPair = NormalOpsWeek(isCleaning, trainGPM, lifeExpectancy, toc, effectiveResinAge, amountOfResin, week, randomGrains, lastWeeks, newResinSaltSplit, regenDuration, isAllTrains, numberOfTrains);
                                tpPrediction.Add(date, weekTpPair);
                                lastWeeks.Date = date;
                            }

                            // REPLACEMENT WEEK
                            else
                            {
                                effectiveResinAge = 1;
                                Tuple<int, double, string> weekTpPair = ReplaceOpsWeek(isCleaning, trainGPM, lifeExpectancy, toc, effectiveResinAge, newResinSaltSplit, amountOfResin, week, lastWeeks, regenDuration, isAllTrains, numberOfTrains);
                                tpPrediction.Add(date, weekTpPair);
                                lastWeeks.Date = date;
                            }
                        }
                    }
                    weekNumber++;
                }
                return tpPrediction;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Calculates the First Week
        /// </summary>
        /// <param name="toc">toc parameter</param>
        /// <param name="isCleaning">is Cleaning</param>
        /// <param name="startingSaltSplit">starting SaltSplit</param>
        /// <param name="amountOfResin">amount Of Resin</param>
        /// <param name="grains">grains parameter</param>
        /// <param name="trainGPM">train GPM parameter</param>
        /// <param name="ageOfResin">age Of Resin</param>
        /// <param name="lastWeeks">last Weeks parameter</param>
        /// <returns></returns>
        private Tuple<int, double, string> CalculateFirstWeek(TOCBuilder toc, bool isCleaning, double startingSaltSplit, double amountOfResin, KeyValuePair<int, double> grains, double trainGPM, double ageOfResin, PreviousWeek lastWeeks, double regenTime, bool isAllTrains, int numberTrains)
        {
            try
            {
                // Determines the number of trains being analyzed
                int numberOfTrains = isAllTrains ? numberTrains : 1;

                // Get the most recent salt split level
                lastWeeks.SaltSplit = startingSaltSplit;

                // Calculate the systems grain capacity in one run
                double grainCapacityPerRun = lastWeeks.SaltSplit * 1000 * (amountOfResin / numberOfTrains);

                // Calculate the throughput based on this weeks salt split
                double throughput = grainCapacityPerRun / grains.Value;

                // Calculate the system run time  (hours per run)
                double hoursRunTime = throughput / (trainGPM * 60);

                // Add in the regeneration time in hours
                double hoursTotalTime = hoursRunTime + regenTime;

                // Calculate the number of runs per week
                double numberOfHoursInWeek = 7 * 24;
                double runsPerWeek = numberOfHoursInWeek / hoursTotalTime;

                // Set data
                lastWeeks.HoursPerRun = hoursRunTime;
                lastWeeks.RegPerWeek = runsPerWeek;

                double silicaFactor = RandomNumberGenerator.Between(97, 100);
                Tuple<int, double, string> weekTpPair = new Tuple<int, double, string>(grains.Key, throughput, "Normal");

                //if (throughput == 0)
              //  {
               //     lastWeeks.RegPerWeek = 0;
               //     lastWeeks.HoursPerRun = 0.0001 * (silicaFactor * 0.01);
               // }

                Tuple<double, double, double, double, double, double> regenData = new Tuple<double, double, double, double, double, double>(grains.Key, lastWeeks.RegPerWeek, lastWeeks.HoursPerRun, regenTime, throughput, lastWeeks.SaltSplit);
                if (isCleaning)
                {
                    this.BeforeSystemConditions.Add(regenData);
                    this.AfterSystemConditions.Add(regenData);
                }
                else
                {
                    this.BeforeSystemConditions.Add(regenData);
                }
                return weekTpPair;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Normal Ops Week
        /// </summary>
        /// <param name="isCleaning">is Cleaning</param>
        /// <param name="trainGPM">train GPM parameter</param>
        /// <param name="lifeExpectancy">life Expectancy</param>
        /// <param name="toc">toc parameter</param>
        /// <param name="effectiveResinAge">effective Resin Age</param>
        /// <param name="amountOfResin">amount Of Resin</param>
        /// <param name="grains">grains parameter</param>
        /// <param name="randomGrains">random Grains</param>
        /// <param name="lastWeeks">last Weeks</param>
        /// <param name="newResinSS">new Resin SS</param>
        /// <returns>Returns the normal ops week</returns>
        private Tuple<int, double, string> NormalOpsWeek(bool isCleaning, double trainGPM, double lifeExpectancy, TOCBuilder toc, double effectiveResinAge, double amountOfResin, KeyValuePair<int, double> grains, Dictionary<int, double> randomGrains, PreviousWeek lastWeeks, double newResinSS, double regenTime, bool isAllTrains, int numberTrains)
        {
            try
            {
                // Get the current weeks salt split level
                double saltSplit = fetchSaltSplitLevel(effectiveResinAge,lifeExpectancy,newResinSS);
                lastWeeks.SaltSplit = saltSplit;

                // Determines the number of trains being analyzed
                int numberOfTrains = isAllTrains ? numberTrains : 1;

                // Get the most recent salt split level
                lastWeeks.SaltSplit = saltSplit;

                // Calculate the systems grain capacity in one run
                double grainCapacityPerRun = lastWeeks.SaltSplit * 1000 * (amountOfResin / numberOfTrains);

                // Calculate the throughput based on this weeks salt split
                double throughput = grainCapacityPerRun / grains.Value;

                // Calculate the system run time  (hours per run)
                double hoursRunTime = throughput / (trainGPM * 60);

                // Add in the regeneration time in hours
                double hoursTotalTime = hoursRunTime + regenTime;

                // Calculate the number of runs per week
                double numberOfHoursInWeek = 7 * 24;
                double runsPerWeek = numberOfHoursInWeek / hoursTotalTime;

                // Set data
                lastWeeks.RegPerWeek = runsPerWeek;
                lastWeeks.HoursPerRun = hoursRunTime;

               // if (throughput <= 0 && lastWeeks != null && lastWeeks.TpData != null)
               // {
               //     throughput = lastWeeks.TpData.Item2;
               // }

                Tuple<int, double, string> weekTpPair = new Tuple<int, double, string>(grains.Key, throughput, "Normal");
                lastWeeks.TpData = weekTpPair;

                //double silicaFactor = RandomNumberGenerator.Between(97, 100);
                //if (throughput == 0)
                //{
                //    lastWeeks.HoursPerRun = 0.0001 * (silicaFactor * 0.01);
                //    lastWeeks.RegPerWeek = 0;
                //}

                Tuple<double, double, double, double, double, double> regenData = new Tuple<double, double, double, double, double, double>(grains.Key, lastWeeks.RegPerWeek, lastWeeks.HoursPerRun, regenTime, throughput, lastWeeks.SaltSplit);
                if (isCleaning)
                {
                    this.AfterSystemConditions.Add(regenData);
                }
                else
                {
                    this.BeforeSystemConditions.Add(regenData);
                }
                return weekTpPair;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Clean Ops Week
        /// </summary>
        /// <param name="toc">toc parameter</param>
        /// <param name="effectiveResinAge">effective Resin Age</param>
        /// <param name="amountOfResin">amount Of Resin</param>
        /// <param name="grains">grains parameter</param>
        /// <param name="cleaningEffectivness"></param>
        /// <param name="newResinSaltSplit">new Resin Salt Split</param>
        /// <param name="lastWeeks">last Weeks parameter</param>
        /// <returns>Returns the Clean ops week</returns>
        private Tuple<int, double, string> CleanOpsWeek(TOCBuilder toc, double effectiveResinAge, double amountOfResin, KeyValuePair<int, double> grains, double cleaningEffectivness, double newResinSaltSplit, PreviousWeek lastWeeks, double trainGPM, double regenTime, double lifeExpectancy, bool isAllTrains, int numberTrains)
        {
            try
            {
                
                double saltSplit = fetchSaltSplitLevel(effectiveResinAge, lifeExpectancy, newResinSaltSplit);

                
                //double saltSplit = lastWeeks.SaltSplit + (lastWeeks.SaltSplit * cleaningEffectivness * 0.01);
                //if (saltSplit > newResinSaltSplit)
                //{
                //    saltSplit = newResinSaltSplit;
                //}
                //double percentIncrease = (saltSplit - lastWeeks.SaltSplit) / lastWeeks.SaltSplit;
                //effectiveResinAge = effectiveResinAge - (effectiveResinAge * percentIncrease);
                //double regenerationTime = toc.RegenTimeCurve(effectiveResinAge) / 60;
                

                // Determines the number of trains being analyzed
                int numberOfTrains = isAllTrains ? numberTrains : 1;

                // Get the most recent salt split level
                lastWeeks.SaltSplit = saltSplit;

                // Calculate the systems grain capacity in one run
                double grainCapacityPerRun = lastWeeks.SaltSplit * 1000 * (amountOfResin / numberOfTrains);

                // Calculate the throughput based on this weeks salt split
                double throughput = grainCapacityPerRun / grains.Value;

                // Calculate the system run time  (hours per run)
                double hoursRunTime = throughput / (trainGPM * 60);

                // Add in the regeneration time in hours
                double hoursTotalTime = hoursRunTime + regenTime;

                // Calculate the number of runs per week
                double numberOfHoursInWeek = 7 * 24;
                double runsPerWeek = numberOfHoursInWeek / hoursTotalTime;

                // Set data
                lastWeeks.HoursPerRun = hoursRunTime;
                lastWeeks.RegPerWeek = runsPerWeek;


                Tuple<int, double, string> weekTpPair = new Tuple<int, double, string>(grains.Key, throughput, "Clean");
                lastWeeks.TpData = weekTpPair;
                Tuple<double, double, double, double, double, double> regenData = new Tuple<double, double, double, double, double, double>(grains.Key, lastWeeks.RegPerWeek, lastWeeks.HoursPerRun, regenTime, throughput, lastWeeks.SaltSplit);
                this.AfterSystemConditions.Add(regenData);
                return weekTpPair;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Replace Ops Week
        /// </summary>
        /// <param name="isCleaning">is Cleaning parameter</param>
        /// <param name="trainGpm">train GPM parameter</param>
        /// <param name="lifeExpectancy">life Expectancy</param>
        /// <param name="toc">toc parameter</param>
        /// <param name="effectiveResinAge">effective Resin Age</param>
        /// <param name="newResinSaltSplit">new Resin Salt Split</param>
        /// <param name="amountOfResin">amount Of Resin</param>
        /// <param name="grains">grains parameter</param>
        /// <param name="lastWeeks">last Weeks</param>
        /// <returns></returns>
        private Tuple<int, double, string> ReplaceOpsWeek(bool isCleaning, double trainGPM, double lifeExpectancy, TOCBuilder toc, double effectiveResinAge, double newResinSaltSplit, double amountOfResin, KeyValuePair<int, double> grains, PreviousWeek lastWeeks, double regenTime, bool isAllTrains, int numberTrains)
        {
            try
            {
                //lifeExpectancy = 0;
                //double regenerationTime = toc.RegenTimeCurve(lifeExpectancy) / 60;
                //double saltSplit = newResinSaltSplit;
                //lastWeeks.SaltSplit = saltSplit;

                // Determines the number of trains being analyzed
                int numberOfTrains = isAllTrains ? numberTrains : 1;

                // Get the most recent salt split level
                lastWeeks.SaltSplit = newResinSaltSplit;

                // Calculate the systems grain capacity in one run
                double grainCapacityPerRun = lastWeeks.SaltSplit * 1000 * (amountOfResin / numberOfTrains);

                // Calculate the throughput based on this weeks salt split
                double throughput = grainCapacityPerRun / grains.Value;

                // Calculate the system run time  (hours per run)
                double hoursRunTime = throughput / (trainGPM * 60);

                // Add in the regeneration time in hours
                double hoursTotalTime = hoursRunTime + regenTime;

                // Calculate the number of runs per week
                double numberOfHoursInWeek = 7 * 24;
                double runsPerWeek = numberOfHoursInWeek / hoursTotalTime;

                // Set data
                lastWeeks.HoursPerRun = hoursRunTime;
                lastWeeks.RegPerWeek = runsPerWeek;

                
                Tuple<int, double, string> weekTpPair = new Tuple<int, double, string>(grains.Key, throughput, "Replace");
                lastWeeks.TpData = weekTpPair;
                double silicaFactor = RandomNumberGenerator.Between(97, 100);

                // Calculate the number of regenerations in this week
                if (throughput == 0)
                {
                    lastWeeks.RegPerWeek = 0;
                    lastWeeks.HoursPerRun = 0.0001 * (silicaFactor * 0.01);
                }

                Tuple<double, double, double, double, double, double> regenData = new Tuple<double, double, double, double, double, double>(grains.Key, lastWeeks.RegPerWeek, lastWeeks.HoursPerRun, regenTime, throughput, lastWeeks.SaltSplit);
                if (isCleaning)
                {
                    this.AfterSystemConditions.Add(regenData);
                }
                else
                {
                    this.BeforeSystemConditions.Add(regenData);
                }
                return weekTpPair;
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Fetch Salt Split Level
        /// </summary>
        /// <param name="effectiveResinAge">efective resin age in weeks</param>
        /// <param name="lifeExpectancy">resin life expectancy in weeks</param>
        /// /// <param name="newResinSS">new resin salt split level</param>
        /// <returns>Returns the salt split level for any week</returns>
        private double fetchSaltSplitLevel(double effectiveResinAge, double lifeExpectancy, double newResinSS)
        {
            double weeKNumber = 312 * (Convert.ToDouble(effectiveResinAge) / lifeExpectancy);
            double[] degPoly = new double[5];
            degPoly[0] = 1.93647597707001E-10;
            degPoly[1] = -1.71818433081473E-07;
            degPoly[2] = 0.0000450031960953974;
            degPoly[3] = -0.001102430677463;
            degPoly[4] = 0.009638951553683;
            double degradation = ((Math.Pow(weeKNumber, 4) * degPoly[0]) + (Math.Pow(weeKNumber, 3) * degPoly[1]) + (Math.Pow(weeKNumber, 2) * degPoly[2]) + (weeKNumber * degPoly[3]) + degPoly[4]);
            double saltSplit = (newResinSS * (1 - degradation));
            return saltSplit;
        }

        /// <summary>
        /// Post Cleaning SaltSPlit Reduction
        /// </summary>
        /// <param name="cleaningSaltSplit">cleaning SaltSplit</param>
        /// <param name="replaceSaltSplit">replace SaltSplit</param>
        /// <returns>Returns the salt split reduction</returns>
        private double PostCleaningSaltSPlitReduction(double cleaningSaltSplit, double replaceSaltSplit, double cleaningEffectiveness, PreviousWeek lastWeeks)
        {
            try
            {
                double afterReductionSS = cleaningSaltSplit;

                if(lastWeeks.SaltSplit < afterReductionSS){
                    afterReductionSS = lastWeeks.SaltSplit - (lastWeeks.SaltSplit * (cleaningEffectiveness * 0.01));
                }
                return afterReductionSS;
            }
            catch
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Previous Week
    /// </summary>
    public class PreviousWeek
    {
        public Tuple<int, double, string> TpData { get; set; }
        public double SaltSplit { get; set; }
        public double HoursPerRun { get; set; }
        public double RegPerWeek { get; set; }
        public DateTime Date { get; set; }
    }
}
