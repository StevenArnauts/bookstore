using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;

namespace Utilities.Entities {

	public interface IRepository {

		void Flush();

	}

	public abstract class BaseRepository<TDomainType, TEntityType> : IRepository where TEntityType : Entity where TDomainType : DomainObject<TEntityType> {

		protected BaseRepository(DbSet<TEntityType> entities) {
			this.Entities = entities;
		}

		public void Flush()
		{
			// do nothing
		}

		public DbSet<TEntityType> Entities { get; }

		public IEnumerable<TDomainType> All() {
			return this.Entities.ToList().Select(this.CreateInstance);
		}

		public IEnumerable<TDomainType> Query(Expression<Func<TEntityType, bool>> condition) {
			IEnumerable<TEntityType> queryable = this.Entities.Where(condition);
			return queryable.Select(this.CreateInstance);
		}

		public TDomainType Get(Guid id) {
			TEntityType entity = this.Entities.Get(p => p.Id == id);
			return this.CreateInstance(entity);
		}

		public TDomainType FirstOrDefault(Expression<Func<TEntityType, bool>> condition) {
			TEntityType entity = this.Entities.FirstOrDefault(condition);
			return  this.CreateInstance(entity);
		}

		public void Delete(Guid id) {
			TEntityType entity = this.Entities.Get(p => p.Id == id);
			this.Entities.Remove(entity);
		}

		protected virtual IQueryable<TEntityType> Loader => this.Entities;

		protected virtual TDomainType CreateInstance(TEntityType entity) {
			var instance = Activator.CreateInstance(typeof (TDomainType), entity) as TDomainType;
			return instance;
		}

	}

}