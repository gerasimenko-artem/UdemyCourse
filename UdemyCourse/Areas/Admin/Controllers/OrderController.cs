using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;
using Udemy.DataAccess.Repository;
using Udemy.DataAccess.Repository.IRepository;
using Udemy.Models;
using Udemy.Models.ViewModels;
using Udemy.Utility;

namespace UdemyCourse.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public OrderViewModel orderViewModel { get; set; }
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
			orderViewModel = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};
			return View(orderViewModel);
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Empoloyee)]

		public IActionResult UpdateOrderDetail()
		{
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderViewModel.OrderHeader.Id);
			orderHeaderFromDb.Name = orderViewModel.OrderHeader.Name;
			orderHeaderFromDb.PhoneNumber = orderViewModel.OrderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = orderViewModel.OrderHeader.StreetAddress;
			orderHeaderFromDb.City = orderViewModel.OrderHeader.City;
			orderHeaderFromDb.State = orderViewModel.OrderHeader.State;
			orderHeaderFromDb.PostalCode = orderViewModel.OrderHeader.PostalCode;
			if (!string.IsNullOrEmpty(orderViewModel.OrderHeader.Carrier))
			{
				orderHeaderFromDb.Carrier = orderViewModel.OrderHeader.Carrier;
			}
			if (!string.IsNullOrEmpty(orderViewModel.OrderHeader.TrackingNumber))
			{
				orderHeaderFromDb.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
			}
			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();
			TempData["Success"] = "Order Details Updated Successfully";

			return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Empoloyee)]
		public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeader.UpdateStatus(orderViewModel.OrderHeader.Id, SD.StatusInProcess);
			_unitOfWork.Save();
			TempData["Success"] = "Order Details Updated Successfully";

			return RedirectToAction(nameof(Details), new { orderId = orderViewModel.OrderHeader.Id });
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Empoloyee)]
		public IActionResult ShipOrder()
		{
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderViewModel.OrderHeader.Id);
			orderHeader.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
			orderHeader.Carrier = orderViewModel.OrderHeader.Carrier;
			orderHeader.OrderStatus = SD.StatusInShiped;
			orderHeader.ShippingDate = DateTime.Now;
			if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
			}
			_unitOfWork.OrderHeader.Update(orderHeader);
			_unitOfWork.Save();
			TempData["Success"] = "Order Details Shipped Successfully";

			return RedirectToAction(nameof(Details), new { orderId = orderViewModel.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Empoloyee)]
		public IActionResult CancelOrder()
		{
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderViewModel.OrderHeader.Id);
			if(orderHeader.PaymentStatus == SD.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId
				};

				var service = new RefundService();
				Refund refund = service.Create(options);
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);

			}
			else
			{
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);

			}
			_unitOfWork.Save();
			TempData["Success"] = "Order Cancelled Successfully";
			return RedirectToAction(nameof(Details), new { orderId = orderViewModel.OrderHeader.Id });

		}
		[ActionName("Details")]
		[HttpPost]
		public IActionResult Details_PAY_NOW()
		{
			orderViewModel.OrderHeader = _unitOfWork.OrderHeader
				.Get(u => u.Id == orderViewModel.OrderHeader.Id, includeProperties: "ApplicationUser");
			orderViewModel.OrderDetail = _unitOfWork.OrderDetail
				.GetAll(u => u.Id == orderViewModel.OrderHeader.Id, includeProperties: "Product");
			var domain = "https://localhost:7229/";
			var options = new SessionCreateOptions
			{
				SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderViewModel.OrderHeader.Id}",
				CancelUrl = domain + $"admin/order/details?orderId={orderViewModel.OrderHeader.Id}",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
			};

			foreach (var item in orderViewModel.OrderDetail)
			{
				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price * 100),// 20.50 = 2050
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Title
						}
					},
					Quantity = item.Count
				};
				options.LineItems.Add(sessionLineItem);
			}

			var service = new SessionService();
			Session session = service.Create(options);

			_unitOfWork.OrderHeader.UpdateStipePaymentID(orderViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();
			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
		}
		public IActionResult PaymentConfirmation(int orderHeaderId)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
			if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);
				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeader.UpdateStipePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}

			return View(orderHeaderId);
		}










		#region API CALLS


		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> objOrderHeader;

			if (User.IsInRole(SD.Role_User_Admin) || User.IsInRole(SD.Role_User_Empoloyee))
			{
				objOrderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				objOrderHeader = _unitOfWork.OrderHeader.
					GetAll(u=>u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
			}
			switch (status)
			{
				case "inprocess":
					objOrderHeader = objOrderHeader.Where(u=> u.OrderStatus == SD.StatusInProcess);
					break;
				case "pending":
					objOrderHeader = objOrderHeader.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
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
