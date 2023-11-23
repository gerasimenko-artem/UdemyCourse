using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Udemy.DataAccess.Repository;
using Udemy.DataAccess.Repository.IRepository;
using Udemy.Models;
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
		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<OrderHeader> objOrderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			return Json(new { data = objOrderHeader });
		}
		#endregion API CALLS

	}
}
