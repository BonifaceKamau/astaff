using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DynamicsNAV365_StaffPortal.Models
{
    public class MidYearAppraisal: OdataRef.MidYearAppraisal
    {
        /*[JsonProperty("No")]
        public string No { get; set; }
    
        [JsonProperty("Staff_No")]
        public string StaffNo { get; set; }
    
        [JsonProperty("Staff_Name")]
        public string StaffName { get; set; }
    
        [JsonProperty("Directorate")]
        public string Directorate { get; set; }
    
        [JsonProperty("Department")]
        public string Department { get; set; }
    
        [JsonProperty("Period")]
        public string Period { get; set; }
    
        [JsonProperty("Created_On")]
        public string CreatedOn { get; set; }
    
        [JsonProperty("Created_By")]
        public string CreatedBy { get; set; }
    
        [JsonProperty("Supervisor")]
        public string Supervisor { get; set; }
    
        [JsonProperty("Supervisor_Name")]
        public string SupervisorName { get; set; }
    
        [JsonProperty("Period_Desc")]
        public string PeriodDesc { get; set; }
    
        [JsonProperty("Designation")]
        public string Designation { get; set; }
    
        [JsonProperty("Sent_to_Supervisor")]
        public string SentToSupervisor { get; set; }
    
        [JsonProperty("Approved_By_Supervisor")]
        public string ApprovedBySupervisor { get; set; }
    
        [JsonProperty("Employee_Comments")]
        public string EmployeeComments { get; set; }
    
        [JsonProperty("Supervisor_Comments")]
        public string SupervisorComments { get; set; }
    
        [JsonProperty("Date")]
        [DataType(DataType.Date)]
        public string Date { get; set; }
    
        [JsonProperty("Status")]
        public string Status { get; set; }
    
        [JsonProperty("Date_Time_Sent_For_Approval")]
        public string DateTimeSentForApproval { get; set; }
    
        [JsonProperty("Date_Time_Approved")]
        public string DateTimeApproved { get; set; }*/

        public List<SelectListItem> PeriodSelect { get; set; }
        public string Type { get; set; }
        public List<SelectListItem> TypeSelect { get; set; }
        public bool? ErrorStatus { get; set; }
        public string errorMessage { get; set; }
    }
}