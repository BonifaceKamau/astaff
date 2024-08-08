//using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
//using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
//using DynamicsNAV365_StaffPortal.Models.EmployeeAppraisal;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
//{
//    public class EmployeeAppraisalController : Controller
//    {
//		string companyName = ServiceConnection.CompanyName;
//		static string companyURL = "";

//		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
//		DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

//		SuccessResponseController successResponse = new SuccessResponseController();
//		InfoResponseController infoResponse = new InfoResponseController();
//		ErrorResponseController errorResponse = new ErrorResponseController();

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

//		IQueryable<GlobalAppraisalObjectives> globalAppraisalObjectives = null;
//		IQueryable<OrganizationalAppraisalLines> organizationActivityCodes = null; 
//		IQueryable<DepartmentalAppraisalLines> DepartmentalAppraisalCodes = null;
//		IQueryable<HRCalendarPeriods> AppraisalPeriods = null;
//		IQueryable<BaseUnitOfMeasures> baseUnitMeasures = null;
//		IEnumerable<SelectListItem> appraisalScoreTypes = null;
//		IEnumerable<SelectListItem> parameterTypes = null; 

//		AccountController accountController = new AccountController();
//		public EmployeeAppraisalController()
//		{
//			employeeNo = AccountController.GetEmployeeNo();
//		}

//		#region New Employee Appraisal
//		[Authorize]
//		public ActionResult NewEmployeeAppraisal()
//		{
//			bool employeeAppraisalCreated = false;
//			string openEmployeeAppraisalNo = "";
//			string EmployeeAppraisalNo = "";

//			try
//			{
//				//Check open employee appraisal exists
//				openEmployeeAppraisalNo = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckOpenEmployeeAppraisalExists(employeeNo);

//				if (!openEmployeeAppraisalNo.Equals(""))
//				{
//					responseHeader = "Open Employee Appraisal Exists";
//					responseMessage = "An open employee appraisal no."+ openEmployeeAppraisalNo + " exists under employee no. " + employeeNo + ", use this employee appraisal card before creating a new one.";
//					detailedResponseMessage = "An open employee appraisal no." + openEmployeeAppraisalNo + " exists under employee no. " + employeeNo + ", use this employee appraisal card before creating a new one.";

//					button1ControllerName = "EmployeeAppraisal";
//					button1ActionName = "EmployeeAppraisalHistory";
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

//				EmployeeAppraisalHeaderModel employeeAppraisalObj = new EmployeeAppraisalHeaderModel();

//				//Create New Employee Appraisal 
//				employeeAppraisalCreated = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CreateEmployeeAppraisalHeader(employeeNo);

//				//Get Employee Appraisal No.
//				EmployeeAppraisalNo = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckOpenEmployeeAppraisalExists(employeeNo);

//				employeeAppraisalObj.DocumentNo = EmployeeAppraisalNo;
//				employeeAppraisalObj.EmployeeNo = employeeNo;
//				employeeAppraisalObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

//				LoadAppraisalPeriods();
//				employeeAppraisalObj.CalendarPeriods = new SelectList(AppraisalPeriods, "Code", "Description");

//				return View(employeeAppraisalObj);
//			}

//			catch(Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}

//		[Authorize]
//		[HttpPost]
//		public ActionResult NewEmployeeAppraisal(EmployeeAppraisalHeaderModel employeeAppraisalObj) 
//		{
//			string DocumentNo = "";

//			try
//			{
//				//get open Document No.
//				DocumentNo = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckOpenEmployeeAppraisalExists(employeeNo);

//				LoadAppraisalPeriods();
//				employeeAppraisalObj.CalendarPeriods = new SelectList(AppraisalPeriods, "Code", "Description");

//				if (ModelState.IsValid)
//				{
//					if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalExists(DocumentNo,employeeNo))
//					{
//						//Check employee appraisal lines
//						if (!dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalLinesExist(DocumentNo))
//						{
//							employeeAppraisalObj.ErrorStatus = true;
//							employeeAppraisalObj.ErrorMessage = "Employee appraisal lines are missing. Add appraisal lines and try again to submit for approval.";

//							return View(employeeAppraisalObj);
//						}

