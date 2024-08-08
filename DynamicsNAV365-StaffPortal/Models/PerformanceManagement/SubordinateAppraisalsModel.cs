using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.PerformanceManagement
{
    public class SubordinateAppraisalsModel
    {
        [Display(Name = "Subordinate Rating")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public decimal SubordinateRating { get; set; }

        [Display(Name = "Subordinate Comments")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please use letters only")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string SubordinateComments { get; set; }

        [Display(Name = "Goals & Targets")]
        public string SubGoalsTargets { get; set; }

        public int LineNo { get; set; }

        public string UserId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNo { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string AppraisalStage { get; set; }
        public string ExternalSourceNo { get; set; }
        public string AppraisalNo { get; set; }
        public string AppraisalStatus { get; set; }
        public string AssignedToNo { get; set; }
        public string AppraisalPeriod { get; set; }
        public bool Appeal { get; set; }
    }
}