using Microsoft.EntityFrameworkCore;

namespace Bookstore.Entities {

	public class BookstoreContext : DbContext {

		public BookstoreContext(DbContextOptions<BookstoreContext> options) : base(options) { }

		public DbSet<Customer> Customers { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<OrderLine> Lines { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Customer>().ToTable("customer");
			modelBuilder.Entity<Order>().ToTable("order");
			modelBuilder.Entity<OrderLine>().ToTable("lines");
			modelBuilder.Entity<Product>().ToTable("product");
		}

	}

}