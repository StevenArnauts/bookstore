using System;
using System.Linq;
using Utilities.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bookstore.Entities {

	public class ProductRepository : BaseRepository<BookstoreContext, Product> {

		public ProductRepository(BookstoreContext context) : base(context) { }

		protected override DbSet<Product> Set => this.Context.Products;
		protected override IQueryable<Product> Query => this.Context.Products;

		public async Task<Product> AddAsync(string name, decimal price, int stock, string id = null) {
			if (this.Items.Any(a => a.Name == name)) throw new Exception("Product " + name + " already exists");
			string i = id ?? Guid.NewGuid().ToString("N").ToUpper();
			var entity = new Product {
				Id = i,
				Name = name,
				Price = price,
				Stock = stock
			}; await this.AddAsync(entity);
			this.Flush();
			return entity;
		}

	}

}