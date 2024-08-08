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
    public class AppraisalResultsController : Controller
    {
        private string companyName = ServiceConnection.CompanyName;

        private static string companyURL = "";

        private DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);

        private DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

        private SuccessResponseController successResponse = new SuccessResponseController();

        private InfoResponseController infoResponse = new InfoResponseController();

        private ErrorResponseController errorResponse = new ErrorResponseController();
        // GET: AppraisalResults
        //Load Appraisal Results - For Appraisee's View (Mid Year)
        [Authorize]
        public ActionResult _AppraisalResults()
        {

            AppraisalResultsModel PeerAppObj = new AppraisalResultsModel();
            var employeeNumber = AccountController.GetEmployeeNo();        

            List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();
            var AppResuts = from ResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                            where ResQuery.Employee_No.Equals(employeeNumber) && ResQuery.Appraisal_Stage.Equals("Mid Year Review") && ResQuery.Appraised.Equals(true)
                            select ResQuery;
            foreach (HRAppraisalHeader Lines in AppResuts)
            {
                AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();
                ResultsAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                ResultsAppLinesObj.AppraisalNo = Lines.No;
                ResultsAppLinesObj.Designation = Lines.Designation;
                ResultsAppLinesObj.Department = Lines.Department_Name;
                ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;

                ResultsAppLinesObj.PerformanceLineScore = Lines.Performance_Line_Scores ?? 0;
                ResultsAppLinesObj.PeerLineScore = Lines.Peer_Line_Scores ?? 0;
                ResultsAppLinesObj.SubordinateLineScore = Lines.Surbodinate_Line_Scores ?? 0;
                ResultsAppLinesObj.CompetencyLineScore = Lines.Competencies_Line_Scores ?? 0;
                ResultsAppLinesObj.ExternalLineScore = Lines.External_Line_Scores ?? 0;
                ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;
                ResultsAppLinesObj.DeclineResults = Lines.Decline ?? false;
                ResultsAppLinesObj.Appeal = Lines.Appealed ?? false;
                ResultsAppLinesObjList.Add(ResultsAppLinesObj);
            }          

            return PartialView("_AppraisalResults", ResultsAppLinesObjList);
            
        }


        //Load Appraisal Results - For Appraisee's View (End Year)
        [Authorize]
        //[HttpPost]
        public ActionResult LoadAppraisalResultsEnd(string Designation) 
        {
            var Des = Designation;
            var StaffNo = AccountController.GetEmployeeNo();
            List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();

            var AppResuts = from ResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                            where ResQuery.Employee_No.Equals(StaffNo) && ResQuery.Appraisal_Stage.Equals("End Year Evaluation") && ResQuery.Appraised.Equals(true)
                            select ResQuery;

            foreach (HRAppraisalHeader Lines in AppResuts)
            {
                AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();

                ResultsAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                ResultsAppLinesObj.AppraisalNo = Lines.No;
                ResultsAppLinesObj.Designation = Lines.Designation;
                ResultsAppLinesObj.Department = Lines.Department_Name;
                ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;

                ResultsAppLinesObj.PerformanceLineScore = Lines.Performance_Line_Scores ?? 0;
                ResultsAppLinesObj.PeerLineScore = Lines.Peer_Line_Scores ?? 0;
                ResultsAppLinesObj.SubordinateLineScore = Lines.Surbodinate_Line_Scores ?? 0;
                ResultsAppLinesObj.CompetencyLineScore = Lines.Competencies_Line_Scores ?? 0;
                ResultsAppLinesObj.ExternalLineScore = Lines.External_Line_Scores ?? 0;
                ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;
                ResultsAppLinesObj.DeclineResults = Lines.Decline ?? false;
                ResultsAppLinesObj.Appeal = Lines.Appealed ?? false;
           

                ResultsAppLinesObjList.Add(ResultsAppLinesObj);
            }
            return View(ResultsAppLinesObjList);
        }

        //Get  Details of Appraisal Results - Mid Year
        [Authorize]
        public ActionResult DetailedResults(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {
                    return RedirectToAction("AppraisalResultsHome", "PerformanceHome");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                //List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();

                var DetailedResuts = from ResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where ResQuery.Employee_No.Equals(StaffNo) && ResQuery.No.Equals(AppraisalNo)
                                     select ResQuery;
                AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();
                foreach (HRAppraisalHeader Lines in DetailedResuts)
                {
                 

                    ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                    ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                    ResultsAppLinesObj.Designation = Lines.Designation;
                    ResultsAppLinesObj.Department = Lines.Department_Name;
                    ResultsAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                    ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                    ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                    ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                    ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;
                    ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;
                    ResultsAppLinesObj.Appeal = Lines.Appealed ?? false;
                    ResultsAppLinesObj.AppraisalNo = Lines.No;

                    ResultsAppLinesObj.PeerLineScore = Lines.Peer_Line_Scores ?? 0;
                    ResultsAppLinesObj.PerformanceLineScore = Lines.Performance_Line_Scores ?? 0;
                    ResultsAppLinesObj.SubordinateLineScore = Lines.Surbodinate_Line_Scores ?? 0;
                    ResultsAppLinesObj.ExternalLineScore = Lines.External_Line_Scores ?? 0;
                    ResultsAppLinesObj.CompetencyLineScore = Lines.Competencies_Line_Scores ?? 0;

                    //ResultsAppLinesObjList.Add(ResultsAppLinesObj);
                }
                return View(ResultsAppLinesObj);
              }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Load View for Appealing Results -Mid Year
        [HttpGet]
        [Authorize]
        public ActionResult AppealResults(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {
                    return RedirectToAction("AppraisalResultsHome", "PerformanceHome");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                //List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();

                var DetailedResuts = from ResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where ResQuery.Employee_No.Equals(StaffNo) && ResQuery.No.Equals(AppraisalNo)
                                     select ResQuery;
                AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();
                foreach (HRAppraisalHeader Lines in DetailedResuts)
                {

                    ResultsAppLinesObj.UserId = Lines.User_ID;
                    ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                    ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                    ResultsAppLinesObj.Designation = Lines.Designation;
                    ResultsAppLinesObj.Department = Lines.Department_Name;
                    ResultsAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                    ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                    ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                    ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                    ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;
                    ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;
                    ResultsAppLinesObj.Appeal = Lines.Appealed ?? false; 
                    ResultsAppLinesObj.AppraisalNo = Lines.No;
                    ResultsAppLinesObj.AppealReason = Lines.Reason_for_Appeal;

                    ResultsAppLinesObj.PeerLineScore = Lines.Peer_Line_Scores ?? 0;
                    ResultsAppLinesObj.PerformanceLineScore = Lines.Performance_Line_Scores ?? 0;
                    ResultsAppLinesObj.SubordinateLineScore = Lines.Surbodinate_Line_Scores ?? 0;
                    ResultsAppLinesObj.ExternalLineScore = Lines.External_Line_Scores ?? 0;
                    ResultsAppLinesObj.CompetencyLineScore = Lines.Competencies_Line_Scores ?? 0;


                    //ResultsAppLinesObjList.Add(ResultsAppLinesObj);
                }
                return View(ResultsAppLinesObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Create Appeal Card Mid Year
        [Authorize]
        [HttpPost]
        public ActionResult AppealResults(AppraisalResultsModel AppealObj) 
        {
            try
            {
                var employeeNumber = AccountController.GetEmployeeNo();
                var CheckTargets = (from TargetsQuery in dynamicsNAVODataServices.dynamicsNAVOData.WorkPlanHeaderTargets
                                    where TargetsQuery.Staff_No.Equals(employeeNumber) && TargetsQuery.User.Equals(AppealObj.UserId)
                                    select TargetsQuery).ToList();
                if (CheckTargets.Count < 1)
                {
                    TempData["notargets"] = "Failed, your targets have not been set. Please check with your supervisor";
                    return View(AppealObj);                  
                }
                else
                {

                    var CheckHeader = (from HeadersQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                       where HeadersQuery.Employee_No.Equals(employeeNumber) && HeadersQuery.Appraisal_Stage.Equals("Mid Year Review") && HeadersQuery.Appealed.Equals(true)
                                       select HeadersQuery).ToList();
                    if (CheckHeader.Count > 0)
                    {
                        TempData["exists"] = "Failed, your have already initiated an appeal for Mid year Results";
                        return View(AppealObj);
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
                            TempData["nomanager"] = "Failed, you have not been assigned a manager";
                            return View(AppealObj);
                        }
                        else
                        {
                            dynamicsNAVSOAPServices.performanceManagement.CreateAppealHeaderMid(AppealObj.UserId);
                            TempData["success"] = "Success, you have successfully initiated an Appeal";
                            return View(AppealObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }



        //Load View for Accepting Results - Mid Year
        [HttpGet]
        [Authorize]
        public ActionResult AcceptResults(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {
                    return RedirectToAction("AppraisalResultsHome", "PerformanceHome");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                //List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();

                var DetailedResuts = from ResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where ResQuery.Employee_No.Equals(StaffNo) && ResQuery.No.Equals(AppraisalNo)
                                     select ResQuery;
                AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();
                foreach (HRAppraisalHeader Lines in DetailedResuts)
                {


                    ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                    ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                    ResultsAppLinesObj.Designation = Lines.Designation;
                    ResultsAppLinesObj.Department = Lines.Department_Name;
                    ResultsAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                    ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                    ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                    ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                    ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;
                    ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;
                    ResultsAppLinesObj.Appeal = Lines.Appealed ?? false;
                    ResultsAppLinesObj.AppraisalNo = Lines.No;
                    //ResultsAppLinesObj.AppealReason = Lines.Reason_for_Appeal;

                    ResultsAppLinesObj.PeerLineScore = Lines.Peer_Line_Scores ?? 0;
                    ResultsAppLinesObj.PerformanceLineScore = Lines.Performance_Line_Scores ?? 0;
                    ResultsAppLinesObj.SubordinateLineScore = Lines.Surbodinate_Line_Scores ?? 0;
                    ResultsAppLinesObj.ExternalLineScore = Lines.External_Line_Scores ?? 0;
                    ResultsAppLinesObj.CompetencyLineScore = Lines.Competencies_Line_Scores ?? 0;


                    //ResultsAppLinesObjList.Add(ResultsAppLinesObj);
                }
                return View(ResultsAppLinesObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        ////Accepting Appraisal Results - Mid Year
        [HttpPost]
        public ActionResult AcceptResults(AppraisalResultsModel AppObj) 
        {

            try
            {
                dynamicsNAVSOAPServices.performanceManagement.AcceptResults(AppObj.AppraisalNo);
                TempData["success"] = "Action Completed Successfully";
                return RedirectToAction("AppraisalResultsHome", "PerformanceHome");
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }


        //Get  Details of Appraisal Results  - End Year
        [Authorize]
        public ActionResult EndYearDetailedResults(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {
                    return RedirectToAction("LoadAppraisalResultsEnd", "AppraisalResults");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                //List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();

                var DetailedResuts = from ResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where ResQuery.Employee_No.Equals(StaffNo) && ResQuery.No.Equals(AppraisalNo)
                                     select ResQuery;
                AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();
                foreach (HRAppraisalHeader Lines in DetailedResuts)
                {


                    ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                    ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                    ResultsAppLinesObj.Designation = Lines.Designation;
                    ResultsAppLinesObj.Department = Lines.Department_Name;
                    ResultsAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                    ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                    ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                    ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                    ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;
                    ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;
                    ResultsAppLinesObj.Appeal = Lines.Appealed ?? false;
                    ResultsAppLinesObj.AppraisalNo = Lines.No;

                    ResultsAppLinesObj.PeerLineScore = Lines.Peer_Line_Scores ?? 0;
                    ResultsAppLinesObj.PerformanceLineScore = Lines.Performance_Line_Scores ?? 0;
                    ResultsAppLinesObj.SubordinateLineScore = Lines.Surbodinate_Line_Scores ?? 0;
                    ResultsAppLinesObj.ExternalLineScore = Lines.External_Line_Scores ?? 0;
                    ResultsAppLinesObj.CompetencyLineScore = Lines.Competencies_Line_Scores ?? 0;

                    //ResultsAppLinesObjList.Add(ResultsAppLinesObj);
                }
                return View(ResultsAppLinesObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Load View for Appealing Results - End Year
        [HttpGet]
        [Authorize]
        public ActionResult EndYearAppealResults(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {
                    return RedirectToAction("LoadAppraisalResultsEnd", "AppraisalResults");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                //List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();

                var DetailedResuts = from ResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where ResQuery.Employee_No.Equals(StaffNo) && ResQuery.No.Equals(AppraisalNo)
                                     select ResQuery;
                AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();
                foreach (HRAppraisalHeader Lines in DetailedResuts)
                {

                    ResultsAppLinesObj.UserId = Lines.User_ID;
                    ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                    ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                    ResultsAppLinesObj.Designation = Lines.Designation;
                    ResultsAppLinesObj.Department = Lines.Department_Name;
                    ResultsAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                    ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                    ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                    ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                    ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;
                    ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;
                    ResultsAppLinesObj.Appeal = Lines.Appealed ?? false;
                    ResultsAppLinesObj.AppraisalNo = Lines.No;
                    ResultsAppLinesObj.AppealReason = Lines.Reason_for_Appeal;

                    ResultsAppLinesObj.PeerLineScore = Lines.Peer_Line_Scores ?? 0;
                    ResultsAppLinesObj.PerformanceLineScore = Lines.Performance_Line_Scores ?? 0;
                    ResultsAppLinesObj.SubordinateLineScore = Lines.Surbodinate_Line_Scores ?? 0;
                    ResultsAppLinesObj.ExternalLineScore = Lines.External_Line_Scores ?? 0;
                    ResultsAppLinesObj.CompetencyLineScore = Lines.Competencies_Line_Scores ?? 0;


                    //ResultsAppLinesObjList.Add(ResultsAppLinesObj);
                }
                return View(ResultsAppLinesObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Create Appeal Card Mid Year
        [Authorize]
        [HttpPost]
        public ActionResult EndYearAppealResults(AppraisalResultsModel AppealObj)
        {
            try
            {
                var employeeNumber = AccountController.GetEmployeeNo();
                var CheckTargets = (from TargetsQuery in dynamicsNAVODataServices.dynamicsNAVOData.WorkPlanHeaderTargets
                                    where TargetsQuery.Staff_No.Equals(employeeNumber) && TargetsQuery.User.Equals(AppealObj.UserId)
                                    select TargetsQuery).ToList();
                if (CheckTargets.Count < 1)
                {
                    TempData["notargets"] = "Failed, your targets have not been set. Please check with your supervisor";
                    return View(AppealObj);
                }
                else
                {

                    var CheckHeader = (from HeadersQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                       where HeadersQuery.Employee_No.Equals(employeeNumber) && HeadersQuery.Appraisal_Stage.Equals("End Year Evaluation") && HeadersQuery.Appealed.Equals(true)
                                       select HeadersQuery).ToList();
                    if (CheckHeader.Count > 0)
                    {
                        TempData["exists"] = "Failed, your have already initiated an appeal for End year Results";
                        return View(AppealObj);
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
                            TempData["nomanager"] = "Failed, you have not been assigned a manager";
                            return View(AppealObj);
                        }
                        else
                        {
                            dynamicsNAVSOAPServices.performanceManagement.CreateAppealHeaderEnd(AppealObj.UserId,AppealObj.AppealReason);
                            TempData["success"] = "Success, you have successfully initiated an Appeal for your End Year Results";
                            return View(AppealObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }


        //Load View for Accepting Results - End Year
        [HttpGet]
        [Authorize]
        public ActionResult EndYearAcceptResults(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {
                    return RedirectToAction("LoadAppraisalResultsEnd", "AppraisalResults");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                //List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();

                var DetailedResuts = from ResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where ResQuery.Employee_No.Equals(StaffNo) && ResQuery.No.Equals(AppraisalNo)
                                     select ResQuery;
                AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();
                foreach (HRAppraisalHeader Lines in DetailedResuts)
                {


                    ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                    ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                    ResultsAppLinesObj.Designation = Lines.Designation;
                    ResultsAppLinesObj.Department = Lines.Department_Name;
                    ResultsAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                    ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                    ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                    ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                    ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;
                    ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;
                    ResultsAppLinesObj.Appeal = Lines.Appealed ?? false;
                    ResultsAppLinesObj.AppraisalNo = Lines.No;
                    //ResultsAppLinesObj.AppealReason = Lines.Reason_for_Appeal;

                    ResultsAppLinesObj.PeerLineScore = Lines.Peer_Line_Scores ?? 0;
                    ResultsAppLinesObj.PerformanceLineScore = Lines.Performance_Line_Scores ?? 0;
                    ResultsAppLinesObj.SubordinateLineScore = Lines.Surbodinate_Line_Scores ?? 0;
                    ResultsAppLinesObj.ExternalLineScore = Lines.External_Line_Scores ?? 0;
                    ResultsAppLinesObj.CompetencyLineScore = Lines.Competencies_Line_Scores ?? 0;

                    //ResultsAppLinesObjList.Add(ResultsAppLinesObj);
                }
                return View(ResultsAppLinesObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }


        ////Accepting Appraisal Results -End Year
        [HttpPost]
        [Authorize]
        public ActionResult EndYearAcceptResults(AppraisalResultsModel AppObj)
        {
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.AcceptResults(AppObj.AppraisalNo);
                TempData["success"] = "Action Completed Successfully";
                return RedirectToAction("LoadAppraisalResultsEnd", "AppraisalResults");
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }
        
        //Load View for Declining Results - End Year
        [HttpGet]
        [Authorize]
        public ActionResult EndYearDeclineResults(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {
                    return RedirectToAction("LoadAppraisalResultsEnd", "AppraisalResults");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                //List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();

                var DetailedResuts = from ResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where ResQuery.Employee_No.Equals(StaffNo) && ResQuery.No.Equals(AppraisalNo)
                                     select ResQuery;
                AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();
                foreach (HRAppraisalHeader Lines in DetailedResuts)
                {


                    ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                    ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                    ResultsAppLinesObj.Designation = Lines.Designation;
                    ResultsAppLinesObj.Department = Lines.Department_Name;
                    ResultsAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                    ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                    ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                    ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                    ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;
                    ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;
                    ResultsAppLinesObj.Appeal = Lines.Appealed ?? false;
                    ResultsAppLinesObj.DeclineResults = Lines.Decline ?? false;
                    ResultsAppLinesObj.DeclineReason = Lines.Reason_for_Decline;
                    ResultsAppLinesObj.AppraisalNo = Lines.No;

                    //ResultsAppLinesObj.AppealReason = Lines.Reason_for_Appeal;

                    ResultsAppLinesObj.PeerLineScore = Lines.Peer_Line_Scores ?? 0;
                    ResultsAppLinesObj.PerformanceLineScore = Lines.Performance_Line_Scores ?? 0;
                    ResultsAppLinesObj.SubordinateLineScore = Lines.Surbodinate_Line_Scores ?? 0;
                    ResultsAppLinesObj.ExternalLineScore = Lines.External_Line_Scores ?? 0;
                    ResultsAppLinesObj.CompetencyLineScore = Lines.Competencies_Line_Scores ?? 0;

                    //ResultsAppLinesObjList.Add(ResultsAppLinesObj);
                }
                return View(ResultsAppLinesObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }


        //Declining Appraisal Results -End Year
        [Authorize]
        [HttpPost]
        public ActionResult EndYearDeclineResults(AppraisalResultsModel AppObj)
        {
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.DeclineResults(AppObj.AppraisalNo, AppObj.DeclineReason);

                TempData["success"] = "Action Completed Successfully";
                return RedirectToAction("LoadAppraisalResultsEnd", "AppraisalResults");
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }


        //Load View for Declining Results - Mid Year
        [HttpGet]
        [Authorize]
        public ActionResult DeclineResults(string AppraisalNo) 
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {
                    return RedirectToAction("AppraisalResultsHome", "PerformanceHome");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                //List<AppraisalResultsModel> ResultsAppLinesObjList = new List<AppraisalResultsModel>();

                var DetailedResuts = from ResQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                     where ResQuery.Employee_No.Equals(StaffNo) && ResQuery.No.Equals(AppraisalNo)
                                     select ResQuery;
                AppraisalResultsModel ResultsAppLinesObj = new AppraisalResultsModel();
                foreach (HRAppraisalHeader Lines in DetailedResuts)
                {


                    ResultsAppLinesObj.EmployeeName = Lines.Employee_Name;
                    ResultsAppLinesObj.EmployeeNo = Lines.Employee_No;
                    ResultsAppLinesObj.Designation = Lines.Designation;
                    ResultsAppLinesObj.Department = Lines.Department_Name;
                    ResultsAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                    ResultsAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                    ResultsAppLinesObj.AppraisedNarration = Lines.Appraised_Narration;
                    ResultsAppLinesObj.AppraisedScore = Lines.Appraised_Score ?? 0;
                    ResultsAppLinesObj.ScoreGrading = Lines.Score_Grading;
                    ResultsAppLinesObj.AcceptResults = Lines.Accept_Result ?? false;
                    ResultsAppLinesObj.Appeal = Lines.Appealed ?? false;
                    ResultsAppLinesObj.AppraisalNo = Lines.No;

                    //ResultsAppLinesObj.AppealReason = Lines.Reason_for_Appeal;

                    ResultsAppLinesObj.PeerLineScore = Lines.Peer_Line_Scores ?? 0;
                    ResultsAppLinesObj.PerformanceLineScore = Lines.Performance_Line_Scores ?? 0;
                    ResultsAppLinesObj.SubordinateLineScore = Lines.Surbodinate_Line_Scores ?? 0;
                    ResultsAppLinesObj.ExternalLineScore = Lines.External_Line_Scores ?? 0;
                    ResultsAppLinesObj.CompetencyLineScore = Lines.Competencies_Line_Scores ?? 0;


                    //ResultsAppLinesObjList.Add(ResultsAppLinesObj);
                }
                return View(ResultsAppLinesObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }


        //Declining Appraisal Results -End Year
        [Authorize]
        [HttpPost]
        public ActionResult DeclineResults(AppraisalResultsModel AppObj)
        {
            try
            {             
                dynamicsNAVSOAPServices.performanceManagement.DeclineResultsMidYr(AppObj.AppraisalNo, AppObj.DeclineReason);

                TempData["success"] = "Action Completed Successfully";
                return RedirectToAction("AppraisalResultsHome", "PerformanceHome");
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }



    }
}