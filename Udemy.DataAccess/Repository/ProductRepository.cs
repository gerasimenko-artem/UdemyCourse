using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DataAccess.Data;
using Udemy.DataAccess.Repository.IRepository;
using Udemy.Models;

namespace Udemy.DataAccess.Repository
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		private readonly ApplicationDbContext _db;
		public ProductRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(Product product)
		{
			var objFormDb = _db.Products.FirstOrDefault(u => u.ProductId == product.ProductId);
			if (objFormDb != null)
			{
				objFormDb.Title = product.Title;
				objFormDb.Description = product.Description;
				objFormDb.Price = product.Price;
				objFormDb.Price50 = product.Price50;
				objFormDb.Price100 = product.Price100;
				objFormDb.ListPrice = product.ListPrice;
				objFormDb.CategoryID = product.CategoryID;
				objFormDb.ISBN = product.ISBN;
				objFormDb.Author = product.Author;
				if (product.ImageURL != null)
				{
					objFormDb.ImageURL = product.ImageURL;
				}
			}
		}

	}
}
