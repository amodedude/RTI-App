// -----------------------------------------------------------------------
// <copyright file="ConductivityController.cs" company="RTI">
// RTI
// </copyright>
// <summary>Conductivity Controller</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Web.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using DotNet.Highcharts.Helpers;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Interfaces.Services;
    using RTI.ModelingSystem.Core.Models;

    #endregion

    /// <summary>
    /// ConductivityController class
    /// </summary>
    [HandleError(View = "ErrorView")]
    public class ConductivityController : Controller
    {
        #region Properties

        /// <summary>
        /// modified Conductivity Repository
        /// </summary>
        private IConductivityRepository modifiedCondRepository;

        /// <summary>
        /// conductivity Service
        /// </summary>
        private IConductivityService conductivityService;

        /// <summary>
        /// customer Repository
        /// </summary>
        private IRepository<customer> customerRepository;

        /// <summary>
        /// modified Customer Repository
        /// </summary>
        private ICustomerRepository modifiedCustRepository;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// ConductivityController controller
        /// </summary>
        /// <param name="modifiedCondRepository">modified Conductivity Repository</param>
        /// <param name="CondClient">Conductivity Client</param>
        public ConductivityController(IConductivityRepository modifiedCondRepository, IConductivityService CondClient, IRepository<customer> custRepo, ICustomerRepository modifiedCustRepository)
        {
            this.modifiedCondRepository = modifiedCondRepository;
            this.conductivityService = CondClient;
            this.customerRepository = custRepo;
            this.modifiedCustRepository = modifiedCustRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Index Action
        /// </summary>
        /// <returns>Returns the view</returns>
        public ActionResult Index()
        {
            return this.View();
        }

        public ActionResult WaterConductivity()
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
                            SSVMDetails.Source1Statistics = this.conductivityService.CalculateStatistics(CustWaterDetails.first_sourceID);
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
                            SSVMDetails.Source2Statistics = this.conductivityService.CalculateStatistics(CustWaterDetails.second_sourceID);
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

            }
            catch
            {
                throw;
            }
            return this.View(SSVMDetails);
        }

        /// <summary>
        /// Conductivity Plot
        /// </summary>
        /// <param name="USGSSource1ID">USGS Source1 ID</param>
        /// <returns>Returns the view</returns>
        public JsonResult ConductivityPlot(long USGSID)
        {
            try
            {
                List<water_data> wd = new List<water_data>();
                wd = this.modifiedCondRepository.GetWaterSourceData(USGSID);
                Dictionary<DateTime, int?> ChartData = new Dictionary<DateTime, int?>();
                int[] d = new int[wd.Count];
                DateTime pointstart;
                object[,] chartData = new object[wd.Count, 2];
                int i = 0;
                foreach (var item in wd)
                {
					if (!string.IsNullOrWhiteSpace(item.measurment_date))
					{
						ChartData.Add(Convert.ToDateTime(item.measurment_date, new CultureInfo("en-US", true)), item.cond);
					}
                    d[i] = Convert.ToInt32(item.cond);
					if (!string.IsNullOrWhiteSpace(wd[0].measurment_date) && DateTime.TryParse(wd[0].measurment_date, new CultureInfo("en-US", true), DateTimeStyles.AdjustToUniversal, out pointstart))
                    {
                        pointstart = Convert.ToDateTime(wd[0].measurment_date, new CultureInfo("en-US", true));
                    }
                    chartData.SetValue(item.measurment_date, i, 0);
                    chartData.SetValue(item.cond, i, 1);
                    i++;
                }

                return Json(ChartData.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Forecast Plot
        /// </summary>
        /// <returns>Returns the view</returns>
        public JsonResult ForecastPlot(long USGSID)
        {
            try
            {
                ForecastData Fd = new ForecastData();
                Fd = this.conductivityService.GetForecastdata(USGSID);
                return Json(Fd, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        #endregion Methods
    }
}