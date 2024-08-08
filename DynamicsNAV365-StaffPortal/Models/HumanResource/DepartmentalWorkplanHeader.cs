using Newtonsoft.Json;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource
{
    public class DepartmentalWorkplanHeader
    {
        [JsonProperty("No")]
        public string Number { get; set; }

        [JsonProperty("Department")]
        public string Department { get; set; }

        [JsonProperty("Workplan_Period")]
        public string WorkplanPeriod { get; set; }

        [JsonProperty("Directorate")]
        public string Directorate { get; set; }

        [JsonProperty("Designation")]
        public string Designation { get; set; }

        [JsonProperty("User")]
        public string User { get; set; }
    }
}