using Microsoft.EntityFrameworkCore;

namespace Bookstore.Identity.Entities {

	public class IdentityContext : DbContext {

		public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<User>().ToTable("user");
		}

	}

}