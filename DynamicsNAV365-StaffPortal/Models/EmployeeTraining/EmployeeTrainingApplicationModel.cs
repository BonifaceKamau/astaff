using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.EmployeeTraining
{
	public class EmployeeTrainingApplicationModel
	{
        [Display(Name = "Application No.")]
        public string ApplicationNo { get; set; }

        [Display(Name ="Approved Training")]
        [Required(AllowEmptyStrings =false, ErrorMessage ="Approved training need cannot be empty")]
        public string TrainingNeed{get;set;} 
        public SelectList TrainingNeedNos { get; set; }

        [Display(Name = "Employee No.")]
        public string EmployeeNo { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Document Date")]
        public string DocumentDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The value of Description cannot be empty")]
        public string Description { get; set; }

        [Display(Name = "Purpose of Training")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Purpose of Training cannot be empty")]
        public string Purpose { get; set; }

        [Display(Name = "Starting Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Starting Date cannot be empty")]
        public string FromDate { get; set; }

        [Display(Name = "Training Calendar Period")]
        public string Year { get; set; }
        public SelectList YearCodes{ get; set; }

        [Display(Name = "End Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "End Date cannot be empty")]
        public string ToDate { get; set; }

        [Display(Name = "Number of days")]
        public string NoOfDays { get; set; }

        [Display(Name = "Estimate cost of Training")]
        public decimal CostOfTraining { get; set; }

        public string Location { get; set; }
        public SelectList Locations { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public bool ErrorStatus { get; set; }
    }
}
