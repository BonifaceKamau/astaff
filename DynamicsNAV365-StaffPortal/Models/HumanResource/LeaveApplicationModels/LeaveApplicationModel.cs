using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.LeaveApplicationModels
{
	public class LeaveApplicationModel
	{
		[Display(Name = "No.")]
		public string No { get; set; }

        public string ApplicationNo { get; set; }

        [Display(Name = "Employee No.")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Employee No. Required")]
		public string EmployeeNo { get; set; }

		[Display(Name = "Employee Name")]
		public string EmployeeName { get; set; }

		public SelectList Employees { get; set; }

		[Display(Name = "Leave Type")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Leave Type Required")]
		public string LeaveType { get; set; }

        public List<SelectListItem> LeaveTypes { get; set; }

        public SelectList EmployeeLeaveTypes { get; set; }

		[Display(Name = "Leave Start Date")]
		//[Required(AllowEmptyStrings = false, ErrorMessage = "Leave Start Date Required")]
		public string LeaveStartDate { get; set; }

		[Display(Name = "Leave Balance")]
	//	[Required(AllowEmptyStrings = false, ErrorMessage = "Applied Days Required")]
		public string LeaveBalance { get; set; }

		[Display(Name = "Applied Days")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Applied Days Required")]
		public string DaysApplied { get; set; }

		[Display(Name = "Approved Days")]
		public string DaysApproved { get; set; }

		[Display(Name = "Leave End Date")]
		public string LeaveEndDate { get; set; }

		[Display(Name = "Return Date")]
		public string LeaveReturnDate { get; set; }

		[Display(Name = "Handover Notes")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Handover Notes is required")]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
        //public string ReasonForLeave { get; set; }
        //[Required(ErrorMessage = "The reason for leave is required.")]
        public string ReasonForLeave { get; set; }

        [Display(Name = "Reliever Name")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Reliever is Required")]
		public string SubstituteEmployeeNo { get; set; }
		
		[Display(Name = "Reliever Acknowledgement")]
		public bool RelieverAcknowledgement { get; set; }

        [Display(Name = "Sent To Reliever")]
        public bool SentToReliever { get; set; } // DataItem 0 - Column 12

        [Display(Name = "Reliever Name")]
		public string SubstituteEmployeeName { get; set; }

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

		[Display(Name = "Days Earned")]
		public string DaysEarned { get; set; }

		[Display(Name = "Days Carried Forward")]
		public string DaysCarriedForward { get; set; }

		[Display(Name = "Days Taken")]
		public string DaysTaken { get; set; }

		[Display(Name = "Leave Balance")]
		public string LeaveBalanceStr { get; set; } 

        //[Display(Name = "Reason for Decline")]
        public string Comments { get; set; }

		[Display(Name = "Status")]
		public string Status { get; set; }

		public bool ErrorStatus { get; set; }
		public string ErrorMessage { get; set; }
	}
}