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
    public class SupervisorAppraisalsController : Controller
    {
        private string companyName = ServiceConnection.CompanyName;

        private static string companyURL = "";

        private DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);

        private DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

        private SuccessResponseController successResponse = new SuccessResponseController();

        private InfoResponseController infoResponse = new InfoResponseController();

        private ErrorResponseController errorResponse = new ErrorResponseController();

        IQueryable<EmployeeQueryTwo> _appraiserSelection = null;

        //Supervisor Appraisals View -Mid Year
        [Authorize]
        public ActionResult _SupervisorAppraisals() 
        {
            var StaffNo = AccountController.GetEmployeeNo();
            List<SupervisorAppraisalsModel> SupervisorAppLinesObjList = new List<SupervisorAppraisalsModel>();

            var SuperAppLines = from SuperQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                where SuperQuery.Supervisor_No.Equals(StaffNo) && SuperQuery.Appraisal_Stage.Equals("Mid Year Review") && SuperQuery.Appraisal_Status.Equals("Supervisor")
                                select SuperQuery;

            foreach (HRAppraisalHeader Lines in SuperAppLines)
            {

                SupervisorAppraisalsModel SupervisorAppLinesObj = new SupervisorAppraisalsModel();

                SupervisorAppLinesObj.AppraisalNo = Lines.No; 
                SupervisorAppLinesObj.EmployeeName = Lines.Employee_Name;
                SupervisorAppLinesObj.EmployeeNo = Lines.Employee_No;
                SupervisorAppLinesObj.Designation = Lines.Designation;
                SupervisorAppLinesObj.Department = Lines.Department_Name;
                SupervisorAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                SupervisorAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                SupervisorAppLinesObj.SupervisorName = Lines.Supervisor_Name;
                SupervisorAppLinesObj.AppraisalStatus = Lines.Appraisal_Status;
                SupervisorAppLinesObj.SupervisorNo = Lines.Supervisor_No;
                SupervisorAppLinesObj.Appeal = Lines.Appealed ?? false;

                SupervisorAppLinesObjList.Add(SupervisorAppLinesObj);
            }
            return PartialView(SupervisorAppLinesObjList);
            //return Json(SupervisorAppLinesObjList, JsonRequestBehavior.AllowGet);
        }

        //Supervisor Appraisals View -End Year
        [Authorize]
        public ActionResult SupervisorAppraisalEnd()
        {
            var StaffNo = AccountController.GetEmployeeNo();
            List<SupervisorAppraisalsModel> SupervisorAppLinesObjList = new List<SupervisorAppraisalsModel>();

            var SuperAppLines = from SuperQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                where SuperQuery.Supervisor_No.Equals(StaffNo) && SuperQuery.Appraisal_Stage.Equals("End Year Evaluation") && SuperQuery.Appraisal_Status.Equals("Supervisor")
                                select SuperQuery;

            foreach (HRAppraisalHeader Lines in SuperAppLines)
            {

                SupervisorAppraisalsModel SupervisorAppLinesObj = new SupervisorAppraisalsModel();

                SupervisorAppLinesObj.AppraisalNo = Lines.No;
                SupervisorAppLinesObj.EmployeeName = Lines.Employee_Name;
                SupervisorAppLinesObj.EmployeeNo = Lines.Employee_No;
                SupervisorAppLinesObj.Designation = Lines.Designation;
                SupervisorAppLinesObj.Department = Lines.Department_Name;
                SupervisorAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                SupervisorAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                SupervisorAppLinesObj.SupervisorName = Lines.Supervisor_Name;
                SupervisorAppLinesObj.AppraisalStatus = Lines.Appraisal_Status;
                SupervisorAppLinesObj.SupervisorNo = Lines.Supervisor_No;
                SupervisorAppLinesObj.Appeal = Lines.Appealed ?? false;
                SupervisorAppLinesObjList.Add(SupervisorAppLinesObj);
            }
            return View(SupervisorAppLinesObjList);
            //return Json(SupervisorAppLinesObjList, JsonRequestBehavior.AllowGet);
        }

        //Get Performance Targets for Appraisee to Supervisor - Mid Year
        [Authorize]       
        public ActionResult SingleAppraiseeForm(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {

                    return RedirectToAction("SupervisorAppraisalHome", "PerformanceHome");
                }
                var Ap = AppraisalNo;
                List<SupervisorAppraisalsModel> SupervisorAppObjList = new List<SupervisorAppraisalsModel>();
                var SuperAppLines = from SuperAppQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalLinesFirst
                                    where SuperAppQuery.Appraisal_No.Equals(AppraisalNo)
                                    select SuperAppQuery;
                ViewBag.AppraisalNo = AppraisalNo;
                foreach (AppraisalLinesFirst Lines in SuperAppLines)
                {

                    SupervisorAppraisalsModel SupervisorAppObj = new SupervisorAppraisalsModel();
                    //Appraisee Fields
                    SupervisorAppObj.Perspective = Lines.Perspective;
                    SupervisorAppObj.Objective = Lines.Objective;
                    SupervisorAppObj.Project = Lines.Project_Name;
                    SupervisorAppObj.Activity = Lines.Activity;
                    SupervisorAppObj.SelfAssessmentScore = Lines.Self_Assesment ?? 0;
                    SupervisorAppObj.AppraiseeComments = Lines.Appraisee_Comments;
                    SupervisorAppObj.EndYearSelfScore = Lines.End_Year_Self_Score ?? 0;
                    SupervisorAppObj.EndYearAppraiseeComments = Lines.EndYear_Self_Comments;
                    SupervisorAppObj.TargetScore = Lines.Target_Score ?? 0;
                    SupervisorAppObj.LineNo = Lines.Line_No;
                    //Supervisor Fields
                    SupervisorAppObj.MidYrSupervisorScore = Lines.Supervisor_Assesment ?? 0;
                    SupervisorAppObj.MidYrSupervisorComments = Lines.Supervisor_Comments;
                    SupervisorAppObj.MidYrAgreedScore = Lines.Agreed_Assesment_Results ?? 0;
                    SupervisorAppObj.EndYrSupervisorScore = Lines.End_Year_Supervisor_Score ?? 0;
                    SupervisorAppObj.EndYrSupervisorComments = Lines.EndYear_Supervisor_Comments;
                    SupervisorAppObj.EndYrAgreedScore = Lines.End_Year_Assessment ?? 0;
                    SupervisorAppObj.EndYrAssessmentComments = Lines.End_Year_Evaluation_Comments;
                    SupervisorAppObj.SupLineNo = Lines.Line_No;



                    SupervisorAppObjList.Add(SupervisorAppObj);
                }
                return View(SupervisorAppObjList);
            }
            catch(Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

            //return Json(SupervisorAppObjList, JsonRequestBehavior.AllowGet);
        }

        //Get Performance Targets for Appraisee to Supervisor - End Year
        [Authorize]
        public ActionResult SingleAppraiseeFormEnd(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {

                    return RedirectToAction("SupervisorAppraisalEnd", "SupervisorAppraisals");
                }
                var Ap = AppraisalNo;
                List<SupervisorAppraisalsModel> SupervisorAppObjList = new List<SupervisorAppraisalsModel>();
                var SuperAppLines = from SuperAppQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalLinesFirst
                                    where SuperAppQuery.Appraisal_No.Equals(AppraisalNo)
                                    select SuperAppQuery;
                ViewBag.AppraisalNo = AppraisalNo;
                foreach (AppraisalLinesFirst Lines in SuperAppLines)
                {

                    SupervisorAppraisalsModel SupervisorAppObj = new SupervisorAppraisalsModel();
                    //Appraisee Fields
                    SupervisorAppObj.Perspective = Lines.Perspective;
                    SupervisorAppObj.Objective = Lines.Objective;
                    SupervisorAppObj.Project = Lines.Project_Name;
                    SupervisorAppObj.Activity = Lines.Activity;
                    SupervisorAppObj.SelfAssessmentScore = Lines.Self_Assesment ?? 0;
                    SupervisorAppObj.AppraiseeComments = Lines.Appraisee_Comments;
                    SupervisorAppObj.EndYearSelfScore = Lines.End_Year_Self_Score ?? 0;
                    SupervisorAppObj.EndYearAppraiseeComments = Lines.EndYear_Self_Comments;
                    SupervisorAppObj.TargetScore = Lines.Target_Score ?? 0;
                    SupervisorAppObj.LineNo = Lines.Line_No;
                    //Supervisor Fields
                    SupervisorAppObj.MidYrSupervisorScore = Lines.Supervisor_Assesment ?? 0;
                    SupervisorAppObj.MidYrSupervisorComments = Lines.Supervisor_Comments;
                    SupervisorAppObj.MidYrAgreedScore = Lines.Agreed_Assesment_Results ?? 0;
                    SupervisorAppObj.EndYrSupervisorScore = Lines.End_Year_Supervisor_Score ?? 0;
                    SupervisorAppObj.EndYrSupervisorComments = Lines.EndYear_Supervisor_Comments;
                    SupervisorAppObj.EndYrAgreedScore = Lines.End_Year_Assessment ?? 0;
                    SupervisorAppObj.EndYrAssessmentComments = Lines.End_Year_Evaluation_Comments;
                    SupervisorAppObj.SupLineNo = Lines.Line_No;



                    SupervisorAppObjList.Add(SupervisorAppObj);
                }
                return View(SupervisorAppObjList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

            //return Json(SupervisorAppObjList, JsonRequestBehavior.AllowGet);
        }
        //Return Appraisal form to Appraisee - From PT View
        [Authorize]
        [HttpPost]
        public ActionResult ReturntoAppraisee(string AppraisalNo) {
            var getAppraiseeResult = from ApprQr in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where ApprQr.No.Equals(AppraisalNo)
                                     select ApprQr;
            var ArrayData = getAppraiseeResult.FirstOrDefault();
            var AppStatus = ArrayData.Status;
            if (AppStatus == "Open") { 
                dynamicsNAVSOAPServices.performanceManagement.ReturntoAppraisee(AppraisalNo);
                TempData["saved"] = "Form returned to Appraisee sucessfully. ";
                return RedirectToAction("SingleAppraiseeForm", "SupervisorAppraisals", new { AppraisalNo = AppraisalNo });

            }
            else 
            {
                TempData["failed"] = "Failed, this Appraisal my have been closed. ";
                return RedirectToAction("SingleAppraiseeForm", "SupervisorAppraisals", new { AppraisalNo = AppraisalNo });
            }
           
        }

        //Return Appraisal form to Appraisee from CC View
        [Authorize]
        [HttpPost]
        public ActionResult ReturntoAppraiseeCC(string AppraisalNo)
        {
            var getAppraiseeResult = from ApprQr in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where ApprQr.No.Equals(AppraisalNo)
                                     select ApprQr;
            var ArrayData = getAppraiseeResult.FirstOrDefault();
            var AppStatus = ArrayData.Status;
            if (AppStatus == "Open")
            {
                dynamicsNAVSOAPServices.performanceManagement.ReturntoAppraisee(AppraisalNo);
                TempData["saved"] = "Form returned to Appraisee sucessfully. ";
                return RedirectToAction("SingleAppraiseeFormCC", "SupervisorAppraisals", new { AppraisalNo = AppraisalNo });

            }
            else
            {
                TempData["failed"] = "Failed, this Appraisal my have been closed. ";
                return RedirectToAction("SingleAppraiseeFormCC", "SupervisorAppraisals", new { AppraisalNo = AppraisalNo });
            }

        }


        //Get Competency Values for Appraisee to Supervisor
        [Authorize]
          public ActionResult SingleAppraiseeFormCC(string AppraisalNo)
            {
            try
            {

                if (AppraisalNo.Equals(""))

                {

                    return RedirectToAction("SupervisorAppraisalHome", "PerformanceHome");
                }
                var Ap = AppraisalNo;
                List<SupervisorAppraisalsModel> SupervisorCCAppObjList = new List<SupervisorAppraisalsModel>();
                var SuperAppLinesCC = from CCAppQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLinesValues
                                      where CCAppQuery.Appraisal_No.Equals(AppraisalNo)
                                      select CCAppQuery;
                foreach (HRAppraisalLinesValues Lines in SuperAppLinesCC)
                {

                    SupervisorAppraisalsModel SupervisorCCAppObj = new SupervisorAppraisalsModel();
                    //Appraisee Fields
                    ViewBag.AppraisalNo = Lines.Appraisal_No;
                    SupervisorCCAppObj.CoreDescription = Lines.Description;
                    SupervisorCCAppObj.CAppScore = Lines.Score ?? 0;
                    SupervisorCCAppObj.CAppComments = Lines.Appraisee_Comments;
                    SupervisorCCAppObj.CSuperScore = Lines.Supervisor_Score ?? 0;
                    SupervisorCCAppObj.CAgreedScore = Lines.Agreed_Score ?? 0;
                    SupervisorCCAppObj.ScoreDescriptors = Lines.Score_Descriptors;
                    SupervisorCCAppObj.AppraisalNo = Lines.Appraisal_No;
                    SupervisorCCAppObj.CoreCode = Lines.Code;

                    SupervisorCCAppObjList.Add(SupervisorCCAppObj);
                }
                return View(SupervisorCCAppObjList);
            }
            catch(Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

            //return Json(SupervisorCCAppObjList, JsonRequestBehavior.AllowGet);
        }


        // Get A single PT Line For Edit from Appraisee to Supervisor -End Year(From Mid year View)
        public ActionResult GetPTAppraiseeLine(int SupLineNo)
        {

            var pcode = SupLineNo;
            //var StaffNo = AccountController.GetEmployeeNo();
            SupervisorAppraisalsModel SupervisorPTLinesObj = new SupervisorAppraisalsModel();
            try
            {

                var PTSupLine = from SupPTQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalLinesFirst
                                where SupPTQuery.Line_No.Equals(SupLineNo)
                                select SupPTQuery;



                foreach (AppraisalLinesFirst Lines in PTSupLine)
                {
                    SupervisorPTLinesObj.AppraisalNo = Lines.Appraisal_No;
                    SupervisorPTLinesObj.SupLineNo = Lines.Line_No;
                    //SupervisorPTLinesObj.Project = Lines.Project_Name;
                    //SupervisorPTLinesObj.Activity = Lines.Activity;
                    SupervisorPTLinesObj.TargetScore = Lines.Target_Score ?? 0;
                    SupervisorPTLinesObj.MidYrSupervisorScore = Lines.Supervisor_Assesment ?? 0;
                    SupervisorPTLinesObj.MidYrSupervisorComments = Lines.Supervisor_Comments;
                    SupervisorPTLinesObj.MidYrAgreedScore = Lines.Agreed_Assesment_Results ?? 0;
                    SupervisorPTLinesObj.EndYrSupervisorScore = Lines.End_Year_Supervisor_Score ?? 0;
                    SupervisorPTLinesObj.EndYrSupervisorComments = Lines.EndYear_Supervisor_Comments;
                    SupervisorPTLinesObj.EndYrAgreedScore = Lines.End_Year_Assessment ?? 0;
                    SupervisorPTLinesObj.EndYrAssessmentComments = Lines.End_Year_Evaluation_Comments;
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            return View(SupervisorPTLinesObj);
            //return Json(SupervisorPTLinesObj, JsonRequestBehavior.AllowGet);
        }

        // Get A single PT Line For Edit from Appraisee to Supervisor -End Year(From End year View)
        public ActionResult GetPTAppraiseeLineEnd(int SupLineNo)
        {

            var pcode = SupLineNo;
            //var StaffNo = AccountController.GetEmployeeNo();
            SupervisorAppraisalsModel SupervisorPTLinesObj = new SupervisorAppraisalsModel();
            try
            {

                var PTSupLine = from SupPTQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalLinesFirst
                                where SupPTQuery.Line_No.Equals(SupLineNo)
                                select SupPTQuery;



                foreach (AppraisalLinesFirst Lines in PTSupLine)
                {
                    SupervisorPTLinesObj.AppraisalNo = Lines.Appraisal_No;
                    SupervisorPTLinesObj.SupLineNo = Lines.Line_No;
                    //SupervisorPTLinesObj.Project = Lines.Project_Name;
                    //SupervisorPTLinesObj.Activity = Lines.Activity;
                    SupervisorPTLinesObj.TargetScore = Lines.Target_Score ?? 0;
                    SupervisorPTLinesObj.MidYrSupervisorScore = Lines.Supervisor_Assesment ?? 0;
                    SupervisorPTLinesObj.MidYrSupervisorComments = Lines.Supervisor_Comments;
                    SupervisorPTLinesObj.MidYrAgreedScore = Lines.Agreed_Assesment_Results ?? 0;
                    SupervisorPTLinesObj.EndYrSupervisorScore = Lines.End_Year_Supervisor_Score ?? 0;
                    SupervisorPTLinesObj.EndYrSupervisorComments = Lines.EndYear_Supervisor_Comments;
                    SupervisorPTLinesObj.EndYrAgreedScore = Lines.End_Year_Assessment ?? 0;
                    SupervisorPTLinesObj.EndYrAssessmentComments = Lines.End_Year_Evaluation_Comments;
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            return View(SupervisorPTLinesObj);
            //return Json(SupervisorPTLinesObj, JsonRequestBehavior.AllowGet);
        }

        // Modify Appraisee PT Line for Supervisor -End Year (From Mid Year View)
        [Authorize]
        [HttpPost]
        public ActionResult GetPTAppraiseeLine(SupervisorAppraisalsModel SupObj)
        //public JsonResult ModifySuperPTLine(int LineNo, decimal MidSupervisorScore, int EndSupervisorScore, decimal EndSupervisorAgreedScore, string MidSupervisorComments, string EndSupervisorComments, string EndAsmComments)
        {
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.SupervisorModifyAppraiseePT(SupObj.SupLineNo, SupObj.EndYrSupervisorScore, SupObj.EndYrAgreedScore,SupObj.EndYrSupervisorComments, SupObj.EndYrAssessmentComments);
                TempData["saved"] = "Changes saved";
                return RedirectToAction("SingleAppraiseeForm", "SupervisorAppraisals", new { AppraisalNo = SupObj.AppraisalNo });
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            
        }
        // Modify Appraisee PT Line for Supervisor -End Year (From End Year View)
        [Authorize]
        [HttpPost]
        public ActionResult GetPTAppraiseeLineEnd(SupervisorAppraisalsModel SupObj)
        //public JsonResult ModifySuperPTLine(int LineNo, decimal MidSupervisorScore, int EndSupervisorScore, decimal EndSupervisorAgreedScore, string MidSupervisorComments, string EndSupervisorComments, string EndAsmComments)
        {
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.SupervisorModifyAppraiseePT(SupObj.SupLineNo, SupObj.EndYrSupervisorScore, SupObj.EndYrAgreedScore, SupObj.EndYrSupervisorComments, SupObj.EndYrAssessmentComments);
                TempData["saved"] = "Changes saved";
                return RedirectToAction("SingleAppraiseeFormEnd", "SupervisorAppraisals", new { AppraisalNo = SupObj.AppraisalNo });
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }


        // Get A single PT Line For Edit from Appraisee to Supervisor -Mid Year
        public ActionResult MidYearPTAppraiseeLine(int SupLineNo)
        {

            var pcode = SupLineNo;
            //var StaffNo = AccountController.GetEmployeeNo();
            SupervisorAppraisalsModel MidYrSupervisorPTLinesObj = new SupervisorAppraisalsModel();
            try
            {

                var PTSupLine = from SupPTQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalLinesFirst
                                where SupPTQuery.Line_No.Equals(SupLineNo)
                                select SupPTQuery;



                foreach (AppraisalLinesFirst Lines in PTSupLine)
                {
                    MidYrSupervisorPTLinesObj.AppraisalNo = Lines.Appraisal_No;
                    MidYrSupervisorPTLinesObj.SupLineNo = Lines.Line_No;                   
                    MidYrSupervisorPTLinesObj.TargetScore = Lines.Target_Score ?? 0;
                    MidYrSupervisorPTLinesObj.MidYrSupervisorScore = Lines.Supervisor_Assesment ?? 0;
                    MidYrSupervisorPTLinesObj.MidYrSupervisorComments = Lines.Supervisor_Comments;
                    MidYrSupervisorPTLinesObj.MidYrAgreedScore = Lines.Agreed_Assesment_Results ?? 0;

                    //SupervisorPTLinesObj.Project = Lines.Project_Name;
                    //SupervisorPTLinesObj.Activity = Lines.Activity;
                    //SupervisorPTLinesObj.EndYrSupervisorScore = Lines.End_Year_Supervisor_Score ?? 0;
                    //SupervisorPTLinesObj.EndYrSupervisorComments = Lines.EndYear_Supervisor_Comments;
                    //SupervisorPTLinesObj.EndYrAgreedScore = Lines.End_Year_Assessment ?? 0;
                    //SupervisorPTLinesObj.EndYrAssessmentComments = Lines.End_Year_Evaluation_Comments;
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            return View(MidYrSupervisorPTLinesObj);
            //return Json(SupervisorPTLinesObj, JsonRequestBehavior.AllowGet);
        }



        // Modify Appraisee PT Line for Supervisor -Mid Year
        [Authorize]
        [HttpPost]
        public ActionResult MidYearPTAppraiseeLine(SupervisorAppraisalsModel SupObj)
        //public JsonResult ModifySuperPTLine(int LineNo, decimal MidSupervisorScore, int EndSupervisorScore, decimal EndSupervisorAgreedScore, string MidSupervisorComments, string EndSupervisorComments, string EndAsmComments)
        {
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.SupervisorModifyAppraiseePTMidYear(SupObj.SupLineNo, SupObj.MidYrSupervisorScore,SupObj.MidYrSupervisorComments,SupObj.MidYrAgreedScore);
                TempData["saved"] = "Changes saved";
                return RedirectToAction("SingleAppraiseeForm", "SupervisorAppraisals", new { AppraisalNo = SupObj.AppraisalNo });
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        // Get A single CC Line For Edit from Appraisee to Supervisor

        public ActionResult GetCCAppraiseeLine(string AppraisalNo, string CoreCode)
        {

            var apn = AppraisalNo;
            var cc = CoreCode;
            SupervisorAppraisalsModel SupervisorCCLinesObj = new SupervisorAppraisalsModel();
            try
            {

                var CCSupLine = from SupCCQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLinesValues
                                where SupCCQuery.Code.Equals(CoreCode) && SupCCQuery.Appraisal_No.Equals(AppraisalNo)
                                select SupCCQuery;

                foreach (HRAppraisalLinesValues Lines in CCSupLine)
                {

                    SupervisorCCLinesObj.CoreCode = Lines.Code;
                    SupervisorCCLinesObj.AppraisalNo = Lines.Appraisal_No;
                    SupervisorCCLinesObj.CoreDescription = Lines.Description;
                    SupervisorCCLinesObj.CSuperScore = Lines.Supervisor_Score ?? 0;
                    SupervisorCCLinesObj.CAgreedScore = Lines.Agreed_Score ?? 0;
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            return View(SupervisorCCLinesObj);
            //return Json(SupervisorCCLinesObj, JsonRequestBehavior.AllowGet);
        }

        // Modify Appraisee CC Line for Supervisor
        [Authorize]
        [HttpPost]
        public ActionResult GetCCAppraiseeLine(SupervisorAppraisalsModel ccObj)
        {
        //public JsonResult ModifySuperCCLine(string CoreCode, string AppraisalNo, decimal CSuperScore, decimal CAgreedScore)
        
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.SupervisorModifyAppraiseeCC(ccObj.CoreCode, ccObj.AppraisalNo, ccObj.CSuperScore, ccObj.CAgreedScore);
                TempData["saved"] = "Changes saved";
                return RedirectToAction("SingleAppraiseeFormCC", "SupervisorAppraisals", new { AppraisalNo = ccObj.AppraisalNo });
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
          
        }


        //Get Final Appraisal View for Employee - End Year
        public ActionResult FinalAppraisal(string AppraisalNo)
        {
            var apn = AppraisalNo;           
            SupervisorAppraisalsModel FinalAppObj = new SupervisorAppraisalsModel();
            try
            {

                var GetAppraiseeData = from Appdata in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                       where Appdata.No.Equals(AppraisalNo) 
                                select Appdata;

                foreach (HRAppraisalHeader Lines in GetAppraiseeData)
                {

                    FinalAppObj.EmployeeName = Lines.Employee_Name;
                    FinalAppObj.Designation = Lines.Designation;
                    FinalAppObj.AppraisalPeriod = Lines.Appraisal_Period;
                    FinalAppObj.AppraisalNo = Lines.No;                    
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            return View(FinalAppObj);

        }


        //Get Final Appraisal View for Employee - Mid Year
        public ActionResult MidYearAppraisal(string AppraisalNo)
        {
            var apn = AppraisalNo;
            SupervisorAppraisalsModel FinalAppObj = new SupervisorAppraisalsModel();
            try
            {

                var GetAppraiseeData = from Appdata in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                       where Appdata.No.Equals(AppraisalNo)
                                       select Appdata;

                foreach (HRAppraisalHeader Lines in GetAppraiseeData)
                {

                    FinalAppObj.EmployeeName = Lines.Employee_Name;
                    FinalAppObj.Designation = Lines.Designation;
                    FinalAppObj.AppraisalPeriod = Lines.Appraisal_Period;
                    FinalAppObj.AppraisalQuarter = Lines.Appraisal_Stage;
                    FinalAppObj.AppraisalNo = Lines.No;

                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            return View(FinalAppObj);

        }
        //Appraise Employee Final -Supervisor - Mid Year
        [HttpPost]
        public ActionResult MidYearAppraisal(SupervisorAppraisalsModel FinalAppObj)
        {
            try
            {
                var employeeNumber = AccountController.GetEmployeeNo();

                var EmployeeNoQuery = from EmpQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
                                      where EmpQuery.No.Equals(employeeNumber)
                                      select EmpQuery;
                var userdata = EmployeeNoQuery.FirstOrDefault();

                var AUserId = userdata.User_ID;
                var CheckAppraisal = from AppraisalQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where AppraisalQuery.No.Equals(FinalAppObj.AppraisalNo)
                                     select AppraisalQuery;
                var Result = CheckAppraisal.FirstOrDefault();
                bool? Appraised = Result.Appraised;
                //var Appraised = Result.Score_Grading;

                if (Appraised == false)
                {

                    dynamicsNAVSOAPServices.performanceManagement.AppraiseStaffMidYear(FinalAppObj.AppraisalNo, AUserId);
                    ViewBag.Success = "Employee Appraised Successfully";
                    return View(FinalAppObj);
                }
                else

                {
                    ViewBag.Failed = "Failed, this Employee has already been appraised.";
                    return View(FinalAppObj);

                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex;
                return View(FinalAppObj);
                //return errorResponse.ApplicationExceptionError(ex);
                // return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        //Appraise Employee Final -Supervisor - End Year
        [HttpPost]
        public ActionResult FinalAppraisal(SupervisorAppraisalsModel FinalAppObj)
        {
            try
            {
                var employeeNumber = AccountController.GetEmployeeNo();

                var EmployeeNoQuery = from EmpQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
                                      where EmpQuery.No.Equals(employeeNumber)
                                      select EmpQuery;
                var userdata = EmployeeNoQuery.FirstOrDefault();

                var AUserId = userdata.User_ID;
                var CheckAppraisal = from AppraisalQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where AppraisalQuery.No.Equals(FinalAppObj.AppraisalNo)
                                     select AppraisalQuery;
                var Result = CheckAppraisal.FirstOrDefault();
                bool? Appraised = Result.Appraised;
                //var Appraised = Result.Score_Grading;

                if (Appraised == false)
                {

                    dynamicsNAVSOAPServices.performanceManagement.AppraiseStaff(FinalAppObj.AppraisalNo, AUserId);
                    ViewBag.Success = "Employee Appraised Successfully";
                    return View(FinalAppObj);
                }
                else

                {
                    ViewBag.Failed = "Failed, this Employee has already been appraised.";
                    return View(FinalAppObj);

                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex;
                return View(FinalAppObj);
                //return errorResponse.ApplicationExceptionError(ex);
               // return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }


        //Get Appraisal Results for an Employee who has been Appraised -End Year
        [Authorize]     
        public ActionResult LoadAppraisalResults(string AppraisalNo)
        {
            try {

                if (AppraisalNo.Equals(""))

                {

                    return RedirectToAction("SupervisorAppraisalEnd", "SupervisorAppraisals");
                }

                var App = AppraisalNo;
                var StaffNo = AccountController.GetEmployeeNo();
                List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();

                var AppResuts = from AppResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                where AppResQuery.Supervisor_No.Equals(StaffNo) && AppResQuery.No.Equals(AppraisalNo) && AppResQuery.Appraised.Equals(true) && AppResQuery.Appraisal_Stage.Equals("End Year Evaluation")
                                select AppResQuery;

                foreach (HRAppraisalHeader Lines in AppResuts)
                {

                    AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();

                    ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                    ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                    ResultsAppLinesObj.Designation = Lines.Designation;
                    ResultsAppLinesObj.Department = Lines.Department_Name;
                    ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                    ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                    ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                    ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;
                    ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;

                    ViewBag.AppraisalNo = Lines.No;
                    ViewBag.Peer = Lines.Peer_Line_Scores ?? 0;
                    ViewBag.Performance= Lines.Performance_Line_Scores ?? 0;
                    ViewBag.Subordinate= Lines.Surbodinate_Line_Scores ?? 0;
                    ViewBag.Competency = Lines.Competencies_Line_Scores ?? 0;
                    ViewBag.External= Lines.External_Line_Scores ?? 0; 
                                       

                    ResultsAppLinesObjList.Add(ResultsAppLinesObj);
                }
                return View(ResultsAppLinesObjList);
                //return Json(ResultsAppLinesObjList, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Get Appraisal Results for an Employee who has been Appraised -Mid Year
        [Authorize]
        public ActionResult LoadAppraisalResultsMid(string AppraisalNo)
        {
            try
            {

                if (AppraisalNo.Equals(""))

                {

                    return RedirectToAction("SupervisorAppraisalHome", "PerformanceHome"); 
                }

                var App = AppraisalNo;
                var StaffNo = AccountController.GetEmployeeNo();
                List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();

                var AppResuts = from AppResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                where AppResQuery.Supervisor_No.Equals(StaffNo) && AppResQuery.Appraisal_Stage.Equals("Mid Year Review") && AppResQuery.No.Equals(AppraisalNo) && AppResQuery.Appraised.Equals(true)
                                select AppResQuery;

                foreach (HRAppraisalHeader Lines in AppResuts)
                {

                    AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();

                    ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                    ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                    ResultsAppLinesObj.Designation = Lines.Designation;
                    ResultsAppLinesObj.Department = Lines.Department_Name;
                    ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                    ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                    ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                    ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;
                    ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;

                    ViewBag.AppraisalNo = Lines.No;
                    ViewBag.Peer = Lines.Peer_Line_Scores ?? 0;
                    ViewBag.Performance = Lines.Performance_Line_Scores ?? 0;
                    ViewBag.Subordinate = Lines.Surbodinate_Line_Scores ?? 0;
                    ViewBag.Competency = Lines.Competencies_Line_Scores ?? 0;
                    ViewBag.External = Lines.External_Line_Scores ?? 0;


                    ResultsAppLinesObjList.Add(ResultsAppLinesObj);
                }
                return View(ResultsAppLinesObjList);
                //return Json(ResultsAppLinesObjList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }


        //Get Appraisee Data for Peer, Subordinate and Customer - Mid Year
        [Authorize]
        //[HttpPost]
        public ActionResult GetAppraisees(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {

                    return RedirectToAction("SupervisorAppraisalHome", "PerformanceHome");
                }
                var App = AppraisalNo;
                var StaffNo = AccountController.GetEmployeeNo();
                //List<SupervisorAppraisalsModel> SupervisorAppLinesObjList = new List<SupervisorAppraisalsModel>();

                var AppraiseeLines = from AppsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where AppsQuery.Supervisor_No.Equals(StaffNo) && AppsQuery.No.Equals(AppraisalNo)
                                     select AppsQuery;
                SupervisorAppraisalsModel SupervisorAppLinesObj = new SupervisorAppraisalsModel();

                var Data = AppraiseeLines.FirstOrDefault();
                LoadEmployeesTable();
                SupervisorAppLinesObj.AppraiserCodes = new SelectList(_appraiserSelection, "No", "Full_Name");
                if (Data != null)
                {
                    SupervisorAppLinesObj.PeerAppraiser = Data.Assign_To_Peers;
                    SupervisorAppLinesObj.SubordinateAppraiser = Data.Assign_To_Subordinate;
                    SupervisorAppLinesObj.CustomerAppraiser = Data.Assign_To_Subordinate;
                    SupervisorAppLinesObj.EmployeeName = Data.Employee_Name;
                    SupervisorAppLinesObj.AppraisalNo = Data.No;
                    return View(SupervisorAppLinesObj);
                    //return Json(SupervisorAppLinesObj, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "missing" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch(Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }

        // Assign Appraisers By Supervisor - Mid Year
        [Authorize]
        [HttpPost]
        public ActionResult GetAppraisees(SupervisorAppraisalsModel appraisersObj)
        {
            LoadEmployeesTable();
            appraisersObj.AppraiserCodes = new SelectList(_appraiserSelection, "No", "Full_Name");
            //public JsonResult AddAppraisers(string AppraisalNo, string PeerAppraiser, string CustomerAppraiser, string SubordinateAppraiser)

            try
            {
                dynamicsNAVSOAPServices.performanceManagement.AssignAppraisers(appraisersObj.PeerAppraiser, appraisersObj.CustomerAppraiser, appraisersObj.SubordinateAppraiser, appraisersObj.AppraisalNo);
                TempData["saved"] = "Changes saved";
                return View(appraisersObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        //Get Appraisee Data for Peer, Subordinate and Customer - End Year
        [Authorize]
        //[HttpPost]
        public ActionResult EndYearGetAppraisees(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {

                    return RedirectToAction("SupervisorAppraisalEnd", "SupervisorAppraisals");
                }
                var App = AppraisalNo;
                var StaffNo = AccountController.GetEmployeeNo();
                //List<SupervisorAppraisalsModel> SupervisorAppLinesObjList = new List<SupervisorAppraisalsModel>();

                var AppraiseeLines = from AppsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where AppsQuery.Supervisor_No.Equals(StaffNo) && AppsQuery.No.Equals(AppraisalNo)
                                     select AppsQuery;
                SupervisorAppraisalsModel SupervisorAppLinesObj = new SupervisorAppraisalsModel();

                var Data = AppraiseeLines.FirstOrDefault();
                LoadEmployeesTable();
                SupervisorAppLinesObj.AppraiserCodes = new SelectList(_appraiserSelection, "No", "Full_Name");
                if (Data != null)
                {
                    SupervisorAppLinesObj.PeerAppraiser = Data.Assign_To_Peers;
                    SupervisorAppLinesObj.SubordinateAppraiser = Data.Assign_To_Subordinate;
                    SupervisorAppLinesObj.CustomerAppraiser = Data.Assign_To_Subordinate;
                    SupervisorAppLinesObj.EmployeeName = Data.Employee_Name;
                    SupervisorAppLinesObj.AppraisalNo = Data.No;
                    return View(SupervisorAppLinesObj);
                    //return Json(SupervisorAppLinesObj, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "missing" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }
        // Assign Appraisers By Supervisor - End Year
        [Authorize]
        [HttpPost]
        public ActionResult EndYearGetAppraisees(SupervisorAppraisalsModel appraisersObj)
        {
            LoadEmployeesTable();
            appraisersObj.AppraiserCodes = new SelectList(_appraiserSelection, "No", "Full_Name");
            //public JsonResult AddAppraisers(string AppraisalNo, string PeerAppraiser, string CustomerAppraiser, string SubordinateAppraiser)

            try
            {
                dynamicsNAVSOAPServices.performanceManagement.AssignAppraisersEndYear(appraisersObj.PeerAppraiser, appraisersObj.CustomerAppraiser, appraisersObj.SubordinateAppraiser, appraisersObj.AppraisalNo);
                TempData["saved"] = "Changes saved";
                return View(appraisersObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }           
        }

        #region Helper Functions
        private void LoadEmployeesTable()
        {
            try
            {
                _appraiserSelection = from _Appraisers in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
                                          //where _HrJoblookupValue.Option.Equals("Qualification")  
                                      select _Appraisers;
            }
            catch (Exception ex)
            {
                //return errorResponse.ApplicationExceptionError(ex);
            }

        }

        #endregion  Helper Functions
    }
}