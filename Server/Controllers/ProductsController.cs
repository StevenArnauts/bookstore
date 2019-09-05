using Microsoft.AspNetCore.Mvc;
using Bookstore.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Bookstore.Models;

namespace Bookstore.Controllers {

	[Authorize(AuthenticationSchemes = "bsid")]
	public class ProductsController : BaseController {

		private readonly ProductRepository _products;

		public ProductsController(ProductRepository products) {
			this._products = products;
		}

		[HttpGet]
		public IActionResult List() {
			return this.View(this._products.Items);
		}

		[HttpGet]
		public IActionResult New() {
			return this.View();
		}

		[HttpPost]
		public async Task<IActionResult> New(ProductSpecification spec) {
			await this._products.AddAsync(spec.Name, spec.Price, spec.Stock);
			return this.RedirectToAction("List");
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id) {
			Product product = await this._products.GetByIdAsync(id);
			ProductModel model = new ProductModel {
				Id = product.Id,
				Name = product.Name,
				Price = product.Price,
				Stock = product.Stock
			};
			return this.View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(string id, ProductModel model, string action) {
			if (action == "Delete") {
				this._products.Remove(id);
			} else {
				Product product = await this._products.GetByIdAsync(id);
				product.Name = model.Name;
				product.Price = model.Price;
				product.Stock = model.Stock;
			}
			this._products.Flush();
			return this.RedirectToAction("List");
		}

		[HttpPost]
		public IActionResult Remove(string id) {
			this._products.Remove(id);
			return this.RedirectToAction("List");
		}

		[HttpGet]
		public IActionResult Detail(string id) {
			var product = this._products.GetById(id);
			return this.View(product);
		}

	}

}
