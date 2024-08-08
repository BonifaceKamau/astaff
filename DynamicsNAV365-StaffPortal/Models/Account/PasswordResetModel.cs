using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.Account
{
	public class PasswordResetModel
	{
		[Display(Name = "Employee No.")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Employee No. is Required")]
		public string EmployeeEmail { get; set; }

		[Display(Name = "New Password")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "New Password Required")]
		[DataType(DataType.Password)]
		[MinLength(8, ErrorMessage = "Minimum 8 characters required")]
		public string Password { get; set; }

		[Display(Name = "Confirm Password")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Confirm Password and New Password do not Match")]
		public string ConfirmPassword { get; set; }
		public string PasswordResetToken { get; set; }
		public bool ErrorStatus { get; set; }
		public string ErrorMessage { get; set; }
	}
}