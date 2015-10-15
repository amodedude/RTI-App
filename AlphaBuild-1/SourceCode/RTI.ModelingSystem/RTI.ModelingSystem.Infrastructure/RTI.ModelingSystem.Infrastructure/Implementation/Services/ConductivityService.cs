// -----------------------------------------------------------------------
// <copyright file="ConductivityService.cs" company="RTI">
// RTI
// </copyright>
// <summary>Conductivity Service</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Services
{
	#region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Interfaces.Services;
    using RTI.ModelingSystem.Core.Models;

	#endregion Usings

	/// <summary>
	/// Conductivity Service class
	/// </summary>
	public class ConductivityService : IConductivityService
	{
		#region Properties

		/// <summary>
		/// modified CondRepository
		/// </summary>
		private IRepository<source> modifiedCondRepository;

		/// <summary>
		/// modified waterdataRepository
		/// </summary>
		private IRepository<water_data> modifiedwaterdataRepository;

		/// <summary>
		/// modified waterdataRepository
		/// </summary>
		private IConductivityRepository modifiedConductivityRepository;

		#endregion Properties

		#region Constructor

		/// <summary>
		/// ConductivityService constructor
		/// </summary>
		/// <param name="sourceRepository">sourceRepository parameter</param>
		/// <param name="waterdataRepository">waterdataRepository parameter</param>
		public ConductivityService(IRepository<source> sourceRepository, IRepository<water_data> waterdataRepository, IConductivityRepository CondRepository)
		{
			this.modifiedCondRepository = sourceRepository;
			this.modifiedwaterdataRepository = waterdataRepository;
			this.modifiedConductivityRepository = CondRepository;
		}

		#endregion Constructor

		#region Methods

		/// <summary>
		/// Gets the Conductivity data
		/// </summary>
		/// <param name="usgsId">USGSID parameter</param>
		/// <returns>Returns water data</returns>
		public water_data GetConductivitydata(long usgsId)
		{
			try
			{
				var data = this.modifiedwaterdataRepository.GetById(usgsId.ToString());
				return data;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Calculates Statistics
		/// </summary>
		/// <param name="USGSID">USGSID parameter</param>
		/// <returns>Returns conductivity statastics</returns>
		public ConductivityStatistics CalculateStatistics(int sourceId)
		{
			try
			{
				ConductivityStatistics CondStats = new ConductivityStatistics();
				long USGSID = this.modifiedConductivityRepository.GetAgentId(sourceId);
				var AllData = this.modifiedwaterdataRepository.GetAll();
				var data = from r in AllData where r.sourceID == USGSID select r;
				int NumberOfDataPoints = data.Count();
				double SumOfSourceOneData = 0, Mean, sum_X_minus_μ_squared = 0, StdDev;
				double sourceMax, sourceMin, sourceMedian, sourceMode, currentCond, percentAboveMean;
                double[] condDataToDouble = new double[NumberOfDataPoints];
                double[] X_minus_μ_squared = new double[NumberOfDataPoints];
				foreach (var element in data)
				{
					SumOfSourceOneData = SumOfSourceOneData + Convert.ToDouble(element.cond);
				}
				Mean = SumOfSourceOneData / NumberOfDataPoints;
				int i = 0;
				foreach (var element in data)
				{
					X_minus_μ_squared[i] = (Convert.ToInt32(element.cond) - Mean) * (Convert.ToInt32(element.cond) - Mean);
					condDataToDouble[i] = Convert.ToDouble(element.cond);
					i++;
				}
				for (int j = 0; j < X_minus_μ_squared.Count(); j++)
				{
					sum_X_minus_μ_squared = sum_X_minus_μ_squared + X_minus_μ_squared[j];
				}
				sum_X_minus_μ_squared = sum_X_minus_μ_squared / NumberOfDataPoints;
				StdDev = Math.Sqrt(sum_X_minus_μ_squared);
				sourceMax = condDataToDouble.Length > 0 ? Math.Round(condDataToDouble.Max(), 2) : 0;
				sourceMin = condDataToDouble.Length > 0 ? Math.Round(condDataToDouble.Min(), 2) : 0;

                // Sort Conductivity Data in Asscending Order:
                var condSorted = condDataToDouble.OrderBy(n => n);
                int halfIndex = NumberOfDataPoints/2; 

                // Calculate the Median Properly:
                if((NumberOfDataPoints % 2) == 0){ // Even Data Set Length
                    sourceMedian = (condSorted.ElementAt(halfIndex) + condSorted.ElementAt(halfIndex + 1)) / 2; // Average the Two Middle Points
                }
                else{ // Odd Data Set Length
                    sourceMedian = condSorted.ElementAt(halfIndex); // Just get the middle point of the data set
                }


				//sourceMedian = Math.Round(((sourceMax + sourceMin) / 2), 2); // This is not the median value!
               	
			
                // Calculate the Modal Value
                Dictionary<double, double> counts = new Dictionary<double, double>();
				foreach (double cond_value in condDataToDouble)
				{
                    if (counts.ContainsKey(cond_value))
                    {
                        counts[cond_value] = counts[cond_value] + 1;
                    }
                    else
                    {
                        counts[cond_value] = 1;
                    }
				}
				sourceMode = double.MinValue; // Get the lowest value a double can represent as not to interfere with finding the max value. 
				double maxVal = double.MinValue;


                foreach (double key in counts.Keys)
                {
                    if (counts[key] > maxVal)
                    {
                        maxVal = counts[key];
                        sourceMode = key;
                    }
                }

				currentCond = condDataToDouble.Length > 0 ? condDataToDouble.Last() : 0;
				percentAboveMean = Mean != double.NaN ? ((currentCond - Mean) / Mean) * 100 : 0;
				CondStats.ArithmeticMean = Mean != double.NaN ? Math.Round(Mean, 2).ToString() : "0";
				CondStats.CurrentConductivity = currentCond.ToString();
				CondStats.MaxConductivity = sourceMax.ToString();
				CondStats.MinConductivity = sourceMin.ToString();
				CondStats.MedianValue = sourceMedian.ToString();
				CondStats.ModalValue = sourceMode.ToString();
				CondStats.PercentAboveMean = Math.Round(percentAboveMean).ToString();
				CondStats.StandardDeviation = Math.Round(StdDev, 2).ToString();
				CondStats.SourceId = USGSID.ToString();
				return CondStats;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Gets Forecast data
		/// </summary>
		/// <param name="usgsId">USGSID parameter</param>
		/// <returns>Returns forecast data</returns>
		public ForecastData GetForecastdata(long usgsId)
		{
			try
			{
				ForecastData Fd = new ForecastData();
				var AllData = modifiedwaterdataRepository.GetAll();
				var data = (from r in AllData where r.sourceID == usgsId select r).ToList();				
				DateTime currentDate = DateTime.Now;
				DateTime firstDate = data.Count > 0 ? DateTime.Parse(data[0].measurment_date, new System.Globalization.CultureInfo("en-US", true)) : new DateTime();
				var numberOfYears = Convert.ToInt32(currentDate.Year) - Convert.ToInt32(firstDate.Year);
				DateTime forcastEndDate = currentDate.AddDays(182);
                DateTime forcastStartDate = currentDate;
                //TimeSpan interval = data.Count > 0 ? (DateTime.Parse(data[2].measurment_date, new System.Globalization.CultureInfo("en-US", true)) - DateTime.Parse(data[1].measurment_date, new System.Globalization.CultureInfo("en-US", true))) : new TimeSpan();
                TimeSpan interval = TimeSpan.FromDays(1);  // Always calculate forecast on a DAILY span interval no matter what!!!
                Dictionary<DateTime, int>[] forcastRawData = new Dictionary<DateTime, int>[numberOfYears];
				Dictionary<DateTime, double> forcastDataAverage = new Dictionary<DateTime, double>();
				Dictionary<DateTime, int> forcastDataMax = new Dictionary<DateTime, int>();
				for (int i = 1; i <= numberOfYears; i++)
				{

					var dateTime = forcastStartDate;
					forcastRawData[i - 1] = new Dictionary<DateTime, int>();
					foreach (var element in data)
					{
						var conductivity = element.cond;
						TimeSpan span = TimeSpan.Zero;
						if ((DateTime.Parse(element.measurment_date, new System.Globalization.CultureInfo("en-US", true)) >= forcastStartDate.AddYears(-1 * i)) && (DateTime.Parse(element.measurment_date, new System.Globalization.CultureInfo("en-US", true)) <= forcastEndDate.AddYears(-1 * i)))
						{
							forcastRawData[i - 1].Add(dateTime, Convert.ToInt32(conductivity));
							span = span + interval;
							dateTime = dateTime.Add(span);
						}
					}

					if (i == numberOfYears)
					{
						forcastDataAverage = forcastRawData.SelectMany(d => d).GroupBy(kvp => kvp.Key).ToDictionary(g => g.Key, g => g.Average(kvp => kvp.Value));
						forcastDataMax = forcastRawData.SelectMany(d => d).GroupBy(kvp => kvp.Key).ToDictionary(g => g.Key, g => g.Max(kvp => kvp.Value));
					}
				}

				foreach (var entry in forcastDataAverage)
				{
					Fd.AverageForecastData.Add(new water_data() { measurment_date = entry.Key.ToString(), cond = Convert.ToInt32(entry.Value) });
				}
				foreach (var entry in forcastDataMax)
				{
					Fd.MaximumForecastData.Add(new water_data() { measurment_date = entry.Key.ToString(), cond = Convert.ToInt32(entry.Value) });
				}
				return Fd;
			}
			catch
			{
				throw;
			}
		}

		#endregion Methods
	}
}
