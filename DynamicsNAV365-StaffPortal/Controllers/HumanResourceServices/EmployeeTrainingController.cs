using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using DynamicsNAV365_StaffPortal.Models.EmployeeTraining;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.CodeHelpers;
using DynamicsNAV365_StaffPortal.Models.HumanResource;
using OdataRef;
using PayrollPeriods = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.PayrollPeriods;

namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    public class EmployeeTrainingController : Controller
    {
        string companyName = ServiceConnection.CompanyName;
        static string companyURL = "";

        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        BCODATAServices dynamicsNAVODataServices = new BCODATAServices(companyURL);

        SuccessResponseController successResponse = new SuccessResponseController();
        InfoResponseController infoResponse = new InfoResponseController();
        ErrorResponseController errorResponse = new ErrorResponseController();


        private string responseHeader = "";
        private string responseMessage = "";
        private string detailedResponseMessage = "";

        private string button1ControllerName = "";
        private string button1ActionName = "";
        private bool button1HasParameters = false;
        private string button1Parameters = "";
        private string button1Name = "";

        private string button2ControllerName = "";
        private string button2ActionName = "";
        private bool button2HasParameters = false;
        private string button2Parameters = "";
        private string button2Name = "";

        IEnumerable<Calendar_Card> CalenderPeriods = null;
        //IQueryable<ApprovedTrainingNeeds> ApprovedTrainingNeeds = null;
        //IQueryable<ApprovedTrainingApplications> TrainingApplications= null;


        IEnumerable<SelectListItem> TrainingTypes = null;
        IEnumerable<SelectListItem> Interventions = null;
        IEnumerable<SelectListItem> ProposedPeriod = null;
        IEnumerable<SelectListItem> As = null;
        IEnumerable<SelectListItem> Bs = null;
        IEnumerable<SelectListItem> Cs = null;
        IEnumerable<SelectListItem> Ds = null;
        IEnumerable<SelectListItem> Es = null;
        IEnumerable<SelectListItem> Fs = null;
        IEnumerable<SelectListItem> Gs = null;
        IEnumerable<SelectListItem> Hs = null;
        IEnumerable<SelectListItem> Is = null;
        IEnumerable<SelectListItem> Js = null;

        AccountController accountController = new AccountController();

        string employeeNo = "";

        public EmployeeTrainingController()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        #region New Training need Application

        /*[Authorize]
        public ActionResult NewTrainingNeedApplication()
        {
            string OpenTrainingApplicationNeedNo = "";
            bool employeeTrainingApplicationCreated = false;
            try
            {

                OpenTrainingApplicationNeedNo = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckOpenTrainingNeedApplicationExists(employeeNo);


                //check open employee training application
                if (!OpenTrainingApplicationNeedNo.Equals(""))
                {


                    responseHeader = "Employee training need Application Exist";
                    responseMessage = "An open training need request no . " + OpenTrainingApplicationNeedNo + ",exist under employee no. " + employeeNo + ", finalize on this training application before creating a new one.";
                    detailedResponseMessage = "An open training need request no . " + OpenTrainingApplicationNeedNo + ",exist under employee no. " + employeeNo + ", finalize on this training application before creating a new one.";

                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "TrainingNeedsHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                }
                //end check open employee training application

                //create Employee application training

                employeeTrainingApplicationCreated = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CreateTrainingNeedApplication(employeeNo);

                OpenTrainingApplicationNeedNo = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckOpenTrainingNeedApplicationExists(employeeNo);

                EmployeeTrainingNeedModel employeeTrainingNeedObj = new EmployeeTrainingNeedModel();


                employeeTrainingNeedObj.ApplicationNo = OpenTrainingApplicationNeedNo;
                employeeTrainingNeedObj.EmployeeNo = employeeNo;
                employeeTrainingNeedObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

                LoadYears();
                employeeTrainingNeedObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                LoadTrainingTypes();
                employeeTrainingNeedObj.TrainingTypes = new SelectList(TrainingTypes, "Text", "Value");

                LoadInterventions();
                employeeTrainingNeedObj.InterventionsRequired = new SelectList(Interventions, "Text", "Value");

                LoadCalenderPeriod();
                employeeTrainingNeedObj.ProposedPeriods = new SelectList(ProposedPeriod, "Text", "Value");


                return View(employeeTrainingNeedObj);
            }
            catch (Exception ex)
            {

                return errorResponse.ApplicationExceptionError(ex);
            }

        }*/

        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult NewTrainingNeedApplication(EmployeeTrainingNeedModel EmployeeTrainingNeedObj)
        {
            bool EmployeeTrainingNeedModified = false;
            bool approvalWorkflowExist = false;
            string OpenTrainingApplicationNeedNo = "";

            if (ModelState.IsValid)
            {
                LoadYears();
                EmployeeTrainingNeedObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                LoadTrainingTypes();
                EmployeeTrainingNeedObj.TrainingTypes = new SelectList(TrainingTypes, "Text", "Value");

                LoadInterventions();
                EmployeeTrainingNeedObj.InterventionsRequired = new SelectList(Interventions, "Text", "Value");

                LoadCalenderPeriod();
                EmployeeTrainingNeedObj.ProposedPeriods = new SelectList(ProposedPeriod, "Text", "Value");

                try
                {
                    OpenTrainingApplicationNeedNo = dynamicsNAVSOAPServices.employeeTrainingManagementWS
                        .CheckOpenTrainingNeedApplicationExists(employeeNo);

                    if (!OpenTrainingApplicationNeedNo.Equals(""))
                    {
                        EmployeeTrainingNeedObj.InterventionRequiredOther =
                            EmployeeTrainingNeedObj.InterventionRequiredOther != null
                                ? EmployeeTrainingNeedObj.InterventionRequiredOther
                                : "";
                        //modify Employee training need application

                        EmployeeTrainingNeedModified =
                            dynamicsNAVSOAPServices.employeeTrainingManagementWS.ModifyTrainingNeedApplication(
                                EmployeeTrainingNeedObj.ApplicationNo, EmployeeTrainingNeedObj.CalenderYear,
                                EmployeeTrainingNeedObj.DevelopmentNeed, EmployeeTrainingNeedObj.InterventionRequired,
                                EmployeeTrainingNeedObj.InterventionRequiredOther, EmployeeTrainingNeedObj.TrainingType,
                                EmployeeTrainingNeedObj.Objectives, EmployeeTrainingNeedObj.Description,
                                EmployeeTrainingNeedObj.ProposedTrainingProvider,
                                EmployeeTrainingNeedObj.ProposedPeriod, EmployeeTrainingNeedObj.EstimatedCost,
                                EmployeeTrainingNeedObj.TrainingLocation,
                                DateTime.Parse(EmployeeTrainingNeedObj.TrainingScheduledDate),
                                DateTime.Parse(EmployeeTrainingNeedObj.TrainingScheduledDateTo),
                                EmployeeTrainingNeedObj.CPDPoints);
                        if (!EmployeeTrainingNeedModified)
                        {
                            EmployeeTrainingNeedObj.ErrorStatus = true;
                            EmployeeTrainingNeedObj.ErrorMessage =
                                "An error was experienced while trying to modify Employee training need no." +
                                EmployeeTrainingNeedObj.ApplicationNo +
                                ", the server might be offline, try again after a while.";
                            return View(EmployeeTrainingNeedObj);
                        }

                        //send for approval
                        approvalWorkflowExist =
                            dynamicsNAVSOAPServices.employeeTrainingManagementWS
                                .CheckTrainingNeedApprovalWorkflowEnabled(EmployeeTrainingNeedObj.ApplicationNo);
                        if (!approvalWorkflowExist)
                        {
                            EmployeeTrainingNeedObj.ErrorStatus = true;
                            EmployeeTrainingNeedObj.ErrorMessage =
                                "An error was experienced while trying to send an approval request for employee training need no." +
                                EmployeeTrainingNeedObj.ApplicationNo + ", the approval workflow was not found. " +
                                ServiceConnection.contactICTDepartment + "";
                            return View(EmployeeTrainingNeedObj);
                        }

                        if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.SendTrainingNeedApprovalRequest(
                                EmployeeTrainingNeedObj.ApplicationNo))
                        {
                            responseHeader = "Success";
                            responseMessage = "Employee training need no." + EmployeeTrainingNeedObj.ApplicationNo +
                                              " was successfully sent for approval. Check with the " + companyName +
                                              " human resource department for approval status.";
                            detailedResponseMessage = "Employee training need no." +
                                                      EmployeeTrainingNeedObj.ApplicationNo +
                                                      " was successfully sent for approval. Check with the " +
                                                      companyName + "  human resource department for approval status.";
                            button1ControllerName = "EmployeeTraining";
                            button1ActionName = "TrainingNeedsHistory";
                            button1Name = "Ok";
                            button1HasParameters = false;
                            button1Parameters = "";
                            return successResponse.ApplicationSuccess(responseHeader, responseMessage,
                                detailedResponseMessage,
                                button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                                button1Name,
                                button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                                button2Name);
                        }
                        else
                        {
                            EmployeeTrainingNeedObj.ErrorStatus = true;
                            EmployeeTrainingNeedObj.ErrorMessage =
                                "An error was experienced while trying to send an approval request for employee training need no." +
                                EmployeeTrainingNeedObj.ApplicationNo + ". " + ServiceConnection.contactICTDepartment +
                                "";
                            return View(EmployeeTrainingNeedObj);
                        }
                    }
                    else
                    {
                        responseHeader = "Employee training need  NotFound";
                        responseMessage = "The employee training need no." + EmployeeTrainingNeedObj.ApplicationNo +
                                          " was not found under employee no." + AccountController.GetEmployeeNo();
                        detailedResponseMessage = "Theemployee training need no." +
                                                  EmployeeTrainingNeedObj.ApplicationNo +
                                                  " was not found under employee no." +
                                                  AccountController.GetEmployeeNo();
                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "TrainingNeedsHistory";
                        button1Name = "Ok";
                        button1HasParameters = false;
                        button1Parameters = "";
                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                            button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                            button1Name,
                            button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                            button2Name);
                    }
                }
                catch (Exception ex)
                {
                    EmployeeTrainingNeedObj.ErrorStatus = true;
                    EmployeeTrainingNeedObj.ErrorMessage = ex.Message.ToString();
                    return View(EmployeeTrainingNeedObj);
                }
            }

            return View(EmployeeTrainingNeedObj);
        }

        #endregion New Trainingneed Application

        /*#region Edit Training needs applications
        public ActionResult OnBeforeEditNeed(string EmpTrainingNeedNo)
        {

            try
            {
                if (EmpTrainingNeedNo.Equals(""))
                {
                    return RedirectToAction("TrainingNeedsHistory", "EmployeeTraining");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingNeedApplicationExist(EmpTrainingNeedNo, AccountController.GetEmployeeNo()))
                {
                    string TrainingNeedStatus = dynamicsNAVSOAPServices.employeeTrainingManagementWS.GetTrainingNeedApplicationStatus(EmpTrainingNeedNo);
                    //if training application is open
                    if (TrainingNeedStatus.Equals("Open"))
                    {
                        return RedirectToAction("EditTrainingNeedApplication", "EmployeeTraining", new { EmpTrainingNeedNo = EmpTrainingNeedNo });
                    }

                    //if training application is pending approval
                    if (TrainingNeedStatus.Equals("Pending Approval"))
                    {
                        responseHeader = "Employee Training need Request Pending Approval";
                        responseMessage = "The Employee training need no." + EmpTrainingNeedNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
                        detailedResponseMessage = "The Employee training." + EmpTrainingNeedNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "EditTrainingNeedApplication";
                        button1HasParameters = true;
                        button1Parameters = "?EmpTrainingNeedNo=" + EmpTrainingNeedNo;
                        button1Name = "Yes";

                        button2ControllerName = "EmployeeTraining";
                        button2ActionName = "TrainingNeedsHistory";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }

                    //if Training application is released
                    if (TrainingNeedStatus.Equals("Released"))
                    {
                        responseHeader = "Training need  Approved";
                        responseMessage = "The Employee training need no." + EmpTrainingNeedNo + " is already approved. Editing not allowed.";
                        detailedResponseMessage = "The Employee training application." + EmpTrainingNeedNo + " is already approved. Editing not allowed.";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "TrainingNeedsHistory";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    //if training application is rejected
                    if (TrainingNeedStatus.Equals("Rejected"))
                    {
                        responseHeader = "Training Application Rejected";
                        responseMessage = "The employee training need no." + EmpTrainingNeedNo + " was rejected. Editing will reopen the document. Do you want to continue?";
                        detailedResponseMessage = "The employee training need no ." + EmpTrainingNeedNo + " was rejected. Editing will reopen the document. Do you want to continue?";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "EditTrainingNeedApplication";
                        button1HasParameters = true;
                        button1Parameters = "?EmpTrainingNeedNo=" + EmpTrainingNeedNo;
                        button1Name = "Yes";

                        button2ControllerName = "EmployeeTraining";
                        button2ActionName = "TrainingNeedsHistory";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    //if application training is posted/reversed
                    if (TrainingNeedStatus.Equals("Posted") || TrainingNeedStatus.Equals("Reversed"))
                    {
                        responseHeader = " Training Application need Posted";
                        responseMessage = "The Employee training application need no." + EmpTrainingNeedNo + " is already posted. Editing not allowed.";
                        detailedResponseMessage = "The Employee training need." + EmpTrainingNeedNo + " is already posted. Editing not allowed.";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "TrainingNeedsHistory";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    return RedirectToAction("TrainingNeedsHistory", "EmployeeTraining");
                }
                else
                {
                    responseHeader = "Employee Application Training needNotFound";
                    responseMessage = "The  employee training need application no." + EmpTrainingNeedNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "The  employee training need application." + EmpTrainingNeedNo + " was not found under employee no." + AccountController.GetEmployeeNo();

                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "TrainingNeedsHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";

                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }

        [Authorize]
        public ActionResult EditTrainingNeedApplication(string EmpTrainingNeedNo)
        {

            try
            {

                if (EmpTrainingNeedNo.Equals(""))
                {
                    return RedirectToAction("TrainingNeedsHistory", "EmployeeTraining");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingNeedApplicationExist(EmpTrainingNeedNo, AccountController.GetEmployeeNo()))
                {
                    string TrainingNeedStatus = dynamicsNAVSOAPServices.employeeTrainingManagementWS.GetTrainingNeedApplicationStatus(EmpTrainingNeedNo);

                    //if application training need is pending approval, cancel approval request
                    if (TrainingNeedStatus.Equals("Pending Approval"))
                    {
                        dynamicsNAVSOAPServices.employeeTrainingManagementWS.CancelTrainingNeedApprovalRequest(EmpTrainingNeedNo);
                    }
                    //if application training is released, reopen and uncommit from budget
                    //if (TrainingNeedStatus.Equals("Released"))
                    //{
                    //    dynamicsNAVSOAPServices.employeeTrainingManagementWS.CancelTrainingApplicationApprovalRequest(EmpTrainingNeedNo);

                    //}
                    ////if application training is rejected, reopen document
                    //if (TrainingNeedStatus.Equals("Rejected"))
                    //{
                    //    dynamicsNAVSOAPServices.employeeTrainingManagementWS.CancelTrainingApplicationApprovalRequest(EmpTrainingNeedNo);
                    //}

                    EmployeeTrainingNeedModel EmployeeTrainingNeedObj = new EmployeeTrainingNeedModel();

                    var EmployeeTrainingNeedApplications = from EmployeeApplicationNeedsQuery in dynamicsNAVODataServices.BCOData.TrainingAnalysis
                                                           where EmployeeApplicationNeedsQuery.No.Equals(EmpTrainingNeedNo)
                                                           select EmployeeApplicationNeedsQuery;


                    foreach (HRTrainingNeeds HRTrainingNeed in EmployeeTrainingNeedApplications)
                    {

                        EmployeeTrainingNeedObj.ApplicationNo = HRTrainingNeed.No;
                        EmployeeTrainingNeedObj.CalenderYear = HRTrainingNeed.Calendar_Year;
                        EmployeeTrainingNeedObj.DevelopmentNeed = HRTrainingNeed.Development_Need;
                        EmployeeTrainingNeedObj.InterventionRequired = HRTrainingNeed.Intervention_Required;
                        EmployeeTrainingNeedObj.InterventionRequiredOther = HRTrainingNeed.Intervention_Required_Other;
                        EmployeeTrainingNeedObj.TrainingType = HRTrainingNeed.Training_Need_Type;
                        EmployeeTrainingNeedObj.Objectives = HRTrainingNeed.Objectives;
                        EmployeeTrainingNeedObj.Description = HRTrainingNeed.Description;
                        EmployeeTrainingNeedObj.ProposedPeriod = HRTrainingNeed.Proposed_Period;
                        EmployeeTrainingNeedObj.EstimatedCost = Convert.ToDecimal(HRTrainingNeed.Estimated_Cost);
                        EmployeeTrainingNeedObj.TrainingLocation = HRTrainingNeed.Training_Location_Venue;
                        EmployeeTrainingNeedObj.TrainingScheduledDate = Convert.ToString(HRTrainingNeed.Training_Scheduled_Date);
                        EmployeeTrainingNeedObj.TrainingScheduledDateTo = Convert.ToString(HRTrainingNeed.Training_Scheduled_Date_To);
                        EmployeeTrainingNeedObj.CPDPoints = Convert.ToDecimal(HRTrainingNeed.CPD_Points_to_be_Earned);
                        EmployeeTrainingNeedObj.ProposedTrainingProvider = HRTrainingNeed.Proposed_Training_Provider;
                        EmployeeTrainingNeedObj.Status = HRTrainingNeed.Status;

                    }

                    LoadYears();
                    EmployeeTrainingNeedObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                    LoadTrainingTypes();
                    EmployeeTrainingNeedObj.TrainingTypes = new SelectList(TrainingTypes,"Text","Value");

                    LoadInterventions();
                    EmployeeTrainingNeedObj.InterventionsRequired = new SelectList(Interventions,"Text","Value");

                    LoadCalenderPeriod();
                    EmployeeTrainingNeedObj.ProposedPeriods = new SelectList(ProposedPeriod,"Text", "Value");


                    return View(EmployeeTrainingNeedObj);
                }
                else
                {
                    responseHeader = "Employee Training need Application NotFound";
                    responseMessage = "The Employee training need application no." + EmpTrainingNeedNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = " Employee training need application no." + EmpTrainingNeedNo + " was not found under employee no." + AccountController.GetEmployeeNo();

                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "TrainingNeedsHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";

                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }

            }
            catch (Exception ex)
            {

                return errorResponse.ApplicationExceptionError(ex);
            }


        }

        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditTrainingNeedApplication(EmployeeTrainingNeedModel employeeTrainingNeedsObj)
        {

            bool EmployeeTrainingNeedModified = false;
            bool approvalWorkflowExist = false;
          
            try
            {
                LoadYears();
                LoadTrainingTypes();
                LoadInterventions();
                LoadCalenderPeriod();

                if (ModelState.IsValid)
                {
                    employeeTrainingNeedsObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                    employeeTrainingNeedsObj.TrainingTypes = new SelectList(TrainingTypes, "Text", "Value");

                    employeeTrainingNeedsObj.InterventionsRequired = new SelectList(Interventions, "Text", "Value");
                    
                    employeeTrainingNeedsObj.ProposedPeriods = new SelectList(ProposedPeriod, "Text", "Value");

                    if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingNeedApplicationExist(employeeTrainingNeedsObj.ApplicationNo,employeeNo))
                    {
                        employeeTrainingNeedsObj.InterventionRequiredOther = employeeTrainingNeedsObj.InterventionRequiredOther != null ? employeeTrainingNeedsObj.InterventionRequiredOther : "";
                        //modify Employee training need application

                        EmployeeTrainingNeedModified = dynamicsNAVSOAPServices.employeeTrainingManagementWS.ModifyTrainingNeedApplication(employeeTrainingNeedsObj.ApplicationNo, employeeTrainingNeedsObj.CalenderYear, employeeTrainingNeedsObj.DevelopmentNeed,
                        employeeTrainingNeedsObj.InterventionRequired, employeeTrainingNeedsObj.InterventionRequiredOther,
                        employeeTrainingNeedsObj.TrainingType, employeeTrainingNeedsObj.Objectives, employeeTrainingNeedsObj.Description, employeeTrainingNeedsObj.ProposedTrainingProvider, employeeTrainingNeedsObj.ProposedPeriod,
                        employeeTrainingNeedsObj.EstimatedCost, employeeTrainingNeedsObj.TrainingLocation, DateTime.Parse(employeeTrainingNeedsObj.TrainingScheduledDate), DateTime.Parse(employeeTrainingNeedsObj.TrainingScheduledDateTo), employeeTrainingNeedsObj.CPDPoints);

                        if (!EmployeeTrainingNeedModified)
                        {
                            employeeTrainingNeedsObj.ErrorStatus = true;
                            employeeTrainingNeedsObj.ErrorMessage = "An error was experienced while trying to modify Employee training no." + employeeTrainingNeedsObj.ApplicationNo + ", the server might be offline, try again after a while.";
                            return View(employeeTrainingNeedsObj);
                        }
                        //send for approval
                        approvalWorkflowExist = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingNeedApprovalWorkflowEnabled(employeeTrainingNeedsObj.ApplicationNo);
                        if (!approvalWorkflowExist)
                        {
                            employeeTrainingNeedsObj.ErrorStatus = true;
                            employeeTrainingNeedsObj.ErrorMessage = "An error was experienced while trying to send an approval request for employee training need no." + employeeTrainingNeedsObj.ApplicationNo + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
                            return View(employeeTrainingNeedsObj);
                        }
                        if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.SendTrainingNeedApprovalRequest(employeeTrainingNeedsObj.ApplicationNo))
                        {

                            responseHeader = "Success";
                            responseMessage = "Employee training application need no." + employeeTrainingNeedsObj.ApplicationNo + " was successfully sent for approval. Check with the " + companyName + " human resource department for approval status.";
                            detailedResponseMessage = "Employee training application need no." + employeeTrainingNeedsObj.ApplicationNo + " was successfully sent for approval. Check with the " + companyName + "  human resource department for approval status.";
                            button1ControllerName = "EmployeeTraining";
                            button1ActionName = "TrainingNeedsHistory";
                            button1Name = "Ok";
                            button1HasParameters = false;
                            button1Parameters = "";
                            return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                        }
                        else
                        {
                            employeeTrainingNeedsObj.ErrorStatus = true;
                            employeeTrainingNeedsObj.ErrorMessage = "An error was experienced while trying to send an approval request for employee training  need no." + employeeTrainingNeedsObj.ApplicationNo + ". " + ServiceConnection.contactICTDepartment + "";
                            return View(employeeTrainingNeedsObj);
                        }
                    }
                    else
                    {
                        responseHeader = "Employee training need Not Found";
                        responseMessage = "The employee training need no." + employeeTrainingNeedsObj.ApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                        detailedResponseMessage = "The employee training need no ." + employeeTrainingNeedsObj.ApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "TrainingNeedsHistory";
                        button1Name = "Ok";
                        button1HasParameters = false;
                        button1Parameters = "";
                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }

                }
                else
                {
                    return View(employeeTrainingNeedsObj);
                }

            }

            catch (Exception ex)
            {
                employeeTrainingNeedsObj.ErrorStatus = true;
                employeeTrainingNeedsObj.ErrorMessage = ex.Message.ToString();
                return View(employeeTrainingNeedsObj);
            }
        }*/
        /*#endregion Edit Training needs applications

        /*#region Training needs applications Approvals
        [Authorize]
        public ActionResult TrainingNeedApprovals(string EmpTrainingNeedNo)
        {

            try
            {
                if (EmpTrainingNeedNo.Equals(""))
                {

                    return RedirectToAction("OpenEntries", "Approval");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingNeedApplicationExist(EmpTrainingNeedNo, AccountController.GetEmployeeNo()))
                {

                    EmployeeTrainingNeedModel EmployeeTrainingNeedObj = new EmployeeTrainingNeedModel();

                    var EmployeeTrainingNeedApplications = from EmployeeApplicationNeedsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRTrainingNeeds
                                                           where EmployeeApplicationNeedsQuery.No.Equals(EmpTrainingNeedNo)
                                                           select EmployeeApplicationNeedsQuery;


                    foreach (HRTrainingNeeds HRTrainingNeed in EmployeeTrainingNeedApplications)
                    {

                        EmployeeTrainingNeedObj.ApplicationNo = HRTrainingNeed.No;
                        EmployeeTrainingNeedObj.CalenderYear = HRTrainingNeed.Calendar_Year;
                        EmployeeTrainingNeedObj.DevelopmentNeed = HRTrainingNeed.Development_Need;
                        EmployeeTrainingNeedObj.InterventionRequired = HRTrainingNeed.Intervention_Required;
                        EmployeeTrainingNeedObj.InterventionRequiredOther = HRTrainingNeed.Intervention_Required_Other;
                        EmployeeTrainingNeedObj.TrainingType = HRTrainingNeed.Training_Need_Type;
                        EmployeeTrainingNeedObj.Objectives = HRTrainingNeed.Objectives;
                        EmployeeTrainingNeedObj.Description = HRTrainingNeed.Description;
                        EmployeeTrainingNeedObj.ProposedPeriod = HRTrainingNeed.Proposed_Period;
                        EmployeeTrainingNeedObj.EstimatedCost = Convert.ToDecimal(HRTrainingNeed.Estimated_Cost);
                        EmployeeTrainingNeedObj.TrainingLocation = HRTrainingNeed.Training_Location_Venue;
                        EmployeeTrainingNeedObj.TrainingScheduledDate = Convert.ToString(HRTrainingNeed.Training_Scheduled_Date);
                        EmployeeTrainingNeedObj.TrainingScheduledDateTo = Convert.ToString(HRTrainingNeed.Training_Scheduled_Date_To);
                        EmployeeTrainingNeedObj.CPDPoints = Convert.ToDecimal(HRTrainingNeed.CPD_Points_to_be_Earned);
                        EmployeeTrainingNeedObj.ProposedTrainingProvider = HRTrainingNeed.Proposed_Training_Provider;
                        EmployeeTrainingNeedObj.Status = HRTrainingNeed.Status;


                    }

                    return View(EmployeeTrainingNeedObj);
                }
                else
                {
                    responseHeader = "Employee training need NotFound";
                    responseMessage = "Employee training application no." + EmpTrainingNeedNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "Employee training application no." + EmpTrainingNeedNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    button1ControllerName = "Approval";
                    button1ActionName = "RequestsToApprove";
                    button1Name = "Ok";
                    button1HasParameters = false;
                    button1Parameters = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            //return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrainingNeedApprovals(EmployeeTrainingNeedModel EmployeeTrainingNeedObj, string Command)
        {

            try
            {
                if (EmployeeTrainingNeedObj.EmployeeNo.Equals(""))
                {
                    return RedirectToAction("RequestsToApprove", "Approval");
                }
                if (Command == "Approve")
                {
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveTrainingApplication(employeeNo, EmployeeTrainingNeedObj.ApplicationNo))
                    {
                        responseHeader = "Success";
                        responseMessage = "Employee training application need no." + EmployeeTrainingNeedObj.EmployeeNo + " was successfully approved.";
                        detailedResponseMessage = "Employee training application need no." + EmployeeTrainingNeedObj.EmployeeNo + " was successfully approved.";

                        button1ControllerName = "Approval";
                        button1ActionName = "RequestsToApprove";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        EmployeeTrainingNeedObj.ErrorStatus = true;
                        EmployeeTrainingNeedObj.ErrorMessage = "Unable to process the employee training request approve action. " + ServiceConnection.contactICTDepartment + "";
                        return View(EmployeeTrainingNeedObj);
                    }
                }
                else if (Command == "Reject")
                {
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectTrainingApplication(employeeNo, EmployeeTrainingNeedObj.ApplicationNo))
                    {
                        responseHeader = "Success";
                        responseMessage = "Employee training need application no." + EmployeeTrainingNeedObj.EmployeeNo + " was successfully rejected.";
                        detailedResponseMessage = "Employee training need application no." + EmployeeTrainingNeedObj.EmployeeNo + " was successfully rejected.";

                        button1ControllerName = "Approval";
                        button1ActionName = "RequestsToApprove";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        EmployeeTrainingNeedObj.ErrorStatus = true;
                        EmployeeTrainingNeedObj.ErrorMessage = "Unable to process the Employee training application request reject action. " + ServiceConnection.contactICTDepartment + "";
                        return View(EmployeeTrainingNeedObj);
                    }
                }
                else
                {
                    EmployeeTrainingNeedObj.ErrorStatus = true;
                    EmployeeTrainingNeedObj.ErrorMessage = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                    return View(EmployeeTrainingNeedObj);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            //return View();
        }
        #endregion  Training needs applications Approvals#1#

        #region View Training needs Applications
        public ActionResult ViewTrainingNeedRequests(string EmpTrainingNeedNo)
        {

            try
            {
                if (EmpTrainingNeedNo.Equals(""))
                {

                    return RedirectToAction("TrainingNeedsHistory", "EmployeeTraining");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingNeedApplicationExist(EmpTrainingNeedNo, AccountController.GetEmployeeNo()))
                {

                    EmployeeTrainingNeedModel EmployeeTrainingNeedObj = new EmployeeTrainingNeedModel();

                    var EmployeeTrainingNeedApplications = from EmployeeApplicationNeedsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRTrainingNeeds
                                                           where EmployeeApplicationNeedsQuery.No.Equals(EmpTrainingNeedNo)
                                                           select EmployeeApplicationNeedsQuery;


                    foreach (HRTrainingNeeds HRTrainingNeed in EmployeeTrainingNeedApplications)
                    {

                        EmployeeTrainingNeedObj.ApplicationNo = HRTrainingNeed.No;
                        EmployeeTrainingNeedObj.CalenderYear = HRTrainingNeed.Calendar_Year;
                        EmployeeTrainingNeedObj.DevelopmentNeed = HRTrainingNeed.Development_Need;
                        EmployeeTrainingNeedObj.InterventionRequired = HRTrainingNeed.Intervention_Required;
                        EmployeeTrainingNeedObj.InterventionRequiredOther = HRTrainingNeed.Intervention_Required_Other;
                        EmployeeTrainingNeedObj.TrainingType = HRTrainingNeed.Training_Need_Type;
                        EmployeeTrainingNeedObj.Objectives = HRTrainingNeed.Objectives;
                        EmployeeTrainingNeedObj.Description = HRTrainingNeed.Description;
                        EmployeeTrainingNeedObj.ProposedPeriod = HRTrainingNeed.Proposed_Period;
                        EmployeeTrainingNeedObj.EstimatedCost = Convert.ToDecimal(HRTrainingNeed.Estimated_Cost);
                        EmployeeTrainingNeedObj.TrainingLocation = HRTrainingNeed.Training_Location_Venue;
                        EmployeeTrainingNeedObj.TrainingScheduledDate = Convert.ToString(HRTrainingNeed.Training_Scheduled_Date);
                        EmployeeTrainingNeedObj.TrainingScheduledDateTo = Convert.ToString(HRTrainingNeed.Training_Scheduled_Date_To);
                        EmployeeTrainingNeedObj.CPDPoints = Convert.ToDecimal(HRTrainingNeed.CPD_Points_to_be_Earned);
                        EmployeeTrainingNeedObj.ProposedTrainingProvider = HRTrainingNeed.Proposed_Training_Provider;
                        EmployeeTrainingNeedObj.Status = HRTrainingNeed.Status;

                    }

                    LoadYears();
                    EmployeeTrainingNeedObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                    LoadTrainingTypes();
                    EmployeeTrainingNeedObj.TrainingTypes = new SelectList(TrainingTypes, "Text", "Value");

                    LoadInterventions();
                    EmployeeTrainingNeedObj.InterventionsRequired = new SelectList(Interventions, "Text", "Value");

                    LoadCalenderPeriod();
                    EmployeeTrainingNeedObj.ProposedPeriods = new SelectList(ProposedPeriod, "Text", "Value");

                    return View(EmployeeTrainingNeedObj);
                }
                else
                {
                    responseHeader = "Employee training need  NotFound";
                    responseMessage = "Employee training need  no." + EmpTrainingNeedNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "Employee training application no." + EmpTrainingNeedNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "TrainingNeedsHistory";
                    button1Name = "Ok";
                    button1HasParameters = false;
                    button1Parameters = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            //return View();
;
        }
        #endregion View Training Applications

        #region Employee training needs  applications History
        [Authorize]
        public ActionResult TrainingNeedsHistory()
        {
            try
            {
                List<EmployeeTrainingNeedModel> EmployeeTrainingNeedList = new List<EmployeeTrainingNeedModel>();

               
                var EmployeeTrainingNeedApplications = from EmployeeApplicationNeedsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRTrainingNeeds
                                                       where EmployeeApplicationNeedsQuery.Employee_No.Equals(employeeNo)
                                                       select EmployeeApplicationNeedsQuery;


                foreach (HRTrainingNeeds HRTrainingNeed in EmployeeTrainingNeedApplications)
                {
                    EmployeeTrainingNeedModel EmployeeTrainingNeedObj = new EmployeeTrainingNeedModel();

                    EmployeeTrainingNeedObj.ApplicationNo = HRTrainingNeed.No;
                    EmployeeTrainingNeedObj.CalenderYear = HRTrainingNeed.Calendar_Year;
                    EmployeeTrainingNeedObj.DevelopmentNeed = HRTrainingNeed.Development_Need;
                    EmployeeTrainingNeedObj.InterventionRequired = HRTrainingNeed.Intervention_Required;
                    EmployeeTrainingNeedObj.InterventionRequiredOther = HRTrainingNeed.Intervention_Required_Other;
                    EmployeeTrainingNeedObj.TrainingType = HRTrainingNeed.Training_Need_Type;
                    EmployeeTrainingNeedObj.Objectives = HRTrainingNeed.Objectives;
                    EmployeeTrainingNeedObj.Description = HRTrainingNeed.Description;
                    EmployeeTrainingNeedObj.ProposedPeriod = HRTrainingNeed.Proposed_Period;
                    EmployeeTrainingNeedObj.EstimatedCost = Convert.ToDecimal(HRTrainingNeed.Estimated_Cost);
                    EmployeeTrainingNeedObj.TrainingLocation = HRTrainingNeed.Training_Location_Venue;
                    EmployeeTrainingNeedObj.TrainingScheduledDate = Convert.ToString(HRTrainingNeed.Training_Scheduled_Date);
                    EmployeeTrainingNeedObj.TrainingScheduledDateTo = Convert.ToString(HRTrainingNeed.Training_Scheduled_Date_To);
                    EmployeeTrainingNeedObj.CPDPoints = Convert.ToDecimal(HRTrainingNeed.CPD_Points_to_be_Earned);
                    EmployeeTrainingNeedObj.ProposedTrainingProvider = HRTrainingNeed.Proposed_Training_Provider;
                    EmployeeTrainingNeedObj.Status = HRTrainingNeed.Status;

                    EmployeeTrainingNeedList.Add(EmployeeTrainingNeedObj);
                }
                return View(EmployeeTrainingNeedList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }
        #endregion Employee training applications history*/

        /*#region Training Need Approvals
        [Authorize]
        public ActionResult TrainingNeedApproval(string TrainingNeedNo)
        {

            try
            {
                if (TrainingNeedNo.Equals(""))
                {

                    return RedirectToAction("RequestsToApprove", "Approval");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingNeedApplicationExist(TrainingNeedNo, AccountController.GetEmployeeNo()))
                {

                    EmployeeTrainingNeedModel EmployeeTrainingNeedObj = new EmployeeTrainingNeedModel();

                    var EmployeeTrainingNeedApplications = from EmployeeApplicationNeedsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRTrainingNeeds
                                                           where EmployeeApplicationNeedsQuery.No.Equals(TrainingNeedNo)
                                                           select EmployeeApplicationNeedsQuery;


                    foreach (HRTrainingNeeds HRTrainingNeed in EmployeeTrainingNeedApplications)
                    {

                        EmployeeTrainingNeedObj.ApplicationNo = HRTrainingNeed.No;
                        EmployeeTrainingNeedObj.CalenderYear = HRTrainingNeed.Calendar_Year;
                        EmployeeTrainingNeedObj.DevelopmentNeed = HRTrainingNeed.Development_Need;
                        EmployeeTrainingNeedObj.InterventionRequired = HRTrainingNeed.Intervention_Required;
                        EmployeeTrainingNeedObj.InterventionRequiredOther = HRTrainingNeed.Intervention_Required_Other;
                        EmployeeTrainingNeedObj.TrainingType = HRTrainingNeed.Training_Need_Type;
                        EmployeeTrainingNeedObj.Objectives = HRTrainingNeed.Objectives;
                        EmployeeTrainingNeedObj.Description = HRTrainingNeed.Description;
                        EmployeeTrainingNeedObj.ProposedPeriod = HRTrainingNeed.Proposed_Period;
                        EmployeeTrainingNeedObj.EstimatedCost = Convert.ToDecimal(HRTrainingNeed.Estimated_Cost);
                        EmployeeTrainingNeedObj.TrainingLocation = HRTrainingNeed.Training_Location_Venue;
                        EmployeeTrainingNeedObj.TrainingScheduledDate = Convert.ToString(HRTrainingNeed.Training_Scheduled_Date);
                        EmployeeTrainingNeedObj.TrainingScheduledDateTo = Convert.ToString(HRTrainingNeed.Training_Scheduled_Date_To);
                        EmployeeTrainingNeedObj.CPDPoints = Convert.ToDecimal(HRTrainingNeed.CPD_Points_to_be_Earned);
                        EmployeeTrainingNeedObj.ProposedTrainingProvider = HRTrainingNeed.Proposed_Training_Provider;
                        EmployeeTrainingNeedObj.Status = HRTrainingNeed.Status;

                    }

                    LoadYears();
                    EmployeeTrainingNeedObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                    LoadTrainingTypes();
                    EmployeeTrainingNeedObj.TrainingTypes = new SelectList(TrainingTypes, "Text", "Value");

                    LoadInterventions();
                    EmployeeTrainingNeedObj.InterventionsRequired = new SelectList(Interventions, "Text", "Value");

                    LoadCalenderPeriod();
                    EmployeeTrainingNeedObj.ProposedPeriods = new SelectList(ProposedPeriod, "Text", "Value");

                    return View(EmployeeTrainingNeedObj);
                }
                else
                {
                    responseHeader = "Training Need NotFound";
                    responseMessage = "Training Need No." + TrainingNeedNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "Training Need No." + TrainingNeedNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                    button1ControllerName = "Approval";
                    button1ActionName = "RequestsToApprove";
                    button1Name = "Ok";
                    button1HasParameters = false;
                    button1Parameters = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
      
        [Authorize]
        [HttpPost]
        public ActionResult TrainingNeedApproval(EmployeeTrainingApplicationModel EmployeeTrainingApplicationObj, string Command)
        {

            try
            {
                if (EmployeeTrainingApplicationObj.ApplicationNo.Equals(""))
                {
                    return RedirectToAction("RequestsToApprove", "Approval");
                }
                if (Command == "Approve")
                {
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveTrainingNeed(employeeNo, EmployeeTrainingApplicationObj.ApplicationNo))
                    {
                        responseHeader = "Success";
                        responseMessage = "Training Need no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully approved.";
                        detailedResponseMessage = "Training Need no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully approved.";

                        button1ControllerName = "Approval";
                        button1ActionName = "RequestsToApprove";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        EmployeeTrainingApplicationObj.ErrorStatus = true;
                        EmployeeTrainingApplicationObj.ErrorMessage = "Unable to process the employee training request approve action. " + ServiceConnection.contactICTDepartment + "";
                        return View(EmployeeTrainingApplicationObj);
                    }
                }
                else if (Command == "Reject")
                {
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectTrainingApplication(employeeNo, EmployeeTrainingApplicationObj.ApplicationNo))
                    {
                        responseHeader = "Success";
                        responseMessage = "Training Need no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully rejected.";
                        detailedResponseMessage = "Training Need no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully rejected.";

                        button1ControllerName = "Approval";
                        button1ActionName = "RequestsToApprove";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        EmployeeTrainingApplicationObj.ErrorStatus = true;
                        EmployeeTrainingApplicationObj.ErrorMessage = "Unable to process the Employee training application request reject action. " + ServiceConnection.contactICTDepartment + "";
                        return View(EmployeeTrainingApplicationObj);
                    }
                }
                else
                {
                    EmployeeTrainingApplicationObj.ErrorStatus = true;
                    EmployeeTrainingApplicationObj.ErrorMessage = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                    return View(EmployeeTrainingApplicationObj);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        #endregion  Training Need Approvals*/

        //#region Document Management

        //[Authorize]
        //public JsonResult GetTrainingNeedApplicationDocuments(string DocumentNo)
        //{
        //    List<DocumentMgmtModel> applicationDocumentsList = new List<DocumentMgmtModel>();

        //    var TrainingNeedUploadedDocuments = from trainingNeedDocumentsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
        //                                    where trainingNeedDocumentsQuery.DocumentNo.Equals(DocumentNo)
        //                                    select trainingNeedDocumentsQuery;

        //    foreach (PortalDocuments trainingDocument in TrainingNeedUploadedDocuments)
        //    {
        //        DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();
        //        documentManagementoBJ.DocumentNo = trainingDocument.DocumentNo;
        //        documentManagementoBJ.DocumentCode = trainingDocument.Document_Code;
        //        documentManagementoBJ.DocumentDescription = trainingDocument.Document_Description;
        //        documentManagementoBJ.DocumentAttached = trainingDocument.Document_Attached ?? false;
        //        documentManagementoBJ.LocalURL = trainingDocument.Local_File_URL;
        //        documentManagementoBJ.SharePointURL = trainingDocument.SharePoint_URL;
        //        applicationDocumentsList.Add(documentManagementoBJ);
        //    }

        //    return Json(applicationDocumentsList, JsonRequestBehavior.AllowGet);
        //}

        //[Authorize]
        //public ActionResult GetTrainingNeedApplicationDocument(string DocumentNo, string DocumentCode)
        //{
        //    try
        //    {
        //        DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

        //        var trainingNeedDocuments = from trainingDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
        //                                where trainingDocumentQuery.DocumentNo.Equals(DocumentNo) && trainingDocumentQuery.Document_Code.Equals(DocumentCode)
        //                                select trainingDocumentQuery;

        //        foreach (PortalDocuments trainingDocument in trainingNeedDocuments)
        //        {
        //            documentManagementoBJ.DocumentNo = trainingDocument.DocumentNo;
        //            documentManagementoBJ.DocumentCode = trainingDocument.Document_Code;
        //            documentManagementoBJ.DocumentDescription = trainingDocument.Document_Description;
        //            documentManagementoBJ.DocumentAttached = trainingDocument.Document_Attached ?? false;
        //            documentManagementoBJ.LocalURL = trainingDocument.Local_File_URL;
        //            documentManagementoBJ.SharePointURL = trainingDocument.SharePoint_URL;
        //        }

        //        return Json(documentManagementoBJ, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return errorResponse.ApplicationExceptionError(ex);
        //    }
        //}

        //[Authorize]
        //[HttpPost]
        //public JsonResult UploadTrainingNeedApplicationDocument(string DocumentNo, string DocumentCode, string DocumentDescription)
        //{
        //    try
        //    {
        //        if (Request.Files.Count > 0)
        //        {
        //            var root = "~/StaffData/" + employeeNo;
        //            bool folderpath = System.IO.Directory.Exists(HttpContext.Server.MapPath(root));

        //            if (!folderpath)
        //            {
        //                System.IO.Directory.CreateDirectory(HttpContext.Server.MapPath(root));
        //            }

        //            var file = Request.Files[0];
        //            string fileExt = System.IO.Path.GetExtension(file.FileName).ToLower();
        //            string fileName = DocumentNo + "_" + DocumentDescription + fileExt;
        //            string path = System.IO.Path.Combine(HttpContext.Server.MapPath(root), fileName);

        //            if (System.IO.File.Exists(path))
        //            {
        //                System.IO.File.Delete(path);
        //            }

        //            file.SaveAs(path);

        //            if (System.IO.File.Exists(path))
        //            {
        //                dynamicsNAVSOAPServices.documentMgmt.ModifySystemFileURL(DocumentNo, DocumentCode, path);
        //                return Json(new { success = true, message = DocumentDescription + " uploaded successfully" }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = DocumentDescription + " was not uploaded. Try Again." }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        return Json(new { success = false, message = "Please select a pdf file!" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[Authorize]
        //[ChildActionOnly]
        //public ActionResult _EmployeeTrainingNeedDocumentLine(string DocumentNo)
        //{

        //    DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

        //    return PartialView(documentManagementoBJ);
        //}

        //[Authorize]
        //[ChildActionOnly]
        //public ActionResult _ViewEmployeeTrainingNeedDocumentLine(string DocumentNo)
        //{


        //    DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

        //    return PartialView(documentManagementoBJ);
        //}

        //#endregion Document Management

        /*#region New Training Application

        [Authorize]
        public ActionResult NewEmployeeTrainingApplication()
        {
            string OpenTrainingApplicationNo = "";
            string ApplicationNo = "";
            try
            {

                OpenTrainingApplicationNo = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckOpenTrainingApplicationExists(employeeNo);


                //check open employee training application
                if (!OpenTrainingApplicationNo.Equals(""))
                {


                    responseHeader = "Employee training Application Exist";
                    responseMessage = "An open Application No. " + OpenTrainingApplicationNo + " exist under employee no. " + employeeNo + ", finalize on this training application before creating a new one.";
                    detailedResponseMessage = "An open Application No. " + OpenTrainingApplicationNo + " exist under employee no. " + employeeNo + ", finalize on this training application before creating a new one.";

                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "EmployeeApplicationsHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                }
                //end check open employee training application

                //create Employee application training

                ApplicationNo = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CreateTrainingApplication(employeeNo);

                EmployeeTrainingApplicationModel EmployeeTrainingApplicationObj = new EmployeeTrainingApplicationModel();


                EmployeeTrainingApplicationObj.ApplicationNo = ApplicationNo;
                EmployeeTrainingApplicationObj.EmployeeNo = employeeNo;
                EmployeeTrainingApplicationObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

                LoadYears();
                EmployeeTrainingApplicationObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                LoadApprovedTrainingNeeds();
                EmployeeTrainingApplicationObj.TrainingNeedNos = new SelectList(ApprovedTrainingNeeds, "No", "Description");

                return View(EmployeeTrainingApplicationObj);
            }
            catch (Exception ex)
            {

                return errorResponse.ApplicationExceptionError(ex);
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult NewEmployeeTrainingApplication(EmployeeTrainingApplicationModel EmployeeTrainingApplicationObj)
        {
            bool EmployeeTrainingModified = false;
            bool approvalWorkflowExist = false;
            try
            {
                LoadYears();
                EmployeeTrainingApplicationObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                LoadApprovedTrainingNeeds();
                EmployeeTrainingApplicationObj.TrainingNeedNos = new SelectList(ApprovedTrainingNeeds, "No", "Description");

                if (ModelState.IsValid)
                {
                    if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingApplicationExist(EmployeeTrainingApplicationObj.ApplicationNo, AccountController.GetEmployeeNo()))
                    {

                        //modify Employee training application

                        EmployeeTrainingModified = dynamicsNAVSOAPServices.employeeTrainingManagementWS.ModifyTrainingApplication(EmployeeTrainingApplicationObj.ApplicationNo, EmployeeTrainingApplicationObj.TrainingNeed, EmployeeTrainingApplicationObj.Year, DateTime.Parse(EmployeeTrainingApplicationObj.FromDate), DateTime.Parse(EmployeeTrainingApplicationObj.ToDate), EmployeeTrainingApplicationObj.Description, EmployeeTrainingApplicationObj.Purpose);
                        if (!EmployeeTrainingModified)
                        {
                            EmployeeTrainingApplicationObj.ErrorStatus = true;
                            EmployeeTrainingApplicationObj.ErrorMessage = "An error was experienced while trying to modify Employee training no." + EmployeeTrainingApplicationObj.ApplicationNo + ", the server might be offline, try again after a while.";
                            return View(EmployeeTrainingApplicationObj);
                        }
                        //send for approval
                        approvalWorkflowExist = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingApplicationApprovalWorkflowEnabled(EmployeeTrainingApplicationObj.ApplicationNo);
                        if (!approvalWorkflowExist)
                        {
                            EmployeeTrainingApplicationObj.ErrorStatus = true;
                            EmployeeTrainingApplicationObj.ErrorMessage = "An error was experienced while trying to send an approval request for employee training no." + EmployeeTrainingApplicationObj.ApplicationNo + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
                            return View(EmployeeTrainingApplicationObj);
                        }

                        if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.SendTrainingApplicationApprovalRequest(EmployeeTrainingApplicationObj.ApplicationNo))
                        {

                            responseHeader = "Success";
                            responseMessage = "Employee training application no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully sent for approval. Check with the " + companyName + " human resource department for approval status.";
                            detailedResponseMessage = "Employee training application no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully sent for approval. Check with the " + companyName + "  human resource department for approval status.";
                            button1ControllerName = "EmployeeTraining";
                            button1ActionName = "EmployeeApplicationsHistory";
                            button1Name = "Ok";
                            button1HasParameters = false;
                            button1Parameters = "";
                            return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                        }
                        else
                        {
                            EmployeeTrainingApplicationObj.ErrorStatus = true;
                            EmployeeTrainingApplicationObj.ErrorMessage = "An error was experienced while trying to send an approval request for employee training no." + EmployeeTrainingApplicationObj.ApplicationNo + ". " + ServiceConnection.contactICTDepartment + "";
                            return View(EmployeeTrainingApplicationObj);
                        }
                    }
                    else
                    {
                        responseHeader = "Training Application NotFound";
                        responseMessage = "The employee training no." + EmployeeTrainingApplicationObj.ApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                        detailedResponseMessage = "The employee training no." + EmployeeTrainingApplicationObj.ApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "EmployeeApplicationsHistory";
                        button1Name = "Ok";
                        button1HasParameters = false;
                        button1Parameters = "";
                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                    }

                }
                else
                {

                    return View(EmployeeTrainingApplicationObj);
                }

            }


            catch (Exception ex)
            {
                EmployeeTrainingApplicationObj.ErrorStatus = true;
                EmployeeTrainingApplicationObj.ErrorMessage = ex.Message.ToString();
                return View(EmployeeTrainingApplicationObj);
            }
        }

        #endregion New Training Application

        #region Edit Training applications
        public ActionResult OnBeforeEdit(string EmpTrainingNo)
        {

            try
            {
                if (EmpTrainingNo.Equals(""))
                {
                    return RedirectToAction("EmployeeApplicationsHistory", "EmployeeTraining");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingApplicationExist(EmpTrainingNo, AccountController.GetEmployeeNo()))
                {
                    string EmpTrainingStatus = dynamicsNAVSOAPServices.employeeTrainingManagementWS.GetTrainingApplicationStatus(EmpTrainingNo);
                    //if training application is open
                    if (EmpTrainingStatus.Equals("Open"))
                    {
                        return RedirectToAction("EditEmployeeTrainingApplication", "EmployeeTraining", new { EmpTrainingNo = EmpTrainingNo });
                    }

                    //if training application is pending approval
                    if (EmpTrainingStatus.Equals("Pending Approval"))
                    {
                        responseHeader = "Employee Training Request Pending Approval";
                        responseMessage = "The Employee training no." + EmpTrainingNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
                        detailedResponseMessage = "The Employee training." + EmpTrainingNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "EditEmployeeTrainingApplication";
                        button1HasParameters = true;
                        button1Parameters = "?EmpTrainingNo=" + EmpTrainingNo;
                        button1Name = "Yes";

                        button2ControllerName = "EmployeeTraining";
                        button2ActionName = "EmployeeApplicationsHistory";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }

                    //if Training application is released
                    if (EmpTrainingStatus.Equals("Released"))
                    {
                        responseHeader = "Training Application Approved";
                        responseMessage = "The Employee training application no." + EmpTrainingNo + " is already approved. Editing not allowed.";
                        detailedResponseMessage = "The Employee training application." + EmpTrainingNo + " is already approved. Editing not allowed.";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "EmployeeApplicationsHistory";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    //if training application is rejected
                    if (EmpTrainingStatus.Equals("Rejected"))
                    {
                        responseHeader = "Training Application Rejected";
                        responseMessage = "The employee training application no." + EmpTrainingNo + " was rejected. Editing will reopen the document. Do you want to continue?";
                        detailedResponseMessage = "The employee training application." + EmpTrainingNo + " was rejected. Editing will reopen the document. Do you want to continue?";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "EditEmployeeTrainingApplication";
                        button1HasParameters = true;
                        button1Parameters = "?EmpTrainingNo=" + EmpTrainingNo;
                        button1Name = "Yes";

                        button2ControllerName = "EmployeeTraining";
                        button2ActionName = "EmployeeApplicationsHistory";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    //if application training is posted/reversed
                    if (EmpTrainingStatus.Equals("Posted") || EmpTrainingStatus.Equals("Reversed"))
                    {
                        responseHeader = "Application Training Posted";
                        responseMessage = "The employee training application." + EmpTrainingNo + " is already posted. Editing not allowed.";
                        detailedResponseMessage = "employee training application." + EmpTrainingNo + " is already posted. Editing not allowed.";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "EmployeeApplicationsHistory";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    return RedirectToAction("EmployeeApplicationsHistory", "EmployeeTraining");
                }
                else
                {
                    responseHeader = "Employee Application Training NotFound";
                    responseMessage = "The  employee training application no." + EmpTrainingNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "The  employee training application." + EmpTrainingNo + " was not found under employee no." + AccountController.GetEmployeeNo();

                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "EmployeeApplicationsHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";

                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }

        [Authorize]
        public ActionResult EditEmployeeTrainingApplication(string EmpTrainingNo)
        {

            try
            {

                if (EmpTrainingNo.Equals(""))
                {
                    return RedirectToAction("EmployeeApplicationsHistory", "EmployeeTraining");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingApplicationExist(EmpTrainingNo, AccountController.GetEmployeeNo()))
                {
                    string EmpTrainingStatus = dynamicsNAVSOAPServices.employeeTrainingManagementWS.GetTrainingApplicationStatus(EmpTrainingNo);

                    //if application training is pending approval, cancel approval request
                    if (EmpTrainingStatus.Equals("Pending Approval"))
                    {
                        dynamicsNAVSOAPServices.employeeTrainingManagementWS.CancelTrainingApplicationApprovalRequest(EmpTrainingNo);
                    }
                    //if application training is released, reopen and uncommit from budget
                    if (EmpTrainingStatus.Equals("Released"))
                    {
                        dynamicsNAVSOAPServices.employeeTrainingManagementWS.CancelTrainingApplicationApprovalRequest(EmpTrainingNo);

                    }
                    //if application training is rejected, reopen document
                    if (EmpTrainingStatus.Equals("Rejected"))
                    {
                        dynamicsNAVSOAPServices.employeeTrainingManagementWS.CancelTrainingApplicationApprovalRequest(EmpTrainingNo);
                    }

                    EmployeeTrainingApplicationModel EmployeeTrainingApplicationObj = new EmployeeTrainingApplicationModel();

                    var EmployeeTrainingApplications = from EmployeeTrainingApplicationsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRTrainingApplications
                                                       where EmployeeTrainingApplicationsQuery.Application_No.Equals(EmpTrainingNo)
                                                       select EmployeeTrainingApplicationsQuery;


                    foreach (HRTrainingApplications HRTrainingApplications in EmployeeTrainingApplications)
                    {

                        EmployeeTrainingApplicationObj.ApplicationNo = HRTrainingApplications.Application_No;
                        EmployeeTrainingApplicationObj.EmployeeName = HRTrainingApplications.Employee_Name;
                        EmployeeTrainingApplicationObj.Description = HRTrainingApplications.Description;
                        EmployeeTrainingApplicationObj.FromDate = HRTrainingApplications.From_Date.Value.ToShortDateString();
                        EmployeeTrainingApplicationObj.ToDate = HRTrainingApplications.To_Date.Value.ToShortDateString();
                        EmployeeTrainingApplicationObj.NoOfDays = HRTrainingApplications.Number_of_Days;
                        (EmployeeTrainingApplicationObj.CostOfTraining) = Convert.ToDecimal(HRTrainingApplications.Estimated_Cost_Of_Training);
                        EmployeeTrainingApplicationObj.Location = HRTrainingApplications.Location;
                        EmployeeTrainingApplicationObj.Purpose = HRTrainingApplications.Purpose_of_Training;
                        EmployeeTrainingApplicationObj.Status = HRTrainingApplications.Status;

                    }

                    LoadYears();
                    EmployeeTrainingApplicationObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                    LoadApprovedTrainingNeeds();
                    EmployeeTrainingApplicationObj.TrainingNeedNos = new SelectList(ApprovedTrainingNeeds, "No", "Description");

                    return View(EmployeeTrainingApplicationObj);
                }
                else
                {
                    responseHeader = "Employee Training Application NotFound";
                    responseMessage = "The Employee training application no." + EmpTrainingNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = " Employee training application no." + EmpTrainingNo + " was not found under employee no." + AccountController.GetEmployeeNo();

                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "EmployeeApplicationsHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";

                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }

            }
            catch (Exception ex)
            {

                return errorResponse.ApplicationExceptionError(ex);
            }


        }

        [Authorize]
        [HttpPost]
        public ActionResult EditEmployeeTrainingApplication(EmployeeTrainingApplicationModel EmployeeTrainingApplicationObj)
        {

            bool EmployeeTrainingModified = false;
            bool approvalWorkflowExist = false;
            try
            {
                LoadYears();
                EmployeeTrainingApplicationObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                LoadApprovedTrainingNeeds();
                EmployeeTrainingApplicationObj.TrainingNeedNos = new SelectList(ApprovedTrainingNeeds, "No", "Description");

                if (ModelState.IsValid)
                {
                    if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingApplicationExist(EmployeeTrainingApplicationObj.ApplicationNo, AccountController.GetEmployeeNo()))
                    {

                        //modify Employee training application

                        EmployeeTrainingModified = dynamicsNAVSOAPServices.employeeTrainingManagementWS.ModifyTrainingApplication(EmployeeTrainingApplicationObj.ApplicationNo, EmployeeTrainingApplicationObj.TrainingNeed, EmployeeTrainingApplicationObj.Year, DateTime.Parse(EmployeeTrainingApplicationObj.FromDate), DateTime.Parse(EmployeeTrainingApplicationObj.ToDate), EmployeeTrainingApplicationObj.Description, EmployeeTrainingApplicationObj.Purpose);
                        if (!EmployeeTrainingModified)
                        {
                            EmployeeTrainingApplicationObj.ErrorStatus = true;
                            EmployeeTrainingApplicationObj.ErrorMessage = "An error was experienced while trying to modify Employee training no." + EmployeeTrainingApplicationObj.ApplicationNo + ", the server might be offline, try again after a while.";
                            return View(EmployeeTrainingApplicationObj);
                        }
                        //send for approval
                        approvalWorkflowExist = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingApplicationApprovalWorkflowEnabled(EmployeeTrainingApplicationObj.ApplicationNo);
                        if (!approvalWorkflowExist)
                        {
                            EmployeeTrainingApplicationObj.ErrorStatus = true;
                            EmployeeTrainingApplicationObj.ErrorMessage = "An error was experienced while trying to send an approval request for employee training no." + EmployeeTrainingApplicationObj.ApplicationNo + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
                            return View(EmployeeTrainingApplicationObj);
                        }

                        if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.SendTrainingApplicationApprovalRequest(EmployeeTrainingApplicationObj.ApplicationNo))
                        {

                            responseHeader = "Success";
                            responseMessage = "Employee training application no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully sent for approval. Check with the " + companyName + " human resource department for approval status.";
                            detailedResponseMessage = "Employee training application no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully sent for approval. Check with the " + companyName + "  human resource department for approval status.";
                            button1ControllerName = "EmployeeTraining";
                            button1ActionName = "EmployeeApplicationsHistory";
                            button1Name = "Ok";
                            button1HasParameters = false;
                            button1Parameters = "";
                            return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                        }
                        else
                        {
                            EmployeeTrainingApplicationObj.ErrorStatus = true;
                            EmployeeTrainingApplicationObj.ErrorMessage = "An error was experienced while trying to send an approval request for employee training no." + EmployeeTrainingApplicationObj.ApplicationNo + ". " + ServiceConnection.contactICTDepartment + "";
                            return View(EmployeeTrainingApplicationObj);
                        }
                    }
                    else
                    {
                        responseHeader = "Employee training NotFound";
                        responseMessage = "The employee training no." + EmployeeTrainingApplicationObj.ApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                        detailedResponseMessage = "The employee training no." + EmployeeTrainingApplicationObj.ApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "EmployeeApplicationsHistory";
                        button1Name = "Ok";
                        button1HasParameters = false;
                        button1Parameters = "";
                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                    }

                }
                else
                {

                    return View(EmployeeTrainingApplicationObj);
                }

            }


            catch (Exception ex)
            {
                EmployeeTrainingApplicationObj.ErrorStatus = true;
                EmployeeTrainingApplicationObj.ErrorMessage = ex.Message.ToString();
                return View(EmployeeTrainingApplicationObj);
            }
        }
        #endregion Edit Training applications

        #region View Training Applications
        public ActionResult ViewTrainingRequests(string ApplicationTrainingNo)
        {

            try
            {
                if (ApplicationTrainingNo.Equals(""))
                {

                    return RedirectToAction("EmployeeApplicationsHistory", "EmployeeTraining");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingApplicationExist(ApplicationTrainingNo, AccountController.GetEmployeeNo()))
                {

                    EmployeeTrainingApplicationModel EmployeeTrainingApplicationObj = new EmployeeTrainingApplicationModel();

                    var EmployeeTrainingApplications = from EmployeeTrainingApplicationsQuery in dynamicsNAVODataServices.BCOData.HRTrainingApplications
                                                       where EmployeeTrainingApplicationsQuery.Application_No.Equals(ApplicationTrainingNo)
                                                       select EmployeeTrainingApplicationsQuery;


                    foreach (HRTrainingApplications HRTrainingApplications in EmployeeTrainingApplications)
                    {
                        EmployeeTrainingApplicationObj.ApplicationNo = HRTrainingApplications.Application_No;
                        EmployeeTrainingApplicationObj.EmployeeName = HRTrainingApplications.Employee_Name;
                        EmployeeTrainingApplicationObj.Description = HRTrainingApplications.Description;
                        EmployeeTrainingApplicationObj.FromDate = HRTrainingApplications.From_Date.Value.ToShortDateString();
                        EmployeeTrainingApplicationObj.ToDate = HRTrainingApplications.To_Date.Value.ToShortDateString();
                        EmployeeTrainingApplicationObj.NoOfDays = HRTrainingApplications.Number_of_Days;
                        (EmployeeTrainingApplicationObj.CostOfTraining) = Convert.ToDecimal(HRTrainingApplications.Estimated_Cost_Of_Training);
                        EmployeeTrainingApplicationObj.Location = HRTrainingApplications.Location;
                        EmployeeTrainingApplicationObj.Purpose = HRTrainingApplications.Purpose_of_Training;
                        EmployeeTrainingApplicationObj.Status = HRTrainingApplications.Status;
                    }

                    LoadYears();
                    EmployeeTrainingApplicationObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                    LoadApprovedTrainingNeeds();
                    EmployeeTrainingApplicationObj.TrainingNeedNos = new SelectList(ApprovedTrainingNeeds, "No", "Description");
                    return View(EmployeeTrainingApplicationObj);

                }
                else
                {
                    responseHeader = "Employee training application NotFound";
                    responseMessage = "Employee training application no." + ApplicationTrainingNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "Employee training application no." + ApplicationTrainingNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "EmployeeApplicationsHistory";
                    button1Name = "Ok";
                    button1HasParameters = false;
                    button1Parameters = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            //return View();
;
        }
        #endregion View Training Applications

        #region Employee training applications History
        [Authorize]
        public ActionResult EmployeeApplicationsHistory()
        {
            try
            {
                List<EmployeeTrainingApplicationModel> EmployeeTrainingApplicationList = new List<EmployeeTrainingApplicationModel>();

                var EmployeeTrainingApplications = from EmployeeTrainingApplicationsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRTrainingApplications
                                                   where EmployeeTrainingApplicationsQuery.Employee_No.Equals(employeeNo)
                                                   select EmployeeTrainingApplicationsQuery;

                foreach (HRTrainingApplications HRTrainingApplications in EmployeeTrainingApplications)
                {
                    EmployeeTrainingApplicationModel EmployeeTrainingApplicationObj = new EmployeeTrainingApplicationModel();
                    EmployeeTrainingApplicationObj.ApplicationNo = HRTrainingApplications.Application_No;
                    EmployeeTrainingApplicationObj.TrainingNeed = HRTrainingApplications.Training_Need_Type;
                    // EmployeeTrainingApplicationObj.TypeOfTraining = HRTrainingApplications.Type_of_Training;
                    EmployeeTrainingApplicationObj.Year = HRTrainingApplications.Calendar_Year;
                    EmployeeTrainingApplicationObj.Description = HRTrainingApplications.Description;
                    EmployeeTrainingApplicationObj.FromDate = HRTrainingApplications.From_Date.Value.ToShortDateString();
                    EmployeeTrainingApplicationObj.ToDate = HRTrainingApplications.To_Date.Value.ToShortDateString();
                    //EmployeeTrainingApplicationObj.NoOfDays = HRTrainingApplications.Number_of_Days;
                    //(EmployeeTrainingApplicationObj.CostOfTraining) = Convert.ToDecimal(HRTrainingApplications.Estimated_Cost_Of_Training);
                    // EmployeeTrainingApplicationObj.Location = HRTrainingApplications.Location;
                    //  EmployeeTrainingApplicationObj.TrainingPurpose = HRTrainingApplications.Purpose_of_Training;

                    EmployeeTrainingApplicationObj.Status = HRTrainingApplications.Status;

                    EmployeeTrainingApplicationList.Add(EmployeeTrainingApplicationObj);
                }
                return View(EmployeeTrainingApplicationList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }
        #endregion Employee training applications history

        #region Training Applications Approvals
        [Authorize]
        public ActionResult TrainingApplicationApproval(string ApplicationNo)
        {

            try
            {
                if (ApplicationNo.Equals(""))
                {

                    return RedirectToAction("RequestsToApprove", "Approval");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingApplicationExist(ApplicationNo, AccountController.GetEmployeeNo()))
                {

                    EmployeeTrainingApplicationModel EmployeeTrainingApplicationObj = new EmployeeTrainingApplicationModel();

                    var EmployeeTrainingApplications = from EmployeeTrainingApplicationsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRTrainingApplications
                                                       where EmployeeTrainingApplicationsQuery.Application_No.Equals(ApplicationNo)
                                                       select EmployeeTrainingApplicationsQuery;


                    foreach (HRTrainingApplications HRTrainingApplications in EmployeeTrainingApplications)
                    {

                        EmployeeTrainingApplicationObj.ApplicationNo = HRTrainingApplications.Application_No;
                        EmployeeTrainingApplicationObj.EmployeeName = HRTrainingApplications.Name;
                        EmployeeTrainingApplicationObj.Description = HRTrainingApplications.Description;
                        EmployeeTrainingApplicationObj.FromDate = HRTrainingApplications.From_Date.Value.ToShortDateString();
                        EmployeeTrainingApplicationObj.ToDate = HRTrainingApplications.To_Date.Value.ToShortDateString();
                        EmployeeTrainingApplicationObj.NoOfDays = HRTrainingApplications.Number_of_Days;
                        EmployeeTrainingApplicationObj.CostOfTraining = Convert.ToDecimal(HRTrainingApplications.Estimated_Cost_Of_Training);
                        EmployeeTrainingApplicationObj.Location = HRTrainingApplications.Location;
                        EmployeeTrainingApplicationObj.Purpose = HRTrainingApplications.Purpose_of_Training;
                        EmployeeTrainingApplicationObj.Status = HRTrainingApplications.Status;

                        LoadYears();
                        EmployeeTrainingApplicationObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                        LoadApprovedTrainingNeeds();
                        EmployeeTrainingApplicationObj.TrainingNeedNos = new SelectList(ApprovedTrainingNeeds, "No", "Description");
                    }


                    return View(EmployeeTrainingApplicationObj);


                }
                else
                {
                    responseHeader = "Training Application NotFound";
                    responseMessage = "Training application no." + ApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "Training application no." + ApplicationNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    button1ControllerName = "Approval";
                    button1ActionName = "RequestsToApprove";
                    button1Name = "Ok";
                    button1HasParameters = false;
                    button1Parameters = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
      
        [Authorize]
        [HttpPost]
        public ActionResult TrainingApplicationApproval(EmployeeTrainingApplicationModel EmployeeTrainingApplicationObj, string Command)
        {

            try
            {
                if (EmployeeTrainingApplicationObj.ApplicationNo.Equals(""))
                {
                    return RedirectToAction("RequestsToApprove", "Approval");
                }
                if (Command == "Approve")
                {
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveTrainingApplication(employeeNo, EmployeeTrainingApplicationObj.ApplicationNo))
                    {
                        responseHeader = "Success";
                        responseMessage = "Training application no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully approved.";
                        detailedResponseMessage = "Training application no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully approved.";

                        button1ControllerName = "Approval";
                        button1ActionName = "RequestsToApprove";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        EmployeeTrainingApplicationObj.ErrorStatus = true;
                        EmployeeTrainingApplicationObj.ErrorMessage = "Unable to process the training application approve action. " + ServiceConnection.contactICTDepartment + "";
                        return View(EmployeeTrainingApplicationObj);
                    }
                }
                else if (Command == "Reject")
                {
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectTrainingApplication(employeeNo, EmployeeTrainingApplicationObj.ApplicationNo))
                    {
                        responseHeader = "Success";
                        responseMessage = "Training application no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully rejected.";
                        detailedResponseMessage = "Training application no." + EmployeeTrainingApplicationObj.ApplicationNo + " was successfully rejected.";

                        button1ControllerName = "Approval";
                        button1ActionName = "RequestsToApprove";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        EmployeeTrainingApplicationObj.ErrorStatus = true;
                        EmployeeTrainingApplicationObj.ErrorMessage = "Unable to process the training application request reject action. " + ServiceConnection.contactICTDepartment + "";
                        return View(EmployeeTrainingApplicationObj);
                    }
                }
                else
                {
                    EmployeeTrainingApplicationObj.ErrorStatus = true;
                    EmployeeTrainingApplicationObj.ErrorMessage = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                    return View(EmployeeTrainingApplicationObj);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        #endregion  Training Applications Approvals*/

        //#region Document Management

        //[Authorize]
        //public JsonResult GetTrainingApplicationDocuments(string DocumentNo)
        //{
        //    List<DocumentMgmtModel> applicationDocumentsList = new List<DocumentMgmtModel>();

        //    var TrainingUploadedDocuments = from trainingDocumentsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
        //                                    where trainingDocumentsQuery.DocumentNo.Equals(DocumentNo)
        //                                    select trainingDocumentsQuery;

        //    foreach (PortalDocuments trainingDocument in TrainingUploadedDocuments)
        //    {
        //        DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();
        //        documentManagementoBJ.DocumentNo = trainingDocument.DocumentNo;
        //        documentManagementoBJ.DocumentCode = trainingDocument.Document_Code;
        //        documentManagementoBJ.DocumentDescription = trainingDocument.Document_Description;
        //        documentManagementoBJ.DocumentAttached = trainingDocument.Document_Attached ?? false;
        //        documentManagementoBJ.LocalURL = trainingDocument.Local_File_URL;
        //        documentManagementoBJ.SharePointURL = trainingDocument.SharePoint_URL;
        //        applicationDocumentsList.Add(documentManagementoBJ);
        //    }

        //    return Json(applicationDocumentsList, JsonRequestBehavior.AllowGet);
        //}

        //[Authorize]
        //public ActionResult GetTrainingApplicationDocument(string DocumentNo, string DocumentCode)
        //{
        //    try
        //    {
        //        DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

        //        var trainingDocuments = from trainingDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
        //                                where trainingDocumentQuery.DocumentNo.Equals(DocumentNo) && trainingDocumentQuery.Document_Code.Equals(DocumentCode)
        //                                select trainingDocumentQuery;

        //        foreach (PortalDocuments trainingDocument in trainingDocuments)
        //        {
        //            documentManagementoBJ.DocumentNo = trainingDocument.DocumentNo;
        //            documentManagementoBJ.DocumentCode = trainingDocument.Document_Code;
        //            documentManagementoBJ.DocumentDescription = trainingDocument.Document_Description;
        //            documentManagementoBJ.DocumentAttached = trainingDocument.Document_Attached ?? false;
        //            documentManagementoBJ.LocalURL = trainingDocument.Local_File_URL;
        //            documentManagementoBJ.SharePointURL = trainingDocument.SharePoint_URL;
        //        }

        //        return Json(documentManagementoBJ, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return errorResponse.ApplicationExceptionError(ex);
        //    }
        //}

        //[Authorize]
        //[HttpPost]
        //public JsonResult UploadTrainingApplicationDocument(string DocumentNo, string DocumentCode, string DocumentDescription)
        //{
        //    try
        //    {
        //        if (Request.Files.Count > 0)
        //        {
        //            var root = "~/StaffData/" + employeeNo;
        //            bool folderpath = System.IO.Directory.Exists(HttpContext.Server.MapPath(root));

        //            if (!folderpath)
        //            {
        //                System.IO.Directory.CreateDirectory(HttpContext.Server.MapPath(root));
        //            }

        //            var file = Request.Files[0];
        //            string fileExt = System.IO.Path.GetExtension(file.FileName).ToLower();
        //            string fileName = DocumentNo + "_" + DocumentDescription + fileExt;
        //            string path = System.IO.Path.Combine(HttpContext.Server.MapPath(root), fileName);

        //            if (System.IO.File.Exists(path))
        //            {
        //                System.IO.File.Delete(path);
        //            }

        //            file.SaveAs(path);

        //            if (System.IO.File.Exists(path))
        //            {
        //                dynamicsNAVSOAPServices.documentMgmt.ModifySystemFileURL(DocumentNo, DocumentCode, path);
        //                return Json(new { success = true, message = DocumentDescription + " uploaded successfully" }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = DocumentDescription + " was not uploaded. Try Again." }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        return Json(new { success = false, message = "Please select a pdf file!" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[Authorize]
        //[ChildActionOnly]
        //public ActionResult _EmployeeTrainingDocumentLine(string DocumentNo)
        //{

        //    DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

        //    return PartialView(documentManagementoBJ);
        //}

        //[Authorize]
        //[ChildActionOnly]
        //public ActionResult _ViewEmployeeTrainingDocumentLine(string DocumentNo)
        //{


        //    DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

        //    return PartialView(documentManagementoBJ);
        //}

        //#endregion Document Management

        /*#region New Training Evaluation Application

        [Authorize]
        public ActionResult NewTrainingEvaluationApplication()
        {
            string OpenEvaluationNo = "";
            bool employeeTrainingEvaluationCreated = false;
            try
            {

                OpenEvaluationNo = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckOpenTrainingEvaluationExists(employeeNo);


                //check open employee training application
                if (!OpenEvaluationNo.Equals(""))
                {


                    responseHeader = "Employee training evaluation Application Exist";
                    responseMessage = "An open training evaluation request no . " + OpenEvaluationNo + ",exist under employee no. " + employeeNo + ", finalize on this training application before creating a new one.";
                    detailedResponseMessage = "An open training evaluation request no . " + OpenEvaluationNo + ",exist under employee no. " + employeeNo + ", finalize on this training application before creating a new one.";

                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "TrainingEvaluationsHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                }
                //end check open employee evaluation application

                //create Employee evaluation training

                employeeTrainingEvaluationCreated = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CreateTrainingEvaluation(employeeNo);

                OpenEvaluationNo = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckOpenTrainingEvaluationExists(employeeNo);

                EmployeeTrainingEvaluationModel employeeTrainingEvaluationObj = new EmployeeTrainingEvaluationModel();


                employeeTrainingEvaluationObj.TrainingEvaluationNo = OpenEvaluationNo;
                employeeTrainingEvaluationObj.EmployeeNo = employeeNo;
                employeeTrainingEvaluationObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

                LoadYears();
                employeeTrainingEvaluationObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                LoadTrainingApplications();
                employeeTrainingEvaluationObj.Applications = new SelectList(TrainingApplications, "Application_No", "Description");

                LoadForObjectivesofTraining();
                LoadForParticipationAndInterractions();
                LoadForCoveredTopics();
                LoadForOrganizedContents();
                LoadForTrainingMaterials();
                LoadForTrainingExperience();
                LoadForTrainerWellPrepared();
                LoadForTrainingObjectivesMet();
                LoadForRatings();
                LoadForTrainerPreparedness();

                employeeTrainingEvaluationObj.TrainingObjectives = new SelectList(As, "Text", "Value");
                employeeTrainingEvaluationObj.Participations = new SelectList(Bs, "Text", "Value");
                employeeTrainingEvaluationObj.Topics = new SelectList(Cs, "Text", "Value");
                employeeTrainingEvaluationObj.Contents = new SelectList(Ds, "Text", "Value");
                employeeTrainingEvaluationObj.Materials = new SelectList(Es, "Text", "Value");
                employeeTrainingEvaluationObj.TrainingExperiences = new SelectList(Fs, "Text", "Value");
                employeeTrainingEvaluationObj.Trainers = new SelectList(Gs, "Text", "Value");
                employeeTrainingEvaluationObj.ObjectivesMet = new SelectList(Hs, "Text", "Value");
                employeeTrainingEvaluationObj.Ratings = new SelectList(Is, "Text", "Value");
                employeeTrainingEvaluationObj.TrainerPreparedness = new SelectList(Js, "Text", "Value");

                return View(employeeTrainingEvaluationObj);
            }
            catch (Exception ex)
            {

                return errorResponse.ApplicationExceptionError(ex);
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult NewTrainingEvaluationApplication(EmployeeTrainingEvaluationModel employeeTrainingEvaluationObj)
        {
            bool EmployeeTrainingEvaluationModified = false;
            bool approvalWorkflowExist = false;
            string OpenTrainingEvaluationNo = "";

            if (ModelState.IsValid)
            {

                LoadYears();
                employeeTrainingEvaluationObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                LoadTrainingApplications();
                employeeTrainingEvaluationObj.Applications = new SelectList(TrainingApplications, "Application_No", "Description");

                LoadForObjectivesofTraining();
                LoadForParticipationAndInterractions();
                LoadForCoveredTopics();
                LoadForOrganizedContents();
                LoadForTrainingMaterials();
                LoadForTrainingExperience();
                LoadForTrainerWellPrepared();
                LoadForTrainingObjectivesMet();
                LoadForRatings();
                LoadForTrainerPreparedness();

                employeeTrainingEvaluationObj.TrainingObjectives = new SelectList(As, "Text", "Value");
                employeeTrainingEvaluationObj.Participations = new SelectList(Bs, "Text", "Value");
                employeeTrainingEvaluationObj.Topics = new SelectList(Cs, "Text", "Value");
                employeeTrainingEvaluationObj.Contents = new SelectList(Ds, "Text", "Value");
                employeeTrainingEvaluationObj.Materials = new SelectList(Es, "Text", "Value");
                employeeTrainingEvaluationObj.TrainingExperiences = new SelectList(Fs, "Text", "Value");
                employeeTrainingEvaluationObj.Trainers = new SelectList(Gs, "Text", "Value");
                employeeTrainingEvaluationObj.ObjectivesMet = new SelectList(Hs, "Text", "Value");
                employeeTrainingEvaluationObj.Ratings = new SelectList(Is, "Text", "Value");
                employeeTrainingEvaluationObj.TrainerPreparedness = new SelectList(Js, "Text", "Value");

                try
                {
                    OpenTrainingEvaluationNo = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckOpenTrainingEvaluationExists(employeeNo);

                    if (!OpenTrainingEvaluationNo.Equals(""))
                    {
                        if (!dynamicsNAVSOAPServices.documentMgmt.CheckDocumentAttached(employeeTrainingEvaluationObj.TrainingEvaluationNo))
                        {
                            employeeTrainingEvaluationObj.ErrorStatus = true;
                            employeeTrainingEvaluationObj.ErrorMessage = "Attach training report before submitting Training Evaluation.";
                            return View(employeeTrainingEvaluationObj);
                        }

                        //modify Employee training need application
                        employeeTrainingEvaluationObj.Comments = employeeTrainingEvaluationObj.Comments != null ? employeeTrainingEvaluationObj.Comments : "";

                        //Modify Training Evaluation
                        EmployeeTrainingEvaluationModified = dynamicsNAVSOAPServices.employeeTrainingManagementWS.ModifyTrainingEvaluation(employeeTrainingEvaluationObj.TrainingEvaluationNo, employeeTrainingEvaluationObj.ApplicationNo, employeeTrainingEvaluationObj.Comments,
                                                                                                                                          employeeTrainingEvaluationObj.TrainingObjective, employeeTrainingEvaluationObj.ParticipationEncouraged, employeeTrainingEvaluationObj.TopicsCovered, employeeTrainingEvaluationObj.ContentOrganised,
                                                                                                                                          employeeTrainingEvaluationObj.MaterialDistributed, employeeTrainingEvaluationObj.TrainingExperience, employeeTrainingEvaluationObj.TrainerWellPrepared, employeeTrainingEvaluationObj.ObjectiveMet,
                                                                                                                                          employeeTrainingEvaluationObj.Rate, employeeTrainingEvaluationObj.TrainerWellPrepared);
                        //send for approval
                        approvalWorkflowExist = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingEvaluationApprovalWorkflowEnabled(employeeTrainingEvaluationObj.TrainingEvaluationNo);
                        if (!approvalWorkflowExist)
                        {
                            employeeTrainingEvaluationObj.ErrorStatus = true;
                            employeeTrainingEvaluationObj.ErrorMessage = "An error was experienced while trying to send an approval request for employee evaluation no." + employeeTrainingEvaluationObj.TrainingEvaluationNo + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
                            return View(employeeTrainingEvaluationObj);
                        }
                        if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.SendTrainingEvaluationApprovalRequest(employeeTrainingEvaluationObj.TrainingEvaluationNo))
                        {

                            responseHeader = "Success";
                            responseMessage = "Employee evaluation no." + employeeTrainingEvaluationObj.TrainingEvaluationNo + " was successfully sent for approval. Check with the " + companyName + " human resource department for approval status.";
                            detailedResponseMessage = "Employee evaluation no." + employeeTrainingEvaluationObj.TrainingEvaluationNo + " was successfully sent for approval. Check with the " + companyName + "  human resource department for approval status.";
                            button1ControllerName = "EmployeeTraining";
                            button1ActionName = "TrainingEvaluationsHistory";
                            button1Name = "Ok";
                            button1HasParameters = false;
                            button1Parameters = "";
                            return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                        }
                        else
                        {
                            employeeTrainingEvaluationObj.ErrorStatus = true;
                            employeeTrainingEvaluationObj.ErrorMessage = "Training Evaluation No." + employeeTrainingEvaluationObj.TrainingEvaluationNo + " failed to be sent for approval.";
                            return View(employeeTrainingEvaluationObj);
                        }
                    }
                    else
                    {
                        responseHeader = "Employee evaluation  NotFound";
                        responseMessage = "The employee evaluation no." + employeeTrainingEvaluationObj.TrainingEvaluationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                        detailedResponseMessage = "The employee evaluation no." + employeeTrainingEvaluationObj.TrainingEvaluationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "TrainingEvaluationsHistory";
                        button1Name = "Ok";
                        button1HasParameters = false;
                        button1Parameters = "";
                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                    }

                }
                catch (Exception ex)
                {
                    employeeTrainingEvaluationObj.ErrorStatus = true;
                    employeeTrainingEvaluationObj.ErrorMessage = ex.Message.ToString();
                    return View(employeeTrainingEvaluationObj);


                }

            }

            return View(employeeTrainingEvaluationObj);
        }

        #endregion New Training evaluation Application*/

        /*#region Edit Training Evaluation applications
        public ActionResult OnBeforeEditEvaluation(string EmpTrainingEvaluationNo)
        {

            try
            {
                if (EmpTrainingEvaluationNo.Equals(""))
                {
                    return RedirectToAction("TrainingEvaluationsHistory", "EmployeeTraining");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingEvaluationExist(EmpTrainingEvaluationNo, AccountController.GetEmployeeNo()))
                {
                    string TrainingEvaluationStatus = dynamicsNAVSOAPServices.employeeTrainingManagementWS.GetTrainingEvaluationStatus(EmpTrainingEvaluationNo);
                    //if training application is open
                    if (TrainingEvaluationStatus.Equals("Open"))
                    {
                        return RedirectToAction("EditTrainingEvaluationApplication", "EmployeeTraining", new { EmpTrainingEvaluationNo = EmpTrainingEvaluationNo });
                    }

                    //if training application is pending approval
                    if (TrainingEvaluationStatus.Equals("Pending Approval"))
                    {
                        responseHeader = "Employee Training evaluation Request Pending Approval";
                        responseMessage = "The Employee training evaluation no." + EmpTrainingEvaluationNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
                        detailedResponseMessage = "The Employee training." + EmpTrainingEvaluationNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "EditTrainingEvaluationApplication";
                        button1HasParameters = true;
                        button1Parameters = "?EmpTrainingEvaluationNo=" + EmpTrainingEvaluationNo;
                        button1Name = "Yes";

                        button2ControllerName = "EmployeeTraining";
                        button2ActionName = "TrainingEvaluationsHistory";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }

                    //if Training evaluation is released
                    if (TrainingEvaluationStatus.Equals("Released"))
                    {
                        responseHeader = "Training evaluation  Approved";
                        responseMessage = "The Employee training evaluation no." + EmpTrainingEvaluationNo + " is already approved. Editing not allowed.";
                        detailedResponseMessage = "The Employee training evaluation." + EmpTrainingEvaluationNo + " is already approved. Editing not allowed.";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "TrainingEvaluationsHistory";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    //if training application is rejected
                    if (TrainingEvaluationStatus.Equals("Rejected"))
                    {
                        responseHeader = "Training Evaluation Rejected";
                        responseMessage = "The employee evaluation no." + EmpTrainingEvaluationNo + " was rejected. Editing will reopen the document. Do you want to continue?";
                        detailedResponseMessage = "The employee evaluation no ." + EmpTrainingEvaluationNo + " was rejected. Editing will reopen the document. Do you want to continue?";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "EditTrainingEvaluationApplication";
                        button1HasParameters = true;
                        button1Parameters = "?EmpTrainingEvaluationNo=" + EmpTrainingEvaluationNo;
                        button1Name = "Yes";

                        button2ControllerName = "EmployeeTraining";
                        button2ActionName = "TrainingEvaluationsHistory";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    //if application training is posted/reversed
                    if (TrainingEvaluationStatus.Equals("Posted") || TrainingEvaluationStatus.Equals("Reversed"))
                    {
                        responseHeader = " Training Evaluation Posted";
                        responseMessage = "The Employee training application evaluation no." + EmpTrainingEvaluationNo + " is already posted. Editing not allowed.";
                        detailedResponseMessage = "The Employee evaluation." + EmpTrainingEvaluationNo + " is already posted. Editing not allowed.";

                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "TrainingEvaluationsHistory";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    return RedirectToAction("TrainingEvaluationsHistory", "EmployeeTraining");
                }
                else
                {
                    responseHeader = "Employee Application evaluation NotFound";
                    responseMessage = "The  employee training evaluation no." + EmpTrainingEvaluationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "The  employee training evaluation." + EmpTrainingEvaluationNo + " was not found under employee no." + AccountController.GetEmployeeNo();

                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "TrainingEvaluationsHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";

                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }

        [Authorize]
        public ActionResult EditTrainingEvaluationApplication(string EmpTrainingEvaluationNo)
        {

            try
            {

                if (EmpTrainingEvaluationNo.Equals(""))
                {
                    return RedirectToAction("TrainingEvaluationsHistory", "EmployeeTraining");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingEvaluationExist(EmpTrainingEvaluationNo, AccountController.GetEmployeeNo()))
                {
                    string TrainingEvaluationStatus = dynamicsNAVSOAPServices.employeeTrainingManagementWS.GetTrainingEvaluationStatus(EmpTrainingEvaluationNo);

                    //if application training need is pending approval, cancel approval request
                    if (TrainingEvaluationStatus.Equals("Pending Approval"))
                    {
                        dynamicsNAVSOAPServices.employeeTrainingManagementWS.CancelTrainingEvaluationApprovalRequest(EmpTrainingEvaluationNo);
                    }
                    //if training evaluation is released, reopen document
                    if (TrainingEvaluationStatus.Equals("Released"))
                    {
                        dynamicsNAVSOAPServices.employeeTrainingManagementWS.CancelTrainingEvaluationApprovalRequest(EmpTrainingEvaluationNo);
                    }

                    //if training evaluation is rejected, reopen document
                    if (TrainingEvaluationStatus.Equals("Rejected"))
                    {
                        dynamicsNAVSOAPServices.employeeTrainingManagementWS.CancelTrainingEvaluationApprovalRequest(EmpTrainingEvaluationNo);
                    }

                    EmployeeTrainingEvaluationModel employeeTrainingEvaluationObj = new EmployeeTrainingEvaluationModel();

                    var EmployeeTrainingEvaluationApplications = from EmployeeApplicationEvaluationsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRTrainingEvaluations
                                                                where EmployeeApplicationEvaluationsQuery.Training_Evaluation_No.Equals(EmpTrainingEvaluationNo)
                                                                select EmployeeApplicationEvaluationsQuery;


                    foreach (HRTrainingEvaluations HRTrainingEvaluation in EmployeeTrainingEvaluationApplications)
                    {

                        employeeTrainingEvaluationObj.TrainingEvaluationNo = HRTrainingEvaluation.Training_Evaluation_No;
                        employeeTrainingEvaluationObj.ApplicationNo = HRTrainingEvaluation.Training_Application_no;
                        employeeTrainingEvaluationObj.CalenderYear = HRTrainingEvaluation.Calendar_Year;
                        employeeTrainingEvaluationObj.DevelopmentNeed = HRTrainingEvaluation.Developement_Need;
                        employeeTrainingEvaluationObj.TrainingProvider = HRTrainingEvaluation.Training_Provider;
                        employeeTrainingEvaluationObj.TrainingLocation = HRTrainingEvaluation.Venue_Location;
                        employeeTrainingEvaluationObj.Objectives = HRTrainingEvaluation.Objectives;
                        employeeTrainingEvaluationObj.Comments = HRTrainingEvaluation.General_Comments_from_Training;
                        employeeTrainingEvaluationObj.TrainingEndDate = HRTrainingEvaluation.Training_Start_Date.Value.ToShortDateString();
                        employeeTrainingEvaluationObj.TrainingEndDate = HRTrainingEvaluation.Training_End_Date.Value.ToShortDateString();
                        employeeTrainingEvaluationObj.Status = HRTrainingEvaluation.Status;

                    }

                    LoadYears();
                    employeeTrainingEvaluationObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                    LoadTrainingApplications();
                    employeeTrainingEvaluationObj.Applications = new SelectList(TrainingApplications, "Application_No", "Description");

                    LoadForObjectivesofTraining();
                    LoadForParticipationAndInterractions();
                    LoadForCoveredTopics();
                    LoadForOrganizedContents();
                    LoadForTrainingMaterials();
                    LoadForTrainingExperience();
                    LoadForTrainerWellPrepared();
                    LoadForTrainingObjectivesMet();
                    LoadForRatings();
                    LoadForTrainerPreparedness();

                    employeeTrainingEvaluationObj.TrainingObjectives = new SelectList(As, "Text", "Value");
                    employeeTrainingEvaluationObj.Participations = new SelectList(Bs, "Text", "Value");
                    employeeTrainingEvaluationObj.Topics = new SelectList(Cs, "Text", "Value");
                    employeeTrainingEvaluationObj.Contents = new SelectList(Ds, "Text", "Value");
                    employeeTrainingEvaluationObj.Materials = new SelectList(Es, "Text", "Value");
                    employeeTrainingEvaluationObj.TrainingExperiences = new SelectList(Fs, "Text", "Value");
                    employeeTrainingEvaluationObj.Trainers = new SelectList(Gs, "Text", "Value");
                    employeeTrainingEvaluationObj.ObjectivesMet = new SelectList(Hs, "Text", "Value");
                    employeeTrainingEvaluationObj.Ratings = new SelectList(Is, "Text", "Value");
                    employeeTrainingEvaluationObj.TrainerPreparedness = new SelectList(Js, "Text", "Value");


                    return View(employeeTrainingEvaluationObj);
                }
                else
                {
                    responseHeader = "Employee evaluation Application NotFound";
                    responseMessage = "The Employee evaluation no." + EmpTrainingEvaluationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = " Employee evaluation no." + EmpTrainingEvaluationNo + " was not found under employee no." + AccountController.GetEmployeeNo();

                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "TrainingEvaluationsHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";

                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }

            }
            catch (Exception ex)
            {

                return errorResponse.ApplicationExceptionError(ex);
            }


        }

        [Authorize]
        [HttpPost]
        public ActionResult EditTrainingEvaluationApplication(EmployeeTrainingEvaluationModel employeeTrainingEvaluationObj)
        {

            bool EmployeeTrainingEvaluationModified = false;
            bool approvalWorkflowExist = false;

            try
            {
                LoadYears();
                LoadTrainingApplications();
                

                LoadForObjectivesofTraining();
                LoadForParticipationAndInterractions();
                LoadForCoveredTopics();
                LoadForOrganizedContents();
                LoadForTrainingMaterials();
                LoadForTrainingExperience();
                LoadForTrainerWellPrepared();
                LoadForTrainingObjectivesMet();
                LoadForRatings();
                LoadForTrainerPreparedness();


                if (ModelState.IsValid)
                {
                    employeeTrainingEvaluationObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");
                    employeeTrainingEvaluationObj.Applications = new SelectList(TrainingApplications, "Application_No", "Description");

                    employeeTrainingEvaluationObj.TrainingObjectives = new SelectList(As, "Text", "Value");
                    employeeTrainingEvaluationObj.Participations = new SelectList(Bs, "Text", "Value");
                    employeeTrainingEvaluationObj.Topics = new SelectList(Cs, "Text", "Value");
                    employeeTrainingEvaluationObj.Contents = new SelectList(Ds, "Text", "Value");
                    employeeTrainingEvaluationObj.Materials = new SelectList(Es, "Text", "Value");
                    employeeTrainingEvaluationObj.TrainingExperiences = new SelectList(Fs, "Text", "Value");
                    employeeTrainingEvaluationObj.Trainers = new SelectList(Gs, "Text", "Value");
                    employeeTrainingEvaluationObj.ObjectivesMet = new SelectList(Hs, "Text", "Value");
                    employeeTrainingEvaluationObj.Ratings = new SelectList(Is, "Text", "Value");
                    employeeTrainingEvaluationObj.TrainerPreparedness = new SelectList(Js, "Text", "Value");

                    if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingEvaluationExist(employeeTrainingEvaluationObj.TrainingEvaluationNo, employeeNo))
                    {
                       employeeTrainingEvaluationObj.Comments = employeeTrainingEvaluationObj.Comments != null? employeeTrainingEvaluationObj.Comments: "";

                        if (!dynamicsNAVSOAPServices.documentMgmt.CheckDocumentAttached(employeeTrainingEvaluationObj.TrainingEvaluationNo))
                        {
                            employeeTrainingEvaluationObj.ErrorStatus = true;
                            employeeTrainingEvaluationObj.ErrorMessage = "Attach training report before submitting Training Evaluation.";
                            return View(employeeTrainingEvaluationObj);
                        }

                        //Modify Training Evaluation
                        EmployeeTrainingEvaluationModified = dynamicsNAVSOAPServices.employeeTrainingManagementWS.ModifyTrainingEvaluation(employeeTrainingEvaluationObj.TrainingEvaluationNo, employeeTrainingEvaluationObj.ApplicationNo, employeeTrainingEvaluationObj.Comments,
                                                                                                                                           employeeTrainingEvaluationObj.TrainingObjective, employeeTrainingEvaluationObj.ParticipationEncouraged, employeeTrainingEvaluationObj.TopicsCovered, employeeTrainingEvaluationObj.ContentOrganised,
                                                                                                                                           employeeTrainingEvaluationObj.MaterialDistributed, employeeTrainingEvaluationObj.TrainingExperience, employeeTrainingEvaluationObj.TrainerWellPrepared, employeeTrainingEvaluationObj.ObjectiveMet,
                                                                                                                                           employeeTrainingEvaluationObj.Rate, employeeTrainingEvaluationObj.TrainerWellPrepared);

                        //send for approval
                        approvalWorkflowExist = dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingEvaluationApprovalWorkflowEnabled(employeeTrainingEvaluationObj.TrainingEvaluationNo);
                        if (!approvalWorkflowExist)
                        {
                            employeeTrainingEvaluationObj.ErrorStatus = true;
                            employeeTrainingEvaluationObj.ErrorMessage = "An error was experienced while trying to send an approval request for employee training evaluation no." + employeeTrainingEvaluationObj.TrainingEvaluationNo + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
                            return View(employeeTrainingEvaluationObj);
                        }
                        if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.SendTrainingEvaluationApprovalRequest(employeeTrainingEvaluationObj.TrainingEvaluationNo))
                        {

                            responseHeader = "Success";
                            responseMessage = "Employee  training evaluation no." + employeeTrainingEvaluationObj.TrainingEvaluationNo + " was successfully sent for approval. Check with the " + companyName + " human resource department for approval status.";
                            detailedResponseMessage = "Employee training evaluation no." + employeeTrainingEvaluationObj.TrainingEvaluationNo + " was successfully sent for approval. Check with the " + companyName + "  human resource department for approval status.";
                            button1ControllerName = "EmployeeTraining";
                            button1ActionName = "TrainingEvaluationsHistory";
                            button1Name = "Ok";
                            button1HasParameters = false;
                            button1Parameters = "";
                            return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                        }
                        else
                        {
                            employeeTrainingEvaluationObj.ErrorStatus = true;
                            employeeTrainingEvaluationObj.ErrorMessage = "Training Evaluation No." + employeeTrainingEvaluationObj.TrainingEvaluationNo + " failed to be sent for approval.";
                            return View(employeeTrainingEvaluationObj);
                        }
                    }
                    else
                    {
                        responseHeader = "Employee evaluation Not Found";
                        responseMessage = "The employee evaluatievaluation no ." + employeeTrainingEvaluationObj.TrainingEvaluationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                        button1ControllerName = "EmployeeTraining";
                        button1ActionName = "TrainingEvaluationsHistory";
                        button1Name = "Ok";
                        button1HasParameters = false;
                        button1Parameters = "";
                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }

                }
                else
                {
                    return View(employeeTrainingEvaluationObj);
                }

            }

            catch (Exception ex)
            {
                employeeTrainingEvaluationObj.ErrorStatus = true;
                employeeTrainingEvaluationObj.ErrorMessage = ex.Message.ToString();
                return View(employeeTrainingEvaluationObj);
            }
        }
        #endregion Edit Training Evaluation applications

        #region Training evaluations Approvals
        [Authorize]
        public ActionResult TrainingEvaluationApprovals(string EvaluationNo)
        {

            try
            {
                if (EvaluationNo.Equals(""))
                {

                    return RedirectToAction("RequestsToApprove", "Approval");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingEvaluationExist(EvaluationNo, AccountController.GetEmployeeNo()))
                {

                    EmployeeTrainingEvaluationModel employeeTrainingEvaluationObj = new EmployeeTrainingEvaluationModel();

                    var EmployeeTrainingEvaluationApplications = from EmployeeApplicationEvaluationsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRTrainingEvaluations
                                                           where EmployeeApplicationEvaluationsQuery.Training_Evaluation_No.Equals(EvaluationNo)
                                                           select EmployeeApplicationEvaluationsQuery;


                    foreach (HRTrainingEvaluations HRTrainingEvaluation in EmployeeTrainingEvaluationApplications)
                    {

                        employeeTrainingEvaluationObj.TrainingEvaluationNo = HRTrainingEvaluation.Training_Evaluation_No;
                        employeeTrainingEvaluationObj.ApplicationNo = HRTrainingEvaluation.Training_Application_no;
                        employeeTrainingEvaluationObj.CalenderYear = HRTrainingEvaluation.Calendar_Year;
                        employeeTrainingEvaluationObj.DevelopmentNeed = HRTrainingEvaluation.Developement_Need;
                        employeeTrainingEvaluationObj.TrainingProvider = HRTrainingEvaluation.Training_Provider;
                        employeeTrainingEvaluationObj.TrainingLocation = HRTrainingEvaluation.Venue_Location;
                        employeeTrainingEvaluationObj.Objectives = HRTrainingEvaluation.Objectives;
                        employeeTrainingEvaluationObj.Comments = HRTrainingEvaluation.General_Comments_from_Training;
                        employeeTrainingEvaluationObj.TrainingEndDate = HRTrainingEvaluation.Training_Start_Date.Value.ToShortDateString();
                        employeeTrainingEvaluationObj.TrainingEndDate = HRTrainingEvaluation.Training_End_Date.Value.ToShortDateString();
                        employeeTrainingEvaluationObj.Status = HRTrainingEvaluation.Status;


                    }

                    return View(EmployeeTrainingEvaluationApplications);
                }
                else
                {
                    responseHeader = "Training Evaluation NotFound";
                    responseMessage = "Training Evaluation No." + EvaluationNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "Training Evaluation No." + EvaluationNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                   
                    button1ControllerName = "Approval";
                    button1ActionName = "RequestsToApprove";
                    button1Name = "Ok";
                    button1HasParameters = false;
                    button1Parameters = "";
                   
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
     
        [Authorize]
        [HttpPost]
        public ActionResult TrainingEvaluationApprovals(EmployeeTrainingEvaluationModel EmployeeTrainingEvaluationObj, string Command)
        {

            try
            {
                if (EmployeeTrainingEvaluationObj.EmployeeNo.Equals(""))
                {
                    return RedirectToAction("RequestsToApprove", "Approval");
                }
                if (Command == "Approve")
                {
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveTrainingApplication(employeeNo, EmployeeTrainingEvaluationObj.ApplicationNo))
                    {
                        responseHeader = "Success";
                        responseMessage = "The Evaluation No." + EmployeeTrainingEvaluationObj.EmployeeNo + " was successfully approved.";
                        detailedResponseMessage = "The Evaluation No." + EmployeeTrainingEvaluationObj.EmployeeNo + " was successfully approved.";

                        button1ControllerName = "Approval";
                        button1ActionName = "RequestsToApprove";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        EmployeeTrainingEvaluationObj.ErrorStatus = true;
                        EmployeeTrainingEvaluationObj.ErrorMessage = "Unable to process the training evaluation approve action. " + ServiceConnection.contactICTDepartment + "";
                        return View(EmployeeTrainingEvaluationObj);
                    }
                }
                else if (Command == "Reject")
                {
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectTrainingApplication(employeeNo, EmployeeTrainingEvaluationObj.ApplicationNo))
                    {
                        responseHeader = "Success";
                        responseMessage = "The Evaluation No." + EmployeeTrainingEvaluationObj.EmployeeNo + " was successfully rejected.";
                        detailedResponseMessage = "The Evaluation No." + EmployeeTrainingEvaluationObj.EmployeeNo + " was successfully rejected.";

                        button1ControllerName = "Approval";
                        button1ActionName = "RequestsToApprove";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        EmployeeTrainingEvaluationObj.ErrorStatus = true;
                        EmployeeTrainingEvaluationObj.ErrorMessage = "Unable to process the training evaluation reject action. " + ServiceConnection.contactICTDepartment + "";
                        return View(EmployeeTrainingEvaluationObj);
                    }
                }
                else
                {
                    EmployeeTrainingEvaluationObj.ErrorStatus = true;
                    EmployeeTrainingEvaluationObj.ErrorMessage = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                    return View(EmployeeTrainingEvaluationObj);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            //return View();
        }

        #endregion  Training evaluations Approvals

        #region View Training Evaluations Applications
        public ActionResult ViewTrainingEvaluationRequests(string EmpTrainingEvaluationNo)
        {

            try
            {
                if (EmpTrainingEvaluationNo.Equals(""))
                {

                    return RedirectToAction("TrainingEvaluationsHistory", "EmployeeTraining");
                }
                if (dynamicsNAVSOAPServices.employeeTrainingManagementWS.CheckTrainingEvaluationExist(EmpTrainingEvaluationNo, AccountController.GetEmployeeNo()))
                {

                    EmployeeTrainingEvaluationModel employeeTrainingEvaluationObj= new EmployeeTrainingEvaluationModel();

                    var EmployeeTrainingEvaluationApplications = from EmployeeApplicationEvaluationsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRTrainingEvaluations
                                                                 where EmployeeApplicationEvaluationsQuery.Training_Evaluation_No.Equals(EmpTrainingEvaluationNo)
                                                                 select EmployeeApplicationEvaluationsQuery;


                    foreach (HRTrainingEvaluations HRTrainingEvaluation in EmployeeTrainingEvaluationApplications)
                    {

                        employeeTrainingEvaluationObj.TrainingEvaluationNo = HRTrainingEvaluation.Training_Evaluation_No;
                        employeeTrainingEvaluationObj.ApplicationNo = HRTrainingEvaluation.Training_Application_no;
                        employeeTrainingEvaluationObj.CalenderYear = HRTrainingEvaluation.Calendar_Year;
                        employeeTrainingEvaluationObj.DevelopmentNeed = HRTrainingEvaluation.Developement_Need;
                        employeeTrainingEvaluationObj.TrainingProvider = HRTrainingEvaluation.Training_Provider;
                        employeeTrainingEvaluationObj.TrainingLocation = HRTrainingEvaluation.Venue_Location;
                        employeeTrainingEvaluationObj.Objectives = HRTrainingEvaluation.Objectives;
                        employeeTrainingEvaluationObj.Comments = HRTrainingEvaluation.General_Comments_from_Training;
                        employeeTrainingEvaluationObj.TrainingStartDate = HRTrainingEvaluation.Training_Start_Date.Value.ToShortDateString();
                        employeeTrainingEvaluationObj.TrainingEndDate = HRTrainingEvaluation.Training_End_Date.Value.ToShortDateString();
                        employeeTrainingEvaluationObj.Status = HRTrainingEvaluation.Status;

                    }

                    LoadYears();
                    employeeTrainingEvaluationObj.YearCodes = new SelectList(CalenderPeriods, "Code", "Description");

                    LoadTrainingApplications();
                    employeeTrainingEvaluationObj.Applications = new SelectList(TrainingApplications, "Application_No", "Description");

                    LoadForObjectivesofTraining();
                    LoadForParticipationAndInterractions();
                    LoadForCoveredTopics();
                    LoadForOrganizedContents();
                    LoadForTrainingMaterials();
                    LoadForTrainingExperience();
                    LoadForTrainerWellPrepared();
                    LoadForTrainingObjectivesMet();
                    LoadForRatings();
                    LoadForTrainerPreparedness();

                    employeeTrainingEvaluationObj.TrainingObjectives = new SelectList(As, "Text", "Value");
                    employeeTrainingEvaluationObj.Participations = new SelectList(Bs, "Text", "Value");
                    employeeTrainingEvaluationObj.Topics = new SelectList(Cs, "Text", "Value");
                    employeeTrainingEvaluationObj.Contents = new SelectList(Ds, "Text", "Value");
                    employeeTrainingEvaluationObj.Materials = new SelectList(Es, "Text", "Value");
                    employeeTrainingEvaluationObj.TrainingExperiences = new SelectList(Fs, "Text", "Value");
                    employeeTrainingEvaluationObj.Trainers = new SelectList(Gs, "Text", "Value");
                    employeeTrainingEvaluationObj.ObjectivesMet = new SelectList(Hs, "Text", "Value");
                    employeeTrainingEvaluationObj.Ratings = new SelectList(Is, "Text", "Value");
                    employeeTrainingEvaluationObj.TrainerPreparedness = new SelectList(Js, "Text", "Value");

                    return View(employeeTrainingEvaluationObj);
                }
                else
                {
                    responseHeader = "Employee evaluation NotFound";
                    responseMessage = "Employe evaluation no." + EmpTrainingEvaluationNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "Employee evaluation no." + EmpTrainingEvaluationNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    button1ControllerName = "EmployeeTraining";
                    button1ActionName = "TrainingEvaluationsHistory";
                    button1Name = "Ok";
                    button1HasParameters = false;
                    button1Parameters = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            //return View();
;
        }

        #endregion View Training Evaluations Applications

        #region Employee training Evaluations History
        [Authorize]
        public ActionResult TrainingEvaluationsHistory()
        {
            try
            {
                List<EmployeeTrainingEvaluationModel> employeeTrainingEvaluationList = new List<EmployeeTrainingEvaluationModel>();


                var EmployeeTrainingEvaluationApplications = from EmployeeApplicationEvaluationsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRTrainingEvaluations
                                                             where EmployeeApplicationEvaluationsQuery.Employee_No.Equals(employeeNo)
                                                             select EmployeeApplicationEvaluationsQuery;


                foreach (HRTrainingEvaluations HRTrainingEvaluation in EmployeeTrainingEvaluationApplications)
                {
                    EmployeeTrainingEvaluationModel employeeTrainingEvaluationObj = new EmployeeTrainingEvaluationModel();

                    employeeTrainingEvaluationObj.TrainingEvaluationNo = HRTrainingEvaluation.Training_Evaluation_No;
                    employeeTrainingEvaluationObj.ApplicationNo = HRTrainingEvaluation.Training_Application_no;
                    employeeTrainingEvaluationObj.CalenderYear = HRTrainingEvaluation.Calendar_Year;
                    employeeTrainingEvaluationObj.DevelopmentNeed = HRTrainingEvaluation.Developement_Need;
                    employeeTrainingEvaluationObj.TrainingProvider = HRTrainingEvaluation.Training_Provider;
                    employeeTrainingEvaluationObj.TrainingLocation = HRTrainingEvaluation.Venue_Location;
                    employeeTrainingEvaluationObj.Objectives = HRTrainingEvaluation.Objectives;
                    employeeTrainingEvaluationObj.Comments = HRTrainingEvaluation.General_Comments_from_Training;
                    employeeTrainingEvaluationObj.TrainingStartDate = HRTrainingEvaluation.Training_Start_Date.Value.ToShortDateString();
                    employeeTrainingEvaluationObj.TrainingEndDate = HRTrainingEvaluation.Training_End_Date.Value.ToShortDateString();
                    employeeTrainingEvaluationObj.Status = HRTrainingEvaluation.Status;

                    employeeTrainingEvaluationList.Add(employeeTrainingEvaluationObj);
                }
                return View(employeeTrainingEvaluationList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }

        }

        #endregion Employee training applications history*/

        //#region Document Management

        //[Authorize]
        //public JsonResult GetTrainingEvaluationApplicationDocuments(string DocumentNo)
        //{
        //    List<DocumentMgmtModel> applicationDocumentsList = new List<DocumentMgmtModel>();

        //    var TrainingEvaluationUploadedDocuments = from trainingNeedDocumentsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
        //                                        where trainingNeedDocumentsQuery.DocumentNo.Equals(DocumentNo)
        //                                        select trainingNeedDocumentsQuery;

        //    foreach (PortalDocuments trainingDocument in TrainingEvaluationUploadedDocuments)
        //    {
        //        DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();
        //        documentManagementoBJ.DocumentNo = trainingDocument.DocumentNo;
        //        documentManagementoBJ.DocumentCode = trainingDocument.Document_Code;
        //        documentManagementoBJ.DocumentDescription = trainingDocument.Document_Description;
        //        documentManagementoBJ.DocumentAttached = trainingDocument.Document_Attached ?? false;
        //        documentManagementoBJ.LocalURL = trainingDocument.Local_File_URL;
        //        documentManagementoBJ.SharePointURL = trainingDocument.SharePoint_URL;
        //        applicationDocumentsList.Add(documentManagementoBJ);
        //    }

        //    return Json(applicationDocumentsList, JsonRequestBehavior.AllowGet);
        //}

        //[Authorize]
        //public ActionResult GetTrainingEvaluationApplicationDocument(string DocumentNo, string DocumentCode)
        //{
        //    try
        //    {
        //        DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

        //        var trainingEvaluationDocuments = from trainingDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
        //                                    where trainingDocumentQuery.DocumentNo.Equals(DocumentNo) && trainingDocumentQuery.Document_Code.Equals(DocumentCode)
        //                                    select trainingDocumentQuery;

        //        foreach (PortalDocuments trainingDocument in trainingEvaluationDocuments)
        //        {
        //            documentManagementoBJ.DocumentNo = trainingDocument.DocumentNo;
        //            documentManagementoBJ.DocumentCode = trainingDocument.Document_Code;
        //            documentManagementoBJ.DocumentDescription = trainingDocument.Document_Description;
        //            documentManagementoBJ.DocumentAttached = trainingDocument.Document_Attached ?? false;
        //            documentManagementoBJ.LocalURL = trainingDocument.Local_File_URL;
        //            documentManagementoBJ.SharePointURL = trainingDocument.SharePoint_URL;
        //        }

        //        return Json(documentManagementoBJ, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return errorResponse.ApplicationExceptionError(ex);
        //    }
        //}

        //[Authorize]
        //[HttpPost]
        //public JsonResult UploadTrainingEvaluationApplicationDocument(string DocumentNo, string DocumentCode, string DocumentDescription)
        //{
        //    try
        //    {
        //        if (Request.Files.Count > 0)
        //        {
        //            var root = "~/StaffData/" + employeeNo;
        //            bool folderpath = System.IO.Directory.Exists(HttpContext.Server.MapPath(root));

        //            if (!folderpath)
        //            {
        //                System.IO.Directory.CreateDirectory(HttpContext.Server.MapPath(root));
        //            }

        //            var file = Request.Files[0];
        //            string fileExt = System.IO.Path.GetExtension(file.FileName).ToLower();
        //            string fileName = DocumentNo + "_" + DocumentCode + fileExt;
        //            string path = System.IO.Path.Combine(HttpContext.Server.MapPath(root), fileName);

        //            if (System.IO.File.Exists(path))
        //            {
        //                System.IO.File.Delete(path);
        //            }

        //            file.SaveAs(path);

        //            if (System.IO.File.Exists(path))
        //            {
        //                dynamicsNAVSOAPServices.documentMgmt.ModifySystemFileURL(DocumentNo, DocumentCode, path);
        //                return Json(new { success = true, message = DocumentDescription + " uploaded successfully" }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = DocumentDescription + " was not uploaded. Try Again." }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        return Json(new { success = false, message = "Please select a pdf file!" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[Authorize]
        //[ChildActionOnly]
        //public ActionResult _EmployeeTrainingEvaluationDocumentLine(string DocumentNo)
        //{

        //    DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

        //    return PartialView(documentManagementoBJ);
        //}

        //[Authorize]
        //[ChildActionOnly]
        //public ActionResult _ViewEmployeeTrainingEvaluationDocumentLine(string DocumentNo)
        //{


        //    DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

        //    return PartialView(documentManagementoBJ);
        //}

        //#endregion Document Management

        #region Helper Functions

        /*[Authorize]
        public JsonResult GetAttendedTrainingApplicationDetails(string ApplicationNo)
        {
            EmployeeTrainingEvaluationModel EmployeeTrainingEvaluationObj = new EmployeeTrainingEvaluationModel();

            var TrainingApplications = from TrainingApplicationQuery in dynamicsNAVODataServices.dynamicsNAVOData.ApprovedTrainingApplications
                                      where TrainingApplicationQuery.Application_No.Equals(ApplicationNo)
                                      select TrainingApplicationQuery;

            foreach (ApprovedTrainingApplications TrainingApplication in TrainingApplications)
            {
                EmployeeTrainingEvaluationObj.ApplicationNo = TrainingApplication.Application_No;
                EmployeeTrainingEvaluationObj.CalenderYear = TrainingApplication.Calendar_Year;
                EmployeeTrainingEvaluationObj.DevelopmentNeed = TrainingApplication.Development_Need;
                EmployeeTrainingEvaluationObj.TrainingProvider = TrainingApplication.Provider_Name;
                EmployeeTrainingEvaluationObj.TrainingLocation = TrainingApplication.Location;
                EmployeeTrainingEvaluationObj.Objectives = TrainingApplication.Purpose_of_Training;
                EmployeeTrainingEvaluationObj.TrainingProvider = TrainingApplication.Provider_Name;
                EmployeeTrainingEvaluationObj.TrainingStartDate = TrainingApplication.From_Date.Value.ToString("dd/MM/yyyy");
                EmployeeTrainingEvaluationObj.TrainingEndDate = TrainingApplication.To_Date.Value.ToString("dd/MM/yyyy");

            }

            return Json(EmployeeTrainingEvaluationObj,JsonRequestBehavior.AllowGet);
        }*/
        private void LoadYears()
        {
            CalenderPeriods = from HRCalendarPeriodQuery in dynamicsNAVODataServices.BCOData.Calendar_Card.Execute()
                select HRCalendarPeriodQuery;
        }

        private void LoadApprovedTrainingNeeds()
        {
            /*ApprovedTrainingNeeds = from employeeTrainingNeedQuery in dynamicsNAVODataServices.dynamicsNAVOData.ApprovedTrainingNeeds
                                    where employeeTrainingNeedQuery.Employee_No.Equals(employeeNo)
                                    select employeeTrainingNeedQuery;*/
        }

        private void LoadTrainingApplications()
        {
            /*TrainingApplications = from employeeTrainingApplicationsQuery in dynamicsNAVODataServices.dynamicsNAVOData.ApprovedTrainingApplications
                                   where employeeTrainingApplicationsQuery.No.Equals(employeeNo)
                                   select employeeTrainingApplicationsQuery;*/
        }

        private void LoadTrainingTypes()
        {
            List<SelectListItem> TrainingNeedTypes = new List<SelectListItem>
            {
                new SelectListItem {Text = "Competency Development", Value = "Competency Development"},
                new SelectListItem {Text = "Continuous Professional", Value = "Continuous Professional"},
                new SelectListItem {Text = "Development", Value = "Development"},
                new SelectListItem {Text = "Self-Development", Value = "Self-Development"},
                new SelectListItem {Text = "PC Compliance", Value = "PC Compliance"}
            };
            TrainingTypes = TrainingNeedTypes;
        }

        private void LoadInterventions()
        {
            List<SelectListItem> InterventionTypes = new List<SelectListItem>
            {
                new SelectListItem {Text = "Training", Value = "Training"},
                new SelectListItem {Text = "Coaching", Value = "Coaching"},
                new SelectListItem {Text = "Mentoring", Value = "Mentoring"},
                new SelectListItem {Text = "Other", Value = "Other"}
            };

            Interventions = InterventionTypes;
        }

        private void LoadCalenderPeriod()
        {
            List<SelectListItem> CalenderPeriod = new List<SelectListItem>
            {
                new SelectListItem {Text = "Calender Period", Value = "Calender Period"}
            };
            ProposedPeriod = CalenderPeriod;
        }

        private void LoadForTrainerPreparedness()
        {
            List<SelectListItem> Jjs = new List<SelectListItem>
            {
                new SelectListItem {Text = "Agree", Value = "Agree"},
                new SelectListItem {Text = "Disagree", Value = "Disagree"},
                new SelectListItem {Text = "Neutral", Value = "Neutral"},
                new SelectListItem {Text = "Strongly Agree", Value = "Strongly Agree"},
                new SelectListItem {Text = "Strongly Disagree", Value = "Strongly Disagree"}
            };

            Js = Jjs;
        }

        private void LoadForRatings()
        {
            List<SelectListItem> Iis = new List<SelectListItem>
            {
                new SelectListItem {Text = "Good", Value = "Good"},
                new SelectListItem {Text = "Excellent", Value = "Excellent"},
                new SelectListItem {Text = "Average", Value = "Average"},
                new SelectListItem {Text = "Poor", Value = "Poor"},
                new SelectListItem {Text = "Very Poor", Value = "Very Poor"}
            };

            Is = Iis;
        }

        private void LoadForTrainingObjectivesMet()
        {
            List<SelectListItem> Hhs = new List<SelectListItem>
            {
                new SelectListItem {Text = "Agree", Value = "Agree"},
                new SelectListItem {Text = "Disagree", Value = "Disagree"},
                new SelectListItem {Text = "Neutral", Value = "Neutral"},
                new SelectListItem {Text = "Strongly Agree", Value = "Strongly Agree"},
                new SelectListItem {Text = "Strongly Disagree", Value = "Strongly Disagree"}
            };

            Hs = Hhs;
        }

        private void LoadForTrainerWellPrepared()
        {
            List<SelectListItem> Ggs = new List<SelectListItem>
            {
                new SelectListItem {Text = "Agree", Value = "Agree"},
                new SelectListItem {Text = "Disagree", Value = "Disagree"},
                new SelectListItem {Text = "Neutral", Value = "Neutral"},
                new SelectListItem {Text = "Strongly Agree", Value = "Strongly Agree"},
                new SelectListItem {Text = "Strongly Disagree", Value = "Strongly Disagree"}
            };

            Gs = Ggs;
        }

        private void LoadForTrainingExperience()
        {
            List<SelectListItem> Ffs = new List<SelectListItem>
            {
                new SelectListItem {Text = "Agree", Value = "Agree"},
                new SelectListItem {Text = "Disagree", Value = "Disagree"},
                new SelectListItem {Text = "Neutral", Value = "Neutral"},
                new SelectListItem {Text = "Strongly Agree", Value = "Strongly Agree"},
                new SelectListItem {Text = "Strongly Disagree", Value = "Strongly Disagree"}
            };

            Fs = Ffs;
        }

        private void LoadForTrainingMaterials()
        {
            List<SelectListItem> Ees = new List<SelectListItem>
            {
                new SelectListItem {Text = "Agree", Value = "Agree"},
                new SelectListItem {Text = "Disagree", Value = "Disagree"},
                new SelectListItem {Text = "Neutral", Value = "Neutral"},
                new SelectListItem {Text = "Strongly Agree", Value = "Strongly Agree"},
                new SelectListItem {Text = "Strongly Disagree", Value = "Strongly Disagree"}
            };

            Es = Ees;
        }

        private void LoadForOrganizedContents()
        {
            List<SelectListItem> Dds = new List<SelectListItem>
            {
                new SelectListItem {Text = "Agree", Value = "Agree"},
                new SelectListItem {Text = "Disagree", Value = "Disagree"},
                new SelectListItem {Text = "Neutral", Value = "Neutral"},
                new SelectListItem {Text = "Strongly Agree", Value = "Strongly Agree"},
                new SelectListItem {Text = "Strongly Disagree", Value = "Strongly Disagree"}
            };

            Ds = Dds;
        }

        private void LoadForCoveredTopics()
        {
            List<SelectListItem> Ccs = new List<SelectListItem>
            {
                new SelectListItem {Text = "Agree", Value = "Agree"},
                new SelectListItem {Text = "Disagree", Value = "Disagree"},
                new SelectListItem {Text = "Neutral", Value = "Neutral"},
                new SelectListItem {Text = "Strongly Agree", Value = "Strongly Agree"},
                new SelectListItem {Text = "Strongly Disagree", Value = "Strongly Disagree"}
            };

            Cs = Ccs;
        }

        private void LoadForParticipationAndInterractions()
        {
            List<SelectListItem> Bbs = new List<SelectListItem>
            {
                new SelectListItem {Text = "Agree", Value = "Agree"},
                new SelectListItem {Text = "Disagree", Value = "Disagree"},
                new SelectListItem {Text = "Neutral", Value = "Neutral"},
                new SelectListItem {Text = "Strongly Agree", Value = "Strongly Agree"},
                new SelectListItem {Text = "Strongly Disagree", Value = "Strongly Disagree"}
            };

            Bs = Bbs;
        }

        private void LoadForObjectivesofTraining()
        {
            List<SelectListItem> Aas = new List<SelectListItem>
            {
                new SelectListItem {Text = "Agree", Value = "Agree"},
                new SelectListItem {Text = "Disagree", Value = "Disagree"},
                new SelectListItem {Text = "Neutral", Value = "Neutral"},
                new SelectListItem {Text = "Strongly Agree", Value = "Strongly Agree"},
                new SelectListItem {Text = "Strongly Disagree", Value = "Strongly Disagree"}
            };

            As = Aas;
        }

        #endregion Helperfuctions

        public ActionResult TrainingNeedsHistory()
        {
            try
            {
                var nav = dynamicsNAVODataServices.BCOData;

                var EmployeeTrainingNeedApplications = from employeeApplicationNeedsQuery in nav.TrainingAssessment
                    where employeeApplicationNeedsQuery.Employee_No.Equals(employeeNo)
                    select employeeApplicationNeedsQuery;
                return View(EmployeeTrainingNeedApplications.ToList());
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        public ActionResult EmployeeApplicationsHistory()
        {
            return View();
        }

        public ActionResult TrainingEvaluationsHistory()
        {
            return View();
        }

        public ActionResult TrainingRequest()
        {
            try
            {
                var nav = dynamicsNAVODataServices.BCOData;

                var requests = from employeeApplicationNeedsQuery in nav.Training_Request
                    where employeeApplicationNeedsQuery.Employee_No.Equals(employeeNo)
                    select employeeApplicationNeedsQuery;
                return View(requests.ToList());
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        public ActionResult TrainingSurvey()
        {
            try
            {
                var nav = dynamicsNAVODataServices.BCOData;

                var requests = from employeeApplicationNeedsQuery in nav.Training_Survey_Card
                    where employeeApplicationNeedsQuery.Employee_No.Equals(employeeNo)
                    select employeeApplicationNeedsQuery;
                return View(requests.ToList());
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        public ActionResult NewTrainingNeedApplication()
        {
            var nav = dynamicsNAVODataServices.BCOData;
            var trainingAnalysis = new TrainingAnalysis
            {
                Training_No = "",
                Employee_No = employeeNo
                //DateTimeAdded = DateTime.Now
            };
            //var df= OdataRef.RecruitmentNeeds.CreateRecruitmentNeeds("");
            nav.AddToTrainingAnalysis(trainingAnalysis);
            nav.SaveChanges();
            return RedirectToAction("ViewTrainingEvaluationRequests", new {id = trainingAnalysis.Training_No});
        }

        public ActionResult ViewTrainingEvaluationRequests(string id)
        {
            ViewBag.id = id;
            var nav = dynamicsNAVODataServices.BCOData;
            var trainingAnalysis = nav.TrainingAnalysis.Where(c => c.Training_No == id).FirstOrDefault();
            var analysisDto = MapperHelper.Map<TrainingAnalysisDTO>(trainingAnalysis);
            analysisDto.YearCodes = nav.Calendar_Card.Select(c => new SelectListItem()
            {
                Value = c.Code,
                Text = c.Code
            });
            return View(analysisDto);
        }

        public ActionResult _knowledgeGap(string Id)
        {
            ViewBag.id = Id;
            var gapsIndividuals = dynamicsNAVODataServices.BCOData.KnowledgeGapsInd
                .Where(c => c.Training_Need_No == Id).ToList();
            return PartialView(gapsIndividuals);
        }

        [HttpPost]
        public ActionResult AddKnowledgegapindividual(string id, int? lineNo)
        {
            var nav = dynamicsNAVODataServices.BCOData;
            var gapsIndividual = nav.KnowledgeGapsInd.AsEnumerable()
                .Where(c => c.Line_No == lineNo).FirstOrDefault();
            if (gapsIndividual == null)
            {
                return PartialView("_knowledgeGapForm", new KnowledgeGapsInd()
                {
                    Training_Need_No = id
                });
            }

            ViewBag.Training_Need_No = id;
            gapsIndividual.Training_Need_No = id;
            return PartialView("_knowledgeGapForm", gapsIndividual);
        }

        public ActionResult DeleteKnowledgegapindividual(int Id)
        {
            try
            {
                var nav = dynamicsNAVODataServices.BCOData;
                var gapsIndividuals = nav.KnowledgeGapsInd
                    .Where(c => c.Line_No == Id).FirstOrDefault();
                if (gapsIndividuals == null)
                {
                    return Json(new {saved = false, messsage = "object not found"}, JsonRequestBehavior.AllowGet);
                }

                nav.DeleteObject(gapsIndividuals);
                nav.SaveChanges();
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false, messsage = e.InnerException?.Message ?? e.Message},
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveKnowledgegapindividual(
            KnowledgeGapsInd KnowledgeGapsIndividual)
        {
            try
            {
                var nav = dynamicsNAVODataServices.BCOData;

                var gapsIndividual = nav.KnowledgeGapsInd
                    .Where(c => c.Line_No == KnowledgeGapsIndividual.Line_No).FirstOrDefault();
                if (gapsIndividual != null)
                {
                    gapsIndividual.Description = KnowledgeGapsIndividual.Description;
                    nav.UpdateObject(gapsIndividual);
                    nav.SaveChanges();
                    return Json(new {success = true}, JsonRequestBehavior.AllowGet);
                }

                nav.AddToKnowledgeGapsInd(KnowledgeGapsIndividual);
                nav.SaveChanges();
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false, message = e.InnerException?.Message ?? e.Message},
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult KnowledgegapindividualModels(string Id)
        {
            var nav = dynamicsNAVODataServices.BCOData;
            var gapsIndividual = nav.KnowledgeGapsInd
                .Where(c => c.Training_Need_No == Id).ToList();
            return PartialView("_rows", gapsIndividual);
        }

        public ActionResult _knowledgeGapDepartment(string Id)
        {
            ViewBag.Id = Id;
            var gapsDepartmentals = dynamicsNAVODataServices.BCOData.KnowledgeGapsDepartmental
                .Where(c => c.Training_Need_No == Id).ToList();
            return PartialView(gapsDepartmentals);
        }

        [HttpPost]
        public ActionResult AddKnowledgegapDepartment(string id, int? lineNo)
        {
            var nav = dynamicsNAVODataServices.BCOData;
            var gapsDepartmental = nav.KnowledgeGapsDepartmental.AsEnumerable()
                .Where(c => c.Line_No == lineNo).FirstOrDefault();
            if (gapsDepartmental == null)
            {
                return PartialView(new KnowledgeGapsDepartmental()
                {
                    Training_Need_No = id
                });
            }

            ViewBag.Training_Need_No = id;
            gapsDepartmental.Training_Need_No = id;
            return PartialView(gapsDepartmental);
        }

        public ActionResult DeleteKnowledgegapDepartment(int Id)
        {
            try
            {
                var nav = dynamicsNAVODataServices.BCOData;
                var gapsDepartmental = nav.KnowledgeGapsDepartmental
                    .Where(c => c.Line_No == Id).FirstOrDefault();
                if (gapsDepartmental == null)
                {
                    return Json(new {saved = false, messsage = "object not found"}, JsonRequestBehavior.AllowGet);
                }

                nav.DeleteObject(gapsDepartmental);
                nav.SaveChanges();
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false, messsage = e.InnerException?.Message ?? e.Message},
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveKnowledgegapDepartment(
            KnowledgeGapsDepartmental KnowledgeGapsDepartmental)
        {
            try
            {
                var nav = dynamicsNAVODataServices.BCOData;

                var gapsDepartmental = nav.KnowledgeGapsDepartmental
                    .Where(c => c.Line_No == KnowledgeGapsDepartmental.Line_No).FirstOrDefault();
                if (gapsDepartmental != null)
                {
                    gapsDepartmental.Description = KnowledgeGapsDepartmental.Description;
                    nav.UpdateObject(gapsDepartmental);
                    nav.SaveChanges();
                    return Json(new {success = true}, JsonRequestBehavior.AllowGet);
                }

                nav.AddToKnowledgeGapsDepartmental(KnowledgeGapsDepartmental);
                nav.SaveChanges();
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false, message = e.InnerException?.Message ?? e.Message},
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult KnowledgegapDepartmentModels(string Id)
        {
            var nav = dynamicsNAVODataServices.BCOData;
            var gapsDepartmental = nav.KnowledgeGapsDepartmental
                .Where(c => c.Training_Need_No == Id).ToList();
            return PartialView("_knowledgeGapDepartmentrows", gapsDepartmental);
        }

        public ActionResult _TrainingProgrammes(string Id)
        {
            ViewBag.Id = Id;
            var gapsDepartmentals = dynamicsNAVODataServices.BCOData.TrainingAnalysisProgrammes
                .Where(c => c.Training_Need_No == Id).ToList();
            return PartialView(gapsDepartmentals);
        }

        public ActionResult DeleteTrainingProgrammes(int Id)
        {
            try
            {
                var nav = dynamicsNAVODataServices.BCOData;
                var programmes = nav.TrainingAnalysisProgrammes
                    .Where(c => c.Line_No == Id).FirstOrDefault();
                if (programmes == null)
                {
                    return Json(new {saved = false, messsage = "object not found"}, JsonRequestBehavior.AllowGet);
                }

                nav.DeleteObject(programmes);
                nav.SaveChanges();
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false, messsage = e.InnerException?.Message ?? e.Message},
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TrainingProgrammesModels(string Id)
        {
            var nav = dynamicsNAVODataServices.BCOData;
            var programmes = nav.TrainingAnalysisProgrammes
                .Where(c => c.Training_Need_No == Id).ToList();
            return PartialView("_TrainingProgrammesrows", programmes);
        }

        public ActionResult SaveTrainingProgrammes(TrainingAnalysisProgrammes TrainingProgrammes)
        {
            try
            {
                var nav = dynamicsNAVODataServices.BCOData;

                var programmes = nav.TrainingAnalysisProgrammes.Where(c => c.Line_No == TrainingProgrammes.Line_No).FirstOrDefault();
                if (programmes != null)
                {
                    programmes.Code = TrainingProgrammes.Code;
                    programmes.Programme = TrainingProgrammes.Programme;
                    programmes.Trainer = TrainingProgrammes.Trainer;
                    programmes.Venue = TrainingProgrammes.Venue;
                    nav.UpdateObject(programmes);
                    nav.SaveChanges();
                    return Json(new {success = true}, JsonRequestBehavior.AllowGet);
                }

                nav.AddToTrainingAnalysisProgrammes(TrainingProgrammes);
                nav.SaveChanges();
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false, message = e.InnerException?.Message ?? e.Message},
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddTrainingProgrammes(string id, int? lineNo)
        {
            var nav = dynamicsNAVODataServices.BCOData;
            var programmes = nav.TrainingAnalysisProgrammes.AsEnumerable()
                .Where(c => c.Line_No == lineNo).FirstOrDefault();
            if (programmes == null)
            {
                return PartialView(new TrainingAnalysisProgrammes()
                {
                    Training_Need_No = id
                });
            }

            ViewBag.Training_Need_No = id;
            programmes.Training_Need_No = id;
            return PartialView(programmes);
        }

        public ActionResult SubmittoHr(string id)
        {
            var nav = dynamicsNAVODataServices.BCOData;
            TempData["Success"] = "Submitted to HR successfully";
            return RedirectToAction("TrainingNeedsHistory");
        }

        public ActionResult NewTrainingSurvey()
        {
            var nav = dynamicsNAVODataServices.BCOData;
            var trainingSurveyCard = new Training_Survey_Card()
            {
                Training_Survey_No = "",
                Employee_No = employeeNo
                //DateTimeAdded = DateTime.Now
            };
            //var df= OdataRef.RecruitmentNeeds.CreateRecruitmentNeeds("");
            nav.AddToTraining_Survey_Card(trainingSurveyCard);
            nav.SaveChanges();
            return RedirectToAction("ViewTrainingSurvey", new {id = trainingSurveyCard.Training_Survey_No});
        }

        public ActionResult ViewTrainingSurvey(string id)
        {
            var nav = dynamicsNAVODataServices.BCOData;
            var trainingSurveyCard = nav.Training_Survey_Card.Where(c => c.Training_Survey_No == id).FirstOrDefault();
            var surveyDto = MapperHelper.Map<TrainingSurveyDto>(trainingSurveyCard);
            var responses = new Dictionary<string, string> {{"",""},{"Yes","Yes"},{"No","No"},{"Maybe","Maybe"} };
            surveyDto.Department_Code_Select = nav.Sub_Responsibility_Center.Execute().Select(c => new SelectListItem()
            {
                Value = c.Code, Text = c.Description,
                Selected = c.Code == trainingSurveyCard?.Department_Code
            });
            surveyDto.Training_No_Select = nav.Training_Request.Execute().Select(c => new SelectListItem()
            {
                Value = c.Request_No, Text = c.Description,
                Selected = c.Request_No == trainingSurveyCard?.Training_No
            });
            
            surveyDto.Training_Objective_Select = responses.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == trainingSurveyCard?.Training_Objective
            });
            surveyDto.Helpful_in_Productivity_Select = responses.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == trainingSurveyCard?.Helpful_in_Productivity
            });
            surveyDto.Did_Trainer_Show_Experience_Select = responses.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == trainingSurveyCard?.Did_Trainer_Show_Experience
            });
            surveyDto.Did_Trainer_Deliver_Promise_Select = responses.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == trainingSurveyCard?.Did_Trainer_Deliver_Promise
            });
            surveyDto.Trainer_answer_questions_Select = responses.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == trainingSurveyCard?.Trainer_answer_questions
            });
            surveyDto.Trainer_Recommendation_Select = new Dictionary<string,string>
            {
                {"",""},{"1","1"},{"2","2"},{"3","3"},{"4","4"},{"5","5"},{"6","6"},{"7","7"},{"8","8"},{"9","9"},{"10","10"},
            }.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == trainingSurveyCard?.Trainer_Recommendation
            });
            return View(surveyDto);
        }

        [HttpPost]
        public ActionResult ViewTrainingSurvey(TrainingSurveyDto TrainingSurveyDto)
        {
            var nav = dynamicsNAVODataServices.BCOData;
            var responses = new Dictionary<string, string> {{"",""},{"Yes","Yes"},{"No","No"},{"Maybe","Maybe"} };
            try
            {
                
                var trainingSurveyCard = nav.Training_Survey_Card.Where(c => c.Training_Survey_No == TrainingSurveyDto.Training_Survey_No).FirstOrDefault();
                if (trainingSurveyCard != null)
                {
                    trainingSurveyCard.Department_Code = TrainingSurveyDto.Department_Code;
                    trainingSurveyCard.Date = TrainingSurveyDto.Date;
                    trainingSurveyCard.Training_No = TrainingSurveyDto.Training_No;
                    trainingSurveyCard.Describe_the_Training = TrainingSurveyDto.Describe_the_Training;
                    trainingSurveyCard.Helpful_in_Productivity = TrainingSurveyDto.Helpful_in_Productivity;
                    trainingSurveyCard.Did_Trainer_Show_Experience = TrainingSurveyDto.Did_Trainer_Show_Experience;
                    trainingSurveyCard.Did_Trainer_Deliver_Promise = TrainingSurveyDto.Did_Trainer_Deliver_Promise;
                    trainingSurveyCard.Trainer_answer_questions = TrainingSurveyDto.Trainer_answer_questions;
                    trainingSurveyCard.Trainer_Recommendation = TrainingSurveyDto.Trainer_Recommendation;
                    trainingSurveyCard.Things_you_have_learnt = TrainingSurveyDto.Things_you_have_learnt;
                    trainingSurveyCard.Additional_comments = TrainingSurveyDto.Additional_comments;
                    nav.UpdateObject(trainingSurveyCard);
                }

                nav.SaveChanges();
                TempData["Success"] = $"Record Updated successfully";
                return RedirectToAction("TrainingSurvey");
            }
            catch (Exception e)
            {
                TrainingSurveyDto.Department_Code_Select = nav.Sub_Responsibility_Center.Execute().Select(c => new SelectListItem()
                {
                    Value = c.Code, Text = c.Description,
                    Selected = c.Code == TrainingSurveyDto?.Department_Code
                });
                TrainingSurveyDto.Training_No_Select = nav.Training_Request.Execute().Select(c => new SelectListItem()
                {
                    Value = c.Request_No, Text = c.Description,
                    Selected = c.Request_No == TrainingSurveyDto?.Training_No
                });
                TrainingSurveyDto.Department_Code_Select = nav.Sub_Responsibility_Center.Execute().Select(c => new SelectListItem()
            {
                Value = c.Code, Text = c.Description,
                Selected = c.Code == TrainingSurveyDto?.Department_Code
            });
            TrainingSurveyDto.Training_No_Select = nav.Training_Request.Execute().Select(c => new SelectListItem()
            {
                Value = c.Request_No, Text = c.Description,
                Selected = c.Request_No == TrainingSurveyDto?.Training_No
            });
            
            TrainingSurveyDto.Training_Objective_Select = responses.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == TrainingSurveyDto?.Training_Objective
            });
            TrainingSurveyDto.Helpful_in_Productivity_Select = responses.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == TrainingSurveyDto?.Helpful_in_Productivity
            });
            TrainingSurveyDto.Did_Trainer_Show_Experience_Select = responses.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == TrainingSurveyDto?.Did_Trainer_Show_Experience
            });
            TrainingSurveyDto.Did_Trainer_Deliver_Promise_Select = responses.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == TrainingSurveyDto?.Did_Trainer_Deliver_Promise
            });
            TrainingSurveyDto.Trainer_answer_questions_Select = responses.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == TrainingSurveyDto?.Trainer_answer_questions
            });
            TrainingSurveyDto.Trainer_Recommendation_Select = new Dictionary<string,string>
            {
                {"",""},{"1","1"},{"2","2"},{"3","3"},{"4","4"},{"5","5"},{"6","6"},{"7","7"},{"8","8"},{"9","9"},{"10","10"},
            }.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(), Text = c.Value,
                Selected = c.Value == TrainingSurveyDto?.Trainer_Recommendation
            });
                TempData["Error"] = $"Error :{e.InnerException?.Message??e.Message}";
                return View(TrainingSurveyDto);
            }
        }

        public ActionResult Printsurvey()
        {
            throw new NotImplementedException();
        }
    }

    public class HRTrainingNeeds
    {
    }
}