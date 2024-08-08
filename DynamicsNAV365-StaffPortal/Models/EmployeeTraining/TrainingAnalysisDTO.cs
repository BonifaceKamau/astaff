using System.Collections.Generic;
using System.Web.Mvc;
using OdataRef;

namespace DynamicsNAV365_StaffPortal.Models.EmployeeTraining
{
    public class TrainingAnalysisDTO:TrainingAnalysis
    {
        public IEnumerable<SelectListItem> YearCodes { get; set; }
    }
}