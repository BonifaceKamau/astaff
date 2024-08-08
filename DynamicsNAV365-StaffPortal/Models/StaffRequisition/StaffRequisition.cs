using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.StaffRequisition
{
    public class StaffRequisition
    {
        public string No { get; set; }
        public string JobNo { get; set; }
        public SelectList Establishments { get; set; } 
        public string JobTitle { get; set; }
        public string RequisitionType { get; set; }
        public SelectList RequisitionTypes { get; set; } 
        public string AppointmentType { get; set; }
        public SelectList AppointmentTypes { get; set; }
        public string ExpectedReportingDate { get; set; }
        public string RejectionComments { get; set; } 
        public string Description { get; set; }
        public string Positions { get; set; }
        public string PositionType { get; set; }
        public SelectList PositionTypes { get; set; }
        public string Status { get; set; }
        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}