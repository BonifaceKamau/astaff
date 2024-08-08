using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.LeaveReimbursementModels
{
    public class LeaveReimbursementModel
    {
		[Display(Name = "Leave Reimbursement No.")]
		public string No { get; set; }

		[Display(Name = "Approved Leave Application")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Approved Leave Application Required")]
		public string ApprovedLeaveApplication { get; set; }
		public SelectList LeaveApplicationNos { get; set; }

		[Display(Name = "Substitute No.")]
		public string SubstituteNo { get; set; }

		[Display(Name = "Substitute Name")]
		public string SubstituteName { get; set; }

		[Display(Name = "Employee No.")]
		public string EmployeeNo { get; set; }

		[Display(Name = "Employee Name")]
		public string EmployeeName { get; set; }

		public SelectList Employees { get; set; }

		[Display(Name = "Leave Type")]
		public string LeaveType { get; set; }

		[Display(Name = "Leave Period")]
		public string LeavePeriod { get; set; }

		[Display(Name = "Leave Start Date")]
		public string LeaveStartDate { get; set; }

		[Display(Name = "Leave Balance")]
		public decimal LeaveBalance { get; set; }

		[Display(Name = "Applied Days")]
		public decimal DaysApplied { get; set; }

		[Display(Name = "Approved Days")]
		public decimal DaysApproved { get; set; }

		[Display(Name = "Leave End Date")]
		public string LeaveEndDate { get; set; }

		[Display(Name = "Return Date")]
		public string LeaveReturnDate { get; set; }

		[Display(Name = "Actual Return Date")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Actual Return Date. Required")]
		public string ActualReturnDate { get; set; }

		[Display(Name = "Days to Reimburse")]
		public decimal DaysToReimburse { get; set; }

		[Display(Name = "Reason For Reimbursement")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Reason For Reimbursement. Required")]
		public string ReasonForReimbursement { get; set; }

		[Display(Name = "Employee Comments")]
		public string ReasonForLeave { get; set; }

		[Display(Name = Dimensions.GlobalDimension1Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.GlobalDimension1Code+" Required")]
		public string GlobalDimension1Code { get; set; }
		public SelectList GlobalDimension1Codes { get; set; }

		[Display(Name = Dimensions.GlobalDimension2Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.GlobalDimension2Code + " Required")]
		public string GlobalDimension2Code { get; set; }
		public SelectList GlobalDimension2Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension3Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension3Code + " Required")]
		public string ShortcutDimension3Code { get; set; }
		public SelectList ShortcutDimension3Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension4Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension4Code + " Required")]
		public string ShortcutDimension4Code { get; set; }
		public SelectList ShortcutDimension4Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension5Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension5Code + " Required")]
		public string ShortcutDimension5Code { get; set; }
		public SelectList ShortcutDimension5Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension6Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension6Code + " Required")]
		public string ShortcutDimension6Code { get; set; }
		public SelectList ShortcutDimension6Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension7Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension7Code + " Required")]
		public string ShortcutDimension7Code { get; set; }
		public SelectList ShortcutDimension7Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension8Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension8Code + " Required")]
		public string ShortcutDimension8Code { get; set; }
		public SelectList ShortcutDimension8Codes { get; set; }

		[Display(Name = "Responsibility Center")]
		//[Required(AllowEmptyStrings = false, ErrorMessage = "Responsibility Center Required")]
		public string ResponsibilityCenter { get; set; }
		public SelectList ResponsibilityCenters { get; set; }

		[Display(Name = "Status")]
		public string Status { get; set; }

		public bool ErrorStatus { get; set; }
		public string ErrorMessage { get; set; }
	}
}