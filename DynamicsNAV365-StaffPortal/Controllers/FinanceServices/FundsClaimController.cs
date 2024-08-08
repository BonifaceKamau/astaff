//using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
//using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
//using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
//using DynamicsNAV365_StaffPortal.Models.Finance.FundsClaim;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace DynamicsNAV365_StaffPortal.Controllers.FinanceServices
//{
//	[NoCache]
//    public class FundsClaimController : Controller
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

//		AccountController accountController = new AccountController();
//		string employeeNo = "";

//		public FundsClaimController()
//		{
//			employeeNo = AccountController.GetEmployeeNo();
//		}

//		#region New Funds Claim
//		[Authorize]
//		public ActionResult NewFundsClaim()
//		{
//			string fundsClaimNo = "";
//			try
//			{
//				FundsClaimHeaderModel fundsClaimObj = new FundsClaimHeaderModel();
//				//Check open funds claim request
//				if (dynamicsNAVSOAPServices.fundsClaimManagementWS.CheckOpenFundsClaimExists(employeeNo))
//				{
//					responseHeader = "Open Funds Claim";
//					responseMessage = "An open funds claim page (i.e. Status=Open) exists under your employee no. " + employeeNo + ", finalize on this page before creating a new one.";
//					detailedResponseMessage = "An open funds claim page (i.e. Status=Open) exists under your employee no. " + employeeNo + ", finalize on this page before creating a new one.";

//					button1ControllerName = "FundsClaim";
//					button1ActionName = "FundsClaimHistory";
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
//				//End check open  funds claim request

//				//Create a new funds claim  request
//				fundsClaimNo = dynamicsNAVSOAPServices.fundsClaimManagementWS.CreateFundsClaimHeader(employeeNo);
//				//End create funds claim  request

//				fundsClaimObj.No = fundsClaimNo;
//				fundsClaimObj.EmployeeNo = employeeNo;
//				fundsClaimObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

//				return View(fundsClaimObj);
//			}
//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}

//		[Authorize]
//		[HttpPost]
//		public ActionResult NewFundsClaim(FundsClaimHeaderModel fundsClaimObj)
//		{
//			bool fundsClaimHeaderModified = false;
//			bool approvalWorkflowExist = false;

//			try
//			{

//				if (ModelState.IsValid)
//				{
//					if (dynamicsNAVSOAPServices.fundsClaimManagementWS.CheckFundsClaimExists(fundsClaimObj.No, AccountController.GetEmployeeNo()))
//					{
//						//Check funds claim lines
//						if (!dynamicsNAVSOAPServices.fundsClaimManagementWS.CheckFundsClaimLinesExist(fundsClaimObj.No))
//						{
//							fundsClaimObj.ErrorStatus = true;
//							fundsClaimObj.ErrorMessage = "The funds claim must contain a minimum of one funds claim line before submitting for approval, click add an funds claim line to proceed.";
//							return View(fundsClaimObj);
//						}

//						//Validate imprest lines
//						string fundsClaimLineError = "";
//						fundsClaimLineError = dynamicsNAVSOAPServices.fundsManagementWS.ValidateImprestLines(fundsClaimObj.No);
//						if (!fundsClaimLineError.Equals(""))
//						{
//							fundsClaimObj.ErrorStatus = true;
//							fundsClaimObj.ErrorMessage = fundsClaimLineError;
//							return View(fundsClaimObj);
//						}

//						//Modify funds claim request
//						fundsClaimHeaderModified = dynamicsNAVSOAPServices.fundsClaimManagementWS.ModifyFundsClaimHeader(fundsClaimObj.No, employeeNo,fundsClaimObj.Description);
//						if (!fundsClaimHeaderModified)
//						{
//							fundsClaimObj.ErrorStatus = true;
//							fundsClaimObj.ErrorMessage = "An error was experienced while trying to modify funds claim no." + fundsClaimObj.No + ", check server connection and submit again.";
//							return View(fundsClaimObj);
//						}

//						//Send funds claim for approval
//						approvalWorkflowExist = dynamicsNAVSOAPServices.fundsClaimManagementWS.CheckFundsClaimApprovalWorkflowEnabled(fundsClaimObj.No);
//						if (!approvalWorkflowExist)
//						{
//							fundsClaimObj.ErrorStatus = true;
//							fundsClaimObj.ErrorMessage = "An error was experienced while trying to send an approval request for funds claim no." + fundsClaimObj.No + ", the approval workflow was not found. Contact the " + companyName + " ICT division for assistance.";
//							return View(fundsClaimObj);
//						}

//						if (dynamicsNAVSOAPServices.fundsClaimManagementWS.SendFundsClaimApprovalRequest(fundsClaimObj.No))
//						{
//							responseHeader = "Success";
//							responseMessage = "Funds Claim no." + fundsClaimObj.No + " was successfully submitted for approval.Follow up with your HOD for approval";
//							detailedResponseMessage = "Funds Claim no." + fundsClaimObj.No + " was successfully submitted for approval.Follow up with your HOD for approval";

