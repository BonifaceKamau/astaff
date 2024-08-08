using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.PerformanceManagement;

namespace DynamicsNAV365_StaffPortal.Controllers.PerformanceManagement
{
    public class PerformanceTargetsController : Controller
    {
        private string companyName = ServiceConnection.CompanyName;

        private static string companyURL = "";

        private DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);

        private DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

        private SuccessResponseController successResponse = new SuccessResponseController();

        private InfoResponseController infoResponse = new InfoResponseController();

        private ErrorResponseController errorResponse = new ErrorResponseController();
        // GET: PerformanceTargets
        public ActionResult _TargetSetting()  
        {
            TargetsModel OfficerTargetsObj = new TargetsModel();

            var employeeNumber = AccountController.GetEmployeeNo();

            var EmployeeNoQuery = from EmpQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
                                  where EmpQuery.No.Equals(employeeNumber)
                                  select EmpQuery;
            var userdata = EmployeeNoQuery.FirstOrDefault();
            var EmployeeDesignationQuery = from DesQuery in dynamicsNAVODataServices.dynamicsNAVOData.UserSetupQuery
                                           where DesQuery.Employee_No.Equals(employeeNumber)
                                           select DesQuery;
            var des = EmployeeDesignationQuery.FirstOrDefault();

            //Officer Parameters
            OfficerTargetsObj.Directorate = userdata.Global_Dimension_1_Code;
            OfficerTargetsObj.Designation = des.Designation;
            OfficerTargetsObj.UserId = userdata.User_ID;
            return PartialView("_TargetSetting", OfficerTargetsObj);
        }
        //Create Officers' Targets Header
        [Authorize]
        [HttpPost]
        public JsonResult CreateOfficerTargetsHeader(string UserId)
        {
            var StaffNo = AccountController.GetEmployeeNo();
            try
            {
                var OfficerTargetsH = from TargetsQuery in dynamicsNAVODataServices.dynamicsNAVOData.WorkPlanHeaderTargets
                                      where TargetsQuery.Staff_No.Equals(StaffNo)
                                      select TargetsQuery;
                var Targetdata = OfficerTargetsH.FirstOrDefault();
                if (Targetdata == null)
                {
                    dynamicsNAVSOAPServices.performanceManagement.CreateOfficerTargetsHeader(StaffNo,UserId);
                    return Json(new { message = "succeeded" }, JsonRequestBehavior.AllowGet);
                }
                else if(Targetdata != null && Targetdata.Staff_No == StaffNo)
                {
                    return Json(new { message = "youexist" }, JsonRequestBehavior.AllowGet);

                }               

                return Json(new { message = "succeeded" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
                //return false
                //return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }
        //Load Target Lines
        [Authorize]
        [HttpPost]
        public JsonResult LoadTargetLines(string Test)
        {
            
                var StaffNo = AccountController.GetEmployeeNo();

                List<TargetsModel> TargetsObjList = new List<TargetsModel>();

                var TargetLines = from TQuery in dynamicsNAVODataServices.dynamicsNAVOData.WorkPlanTargetLines
                                  where TQuery.Staff_No.Equals(StaffNo)
                                  select TQuery;

                var HeaderStatusQuery = from HQry in dynamicsNAVODataServices.dynamicsNAVOData.WorkPlanHeaderTargets
                                        where HQry.Staff_No.Equals(StaffNo)
                                        select HQry;
                var HStatus = HeaderStatusQuery.FirstOrDefault();
                var ApprovalStatus = HStatus.Status;

                foreach (WorkPlanTargetLines Lines in TargetLines)
                {

                    TargetsModel TargetLinesObj = new TargetsModel();
                    TargetLinesObj.TargetApprovalStatus = ApprovalStatus;
                    TargetLinesObj.PerformanceObjective = Lines.Performance_Objective;
                    TargetLinesObj.DepartmentalObjective = Lines.Departmental_Objective;
                    //TargetLinesObj.Project = Lines.Project;
                    TargetLinesObj.Activity = Lines.Activity;
                    TargetLinesObj.PMI = Lines.Performance_Measure_Indicator;
                    TargetLinesObj.PerformanceOutcome = Lines.Performance_Outcome;
                    TargetLinesObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                    TargetLinesObj.Directorate = Lines.Directorate;
                    TargetLinesObj.Department = Lines.Department;
                    TargetLinesObj.Perspective = Lines.Perspective;
                    TargetLinesObj.WeightTotal = Lines.Weight_Total ?? 0;
                    TargetLinesObj.Targets = Lines.Targets;
                    TargetLinesObj.TargetScore = Lines.Target_Score ?? 0;
                    TargetLinesObj.PrimaryCode = Lines.TargetLine_No;
                    TargetsObjList.Add(TargetLinesObj);
                }
                return Json(TargetsObjList, JsonRequestBehavior.AllowGet);
           
        }

        // Get A single Target Line For Edit
        [HttpPost]       
        public JsonResult GetTargetLineForEdit(string PrimaryCode)
        {
            
                var pcode = PrimaryCode;
            var StaffNo = AccountController.GetEmployeeNo();
                TargetsModel TargetLinesObj = new TargetsModel();
            try {

                var SingleTargetLine = from TargetQuery in dynamicsNAVODataServices.dynamicsNAVOData.WorkPlanTargetLines
                                       where TargetQuery.TargetLine_No.Equals(PrimaryCode) && TargetQuery.Staff_No.Equals(StaffNo)
                                       select TargetQuery;



                foreach (WorkPlanTargetLines Lines in SingleTargetLine)
                {

                    TargetLinesObj.PrimaryCode = Lines.TargetLine_No;
                    TargetLinesObj.PerformanceObjective = Lines.Performance_Objective;
                    TargetLinesObj.DepartmentalObjective = Lines.Departmental_Objective;
                    TargetLinesObj.Project = Lines.Project;
                    TargetLinesObj.Activity = Lines.Activity;
                    TargetLinesObj.PMI = Lines.Performance_Measure_Indicator;
                    TargetLinesObj.PerformanceOutcome = Lines.Performance_Outcome;
                    TargetLinesObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                    TargetLinesObj.Perspective = Lines.Perspective;
                    TargetLinesObj.Targets = Lines.Targets;
                    TargetLinesObj.TargetScore = Lines.Target_Score ?? 0;
                    TargetLinesObj.WeightTotal = Lines.Weight_Total ?? 0;
                }
            }
            catch (Exception ex)
            {

            }
            return Json(TargetLinesObj, JsonRequestBehavior.AllowGet);           
            
        }


        // Modify Selected Target Line
        [HttpPost]
        public JsonResult ModifyTargetLine(string PrimaryCode, int TargetScore)
        {
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.ModifyTargetLine(TargetScore,PrimaryCode);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        //Send Targets Approval Request
        [Authorize]
        [HttpPost]
        public JsonResult SendTargetApprovalRequest(string UserId)
        {
            var StaffNo = AccountController.GetEmployeeNo();
            var TargetsHeaderNo = "";
            try
            {
                var OfficerTargetsH = from TargetsQuery in dynamicsNAVODataServices.dynamicsNAVOData.WorkPlanHeaderTargets
                                      where TargetsQuery.Staff_No.Equals(StaffNo)
                                      select TargetsQuery;
                var Targetdata = OfficerTargetsH.FirstOrDefault();

                if(Targetdata!=null && Targetdata.Status != "Open")
                {
                    return Json(new { message = "alreadysent" }, JsonRequestBehavior.AllowGet);
                }
                else if (Targetdata != null)
                {
                    TargetsHeaderNo = Targetdata.Workplan_No;
                    dynamicsNAVSOAPServices.performanceManagement.SendTargetsApprovalRequest(TargetsHeaderNo);
                    return Json(new { message = "succeeded" }, JsonRequestBehavior.AllowGet);
                }               
               
                else
                {
                    return Json(new { message = "headernull" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { message = "succeeded" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //return false
                return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}