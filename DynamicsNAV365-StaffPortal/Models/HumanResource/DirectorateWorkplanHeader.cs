using Newtonsoft.Json;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource
{
    public class DirectorateWorkplanHeader
    {
        [JsonProperty("No")]
        public string Number { get; set; }
    
        [JsonProperty("Workplan_Period")]
        public string WorkplanPeriod { get; set; }
    
        [JsonProperty("User")]
        public string User { get; set; }
    
        [JsonProperty("Designation")]
        public string Designation { get; set; }
    
        [JsonProperty("Directorate")]
        public string Directorate { get; set; }
    }
}