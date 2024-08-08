using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.LoanApplicationModels
{
    public class LoanApplicationModel
    {
        public string No { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public string DocumentDate { get; set; }
        public string Amount { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Loan Product Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Loan Product Type is required")]
        public string LoanProductType { get; set; }
        public SelectList LoanProductTypes { get; set; } 
        public string Status { get; set; }
        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}