//						//Modify employee appraisal header
//						dynamicsNAVSOAPServices.employeeAppraisalManagementWS.ModifyEmployeeAppraisalHeader(employeeAppraisalObj.DocumentNo, employeeAppraisalObj.CalendarPeriod, employeeAppraisalObj.Description);

//						//Check approval workflow enabled
//						if (!dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalApprovalWorkflowEnabled(employeeAppraisalObj.DocumentNo))
//						{
//							employeeAppraisalObj.ErrorStatus = true;
//							employeeAppraisalObj.ErrorMessage = "No approval workflow enabled for Employee Appraisal.";

//							return View(employeeAppraisalObj);
//						}

//						//Send approval request
//						if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.SendEmployeeAppraisalApprovalRequest(employeeAppraisalObj.DocumentNo))
//						{
//							responseHeader = "Success";
//							responseMessage = "The employee appraisal no." + employeeAppraisalObj.DocumentNo + " was successfully sent for approval. Once approved you will get a notification from your HOD";
//							detailedResponseMessage = "The employee appraisal no." + employeeAppraisalObj.DocumentNo + " was successfully sent for approval. Once approved you will get a notification from your HOD";

//							button1ControllerName = "EmployeeAppraisal";
//							button1ActionName = "EmployeeAppraisalHistory";
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
//							employeeAppraisalObj.ErrorStatus = true;
//							employeeAppraisalObj.ErrorMessage = "An error was experienced while trying to send an approval request for employee appraisal no." + employeeAppraisalObj.DocumentNo + ". Contact the " + companyName + " ICT division for assistance.";
//							return View(employeeAppraisalObj);
//						}
//					}
//					else
//					{
//						responseHeader = "Employee Appraisal NotFound";
//						responseMessage = "The employee appraisal no." + employeeAppraisalObj.DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();
//						detailedResponseMessage = "The employee appraisal no." + employeeAppraisalObj.DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();

//						button1ControllerName = "EmployeeAppraisal";
//						button1ActionName = "EmployeeAppraisalHistory";
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
//					return View(employeeAppraisalObj);
//				}
				
//			}
//			catch(Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}
//		#endregion End Employee Appraisal

//		#region Edit Employee Appraisal
//		[Authorize]
//		public ActionResult OnBeforeEdit(string DocumentNo)
//		{
//			try
//			{
//				if (DocumentNo.Equals(""))
//				{
//					return RedirectToAction("EmployeeAppraisalHistory");
//				}
//				if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalExists(DocumentNo, AccountController.GetEmployeeNo()))
//				{
//					string employeeAppraisalStatus = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.GetEmployeeAppraisalStatus(DocumentNo);

//					//if employee appraisal is open
//					if (employeeAppraisalStatus.Equals("Open"))
//					{
//						return RedirectToAction("EditEmployeeAppraisal", "EmployeeAppraisal", new { DocumentNo = DocumentNo }); 
//					}

//					//if employee appraisal is pending approval
//					if (employeeAppraisalStatus.Equals("Pending Approval"))
//					{
//						responseHeader = "Employee Appraisal Pending Approval";
//						responseMessage = "The employee appraisal no." + DocumentNo + " is already submitted for approval. Editing not allowed.";
//						detailedResponseMessage = "The employee appraisal no." + DocumentNo + " is already submitted for approval. Editing not allowed.";

//						button1ControllerName = "EmployeeAppraisal";
//						button1ActionName = "EmployeeAppraisalHistory";
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
//						responseHeader = "Employee Appraisal Approved";
//						responseMessage = "The employee appraisal no." + DocumentNo + " is already approved. Editing not allowed.";
//						detailedResponseMessage = "The employee appraisal no." + DocumentNo + " is already approved. Editing not allowed.";

//						button1ControllerName = "EmployeeAppraisal";
//						button1ActionName = "EmployeeAppraisalHistory";
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
//						responseHeader = "Employee Appraisal Rejected";
//						responseMessage = "The employee appraisal no." + DocumentNo + " was rejected. Editing will reopen the document. Do you want to continue?";
//						detailedResponseMessage = "The employee appraisal no." + DocumentNo + " was rejected. Editing will reopen the document. Do you want to continue?";

