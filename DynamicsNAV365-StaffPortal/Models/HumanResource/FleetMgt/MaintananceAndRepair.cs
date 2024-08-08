using System.Collections.Generic;
using System.Web.Mvc;
using OdataRef;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.FleetMgt
{
    public class MaintananceAndRepair:Maintanance_and_repair
    {
        public IEnumerable<SelectListItem> Service_Type_Select { get; set; }
        public IEnumerable<SelectListItem> Service_Provider_Select { get; set; }
        public IEnumerable<SelectListItem> Service_Interval_Type_Select { get; set; }
        public IEnumerable<SelectListItem> Reg_No_Select { get; set; }
    }
}