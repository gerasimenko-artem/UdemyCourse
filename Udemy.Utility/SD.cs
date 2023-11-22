using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Utility
{
	public static class SD
	{
		public const string Role_User_Cust = "Customer";
		public const string Role_User_Comp = "Company";
		public const string Role_User_Admin = "Admin";
		public const string Role_User_Empoloyee = "Empoloyee";

		public const string StatusPending = "Pending";
		public const string StatusInProcess = "InProcess";
		public const string StatusInShiped = "Shiped";
		public const string StatusCancelled = "Cancelled";
		public const string StatusRefunded = "Refunded";
		public const string StatusApproved = "Approved";

		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
		public const string PaymentStatusRejected = "Rejected";

	}
}
