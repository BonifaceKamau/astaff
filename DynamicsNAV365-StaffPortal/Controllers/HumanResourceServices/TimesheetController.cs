using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.Models.HumanResource.TimeSheets;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using System.Net.NetworkInformation;


namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    public class TimesheetController : Controller
    {
        // GET: Timesheet
        public ActionResult Index()
            //string ApprovedTimesheet
        {
            AccountController accountController = new AccountController();
            string employeeNo = "";

            employeeNo = AccountController.GetEmployeeNo();

            List<TimeSheetModel> Leavetypes = new List<TimeSheetModel>();
            string pageData = "EmployeeTimeSheets?$filter=Employee_No eq '" + employeeNo + "' &$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(pageData);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    TimeSheetModel DList1 = new TimeSheetModel();
                    DList1.No = (string)config1["TS_No"];
                    DList1.EmpName = (string)config1["Employee_Name"];
                    DList1.EmpNo = employeeNo;
                    DList1.JobTitle = (string)config1["Job_Title"];
                    DList1.Location = (string)config1["Location"];
                    DList1.Dimension1 = (string)config1["Global_Dimension_1_Code"];
                    DList1.Dimension2 = (string)config1["Global_Dimension_2_Code"];
                    DList1.Month = (string)config1["Month"];
                    DList1.Year = (string)config1["Year"];
                    DList1.Category = (string)config1["Category"];
                    DList1.HoursWorked = (string)config1["Hours_Worked"];
                    DList1.TotalDaysWorked = (string)config1["Total_Days_Worked"];
                    DList1.PeriodStartDate = (string)config1["Period_Start_date"];
                    DList1.PeriodEndDate = (string)config1["Period_End_Date"];
                    DList1.ApprovalStatus = (string)config1["Approval_Status"];
                    DList1.LineNo = (string)config1["Line_No"];
                    Leavetypes.Add(DList1);
                }
            }
            ViewBag.ActiveTab = "IndexT";
            //if (!string.IsNullOrEmpty(ApprovedTimesheet))
            //    ViewBag.ActiveTab = "ApprovedTimesheet";
            return View("~/Views/Timesheet/TimeSheets.cshtml", Leavetypes);
        }

        public ActionResult SupervisorTimesheet()
        {
            AccountController accountController = new AccountController();
            string employeeNo = "";

            employeeNo = AccountController.GetEmployeeNo();

            List<TimeSheetModel> Leavetypes = new List<TimeSheetModel>();
            string pageData = "EmployeeTimeSheets?$filter=Supervisor_No eq '" + employeeNo + "' and Approval_Status eq 'Supervisor' &$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(pageData);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    TimeSheetModel DList1 = new TimeSheetModel();
                    DList1.No = (string)config1["TS_No"];
                    DList1.EmpNo = (string)config1["Employee_No"];
                    DList1.EmpName = (string)config1["Employee_Name"];
                    DList1.JobTitle = (string)config1["Job_Title"];
                    DList1.Location = (string)config1["Location"];
                    DList1.Dimension1 = (string)config1["Global_Dimension_1_Code"];
                    DList1.Dimension2 = (string)config1["Global_Dimension_2_Code"];
                    DList1.Month = (string)config1["Month"];
                    DList1.Year = (string)config1["Year"];
                    DList1.Category = (string)config1["Category"];
                    DList1.HoursWorked = (string)config1["Hours_Worked"];
                    DList1.TotalDaysWorked = (string)config1["Total_Days_Worked"];
                    DList1.PeriodStartDate = (string)config1["Period_Start_date"];
                    DList1.PeriodEndDate = (string)config1["Period_End_Date"];
                    DList1.ApprovalStatus = (string)config1["Approval_Status"];
                    DList1.LineNo = (string)config1["Line_No"];
                    Leavetypes.Add(DList1);
                }
            }
            return View("~/Views/Timesheet/SupervisorTimeSheetList.cshtml", Leavetypes);
        }
        public ActionResult ApprovedTimesheet()
        {
            AccountController accountController = new AccountController();
            string employeeNo = "";

            employeeNo = AccountController.GetEmployeeNo();

            List<TimeSheetModel> Leavetypes = new List<TimeSheetModel>();
            string pageData = "EmployeeTimeSheets?$filter=Employee_No eq '" + employeeNo + "' and Approval_Status eq 'Approved'&$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(pageData);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    TimeSheetModel DList1 = new TimeSheetModel();
                    DList1.No = (string)config1["TS_No"];
                    DList1.EmpName = (string)config1["Employee_Name"];
                    DList1.JobTitle = (string)config1["Job_Title"];
                    DList1.Location = (string)config1["Location"];
                    DList1.Dimension1 = (string)config1["Global_Dimension_1_Code"];
                    DList1.Dimension2 = (string)config1["Global_Dimension_2_Code"];
                    DList1.Month = (string)config1["Month"];
                    DList1.Year = (string)config1["Year"];
                    DList1.Category = (string)config1["Category"];
                    DList1.HoursWorked = (string)config1["Hours_Worked"];
                    DList1.TotalDaysWorked = (string)config1["Total_Days_Worked"];
                    DList1.PeriodStartDate = (string)config1["Period_Start_date"];
                    DList1.PeriodEndDate = (string)config1["Period_End_Date"];
                    DList1.ApprovalStatus = (string)config1["Approval_Status"];
                    DList1.LineNo = (string)config1["Line_No"];
                    Leavetypes.Add(DList1);
                }
            }
            ViewBag.ActiveTab = "ApprovedTimesheet";
            return View("~/Views/Timesheet/TimeSheets.cshtml", Leavetypes);
        }
        public ActionResult ViewSupervisorTimeSheet(string DocNo, string EmpNo)
        {
            TimeSheetModelHeader timesheetsheader = new TimeSheetModelHeader();
            AccountController accountController = new AccountController();
            string employeeNo = "";

            employeeNo = AccountController.GetEmployeeNo();

            List<TimeSheetModelHeader> Leavetypes = new List<TimeSheetModelHeader>();
            //string pageData = "EmployeeTimeSheets?$filter=TS_No eq '" + DocNo + "' and Line_No eq "+EmpNo+" &$format=json";
            string pageData = "EmployeeTimeSheets?$filter= Line_No eq " + DocNo + " and Employee_No eq '"+EmpNo+"' &$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(pageData);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    //TimeSheetModelHeader DList1 = new TimeSheetModelHeader();
                    timesheetsheader.No = (string)config1["TS_No"];
                    timesheetsheader.EmpNo = employeeNo;
                    timesheetsheader.EmpName = (string)config1["Employee_Name"];
                    timesheetsheader.JobTitle = (string)config1["Job_Title"];
                    timesheetsheader.Location = (string)config1["Location"];
                    timesheetsheader.Dimension1 = (string)config1["Global_Dimension_1_Code"];
                    timesheetsheader.Dimension2 = (string)config1["Global_Dimension_2_Code"];
                    timesheetsheader.Month = (string)config1["Month"];
                    timesheetsheader.Year = (string)config1["Year"];
                    timesheetsheader.Category = (string)config1["Category"];
                    timesheetsheader.HoursWorked = (string)config1["Hours_Worked"];
                    timesheetsheader.TotalDaysWorked = (string)config1["Total_Days_Worked"];
                    timesheetsheader.PeriodStartDate = (string)config1["Period_Start_date"];
                    timesheetsheader.PeriodEndDate = (string)config1["Period_End_Date"];
                    timesheetsheader.ApprovalStatus = (string)config1["Approval_Status"];
                    timesheetsheader.ListofTasks = (string)config1["List_of_Key_Tasks_Undertaken"];
                    timesheetsheader.LineNo = (string)config1["Line_No"];
                    //Leavetypes.Add(DList1);
                }
            }


            return View(timesheetsheader);
            //return View("~/Views/Timesheet/ViewTimeSheet.cshtml", Leavetypes);
        }
        public ActionResult ViewTimeSheet(string DocNo)
        {
            TimeSheetModelHeader timesheetsheader = new TimeSheetModelHeader();
            AccountController accountController = new AccountController();
            string employeeNo = "";

            employeeNo = AccountController.GetEmployeeNo();

            //List<TimeSheetModelHeader> Leavetypes = new List<TimeSheetModelHeader>();
            string pageData = "EmployeeTimeSheets?$filter=Line_No eq " + DocNo + " and Employee_No eq '" + employeeNo + "'&$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(pageData);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    //TimeSheetModelHeader DList1 = new TimeSheetModelHeader();
                    timesheetsheader.No = (string)config1["TS_No"];
                    timesheetsheader.EmpNo = employeeNo;
                    timesheetsheader.EmpName = (string)config1["Employee_Name"];
                    timesheetsheader.JobTitle = (string)config1["Job_Title"];
                    timesheetsheader.Location = (string)config1["Location"];
                    timesheetsheader.Dimension1 = (string)config1["Global_Dimension_1_Code"];
                    timesheetsheader.Dimension2 = (string)config1["Global_Dimension_2_Code"];
                    timesheetsheader.Month = (string)config1["Month"];
                    timesheetsheader.Year = (string)config1["Year"];
                    timesheetsheader.Category = (string)config1["Category"];
                    timesheetsheader.HoursWorked = (string)config1["Hours_Worked"];
                    timesheetsheader.TotalDaysWorked = (string)config1["Total_Days_Worked"];
                    timesheetsheader.PeriodStartDate = (string)config1["Period_Start_date"];
                    timesheetsheader.PeriodEndDate = (string)config1["Period_End_Date"];
                    timesheetsheader.ApprovalStatus = (string)config1["Approval_Status"];
                    timesheetsheader.ListofTasks = (string)config1["List_of_Key_Tasks_Undertaken"];
                    timesheetsheader.LineNo = (string)config1["Line_No"];
                    timesheetsheader.Comments = (string)config1["Comments"];
                    //Leavetypes.Add(DList1);
                }
            }


            return View(timesheetsheader);
            //return View("~/Views/Timesheet/ViewTimeSheet.cshtml", Leavetypes);
        }
        public PartialViewResult TimeSheetLinesView(string DocNo)
        {
            string employeeNo = "";

            employeeNo = AccountController.GetEmployeeNo();
            List<TimeSheetModelLines> ImpLines = new List<TimeSheetModelLines>();
            string pageLine = "TimesheetLedgerEntries?$filter=TS_No eq '" + DocNo + "' and Employee_No eq '" + employeeNo + "'&$format=json";
            HttpWebResponse httpResponseLine = Models.Credentials.GetOdataData(pageLine);
            using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    TimeSheetModelLines ImLine = new TimeSheetModelLines();
                    ImLine.Description = (string)config["Description"];
                    ImLine.Month = (string)config["Month"];
                    ImLine.Year = (string)config["Year"];
                    ImLine.Date = (string)config["Date"];
                    ImLine.Day = (string)config["Day"];
                    ImLine.Hours = (string)config["Hours"];
                    ImLine.LedgerNo = (string)config["Ledger_No"];
                    ImLine.DocNo = (string)config["TS_No"];
                    ImLine.DayType = (string)config["Day_Type"];
                    string nonworking = "";
                    if ((string)config["Non_Working_Day"] == "true")
                    {
                        nonworking = "Yes";
                    }
                    else
                    {
                        nonworking = "No";
                    }
                    ImLine.NonWorkingDay = nonworking;
                    ImLine.ApprovalStatus = (string)config["Apprv_Status"];
                    ImpLines.Add(ImLine);
                }
            }

            TimeSheetLinesList Lines = new TimeSheetLinesList
            {
                ListOfTimeSheetLines = ImpLines
            };
            return PartialView("~/Views/Timesheet/_ViewTimeSheetLine.cshtml", Lines);

        }
        public PartialViewResult TasksLineView(string DocNo, string Year, string Month, string TSLineNo)
        {
            //string employeeNo = "";
            int ln = Convert.ToInt32(TSLineNo);
            //employeeNo = AccountController.GetEmployeeNo();
            List<TasksUndertakenModel> ImpLines = new List<TasksUndertakenModel>();
            string pageLine = "TimesheetTasks?$filter=Timesheet_Code eq '" + DocNo + "' and TS_Line_No eq " + ln + " and Year eq '" + Year + "' and Month eq '" + Month + "'&$format=json";
            HttpWebResponse httpResponseLine = Models.Credentials.GetOdataData(pageLine);
            using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    TasksUndertakenModel ImLine = new TasksUndertakenModel();
                    ImLine.Dimension2 = (string)config["Shortcut_Dimension_2_Code"];
                    ImLine.TasksUndertaken = (string)config["Task_Undertaken"];
                    ImLine.HoursWorked = (string)config["Hours"];
                    ImLine.LineNo = (string)config["TS_Line_No"];
                    ImLine.Month = (string)config["Month"];
                    ImLine.Year = (string)config["Year"];
                    ImLine.DocNo = (string)config["Timesheet_Code"];
                    ImLine.ApprovalStatus = (string)config["Approval_Status"];
                    ImpLines.Add(ImLine);
                }
            }

            TimeSheetLinesList Lines = new TimeSheetLinesList
            {
                ListOfTimeSheetTasks = ImpLines
            };
            return PartialView("~/Views/Timesheet/TasksUndertakenLine.cshtml", Lines);

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult EditTimeSheetEntry(string LnNo, string DocNo)
        {
            try
            {
                string employeeNo = "";
                int ln = Convert.ToInt32(LnNo);
                employeeNo = AccountController.GetEmployeeNo();
                TimeSheetModelLines ImLine = new TimeSheetModelLines();
                string pageLine = "TimesheetLedgerEntries?$filter=Employee_No eq '" + employeeNo + "' and Ledger_No eq " + LnNo + " and TS_No eq '" + DocNo + "' &$format=json";
                HttpWebResponse httpResponseLine = Models.Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ImLine.Description = (string)config["Description"];
                        ImLine.Month = (string)config["Month"];
                        ImLine.Year = (string)config["Year"];
                        ImLine.Date = (string)config["Date"];
                        ImLine.Day = (string)config["Day"];
                        ImLine.Hours = (string)config["Hours"];
                        ImLine.LedgerNo = (string)config["Ledger_No"];
                        ImLine.DocNo = (string)config["TS_No"];
                        ImLine.ListofTasks = (string)config["Tasks"];
                        string nonworking = "";
                        if ((string)config["Non_Working_Day"] == "true")
                        {
                            nonworking = "Yes";
                        }
                        else
                        {
                            nonworking = "No";
                        }
                        ImLine.NonWorkingDay = nonworking;
                        //ImpLines.Add(ImLine);
                    }
                }


                return PartialView("~/Views/Timesheet/_TimeSheetEditItemForm.cshtml", ImLine);
            }
            catch (Exception ex)
            {
                return PartialView("~/Views/Timesheet/_TimeSheetEditItemForm.cshtml", ex.Message);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult AddTasks(string LnNo, string DocNo, string Mnth, string Year)
        {
            try
            {
                ProgramsList programList = new ProgramsList();
                #region Dimension 1 List
                List<DimensionValues> DimensionValues = new List<DimensionValues>();
                string dimension1list = "DimensionValues?$filter=Global_Dimension_No eq 2 and Blocked eq false &$format=json";

                HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(dimension1list);
                using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                {
                    var result1 = streamReader1.ReadToEnd();

                    var details1 = JObject.Parse(result1);

                    foreach (JObject config1 in details1["value"])
                    {
                        DimensionValues DList1 = new DimensionValues();
                        DList1.Code = (string)config1["Code"];
                        DList1.Name = (string)config1["Name"];
                        DimensionValues.Add(DList1);
                    }
                }
                #endregion
                programList = new ProgramsList
                {
                    ListofPrograms = DimensionValues.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Code,
                                              Value = x.Code
                                          }).OrderBy(x => x.Text).ToList(),

                };

                return PartialView("~/Views/Timesheet/_TimeSheetAddTask.cshtml", programList);
            }
            catch (Exception ex)
            {
                return PartialView("~/Views/Timesheet/_TimeSheetAddTask.cshtml", ex.Message);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateTimeSheetEntry(string LnNo, string DocNo, string Hrs, string Tasks)
        {
            try
            {
                string employeeNo = "";
                int ln = Convert.ToInt32(LnNo);
                employeeNo = AccountController.GetEmployeeNo();
                bool r = false;
                string msg = "";
                r = Models.Credentials.ObjNav.UpdateTimeSheetEntry(DocNo,ln, employeeNo, Convert.ToDecimal(Hrs), Tasks);
                if (r)
                {
                    msg = "Timesheet Updated Successfully";
                }
                else
                {
                    msg = "Update failed";
                }
                return Json(new { message = msg, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitTask(string ProgramCode, string Mth, string Year, string LnNo, string DocNo, string Hrs, string Tasks)
        {
            try
            {
                string employeeNo = "";
                int ln = Convert.ToInt32(LnNo);
                employeeNo = AccountController.GetEmployeeNo();
                Models.Credentials.ObjNav.InsertTimesheetTasks(DocNo, Mth, employeeNo, Year, Tasks, ProgramCode,Convert.ToDecimal(Hrs), ln);
                return Json(new { message = "Task updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteTask(string LnNo, string DocNo, string Mth, string Year)
        {
            try
            {
                int ln = Convert.ToInt32(LnNo);
                Models.Credentials.ObjNav.DeleteTimesheetTasks(DocNo, Mth, Year, ln);
                return Json(new { message = "Task deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SendToSupervisor(string DocNo, string Mth, string Yr)
        {
            try
            {
                string employeeNo = "";
                employeeNo = AccountController.GetEmployeeNo();
                Models.Credentials.ObjNav.SendToSupervisor(DocNo, Mth, employeeNo, Yr);
                return Json(new { message = "Timesheet successfully sent to supervisor", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ReturntoEmployee(string DocNo, string Mth, string Yr, string Comments, string LnNo)
        {
            try
            {
                string employeeNo = "";
                employeeNo = AccountController.GetEmployeeNo();
                Models.Credentials.ObjNav.ReturnToEmployee(DocNo, Mth, employeeNo, Yr, Comments, Convert.ToInt32(LnNo));
                return Json(new { message = "Timesheet successfully sent to employee", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ApproveTimesheet(string DocNo, string Mth, string Yr, string LnNo)
        {
            try
            {
                string employeeNo = "";
                employeeNo = AccountController.GetEmployeeNo();
                Models.Credentials.ObjNav.ApproveTimesheet(DocNo, Mth, employeeNo, Yr, Convert.ToInt32(LnNo));
                return Json(new { message = "Timesheet successfully approved", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult TimesheetReport(string DocNo, string Mth, string Yr)
        {
            try
            {
                string employeeNo = "";
                employeeNo = AccountController.GetEmployeeNo();
                Models.Credentials.ObjNav.SendToSupervisor(DocNo, Mth, employeeNo, Yr);
                return Json(new { message = "Timesheet successfully sent to supervisor", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GenerateTimesheetReport(string DocNo, string Mth, string Yr, string EmpNo)
        {
            try
            {
                string employeeNo = "";
                string filename = "";
                string filenane = "";
                employeeNo = AccountController.GetEmployeeNo();
                filename = "Timesheet_" + Mth + "_" + EmpNo + ".pdf";
                filenane = Models.Credentials.ObjNav.GenerateTimeSheetReport(Convert.ToInt32(EmpNo), Mth, Yr, filename, DocNo);
                return Json(new { message = "https://ess.cihebkenya.org/reports/"+filename, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GenerateSupervisorTimesheetReport(string DocNo, string Mth, string Yr, string EmpNo)
        {
            try
            {
                string employeeNo = "";
                string filename = "";
                string filenane = "";
                //employeeNo = "000"+EmpNo;
                filename = "Timesheet_" + Mth + "_" + EmpNo + ".pdf";
                filenane = Models.Credentials.ObjNav.GenerateTimeSheetReport(Convert.ToInt32(EmpNo), Mth, Yr, filename, DocNo);
                return Json(new { message = "https://ess.cihebkenya.org/reports/" + filename, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateTasks(string DocNo, string Mth, string Yr, string EmpNo, string tasks)
        {
            try
            {
                string employeeNo = "";
                employeeNo = AccountController.GetEmployeeNo();
                Models.Credentials.ObjNav.UpdateTasks(DocNo, Mth, employeeNo, Yr, tasks);
                return Json(new { message = "Timesheet successfully updated", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }


    }

}