//						button1ControllerName = "EmployeeAppraisal";
//						button1ActionName = "EditEmployeeAppraisal";
//						button1Name = "Yes";
//						button1HasParameters = true;
//						button1Parameters = "?DocumentNo=" + DocumentNo;

//						button2ActionName = "EmployeeAppraisal";
//						button2ActionName = "EmployeeAppraisalHistory";
//						button2Name = "No";
//						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}

//					//if employee appraisal is posted/reversed
//					if (employeeAppraisalStatus.Equals("Posted") || employeeAppraisalStatus.Equals("Reversed"))
//					{
//						responseHeader = "Employee Appraisal Posted";
//						responseMessage = "The employee appraisal no." + DocumentNo + " is already posted. Editing not allowed.";
//						detailedResponseMessage = "The employee appraisal no." + DocumentNo + " is already posted. Editing not allowed.";

//						button1ControllerName = "EmployeeAppraisal";
//						button1ActionName = "EmployeeAppraisalHistory";
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
//					responseHeader = "Employee Appraisal NotFound";
//					responseMessage = "The employee appraisal no." + DocumentNo + " was not found under employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The employee appraisal no." + DocumentNo + " was not found under employee no." + AccountController.GetEmployeeNo();
//					button1ControllerName = "EmployeeAppraisal";
//					button1ActionName = "EmployeeAppraisalHistory";
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
//		public ActionResult EditEmployeeAppraisal(string DocumentNo)
//		{
//			try
//			{
//				if (DocumentNo.Equals(""))
//				{
//					return RedirectToAction("EmployeeAppraisalHistory");
//				}

//				if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalExists(DocumentNo, employeeNo))
//				{
//					string employeeAppraisalStatus = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.GetEmployeeAppraisalStatus(DocumentNo);
					
//					//if employee appraisal  is pending approval, cancel approval request
//					if (employeeAppraisalStatus.Equals("Pending Approval"))
//					{
//						dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CancelEmployeeAppraisalApprovalRequest(DocumentNo);
//					}

//					//if employee appraisal  is released, reopen and uncommit 
//					if (employeeAppraisalStatus.Equals("Released"))
//					{
//						dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CancelEmployeeAppraisalApprovalRequest(DocumentNo);
//					}

//					//if employee appraisal is rejected, reopen document
//					if (employeeAppraisalStatus.Equals("Rejected"))
//					{
//						dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CancelEmployeeAppraisalApprovalRequest(DocumentNo);
//					}

//					EmployeeAppraisalHeaderModel employeeAppraisalObj = new EmployeeAppraisalHeaderModel();

//					var employeeAppraisals = from employeeAppraisalsQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeAppraisals
//											 where employeeAppraisalsQuery.Employee_No.Equals(employeeNo)
//											 select employeeAppraisalsQuery;

//					foreach (EmployeeAppraisals employeeAppraisal in employeeAppraisals)
//					{
//						employeeAppraisalObj.DocumentNo = employeeAppraisal.No;
//						employeeAppraisalObj.CalendarPeriod = employeeAppraisal.Appraisal_Period;
//						employeeAppraisalObj.AppraisalStage = employeeAppraisal.Appraisal_Stage;
//						employeeAppraisalObj.Description = employeeAppraisal.Description;
//						employeeAppraisalObj.Status = employeeAppraisal.Status;
//					}

//					LoadAppraisalPeriods();
//					employeeAppraisalObj.CalendarPeriods = new SelectList(AppraisalPeriods, "Code", "Description");

//					return View(employeeAppraisalObj);
//				}

//				else
//				{
//					responseHeader = "Employee Appraisal NotFound";
//					responseMessage = "The employee appraisal no." + DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The employee appraisal no." + DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();

//					button1ControllerName = "EmployeeAppraisal";
//					button1ActionName = "EmployeeAppraisalHistory";
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
//		public ActionResult EditEmployeeAppraisal(EmployeeAppraisalHeaderModel employeeAppraisalObj)
//		{
//			string DocumentNo = "";
//			try
//			{
//				LoadAppraisalPeriods();
//				employeeAppraisalObj.CalendarPeriods = new SelectList(AppraisalPeriods, "Code", "Description");

