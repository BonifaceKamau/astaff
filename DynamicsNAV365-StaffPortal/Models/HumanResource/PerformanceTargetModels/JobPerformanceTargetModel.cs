using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.PerformanceTargetModels
{
    public class JobPerformanceTargetModel
    {
        public string AppraisalPeriod { get; set; }
        public string ObjectiveCode { get; set; }
        public string ObjectiveDescription { get; set; }
        public string PerspectiveType { get; set; }
        public string DepartmentName { get; set; }
        public string PerspectiveDescription { get; set; }
    }
}