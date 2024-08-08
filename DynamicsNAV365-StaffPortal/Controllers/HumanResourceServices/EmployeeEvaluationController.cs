//using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
//using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
//using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
//using DynamicsNAV365_StaffPortal.Models.EmployeeAppraisal;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
//{
//	public class EmployeeEvaluationController : Controller
//	{
//		string companyName = ServiceConnection.CompanyName;
//		static string companyURL = "";

//		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
//		DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

//		SuccessResponseController successResponse = new SuccessResponseController();
//		InfoResponseController infoResponse = new InfoResponseController();
//		ErrorResponseController errorResponse = new ErrorResponseController();

//		IQueryable<GlobalAppraisalObjectives> globalAppraisalObjectives = null;
//		IQueryable<OrganizationalAppraisalLines> OrganizationAppraisalCodes = null;
//		IQueryable<DepartmentalAppraisalLines> DepartmentalAppraisalCodes = null;
//		IQueryable<HRCalendarPeriods> AppraisalPeriods = null;
//		IQueryable<ApprovedEmployeeAppraisal> approvedEmployeeAppraisals = null;
//		IQueryable<BaseUnitOfMeasures> baseUnitMeasures = null;
//		IEnumerable<SelectListItem> AppraisalEvaluationStages = null;

//		private string responseHeader = "";
//		private string responseMessage = "";
//		private string detailedResponseMessage = "";

//		private string button1ControllerName = "";
//		private string button1ActionName = "";
//		private bool button1HasParameters = false;
//		private string button1Parameters = "";
//		private string button1Name = "";

//		private string button2ControllerName = "";
//		private string button2ActionName = "";
//		private bool button2HasParameters = false;
//		private string button2Parameters = "";
//		private string button2Name = "";

//		private string employeeNo = "";

//		AccountController accountController = new AccountController();
//		public EmployeeEvaluationController()
//		{
//			employeeNo = AccountController.GetEmployeeNo();
//		}

//		#region New Appraisal Evaluation 
//		[Authorize]
//		public ActionResult NewAppraisalEvaluation()
//		{
//			bool appraisalEvaluationCreated = false;
//			string openAppraisalEvaluationNo = "";
//			string appraisalEvaluationNo = "";

//			try
//			{
//				//Check open appraisal evaluation exists
//				openAppraisalEvaluationNo = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckOpenEmployeeAppraisalEvaluationExists(employeeNo);

//				if (!openAppraisalEvaluationNo.Equals(""))
//				{
//					responseHeader = "Open Appraisal Evaluation Exists";
//					responseMessage = "An open appraisal evaluation no." + openAppraisalEvaluationNo + " exists under employee no. " + employeeNo + ", use this employee appraisal card before creating a new one.";
//					detailedResponseMessage = "An open employee appraisal no." + openAppraisalEvaluationNo + " exists under employee no. " + employeeNo + ", use this employee appraisal card before creating a new one.";

//					button1ControllerName = "EmployeeEvaluation";
//					button1ActionName = "AppraisalEvaluationHistory";
//					button1Name = "Ok";
//					button1Parameters = "";

//					button2ControllerName = "";
//					button2ActionName = "";
//					button2HasParameters = false;
//					button2Parameters = "";
//					button2Name = "";

//					return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//														  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//														  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//				}

//				AppraisalEvaluationHeaderModel employeeAppraisalEvaluationObj = new AppraisalEvaluationHeaderModel();

//				//Create Appraisal Evaluation
//				appraisalEvaluationCreated = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CreateEmployeeAppraisalEvaluationHeader(employeeNo);

//				//Get appraisal Evaluation No.
//				appraisalEvaluationNo = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckOpenEmployeeAppraisalExists(employeeNo);

//				employeeAppraisalEvaluationObj.No = appraisalEvaluationNo;
//				employeeAppraisalEvaluationObj.EmployeeNo = employeeNo;
//				employeeAppraisalEvaluationObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

//				LoadAppraisalEvaluationStages();
//				employeeAppraisalEvaluationObj.EvaluationStages = new SelectList(AppraisalEvaluationStages, "Text", "Value");

//				LoadApprovedAppraisals();
//				employeeAppraisalEvaluationObj.AppraisalNos = new SelectList(approvedEmployeeAppraisals, "No", "Description");

//				return View(employeeAppraisalEvaluationObj);
//			}

//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}

//		[Authorize]
//		[HttpPost]
//		public ActionResult NewAppraisalEvaluation(AppraisalEvaluationHeaderModel employeeAppraisalEvaluationObj)
//		{
//			string DocumentNo = "";

//			try
//			{
//				LoadAppraisalEvaluationStages();
//				employeeAppraisalEvaluationObj.EvaluationStages = new SelectList(AppraisalEvaluationStages, "Text", "Value");

//				LoadApprovedAppraisals();
//				employeeAppraisalEvaluationObj.AppraisalNos = new SelectList(approvedEmployeeAppraisals, "No", "Description");

//				//get open Document No.
//				DocumentNo = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckOpenEmployeeAppraisalEvaluationExists(employeeNo);

//				if (ModelState.IsValid)
//				{
//					if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalEvaluationExists(DocumentNo, employeeNo))
//					{
//						//Check employee appraisal lines
//						if (!dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckAppraisalEvaluationLinesExist(DocumentNo))
//						{
//							employeeAppraisalEvaluationObj.ErrorStatus = true;
//							employeeAppraisalEvaluationObj.ErrorMessage = "You are required to update the Actual Value in appraisal evaluation line before submitting.";