//				if (ModelState.IsValid)
//				{
//					//get open Document No.
//					DocumentNo = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckOpenEmployeeAppraisalExists(employeeNo);

//					if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalExists(DocumentNo, employeeNo))
//					{
//						//Check employee appraisal lines
//						if (!dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalLinesExist(DocumentNo))
//						{
//							employeeAppraisalObj.ErrorStatus = true;
//							employeeAppraisalObj.ErrorMessage = "Employee appraisal lines are missing. Add appraisal lines and try again to submit for approval.";
//							return View(employeeAppraisalObj);
//						}

//						//Modify employee appraisal header
//						dynamicsNAVSOAPServices.employeeAppraisalManagementWS.ModifyEmployeeAppraisalHeader(employeeAppraisalObj.DocumentNo, employeeAppraisalObj.CalendarPeriod, employeeAppraisalObj.Description);

//						//Check approval workflow enabled
//						if (!dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalApprovalWorkflowEnabled(employeeAppraisalObj.DocumentNo))
//						{
//							employeeAppraisalObj.ErrorStatus = true;
//							employeeAppraisalObj.ErrorMessage = "No approval workflow enabled for Employee Appraisal.";

//							return View(employeeAppraisalObj);
//						}

//						//Send approval request
//						if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.SendEmployeeAppraisalApprovalRequest(employeeAppraisalObj.DocumentNo))
//						{
//							responseHeader = "Success";
//							responseMessage = "The employee appraisal no." + employeeAppraisalObj.DocumentNo + " was successfully sent for approval. Once approved you will get a notification from your HOD";
//							detailedResponseMessage = "The employee appraisal no." + employeeAppraisalObj.DocumentNo + " was successfully sent for approval. Once approved you will get a notification from your HOD";

//							button1ControllerName = "EmployeeAppraisal";
//							button1ActionName = "EmployeeAppraisalHistory";
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
//							employeeAppraisalObj.ErrorStatus = true;
//							employeeAppraisalObj.ErrorMessage = "An error was experienced while trying to send an approval request for employee appraisal no." + employeeAppraisalObj.DocumentNo + ". Contact the " + companyName + " ICT division for assistance.";
//							return View(employeeAppraisalObj);
//						}
//					}
//					else
//					{
//						responseHeader = "Employee Appraisal NotFound";
//						responseMessage = "The employee appraisal no." + employeeAppraisalObj.DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();
//						detailedResponseMessage = "The employee appraisal no." + employeeAppraisalObj.DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();

//						button1ControllerName = "EmployeeAppraisal";
//						button1ActionName = "EmployeeAppraisalHistory";
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
//					return View(employeeAppraisalObj);
//				}

//			}
//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}
//		#endregion Edit Employee Appraisal 

//		#region View Employee Appraisal
//		[Authorize]
//		public ActionResult ViewEmployeeAppraisal(string DocumentNo)
//		{
//			try
//			{
//				if (DocumentNo.Equals(""))
//				{
//					return RedirectToAction("EmployeeAppraisalHistory");
//				}

//				if (dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CheckEmployeeAppraisalExists(DocumentNo,employeeNo))
//				{
//					EmployeeAppraisalHeaderModel employeeAppraisalObj = new EmployeeAppraisalHeaderModel();

//					var employeeAppraisals = from employeeAppraisalsQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeAppraisals
//											 where employeeAppraisalsQuery.Employee_No.Equals(employeeNo)
//											 select employeeAppraisalsQuery;

//					foreach (EmployeeAppraisals employeeAppraisal in employeeAppraisals)
//					{
//						employeeAppraisalObj.DocumentNo = employeeAppraisal.No;
//						employeeAppraisalObj.CalendarPeriod = employeeAppraisal.Appraisal_Period;
//						employeeAppraisalObj.AppraisalStage = employeeAppraisal.Appraisal_Stage;
//						employeeAppraisalObj.Description = employeeAppraisal.Description;
//						employeeAppraisalObj.Status = employeeAppraisal.Status;
//					}

