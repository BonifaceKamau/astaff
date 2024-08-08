using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.StaffRequisition
{
    public class AcademicRequirements
    {
        public int LineNo { get; set; }
        public string DocumentNo { get; set; }
        public string Code { get; set; }
        public SelectList AcademicLevels { get; set; }
        public string Description1 { get; set; }
        public bool LineErrorStatus { get; set; } 
    }
}