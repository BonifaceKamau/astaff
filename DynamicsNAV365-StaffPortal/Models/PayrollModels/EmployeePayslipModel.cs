using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.PayrollModels
{
	public class EmployeePayslipModel
	{
		public string EmployeeNo { get; set; }

		[Display(Name = "Payroll Period")]
		//[Required(AllowEmptyStrings = false, ErrorMessage = "Payroll Period Required")]
		public string PayrollPeriod { get; set; }
        public string StartingDate { get; set; }
        public SelectList PayrollPeriods { get; set; }
		public bool ErrorStatus { get; set; }
		public string ErrorMessage { get; set; }
        public List<SelectListItem> ListofPeriods { get; set; }
    }
    public class Periods
    {
        public string StartDate { get; set; }
        
    }
}