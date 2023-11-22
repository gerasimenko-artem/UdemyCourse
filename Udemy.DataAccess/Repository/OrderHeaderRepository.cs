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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext _db;
		public OrderHeaderRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(OrderHeader orderHeader)
		{
			_db.OrderHeaders.Update(orderHeader);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);
			if(orderFromDb != null)
			{
				orderFromDb.OrderStatus = orderStatus;
				if (string.IsNullOrEmpty(paymentStatus))
				{
					orderFromDb.PaymentStatus = paymentStatus;
				}
			}
		
		}

		public void UpdateStipePaymentID(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if (!string.IsNullOrEmpty(sessionId))
			{
				orderFromDb.SessionId = sessionId;
			}
			if (string.IsNullOrEmpty(paymentIntentId))
			{
				orderFromDb.PaymentIntentId = paymentIntentId;
				orderFromDb.PaymentDate = DateTime.Now;
			}
		}
	}
}
