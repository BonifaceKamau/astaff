using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DynamicsNAV365_StaffPortal.Models.Procurement.BidAnalysis
{
    public class BidAnalysisHeaderModel
    {
        public string No { get; set; }

        [Display(Name = "Tender No")]
        public string TenderNo { get; set; }

        [Display(Name = "Reason for Award")]
        public string ReasonforAward { get; set; }
        public string AwardedSupplier { get; set; }
        public string AwardedSupplierName { get; set; }
        public string Amount { get; set; }

        //public string DocumentNo { get; set; }
        //public string Title { get; set; }
        //public string RequisitionNo { get; set; }
        //public string ProcurementPlanNo { get; set; }
        //public string SupplierCategory { get; set; }
        //public string ProcurementMethod { get; set; }
        //public string EvaluateBasedOn { get; set; }
        //public string CreationDate { get; set; }
        //public string AdvertStartDate { get; set; }
        //public string ClosingDateTime { get; set; }
        //public string TenderOpeningDate { get; set; }
        //public string SupplierCategoryDescription { get; set; }
        //public string EligibilityDescription { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}