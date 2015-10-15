// -----------------------------------------------------------------------
// <copyright file="CostAnalyzerController.cs" company="RTI">
// RTI
// </copyright>
// <summary>Cost Analyzer Controller</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Web.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Interfaces.Services;
    using RTI.ModelingSystem.Core.Models;

    #endregion

    [HandleError(View = "ErrorView")]
    public class CostAnalyzerController : Controller
    {
        #region Properties
        /// <summary>
        /// customer Repository
        /// </summary>
        private IRepository<customer> customerRepository;

        /// <summary>
        /// modified Customer Repository
        /// </summary>
        private ICustomerRepository modifiedCustRepository;

        /// <summary>
        /// vessel Repository
        /// </summary>
        private IRepository<vessel> vesselRepository;

        /// <summary>
        /// CostAnalyzer Service
        /// </summary>
        private ICostAnalyzerService costAnalyzerService;

        #endregion Properties

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="custRepo"></param>
        /// <param name="modifiedCustRepository"></param>
        /// <param name="vesselRepo"></param>
        public CostAnalyzerController(IRepository<customer> custRepo, ICustomerRepository modifiedCustRepository, IRepository<vessel> vesselRepo, ICostAnalyzerService costAnalyzerService)
        {
            this.customerRepository = custRepo;
            this.modifiedCustRepository = modifiedCustRepository;
            this.vesselRepository = vesselRepo;
            this.costAnalyzerService = costAnalyzerService;
        }

        public CostAnalyzerController()
        {

        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// CostAnalyzer
        /// </summary>        
        /// <returns></returns>
        public ActionResult CostAnalyzer()
        {
            long customerId = 0;
            if (this.Session["CustomerId"] != null)
            {
                customerId = Convert.ToInt64(this.Session["CustomerId"]);
            }
            customer CustDetails = new customer();
            SystemSummaryViewModel SSVMDetails = new SystemSummaryViewModel();
            customer_water CustWaterDetails = null;
            try
            {
                //If valid customer id does not exist in the session
                if (customerId != 0)
                {
                    var Customer = this.customerRepository.GetAll();
                    CustDetails = Customer.Where(x => x.customerID == customerId).FirstOrDefault();
                    SSVMDetails = new SystemSummaryViewModel();
                    SSVMDetails.CustomerDetails = CustDetails;
                    CustWaterDetails = this.modifiedCustRepository.GetWaterSourceIds(customerId);
                }

                if (CustWaterDetails != null)
                {
                    if (CustWaterDetails.first_sourceID != 0)
                    {
                        SSVMDetails.WaterSourceOne = this.modifiedCustRepository.GetWaterSourceDetails(CustWaterDetails.first_sourceID);//Get first watersource details
                        if (SSVMDetails.WaterSourceOne != null)
                        {
                            SSVMDetails.WaterSourceOne.full_site_name = SSVMDetails.WaterSourceOne.full_site_name.Replace("@", string.Empty);//Remove @ symbol from the watersource name
                        }
                    }
                    else
                    {
                        SSVMDetails.WaterSourceOne = null;
                    }
                    if (CustWaterDetails.second_sourceID != 0)
                    {
                        SSVMDetails.WaterSourceTwo = this.modifiedCustRepository.GetWaterSourceDetails(CustWaterDetails.second_sourceID);//Get second watersource details
                        if (SSVMDetails.WaterSourceTwo != null)
                        {
                            SSVMDetails.WaterSourceTwo.full_site_name = SSVMDetails.WaterSourceTwo.full_site_name.Replace("@ ", string.Empty);//Remove @ symbol from the watersource name
                        }
                    }
                    else
                    {
                        SSVMDetails.WaterSourceTwo = null;
                    }
                    SSVMDetails.Trains = this.modifiedCustRepository.GetCustomerTrains(customerId);//Get list of cutomer train details
                }
                else
                {
                    this.Session["IsNewCustomer"] = "True";
                    this.Session["HasTrainDetails"] = "Check";
                }
                SSVMDetails.CustomerType = Convert.ToString(this.Session["IsNewCustomer"]);
                SSVMDetails.HasTrainDetails = Convert.ToString(this.Session["HasTrainDetails"]);
                if (Convert.ToString(this.Session["HasTrainDetails"]) == "Verify")
                {
                    var count = (this.vesselRepository.GetAll().Where(p => p.vessel_customerID == (customerId).ToString())).Count();
                    if (count != 0)
                    {
                        SSVMDetails.HasTrainDetails = "Yes";
                    }
                    else
                    {
                        SSVMDetails.HasTrainDetails = "No";
                    }
                }
            }
            catch
            {
                throw;
            }
            return this.View(SSVMDetails);
        }

        /// <summary>
        /// Gets the Cost Settings
        /// </summary>
        /// <returns>Returns the view</returns>
        public ActionResult GetCostSettings()
        {
            string customerId = Convert.ToString(this.Session["CustomerId"]);
            string selectedTrain = Convert.ToString(this.Session["SelectedTrain"]);

            CostSettings settings = null;

            try
            {
                settings = costAnalyzerService.GetCostSettings(customerId, selectedTrain);
                if (selectedTrain == "0") {
                    settings.SelectedTrain = "All";
                }
                else {
                    settings.SelectedTrain = Convert.ToString(Session["TrainNumber"]);
                }
                
            }
            catch
            {
                throw;
            }

            return this.PartialView("CostSettings", settings);
        }

        /// <summary>
        /// Plots the CostAnalyzer Chart
        /// </summary>
        /// <returns>Returns the view</returns>
        public JsonResult PlotCostAnalyzerChart(double? acidPrice = null, double? causticPrice = null, int? acidUsage = null, int? causticUsage = null, int? acidPercent = null, int? causticPercent = null, int? cationResin = null, int? anionResin = null, int? cationCleanPrice = null, int? anionCleanPrice = null, int? cationDiscount = null, int? anionDiscount = null, double? cationReplacePrice = null, double? anionReplacePrice = null, bool loadOnSettingsUpdate = false)
        {
            try
            {                
                PriceData DataToSend = this.Session["Data_ToSend"] != null ? Session["Data_ToSend"] as PriceData : new PriceData();
                // Get the Customer Id
                string CustId = this.Session["CustomerId"] != null ? Session["CustomerId"].ToString() : string.Empty;
                Int64 ID = Convert.ToInt64(CustId);
                bool isFirstLoad = true;
                if (loadOnSettingsUpdate)
                {
                    isFirstLoad = false;
                }

                int CurrentTrain = 1;//set the current train in scope
                if (Session["SelectedTrain"]!=null)
                {
                    CurrentTrain = int.Parse(Session["SelectedTrain"].ToString());
                }

                List<train> trains = new List<train>();
                List<vessel> vessels = new List<vessel>();
                customer customer = new customer();
                customer = this.customerRepository.GetAll().Where(cus => cus.customerID == ID).First();

                //Get the list of trains and vessels 
                if (CurrentTrain == 0)
                {
                    trains = this.modifiedCustRepository.GetCustomerTrains(ID);
                    vessels = this.vesselRepository.GetAll().Where(ves => ves.vessel_customerID == CustId).ToList();
                }
                else
                {
                    trains = trains = this.modifiedCustRepository.GetCustomerTrains(ID).Where(train => train.trainID == CurrentTrain).ToList();
                    vessels = this.vesselRepository.GetAll().Where(ves => (ves.vessel_customerID == CustId) && (ves.train_trainID == CurrentTrain)).ToList();
                }

                List<int> acidUsages = new List<int>();
                List<int> causticUsages = new List<int>();
                List<int> cationSizes = new List<int>();
                List<int> anionSizes = new List<int>();
                List<double> cationReplacementPrices = new List<double>(); 
                List<double> anionReplacementPrices = new List<double>(); 

                // Set Data
                int count = 0;
                foreach(vessel vessel in vessels)
                {
                    if(count % 2 != 0) // If vessel is an anion vessel
                    {
                        causticUsages.Add(Convert.ToInt32(vessel.lbs_chemical));
                        anionSizes.Add(Convert.ToInt32(vessel.size));
                        anionReplacementPrices.Add(Convert.ToDouble(vessel.price_per_cuft));
                        
                    }
                    else
                    {
                        acidUsages.Add(Convert.ToInt32(vessel.lbs_chemical));
                        cationSizes.Add(Convert.ToInt32(vessel.size));
                        cationReplacementPrices.Add(Convert.ToDouble(vessel.price_per_cuft));
                    }
                        count++;
                }

                // Get price data from the databse if not passed into function or use the default values
                double _priceAcid = acidPrice == null ? Convert.ToDouble(customer.acid_price) : (double)acidPrice;
                double _priceCaustic = causticPrice == null ? Convert.ToDouble(customer.caustic_price) : (double)causticPrice;
                int _uasgeAcid = acidUsage == null ? Convert.ToInt32(Math.Round(acidUsages.Average())) : (int)acidUsage;
                int _usageCaustic = causticUsage == null ? Convert.ToInt32(Math.Round(causticUsages.Average())) : (int)causticUsage;
                int _percentAcid = acidPercent == null ? 100 : (int)acidPercent;           // Acid Concentration (default 100%)
                int _percentCaustic = causticPercent == null ? 100 : (int)causticPercent;  // Caustic Concentration (default 100%)
                int _sizeCation = cationResin == null ? Convert.ToInt32(cationSizes.Sum()) : (int)cationResin;
                int _sizeAnion = anionResin == null ? Convert.ToInt32(anionSizes.Sum()) : (int)anionResin;
                int _priceCleanCation = cationCleanPrice == null ? 32 : (int)cationCleanPrice;  // $32 per cuft default cation cleaning price
                int _priceCleanAnion = anionCleanPrice == null ? 52 : (int)anionCleanPrice;     // $52 per cuft default anion cleaning price
                int _discounCation = cationDiscount == null ? 0 : (int)cationDiscount;
                int _discountAnion = anionDiscount == null ? 0 : (int)anionDiscount;
                double _priceReplaceCation = cationReplacePrice == null ? Convert.ToInt32(Math.Round(cationReplacementPrices.Average())) : (double)cationReplacePrice;
                double _priceReplaceAnion = anionReplacePrice == null ? Convert.ToInt32(Math.Round(anionReplacementPrices.Average())) : (double)anionReplacePrice;

                DataToSend.AcidPrice = _priceAcid;
                DataToSend.CausticPrice = _priceCaustic;
                DataToSend.AcidUsage = _uasgeAcid;
                DataToSend.CausticUsage = _usageCaustic;
                DataToSend.AmountAnion = _sizeAnion;
                DataToSend.AmountCation = _sizeCation;
                DataToSend.cleaningPriceAnion = _priceCleanAnion;
                DataToSend.cleaningPriceCation = _priceCleanCation;
                DataToSend.cationDiscountPercent = _discounCation;
                DataToSend.anionDiscountPercent = _discountAnion;
                DataToSend.replacePirceAnion = _priceReplaceAnion;
                DataToSend.replacePriceCation = _priceReplaceCation;
                DataToSend.acidConcentratoin = _percentAcid;
                DataToSend.causticConcentration = _percentCaustic;

                
                
                var data = this.costAnalyzerService.OpenCostWindow(DataToSend, CurrentTrain, CustId, isFirstLoad);

                int numWksMeetingDemand_Cleaning = 0;
                int numWksMeetingDemand_NotCleaning = 0;
                double demand = Convert.ToDouble(customer.demand); // Test at 38 million gal
                double numTrains = Convert.ToDouble(customer.num_trains); // Test with customer having 3 trains


                List<ThroughputRegenPair> regenTP_PairClean = new List<ThroughputRegenPair>();
                List<ThroughputRegenPair> regenTP_PairNoClean = new List<ThroughputRegenPair>();

                // Merge the number of regens per week and the throughput values
                for (int week = 0; week <= DataToSend.RegensPerWeekClean.Count()-1; week++)
                {
                    ThroughputRegenPair weekRegTpPair = new ThroughputRegenPair();
                    weekRegTpPair.throughput = week <= DataToSend.CleanThroughput.Count()-1 ? DataToSend.CleanThroughput.ElementAt(week).Value.Item2 : 0;
                    weekRegTpPair.numberOfRegens = week <= DataToSend.RegensPerWeekClean.Count() - 1 ? DataToSend.RegensPerWeekClean.ElementAt(week).Item2 : 0;
                    regenTP_PairClean.Add(weekRegTpPair);
                }

                // Merge the number of regens per week and the throughput values
                for (int week = 0; week <= DataToSend.RegensPerWeekNormalOps.Count()-1; week++)
                {
                    ThroughputRegenPair weekRegTpPair = new ThroughputRegenPair();
                    weekRegTpPair.throughput = week <= DataToSend.NormalOpsThroughput.Count() - 1 ? DataToSend.NormalOpsThroughput.ElementAt(week).Value.Item2 : 0;
                    weekRegTpPair.numberOfRegens = week <= DataToSend.RegensPerWeekNormalOps.Count() - 1 ? DataToSend.RegensPerWeekNormalOps.ElementAt(week).Item2 : 0;
                    regenTP_PairNoClean.Add(weekRegTpPair);
                }


                    // Check how many weeks meet the total demand before RTI cleaning 
                foreach (var week in regenTP_PairClean)
                    {
                        double effectiveDemand = week.throughput * 4.34524 * numTrains * week.numberOfRegens;
                            if (effectiveDemand >= demand)
                            {
                                numWksMeetingDemand_Cleaning++;
                            }
                    }

                // Check how many weeks meet the toal demand after RTI cleaning 
                foreach (var week in regenTP_PairNoClean)
                {
                    double effectiveDemand = week.throughput * 4.34524 * numTrains * week.numberOfRegens;
                        if (effectiveDemand >= demand)
                        {
                            numWksMeetingDemand_NotCleaning++;
                        }
                }

                DataToSend.numDaysMeetingDemand_NormalOps = numWksMeetingDemand_NotCleaning;
                DataToSend.numDaysMeetingDemand_Clean = numWksMeetingDemand_Cleaning;

                Tuple<int, double, double> daysMeetingDemand_Before = new Tuple<int, double, double>(numWksMeetingDemand_NotCleaning, 0,0);
                Tuple<int, double, double> daysMeetingDemand_After = new Tuple<int, double, double>(numWksMeetingDemand_Cleaning, 0, 0);
                data.Add(daysMeetingDemand_Before);
                data.Add(daysMeetingDemand_After);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        public ActionResult GetResultsTable(string weekNumber, bool isClick)
        {
            try
            {
                CostAnalyzerResult CostAnalyzerResultData = new CostAnalyzerResult();
                CostAnalyzerResultData.LoadWeeklyBreakDown = isClick;
                if (weekNumber != null)
                {
                    //modelIndex = Math.Round(e.HitTestResult.Index + 1, 0);
                    double?[] Cond_SS_TP = new double?[6];
                    Cond_SS_TP = this.costAnalyzerService.SelectedWeekDataFinder(long.Parse(weekNumber));

                    // Set display content 
                    CostAnalyzerResultData.WeekNumber = weekNumber;

                    if (Cond_SS_TP != null && Cond_SS_TP.Count() != 0)
                    {
                        if (Cond_SS_TP[0] != null)
                        {
                            CostAnalyzerResultData.Conductivity = Cond_SS_TP[0].ToString();
                            if (Cond_SS_TP[0] > Cond_SS_TP[5])
                            {
                                CostAnalyzerResultData.ConductivityColor = "RED";
                            }
                            else
                            {
                                CostAnalyzerResultData.ConductivityColor = "BLACK";
                            }
                        }
                        else
                        {
                            CostAnalyzerResultData.Conductivity = "N/A";
                        }
                        if (Cond_SS_TP[1] != null)
                        {
                            CostAnalyzerResultData.SaltSplitBefore = Cond_SS_TP[1].ToString();
                        }
                        else
                        {
                            CostAnalyzerResultData.SaltSplitBefore = "N/A";
                        }
                        if (Cond_SS_TP[2] != null)
                        {
                            CostAnalyzerResultData.SaltSplitAfter = Cond_SS_TP[2].ToString();
                        }
                        else
                        {
                            CostAnalyzerResultData.SaltSplitAfter = "N/A";
                        }
                        if (Cond_SS_TP[3] != null)
                        {
                            CostAnalyzerResultData.ThroughputBefore = string.Format("{0:n0}", Cond_SS_TP[3]);
                        }
                        else
                        {
                            CostAnalyzerResultData.ThroughputBefore = "N/A";
                        }
                        if (Cond_SS_TP[4] != null)
                        {
                            CostAnalyzerResultData.ThroughputAfter = string.Format("{0:n0}", Cond_SS_TP[4]);
                        }
                        else
                        {
                            CostAnalyzerResultData.ThroughputAfter = "N/A";
                        }
                    }
                }
                    CostAnalyzerResult CostAnalyzerResultDataNew = this.costAnalyzerService.GetCostAnalyzerResultsData();
                    if (CostAnalyzerResultDataNew != null)
                    {
                        CostAnalyzerResultData.TotalWeeklyCostBefore = CostAnalyzerResultDataNew.TotalWeeklyCostBefore;
                        CostAnalyzerResultData.TotalWeeklyCostAfter = CostAnalyzerResultDataNew.TotalWeeklyCostAfter;

                        CostAnalyzerResultData.RegenWeeklyCostBefore = CostAnalyzerResultDataNew.RegenWeeklyCostBefore;
                        CostAnalyzerResultData.RegenWeeklyCostAfter = CostAnalyzerResultDataNew.RegenWeeklyCostAfter;

                        CostAnalyzerResultData.CleaningCostBefore = CostAnalyzerResultDataNew.CleaningCostBefore;
                        CostAnalyzerResultData.CleaningCostAfter = CostAnalyzerResultDataNew.CleaningCostAfter;

                        CostAnalyzerResultData.ReplacementCostBefore = CostAnalyzerResultDataNew.ReplacementCostBefore;
                        CostAnalyzerResultData.ReplacementCostAfter = CostAnalyzerResultDataNew.ReplacementCostAfter;

                        CostAnalyzerResultData.TotalOpsCostBefore = CostAnalyzerResultDataNew.TotalOpsCostBefore;
                        CostAnalyzerResultData.TotalOpsCostAfter = CostAnalyzerResultDataNew.TotalOpsCostAfter;

                        CostAnalyzerResultData.AvgCostPerGalBefore = CostAnalyzerResultDataNew.AvgCostPerGalBefore;
                        CostAnalyzerResultData.AvgCostPerGalAfter = CostAnalyzerResultDataNew.AvgCostPerGalAfter;
                    }
                

                return this.PartialView("_CostAnalyzerResultsTable", CostAnalyzerResultData);

            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// GetCumulativeSavings
        /// </summary>        
        public ActionResult GetCumulativeSavings()
        {
            List<string> CumulativeSavingsData = this.costAnalyzerService.GetCumulativeSavings();
            if (CumulativeSavingsData != null && CumulativeSavingsData.Count > 0)
            {
                Session["CumulativeSavings"] = CumulativeSavingsData[0];
                Session["ROI"] = CumulativeSavingsData[1];
            }
            return this.PartialView("_CumulativeSavings");
        }

        #endregion Methods
    }

    public class ThroughputRegenPair{
        public double throughput { get; set; }
        public double numberOfRegens { get; set; }
    }
}
