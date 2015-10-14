// -----------------------------------------------------------------------
// <copyright file="RandomNumberSimulation.cs" company="RTI">
// RTI
// </copyright>
// <summary>Random Number Simulation</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Services
{
	#region Usings

	using System;
	using System.Collections.Generic;
	using System.Linq;

	#endregion Usings

	/// <summary>
	/// Random Number Simulation class
	/// </summary>
	public class RandomNumberSimulation
	{
		/// <summary>
		/// forcastBase Data
		/// </summary>
		private Dictionary<DateTime, Tuple<int, double, double>> forcastBaseData;

		/// <summary>
		/// random Values
		/// </summary>
		private Dictionary<DateTime, Tuple<int, double>> randomValues;

		/// <summary>
		/// confidence value
		/// </summary>
		private int confidence;

		/// <summary>
		/// number of Iterations
		/// </summary>
		private int numberOfIterations;

		/// <summary>
		/// standard Deviation Interval
		/// </summary>
		private int standardDeviationInterval;

		/// <summary>
		/// random Numbers All Loops
		/// </summary>
		private List<List<double>> randomNumbersAllLoops = new List<List<double>>();

		/// <summary>
		/// randomized Conductivity
		/// </summary>
		public static List<double> randomizedConductivity = new List<double>();

		/// <summary>
		/// Begining Calculation
		/// </summary>
		/// <param name="conductivityAndStandardData">conductivity And Standard Data</param>
		/// <param name="confidenceInterval">confidence Interval</param>
		/// <param name="numberIterations">number Iterations</param>
		/// <param name="calculationMethod">calculation Method</param>
		/// <param name="standardDevThreshold">standardDev Threshold</param>
		/// <param name="resinAge">resin Age</param>
		/// <returns>Returns the calculated value</returns>
		public Dictionary<int, double> BeginCalculation(Dictionary<DateTime, Tuple<int, double, double>> conductivityAndStandardData, int confidenceInterval, int numberIterations, string calculationMethod, int standardDevThreshold, double resinAge)
		{
			try
			{
				Dictionary<int, double> output = new Dictionary<int, double>();
				List<double> grains = new List<double>();
				List<double> conductivity = new List<double>();
				int weeksInService = Convert.ToInt32(Math.Round(resinAge, 0, MidpointRounding.AwayFromZero));
				randomNumbersAllLoops.Clear();
				forcastBaseData = conductivityAndStandardData;
				confidence = confidenceInterval;
				standardDeviationInterval = standardDevThreshold;
				numberOfIterations = numberIterations;
				RandomizeConductivity();
				conductivity = MergeData(calculationMethod);
				grains = Grains(conductivity);
				foreach (var grain in grains)
				{
					output.Add(weeksInService, grain);
					weeksInService++;
				}

				return output;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Randomizes the Conductivity
		/// </summary>
		private void RandomizeConductivity()
		{
			try
			{
				randomValues = new Dictionary<DateTime, Tuple<int, double>>();
				for (int loop = 1; loop <= numberOfIterations; loop++)
				{
					randomValues.Clear();
					foreach (var week in forcastBaseData)
					{
						DateTime beginingOfWeekDate = week.Key;
						double baseConductivity = week.Value.Item3;
						double randomMax = baseConductivity + (week.Value.Item1 * standardDeviationInterval);
						double randomMin = baseConductivity - (week.Value.Item1 * standardDeviationInterval);
						double randomValue = RandomNumberGenerator.Between(randomMin, randomMax);
						Tuple<int, double> randomConductivityValue = new Tuple<int, double>(week.Value.Item1, randomValue);
						randomValues.Add(beginingOfWeekDate, randomConductivityValue);
					}
					List<double> randomNumberPerLoop = new List<double>();
					foreach (var week in randomValues)
					{
						randomNumberPerLoop.Add(week.Value.Item2);
					}
					randomNumbersAllLoops.Add(randomNumberPerLoop);
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Merges the Data
		/// </summary>
		/// <param name="combinationMethod">combination Method</param>
		/// <returns>Returns the data</returns>
		private List<double> MergeData(string combinationMethod)
		{
			try
			{
				randomizedConductivity.Clear();
				if (combinationMethod == "Max")
				{
					randomizedConductivity = randomNumbersAllLoops[0].Select((v, c) => randomNumbersAllLoops.Max(r => r[c])).ToList();
				}
				else if (combinationMethod == "Min")
				{
					randomizedConductivity = randomNumbersAllLoops[0].Select((v, c) => randomNumbersAllLoops.Max(r => r[c])).ToList();
				}
				else if (combinationMethod == "Mean")
				{
					randomizedConductivity = randomNumbersAllLoops[0].Select((v, c) => randomNumbersAllLoops.Average(r => r[c])).ToList();
				}
				else if (combinationMethod == "Mode")
				{
					randomNumbersAllLoops = Transpose<double>(randomNumbersAllLoops);
					foreach (List<double> subList in randomNumbersAllLoops)
					{
						List<int> intSubList = new List<int>();
						foreach (double value in subList)
						{
							intSubList.Add(Convert.ToInt32(value));
						}
						randomizedConductivity.Add(Mode(intSubList));
					}
				}
				return randomizedConductivity;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Transposes the lists
		/// </summary>
		/// <typeparam name="T">Any entity</typeparam>
		/// <param name="lists">lists data parameter</param>
		/// <returns>Returns the transposed data</returns>
		private static List<List<T>> Transpose<T>(List<List<T>> lists)
		{
			try
			{
				var longest = lists.Any() ? lists.Max(l => l.Count) : 0;
				List<List<T>> outer = new List<List<T>>(longest);
				for (int i = 0; i < longest; i++)
				{
					outer.Add(new List<T>(lists.Count));
				}
				for (int j = 0; j < lists.Count; j++)
				{
					for (int i = 0; i < longest; i++)
					{
						outer[i].Add(lists[j].Count > i ? lists[j][i] : default(T));
					}
				}
				return outer;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the Mode
		/// </summary>
		/// <param name="x">data from mode to find</param>
		/// <returns>Returns the mode</returns>
		private static int Mode(List<int> x)
		{
			try
			{
				int mode = x.GroupBy(v => v).OrderByDescending(g => g.Count()).FirstOrDefault().Key;
				return mode;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the Grains
		/// </summary>
		/// <param name="conductivity">conductivity parameter</param>
		/// <returns>Returns the Grains</returns>
		private static List<double> Grains(List<double> conductivity)
		{
			try
			{
				List<double> converted = new List<double>();
				foreach (var value in conductivity)
				{
					converted.Add(value * (0.7 / 17));
				}
				return converted;
			}
			catch
			{
				throw;
			}
		}
	}
}
