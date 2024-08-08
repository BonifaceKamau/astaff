using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.PerformanceManagement
{
    public class AppraisalResultsModel
    {
        public string EmployeeName { get; set; } 
        public string EmployeeNo { get; set; }
        public string AppraisalNo { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string AppraisalPeriod { get; set; }
        public string AppraisalStage { get; set; }

        [Display(Name = "Final Performance Rating")]
        public decimal AppraisedScore { get; set; }

        [Display(Name = "Scores from Peers")]
        public decimal PeerLineScore { get; set; }

        [Display(Name = "Scores from Customer")]
        public decimal ExternalLineScore { get; set; }

        [Display(Name = "Sub-ordinate Scores")]
        public decimal SubordinateLineScore { get; set; }

        [Display(Name = "Core Performance Targets score")]
        public decimal PerformanceLineScore { get; set; }

        [Display(Name = "Core Competencies score")]
        public decimal CompetencyLineScore { get; set; }
        public bool Appeal { get; set; }

        public string AppraisedNarration { get; set; }
        public string ScoreGrading { get; set; }
        public string UserId { get; set; }
        public bool AcceptResults { get; set; }
        public bool DeclineResults { get; set; }

        [Display(Name = "Reason For Appeal")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Reason is  Required")]
        [StringLength(250, ErrorMessage = "This field should not exceed 250 characters")]
        public string AppealReason { get; set; }

        [Display(Name = "Reason For Declining")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Reason is  Required")]
        [StringLength(250, ErrorMessage = "This field should not exceed 250 characters")]
        public string DeclineReason { get; set; }


    }
}