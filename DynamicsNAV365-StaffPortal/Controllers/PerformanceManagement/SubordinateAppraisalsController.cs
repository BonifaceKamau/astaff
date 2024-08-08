using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.PerformanceManagement;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.PerformanceManagement
{
    public class SubordinateAppraisalsController : Controller
    {
        private string companyName = ServiceConnection.CompanyName;

        private static string companyURL = "";

        private DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);

        private DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

        private SuccessResponseController successResponse = new SuccessResponseController();

        private InfoResponseController infoResponse = new InfoResponseController();

        private ErrorResponseController errorResponse = new ErrorResponseController();

        // GET: SubordinateAppraisals
        public ActionResult _SubordinateAppraisals() 
        {
            //SubordinateAppraisalsModel SubordinateAppObj = new SubordinateAppraisalsModel();
            //var employeeNumber = AccountController.GetEmployeeNo();
            //var EmployeeNoQuery = from EmpQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
            //                      where EmpQuery.No.Equals(employeeNumber)
            //                      select EmpQuery;
            //var userdata = EmployeeNoQuery.FirstOrDefault();
            //SubordinateAppObj.UserId = userdata.User_ID;
            //var Des = Designation;

            var StaffNo = AccountController.GetEmployeeNo();
            List<SubordinateAppraisalsModel> SubordinateAppLinesObjList = new List<SubordinateAppraisalsModel>();

            var CustomerAppLines = from CustQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                   where CustQuery.Assign_To_Subordinate.Equals(StaffNo)
                                   select CustQuery;

            foreach (HRAppraisalHeader Lines in CustomerAppLines)
            {

                SubordinateAppraisalsModel SubordinateAppLinesObj = new SubordinateAppraisalsModel();

                SubordinateAppLinesObj.AppraisalNo = Lines.No;
                SubordinateAppLinesObj.EmployeeName = Lines.Employee_Name;
                SubordinateAppLinesObj.EmployeeNo = Lines.Employee_No;
                SubordinateAppLinesObj.Designation = Lines.Designation;
                SubordinateAppLinesObj.Department = Lines.Department_Name;
                SubordinateAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                SubordinateAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                SubordinateAppLinesObj.ExternalSourceNo = Lines.Assign_To_Subordinate;
                SubordinateAppLinesObj.AppraisalStatus = Lines.Appraisal_Status;
                SubordinateAppLinesObj.Appeal = Lines.Appealed ?? false;

                SubordinateAppLinesObjList.Add(SubordinateAppLinesObj);
            }
            return PartialView(SubordinateAppLinesObjList);
            //return Json(SubordinateAppLinesObjList, JsonRequestBehavior.AllowGet);
            //return PartialView("_SubordinateAppraisals", SubordinateAppObj);
        }

        ////Load Subordinate Headers 
        //[Authorize]
        ////[HttpPost]
        //public JsonResult LoadSubordinateHeader(string Designation)
        //{            
        //}


        //Get Subordinate Lines 
        [Authorize]       
        public ActionResult SingleSubordinateForm(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {
                    //return RedirectToAction("ManageProject", "Projects", new { ProjectNo = TasksObjectEdit.ProjectNo });
                    return RedirectToAction("SubordinateAppraisalHome", "PerformanceHome");
                }
                var Ap = AppraisalNo;
                List<SubordinateAppraisalsModel> SubordinateAppObjList = new List<SubordinateAppraisalsModel>();
                var CustomerAppLines = from CustAppQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLines
                                       where CustAppQuery.Appraisal_No.Equals(AppraisalNo) && CustAppQuery.Categorize_As.Equals("Employee's Subordinates")
                                       select CustAppQuery;
                foreach (HRAppraisalLines Lines in CustomerAppLines)
                {
                    SubordinateAppraisalsModel SubordinateAppObj = new SubordinateAppraisalsModel();

                    SubordinateAppObj.LineNo = Lines.Line_No;
                    SubordinateAppObj.SubGoalsTargets = Lines.Perfomance_Goals_and_Targets;
                    SubordinateAppObj.AppraisalNo = Lines.Appraisal_No;
                    SubordinateAppObj.SubordinateRating = Lines.Sub_ordinates_Rating ?? 0;
                    SubordinateAppObj.SubordinateComments = Lines.Subordinates_Comments;
                    SubordinateAppObjList.Add(SubordinateAppObj);
                }
                return View(SubordinateAppObjList);
            }
            catch(Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            //return Json(SubordinateAppObjList, JsonRequestBehavior.AllowGet);
        }

        //Get A single Subordinate Line for Edit
        [Authorize]
        public ActionResult GetSubordinateLineForedit(int LineNo)
        {
            try
            {
                if (LineNo.Equals(""))

                {
                   
                    return RedirectToAction("SubordinateAppraisalHome", "PerformanceHome");
                }
                var pcode = LineNo;
                SubordinateAppraisalsModel SubordinateLinesObj = new SubordinateAppraisalsModel();
           
                var SubLineQuery = from SubQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLines
                                    where SubQuery.Line_No.Equals(LineNo)
                                    select SubQuery;

                foreach (HRAppraisalLines Lines in SubLineQuery)
                {
                    SubordinateLinesObj.SubGoalsTargets = Lines.Perfomance_Goals_and_Targets;
                    SubordinateLinesObj.SubordinateRating = Lines.Sub_ordinates_Rating ?? 0;
                    SubordinateLinesObj.SubordinateComments = Lines.Supervisor_Comments;
                    SubordinateLinesObj.AppraisalNo = Lines.Appraisal_No;
                    SubordinateLinesObj.LineNo = Lines.Line_No;
                }
                return View(SubordinateLinesObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
          
            //return Json(SubordinateLinesObj, JsonRequestBehavior.AllowGet);
        }

        //Modify Subordinate Line
        [Authorize]
        [HttpPost]
        public ActionResult GetSubordinateLineForedit(SubordinateAppraisalsModel SubObj)
        {
        //public JsonResult ModifySubordinateLine(int LineNo, decimal SubordinateRating, string SubordinateComments)
        //{
           try
            {
                dynamicsNAVSOAPServices.performanceManagement.ModifySubordinateLine(SubObj.LineNo, SubObj.SubordinateRating, SubObj.SubordinateComments);
                TempData["saved"] = "Changes saved";
                return RedirectToAction("SingleSubordinateForm", "SubordinateAppraisals", new { AppraisalNo = SubObj.AppraisalNo });
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
           // return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}