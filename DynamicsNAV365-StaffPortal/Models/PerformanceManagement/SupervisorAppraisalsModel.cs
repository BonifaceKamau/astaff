using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.PerformanceManagement
{
    public class SupervisorAppraisalsModel

    {
		[Display(Name = "Perspective")]
		public string Perspective { get; set; } 
		[Display(Name = "Objective")]
		public string Objective { get; set; }

		[Display(Name = "Project")]
		public string Project { get; set; }

		[Display(Name = "Activity")]
		public string Activity { get; set; }

		[Display(Name = "Appraisal Period")]
		public string AppraisalPeriod { get; set; }

        [Display(Name = "Appraisal Quarter")]
        public string AppraisalQuarter { get; set; }

        [Display(Name = "Mid yr Self Score")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public decimal SelfAssessmentScore { get; set; }

		[Display(Name = "Appraisee Comments")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please use letters only")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Appraisee Comment is required")]
        public string AppraiseeComments { get; set; }

		[Display(Name = "End-Yr Self Score")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "End Year self Score is required")]
        public int EndYearSelfScore { get; set; }

		[Display(Name = "End-Yr Appraisee Comments")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please use letters only")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public string EndYearAppraiseeComments { get; set; }

		[Display(Name = "Target Score")]
		public decimal TargetScore { get; set; }

        //Supervisor Fields
        [Display(Name = "Mid-Yr Supervisor Score")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public decimal MidYrSupervisorScore { get; set; }

        [Display(Name = "Mid-Yr Agreed Score")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please fill in the agreed score for Mid Year")]
        public decimal MidYrAgreedScore { get; set; }

        [Display(Name = "Mid-Yr Supervisor Comments")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please use letters only")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string MidYrSupervisorComments { get; set; }  

        [Display(Name = "End-Yr Supervisor Score")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public int EndYrSupervisorScore { get; set; }

        [Display(Name = "End-Yr Supervisor Comments")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please use letters only")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string EndYrSupervisorComments { get; set; }

        [Display(Name = "End-Yr Agreed Score")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public decimal EndYrAgreedScore { get; set; }

        [Display(Name = "End-Yr Assessment Comments")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please use letters only")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string EndYrAssessmentComments { get; set; }


        [Display(Name = "Description")]
        public string CoreDescription { get; set; }

        [Display(Name = "Appraisee Score")]
        public decimal CAppScore { get; set; }

        [Display(Name = "Appraisee Comments")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please use letters only")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string CAppComments { get; set; }

        [Display(Name = "Supervisor Score")]
        public decimal CSuperScore { get; set; }

        [Display(Name = "Agreed Score")]
        public decimal CAgreedScore { get; set; }

        [Display(Name = "Score Descriptors")]
        public string ScoreDescriptors { get; set; }

        public string CoreCode { get; set; }
        public int SupLineNo { get; set; }
       
        [Display(Name="A Peer")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Assign a Peer")]
        public string PeerAppraiser { get; set; }

        public IEnumerable<SelectListItem> AppraiserCodes { get; set; }

        [Display(Name = "Internal/External Customer")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please a Person to evaluate as an Internal/External Customer")]
        public string CustomerAppraiser { get; set; }

        [Display(Name = "A Subordinate")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Assign a subordinate Member")]
        public string SubordinateAppraiser { get; set; } 

        public string EmployeeName { get; set; }
        public string EmployeeNo { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string AppraisalStage { get; set; }
        public string SupervisorName { get; set; }
        public string AppraisalNo { get; set; }
        public string AppraisalStatus { get; set; }
        public string SupervisorNo { get; set; }

        public bool Appeal { get; set; }
        public int LineNo { get; set; }
		public string UserId { get; set; }
	}
}