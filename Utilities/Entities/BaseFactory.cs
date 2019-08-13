namespace Utilities.Entities {

	public abstract class BaseFactory<TContext> : IFactory where TContext : IDbContext {

		protected BaseFactory(IUnitOfWork<TContext> context) {
			this.UnitOfWork = context;
		}

		protected IUnitOfWork<TContext> UnitOfWork { get; }

		public void Flush() {
			this.UnitOfWork.SaveChanges();
		}

	}

}