//							button1ControllerName = "FundsClaim";
//							button1ActionName = "FundsClaimHistory";
//							button1HasParameters = false;
//							button1Parameters = "";
//							button1Name = "Ok";

//							button2ControllerName = "";
//							button2ActionName = "";
//							button2HasParameters = false;
//							button2Parameters = "";
//							button2Name = "";

//							return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
//																		button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//																		button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//						}
//						else
//						{
//							fundsClaimObj.ErrorStatus = true;
//							fundsClaimObj.ErrorMessage = "An error was experienced while trying to send an approval request for funds claim no." + fundsClaimObj.No + ". Contact the " + companyName + " ICT division for assistance.";
//							return View(fundsClaimObj);
//						}
//					}
//					else
//					{
//						responseHeader = "Funds Claim NotFound";
//						responseMessage = "The funds claim no." + fundsClaimObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
//						detailedResponseMessage = "The funds claim no." + fundsClaimObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

//						button1ControllerName = "FundsClaim";
//						button1ActionName = "FundsClaimHistory";
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
//				}
//				else
//				{
//					return View(fundsClaimObj);
//				}
//			}
//			catch (Exception ex)
//			{
//				fundsClaimObj.ErrorStatus = true;
//				fundsClaimObj.ErrorMessage = ex.Message.ToString();
//				return View(fundsClaimObj);
//			}
//		}
//		#endregion End New Funds Claim

//		#region Edit Funds Claim
//		[Authorize]
//		public ActionResult OnBeforeFundsClaimEdit(string FundsClaimNo)
//		{
//			try
//			{
//				if (FundsClaimNo.Equals(""))
//				{
//					return RedirectToAction("FundsClaimHistory", "FundsClaim");
//				}
//				if (dynamicsNAVSOAPServices.fundsClaimManagementWS.CheckFundsClaimExists(FundsClaimNo, AccountController.GetEmployeeNo()))
//				{
//					string fundsClaimStatus = dynamicsNAVSOAPServices.fundsClaimManagementWS.GetFundsClaimStatus(FundsClaimNo);

//					//if funds claim is open
//					if (fundsClaimStatus.Equals("Open"))
//					{
//						return RedirectToAction("EditFundsClaim", "FundsClaim", new { FundsClaimNo = FundsClaimNo });
//					}

//					//if fundsClaimStatus is Pending Approval
//					if (fundsClaimStatus.Equals("Pending Approval"))
//					{
//						responseHeader = "Funds Claim Pending Approval";
//						responseMessage = "The funds claim no." + FundsClaimNo + " is already submitted for approval. Editing not allowed.";
//						detailedResponseMessage = "The funds claim no." + FundsClaimNo + " is already submitted for approval. Editing not allowed.";

//						button1ControllerName = "FundsClaim";
//						button1ActionName = "FundsClaimHistory";
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

//					//if funds claim is released
//					if (fundsClaimStatus.Equals("Approved"))
//					{
//						responseHeader = "Funds Claim Approved";
//						responseMessage = "The funds claim no." + FundsClaimNo + " is already approved. Editing not allowed.";
//						detailedResponseMessage = "The funds claim no." + FundsClaimNo + " is already approved. Editing not allowed.";

//						button1ControllerName = "FundsClaim";
//						button1ActionName = "FundsClaimHistory";
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

//					//if funds claim is rejected
//					if (fundsClaimStatus.Equals("Rejected"))
//					{
//						responseHeader = "Funds Claim Rejected";
//						responseMessage = "The funds claim no." + FundsClaimNo + " was rejected. Editing will reopen the document. Do you want to continue?";
//						detailedResponseMessage = "The funds claim no." + FundsClaimNo + " was rejected. Editing will reopen the document. Do you want to continue?";

//						button1ControllerName = "FundsClaim";
//						button1ActionName = "EditFundsClaim";
//						button1HasParameters = true;
//						button1Parameters = "?FundsClaimNo=" + FundsClaimNo;
//						button1Name = "Yes";

//						button2ControllerName = "FundsClaim";
//						button2ActionName = "FundsClaimHistory";
//						button2HasParameters = false;
//						button2Parameters = "";
//						button2Name = "";

//						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
//															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//					}

//					//if funds Claim Posted
//					if (fundsClaimStatus.Equals("Posted"))
//					{
//						responseHeader = "Funds Claim Posted";
//						responseMessage = "The funds claim no." + FundsClaimNo + " is already posted. Editing not allowed.";
//						detailedResponseMessage = "The funds claim no." + FundsClaimNo + " is already posted. Editing not allowed.";

