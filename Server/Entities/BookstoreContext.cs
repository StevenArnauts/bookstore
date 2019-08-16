﻿using Microsoft.EntityFrameworkCore;
using Utilities.Logging;

namespace Bookstore.Entities {

	public class BookstoreContext : DbContext {

		public BookstoreContext(DbContextOptions<BookstoreContext> options) : base(options) { }

		public DbSet<Customer> Customers { get; set; }
		public DbSet<Order> Orders { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql("Host=localhost;Database=BookStore;Username=postgres;Password=pencil42");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Customer>().ToTable("customer");
			modelBuilder.Entity<Order>().ToTable("order");
		}

	}

}