// -----------------------------------------------------------------------
// <copyright file="PredictiveSystemController.cs" company="RTI">
// RTI
// </copyright>
// <summary>Predictive System Controller</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Web.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Web.Mvc;
    using DotNet.Highcharts;
    using DotNet.Highcharts.Enums;
    using DotNet.Highcharts.Helpers;
    using DotNet.Highcharts.Options;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Interfaces.Services;
    using RTI.ModelingSystem.Core.Models;
    //using RTI.ModelingSystem.Web.Models;
    using Point = DotNet.Highcharts.Options.Point;
    using System.Globalization;

    #endregion Usings



    /// <summary>
    /// PredictiveSystemController class
    /// </summary>
    [HandleError(View = "ErrorView")]
    public class PredictiveSystemController : Controller
    {
        #region Properties

        /// <summary>
        /// New System Conditions
        /// </summary>
        public static SystemConditions newSystemConditions = new SystemConditions();

        /// <summary>
        /// modified Predictive Repository
        /// </summary>
        private IPredictiveModelRepository modifiedPredictiveRepository;

        /// <summary>
        /// predictive Model Service
        /// </summary>
        private IPredictiveModelService predictiveModelService;

        /// <summary>
        /// customer Repository
        /// </summary>
        private IRepository<customer> customerRepository;

        /// <summary>
        /// modified Customer Repository
        /// </summary>
        private ICustomerRepository modifiedCustomerRepository;

        /// <summary>
        /// vessel Repository
        /// </summary>
        private IRepository<vessel> vesselRepository;

        /// <summary>
        /// train Repository
        /// </summary>
        private IRepository<train> trainRepository;

        /// <summary>
        /// Current Salt Split
        /// </summary>
        private double currentSaltSplit = 0;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PredictiveSystemController" /> class
        /// </summary>
        /// <param name="modifiedPredictiveRepository">modified Predictive Repository</param>
        /// <param name="predictiveModelService">predictive Model Service</param>
        /// <param name="customerRepository">customer Repository</param>
        /// <param name="modifiedCustomerRepository">modified Customer Repository</param>
        /// <param name="vesselRepository">vessel Repository</param>
        /// <param name="trainRepository">train Repository</param>
        public PredictiveSystemController(IPredictiveModelRepository modifiedPredictiveRepository, IPredictiveModelService predictiveModelService, IRepository<customer> customerRepository, ICustomerRepository modifiedCustomerRepository, IRepository<vessel> vesselRepository, IRepository<train> trainRepository)
        {
            this.modifiedPredictiveRepository = modifiedPredictiveRepository;
            this.predictiveModelService = predictiveModelService;
            this.customerRepository = customerRepository;
            this.modifiedCustomerRepository = modifiedCustomerRepository;
            this.vesselRepository = vesselRepository;
            this.trainRepository = trainRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Default index view
        /// </summary>
        /// <returns>Returns the index view</returns>
        public ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Predictive System Performance view
        /// </summary>
        /// <param name="id">customer identifier</param>
        /// <returns>Returns the Predictive System Performance view</returns>
        public ActionResult PredictiveSystemPerformance()
        {

            long customerId = 0;
            if (this.Session["CustomerId"] != null)
            {
                customerId = Convert.ToInt64(this.Session["CustomerId"]);
            }
            customer customerDetails = new customer();
            SystemSummaryViewModel ssvmDetails = new SystemSummaryViewModel();
            customer_water custWaterDetails = null;
            try
            {
                if (customerId != 0)
                {
                    var Customer = this.customerRepository.GetAll();
                    customerDetails = Customer.Where(x => x.customerID == customerId).FirstOrDefault();
                    ssvmDetails = new SystemSummaryViewModel();
                    ssvmDetails.CustomerDetails = customerDetails;
                    custWaterDetails = this.modifiedCustomerRepository.GetWaterSourceIds(customerId);
                }
                if (custWaterDetails != null)
                {
                    if (custWaterDetails.first_sourceID != 0)
                    {
                        ssvmDetails.WaterSourceOne = this.modifiedCustomerRepository.GetWaterSourceDetails(custWaterDetails.first_sourceID);//Get first watersource details
                        if (ssvmDetails.WaterSourceOne != null)
                        {
                            ssvmDetails.WaterSourceOne.full_site_name = ssvmDetails.WaterSourceOne.full_site_name.Replace("@", string.Empty);//Remove @ symbol from the watersource name
                        }
                    }
                    else
                    {
                        ssvmDetails.WaterSourceOne = null;
                    }
                    if (custWaterDetails.second_sourceID != 0)
                    {
                        ssvmDetails.WaterSourceTwo = this.modifiedCustomerRepository.GetWaterSourceDetails(custWaterDetails.second_sourceID);//Get second watersource details
                        if (ssvmDetails.WaterSourceTwo != null)
                        {
                            ssvmDetails.WaterSourceTwo.full_site_name = ssvmDetails.WaterSourceTwo.full_site_name.Replace("@ ", string.Empty);//Remove @ symbol from the watersource name
                        }
                    }
                    else
                    {
                        ssvmDetails.WaterSourceTwo = null;
                    }
                    ssvmDetails.Trains = this.modifiedCustomerRepository.GetCustomerTrains(customerId);//Get list of cutomer train details
                }
                else
                {
                    this.Session["IsNewCustomer"] = "True";
                    this.Session["HasTrainDetails"] = "Check";
                }
                ssvmDetails.CustomerType = Convert.ToString(this.Session["IsNewCustomer"]);
                ssvmDetails.HasTrainDetails = Convert.ToString(this.Session["HasTrainDetails"]);
                if (Convert.ToString(this.Session["HasTrainDetails"]) == "Verify")
                {
                    var count = this.vesselRepository.GetAll().Where(p => p.vessel_customerID == (customerId).ToString()).Count();
                    if (count != 0)
                    {
                        ssvmDetails.HasTrainDetails = "Yes";
                    }
                    else
                    {
                        ssvmDetails.HasTrainDetails = "No";
                    }
                }
                WaterStatisticsViewModel objWaterStatisticsViewModel = new WaterStatisticsViewModel();
                objWaterStatisticsViewModel = this.modifiedPredictiveRepository.GetWaterStatistics(customerId);
                ssvmDetails.waterStatisticsViewModeldetails = objWaterStatisticsViewModel;
            }
            catch (Exception)
            {
                throw;
            }
            return this.View(ssvmDetails);
        }

        /// <summary>
        /// Plots the Salt Split Chart
        /// </summary>
        /// <param name="numWeeks">number of Weeks</param>
        /// <param name="AvgResinage">Average Resin age</param>
        /// <param name="startingSS">starting Salt Split</param>
        /// <param name="maxDegSS">maximum Degradation Salt Split</param>
        /// <param name="SelectedTrain">Selected Train</param>
        /// <param name="CleaningEffectiveness">Cleaning Effectiveness</param>
        /// <param name="IsDashboard">IsDashboard flag</param>
        /// <returns>Returns the View</returns>
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult PlotSaltSplitChart(double numWeeks = 0, double AvgResinage = 0, double startingSS = 25.0, double maxDegSS = 62.0, string SelectedTrain = "0", double CleaningEffectiveness = 28.0, bool IsDashboard = false)
        {
            try
            {
                // This locatoin is where the incorrect life expectancy is passed
                Highcharts chart = this.FetchSaltSplitChartData(numWeeks, AvgResinage, startingSS, maxDegSS, SelectedTrain, CleaningEffectiveness, IsDashboard);
                this.Session["SelectedTrain"] = SelectedTrain;
                return this.PartialView("_SaltSplitChart", chart);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Fetches the salt split chart data.
        /// </summary>
        /// <param name="numWeeks">The number weeks.</param>
        /// <param name="avgResinage">The average resin age.</param>
        /// <param name="startingSS">The starting system.</param>
        /// <param name="maxDegSS">The maximum deg system.</param>
        /// <param name="selectedTrain">The selected train.</param>
        /// <param name="CleaningEffectiveness">The cleaning effectiveness.</param>
        /// <param name="isDashboard">if set to <c>true</c> [is dashboard].</param>
        /// <returns></returns>
        private Highcharts FetchSaltSplitChartData(double numWeeks, double avgResinage, double startingSS, double maxDegSS, string selectedTrain, double CleaningEffectiveness, bool isDashboard)
        {
            try
            {
                Dictionary<double, double> currentSS = new Dictionary<double, double>();
                List<double> minimumSS = this.predictiveModelService.CalculateMinSaltSplit(Convert.ToInt64(this.Session["CustomerId"]), maxDegSS, selectedTrain);
                double minimumSaltSplit = minimumSS[0];
                double dblResinAge;
                if (avgResinage == 0)
                {
                    dblResinAge = minimumSS[1];
                }
                else
                {
                    dblResinAge = avgResinage;
                }
                if (numWeeks == 0)
                {
                    numWeeks = Convert.ToInt32(minimumSS[2]) * 52;
                }
                Number width, height;
                Dictionary<double, double> chartPoints = new Dictionary<double, double>();
                if (isDashboard == true)
                {
                    width = 500;
                    height = 290;
                }
                else
                {
                    width = 650;
                    height = 265;
                }
                chartPoints = this.predictiveModelService.ComputeDataPoints(numWeeks, startingSS, maxDegSS);
                Point[] afterCleaningSS = new Point[1];
                Point[] currentSaltSplitPoints = new Point[1];
                Point[] chartdataPoints = new Point[chartPoints.Count];
                object[] minThresholdSeries = new object[chartPoints.Count];
                int i = 0;
                foreach (var item in chartPoints)
                {
                    chartdataPoints[i] = new Point();
                    chartdataPoints[i].X = item.Key;
                    chartdataPoints[i].Y = item.Value;
                    minThresholdSeries.SetValue(minimumSaltSplit, i);
                    i++;
                }
                Point point1 = new Point();
                Point point2 = new Point();
                currentSS = this.predictiveModelService.CurrentSSConditions(dblResinAge, CleaningEffectiveness, startingSS, numWeeks);
                this.currentSaltSplit = (currentSS != null && currentSS.Count > 0) ? currentSS.ElementAt(0).Value : 0;

                point1.X = (currentSS != null && currentSS.Count > 0) ? currentSS.ElementAt(0).Key : 0;
                point1.Y = (currentSS != null && currentSS.Count > 0) ? currentSS.ElementAt(0).Value : 0;
                afterCleaningSS[0] = point1;
                if (currentSS != null && currentSS.Count > 1)
                {
                    point2.X = currentSS.ElementAt(1).Key;
                    point2.Y = currentSS.ElementAt(1).Value;
                }
                currentSaltSplitPoints[0] = point2;
                Highcharts chart = new Highcharts("chart1")
                    .InitChart(new Chart { Width = width, Height = height, DefaultSeriesType = ChartTypes.Spline, ZoomType = ZoomTypes.Xy, SpacingRight = 30 })
                    .SetTitle(new Title { Text = "Salt Split Degradation" })
                    .SetCredits(new Credits { Enabled = false })
                    .SetTooltip(new Tooltip
                    {
                        HeaderFormat = "<b>{series.name}</b><br>",
                        PointFormat = "<b>Week :{point.x:.0f}<br> <b>Salt Split: {point.y:.2f}</b>",
                        Enabled = true
                    })
                    .SetLegend(new Legend
                    {
                        BorderWidth = 1,
                        Width = 450
                    })
                    .SetXAxis(new XAxis
                    {
                        Type = AxisTypes.Linear,
                        MinRange = 0,
                        Max = numWeeks,
                        Title = new XAxisTitle { Text = "Number of Weeks" },
                        TickInterval = 50
                    })
                    .SetYAxis(new YAxis
                    {
                        Title = new YAxisTitle { Text = "Salt Split" },
                        Min = 0,
                        Max = 30,
                        StartOnTick = false,
                        EndOnTick = false,
                        TickInterval = 10,
                        PlotLines = new YAxisPlotLines[1] { new YAxisPlotLines { Value = minimumSaltSplit, Color = Color.Red, Width = 2, DashStyle = DashStyles.ShortDash,
						Label = new YAxisPlotLinesLabel { Text = "MinimumSS For Demand=" + Math.Round(minimumSaltSplit, 2) + string.Empty } } }

                    })
                    .SetPlotOptions(new PlotOptions
                    {
                        Spline = new PlotOptionsSpline
                        {
                            LineWidth = 2,
                            Marker = new PlotOptionsSplineMarker
                            {
                                Enabled = false,
                                States = new PlotOptionsSplineMarkerStates
                                {
                                    Hover = new PlotOptionsSplineMarkerStatesHover
                                    {
                                        Enabled = true,
                                        Radius = 2
                                    }
                                }
                            },
                            Shadow = false,
                            States = new PlotOptionsSplineStates { Hover = new PlotOptionsSplineStatesHover { LineWidth = 1 } },
                            Point = new PlotOptionsSplinePoint()
                        }
                    })
                    .SetSeries(new[] { new Series{
                    Name = "Salt Split degradation",
                    Type = ChartTypes.Spline,
                    Data = new Data(chartdataPoints),
                    Color = Color.Green,
                   
                },
                new Series
                {
                    Name = "Current SaltSplit",                   
                    Type = ChartTypes.Scatter,
                    Data = new Data(afterCleaningSS),
                    Color = Color.Blue
                },
                new Series
                {
                    Name = "After Cleaning SaltSplit",
                    Type = ChartTypes.Scatter,
                    Data = new Data(currentSaltSplitPoints),
                    Color = Color.DarkRed
                }
                });
                return chart;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [OutputCache(Duration = 0, VaryByParam = "none")]
        /// <summary>
        /// System Conditions Action
        /// </summary>
        /// <returns>Returns the view</returns>
        public PartialViewResult SystemConditions()
        {
            try
            {
                return PartialView("_SystemConditions", newSystemConditions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Sets the Simulation Method
        /// </summary>
        /// <param name="simMethod">Simulation Method</param>
        /// <returns>Retutns the Simulation Method</returns>
        private static string SetSimMethod(string simMethod)
        {
            try
            {
                if (simMethod.Equals("Modal value"))
                {
                    simMethod = "Mode";
                }
                else if (simMethod.Equals("Best case"))
                {
                    simMethod = "Min";
                }
                else if (simMethod.Equals("Worst case"))
                {
                    simMethod = "Max";
                }
                else if (simMethod.Equals("Average value"))
                {
                    simMethod = "Mean";
                }
                return simMethod;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Throughput Chart Action
        /// </summary>		
        /// <param name="startingSS">starting Salt Split</param>
        /// <param name="resinLifeExpectancy">resin Life Expectancy</param>
        /// <param name="simulationconfidence">simulation confidence</param>
        /// <param name="num_simulation_iterations">number of simulation iterations</param>
        /// <param name="simMethod">simulation Method</param>
        /// <param name="stdDev_threshold">stdDev threshold</param>
        /// <param name="resinAge">resin Age parameter</param>
        /// <param name="RTIcleaning_effectivness">RTI cleaning effectivness</param>
        /// <param name="Replacement_Level">Replacement Level</param>
        /// <param name="RTIcleaning_Level">RTI cleaning Level</param>
        /// <param name="ReGen_effectivness">ReGeneration effectivness</param>
        /// <param name="SelectedTrain">Selected Train</param>
        /// <param name="DontReplaceResin">Dont Replace Resin</param>
        /// <param name="IsDashboard">IsDashboard flag</param>
        /// <returns>Returns the view</returns>
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult ThroughputChart(double startingSS = 25.0, double resinLifeExpectancy = 0, int simulationconfidence = 95, int num_simulation_iterations = 100, string simMethod = "Modal value", int stdDev_threshold = 2, double resinAge = 0, double MaxDegradation = 62.0, double Replacement_Level = 10, double RTIcleaning_Level = 17.0, double ReGen_effectivness = 99.75, string SelectedTrain = "0", bool DontReplaceResin = false, double CleaningEffectiveness = 28.0, bool IsDashboard = false)
        {
            try
            {

                var customerId = this.Session["CustomerId"].ToString();
                string calculationMethod = SetSimMethod(simMethod);
                List<double> minimumSS = this.predictiveModelService.CalculateMinSaltSplit(Convert.ToInt64(this.Session["CustomerId"]), MaxDegradation, SelectedTrain);
                double minimumSaltSplit = minimumSS[0];
                double dblResinAge = 0;
                if (resinAge == 0)
                {
                    dblResinAge = minimumSS[1];
                }
                else
                {
                    dblResinAge = resinAge;
                }
                if (resinLifeExpectancy == 0)
                {
                    resinLifeExpectancy = Convert.ToInt32(minimumSS[2]) * 52;
                }

                this.predictiveModelService.ComputeDataPoints(resinLifeExpectancy, startingSS, MaxDegradation);
                Dictionary<double, double> currentSS = this.predictiveModelService.CurrentSSConditions(dblResinAge, CleaningEffectiveness, startingSS, resinLifeExpectancy);
                if (currentSS != null && currentSS.Count > 1)
                {
                    this.currentSaltSplit = currentSS.ElementAt(0).Value;
                }
                PriceData priceData = this.predictiveModelService.Thoughputchart(customerId, this.currentSaltSplit, startingSS, resinLifeExpectancy, simulationconfidence, num_simulation_iterations, calculationMethod
                    , stdDev_threshold, dblResinAge, Replacement_Level, RTIcleaning_Level, SelectedTrain, DontReplaceResin);
                Highcharts chart = new Highcharts("Charts");
                if (priceData != null && priceData.CleanThroughput != null && priceData.NormalOpsThroughput != null)
                {
                    this.Session["Data_ToSend"] = priceData;
                    if (priceData.CleanThroughput.Count > 0 && priceData.NormalOpsThroughput.Count > 0)
                    {
                        chart = plotTPData(priceData.CleanThroughput, priceData.NormalOpsThroughput, IsDashboard);
                    }
                    if (currentSS != null && currentSS.Count > 1)
                    {
                        newSystemConditions.SaltSplitTodayBefore = Math.Round(currentSS.ElementAt(0).Value, 2);
                    }
                    if (currentSS != null && currentSS.Count > 1)
                    {
                        newSystemConditions.SaltSplitTodayAfter = Math.Round(currentSS.ElementAt(1).Value, 2);
                    }


                    newSystemConditions.TrainDisplay = SelectedTrain == "0" ? "All Trains" : "Train " + minimumSS[3];
                    newSystemConditions.RegenTimeAverageBefore = priceData.RegenTimeAverageBefore;
                    newSystemConditions.RegenTimeAverageAfter = priceData.RegenTimeAverageAfter;
                    newSystemConditions.RegensPerWeekAverageBefore = priceData.RegensPerWeekAverageBefore;
                    newSystemConditions.RegensPerWeekAverageAfter = priceData.RegensPerWeekAverageAfter;
                    newSystemConditions.HoursPerRunAverageBefore = priceData.HoursPerRunAverageBefore;
                    newSystemConditions.HoursPerRunAverageAfter = priceData.HoursPerRunAverageAfter;
                    newSystemConditions.ThroughputAverageBefore = priceData.ThroughputAverageBefore;
                    newSystemConditions.ThroughputAverageAfter = priceData.ThroughputAverageAfter;

                }
                return this.View("_ThroghPutChart", chart);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Builds the high chart for Throughput chart
        /// </summary>
        /// <param name="tpCurveClean">TP curve clean</param>
        /// <param name="tpCurveNoClean">TP curve no clean</param>
        /// <param name="isDashboard">IsDashboard flag</param>
        /// <returns>Returns the high chart</returns>
        private Highcharts plotTPData(Dictionary<DateTime, Tuple<int, double, string>> tpCurveClean, Dictionary<DateTime, Tuple<int, double, string>> tpCurveNoClean, bool isDashboard)
        {
            try
            {
                double cleanAverage = 0;
                double noCleanAverage = 0;
                double xaxisMinValue = 0.0;
                double xaxisMaxValue = Convert.ToDouble(tpCurveClean.Last().Value.Item1);
                List<Point> replacePts = new List<Point>();
                List<Point> cleanedPts = new List<Point>();
                List<double> xseries = new List<double>();
                double[] chartdata1 = new double[tpCurveClean.Count];
                double[] chartdata2 = new double[tpCurveNoClean.Count];
                Point[] chartData_Point1 = new Point[tpCurveClean.Count];
                Point[] chartData_Point2 = new Point[tpCurveNoClean.Count];
                int counter = 0;
                Number width, height;
                foreach (DateTime dateTime in tpCurveClean.Keys)
                {
                    Tuple<int, double, string> tuple = tpCurveClean[dateTime];
                    chartData_Point1[counter] = new Point();
                    chartData_Point1[counter].X = tuple.Item1;
                    chartData_Point1[counter].Y = !double.IsInfinity(tuple.Item2) ? tuple.Item2 : 0;
                    chartdata1[counter] = !double.IsInfinity(tuple.Item2) ? tuple.Item2 : 0;
                    xseries.Add(tuple.Item1);
                    counter++;
                    if (tuple.Item3 == "Clean")
                    {
                        cleanedPts.Add(new Point() { X = tuple.Item1, Y = tuple.Item2 });
                    }
                }
                Number? MaximumWeek = chartData_Point1 != null && chartData_Point1.Length > 0 ? chartData_Point1[chartdata1.Count() - 1].X : 0;

                cleanAverage = chartdata1.Length > 0 ? chartdata1.Average() : 0;
                counter = 0;
                if (isDashboard == true)
                {
                    width = 500;
                    height = 290;
                }
                else
                {
                    width = 650;
                    height = 300;
                }
                foreach (DateTime dateTime in tpCurveNoClean.Keys)
                {
                    Tuple<int, double, string> tuple = tpCurveNoClean[dateTime];
                    chartData_Point2[counter] = new Point();
                    chartData_Point2[counter].X = tuple.Item1;
                    chartData_Point2[counter].Y = !double.IsInfinity(tuple.Item2) ? tuple.Item2 : 0;
                    chartdata2[counter] = !double.IsInfinity(tuple.Item2) ? tuple.Item2 : 0;
                    counter++;
                    if (tuple.Item3 == "Replace")
                    {
                        replacePts.Add(new Point() { X = tuple.Item1, Y = tuple.Item2 });
                    }
                }
                noCleanAverage = chartdata2.Length > 0 ? chartdata2.Average() : 0;
                xaxisMinValue = xseries.Count > 0 ? xseries[0] : 0;

                // Set up area color fill gradient parameters
                Gradient gradLine = new Gradient();
                int[] LinearGradient = new int[] { 0, 0, 0, 300 };
                gradLine.LinearGradient = LinearGradient;
                object[,] stops2 = new object[,] { { 0, "#F7EF9B" }, { 1, "#FFFFFF" } };
                gradLine.Stops = stops2;

                Series withCleaning = new Series
                {
                    Name = "With Cleaning",
                    Data = new Data(chartData_Point1),
                    PlotOptionsArea = new PlotOptionsArea
                    {
                        FillColor = new BackColorOrGradient(Color.FromArgb(30, 0, 128, 0)),
                        Color = Color.FromArgb(80, 153, 80)
                    }
                };

                Series withoutCleaning = new Series
                {
                    Name = "Without Cleaning",
                    Data = new Data(chartData_Point2),
                    PlotOptionsArea = new PlotOptionsArea
                    {
                        FillColor = new BackColorOrGradient(gradLine),
                        Color = System.Drawing.Color.Goldenrod
                    }
                };
                Highcharts chart1 = new Highcharts("chart")
                    .SetOptions(new GlobalOptions
                    {
                        Lang = new DotNet.Highcharts.Helpers.Lang { ThousandsSep = ",", DecimalPoint = "." }
                    })
                   .InitChart(new Chart { Width = width, Height = height, DefaultSeriesType = ChartTypes.Area, ZoomType = ZoomTypes.X, SpacingRight = 20 })
                   .SetTitle(new Title { Text = "Throughput Forecast" })
                   .SetTooltip(new Tooltip
                   {
                       HeaderFormat = "<b>Week: {point.x:.0f}</b><br>",
                       //PointFormat = "<b><span style=\"color:this.data.marker.fillcolor\">{series.name}: {point.y:,.2f}</b></span><br>",

                       // Use javascript function to control the tool tip coloring, to add a week number display, and to add thousands place seperartors 
                       Formatter = @"function() {var s = [];var X = '';$.each(this.points, function(index,point){if((index%2)==0){s.push('<span style=""color:#E6B800;font-weight:bold;"">'+ point.series.name + ' : <span>' + '<b style=""font-weight:bold;"">' + point.y.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, "","") + '</b>');}else{s.push('<span style=""color:#509950;font-weight:bold;"">'+ point.series.name + ' : <span>' + '<b style=""font-weight:bold;"">' + point.y.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, "","") + '</b>');}X = point.x;});var header = '<span style=""font-weight:bold;"">Week: ' + X.toString() + '<span>';  s.splice(0, 0, header); var temp1 = s[1];var temp2 = s[2];s[1] = temp2;s[2] = temp1;if(s.length >= 5){temp1 = s[4];temp2 = s[3];s[4] = temp2;s[3] = temp1;}return s.join('<br>');}",
                       Enabled = true,
                       Shared = true
                   })
                   .SetXAxis(new XAxis
                   {
                       Title = new XAxisTitle { Text = "Number of Weeks" },
                       Type = AxisTypes.Linear,
                       MinRange = 0,
                       Min = xaxisMinValue,
                       Max = xaxisMaxValue,
                       TickInterval = 20,
                       Labels = new XAxisLabels { Formatter = "function() { return this.value;  }" }
                   })
                   .SetCredits(new Credits { Enabled = false })
                   .SetLegend(new Legend
                    {
                        BorderWidth = 1,
                    })
                   .SetYAxis(new YAxis
                   {
                       Title = new YAxisTitle { Text = "Throughput" },
                       Labels = new YAxisLabels { Formatter = "function() { return this.value; }" }
                   })
                   .SetPlotOptions(new PlotOptions
                   {
                       Area = new PlotOptionsArea
                       {
                           Marker = new PlotOptionsAreaMarker
                           {
                               Enabled = false,
                               Symbol = "circle",
                               Radius = 2,
                               States = new PlotOptionsAreaMarkerStates
                               {
                                   Hover = new PlotOptionsAreaMarkerStatesHover { Enabled = true }
                               }
                           },
                           PointInterval = 1,
                           PointStart = new PointStart(xaxisMinValue),
                       }
                   })
                   .SetSeries(new[] {  withoutCleaning, withCleaning,
				   new Series
				   {
				       Name = "Replace",                   
				       Type = ChartTypes.Scatter,
				       Data = new Data(replacePts.ToArray()),
				       Color = Color.Blue,
				   },
                   new Series
				   {
				       Name = "Clean",                   
				       Type = ChartTypes.Scatter,
				       Data = new Data(cleanedPts.ToArray()),
				       Color = Color.Red
                   },
                   new Series
				   {
				       Name = "Without Cleaning Average",                   
				       Type = ChartTypes.Spline,
				       Data = new Data(new[] { new Point
                                                       {
                                                           X = xaxisMinValue,
                                                           Y = noCleanAverage,
                                                       },
                                                       new Point
                                                       {
                                                           X = MaximumWeek,
                                                           Y = noCleanAverage,
                                                       }
                                                   }),
				       Color = Color.Goldenrod
,
                       PlotOptionsSpline=new PlotOptionsSpline
                       {
                           DashStyle=DashStyles.ShortDash,
                           LineWidth=1,
                           Marker = new PlotOptionsSplineMarker{Enabled = false},
                       }
				     },
                     new Series
				   {
				       Name = "With Cleaning Average",                   
				       Type = ChartTypes.Spline,
				       Data = new Data(new[] { new Point
                                                       {
                                                           X = xaxisMinValue,
                                                           Y = cleanAverage,
                                                       },
                                                       new Point
                                                       {
                                                           X = MaximumWeek,
                                                           Y = cleanAverage,
                                                       }
                                                   }),
				       Color = Color.ForestGreen,
                       PlotOptionsSpline=new PlotOptionsSpline
                       {
                           DashStyle=DashStyles.ShortDash,
                           LineWidth=1,
                           Marker = new PlotOptionsSplineMarker{Enabled = false}
                       }
				     }
                   }
                );
                return chart1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the Performance Settings
        /// </summary>
        /// <returns>Returns the view</returns>
        public ActionResult GetPerformanceSettings(string SelectedTrain = "0")
        {
            try
            {
                int selectedTrain = Convert.ToInt32(SelectedTrain);
                string customerId = this.Session["CustomerId"].ToString();
                PerformanceSettings settings = new PerformanceSettings();
                List<double> replacementPlan = new List<double>();
                List<TimeSpan> intervalList = new List<TimeSpan>();
                TimeSpan intervalSum = new TimeSpan();
                settings.trainList = new List<SelectListItem>();
                List<train> trains = this.trainRepository.GetAll().Where(p => p.customer_customerID.ToString() == customerId).ToList();
                settings.trainList.Add(new SelectListItem() { Text = "All Trains", Value = string.Empty + 0 });
                if (trains != null && trains.Count != 0)
                {
                    for (int i = 0; i < trains.Count; i++)
                    {
                        if (selectedTrain == 0)
                        {
                            settings.GPM += Convert.ToDouble(trains[i].gpm);
                        }
                        else if (selectedTrain == trains[i].trainID)
                        {
                            this.Session["TrainNumber"] = i + 1;
                            settings.GPM += Convert.ToDouble(trains[i].gpm);
                        }
                        settings.trainList.Add(new SelectListItem() { Text = "Train " + (i + 1), Value = string.Empty + trains[i].trainID });
                    }
                    settings.selectedTrain = 0;
                    this.Session["SelectedTrain"] = settings.selectedTrain;
                }
                List<vessel> vessels = this.vesselRepository.GetAll().Where(p => p.vessel_customerID == customerId).ToList();
                if (vessels != null && vessels.Count != 0)
                {
                    foreach (vessel vessel in vessels)
                    {
                        if (selectedTrain == 0)
                        {
                            if (vessel.vessel_number % 2 == 0)
                            {
                                replacementPlan.Add(Convert.ToDouble(vessel.replacement_plan));
                                DateTime purchasedate = Convert.ToDateTime(vessel.date_replaced, new CultureInfo("en-US", true));
                                TimeSpan interval = DateTime.Today - purchasedate;
                                intervalList.Add(interval);
                            }
                            settings.Size += Convert.ToDouble(vessel.size.Replace("$", string.Empty));
                        }
                        else if (selectedTrain == vessel.train_trainID)
                        {
                            if (vessel.vessel_number % 2 == 0)
                            {
                                replacementPlan.Add(Convert.ToDouble(vessel.replacement_plan));
                                DateTime purchasedate = Convert.ToDateTime(vessel.date_replaced, new CultureInfo("en-US", true));
                                TimeSpan interval = DateTime.Today - purchasedate;
                                intervalList.Add(interval);
                            }
                            settings.Size += Convert.ToDouble(vessel.size.Replace("$", string.Empty));
                        }
                    }

                    if (replacementPlan.Count > 0)
                    {
                        settings.ResinLifeExpectancy = Convert.ToInt32(replacementPlan.Average()) * 52;
                    }
                    else
                    {
                        settings.ResinLifeExpectancy = 364;
                    }

                    foreach (var span in intervalList)
                    {
                        intervalSum += span;
                    }
                    if (intervalList.Count() > 0)
                    {

                        TimeSpan averageResinAge = TimeSpan.FromMilliseconds(intervalSum.TotalMilliseconds / intervalList.Count());
                        settings.AverageResinAge = Convert.ToInt32(averageResinAge.TotalDays / 7);
                    }
                }
                return this.PartialView("PerformanceSettings", settings);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Methods
    }
}