//						button1ControllerName = "FundsClaim";
//						button1ActionName = "FundsClaimHistory";
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
//					return RedirectToAction("FundsClaimHistory", "FundsClaim");
//				}
//				else
//				{
//					responseHeader = "Funds Claim NotFound";
//					responseMessage = "The funds claim no." + FundsClaimNo + " was not found under employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The funds claim no." + FundsClaimNo + " was not found under employee no." + AccountController.GetEmployeeNo();

//					button1ControllerName = "FundsClaim";
//					button1ActionName = "FundsClaimHistory";
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
//		public ActionResult EditFundsClaim(string FundsClaimNo)
//		{
//			try
//			{
//				if (FundsClaimNo.Equals(""))
//				{
//					return RedirectToAction("FundsClaimHistory", "FundsClaim");
//				}
//				if (dynamicsNAVSOAPServices.fundsClaimManagementWS.CheckFundsClaimExists(FundsClaimNo, AccountController.GetEmployeeNo()))
//				{
//					string fundsClaimStatus = dynamicsNAVSOAPServices.fundsClaimManagementWS.GetFundsClaimStatus(FundsClaimNo);

//					//if funds claim is pending approval, cancel approval request
//					if (fundsClaimStatus.Equals("Pending Approval"))
//					{
//						dynamicsNAVSOAPServices.fundsClaimManagementWS.CancelFundsClaimApprovalRequest(fundsClaimStatus);
//					}
//					//if funds claim is released, reopen and uncommit from budget
//					if (fundsClaimStatus.Equals("Released"))
//					{
//						dynamicsNAVSOAPServices.fundsClaimManagementWS.CancelFundsClaimApprovalRequest(fundsClaimStatus);
//						dynamicsNAVSOAPServices.fundsManagementWS.CancelImprestBudgetCommitment(FundsClaimNo);
//					}
//					//if funds claim is rejected, reopen document
//					if (fundsClaimStatus.Equals("Rejected"))
//					{
//						dynamicsNAVSOAPServices.fundsClaimManagementWS.CancelFundsClaimApprovalRequest(FundsClaimNo);
//					}

//					FundsClaimHeaderModel fundsClaimObj = new FundsClaimHeaderModel();

//					var fundsClaims = from fundsClaimQuery in dynamicsNAVODataServices.dynamicsNAVOData.FundsClaimRequests
//									  where fundsClaimQuery.No.Equals(FundsClaimNo)
//									  select fundsClaimQuery;

//					foreach (FundsClaimRequests fundsClaim in fundsClaims)
//					{
//						fundsClaimObj.No = fundsClaim.No;
//						fundsClaimObj.EmployeeNo = fundsClaim.Payee_No;
//						fundsClaimObj.EmployeeName = fundsClaim.Payee_Name;
//						fundsClaimObj.Amount = (fundsClaim.Amount ?? 0).ToString("N");
//						fundsClaimObj.Description = fundsClaim.Description;
//						fundsClaimObj.Status = fundsClaim.Status;
//					}

//					return View(fundsClaimObj);
//				}
//				else
//				{
//					responseHeader = "Funds Claim NotFound";
//					responseMessage = "The funds claim no." + FundsClaimNo + " was not found under employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The funds claim no." + FundsClaimNo + " was not found under employee no." + AccountController.GetEmployeeNo();

//					button1ControllerName = "FundsClaim";
//					button1ActionName = "FundsClaimHistory";
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
//		public ActionResult EditFundsClaim(FundsClaimHeaderModel fundsClaimObj)
//		{
//			bool fundsClaimHeaderModified = false;
//			bool approvalWorkflowExist = false;

//			try
//			{

//				if (ModelState.IsValid)
//				{
//					if (dynamicsNAVSOAPServices.fundsClaimManagementWS.CheckFundsClaimExists(fundsClaimObj.No, AccountController.GetEmployeeNo()))
//					{
//						//Check funds claim lines
//						if (!dynamicsNAVSOAPServices.fundsClaimManagementWS.CheckFundsClaimLinesExist(fundsClaimObj.No))
//						{
//							fundsClaimObj.ErrorStatus = true;
//							fundsClaimObj.ErrorMessage = "The funds claim must contain a minimum of one funds claim line before submitting for approval, click add an funds claim line to proceed.";
//							return View(fundsClaimObj);
//						}

//						//Validate imprest lines
//						string fundsClaimLineError = "";
//						fundsClaimLineError = dynamicsNAVSOAPServices.fundsManagementWS.ValidateImprestLines(fundsClaimObj.No);
//						if (!fundsClaimLineError.Equals(""))
//						{
//							fundsClaimObj.ErrorStatus = true;
//							fundsClaimObj.ErrorMessage = fundsClaimLineError;
//							return View(fundsClaimObj);
//						}

//						//Modify funds claim request