//							return View(employeeAppraisalEvaluationObj);
//						}

//						//Modify employee appraisal header
//						dynamicsNAVSOAPServices.employeeAppraisalManagementWS.ModifyEmployeeAppraisalEvaluationHeader(employeeAppraisalEvaluationObj.No, employeeAppraisalEvaluationObj.ApprovedAppraisal, employeeAppraisalEvaluationObj.EvaluationStage);

//						//Check approval workflow enabled
//						if (!dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalEvaluationApprovalWorkflowEnabled(employeeAppraisalEvaluationObj.No))
//						{
//							employeeAppraisalEvaluationObj.ErrorStatus = true;
//							employeeAppraisalEvaluationObj.ErrorMessage = "No approval workflow enabled for employee appraisal evaluation record";

//							return View(employeeAppraisalEvaluationObj);
//						}

//						//Send approval request
//						if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.SendEmployeeAppraisalEvaluationApprovalRequest(employeeAppraisalEvaluationObj.No))
//						{
//							responseHeader = "Success";
//							responseMessage = "The appraisal evaluation no." + employeeAppraisalEvaluationObj.No + " was successfully sent for approval. Once approved you will get a notification from your HOD";
//							detailedResponseMessage = "The appraisal evaluation no." + employeeAppraisalEvaluationObj.No + " was successfully sent for approval. Once approved you will get a notification from your HOD";

//							button1ControllerName = "EmployeeEvaluation";
//							button1ActionName = "AppraisalEvaluationHistory";
//							button1Name = "Ok";
//							button1Parameters = "";

//							button2ControllerName = "";
//							button2ActionName = "";
//							button2HasParameters = false;
//							button2Parameters = "";
//							button2Name = "";

//							return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
//																  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//																  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//						}
//						else
//						{
//							employeeAppraisalEvaluationObj.ErrorStatus = true;
//							employeeAppraisalEvaluationObj.ErrorMessage = "An error was experienced while trying to send an approval request for appraisal evaluation no." + employeeAppraisalEvaluationObj.No + ". Contact the " + companyName + " ICT division for assistance.";
//							return View(employeeAppraisalEvaluationObj);
//						}
//					}
//					else
//					{
//						responseHeader = "Appraisal Evaluation NotFound";
//						responseMessage = "The appraisal evaluation no." + employeeAppraisalEvaluationObj.No + " was not found for employee no." + AccountController.GetEmployeeNo();
//						detailedResponseMessage = "The appraisal evaluation no." + employeeAppraisalEvaluationObj.No + " was not found for employee no." + AccountController.GetEmployeeNo();

//						button1ControllerName = "EmployeeEvaluation";
//						button1ActionName = "AppraisalEvaluationHistory";
//						button1Name = "Ok";
//						button1Parameters = "";

//						button2ControllerName = "";
//						button2ActionName = "";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";


//						return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}
//				}

//				else
//				{
//					return View(employeeAppraisalEvaluationObj);
//				}

//			}
//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}
//		#endregion End Appraisal Evaluation

//		#region Edit Appraisal Evaluation
//		[Authorize]
//		public ActionResult OnBeforeEdit(string DocumentNo)
//		{
//			try
//			{
//				if (DocumentNo.Equals(""))
//				{
//					return RedirectToAction("AppraisalEvaluationHistory");
//				}
//				if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalEvaluationExists(DocumentNo, AccountController.GetEmployeeNo()))
//				{
//					string employeeAppraisalStatus = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.GetEmployeeAppraisalEvaluationStatus(DocumentNo);

//					//if employee appraisal is open
//					if (employeeAppraisalStatus.Equals("Open"))
//					{
//						return RedirectToAction("EditAppraisalEvaluation", "EmployeeEvaluation", new { DocumentNo = DocumentNo });
//					}

//					//if employee appraisal is pending approval
//					if (employeeAppraisalStatus.Equals("Pending Approval"))
//					{
//						responseHeader = "Appraisal Evaluation Pending Approval";
//						responseMessage = "The appraisal evaluation no." + DocumentNo + " is already submitted for approval. Editing not allowed.";
//						detailedResponseMessage = "The appraisal evaluation no." + DocumentNo + " is already submitted for approval. Editing not allowed.";

//						button1ControllerName = "EmployeeEvaluation";
//						button1ActionName = "AppraisalEvaluationHistory";
//						button1Name = "Ok";
//						button1Parameters = "";

//						button2ControllerName = "";
//						button2ActionName = "";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";

//						return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}

//					//if employee appraisal is released
//					if (employeeAppraisalStatus.Equals("Released"))
//					{
//						responseHeader = "Appraisal Evaluation Approved";
//						responseMessage = "The appraisal evaluation no." + DocumentNo + " is already approved. Editing not allowed.";
//						detailedResponseMessage = "The appraisal evaluation no." + DocumentNo + " is already approved. Editing not allowed.";

//						button1ControllerName = "EmployeeEvaluation";
//						button1ActionName = "AppraisalEvaluationHistory";
//						button1Name = "Ok";
//						button1Parameters = "";

//						button2ControllerName = "";
//						button2ActionName = "";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";


//						return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}

