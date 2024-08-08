using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.EmployeeTraining
{
	public class EmployeeTrainingNeedModel
	{
        [Display(Name ="Training Need No." )]
        public string ApplicationNo { get; set; }
       
        public string EmployeeNo { get; set; }

        public string EmployeeName { get; set; }

        [Display(Name = "Calender Year")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Calendar Year cannot be empty")]
        public string CalenderYear { get; set; }
        public SelectList YearCodes { get; set; }

     
        [Display(Name = "Development Need")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Development Need cannot be empty")]
        public string DevelopmentNeed { get; set; }

        [Display(Name = "Intervention Required")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Intervention Required cannot be empty")]
        public string InterventionRequired { get; set; }
        public SelectList InterventionsRequired { get; set; }


        [Display(Name = "Training Type.")]
        public string TrainingType;
        public SelectList TrainingTypes;

        [Display(Name = "Proposed Calendar Period")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Proposed calendar period cannot be empty")]
        public string ProposedPeriod { get; set; }
        public SelectList ProposedPeriods { get; set; }

       
        [Display(Name = "Estimated Cost.(Kenya Shilings)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Estimated cost cannot be empty")]
        public decimal EstimatedCost { get; set; }

       
        [Display(Name = "Training Venue")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Training venue cannot be empty")]
        public string TrainingLocation { get; set; }

        [Display(Name = "Start Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Start Date cannot be empty")]
        public string TrainingScheduledDate { get; set; }

        [Display(Name = "End Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "End Date cannot be empty")]
        public string TrainingScheduledDateTo { get; set; }
        
        [Display(Name = "CPD Points to be earned")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "CPD Points to be earned cannot be empty")]
        public decimal CPDPoints { get; set; }

        [Display(Name = "Training Provider")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Provider/vendor cannot be empty")]
        public string ProposedTrainingProvider { get; set; }

        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public bool ErrorStatus { get; set; }
        public string InterventionRequiredOther { get; set; }
        public string Objectives { get; set; }
        public string Description { get; set; }
	}
}