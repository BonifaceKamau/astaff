using System;
using Newtonsoft.Json;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource
{
    public class WorkplanLine
    {
        [JsonProperty("No")] public string Number { get; set; }

        [JsonProperty("Performance_Objective")]
        public string PerformanceObjective { get; set; }

        [JsonProperty("Project")] public string Project { get; set; }

        [JsonProperty("Activity")] public string Activity { get; set; }

        [JsonProperty("Performance_Measure_Indicator")]
        public string PerformanceMeasure { get; set; }

        [JsonProperty("Completion_Date_Workplan")]
        public DateTime CompletionDateWorkplan { get; set; }

        [JsonProperty("Frequency_Reporting")] public string FrequencyReporting { get; set; }

        [JsonProperty("Department")] public string Department { get; set; }

        [JsonProperty("Perspective")] public string Perspective { get; set; }

        [JsonProperty("Period")] public string Period { get; set; }

        [JsonProperty("Targets")] public string Targets { get; set; }

        [JsonProperty("Responsible_Person")] public string ResponsiblePerson { get; set; }

        [JsonProperty("No_Series")] public string NumberSeries { get; set; }

        [JsonProperty("Output")] public string Output { get; set; }

        [JsonProperty("Objective_Nos")] public string ObjectiveNumbers { get; set; }

        [JsonProperty("Performance_Outcome")] public string PerformanceOutcome { get; set; }

        [JsonProperty("Completion_Date")] public DateTime CompletionDate { get; set; }

        [JsonProperty("Weight_Total")] public int WeightTotal { get; set; }

        [JsonProperty("Header_No")] public string HeaderNumber { get; set; }

        [JsonProperty("CWP_Line")] public string CWLine { get; set; }

        [JsonProperty("Director")] public string Director { get; set; }

        [JsonProperty("Directorate")] public string Directorate { get; set; }

        [JsonProperty("Departmental_Objective")]
        public string DepartmentalObjective { get; set; }

        [JsonProperty("Designation")] public string Designation { get; set; }

        [JsonProperty("Target_Score")] public int TargetScore { get; set; }

        [JsonProperty("Staff_No")] public string StaffNumber { get; set; }

        [JsonProperty("TargetLine_No")] public string TargetLineNumber { get; set; }

        [JsonProperty("No_Series_Target")] public string NumberSeriesTarget { get; set; }
    }
}