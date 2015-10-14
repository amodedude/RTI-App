// -----------------------------------------------------------------------
// <copyright file="Repository.cs" company="RTI">
// RTI
// </copyright>
// <summary>Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Repository
{
	#region Usings

	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Linq.Expressions;
	using RTI.ModelingSystem.Core.Interfaces.Repository;
	using RTI.ModelingSystem.Core.Models;
	using RTI.ModelingSystem.Infrastructure.Data;
	using RTI.ModelingSystem.Core.DBModels;

	#endregion Usings

	/// <summary>
	/// Repository class
	/// </summary>
	/// <typeparam name="T">Generic class</typeparam>
	public class Repository<T> : IRepository<T> where T : class
	{
		#region Properties

		/// <summary>
		/// rti Context
		/// </summary>
		protected RtiContext rtiContext;

		/// <summary>
		/// Db set property
		/// </summary>
		protected DbSet<T> DbSet;

		#endregion Properties

		#region Constructor

		/// <summary>
		/// Repository constructor
		/// </summary>
		/// <param name="rtiContext">rtiContext parameter</param>
		public Repository(RtiContext rtiContext)
		{
			this.rtiContext = rtiContext;
			this.DbSet = rtiContext.Set<T>();
		}

		#endregion Constructor

		#region IRepository<T> Members

		/// <summary>
		/// Inserts entity
		/// </summary>
		/// <param name="entity">entity parameter</param>
		public virtual void Insert(T entity)
		{
			DbSet.Add(entity);
		}

		/// <summary>
		/// Updates the entity
		/// </summary>
		/// <param name="entity">entity parameter</param>
		public virtual void Update(T entity)
		{
			rtiContext.Entry(entity).State = EntityState.Modified;
		}

		/// <summary>
		/// Deletes the entity
		/// </summary>
		/// <param name="entity">entity parameter</param>
		public void Delete(T entity)
		{
			DbSet.Remove(entity);
		}

		/// <summary>
		/// Finds the entity
		/// </summary>
		/// <param name="predicate">predicate parameter</param>
		/// <returns>Returns the list of entities</returns>
		public virtual IQueryable<T> Find(Expression<Func<T, bool>> predicate)
		{
			return DbSet.Where(predicate);
		}

		/// <summary>
		/// Gets All the entities
		/// </summary>
		/// <returns>Returns the list of entities</returns>
		public virtual IQueryable<T> GetAll()
		{
			return DbSet;
		}

		/// <summary>
		/// Gets entity By Id
		/// </summary>
		/// <param name="id">id parameter</param>
		/// <returns>Returns the entity</returns>
		public virtual T GetById(string id)
		{
			return DbSet.Find(id);
		}

		/// <summary>
		/// Submit  the Changes
		/// </summary>
		public virtual void SubmitChanges()
		{
			rtiContext.SaveChanges();
		}

		/// <summary>
		/// Gets the entity By Id
		/// </summary>
		/// <param name="id">id parameter</param>
		/// <returns>Returns the entity</returns>
		public T GetById(long id)
		{
			return DbSet.Find(id);
		}

		/// <summary>
		/// Inserts And Gets the ID
		/// </summary>
		/// <param name="customer">customer parameter</param>
		/// <returns>Returns the id</returns>
		public long InsertAndGetID(customer customer)
		{
			rtiContext.customers.Add(customer);
			rtiContext.SaveChanges();
			return customer.customerID;
		}

		#endregion

	}
}