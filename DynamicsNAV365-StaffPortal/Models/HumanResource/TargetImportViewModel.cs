using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource
{
    public class TargetImportViewModel
    {
        [Required]
        public string Doc_No { get; set; }
        
        [Required]
        public string Period { get; set; }
        public string KPIdesc { get; set; }
        public string No { get; set; }
        
        public string Command { get; set; }
        
        [Required(ErrorMessage = "Please select a file to upload.")]
        public HttpPostedFileBase UploadedFile { get; set; }
        
        public bool ErrorStatus { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}