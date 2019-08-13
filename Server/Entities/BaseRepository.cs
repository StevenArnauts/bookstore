using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;

namespace Bookstore.Entities {

	public abstract class BaseRepository<TEntity> : IRepository where TEntity : Entity {

		private readonly List<TEntity> _entities = new List<TEntity>();

		public void Add(TEntity entity) {
			if (this._entities.Any(a => a.Id == entity.Id)) throw new Exception(typeof(TEntity).Name + " " + entity.Id + " already exists");
			this._entities.Add(entity);
		}

		public IEnumerable<TEntity> Items => this._entities.AsReadOnly();

		public TEntity GetById(string id) {
			return this._entities.Get(u => u.Id == id);
		}

		public TEntity Remove(string id) {
			TEntity entity = this.GetById(id);
			this._entities.Remove(entity);
			return entity;
		}


	}

}