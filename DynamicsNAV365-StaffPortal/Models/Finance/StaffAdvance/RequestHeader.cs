using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DynamicsNAV365_StaffPortal.Models.Finance.StaffAdvance
{
    public class Odata<T>
    {
        [JsonProperty("value")] public List<T> ListValues { get; set; }
        [JsonProperty("@odata.context")] public string OdataContext { get; set; }
    }

    public class RequestHeader
    {
        public string No { get; set; }

        /*public string Code { get; set; }
        public DateTime RequestDate { get; set; }
        public string TripNo { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public DateTime TripStartDate { get; set; }
        public DateTime TripExpectedEndDate { get; set; }
        public decimal NumberOfDays { get; set; }
        public string GlobalDimension1Code { get; set; }
        public string NoSeries { get; set; }
        public DateTime DeadlineForReturn { get; set; }*/
        public string Status { get; set; }

        [JsonProperty("Global_Dimension_2_Code")]
        [DisplayName("Program")]
        public string GlobalDimension2Code { get; set; }

        [JsonProperty("Imprest_Amount")] public decimal ImprestAmount { get; set; }
        public decimal Balance { get; set; }
        [JsonProperty("Posted_Date")] public DateTime PostedDate { get; set; }
        [JsonProperty("Request_Date")] public DateTime RequestDate { get; set; }
        [JsonProperty("Employee_No")] public string EmployeeNo { get; set; }
        [JsonProperty("Employee_Name")]public string EmployeeName { get; set; }
        [JsonProperty("Purpose_of_Imprest")]public string PurposeOfImprest { get; set; }
        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
        [JsonProperty("Create_PV")]
        [DisplayName("Create PV")]
        public bool CreatePV { get; set; }

        public SelectList GlobalDimension2CodesSelect { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        [Display(Name = "Date From")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Imprest Date From. Required")]
        public string DateFrom { get; set; }

        [Display(Name = "To Date.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Imprest Date To. Required")]
        public string DateTo { get; set; }
    }

    public enum Status
    {
        Open,
        Released,
        [JsonProperty("Pending Approval")]
        PendingApproval,
        [JsonProperty("Pending Prepayment")]
        PendingPrepayment,
        Rejected
    }

    public class DateFormula
    {
        public string Formula { get; set; }
    }
}