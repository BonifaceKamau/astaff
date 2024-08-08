//using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
//using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
//using DynamicsNAV365_StaffPortal.Models.HumanResource.LeaveReimbursementModels;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
//{
//    public class LeaveReimbursementController : Controller
//    {
//		static string companyName = ServiceConnection.CompanyName;
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

//		IQueryable<HRLeaveApplications> leaveApplications = null;

//		AccountController accountController = new AccountController();
//		string employeeNo = "";
//		public LeaveReimbursementController()
//		{
//			employeeNo = AccountController.GetEmployeeNo();
//		}

//		#region New Leave Reimbursement
//		[Authorize]
//		public ActionResult NewLeaveReimbursment()
//		{
//			try
//			{
//				//Check open leave reimbursement
//				if (dynamicsNAVSOAPServices.hrManagementWS.CheckOpenLeaveReimbursementExists(employeeNo))
//				{
//					responseHeader = "Leave Recall Request Exist";
//					responseMessage = "An open leave reimbursement exists for employee no. " + employeeNo + ", finalize on this leave application before creating a new one.";
//					detailedResponseMessage = "An open leave reimbursement exists for employee no. " + employeeNo + ", finalize on this leave application before creating a new one.";

//					button1ControllerName = "LeaveReimbursement";
//					button1ActionName = "LeaveReimbursementHistory";
//					button1HasParameters = false;
//					button1Parameters = "";
//					button1Name = "Ok";

//					button2ControllerName = "";
//					button2ActionName = "";
//					button2HasParameters = false;
//					button2Parameters = "";
//					button2Name = "";
//					return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//														  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//														  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//				}
//				//End check open leave reimbursement

//				//Create Leave Reimbursement 
//				bool leaveReimbursementCreated = dynamicsNAVSOAPServices.hrManagementWS.CreateLeaveReimbursement(employeeNo);
//				//End Create Leave Reimbursement 

//				//Get Leave Reimbursement No
//				string leaveReimbursementNo = dynamicsNAVSOAPServices.hrManagementWS.GetLeaveReimbursementNo(employeeNo);
//				//End Get Leave Reimbursement No

//				LeaveReimbursementModel leaveReimbursementObj = new LeaveReimbursementModel();
//				leaveReimbursementObj.No = leaveReimbursementNo;
//				leaveReimbursementObj.EmployeeNo = AccountController.GetEmployeeNo();
//				leaveReimbursementObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

//				LoadLeaveApplications();
//				leaveReimbursementObj.LeaveApplicationNos = new SelectList(leaveApplications, "No", "Leave_Type");

//				return View(leaveReimbursementObj);
//			}
//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}

//		[Authorize]
//		[HttpPost]
//		[ValidateAntiForgeryToken]
//		public ActionResult NewLeaveReimbursement(LeaveReimbursementModel leaveReimbursementObj)
//		{
//			bool leaveReimbursementModified = false;
//			string leaveReimbursementNo = "";
//			try
//			{
//				leaveReimbursementObj.EmployeeNo = AccountController.GetEmployeeNo();
//				leaveReimbursementObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);
//				leaveReimbursementObj.GlobalDimension1Code = "";

//				LoadLeaveApplications();
//				leaveReimbursementObj.LeaveApplicationNos = new SelectList(leaveApplications, "No", "Leave_Type");

//				leaveReimbursementNo = dynamicsNAVSOAPServices.hrManagementWS.GetLeaveReimbursementNo(employeeNo);

//				if (ModelState.IsValid)
//				{

//					leaveReimbursementModified = dynamicsNAVSOAPServices.hrManagementWS.ModifyLeaveReimbursement(leaveReimbursementNo, leaveReimbursementObj.EmployeeNo, leaveReimbursementObj.ApprovedLeaveApplication, DateTime.Parse(leaveReimbursementObj.ActualReturnDate), leaveReimbursementObj.ReasonForReimbursement);

