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
	public class CompanyRepository : Repository<Company>, ICompanyRepository
	{
		private readonly ApplicationDbContext _db;
		public CompanyRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(Company company)
		{
			_db.Update(company);
		}
	}
}
