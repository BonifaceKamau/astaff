using System.Collections.Generic;
using System.Web.Mvc;
using OdataRef;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource
{
    public class Appraisal: Staff_Appraisal_Header
    {
        public bool ErrorStatus { get; set; }
        public string errorMessage { get; set; }
        public IEnumerable<SelectListItem> PeriodSelect { get; set; }
        public IEnumerable<SelectListItem> TypeSelect { get; set; }
    }
}