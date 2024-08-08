using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource
{
    public class WorkplanHeaderModel
    {
        [JsonProperty("Workplan_No")]
        public string WorkplanNo { get; set; } // DataItem 0 - Column 0
    
        [JsonProperty("Workplan_Date")]
        public DateTime WorkplanDate { get; set; } // DataItem 0 - Column 1
    
        [JsonProperty("Staff_No")]
        public string StaffNo { get; set; } // DataItem 0 - Column 2
    
        [JsonProperty("Staff_Name")]
        public string StaffName { get; set; } // DataItem 0 - Column 3
    
        [JsonProperty("Period")]
        public string Period { get; set; } // DataItem 0 - Column 4
    
        [JsonProperty("Staff_Department")]
        public string StaffDepartment { get; set; } // DataItem 0 - Column 5
    
        [JsonProperty("No_Series")]
        public string NoSeries { get; set; } // DataItem 0 - Column 6
    
        [JsonProperty("User")]
        public string User { get; set; } // DataItem 0 - Column 7
    
        [JsonProperty("Status")]
        public string Status { get; set; } // DataItem 0 - Column 8
    
        [JsonProperty("Directorate")]
        public string Directorate { get; set; } // DataItem 0 - Column 9
    
        [JsonProperty("Director")]
        public string Director { get; set; } // DataItem 0 - Column 10
    
        [JsonProperty("Type")]
        public string Type { get; set; } // DataItem 0 - Column 11

        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
        public IEnumerable<SelectListItem> PeriodSelect { get; set; }
        public IEnumerable<SelectListItem> Typeselect { get; set; }
    }
}