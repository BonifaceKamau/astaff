using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.ProjectManagement
{
    public class ProjectRisksModel
    {
        [Display(Name = "Project No")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Project Number is Required")]
        public string ProjectCode { get; set; }

        [Display(Name = "Risk Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is Required")]
        public string RiskDescription { get; set; }

        [Display(Name = "Impact")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Impact is Required")]
        public string ImpactDescription { get; set; }

        [Display(Name = "Impact Level")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Indicate the Impact Level")]
        public int ImpactLevel { get; set; }

        [Display(Name = "Probability Level")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Probability Level is Required")]
        public int ProbabilityLevel { get; set; }

        [Display(Name = "Priority Level")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Priority Level is Required")]
        public int PriorityLevel { get; set; }

        [Display(Name = "Mitigation Notes")]
        [StringLength(250,ErrorMessage = "This field should not exceed 250 characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")] 
        public string MitigationNotes { get; set; }


        public string OwnerCode { get; set; }

        [Display(Name = "Owner Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Owner name is Required")]
        public string OwnerName { get; set; }

        [Display(Name = "Objective Related")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is Required")]
        public string ObjectiveRelated { get; set; }

        [Display(Name = "Risk Start Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please fill in the Risk Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? RiskStartDate { get; set; }
        //public string RiskStartDate { get; set; }

        [Display(Name = "Risk End Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please fill in the Risk End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? RiskEndDate { get; set; }
        //public string RiskEndDate { get; set; }

        public string OnlineFile { get; set; }
        public int LineNo { get; set; }

    }
}