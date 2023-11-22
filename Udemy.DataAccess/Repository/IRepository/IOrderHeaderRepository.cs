using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Models;

namespace Udemy.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{
		void Update(OrderHeader orderHeder);
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
		void UpdateStipePaymentID(int id, string sessionId, string paymentIntentId);

	}
}
