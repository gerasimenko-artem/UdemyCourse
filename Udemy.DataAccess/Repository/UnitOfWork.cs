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
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _db;
		public IProductRepository Product { get; private set; }
		public ICategoryRepository Category { get; private set; }
		public ICompanyRepository Company { get; private set; }
		public IShoppingCartRepository ShoppingCart { get; private set; }
		public UnitOfWork(ApplicationDbContext db)
		{
			_db = db;
			ShoppingCart = new ShoppingCartRepository(_db);
			Category = new CategoryRepository(_db);
			Product = new ProductRepository(_db);
			Company = new CompanyRepository(_db);
		}	
		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
