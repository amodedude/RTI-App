// -----------------------------------------------------------------------
// <copyright file="ProcessData.cs" company="RTI">
// RTI
// </copyright>
// <summary>Process Data</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Services
{
	#region Usings

	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	#endregion Usings

	/// <summary>
	/// Process Data
	/// </summary>
	public class ProcessData
	{
		#region Properties

		/// <summary>
		/// wkly Avg CondData Source1
		/// </summary>
		private Dictionary<DateTime, Tuple<int, int, List<int>>> weeklyAverageCondDataSource1 = new Dictionary<DateTime, Tuple<int, int, List<int>>>();

		/// <summary>
		/// wkly Avg Cond Data Source2
		/// </summary>
		private Dictionary<DateTime, Tuple<int, int, List<int>>> weeklyAverageCondDataSource2 = new Dictionary<DateTime, Tuple<int, int, List<int>>>();

		/// <summary>
		/// wkly Weightedt AvgCondData Source1
		/// </summary>
		private Dictionary<DateTime, Tuple<int, int>> weeklyWeightedAverageCondDataSource1 = new Dictionary<DateTime, Tuple<int, int>>();

		/// <summary>
		/// wkly Weighted tAvg Cond Data Source2
		/// </summary>
		private Dictionary<DateTime, Tuple<int, int>> weeklyWeightedAverageCondDataSource2 = new Dictionary<DateTime, Tuple<int, int>>();

		/// <summary>
		/// wkly Weighted AvgCond stdDev
		/// </summary>
		public static Dictionary<DateTime, Tuple<int, double, double>> WeeklyWeightedAverageCondstdDev = new Dictionary<DateTime, Tuple<int, double, double>>();

		/// <summary>
		/// totalt Weighted Average
		/// </summary>
		private static double totaltWeightedAverage;

		/// <summary>
		/// total Weighted Std Dev
		/// </summary>
		private static double totalWeightedStandardDeviation;

		/// <summary>
		/// average Source1
		/// </summary>
		private static double averageSource1;

		/// <summary>
		/// average Source2
		/// </summary>
		private static double averageSource2;

		/// <summary>
		/// stdDev Source1
		/// </summary>
		private static double standardDeviationSource1;

		/// <summary>
		/// stdDev Source2
		/// </summary>
		private static double standardDeviationSource2;

		/// <summary>
		/// wkly StDev CondData Source1
		/// </summary>
		private Dictionary<DateTime, Tuple<int, int, List<int>>> weeklyStandardDeviationConductivityDataSource1 = new Dictionary<DateTime, Tuple<int, int, List<int>>>();

		/// <summary>
		/// wkly StDevCondData Source2
		/// </summary>
		private Dictionary<DateTime, Tuple<int, int, List<int>>> weeklyStandardDeviationConductivityDataSource2 = new Dictionary<DateTime, Tuple<int, int, List<int>>>();

		/// <summary>
		/// cond Source1
		/// </summary>
		private static List<string>[] conductivitySource1 = new List<string>[5];

		/// <summary>
		/// cond Source2
		/// </summary>
		private static List<string>[] condictovitySource2 = new List<string>[5];

		/// <summary>
		/// conductivity Data S1
		/// </summary>
		private Dictionary<DateTime, Tuple<int, int>> conductivityDataS1 = new Dictionary<DateTime, Tuple<int, int>>();

		/// <summary>
		/// conductivity Data S2
		/// </summary>
		private Dictionary<DateTime, Tuple<int, int>> conductivityDataS2 = new Dictionary<DateTime, Tuple<int, int>>();

		/// <summary>
		/// data For Wk To Average
		/// </summary>
		private Dictionary<DateTime, Tuple<int, int>> dataForWeekToAverage = new Dictionary<DateTime, Tuple<int, int>>();

		/// <summary>
		/// using Two Sources
		/// </summary>
		private static bool usingTwoSources;

		/// <summary>
		/// sourceOne Slider Percent
		/// </summary>
		private static double sourceOneSliderPercent;

		/// <summary>
		/// source Two SliderPercent
		/// </summary>
		public static double SourceTwoSliderPercent;

		#endregion Properties

		#region Methods

		/// <summary>
		/// Computes the average
		/// </summary>
		/// <param name="cond1">cond1 parameter</param>
		/// <param name="cond2">cond2 parameter</param>
		/// <param name="sourceOnePercent">source One Percent</param>
		public void Compute(List<string>[] cond1, List<string>[] cond2, double sourceOnePercent)
		{
			try
			{
				ProcessData.conductivitySource1 = cond1;
				ProcessData.condictovitySource2 = cond2;
				this.FilterData(1);
				this.FilterData(2);
				this.WeightedAverageCalculator();
				this.WeightedStandardDeviationCalculator(sourceOnePercent);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// filters the Data
		/// </summary>
		/// <param name="sourceNumber">source Number</param>
		private void FilterData(int sourceNumber)
		{
			try
			{
				int numDataPoints;
				if (sourceNumber == 1)
				{
					numDataPoints = conductivitySource1[1].Count;
				}
				else
				{
					numDataPoints = condictovitySource2[1].Count;
				}
				int[] condVal = new int[numDataPoints];
				int[] weekNumbers = new int[numDataPoints];
				List<DateTime>[] dateList = new List<DateTime>[1];
				dateList[0] = new List<DateTime>();
				DateTime dateTime;
				int i = 0;
				if (sourceNumber == 1)
				{
					foreach (var date in conductivitySource1[3])
					{
						dateTime = date != null ? DateTime.Parse(date, new System.Globalization.CultureInfo("en-US", true)) : new DateTime();
						weekNumbers[i] = WeekNumberCalculator(dateTime);
						dateList[0].Add(dateTime);
						i++;
					}
					i = 0;
					foreach (var value in conductivitySource1[1])
					{
						condVal[i] = !string.IsNullOrWhiteSpace(value) ? Convert.ToInt32(value) : 0;
						i++;
					}
					i = 0;
					foreach (var date in dateList[0])
					{
						Tuple<int, int> weekNumCondVal = new Tuple<int, int>(weekNumbers[i], condVal[i]);
						this.conductivityDataS1.Add(date, weekNumCondVal);
						i++;
					}
					this.AverageDataByWeek(this.conductivityDataS1, sourceNumber);
					this.StandardDeviationDataByWeek(this.conductivityDataS1, sourceNumber);
				}
				else
				{
					foreach (var date in condictovitySource2[3])
					{
						dateTime = date != null ? DateTime.Parse(date, new System.Globalization.CultureInfo("en-US", true)) : new DateTime();
						weekNumbers[i] = WeekNumberCalculator(dateTime);
						dateList[0].Add(dateTime);
						i++;
					}
					i = 0;
					foreach (var value in condictovitySource2[1])
					{
						condVal[i] = !string.IsNullOrWhiteSpace(value) ? Convert.ToInt32(value) : 0;
						i++;
					}
					i = 0;
					foreach (var date in dateList[0])
					{
						Tuple<int, int> weekNumCondVal = new Tuple<int, int>(weekNumbers[i], condVal[i]);
						this.conductivityDataS2.Add(date, weekNumCondVal);
						i++;
					}
					this.AverageDataByWeek(this.conductivityDataS2, sourceNumber);
					this.StandardDeviationDataByWeek(this.conductivityDataS2, sourceNumber);
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// week Number Calculator
		/// </summary>
		/// <param name="fromDate">from Date parameter</param>
		/// <returns>Returns the week number</returns>
		private static int WeekNumberCalculator(DateTime fromDate)
		{
			try
			{
				DateTime startOfYear = fromDate.AddDays(-fromDate.Day + 1).AddMonths(-fromDate.Month + 1);
				DateTime endOfYear = startOfYear.AddYears(1).AddDays(-1);
				int[] ISO8601Correction = { 6, 7, 8, 9, 10, 4, 5 };
				int nds = fromDate.Subtract(startOfYear).Days + ISO8601Correction[(int)startOfYear.DayOfWeek];
				int weekNumber = nds / 7;
				switch (weekNumber)
				{
					case 0:
						weekNumber = WeekNumberCalculator(startOfYear.AddDays(-1));
						break;
					case 53:
						if (endOfYear.DayOfWeek < DayOfWeek.Thursday)
						{
							weekNumber = 1;
						}
						break;
				}
				return weekNumber;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// average Data By Week
		/// </summary>
		/// <param name="conductivityData">conductivity Data</param>
		/// <param name="sourceNumber">source Number</param>
		private void AverageDataByWeek(Dictionary<DateTime, Tuple<int, int>> conductivityData, int sourceNumber)
		{
			try
			{
				DateTime firstDate = new DateTime();
				if (conductivityData != null && conductivityData.Count > 0)
				{
					firstDate = conductivityData.FirstOrDefault().Key;
				}
				DateTime lastDate = new DateTime();
				if (conductivityData != null && conductivityData.Count > 0)
				{
					lastDate = conductivityData.LastOrDefault().Key;
				}
				int weeks = Convert.ToInt32(((lastDate - firstDate).TotalDays / 7) + 1);

				DateTime previousDate = new DateTime();
				if (conductivityData != null && conductivityData.Count > 0)
				{
					previousDate = conductivityData.FirstOrDefault().Key;
				}
				int nextWeek = 0;
				if (conductivityData != null && conductivityData.Count > 0)
				{
					nextWeek = WeekNumberCalculator(conductivityData.LastOrDefault().Key.AddDays(7));
				}
				List<int>[] groupedWeekCondData = new List<int>[weeks];

				int i = 0;
				foreach (var entry in conductivityData)
				{
					int currentWeek = WeekNumberCalculator(entry.Key);
					int average;
					if (currentWeek != nextWeek && !this.dataForWeekToAverage.ContainsKey(entry.Key))
					{
						this.dataForWeekToAverage.Add(entry.Key, entry.Value);
					}
					else
					{
						List<int> valuesForWeek = new List<int>();
						foreach (var condVal in this.dataForWeekToAverage.Values)
						{
							valuesForWeek.Add(condVal.Item2);
						}

						if (this.dataForWeekToAverage.Count == 0)
						{
							valuesForWeek.Add(entry.Value.Item2);
						}

						groupedWeekCondData[i] = valuesForWeek;
						average = Convert.ToInt32(valuesForWeek.Average());
						Tuple<int, int, List<int>> weekCondVal = new Tuple<int, int, List<int>>(currentWeek, average, groupedWeekCondData[i]);
						if (!this.weeklyAverageCondDataSource1.ContainsKey(entry.Key))
						{
							if (sourceNumber == 1)
							{
								this.weeklyAverageCondDataSource1.Add(entry.Key, weekCondVal);
							}
							else
							{
								this.weeklyAverageCondDataSource2.Add(entry.Key, weekCondVal);
							}
						}
						nextWeek = WeekNumberCalculator(entry.Key.AddDays(7));
						this.dataForWeekToAverage.Clear();
						i++;
					}
					previousDate = entry.Key;
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Standard Deviation Data By Week
		/// </summary>
		/// <param name="conductivityData">conductivity Data</param>
		/// <param name="sourceNumber">source Number</param>
		private void StandardDeviationDataByWeek(Dictionary<DateTime, Tuple<int, int>> conductivityData, int sourceNumber)
		{
			try
			{
				DateTime firstDate = new DateTime();
				if (conductivityData != null && conductivityData.Count > 0)
				{
					firstDate = conductivityData.FirstOrDefault().Key;
				}
				DateTime lastDate = new DateTime();
				if (conductivityData != null && conductivityData.Count > 0)
				{
					lastDate = conductivityData.Last().Key;
				}
				int weeks = Convert.ToInt32(((lastDate - firstDate).TotalDays / 7) + 1);
				List<int>[] groupedWeekCondData = new List<int>[weeks];
				DateTime previousDate = new DateTime();
				if (conductivityData != null && conductivityData.Count > 0)
				{
					previousDate = conductivityData.FirstOrDefault().Key;
				}
				int nextWeek = 0;
				if (conductivityData != null && conductivityData.Count > 0)
				{
					nextWeek = WeekNumberCalculator(conductivityData.FirstOrDefault().Key.AddDays(7));
				}
				int i = 0;
				foreach (var entry in conductivityData)
				{
					int currentWeek = WeekNumberCalculator(entry.Key);
					int standardDeviation;
					if (currentWeek != nextWeek && !this.dataForWeekToAverage.Keys.Contains(entry.Key))
					{
						this.dataForWeekToAverage.Add(entry.Key, entry.Value);
					}
					else
					{
						List<int> valuesForWeek = new List<int>();
						foreach (var condVal in this.dataForWeekToAverage.Values)
						{
							valuesForWeek.Add(condVal.Item2);
						}
						groupedWeekCondData[i] = valuesForWeek;
						if (valuesForWeek.Count > 1)
						{
							standardDeviation = Convert.ToInt32(StandardDeviation(valuesForWeek));

						}
						else
						{
							standardDeviation = 0;
						}
						Tuple<int, int, List<int>> weekCondVal = new Tuple<int, int, List<int>>(currentWeek, standardDeviation, groupedWeekCondData[i]);
						if (sourceNumber == 1)
						{
							weeklyStandardDeviationConductivityDataSource1.Add(entry.Key, weekCondVal);
						}
						else
						{
							this.weeklyStandardDeviationConductivityDataSource2.Add(entry.Key, weekCondVal);
						}
						nextWeek = WeekNumberCalculator(entry.Key.AddDays(7));
						this.dataForWeekToAverage.Clear();
					}
					previousDate = entry.Key;
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Standard Deviation
		/// </summary>
		/// <param name="valueList">value List</param>
		/// <returns>Returns the standard deviation</returns>
		public static double StandardDeviation(List<int> valueList)
		{
			try
			{
				double mean = 0.0;
				double sum = 0.0;
				int k = 1;
				foreach (int value in valueList)
				{
					double tmpM = mean;
					mean += (value - tmpM) / k;
					sum += (value - tmpM) * (value - mean);
					k++;
				}
				return Math.Sqrt(sum / (k - 2));
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// weighted Average Calculator
		/// </summary>
		private void WeightedAverageCalculator()
		{
			try
			{
				double sourcePercentOne = 0.0;
				double sourcePercentTwo = 0.0;
				if (sourceOneSliderPercent == 100)
				{
					sourcePercentTwo = 0;
				}
				else
				{
					sourcePercentTwo = 100 - sourceOneSliderPercent;
				}
				sourcePercentOne = sourceOneSliderPercent * 0.01;
				sourcePercentTwo = SourceTwoSliderPercent * 0.01;

				List<int> sourceOneWkAvgList = new List<int>();
				List<int> sourceTwoWkAvgList = new List<int>();

				if (usingTwoSources)
				{
					foreach (var avgCond in this.weeklyAverageCondDataSource1)
					{
						Tuple<int, int> wkWtAvg = new Tuple<int, int>(avgCond.Value.Item1, avgCond.Value.Item2);
						if (!this.weeklyWeightedAverageCondDataSource1.ContainsKey(avgCond.Key))
						{
							this.weeklyWeightedAverageCondDataSource1.Add(avgCond.Key, wkWtAvg);
						}
					}

					foreach (var avgCond in this.weeklyAverageCondDataSource2)
					{
						Tuple<int, int> wkWtAvg = new Tuple<int, int>(avgCond.Value.Item1, avgCond.Value.Item2);
						if (!this.weeklyWeightedAverageCondDataSource2.ContainsKey(avgCond.Key))
						{
							this.weeklyWeightedAverageCondDataSource2.Add(avgCond.Key, wkWtAvg);
						}
					}
				}
				else
				{
					foreach (var avgCond in this.weeklyAverageCondDataSource1)
					{
						int average = avgCond.Value.Item2;
						Tuple<int, int> wkWtAvg = new Tuple<int, int>(avgCond.Value.Item1, avgCond.Value.Item2);
						if (!this.weeklyWeightedAverageCondDataSource1.ContainsKey(avgCond.Key))
						{
							this.weeklyWeightedAverageCondDataSource1.Add(avgCond.Key, wkWtAvg);
						}
					}
				}

				if (usingTwoSources)
				{
					foreach (var avgCond in this.weeklyAverageCondDataSource1)
					{
						sourceOneWkAvgList.Add(avgCond.Value.Item2);
					}

					foreach (var avgCond in this.weeklyAverageCondDataSource2)
					{
						sourceTwoWkAvgList.Add(avgCond.Value.Item2);
					}
					averageSource1 = sourceOneWkAvgList != null && sourceOneWkAvgList.Count > 0 ? sourceOneWkAvgList.Average() : 0;
					averageSource2 = sourceTwoWkAvgList != null && sourceTwoWkAvgList.Count > 0 ? sourceTwoWkAvgList.Average() : 0;
					totaltWeightedAverage = (averageSource1 * sourcePercentOne) + (averageSource2 * sourcePercentTwo);
				}
				else
				{
					foreach (var avgCond in this.weeklyAverageCondDataSource1)
					{
						sourceOneWkAvgList.Add(avgCond.Value.Item2);
					}
					if (sourceOneWkAvgList != null && sourceOneWkAvgList.Count > 0)
					{
						averageSource1 = sourceOneWkAvgList.Average();
					}
					totaltWeightedAverage = averageSource1 * sourcePercentOne;
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// weighted Standard Deviation Calculator
		/// </summary>
		/// <param name="sourceOnePercent">source One Percent</param>
		private void WeightedStandardDeviationCalculator(double sourceOnePercent)
		{
			try
			{
				sourceOneSliderPercent = sourceOnePercent;
				double sourcePercentOne = sourceOneSliderPercent * 0.01;
				double sourcePercentTwo = SourceTwoSliderPercent * 0.01;
				List<int> sourceOneWkStdDevList = new List<int>();
				List<int> sourceTwoWkStdDevList = new List<int>();
				usingTwoSources = true;
				if (usingTwoSources)
				{
					foreach (var stdDevCond in this.weeklyStandardDeviationConductivityDataSource1)
					{
						sourceOneWkStdDevList.Add(stdDevCond.Value.Item2);
					}
					foreach (var stdDevCond in this.weeklyStandardDeviationConductivityDataSource2)
					{
						sourceTwoWkStdDevList.Add(stdDevCond.Value.Item2);
					}
                    standardDeviationSource1 = sourceOneWkStdDevList.Count() > 0 ? sourceOneWkStdDevList.Average() : 0;
                    standardDeviationSource2 = sourceTwoWkStdDevList.Count() > 0 ? sourceTwoWkStdDevList.Average() : 0;
					totalWeightedStandardDeviation = (standardDeviationSource1 * sourcePercentOne) + (standardDeviationSource2 * sourcePercentTwo);
					int maxWeeksSource1 = this.weeklyStandardDeviationConductivityDataSource1.Count;
					int maxWeeksSource2 = this.weeklyStandardDeviationConductivityDataSource2.Count;
					int maxWeeks;
					bool sourceOneIsBigger;
					bool sourceTwoIsBigger;
					if (maxWeeksSource1 >= maxWeeksSource2)
					{
						maxWeeks = maxWeeksSource1;
						sourceOneIsBigger = true;
						sourceTwoIsBigger = false;
					}
					else
					{
						maxWeeks = maxWeeksSource2;
						sourceOneIsBigger = false;
						sourceTwoIsBigger = true;
					}
					WeeklyWeightedAverageCondstdDev.Clear();
                    int stopPoint = 0;
					for (int i = 0; i <= maxWeeks - 1; i++)
					{
						DateTime date = new DateTime();
						int week = new int();
						double averageCondS1 = 0;
						double averageCondS2 = 0;
						double stdDevVal1 = 0;
						double stdDevVal2 = 0;
						if (sourceOneIsBigger)
						{
							date = this.weeklyStandardDeviationConductivityDataSource1.ElementAt(i).Key;
							week = this.weeklyStandardDeviationConductivityDataSource1.ElementAt(i).Value.Item1;
						}
						else if (sourceTwoIsBigger)
						{
							date = this.weeklyStandardDeviationConductivityDataSource2.ElementAt(i).Key;
							week = this.weeklyStandardDeviationConductivityDataSource2.ElementAt(i).Value.Item1;
						}

						if (i < this.weeklyAverageCondDataSource1.Count)
						{
							averageCondS1 = this.weeklyAverageCondDataSource1.ElementAt(i).Value.Item3.Average();
						}
                        else if(i == this.weeklyAverageCondDataSource1.Count && i!=0)
                        {
                            stopPoint = i;
                            averageCondS1 = this.weeklyAverageCondDataSource1.ElementAt(i-stopPoint).Value.Item3.Average();
                        }
                        else if(i!=0)
                        {
                            averageCondS1 = this.weeklyAverageCondDataSource1.ElementAt(i - stopPoint).Value.Item3.Average();
                        }

						if (i < this.weeklyAverageCondDataSource2.Count)
						{
							averageCondS2 = this.weeklyAverageCondDataSource2.ElementAt(i).Value.Item3.Average();
						}


                        double avgConductivity;
                        if (averageCondS2 > 0)
                            avgConductivity = (averageCondS1 + averageCondS2) / 2;
                        else
                            avgConductivity = averageCondS1;

						if (i < this.weeklyStandardDeviationConductivityDataSource1.Count)
						{
							stdDevVal1 = this.weeklyStandardDeviationConductivityDataSource1.ElementAt(i).Value.Item2;
						}
						if (i < this.weeklyStandardDeviationConductivityDataSource2.Count)
						{
							stdDevVal2 = this.weeklyStandardDeviationConductivityDataSource2.ElementAt(i).Value.Item2;
						}
						double avgStdDev = (stdDevVal1 + stdDevVal2) / 2;
						Tuple<int, double, double> weekData = new Tuple<int, double, double>(week, avgStdDev, avgConductivity);
						if (!WeeklyWeightedAverageCondstdDev.ContainsKey(date))
						{
							WeeklyWeightedAverageCondstdDev.Add(date, weekData);
						}
					}
				}
				else
				{
					foreach (var stdDevCond in this.weeklyStandardDeviationConductivityDataSource1)
					{
						sourceOneWkStdDevList.Add(stdDevCond.Value.Item2);
					}
					if (sourceOneWkStdDevList != null && sourceOneWkStdDevList.Count > 0)
					{
						standardDeviationSource1 = sourceOneWkStdDevList.Average();
					}
					totalWeightedStandardDeviation = standardDeviationSource1 * sourcePercentOne;
					int maxWeeks = this.weeklyAverageCondDataSource1.Count;
					WeeklyWeightedAverageCondstdDev.Clear();
					for (int i = 0; i < maxWeeks; i++)
					{
						DateTime date = new DateTime();
						int week = new int();
						double averageCondS1 = new double();
						double stdDevVal1 = new double();
						if (i < this.weeklyAverageCondDataSource1.Count)
						{
							date = this.weeklyAverageCondDataSource1.ElementAt(i).Key;
							week = this.weeklyAverageCondDataSource1.ElementAt(i).Value.Item1;
							averageCondS1 = this.weeklyAverageCondDataSource1.ElementAt(i).Value.Item3.Average();
						}
						if (i < this.weeklyStandardDeviationConductivityDataSource1.Count)
						{
							stdDevVal1 = this.weeklyStandardDeviationConductivityDataSource1.ElementAt(i).Value.Item2;
						}
						Tuple<int, double, double> weekData = new Tuple<int, double, double>(week, stdDevVal1, averageCondS1);
						if (!WeeklyWeightedAverageCondstdDev.ContainsKey(date))
						{
							WeeklyWeightedAverageCondstdDev.Add(date, weekData);
						}
					}
				}
			}
			catch
			{
				throw;
			}
		}

		#endregion Methods
	}
}