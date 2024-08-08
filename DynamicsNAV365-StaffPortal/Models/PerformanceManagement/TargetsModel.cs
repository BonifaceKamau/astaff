using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.PerformanceManagement
{
    public class TargetsModel
    {
		[Display(Name = "Performance Objective")]
		public string PerformanceObjective { get; set; }		

		[Display(Name = "Project")]
		public string Project { get; set; }

		[Display(Name = "Activity")]
		public string Activity { get; set; }

		[Display(Name = "Performance Measure/Indicator")]
		public string PMI { get; set; } 

		[Display(Name = "Performance Outcome")]
		public string PerformanceOutcome { get; set; }

		[Display(Name = "Completion Date")]		
		[Required(AllowEmptyStrings = false, ErrorMessage = "Completion Date Required")]
		public string CompletionDate { get; set; }

		[Display(Name = "Weight Total")]
		public decimal? WeightTotal { get; set; }

        [Display(Name = "Targets")]
        public string Targets { get; set; }

        [Display(Name = "Target Score")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is  Required")]
        public int TargetScore { get; set; }

        [Display(Name = "Perspective")]
		public string Perspective { get; set; }
		

		[Display(Name = "Directorate")]
		public string Directorate { get; set; }

		[Display(Name = "Designation")]
		public string Designation { get; set; }
		public string Department { get; set; }
		public string DepartmentalObjective { get; set; }
        public string PrimaryCode { get; set; }
        public string TargetApprovalStatus { get; set; }

        public string UserId { get; set; }
		
	}
}