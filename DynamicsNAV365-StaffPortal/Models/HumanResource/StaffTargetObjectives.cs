using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource
{
    public class StaffTargetObjectives 
    {
        public string errorMessage{ get; set; }

        [JsonProperty("No")]
        public string Number { get; set; } // DataItem 0 - Column 0

        [JsonProperty("Staff_No")]
        public string StaffNo { get; set; } // DataItem 0 - Column 1

        [JsonProperty("Staff_Name")]
        public string StaffName { get; set; } // DataItem 0 - Column 2

        [JsonProperty("Directorate")]
        public string Directorate { get; set; } // DataItem 0 - Column 3

        [JsonProperty("Department")]
        public string Department { get; set; } // DataItem 0 - Column 4

        [JsonProperty("Period")]
        public string Period { get; set; } // DataItem 0 - Column 5

        [JsonProperty("Created_On")]
        public DateTime CreatedOn { get; set; } // DataItem 0 - Column 6

        [JsonProperty("Created_By")]
        public string CreatedBy { get; set; } // DataItem 0 - Column 7

        [JsonProperty("Supervisor")]
        public string Supervisor { get; set; } // DataItem 0 - Column 8

        [JsonProperty("Supervisor_Name")]
        public string SupervisorName { get; set; } // DataItem 0 - Column 9

        [JsonProperty("Period_Desc")]
        public string PeriodDesc { get; set; } // DataItem 0 - Column 10

        [JsonProperty("Designation")]
        public string Designation { get; set; } // DataItem 0 - Column 11

        [JsonProperty("HR_Approval_Status")]
        public string HRApprovalStatus { get; set; }

        [JsonProperty("Sent_to_Supervisor")]
        public bool SentToSupervisor { get; set; } // DataItem 0 - Column 12

        [JsonProperty("Accepted_By_Staff")]
        //public bool ApprovedBySupervisor { get; set; }
        public bool AcceptedByStaff { get; set; }

        [JsonProperty("Accepted_by_Supervisor")]
        public bool AcceptedbySupervisor { get; set; }

        [JsonProperty("Acknowledged_Staff")]
        public bool Acknowledged { get; set; }

        [JsonProperty("Approval_Status")]
        public string ApprovalStatus { get; set; } // DataItem 0 - Column 11

        [JsonProperty("Staff")]
        public string Staff { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }
        public List<SelectListItem> PeriodSelect { get; set; }
        public bool? ErrorStatus { get; set; }

        public bool isHr { get; set; }
       
        public string Type { get; set; }

        public List<SelectListItem> EmpManagers { get; set; }
        public List<SelectListItem> TypeSelect { get; set; }
    }
}