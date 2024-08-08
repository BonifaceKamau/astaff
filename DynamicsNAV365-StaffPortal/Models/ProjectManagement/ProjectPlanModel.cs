using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.ProjectManagement
{
    public class ProjectPlanModel
    {
        public string Code { get; set; }

        [Display(Name = "Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is Required")]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Status is Required")]
        public string Status { get; set; }

        [Display(Name = "Project Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Project Number is Required")]
        public string ProjectNumber { get; set; }

        [Display(Name = "Project Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Project Name is Required")]
        public string ProjectName { get; set; }

        [Display(Name = "Approval Status")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Approval Status is Required")]
        public string ApprovalStatus { get; set; }

        [Display(Name = "Start Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please fill in the Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }


        [Display(Name = "End Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please fill in the End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

    }
}