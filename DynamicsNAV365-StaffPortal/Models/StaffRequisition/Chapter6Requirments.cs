using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.StaffRequisition
{
    public class Chapter6Requirments
    {
        public int LineNo { get; set; }
        public string DocumentNo { get; set; }
        public string Code { get; set; }
        public SelectList Codes { get; set; }
        public string Description5{ get; set; }  
        public bool Mandatory { get; set; }  
        public bool LineErrorStatus { get; set;}
    }
}