//					LoadAppraisalPeriods();
//					employeeAppraisalObj.CalendarPeriods = new SelectList(AppraisalPeriods, "Code", "Description");

//					return View(employeeAppraisalObj);
//				}

//				else
//				{
//					responseHeader = "Employee Appraisal NotFound";
//					responseMessage = "The employee appraisal no." + DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The employee appraisal no." + DocumentNo + " was not found for employee no." + AccountController.GetEmployeeNo();

//					button1ControllerName = "EmployeeAppraisal";
//					button1ActionName = "EmployeeAppraisalHistory";
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

//			catch(Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}
//		#endregion View Employee Appraisal

//		#region Employee Appraisal History
//		public ActionResult EmployeeAppraisalHistory()
//		{
//			try
//			{
//				List<EmployeeAppraisalHeaderModel> employeeAppraisalList = new List<EmployeeAppraisalHeaderModel>();

//				var employeeAppraisals = from employeeAppraisalsQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeAppraisals
//										 where employeeAppraisalsQuery.Employee_No.Equals(employeeNo)
//										 select employeeAppraisalsQuery;

//				foreach (EmployeeAppraisals employeeAppraisal in employeeAppraisals)
//				{
//					EmployeeAppraisalHeaderModel employeeAppraisalObj = new EmployeeAppraisalHeaderModel();
//					employeeAppraisalObj.EmployeeNo = employeeAppraisal.Employee_No;
//					employeeAppraisalObj.EmployeeName = employeeAppraisal.Employee_Name;
//					employeeAppraisalObj.DocumentNo = employeeAppraisal.No;
//					employeeAppraisalObj.CalendarPeriod = employeeAppraisal.Appraisal_Period;
//					employeeAppraisalObj.AppraisalStage = employeeAppraisal.Appraisal_Stage;
//					employeeAppraisalObj.Description = employeeAppraisal.Description;
//					employeeAppraisalObj.Status = employeeAppraisal.Status;
//					employeeAppraisalList.Add(employeeAppraisalObj);
//				}
//				return View(employeeAppraisalList);
//			}
//			catch(Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}
//		#endregion Employee Appraisal History

//		#region Employee Appraisal Line

//		[Authorize]
//		[ChildActionOnly]
//		public ActionResult _EmployeeAppraisalLine(string DocumentNo)
//		{ 
//			EmployeeAppraisalLineModel employeeAppraisalObj = new EmployeeAppraisalLineModel();

//			LoadAppraisalObjectives();
//			LoadAppraisalPeriods();
//			LoadBaseUnitofMeasures();
//			LoadOrganizationalAppraisalCodes();
//			LoadDepartmentalAppraisalCodes();
//			LoadParameterTypes();
//			LoadAppraisalScoreTypes();

//			employeeAppraisalObj.ParameterTypes = new SelectList(parameterTypes, "Text", "Value");
//			employeeAppraisalObj.AppraisalScoreTypes = new SelectList(appraisalScoreTypes, "Text", "Value");
//			employeeAppraisalObj.AppraisalObjectives = new SelectList(globalAppraisalObjectives, "Code", "Description");
//			employeeAppraisalObj.AppraisalPeriods = new SelectList(AppraisalPeriods, "Code", "Description");
//			employeeAppraisalObj.BUMs = new SelectList(baseUnitMeasures, "Code", "Description");
//			employeeAppraisalObj.OrganizationActivityCodes = new SelectList(organizationActivityCodes, "Activity_Code", "Activity_Description");
//			employeeAppraisalObj.DepartmentActivityCodes = new SelectList(DepartmentalAppraisalCodes, "Activity_Code", "Activity_Description");

//			return PartialView(employeeAppraisalObj);
//		}

//		[Authorize]
//		[ChildActionOnly]
//		public ActionResult _ViewEmployeeAppraisalLine(string DocumentNo)
//		{
//			EmployeeAppraisalLineModel employeeAppraisalObj = new EmployeeAppraisalLineModel();

//			LoadAppraisalObjectives();
//			LoadAppraisalPeriods();
//			LoadBaseUnitofMeasures();
//			LoadOrganizationalAppraisalCodes();
//			LoadDepartmentalAppraisalCodes();
//			LoadParameterTypes();
//			LoadAppraisalScoreTypes();