//					if (!leaveReimbursementModified)
//					{
//						leaveReimbursementObj.ErrorStatus = true;
//						leaveReimbursementObj.ErrorMessage = "An error was experienced when trying to create you leave reimbursement. Contact the " + companyName + " ICT division for assistance.";
//						return View(leaveReimbursementObj);
//					}

//					leaveReimbursementNo = dynamicsNAVSOAPServices.hrManagementWS.GetLeaveReimbursementNo(AccountController.GetEmployeeNo());

//					if (leaveReimbursementNo.Equals(""))
//					{
//						leaveReimbursementObj.ErrorStatus = true;
//						leaveReimbursementObj.ErrorMessage = "An error was experienced when trying to create you leave reimbursement. Contact the " + companyName + " ICT division for assistance.";
//						return View(leaveReimbursementObj);
//					}

//					if (!dynamicsNAVSOAPServices.hrManagementWS.CheckLeaveReimbursementApprovalWorkflowEnabled(leaveReimbursementNo))
//					{
//						leaveReimbursementObj.No = leaveReimbursementNo;
//						leaveReimbursementObj.ErrorStatus = true;
//						leaveReimbursementObj.ErrorMessage = "An error was experienced when sending your leave reimbursement no." + leaveReimbursementNo + " for approval. Try again or contact the " + companyName + " ICT division for assistance.";
//						return View(leaveReimbursementObj);
//					}

//					if (dynamicsNAVSOAPServices.hrManagementWS.SendLeaveReimbursementApprovalRequest(leaveReimbursementNo))
//					{
//						responseHeader = "Leave Reimbursement Successful";
//						responseMessage = "Your leave reimbursement was successfully sent for approval. Once approved, you will receive an email containing your leave details.";
//						detailedResponseMessage = "Your leave reimbursement was successfully sent for approval. Once approved, you will receive an email containing your leave details.";

//						button1ControllerName = "LeaveReimbursement";
//						button1ActionName = "LeaveReimbursementHistory";
//						button1HasParameters = false;
//						button1Parameters = "";
//						button1Name = "Ok";

//						button2ControllerName = "";
//						button2ActionName = "";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";

//						return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
//																	button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//																	button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}
//					else
//					{
//						leaveReimbursementObj.No = leaveReimbursementNo;
//						leaveReimbursementObj.ErrorStatus = true;
//						leaveReimbursementObj.ErrorMessage = "An error was experienced when sending your leave reimbursement no." + leaveReimbursementNo + " for approval. Try again or contact the " + companyName + " ICT division for assistance.";
//						return View(leaveReimbursementObj);
//					}

//				}
//				else
//				{
//					return View(leaveReimbursementObj);
//				}
//			}
//			catch (Exception ex)
//			{
//				leaveReimbursementObj.No = leaveReimbursementNo;
//				leaveReimbursementObj.ErrorStatus = true;
//				leaveReimbursementObj.ErrorMessage = ex.Message;
//				return View(leaveReimbursementObj);
//			}
//		}
//		#endregion New Leave Reimbursement	

//		#region Edit Leave Reimbursement
//		[Authorize]
//		public ActionResult OnBeforeEdit(string LeaveReimbursementNo)
//		{
//			try
//			{
//				if (LeaveReimbursementNo.Equals(""))
//				{
//					return RedirectToAction("LeaveReimbursementHistory", "LeaveReimbursement");
//				}
//				if (dynamicsNAVSOAPServices.hrManagementWS.CheckLeaveReimbursementExists(LeaveReimbursementNo, AccountController.GetEmployeeNo()))
//				{
//					string leaveReimbursementStatus = dynamicsNAVSOAPServices.hrManagementWS.GetLeaveReimbursementStatus(LeaveReimbursementNo);
//					//if leave reimbursement is open
//					if (leaveReimbursementStatus.Equals("Open"))
//					{
//						return RedirectToAction("EditLeaveReimbursement", "LeaveReimbursement", new { LeaveReimbursementNo = LeaveReimbursementNo });
//					}
//					//if leave reimbursement is pending approval
//					if (leaveReimbursementStatus.Equals("Pending Approval"))
//					{
//						responseHeader = "Leave Reimbursement Pending Approval";
//						responseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " is already under approval process. Editing not allowed.";
//						detailedResponseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " is already under approval process. Editing not allowed.";

