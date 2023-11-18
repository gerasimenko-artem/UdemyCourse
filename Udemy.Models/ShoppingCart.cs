using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Models
{
	public class ShoppingCart
	{
		public int ShoppingCartId { get; set; }
		public int ProductIdent { get; set; }
		[ValidateNever]
		public Product Product{ get; set; }
		[Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]

		public int Count { get; set; }
		public string ApplicationUserId { get; set; }
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }	


	}
}
