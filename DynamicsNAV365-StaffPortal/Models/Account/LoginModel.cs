using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.Account
{
	public class LoginModel
	{
		[Display(Name = "Username")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
		public string EmployeeNoOrEmail { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Remember Me")]
		public bool RememberMe { get; set; }
		public bool ErrorStatus { get; set; }
		public string ErrorMessage { get; set; }
	}
}