//						fundsClaimHeaderModified = dynamicsNAVSOAPServices.fundsClaimManagementWS.ModifyFundsClaimHeader(fundsClaimObj.No, employeeNo,fundsClaimObj.Description);
//						if (!fundsClaimHeaderModified)
//						{
//							fundsClaimObj.ErrorStatus = true;
//							fundsClaimObj.ErrorMessage = "An error was experienced while trying to modify funds claim no." + fundsClaimObj.No + ", check server connection and submit again.";
//							return View(fundsClaimObj);
//						}

//						//Send funds claim for approval
//						approvalWorkflowExist = dynamicsNAVSOAPServices.fundsClaimManagementWS.CheckFundsClaimApprovalWorkflowEnabled(fundsClaimObj.No);
//						if (!approvalWorkflowExist)
//						{
//							fundsClaimObj.ErrorStatus = true;
//							fundsClaimObj.ErrorMessage = "An error was experienced while trying to send an approval request for funds claim no." + fundsClaimObj.No + ", the approval workflow was not found. Contact the " + companyName + " ICT division for assistance.";
//							return View(fundsClaimObj);
//						}

//						if (dynamicsNAVSOAPServices.fundsClaimManagementWS.SendFundsClaimApprovalRequest(fundsClaimObj.No))
//						{
//							responseHeader = "Success";
//							responseMessage = "Funds Claim no." + fundsClaimObj.No + " was successfully submitted for approval.Follow up with your HOD for approval";
//							detailedResponseMessage = "Funds Claim no." + fundsClaimObj.No + " was successfully submitted for approval.Follow up with your HOD for approval";

//							button1ControllerName = "FundsClaim";
//							button1ActionName = "FundsClaimHistory";
//							button1HasParameters = false;
//							button1Parameters = "";
//							button1Name = "Ok";

//							button2ControllerName = "";
//							button2ActionName = "";
//							button2HasParameters = false;
//							button2Parameters = "";
//							button2Name = "";

//							return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
//																		button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
//																		button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
//						}
//						else
//						{
//							fundsClaimObj.ErrorStatus = true;
//							fundsClaimObj.ErrorMessage = "An error was experienced while trying to send an approval request for funds claim no." + fundsClaimObj.No + ". Contact the " + companyName + " ICT division for assistance.";
//							return View(fundsClaimObj);
//						}
//					}
//					else
//					{
//						responseHeader = "Funds Claim NotFound";
//						responseMessage = "The funds claim no." + fundsClaimObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
//						detailedResponseMessage = "The funds claim no." + fundsClaimObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

//						button1ControllerName = "FundsClaim";
//						button1ActionName = "FundsClaimHistory";
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
//				}
//				else
//				{
//					return View(fundsClaimObj);
//				}
//			}
//			catch (Exception ex)
//			{
//				fundsClaimObj.ErrorStatus = true;
//				fundsClaimObj.ErrorMessage = ex.Message.ToString();
//				return View(fundsClaimObj);
//			}
//		}

//		#endregion Edit Funds Claim

//		#region View Funds Claim
//		[Authorize]
//		public ActionResult ViewFundsClaim(string FundsClaimNo)
//		{
//			try
//			{
//				if (FundsClaimNo.Equals(""))
//				{
//					return RedirectToAction("FundsClaimHistory", "FundsClaim");
//				}
//				if (dynamicsNAVSOAPServices.fundsClaimManagementWS.CheckFundsClaimExists(FundsClaimNo, AccountController.GetEmployeeNo()))
//				{
//					FundsClaimHeaderModel fundsClaimObj = new FundsClaimHeaderModel();

//					var fundsClaims = from fundsClaimQuery in dynamicsNAVODataServices.dynamicsNAVOData.FundsClaimRequests
//									  where fundsClaimQuery.No.Equals(FundsClaimNo)
//									  select fundsClaimQuery;

//					foreach (FundsClaimRequests fundsClaim in fundsClaims)
//					{
//						fundsClaimObj.No = fundsClaim.No;
//						fundsClaimObj.EmployeeNo = fundsClaim.Payee_No;
//						fundsClaimObj.EmployeeName = fundsClaim.Payee_Name;
//						fundsClaimObj.Amount = (fundsClaim.Amount ?? 0).ToString("N");
//						fundsClaimObj.Description = fundsClaim.Description;
//						fundsClaimObj.Status = fundsClaim.Status;
//					}

//					return View(fundsClaimObj);
//				}
//				else
//				{
//					responseHeader = "Funds Claim NotFound";
//					responseMessage = "The funds claim no." + FundsClaimNo + " was not found under employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The funds claim no." + FundsClaimNo + " was not found under employee no." + AccountController.GetEmployeeNo();

//					button1ControllerName = "FundsClaim";
//					button1ActionName = "FundsClaimHistory";
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
//		#endregion View Funds Claim

//		#region Funds Claim history
//		[Authorize]
//		public ActionResult FundsClaimHistory()
//		{
//			try
//			{
//				List<FundsClaimHeaderModel> fundsClaimList = new List<FundsClaimHeaderModel>();

