// -----------------------------------------------------------------------
// <copyright file="IRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>IRepository</summary>
// -----------------------------------------------------------------------

using RTI.ModelingSystem.Core.DBModels;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace RTI.ModelingSystem.Core.Interfaces.Repository
{
	public interface IRepository<T>
	{
		/// <summary>
		/// Inserts the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		void Insert(T entity);

		/// <summary>
		/// Updates the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		void Update(T entity);

		/// <summary>
		/// Deletes the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		void Delete(T entity);

		/// <summary>
		/// Finds the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>Returns the entity</returns>
		IQueryable<T> Find(Expression<Func<T, bool>> predicate);

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>Returns the list of entities</returns>
		IQueryable<T> GetAll();

		/// <summary>
		/// Gets the by identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the entity</returns>
		T GetById(string id);

		/// <summary>
		/// Gets the by identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the entity</returns>
		T GetById(long id);

		/// <summary>
		/// Submits the changes.
		/// </summary>
		void SubmitChanges();

		/// <summary>
		/// Inserts the and get identifier.
		/// </summary>
		/// <param name="customer">The customer.</param>
		/// <returns>Returns the customer id</returns>
		long InsertAndGetID(customer customer);
	}
}
