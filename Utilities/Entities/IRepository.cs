namespace Utilities.Entities {

	public interface IRepository<TContext> : IRepository where TContext : IDbContext
	{
		IUnitOfWork<TContext> UnitOfWork { get; }
	}

}