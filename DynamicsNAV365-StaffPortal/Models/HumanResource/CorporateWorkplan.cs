using System;
using Newtonsoft.Json;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource
{
    public class CorporateWorkplan
    {
        [JsonProperty("No")]
        public string Number { get; set; }

        [JsonProperty("Performance_Objective")]
        public string PerformanceObjective { get; set; }

        [JsonProperty("Project")]
        public string Project { get; set; }

        [JsonProperty("Activity")]
        public string Activity { get; set; }

        [JsonProperty("Performance_Measure_Indicator")]
        public string PerformanceMeasure { get; set; }

        [JsonProperty("Completion_Date")]
        public DateTime CompletionDate { get; set; }

        [JsonProperty("Frequency_Reporting")]
        public string FrequencyReporting { get; set; }

        [JsonProperty("Department")]
        public string Department { get; set; }

        [JsonProperty("Perspective")]
        public string Perspective { get; set; }

        [JsonProperty("Period")]
        public string Period { get; set; }

        [JsonProperty("Targets")]
        public string Targets { get; set; }

        [JsonProperty("Responsible_Person")]
        public string ResponsiblePerson { get; set; }

        [JsonProperty("No_Series")]
        public string NumberSeries { get; set; }

        [JsonProperty("Output")]
        public string Output { get; set; }

        [JsonProperty("Objective_Nos")]
        public string ObjectiveNumbers { get; set; }
    }
}