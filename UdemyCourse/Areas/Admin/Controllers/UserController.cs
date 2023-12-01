using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Udemy.DataAccess.Data;
using Udemy.DataAccess.Repository;
using Udemy.DataAccess.Repository.IRepository;
using Udemy.Models;
using Udemy.Models.ViewModels;
using Udemy.Utility;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UdemyCourse.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_User_Admin)]

	public class UserController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly IUnitOfWork _unitOfWork;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;
		UserViewModel userViewModel;



		public UserController(
			ApplicationDbContext db, 
			IUnitOfWork unitOfWork, 
			RoleManager<IdentityRole> roleManager,
			UserManager<IdentityUser> userManager
			)
		{
			_db = db;
			_userManager = userManager;
			_unitOfWork = unitOfWork;
			_roleManager = roleManager;
		}
		public IActionResult UserIndex()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Permission(string? userId)
		{
			string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
			userViewModel = new UserViewModel()
			{
				ApplicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
				CompanyList = _unitOfWork.Company.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),

				RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
				{
					Text = i,
					Value = i
				}),
				
			};
			userViewModel.ApplicationUser.Role = _db.Roles.FirstOrDefault(x => x.Id == RoleId).Name;
			return View(userViewModel);

			
		}
		

		[HttpPost]
		public  IActionResult Permission(UserViewModel userViewModel)
		{

			string roleId = _db.UserRoles.FirstOrDefault(u=> u.UserId == userViewModel.ApplicationUser.Id).RoleId;
			string oldRole = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;

			if(!(userViewModel.ApplicationUser.Role == oldRole))
			{
				ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userViewModel.ApplicationUser.Id);
				if(userViewModel.ApplicationUser.Role ==SD.Role_User_Comp)
				{
					applicationUser.CompanyId = userViewModel.ApplicationUser.CompanyId;
				}
				if(oldRole == SD.Role_User_Comp)
				{
					applicationUser.CompanyId = null;
				}

				_userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
				_userManager.AddToRoleAsync(applicationUser, userViewModel.ApplicationUser.Role).GetAwaiter().GetResult();

				_db.SaveChanges();
			}
			return RedirectToAction("UserIndex");

		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<ApplicationUser> objUserList ;
			objUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();
			var userRoles = _db.UserRoles.ToList();
			var roles = _db.Roles.ToList();

			foreach (var user in objUserList)
			{
				var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
				user.Role =  roles.FirstOrDefault(u => u.Id == roleId).Name;
				
				if(user.Company == null)
				{
					user.Company = new() { Name = "" };
				}
			}
			return Json(new { data = objUserList });
		}
		[HttpPost]
		public IActionResult LockUnlock([FromBody]string? id)
		{
			var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
			if(objFromDb == null)
			{
				return Json(new { success = true, message = "Error while Locking/Unloking!" });
			}
			
			if(objFromDb.LockoutEnd!=null && objFromDb.LockoutEnd > DateTime.Now)
			{
				objFromDb.LockoutEnd = DateTime.Now;
			}
			else
			{
				objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
			}
			_db.SaveChanges();
			return Json(new { success = true, message = "Operation Succesful!" });
		}

	}

		#endregion API CALLS

	
}

