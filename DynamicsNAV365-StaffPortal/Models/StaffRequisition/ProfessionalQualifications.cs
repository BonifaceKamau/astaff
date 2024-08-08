using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.StaffRequisition
{
    public class ProfessionalQualifications
    {
        public  int LineNo { get; set; }
        public string DocumentNo { get; set; }
        public string Code { get; set; }
        public SelectList ProfessionalLevels { get; set; }  
        public string Description2 { get; set; } 
        public bool LineErrorStatus { get; set; }
    }
}