// -----------------------------------------------------------------------
// <copyright file="PredictiveModelRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>Predictive Model Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Repository
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Models;
    using RTI.ModelingSystem.Infrastructure.Data;
    using System.Globalization;
    using RTI.ModelingSystem.Core.DBModels;

    #endregion Usings

    /// <summary>
    /// PredictiveModelRepository class
    /// </summary>
    public class PredictiveModelRepository : Repository<source>, IPredictiveModelRepository
    {
        #region Properties

        /// <summary>
        /// data For WkToAverage
        /// </summary>
        private Dictionary<DateTime, int> dataForWeekToAverage = new Dictionary<DateTime, int>();

        /// <summary>
        /// wkly Avg CondDataSource1
        /// </summary>
        public Dictionary<DateTime, Tuple<int, int, List<int>>> weeklyAverageCondiuctivityDataSource1 = new Dictionary<DateTime, Tuple<int, int, List<int>>>();

        /// <summary>
        /// wkly Avg CondDataSource2
        /// </summary>
        public Dictionary<DateTime, Tuple<int, int, List<int>>> weeklyAverageConductivityDataSource2 = new Dictionary<DateTime, Tuple<int, int, List<int>>>();

        /// <summary>
        /// wkly StDev CondDataSource1
        /// </summary>
        public Dictionary<DateTime, Tuple<int, int, List<int>>> weeklyStandardDeviationConductivityDataSource1 = new Dictionary<DateTime, Tuple<int, int, List<int>>>();

        /// <summary>
        /// wkly StDev CondDataSource2
        /// </summary>
        public Dictionary<DateTime, Tuple<int, int, List<int>>> weeklyStandardDeviationConductivityDataSource2 = new Dictionary<DateTime, Tuple<int, int, List<int>>>();

        /// <summary>
        /// Db Context
        /// </summary>
        private RtiContext dbContext;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// PredictiveModelRepository constructor
        /// </summary>
        /// <param name="datacontext">datacontext parameter</param>
        public PredictiveModelRepository(RtiContext datacontext)
            : base(datacontext)
        {
            this.dbContext = datacontext;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Gets the Water Source data
        /// </summary>
        /// <param name="usgsId">USGSID parameter</param>
        /// <returns>Returns the list of water data</returns>
        public List<water_data> GetWaterSourcedata(long usgsId)
        {
            try
            {
                var waterdata = rtiContext.water_data != null ? (from r in rtiContext.water_data
                                                                 where r.sourceID == usgsId
                                                                 select r).ToList()
                                                             : new List<water_data>();
                return waterdata;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the Water Statistics
        /// </summary>
        /// <param name="customerId">customerId parameter</param>
        /// <returns>Returns the water statastics</returns>
        public WaterStatisticsViewModel GetWaterStatistics(long customerId)
        {
            try
            {
                var custWater = rtiContext.customer_water != null ? (from r in rtiContext.customer_water
                                                                     where r.customer_customerID.ToString() == customerId.ToString()
                                                                     select r).FirstOrDefault()
                                                           : new customer_water();

                if (custWater.first_sourceID != 0)
                {
                    var Source1 = (from r in rtiContext.sources
                                   where r.sources_sourceID == custWater.first_sourceID
                                   select r).FirstOrDefault();

                    GetAverageDataByWeek(Source1.agency_id, 1);
                    StdDevDataByWeek(Source1.agency_id, 1);
                }
                if (custWater.second_sourceID != 0)
                {
                    var Source2 = (from r in rtiContext.sources
                                   where r.sources_sourceID == custWater.second_sourceID
                                   select r).FirstOrDefault();

                    GetAverageDataByWeek(Source2.agency_id, 2);
                    StdDevDataByWeek(Source2.agency_id, 2);
                }
                List<int> sourceOneWkAvgList = new List<int>();
                List<int> sourceTwoWkAvgList = new List<int>();
                List<int> sourceOneWkStdDevList = new List<int>();
                List<int> sourceTwoWkStdDevList = new List<int>();
                double totaltWeightedAverage = 0;
                double totalWeightedStdDev = 0;
                double sourcePercentOne;
                double sourcePercentTwo;
                double sourceOneSliderPercent = custWater.firstSourcePercentage;
                double sourceTwoSliderPercent = 100 - custWater.firstSourcePercentage;
                double averageSource1 = 0;
                double averageSource2 = 0;
                double stdDevSource1 = 0;
                double stdDevSource2 = 0;
                bool hasTwoSources = false;
                sourcePercentOne = sourceOneSliderPercent * 0.01;
                sourcePercentTwo = sourceTwoSliderPercent * 0.01;

                if (custWater.first_sourceID != 0 && custWater.second_sourceID != 0)
                {
                    foreach (var avgCond in weeklyAverageCondiuctivityDataSource1)
                    {
                        sourceOneWkAvgList.Add(avgCond.Value.Item2);
                    }

                    // Weighted Average Souce Two
                    foreach (var avgCond in weeklyAverageConductivityDataSource2)
                    {
                        sourceTwoWkAvgList.Add(avgCond.Value.Item2);
                    }

                    if (sourceOneWkAvgList != null && sourceOneWkAvgList.Count != 0)
                    {
                        averageSource1 = sourceOneWkAvgList.Average();
                    }
                    if (sourceTwoWkAvgList != null && sourceTwoWkAvgList.Count != 0)
                    {
                        averageSource2 = sourceTwoWkAvgList.Average();
                    }

                    totaltWeightedAverage = (averageSource1 * sourcePercentOne) + (averageSource2 * sourcePercentTwo);

                    foreach (var stdDevCond in weeklyStandardDeviationConductivityDataSource1)
                    {
                        sourceOneWkStdDevList.Add(stdDevCond.Value.Item2);
                    }

                    // Weighted StdDev Souce Two
                    foreach (var stdDevCond in weeklyStandardDeviationConductivityDataSource2)
                    {
                        sourceTwoWkStdDevList.Add(stdDevCond.Value.Item2);
                    }

                    if (sourceOneWkStdDevList != null && sourceOneWkStdDevList.Count != 0)
                    {
                        stdDevSource1 = sourceOneWkStdDevList.Average();
                    }
                    if (sourceTwoWkStdDevList != null && sourceTwoWkStdDevList.Count != 0)
                    {
                        stdDevSource2 = sourceTwoWkStdDevList.Average();
                    }

                    totalWeightedStdDev = (stdDevSource1 * sourcePercentOne) + (stdDevSource2 * sourcePercentTwo);

                    hasTwoSources = true;
                }
                else if (custWater.first_sourceID != 0)
                {
                    foreach (var avgCond in weeklyAverageCondiuctivityDataSource1)
                    {
                        sourceOneWkAvgList.Add(avgCond.Value.Item2);
                    }

                    if (sourceOneWkAvgList != null && sourceOneWkAvgList.Count != 0)
                    {
                        averageSource1 = sourceOneWkAvgList.Average();
                        totaltWeightedAverage = averageSource1 * sourcePercentOne;
                    }

                    // Weighted StdDev Source One
                    foreach (var stdDevCond in weeklyStandardDeviationConductivityDataSource1)
                    {
                        sourceOneWkStdDevList.Add(stdDevCond.Value.Item2);
                    }

                    if (sourceOneWkStdDevList != null && sourceOneWkStdDevList.Count != 0)
                    {
                        stdDevSource1 = sourceOneWkStdDevList.Average();
                        totalWeightedStdDev = stdDevSource1 * sourcePercentOne;
                    }
                }

                // Average Conductivity in Microseconds
                double wtAvgSourceOneDiaplay = Math.Round(averageSource1, 2, MidpointRounding.AwayFromZero);
                double wtAvgSourceTwoDiaplay = Math.Round(averageSource2, 2, MidpointRounding.AwayFromZero);
                double wtAvgTotalDisplay = Math.Round(totaltWeightedAverage, 2, MidpointRounding.AwayFromZero);

                // Average Conductivity in Grains
                double grainsS1 = Math.Round(averageSource1 * (1 / (2 * 17.1)), 2, MidpointRounding.AwayFromZero);
                double grainsS2 = Math.Round(averageSource2 * (1 / (2 * 17.1)), 2, MidpointRounding.AwayFromZero);
                double grainsWtTotal = Math.Round(totaltWeightedAverage * (1 / (2 * 17.1)), 2, MidpointRounding.AwayFromZero);

                // Total average standard deviation
                double wtStdDevSourceOneDisplay = Math.Round(stdDevSource1, 2, MidpointRounding.AwayFromZero);
                double wtStdDevSourceTwoDisplay = Math.Round(stdDevSource2, 2, MidpointRounding.AwayFromZero);
                double wtStdDevTotalDisplay = Math.Round(totalWeightedStdDev, 2, MidpointRounding.AwayFromZero);

                WaterStatisticsViewModel waterStatisticsViewModel = new WaterStatisticsViewModel();
                waterStatisticsViewModel.WtAvgSourceOneDiaplay = wtAvgSourceOneDiaplay;
                waterStatisticsViewModel.GrainsCuFtSourceOneDisplay = grainsS1;
                waterStatisticsViewModel.WtStdDevSourceOneDisplay = wtStdDevSourceOneDisplay;
                waterStatisticsViewModel.WtAvgSourceTwoDisplay = wtAvgSourceTwoDiaplay;
                waterStatisticsViewModel.GrainsCuFtSourceTwoDisplay = grainsS2;
                waterStatisticsViewModel.WtStdDevSourceTwoDisplay = wtStdDevSourceTwoDisplay;
                waterStatisticsViewModel.WtAvgTotalDisplay = wtAvgTotalDisplay;
                waterStatisticsViewModel.GrainsCuFtTotalDisplay = grainsWtTotal;
                waterStatisticsViewModel.WtStdDevTotalDisplay = wtStdDevTotalDisplay;
                waterStatisticsViewModel.usingTwoSources = hasTwoSources;
                return waterStatisticsViewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the Average Data By Week
        /// </summary>
        /// <param name="agencyId">agencyId parameter</param>
        /// <param name="order">order parameter</param>
        public void GetAverageDataByWeek(string agencyId, int order)
        {
            try
            {
                var waterdata = (from r in rtiContext.water_data
                                 where r.sourceID.ToString() == agencyId
                                 select r).ToList();

                if (waterdata != null && waterdata.Count != 0)
                {

                    var firstSelectedDate = waterdata.FirstOrDefault();
                    var LastSelectedDate = waterdata.LastOrDefault();
                    DateTime firstDate;
                    DateTime lastDate;
                    DateTime previousDate;

                    //First Selected Date
                    var firstDateData = firstSelectedDate == null ? DateTime.Now.ToString() : firstSelectedDate.measurment_date;
                    firstDate = ConvertDateString(firstDateData);

                    //Last Selected Date
                    var lastDateData = LastSelectedDate == null ? DateTime.Now.ToString() : LastSelectedDate.measurment_date;
                    lastDate = ConvertDateString(lastDateData);

                    //Previous Selected Date
                    var previousDateData = firstSelectedDate == null ? DateTime.Now.ToString() : firstSelectedDate.measurment_date;
                    previousDate = ConvertDateString(previousDateData);

                    int weeks = Convert.ToInt32(((lastDate - firstDate).TotalDays / 7) + 1);

                    int nextWeek = WeekNumberCalculator(previousDate.AddDays(7));
                    List<int>[] groupedWeekCondData = new List<int>[weeks];

                    // Calculate weekly average
                    int i = 0;
                    foreach (var entry in waterdata)
                    {
                        DateTime tmpMeasurmentDate = DateTime.Parse(entry.measurment_date, new CultureInfo("en-US", true));
                        int currentWeek = WeekNumberCalculator(tmpMeasurmentDate);
                        int average;
                        if (currentWeek != nextWeek)
                        {
                            dataForWeekToAverage.Add(tmpMeasurmentDate, entry.cond.Value);
                        }
                        else
                        {
                            List<int> valuesForWeek = new List<int>();
                            foreach (var condVal in dataForWeekToAverage.Values)
                            {
                                valuesForWeek.Add(condVal);
                            }
                            if (dataForWeekToAverage.Count == 0)
                            {
                                valuesForWeek.Add(entry.cond.Value);
                            }
                            groupedWeekCondData[i] = valuesForWeek;
                            average = Convert.ToInt32(valuesForWeek.Average());
                            Tuple<int, int, List<int>> weekCondVal = new Tuple<int, int, List<int>>(currentWeek, average, groupedWeekCondData[i]);
                            if (order == 1)
                            {
                                weeklyAverageCondiuctivityDataSource1.Add(tmpMeasurmentDate, weekCondVal);
                            }
                            else if (order == 2)
                            {
                                weeklyAverageConductivityDataSource2.Add(tmpMeasurmentDate, weekCondVal);
                            }
                            nextWeek = WeekNumberCalculator(tmpMeasurmentDate.AddDays(7));
                            dataForWeekToAverage.Clear();
                            i++;
                        }
                        previousDate = tmpMeasurmentDate;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// std Dev Data By Week
        /// </summary>
        /// <param name="agencyId">agencyId parameter</param>
        /// <param name="order">order parameter</param>
        private void StdDevDataByWeek(string agencyId, int order)
        {
            try
            {
                var waterdata = (from r in rtiContext.water_data
                                 where r.sourceID.ToString() == agencyId
                                 select r).ToList();

                if (waterdata != null && waterdata.Count > 0)
                {
                    var firstSelectedDate = waterdata.FirstOrDefault();
                    var LastSelectedDate = waterdata.LastOrDefault();
                    DateTime firstDate;
                    DateTime lastDate;
                    DateTime previousDate;

                    //First Selected Date
                    var firstDateData = firstSelectedDate == null ? DateTime.Now.ToString() : firstSelectedDate.measurment_date;
                    firstDate = ConvertDateString(firstDateData);

                    //Last Selected Date
                    var lastDateData = LastSelectedDate == null ? DateTime.Now.ToString() : LastSelectedDate.measurment_date;
                    lastDate = ConvertDateString(lastDateData);

                    //Previous Selected Date
                    var previousDateData = firstSelectedDate == null ? DateTime.Now.ToString() : firstSelectedDate.measurment_date;
                    previousDate = ConvertDateString(previousDateData);

                    int weeks = Convert.ToInt32(((lastDate - firstDate).TotalDays / 7) + 1);
                    // Declare local variables
                    int nextWeek = WeekNumberCalculator(previousDate.AddDays(7));
                    List<int>[] groupedWeekCondData = new List<int>[weeks];

                    // Calculate weekly average
                    int i = 0;
                    foreach (var entry in waterdata)
                    {
                        int currentWeek = WeekNumberCalculator(ConvertDateString(entry.measurment_date));
                        int standardDeviation;
                        if (currentWeek != nextWeek)
                        {
                            dataForWeekToAverage.Add(ConvertDateString(entry.measurment_date), entry.cond.Value);
                        }
                        else
                        {
                            List<int> valuesForWeek = new List<int>();
                            foreach (var condVal in dataForWeekToAverage.Values)
                            {
                                valuesForWeek.Add(condVal);
                            }

                            groupedWeekCondData[i] = valuesForWeek;
                            if (valuesForWeek.Count > 1)
                            {
                                standardDeviation = Convert.ToInt32(StandardDeviation(valuesForWeek));
                            }
                            else
                            {
                                standardDeviation = 0;
                            }
                            Tuple<int, int, List<int>> weekCondVal = new Tuple<int, int, List<int>>(currentWeek, standardDeviation, groupedWeekCondData[i]);

                            if (order == 1)
                            {
                                weeklyStandardDeviationConductivityDataSource1.Add(ConvertDateString(entry.measurment_date), weekCondVal);
                            }
                            else if (order == 2)
                            {
                                weeklyStandardDeviationConductivityDataSource2.Add(ConvertDateString(entry.measurment_date), weekCondVal);
                            }
                            nextWeek = WeekNumberCalculator(ConvertDateString(entry.measurment_date).AddDays(7));
                            dataForWeekToAverage.Clear();
                        }
                        previousDate = ConvertDateString(entry.measurment_date);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Standard Deviation
        /// </summary>
        /// <param name="valueList">valueList parameter</param>
        /// <returns>Returns Standard Deviation</returns>
        public static double StandardDeviation(List<int> valueList)
        {
            try
            {
                double mean = 0.0;
                double sum = 0.0;
                int k = 1;
                foreach (int value in valueList)
                {
                    double tmpM = mean;
                    mean += (value - tmpM) / k;
                    sum += (value - tmpM) * (value - mean);
                    k++;
                }
                return Math.Sqrt(sum / (k - 2));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Fetches SourceId TP
        /// </summary>
        /// <param name="customerId">CustomerId parameter</param>
        /// <returns>Returns sources</returns>
        public double[] FetchSourceIdTP(string customerId)
        {
            try
            {
                double SourceId = 0;
                double SourceIdSecond = 0;
                string agencyIdFirstSource = string.Empty;
                string agencyIdSecondSource = string.Empty;
                var custWater = (from r in rtiContext.customer_water where r.customer_customerID.ToString() == customerId select r).FirstOrDefault();
                if (custWater.first_sourceID != 0)
                {
                    var agency = (from r in rtiContext.sources where r.sources_sourceID == custWater.first_sourceID select r).FirstOrDefault();
                    agencyIdFirstSource = agency.agency_id;
                }
                if (custWater.second_sourceID != 0)
                {
                    var agency = (from r in rtiContext.sources where r.sources_sourceID == custWater.second_sourceID select r).FirstOrDefault();
                    agencyIdSecondSource = agency.agency_id;
                }
                SourceId = Convert.ToDouble(agencyIdFirstSource);
                if (agencyIdSecondSource != null && !agencyIdSecondSource.Trim().Equals(string.Empty))
                {
                    SourceIdSecond = Convert.ToDouble(agencyIdSecondSource);
                }
                double[] arrSources = new double[2];
                arrSources[0] = SourceId;
                arrSources[1] = SourceIdSecond;
                return arrSources;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the Grains Weight Total
        /// </summary>
        /// <param name="customerId">customerId parameter</param>
        /// <returns>Returns total grains weight</returns>
        public double GetGrainsWeightTotal(string customerId)
        {
            try
            {
                var custWater = (from r in rtiContext.customer_water
                                 where r.customer_customerID.ToString() == customerId
                                 select r).FirstOrDefault();

                if (custWater.first_sourceID != 0)
                {
                    var Source1 = (from r in rtiContext.sources
                                   where r.sources_sourceID == custWater.first_sourceID
                                   select r).FirstOrDefault();

                    GetAverageDataByWeek(Source1.agency_id, 1);
                }
                if (custWater.second_sourceID != 0)
                {
                    var Source2 = (from r in rtiContext.sources
                                   where r.sources_sourceID == custWater.second_sourceID
                                   select r).FirstOrDefault();

                    GetAverageDataByWeek(Source2.agency_id, 2);
                }

                List<int> sourceOneWkAvgList = new List<int>();
                List<int> sourceTwoWkAvgList = new List<int>();
                double totaltWeightedAverage = 0;
                double sourcePercentOne;
                double sourcePercentTwo;
                double sourceOneSliderPercent = custWater.firstSourcePercentage;
                double sourceTwoSliderPercent = 100 - custWater.firstSourcePercentage;
                sourcePercentOne = sourceOneSliderPercent * 0.01;
                sourcePercentTwo = sourceTwoSliderPercent * 0.01;
                if (custWater.first_sourceID != 0 && custWater.second_sourceID != 0)
                {
                    foreach (var avgCond in weeklyAverageCondiuctivityDataSource1)
                    {
                        sourceOneWkAvgList.Add(avgCond.Value.Item2);
                    }

                    // Weighted Average Souce Two
                    foreach (var avgCond in weeklyAverageConductivityDataSource2)
                    {
                        sourceTwoWkAvgList.Add(avgCond.Value.Item2);
                    }

                    double averageSource1 = 0.0;
                    double averageSource2 = 0.0;
                    if (sourceOneWkAvgList != null && sourceOneWkAvgList.Count != 0)
                    {
                        averageSource1 = sourceOneWkAvgList.Average();
                    }
                    if (sourceTwoWkAvgList != null && sourceTwoWkAvgList.Count != 0)
                    {
                        averageSource2 = sourceTwoWkAvgList.Average();
                    }

                    totaltWeightedAverage = (averageSource1 * sourcePercentOne) + (averageSource2 * sourcePercentTwo);
                }
                else if (custWater.first_sourceID != 0)
                {
                    foreach (var avgCond in weeklyAverageCondiuctivityDataSource1)
                    {
                        sourceOneWkAvgList.Add(avgCond.Value.Item2);
                    }

                    if (sourceOneWkAvgList != null && sourceOneWkAvgList.Count != 0)
                    {
                        double averageSource1 = sourceOneWkAvgList.Average();
                        totaltWeightedAverage = (averageSource1 * sourcePercentOne);
                    }
                }
                double grainsWtTotal = Math.Round(totaltWeightedAverage * (1 / (2 * 17.1)), 2, MidpointRounding.AwayFromZero);
                return grainsWtTotal;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// week Number Calculator
        /// </summary>
        /// <param name="fromDate">fromDate parameter</param>
        /// <returns>Returns week number</returns>
        private int WeekNumberCalculator(DateTime fromDate)
        {
            try
            {
                // Get Jan 1st of the year
                DateTime startOfYear = fromDate.AddDays(-fromDate.Day + 1).AddMonths(-fromDate.Month + 1);

                // Get dec 31st of the year
                DateTime endOfYear = startOfYear.AddYears(1).AddDays(-1);
                int[] ISO8601Correction = { 6, 7, 8, 9, 10, 4, 5 };
                int nds = fromDate.Subtract(startOfYear).Days + ISO8601Correction[(int)startOfYear.DayOfWeek];
                int weekNumber = nds / 7;

                switch (weekNumber)
                {
                    case 0:
                        // Return weeknumber of dec 31st of the previous year
                        weekNumber = WeekNumberCalculator(startOfYear.AddDays(-1));
                        break;
                    case 53:
                        // If dec 31st falls before thursday it is week 01 of next year
                        if (endOfYear.DayOfWeek < DayOfWeek.Thursday)
                        {
                            weekNumber = 1;
                        }
                        break;
                }
                return weekNumber;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the Water Demand
        /// </summary>
        /// <param name="customerId">CustomerId parameter</param>
        /// <returns>Returns water demand</returns>
        public int GetWaterDemand(long customerId)
        {
            try
            {
                var demand = dbContext.customers != null ? (from r in dbContext.customers
                                                            where r.customerID == customerId
                                                            select r).FirstOrDefault()
                                                                : new customer();

                int waterdemand = Convert.ToInt32(demand.demand);
                return waterdemand;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the Customer Trains
        /// </summary>
        /// <param name="customerId">CustomerId parameter</param>
        /// <returns>Returns list of trains</returns>
        public List<train> GetCustomerTrains(long customerId)
        {
            try
            {
                var TrainData = dbContext.trains != null ? (from r in dbContext.trains
                                                            where r.customer_customerID == customerId
                                                            select r).ToList()
                                                                : new List<train>();
                return TrainData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the Customer Vessels
        /// </summary>
        /// <param name="customerId">CustomerId parameter</param>
        /// <returns>Returns list of vessels</returns>
        public List<vessel> GetCustomerVessels(long customerId)
        {
            try
            {
                var VesselData = dbContext.vessels != null ? (from r in dbContext.vessels
                                                              where r.vessel_customerID == customerId.ToString()
                                                              select r).ToList()
                                                                  : new List<vessel>();
                return VesselData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Converts the date string.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>Returns the converted Datetime</returns>
        private static DateTime ConvertDateString(string date)
        {
            try
            {
                DateTime returnDate = DateTime.Parse(date, new CultureInfo("en-US", true));
                return returnDate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetFirstSourcePercentage(string customerId)
        {
            try
            {
                int FirstSourcePercentage = (from r in dbContext.customer_water
                                             where r.customer_customerID.ToString() == customerId
                                             select r.firstSourcePercentage).FirstOrDefault();
                return FirstSourcePercentage;
            }
            catch (Exception) {
                throw;
            }
        }

        #endregion Methods
    }
}
