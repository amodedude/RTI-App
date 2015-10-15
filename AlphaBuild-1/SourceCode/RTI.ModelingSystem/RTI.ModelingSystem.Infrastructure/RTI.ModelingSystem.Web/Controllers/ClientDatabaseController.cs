// -----------------------------------------------------------------------
// <copyright file="ClientDatabaseController.cs" company="RTI">
// RTI
// </copyright>
// <summary>Client Database Controller</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Web.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Transactions;
    using System.Web.Mvc;
    using DotNet.Highcharts;
    using DotNet.Highcharts.Enums;
    using DotNet.Highcharts.Helpers;
    using DotNet.Highcharts.Options;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Models;
    using RTI.ModelingSystem.Core.DBModels;

    #endregion Usings

    [HandleError(View = "ErrorView")]
    public class ClientDatabaseController : Controller
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
        /// source Repository
        /// </summary>
        private IRepository<source> sourceRepository;

        /// <summary>
        /// train Repository
        /// </summary>
        private IRepository<train> trainRepository;

        /// <summary>
        /// modified Train Repository
        /// </summary>
        private ITrainRepository modifiedTrainRepository;

        /// <summary>
        /// modified Customer Water Repository
        /// </summary>
        private IRepository<customer_water> modifiedCustWaterRepository;

        /// <summary>
        /// modified Conductivity Repository
        /// </summary>
        private IConductivityRepository modifiedCondRepository;

        /// <summary>
        /// modified Vessel Repository
        /// </summary>
        private IVesselRepository modifiedVesselRepository;

        /// <summary>
        /// vessel Repository
        /// </summary>
        private IRepository<vessel> vesselRepository;

        /// <summary>
        /// modified Resin Products Repository
        /// </summary>
        private IResinProductsRepository modifiedResinProductsRepository;

        #endregion Properties

        #region Constructor

        public ClientDatabaseController(IConductivityRepository CondRepo, IRepository<customer> custRepo, ICustomerRepository modifiedCustRepository, IRepository<source> sourceRepository, IRepository<train> trainRepo,
            IRepository<customer_water> custWaterRepo, IVesselRepository modifiedVesselRepository, IRepository<vessel> vesselRepo, ITrainRepository modifiedTrainRepository, IResinProductsRepository modifiedResinProductsRepository)
        {
            this.customerRepository = custRepo;
            this.modifiedCustRepository = modifiedCustRepository;
            this.sourceRepository = sourceRepository;
            this.trainRepository = trainRepo;
            this.modifiedCustWaterRepository = custWaterRepo;
            this.modifiedCondRepository = CondRepo;
            this.modifiedVesselRepository = modifiedVesselRepository;
            this.vesselRepository = vesselRepo;
            this.modifiedTrainRepository = modifiedTrainRepository;
            this.modifiedResinProductsRepository = modifiedResinProductsRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// DashBoard Action
        /// </summary>
        /// <returns>Returns the view</returns>
        public ActionResult DashBoard()
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
                if (Convert.ToString(this.Session["IsDuplicate"]) == "True")
                {
                    SSVMDetails.CustomerType = "Duplicate";
                }
                else
                {
                    SSVMDetails.CustomerType = Convert.ToString(this.Session["IsNewCustomer"]);

                }
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
        /// Gets the System Settings
        /// </summary>
        /// <returns>Returns the view</returns>
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetSystemSettings()
        {
            SystemSettings settings = new SystemSettings();
            try
            {
                string customerId = this.Session["CustomerId"].ToString();
                customer customer = this.modifiedCustRepository.FindById(customerId);
                var train = this.trainRepository.GetAll().Where(p => p.customer_customerID == customer.customerID).FirstOrDefault();
                if (train != null)
                {
                    if (train.using_manifold.Equals("NO"))
                    {
                        settings.isManifold = false;
                    }
                    else
                    {
                        settings.isManifold = true;
                    }
                    this.Session["IsNewCustomer"] = "False";
                }
                else
                {
                    this.Session["IsNewCustomer"] = "True";
                }
                settings.acid_price = customer.acid_price;
                settings.caustic_price = customer.caustic_price;
                settings.demand = Convert.ToString(customer.demand);
                var sources = this.sourceRepository.GetAll().OrderBy(item => item.full_site_name);
                foreach (var item in sources)
                {
                    if (item.state_name == customer.state && item.city == customer.city)
                    {
                        settings.WaterSourceList1.Add(new SelectListItem() { Text = item.full_site_name, Value = Convert.ToString(item.sources_sourceID) });
                    }
                }
                foreach (var item in sources)
                {
                    if (item.state_name == customer.state && item.city == customer.city)
                    {
                        settings.WaterSourceList2.Add(new SelectListItem() { Text = item.full_site_name, Value = Convert.ToString(item.sources_sourceID) });
                    }
                }
                var allCustWaters = this.modifiedCustWaterRepository.GetAll();
                var custWaters = allCustWaters.Where(p => p.customer_customerID == customer.customerID).FirstOrDefault();
                source firstWaterSource = null;
                source secondWaterSource = null;

                if (custWaters != null)
                {
                    if (custWaters.first_sourceID != 0)
                    {
                        firstWaterSource = sources.Where(p => p.sources_sourceID == custWaters.first_sourceID && p.state_name == customer.state).FirstOrDefault();
                        if (firstWaterSource != null)
                        {
                            settings.firstWaterSource = Convert.ToString(firstWaterSource.sources_sourceID);
                        }
                        else
                        {
                            settings.firstWaterSource = string.Empty;
                        }
                    }
                    if (custWaters.second_sourceID != 0)
                    {
                        secondWaterSource = sources.Where(p => p.sources_sourceID == custWaters.second_sourceID && p.state_name == customer.state).FirstOrDefault();
                        if (secondWaterSource != null)
                        {
                            settings.secondWaterSource = Convert.ToString(secondWaterSource.sources_sourceID);
                        }
                        else
                        {
                            settings.secondWaterSource = string.Empty;
                        }
                    }
                    settings.firstWSPercentage = custWaters.firstSourcePercentage;
                }
                if (firstWaterSource != null && secondWaterSource != null)
                {
                    settings.HasTwoSources = true;
                }
            }
            catch
            {
                throw;
            }
            return this.PartialView("_SystemSettings", settings);
        }

        /// <summary>
        /// Train Settings Action
        /// </summary>
        /// <param name="Data">Data of the train</param>
        /// <returns>Returns the view</returns>
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult TrainSettings(string Data)
        {
            try
            {
                int CustomerId = 0;
                if (this.Session["CustomerId"] != null)
                {
                    CustomerId = Convert.ToInt16(this.Session["CustomerId"]);
                }
                List<train> trains = this.trainRepository.GetAll().Where(p => p.customer_customerID == CustomerId).ToList();
                Trainsettings trainDetails = new Trainsettings();
                var customer = this.customerRepository.GetById(CustomerId);
                int NoofTrains = Convert.ToInt16(customer.num_trains);
                var trainData = this.modifiedCondRepository.GetTrains(CustomerId);

                if (trainData != 0)
                {
                    long trId = 0;
                    if (Data == null)
                    {
                        trId = this.modifiedCondRepository.GetTrainIdByCsutomer(CustomerId);
                    }
                    else
                    {
                        trId = Convert.ToInt16(Data);
                    }
                    var train = this.modifiedCondRepository.GetTrainDetailsByCustomerIdandTrainId(trId, CustomerId);
                    if (trains.Count == 0)
                    {
                        for (int i = 0; i < NoofTrains; i++)
                        {
                            trainDetails.TrianNoList.Add(new SelectListItem() { Text = "Train " + (i + 1), Value = "0" });
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trains.Count; i++)
                        {
                            trainDetails.TrianNoList.Add(new SelectListItem() { Text = "Train " + (i + 1), Value = trains[i].trainID.ToString() });
                        }
                    }
                    trainDetails.Train = train;
                    if (train != null && train.num_beds_cation == 2)
                    {
                        trainDetails.cation = true;
                    }
                    if (train != null && train.num_beds_anion == 2)
                    {
                        trainDetails.anion = true;
                    }
                    if (train != null && train.using_manifold == "NO")
                    {
                        trainDetails.manifold = false;
                    }
                    else if (train != null && train.using_manifold == "YES")
                    {
                        trainDetails.manifold = true;
                    }
                    List<vessel> lstVessels = new List<vessel>();
                    List<int> VesselNumbers = new List<int>();

                    if (Data == null)
                    {
                        lstVessels = this.modifiedVesselRepository.FetchVesselsList(Convert.ToInt16(trId));
                        trainDetails.VesslsList = lstVessels;
                        for (int i = 0; i < trainDetails.VesslsList.Count; i++)
                        {

                            var resin = this.modifiedVesselRepository.GetResinById(lstVessels[i].resin_data_product_id);
                            if (resin != null)
                            {
                                lstVessels[i].ResinModel = resin.model_number;
                                lstVessels[i].ResinData = resin.primary_type + " " + resin.manufacturer + "(" + resin.model_number + ")";
                            }
                            if (i == 2 || i == 3)
                            {
                                VesselNumbers.Add(Convert.ToInt32(lstVessels[i].vessel_number));
                            }

                            if (lstVessels[i].with_degassifier == "1")
                            {
                                lstVessels[i].Degasifier = true;
                            }
                            if (lstVessels[i].with_polisher == "1")
                            {
                                lstVessels[i].Polisher = true;
                            }
                            if (trainDetails.manifold == false) // Set number of regens to "N/A" if the user is not using a manifold 
                            {
                                lstVessels[i].num_regens = "N/A";
                            }
                        }
                        if (VesselNumbers.Count == 1)
                        {
                            lstVessels.Add(new vessel() { vessel_number = VesselNumbers[0] == 3 ? 4 : 3 });
                            lstVessels = lstVessels.OrderBy(x => x.vessel_number).ToList();
                            trainDetails.VesslsList = lstVessels;
                        }
                        else if (VesselNumbers.Count == 2)
                        {
                            lstVessels = lstVessels.OrderBy(x => x.vessel_number).ToList();
                            trainDetails.VesslsList = lstVessels;
                        }
                        else
                        {
                            for (int i = trainDetails.VesslsList.Count; i < 4; i++)
                            {

                                lstVessels.Add(new vessel());

                            }
                        }

                        return this.PartialView("_TrainSettings", trainDetails);
                    }
                    else // This happens when the user switches between trains!!!
                    {
                        lstVessels = this.modifiedVesselRepository.FetchVesselsList(Convert.ToInt16(trId));
                        trainDetails.VesslsList = lstVessels;
                        int i = 0;
                        if (lstVessels != null)
                        {
                            foreach (var Vessel in lstVessels)
                            {
                                if (i == 2 || i == 3)
                                {
                                    VesselNumbers.Add(Convert.ToInt32(Vessel.vessel_number));
                                }
                                var resin = this.modifiedVesselRepository.GetResinById(Vessel.resin_data_product_id);
                                Vessel.ResinModel = resin.model_number;
                                Vessel.ResinData = resin.primary_type + " " + resin.manufacturer + "(" + resin.model_number + ")";
                                if (lstVessels[i].with_degassifier == "1")
                                {
                                    lstVessels[i].Degasifier = true;
                                }
                                if (lstVessels[i].with_polisher == "1")
                                {
                                    lstVessels[i].Polisher = true;
                                }
                                i = i + 1;
                            }
                        }
                        if (VesselNumbers.Count == 1)
                        {
                            lstVessels.Add(new vessel() { vessel_number = VesselNumbers[0] == 3 ? 4 : 3 });
                            lstVessels = lstVessels.OrderBy(x => x.vessel_number).ToList();
                            trainDetails.VesslsList = lstVessels;
                        }
                        else if (VesselNumbers.Count == 2)
                        {
                            lstVessels = lstVessels.OrderBy(x => x.vessel_number).ToList();
                            trainDetails.VesslsList = lstVessels;
                        }
                        else if (trainDetails.VesslsList != null)
                        {
                            for (int j = trainDetails.VesslsList.Count; j < 4; j++)
                            {
                                lstVessels.Add(new vessel());
                            }
                        }
                        return this.PartialView("_bedsettings", trainDetails);
                    }
                }
                else
                {
                    trainDetails.Train = new train();
                    trainDetails.cation = false;
                    trainDetails.anion = false;
                    trainDetails.Train.using_manifold = "NO";
                    trainDetails.manifold = false;
                    for (int i = 0; i < NoofTrains; i++)
                    {
                        trainDetails.TrianNoList.Add(new SelectListItem() { Text = "Train " + (i + 1), Value = "0" });
                    }
                    List<vessel> lstVessels = new List<vessel>();
                    for (int i = 0; i < 4; i++)
                    {
                        lstVessels.Add(new vessel());
                    }
                    trainDetails.VesslsList = lstVessels;
                    return this.PartialView("_TrainSettings", trainDetails);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Train Settings parameter
        /// </summary>
        /// <param name="trainSettings">train Settings</param>
        /// <returns>Returns the view</returns>
        [HttpPost]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [ValidateAntiForgeryToken]
        public ActionResult TrainSettings(Trainsettings trainSettings)
        {
            train train = new train();
            try
            {
                train = ProcessTrainSettings(trainSettings, train);
            }
            catch
            {
                throw;
            }

            return RedirectToAction("DashBoard");
        }

        [HttpPost]
        public string TrainSettingsWithoutRedirect(Trainsettings trainSettings)
        {
            train train = new train();
            try
            {
                train = ProcessTrainSettings(trainSettings, train);
                return "true";
            }
            catch
            {
                return "false";
                throw;
            }
        }

        /// <summary>
        /// Processes the train settings.
        /// </summary>
        /// <param name="trainSettings">The train settings.</param>
        /// <param name="train">The train.</param>
        /// <returns></returns>
        private train ProcessTrainSettings(Trainsettings trainSettings, train train)
        {
            List<vessel> lstVessels = new List<vessel>();
            train = trainSettings.Train;
            train.customer_customerID = Convert.ToInt32(this.Session["CustomerId"]);
            if (train.using_manifold == null)
            {
                train.using_manifold = "NO";
            }
            var TrainInDB = this.modifiedCondRepository.GetTrainDetailsByCustomerIdandTrainId(train.trainID, train.customer_customerID);
            if (TrainInDB != null)
            {
                this.modifiedTrainRepository.UpdateTrain(train);
            }
            else
            {
                this.trainRepository.Insert(train);
            }
            this.trainRepository.SubmitChanges();
            List<vessel> vessels = null;
            if (trainSettings.Train != null && trainSettings.Train.trainID != 0)
            {
                vessels = this.modifiedVesselRepository.FetchVesselsList(trainSettings.Train.trainID);
            }
            if (vessels == null || vessels.Count == 0)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    int vesselNumber = 0;
                    foreach (vessel vessel in trainSettings.VesslsList)
                    {
                        try
                        {
                            vesselNumber = vesselNumber + 1;
                            if (vessel.UpdateVessel == "True")
                            {
                                vessel.with_degassifier = vessel.Degasifier == true ? "1" : "0";
                                vessel.with_polisher = vessel.Polisher == true ? "1" : "0";
                                if (vesselNumber > 0 && vesselNumber < 5)
                                {
                                    vessel.vessel_number = vesselNumber;
                                    if (vesselNumber == 1 || vesselNumber == 3)
                                    {
                                        vessel.bed_number = "1";
                                    }
                                    else
                                    {
                                        vessel.bed_number = "2";
                                    }
                                }
                                vessel.vesselID = this.modifiedVesselRepository.GetNewVesselId();
                                vessel.vessel_customerID = this.Session["CustomerId"].ToString();
                                vessel.train_trainID = train.trainID;
                                int resinDataProductId = this.modifiedVesselRepository.GetResinId(vessel.ResinModel);
                                vessel.resin_data_product_id = resinDataProductId;

                                if (vessel.num_regens == null)
                                {
                                    vessel.num_regens = "00";
                                }

                                this.modifiedVesselRepository.InsertVessel(vessel);
                                this.vesselRepository.SubmitChanges();
                            }
                            else if (vessel.UpdateVessel == "False" && !string.IsNullOrEmpty(vessel.name))
                            {
                                this.modifiedVesselRepository.DeleteVessel(vessel);
                            }
                        }
                        catch
                        {
                            throw;

                        }
                    }
                    scope.Complete();
                }
                this.Session["HasTrainDetails"] = "Yes";
            }
            else
            {
                train = trainSettings.Train;
                if (train.customer_customerID == 0)
                {
                    train.customer_customerID = Convert.ToInt32(this.Session["CustomerId"]);
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    int vesselNumber = 0;
                    lstVessels = this.modifiedVesselRepository.FetchVesselsList(Convert.ToInt16(train.trainID));
                    int vesselcount = 0;
                    foreach (vessel vessel in trainSettings.VesslsList)  // This Updates the DB's Vessel Data with updated view information entered in by the user
                    {
                        vesselNumber = vesselNumber + 1;
                        if (vessel.UpdateVessel == "True")
                        {
                            vessel.with_degassifier = vessel.Degasifier == true ? "1" : "0";
                            vessel.with_polisher = vessel.Polisher == true ? "1" : "0";
                            vessel.throughput = vessel.throughput == null ? "0" : vessel.throughput;
                            int resinDataProductId = this.modifiedVesselRepository.GetResinId(vessel.ResinModel);
                            vessel.resin_data_product_id = resinDataProductId;
                            if (vesselNumber > 0 && vesselNumber < 5)
                            {
                                vessel.vessel_number = vesselNumber;
                                if (vesselNumber == 1 || vesselNumber == 2)
                                {
                                    vessel.bed_number = "1";
                                }
                                else
                                {
                                    vessel.bed_number = "2";
                                }
                            }
                            vessel.vessel_customerID = this.Session["CustomerId"].ToString();
                            vessel.train_trainID = train.trainID;

                            // Ensure that all num-regens data is populated 
                            if(train.using_manifold == "NO")
                            {
                                vessel.num_regens = (lstVessels.ElementAt(vesselcount).num_regens) != null ? lstVessels.ElementAt(vesselcount).num_regens : "00";  // If we are not using a manifold store the previous manifold data (if it exists, otherwise store 00)
                                vessel.throughput = (lstVessels.ElementAt(vesselcount).throughput) != null ? lstVessels.ElementAt(vesselcount).throughput : "00";  // If we are not using a manifold store the previous manifold data (if it exists, otherwise store 00)
                            }
                            else if (vessel.num_regens == null || vessel.throughput == null)
                            {
                                vessel.num_regens = "000";
                                vessel.throughput = "000";
                            }


                            try
                            {
                                this.modifiedVesselRepository.UpdateVessel(vessel);//Delete vessel if its not being updated to DB
                            }
                            catch
                            {
                                throw;
                            }
                        }
                        else if (vessel.UpdateVessel == "False" && !string.IsNullOrEmpty(vessel.name))
                        {
                            this.modifiedVesselRepository.DeleteVessel(vessel);
                        }
                        vesselcount++;
                    }
                    scope.Complete();
                }
            }
            return train;
        }

        /// <summary>
        /// Updates the System Settings
        /// </summary>
        /// <param name="systemSettings">system settings</param>
        /// <returns>Returns the view</returns>
        [HttpPost]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult UpdateSystemSettings(SystemSettings systemSettings)
        {
            try
            {
                if (systemSettings != null && !string.IsNullOrEmpty(systemSettings.demand))
                {
                    systemSettings.demand = systemSettings.demand.Replace(",", "");
                }

                if (this.Session["IsNewCustomer"].ToString() == "True")
                {
                    this.modifiedCondRepository.AddSystemSettings(systemSettings, this.Session["CustomerId"].ToString());
                    this.Session["HasTrainDetails"] = "No";
                    return RedirectToAction("DashBoard");
                }
                else
                {
                    this.modifiedCondRepository.UpdateSystemSettings(systemSettings, this.Session["CustomerId"].ToString());
                    this.Session["HasTrainDetails"] = "Verify";
                    return RedirectToAction("DashBoard");
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Auto complete
        /// </summary>
        /// <param name="term">keyword of the product</param>
        /// <returns>Returns the json result</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Autocomplete(string term)
        {
            try
            {
                var result = new List<KeyValuePair<string, string>>();
                var res = this.modifiedTrainRepository.ResinProductModelsData(term);
                foreach (var item in res)
                {
                    result.Add(new KeyValuePair<string, string>(item.model_number + "(" + item.resin_product_id + ")", (item.primary_type + " " + item.manufacturer + " (" + item.model_number) + ")"));
                }
                var result3 = result.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
                return this.Json(result3, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the Resin Lookup Modal
        /// </summary>
        /// <param name="bedInfo">bedInfo parameter</param>
        /// <param name="productName">Product Name</param>
        /// <param name="searchText">Search Text</param>
        /// <returns>Returns the view</returns>
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetResinLookupModal(string bedInfo, string productName, string searchText)
        {
            try
            {
                return this.PartialView("PartialResinLookupModal", this.BuildResinLookupModel(bedInfo, productName, searchText));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Builds the resin lookup model
        /// </summary>
        /// <param name="bedInfo">bedInfo parameter</param>
        /// <param name="productName">Product Name</param>
        /// <param name="searchText">Search Text</param>
        /// <returns>Returns the resin model</returns>
        private ResinModel BuildResinLookupModel(string bedInfo, string productName, string searchText)
        {
            try
            {
                ResinModel resinModel = new ResinModel();
                List<string> resinProductNames = new List<string>();
                List<resin_products> resinProducts = new List<resin_products>();
                resinProductNames = GetAllResinProductNames();
                resinProducts = GetAllResinProductDetails(bedInfo);
                if (resinProductNames != null && resinProducts != null)
                {
                    resinProducts = productName == string.Empty ? resinProducts : resinProducts.Where(item => item.name == productName).ToList();
                    resinProducts = searchText == string.Empty ? resinProducts : resinProducts.Where(item => item.model_number.ToLower().Contains(searchText.ToLower())).ToList();
                    resinModel.ProductNamesList = resinProductNames;
                    resinModel.ResinProductsList = resinProducts;
                    resinModel.SelectedProduct = productName;
                }
                return resinModel;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the search filter result for resin lookup model
        /// </summary>
        /// <param name="bedInfo">bedInfo parameter</param>
        /// <param name="productName">Product Name</param>
        /// <param name="searchText">Search Text</param>
        /// <returns>Returns the json result</returns>
        public JsonResult ResinLookupSearchFilter(string bedInfo, string productName, string searchText)
        {
            try
            {
                return this.Json(this.BuildResinLookupModel(bedInfo, productName, searchText), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets All the Resin Product Names
        /// </summary>
        /// <returns>Returns the list of product name</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public List<string> GetAllResinProductNames()
        {
            try
            {
                List<string> result = new List<string>();
                result = this.modifiedResinProductsRepository.GetAllResinProductNames();
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets All the Resin Product Details
        /// </summary>
        /// <param name="BedType">Bed Type parameter</param>
        /// <returns>Returns the list of resin products</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public List<resin_products> GetAllResinProductDetails(string BedType)
        {
            try
            {
                var result = this.modifiedResinProductsRepository.GetAllResinProductDetails();
                for (int i = 0; i < result.Count; i++)
                {
                    result[i].color = BedType;

                    if (!string.IsNullOrEmpty(result[i].price_per_cuft))
                    {
                        result[i].price_per_cuft = string.Concat("$", Convert.ToDecimal(result[i].price_per_cuft.Replace("$", "")).ToString("0.00"));
                    }
                    else
                    {
                        result[i].price_per_cuft = "$0.00";
                    }
                    result[i].physical_structure = result[i].resin_product_id.ToString().PadLeft(3, '0');
                    result[i].teir = result[i].primary_type + ", " + result[i].manufacturer + " (" + result[i].name + ")";
                    result[i].group = result[i].model_number + "(" + result[i].physical_structure + ")";
                }
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets All the Resin Product Details By Product Name
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns>Returns the view</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetAllResinProductDetailsByProductName(string productName)
        {
            try
            {
                var result = new List<resin_products>();
                string product_name = productName.Trim();
                result = this.modifiedResinProductsRepository.GetAllResinProductDetailsByProductName(product_name);
                for (int i = 0; i < result.Count; i++)
                {
                    if (!string.IsNullOrEmpty(result[i].price_per_cuft))
                    {
                        result[i].price_per_cuft = string.Concat("$", Convert.ToDecimal(result[i].price_per_cuft.Replace("$", "")).ToString("0.00"));
                    }
                    else
                    {
                        result[i].price_per_cuft = "$0.00";
                    }

                    result[i].physical_structure = result[i].resin_product_id.ToString().PadLeft(3, '0');
                    result[i].teir = result[i].primary_type + ", " + result[i].manufacturer + " (" + result[i].name + ")";
                }
                ResinModel resinModel = new ResinModel();
                List<string> resinProductNames = new List<string>();
                List<resin_products> resinProducts = new List<resin_products>();
                resinProductNames = GetAllResinProductNames();
                resinProducts = result;
                if (resinProductNames != null && resinProducts != null)
                {
                    resinModel.ProductNamesList = resinProductNames;
                    resinModel.ResinProductsList = resinProducts;
                }
                return this.PartialView("PartialResinLookupModal_Child", resinModel);
            }
            catch
            {
                throw;
            }
        }

        public ActionResult Errorview()
        {
            return View("Error");
        }

        public string IsResinModelValid(string resinModelNumber)
        {
            int resinDataProductId = this.modifiedVesselRepository.GetResinId(resinModelNumber);
            if(resinDataProductId==-1)
            {
                return "False";
            }
            else
            {
                return "True";
            }
        }

        #endregion Methods
    }
}