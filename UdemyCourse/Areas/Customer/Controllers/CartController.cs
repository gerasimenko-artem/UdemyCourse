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
		
		public IActionResult Summary()
		{
			return View();
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
			foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartViewModel.OrderTotal += (cart.Price * cart.Count);
			}
			return View(ShoppingCartViewModel);

		}


		public IActionResult PlusProductToCartShopping(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId);
			cartFromDb.Count++;
			_unitOfWork.ShoppingCart.Update(cartFromDb);
			_unitOfWork.Save();
			return RedirectToAction(nameof(CartIndex));
		}

		public IActionResult MinusProductInCartShopping(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId);
			if (cartFromDb.Count <= 1)
			{
				_unitOfWork.ShoppingCart.Remove(cartFromDb);
			}
			else
			{
				cartFromDb.Count--;
				_unitOfWork.ShoppingCart.Update(cartFromDb);
			}
			_unitOfWork.Save();
			return RedirectToAction(nameof(CartIndex));
		}


		public IActionResult DeleteProductInCartShopping(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId);
			_unitOfWork.ShoppingCart.Remove(cartFromDb);
			_unitOfWork.Save();
			return RedirectToAction(nameof(CartIndex));
		}

		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 50)
			{
				return shoppingCart.Product.Price;
			}
			else if (shoppingCart.Count <= 100)
			{
				return shoppingCart.Product.Price50;
			}
			else
			{
				return shoppingCart.Product.Price100;
			}
		}
	}
}