//						button1ControllerName = "LeaveReimbursement";
//						button1ActionName = "LeaveReimbursementHistory";
//						button1HasParameters = false;
//						button1Parameters = "";
//						button1Name = "Ok";

//						button2ControllerName = "";
//						button2ActionName = "";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";
//						return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}
//					//if leave reimbursement is released
//					if (leaveReimbursementStatus.Equals("Released"))
//					{
//						responseHeader = "Leave Reimbursement Approved";
//						responseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " is approved. Editing will reopen the document. Do you want to continue?";
//						detailedResponseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " is approved. Editing will reopen the document. Do you want to continue?";

//						button1ControllerName = "LeaveReimbursement";
//						button1ActionName = "EditLeaveReimbursement";
//						button1HasParameters = true;
//						button1Parameters = "?LeaveReimbursementNo=" + LeaveReimbursementNo;
//						button1Name = "Ok";

//						button2ControllerName = "LeaveReimbursement";
//						button2ActionName = "LeaveReimbursementHistory";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";

//						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}
//					//if leave reimbursement is rejected
//					if (leaveReimbursementStatus.Equals("Rejected"))
//					{
//						responseHeader = "Leave Reimbursement Rejected";
//						responseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " was rejected. Editing will reopen the document. Do you want to continue?";
//						detailedResponseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " was rejected. Editing will reopen the document. Do you want to continue?";

//						button1ControllerName = "LeaveReimbursement";
//						button1ActionName = "EditLeaveReimbursement";
//						button1HasParameters = true;
//						button1Parameters = "?LeaveReimbursementNo=" + LeaveReimbursementNo;
//						button1Name = "Ok";

//						button2ControllerName = "LeaveReimbursement";
//						button2ActionName = "LeaveReimbursementHistory";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";

//						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}
//					//if leave reimbursement is posted/reversed
//					if (leaveReimbursementStatus.Equals("Posted") || leaveReimbursementStatus.Equals("Reversed"))
//					{
//						responseHeader = "Leave Reimbursement Posted";
//						responseMessage = "The leave application no." + LeaveReimbursementNo + " is already posted. Editing not allowed.";
//						detailedResponseMessage = "The leave application no." + LeaveReimbursementNo + " is already posted. Editing not allowed.";

//						button1ControllerName = "LeaveReimbursement";
//						button1ActionName = "LeaveReimbursementHistory";
//						button1HasParameters = false;
//						button1Parameters = "";
//						button1Name = "Ok";

//						button2ControllerName = "";
//						button2ActionName = "";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";
//						return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}
//					return RedirectToAction("LeaveReimbursementHistory", "LeaveReimbursement");
//				}
//				else
//				{
//					responseHeader = "Leave Reimbursement NotFound";
//					responseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " was not found under employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " was not found under employee no." + AccountController.GetEmployeeNo();

//					button1ControllerName = "LeaveReimbursement";
//					button1ActionName = "LeaveReimbursementHistory";
//					button1HasParameters = false;
//					button1Parameters = "";
//					button1Name = "Ok";

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
//		public ActionResult EditLeaveReimbursement(string LeaveReimbursementNo)
//		{
//			try
//			{
//				if (LeaveReimbursementNo.Equals(""))
//				{
//					return RedirectToAction("LeaveReimbursementHistory", "LeaveReimbursement");
//				}
//				if (dynamicsNAVSOAPServices.hrManagementWS.CheckLeaveReimbursementExists(LeaveReimbursementNo, AccountController.GetEmployeeNo()))
//				{
//					string leaveReimbursementStatus = dynamicsNAVSOAPServices.hrManagementWS.GetLeaveReimbursementStatus(LeaveReimbursementNo);

