using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.SalaryAdvanceModels
{
    public class SalaryAdvanceHeaderModel
    {
        public string No { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public string Amount { get; set; }
        public string DocumentDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}