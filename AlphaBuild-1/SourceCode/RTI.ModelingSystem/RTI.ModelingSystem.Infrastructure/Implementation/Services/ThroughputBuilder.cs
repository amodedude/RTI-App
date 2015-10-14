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
		public Dictionary<DateTime, Tuple<int, double, string>> ThroughputBuild(double replacementLevel, double rticleaningLevel, double startingSaltSplit, Dictionary<int, double> randomGrains, double amountOfResin, double cleaningEffectivness, double trainGPM, double ageOfResin, double newResinSaltSplit, double lifeExpectancy, bool isCleaning, bool isReplacing)
		{
			try
			{
				Dictionary<DateTime, Tuple<int, double, string>> tpPrediction = new Dictionary<DateTime, Tuple<int, double, string>>();
				TOCBuilder toc = new TOCBuilder();
				PreviousWeek lastWeeks = new PreviousWeek();
				DateTime date = new DateTime();
				date = DateTime.Now.Date;
				double saltSplitLimit = newResinSaltSplit * .999;
				double effectiveResinAge = ageOfResin;
				double origionalCleaningLevel = replacementLevel;
				foreach (var week in randomGrains)
				{
					if (week.Key == randomGrains.FirstOrDefault().Key)
					{
						Tuple<int, double, string> weekTpPair = CalculateFirstWeek(toc, isCleaning, startingSaltSplit, amountOfResin, week, trainGPM, ageOfResin, lastWeeks);
						tpPrediction.Add(date, weekTpPair);
						lastWeeks.Date = date;
					}
					else if (isCleaning)
					{
						date = lastWeeks.Date.AddDays(7);
						effectiveResinAge += 1;
						if (lastWeeks.SaltSplit > rticleaningLevel)
						{
							Tuple<int, double, string> weekTpPair = NormalOpsWeek(isCleaning, trainGPM, lifeExpectancy, toc, effectiveResinAge, amountOfResin, week, randomGrains, lastWeeks, newResinSaltSplit);
							tpPrediction.Add(date, weekTpPair);
							lastWeeks.Date = date;
						}
						else if ((lastWeeks.SaltSplit > replacementLevel) && (lastWeeks.SaltSplit < rticleaningLevel))
						{
							effectiveResinAge = (effectiveResinAge - ageOfResin) * (1 - (cleaningEffectivness * 0.01));
							Tuple<int, double, string> weekTpPair = CleanOpsWeek(toc, effectiveResinAge, amountOfResin, week, cleaningEffectivness, newResinSaltSplit, lastWeeks);
							rticleaningLevel = PostCleaningSaltSPlitReduction(rticleaningLevel, replacementLevel);
							tpPrediction.Add(date, weekTpPair);
							lastWeeks.Date = date;
						}
						else
						{
							if (isReplacing)
							{
								effectiveResinAge = 1;
								Tuple<int, double, string> weekTpPair = ReplaceOpsWeek(isCleaning, trainGPM, lifeExpectancy, toc, effectiveResinAge, newResinSaltSplit, amountOfResin, week, lastWeeks);
								rticleaningLevel = origionalCleaningLevel;
								tpPrediction.Add(date, weekTpPair);
								lastWeeks.Date = date;
							}
							else
							{
								Tuple<int, double, string> weekTpPair = NormalOpsWeek(isCleaning, trainGPM, lifeExpectancy, toc, effectiveResinAge, amountOfResin, week, randomGrains, lastWeeks, newResinSaltSplit);
								tpPrediction.Add(date, weekTpPair);
								lastWeeks.Date = date;
							}
						}
					}
					else
					{
						date = lastWeeks.Date.AddDays(7);
						effectiveResinAge += 1;
						if (lastWeeks.SaltSplit > replacementLevel || isReplacing)
						{
							Tuple<int, double, string> weekTpPair = NormalOpsWeek(isCleaning, trainGPM, lifeExpectancy, toc, effectiveResinAge, amountOfResin, week, randomGrains, lastWeeks, newResinSaltSplit);
							tpPrediction.Add(date, weekTpPair);
							lastWeeks.Date = date;
						}
						else
						{
							effectiveResinAge = 1;
							Tuple<int, double, string> weekTpPair = ReplaceOpsWeek(isCleaning, trainGPM, lifeExpectancy, toc, effectiveResinAge, newResinSaltSplit, amountOfResin, week, lastWeeks);
							tpPrediction.Add(date, weekTpPair);
							lastWeeks.Date = date;
						}
					}
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
		private Tuple<int, double, string> CalculateFirstWeek(TOCBuilder toc, bool isCleaning, double startingSaltSplit, double amountOfResin, KeyValuePair<int, double> grains, double trainGPM, double ageOfResin, PreviousWeek lastWeeks)
		{
			try
			{
				lastWeeks.SaltSplit = startingSaltSplit;
				double throughput = ((startingSaltSplit * amountOfResin) / ((grains.Value) * 1000)) * 1000000;
				double silicaFactor = RandomNumberGenerator.Between(97, 100);
				Tuple<int, double, string> weekTpPair = new Tuple<int, double, string>(grains.Key, throughput, "Normal");
				if (throughput == 0)
				{
					lastWeeks.HoursPerRun = 0.0001 * (silicaFactor * 0.01);
				}
				else
				{
					lastWeeks.HoursPerRun = ((throughput / trainGPM) / 60) * (silicaFactor * 0.01);
				}
				double regenerationTime = toc.RegenTimeCurve(ageOfResin) / 60;
				double totalTime = regenerationTime + lastWeeks.HoursPerRun;
				if (throughput == 0)
				{
					lastWeeks.RegPerWeek = 0;
				}
				else
				{
					lastWeeks.RegPerWeek = (24 * 7) / totalTime;
				}
				Tuple<double, double, double, double, double, double> regenData = new Tuple<double, double, double, double, double, double>(grains.Key, lastWeeks.RegPerWeek, lastWeeks.HoursPerRun, regenerationTime, throughput, lastWeeks.SaltSplit);
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
		private Tuple<int, double, string> NormalOpsWeek(bool isCleaning, double trainGPM, double lifeExpectancy, TOCBuilder toc, double effectiveResinAge, double amountOfResin, KeyValuePair<int, double> grains, Dictionary<int, double> randomGrains, PreviousWeek lastWeeks, double newResinSS)
		{
			try
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
				lastWeeks.SaltSplit = saltSplit;
				double regenerationTime = toc.RegenTimeCurve(effectiveResinAge) / 60;
				double throughput = ((saltSplit * amountOfResin) / (grains.Value * 1000)) * 1000000;
				if (throughput <= 0 && lastWeeks != null && lastWeeks.TpData != null)
				{
					throughput = lastWeeks.TpData.Item2;
				}
				Tuple<int, double, string> weekTpPair = new Tuple<int, double, string>(grains.Key, throughput, "Normal");
				lastWeeks.TpData = weekTpPair;
				double silicaFactor = RandomNumberGenerator.Between(97, 100);
				if (throughput == 0)
				{
					lastWeeks.HoursPerRun = 0.0001 * (silicaFactor * 0.01);
				}
				else
				{
					lastWeeks.HoursPerRun = ((throughput / trainGPM) / 60) * (silicaFactor * 0.01);
				}
				double totalTime = regenerationTime + lastWeeks.HoursPerRun;
				if (throughput == 0)
				{
					lastWeeks.RegPerWeek = 0;
				}
				else
				{
					lastWeeks.RegPerWeek = (24 * 7) / totalTime;
				}
				Tuple<double, double, double, double, double, double> regenData = new Tuple<double, double, double, double, double, double>(grains.Key, lastWeeks.RegPerWeek, lastWeeks.HoursPerRun, regenerationTime, throughput, lastWeeks.SaltSplit);
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
		private Tuple<int, double, string> CleanOpsWeek(TOCBuilder toc, double effectiveResinAge, double amountOfResin, KeyValuePair<int, double> grains, double cleaningEffectivness, double newResinSaltSplit, PreviousWeek lastWeeks)
		{
			try
			{
				double saltSplit = lastWeeks.SaltSplit + (lastWeeks.SaltSplit * cleaningEffectivness * 0.01);
				if (saltSplit > newResinSaltSplit)
				{
					saltSplit = newResinSaltSplit;
				}
				double postCleaningSaltSplit = saltSplit * (cleaningEffectivness * 0.01);
				double percentIncrease = (saltSplit - lastWeeks.SaltSplit) / lastWeeks.SaltSplit;
				effectiveResinAge = effectiveResinAge - (effectiveResinAge * percentIncrease);
				double regenerationTime = toc.RegenTimeCurve(effectiveResinAge) / 60;
				lastWeeks.SaltSplit = saltSplit;
				double throughput = ((saltSplit * amountOfResin) / (grains.Value * 1000)) * 1000000;
				Tuple<int, double, string> weekTpPair = new Tuple<int, double, string>(grains.Key, throughput, "Clean");
				lastWeeks.TpData = weekTpPair;
				Tuple<double, double, double, double, double, double> regenData = new Tuple<double, double, double, double, double, double>(grains.Key, lastWeeks.RegPerWeek, lastWeeks.HoursPerRun, regenerationTime, throughput, lastWeeks.SaltSplit);
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
		private Tuple<int, double, string> ReplaceOpsWeek(bool isCleaning, double trainGpm, double lifeExpectancy, TOCBuilder toc, double effectiveResinAge, double newResinSaltSplit, double amountOfResin, KeyValuePair<int, double> grains, PreviousWeek lastWeeks)
		{
			try
			{
				lifeExpectancy = 0;
				double regenerationTime = toc.RegenTimeCurve(lifeExpectancy) / 60;
				double saltSplit = newResinSaltSplit;
				lastWeeks.SaltSplit = saltSplit;
				double throughput = ((saltSplit * amountOfResin) / (grains.Value * 1000)) * 1000000;
				Tuple<int, double, string> weekTpPair = new Tuple<int, double, string>(grains.Key, throughput, "Replace");
				lastWeeks.TpData = weekTpPair;
				double silicaFactor = RandomNumberGenerator.Between(97, 100);
				if (throughput == 0)
				{
					lastWeeks.HoursPerRun = 0.0001 * (silicaFactor * 0.01);
				}
				else
				{
					lastWeeks.HoursPerRun = ((throughput / trainGpm) / 60) * (silicaFactor * 0.01);
				}
				double totalTime = regenerationTime + lastWeeks.HoursPerRun;

				// Calculate the number of regenerations in this week
				if (throughput == 0)
				{
					lastWeeks.RegPerWeek = 0;
				}
				else
				{
					lastWeeks.RegPerWeek = (24 * 7) / totalTime;
				}
				Tuple<double, double, double, double, double, double> regenData = new Tuple<double, double, double, double, double, double>(grains.Key, lastWeeks.RegPerWeek, lastWeeks.HoursPerRun, regenerationTime, throughput, lastWeeks.SaltSplit);
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
		/// Post Cleaning SaltSPlit Reduction
		/// </summary>
		/// <param name="cleaningSaltSplit">cleaning SaltSplit</param>
		/// <param name="replaceSaltSplit">replace SaltSplit</param>
		/// <returns>Returns the salt split reduction</returns>
		private double PostCleaningSaltSPlitReduction(double cleaningSaltSplit, double replaceSaltSplit)
		{
			try
			{
				double afterReductionSS = cleaningSaltSplit - (cleaningSaltSplit * .20);
				if (afterReductionSS >= replaceSaltSplit)
				{
					cleaningSaltSplit = afterReductionSS;
				}
				return cleaningSaltSplit;
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
