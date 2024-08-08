using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal;
using DynamicsNAV365_StaffPortal.Controllers;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.PerformanceManagement;
using System.Globalization;

namespace DynamicsNAV365_StaffPortal.Controllers.PerformanceManagement
{
    public class AppraisalsController : Controller
    {
        private string companyName = ServiceConnection.CompanyName;           

        private static string companyURL = "";

        private DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);

        private DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

        private SuccessResponseController successResponse = new SuccessResponseController();

        private InfoResponseController infoResponse = new InfoResponseController();

        private ErrorResponseController errorResponse = new ErrorResponseController();
        

        // GET: Appraisals
        [Authorize]
        public ActionResult _Appraisals()
        {
            AppraisalsModel AppraisalObj = new AppraisalsModel(); 

            var employeeNumber = AccountController.GetEmployeeNo();

            var EmployeeNoQuery = from EmpQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
                                  where EmpQuery.No.Equals(employeeNumber)
                                  select EmpQuery;
            var userdata = EmployeeNoQuery.FirstOrDefault();
            //var EmployeeDesignationQuery = from DesQuery in dynamicsNAVODataServices.dynamicsNAVOData.UserSetupQuery
            //                               where DesQuery.Employee_No.Equals(employeeNumber)
            //                               select DesQuery;
            //var des = EmployeeDesignationQuery.FirstOrDefault();

            //Target Setter Parameters            
            AppraisalObj.UserId = userdata.User_ID;
            return PartialView("_Appraisals", AppraisalObj);
        }

        // Mid Year Appraisals
        [Authorize]
        public ActionResult MidAppraisals()
        {
            AppraisalsModel AppraisalObj = new AppraisalsModel();

            var employeeNumber = AccountController.GetEmployeeNo();

            var EmployeeNoQuery = from EmpQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
                                  where EmpQuery.No.Equals(employeeNumber)
                                  select EmpQuery;
            var userdata = EmployeeNoQuery.FirstOrDefault();                
            AppraisalObj.UserId = userdata.User_ID;
            return View(AppraisalObj);
        }
        // End Year Appraisals
        [Authorize]
        public ActionResult EndYearAppraisal()
        {
            AppraisalsModel AppraisalObj = new AppraisalsModel();

            var employeeNumber = AccountController.GetEmployeeNo();

            var EmployeeNoQuery = from EmpQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
                                  where EmpQuery.No.Equals(employeeNumber)
                                  select EmpQuery;
            var userdata = EmployeeNoQuery.FirstOrDefault();
            AppraisalObj.UserId = userdata.User_ID;
            return View(AppraisalObj);
        }