//					//if leave reimbursement is released, reopen document
//					if (leaveReimbursementStatus.Equals("Released"))
//					{
//						dynamicsNAVSOAPServices.hrManagementWS.CancelLeaveReimbursementApprovalRequest(LeaveReimbursementNo);
//					}
//					//if leave reimbursement is rejected, reopen document
//					if (leaveReimbursementStatus.Equals("Rejected"))
//					{
//						dynamicsNAVSOAPServices.hrManagementWS.CancelLeaveReimbursementApprovalRequest(LeaveReimbursementNo);
//					}

//					LeaveReimbursementModel leaveReimbursementObj = new LeaveReimbursementModel();

//					var leaveReimbursements = from leaveReimbursementsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRLeaveReimbursement
//											  where leaveReimbursementsQuery.No.Equals(LeaveReimbursementNo)
//											  select leaveReimbursementsQuery;

//					foreach (HRLeaveReimbursement leaveReimbursement in leaveReimbursements)
//					{
//						leaveReimbursementObj.No = leaveReimbursement.No;
//						leaveReimbursementObj.EmployeeNo = leaveReimbursement.Employee_No;
//						leaveReimbursementObj.EmployeeName = leaveReimbursement.Employee_Name;
//						leaveReimbursementObj.LeaveType = leaveReimbursement.Leave_Type;
//						leaveReimbursementObj.LeaveStartDate = leaveReimbursement.Leave_Start_Date.Value.ToString("dd-MM-yyyy");
//						leaveReimbursementObj.LeaveBalance = leaveReimbursement.Leave_Balance.Value;
//						leaveReimbursementObj.DaysApplied = leaveReimbursement.Leave_Days_Applied.Value;
//						leaveReimbursementObj.DaysApproved = leaveReimbursement.Leave_Days_Approved ?? 0;
//						leaveReimbursementObj.LeaveEndDate = leaveReimbursement.Leave_End_Date.Value.ToString("dd-MM-yyyy");
//						leaveReimbursementObj.LeaveReturnDate = leaveReimbursement.Leave_Return_Date.Value.ToString("dd-MM-yyyy");
//						leaveReimbursementObj.ReasonForLeave = leaveReimbursement.Reason_for_Leave;
//						leaveReimbursementObj.SubstituteNo = leaveReimbursement.Substitute_No;
//						leaveReimbursementObj.SubstituteName = leaveReimbursement.Substitute_Name;
//						leaveReimbursementObj.ApprovedLeaveApplication = leaveReimbursement.Leave_Type;
//						leaveReimbursementObj.Status = leaveReimbursement.Status;
//					}

//					LoadLeaveApplications();
//					leaveReimbursementObj.LeaveApplicationNos = new SelectList(leaveApplications, "No", "Leave_Type");

//					return View(leaveReimbursementObj);
//				}
//				else
//				{
//					responseHeader = "Leave Reimbursement NotFound";
//					responseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " was not found under employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " was not found under employee no." + AccountController.GetEmployeeNo();

//					button1ControllerName = "LeaveReimbursement";
//					button1ActionName = "LeaveReimbursementHistory";
//					button1HasParameters = false;
//					button1Parameters = "";
//					button1Name = "Ok";

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
//		[ValidateAntiForgeryToken]
//		public ActionResult EditLeaveReimbursement(LeaveReimbursementModel leaveReimbursementObj)
//		{
//			string leaveReimbursementNo = "";
//			try
//			{
//				leaveReimbursementObj.EmployeeNo = AccountController.GetEmployeeNo();
//				leaveReimbursementObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

//				LoadLeaveApplications();
//				leaveReimbursementObj.LeaveApplicationNos = new SelectList(leaveApplications, "No", "Leave_Type"); 

