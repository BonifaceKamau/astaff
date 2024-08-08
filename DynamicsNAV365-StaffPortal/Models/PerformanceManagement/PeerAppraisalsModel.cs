using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.PerformanceManagement
{
    public class PeerAppraisalsModel
    {
        [Display(Name = "Peer Rating")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public decimal PeerRating { get; set; }

        [Display(Name = "Peer Comments")]       
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please use letters only")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string PeerComments { get; set; }

        [Display(Name = "Goals & Targets")]
        public string GoalsTargets { get; set;}

        public int LineNo { get; set; }

        public string UserId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNo { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string AppraisalStage { get; set; }
        public string PeerNumber { get; set; }
        public string AppraisalNo { get; set; }
        public string AppraisalStatus { get; set; }
        public string SupervisorNo { get; set; }
        public string AppraisalPeriod { get; set; }
        public bool Appeal { get; set; }

    }
}