//					//if employee appraisal is rejected
//					if (employeeAppraisalStatus.Equals("Rejected"))
//					{
//						responseHeader = "Appraisal Evaluation Rejected";
//						responseMessage = "The appraisal evaluation no." + DocumentNo + " was rejected. Editing will reopen the document. Do you want to continue?";
//						detailedResponseMessage = "The appraisal evaluation no." + DocumentNo + " was rejected. Editing will reopen the document. Do you want to continue?";

//						button1ControllerName = "EmployeeEvaluation";
//						button1ActionName = "EditAppraisalEvaluation";
//						button1Name = "Yes";
//						button1HasParameters = true;
//						button1Parameters = "?DocumentNo=" + DocumentNo;

//						button2ActionName = "EmployeeEvaluation";
//						button2ActionName = "AppraisalEvaluationHistory";
//						button2Name = "No";
//						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}

//					//if employee appraisal is posted/reversed
//					if (employeeAppraisalStatus.Equals("Posted") || employeeAppraisalStatus.Equals("Reversed"))
//					{
//						responseHeader = "Employee Appraisal Posted";
//						responseMessage = "The appraisal Evaluation no." + DocumentNo + " is already posted. Editing not allowed.";
//						detailedResponseMessage = "The appraisal Evaluation no." + DocumentNo + " is already posted. Editing not allowed.";

//						button1ControllerName = "EmployeeEvaluation";
//						button1ActionName = "AppraisalEvaluationHistory";
//						button1Name = "Ok";
//						button1Parameters = "";

//						button2ControllerName = "";
//						button2ActionName = "";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";


//						return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}
//					return RedirectToAction("EmployeeAppraisalHistory", "EmployeeAppraisal");
//				}
//				else
//				{
//					responseHeader = "Appraisal Evaluation NotFound";
//					responseMessage = "The appraisal evaluation no." + DocumentNo + " was not found under employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The appraisal evaluation no." + DocumentNo + " was not found under employee no." + AccountController.GetEmployeeNo();
//					button1ControllerName = "EmployeeEvaluation";
//					button1ActionName = "AppraisalEvaluationHistory";
//					button1Name = "Ok";
//					button1Parameters = "";

//					button2ControllerName = "";
//					button2ActionName = "";
//					button2HasParameters = false;
//					button2Parameters = "";
//					button2Name = "";


//					return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//														  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//														  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//				}
//			}
//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}

//		[Authorize]
//		public ActionResult EditAppraisalEvaluation(string DocumentNo)
//		{
//			try
//			{
//				if (DocumentNo.Equals(""))
//				{
//					return RedirectToAction("AppraisalEvaluationHistory");
//				}

//				if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalEvaluationExists(DocumentNo, employeeNo))
//				{
//					string employeeAppraisalStatus = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.GetEmployeeAppraisalEvaluationStatus(DocumentNo);

//					//if employee appraisal  is pending approval, cancel approval request
//					if (employeeAppraisalStatus.Equals("Pending Approval"))
//					{
//						dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CancelEmployeeAppraisalEvaluationApprovalRequest(DocumentNo);
//					}

//					//if employee appraisal  is released, reopen and uncommit 
//					if (employeeAppraisalStatus.Equals("Released"))
//					{
//						dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CancelEmployeeAppraisalEvaluationApprovalRequest(DocumentNo);
//					}

//					//if employee appraisal is rejected, reopen document
//					if (employeeAppraisalStatus.Equals("Rejected"))
//					{
//						dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CancelEmployeeAppraisalEvaluationApprovalRequest(DocumentNo);
//					}

//					AppraisalEvaluationHeaderModel employeeAppraisalEvaluationObj = new AppraisalEvaluationHeaderModel();

//					var employeeAppraisalEvaluations = from employeeAppraisalEvaluationQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalEvaluations
//													   where employeeAppraisalEvaluationQuery.Employee_No.Equals(employeeNo)
//													   select employeeAppraisalEvaluationQuery;


//					foreach (AppraisalEvaluations employeeAppraisalEvaluation in employeeAppraisalEvaluations)
//					{
//						employeeAppraisalEvaluationObj.EmployeeNo = employeeAppraisalEvaluation.Employee_No;
//						employeeAppraisalEvaluationObj.EmployeeName = employeeAppraisalEvaluation.Employee_Name;
//						employeeAppraisalEvaluationObj.No = employeeAppraisalEvaluation.No;
//						employeeAppraisalEvaluationObj.ApprovedAppraisal = employeeAppraisalEvaluation.Appraisal_No;
//						employeeAppraisalEvaluationObj.EvaluationStage = employeeAppraisalEvaluation.Evaluation_Stage;
//						employeeAppraisalEvaluationObj.Status = employeeAppraisalEvaluation.Status;
//					}

//					LoadAppraisalEvaluationStages();
//					employeeAppraisalEvaluationObj.EvaluationStages = new SelectList(AppraisalEvaluationStages, "Text", "Value");

//					LoadApprovedAppraisals();
//					employeeAppraisalEvaluationObj.AppraisalNos = new SelectList(approvedEmployeeAppraisals, "No", "Description");

//					return View(employeeAppraisalEvaluationObj);
//				}

//				else
//				{
//					responseHeader = "Appraisal Evaluation NotFound";
//					responseMessage = "The appraisal evaluation no." + DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The appraisal evaluation no." + DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();

