using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.EmployeeAppraisal
{
	public class EmployeeAppraisalHeaderModel
	{
		public string DocumentNo { get; set; }
		public string EmployeeNo { get; set; }
		public string EmployeeName { get; set; }

		[Display (Name ="Appraisal Period")]
		public string CalendarPeriod {get; set;}
		public SelectList CalendarPeriods { get; set; } 
		public string AppraisalStage { get; set; }
		public string Description{ get; set; }
		public string Status { get; set; }
		public string ErrorMessage { get; set; }
		public bool ErrorStatus { get; set; }
	}
}