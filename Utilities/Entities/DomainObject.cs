using System;

namespace Utilities.Entities {

	public abstract class DomainObject<TEntityType> where TEntityType : Entity {

		protected DomainObject(TEntityType entity) {
			this.Entity = entity;
		}

		public Guid Id => this.Entity.Id;

		public TEntityType Entity { get; }

	}

}