//			employeeAppraisalObj.ParameterTypes = new SelectList(parameterTypes, "Text", "Value");
//			employeeAppraisalObj.AppraisalScoreTypes = new SelectList(appraisalScoreTypes, "Text", "Value");
//			employeeAppraisalObj.AppraisalObjectives = new SelectList(globalAppraisalObjectives, "Code", "Description");
//			employeeAppraisalObj.AppraisalPeriods = new SelectList(AppraisalPeriods, "Code", "Description");
//			employeeAppraisalObj.BUMs = new SelectList(baseUnitMeasures, "Code", "Description");
//			employeeAppraisalObj.OrganizationActivityCodes = new SelectList(organizationActivityCodes, "Activity_Code", "Activity_Description");
//			employeeAppraisalObj.DepartmentActivityCodes = new SelectList(DepartmentalAppraisalCodes, "Activity_Code", "Activity_Description");

//			return PartialView(employeeAppraisalObj); 
//		}

//		[Authorize]
//		public ActionResult GetEmployeeAppraisalLines(string DocumentNo)
//		{
//			try
//			{
//				List<EmployeeAppraisalLineModel> employeeAppraisalList = new List<EmployeeAppraisalLineModel>();

//				var employeeAppraisalLines = from employeeAppraisalQuery in dynamicsNAVODataServices.dynamicsNAVOData.IndividualAppraisalLines
//											 where employeeAppraisalQuery.Appraisal_No.Equals(DocumentNo)
//											 select employeeAppraisalQuery;

//				foreach (IndividualAppraisalLines employeeAppraisalLine in employeeAppraisalLines)
//				{
//					EmployeeAppraisalLineModel employeeAppraisalObj = new EmployeeAppraisalLineModel();
//					employeeAppraisalObj.AppraisalPeriod = employeeAppraisalLine.Appraisal_Period;
//					employeeAppraisalObj.AppraisalObjective = employeeAppraisalLine.Appraisal_Objective;
//					employeeAppraisalObj.ActivityCode = employeeAppraisalLine.Activity_Code;
//					employeeAppraisalObj.OrganizationActivityCode = employeeAppraisalLine.Organization_Activity_Descrp;
//					employeeAppraisalObj.DepartmentActivityCode = employeeAppraisalLine.Departmental_Activity_Descrp;
//					employeeAppraisalObj.ActivityDescription = employeeAppraisalLine.Activity_Description;
//					employeeAppraisalObj.ObjectiveWeight = employeeAppraisalLine.Objective_Weight ?? 0;
//					employeeAppraisalObj.ActivityWeight = employeeAppraisalLine.Activity_Weight ?? 0;
//					employeeAppraisalObj.TargetValue = employeeAppraisalLine.Target_Value ?? 0;
//					employeeAppraisalObj.AppraisalScoreType = employeeAppraisalLine.Appraisal_Score_Type;
//					employeeAppraisalObj.ParameterType = employeeAppraisalLine.Parameter_Type;
//					employeeAppraisalObj.BUM = employeeAppraisalLine.Base_Unit_of_Measure;
//					employeeAppraisalObj.Q1ActualValue = employeeAppraisalLine.Q1_Actual_Value ?? 0;

//					employeeAppraisalList.Add(employeeAppraisalObj);
//				}

//				return Json(employeeAppraisalList, JsonRequestBehavior.AllowGet);
//			}

//			catch(Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}

//		[Authorize]
//		public ActionResult GetEmployeeAppraisalLine(string DocumentNo, string ActivityCode ,string AppraisalPeriod, string AppraisalObjective, string OrganizationActivityCode, string DepartmentActivityCode) 
//		{
//			try
//			{
//				EmployeeAppraisalLineModel employeeAppraisalObj = new EmployeeAppraisalLineModel();

