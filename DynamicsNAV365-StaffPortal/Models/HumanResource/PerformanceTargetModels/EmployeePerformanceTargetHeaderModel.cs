using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.PerformanceTargetModels
{
    public class EmployeePerformanceTargetHeaderModel
    {
        public string No { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public string SupervisorNo { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorUserID { get; set; }
        public string AppraisalType { get; set; }
        public string AppraisalPeriod { get; set; }
        public string AppraisalStage { get; set; }
        public string UserID { get; set; }
        public string DateofFirstAppointment { get; set; }
        public string Designation { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string CommentsAppraisee { get; set; }
        public string CommentsAppraiser { get; set; }
        public string DocumentDate { get; set; }
        public string EvaluationPeriodStartDate { get; set; }
        public string EvaluationPeriodEndDate { get; set; }
        public string TargetType { get; set; }
        public string FinalScores { get; set; }
        public string TotalScores { get; set; }
        public string RatingRemarks { get; set; }
        public string Email { get; set; }
        public string AppraisalStatus { get; set; }
        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; } 
    }
}