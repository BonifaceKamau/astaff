using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.ProjectManagement
{
    public class PlanActivitiesModel
    {
      
        public string Code { get; set; }
        public int LineNo { get; set; }
   
        public IEnumerable<SelectListItem> StrategicObjectiveCodes { get; set; }

        [Display(Name = "Activity")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is Required")]
        public string Activity { get; set; }

        [Display(Name = "Start Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please fill in the Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        //public string RiskStartDate { get; set; }

        [Display(Name = "End Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please fill in the End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }



        [Display(Name = "Status")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Status is Required")]
        public string Status { get; set; }

        [Display(Name = "Strategic Plan")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Please select a strategic Plan")]
        public string StrategicPlan { get; set; }
        public IEnumerable<SelectListItem> StrategicPlanCodes { get; set; }



        [Display(Name = "Strategic Objective")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "This field is Required")]
        public string StrategicObjective { get; set; }


        public string ProjectNumber { get; set; }

      

    }
}