//					button1ControllerName = "EmployeeEvaluation";
//					button1ActionName = "AppraisalEvaluationHistory";
//					button1Name = "Ok";
//					button1Parameters = "";

//					button2ControllerName = "";
//					button2ActionName = "";
//					button2HasParameters = false;
//					button2Parameters = "";
//					button2Name = "";


//					return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//														  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//														  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//				}

//			}

//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}

//		[Authorize]
//		[HttpPost]
//		public ActionResult EditAppraisalEvaluation(AppraisalEvaluationHeaderModel employeeAppraisalEvaluationObj)
//		{
//			string DocumentNo = "";

//			try
//			{
//				LoadAppraisalEvaluationStages();
//				employeeAppraisalEvaluationObj.EvaluationStages = new SelectList(AppraisalEvaluationStages, "Text", "Value");

//				LoadApprovedAppraisals();
//				employeeAppraisalEvaluationObj.AppraisalNos = new SelectList(approvedEmployeeAppraisals, "No", "Description");

//				//get open Document No.
//				DocumentNo = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckOpenEmployeeAppraisalEvaluationExists(employeeNo);

//				if (ModelState.IsValid)
//				{
//					if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalEvaluationExists(DocumentNo, employeeNo))
//					{
//						//Check employee appraisal lines
//						if (!dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckAppraisalEvaluationLinesExist(DocumentNo))
//						{
//							employeeAppraisalEvaluationObj.ErrorStatus = true;
//							employeeAppraisalEvaluationObj.ErrorMessage = "You are required to update the Actual Value in appraisal evaluation line before submitting.";

//							return View(employeeAppraisalEvaluationObj);
//						}

//						//Modify employee appraisal header
//						dynamicsNAVSOAPServices.employeeAppraisalManagementWS.ModifyEmployeeAppraisalEvaluationHeader(employeeAppraisalEvaluationObj.No, employeeAppraisalEvaluationObj.ApprovedAppraisal, employeeAppraisalEvaluationObj.EvaluationStage);

//						//Check approval workflow enabled
//						if (!dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalEvaluationApprovalWorkflowEnabled(employeeAppraisalEvaluationObj.No))
//						{
//							employeeAppraisalEvaluationObj.ErrorStatus = true;
//							employeeAppraisalEvaluationObj.ErrorMessage = "No approval workflow enabled for employee appraisal evaluation record";

//							return View(employeeAppraisalEvaluationObj);
//						}

//						//Send approval request
//						if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.SendEmployeeAppraisalEvaluationApprovalRequest(employeeAppraisalEvaluationObj.No))
//						{
//							responseHeader = "Success";
//							responseMessage = "The appraisal evaluation no." + employeeAppraisalEvaluationObj.No + " was successfully sent for approval. Once approved you will get a notification from your HOD";
//							detailedResponseMessage = "The appraisal evaluation no." + employeeAppraisalEvaluationObj.No + " was successfully sent for approval. Once approved you will get a notification from your HOD";

//							button1ControllerName = "EmployeeEvaluation";
//							button1ActionName = "AppraisalEvaluationHistory";
//							button1Name = "Ok";
//							button1Parameters = "";

//							button2ControllerName = "";
//							button2ActionName = "";
//							button2HasParameters = false;
//							button2Parameters = "";
//							button2Name = "";

//							return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
//																  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//																  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//						}
//						else
//						{
//							employeeAppraisalEvaluationObj.ErrorStatus = true;
//							employeeAppraisalEvaluationObj.ErrorMessage = "An error was experienced while trying to send an approval request for appraisal evaluation no." + employeeAppraisalEvaluationObj.No + ". Contact the " + companyName + " ICT division for assistance.";
//							return View(employeeAppraisalEvaluationObj);
//						}
//					}
//					else
//					{
//						responseHeader = "Appraisal Evaluation NotFound";
//						responseMessage = "The appraisal evaluation no." + employeeAppraisalEvaluationObj.No + " was not found for employee no." + AccountController.GetEmployeeNo();
//						detailedResponseMessage = "The appraisal evaluation no." + employeeAppraisalEvaluationObj.No + " was not found for employee no." + AccountController.GetEmployeeNo();

//						button1ControllerName = "EmployeeEvaluation";
//						button1ActionName = "AppraisalEvaluationHistory";
//						button1Name = "Ok";
//						button1Parameters = "";

//						button2ControllerName = "";
//						button2ActionName = "";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";


//						return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}
//				}

//				else
//				{
//					return View(employeeAppraisalEvaluationObj);
//				}

//			}
//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}
//		#endregion Edit Edit Appraisal Evaluation

//		#region View Appraisal Evaluation
//		[Authorize]
//		public ActionResult ViewAppraisalEvaluation(string DocumentNo)
//		{
//			try
//			{
//				if (DocumentNo.Equals(""))
//				{
//					return RedirectToAction("AppraisalEvaluationHistory");
//				}

//				if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalEvaluationExists(DocumentNo, employeeNo))
//				{
//					AppraisalEvaluationHeaderModel employeeAppraisalEvaluationObj = new AppraisalEvaluationHeaderModel();

//					var employeeAppraisalEvaluations = from employeeAppraisalEvaluationQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalEvaluations
//													   where employeeAppraisalEvaluationQuery.Employee_No.Equals(employeeNo)
//													   select employeeAppraisalEvaluationQuery;


