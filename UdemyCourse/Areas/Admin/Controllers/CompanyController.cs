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

	public class CompanyController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private IWebHostEnvironment _webHostEnvironment;
		public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult CompanyIndex()
		{
			List<Company> objCategoryList = _unitOfWork.Company.GetAll().ToList();
			return View(objCategoryList);
		}
		public IActionResult CompanyUpsert(int? id)
		{
			if (id == null || id == 0)
			{

				return View(new Company());
			}
			else
			{
				Company company = _unitOfWork.Company.Get(u => u.Id == id);
				return View(company);
			}

		}

		[HttpPost]
		public IActionResult CompanyUpsert(Company company)
		{
			if (ModelState.IsValid)
			{
				if (company.Id == 0)
				{
					_unitOfWork.Company.Add(company);
					TempData["success"] = "Company created successfully!";

				}
				else
				{
					_unitOfWork.Company.Update(company);
					TempData["success"] = "Company updated successfully!";

				}
				_unitOfWork.Save();
				return RedirectToAction("CompanyIndex");
			}
			else
			{
				return View(company);
			}
		}
		public IActionResult CompanyDelete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Company CompanyFromDb = _unitOfWork.Company.Get(u => u.Id == id);
			if (CompanyFromDb == null)
			{
				return NotFound();
			}
			return View(CompanyFromDb);
		}
		[HttpPost, ActionName("CompanyDelete")]
		public IActionResult CompanyDeletePOST(int? id)
		{
			Company obj = _unitOfWork.Company.Get(u => u.Id == id);
			if (id == null || id == 0)
			{
				return NotFound();
			}
			else
			{
				_unitOfWork.Company.Remove(obj);
				_unitOfWork.Save();
				TempData["success"] = "Company deleted successfully!";
				return RedirectToAction("CompanyIndex");
			}

		}
		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
			return Json(new { data = objCompanyList });
		}
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			Company CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
			if (CompanyToBeDeleted == null)
			{	
				return Json(new { success = false, message = "Error while deleting!" });
			}

			_unitOfWork.Company.Remove(CompanyToBeDeleted);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Delete Succesful!" });
		}
		#endregion API CALLS

	}
}

