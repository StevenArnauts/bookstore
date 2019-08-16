using Microsoft.EntityFrameworkCore;

namespace Bookstore.Entities {

	public class BookstoreContext : DbContext {

		public BookstoreContext(DbContextOptions<BookstoreContext> options) : base(options) { }

		public DbSet<Customer> Customers { get; set; }
		public DbSet<Order> Orders { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Customer>().ToTable("customer");
			modelBuilder.Entity<Order>().ToTable("order");
		}

	}

}