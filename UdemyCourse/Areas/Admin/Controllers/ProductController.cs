using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Udemy.DataAccess.Repository;
using Udemy.DataAccess.Repository.IRepository;
using Udemy.Models;
using Udemy.Models.ViewModels;
using Udemy.Utility;

namespace UdemyCourse.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_User_Admin)]

	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult ProductIndex()
		{
			List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
			return View(objProductList);
		}
		public IActionResult ProductUpsert(int? id)
		{
			ProductViewModel productViewModel = new()
			{
				CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.CategoryId.ToString()
				}),
				Product = new Product()
			};
			if (id == null || id == 0)
			{
				return View(productViewModel);
			}
			else
			{
				productViewModel.Product = _unitOfWork.Product.Get(u => u.ProductId == id);
				return View(productViewModel);
			}

		}

		[HttpPost]
		public IActionResult ProductUpsert(ProductViewModel productViewModel, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (file != null)
				{
					string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\product");

					if (!string.IsNullOrEmpty(productViewModel.Product.ImageURL)) {
						var oldImage = Path.Combine(wwwRootPath, productViewModel.Product.ImageURL.TrimStart('\\'));

						if (System.IO.File.Exists(oldImage))
						{
							System.IO.File.Delete(oldImage);
						}
					}

					using (var filestream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
					{
						file.CopyTo(filestream);
					}
					productViewModel.Product.ImageURL = @"\images\product\" + filename;
				}

				if (productViewModel.Product.ProductId == 0)
				{
					_unitOfWork.Product.Add(productViewModel.Product);
					TempData["success"] = "Product created successfully!";
				}
				else
				{
					_unitOfWork.Product.Update(productViewModel.Product);
					TempData["success"] = "Product updated successfully!";
				}
				_unitOfWork.Save();
				return RedirectToAction("ProductIndex");
			}
			else
			{
				productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.CategoryId.ToString()
				});
				return View(productViewModel);
			}
		}
		public IActionResult ProductDelete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Product productFromDb = _unitOfWork.Product.Get(u => u.ProductId == id);
			if (productFromDb == null)
			{
				return NotFound();
			}
			return View(productFromDb);
		}
		[HttpPost, ActionName("ProductDelete")]
		public IActionResult ProductDeletePOST(int? id)
		{
			Product obj = _unitOfWork.Product.Get(u => u.ProductId == id);
			if (id == null || id == 0)
			{
				return NotFound();
			}
			else
			{
				_unitOfWork.Product.Remove(obj);
				_unitOfWork.Save();
				TempData["success"] = "Product deleted successfully!";
				return RedirectToAction("ProductIndex");
			}

		}
		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return Json(new { data = objProductList });
		}
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			Product productToBeDeleted = _unitOfWork.Product.Get(u => u.ProductId == id);
			if (productToBeDeleted == null)
			{	
				return Json(new { success = false, message = "Error while deleting!" });
			}

			var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageURL.TrimStart('\\'));

			if (System.IO.File.Exists(oldImage))
			{
				System.IO.File.Delete(oldImage);
			}

			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Delete Succesful!" });
		}
		#endregion API CALLS

	}
}