//				var fundsClaims = from fundsClaimQuery in dynamicsNAVODataServices.dynamicsNAVOData.FundsClaimRequests
//								  where fundsClaimQuery.Payee_No.Equals(employeeNo)
//								  select fundsClaimQuery;

//				foreach (FundsClaimRequests fundsClaim in fundsClaims)
//				{
//					FundsClaimHeaderModel fundsClaimObj = new FundsClaimHeaderModel();
//					fundsClaimObj.No = fundsClaim.No;
//					fundsClaimObj.EmployeeNo = fundsClaim.Payee_No;
//					fundsClaimObj.EmployeeName = fundsClaim.Payee_Name;
//					fundsClaimObj.Amount = (fundsClaim.Amount ?? 0).ToString("N");
//					fundsClaimObj.Description = fundsClaim.Description;
//					fundsClaimObj.Status = fundsClaim.Status;

//					fundsClaimList.Add(fundsClaimObj);
//				}

//				return View(fundsClaimList);
//			}
//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}
//		#endregion Funds Claim history

//		#region Funds Claim Approval
//		[Authorize]
//		public ActionResult FundsClaimApproval(string DocumentNo)
//		{
//			try
//			{
//				if (DocumentNo.Equals(""))
//				{
//					return RedirectToAction("OpenEntries", "Approval");
//				}

//				if (dynamicsNAVSOAPServices.fundsClaimManagementWS.CheckFundsClaimExists(DocumentNo, AccountController.GetEmployeeNo()))
//				{
//					FundsClaimHeaderModel fundsClaimHeaderObj = new FundsClaimHeaderModel();


//					var fundsClaimRequests = from fundsClaimRequestQuery in dynamicsNAVODataServices.dynamicsNAVOData.FundsClaimRequests
//											 where fundsClaimRequestQuery.No.Equals(DocumentNo)
//											 select fundsClaimRequestQuery;

//					foreach (FundsClaimRequests fundsClaimRequest in fundsClaimRequests)
//					{
//						fundsClaimHeaderObj.No = fundsClaimRequest.No;
//						fundsClaimHeaderObj.EmployeeNo = fundsClaimRequest.Payee_No;
//						fundsClaimHeaderObj.EmployeeName = fundsClaimRequest.Payee_Name;
//						fundsClaimHeaderObj.DocumentDate = fundsClaimRequest.Document_Date != null ? fundsClaimRequest.Document_Date.Value.ToShortDateString() : "n/a";
//						fundsClaimHeaderObj.PostingDate = fundsClaimRequest.Posting_Date != null ? fundsClaimRequest.Posting_Date.Value.ToShortDateString() : "n/a";
//						fundsClaimHeaderObj.BankAccountNo = fundsClaimRequest.Bank_Account_No;
//						fundsClaimHeaderObj.BankAccountName = fundsClaimRequest.Bank_Account_Name;
//						fundsClaimHeaderObj.ReferenceNo = fundsClaimRequest.Reference_No;
//						fundsClaimHeaderObj.CurrencyCode = fundsClaimRequest.Currency_Code;
//						fundsClaimHeaderObj.Amount = (fundsClaimRequest.Amount ?? 0).ToString("N");
//						fundsClaimHeaderObj.Description = fundsClaimRequest.Description;
//						fundsClaimHeaderObj.Status = fundsClaimRequest.Status;
//						fundsClaimHeaderObj.GlobalDimension1Code = fundsClaimRequest.Global_Dimension_1_Code;
//						fundsClaimHeaderObj.GlobalDimension2Code = fundsClaimRequest.Global_Dimension_2_Code;
//						fundsClaimHeaderObj.ShortcutDimension3Code = fundsClaimRequest.Shortcut_Dimension_3_Code;
//						fundsClaimHeaderObj.ShortcutDimension4Code = fundsClaimRequest.Shortcut_Dimension_4_Code;
//						fundsClaimHeaderObj.ShortcutDimension5Code = fundsClaimRequest.Shortcut_Dimension_5_Code;
//						fundsClaimHeaderObj.ShortcutDimension6Code = fundsClaimRequest.Shortcut_Dimension_6_Code;
//						fundsClaimHeaderObj.ShortcutDimension7Code = fundsClaimRequest.Shortcut_Dimension_7_Code;
//						fundsClaimHeaderObj.ShortcutDimension8Code = fundsClaimRequest.Shortcut_Dimension_8_Code;
//					}

//					return View(fundsClaimHeaderObj);
//				}
//				else
//				{
//					responseHeader = "Funds Claim Not Found";
//					responseMessage = "The funds claim no." + DocumentNo + " was not found under employee no." + AccountController.GetEmployeeNo();
//					detailedResponseMessage = "The funds claim no." + DocumentNo + " was not found under employee no." + AccountController.GetEmployeeNo();