//				if (ModelState.IsValid)
//				{

//					leaveReimbursementNo = dynamicsNAVSOAPServices.hrManagementWS.GetLeaveReimbursementNo(employeeNo);

//					if (leaveReimbursementNo.Equals(""))
//					{
//						return RedirectToAction("LeaveReimbursementHistory", "LeaveReimbursement");
//					}

//					bool leaveReimbursementModified = dynamicsNAVSOAPServices.hrManagementWS.ModifyLeaveReimbursement(leaveReimbursementNo, leaveReimbursementObj.EmployeeNo, leaveReimbursementObj.ApprovedLeaveApplication, DateTime.Parse(leaveReimbursementObj.ActualReturnDate), leaveReimbursementObj.ReasonForReimbursement);


//					if (!dynamicsNAVSOAPServices.hrManagementWS.CheckLeaveReimbursementApprovalWorkflowEnabled(leaveReimbursementNo))
//					{
//						leaveReimbursementObj.No = leaveReimbursementNo;
//						leaveReimbursementObj.ErrorStatus = true;
//						leaveReimbursementObj.ErrorMessage = "An error was experienced when sending your leave reimbursement no." + leaveReimbursementNo + " for approval. Try again or contact the " + companyName + " ICT division for assistance.";

//						return View(leaveReimbursementObj);
//					}
//					if (dynamicsNAVSOAPServices.hrManagementWS.SendLeaveReimbursementApprovalRequest(leaveReimbursementNo))
//					{
//						responseHeader = "Leave Reimbursement Successful";
//						responseMessage = "Your leave reimbursement was successfully sent for approval. Once approved, you will receive an email containing your leave details.";
//						detailedResponseMessage = "Your leave reimbursement was successfully sent for approval. Once approved, you will receive an email containing your leave details.";

//						button1ControllerName = "LeaveReimbursement";
//						button1ActionName = "LeaveReimbursementHistory";
//						button1HasParameters = false;
//						button1Parameters = "";
//						button1Name = "Ok";

//						button2ControllerName = "";
//						button2ActionName = "";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";

//						return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
//																	button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//																	button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}
//					else
//					{
//						leaveReimbursementObj.No = leaveReimbursementNo;
//						leaveReimbursementObj.ErrorStatus = true;
//						leaveReimbursementObj.ErrorMessage = "An error was experienced when sending your leave application for approval. Try again or contact the " + companyName + " ICT division for assistance.";
//						return View(leaveReimbursementObj);
//					}
//				}
//				else
//				{
//					return View(leaveReimbursementObj);
//				}
//			}
//			catch (Exception ex)
//			{
//				leaveReimbursementObj.No = leaveReimbursementNo;
//				leaveReimbursementObj.ErrorStatus = true;
//				leaveReimbursementObj.ErrorMessage = ex.Message;

//				return View(leaveReimbursementObj);
//			}
//		}
//		#endregion Edit Leave Reimbursement

//		#region View Leave Reimbursement
//		[Authorize]
//		public ActionResult ViewLeaveReimbursement(string LeaveReimbursementNo)
//		{
//			try
//			{
//				if (LeaveReimbursementNo.Equals(""))
//				{
//					return RedirectToAction("LeaveReimbursementHistory", "LeaveReimbursement");
//				}
//				if (dynamicsNAVSOAPServices.hrManagementWS.CheckLeaveReimbursementExists(LeaveReimbursementNo, AccountController.GetEmployeeNo()))
//				{
//					LeaveReimbursementModel leaveReimbursementObj = new LeaveReimbursementModel();

//					var leaveReimbursements = from leaveReimbursementsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRLeaveReimbursement
//											  where leaveReimbursementsQuery.No.Equals(LeaveReimbursementNo)
//											  select leaveReimbursementsQuery;