//				var employeeAppraisalLines = from employeeAppraisalQuery in dynamicsNAVODataServices.dynamicsNAVOData.IndividualAppraisalLines
//											 where employeeAppraisalQuery.Appraisal_No.Equals(DocumentNo) && employeeAppraisalQuery.Activity_Code.Equals(ActivityCode) && employeeAppraisalQuery.Appraisal_Period.Equals(AppraisalPeriod) && employeeAppraisalQuery.Appraisal_Objective.Equals(AppraisalObjective) && employeeAppraisalQuery.Organization_Activity_Code.Equals(OrganizationActivityCode) && employeeAppraisalQuery.Departmental_Activity_Code.Equals(DepartmentActivityCode)
//											 select employeeAppraisalQuery;

//				foreach (IndividualAppraisalLines employeeAppraisalLine in employeeAppraisalLines)
//				{
//					employeeAppraisalObj.ActivityCode = employeeAppraisalLine.Activity_Code;
//					employeeAppraisalObj.ActivityDescription = employeeAppraisalLine.Activity_Description;
//					employeeAppraisalObj.ActivityWeight = employeeAppraisalLine.Activity_Weight ?? 0;
//					employeeAppraisalObj.AppraisalObjective = employeeAppraisalLine.Appraisal_Objective;
//					employeeAppraisalObj.AppraisalPeriod = employeeAppraisalLine.Appraisal_Period;
//					employeeAppraisalObj.BUM = employeeAppraisalLine.Base_Unit_of_Measure;
//					employeeAppraisalObj.OrganizationActivityCode = employeeAppraisalLine.Organization_Activity_Code;
//					employeeAppraisalObj.DepartmentActivityCode = employeeAppraisalLine.Departmental_Activity_Code;
//					employeeAppraisalObj.Q1ActualValue = employeeAppraisalLine.Q1_Actual_Value ?? 0;
//				}

//				return Json(employeeAppraisalObj, JsonRequestBehavior.AllowGet);
//			}

//			catch(Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}

//		[Authorize]
//		public JsonResult CreateEmployeeAppraisalLine(EmployeeAppraisalLineModel employeeAppraisalObj)
//		{
//			bool employeeAppraisalLineCreated = false;

//			employeeAppraisalLineCreated = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.CreateEmployeeAppraisalLine(employeeAppraisalObj.DocumentNo, employeeAppraisalObj.AppraisalPeriod, employeeAppraisalObj.AppraisalObjective, employeeAppraisalObj.OrganizationActivityCode,
//																															 employeeAppraisalObj.DepartmentActivityCode, employeeAppraisalObj.ActivityDescription, employeeAppraisalObj.ActivityWeight, employeeAppraisalObj.TargetValue,
//																															 employeeAppraisalObj.AppraisalScoreType, employeeAppraisalObj.ParameterType, employeeAppraisalObj.BUM);

//			return Json(new { EmployeeAppraisalLineCreated = employeeAppraisalLineCreated }, JsonRequestBehavior.AllowGet);
//		}

//		[Authorize]
//		public JsonResult ModifyEmployeeAppraisalLine(EmployeeAppraisalLineModel employeeAppraisalObj)
//		{
//			bool employeeAppraisalLineModified = false;

//			employeeAppraisalLineModified = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.ModifyEmployeeAppraisalLine(employeeAppraisalObj.DocumentNo,employeeAppraisalObj.AppraisalObjective, employeeAppraisalObj.AppraisalPeriod, employeeAppraisalObj.ActivityCode,
//																															 employeeAppraisalObj.OrganizationActivityCode, employeeAppraisalObj.DepartmentActivityCode, employeeAppraisalObj.ActivityDescription,
//																															 employeeAppraisalObj.ActivityWeight, employeeAppraisalObj.TargetValue, employeeAppraisalObj.AppraisalScoreType, employeeAppraisalObj.ParameterType, 
//																															 employeeAppraisalObj.BUM);

//			return Json(new { EmployeeAppraisalLineModified = employeeAppraisalLineModified }, JsonRequestBehavior.AllowGet); 
//		}

//		[Authorize]
//		public JsonResult DeleteEmployeeAppraisalLine(string DocumentNo, string ActivityCode, string AppraisalPeriod, string AppraisalObjective, string OrganizationActivityCode, string DepartmentActivityCode)
//		{
//			bool employeeAppraisalLineDeleted = false; 