//					button1ControllerName = "Approval";
//					button1ActionName = "OpenEntries";
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
//		public ActionResult FundsClaimApproval(FundsClaimHeaderModel fundsClaimHeaderObj, string Command)
//		{
//			try
//			{
//				if (fundsClaimHeaderObj.No.Equals(""))
//				{
//					return RedirectToAction("OpenEntries", "Approval");
//				}
//				if (Command == "Approve")
//				{
//					if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveImprestRequest(employeeNo, fundsClaimHeaderObj.No))
//					{
//						responseHeader = "Success";
//						responseMessage = "Funds Claim Request no." + fundsClaimHeaderObj.No + " was successfully approved.";
//						detailedResponseMessage = "Funds Claim Request no." + fundsClaimHeaderObj.No + " was successfully approved.";

//						button1ControllerName = "Approval";
//						button1ActionName = "OpenEntries";
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
//						fundsClaimHeaderObj.ErrorStatus = true;
//						fundsClaimHeaderObj.ErrorMessage = "Unable to process the funds claim request approve action. Contact the " + companyName + " for assistance.";
//						return View(fundsClaimHeaderObj);
//					}
//				}
//				else if (Command == "Reject")
//				{
//					if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectImprestRequest(employeeNo, fundsClaimHeaderObj.No, fundsClaimHeaderObj.Description))
//					{
//						responseHeader = "Success";
//						responseMessage = "Funds claim request no." + fundsClaimHeaderObj.No + " was successfully rejected.";
//						detailedResponseMessage = "Funs claim request no." + fundsClaimHeaderObj.No + " was successfully rejected.";

//						button1ControllerName = "Approval";
//						button1ActionName = "OpenEntries";
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
//						fundsClaimHeaderObj.ErrorStatus = true;
//						fundsClaimHeaderObj.ErrorMessage = "Unable to process the funds claim request reject action. Contact the " + companyName + " for assistance.";
//						return View(fundsClaimHeaderObj);
//					}
//				}
//				else
//				{
//					fundsClaimHeaderObj.ErrorStatus = true;
//					fundsClaimHeaderObj.ErrorMessage = "Unable to process the approve/reject action. Contact the " + companyName + " for assistance.";
//					return View(fundsClaimHeaderObj);
//				}
//			}
//			catch (Exception ex)
//			{
//				return errorResponse.ApplicationExceptionError(ex);
//			}
//		}
//		#endregion Funds Claim Approval

//		#region Funds Claim Line
//		[ChildActionOnly]
//		[Authorize]
//		public ActionResult _FundsClaimLine(string DocumentNo)
//		{
//			FundsClaimLineModel FundsClaimLineObj = new FundsClaimLineModel();
//			LoadFundsClaimTransactionCodes();
//			FundsClaimLineObj.ImprestCodes = new SelectList(fundsClaimTransactionCode, "Transaction_Code", "Description");
//			return PartialView(FundsClaimLineObj);
//		}

//		[ChildActionOnly]
//		[Authorize]
//		public ActionResult _ViewFundsClaimLine(string DocumentNo)
//		{
//			FundsClaimLineModel FundsClaimLineObj = new FundsClaimLineModel();
//			LoadFundsClaimTransactionCodes();
//			FundsClaimLineObj.ImprestCodes = new SelectList(fundsClaimTransactionCode, "Transaction_Code", "Description");
//			return PartialView(FundsClaimLineObj);
//		}

//		[Authorize]
//		public JsonResult GetFundsClaimLines(string DocumentNo)
//		{
//			List<FundsClaimLineModel> fundsClaimLinesList = new List<FundsClaimLineModel>();

//			var fundsClaimLines = from fundsClaimQuery in dynamicsNAVODataServices.dynamicsNAVOData.FundsClaimLines
//								  where fundsClaimQuery.Document_No.Equals(DocumentNo)
//								  select fundsClaimQuery;

//			foreach (FundsClaimLines fundsClaimLine in fundsClaimLines)
//			{
//				FundsClaimLineModel FundsClaimLineObj = new FundsClaimLineModel();
//				FundsClaimLineObj.LineNo = fundsClaimLine.Line_No;
//				FundsClaimLineObj.DocumentNo = fundsClaimLine.Document_No;
//				FundsClaimLineObj.ImprestCode = fundsClaimLine.Funds_Claim_Code;
//				FundsClaimLineObj.LineDescription = fundsClaimLine.Description;
//				FundsClaimLineObj.LineAmount = fundsClaimLine.Amount ?? 0;

//				fundsClaimLinesList.Add(FundsClaimLineObj);
//			}

//			return Json(fundsClaimLinesList, JsonRequestBehavior.AllowGet);
//		}

//		[Authorize]
//		public JsonResult GetFundsClaimLine(int LineNo, string DocumentNo)
//		{
//			FundsClaimLineModel FundsClaimLineObj = new FundsClaimLineModel();

