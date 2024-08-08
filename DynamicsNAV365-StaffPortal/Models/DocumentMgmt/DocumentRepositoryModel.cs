using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.DocumentMgmt
{
    public class DocumentRepositoryModel
    {
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }

        [Display(Name= "Document Type")]
        [Required(ErrorMessage ="Please select document",AllowEmptyStrings =false)]
        public string DocumentType { get; set; }
        public SelectList DocumentTypes { get; set; }
        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}