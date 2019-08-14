using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;

namespace Bookstore.Entities {

	public abstract class BaseRepository<TEntity> : IRepository where TEntity : Entity {

		private readonly BookstoreContext _context;

		protected BaseRepository(BookstoreContext context) {
			this._context = context;
		}

		protected abstract DbSet<TEntity> Set { get; }
		protected abstract IQueryable<TEntity> Query { get; }

		protected BookstoreContext Context => this._context;	

		public void Add(TEntity entity) {
			if (this.Set.Any(a => a.Id == entity.Id)) throw new Exception(typeof(TEntity).Name + " " + entity.Id + " already exists");
			this.Set.Add(entity);
			this.Flush();
		}

		public IEnumerable<TEntity> Items => this.Set;

		public TEntity GetById(string id) {
			return this.Query.Get(u => u.Id == id);
		}

		public TEntity Remove(string id) {
			TEntity entity = this.GetById(id);
			this.Set.Remove(entity);
			this.Flush();
			return entity;
		}

		public void Clear() {
			this.Set.Clear();
		}

		public void Flush() {
			this._context.SaveChanges();
		}

	}

}