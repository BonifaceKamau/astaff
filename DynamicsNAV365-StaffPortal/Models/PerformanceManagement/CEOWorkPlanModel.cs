using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.PerformanceManagement
{
    public class CEOWorkPlanModel
    {
		public string No { get; set; }
        public int DirectorCount { get; set; }

		[Display(Name = "Performance Objective")]
		public string PerformanceObjective { get; set; }
		public IEnumerable<SelectListItem> PerformanceObjectiveCodes { get; set; }

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

        //[Display(Name = "Completion Date")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Completion Date Required")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        //public DateTime? CompletionDate { get; set; }


        [Display(Name = "Weight Total")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Weight Required")]
        public decimal ? WeightTotal { get; set; }

		[Display(Name = "Perspective")]
		public string Perspective { get; set; }
        public IEnumerable<SelectListItem> PerspectiveCodes { get; set; }

        [Display(Name = "Directorate")]
		public string Directorate { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }
        public string Department { get; set; }

		[Display(Name = "Departmental Objective")]
		public string DepartmentalObjective { get; set; }
        //[Display(Name = "Designation")]
        public string UserId { get; set; }
        public string Code { get; set; }
		public string HeaderNo { get; set; }


	}
}