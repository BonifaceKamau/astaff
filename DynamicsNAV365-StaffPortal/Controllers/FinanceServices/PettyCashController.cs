using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using DynamicsNAV365_StaffPortal.Models.Finance.PettyCash;
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
using static DynamicsNAV365_StaffPortal.Models.Finance.PettyCash.PettyCashLineModel;

namespace DynamicsNAV365_StaffPortal.Controllers.FinanceServices
{
    public class PettyCashController : Controller
    {
		static string companyURL = "";

		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
		DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
		BCODATAServices _bcodataServices = new BCODATAServices(companyURL);

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

		//IQueryable<FundsTransactionCodes> imprestCodes = null;
		IEnumerable<SelectListItem> imprestTypes = null;
		List<FundsTransactionModel> imprestCodes = null;
        List<TransactionTypeModel> impresttransactions = null;

        AccountController accountController = new AccountController(); 
		string employeeNo = "";

		public PettyCashController()
		{
			employeeNo = AccountController.GetEmployeeNo();
		}

		#region New Petty Cash  Request
	
		[Authorize]
		public ActionResult NewPettyCashRequest()
		{
			string imprestNo = "";
			string openPettyCashNo = "";
			try
			{
				PettyCashHeaderModel PettyCashRequestObj = new PettyCashHeaderModel();
				//Check open imprest request
				openPettyCashNo = dynamicsNAVSOAPServices.fundsManagementWS.CheckOpenPettyCashExists(employeeNo);
				if (!openPettyCashNo.Equals(""))
				{
					responseHeader = "Open Petty Cash Request";
					responseMessage = "An open petty cash request No. ("+ openPettyCashNo + ") exists under your account. You have to edit this document and submit for approval.";
					detailedResponseMessage = "An open petty cash request No. (" + openPettyCashNo + ") exists under your account. You have to edit this document and submit for approval.";

					button1ControllerName = "PettyCash";
					button1ActionName = "PettyCashRequestHistory";
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
				imprestNo = dynamicsNAVSOAPServices.fundsManagementWS.CreatePettyCashHeader(employeeNo);
                return RedirectToAction("EditPettyCashRequest", "PettyCash", new { PettyCashNo = imprestNo });
                //End create imprest request

                /*PettyCashRequestObj.No = imprestNo;
				PettyCashRequestObj.EmployeeNo = employeeNo;
				PettyCashRequestObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);
                PettyCashRequestObj.GlobalDimension1Code = "";

				//LoadCurrencies();
				LoadImprestRequestDimensions(PettyCashRequestObj.GlobalDimension1Code);
                PettyCashRequestObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
                PettyCashRequestObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");

                return View(PettyCashRequestObj);*/
            }
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		public ActionResult NewPettyCashRequest(PettyCashHeaderModel PettyCashRequestObj)
		{
			bool imprestRequestModified = false;
			bool approvalWorkflowExist = false;

            LoadImprestRequestDimensions(PettyCashRequestObj.GlobalDimension1Code);
            PettyCashRequestObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
            PettyCashRequestObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");

            try
			{
				//LoadCurrencies();
				LoadImprestRequestDimensions(PettyCashRequestObj.GlobalDimension1Code);

				if (ModelState.IsValid)
				{
					if (dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashExists(PettyCashRequestObj.No, AccountController.GetEmployeeNo()))
					{
						//Check imprest lines
						if (!dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashLinesExist(PettyCashRequestObj.No))
						{
							PettyCashRequestObj.ErrorStatus = true;
							PettyCashRequestObj.ErrorMessage = "Petty Cash lines missing, the petty cash must contain a minimum of one petty cash line, add an petty cash line to continue.";
							return View(PettyCashRequestObj);
						}
						//Validate imprest lines
						string imprestLineError = "";
						imprestLineError = dynamicsNAVSOAPServices.fundsManagementWS.ValidatePettyCashLines(PettyCashRequestObj.No);
						if (!imprestLineError.Equals(""))
						{
							PettyCashRequestObj.ErrorStatus = true;
							PettyCashRequestObj.ErrorMessage = imprestLineError;
							return View(PettyCashRequestObj);
						}

						//Modify petty cash request
						imprestRequestModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyPettyCashHeader(PettyCashRequestObj.No,PettyCashRequestObj.Description);
						if (!imprestRequestModified)
						{
							PettyCashRequestObj.ErrorStatus = true;
							PettyCashRequestObj.ErrorMessage = "An error was experienced while trying to modify petty cash no." + PettyCashRequestObj.No + ", the server might be offline, try again after a while.";
							return View(PettyCashRequestObj);
						}
						//Send imprest for approval
						approvalWorkflowExist = dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashApprovalWorkflowEnabled(PettyCashRequestObj.No);
						if (!approvalWorkflowExist)
						{
							PettyCashRequestObj.ErrorStatus = true;
							PettyCashRequestObj.ErrorMessage = "An error was experienced while trying to send an approval request for petty cash no." + PettyCashRequestObj.No + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
							return View(PettyCashRequestObj);
						}

						if (dynamicsNAVSOAPServices.fundsManagementWS.SendPettyCashApprovalRequest(PettyCashRequestObj.No))
						{
							responseHeader = "Success";
							responseMessage = "Petty Cash no." + PettyCashRequestObj.No + " was successfully sent for approval.";
							detailedResponseMessage = "Petty Cash no." + PettyCashRequestObj.No + " was successfully sent for approval.";

							button1ControllerName = "PettyCash";
							button1ActionName = "PettyCashRequestHistory";
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
						responseHeader = "Petty Cash NotFound";
						responseMessage = "The Petty Cash no." + PettyCashRequestObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
						detailedResponseMessage = "The Petty Cash no." + PettyCashRequestObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

						button1ControllerName = "PettyCash";
						button1ActionName = "PettyCashRequestHistory";
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
		#endregion New Petty Cash Request

		#region Edit Petty Cash Request
	
		[Authorize]
		public ActionResult OnBeforeEdit(string PettyCashNo)
		{
			try
			{
				if (PettyCashNo.Equals(""))
				{
					return RedirectToAction("PettyCashRequestHistory", "PettyCash");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashExists(PettyCashNo, AccountController.GetEmployeeNo()))
				{
					string imprestStatus = GetImprestStatus(PettyCashNo);
					//if imprest is open
					if (imprestStatus.Equals("Open"))
					{
						return RedirectToAction("EditPettyCashRequest", "PettyCash", new { PettyCashNo = PettyCashNo });
					}

					//if imprest is pending approval
					if (imprestStatus.Equals("Pending Approval"))
					{
						responseHeader = "Petty Cash Request Pending Approval";
						responseMessage = "The petty cash no." + PettyCashNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
						detailedResponseMessage = "The petty cash no." + PettyCashNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";

						button1ControllerName = "PettyCash";
						button1ActionName = "EditPettyCashRequest";
						button1HasParameters = true;
						button1Parameters = "?PettyCashNo=" + PettyCashNo;
						button1Name = "Yes";

						button2ControllerName = "PettyCash";
						button2ActionName = "PettyCashRequestHistory";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}

					//if imprest is released
					if (imprestStatus.Equals("Released"))
					{
						responseHeader = "Petty Cash Approved";
						responseMessage = "The petty cash no." + PettyCashNo + " is already approved. Editing not allowed.";
						detailedResponseMessage = "The petty cash no." + PettyCashNo + " is already approved. Editing not allowed.";

						button1ControllerName = "PettyCash";
						button1ActionName = "PettyCashRequestHistory";
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
					//if imprest is rejected
					if (imprestStatus.Equals("Rejected"))
					{
						responseHeader = "Petty Cash Rejected";
						responseMessage = "The petty cahs no." + PettyCashNo + " was rejected. Editing will reopen the document. Do you want to continue?";
						detailedResponseMessage = "The petty cash no." + PettyCashNo + " was rejected. Editing will reopen the document. Do you want to continue?";

						button1ControllerName = "PettyCash";
						button1ActionName = "EditPettyCashRequest";
						button1HasParameters = true;
						button1Parameters = "?PettyCashNo=" + PettyCashNo;
						button1Name = "Yes";

						button2ControllerName = "PettyCash";
						button2ActionName = "PettyCashRequestHistory";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}
					//if imprest is posted/reversed
					if (imprestStatus.Equals("Posted") || imprestStatus.Equals("Reversed"))
					{
						responseHeader = "Petty Cash Posted";
						responseMessage = "The petty cash no." + PettyCashNo + " is already posted. Editing not allowed.";
						detailedResponseMessage = "The petty cash no." + PettyCashNo + " is already posted. Editing not allowed.";

						button1ControllerName = "PettyCash";
						button1ActionName = "PettyCashRequestHistory";
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
					return RedirectToAction("PettyCashRequestHistory", "PettyCash");
				}
				else
				{
					responseHeader = "Petty Cash NotFound";
					responseMessage = "The petty cash no." + PettyCashNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The petty cash no." + PettyCashNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "PettyCash";
					button1ActionName = "PettyCashRequestHistory";
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
		public ActionResult EditPettyCashRequest(string PettyCashNo)
		{
			try
			{
				if (PettyCashNo.Equals(""))
				{
					return RedirectToAction("PettyCashRequestHistory", "PettyCash");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashExists(PettyCashNo, AccountController.GetEmployeeNo()))
				{
					string imprestStatus = dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashSurrenderStatus(PettyCashNo);

					//if imprest is pending approval, cancel approval request
					if (imprestStatus.Equals("Pending Approval"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.CancelPettyCashSurrenderApprovalRequest(PettyCashNo);
					}
					//if imprest is released, reopen and uncommit from budget
					if (imprestStatus.Equals("Released"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.ReopenPettyCashRequest(PettyCashNo);
						dynamicsNAVSOAPServices.fundsManagementWS.CancelImprestBudgetCommitment(PettyCashNo);
					}
					//if imprest is rejected, reopen document
					if (imprestStatus.Equals("Rejected"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.ReopenPettyCashRequest(PettyCashNo);
					}

					PettyCashHeaderModel pettyCashRequestObj = new PettyCashHeaderModel();

					//var imprestRequests = from imprestRequestsQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestRequests
					//					  where imprestRequestsQuery.No.Equals(PettyCashNo)
					//					  select imprestRequestsQuery;

					dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCash(PettyCashNo, ""));

					foreach (var imprestRequest in imprestRequests)
					{
						pettyCashRequestObj.No = imprestRequest.No;
						pettyCashRequestObj.DocumentDate = imprestRequest.DocumentDate;
						pettyCashRequestObj.PostingDate = imprestRequest.PostingDate;
						pettyCashRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
						pettyCashRequestObj.BankAccountName = imprestRequest.BankAccountName;
						pettyCashRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
						pettyCashRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
						pettyCashRequestObj.EmployeeName = imprestRequest.EmployeeName;
                        //imprestRequestObj.DateFrom = imprestRequest.DateFrom;
                        //imprestRequestObj.DateTo = imprestRequest.DateTo;
                        //imprestRequestObj.ImprestType = imprestRequest.Type;
                        pettyCashRequestObj.CurrencyCode = imprestRequest.ImprestType;
						//imprestRequestObj.Surrendered = imprestRequest.Surrendered ?? false;
						pettyCashRequestObj.Description = imprestRequest.Description;
                        pettyCashRequestObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
                        pettyCashRequestObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
                        pettyCashRequestObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
                        pettyCashRequestObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
                        pettyCashRequestObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
                        pettyCashRequestObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
                        pettyCashRequestObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
                        pettyCashRequestObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
                        pettyCashRequestObj.Amount = imprestRequest.Amount;
						pettyCashRequestObj.Status = imprestRequest.Status;
                        pettyCashRequestObj.PettyCashType = imprestRequest.PettyCashRequestType;

                    }

     //               LoadCurrencies();
                    LoadImprestRequestDimensions(pettyCashRequestObj.GlobalDimension1Code);

					//pettyCashRequestObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
					pettyCashRequestObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
					pettyCashRequestObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
					//pettyCashRequestObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
					//pettyCashRequestObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
					//pettyCashRequestObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
					//pettyCashRequestObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
					//pettyCashRequestObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
					//pettyCashRequestObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

                    return View(pettyCashRequestObj);
				}
				else
				{
					responseHeader = "Petty Cash NotFound";
					responseMessage = "The petty cash no." + PettyCashNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The petty cash no." + PettyCashNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "PettyCash";
					button1ActionName = "PettyCashRequestHistory";
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

        public JsonResult GetPettyCashAccounts(string item)
        {
            try
            {
                #region Items List
                List<DimensionValues> DimensionValues = new List<DimensionValues>();
                string dimension1list = "";
                
                if (item == "bank")
                {
                     dimension1list = "PettyCashBanks?$filter=Banker_ID eq '" + HttpUtility.UrlEncode(Session["UserID"].ToString()) + "' &$format=json";
                }
                else
                {
                    dimension1list = "ImprestTransactionTypes?$filter=Type eq 'Petty Cash' & $format=json";
                }
                

                HttpWebResponse httpResponse = Models.Credentials.GetOdataData(dimension1list);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues DList1 = new DimensionValues();
                        DList1.Code = (string)config["No"];
                        DList1.Name = (string)config["Name"];
                        DimensionValues.Add(DList1);
                    }
                }
                #endregion
                DropdownListData DropDownData = new DropdownListData
                {
                    ListOfddlData = DimensionValues.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Code,
                                         Value = x.Name
                                     }).ToList()
                };
                return Json(new { DropDownData, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
		[HttpPost]
		public async Task<ActionResult> EditPettyCashRequest(PettyCashHeaderModel PettyCashRequestObj, string Command) 
		{
			bool imprestRequestModified = false;
			bool approvalWorkflowExist = false;
			try
			{
				//LoadCurrencies();
				LoadImprestRequestDimensions(PettyCashRequestObj.GlobalDimension1Code);

				//PettyCashRequestObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
				PettyCashRequestObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
				PettyCashRequestObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
				//PettyCashRequestObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
				//PettyCashRequestObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
				//PettyCashRequestObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
				//PettyCashRequestObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
				//PettyCashRequestObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
				//PettyCashRequestObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

			if(Command.Equals("Submit For Approval")) 
				{
					if (ModelState.IsValid)
					{
						if (dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashExists(PettyCashRequestObj.No, AccountController.GetEmployeeNo()))
						{
							//Check imprest lines
							if (!dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashLinesExist(PettyCashRequestObj.No))
							{
								PettyCashRequestObj.ErrorStatus = true;
								PettyCashRequestObj.ErrorMessage = "Petty Cash lines missing, the petty cash must contain a minimum of one petty cash line, add an petty cash line to continue.";
								return View(PettyCashRequestObj);
							}
							//Validate imprest lines
							string imprestLineError = "";
							imprestLineError = dynamicsNAVSOAPServices.fundsManagementWS.ValidatePettyCashLines(PettyCashRequestObj.No);
							if (!imprestLineError.Equals(""))
							{
								PettyCashRequestObj.ErrorStatus = true;
								PettyCashRequestObj.ErrorMessage = imprestLineError;
								return View(PettyCashRequestObj);
							}

							imprestRequestModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyPettyCashHeader(PettyCashRequestObj.No,PettyCashRequestObj.Description);

							if (!imprestRequestModified)
							{
								PettyCashRequestObj.ErrorStatus = true;
								PettyCashRequestObj.ErrorMessage = "An error was experienced while trying to modify petty cash no." + PettyCashRequestObj.No + ", the server might be offline, try again after a while.";
								return View(PettyCashRequestObj);
							}

							//Send imprest for approval
							approvalWorkflowExist = dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestApprovalWorkflowEnabled(PettyCashRequestObj.No);
							if (!approvalWorkflowExist)
							{
								PettyCashRequestObj.ErrorStatus = true;
								PettyCashRequestObj.ErrorMessage = "An error was experienced while trying to send an approval request for petty cash no." + PettyCashRequestObj.No + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
								return View(PettyCashRequestObj);
							}

							if (dynamicsNAVSOAPServices.fundsManagementWS.SendImprestApprovalRequest(PettyCashRequestObj.No))
							{
								responseHeader = "Success";
								responseMessage = "Petty cash no." + PettyCashRequestObj.No + " was successfully sent for approval. Check with your HOD for approval for approval status.";
								detailedResponseMessage = "Petty Cash no." + PettyCashRequestObj.No + " was successfully sent for approval. Check with your HOD for approval for approval status.";

								button1ControllerName = "PettyCash";
								button1ActionName = "PettyCashRequestHistory";
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
								PettyCashRequestObj.ErrorMessage = "An error was experienced while trying to send an approval request for petty cash no." + PettyCashRequestObj.No + ". " + ServiceConnection.contactICTDepartment + "";
								return View(PettyCashRequestObj);
							}
						}
						else
						{
							responseHeader = "Petty Cash NotFound";
							responseMessage = "The petty cash no." + PettyCashRequestObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
							detailedResponseMessage = "The petty cash no." + PettyCashRequestObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

							button1ControllerName = "PettyCash";
							button1ActionName = "PettyCashRequestHistory";
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
			if (Command.Equals("View Attachment"))
				{
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(PettyCashRequestObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
					//string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(PettyCashRequestObj.No);

					//string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

					//if (!fileURL.Equals(""))
					//{
						//using (WebClient wc = new WebClient())
						//{
						//	if (ext.Equals(".pdf"))
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "application/pdf");
						//	}

						//	else if (ext.Equals(".doc"))
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "application/msword");
						//	}

						//	else if (ext.Equals(".docx"))
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
						//	}

						//	else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "image/jpeg");
						//	}

						//	else if (ext.Equals(".json"))
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "application/json");
						//	}

						//	else if (ext.Equals(".ppt"))
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "application/vnd.ms-powerpoint");
						//	}

						//	else if (ext.Equals(".png"))
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "image/png");
						//	}

						//	else if (ext.Equals(".pptx"))
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
						//	}

						//	else if (ext.Equals(".rar"))
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "application/vnd.rar");
						//	}

						//	else if (ext.Equals(".xls"))
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "application/vnd.ms-excel");
						//	}

						//	else if (ext.Equals(".xlsx"))
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
						//	}

						//	else
						//	{
						//		var byteArr = await wc.DownloadDataTaskAsync(fileURL);
						//		return File(byteArr, "text/plain");
						//	}
					//	}
					//}


					//else
					//{
					//	return View(PettyCashRequestObj);
					//}
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

		#endregion Edit Petty Cash Request

		#region View Petty Cash Request
	
		[Authorize]
		public ActionResult ViewPettyCashRequest(string PettyCashNo)
		{
			try
			{
				if (PettyCashNo.Equals(""))
				{
					return RedirectToAction("PettyCashRequestHistory", "PettyCash");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckPettyCashExists(PettyCashNo, AccountController.GetEmployeeNo()))
				{
					PettyCashHeaderModel pettyCashRequestObj = new PettyCashHeaderModel();

					//var imprestRequests = from imprestRequestsQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestRequests
					//					  where imprestRequestsQuery.No.Equals(PettyCashNo)
					//					  select imprestRequestsQuery;

					dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCash(PettyCashNo, ""));

					foreach (var imprestRequest in imprestRequests)
					{
						pettyCashRequestObj.No = imprestRequest.No;
						pettyCashRequestObj.DocumentDate = imprestRequest.DocumentDate;
						pettyCashRequestObj.PostingDate = imprestRequest.PostingDate;
						pettyCashRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
						pettyCashRequestObj.BankAccountName = imprestRequest.BankAccountName;
						pettyCashRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
						pettyCashRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
						pettyCashRequestObj.EmployeeName = imprestRequest.EmployeeName;
                        //imprestRequestObj.DateFrom = imprestRequest.DateFrom;
                        //imprestRequestObj.DateTo = imprestRequest.DateTo;
                        //imprestRequestObj.ImprestType = imprestRequest.Type;
                        pettyCashRequestObj.CurrencyCode = imprestRequest.ImprestType;
						//imprestRequestObj.Surrendered = imprestRequest.Surrendered ?? false;
						pettyCashRequestObj.Description = imprestRequest.Description;
						pettyCashRequestObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
						pettyCashRequestObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
						pettyCashRequestObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
						pettyCashRequestObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
						pettyCashRequestObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
						pettyCashRequestObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
						pettyCashRequestObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
						pettyCashRequestObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
						pettyCashRequestObj.Amount = imprestRequest.Amount;
						pettyCashRequestObj.Status = imprestRequest.Status;
					}

                    string globalDimension1Code = dynamicsNAVSOAPServices.procurementManagementWS.GetGlobalDimension1Code(PettyCashNo);

                    //LoadCurrencies();
                    LoadImprestRequestDimensions(pettyCashRequestObj.GlobalDimension1Code);

                    //pettyCashRequestObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
                    pettyCashRequestObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
					pettyCashRequestObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
					//pettyCashRequestObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
					//pettyCashRequestObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
					//pettyCashRequestObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
					//pettyCashRequestObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
					//pettyCashRequestObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
					//pettyCashRequestObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

					return View(pettyCashRequestObj);
				}
				else
				{
					responseHeader = "Pettu Cash Not Found";
					responseMessage = "The petty cash no." + PettyCashNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The petty cash no." + PettyCashNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "PettyCash";
					button1ActionName = "PettyCashRequestHistory";
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
		public async Task<ActionResult> ViewPettyCashRequest(PettyCashHeaderModel pettyCashRequestObj,string Command)
		{
			try
			{
				if (pettyCashRequestObj.No.Equals(""))
				{
					return RedirectToAction("PettyCashRequestHistory", "PettyCash");
				}
				if (Command.Equals("View Attachment"))
				{
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(pettyCashRequestObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");

					//string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(pettyCashRequestObj.No);

					//string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

					//if (!fileURL.Equals(""))
					//{
					//	using (WebClient wc = new WebClient())
					//	{
					//		if (ext.Equals(".pdf"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/pdf");
					//		}

					//		else if (ext.Equals(".doc"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/msword");
					//		}

					//		else if (ext.Equals(".docx"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
					//		}

					//		else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "image/jpeg");
					//		}

					//		else if (ext.Equals(".json"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/json");
					//		}

					//		else if (ext.Equals(".ppt"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.ms-powerpoint");
					//		}

					//		else if (ext.Equals(".png"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "image/png");
					//		}

					//		else if (ext.Equals(".pptx"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
					//		}

					//		else if (ext.Equals(".rar"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.rar");
					//		}

					//		else if (ext.Equals(".xls"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.ms-excel");
					//		}

					//		else if (ext.Equals(".xlsx"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
					//		}

					//		else
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "text/plain");
					//		}
					//	}
					//}


					//else
					//{
					//	return View(pettyCashRequestObj);
					//}
				}
				else
				{
					pettyCashRequestObj.ErrorStatus = true;
					//leaveApplicationObj2.ErrorMessage = "Unable to process the approve/reject action. Contact the " + companyName + " for assistance.";
					return View(pettyCashRequestObj);
				}
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		#endregion View Petty Cash Request

		#region Petty Cash requests history

		[Authorize]
		public ActionResult PettyCashRequestHistory()
		{
			try
			{
				List<PettyCashHeaderModel> pettyCashRequestsList = new List<PettyCashHeaderModel>();
				FinanceHomeController financeHomeController = new FinanceHomeController();

				//var imprestRequests = from imprestRequestsQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestRequests
				//					  where imprestRequestsQuery.HR_Employee_No.Equals(employeeNo) && imprestRequestsQuery.Type.Equals("Petty Cash")
				//					  select imprestRequestsQuery;

				dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCash("", employeeNo));

				foreach (var imprestRequest in imprestRequests)
				{
					PettyCashHeaderModel imprestRequestObj = new PettyCashHeaderModel();
					imprestRequestObj.No = imprestRequest.No;
					imprestRequestObj.DocumentDate = imprestRequest.DocumentDate;
					imprestRequestObj.PostingDate = imprestRequest.PostingDate;
					imprestRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
					imprestRequestObj.BankAccountName = imprestRequest.BankAccountName;
					imprestRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
					imprestRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
					imprestRequestObj.EmployeeName = imprestRequest.EmployeeName;
                    //imprestRequestObj.DateFrom = imprestRequest.DateFrom;
                    //imprestRequestObj.DateTo = imprestRequest.DateTo;
                    //imprestRequestObj.ImprestType = imprestRequest.Type;
                    imprestRequestObj.CurrencyCode = imprestRequest.ImprestType;
					//imprestRequestObj.Surrendered = imprestRequest.Surrendered ?? false;
					imprestRequestObj.Description = imprestRequest.Description;
					imprestRequestObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
					imprestRequestObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
					imprestRequestObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
					imprestRequestObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
					imprestRequestObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
					imprestRequestObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
					imprestRequestObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
					imprestRequestObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
					imprestRequestObj.Amount = imprestRequest.Amount;
					imprestRequestObj.Status = imprestRequest.Status;
					pettyCashRequestsList.Add(imprestRequestObj);
				}
				return View(pettyCashRequestsList);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		#endregion Petty Cash requests history

		#region Petty Cash Line

		[ChildActionOnly]
		[Authorize]
		public ActionResult _PettyCashLine(string PettyCashNo)
		{
			PettyCashLineModel PettyCashLineObj = new PettyCashLineModel();
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
			LoadImprestRequestCodes();
			PettyCashLineObj.ImprestCodes = new SelectList(imprestCodes, "TransactionCode", "TransactionDescription");
			PettyCashLineObj.impresttransactions = new SelectList(impresttransactions, "TransactCode");
            PettyCashLineObj.Dimension1s = new SelectList(DimensionValues, "Code", "Name");
            PettyCashLineObj.Dimension2s = new SelectList(Enumerable.Empty<SelectListItem>());
            PettyCashLineObj.Dimension3s = new SelectList(Enumerable.Empty<SelectListItem>());
            PettyCashLineObj.Dimension4s = new SelectList(Enumerable.Empty<SelectListItem>());
            PettyCashLineObj.Dimension5s = new SelectList(Enumerable.Empty<SelectListItem>());
            PettyCashLineObj.Dimension6s = new SelectList(Enumerable.Empty<SelectListItem>());
            PettyCashLineObj.Dimension7s = new SelectList(Enumerable.Empty<SelectListItem>());
			PettyCashLineObj.PettyCashLinesTypes = _bcodataServices.BCOData.ImprestTransactionTypes.Execute().Select(c => new SelectListItem
			{
				Text = $"{c.Code}:{c.Transaction_Name}",
				Value = c.Code
			}).ToList();
            //PettyCashLineObj.PettyCashType = dynamicsNAVSOAPServices.fundsManagementWS.GetRequestType(AccountController.GetEmployeeNo());

            return PartialView(PettyCashLineObj);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewPettyCashLine(string PettyCashNo)
		{
			PettyCashLineModel PettyCashLineObj = new PettyCashLineModel();

            LoadImprestRequestCodes();
		//	PettyCashLineObj.ImprestCodes = new SelectList(imprestCodes, "No", "Name");
			PettyCashLineObj.ImprestCodes = new SelectList(imprestCodes, "TransactionCode", "TransactionDescription");

            return PartialView(PettyCashLineObj);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _PettyCashSurrenderLine(string PettyCashNo)
		{
			PettyCashLineModel PettyCashLineObj = new PettyCashLineModel();

			LoadImprestRequestCodes();
			PettyCashLineObj.ImprestCodes = new SelectList(imprestCodes, "TransactionCode", "TransactionDescription");
            PettyCashLineObj.impresttransactions = new SelectList(impresttransactions, "TransactCode");

            return PartialView(PettyCashLineObj);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewPettyCashSurrenderLine(string PettyCashNo) 
		{
			PettyCashLineModel PettyCashLineObj = new PettyCashLineModel();

			LoadImprestRequestCodes();
			//	PettyCashLineObj.ImprestCodes = new SelectList(imprestCodes, "No", "Name");
			PettyCashLineObj.ImprestCodes = new SelectList(imprestCodes, "TransactionCode", "TransactionDescription");
            PettyCashLineObj.impresttransactions = new SelectList(impresttransactions, "TransactCode");

            return PartialView(PettyCashLineObj);
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
                filename = "PettyCash_" + employeeNo + "_" + DocNo + ".pdf";
                filenane = Models.Credentials.ObjNav.GeneratePettyCashReport(filename, DocNo);
                return Json(new { message = "https://ess.cihebkenya.org/reports/" + filename, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
		public JsonResult GetPettyCashLinesAjax(string DocumentNo)
		{
            List<PettyCashLineModel> pettyCashLinesList = new List<PettyCashLineModel>();
            string pettycashlines = "PettyCashLines?$filter=Document_No eq '" + DocumentNo + "' &$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(pettycashlines);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    PettyCashLineModel PettyCashLineObj = new PettyCashLineModel();
                    PettyCashLineObj.LineNo = (string)config1["Line_No"];
                    PettyCashLineObj.DocumentNo = (string)config1["Document_No"];
                    PettyCashLineObj.AccountType = (string)config1["Account_Type"];
                    PettyCashLineObj.AccountNo = (string)config1["Account_No"];
                    PettyCashLineObj.AccountName = (string)config1["Cash_Book_Name"];
                    PettyCashLineObj.LineDescription = (string)config1["Description"];
                    PettyCashLineObj.impresttransaction = (string)config1["Type"];
                    PettyCashLineObj.LineAmount = (string)config1["Amount"]; ;
                    PettyCashLineObj.Dimension1 = (string)config1["Shortcut_Dimension_1_Code"];
                    PettyCashLineObj.Dimension2 = (string)config1["Shortcut_Dimension_2_Code"];
                    //PettyCashLineObj.Dimension3 = (string)config1["ShortcutDimCode3"];
                    //PettyCashLineObj.Dimension4 = (string)config1["ShortcutDimCode4"];
                    //PettyCashLineObj.Dimension5 = (string)config1["ShortcutDimCode5"];
                    //PettyCashLineObj.Dimension6 = (string)config1["ShortcutDimCode6"];
                    //PettyCashLineObj.Dimension7 = (string)config1["ShortcutDimCode7"];
                    PettyCashLineObj.Status = (string)config1["Status"];
                    pettyCashLinesList.Add(PettyCashLineObj);
                }
            }

            return Json(pettyCashLinesList, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult GetPettyCashLine(int LineNo, string DocumentNo)
		{
			PettyCashLineModel PettyCashLineObj = new PettyCashLineModel();

			//var imprestLines = from imprestLinesQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestLines
			//				   where imprestLinesQuery.Line_No.Equals(LineNo) && imprestLinesQuery.Document_No.Equals(DocumentNo)
			//				   select imprestLinesQuery;

			dynamic imprestLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashByLine(LineNo, DocumentNo));

			foreach (var imprestLine in imprestLines)
			{
				PettyCashLineObj.LineNo = imprestLine.LineNo;
				PettyCashLineObj.DocumentNo = imprestLine.DocumentNo;
				PettyCashLineObj.ImprestCode = imprestLine.ImprestCode;
				PettyCashLineObj.impresttransaction = imprestLine.Type;
                PettyCashLineObj.FromCity = imprestLine.FromCity;
				PettyCashLineObj.AccountType = imprestLine.AccountType;
				PettyCashLineObj.ToCity = imprestLine.ToCity;
				PettyCashLineObj.AccountNo = imprestLine.AccountNo;
				PettyCashLineObj.AccountName = imprestLine.AccountName;
				PettyCashLineObj.LineDescription = imprestLine.LineDescription;
				PettyCashLineObj.LineAmount = imprestLine.LineAmount;
                PettyCashLineObj.LineGlobalDimension1Code = imprestLine.LineGlobalDimension1Code;
                PettyCashLineObj.LineGlobalDimension2Code = imprestLine.LineGlobalDimension2Code;
            }

			return Json(PettyCashLineObj, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult CreatePettyCashLine(PettyCashLineModel PettyCashLineObj) 
		{
			bool pettyCashLineCreated = false;
            try
            {
                pettyCashLineCreated = dynamicsNAVSOAPServices.fundsManagementWS.CreatePettyCashLine(PettyCashLineObj.DocumentNo, PettyCashLineObj.ImprestCode, PettyCashLineObj.LineDescription, PettyCashLineObj.impresttransaction,
																							  Convert.ToDecimal(PettyCashLineObj.LineAmount), PettyCashLineObj.LineGlobalDimension1Code ?? "", PettyCashLineObj.LineGlobalDimension2Code ?? "", PettyCashLineObj.LineShortcutDimension3Code ?? "", PettyCashLineObj.LineShortcutDimension4Code ?? "",
                                                                                             PettyCashLineObj.LineShortcutDimension5Code ?? "", PettyCashLineObj.LineShortcutDimension6Code ?? "", PettyCashLineObj.LineShortcutDimension7Code ?? "");

                //return Json(new { PettyCashLineCreated = pettyCashLineCreated }, JsonRequestBehavior.AllowGet);
                return Json(new { message = "Successful", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }

			
		}

		[Authorize]
		public JsonResult ModifyPettyCashLine(PettyCashLineModel PettyCashLineObj) 
		{
			try
			{
				bool pettyCashLineModified = false;
				pettyCashLineModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyPettyCashLine(Convert.ToInt32(PettyCashLineObj.LineNo), PettyCashLineObj.DocumentNo, PettyCashLineObj.ImprestCode??"", PettyCashLineObj.impresttransaction ?? "",
					PettyCashLineObj.LineDescription??"", Convert.ToDecimal(PettyCashLineObj.LineAmount), PettyCashLineObj.LineGlobalDimension1Code??"", PettyCashLineObj.LineGlobalDimension2Code??"", PettyCashLineObj.LineShortcutDimension3Code ?? "", PettyCashLineObj.LineShortcutDimension4Code ?? "",
					PettyCashLineObj.LineShortcutDimension5Code ?? "", PettyCashLineObj.LineShortcutDimension6Code ?? "", PettyCashLineObj.LineShortcutDimension7Code ?? "");

				return Json(new { success =true, PettyCashLineModified = pettyCashLineModified }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				return Json(new { success =false, message = e.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		[Authorize]
		public JsonResult DeletePettyCashLine(int LineNo, string DocumentNo)
		{
			bool pettyCashLineDeleted = false;

			pettyCashLineDeleted = dynamicsNAVSOAPServices.fundsManagementWS.DeletePettyCashLine(LineNo, DocumentNo);

			return Json(new { PettyCashLineDeleted = pettyCashLineDeleted }, JsonRequestBehavior.AllowGet); 
		}
		#endregion Petty Cash Line

		#region Petty Cash Approval
	
		[Authorize]
		public ActionResult PettyCashApproval(string PettyCashNo)
		{
			try
			{
				if (PettyCashNo.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}

				PettyCashHeaderModel PettyCashRequestObj = new PettyCashHeaderModel();

				dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCash(PettyCashNo, ""));

				foreach (var imprestRequest in imprestRequests)
				{
					PettyCashRequestObj.No = imprestRequest.No;
					PettyCashRequestObj.DocumentDate = imprestRequest.DocumentDate;
					PettyCashRequestObj.PostingDate = imprestRequest.PostingDate;
					PettyCashRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
					PettyCashRequestObj.BankAccountName = imprestRequest.BankAccountName;
					PettyCashRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
					PettyCashRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
					PettyCashRequestObj.EmployeeName = imprestRequest.EmployeeName;
                    //PettyCashRequestObj.DateFrom = imprestRequest.DateFrom;
                    //PettyCashRequestObj.DateTo = imprestRequest.DateTo;
                    //PettyCashRequestObj.ImprestType = imprestRequest.Type;
                    PettyCashRequestObj.CurrencyCode = imprestRequest.ImprestType;
					//imprestRequestObj.Surrendered = imprestRequest.Surrendered ?? false;
					PettyCashRequestObj.Description = imprestRequest.Description;
					PettyCashRequestObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
					PettyCashRequestObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
					PettyCashRequestObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
					PettyCashRequestObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
					PettyCashRequestObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
					PettyCashRequestObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
					PettyCashRequestObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
					PettyCashRequestObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
					PettyCashRequestObj.Amount = imprestRequest.Amount;
					PettyCashRequestObj.Status = imprestRequest.Status;
				}

				//LoadCurrencies();
				LoadImprestRequestDimensions(PettyCashRequestObj.GlobalDimension1Code);

				//PettyCashRequestObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
				PettyCashRequestObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
				PettyCashRequestObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
				//PettyCashRequestObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
				//PettyCashRequestObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
				//PettyCashRequestObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
				//PettyCashRequestObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
				//PettyCashRequestObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
				//PettyCashRequestObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

				return View(PettyCashRequestObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> PettyCashApproval(PettyCashHeaderModel PettyCashRequestObj, string Command) 
		{
			try
			{
				if (PettyCashRequestObj.No.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}
				if (Command == "Approve")
				{
					PettyCashRequestObj.Comments = PettyCashRequestObj.Comments != null ? PettyCashRequestObj.Comments : "";
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveImprestRequest(employeeNo, PettyCashRequestObj.No, AccountController.GetEmployeeNo()))
					{
                        TempData["Success"] = "Petty Cash Document No." + PettyCashRequestObj.No + " was successfully approved.";
                        return RedirectToAction("OpenEntries", "Approval");
                        //responseHeader = "Success";
                        //responseMessage = "Petty Cash Document No." + PettyCashRequestObj.No + " was successfully approved.";
                        //detailedResponseMessage = "Petty Cash Document No." + PettyCashRequestObj.No + " was successfully approved.";

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
                        return View(PettyCashRequestObj);
                        //PettyCashRequestObj.ErrorStatus = true;
                        //PettyCashRequestObj.ErrorMessage = "Unable to process the imprest request approve action. " + ServiceConnection.contactICTDepartment + "";
                        //return View(PettyCashRequestObj);
                    }
                }
				else if (Command == "Reject")
				{
					PettyCashRequestObj.Comments = PettyCashRequestObj.Comments != null ? PettyCashRequestObj.Comments : "";
					if (PettyCashRequestObj.Comments.Equals(""))
					{
						dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCash(PettyCashRequestObj.No, ""));

						foreach (var imprestRequest in imprestRequests)
						{
							PettyCashRequestObj.No = imprestRequest.No;
							PettyCashRequestObj.DocumentDate = imprestRequest.DocumentDate;
							PettyCashRequestObj.PostingDate = imprestRequest.PostingDate;
							PettyCashRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
							PettyCashRequestObj.BankAccountName = imprestRequest.BankAccountName;
							PettyCashRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
							PettyCashRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
							PettyCashRequestObj.EmployeeName = imprestRequest.EmployeeName;
                            //PettyCashRequestObj.DateFrom = imprestRequest.DateFrom;
                            //PettyCashRequestObj.DateTo = imprestRequest.DateTo;
                            //PettyCashRequestObj.ImprestType = imprestRequest.Type;
                            PettyCashRequestObj.CurrencyCode = imprestRequest.ImprestType;
							//imprestRequestObj.Surrendered = imprestRequest.Surrendered ?? false;
							PettyCashRequestObj.Description = imprestRequest.Description;
							PettyCashRequestObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
							PettyCashRequestObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
							PettyCashRequestObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
							PettyCashRequestObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
							PettyCashRequestObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
							PettyCashRequestObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
							PettyCashRequestObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
							PettyCashRequestObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
							PettyCashRequestObj.Amount = imprestRequest.Amount;
							PettyCashRequestObj.Status = imprestRequest.Status;
						}
                        TempData["Error"] = "Kindly provide reason (s) for declining/rejecting this document.";
                        return View(PettyCashRequestObj);
                        //PettyCashRequestObj.ErrorStatus = true;
                        //PettyCashRequestObj.ErrorMessage = "Kindly provide reason (s) for declining/rejecting this document.";
                        //return View(PettyCashRequestObj);
                    }
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectImprestRequest(employeeNo, PettyCashRequestObj.No, PettyCashRequestObj.Comments, AccountController.GetEmployeeNo()))
					{
                        TempData["Success"] = "Petty Cash Document no." + PettyCashRequestObj.No + " was successfully rejected.";
                        return RedirectToAction("OpenEntries", "Approval");
                        //responseHeader = "Success";
                        //responseMessage = "Petty Cash Document no." + PettyCashRequestObj.No + " was successfully rejected.";
                        //detailedResponseMessage = "Petty Cash Document no." + PettyCashRequestObj.No + " was successfully rejected.";

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
                        return View(PettyCashRequestObj);
                        //PettyCashRequestObj.ErrorStatus = true;
                        //PettyCashRequestObj.ErrorMessage = "Unable to process the imprest request reject action. " + ServiceConnection.contactICTDepartment + "";
                        //return View(PettyCashRequestObj);
                    }
				}
				else if (Command == "View Attachment")
				{
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(PettyCashRequestObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
					//string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(PettyCashRequestObj.No);

					//string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

					//if (!fileURL.Equals(""))
					//{
					//	using (WebClient wc = new WebClient())
					//	{
					//		if (ext.Equals(".pdf"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/pdf");
					//		}

					//		else if (ext.Equals(".doc"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/msword");
					//		}

					//		else if (ext.Equals(".docx"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
					//		}

					//		else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "image/jpeg");
					//		}

					//		else if (ext.Equals(".json"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/json");
					//		}

					//		else if (ext.Equals(".ppt"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.ms-powerpoint");
					//		}

					//		else if (ext.Equals(".png"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "image/png");
					//		}

					//		else if (ext.Equals(".pptx"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
					//		}

					//		else if (ext.Equals(".rar"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.rar");
					//		}

					//		else if (ext.Equals(".xls"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.ms-excel");
					//		}

					//		else if (ext.Equals(".xlsx"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
					//		}

					//		else
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "text/plain");
					//		}
					//	}
					//}


					//else
					//{
					//	return View(PettyCashRequestObj);
					//}
				}
				else
				{
                    TempData["Error"] = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                    return View(PettyCashRequestObj);
                    //PettyCashRequestObj.ErrorStatus = true;
                    //PettyCashRequestObj.ErrorMessage = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                    //return View(PettyCashRequestObj);
                }
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		#endregion Petty Cash Approval

		#region Document Management

		[Authorize]
		public JsonResult GetPettyCashPortalDocuments(string DocumentNo)
		{
			List<DocumentMgmtModel> applicationDocumentsList = new List<DocumentMgmtModel>();

			//var imprestUploadedDocuments = from imprestDocumentsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
			//							   where imprestDocumentsQuery.DocumentNo.Equals(DocumentNo)
			//							   select imprestDocumentsQuery;

			dynamic imprestUploadedDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(DocumentNo));

			foreach (var imprestDocument in imprestUploadedDocuments)
			{
				DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();
				documentManagementoBJ.LineNo = imprestDocument.LineNo;
				documentManagementoBJ.DocumentNo = imprestDocument.DocumentNo;
				documentManagementoBJ.DocumentCode = imprestDocument.DocumentCode;
				documentManagementoBJ.DocumentDescription = imprestDocument.DocumentDescription;
				documentManagementoBJ.DocumentAttached = imprestDocument.DocumentAttached;
				documentManagementoBJ.FileName = imprestDocument.FileName;

				applicationDocumentsList.Add(documentManagementoBJ);
			}
			return Json(applicationDocumentsList.ToList(), JsonRequestBehavior.AllowGet);
		} 

		[Authorize]
		[HttpPost]
		public JsonResult UploadPettyCashDocumentLink(string DocumentNo, string DocumentCode, string DocumentDescription)
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
					string fileName = DocumentNo + "_PRF" + fileExt;
					string path = Path.Combine(HttpContext.Server.MapPath(root), fileName);

					if (System.IO.File.Exists(path))
					{
						System.IO.File.Delete(path);
					}

                    if (fileExt == ".pdf" || fileExt == ".eml" || fileExt == ".xlsx" || fileExt == ".csv" || fileExt == ".rtf" || fileExt == ".doc" || fileExt == ".docx" || fileExt == ".jpg" || fileExt == ".jpeg" || fileExt == ".png"|| fileExt == ".msg")
                    {
                        DocumentDescription = "Document";
                        file.SaveAs(path);

                        if (System.IO.File.Exists(path))
                        {
                            bool ret = false;
                            ret = dynamicsNAVSOAPServices.documentMgmt.InsertImprestAttachment(DocumentNo, path, 51525003, "Petty Cash");
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

                    //               if (System.IO.File.Exists(path))
                    //{
                    ////	dynamicsNAVSOAPServices.documentMgmt.ModifySystemFileURL(DocumentNo, DocumentCode, fileName);

                    //	string username = ServiceConnection.sharePointUser;
                    //	string Password = ServiceConnection.sharePointUserPassword;

                    //	var securePassword = new SecureString();
                    //	foreach (char c in Password)
                    //	{
                    //		securePassword.AppendChar(c);
                    //	}

                    //	using (var ctx = new ClientContext(ServiceConnection.sharePointSiteUrl))
                    //	{
                    //		ctx.Credentials = new Microsoft.SharePoint.Client.SharePointOnlineCredentials(username, securePassword);
                    //		Web web = ctx.Web;
                    //		ctx.Load(web);

                    //		//Ssl3: Secure Socket Layer (SSL) 3.0 security protocol.
                    //		//Tls: Transport Layer Security (TLS) 1.0 security protocol
                    //		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                    //					   SecurityProtocolType.Tls11 |
                    //					   SecurityProtocolType.Tls12;

                    //		ctx.ExecuteQuery();

                    //		FileCreationInformation newFile = new FileCreationInformation();
                    //		//newFile.Content = System.IO.File.ReadAllBytes(path);
                    //		newFile.ContentStream = new MemoryStream(System.IO.File.ReadAllBytes(path));
                    //		newFile.Url = Path.GetFileName(path);
                    //		newFile.Overwrite = true;

                    //		List byTitle = ctx.Web.Lists.GetByTitle(ServiceConnection.FinanceFolderTitle);
                    //		Folder folder = byTitle.RootFolder.Folders.GetByUrl(ServiceConnection.PettyCashFolder);
                    //		ctx.Load(folder);
                    //		ctx.ExecuteQuery();

                    //		Microsoft.SharePoint.Client.File uploadFile = folder.Files.Add(newFile);
                    //		ctx.Load(byTitle);
                    //		ctx.Load(uploadFile);
                    //		ctx.ExecuteQuery();

                    //		string SharePointUrl = ServiceConnection.sharePointSiteUrl + "/" + ServiceConnection.FinanceFolderTitle + "/" + ServiceConnection.ImprestFolder + "/" + Path.GetFileName(path);

                    //		dynamicsNAVSOAPServices.documentMgmt.UploadFileToSharePointAndNAV(DocumentNo, DocumentCode, fileName, SharePointUrl);
                    //	}

                    //	return Json(new { success = true, message = DocumentDescription + " uploaded successfully" }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //	return Json(new { success = false, message = DocumentDescription + " was not uploaded. Try Again." }, JsonRequestBehavior.AllowGet);
                    //}
                }
				return Json(new { success = false, message = "Please select a pdf file!" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		[Authorize]
		public JsonResult DeletePettyCashDocuments(string DocumentNo)
		{
			bool imprestDocumentsDeleted = false;

			//imprestDocumentsDeleted = dynamicsNAVSOAPServices.fundsManagementWS.DeleteImprestSurrenderUploadedDocument(DocumentNo);

			return Json(new { ImprestDocumentsDeleted = imprestDocumentsDeleted }, JsonRequestBehavior.AllowGet);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _PettyCashDocumentLine(string DocumentNo)
		{
			DocumentMgmtModel documentManagementObj = new DocumentMgmtModel();

			//var imprestUploadedDocuments = from imprestDocumentsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
			//							   where imprestDocumentsQuery.DocumentNo.Equals(DocumentNo)
			//							   select imprestDocumentsQuery;

			dynamic imprestUploadedDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(DocumentNo));

			foreach (var imprestDocument in imprestUploadedDocuments)
			{
				documentManagementObj.LineNo = imprestDocument.LineNo;
				documentManagementObj.DocumentNo = imprestDocument.DocumentNo;
				documentManagementObj.DocumentCode = imprestDocument.DocumentCode;
				documentManagementObj.DocumentDescription = imprestDocument.DocumentDescription;
				documentManagementObj.DocumentAttached = imprestDocument.DocumentAttached;
				documentManagementObj.FileName = imprestDocument.FileName;
			}
			return PartialView(documentManagementObj);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewPettyCashDocumentLine(string HeaderNo)
		{
			DocumentMgmtModel documentManagementObj = new DocumentMgmtModel();

			//var imprestUploadedDocuments = from imprestDocumentsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
			//							   where imprestDocumentsQuery.DocumentNo.Equals(DocumentNo)
			//							   select imprestDocumentsQuery;

			//dynamic imprestUploadedDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(HeaderNo));

			//foreach (var imprestDocument in imprestUploadedDocuments)
			//{
			//	documentManagementObj.LineNo = imprestDocument.LineNo;
			//	documentManagementObj.DocumentNo = imprestDocument.DocumentNo;
			//	documentManagementObj.DocumentCode = imprestDocument.DocumentCode;
			//	documentManagementObj.DocumentDescription = imprestDocument.DocumentDescription;
			//	documentManagementObj.DocumentAttached = imprestDocument.DocumentAttached;
			//	documentManagementObj.FileName = imprestDocument.FileName;
			//}

			return PartialView(documentManagementObj);
		}

		[Authorize]
		public ActionResult GetPettyCashPortalDocumentLink(string LineNo, string DocumentNo, string DocumentCode)
		{
			try 
			{
				DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

				//var imprestDocuments = from imprestDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
				//					   where imprestDocumentQuery.DocumentNo.Equals(DocumentNo) && imprestDocumentQuery.Document_Code.Equals(DocumentCode)
				//					   select imprestDocumentQuery;

				dynamic imprestDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocumentByDocCode(Convert.ToInt32(LineNo), DocumentNo, DocumentCode));

				foreach (var imprestDocument in imprestDocuments)
				{
					documentManagementoBJ.LineNo = imprestDocument.LineNo;
					documentManagementoBJ.DocumentNo = imprestDocument.DocumentNo;
					documentManagementoBJ.DocumentCode = imprestDocument.DocumentCode;
					documentManagementoBJ.DocumentDescription = imprestDocument.DocumentDescription;
					documentManagementoBJ.DocumentAttached = imprestDocument.DocumentAttached;
					documentManagementoBJ.FileName = imprestDocument.FileName;
				}

				return Json(documentManagementoBJ, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		#endregion Document Management

		#region Helper Functions
		public JsonResult GetPettyCashAmount(string DocumentNo)
		{
			decimal pettyCashAmount = 0;
			pettyCashAmount = dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashAmount(DocumentNo);
			return Json(new { Amount = pettyCashAmount }, JsonRequestBehavior.AllowGet);
		}
		public string GetImprestStatus(string DocumentNo)
		{
			return dynamicsNAVSOAPServices.fundsManagementWS.GetImprestStatus(DocumentNo);
		}
		private void LoadCurrencies()
		{
			currencyCodes = from currenciesQuery in dynamicsNAVODataServices.dynamicsNAVOData.Currencies
							select currenciesQuery;
		}
		private void LoadImprestRequestDimensions(string GlobalDimension1Code)
		{
			globalDimension1Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
									where dimensionValuesQuery.Global_Dimension_No.Equals(1) && dimensionValuesQuery.Blocked.Equals(false)
									select dimensionValuesQuery;
			globalDimension2Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
									where dimensionValuesQuery.Global_Dimension_No.Equals(2) && dimensionValuesQuery.Blocked.Equals(false)
									select dimensionValuesQuery;
   //         shortcutDimension3Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
			//						  where dimensionValuesQuery.Global_Dimension_No.Equals(3) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
			//						  select dimensionValuesQuery;
			//shortcutDimension4Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
			//						  where dimensionValuesQuery.Global_Dimension_No.Equals(4) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
			//						  select dimensionValuesQuery;
			//shortcutDimension5Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
			//						  where dimensionValuesQuery.Global_Dimension_No.Equals(5) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
			//						  select dimensionValuesQuery;
			//shortcutDimension6Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
			//						  where dimensionValuesQuery.Global_Dimension_No.Equals(6) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
			//						  select dimensionValuesQuery;
			//shortcutDimension7Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
			//						  where dimensionValuesQuery.Global_Dimension_No.Equals(7) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
			//						  select dimensionValuesQuery;
			//shortcutDimension8Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
			//						  where dimensionValuesQuery.Global_Dimension_No.Equals(8) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
			//						  select dimensionValuesQuery;
		}
		private void LoadImprestRequestCodes()
		{
			 imprestCodes = new List<FundsTransactionModel>();

			//dynamic fundsTransactionCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPettyCashBankAccounts(employeeNo));
			var Chart_of_Accounts = _bcodataServices.BCOData.Chart_of_Accounts.Execute().Select(c => new {c.No, c.Name});
			/*foreach (var fundsTransactionCode in fundsTransactionCodes)
			{
				FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
				FundsTransactionObj.TransactionCode = fundsTransactionCode.No;
				FundsTransactionObj.TransactionDescription = fundsTransactionCode.Name;

				imprestCodes.Add(FundsTransactionObj);
			}*/
			foreach (var chartOfAccount in Chart_of_Accounts)
			{
				FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
				FundsTransactionObj.TransactionCode = chartOfAccount.No;
				FundsTransactionObj.TransactionDescription = $"{chartOfAccount.No}: {chartOfAccount.Name}";

				imprestCodes.Add(FundsTransactionObj);
			}

            impresttransactions = new List<TransactionTypeModel>();
            var Imprest_Transaction_Types = _bcodataServices.BCOData.Imprest_Transaction_Types.Execute().Select(c => new { c.Code , c.Transaction_Name});
            foreach (var ImprestTransactionType in Imprest_Transaction_Types)
            {
                TransactionTypeModel FundsTransactionObj = new TransactionTypeModel();
                //FundsTransactionObj.TransactCode = ImprestTransactionTypes.Transaction_Name;
                //FundsTransactionObj.TransactionDescription = $"{ImprestTransactionTypes.No}: {ImprestTransactionTypes.Name}";

                impresttransactions.Add(FundsTransactionObj);
            }
            //	LoadImprestRequestCodes();
            //imprestLineObj.ImprestCodes = new SelectList(imprestCodes, "TransactionCode", "TransactionDescription");

        }
        //imprestTypes
        private void LoadImprestTypes()
		{
			List<SelectListItem> _imprestTypes = new List<SelectListItem> { new SelectListItem { Text = "1", Value = "Imprest" }, new SelectListItem { Text = "2", Value = "Petty Cash" }/*, new SelectListItem { Text = "3", Value = "Funds Claim" } */};

			imprestTypes = _imprestTypes;
		}
		#endregion Helper Functions

		public ActionResult UpdateLines(string impresttransaction)
		{
			var transactionTypes = _bcodataServices.BCOData.Imprest_Transaction_Types.Execute()
				.FirstOrDefault(c => c.Code == impresttransaction);
			return Json(transactionTypes, JsonRequestBehavior.AllowGet);
		}
    }
}