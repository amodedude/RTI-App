// -----------------------------------------------------------------------
// <copyright file="VesselRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>Vessel Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Repository
{
    #region Usings

    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Infrastructure.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;


    #endregion Usings

    /// <summary>
    /// VesselRepository class
    /// </summary>
    public class VesselRepository : Repository<vessel>, IVesselRepository
    {
        #region Properties

        /// <summary>
        /// Db Context
        /// </summary>
        private RtiContext bContext;

        #endregion Properties

        #region Constructor

        public VesselRepository(RtiContext datacontext)
            : base(datacontext)
        {
            this.bContext = datacontext;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Fetches the Cation1 Details
        /// </summary>
        /// <param name="trainId">train identifier</param>
        /// <returns>Returns the vessel</returns>
        public vessel FetchCation1Details(int trainId)
        {
            try
            {
                var vessel = bContext.vessels != null ? (from r in bContext.vessels
                                                         where (r.train_trainID == trainId)
                                                         select r).FirstOrDefault()
                                                             : new vessel();
                return vessel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Fetches the Vessels List
        /// </summary>
        /// <param name="trainId">train Identifier</param>
        /// <returns>Returns the list of vessels</returns>
        public List<vessel> FetchVesselsList(long trainId)
        {
            try
            {
                var vesselList = (from r in bContext.vessels
                                  join rp in bContext.resin_products on r.resin_data_product_id equals rp.resin_product_id
                                  where r.train_trainID == trainId
                                  select new
                                  {
                                      vesselID = r.vesselID,
                                      vessel_number = r.vessel_number,
                                      name = r.name,
                                      size = r.size,
                                      bed_number = r.bed_number,
                                      lbs_chemical = r.lbs_chemical,
                                      date_replaced = r.date_replaced,
                                      replacement_plan = r.replacement_plan,
                                      throughput = r.throughput,
                                      num_regens = r.num_regens,
                                      toc = r.toc,
                                      with_degassifier = r.with_degassifier,
                                      with_polisher = r.with_polisher,
                                      vessel_customerID = r.vessel_customerID,
                                      train_trainID = r.train_trainID,
                                      resin_data_product_id = r.resin_data_product_id,
                                      price_per_cuft = r.price_per_cuft,
                                      salt_split_CAP = rp.salt_split_CAP,
                                      Salt_Split = r.Salt_Split
                                  }).ToList();

                List<vessel> VesList = new List<vessel>();
                foreach (var item in vesselList)
                {
                    VesList.Add(new vessel
                    {
                        vesselID = item.vesselID,
                        vessel_number = item.vessel_number,
                        name = item.name,
                        size = item.size,
                        bed_number = item.bed_number,
                        lbs_chemical = item.lbs_chemical,
                        date_replaced = item.date_replaced,
                        replacement_plan = item.replacement_plan,
                        throughput = item.throughput,
                        num_regens = item.num_regens,
                        toc = item.toc,
                        with_degassifier = item.with_degassifier,
                        with_polisher = item.with_polisher,
                        vessel_customerID = item.vessel_customerID,
                        train_trainID = item.train_trainID,
                        resin_data_product_id = item.resin_data_product_id,
                        price_per_cuft = Convert.ToDecimal(item.price_per_cuft),
                        salt_split_CAP = item.salt_split_CAP,
                        Salt_Split = item.Salt_Split
                    });
                }
                return VesList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the New Vessel Id
        /// </summary>
        /// <returns>Returns the new vessel id</returns>
        public int GetNewVesselId()
        {
            return bContext.vessels != null ? (bContext.vessels.OrderByDescending(v => v.vesselID).FirstOrDefault().vesselID) + 1 : 1;
        }

        /// <summary>
        /// Inserts the Vessel
        /// </summary>
        /// <param name="vesselObj">Vessel parameter</param>
        public void InsertVessel(vessel vesselObj)
        {
            try
            {
                vessel NewVessel = new vessel();
                NewVessel.vesselID = vesselObj.vesselID;
                NewVessel.vessel_number = vesselObj.vessel_number;
                NewVessel.price_per_cuft = vesselObj.price_per_cuft;
                NewVessel.name = vesselObj.name;
                NewVessel.size = vesselObj.size;
                NewVessel.bed_number = vesselObj.bed_number;
                NewVessel.lbs_chemical = vesselObj.lbs_chemical;
                NewVessel.date_replaced = vesselObj.date_replaced;
                NewVessel.replacement_plan = vesselObj.replacement_plan;
                if (!string.IsNullOrEmpty(vesselObj.throughput))
                {
                    NewVessel.throughput = vesselObj.throughput;
                }
                else
                {
                    NewVessel.throughput = "0";
                }
                NewVessel.num_regens = vesselObj.num_regens;
                NewVessel.toc = vesselObj.toc;
                NewVessel.with_degassifier = vesselObj.with_degassifier;
                NewVessel.with_polisher = vesselObj.with_polisher;
                NewVessel.vessel_customerID = vesselObj.vessel_customerID;
                NewVessel.train_trainID = vesselObj.train_trainID;
                NewVessel.resin_data_product_id = vesselObj.resin_data_product_id;

                if (!double.IsNaN(vesselObj.Salt_Split))
                {
                    NewVessel.Salt_Split = vesselObj.Salt_Split;
                }
                else
                {
                    NewVessel.Salt_Split = 0;
                }
                if (rtiContext.vessels != null)
                {
                    rtiContext.vessels.Add(NewVessel);
                    rtiContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the ResinId
        /// </summary>
        /// <param name="resinModel">ResinModel parameter</param>
        /// <returns>Returns the resin id</returns>
        public int GetResinId(string resinModel)
        {
            var Resin = (from R in bContext.resin_products
                         where (R.model_number == resinModel)
                         select R).ToList();
            if (Resin.Count() != 0) 
            {
                return (bContext.resin_products != null && bContext.resin_products.Count() > 0) ?
                    (from R in bContext.resin_products
                     where (R.model_number == resinModel)
                     select R).FirstOrDefault().resin_product_id
                     : 0;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Updates the Vessel
        /// </summary>
        /// <param name="vesselObj">Vessel parameter</param>
        public void UpdateVessel(vessel vesselObj)
        {
            try
            {
                var VesselDB = bContext.vessels != null ? (from V in bContext.vessels
                                                           where V.vesselID == vesselObj.vesselID && V.train_trainID == vesselObj.train_trainID
                                                           select V).FirstOrDefault()
                                                               : new vessel();

                if (VesselDB != null && bContext.vessels != null)
                {
                    bContext.vessels.Remove(VesselDB);
                    rtiContext.SaveChanges();
                    bContext.vessels.Add(vesselObj);
                    bContext.SaveChanges();
                }
                else
                {
                    vesselObj.vesselID = GetNewVesselId();
                    InsertVessel(vesselObj);
                    rtiContext.SaveChanges();
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        Console.WriteLine(message);
                        // raise a new exception nesting  
                        // the current instance as InnerException  
                        //raise = new InvalidOperationException(message, raise);
                    }
                }  
                throw;
            }
        }

        /// <summary>
        /// Gets the Resin By Id
        /// </summary>
        /// <param name="resinProductId">Resin product id</param>
        /// <returns>Returns the resin product</returns>
        public resin_products GetResinById(int resinProductId)
        {
            try
            {
                var resinProducts = bContext.resin_products != null ? (from R in bContext.resin_products
                                                                       where (R.resin_product_id == resinProductId)
                                                                       select R).FirstOrDefault()
                                                              : new resin_products();

                return resinProducts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes the vessel.
        /// </summary>
        /// <param name="vesselObj">The vessel object.</param>
        public void DeleteVessel(vessel vesselObj)
        {
            try
            {
                var VesselDB = bContext.vessels != null ? (from V in bContext.vessels
                                                           where V.vesselID == vesselObj.vesselID && V.train_trainID == vesselObj.train_trainID
                                                           select V).FirstOrDefault()
                                                               : new vessel();

                if (VesselDB != null && bContext.vessels != null)
                {
                    bContext.vessels.Remove(VesselDB);
                    rtiContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Methods

    }
}
