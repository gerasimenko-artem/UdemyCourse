using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Models.ViewModels
{
	public class OrderViewModel
	{
		public OrderHeader OrderHeader { get; set; }
		public IEnumerable<OrderDetail> OrderDetail { get; set; }
		public string OrderDateString
		{
			get
			{
				return OrderHeader.OrderDate.ToShortDateString();
			}
		}

		public string OrderShippingDateString
		{
			get
			{
				return OrderHeader.ShippingDate.ToShortDateString();
			}
		}

		public string PaymentDueDateString
		{
			get
			{
				return OrderHeader.PaymentDueDate.ToShortDateString();
			}
		}

		public string PaymentDateString
		{
			get
			{
				return OrderHeader.PaymentDate.ToShortDateString();
			}
		}


	}
}
