using System.Collections.Generic;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource
{
    public class StaffRecruitmentNeeds: OdataRef.RecruitmentNeeds
    {
        public IEnumerable<SelectListItem> Appointment_Type_Select { get; set; }
        public IEnumerable<SelectListItem> Job_ID_Select { get; set; }
        public IEnumerable<SelectListItem> Global_Dimension_1_Code_Select { get; set; }
        public IEnumerable<SelectListItem> Reason_for_Recruitment_Select { get; set; }
        public IEnumerable<SelectListItem> Requisition_Type_Select { get; set; }
        public IEnumerable<SelectListItem> Location_Select { get; set; }
        public IEnumerable<SelectListItem> Reporting_To_Select { get; set; }
    }
}