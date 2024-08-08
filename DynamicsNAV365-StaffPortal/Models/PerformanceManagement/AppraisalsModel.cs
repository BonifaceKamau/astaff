using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.PerformanceManagement
{
    public class AppraisalsModel
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

		[Display(Name = "Mid yr Self Score")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public decimal SelfAssessmentScore { get; set; }

		[Display(Name = "Appraisee Comments")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please use letters only")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string AppraiseeComments { get; set; }

		[Display(Name = "End-Yr Self Score")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public int EndYearSelfScore { get; set; }

		[Display(Name = "End-Yr Appraisee Comments")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please use letters only")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string EndYearAppraiseeComments { get; set; }

        [Display(Name = "Target Score")]
        public decimal TargetScore { get; set; }


        public int LineNo { get; set; }
		public string UserId { get; set; }

		public string AppraisalStatus { get; set; }

		//public string Department { get; set; }
		//public string DepartmentalObjective { get; set; }
		//public string PrimaryCode { get; set; }
		//public string TargetApprovalStatus { get; set; }


	}
}