//			employeeAppraisalLineDeleted = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.DeleteEmployeeAppraisalLine(ActivityCode, DocumentNo, AppraisalPeriod, AppraisalObjective, OrganizationActivityCode, DepartmentActivityCode);

//			return Json(new { EmployeeAppraisalLineDeleted = employeeAppraisalLineDeleted }, JsonRequestBehavior.AllowGet);
//		}

//		#endregion Employee Appraisal Line

//		#region Helper Functions	
//		public JsonResult GetObjectiveWeight(string GlobalAppraisalObjective)
//		{
//			decimal objectiveWeight = 0;
//			objectiveWeight = dynamicsNAVSOAPServices.employeeAppraisalManagementWS.GetObjectiveWeight(GlobalAppraisalObjective);
//			return Json(new { ObjectiveWeight = objectiveWeight }, JsonRequestBehavior.AllowGet);
//		}
//		public JsonResult GetOrganizationActivityCodes(string GlobalObjective)
//        {
//			if (!GlobalObjective.Equals(""))
//			{
//				 var organizationalActivityCodes = from _organizationActivityCode in dynamicsNAVODataServices.dynamicsNAVOData.OrganizationalAppraisalLines
//										    	 where _organizationActivityCode.Appraisal_Objective.Equals(GlobalObjective)
//										    	 select _organizationActivityCode;

//				return Json(organizationalActivityCodes.ToList(), JsonRequestBehavior.AllowGet);
//			}
//			else
//			{
//				return Json(new List<string>(), JsonRequestBehavior.AllowGet);
//			}
//		}
//		public JsonResult GetDepartmentalAppraisalCodes(string OrganizationalAppraisalCode)
//		{
//			if (!OrganizationalAppraisalCode.Equals(""))
//			{
//				DepartmentalAppraisalCodes = from departmentAppraisalCodesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalAppraisalLines
//											   where departmentAppraisalCodesQuery.Organization_Activity_Code.Equals(OrganizationalAppraisalCode)
//											   select departmentAppraisalCodesQuery;

//				return Json(DepartmentalAppraisalCodes.ToList(), JsonRequestBehavior.AllowGet);
//			}
//			else
//			{
//				return Json(new List<string>(), JsonRequestBehavior.AllowGet);
//			}
//		}
//		private void LoadAppraisalObjectives()
//		{
//			globalAppraisalObjectives = from AppraisalObjectiveQuery in dynamicsNAVODataServices.dynamicsNAVOData.GlobalAppraisalObjectives
//								        select AppraisalObjectiveQuery;
//		}
//		private void LoadOrganizationalAppraisalCodes() 
//		{
//			 organizationActivityCodes = from organizationalAppraisalQuery in dynamicsNAVODataServices.dynamicsNAVOData.OrganizationalAppraisalLines
//											select organizationalAppraisalQuery;
//		}
//		private void LoadDepartmentalAppraisalCodes() 
//		{
//			DepartmentalAppraisalCodes = from departmentalAppraisalQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalAppraisalLines
//										    select departmentalAppraisalQuery;
//		}	
//		private void LoadBaseUnitofMeasures()
//		{
//			baseUnitMeasures = (from BUMQuery in dynamicsNAVODataServices.dynamicsNAVOData.BaseUnitOfMeasures
//				            	select BUMQuery);
//		}
//		private void LoadAppraisalPeriods()
//		{
//			AppraisalPeriods = from AppraisalPeriodQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRCalendarPeriods
//							   select AppraisalPeriodQuery;
//		}
//		private void LoadParameterTypes()
//		{
//			List<SelectListItem> _parameterTypes = new List<SelectListItem> { new SelectListItem { Text = "Time-Based", Value = "Time-Based" },
//																				   new SelectListItem { Text = "Value Based", Value = "Value-Based" }};

//			parameterTypes = _parameterTypes;
//		}
//		private void LoadAppraisalScoreTypes()
//		{
//			List<SelectListItem> _appraisalScoreTypes = new List<SelectListItem> { new SelectListItem { Text = "Core", Value = "Core" },
//																			       new SelectListItem { Text = "Non-Core", Value = "Non-Core" }};

//			appraisalScoreTypes = _appraisalScoreTypes;
//		}
//		#endregion Helper Functions
//	}
//}