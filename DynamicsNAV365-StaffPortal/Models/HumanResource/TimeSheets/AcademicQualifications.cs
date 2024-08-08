using System.Collections.Generic;
using System.Web.Mvc;
using OdataRef;
using OdataRefV4.NAV;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.TimeSheets
{
    public class AcademicQualifications:NeedsRequirements
    {
        public IEnumerable<SelectListItem> Education_Level_Id_Select { get; set; }
        public IEnumerable<SelectListItem> Course_Id_Select { get; set; }
    }
}