//			var fundsClaimLines = from fundsClaimQuery in dynamicsNAVODataServices.dynamicsNAVOData.FundsClaimLines
//								  where fundsClaimQuery.Line_No.Equals(LineNo) && fundsClaimQuery.Document_No.Equals(DocumentNo)
//								  select fundsClaimQuery;

//			foreach (FundsClaimLines fundsClaimLine in fundsClaimLines)
//			{
//				FundsClaimLineObj.LineNo = fundsClaimLine.Line_No;
//				FundsClaimLineObj.DocumentNo = fundsClaimLine.Document_No;
//				FundsClaimLineObj.ImprestCode = fundsClaimLine.Funds_Claim_Code;
//				FundsClaimLineObj.LineDescription = fundsClaimLine.Description;
//				FundsClaimLineObj.LineAmount = fundsClaimLine.Amount ?? 0;
//			}

//			return Json(FundsClaimLineObj, JsonRequestBehavior.AllowGet);
//		}

//		[Authorize]
//		public JsonResult CreateFundsClaimLine(FundsClaimLineModel FundClaimLineObj)
//		{
//			bool FundsClaimLineCreated = false;

//			FundClaimLineObj.FromCity = FundClaimLineObj.FromCity != null ? FundClaimLineObj.FromCity : "";
//			FundClaimLineObj.ToCity = FundClaimLineObj.ToCity != null ? FundClaimLineObj.ToCity : "";

//			FundsClaimLineCreated = dynamicsNAVSOAPServices.fundsClaimManagementWS.CreateFundsClaimLine(FundClaimLineObj.DocumentNo, FundClaimLineObj.ImprestCode, FundClaimLineObj.LineAmount, FundClaimLineObj.FromCity, FundClaimLineObj.ToCity, FundClaimLineObj.LineDescription);

//			return Json(new { FundClaimsLineCreated = FundsClaimLineCreated }, JsonRequestBehavior.AllowGet);
//		}

//		[Authorize]
//		public JsonResult ModifyFundsClaimLine(FundsClaimLineModel FundClaimLineObj)
//		{
//			bool FundsClaimLineModified = false;

//			FundClaimLineObj.FromCity = FundClaimLineObj.FromCity != null ? FundClaimLineObj.FromCity : "";
//			FundClaimLineObj.ToCity = FundClaimLineObj.ToCity != null ? FundClaimLineObj.ToCity : "";

//			FundsClaimLineModified = dynamicsNAVSOAPServices.fundsClaimManagementWS.ModifyFundsClaimLine(FundClaimLineObj.LineNo, FundClaimLineObj.DocumentNo, FundClaimLineObj.ImprestCode, FundClaimLineObj.LineAmount, FundClaimLineObj.FromCity, FundClaimLineObj.ToCity, FundClaimLineObj.LineDescription);

//			return Json(new { FundClaimsLineModified = FundsClaimLineModified }, JsonRequestBehavior.AllowGet);
//		}

//		[Authorize]
//		public JsonResult DeleteFundsClaimLine(int LineNo, string DocumentNo)
//		{
//			bool FundsClaimLineDeleted = false;

//			FundsClaimLineDeleted = dynamicsNAVSOAPServices.fundsClaimManagementWS.DeleteFundsClaimLine(LineNo, DocumentNo);

//			return Json(new { FundClaimsLineDeleted = FundsClaimLineDeleted }, JsonRequestBehavior.AllowGet);
//		}
//		#endregion Funds Claim Line

//		//#region Attachments

//		//[Authorize]
//		//public JsonResult LoadAttachedDocuments(string DocumentNo)
//		//{
//		//	List<DocumentMgmtModel> portalDocumentList = new List<DocumentMgmtModel>();

//		//	var UploadedDocuments = from uploadedDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
//		//							where uploadedDocumentQuery.DocumentNo.Equals(DocumentNo)
//		//							select uploadedDocumentQuery;

//		//	foreach (PortalDocuments uploadedDocument in UploadedDocuments)
//		//	{
//		//		DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();
//		//		documentManagementoBJ.DocumentNo = uploadedDocument.DocumentNo;
//		//		documentManagementoBJ.DocumentCode = uploadedDocument.Document_Code;
//		//		documentManagementoBJ.DocumentDescription = uploadedDocument.Document_Description;
//		//		documentManagementoBJ.DocumentAttached = uploadedDocument.Document_Attached ?? false;
//		//		documentManagementoBJ.FileName = uploadedDocument.FileName;
//		//		documentManagementoBJ.SharePointURL = uploadedDocument.SharePoint_URL;
//		//		portalDocumentList.Add(documentManagementoBJ);
//		//	}

//		//	return Json(portalDocumentList, JsonRequestBehavior.AllowGet);
//		//}

//		//[Authorize]
//		//[HttpPost]
//		//public JsonResult UploadDocument(string DocumentNo, string DocumentCode, string DocumentDescription)
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
//		//				dynamicsNAVSOAPServices.documentMgmt.ModifySystemFileURL(DocumentNo, DocumentCode, path);
					
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