//					foreach (HRLeaveReimbursement leaveReimbursement in leaveReimbursements)
//					{
//						leaveReimbursementObj.No = leaveReimbursement.No;
//						leaveReimbursementObj.ApprovedLeaveApplication = leaveReimbursement.Leave_Type;
//						leaveReimbursementObj.EmployeeNo = leaveReimbursement.Employee_No;
//						leaveReimbursementObj.EmployeeName = leaveReimbursement.Employee_Name;
//						leaveReimbursementObj.LeaveType = leaveReimbursement.Leave_Type;
//						leaveReimbursementObj.LeaveStartDate = leaveReimbursement.Leave_Start_Date.Value.ToString("dd-MM-yyyy");
//						leaveReimbursementObj.LeaveBalance = leaveReimbursement.Leave_Balance.Value;
//						leaveReimbursementObj.DaysApplied = leaveReimbursement.Leave_Days_Applied.Value;
//						leaveReimbursementObj.DaysApproved = leaveReimbursement.Leave_Days_Approved ?? 0;
//						leaveReimbursementObj.LeaveEndDate = leaveReimbursement.Leave_End_Date.Value.ToString("dd-MM-yyyy");
//						leaveReimbursementObj.LeaveReturnDate = leaveReimbursement.Leave_Return_Date.Value.ToString("dd-MM-yyyy");
//						leaveReimbursementObj.ReasonForLeave = leaveReimbursement.Reason_for_Leave;
//						leaveReimbursementObj.ActualReturnDate = leaveReimbursement.Actual_Return_Date.Value.ToString("dd-MM-yyyy");
//						leaveReimbursementObj.ReasonForReimbursement = leaveReimbursement.Reason_for_Reimbursement;
//						leaveReimbursementObj.SubstituteNo = leaveReimbursement.Substitute_No;
//						leaveReimbursementObj.SubstituteName = leaveReimbursement.Substitute_Name;
//						leaveReimbursementObj.Status = leaveReimbursement.Status;
//					}

//					LoadLeaveApplications();
//					leaveReimbursementObj.LeaveApplicationNos = new SelectList(leaveApplications, "No", "Leave_Type");

//					return View(leaveReimbursementObj);
//				}
//				else
//				{
//					responseHeader = "Leave Reimbursement NotFound";
//					responseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " was not found under employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The leave reimbursement no." + LeaveReimbursementNo + " was not found under employee no." + AccountController.GetEmployeeNo();

//					button1ControllerName = "LeaveReimbursement";
//					button1ActionName = "LeaveReimbursementHistory";
//					button1HasParameters = false;
//					button1Parameters = "";
//					button1Name = "Ok";

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

//		#endregion View Leave Reimbursement

//		#region Leave Reimbursement History
//		[Authorize]
//		public ActionResult LeaveReimbursementHistory()
//		{
//			bool enabled = true;
//			if (!enabled)
//			{
//				responseHeader = "Leave Reimbursement Disabled";
//				responseMessage = "The leave reimbursement service is not available at the moment. Contact the " + companyName + "ICT division for assistance.";
//				detailedResponseMessage = "The leave reimbursement service is not available at the moment. Contact the " + companyName + "ICT division for assistance.";

//				button1ControllerName = "LeaveReimbursement";
//				button1ActionName = "LeaveReimbursementHistory";
//				button1HasParameters = false;
//				button1Parameters = "";
//				button1Name = "Ok";

//				button2ControllerName = "";
//				button2ActionName = "";
//				button2HasParameters = false;
//				button2Parameters = "";
//				button2Name = "";
//				return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
//													  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//													  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//			}

//			List<LeaveReimbursementModel> leaveApplicationList = new List<LeaveReimbursementModel>();

//			try
//			{
//				var leaveReimbursements = (from leaveReimbursementQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRLeaveReimbursement
//										   where leaveReimbursementQuery.Employee_No.Equals(AccountController.GetEmployeeNo())
//										   select leaveReimbursementQuery);

