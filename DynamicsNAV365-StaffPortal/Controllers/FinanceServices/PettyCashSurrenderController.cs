using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using DynamicsNAV365_StaffPortal.Models.Finance.ImprestSurrender;
using DynamicsNAV365_StaffPortal.Models.Finance.PettyCashSurrender;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.FinanceServices
{
    public class PettyCashSurrenderController : Controller
    {
		static string companyURL = "";

		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
		DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

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

		IQueryable<Currencies> currencyCodes = null;
		IQueryable<DimensionValues> globalDimension1Codes = null;
		IQueryable<DimensionValues> globalDimension2Codes = null;
		IQueryable<DimensionValues> shortcutDimension3Codes = null;
		IQueryable<DimensionValues> shortcutDimension4Codes = null;
		IQueryable<DimensionValues> shortcutDimension5Codes = null;
		IQueryable<DimensionValues> shortcutDimension6Codes = null;
		IQueryable<DimensionValues> shortcutDimension7Codes = null;
		IQueryable<DimensionValues> shortcutDimension8Codes = null;

		List<FundsTransactionModel> pettyCashSurrenderCodes = null; 
		List<PettyCashTransactionTypes> pettyCashTransactionCodes = null;
		List<ReceiptList> receiptLineList = null;
        List<UnsurrenderedPettyCashCodes> unsurrenderedPettyCashCodes = null;

        AccountController accountController = new AccountController();
		string employeeNo = "";

		public PettyCashSurrenderController()
        {
			employeeNo = AccountController.GetEmployeeNo();
        }

		#region New PettyCash  Surrender

		[Authorize]
		public ActionResult NewPettyCashSurrender() 
		{
			string imprestNo = "";
			string openPettyCashNo = "";
			try
			{
				PettyCashSurrenderHeaderModel PettyCashRequestObj = new PettyCashSurrenderHeaderModel();
				//Check open imprest request
				openPettyCashNo = dynamicsNAVSOAPServices.fundsManagementWS.CheckOpenPettyCashSurrenderExists(employeeNo);
				if (!openPettyCashNo.Equals(""))
				{
					responseHeader = "Open PettyCash Surrender";
					responseMessage = "An open pettycash surrender No. (" + openPettyCashNo + ") exists under your account. You have to edit this document and submit for approval.";
					detailedResponseMessage = "An open pettycash surrender No. (" + openPettyCashNo + ") exists under your account. You have to edit this document and submit for approval.";

					button1ControllerName = "PettyCashSurrender";
					button1ActionName = "PettyCashSurrenderHistory";
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
				//End check open imprest request

				//Create a new imprest request
				imprestNo = dynamicsNAVSOAPServices.fundsManagementWS.CreatePettyCashSurrenderHeader(employeeNo);
				//End create imprest request

				PettyCashRequestObj.No = imprestNo;
				PettyCashRequestObj.EmployeeNo = employeeNo;
				PettyCashRequestObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);
				PettyCashRequestObj.GlobalDimension1Code = "";

				//LoadCurrencies();
				//LoadPettyCashSurrenderDimensions(PettyCashRequestObj.GlobalDimension1Code);

				LoadPettyshBankAccounts();
				PettyCashRequestObj.BankAccountNos = new SelectList(pettyCashSurrenderCodes, "TransactionCode", "TransactionDescription");

				return View(PettyCashRequestObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		public ActionResult NewPettyCashSurrender(PettyCashSurrenderHeaderModel PettyCashRequestObj)
		{
			bool imprestRequestModified = false;
			bool approvalWorkflowExist = false;

			try
			{
                //LoadCurrencies();
                //LoadPettyCashSurrenderDimensions(PettyCashRequestObj.GlobalDimension1Code);
                LoadPettyCashSurrenderCodes();
                PettyCashRequestObj.UnsurrenderedPettyCashs = new SelectList(unsurrenderedPettyCashCodes, "No", "Description");


                LoadPettyshBankAccounts();
				PettyCashRequestObj.BankAccountNos = new SelectList(pettyCashSurrenderCodes, "No", "Name");

				if (ModelState.IsValid)
				{
					if (dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashSurrenderExists(PettyCashRequestObj.No, AccountController.GetEmployeeNo()))
					{
						//Check imprest lines
						if (!dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashLinesExist(PettyCashRequestObj.No))
						{
							PettyCashRequestObj.ErrorStatus = true;
							PettyCashRequestObj.ErrorMessage = "PettyCash surrender lines missing, the pettycash surrender must contain a minimum of one pettycash surrender line, add a line to continue.";
							return View(PettyCashRequestObj);
						}

						//Modify petty cash request
						imprestRequestModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyPettyCashSurrenderHeader(PettyCashRequestObj.No, PettyCashRequestObj.BankAccountNo ?? "", PettyCashRequestObj.Description); 
						if (!imprestRequestModified)
						{
							PettyCashRequestObj.ErrorStatus = true;
							PettyCashRequestObj.ErrorMessage = "An error was experienced while trying to modify petty cash no." + PettyCashRequestObj.No + ", the server might be offline, try again after a while.";
							return View(PettyCashRequestObj);
						}

						//Send imprest for approval
						approvalWorkflowExist = dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashSurrenderApprovalWorkflowEnabled(PettyCashRequestObj.No);
						if (!approvalWorkflowExist)
						{
							PettyCashRequestObj.ErrorStatus = true;
							PettyCashRequestObj.ErrorMessage = "An error was experienced while trying to send an approval request for petty cash no." + PettyCashRequestObj.No + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
							return View(PettyCashRequestObj);
						}

						if (dynamicsNAVSOAPServices.fundsManagementWS.SendPettyCashSurrenderApprovalRequest(PettyCashRequestObj.No)) 
						{
							responseHeader = "Success";
							responseMessage = "PettyCash surrender no." + PettyCashRequestObj.No + " was successfully sent for approval. You shall get an email notification once approved.";
							detailedResponseMessage = "PettyCash surrender no." + PettyCashRequestObj.No + " was successfully sent for approval. You shall get an email notification once approved.";

							button1ControllerName = "PettyCashSurrender";
							button1ActionName = "PettyCashSurrenderHistory";
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
							PettyCashRequestObj.ErrorStatus = true;
							PettyCashRequestObj.ErrorMessage = "An error was experienced while trying to send an approval request for imprest no." + PettyCashRequestObj.No + ". " + ServiceConnection.contactICTDepartment + "";
							return View(PettyCashRequestObj);
						}
					}
					else
					{
						responseHeader = "PettyCash Surrender NotFound";
						responseMessage = "The PettyCash surrender no." + PettyCashRequestObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
						detailedResponseMessage = "The PettyCash  surrender no." + PettyCashRequestObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

						button1ControllerName = "PettyCashSurrender";
						button1ActionName = "PettyCashSurrenderHistory";
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
				else
				{
					return View(PettyCashRequestObj);
				}
			}
			catch (Exception ex)
			{
				PettyCashRequestObj.ErrorStatus = true;
				PettyCashRequestObj.ErrorMessage = ex.Message.ToString();
				return View(PettyCashRequestObj);
			}
		}

		#endregion New PettyCash Surrender

		#region Edit PettyCash Surrender 

		[Authorize]
		public ActionResult OnBeforeEdit(string PettyCashSurrenderNo) 
		{
			try
			{
				if (PettyCashSurrenderNo.Equals(""))
				{
					return RedirectToAction("PettyCasSurrenderHistory", "PettyCashSurrender"); 
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashSurrenderExists(PettyCashSurrenderNo, AccountController.GetEmployeeNo()))
				{
					string pettyCashSurrenderStatus = GetPettyCashSurrenderStatus(PettyCashSurrenderNo); 
				
					//if petty cash surrender is open
					if (pettyCashSurrenderStatus.Equals("Open"))
					{
						return RedirectToAction("EditPettyCashSurrender", "PettyCashSurrender", new { PettyCashSurrenderNo = PettyCashSurrenderNo });
					}

					//if petty cash surrender is pending approval
					if (pettyCashSurrenderStatus.Equals("Pending Approval"))
					{
						responseHeader = "PettyCash Surrender Pending Approval";
						responseMessage = "The pettycash surrender no." + PettyCashSurrenderNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
						detailedResponseMessage = "The pettycash surrender no." + PettyCashSurrenderNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";

						button1ControllerName = "PettyCashSurrender";
						button1ActionName = "EditPettyCashSurrender";
						button1HasParameters = true;
						button1Parameters = "?PettyCashSurrenderNo=" + PettyCashSurrenderNo;
						button1Name = "Yes";

						button2ControllerName = "PettyCashSurrender";
						button2ActionName = "PettyCashSurrenderHistory"; 
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}

					//if petty cash surrender is released
					if (pettyCashSurrenderStatus.Equals("Released"))
					{
						responseHeader = "PettyCash Surrender Approved";
						responseMessage = "The pettycash surrender no." + PettyCashSurrenderNo + " is already approved. Editing not allowed.";
						detailedResponseMessage = "The pettycash surrender no." + PettyCashSurrenderNo  + " is already approved. Editing not allowed.";

						button1ControllerName = "PettyCashSurrender";
						button1ActionName = "PettyCashSurrenderHistory";
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

					//if petty cash surrender is rejected
					if (pettyCashSurrenderStatus.Equals("Rejected"))
					{
						responseHeader = "PettyCash Surrender Rejected";
						responseMessage = "The pettycash surrender no." + PettyCashSurrenderNo + " was rejected. Editing will reopen the document. Do you want to continue?";
						detailedResponseMessage = "The pettycash surrender no." + PettyCashSurrenderNo  + " was rejected. Editing will reopen the document. Do you want to continue?";

						button1ControllerName = "PettyCashSurrender";
						button1ActionName = "EditPettyCashSurrender";
						button1HasParameters = true;
						button1Parameters = "?PettyCashSurrenderNo=" + PettyCashSurrenderNo;
						button1Name = "Yes";

						button2ControllerName = "PettyCashSurrender";
						button2ActionName = "PettyCashSurrenderHistory";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}

					//if petty cash surrender is posted/reversed
					if (pettyCashSurrenderStatus.Equals("Posted") || pettyCashSurrenderStatus.Equals("Reversed"))
					{
						responseHeader = "PettyCash Surrender Posted";
						responseMessage = "The pettycash surrender no." + PettyCashSurrenderNo + " is already posted. Editing not allowed.";
						detailedResponseMessage = "The pettycash surrender no." + PettyCashSurrenderNo  + " is already posted. Editing not allowed.";

						button1ControllerName = "PettyCashSurrender";
						button1ActionName = "PettyCashSurrenderHistory";
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
					return RedirectToAction("PettyCashSurrenderHistory", "PettyCashSurrender");
				}
				else
				{
					responseHeader = "PettyCash Surrender NotFound";
					responseMessage = "The pettycash surrender  no." + PettyCashSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The pettycash surrender no." + PettyCashSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "PettyCashSurrender";
					button1ActionName = "PettyCashSurrenderHistory";
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
		public ActionResult EditPettyCashSurrender(string PettyCashSurrenderNo)  
		{
			try
			{
				if (PettyCashSurrenderNo.Equals(""))  
				{
					return RedirectToAction("PettyCashSurrenderHistory", "PettyCashSurrender");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashSurrenderExists(PettyCashSurrenderNo, AccountController.GetEmployeeNo()))
				{
					string imprestStatus = dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashSurrenderStatus(PettyCashSurrenderNo);

					//if petty cash surrender is pending approval, cancel approval request
					if (imprestStatus.Equals("Pending Approval"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.CancelPettyCashSurrenderApprovalRequest(PettyCashSurrenderNo);
					}
					//if petty cash surrender is released, reopen and uncommit from budget
					if (imprestStatus.Equals("Released"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.ReopenImprestRequest(PettyCashSurrenderNo);
						dynamicsNAVSOAPServices.fundsManagementWS.CancelImprestBudgetCommitment(PettyCashSurrenderNo);
					}
					//if imprest is rejected, reopen document
					if (imprestStatus.Equals("Rejected"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.CancelPettyCashSurrenderApprovalRequest(PettyCashSurrenderNo);
					}

					PettyCashSurrenderHeaderModel pettyCashSurrenderObj = new PettyCashSurrenderHeaderModel(); 

					dynamic pettyCashSurrenders = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashSurrender(PettyCashSurrenderNo, ""));

					foreach (var pettyCashSurrender in pettyCashSurrenders)
					{
						pettyCashSurrenderObj.No = pettyCashSurrender.No;
						pettyCashSurrenderObj.DocumentDate = pettyCashSurrender.DocumentDate;
						pettyCashSurrenderObj.PostingDate = pettyCashSurrender.PostingDate;
						pettyCashSurrenderObj.BankAccountNo = pettyCashSurrender.BankAccountNo;
						pettyCashSurrenderObj.BankAccountName = pettyCashSurrender.BankAccountName;
						pettyCashSurrenderObj.ReferenceNo = pettyCashSurrender.ReferenceNo;
						pettyCashSurrenderObj.EmployeeNo = pettyCashSurrender.EmployeeNo;
						pettyCashSurrenderObj.EmployeeName = pettyCashSurrender.EmployeeName;
						pettyCashSurrenderObj.CurrencyCode = pettyCashSurrender.ImprestType;
						pettyCashSurrenderObj.Description = pettyCashSurrender.Description;
						pettyCashSurrenderObj.GlobalDimension1Code = pettyCashSurrender.GlobalDimension1Code;
						pettyCashSurrenderObj.GlobalDimension2Code = pettyCashSurrender.GlobalDimension2Code;
						pettyCashSurrenderObj.ShortcutDimension3Code = pettyCashSurrender.ShortcutDimension3Code;
						pettyCashSurrenderObj.ShortcutDimension4Code = pettyCashSurrender.ShortcutDimension4Code;
						pettyCashSurrenderObj.ShortcutDimension5Code = pettyCashSurrender.ShortcutDimension5Code;
						pettyCashSurrenderObj.ShortcutDimension6Code = pettyCashSurrender.ShortcutDimension6Code;
						pettyCashSurrenderObj.ShortcutDimension7Code = pettyCashSurrender.ShortcutDimension7Code;
						pettyCashSurrenderObj.ShortcutDimension8Code = pettyCashSurrender.ShortcutDimension8Code;
						pettyCashSurrenderObj.Amount = pettyCashSurrender.Amount;
						pettyCashSurrenderObj.Status = pettyCashSurrender.Status;
                        pettyCashSurrenderObj.PettyCashType = pettyCashSurrender.PettyCashRequestType;
                    }

                    //LoadCurrencies();
                    //LoadPettyCashSurrenderDimensions(pettyCashSurrenderObj.GlobalDimension1Code);
                    LoadPettyCashSurrenderCodes();
                    pettyCashSurrenderObj.UnsurrenderedPettyCashs = new SelectList(unsurrenderedPettyCashCodes, "No", "Description");

                    LoadPettyshBankAccounts();

					pettyCashSurrenderObj.BankAccountNos = new SelectList(pettyCashSurrenderCodes, "TransactionCode", "TransactionDescription");
					//pettyCashSurrenderObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
					//pettyCashSurrenderObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
					//pettyCashSurrenderObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
					//pettyCashSurrenderObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
					//pettyCashSurrenderObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
					//pettyCashSurrenderObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
					//pettyCashSurrenderObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
					//pettyCashSurrenderObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
					//pettyCashSurrenderObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

					return View(pettyCashSurrenderObj);
				}
				else
				{
					responseHeader = "PettyCash Surrender NotFound";
					responseMessage = "The pettycash surrender no." + PettyCashSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The pettycash surrender no." + PettyCashSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "PettyCashSurrender";
					button1ActionName = "PettyCashSurrenderHistory";
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

        private void LoadPettyCashSurrenderCodes()
        {
            unsurrenderedPettyCashCodes = new List<UnsurrenderedPettyCashCodes>();

            dynamic unsurrenderImprestList = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetUnsurrenderedPettyCash(employeeNo));
            foreach (var unsurrenderImprest in unsurrenderImprestList)
            {
                UnsurrenderedPettyCashCodes unsurrenderedImprestObj = new UnsurrenderedPettyCashCodes();
                unsurrenderedImprestObj.No = unsurrenderImprest.No;
                unsurrenderedImprestObj.Description = unsurrenderImprest.Description;

                unsurrenderedPettyCashCodes.Add(unsurrenderedImprestObj);
            }
        }

        [Authorize]
		[HttpPost]
		public async Task<ActionResult> EditPettyCashSurrender(PettyCashSurrenderHeaderModel pettyCashSurrenderObj, string Command)
		{
			bool imprestRequestModified = false;
			bool approvalWorkflowExist = false;
			try
			{

                LoadPettyCashSurrenderCodes();
                pettyCashSurrenderObj.UnsurrenderedPettyCashs = new SelectList(unsurrenderedPettyCashCodes, "No", "Description");

                LoadPettyshBankAccounts();

				pettyCashSurrenderObj.BankAccountNos = new SelectList(pettyCashSurrenderCodes, "TransactionCode", "TransactionDescription");

				if (Command.Equals("Submit For Approval"))
				{
					if (ModelState.IsValid)
					{
						if (dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashSurrenderExists(pettyCashSurrenderObj.No, AccountController.GetEmployeeNo()))
						{
							//Check petty cash surrender lines
							if (!dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashLinesExist(pettyCashSurrenderObj.No))
							{
								pettyCashSurrenderObj.ErrorStatus = true;
								pettyCashSurrenderObj.ErrorMessage = "PettyCash surrender lines missing, the pettycash surrender must contain a minimum of one pettycash surrender line, add an petty cash line to continue.";
								return View(pettyCashSurrenderObj);
							}

							imprestRequestModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyPettyCashSurrenderHeader(pettyCashSurrenderObj.No,pettyCashSurrenderObj.BankAccountNo ?? "", pettyCashSurrenderObj.Description);

							if (!imprestRequestModified)
							{
								pettyCashSurrenderObj.ErrorStatus = true;
								pettyCashSurrenderObj.ErrorMessage = "An error was experienced while trying to modify pettycash surrender no." + pettyCashSurrenderObj.No + ", the server might be offline, try again after a while.";
								return View(pettyCashSurrenderObj);
							}

							//Send imprest for approval
							approvalWorkflowExist = dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashSurrenderApprovalWorkflowEnabled(pettyCashSurrenderObj.No);
							if (!approvalWorkflowExist)
							{
								pettyCashSurrenderObj.ErrorStatus = true;
								pettyCashSurrenderObj.ErrorMessage = "An error was experienced while trying to send an approval request for pettycash surrender no." + pettyCashSurrenderObj.No + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
								return View(pettyCashSurrenderObj);
							}

							if (dynamicsNAVSOAPServices.fundsManagementWS.SendPettyCashSurrenderApprovalRequest(pettyCashSurrenderObj.No))
							{
								responseHeader = "Success";
								responseMessage = "Pettycash surrender no." + pettyCashSurrenderObj.No + " was successfully sent for approval. You will get an email notification approved.";
								detailedResponseMessage = "PettyCash surrender no." + pettyCashSurrenderObj.No + " was successfully sent for approval.  You will get an email notification approved.";

								button1ControllerName = "PettyCashSurrender";
								button1ActionName = "PettyCashSurrenderHistory"; 
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
								pettyCashSurrenderObj.ErrorStatus = true;
								pettyCashSurrenderObj.ErrorMessage = "An error was experienced while trying to send an approval request for pettycash surrender no." + pettyCashSurrenderObj.No + ". " + ServiceConnection.contactICTDepartment + "";
								return View(pettyCashSurrenderObj);
							}
						}
						else
						{
							responseHeader = "PettyCash Surrender NotFound";
							responseMessage = "The pettycash surrender no." + pettyCashSurrenderObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
							detailedResponseMessage = "The pettycash  surrender no." + pettyCashSurrenderObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

							button1ControllerName = "PettyCashSurrender";
							button1ActionName = "PettyCashSurrenderHistory";
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
					else
					{
						return View(pettyCashSurrenderObj);
					}
				}
				if (Command.Equals("View Attachment"))
				{
					string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(pettyCashSurrenderObj.No);

					string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

					if (!fileURL.Equals(""))
					{
						using (WebClient wc = new WebClient())
						{
							if (ext.Equals(".pdf"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/pdf");
							}

							else if (ext.Equals(".doc"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/msword");
							}

							else if (ext.Equals(".docx"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
							}

							else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "image/jpeg");
							}

							else if (ext.Equals(".json"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/json");
							}

							else if (ext.Equals(".ppt"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.ms-powerpoint");
							}

							else if (ext.Equals(".png"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "image/png");
							}

							else if (ext.Equals(".pptx"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
							}

							else if (ext.Equals(".rar"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.rar");
							}

							else if (ext.Equals(".xls"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.ms-excel");
							}

							else if (ext.Equals(".xlsx"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
							}

							else
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "text/plain");
							}
						}
					}

					else
					{
						return View(pettyCashSurrenderObj);
					}
				}
				else
				{
					return View(pettyCashSurrenderObj);
				}
			}
			catch (Exception ex)
			{
				pettyCashSurrenderObj.ErrorStatus = true;
				pettyCashSurrenderObj.ErrorMessage = ex.Message.ToString();
				return View(pettyCashSurrenderObj);
			}
		}

		#endregion Edit PettyCash Surrender

		#region View PettyCash Surrender

		[Authorize]
		public ActionResult ViewPettyCashSurrender(string PettyCashSurrenderNo) 
		{
			try
			{
				if (PettyCashSurrenderNo.Equals(""))
				{
					return RedirectToAction("PettyCashSurrenderHistory", "PettyCashSurrender");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashSurrenderExists(PettyCashSurrenderNo, AccountController.GetEmployeeNo()))
				{
					PettyCashSurrenderHeaderModel pettyCashSurrenderObj = new PettyCashSurrenderHeaderModel();

					dynamic pettyCashSurrenders = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashSurrender(PettyCashSurrenderNo, ""));

					foreach (var pettyCashSurrender in pettyCashSurrenders)
					{
						pettyCashSurrenderObj.No = pettyCashSurrender.No;
						pettyCashSurrenderObj.DocumentDate = pettyCashSurrender.DocumentDate;
						pettyCashSurrenderObj.PostingDate = pettyCashSurrender.PostingDate;
						pettyCashSurrenderObj.BankAccountNo = pettyCashSurrender.BankAccountNo;
						pettyCashSurrenderObj.BankAccountName = pettyCashSurrender.BankAccountName;
						pettyCashSurrenderObj.ReferenceNo = pettyCashSurrender.ReferenceNo;
						pettyCashSurrenderObj.EmployeeNo = pettyCashSurrender.EmployeeNo;
						pettyCashSurrenderObj.EmployeeName = pettyCashSurrender.EmployeeName;
						pettyCashSurrenderObj.CurrencyCode = pettyCashSurrender.ImprestType;
						pettyCashSurrenderObj.Description = pettyCashSurrender.Description;
						pettyCashSurrenderObj.GlobalDimension1Code = pettyCashSurrender.GlobalDimension1Code;
						pettyCashSurrenderObj.GlobalDimension2Code = pettyCashSurrender.GlobalDimension2Code;
						pettyCashSurrenderObj.ShortcutDimension3Code = pettyCashSurrender.ShortcutDimension3Code;
						pettyCashSurrenderObj.ShortcutDimension4Code = pettyCashSurrender.ShortcutDimension4Code;
						pettyCashSurrenderObj.ShortcutDimension5Code = pettyCashSurrender.ShortcutDimension5Code;
						pettyCashSurrenderObj.ShortcutDimension6Code = pettyCashSurrender.ShortcutDimension6Code;
						pettyCashSurrenderObj.ShortcutDimension7Code = pettyCashSurrender.ShortcutDimension7Code;
						pettyCashSurrenderObj.ShortcutDimension8Code = pettyCashSurrender.ShortcutDimension8Code;
						pettyCashSurrenderObj.Amount = pettyCashSurrender.Amount;
						pettyCashSurrenderObj.Status = pettyCashSurrender.Status;
					}

					//LoadCurrencies();
					//LoadPettyCashSurrenderDimensions(pettyCashSurrenderObj.GlobalDimension1Code);
					LoadPettyshBankAccounts();

					pettyCashSurrenderObj.BankAccountNos = new SelectList(pettyCashSurrenderCodes, "TransactionCode", "TransactionDescription");
					//pettyCashSurrenderObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
					//pettyCashSurrenderObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
					//pettyCashSurrenderObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
					//pettyCashSurrenderObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
					//pettyCashSurrenderObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
					//pettyCashSurrenderObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
					//pettyCashSurrenderObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
					//pettyCashSurrenderObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
					//pettyCashSurrenderObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

					return View(pettyCashSurrenderObj);
				}
				else
				{
					responseHeader = "PettyCash Surrender NotFound";
					responseMessage = "The pettycash surrender no." + PettyCashSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The pettycash surrender no." + PettyCashSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "PettyCashSurrender";
					button1ActionName = "PettyCashSurrenderHistory";
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
		public async Task<ActionResult> ViewPettyCashSurrender(PettyCashSurrenderHeaderModel pettyCashSurrenderObj, string Command)
		{
			try
			{
				if (pettyCashSurrenderObj.No.Equals(""))
				{
					return RedirectToAction("PettyCashSurrenderHistory", "PettyCashSurrender");
				}
				if (Command.Equals("View Attachment"))
				{
					string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(pettyCashSurrenderObj.No);

					string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

					if (!fileURL.Equals(""))
					{
						using (WebClient wc = new WebClient())
						{
							if (ext.Equals(".pdf"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/pdf");
							}

							else if (ext.Equals(".doc"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/msword");
							}

							else if (ext.Equals(".docx"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
							}

							else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "image/jpeg");
							}

							else if (ext.Equals(".json"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/json");
							}

							else if (ext.Equals(".ppt"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.ms-powerpoint");
							}

							else if (ext.Equals(".png"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "image/png");
							}

							else if (ext.Equals(".pptx"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
							}

							else if (ext.Equals(".rar"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.rar");
							}

							else if (ext.Equals(".xls"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.ms-excel");
							}

							else if (ext.Equals(".xlsx"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
							}

							else
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "text/plain");
							}
						}
					}


					else
					{
						return View(pettyCashSurrenderObj);
					}
				}
				else
				{
					pettyCashSurrenderObj.ErrorStatus = true;
					//leaveApplicationObj2.ErrorMessage = "Unable to process the approve/reject action. Contact the " + companyName + " for assistance.";
					return View(pettyCashSurrenderObj);
				}
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		#endregion View PettyCash Surrender

		#region PettyCash Surrender history

		[Authorize]
		public ActionResult PettyCashSurrenderHistory() 
		{
			try
			{
				List<PettyCashSurrenderHeaderModel> pettyCashSurrenderList = new List<PettyCashSurrenderHeaderModel>();
				FinanceHomeController financeHomeController = new FinanceHomeController();

				dynamic pettyCashSurrenders = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashSurrender("", employeeNo));

				foreach (var pettyCashSurrender in pettyCashSurrenders)
				{
					PettyCashSurrenderHeaderModel pettyCashSurrenderObj = new PettyCashSurrenderHeaderModel();
					pettyCashSurrenderObj.No = pettyCashSurrender.No;
					pettyCashSurrenderObj.DocumentDate = pettyCashSurrender.DocumentDate;
					pettyCashSurrenderObj.PostingDate = pettyCashSurrender.PostingDate;
					pettyCashSurrenderObj.BankAccountNo = pettyCashSurrender.BankAccountNo;
					pettyCashSurrenderObj.BankAccountName = pettyCashSurrender.BankAccountName;
					pettyCashSurrenderObj.ReferenceNo = pettyCashSurrender.ReferenceNo;
					pettyCashSurrenderObj.EmployeeNo = pettyCashSurrender.EmployeeNo;
					pettyCashSurrenderObj.EmployeeName = pettyCashSurrender.EmployeeName;
					pettyCashSurrenderObj.CurrencyCode = pettyCashSurrender.ImprestType;
					pettyCashSurrenderObj.Description = pettyCashSurrender.Description;
					pettyCashSurrenderObj.GlobalDimension1Code = pettyCashSurrender.GlobalDimension1Code;
					pettyCashSurrenderObj.GlobalDimension2Code = pettyCashSurrender.GlobalDimension2Code;
					pettyCashSurrenderObj.ShortcutDimension3Code = pettyCashSurrender.ShortcutDimension3Code;
					pettyCashSurrenderObj.ShortcutDimension4Code = pettyCashSurrender.ShortcutDimension4Code;
					pettyCashSurrenderObj.ShortcutDimension5Code = pettyCashSurrender.ShortcutDimension5Code;
					pettyCashSurrenderObj.ShortcutDimension6Code = pettyCashSurrender.ShortcutDimension6Code;
					pettyCashSurrenderObj.ShortcutDimension7Code = pettyCashSurrender.ShortcutDimension7Code;
					pettyCashSurrenderObj.ShortcutDimension8Code = pettyCashSurrender.ShortcutDimension8Code;
					pettyCashSurrenderObj.Amount = pettyCashSurrender.Amount;
					pettyCashSurrenderObj.Status = pettyCashSurrender.Status;

					pettyCashSurrenderList.Add(pettyCashSurrenderObj);
				}
				return View(pettyCashSurrenderList);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		#endregion End PettyCash Surrender history

		#region PettyCash Surrender Line

		[ChildActionOnly]
		[Authorize]
		public ActionResult _PettyCashSurrenderLine(string PettyCashSurrenderNo) 
		{
			PettyCashSurrenderLineModel PettyCashSurrenderLineObj = new PettyCashSurrenderLineModel();
            #region Dimension 1 List
            List<DimensionValues> DimensionValues = new List<DimensionValues>();
            string dimension1list = "DimensionValues?$filter=Global_Dimension_No eq 1 and Blocked eq false &$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(dimension1list);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    DimensionValues DList1 = new DimensionValues();
                    DList1.Code = (string)config1["Code"];
                    DList1.Name = (string)config1["Name"];
                    DimensionValues.Add(DList1);
                }
            }
            #endregion
            LoadPettyCashTransactionTypes();
			PettyCashSurrenderLineObj.PettyCashTransactionTypes = new SelectList(pettyCashTransactionCodes, "No", "Name");

			LoadReceiptLines();
			PettyCashSurrenderLineObj.ReceiptNos = new SelectList(receiptLineList, "ReceiptNo", "Description");
            PettyCashSurrenderLineObj.Dimension1s = new SelectList(DimensionValues, "Code", "Name");
            PettyCashSurrenderLineObj.Dimension2s = new SelectList(Enumerable.Empty<SelectListItem>());
            PettyCashSurrenderLineObj.Dimension3s = new SelectList(Enumerable.Empty<SelectListItem>());
            PettyCashSurrenderLineObj.Dimension4s = new SelectList(Enumerable.Empty<SelectListItem>());
            PettyCashSurrenderLineObj.Dimension5s = new SelectList(Enumerable.Empty<SelectListItem>());
            PettyCashSurrenderLineObj.Dimension6s = new SelectList(Enumerable.Empty<SelectListItem>());
            PettyCashSurrenderLineObj.Dimension7s = new SelectList(Enumerable.Empty<SelectListItem>());
            return PartialView(PettyCashSurrenderLineObj);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewPettyCashSurrenderLine(string PettyCashSurrenderNo)
		{
			PettyCashSurrenderLineModel PettyCashSurrenderLineObj = new PettyCashSurrenderLineModel();

			LoadPettyCashTransactionTypes();
			PettyCashSurrenderLineObj.PettyCashTransactionTypes = new SelectList(pettyCashTransactionCodes, "No", "Name");

			LoadReceiptLines();
			PettyCashSurrenderLineObj.ReceiptNos = new SelectList(receiptLineList, "ReceiptNo", "Description");

			return PartialView(PettyCashSurrenderLineObj);
		} 

		[Authorize]
		public JsonResult GetPettyCashSurrenderLines(string DocumentNo) 
		{
            List<PettyCashSurrenderLineModel> pettyCashSurrenderLinesList = new List<PettyCashSurrenderLineModel>();
            string imprestlines = "PettyCashSurrenderLines?$filter=Document_No eq '" + DocumentNo + "' &$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(imprestlines);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    PettyCashSurrenderLineModel imprestLineObj = new PettyCashSurrenderLineModel();
                    imprestLineObj.LineNo = (string)config1["Line_No"];
                    imprestLineObj.DocumentNo = (string)config1["Document_No"];
                    imprestLineObj.PettyCashTransactionType = (string)config1["Type"];
                    
                    imprestLineObj.AccountType = (string)config1["Account_Type"];
                    imprestLineObj.AccountNo = (string)config1["Account_No"];
                    imprestLineObj.AccountName = (string)config1["Transaction_Type"];
                    imprestLineObj.LineDescription = (string)config1["Description"];
                    imprestLineObj.LineAmount = (string)config1["Amount"];
                    imprestLineObj.LineActualAmount = (string)config1["Actual_Spent"];
                    imprestLineObj.Dimension1 = (string)config1["Shortcut_Dimension_1_Code"];
                    imprestLineObj.Dimension2 = (string)config1["Shortcut_Dimension_2_Code"];
                    imprestLineObj.Dimension3 = (string)config1["ShortcutDimCode3"];
                    imprestLineObj.Dimension4 = (string)config1["ShortcutDimCode4"];
                    imprestLineObj.Dimension5 = (string)config1["ShortcutDimCode5"];
                    imprestLineObj.Dimension6 = (string)config1["ShortcutDimCode6"];
                    imprestLineObj.Dimension7 = (string)config1["ShortcutDimCode7"];
                    imprestLineObj.Status = (string)config1["Status"];
                    pettyCashSurrenderLinesList.Add(imprestLineObj);
                }
            }
            #endregion
            //List<PettyCashSurrenderLineModel> pettyCashSurrenderLinesList = new List<PettyCashSurrenderLineModel>();

            //dynamic pettyCashSurrenderLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashSurrenderLines(DocumentNo));

            //foreach (var pettyCashSurrenderLine in pettyCashSurrenderLines)
            //{
            //	PettyCashSurrenderLineModel PettyCashSurrenderLineObj = new PettyCashSurrenderLineModel();
            //	PettyCashSurrenderLineObj.LineNo = pettyCashSurrenderLine.LineNo;
            //	PettyCashSurrenderLineObj.DocumentNo = pettyCashSurrenderLine.DocumentNo;
            //	PettyCashSurrenderLineObj.PettyCashTransactionType = pettyCashSurrenderLine.ImprestCode;
            //	PettyCashSurrenderLineObj.AccountType = pettyCashSurrenderLine.AccountType;
            //	PettyCashSurrenderLineObj.AccountNo = pettyCashSurrenderLine.AccountNo;
            //	PettyCashSurrenderLineObj.AccountName = pettyCashSurrenderLine.AccountName;
            //	PettyCashSurrenderLineObj.LineDescription = pettyCashSurrenderLine.LineDescription;
            //	PettyCashSurrenderLineObj.LineAmount = pettyCashSurrenderLine.LineAmount;
            //             PettyCashSurrenderLineObj.LineActualAmount = pettyCashSurrenderLine.LineActualAmount;
            //             PettyCashSurrenderLineObj.LineGlobalDimension1Code = pettyCashSurrenderLine.Dimension1;
            //             PettyCashSurrenderLineObj.LineGlobalDimension2Code = pettyCashSurrenderLine.Dimension2;
            //             PettyCashSurrenderLineObj.LineShortcutDimension3Code = pettyCashSurrenderLine.Dimension3;

            //             pettyCashSurrenderLinesList.Add(PettyCashSurrenderLineObj);
            //}

            return Json(pettyCashSurrenderLinesList, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult GetPettyCashSurrenderLine(int LineNo, string DocumentNo)
		{
			PettyCashSurrenderLineModel PettyCashSurrenderLineObj = new PettyCashSurrenderLineModel();

			dynamic pettyCashSurrenderLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashSurrenderByLine(LineNo, DocumentNo));

			foreach (var pettyCashSurrenderLine in pettyCashSurrenderLines)
			{
				PettyCashSurrenderLineObj.LineNo = pettyCashSurrenderLine.LineNo;
				PettyCashSurrenderLineObj.DocumentNo = pettyCashSurrenderLine.DocumentNo;
				PettyCashSurrenderLineObj.PettyCashTransactionType = pettyCashSurrenderLine.ImprestCode;
				PettyCashSurrenderLineObj.AccountType = pettyCashSurrenderLine.AccountType;
				PettyCashSurrenderLineObj.AccountNo = pettyCashSurrenderLine.AccountNo;
				PettyCashSurrenderLineObj.AccountName = pettyCashSurrenderLine.AccountName;
				PettyCashSurrenderLineObj.LineDescription = pettyCashSurrenderLine.LineDescription;
				PettyCashSurrenderLineObj.LineAmount = pettyCashSurrenderLine.LineAmount;
			}

			return Json(PettyCashSurrenderLineObj, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult CreatePettyCashSurrenderLine(PettyCashSurrenderLineModel PettyCashSurrenderLineObj)
		{
			bool pettyCashSurrenderLineCreated = false;

			pettyCashSurrenderLineCreated = dynamicsNAVSOAPServices.fundsManagementWS.CreatePettyCashSurrenderLine(PettyCashSurrenderLineObj.DocumentNo, PettyCashSurrenderLineObj.PettyCashTransactionType,
																							   PettyCashSurrenderLineObj.LineDescription, Convert.ToDecimal(PettyCashSurrenderLineObj.LineAmount));

			return Json(new { PettyCashSurrenderLineCreated = pettyCashSurrenderLineCreated }, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult ModifyPettyCashSurrenderLine(PettyCashSurrenderLineModel PettyCashSurrenderLineObj) 
		{
			bool pettyCashSurrenderLineModified = false;
			pettyCashSurrenderLineModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyPettyCashSurrenderLine(Convert.ToInt32(PettyCashSurrenderLineObj.LineNo), PettyCashSurrenderLineObj.DocumentNo, PettyCashSurrenderLineObj.PettyCashTransactionType ?? "",
																								 PettyCashSurrenderLineObj.LineDescription ?? "", Convert.ToDecimal(PettyCashSurrenderLineObj.LineActualAmount));
			 
			return Json(new { PettyCashSurrenderLineModified = pettyCashSurrenderLineModified }, JsonRequestBehavior.AllowGet);
		}

        public JsonResult ValidatePettyCashSurrenderLines(string DocumentNo, string UnsurrenderedImprest)
        {
            try {
                return Json(dynamicsNAVSOAPServices.fundsManagementWS.InsertPettyCashLines(DocumentNo, UnsurrenderedImprest));
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
         }
        [Authorize]
		public JsonResult DeletePettyCashSurrenderLine(int LineNo, string DocumentNo)
		{
			bool pettyCashSurrenderLineDeleted = false;

			pettyCashSurrenderLineDeleted = dynamicsNAVSOAPServices.fundsManagementWS.DeletePettyCashSurrenderLine(LineNo, DocumentNo);

			return Json(new { PettyCashSurrenderLineDeleted = pettyCashSurrenderLineDeleted }, JsonRequestBehavior.AllowGet);
		}


		#region PettyCash Surrender Approval

		[Authorize]
		public ActionResult PettyCashSurrenderApproval(string PettyCashSurrenderNo)
		{
			try
			{
				if (PettyCashSurrenderNo.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}

				PettyCashSurrenderHeaderModel pettyCashSurrenderObj = new PettyCashSurrenderHeaderModel(); 

				dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashSurrender(PettyCashSurrenderNo, ""));

				foreach (var imprestRequest in imprestRequests)
				{
					pettyCashSurrenderObj.No = imprestRequest.No;
					pettyCashSurrenderObj.DocumentDate = imprestRequest.DocumentDate;
					pettyCashSurrenderObj.PostingDate = imprestRequest.PostingDate;
					pettyCashSurrenderObj.BankAccountNo = imprestRequest.BankAccountNo;
					pettyCashSurrenderObj.BankAccountName = imprestRequest.BankAccountName;
					pettyCashSurrenderObj.ReferenceNo = imprestRequest.ReferenceNo;
					pettyCashSurrenderObj.EmployeeNo = imprestRequest.EmployeeNo;
					pettyCashSurrenderObj.EmployeeName = imprestRequest.EmployeeName;
					pettyCashSurrenderObj.CurrencyCode = imprestRequest.ImprestType;
					pettyCashSurrenderObj.Description = imprestRequest.Description;
					pettyCashSurrenderObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
					pettyCashSurrenderObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
					pettyCashSurrenderObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
					pettyCashSurrenderObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
					pettyCashSurrenderObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
					pettyCashSurrenderObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
					pettyCashSurrenderObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
					pettyCashSurrenderObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
					pettyCashSurrenderObj.Amount = imprestRequest.Amount;
					pettyCashSurrenderObj.Status = imprestRequest.Status;
				}

				//LoadCurrencies();
				//LoadPettyCashSurrenderDimensions(pettyCashSurrenderObj.GlobalDimension1Code);

				LoadPettyshBankAccounts();

				pettyCashSurrenderObj.BankAccountNos = new SelectList(pettyCashSurrenderCodes, "TransactionCode", "TransactionDescription");
				//pettyCashSurrenderObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
				//pettyCashSurrenderObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
				//pettyCashSurrenderObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
				//pettyCashSurrenderObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
				//pettyCashSurrenderObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
				//pettyCashSurrenderObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
				//pettyCashSurrenderObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
				//pettyCashSurrenderObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
				//pettyCashSurrenderObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

				return View(pettyCashSurrenderObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> PettyCashSurrenderApproval(PettyCashSurrenderHeaderModel pettyCashSurrenderObj, string Command)
		{
			try
			{
				if (pettyCashSurrenderObj.No.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}
				if (Command == "Approve")
				{
					pettyCashSurrenderObj.Comments = pettyCashSurrenderObj.Comments != null ? pettyCashSurrenderObj.Comments : "";
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveImprestRequest(employeeNo, pettyCashSurrenderObj.No, AccountController.GetEmployeeNo()))
					{
                        TempData["Success"] = "PettyCash Surrender no." + pettyCashSurrenderObj.No + " was successfully approved.";
                        return RedirectToAction("OpenEntries", "Approval");
                        //responseHeader = "Success";
                        //responseMessage = "PettyCash Surrender No." + pettyCashSurrenderObj.No + " was successfully approved.";
                        //detailedResponseMessage = "PettyCash Surrender No." + pettyCashSurrenderObj.No + " was successfully approved.";

                        //button1ControllerName = "Approval";
                        //button1ActionName = "OpenEntries";
                        //button1HasParameters = false;
                        //button1Parameters = "";
                        //button1Name = "Ok";

                        //button2ControllerName = "";
                        //button2ActionName = "";
                        //button2HasParameters = false;
                        //button2Parameters = "";
                        //button2Name = "";

                        //return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                        //											button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                        //											button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
					else
					{
                        TempData["Error"] = "Unable to process the imprest request approve action. " + ServiceConnection.contactICTDepartment + "";
                        return View(pettyCashSurrenderObj);
                        //pettyCashSurrenderObj.ErrorStatus = true;
                        //pettyCashSurrenderObj.ErrorMessage = "Unable to process the imprest request approve action. " + ServiceConnection.contactICTDepartment + "";
                        //return View(pettyCashSurrenderObj);
                    }
				}
				else if (Command == "Reject")
				{
					pettyCashSurrenderObj.Comments = pettyCashSurrenderObj.Comments != null ? pettyCashSurrenderObj.Comments : "";
					if (pettyCashSurrenderObj.Comments.Equals(""))
					{
						dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashSurrender(pettyCashSurrenderObj.No, ""));

						foreach (var imprestRequest in imprestRequests)
						{
							pettyCashSurrenderObj.No = imprestRequest.No;
							pettyCashSurrenderObj.DocumentDate = imprestRequest.DocumentDate;
							pettyCashSurrenderObj.PostingDate = imprestRequest.PostingDate;
							pettyCashSurrenderObj.BankAccountNo = imprestRequest.BankAccountNo;
							pettyCashSurrenderObj.BankAccountName = imprestRequest.BankAccountName;
							pettyCashSurrenderObj.ReferenceNo = imprestRequest.ReferenceNo;
							pettyCashSurrenderObj.EmployeeNo = imprestRequest.EmployeeNo;
							pettyCashSurrenderObj.EmployeeName = imprestRequest.EmployeeName;
							pettyCashSurrenderObj.CurrencyCode = imprestRequest.ImprestType;
							pettyCashSurrenderObj.Description = imprestRequest.Description;
							pettyCashSurrenderObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
							pettyCashSurrenderObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
							pettyCashSurrenderObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
							pettyCashSurrenderObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
							pettyCashSurrenderObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
							pettyCashSurrenderObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
							pettyCashSurrenderObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
							pettyCashSurrenderObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
							pettyCashSurrenderObj.Amount = imprestRequest.Amount;
							pettyCashSurrenderObj.Status = imprestRequest.Status;
						}
                        TempData["Error"] = "Kindly provide reason(s) for declining/rejecting this document.";
                        return View(pettyCashSurrenderObj);
                        //pettyCashSurrenderObj.ErrorStatus = true;
                        //pettyCashSurrenderObj.ErrorMessage = "Kindly provide reason (s) for declining/rejecting this document.";
                        //return View(pettyCashSurrenderObj);
                    }
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectImprestRequest(employeeNo, pettyCashSurrenderObj.No, pettyCashSurrenderObj.Comments, AccountController.GetEmployeeNo()))
					{
                        TempData["Success"] = "PettyCash Surrender no." + pettyCashSurrenderObj.No + " was successfully rejected.";
                        return RedirectToAction("OpenEntries", "Approval");
                        //responseHeader = "Success";
                        //responseMessage = "PettyCash Surrender no." + pettyCashSurrenderObj.No + " was successfully rejected.";
                        //detailedResponseMessage = "PettyCash Surrender no." + pettyCashSurrenderObj.No + " was successfully rejected.";

                        //button1ControllerName = "Approval";
                        //button1ActionName = "OpenEntries";
                        //button1HasParameters = false;
                        //button1Parameters = "";
                        //button1Name = "Ok";

                        //button2ControllerName = "";
                        //button2ActionName = "";
                        //button2HasParameters = false;
                        //button2Parameters = "";
                        //button2Name = "";

                        //return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                        //											button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                        //											button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
					else
					{
                        TempData["Error"] = "Unable to process the imprest request reject action. " + ServiceConnection.contactICTDepartment + "";
                        return View(pettyCashSurrenderObj);
                        //pettyCashSurrenderObj.ErrorStatus = true;
                        //pettyCashSurrenderObj.ErrorMessage = "Unable to process the imprest request reject action. " + ServiceConnection.contactICTDepartment + "";
                        //return View(pettyCashSurrenderObj);
                    }
				}
				else if (Command == "View Attachment")
				{
					string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(pettyCashSurrenderObj.No);

					string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

					if (!fileURL.Equals(""))
					{
						using (WebClient wc = new WebClient())
						{
							if (ext.Equals(".pdf"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/pdf");
							}

							else if (ext.Equals(".doc"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/msword");
							}

							else if (ext.Equals(".docx"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
							}

							else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "image/jpeg");
							}

							else if (ext.Equals(".json"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/json");
							}

							else if (ext.Equals(".ppt"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.ms-powerpoint");
							}

							else if (ext.Equals(".png"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "image/png");
							}

							else if (ext.Equals(".pptx"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
							}

							else if (ext.Equals(".rar"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.rar");
							}

							else if (ext.Equals(".xls"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.ms-excel");
							}

							else if (ext.Equals(".xlsx"))
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
							}

							else
							{
								var byteArr = await wc.DownloadDataTaskAsync(fileURL);
								return File(byteArr, "text/plain");
							}
						}
					}


					else
					{
						return View(pettyCashSurrenderObj);
					}
				}
				else
				{
                    TempData["Error"] = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                    return View(pettyCashSurrenderObj);
                    //pettyCashSurrenderObj.ErrorStatus = true;
                    //pettyCashSurrenderObj.ErrorMessage = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                    //return View(pettyCashSurrenderObj);
                }
            }
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		#endregion PettyCash Surrender Approval

		#region Document Management

		[ChildActionOnly]
		[Authorize]
		public ActionResult _PettyCashSurrenderDocumentLine(string DocumentNo)
		{
			DocumentMgmtModel pettyCashSurrenderDocumentObj = new DocumentMgmtModel();
			return PartialView(pettyCashSurrenderDocumentObj);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewPettyCashSurrenderDocumentLine(string HeaderNo)
		{
			DocumentMgmtModel pettyCashSurrenderDocumentObj = new DocumentMgmtModel();
			return PartialView(pettyCashSurrenderDocumentObj);
		}

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GenerateReport(string DocNo)
        {
            try
            {
                string employeeNo = "";
                string filename = "";
                string filenane = "";
                employeeNo = AccountController.GetEmployeeNo();
                filename = "PettyCashSurrender_" + employeeNo + "_" + DocNo + ".pdf";
                filenane = Models.Credentials.ObjNav.GeneratePettyCashSurrenderReport(filename, DocNo);
                return Json(new { message = "https://ess.cihebkenya.org/reports/" + filename, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
		[HttpPost]
		public JsonResult UploadPettyCashSurrenderDocumentLink(string DocumentNo, string DocumentCode, string DocumentDescription)
		{
            try
            {
                if (Request.Files.Count > 0)
                {
                    var root = "~/StaffData/";
                    bool folderpath = Directory.Exists(HttpContext.Server.MapPath(root));

                    if (!folderpath)
                    {
                        Directory.CreateDirectory(HttpContext.Server.MapPath(root));
                    }

                    var file = Request.Files[0];
                    string fileExt = Path.GetExtension(file.FileName).ToLower();
                    string fileName = DocumentNo + "-" + DocumentCode + fileExt;
                    string path = Path.Combine(HttpContext.Server.MapPath(root), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    if (fileExt == ".pdf" || fileExt == ".eml" || fileExt == ".xlsx" || fileExt == ".csv" || fileExt == ".rtf" || fileExt == ".doc" || fileExt == ".docx" || fileExt == ".jpg" || fileExt == ".jpeg" || fileExt == ".png"|| fileExt == ".msg")
                    {
                        file.SaveAs(path);

                        if (System.IO.File.Exists(path))
                        {
                            bool ret = false;
                            ret = dynamicsNAVSOAPServices.documentMgmt.InsertImprestAttachment(DocumentNo, path, 51525003, "Petty Cash Surrender");
                            //dynamicsNAVSOAPServices.documentMgmt.UploadFileToSharePointAndNAV(DocumentNo, DocumentCode, fileName, path);
                            if (ret)
                            {
                                return Json(new { success = true, message = DocumentDescription + " uploaded successfully" }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { success = false, message = DocumentDescription + " was not uploaded" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { success = false, message = DocumentDescription + " was not uploaded" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Unsupported file format" }, JsonRequestBehavior.AllowGet);
                    }


                }
                return Json(new { success = false, message = "Please select a pdf file!" }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

		[Authorize] 
		public JsonResult GetPettyCashSurrenderDocuments(string DocumentNo) 
		{
			List<DocumentMgmtModel> pettyCashSurrenderDocumentList = new List<DocumentMgmtModel>(); 

			dynamic pettyCashSurrenderDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(DocumentNo));

			foreach (var pettyCashSurrenderDocument in pettyCashSurrenderDocuments)
			{
				DocumentMgmtModel pettyCashSurrenderDocumentObj = new DocumentMgmtModel();
				pettyCashSurrenderDocumentObj.LineNo = pettyCashSurrenderDocument.LineNo;
				pettyCashSurrenderDocumentObj.DocumentNo = pettyCashSurrenderDocument.DocumentNo;
				pettyCashSurrenderDocumentObj.DocumentCode = pettyCashSurrenderDocument.DocumentCode;
				pettyCashSurrenderDocumentObj.DocumentDescription = pettyCashSurrenderDocument.DocumentDescription;
				pettyCashSurrenderDocumentObj.DocumentAttached = pettyCashSurrenderDocument.DocumentAttached;
				pettyCashSurrenderDocumentObj.FileName = pettyCashSurrenderDocument.FileName;

				pettyCashSurrenderDocumentList.Add(pettyCashSurrenderDocumentObj);
			}
			return Json(pettyCashSurrenderDocumentList.ToList(), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public ActionResult GetPettyCashSurrenderDocumentLink(string LineNo, string DocumentNo, string DocumentCode) 
		{
			try
			{
				DocumentMgmtModel pettyCashSurrenderDocumentObj = new DocumentMgmtModel(); 

				dynamic pettyCashSurrenderDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocumentByDocCode(Convert.ToInt32(LineNo), DocumentNo, DocumentCode));

				foreach (var pettyCashSurrenderDocument in pettyCashSurrenderDocuments)
				{
					pettyCashSurrenderDocumentObj.LineNo = pettyCashSurrenderDocument.LineNo;
					pettyCashSurrenderDocumentObj.DocumentNo = pettyCashSurrenderDocument.DocumentNo;
					pettyCashSurrenderDocumentObj.DocumentCode = pettyCashSurrenderDocument.DocumentCode;
					pettyCashSurrenderDocumentObj.DocumentDescription = pettyCashSurrenderDocument.DocumentDescription;
					pettyCashSurrenderDocumentObj.DocumentAttached = pettyCashSurrenderDocument.DocumentAttached;
					pettyCashSurrenderDocumentObj.FileName = pettyCashSurrenderDocument.FileName;
				}

				return Json(pettyCashSurrenderDocumentObj, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		#endregion Document Management

		#region Helper Functions
		public JsonResult GetPettyCashSurrenderAmount(string DocumentNo) 
		{
			decimal pettyCashSurrenderAmount = 0;
			pettyCashSurrenderAmount = dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashSurrenderAmount(DocumentNo);  
			return Json(new { Amount = pettyCashSurrenderAmount }, JsonRequestBehavior.AllowGet); 
		}
		public string GetPettyCashSurrenderStatus(string DocumentNo) 
		{
			return dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashSurrenderStatus(DocumentNo);
		}
		private void LoadCurrencies()
		{
			currencyCodes = from currenciesQuery in dynamicsNAVODataServices.dynamicsNAVOData.Currencies
							select currenciesQuery;
		}

		private void LoadPettyshBankAccounts() 
		{
			pettyCashSurrenderCodes = new List<FundsTransactionModel>();

			dynamic fundsTransactionCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashBankAccounts(employeeNo));
			foreach (var fundsTransactionCode in fundsTransactionCodes)
			{
				FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
				FundsTransactionObj.TransactionCode = fundsTransactionCode.No;
				FundsTransactionObj.TransactionDescription = fundsTransactionCode.Name;

				pettyCashSurrenderCodes.Add(FundsTransactionObj);
			}
		}
		private void LoadPettyCashTransactionTypes()
		{
			pettyCashTransactionCodes = new List<PettyCashTransactionTypes>();

			dynamic fundsTransactionCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashTransactionTypes());
			foreach (var fundsTransactionCode in fundsTransactionCodes)
			{
				PettyCashTransactionTypes pettyCashTransactionTypeObj = new PettyCashTransactionTypes();
				pettyCashTransactionTypeObj.No = fundsTransactionCode.No;
				pettyCashTransactionTypeObj.Name = fundsTransactionCode.Name;

				pettyCashTransactionCodes.Add(pettyCashTransactionTypeObj);
			}
		}

		private void LoadReceiptLines()
		{
			receiptLineList = JsonConvert.DeserializeObject<List<ReceiptList>>(dynamicsNAVSOAPServices.fundsManagementWS.GetReceiptList(employeeNo));
		}
		#endregion Helper Functions
	}
}