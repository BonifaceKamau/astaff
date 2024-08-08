using System.ComponentModel.DataAnnotations;

namespace DynamicsNAV365_StaffPortal.Models.Account
{
	public class SendPasswordResetLinkModel
	{
		[Display(Name = "Employee email.")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Employee email. is Required")]
		public string EmployeeEmail { get; set; }
		public bool ErrorStatus { get; set; }
		public string ErrorMessage { get; set; }
	}
}