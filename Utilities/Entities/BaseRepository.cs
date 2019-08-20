using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;

namespace Utilities.Entities {

	public abstract class BaseRepository<TContext, TEntity> : IRepository where TEntity : Entity where TContext : DbContext {

		private readonly TContext _context;

		protected BaseRepository(TContext context) {
			this._context = context;
		}

		protected abstract DbSet<TEntity> Set { get; }
		protected abstract IQueryable<TEntity> Query { get; }

		public IEnumerable<TEntity> Items => this.Set;

		protected TContext Context => this._context;

		public void Add(TEntity entity) {
			if (this.Set.Any(a => a.Id == entity.Id)) throw new Exception(typeof(TEntity).Name + " " + entity.Id + " already exists");
			this.Set.Add(entity);
			this.Flush();
		}

		public async Task AddAsync(TEntity entity) {
			if (this.Set.Any(a => a.Id == entity.Id)) throw new Exception(typeof(TEntity).Name + " " + entity.Id + " already exists");
			await this.Set.AddAsync(entity);
			this.Flush();
		}

		public async Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> query) {
			return await this.Query.Where(query).ToListAsync();
		}

		public TEntity GetById(string id) {
			return this.Query.Get(u => u.Id == id);
		}

		public async Task<TEntity> GetByIdAsync(string id) {
			return await this.Query.GetAsync(u => u.Id == id);
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