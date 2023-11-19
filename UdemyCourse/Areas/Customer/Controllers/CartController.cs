using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Security.Claims;
using Udemy.DataAccess.Repository.IRepository;
using Udemy.Models;
using Udemy.Models.ViewModels;

namespace UdemyCourse.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		public ShoppingCartViewModel ShoppingCartViewModel { get; set; }
		private readonly IUnitOfWork _unitOfWork;

		public CartController(ILogger<CartController> logger, IUnitOfWork unitOfWork)
		{

			_unitOfWork = unitOfWork;

		}
		public IActionResult CartIndex()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartViewModel = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
				OrderTotal = 0
			};

			return View(ShoppingCartViewModel);

		}
	}
}
