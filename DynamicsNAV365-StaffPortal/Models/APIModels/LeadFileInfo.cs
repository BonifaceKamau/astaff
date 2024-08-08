using System;

namespace DynamicsNAV365_StaffPortal.Models.APIModels
{
    public class LeadFileInfo
    {
        public string customer_id { get; set; }
        public string Booking_id { get; set; }
        public string Plot_number { get; set; }
        public string Marketer { get; set; }
        public decimal? InterestAmount { get; set; }
        public int? NoOfInstallments { get; set; }
        public DateTime? BookingDate { get; set; }
        public string purchase_type { get; set; }
    }
}