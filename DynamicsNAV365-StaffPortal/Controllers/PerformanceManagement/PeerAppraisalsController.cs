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
    public class PeerAppraisalsController : Controller
    {
        private string companyName = ServiceConnection.CompanyName;

        private static string companyURL = "";

        private DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);

        private DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

        private SuccessResponseController successResponse = new SuccessResponseController();

        private InfoResponseController infoResponse = new InfoResponseController();

        private ErrorResponseController errorResponse = new ErrorResponseController();
        // GET: People Listed as your peers (appraisal)
        public ActionResult _PeerAppraisals() 
        {
            //PeerAppObj.UserId = userdata.User_ID;
            //return PartialView("_PeerAppraisals", PeerAppObj);
            ////var Des = Designation;
            //var EmployeeNoQuery = from EmpQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
            //                      where EmpQuery.No.Equals(employeeNumber)
            //                      select EmpQuery;
            //var userdata = EmployeeNoQuery.FirstOrDefault();

            var employeeNumber = AccountController.GetEmployeeNo();           
       
            List<PeerAppraisalsModel> PeerAppLinesObjList = new List<PeerAppraisalsModel>();

            var PeerAppLines = from PeerQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                               where PeerQuery.Assign_To_Peers.Equals(employeeNumber)
                               select PeerQuery;

            foreach (HRAppraisalHeader Lines in PeerAppLines)
            {

                PeerAppraisalsModel PeerAppLinesObj = new PeerAppraisalsModel();

                PeerAppLinesObj.AppraisalNo = Lines.No;
                PeerAppLinesObj.EmployeeName = Lines.Employee_Name;
                PeerAppLinesObj.EmployeeNo = Lines.Employee_No;
                PeerAppLinesObj.Designation = Lines.Designation;
                PeerAppLinesObj.Department = Lines.Department_Name;
                PeerAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                PeerAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                PeerAppLinesObj.PeerNumber = Lines.Assign_To_Peers;
                PeerAppLinesObj.AppraisalStatus = Lines.Appraisal_Status;
                PeerAppLinesObj.Appeal = Lines.Appealed ?? false;

                PeerAppLinesObjList.Add(PeerAppLinesObj);
            }
            return PartialView(PeerAppLinesObjList);
        }
        
        //Get Peer Lines 
        [Authorize]
        //[HttpPost]
        public ActionResult SinglePeerForm(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {

                    return RedirectToAction("PeerAppraisalHome", "PerformanceHome");
                }
                var Ap = AppraisalNo;
                List<PeerAppraisalsModel> PeerAppObjList = new List<PeerAppraisalsModel>();
                var SuperAppLines = from SuperAppQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLines
                                    where SuperAppQuery.Appraisal_No.Equals(AppraisalNo) && SuperAppQuery.Categorize_As.Equals("Employee's Peers")
                                    select SuperAppQuery;
                var GetName = from NameQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                              where NameQuery.No.Equals(AppraisalNo)
                              select NameQuery;
                var NameVal = GetName.FirstOrDefault();
                var EmployeeName = NameVal.Employee_Name;

                foreach (HRAppraisalLines Lines in SuperAppLines)
                {
                    PeerAppraisalsModel PeerAppObj = new PeerAppraisalsModel();
                    ViewBag.EmployeeName = EmployeeName;
                    PeerAppObj.LineNo = Lines.Line_No;
                    PeerAppObj.GoalsTargets = Lines.Perfomance_Goals_and_Targets;
                    PeerAppObj.PeerRating = Lines.Peer_Rating ?? 0;
                    PeerAppObj.PeerComments = Lines.Peer_Comments;
                    PeerAppObj.AppraisalNo = Lines.Appraisal_No;
                    PeerAppObjList.Add(PeerAppObj);
                }
                return View(PeerAppObjList);
            }
            catch(Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            
        }

        //Get A single peer Line for Edit
        //[HttpPost]
        public ActionResult GetPeerLineForedit(int LineNo)
        {
            try
            {
                if (LineNo.Equals(""))

                {

                    return RedirectToAction("PeerAppraisalHome", "PerformanceHome");
                }
                var pcode = LineNo;           
                 PeerAppraisalsModel PeerLinesObj = new PeerAppraisalsModel();
            

                var PeerLineQuery = from PeerQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLines
                                    where PeerQuery.Line_No.Equals(LineNo)
                                select PeerQuery;

                foreach (HRAppraisalLines Lines in PeerLineQuery)
                {
                    PeerLinesObj.GoalsTargets = Lines.Perfomance_Goals_and_Targets;
                    PeerLinesObj.LineNo = Lines.Line_No;
                    PeerLinesObj.PeerRating = Lines.Peer_Rating ?? 0;
                    PeerLinesObj.PeerComments = Lines.Peer_Comments;
                    PeerLinesObj.AppraisalNo = Lines.Appraisal_No;
                }
                return View(PeerLinesObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
           
        }
        // Modify Peer Line
        [Authorize]
        [HttpPost]
        //public JsonResult GetPeerLineForedit(int LineNo, decimal PeerRating, string PeerComments)
        public ActionResult GetPeerLineForedit(PeerAppraisalsModel PeerObj)
        {
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.ModifyPeerLine(PeerObj.PeerRating, PeerObj.PeerComments, PeerObj.LineNo);
                TempData["saved"] = "Changes saved";
                return RedirectToAction("SinglePeerForm", "PeerAppraisals", new { AppraisalNo = PeerObj.AppraisalNo });
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
          
        }

    }
}