//					foreach (AppraisalEvaluations employeeAppraisalEvaluation in employeeAppraisalEvaluations)
//					{
//						employeeAppraisalEvaluationObj.EmployeeNo = employeeAppraisalEvaluation.Employee_No;
//						employeeAppraisalEvaluationObj.EmployeeName = employeeAppraisalEvaluation.Employee_Name;
//						employeeAppraisalEvaluationObj.No = employeeAppraisalEvaluation.No;
//						employeeAppraisalEvaluationObj.ApprovedAppraisal = employeeAppraisalEvaluation.Appraisal_No;
//						employeeAppraisalEvaluationObj.EvaluationStage = employeeAppraisalEvaluation.Evaluation_Stage;
//						employeeAppraisalEvaluationObj.Status = employeeAppraisalEvaluation.Status;
//					}

//					LoadAppraisalEvaluationStages();
//					employeeAppraisalEvaluationObj.EvaluationStages = new SelectList(AppraisalEvaluationStages, "Text", "Value");

//					LoadApprovedAppraisals();
//					employeeAppraisalEvaluationObj.AppraisalNos = new SelectList(approvedEmployeeAppraisals, "No", "Description");

//					return View(employeeAppraisalEvaluationObj);
//				}

//				else
//				{
//					responseHeader = "Appraisal Evaluation NotFound";
//					responseMessage = "The appraisal evaluation no." + DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The appraisal evaluation no." + DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();

//					button1ControllerName = "EmployeeEvaluation";
//					button1ActionName = "AppraisalEvaluationHistory";
//					button1Name = "Ok";
//					button1Parameters = "";

//					button2ControllerName = "";
//					button2ActionName = "";
//					button2HasParameters = false;
//					button2Parameters = "";
//					button2Name = "";


//					return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//														  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//														  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//				}

//			}

//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}
//		#endregion View Appraisal Evaluation

//		#region Appraisal Evaluation History
//		[Authorize]
//		public ActionResult AppraisalEvaluationHistory()
//		{
//			try
//			{
//				List<AppraisalEvaluationHeaderModel> employeeAppraisalEvaluationList = new List<AppraisalEvaluationHeaderModel>();

//				var employeeAppraisalEvaluations = from employeeAppraisalEvaluationQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalEvaluations
//												   where employeeAppraisalEvaluationQuery.Employee_No.Equals(employeeNo)
//												   select employeeAppraisalEvaluationQuery;

//				foreach (AppraisalEvaluations employeeAppraisalEvaluation in employeeAppraisalEvaluations)
//				{
//					AppraisalEvaluationHeaderModel employeeAppraisalEvaluationObj = new AppraisalEvaluationHeaderModel();
//					employeeAppraisalEvaluationObj.EmployeeNo = employeeAppraisalEvaluation.Employee_No;
//					employeeAppraisalEvaluationObj.EmployeeName = employeeAppraisalEvaluation.Employee_Name;
//					employeeAppraisalEvaluationObj.No = employeeAppraisalEvaluation.No;
//					employeeAppraisalEvaluationObj.ApprovedAppraisal = employeeAppraisalEvaluation.Appraisal_No;
//					employeeAppraisalEvaluationObj.EvaluationStage = employeeAppraisalEvaluation.Evaluation_Stage;
//					employeeAppraisalEvaluationObj.Status = employeeAppraisalEvaluation.Status;
//					employeeAppraisalEvaluationList.Add(employeeAppraisalEvaluationObj);
//				}
//				return View(employeeAppraisalEvaluationList);
//			}
//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}
//		#endregion Appraisal Evaluation History

//		#region Appraisal Evaluation Lines

//		[ChildActionOnly]
//		[Authorize]
//		public ActionResult _AppraisalEvaluationLine(string DocumentNo)
//		{
//			AppraisalEvaluationLineModel appraisalEvaluationObj = new AppraisalEvaluationLineModel();

//			LoadAppraisalObjectives();
//			LoadAppraisalPeriods();
//			LoadBaseUnitofMeasures();
//			LoadOrganizationalAppraisalCodes();
//			LoadDepartmentalAppraisalCodes();

//			appraisalEvaluationObj.AppraisalObjectives = new SelectList(globalAppraisalObjectives, "Code", "Description");
//			appraisalEvaluationObj.AppraisalPeriods = new SelectList(AppraisalPeriods, "Code", "Description");
//			appraisalEvaluationObj.BUMs = new SelectList(baseUnitMeasures, "Code", "Description");
//			appraisalEvaluationObj.OrganizationActivityCodes = new SelectList(OrganizationAppraisalCodes, "Activity_Code", "Activity_Description");
//			appraisalEvaluationObj.DepartmentActivityCodes = new SelectList(DepartmentalAppraisalCodes, "Activity_Code", "Activity_Description");

//			return PartialView(appraisalEvaluationObj);
//		}

//		[ChildActionOnly]
//		[Authorize]
//		public ActionResult _ViewAppraisalEvaluationLine(string DocumentNo)
//		{
//			AppraisalEvaluationLineModel appraisalEvaluationObj = new AppraisalEvaluationLineModel();

//			LoadAppraisalObjectives();
//			LoadAppraisalPeriods();
//			LoadBaseUnitofMeasures();
//			LoadOrganizationalAppraisalCodes();
//			LoadDepartmentalAppraisalCodes();

