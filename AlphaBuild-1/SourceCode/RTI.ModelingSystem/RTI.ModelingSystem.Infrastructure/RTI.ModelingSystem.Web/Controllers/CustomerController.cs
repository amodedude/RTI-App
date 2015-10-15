// -----------------------------------------------------------------------
// <copyright file="CustomerController.cs" company="RTI">
// RTI
// </copyright>
// <summary>Customer Controller</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Web.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;

    #endregion Usings

    /// <summary>
    /// CustomerController class
    /// </summary>
    [HandleError(View = "ErrorView")]
    public class CustomerController : Controller
    {
        #region Properties

        /// <summary>
        /// customer Repository
        /// </summary>
        private IRepository<customer> customerRepository;

        /// <summary>
        /// modified CustRepository
        /// </summary>
        private ICustomerRepository modifiedCustRepository;

        /// <summary>
        /// source Repository
        /// </summary>
        private IRepository<source> sourceRepository;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="custRepo"></param>
        /// <param name="modifiedCustRepository"></param>
        /// <param name="SourceRepo"></param>
        public CustomerController(IRepository<customer> custRepo, ICustomerRepository modifiedCustRepository, IRepository<source> SourceRepo)
        {
            this.customerRepository = custRepo;
            this.sourceRepository = SourceRepo;
            this.modifiedCustRepository = modifiedCustRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Index view
        /// </summary>
        /// <returns>Returns the view</returns>
        public ActionResult Index()
        {
            try
            {
                var customersList = this.customerRepository.GetAll();
                return this.View(customersList);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Details of the customer
        /// </summary>
        /// <param name="id">Customer identifier</param>
        /// <returns>Returns the json result</returns>
        public JsonResult Details(string id)
        {
            try
            {
                customer customer = this.modifiedCustRepository.FindById(id);
                return this.Json(customer, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Create the customer
        /// </summary>
        /// <param name="customerId">customer Id</param>
        /// <returns>Returns the view</returns>
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Create(long? customerId)
        {
            try
            {
                customer Cust = new customer()
                {
                    customerList = new List<SelectListItem>(),
                    StateList = new List<SelectListItem>(),
                    TrainList = new List<SelectListItem>(),
                    CityList = new List<SelectListItem>()
                };
                if (customerId != null && customerId != -1)
                {
                    Cust = this.modifiedCustRepository.FindById(Convert.ToString(customerId));

                }
                List<customer> c = new List<customer>();
                List<source> s = new List<source>();
				c = this.customerRepository.GetAll().OrderBy(item => item.name).ToList();
                s = this.modifiedCustRepository.GetStateList();
                Cust.customerList.Add(new SelectListItem() { Text = "New Customer..", Value = "-1" });
                foreach (var item in c)
                {
                    Cust.customerList.Add(new SelectListItem() { Text = item.name, Value = Convert.ToString(item.customerID) });
                }

				var createCustomerData = s.OrderBy(item => item.state_name).GroupBy(g => new { g.state_name, g.state }).Select(g => g.FirstOrDefault()).ToList();

                foreach (var item in createCustomerData)
                {
                    Cust.StateList.Add(new SelectListItem() { Text = item.state_name, Value = item.state_name });
                }
                for (int i = 1; i <= 20; i++)
                {
                    Cust.TrainList.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                }
                if (customerId != null && customerId != -1)
                {
					foreach (var item in s.Where(x => x.state_name == Cust.state).OrderBy(item => item.city)
                        .GroupBy(g => new { 
                            g.city 
                        })
                        .Select(g => g.FirstOrDefault()).ToList())
                    {
                        Cust.CityList.Add(new SelectListItem() { Text = item.city, Value = item.city });
                    }
                    this.Session["IsDuplicate"] = "False";
                    return this.PartialView("_Edit", Cust);
                }
                else
                {
                    if (Convert.ToString(this.Session["IsDuplicate"]) == "True")
                    {
                        ModelState.AddModelError("notes", "Customer Already exists");
                    }
                    this.Session["IsDuplicate"] = "False";
                    return this.PartialView("_Create", Cust);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Creates the customer
        /// </summary>
        /// <param name="Customer">Customer parameter</param>
        /// <returns>Returns the view</returns>
        [HttpPost]
        [ValidateInput(false)]
        [OutputCache(Duration = 1, VaryByParam = "none")]
        public ActionResult Create(customer Customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Customer.customerID == -1)
                    {
                        long Id = this.customerRepository.InsertAndGetID(Customer);
                        this.Session["CustomerId"] = Id;
                        this.Session["IsNewCustomer"] = "True";
                        this.Session["IsDuplicate"] = "False";
                        this.Session["HasTrainDetails"] = "Check";
                        return RedirectToAction("Dashboard", "ClientDatabase");
                    }
                    else
                    {
                        this.modifiedCustRepository.UpdateCustomer(Customer);
                        this.Session["CustomerId"] = Customer.customerID;
                        this.Session["IsNewCustomer"] = "False";
                        this.Session["IsDuplicate"] = "False";
                        this.Session["HasTrainDetails"] = "Verify";
                        return RedirectToAction("Dashboard", "ClientDatabase", new { id = Customer.customerID });
                    }
                }
                else
                {
                    this.Session["IsDuplicate"] = "True";
                    return RedirectToAction("Dashboard", "ClientDatabase");
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Edits the customer
        /// </summary>
        /// <param name="id">Customer identifier</param>
        /// <returns>Returns the view</returns>
        public ActionResult Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                List<customer> c = new List<customer>();
                customer cust = this.modifiedCustRepository.FindById(id);
                c = this.customerRepository.GetAll().ToList();
                if (cust == null)
                {
                    return HttpNotFound();
                }
                cust.CityList.Add(new SelectListItem() { Text = "--Select--", Value = "-1" });
                cust.customerList.Add(new SelectListItem() { Text = "New Customer..", Value = "0" });
                foreach (var item in c)
                {
                    cust.customerList.Add(new SelectListItem() { Text = item.name, Value = Convert.ToString(item.customerID) });
                }

                return this.View(cust);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Edits the customer
        /// </summary>
        /// <param name="customer">customer parameter</param>
        /// <returns>Returns the view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this.modifiedCustRepository.UpdateCustomer(customer);
                    return RedirectToAction("Index");
                }
                return this.View(customer);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes the customer
        /// </summary>
        /// <param name="id">Customer identifier</param>
        /// <returns>Returns the view</returns>
        public ActionResult Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                customer customer = this.modifiedCustRepository.FindById(id);
                if (customer == null)
                {
                    return HttpNotFound();
                }
                return this.View(customer);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the City List
        /// </summary>
        /// <param name="State">State parameter</param>
        /// <returns>Returns the json result</returns>
        [HttpGet]
        public JsonResult GetCityList(string State)
        {
            try
            {
                List<SelectListItem> CityList = new List<SelectListItem>();
                List<source> z = this.modifiedCustRepository.GetStateList()
                    .Where(x => x.state_name == State)
					.OrderBy(item => item.city)
                    .GroupBy(g => new { g.city })
                    .Select(g => g.FirstOrDefault()).ToList();
                foreach (var item in z)
                {
                    CityList.Add(new SelectListItem() { Text = item.city, Value = item.city });
                }
                return this.Json(new SelectList(CityList, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        public bool CheckForDuplicate(string name, string state, string city)
        {
            try
            {
                customer customer = new customer();
                customer.name = name;
                customer.city = city;
                customer.state = state;
                return this.modifiedCustRepository.CheckForDuplicates(customer);
            }
            catch
            {
                throw;
            }
        }

        #endregion Methods
    }
}
