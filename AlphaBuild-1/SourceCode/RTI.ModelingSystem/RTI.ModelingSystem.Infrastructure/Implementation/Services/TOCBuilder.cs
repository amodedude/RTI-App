// -----------------------------------------------------------------------
// <copyright file="TOCBuilder.cs" company="RTI">
// RTI
// </copyright>
// <summary>TOC Builder</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Services
{
	public class TOCBuilder
	{
		/// <summary>
		/// m regin value
		/// </summary>
		private double mregin = 1.23017248018537;

		/// <summary>
		/// b regin value
		/// </summary>
		private double bregin = 99.7729161574187;
		
		/// <summary>
		/// Regen time curve
		/// </summary>
		/// <param name="numberOfweeks">number Of weeks</param>
		/// <returns>Returns the regin time curve</returns>
		public double RegenTimeCurve(double numberOfweeks)
		{
			try
			{
				double regenTime = (mregin * numberOfweeks) + bregin;
				return regenTime;
			}
			catch
			{
				throw;
			}
		}
	}
}
