using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Udemy.DataAccess.Repository;
using Udemy.DataAccess.Repository.IRepository;
using Udemy.Models;
using Udemy.Models.ViewModels;
using Udemy.Utility;

namespace UdemyCourse.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_User_Admin)]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public OrderController(IUnitOfWork unitOfWork)
		{

			_unitOfWork = unitOfWork;
		}
		public IActionResult OrderIndex()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			OrderViewModel orderViewModel = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};
			return View(orderViewModel);
		}


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> objOrderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

			switch (status)
			{
				case "inprocess":
					objOrderHeader = objOrderHeader.Where(u=> u.OrderStatus == SD.StatusInProcess);
					break;
				case "pending":
					objOrderHeader = objOrderHeader.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
					break;
				case "completed":
					objOrderHeader = objOrderHeader.Where(u => u.OrderStatus == SD.StatusInShiped);
					break;
				case "approved":
					objOrderHeader = objOrderHeader.Where(u => u.OrderStatus == SD.StatusApproved);
					break;
				default:
					break;
			}
			return Json(new { data = objOrderHeader });
		}
		#endregion API CALLS

	}
}