//			appraisalEvaluationObj.AppraisalObjectives = new SelectList(globalAppraisalObjectives, "Code", "Description");
//			appraisalEvaluationObj.AppraisalPeriods = new SelectList(AppraisalPeriods, "Code", "Description");
//			appraisalEvaluationObj.BUMs = new SelectList(baseUnitMeasures, "Code", "Description");
//			appraisalEvaluationObj.OrganizationActivityCodes = new SelectList(OrganizationAppraisalCodes, "Activity_Code", "Activity_Description");
//			appraisalEvaluationObj.DepartmentActivityCodes = new SelectList(DepartmentalAppraisalCodes, "Activity_Code", "Activity_Description");

//			return PartialView(appraisalEvaluationObj);
//		}

//		[Authorize]
//		public ActionResult GetAppraisalEvaluationLines(string DocumentNo)
//		{
//			try
//			{
//				List<AppraisalEvaluationLineModel> appraisalEvaluationList = new List<AppraisalEvaluationLineModel>();

//				var employeeAppraisalEvaluationLines = from AppraisalEvaluationQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalEvaluationLines
//													   where AppraisalEvaluationQuery.Evaluation_No.Equals(DocumentNo)
//													   select AppraisalEvaluationQuery;

//				foreach (AppraisalEvaluationLines AppraisalEvaluationLine in employeeAppraisalEvaluationLines)
//				{
//					AppraisalEvaluationLineModel appraisalEvaluationObj = new AppraisalEvaluationLineModel();
//					appraisalEvaluationObj.LineNo = AppraisalEvaluationLine.Line_No;
//					appraisalEvaluationObj.EvaluationNo = AppraisalEvaluationLine.Evaluation_No;
//					appraisalEvaluationObj.AppraisalNo = AppraisalEvaluationLine.Appraisal_No;
//					appraisalEvaluationObj.ActivityCode = AppraisalEvaluationLine.Activity_Code;
//					appraisalEvaluationObj.AppraisalPeriod = AppraisalEvaluationLine.Appraisal_Period;
//					appraisalEvaluationObj.AppraisalObjective = AppraisalEvaluationLine.Appraisal_Objective;
//					appraisalEvaluationObj.OrganizationActivityCode = AppraisalEvaluationLine.Organization_Activity_Descrp;
//					appraisalEvaluationObj.DepartmentActivityCode = AppraisalEvaluationLine.Departmental_Activity_Descrp;
//					appraisalEvaluationObj.ActivityDescription = AppraisalEvaluationLine.Activity_Description;
//					appraisalEvaluationObj.EvaluationNo = AppraisalEvaluationLine.Evaluation_No;
//					appraisalEvaluationObj.BUM = AppraisalEvaluationLine.Base_Unit_of_Measure;
//					appraisalEvaluationObj.TargetValue = AppraisalEvaluationLine.Target_Value ?? 0;
//					appraisalEvaluationObj.ActualValue = AppraisalEvaluationLine.Actual_Value ?? 0;
//					appraisalEvaluationList.Add(appraisalEvaluationObj);
//				}

//				return Json(appraisalEvaluationList, JsonRequestBehavior.AllowGet);
//			}

//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}

//		[Authorize]
//		public ActionResult GetAppraisalEvaluationLine(int LineNo, string DocumentNo)
//		{
//			try
//			{
//				AppraisalEvaluationLineModel appraisalEvaluationObj = new AppraisalEvaluationLineModel();

//				var employeeAppraisalEvaluationLines = from AppraisalEvaluationQuery in dynamicsNAVODataServices.dynamicsNAVOData.AppraisalEvaluationLines
//													   where AppraisalEvaluationQuery.Line_No.Equals(LineNo) && AppraisalEvaluationQuery.Evaluation_No.Equals(DocumentNo)
//													   select AppraisalEvaluationQuery;

//				foreach (AppraisalEvaluationLines AppraisalEvaluationLine in employeeAppraisalEvaluationLines)
//				{
//					appraisalEvaluationObj.LineNo = AppraisalEvaluationLine.Line_No;
//					appraisalEvaluationObj.EvaluationNo = AppraisalEvaluationLine.Evaluation_No;
//					appraisalEvaluationObj.AppraisalNo = AppraisalEvaluationLine.Appraisal_No;
//					appraisalEvaluationObj.ActivityCode = AppraisalEvaluationLine.Activity_Code;
//					appraisalEvaluationObj.AppraisalPeriod = AppraisalEvaluationLine.Appraisal_Period;
//					appraisalEvaluationObj.AppraisalObjective = AppraisalEvaluationLine.Appraisal_Objective;
//					appraisalEvaluationObj.OrganizationActivityCode = AppraisalEvaluationLine.Organization_Activity_Descrp;
//					appraisalEvaluationObj.DepartmentActivityCode = AppraisalEvaluationLine.Departmental_Activity_Descrp;
//					appraisalEvaluationObj.ActivityDescription = AppraisalEvaluationLine.Activity_Description;
//					appraisalEvaluationObj.EvaluationNo = AppraisalEvaluationLine.Evaluation_No;
//					appraisalEvaluationObj.BUM = AppraisalEvaluationLine.Base_Unit_of_Measure;
//					appraisalEvaluationObj.TargetValue = AppraisalEvaluationLine.Target_Value ?? 0;
//					appraisalEvaluationObj.ActualValue = AppraisalEvaluationLine.Actual_Value ?? 0;
//				}

