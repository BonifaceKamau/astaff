namespace DynamicsNAV365_StaffPortal.Models.Procurement.Purchases
{
    public class PurchaseLineModel
    {
        public string LineNo { get; set; }
        public string DocumentNo { get; set; }
        public string DocumentType { get; set; }
        public string Type { get; set; }
        public string No { get; set; }
        public string LocationCode { get; set; }
        public string UnitCost { get; set; }
        public string LineDiscountAmount { get; set; }
        public string PlannedReceiptDate { get; set; }
        public string ExpectedReceiptDate { get; set; }
        public string LineAmount { get; set; }
        public string LineVATAmount { get; set; }
        public string LineAmountInclVAT { get; set; } 
        public string LineDescription { get; set; }
        public string LineDescription2 { get; set; }
        public string Qty { get; set; }
        public string QtyBase { get; set; }
        public string QtyInvoiced { get; set; }
        public string QtyReceived { get; set; }
    }
}