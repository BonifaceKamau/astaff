using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.SalaryAdvanceModels
{
    public class SalaryAdvanceLineModel
    {
        public string LineNo { get; set; } 
        public string DocumentNo { get; set; } 
        public string LineAmount { get; set; }
        public bool LineErrorStatus { get; set; }
        public string LineErrorMessage { get; set; }
    }
}