//				return Json(appraisalEvaluationObj, JsonRequestBehavior.AllowGet);
//			}

//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}

//		[Authorize]
//		public JsonResult ModifyAppraisalEvaluationLine(AppraisalEvaluationLineModel appraisalEvaluationObj)
//		{
//			bool appraisalEvaluationModified = false;

//			appraisalEvaluationModified = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.ModifyAppraisalEvaluationLine(appraisalEvaluationObj.LineNo, appraisalEvaluationObj.EvaluationNo, appraisalEvaluationObj.ActualValue);

//			return Json(new { AppraisalEvaluationModified = appraisalEvaluationModified }, JsonRequestBehavior.AllowGet);
//		}

//		[Authorize]
//		public JsonResult DeleteAppraisalEvaluationLine(int LineNo, string DocumentNo)
//		{
//			bool appraisalEvaluationLineDeleted = false;

//			appraisalEvaluationLineDeleted = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.DeleteAppraisalEvaluationLine(LineNo, DocumentNo);

//			return Json(new { AppraisalEvaluationLineDeleted = appraisalEvaluationLineDeleted }, JsonRequestBehavior.AllowGet);
//		}

//		#endregion Appraisal Evaluation Lines

//		//#region Documents Management

//		//[ChildActionOnly]
//		//[Authorize]
//		//public ActionResult _AppraisalEvaluationDocument(string DocumentNo)
//		//{
//		//	DocumentMgmtModel portalDocumentObj = new DocumentMgmtModel();

//		//	return PartialView(portalDocumentObj);
//		//}

//		//[ChildActionOnly]
//		//[Authorize]
//		//public ActionResult _ViewAppraisalEvaluationDocument(string DocumentNo)
//		//{
//		//	DocumentMgmtModel portalDocumentObj = new DocumentMgmtModel();

//		//	return PartialView(portalDocumentObj);
//		//}

//		//[Authorize]
//		//public JsonResult GetAppraisalEvaluationDocuments(string DocumentNo)
//		//{
//		//	List<DocumentMgmtModel> portalDocumentsList = new List<DocumentMgmtModel>();

//		//	var portalDocuments = from portalDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
//		//						  where portalDocumentQuery.DocumentNo.Equals(DocumentNo)
//		//						  select portalDocumentQuery;

//		//	foreach (PortalDocuments portalDocument in portalDocuments)
//		//	{
//		//		DocumentMgmtModel portalDocumentObj = new DocumentMgmtModel();
//		//		portalDocumentObj.DocumentNo = portalDocument.DocumentNo;
//		//		portalDocumentObj.DocumentCode = portalDocument.Document_Code;
//		//		portalDocumentObj.DocumentDescription = portalDocument.Document_Description;
//		//		portalDocumentObj.DocumentAttached = portalDocument.Document_Attached ?? false;
//		//		portalDocumentObj.LocalURL = portalDocument.Local_File_URL;
//		//		portalDocumentObj.SharePointURL = portalDocument.SharePoint_URL;
//		//		portalDocumentsList.Add(portalDocumentObj);
//		//	}

//		//	return Json(portalDocumentsList, JsonRequestBehavior.AllowGet);
//		//}

//		//[Authorize]
//		//public JsonResult GetAppraisalEvaluationDocumentsView(string DocumentNo)
//		//{
//		//	List<DocumentMgmtModel> portalDocumentsList = new List<DocumentMgmtModel>();

//		//	var appraisalEvaluationDocuments = from appraisalEvaluationDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
//		//									   where appraisalEvaluationDocumentQuery.DocumentNo.Equals(DocumentNo)
//		//									   select appraisalEvaluationDocumentQuery;

//		//	foreach (PortalDocuments appraisalEvaluationDocument in appraisalEvaluationDocuments)
//		//	{
//		//		DocumentMgmtModel portalDocumentObj = new DocumentMgmtModel();
//		//		portalDocumentObj.DocumentNo = appraisalEvaluationDocument.DocumentNo;
//		//		portalDocumentObj.DocumentCode = appraisalEvaluationDocument.Document_Code;
//		//		portalDocumentObj.DocumentDescription = appraisalEvaluationDocument.Document_Description;
//		//		portalDocumentObj.DocumentAttached = appraisalEvaluationDocument.Document_Attached ?? false;
//		//		portalDocumentObj.LocalURL = appraisalEvaluationDocument.Local_File_URL;
//		//		portalDocumentObj.SharePointURL = appraisalEvaluationDocument.SharePoint_URL;
//		//		portalDocumentsList.Add(portalDocumentObj);
//		//	}

//		//	return Json(portalDocumentsList, JsonRequestBehavior.AllowGet);
//		//}

//		//[Authorize]
//		//public ActionResult GetAppraisalEvaluationDocument(string DocumentNo, string DocumentCode)
//		//{
//		//	try
//		//	{
//		//		DocumentMgmtModel portalDocumentObj = new DocumentMgmtModel();

//		//		var portalDocuments = from portalDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
//		//							  where portalDocumentQuery.DocumentNo.Equals(DocumentNo) && portalDocumentQuery.Document_Code.Equals(DocumentCode)
//		//							  select portalDocumentQuery;

