using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.TimeSheets
{
    public class TimeSheetModel
    {
        public string No { get; set; }
        public string LineNo { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string JobTitle { get; set; }
        public string JobTitleCode { get; set; }
        public string Status { get; set; }
        public string Dimension1 { get; set; }
        public string Dimension2 { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Category { get; set; }
        public string HoursWorked { get; set; }
        public string TotalDaysWorked { get; set; }
        public string PeriodStartDate { get; set; }
        public string PeriodEndDate { get; set; }
        public string SupervisorNo { get; set; }
        public string ApprovalStatus { get; set; }

        public string Location { get; set; }
    }

    public class TimeSheetModelHeader
    {
        public string No { get; set; }
        public string LineNo { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string JobTitle { get; set; }
        public string JobTitleCode { get; set; }
        public string Status { get; set; }
        public string Dimension1 { get; set; }
        public string Dimension2 { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Category { get; set; }
        public string HoursWorked { get; set; }
        public string TotalDaysWorked { get; set; }
        public string PeriodStartDate { get; set; }
        public string PeriodEndDate { get; set; }
        public string SupervisorNo { get; set; }
        public string ApprovalStatus { get; set; }
        public string ListofTasks { get; set; }
        public string Comments { get; set; }

        public string Location { get; set; }
        public Boolean ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class TimeSheetModelLines
    {
        public string Description { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Date { get; set; }
        public string Hours { get; set; }
        public string NonWorkingDay { get; set; }
        public string DayType { get; set; }
        public string Day { get; set; }
        public string LineNo { get; set; }
        public string DocNo { get; set; }
        public string LedgerNo { get; set; }
        public string ListofTasks { get; set; }
        public string ApprovalStatus { get; set; }




    }
    public class TasksUndertakenModel
    {
        public string No { get; set; }
        public string LineNo { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string Dimension2 { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Category { get; set; }
        public string HoursWorked { get; set; }
        public string TasksUndertaken { get; set; }
        public string DocNo { get; set; }
        public string ApprovalStatus { get; set; }
    }
    public class TimeSheetLinesList
    {
        public List<TimeSheetModelHeader> ListofTimeSheetHeader { get; set; }
        public List<TimeSheetModelLines> ListOfTimeSheetLines { get; set; }
        public List<TasksUndertakenModel> ListOfTimeSheetTasks { get; set; }
    }
    public class ProgramsList
    {
        public string Code { get; set; }
        public List<SelectListItem> ListofPrograms { get; set; }
    }
 }