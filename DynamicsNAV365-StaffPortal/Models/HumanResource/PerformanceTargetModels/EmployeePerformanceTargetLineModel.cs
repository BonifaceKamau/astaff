using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.PerformanceTargetModels
{
    public class EmployeePerformanceTargetLineModel
    {
        public string LineNo { get; set; }
        public string AppraisalNo { get; set; }
        public string AgreedPerformanceTargets { get; set; }
        public string AgreedScore { get; set; }
        public string AgreedAssesmentResults { get; set; }
        public string CommentsAfterReview { get; set; } 
        public string AppraiseeComments { get; set; }
        public string KeyPerformanceIndicator { get; set; }
        public string KeyResultAreasOutput { get; set; }
        public string SelfAssessment { get; set; }
        public string SelfScore { get; set; }
        public string SupervisorAssessment { get; set; }
        public string SupervisorsScore { get; set; }
        public string SupervisorComments { get; set; }
        public string EndYearAssessment { get; set; }
        public string EndYearEvaluationComments { get; set; }
        public bool LineErrorStatus { get; set; }
        public string LineErrorMessage { get; set; }
    }
}