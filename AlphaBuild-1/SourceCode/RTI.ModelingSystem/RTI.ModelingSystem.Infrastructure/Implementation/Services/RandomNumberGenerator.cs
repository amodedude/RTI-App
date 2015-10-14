// -----------------------------------------------------------------------
// <copyright file="RandomNumberGenerator.cs" company="RTI">
// RTI
// </copyright>
// <summary>Random Number Generator</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Services
{
	#region Usings

	using System;
	using System.Security.Cryptography;

	#endregion Usings

	/// <summary>
	/// RandomNumberGenerator class
	/// </summary>
	public class RandomNumberGenerator
	{
		/// <summary>
		/// Crypto service provider for random number generator
		/// </summary>
		private static readonly RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();

		/// <summary>
		/// Generates the random number between the min and max values
		/// </summary>
		/// <param name="minimumValue">minimum Value</param>
		/// <param name="maximumValue">maximum Value</param>
		/// <returns>Returns the random number</returns>
		public static double Between(double minimumValue, double maximumValue)
		{
			try
			{
				byte[] randomNumber = new byte[1];
				generator.GetBytes(randomNumber);
				double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);
				double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);
				double range = maximumValue - minimumValue + 1;
				double randomValueInRange = Math.Floor(multiplier * range);
				return (double)(minimumValue + randomValueInRange);
			}
			catch
			{
				throw;
			}
		}

	}
}
