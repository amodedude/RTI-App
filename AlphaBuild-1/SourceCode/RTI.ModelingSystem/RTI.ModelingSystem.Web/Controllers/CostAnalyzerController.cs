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
        public JsonResult PlotCostAnalyzerChart(double acidPrice = 0.0, double causticPrice = 0.0, int acidUsage = 0, int causticUsage = 0, int cationResin = 0, int anionResin = 0, bool loadOnSettingsUpdate = false)
        {
            try
            {
                PriceData DataToSend = this.Session["Data_ToSend"] != null ? Session["Data_ToSend"] as PriceData : new PriceData();
                string CustId = this.Session["CustomerId"] != null ? Session["CustomerId"].ToString() : string.Empty;

                if (loadOnSettingsUpdate)
                {
                    DataToSend.AcidPrice = acidPrice;
                    DataToSend.CausticPrice = causticPrice;
                    DataToSend.AcidUsage = acidUsage;
                    DataToSend.CausticUsage = causticUsage;
                    DataToSend.AmountCation = cationResin;
                    DataToSend.AmountAnion = anionResin;
                }
                else
                {
                    DataToSend.AcidUsage = 6;
                    DataToSend.CausticUsage = 6;
                    DataToSend.AmountCation = 600;
                    DataToSend.AmountAnion = 600;
                }

                int CurrentTrain = 1;//set the current train in scope
                //if (Session["SelectedTrain"]!=null)
                //{
                //    CurrentTrain = int.Parse(Session["SelectedTrain"].ToString());
                //}
                var data = this.costAnalyzerService.OpenCostWindow(DataToSend, CurrentTrain, CustId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        public ActionResult GetResultsTable(string weekNumber)
        {
            try
            {
                CostAnalyzerResult CostAnalyzerResultData = new CostAnalyzerResult();
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
}