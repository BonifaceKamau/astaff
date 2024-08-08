using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using System.ComponentModel.DataAnnotations;

namespace DynamicsNAV365_StaffPortal.Models.Procurement.Purchases
{
    public class PurchaseHeaderModel
    {
        public string No { get; set; }
        public string VendorNo { get; set; }
        public string VendorName { get; set; }
        public string DocumentType { get; set; }
        public string PurchaseType { get; set; }
        public string InvoiceNo { get; set; }
        public string Amount { get; set; }
        public string AmountIncVAT { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public string GlobalDimension1 { get; set; }

        [Display(Name = Dimensions.GlobalDimension2Code)]
        public string GlobalDimension2 { get; set; }
        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}