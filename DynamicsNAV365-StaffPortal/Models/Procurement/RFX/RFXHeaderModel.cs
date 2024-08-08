namespace DynamicsNAV365_StaffPortal.Models.Procurement.RFX
{
    public class RFXHeaderModel
    {
        public string No { get; set; }
        public string DocumentNo { get; set; }
        public string Title { get; set; }
        public string RequisitionNo { get; set; }
        public string ProcurementPlanNo { get; set; }
        public string SupplierCategory { get; set; }
        public string ProcurementMethod { get; set; }
        public string EvaluateBasedOn { get; set; }
        public string CreationDate { get; set; }
        public string AdvertStartDate { get; set; }
        public string ClosingDateTime { get; set; }
        public string TenderOpeningDate { get; set; }
        public string SupplierCategoryDescription { get; set; }
        public string EligibilityDescription { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}