//				foreach (HRLeaveReimbursement leaveReimbursement in leaveReimbursements)
//				{
//					LeaveReimbursementModel leaveReimbursementObj = new LeaveReimbursementModel();
//					leaveReimbursementObj.No = leaveReimbursement.No;
//					leaveReimbursementObj.EmployeeNo = leaveReimbursement.Employee_No;
//					leaveReimbursementObj.EmployeeName = leaveReimbursement.Employee_Name;
//					leaveReimbursementObj.LeaveType = leaveReimbursement.Leave_Type;
//					leaveReimbursementObj.LeaveStartDate = leaveReimbursement.Leave_Start_Date.Value.ToString("dd-MM-yyyy");
//					leaveReimbursementObj.DaysApplied = leaveReimbursement.Leave_Days_Applied ?? 1;
//					leaveReimbursementObj.DaysApproved = leaveReimbursement.Leave_Days_Approved ?? 0;
//					leaveReimbursementObj.LeaveEndDate = leaveReimbursement.Leave_End_Date.Value.ToString("dd-MM-yyyy");
//					leaveReimbursementObj.LeaveReturnDate = leaveReimbursement.Leave_Return_Date.Value.ToString("dd-MM-yyyy");
//					leaveReimbursementObj.ReasonForLeave = leaveReimbursement.Reason_for_Leave;
//					leaveReimbursementObj.SubstituteNo = leaveReimbursement.Substitute_No;
//					leaveReimbursementObj.SubstituteName = leaveReimbursement.Substitute_Name;
//					leaveReimbursementObj.Status = leaveReimbursement.Status;
//					leaveApplicationList.Add(leaveReimbursementObj);
//				}
//				return View(leaveApplicationList);
//			}
//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}
//		#endregion Leave Reimbursement Histoty

//		#region Helper Functions
//		private void LoadLeaveApplications()
//		{
//			leaveApplications = (from leaveApplicationQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRLeaveApplications
//								 where leaveApplicationQuery.Employee_No.Equals(AccountController.GetEmployeeNo()) && leaveApplicationQuery.Status.Equals("Posted")
//								 select leaveApplicationQuery);
//		}

//		[Authorize]
//		public JsonResult GetLeaveApplicationDetails(string ApprovedLeaveApplication)
//		{
//			LeaveReimbursementModel leaveReimbursementObj = new LeaveReimbursementModel();

//			leaveApplications = from leaveApplicationQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRLeaveApplications
//								where leaveApplicationQuery.No.Equals(ApprovedLeaveApplication)
//								select leaveApplicationQuery;

//			foreach (HRLeaveApplications leaveApplication in leaveApplications)
//			{
//				leaveReimbursementObj.LeaveType = leaveApplication.Leave_Type;
//				leaveReimbursementObj.LeavePeriod = leaveApplication.Leave_Period;
//				leaveReimbursementObj.LeaveStartDate = leaveApplication.Leave_Start_Date.Value.ToString("dd-MM-yyyy");
//				leaveReimbursementObj.DaysApplied = (decimal)leaveApplication.Days_Applied;
//				leaveReimbursementObj.DaysApproved = leaveApplication.Days_Approved ?? 0;
//				leaveReimbursementObj.LeaveEndDate = leaveApplication.Leave_End_Date.Value.ToString("dd-MM-yyyy");
//				leaveReimbursementObj.LeaveReturnDate = leaveApplication.Leave_Return_Date.Value.ToString("dd-MM-yyyy");
//				leaveReimbursementObj.SubstituteNo = leaveApplication.Substitute_No;
//				leaveReimbursementObj.SubstituteName = leaveApplication.Substitute_Name;
//				leaveReimbursementObj.ReasonForLeave = leaveApplication.Reason_for_Leave;
//			}

//			return Json(leaveReimbursementObj, JsonRequestBehavior.AllowGet);

//		}
//		#endregion Helper Functions
//	}
//}