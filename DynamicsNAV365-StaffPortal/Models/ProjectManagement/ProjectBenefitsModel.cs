using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.ProjectManagement
{
    public class ProjectBenefitsModel
    {
        [Display(Name = "Project No")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Project Number is Required")]
        public string ProjectNo { get; set; }

        [Display(Name = "Benefit")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public string BenefitText { get; set; }

        [Display(Name = "Objective Supported")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public string ObjectiveSupported { get; set; }

        [Display(Name = "Benefit Owner")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public string BenefitOwner { get; set; }

        [Display(Name = "Beneficiaries")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public string BeneficiariesText { get; set; }


        [Display(Name = "KPI")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "KPI is required")]
        public string KPIText { get; set; }

        [Display(Name = "Measure")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Measure is required")]
        public string MeasureText { get; set; }

        [Display(Name = "Frequency")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Frequency is required")]
        public string FrequencyText { get; set; }

        [Display(Name = "Benefit Value")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Benefit is required")]
        public string BenefitValue { get; set; } 

        [Display(Name = "Base Line Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please fill in the Baseline Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? BaselineDate { get; set; }

        [Display(Name = "Actual Realisation Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Actual Realisation Date is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? ActualRealisationDate { get; set; }

        [Display(Name = "Target Value")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Target Value is required")]
        public string TargetValue { get; set; }

        [Display(Name = "Realisation Status")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Realisation Status is required")]
        public string RealisationStatus { get; set; }

        [Display(Name = "Assumption")]
        [StringLength(250, ErrorMessage = "This field should not exceed 250 characters")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "This  field is required")]
        public string AssumptionsText { get; set; }

        [Display(Name = "Benefit Risk")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "This  field is required")]
        public string BenefitRisks { get; set; }

        [Display(Name = "Notes")]
        [StringLength(250, ErrorMessage = "This field should not exceed 250 characters")]
        public string NotesText { get; set; }
        
        public string OnlineFile { get; set; }
        public int LineNo { get; set; }

    }
}