        //Load Appraisal  Lines for An Employee -Mid Year
        [Authorize]
        [HttpPost]
        public JsonResult GetAppraisalLinesMid(string TestVar)
        {
            string AppraisalNo = "";
            string AppStatus = "";
            var StaffNo = AccountController.GetEmployeeNo(); 
            List<AppraisalsModel> AppLinesObjList = new List<AppraisalsModel>();

            var AppealCheck = (from AppealQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                             where AppealQuery.Employee_No.Equals(StaffNo) && AppealQuery.Appealed.Equals(true) && AppealQuery.Appraisal_Stage.Equals("Mid Year Review") && (AppealQuery.Appraisal_Status.Equals("Appraisee") || AppealQuery.Appraisal_Status.Equals("Supervisor"))
                             select AppealQuery).ToList();
            if (AppealCheck.Count > 0) {
                var One = AppealCheck.FirstOrDefault();
                AppraisalNo = One.No;
                AppStatus = One.Appraisal_Status;
            }
            if (AppraisalNo == "")
            {
                var AppNumber = (from AppNoQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                 where AppNoQuery.Employee_No.Equals(StaffNo) && AppNoQuery.Appraisal_Stage.Equals("Mid Year Review") && (AppNoQuery.Appraisal_Status.Equals("Appraisee") || AppNoQuery.Appraisal_Status.Equals("Supervisor"))
                                 select AppNoQuery).ToList();

                if (AppNumber.Count == 1)
                {

                    var NumberVal = AppNumber.FirstOrDefault();
                    AppraisalNo = NumberVal.No;
                    AppStatus = NumberVal.Appraisal_Status;

                    var AppLines = from AppQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalLinesFirst
                                   where AppQuery.Staff_No.Equals(StaffNo) && AppQuery.Appraisal_No.Equals(AppraisalNo)
                                   select AppQuery;

                    foreach (AppraisalLinesFirst Lines in AppLines)
                    {

                        AppraisalsModel AppLinesObj = new AppraisalsModel();
                        AppLinesObj.Perspective = Lines.Perspective;
                        AppLinesObj.Objective = Lines.Objective;
                        AppLinesObj.Project = Lines.Project_Name;
                        AppLinesObj.Activity = Lines.Activity;
                        AppLinesObj.SelfAssessmentScore = Lines.Self_Assesment ?? 0;
                        AppLinesObj.AppraiseeComments = Lines.Appraisee_Comments;
                        AppLinesObj.EndYearSelfScore = Lines.End_Year_Self_Score ?? 0;
                        AppLinesObj.EndYearAppraiseeComments = Lines.EndYear_Self_Comments;
                        AppLinesObj.TargetScore = Lines.Target_Score ?? 0;
                        AppLinesObj.LineNo = Lines.Line_No;
                        AppLinesObj.AppraisalStatus = AppStatus;
                        AppLinesObjList.Add(AppLinesObj);
                    }
                    return Json(AppLinesObjList, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var AppLines = from AppQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalLinesFirst
                               where AppQuery.Staff_No.Equals(StaffNo) && AppQuery.Appraisal_No.Equals(AppraisalNo)
                               select AppQuery;

                foreach (AppraisalLinesFirst Lines in AppLines)
                {

                    AppraisalsModel AppLinesObj = new AppraisalsModel();
                    AppLinesObj.Perspective = Lines.Perspective;
                    AppLinesObj.Objective = Lines.Objective;
                    AppLinesObj.Project = Lines.Project_Name;
                    AppLinesObj.Activity = Lines.Activity;
                    AppLinesObj.SelfAssessmentScore = Lines.Self_Assesment ?? 0;
                    AppLinesObj.AppraiseeComments = Lines.Appraisee_Comments;
                    AppLinesObj.EndYearSelfScore = Lines.End_Year_Self_Score ?? 0;
                    AppLinesObj.EndYearAppraiseeComments = Lines.EndYear_Self_Comments;
                    AppLinesObj.TargetScore = Lines.Target_Score ?? 0;
                    AppLinesObj.LineNo = Lines.Line_No;
                    AppLinesObj.AppraisalStatus = AppStatus;
                    AppLinesObjList.Add(AppLinesObj);
                }
                return Json(AppLinesObjList, JsonRequestBehavior.AllowGet);
            }
            return Json(AppLinesObjList, JsonRequestBehavior.AllowGet);
        }


        //Load Appraisal  Lines(PT) for An Employee -End Year
        [Authorize]
        [HttpPost]
        public JsonResult GetAppraisalLinesEnd(string TestVar)
        {
            var StaffNo = AccountController.GetEmployeeNo();
            string AppraisalNo = "";
            string AppStatus = "";
            List<AppraisalsModel> AppLinesObjList = new List<AppraisalsModel>();

            var AppealCheckEnd = (from AppealEndQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                               where AppealEndQuery.Employee_No.Equals(StaffNo) && AppealEndQuery.Appealed.Equals(true) && AppealEndQuery.Appraisal_Stage.Equals("End Year Evaluation") && (AppealEndQuery.Appraisal_Status.Equals("Appraisee") || AppealEndQuery.Appraisal_Status.Equals("Supervisor"))
                               select AppealEndQuery).ToList();
            if (AppealCheckEnd.Count > 0)
            {
                var DataEnd = AppealCheckEnd.FirstOrDefault();
                AppraisalNo = DataEnd.No;
                AppStatus = DataEnd.Appraisal_Status;
            }
            if (AppraisalNo == "")
            {
                var AppNumber = (from AppNoQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                 where AppNoQuery.Employee_No.Equals(StaffNo) && AppNoQuery.Appraisal_Stage.Equals("End Year Evaluation") && (AppNoQuery.Appraisal_Status.Equals("Appraisee") || AppNoQuery.Appraisal_Status.Equals("Supervisor"))
                                 select AppNoQuery).ToList();
                if (AppNumber.Count == 1)
                {
                    var NumberVal = AppNumber.FirstOrDefault();
                    AppraisalNo = NumberVal.No;
                    AppStatus = NumberVal.Appraisal_Status;

                    var AppLines = from AppQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalLinesFirst
                                   where AppQuery.Staff_No.Equals(StaffNo) && AppQuery.Appraisal_No.Equals(AppraisalNo)
                                   select AppQuery;

                    foreach (AppraisalLinesFirst Lines in AppLines)
                    {

                        AppraisalsModel AppLinesObj = new AppraisalsModel();
                        AppLinesObj.Perspective = Lines.Perspective;
                        AppLinesObj.Objective = Lines.Objective;
                        AppLinesObj.Project = Lines.Project_Name;
                        AppLinesObj.Activity = Lines.Activity;
                        AppLinesObj.SelfAssessmentScore = Lines.Self_Assesment ?? 0;
                        AppLinesObj.AppraiseeComments = Lines.Appraisee_Comments;
                        AppLinesObj.EndYearSelfScore = Lines.End_Year_Self_Score ?? 0;
                        AppLinesObj.EndYearAppraiseeComments = Lines.EndYear_Self_Comments;
                        AppLinesObj.TargetScore = Lines.Target_Score ?? 0;
                        AppLinesObj.LineNo = Lines.Line_No;
                        AppLinesObj.AppraisalStatus = AppStatus;
                        AppLinesObjList.Add(AppLinesObj);
                    }
                    return Json(AppLinesObjList, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var AppLines = from AppQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalLinesFirst
                               where AppQuery.Staff_No.Equals(StaffNo) && AppQuery.Appraisal_No.Equals(AppraisalNo)
                               select AppQuery;

                foreach (AppraisalLinesFirst Lines in AppLines)
                {

                    AppraisalsModel AppLinesObj = new AppraisalsModel();
                    AppLinesObj.Perspective = Lines.Perspective;
                    AppLinesObj.Objective = Lines.Objective;
                    AppLinesObj.Project = Lines.Project_Name;
                    AppLinesObj.Activity = Lines.Activity;
                    AppLinesObj.SelfAssessmentScore = Lines.Self_Assesment ?? 0;
                    AppLinesObj.AppraiseeComments = Lines.Appraisee_Comments;
                    AppLinesObj.EndYearSelfScore = Lines.End_Year_Self_Score ?? 0;
                    AppLinesObj.EndYearAppraiseeComments = Lines.EndYear_Self_Comments;
                    AppLinesObj.TargetScore = Lines.Target_Score ?? 0;
                    AppLinesObj.LineNo = Lines.Line_No;
                    AppLinesObj.AppraisalStatus = AppStatus;
                    AppLinesObjList.Add(AppLinesObj);
                }
                return Json(AppLinesObjList, JsonRequestBehavior.AllowGet);
            }          
            return Json(AppLinesObjList, JsonRequestBehavior.AllowGet);
        }

        // Get Appraisal Line For Edit
        [HttpPost]
        public JsonResult GetAppLineForEdit(int LineNo)
        {
            AppraisalsModel AppraisalEditLinesObj = new AppraisalsModel();

             var SingleAppLine = from ApQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalLinesFirst
                                    where ApQuery.Line_No.Equals(LineNo)
                                    select ApQuery;

                foreach (AppraisalLinesFirst Lines in SingleAppLine)
                {

                    AppraisalEditLinesObj.LineNo = Lines.Line_No;
                    AppraisalEditLinesObj.SelfAssessmentScore = Lines.Self_Assesment ?? 0;
                    AppraisalEditLinesObj.AppraiseeComments = Lines.Appraisee_Comments;
                    AppraisalEditLinesObj.EndYearSelfScore = Lines.End_Year_Self_Score ?? 0;
                    AppraisalEditLinesObj.EndYearAppraiseeComments = Lines.EndYear_Self_Comments;
                    AppraisalEditLinesObj.TargetScore = Lines.Target_Score ?? 0;


                }
                return Json(AppraisalEditLinesObj, JsonRequestBehavior.AllowGet);            
        }

        // Get Appraisal Line For Edit -Mid Yr
        [HttpPost]
        public JsonResult GetAppLineForEditMidYr(int LineNo)
        {
            AppraisalsModel AppraisalEditLinesObj = new AppraisalsModel();

            var SingleAppLine = from ApQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalLinesFirst
                                where ApQuery.Line_No.Equals(LineNo)
                                select ApQuery;

            foreach (AppraisalLinesFirst Lines in SingleAppLine)
            {

                AppraisalEditLinesObj.LineNo = Lines.Line_No;
                AppraisalEditLinesObj.SelfAssessmentScore = Lines.Self_Assesment ?? 0;
                AppraisalEditLinesObj.AppraiseeComments = Lines.Appraisee_Comments;
                //AppraisalEditLinesObj.EndYearSelfScore = Lines.End_Year_Self_Score ?? 0;
                //AppraisalEditLinesObj.EndYearAppraiseeComments = Lines.EndYear_Self_Comments;
                AppraisalEditLinesObj.TargetScore = Lines.Target_Score ?? 0;


            }
            return Json(AppraisalEditLinesObj, JsonRequestBehavior.AllowGet);
        }
        //// Modify Appraisee PT - End Year
        [Authorize]
        [HttpPost]
        public JsonResult ModifyAppLine(string EndYearAppraiseeComments, int EndYearSelfScore, int LineNo)
        {
            var StaffNo = AccountController.GetEmployeeNo();         

            var AppNumber = (from AppNoQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                             where AppNoQuery.Employee_No.Equals(StaffNo) && AppNoQuery.Appraisal_Status.Equals("Appraisee")
                             select AppNoQuery).ToList();
            if (AppNumber.Count < 1)
            {
                return Json(new { message = "supervisor" }, JsonRequestBehavior.AllowGet);
            }
            else {
                dynamicsNAVSOAPServices.performanceManagement.ModifyAppraisalLine(EndYearAppraiseeComments, EndYearSelfScore, LineNo);
                return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
            }           
        }
        //// Modify Appraisee PT - Mid Year
        [Authorize]
        [HttpPost]
        public JsonResult ModifyAppLineMid(string AppraiseeComments,decimal SelfAssessmentScore, int LineNo)
        {
            var StaffNo = AccountController.GetEmployeeNo();

            var AppNumber = (from AppNoQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                             where AppNoQuery.Employee_No.Equals(StaffNo) && AppNoQuery.Appraisal_Status.Equals("Appraisee")
                             select AppNoQuery).ToList();
            if (AppNumber.Count < 1)
            {
                return Json(new { message = "supervisor" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                dynamicsNAVSOAPServices.performanceManagement.ModifyAppraisalLineMidYear(LineNo,AppraiseeComments,SelfAssessmentScore);
                return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
            }
        }


        //Render Core Competencies Partial View -Mid Years
        [Authorize]        
        public ActionResult _CoreCompetencies()
        {           
            return PartialView();       
           
        }

        //Render Core Competencies Partial View - End Year
        [Authorize]
        public ActionResult _CoreCompetenciesEnd()
        {
            return PartialView();        
        }

        //Load Competency  Lines for An Employee -Mid Year
        [Authorize]
        [HttpPost]
        public JsonResult GetCompetencyLines(string TestVar)
        {

            List<CoreCompetenciesModel> CompetenciesObjList = new List<CoreCompetenciesModel>();

            var employeeNumber = AccountController.GetEmployeeNo();
            string AppraisalNo = "";
            var AppealCore = (from AppealCoreQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                             where AppealCoreQuery.Employee_No.Equals(employeeNumber) && AppealCoreQuery.Appealed.Equals(true) && AppealCoreQuery.Appraisal_Stage.Equals("Mid Year Review") && (AppealCoreQuery.Appraisal_Status.Equals("Appraisee") || AppealCoreQuery.Appraisal_Status.Equals("Supervisor"))
                             select AppealCoreQuery).ToList();
            if (AppealCore.Count > 0)
            {
                var DataAppeal = AppealCore.FirstOrDefault();
                AppraisalNo = DataAppeal.No;
            }
            if (AppraisalNo == "")
            {
                var AppNumber = (from AppNoQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                 where AppNoQuery.Employee_No.Equals(employeeNumber) && AppNoQuery.Appraisal_Stage.Equals("Mid Year Review") && (AppNoQuery.Appraisal_Status.Equals("Appraisee") || AppNoQuery.Appraisal_Status.Equals("Supervisor"))
                                 select AppNoQuery).ToList();
                if (AppNumber.Count == 1)
                {
                    var NumberVal = AppNumber.FirstOrDefault();
                    AppraisalNo = NumberVal.No;
                    var Values = from CoreQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLinesValues
                                 where CoreQuery.Appraisal_No.Equals(AppraisalNo)
                                 select CoreQuery;

                    foreach (HRAppraisalLinesValues Lines in Values)
                    {
                        CoreCompetenciesModel CompetenciesObj = new CoreCompetenciesModel();
                        CompetenciesObj.AppraisalNo = Lines.Appraisal_No;
                        CompetenciesObj.Code = Lines.Code;
                        CompetenciesObj.Description = Lines.Description;
                        CompetenciesObj.Score = Lines.Score ?? 0;
                        CompetenciesObj.AppraiseeComments = Lines.Appraisee_Comments;
                        CompetenciesObjList.Add(CompetenciesObj);

                    }
                    return Json(CompetenciesObjList, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                //var AppNumber = (from AppNoQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                //                 where AppNoQuery.Employee_No.Equals(employeeNumber) && AppNoQuery.Appraisal_Stage.Equals("Mid Year Review") && (AppNoQuery.Appraisal_Status.Equals("Appraisee") || AppNoQuery.Appraisal_Status.Equals("Supervisor"))
                //                 select AppNoQuery).ToList();
                //if (AppNumber.Count == 1)
                //{
                //    var NumberVal = AppNumber.FirstOrDefault();
                    //AppraisalNo = NumberVal.No;
                    var Values = from CoreQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLinesValues
                                 where CoreQuery.Appraisal_No.Equals(AppraisalNo)
                                 select CoreQuery;

                    foreach (HRAppraisalLinesValues Lines in Values)
                    {
                        CoreCompetenciesModel CompetenciesObj = new CoreCompetenciesModel();
                        CompetenciesObj.AppraisalNo = Lines.Appraisal_No;
                        CompetenciesObj.Code = Lines.Code;
                        CompetenciesObj.Description = Lines.Description;
                        CompetenciesObj.Score = Lines.Score ?? 0;
                        CompetenciesObj.AppraiseeComments = Lines.Appraisee_Comments;
                        CompetenciesObjList.Add(CompetenciesObj);

                    }
                    return Json(CompetenciesObjList, JsonRequestBehavior.AllowGet);
                //}
            }            
            return Json(CompetenciesObjList, JsonRequestBehavior.AllowGet);
        }


        //Load Competency  Lines for An Employee - End Year
        [Authorize]
        [HttpPost]
        public JsonResult GetCompetencyLinesEnd(string TestVar)
        {

            List<CoreCompetenciesModel> CompetenciesObjList = new List<CoreCompetenciesModel>();

            var employeeNumber = AccountController.GetEmployeeNo();
            string AppraisalNo = "";
            var EndCompCheck = (from CeQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                             where CeQuery.Employee_No.Equals(employeeNumber) && CeQuery.Appealed.Equals(true) && CeQuery.Appraisal_Stage.Equals("End Year Evaluation") && (CeQuery.Appraisal_Status.Equals("Appraisee") || CeQuery.Appraisal_Status.Equals("Supervisor"))
                             select CeQuery).ToList();
            if (EndCompCheck.Count > 0)
            {
                var CendData = EndCompCheck.FirstOrDefault();
                AppraisalNo = CendData.No;
            }
            if (AppraisalNo=="")
            {
                var AppNumber = (from AppNoQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                 where AppNoQuery.Employee_No.Equals(employeeNumber) && AppNoQuery.Appraisal_Stage.Equals("End Year Evaluation") && (AppNoQuery.Appraisal_Status.Equals("Appraisee") || AppNoQuery.Appraisal_Status.Equals("Supervisor"))
                                 select AppNoQuery).ToList();

                if (AppNumber.Count == 1)
                {
                    var NumberVal = AppNumber.FirstOrDefault();
                    AppraisalNo = NumberVal.No;
                    var Values = from CoreQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLinesValues
                                 where CoreQuery.Appraisal_No.Equals(AppraisalNo)
                                 select CoreQuery;

                    foreach (HRAppraisalLinesValues Lines in Values)
                    {
                        CoreCompetenciesModel CompetenciesObj = new CoreCompetenciesModel();
                        CompetenciesObj.AppraisalNo = Lines.Appraisal_No;
                        CompetenciesObj.Code = Lines.Code;
                        CompetenciesObj.Description = Lines.Description;
                        CompetenciesObj.Score = Lines.Score ?? 0;
                        CompetenciesObj.AppraiseeComments = Lines.Appraisee_Comments;
                        CompetenciesObjList.Add(CompetenciesObj);

                    }
                    return Json(CompetenciesObjList, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var Values = from CoreQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLinesValues
                             where CoreQuery.Appraisal_No.Equals(AppraisalNo)
                             select CoreQuery;

                foreach (HRAppraisalLinesValues Lines in Values)
                {
                    CoreCompetenciesModel CompetenciesObj = new CoreCompetenciesModel();
                    CompetenciesObj.AppraisalNo = Lines.Appraisal_No;
                    CompetenciesObj.Code = Lines.Code;
                    CompetenciesObj.Description = Lines.Description;
                    CompetenciesObj.Score = Lines.Score ?? 0;
                    CompetenciesObj.AppraiseeComments = Lines.Appraisee_Comments;
                    CompetenciesObjList.Add(CompetenciesObj);

                }
                return Json(CompetenciesObjList, JsonRequestBehavior.AllowGet);
            }
          
            return Json(CompetenciesObjList, JsonRequestBehavior.AllowGet);

        }

        // Get Core Value Line For Edit -Mid -Year
        [HttpPost]
        public JsonResult GetCoreValueForEdit(string Code)
        {
            var employeeNumber = AccountController.GetEmployeeNo();
            var AppNumber = (from AppNoQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                            where AppNoQuery.Employee_No.Equals(employeeNumber) && AppNoQuery.Appraisal_Stage.Equals("Mid Year Review") && AppNoQuery.Appraisal_Status.Equals("Appraisee")
                            select AppNoQuery).ToList();
            var NumberVal = AppNumber.FirstOrDefault();
            var AppraisalNo = NumberVal.No;
            CoreCompetenciesModel CoreValueObj = new CoreCompetenciesModel();

            var SingleCoreLine = from CoreQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLinesValues
                                where CoreQuery.Code.Equals(Code) && CoreQuery.Appraisal_No.Equals(AppraisalNo)
                                 select CoreQuery;

            foreach (HRAppraisalLinesValues Lines in SingleCoreLine) 
            {

                CoreValueObj.Score = Lines.Score ?? 0;
                CoreValueObj.Description = Lines.Description;
                CoreValueObj.AppraiseeComments = Lines.Appraisee_Comments;
                CoreValueObj.Code = Lines.Code;

            }
            return Json(CoreValueObj, JsonRequestBehavior.AllowGet);
        }


        // Get Core Value Line For Edit -End -Year
        [HttpPost]
        public JsonResult GetCoreValueForEditEnd(string Code)
        {
            var employeeNumber = AccountController.GetEmployeeNo();
            var AppNumber = (from AppNoQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                             where AppNoQuery.Employee_No.Equals(employeeNumber) && AppNoQuery.Appraisal_Stage.Equals("End Year Evaluation") && AppNoQuery.Appraisal_Status.Equals("Appraisee")
                             select AppNoQuery).ToList();
            var NumberVal = AppNumber.FirstOrDefault();
            var AppraisalNo = NumberVal.No;
            CoreCompetenciesModel CoreValueObj = new CoreCompetenciesModel();

            var SingleCoreLine = from CoreQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLinesValues
                                 where CoreQuery.Code.Equals(Code) && CoreQuery.Appraisal_No.Equals(AppraisalNo)
                                 select CoreQuery;

            foreach (HRAppraisalLinesValues Lines in SingleCoreLine)
            {

                CoreValueObj.Score = Lines.Score ?? 0;
                CoreValueObj.Description = Lines.Description;
                CoreValueObj.AppraiseeComments = Lines.Appraisee_Comments;
                CoreValueObj.Code = Lines.Code;

            }
            return Json(CoreValueObj, JsonRequestBehavior.AllowGet);
        }


        //Modify Employee Core Competency Values Line - Mid Year
        [Authorize]
        [HttpPost]
        public JsonResult ModifyCoreValueLine(decimal Score, string AppraiseeComments, string Code)
        {
            var employeeNumber = AccountController.GetEmployeeNo();
            var AppNumber = from AppNoQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                            where AppNoQuery.Employee_No.Equals(employeeNumber) &&AppNoQuery.Appraisal_Stage.Equals("Mid Year Review")&& AppNoQuery.Appraisal_Status.Equals("Appraisee")
                            select AppNoQuery;
            var NumberVal = AppNumber.FirstOrDefault();
            var AppraisalNo = NumberVal.No;
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.ModifyCompetencyLine(Score, AppraiseeComments, Code, AppraisalNo);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
                //return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet); 
        }


        //Modify Employee Core Competency Values Line - End Year
        [Authorize]
        [HttpPost]
        public JsonResult ModifyCoreValueLineEnd(decimal Score, string AppraiseeComments, string Code)
        {
            var employeeNumber = AccountController.GetEmployeeNo();
            var AppNumber = from AppNoQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                            where AppNoQuery.Employee_No.Equals(employeeNumber) &&AppNoQuery.Appraisal_Stage.Equals("End Year Evaluation") && AppNoQuery.Appraisal_Status.Equals("Appraisee")
                            select AppNoQuery;
            var NumberVal = AppNumber.FirstOrDefault();
            var AppraisalNo = NumberVal.No;
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.ModifyCompetencyLine(Score, AppraiseeComments, Code, AppraisalNo);
            }
            catch (Exception ex)
            {

                return Json(new { result = false, error = ex.Message });
                //return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        //Create Appraisal Header - Mid Year
        [Authorize]
        [HttpPost]
        public JsonResult CreateAppraisalHeaderMid(string UserId)    
        {
            try
            {
            var employeeNumber = AccountController.GetEmployeeNo();
            var CheckTargets = (from TargetsQuery in dynamicsNAVODataServices.dynamicsNAVOData.WorkPlanHeaderTargets
                                   where TargetsQuery.Staff_No.Equals(employeeNumber) && TargetsQuery.User.Equals(UserId)
                                select TargetsQuery).ToList();
                if (CheckTargets.Count < 1)
                {
                    return Json(new { message = "notargets" }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    var CheckHeader = (from HeadersQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                       where HeadersQuery.Employee_No.Equals(employeeNumber) && HeadersQuery.Appraisal_Stage.Equals("Mid Year Review")
                                       select HeadersQuery).ToList();
                    if (CheckHeader.Count > 0)
                    {
                        return Json(new { message = "exists" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var CheckManager = from mQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
                                           where mQuery.No.Equals(employeeNumber)
                                           select mQuery;
                        var CData = CheckManager.FirstOrDefault();
                        var ManagerNo = CData.Manager_No;
                        if (ManagerNo.Equals(""))
                        {
                            return Json(new { message = "managernill" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dynamicsNAVSOAPServices.performanceManagement.CreateAppraisalHeaderMid(UserId);
                            return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
            }

        }

        //Create Appraisal Header - End Year
        [Authorize]
        [HttpPost]
        public JsonResult CreateAppraisalHeaderEnd(string UserId)
        {
            try
            {
                var employeeNumber = AccountController.GetEmployeeNo();
                var CheckTargets = (from TargetsQuery in dynamicsNAVODataServices.dynamicsNAVOData.WorkPlanHeaderTargets
                                    where TargetsQuery.Staff_No.Equals(employeeNumber) && TargetsQuery.User.Equals(UserId)
                                    select TargetsQuery).ToList();
                if (CheckTargets.Count < 1)
                {
                    return Json(new { message = "notargets" }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    var CheckHeader = (from HeadersQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                       where HeadersQuery.Employee_No.Equals(employeeNumber) && HeadersQuery.Appraisal_Stage.Equals("End Year Evaluation")
                                       select HeadersQuery).ToList();
                    if (CheckHeader.Count > 0)
                    {
                        return Json(new { message = "exists" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var CheckManager = from mQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
                                           where mQuery.No.Equals(employeeNumber)
                                           select mQuery;
                        var CData = CheckManager.FirstOrDefault();
                        var ManagerNo = CData.Manager_No;
                        if (ManagerNo.Equals(""))
                        {
                            return Json(new { message = "managernill" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dynamicsNAVSOAPServices.performanceManagement.CreateAppraisalHeaderEnd(UserId);
                            return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
                //return Json(new { message = "failed"}, JsonRequestBehavior.AllowGet);
            }

        }


        //Send Targets to Supervisor - Mid Year
        [Authorize]
        [HttpPost]
        public JsonResult SendTargetToSupervisor(string Test)
        {
            var employeeNumber = AccountController.GetEmployeeNo();

            try {
             var CheckSubmission = (from SQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                where SQuery.Employee_No.Equals(employeeNumber) &&SQuery.Appraisal_Stage.Equals("Mid Year Review") && SQuery.Status.Equals("Open")
                                select SQuery).ToList();
                if (CheckSubmission.Count > 0)
                {
                    dynamicsNAVSOAPServices.performanceManagement.SendTargetToSupervisor(employeeNumber);
                    return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "error" }, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
                //return Json(new { message = "error" }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        //Send Targets to Supervisor - End Year
        [Authorize]
        [HttpPost]
        public JsonResult SendTargetToSupervisorEnd(string Test)
        {
            var employeeNumber = AccountController.GetEmployeeNo();

            try
            {
                var CheckSubmission = (from SQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                       where SQuery.Employee_No.Equals(employeeNumber) &&SQuery.Appraisal_Stage.Equals("End Year Evaluation") && SQuery.Status.Equals("Open")
                                       select SQuery).ToList();
                if (CheckSubmission.Count > 0)
                {
                    dynamicsNAVSOAPServices.performanceManagement.SendTargetToSupervisorEnd(employeeNumber);
                    return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "error" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
                //return Json(new { message = "error" }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


    }
    
}