//		//[Authorize]
//		//public JsonResult DeleteRequiredDocuments(string DocumentNo)
//		//{
//		//	bool uploadedDocumentDeleted = false;

//		////	uploadedDocumentDeleted = dynamicsNAVSOAPServices.procurementManagementWS.DeleteProcurementUploadedDocument(DocumentNo);

//		//	return Json(new { UploadedDocumentDeleted = uploadedDocumentDeleted }, JsonRequestBehavior.AllowGet);
//		//}

//		//[ChildActionOnly]
//		//[Authorize]
//		//public ActionResult _UploadDocumentLine(string DocumentNo)
//		//{
//		//	DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

//		//	var UploadedDocuments = from uploadedDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
//		//							where uploadedDocumentQuery.DocumentNo.Equals(DocumentNo)
//		//							select uploadedDocumentQuery;

//		//	foreach (PortalDocuments uploadedDocument in UploadedDocuments)
//		//	{
//		//		documentManagementoBJ.DocumentNo = uploadedDocument.DocumentNo;
//		//		documentManagementoBJ.DocumentCode = uploadedDocument.Document_Code;
//		//		documentManagementoBJ.DocumentDescription = uploadedDocument.Document_Description;
//		//		documentManagementoBJ.DocumentAttached = uploadedDocument.Document_Attached ?? false;
//		//		documentManagementoBJ.LocalURL = uploadedDocument.Local_File_URL;
//		//		documentManagementoBJ.SharePointURL = uploadedDocument.SharePoint_URL;
//		//	}
//		//	return PartialView(documentManagementoBJ);
//		//}

//		//[ChildActionOnly]
//		//[Authorize]
//		//public ActionResult _ViewUplodedDocumentLine(string DocumentNo)
//		//{
//		//	DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

//		//	var UploadedDocuments = from uploadedDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
//		//							where uploadedDocumentQuery.DocumentNo.Equals(DocumentNo)
//		//							select uploadedDocumentQuery;

//		//	foreach (PortalDocuments uploadedDocument in UploadedDocuments)
//		//	{
//		//		documentManagementoBJ.DocumentNo = uploadedDocument.DocumentNo;
//		//		documentManagementoBJ.DocumentCode = uploadedDocument.Document_Code;
//		//		documentManagementoBJ.DocumentDescription = uploadedDocument.Document_Description;
//		//		documentManagementoBJ.DocumentAttached = uploadedDocument.Document_Attached ?? false;
//		//		documentManagementoBJ.LocalURL = uploadedDocument.Local_File_URL;
//		//		documentManagementoBJ.SharePointURL = uploadedDocument.SharePoint_URL;
//		//	}
//		//	return PartialView(documentManagementoBJ);
//		//}

//		//[Authorize]
//		//public ActionResult GetAttachedDocument(string DocumentNo, string DocumentCode)
//		//{
//		//	try
//		//	{
//		//		DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

//		//		var UploadedDocuments = from uploadedDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
//		//								where uploadedDocumentQuery.DocumentNo.Equals(DocumentNo) && uploadedDocumentQuery.Document_Code.Equals(DocumentCode)
//		//								select uploadedDocumentQuery;

//		//		foreach (PortalDocuments uploadedDocument in UploadedDocuments)
//		//		{
//		//			documentManagementoBJ.DocumentNo = uploadedDocument.DocumentNo;
//		//			documentManagementoBJ.DocumentCode = uploadedDocument.Document_Code;
//		//			documentManagementoBJ.DocumentDescription = uploadedDocument.Document_Description;
//		//			documentManagementoBJ.DocumentAttached = uploadedDocument.Document_Attached ?? false;
//		//			documentManagementoBJ.LocalURL = uploadedDocument.Local_File_URL;
//		//			documentManagementoBJ.SharePointURL = uploadedDocument.SharePoint_URL;
//		//		}
//		//		return Json(documentManagementoBJ, JsonRequestBehavior.AllowGet);
//		//	}
//		//	catch (Exception ex)
//		//	{
//		//		return errorResponse.ApplicationExceptionError(ex);
//		//	}
//		//}

//		//#endregion Attachments 

//		#region Helper Functions
//		public JsonResult GetFundsClaimAmount(string DocumentNo)
//		{
//			decimal fundsClaimAmount = 0;
//			fundsClaimAmount = dynamicsNAVSOAPServices.fundsClaimManagementWS.GetFundsClaimAmount(DocumentNo);
//			return Json(new { Amount = fundsClaimAmount }, JsonRequestBehavior.AllowGet);
//		}
//		private void LoadFundsClaimTransactionCodes()
//		{
//			fundsClaimTransactionCode = from fundsTransactionQuery in dynamicsNAVODataServices.dynamicsNAVOData.FundsClaimTransactionCodes
//										select fundsTransactionQuery;
//		}

//		#endregion Helper Functions
//	}
//}