//		//		foreach (PortalDocuments portalDocument in portalDocuments)
//		//		{
//		//			portalDocumentObj.DocumentNo = portalDocument.DocumentNo;
//		//			portalDocumentObj.DocumentCode = portalDocument.Document_Code;
//		//			portalDocumentObj.DocumentDescription = portalDocument.Document_Description;
//		//			portalDocumentObj.DocumentAttached = portalDocument.Document_Attached ?? false;
//		//			portalDocumentObj.LocalURL = portalDocument.Local_File_URL;
//		//			portalDocumentObj.SharePointURL = portalDocument.SharePoint_URL;
//		//		}

//		//		return Json(portalDocumentObj, JsonRequestBehavior.AllowGet);
//		//	}
//		//	catch (Exception ex)
//		//	{
//		//		return errorResponse.ApplicationExceptionError(ex);
//		//	}
//		//}

//		//[Authorize]
//		//[HttpPost]
//		//public JsonResult UploadAppraisalEvaluationDocument(string DocumentNo, string DocumentCode, string DocumentDescription)
//		//{
//		//	try
//		//	{
//		//		if (Request.Files.Count > 0)
//		//		{
//		//			var root = "~/StaffData/" + employeeNo;
//		//			bool folderpath = System.IO.Directory.Exists(HttpContext.Server.MapPath(root));

//		//			if (!folderpath)
//		//			{
//		//				System.IO.Directory.CreateDirectory(HttpContext.Server.MapPath(root));
//		//			}

//		//			var file = Request.Files[0];
//		//			string fileExt = System.IO.Path.GetExtension(file.FileName).ToLower();
//		//			string fileName = DocumentNo + "_" + DocumentDescription + fileExt;
//		//			string path = System.IO.Path.Combine(HttpContext.Server.MapPath(root), fileName);

//		//			if (System.IO.File.Exists(path))
//		//			{
//		//				System.IO.File.Delete(path);
//		//			}

//		//			file.SaveAs(path);

//		//			if (System.IO.File.Exists(path))
//		//			{
//		//				dynamicsNAVSOAPServices.documentMgmt.ModifyDocumentEntryFileURL(DocumentNo, DocumentCode, path);

//		//				return Json(new { success = true, message = DocumentDescription + " uploaded successfully" }, JsonRequestBehavior.AllowGet);
//		//			}
//		//			else
//		//			{
//		//				return Json(new { success = false, message = DocumentDescription + " was not uploaded. Try Again." }, JsonRequestBehavior.AllowGet);
//		//			}
//		//		}
//		//		return Json(new { success = false, message = "Please select a pdf file!" }, JsonRequestBehavior.AllowGet);
//		//	}
//		//	catch (Exception ex)
//		//	{
//		//		return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
//		//	}
//		//}

//		//#endregion Documents Management

//		#region Helper Functions
//		private void LoadAppraisalEvaluationStages()
//		{
//			List<SelectListItem> evaluationStages = new List<SelectListItem> { new SelectListItem { Text = "Quater One Evaluation", Value = "Quater One Evaluation" },
//																				  new SelectListItem { Text = "Mid Year Evaluation", Value = "Mid Year Evaluation" },
//																			   new SelectListItem { Text = "Quater Three Evaluation", Value = "Quater Three Evaluation" },
//																			   new SelectListItem { Text = "End Year Evaluation", Value = "End Year Evaluation" }};

//			AppraisalEvaluationStages = evaluationStages;
//		}
//		private void LoadApprovedAppraisals()
//		{
//			approvedEmployeeAppraisals = from employeeAppraisalQuery in dynamicsNAVODataServices.dynamicsNAVOData.ApprovedEmployeeAppraisal
//										 where employeeAppraisalQuery.Employee_No.Equals(employeeNo)
//										 select employeeAppraisalQuery;
//		}
//		private void LoadAppraisalObjectives()
//		{
//			globalAppraisalObjectives = from AppraisalObjectiveQuery in dynamicsNAVODataServices.dynamicsNAVOData.GlobalAppraisalObjectives
//										select AppraisalObjectiveQuery;
//		}
//		private void LoadOrganizationalAppraisalCodes()
//		{
//			OrganizationAppraisalCodes = from organizationalAppraisalQuery in dynamicsNAVODataServices.dynamicsNAVOData.OrganizationalAppraisalLines
//										 select organizationalAppraisalQuery;
//		}
//		private void LoadDepartmentalAppraisalCodes()
//		{
//			DepartmentalAppraisalCodes = from departmentalAppraisalQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalAppraisalLines
//										 select departmentalAppraisalQuery;
//		}
//		private void LoadBaseUnitofMeasures()
//		{
//			baseUnitMeasures = (from BUMQuery in dynamicsNAVODataServices.dynamicsNAVOData.BaseUnitOfMeasures
//								select BUMQuery);
//		}
//		private void LoadAppraisalPeriods()
//		{
//			AppraisalPeriods = from AppraisalPeriodQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRCalendarPeriods
//							   select AppraisalPeriodQuery;
//		}
//		public JsonResult InsertAppraisalEvaluationLines(string DocumentNo, string AppraisalNo)
//		{
//			return Json(dynamicsNAVSOAPServices.employeeAppraisalManagementWS.ValidateEmployeeAppraisalLines(DocumentNo, AppraisalNo));
//		}
//		#endregion Helper Functions
//	}
//}