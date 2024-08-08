using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.PayrollModels
{
    public class EmployeeP9Model
    {
        public string EmployeeNo { get; set; }

        //[Display(Name = "Payroll Year")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Payroll Year Required")]
        //public int PayrollYear { get; set; }
        //public SelectList PayrollYears { get; set; }

        [Display(Name = "Period Start Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Period Start Date Required")]
        public string StartDate { get; set; } 

        [Display(Name = "Period End Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Period End Date Required")]
        public string EndDate { get; set; }
        public SelectList PayrollPeriods { get; set; }
        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}