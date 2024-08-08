using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using DynamicsNAV365_StaffPortal.Models.Finance.Imprest;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.Models.Finance.StaffAdvance;

namespace DynamicsNAV365_StaffPortal.Controllers.FinanceServices
{
    [NoCache]
    public class ImprestController : Controller
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

		List<ImprestCategory> imprestCategoryList = null;

		AccountController accountController = new AccountController();
		string employeeNo = "";

		public ImprestController()
		{
			employeeNo = AccountController.GetEmployeeNo();

		}

		#region New Imprest Request

		[Authorize]
		public ActionResult NewImprestRequest()
		{
			string openImprestNo = "";
			string imprestNo = ""; 

			try
			{
				ImprestHeaderModel imprestRequestObj = new ImprestHeaderModel();

                if (dynamicsNAVSOAPServices.fundsManagementWS.PreviousImprestSurrendered(employeeNo))
                {
					return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
														  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
														  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
				}

				openImprestNo = dynamicsNAVSOAPServices.fundsManagementWS.CheckOpenImprestRequestExists(employeeNo);

				//Check open imprest request
				if (!openImprestNo.Equals(""))
				{
					responseHeader = "Open Imprest Request";
					responseMessage = "An open imprest request No. "+ openImprestNo + " exists under your employee no. " + employeeNo + ", finalize on this imprest before creating a new one.";
					detailedResponseMessage = "An open imprest request No. " + openImprestNo + " exists under your employee no. " + employeeNo + ", finalize on this imprest before creating a new one.";

					button1ControllerName = "Imprest";
					button1ActionName = "ImprestRequestHistory";
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
				imprestNo = dynamicsNAVSOAPServices.fundsManagementWS.CreateImprestHeader(employeeNo);
				return RedirectToAction("EditImprestRequest", new {ImprestNo=imprestNo});
				//End create imprest request

				/*imprestRequestObj.No = imprestNo;
				imprestRequestObj.EmployeeNo = employeeNo;
				imprestRequestObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);
				imprestRequestObj.GlobalDimension1Code = "";

				//LoadCurrencies();
				//LoadImprestRequestDimensions(imprestRequestObj.GlobalDimension1Code);

				return View(imprestRequestObj);*/
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		public ActionResult NewImprestRequest(ImprestHeaderModel ImprestRequestObj)
		{
			bool imprestRequestModified = false;
			bool approvalWorkflowExist = false;

			try
			{
				//LoadCurrencies();
				//LoadImprestRequestDimensions(ImprestRequestObj.GlobalDimension1Code);

				if (ModelState.IsValid)
				{
					if (dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestRequestExists(ImprestRequestObj.No, AccountController.GetEmployeeNo()))
					{
						//Check imprest lines
						if (!dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestLinesExist(ImprestRequestObj.No))
						{
							ImprestRequestObj.ErrorStatus = true;
							ImprestRequestObj.ErrorMessage = "Imprest lines missing, the imprest must contain a minimum of one imprest line, add an imprest line to continue.";
							return View(ImprestRequestObj);
						}
						//Validate imprest lines
						string imprestLineError = "";
						imprestLineError = dynamicsNAVSOAPServices.fundsManagementWS.ValidateImprestLines(ImprestRequestObj.No);
						if (!imprestLineError.Equals(""))
						{
							ImprestRequestObj.ErrorStatus = true;
							ImprestRequestObj.ErrorMessage = imprestLineError;
							return View(ImprestRequestObj);
						}

                        //Modify imprest request
                        ImprestRequestObj.Destination = ImprestRequestObj.Destination != null ? ImprestRequestObj.Destination : "";
                        ImprestRequestObj.DateFrom = ImprestRequestObj.DateFrom != "01/01/0001" ? ImprestRequestObj.DateFrom : DateTime.Now.ToString();
                        ImprestRequestObj.DateTo = ImprestRequestObj.DateTo != "01/01/0001" ? ImprestRequestObj.DateTo : DateTime.Now.ToString();

                        imprestRequestModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyImprestHeader(ImprestRequestObj.No, ImprestRequestObj.EmployeeNo, ImprestRequestObj.Description);
						if (!imprestRequestModified)
						{
							ImprestRequestObj.ErrorStatus = true;
							ImprestRequestObj.ErrorMessage = "An error was experienced while trying to modify imprest no." + ImprestRequestObj.No + ", the server might be offline, try again after a while.";
							return View(ImprestRequestObj);
						}
						//Send imprest for approval
						approvalWorkflowExist = dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestApprovalWorkflowEnabled(ImprestRequestObj.No);
						if (!approvalWorkflowExist)
						{
							ImprestRequestObj.ErrorStatus = true;
							ImprestRequestObj.ErrorMessage = "An error was experienced while trying to send an approval request for imprest no." + ImprestRequestObj.No + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
							return View(ImprestRequestObj);
						}

						if (dynamicsNAVSOAPServices.fundsManagementWS.SendImprestApprovalRequest(ImprestRequestObj.No))
						{
							responseHeader = "Success";
							//responseMessage = "Imprest no." + ImprestRequestObj.No + " was successfully sent for approval. Check with your HOD for approval for approval status.";
							//	detailedResponseMessage = "Imprest no." + ImprestRequestObj.No + " was successfully sent for approval. Check with your HOD for approval for approval status.";
							responseMessage = "Imprest no." + ImprestRequestObj.No + " was successfully sent for approval.";
							detailedResponseMessage = "Imprest no." + ImprestRequestObj.No + " was successfully sent for approval.";

							button1ControllerName = "Imprest";
							button1ActionName = "ImprestRequestHistory";
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
							ImprestRequestObj.ErrorStatus = true;
							ImprestRequestObj.ErrorMessage = "An error was experienced while trying to send an approval request for imprest no." + ImprestRequestObj.No + ". " + ServiceConnection.contactICTDepartment + "";
							return View(ImprestRequestObj);
						}
					}
					else
					{
						responseHeader = "Imprest NotFound";
						responseMessage = "The imprest no." + ImprestRequestObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
						detailedResponseMessage = "The imprest no." + ImprestRequestObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

						button1ControllerName = "Imprest";
						button1ActionName = "ImprestRequestHistory";
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
					return View(ImprestRequestObj);
				}
			}
			catch (Exception ex)
			{
				ImprestRequestObj.ErrorStatus = true;
				ImprestRequestObj.ErrorMessage = ex.Message.ToString();
				return View(ImprestRequestObj);
			}
		}
		#endregion New Imprest Request

		#region Edit Imprest Request

		[Authorize]
		public ActionResult OnBeforeEdit(string ImprestNo)
		{
			try
			{
				if (ImprestNo.Equals(""))
				{
					return RedirectToAction("ImprestRequestHistory", "Imprest");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestRequestExists(ImprestNo, AccountController.GetEmployeeNo()))
				{
					string imprestStatus = GetImprestStatus(ImprestNo);
					//if imprest is open
					if (imprestStatus.Equals("Open"))
					{
						return RedirectToAction("EditImprestRequest", "Imprest", new { ImprestNo = ImprestNo });
					}

					//if imprest is pending approval
					if (imprestStatus.Equals("Pending Approval"))
					{
						responseHeader = "Imprest Request Pending Approval";
						responseMessage = "The imprest no." + ImprestNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
						detailedResponseMessage = "The imprest no." + ImprestNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";

						button1ControllerName = "Imprest";
						button1ActionName = "EditImprestRequest";
						button1HasParameters = true;
						button1Parameters = "?ImprestNo=" + ImprestNo;
						button1Name = "Yes";

						button2ControllerName = "Imprest";
						button2ActionName = "ImprestRequestHistory";
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
						responseHeader = "Imprest Approved";
						responseMessage = "The imprest no." + ImprestNo + " is already approved. Editing not allowed.";
						detailedResponseMessage = "The imprest no." + ImprestNo + " is already approved. Editing not allowed.";

						button1ControllerName = "Imprest";
						button1ActionName = "ImprestRequestHistory";
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
						responseHeader = "Imprest Rejected";
						responseMessage = "The imprest no." + ImprestNo + " was rejected. Editing will reopen the document. Do you want to continue?";
						detailedResponseMessage = "The imprest no." + ImprestNo + " was rejected. Editing will reopen the document. Do you want to continue?";

						button1ControllerName = "Imprest";
						button1ActionName = "EditImprestRequest";
						button1HasParameters = true;
						button1Parameters = "?ImprestNo=" + ImprestNo;
						button1Name = "Yes";

						button2ControllerName = "Imprest";
						button2ActionName = "ImprestRequestHistory";
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
						responseHeader = "Imprest Posted";
						responseMessage = "The imprest no." + ImprestNo + " is already posted. Editing not allowed.";
						detailedResponseMessage = "The imprest no." + ImprestNo + " is already posted. Editing not allowed.";

						button1ControllerName = "Imprest";
						button1ActionName = "ImprestRequestHistory";
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
					return RedirectToAction("ImprestRequestHistory", "Imprest");
				}
				else
				{
					responseHeader = "Imprest NotFound";
					responseMessage = "The imprest no." + ImprestNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The imprest no." + ImprestNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "Imprest";
					button1ActionName = "ImprestRequestHistory";
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
		public ActionResult EditImprestRequest(string ImprestNo)
		{
			try
			{
				if (ImprestNo.Equals(""))
				{
					return RedirectToAction("ImprestRequestHistory", "Imprest");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestRequestExists(ImprestNo, AccountController.GetEmployeeNo()))
				{
					string imprestStatus = dynamicsNAVSOAPServices.fundsManagementWS.GetImprestStatus(ImprestNo);

					//if imprest is pending approval, cancel approval request
					if (imprestStatus.Equals("Pending Approval"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.CancelImprestApprovalRequest(ImprestNo);
					}
					//if imprest is released, reopen and uncommit from budget
					if (imprestStatus.Equals("Released"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.ReopenImprestRequest(ImprestNo);
						dynamicsNAVSOAPServices.fundsManagementWS.CancelImprestBudgetCommitment(ImprestNo);
					}
					//if imprest is rejected, reopen document
					if (imprestStatus.Equals("Rejected"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.ReopenImprestRequest(ImprestNo);
					}

					ImprestHeaderModel imprestRequestObj = new ImprestHeaderModel();
					//var imprestRequests = from imprestRequestsQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestRequests
					//					  where imprestRequestsQuery.No.Equals(ImprestNo)
					//					  select imprestRequestsQuery;

					dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestRequests(ImprestNo, ""));
					foreach (var imprestRequest in imprestRequests)
					{
						imprestRequestObj.No = imprestRequest.No;
						imprestRequestObj.DocumentDate = imprestRequest.DocumentDate;
						imprestRequestObj.PostingDate = imprestRequest.PostingDate;
						imprestRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
						imprestRequestObj.BankAccountName = imprestRequest.BankAccountName;
						imprestRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
						imprestRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
						imprestRequestObj.EmployeeName = imprestRequest.EmployeeName;
                        imprestRequestObj.DateFrom = Convert.ToDateTime(imprestRequest.DateFrom).ToString("dd-MM-yy");//imprestRequest.DateFrom.ToString("dd-MM-yy");

                        imprestRequestObj.DateTo = Convert.ToDateTime(imprestRequest.DateTo).ToString("dd-MM-yy");//imprestRequest.DateTo.ToString("dd-MM-yy");

                        imprestRequestObj.ImprestType = imprestRequest.Type;
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
					}

                    List<DimensionValues> DimensionValues = new List<DimensionValues>();
                    string dimension1list = "DimensionValues?$filter=Global_Dimension_No eq 2 and Blocked eq false &$format=json";

                    HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(dimension1list);
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

                    //LoadCurrencies();
                    //LoadImprestRequestDimensions(imprestRequestObj.GlobalDimension1Code);

                    //imprestRequestObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
                    //imprestRequestObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
                    imprestRequestObj.GlobalDimension2Codes = new SelectList(DimensionValues, "Code", "Code");
					//imprestRequestObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
					//imprestRequestObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
					//imprestRequestObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
					//imprestRequestObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
					//imprestRequestObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
					//imprestRequestObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

					return View(imprestRequestObj);
				}
				else
				{
					responseHeader = "Imprest NotFound";
					responseMessage = "The imprest no." + ImprestNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The imprest no." + ImprestNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "Imprest";
					button1ActionName = "ImprestRequestHistory";
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
		public async Task<ActionResult> EditImprestRequest(ImprestHeaderModel ImprestRequestObj, string Command)
		{
			bool imprestRequestModified = false;
			bool approvalWorkflowExist = false;
            List<DimensionValues> DimensionValues = new List<DimensionValues>();
            string dimension1list = "DimensionValues?$filter=Global_Dimension_No eq 2 and Blocked eq false &$format=json";
            HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(dimension1list);
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
            if (ImprestRequestObj.GlobalDimension2Codes == null)
                ImprestRequestObj.GlobalDimension2Codes = new SelectList(DimensionValues, "Code", "Code");
            try
			{
				if (Command.Equals("Submit For Approval"))
                {
					if (ModelState.IsValid)
					{
						if (dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestRequestExists(ImprestRequestObj.No, AccountController.GetEmployeeNo()))
						{
							//Check imprest lines
							if (!dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestLinesExist(ImprestRequestObj.No))
							{
								ImprestRequestObj.ErrorStatus = true;
								ImprestRequestObj.ErrorMessage = "Imprest lines missing, the imprest must contain a minimum of one imprest line, add an imprest line to continue.";
								return View(ImprestRequestObj);
							}
							//Validate imprest lines
							string imprestLineError = "";
							imprestLineError = dynamicsNAVSOAPServices.fundsManagementWS.ValidateImprestLines(ImprestRequestObj.No);
							if (!imprestLineError.Equals(""))
							{
								ImprestRequestObj.ErrorStatus = true;
								ImprestRequestObj.ErrorMessage = imprestLineError;
								return View(ImprestRequestObj);
							}

							ImprestRequestObj.Destination = ImprestRequestObj.Destination != null ? ImprestRequestObj.Destination : "";
							ImprestRequestObj.DateFrom = ImprestRequestObj.DateFrom != "01/01/0001" ? ImprestRequestObj.DateFrom : DateTime.Now.ToString();
							ImprestRequestObj.DateTo = ImprestRequestObj.DateTo != "01/01/0001" ? ImprestRequestObj.DateTo : DateTime.Now.ToString();
                            //DateTime datefrom = DateTime.ParseExact(ImprestRequestObj.DateFrom.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //DateTime dateto = DateTime.ParseExact(ImprestRequestObj.DateTo, "MM/dd/yyyy", null);

                            imprestRequestModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyImprestHeader(ImprestRequestObj.No, ImprestRequestObj.EmployeeNo,ImprestRequestObj.Description);

							if (!imprestRequestModified)
							{
								ImprestRequestObj.ErrorStatus = true;
								ImprestRequestObj.ErrorMessage = "An error was experienced while trying to modify imprest no." + ImprestRequestObj.No + ", the server might be offline, try again after a while.";
								return View(ImprestRequestObj);
							}
							//Send imprest for approval
							approvalWorkflowExist = dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestApprovalWorkflowEnabled(ImprestRequestObj.No);

							if (!approvalWorkflowExist)
							{
								ImprestRequestObj.ErrorStatus = true;
								ImprestRequestObj.ErrorMessage = "An error was experienced while trying to send an approval request for imprest no." + ImprestRequestObj.No + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
								return View(ImprestRequestObj);
							}

							if (dynamicsNAVSOAPServices.fundsManagementWS.SendImprestApprovalRequest(ImprestRequestObj.No))
							{
								responseHeader = "Success";
								responseMessage = "Imprest no." + ImprestRequestObj.No + " was successfully sent for approval.";
								detailedResponseMessage = "Imprest no." + ImprestRequestObj.No + " was successfully sent for approval.";

								button1ControllerName = "Imprest";
								button1ActionName = "ImprestRequestHistory";
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
								ImprestRequestObj.ErrorStatus = true;
								ImprestRequestObj.ErrorMessage = "An error was experienced while trying to send an approval request for imprest no." + ImprestRequestObj.No + ". " + ServiceConnection.contactICTDepartment + "";
								return View(ImprestRequestObj);
							}
						}
						else
						{
							responseHeader = "Imprest NotFound";
							responseMessage = "The imprest no." + ImprestRequestObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
							detailedResponseMessage = "The imprest no." + ImprestRequestObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

							button1ControllerName = "Imprest";
							button1ActionName = "ImprestRequestHistory";
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
				}
				if (Command.Equals("View Attachment"))
				{
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(ImprestRequestObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                    //using (ClientContext clientContext = new ClientContext(ServiceConnection.sharePointSiteUrl))
                    //{
                    //SecureString passWord = new SecureString();

                    //foreach (char c in ServiceConnection.sharePointUserPassword.ToCharArray()) passWord.AppendChar(c);

                    //clientContext.Credentials = new SharePointOnlineCredentials(ServiceConnection.sharePointUser, passWord);

                    //Web web = clientContext.Web;
                    //clientContext.Load(web);
                    //clientContext.ExecuteQuery();

                    ////string fileURL = clientContext +"/"+ "Finance Management/IMPREST/IMP00021-SUPPORTING DOCUMENT.pdf";

                    //string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(ImprestRequestObj.No);

                    //string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

                    //if (!fileURL.Equals(""))
                    //{
                    //	using (WebClient wc = new WebClient())
                    //	{
                    //if (ext.Equals(".pdf"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/pdf");
                    //}

                    //else if (ext.Equals(".doc"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/msword");
                    //}

                    //else if (ext.Equals(".docx"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                    //}

                    //else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "image/jpeg");
                    //}

                    //else if (ext.Equals(".json"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/json");
                    //}

                    //else if (ext.Equals(".ppt"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.ms-powerpoint");
                    //}

                    //else if (ext.Equals(".png"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "image/png");
                    //}

                    //else if (ext.Equals(".pptx"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
                    //}

                    //else if (ext.Equals(".rar"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.rar");
                    //}

                    //else if (ext.Equals(".xls"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.ms-excel");
                    //}

                    //else if (ext.Equals(".xlsx"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    //}

                    //else
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "text/plain");
                    //}
                    //	}
                    //}

                    //else
                    //{
                    //	return View(ImprestRequestObj);
                    //}
                    //}
                }
				else
				{
					return View(ImprestRequestObj);
				}
			}
			catch (Exception ex)
			{
				ImprestRequestObj.ErrorStatus = true;
				ImprestRequestObj.ErrorMessage = ex.Message.ToString();
				return View(ImprestRequestObj);
			}
		}

		#endregion Edit Imprest Request

		#region View Imprest Request
	
		[Authorize]
		public ActionResult ViewImprestRequest(string ImprestNo)
		{
			try
			{
				if (ImprestNo.Equals(""))
				{
					return RedirectToAction("ImprestRequestHistory", "Imprest");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestRequestExists(ImprestNo, AccountController.GetEmployeeNo()))
				{
					ImprestHeaderModel imprestRequestObj = new ImprestHeaderModel();

					dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestRequests(ImprestNo, ""));
					foreach (var imprestRequest in imprestRequests)
					{
						imprestRequestObj.No = imprestRequest.No;
						imprestRequestObj.DocumentDate = imprestRequest.DocumentDate;
						imprestRequestObj.PostingDate = imprestRequest.PostingDate;
						imprestRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
						imprestRequestObj.BankAccountName = imprestRequest.BankAccountName;
						imprestRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
						imprestRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
						imprestRequestObj.EmployeeName = imprestRequest.EmployeeName;
						imprestRequestObj.DateFrom = imprestRequest.DateFrom.ToString("dd-MM-yy");
						imprestRequestObj.DateTo = imprestRequest.DateTo.ToString("dd-MM-yy");
						imprestRequestObj.ImprestType = imprestRequest.Type;
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
						imprestRequestObj.Comments = dynamicsNAVSOAPServices.ApprovalsMgmt.RejectionComments(imprestRequestObj.No);
						imprestRequestObj.Status = imprestRequest.Status;
					}

					//LoadCurrencies();
					//LoadImprestRequestDimensions(imprestRequestObj.GlobalDimension1Code);

					//imprestRequestObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
					//imprestRequestObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
					//imprestRequestObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
					//imprestRequestObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
					//imprestRequestObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
					//imprestRequestObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
					//imprestRequestObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
					//imprestRequestObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
					//imprestRequestObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

					return View(imprestRequestObj);
				}
				else
				{
					responseHeader = "Imprest Not Found";
					responseMessage = "The imprest no." + ImprestNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The imprest no." + ImprestNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "Imprest";
					button1ActionName = "ImprestRequestHistory";
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
		public async Task<ActionResult> ViewImprestRequest(ImprestHeaderModel pettyCashRequestObj, string Command)
		{
			try
			{
				if (pettyCashRequestObj.No.Equals(""))
				{
					return RedirectToAction("ImprestRequestHistory", "Imprest");
				}
                if (Command.Equals("View Attachment"))
                {
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(pettyCashRequestObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                    // string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(pettyCashRequestObj.No);

                    //if (!fileURL.Equals(""))
                    //{

                    //	using (WebClient wc = new WebClient())
                    //	{
                    //		string ext = Path.GetExtension(fileURL);

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

		[HttpGet]
		public virtual ActionResult Download()
		{
			ClientContext ctx = new ClientContext(ServiceConnection.sharePointSiteUrl);
			SecureString passWord = new SecureString();
			foreach (char c in ServiceConnection.sharePointUserPassword.ToCharArray()) passWord.AppendChar(c);
			ctx.Credentials = new SharePointOnlineCredentials(ServiceConnection.sharePointUser, passWord);
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(ctx, "/sites/Test/testdoc.docx");
			return File(fileInfo.Stream, "application/octet-stream", "testdoc.docx");
		}
		#endregion View Imprest Request

		#region Imprest requests history

		[Authorize]
		public ActionResult ImprestRequestHistory()
		{
			try
			{
				List<ImprestHeaderModel> imprestRequestsList = new List<ImprestHeaderModel>();
				FinanceHomeController financeHomeController = new FinanceHomeController();

				dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestRequests("", employeeNo));

				foreach (var imprestRequest in imprestRequests)
				{
					ImprestHeaderModel imprestRequestObj = new ImprestHeaderModel();
					imprestRequestObj.No = imprestRequest.No;
					imprestRequestObj.DocumentDate = imprestRequest.DocumentDate;
					imprestRequestObj.PostingDate = imprestRequest.PostingDate; 
					imprestRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
					imprestRequestObj.BankAccountName = imprestRequest.BankAccountName;
					imprestRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
					imprestRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
					imprestRequestObj.EmployeeName = imprestRequest.EmployeeName;
					imprestRequestObj.DateFrom = imprestRequest.DateFrom.ToString("dd-MM-yy");
					imprestRequestObj.DateTo = imprestRequest.DateTo.ToString("dd-MM-yy");
					imprestRequestObj.ImprestType = imprestRequest.Type;
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
					imprestRequestObj.Comments = dynamicsNAVSOAPServices.ApprovalsMgmt.RejectionComments(imprestRequestObj.No);
					imprestRequestObj.Status = imprestRequest.Status;

					imprestRequestsList.Add(imprestRequestObj);
				}
				return View(imprestRequestsList);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		public JsonResult GetImprestRequests()
		{
			List<ImprestHeaderModel> imprestRequestsList = new List<ImprestHeaderModel>();
			FinanceHomeController financeHomeController = new FinanceHomeController();

			//var imprestRequests = from imprestRequestsQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestRequests
			//					  where imprestRequestsQuery.HR_Employee_No.Equals(employeeNo)
			//					  select imprestRequestsQuery;

			dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestRequests("", employeeNo));

			foreach (var imprestRequest in imprestRequests)
			{
				ImprestHeaderModel imprestRequestObj = new ImprestHeaderModel();
				imprestRequestObj.No = imprestRequest.No;
				imprestRequestObj.DocumentDate = imprestRequest.DocumentDate;
				imprestRequestObj.PostingDate = imprestRequest.PostingDate;
				imprestRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
				imprestRequestObj.BankAccountName = imprestRequest.BankAccountName;
				imprestRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
				imprestRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
				imprestRequestObj.EmployeeName = imprestRequest.EmployeeName;
				imprestRequestObj.DateFrom = imprestRequest.DateFrom.ToString("dd-MM-yy");
				imprestRequestObj.DateTo = imprestRequest.DateTo.ToString("dd-MM-yy");
				imprestRequestObj.ImprestType = imprestRequest.Type;
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
				imprestRequestsList.Add(imprestRequestObj);
			}
			return Json(imprestRequestsList, JsonRequestBehavior.AllowGet);
		}

		#endregion Imprest requests history

		#region Imprest Line
	
		[ChildActionOnly]
		[Authorize]
		public ActionResult _ImprestLine(string ImprestNo)
		{
			ImprestLineModel imprestLineObj = new ImprestLineModel();

			List<FundsTransactionModel> imprestCodes = new List<FundsTransactionModel>();

			dynamic fundsTransactionCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestTransactionCodes());
			imprestLineObj.CurrencyCodeSelect = Currencys("", "").Select(c => new SelectListItem
			{
				Text = c.Code,
				Value = c.Code
			});
			foreach (var fundsTransactionCode in fundsTransactionCodes)
			{
				FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
				FundsTransactionObj.TransactionCode = fundsTransactionCode.TransactionCode;
				FundsTransactionObj.TransactionDescription = fundsTransactionCode.TransactionDescription;

				imprestCodes.Add(FundsTransactionObj);
			}
            #region Dimension 1 List
            List<DimensionValues> DimensionValues = new List<DimensionValues>();
            string dimension1list = "DimensionValues?$filter=Global_Dimension_No eq 1 and Blocked eq false &$format=json";

            HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(dimension1list);
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

            imprestLineObj.TransactionTypeS = new SelectList(imprestCodes, "TransactionCode", "TransactionDescription");
            imprestLineObj.Dimension1s = new SelectList(DimensionValues, "Code", "Name");
            imprestLineObj.Dimension2s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.Dimension3s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.Dimension4s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.Dimension5s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.Dimension6s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.Dimension7s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.CurrencyCodeSelect = _bcodataServices.BCOData.currency.ToList().Select(c =>
	            new SelectListItem
	            {
		            Value = c.Code,
		            Text = $"{c.Code} : {c.Description}"
	            });
            imprestLineObj.UnitOfMeasureSelect = _bcodataServices.BCOData.Units_of_Measure.ToList().Select(c =>
	            new SelectListItem
	            {
		            Value = c.Code,
		            Text = $"{c.Code} : {c.Description}"
	            });

            return PartialView(imprestLineObj);
		}
		
		public List<Currency> Currencys(string empNo, string no)
		{
			string filterQuery = string.Empty;
			if (!string.IsNullOrEmpty(empNo))
			{
				filterQuery += $"Employee_No eq '{empNo}'";
			}

			if (!string.IsNullOrEmpty(no))
			{
				if (!string.IsNullOrEmpty(filterQuery))
				{
					filterQuery += " and ";
				}

				filterQuery += $"No eq '{no}'";
			}

			var url = string.IsNullOrEmpty(filterQuery)?"currency?$format=json": $"currency?$filter={filterQuery}&$format=json";
			var httpResponseDestForeign = Credentials.GetOdataData(url);
			httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
			using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
			{
				var result1 = streamReader1.ReadToEnd();
				var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<Currency>>(result1);
				return StaffAdvanceList?.ListValues;
			}
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewImprestLine(string DocumentNo)
		{
			ImprestLineModel imprestLineObj = new ImprestLineModel();

			List<FundsTransactionModel> imprestCodes = new List<FundsTransactionModel>();

			dynamic fundsTransactionCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestTransactionCodes());
			foreach (var fundsTransactionCode in fundsTransactionCodes)
			{
				FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
				FundsTransactionObj.TransactionCode = fundsTransactionCode.TransactionCode;
				FundsTransactionObj.TransactionDescription = fundsTransactionCode.TransactionDescription;

				imprestCodes.Add(FundsTransactionObj);
			}

			imprestLineObj.TransactionTypeS = new SelectList(imprestCodes, "TransactionCode", "TransactionDescription");
			LoadImprestCategories();
			imprestLineObj.ImprestCategories = new SelectList(imprestCategoryList, "Code", "Description");
			imprestLineObj.ImprestElements = new SelectList(Enumerable.Empty<SelectListItem>());
			imprestLineObj.SRCDestinations = new SelectList(Enumerable.Empty<SelectListItem>());
			imprestLineObj.SalaryScales = new SelectList(Enumerable.Empty<SelectListItem>());

			return PartialView(imprestLineObj);
		}

		[Authorize]
		public JsonResult GetImprestLinesAjax(string DocumentNo)
		{
			List<ImprestLineModel> imprestLinesList = new List<ImprestLineModel>();
            string imprestlines = "ImprestLines?$filter=Document_No eq '"+DocumentNo+"' &$format=json";

            HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(imprestlines);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    ImprestLineModel imprestLineObj = new ImprestLineModel();
                    imprestLineObj.LineNo = (string)config1["Line_No"];
                    imprestLineObj.DocumentNo = (string)config1["Document_No"];
                    imprestLineObj.TransactionType = (string)config1["Transaction_Type"];
                    imprestLineObj.ImprestCategory = (string)config1["Imprest_Category"];
                    imprestLineObj.ImprestElement = (string)config1["Imprest_Element"];
                    imprestLineObj.SRCDestination = (string)config1["Destination"];
                    imprestLineObj.SalaryScale = (string)config1["Salary_Scale"];
                    imprestLineObj.AccountType = (string)config1["Account_Type"];
                    imprestLineObj.AccountNo = (string)config1["Account_No"];
                    imprestLineObj.AccountName = (string)config1["Transaction_Type"];
                    imprestLineObj.LineDescription = (string)config1["Description"];
                    imprestLineObj.LineAmount = (string)config1["Amount"];
                    imprestLineObj.Days = (string)config1["Quantity"];
                    imprestLineObj.Dimension1 = (string)config1["Shortcut_Dimension_1_Code"];
                    imprestLineObj.Dimension2 = (string)config1["Shortcut_Dimension_2_Code"];
                    imprestLineObj.Dimension3 = (string)config1["ShortcutDimCode3"];
                    imprestLineObj.Dimension4 = (string)config1["ShortcutDimCode4"];
                    imprestLineObj.Dimension5 = (string)config1["ShortcutDimCode5"];
                    imprestLineObj.Dimension6 = (string)config1["ShortcutDimCode6"];
                    imprestLineObj.Dimension7 = (string)config1["ShortcutDimCode7"];
                    imprestLineObj.UnitPrice = (string)config1["Unit_Price"];
                    imprestLineObj.UnitOfMeasure = (string)config1["Unit_of_Measure"];
                    imprestLineObj.CurrencyCode = (string)config1["Currency_Code"];
                    imprestLineObj.Status = (string)config1["Status"];
                    imprestLinesList.Add(imprestLineObj);
                }
            }
            #endregion

            //dynamic imprestLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestRequestLines(DocumentNo));

            //foreach (var imprestLine in imprestLines)
            //{
            //	ImprestLineModel imprestLineObj = new ImprestLineModel();
            //	imprestLineObj.LineNo = imprestLine.LineNo;
            //	imprestLineObj.DocumentNo = imprestLine.DocumentNo;
            //	imprestLineObj.TransactionType = imprestLine.ImprestCode;
            //	imprestLineObj.ImprestCategory = imprestLine.TravelType;
            //	imprestLineObj.ImprestElement = imprestLine.AdvanceType;
            //	imprestLineObj.SRCDestination = imprestLine.Destination;
            //	imprestLineObj.SalaryScale = imprestLine.SalaryScale;
            //	//imprestLineObj.FromCity = imprestLine.FromCity;
            //	imprestLineObj.AccountType = imprestLine.AccountType;
            //	//imprestLineObj.ToCity = imprestLine.ToCity;
            //	imprestLineObj.AccountNo = imprestLine.AccountNo;
            //	imprestLineObj.AccountName = imprestLine.AccountName;
            //	imprestLineObj.LineDescription = imprestLine.LineDescription;
            //	imprestLineObj.LineAmount = imprestLine.LineAmount;
            //	imprestLineObj.Days = imprestLine.Days;
            //             imprestLinesList.Add(imprestLineObj);
            //}
            return Json(imprestLinesList, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult GetImprestLine(string LineNo, string DocumentNo)
		{
			ImprestLineModel imprestLineObj = new ImprestLineModel();

			dynamic imprestLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestRequestByLine(Convert.ToInt32(LineNo), DocumentNo));
			foreach (var imprestLine in imprestLines)
			{
				imprestLineObj.LineNo = imprestLine.LineNo;
				imprestLineObj.DocumentNo = imprestLine.DocumentNo;
				imprestLineObj.TransactionType = imprestLine.ImprestCode;
				imprestLineObj.ImprestCategory = imprestLine.TravelType;
				imprestLineObj.ImprestElement = imprestLine.AdvanceType;
				imprestLineObj.SRCDestination = imprestLine.Destination;
				imprestLineObj.SalaryScale = imprestLine.SalaryScale;
				//imprestLineObj.FromCity = imprestLine.FromCity;
				imprestLineObj.AccountType = imprestLine.AccountType;
			//	imprestLineObj.ToCity = imprestLine.ToCity;
				imprestLineObj.AccountNo = imprestLine.AccountNo;
				imprestLineObj.AccountName = imprestLine.AccountName;
				imprestLineObj.LineDescription = imprestLine.LineDescription;
				imprestLineObj.LineAmount = imprestLine.LineAmount;
				imprestLineObj.Days = imprestLine.Days;
				imprestLineObj.UnitPrice = imprestLine.UnitPrice;
				imprestLineObj.CurrencyCode = imprestLine.CurrencyCode;
				imprestLineObj.UnitOfMeasure = imprestLine.UnitofMeasure;
			}

			return Json(imprestLineObj, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult CreateImprestLine(ImprestLineModel ImprestLineObj)
		{

			try
			{
				bool imprestLineCreated = false;
				//DateTime datefrom = DateTime.ParseExact(ImprestLineObj.StartDate, "MM/dd/yyyy",
				//                           System.Globalization.CultureInfo.InvariantCulture);
				//DateTime dateto = DateTime.ParseExact(ImprestLineObj.EndDate, "MM-dd-yyyy",
				//                           System.Globalization.CultureInfo.InvariantCulture);

				ImprestLineObj.Days = ImprestLineObj.Days != null ? ImprestLineObj.Days : "0";

				if (Convert.ToDecimal(ImprestLineObj.LineAmount) < 0)
				{
					return Json(new { success = false, message = "The system cannot allow you to add/save zero "+ (ImprestLineObj.LineAmount) +" amount. Please type amount or liaise with finance for assistance." }, JsonRequestBehavior.AllowGet);
				}
				if (ImprestLineObj.HeaderDescripton == "")
				{
					return Json(new { success = false, message = "Kindly add header description" }, JsonRequestBehavior.AllowGet);
				}

				if (ImprestLineObj.StartDate == null)
				{
					return Json(new { success = false, message = "Kindly add start date" }, JsonRequestBehavior.AllowGet);
				}
				if (ImprestLineObj.EndDate == null)
				{
					return Json(new { success = false, message = "Kindly add end date" }, JsonRequestBehavior.AllowGet);
				}

				/*imprestLineCreated = dynamicsNAVSOAPServices.fundsManagementWS.CreateImprestLine(ImprestLineObj.DocumentNo, ImprestLineObj.TransactionType, 
					Convert.ToDecimal(ImprestLineObj.Days),Convert.ToDecimal(ImprestLineObj.LineAmount), 
					ImprestLineObj.LineDescription??"", ImprestLineObj.LineGlobalDimension1Code??"", ImprestLineObj.LineGlobalDimension2Code??"", ImprestLineObj.LineShortcutDimension3Code ?? "", ImprestLineObj.LineShortcutDimension4Code ?? "",
					ImprestLineObj.LineShortcutDimension5Code ?? "", ImprestLineObj.LineShortcutDimension6Code ?? "", ImprestLineObj.LineShortcutDimension7Code ?? "",
					DateTime.Parse(ImprestLineObj.StartDate), DateTime.Parse(ImprestLineObj.EndDate), ImprestLineObj.HeaderDescripton,Convert.ToDecimal(ImprestLineObj.UnitPrice),ImprestLineObj.CurrencyCode??"",Convert.ToDecimal(ImprestLineObj.Days),ImprestLineObj.UnitOfMeasure??"");*/
				return Json(imprestLineCreated ? new { success = true, message = "imprest line added successfully." } : new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
			}			
		}

		public ActionResult ApprovalCommentLines(string documentNo, string documentType)
		{
			var commentLines = _bcodataServices.BCOData.ApprovalCommentLine.ToList().Where(c=>c.Record_ID_to_Approve == $"{documentType}: {documentNo}");
			return PartialView(commentLines);
		}
		
		[Authorize]
		public JsonResult CreateImprestLineForAdvance(ImprestLineModel ImprestLineObj)
		{
			try
			{
				bool imprestLineCreated = false;
				//DateTime datefrom = DateTime.ParseExact(ImprestLineObj.StartDate, "MM/dd/yyyy",
				//                           System.Globalization.CultureInfo.InvariantCulture);
				//DateTime dateto = DateTime.ParseExact(ImprestLineObj.EndDate, "MM-dd-yyyy",
				//                           System.Globalization.CultureInfo.InvariantCulture);

				ImprestLineObj.Days = ImprestLineObj.Days != null ? ImprestLineObj.Days : "0";

				if (Convert.ToDecimal(ImprestLineObj.LineAmount) < 0)
				{
					return Json(new { success = false, message = "The system cannot allow you to add/save zero "+ (ImprestLineObj.LineAmount) +" amount. Please type amount or liaise with finance for assistance." }, JsonRequestBehavior.AllowGet);
				}
				if (ImprestLineObj.HeaderDescripton == "")
				{
					return Json(new { success = false, message = "Kindly add header description" }, JsonRequestBehavior.AllowGet);
				}

				imprestLineCreated = dynamicsNAVSOAPServices.StaffAdvance.CreateImprestLine(ImprestLineObj.DocumentNo, ImprestLineObj.TransactionType, 
					Convert.ToDecimal(ImprestLineObj.Days),Convert.ToDecimal(ImprestLineObj.LineAmount), 
					ImprestLineObj.LineDescription??"", ImprestLineObj.LineGlobalDimension1Code??"", ImprestLineObj.LineGlobalDimension2Code??"", ImprestLineObj.LineShortcutDimension3Code ?? "", ImprestLineObj.LineShortcutDimension4Code ?? "",
					ImprestLineObj.LineShortcutDimension5Code ?? "", ImprestLineObj.LineShortcutDimension6Code ?? "", ImprestLineObj.LineShortcutDimension7Code ?? "");
				return Json(imprestLineCreated ? new { success = true, message = "imprest line added successfully." } : new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
			}
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
                filename = "TravelRequest_" + employeeNo + "_" + DocNo + ".pdf";
                filenane = Credentials.ObjNav.GenerateTravelReport(filename,DocNo);
                return Json(new { message = "https://ess.cihebkenya.org/reports/" + filename, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
		public JsonResult ModifyImprestLine(ImprestLineModel ImprestLineObj, bool? checkDate)
		{
			try
			{
				bool imprestLineModified = false;
				//var StartDate = DateTime.ParseExact(ImprestLineObj.StartDate, "MM-dd-yyyy",System.Globalization.CultureInfo.InvariantCulture);
				//var EndDate = DateTime.ParseExact(ImprestLineObj.EndDate, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);

            

			

				ImprestLineObj.Days = ImprestLineObj.Days != null ? ImprestLineObj.Days : "0";

				if (Convert.ToDecimal(ImprestLineObj.LineAmount) < 0)
				{
					return Json(new { success = false, message = "The system cannot allow you to add/save zero " + (ImprestLineObj.LineAmount) + " amount. Please type amount or liaise with finance for assistance." }, JsonRequestBehavior.AllowGet);
				}
				if (ImprestLineObj.HeaderDescripton == "")
				{
					return Json(new { success = false, message = "Kindly add header description" }, JsonRequestBehavior.AllowGet);
				}

				if (ImprestLineObj.StartDate == null && checkDate == true)
				{
					return Json(new { success = false, message = "Kindly add start date" }, JsonRequestBehavior.AllowGet);
				}
				if (ImprestLineObj.EndDate == null && checkDate == true)
				{
					return Json(new { success = false, message = "Kindly add end date" }, JsonRequestBehavior.AllowGet);
				}

				/*imprestLineModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyImprestLine(Convert.ToInt32(ImprestLineObj.LineNo), ImprestLineObj.DocumentNo, ImprestLineObj.TransactionType, "",
					ImprestLineObj.ImprestElement ?? "", ImprestLineObj.SalaryScale ?? "", ImprestLineObj.SRCDestination ?? "", Convert.ToDecimal(ImprestLineObj.Days),
					Convert.ToDecimal(ImprestLineObj.LineAmount), ImprestLineObj.LineDescription ?? "", ImprestLineObj.LineGlobalDimension1Code ?? "", 
					ImprestLineObj.LineGlobalDimension2Code ?? "", ImprestLineObj.LineShortcutDimension3Code ?? "", 
					ImprestLineObj.LineShortcutDimension4Code ?? "", ImprestLineObj.LineShortcutDimension5Code ?? "", ImprestLineObj.LineShortcutDimension6Code ?? "", ImprestLineObj.LineShortcutDimension7Code ?? "",ImprestLineObj.UnitOfMeasure??""
					,ImprestLineObj.CurrencyCode??"",Convert.ToDecimal(ImprestLineObj.UnitPrice??decimal.Zero.ToString()));*/

				if (imprestLineModified)
				{
					//return Json(new { ImprestLineCreated = imprestLineCreated }, JsonRequestBehavior.AllowGet);
					return Json(new { success = true, message = "imprest line added successfully." }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception e)
			{
				return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
			}
        }

		[Authorize]
		public JsonResult DeleteImprestLine(int LineNo, string DocumentNo)
		{
			bool imprestLineDeleted = false;

			imprestLineDeleted = dynamicsNAVSOAPServices.fundsManagementWS.DeleteImprestLine(LineNo, DocumentNo);

			return Json(new { ImprestLineDeleted = imprestLineDeleted }, JsonRequestBehavior.AllowGet);
		}

		#region Imprest Approval
	
		[Authorize]
		public ActionResult ImprestApproval(string ImprestNo)
		{
			try
			{
				if (ImprestNo.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}


				ImprestHeaderModel imprestRequestObj = new ImprestHeaderModel();

				//var imprestRequests = from imprestRequestsQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestRequests
				//					  where imprestRequestsQuery.No.Equals(ImprestNo)
				//					  select imprestRequestsQuery;

				dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestRequests(ImprestNo, ""));

				foreach (var imprestRequest in imprestRequests)
				{
					imprestRequestObj.No = imprestRequest.No;
					imprestRequestObj.DocumentDate = imprestRequest.DocumentDate;
					imprestRequestObj.PostingDate = imprestRequest.PostingDate;
					imprestRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
					imprestRequestObj.BankAccountName = imprestRequest.BankAccountName;
					imprestRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
					imprestRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
					imprestRequestObj.EmployeeName = imprestRequest.EmployeeName;
					imprestRequestObj.DateFrom = imprestRequest.DateFrom.ToString("dd-MM-yy");
					imprestRequestObj.DateTo = imprestRequest.DateTo.ToString("dd-MM-yy");
					imprestRequestObj.ImprestType = imprestRequest.Type;
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
				}

				//LoadCurrencies();
				//LoadImprestRequestDimensions(imprestRequestObj.GlobalDimension1Code);

				//imprestRequestObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
				//imprestRequestObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
				//imprestRequestObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
				//imprestRequestObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
				//imprestRequestObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
				//imprestRequestObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
				//imprestRequestObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
				//imprestRequestObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
				//imprestRequestObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

				return View(imprestRequestObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ImprestApproval(ImprestHeaderModel ImprestRequestObj, string Command)
		{
			try
			{
				if (ImprestRequestObj.No.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}
				if (Command == "Approve")
				{
					ImprestRequestObj.Comments = ImprestRequestObj.Comments != null ? ImprestRequestObj.Comments : "";
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveImprestRequest(employeeNo, ImprestRequestObj.No, AccountController.GetEmployeeNo()))
					{
                        TempData["Success"] = "Imprest Request no." + ImprestRequestObj.No + " was successfully approved.";
                        return RedirectToAction("OpenEntries", "Approval");
                        //responseHeader = "Success";
                        //responseMessage = "Imprest Request no." + ImprestRequestObj.No + " was successfully approved.";
                        //detailedResponseMessage = "Imprest Request no." + ImprestRequestObj.No + " was successfully approved.";

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
                        return View(ImprestRequestObj);
                        //ImprestRequestObj.ErrorStatus = true;
                        //ImprestRequestObj.ErrorMessage = "Unable to process the imprest request approve action. " + ServiceConnection.contactICTDepartment + "";
                        //return View(ImprestRequestObj);
                    }
				}

				if (Command == "Reject")
				{
					ImprestRequestObj.Comments = ImprestRequestObj.Comments != null ? ImprestRequestObj.Comments : "";
					if (ImprestRequestObj.Comments.Equals(""))
					{
						dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestRequests(ImprestRequestObj.No, ""));

						foreach (var imprestRequest in imprestRequests)
						{
							ImprestRequestObj.No = imprestRequest.No;
							ImprestRequestObj.DocumentDate = imprestRequest.DocumentDate;
							ImprestRequestObj.PostingDate = imprestRequest.PostingDate;
							ImprestRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
							ImprestRequestObj.BankAccountName = imprestRequest.BankAccountName;
							ImprestRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
							ImprestRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
							ImprestRequestObj.EmployeeName = imprestRequest.EmployeeName;
							ImprestRequestObj.DateFrom = imprestRequest.DateFrom.ToString("dd-MM-yy");
							ImprestRequestObj.DateTo = imprestRequest.DateTo.ToString("dd-MM-yy");
							ImprestRequestObj.ImprestType = imprestRequest.Type;
							ImprestRequestObj.CurrencyCode = imprestRequest.ImprestType;
							ImprestRequestObj.Description = imprestRequest.Description;
							ImprestRequestObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
							ImprestRequestObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
							ImprestRequestObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
							ImprestRequestObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
							ImprestRequestObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
							ImprestRequestObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
							ImprestRequestObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
							ImprestRequestObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
							ImprestRequestObj.Amount = imprestRequest.Amount;

						}
                        TempData["Error"] = "Kindly provide reason (s) for declining/rejecting this document.";
                        return View(ImprestRequestObj);
                        //ImprestRequestObj.ErrorStatus = true;
                        //ImprestRequestObj.ErrorMessage = "Kindly provide reason (s) for declining/rejecting this document.";
                        //return View(ImprestRequestObj);
                    }

					if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectImprestRequest(employeeNo, ImprestRequestObj.No, ImprestRequestObj.Comments, AccountController.GetEmployeeNo()))
					{
                        TempData["Success"] = "Imprest Request no." + ImprestRequestObj.No + " was successfully rejected.";
                        return RedirectToAction("OpenEntries", "Approval");
                        //responseHeader = "Success";
                        //responseMessage = "Imprest request no." + ImprestRequestObj.No + " was successfully rejected.";
                        //detailedResponseMessage = "Imprest request no." + ImprestRequestObj.No + " was successfully rejected.";

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
                        return View(ImprestRequestObj);
                        //ImprestRequestObj.ErrorStatus = true;
                        //ImprestRequestObj.ErrorMessage = "Unable to process the imprest request reject action. " + ServiceConnection.contactICTDepartment + "";
                        //return View(ImprestRequestObj);
                    }
				}
				else if (Command == "View Attachment")
				{

                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(ImprestRequestObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                    //               string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

                    //if (!fileURL.Equals(""))
                    //{
                    //	using (WebClient wc = new WebClient())
                    //	{
                    //if (ext.Equals(".pdf"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/pdf");
                    //}

                    //else if (ext.Equals(".doc"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/msword");
                    //}

                    //else if (ext.Equals(".docx"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                    //}

                    //else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "image/jpeg");
                    //}

                    //else if (ext.Equals(".json"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/json");
                    //}

                    //else if (ext.Equals(".ppt"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.ms-powerpoint");
                    //}

                    //else if (ext.Equals(".png"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "image/png");
                    //}

                    //else if (ext.Equals(".pptx"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
                    //}

                    //else if (ext.Equals(".rar"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.rar");
                    //}

                    //else if (ext.Equals(".xls"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.ms-excel");
                    //}

                    //else if (ext.Equals(".xlsx"))
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    //}

                    //else
                    //{
                    //	var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //	return File(byteArr, "text/plain");
                    //}
                    //	}
                    //}


                    //else
                    //{
                    //	return View(ImprestRequestObj);
                    //}
                }
				else
				{
                    TempData["Error"] = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                    return View(ImprestRequestObj);
                    //ImprestRequestObj.ErrorStatus = true;
                    //ImprestRequestObj.ErrorMessage = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                    //return View(ImprestRequestObj);
                }
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		#endregion Imprest Approval

		#region Document Management

		[Authorize]
		public JsonResult GetImprestPortalDocuments(string DocumentNo)
		{
			List<DocumentMgmtModel> documentsList = new List<DocumentMgmtModel>();

			dynamic imprestDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(DocumentNo));

			foreach (var imprestDocument in imprestDocuments)
			{
				DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();
				documentManagementoBJ.LineNo = imprestDocument.LineNo;
				documentManagementoBJ.DocumentNo = imprestDocument.DocumentNo;
				documentManagementoBJ.DocumentCode = imprestDocument.DocumentCode;
				documentManagementoBJ.DocumentDescription = imprestDocument.DocumentDescription;
				documentManagementoBJ.DocumentAttached = imprestDocument.DocumentAttached;
				documentManagementoBJ.FileName = imprestDocument.FileName;

				documentsList.Add(documentManagementoBJ);
			}
			return Json(documentsList.ToList(), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		[HttpPost]
		public JsonResult UploadImprestDocumentLink(string DocumentNo, string DocumentCode, string DocumentDescription)
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
                            ret = dynamicsNAVSOAPServices.documentMgmt.InsertImprestAttachment(DocumentNo, path, 51525003, "Travel Request");
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
		public JsonResult DeleteImprestDocuments(string DocumentNo)
		{
			bool imprestDocumentsDeleted = false;

			//imprestDocumentsDeleted = dynamicsNAVSOAPServices.fundsManagementWS.DeleteImprestSurrenderUploadedDocument(DocumentNo);

			return Json(new { ImprestDocumentsDeleted = imprestDocumentsDeleted }, JsonRequestBehavior.AllowGet);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ImprestDocumentLine(string DocumentNo)
		{
			DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

            var imprestUploadedDocuments = from imprestDocumentsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
                                           where imprestDocumentsQuery.DocumentNo.Equals(DocumentNo)
                                           select imprestDocumentsQuery;

            dynamic imprestDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(DocumentNo));

            foreach (var imprestDocument in imprestDocuments)
            {
                documentManagementoBJ.LineNo = imprestDocument.LineNo;
                documentManagementoBJ.DocumentNo = imprestDocument.DocumentNo;
                documentManagementoBJ.DocumentCode = imprestDocument.DocumentCode;
                documentManagementoBJ.DocumentDescription = imprestDocument.DocumentDescription;
                documentManagementoBJ.DocumentAttached = imprestDocument.DocumentAttached ?? false;
                documentManagementoBJ.FileName = imprestDocument.FileName;
            }
            return PartialView(documentManagementoBJ);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewImprestDocumentLine(string DocumentNo)
		{
			DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

			//var imprestUploadedDocuments = from imprestDocumentsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
			//							   where imprestDocumentsQuery.DocumentNo.Equals(DocumentNo)
			//							   select imprestDocumentsQuery;

			//dynamic imprestDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(DocumentNo));

			//foreach (var imprestDocument in imprestDocuments)
			//{
			//	documentManagementoBJ.LineNo = imprestDocument.LineNo;
			//	documentManagementoBJ.DocumentNo = imprestDocument.DocumentNo;
			//	documentManagementoBJ.DocumentCode = imprestDocument.DocumentCode;
			//	documentManagementoBJ.DocumentDescription = imprestDocument.DocumentDescription;
			//	documentManagementoBJ.DocumentAttached = imprestDocument.DocumentAttached ?? false;
			//	documentManagementoBJ.FileName = imprestDocument.FileName;
			//}
			return PartialView(documentManagementoBJ);
		}

		[Authorize]
		public ActionResult GetImprestPortalDocumentLink(string LineNo,string DocumentNo, string DocumentCode)
		{
			try
			{
				DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

				//var imprestDocuments = from imprestDocumentQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
				//					   where imprestDocumentQuery.DocumentNo.Equals(DocumentNo) && imprestDocumentQuery.Document_Code.Equals(DocumentCode)
				//					   select imprestDocumentQuery;

				dynamic imprestDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocumentByDocCode(Convert.ToInt32(LineNo),DocumentNo, DocumentCode));

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
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateHeader(string Description, string DateFrom, string GlobalDimension2Code, string DateTo, string DocNo)
        {
            try
            {
                bool ret;
                bool successVal = false;
                string msg = "";

                ret = Credentials.ObjNav.UpdateImprestRequestHeader(DocNo, Description, GlobalDimension2Code, DateTime.Parse(DateFrom), DateTime.Parse(DateTo));
                if (ret)
                {
                    msg = "Updated Successfully";
                    successVal = true;
                }
                else
                {
                    msg = "Details Failed to Update";
                    successVal = true;
                }

                return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Document Management

        #region Helper Functions
        public string GetImprestStatus(string DocumentNo)
		{
			return dynamicsNAVSOAPServices.fundsManagementWS.GetImprestStatus(DocumentNo);
		}
		private void LoadCurrencies()
		{
			currencyCodes = from currenciesQuery in dynamicsNAVODataServices.dynamicsNAVOData.Currencies
							select currenciesQuery;
		}
		//private void LoadImprestRequestDimensions(string GlobalDimension1Code)
		//{
		//	globalDimension1Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
		//							where dimensionValuesQuery.Global_Dimension_No.Equals(1) && dimensionValuesQuery.Blocked.Equals(false)
		//							select dimensionValuesQuery;
		//	globalDimension2Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
		//							where dimensionValuesQuery.Global_Dimension_No.Equals(2) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
		//							select dimensionValuesQuery;
		//	shortcutDimension3Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
		//							  where dimensionValuesQuery.Global_Dimension_No.Equals(3) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
		//							  select dimensionValuesQuery;
		//	shortcutDimension4Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
		//							  where dimensionValuesQuery.Global_Dimension_No.Equals(4) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
		//							  select dimensionValuesQuery;
		//	shortcutDimension5Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
		//							  where dimensionValuesQuery.Global_Dimension_No.Equals(5) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
		//							  select dimensionValuesQuery;
		//	shortcutDimension6Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
		//							  where dimensionValuesQuery.Global_Dimension_No.Equals(6) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
		//							  select dimensionValuesQuery;
		//	shortcutDimension7Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
		//							  where dimensionValuesQuery.Global_Dimension_No.Equals(7) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
		//							  select dimensionValuesQuery;
		//	shortcutDimension8Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
		//							  where dimensionValuesQuery.Global_Dimension_No.Equals(8) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) && dimensionValuesQuery.Blocked.Equals(false)
		//							  select dimensionValuesQuery;
		//}
		private void LoadImprestRequestCodes()
		{

			//imprestCodes = from imprestCodesQuery in dynamicsNAVODataServices.dynamicsNAVOData.FundsTransactionCodes
			//			   select imprestCodesQuery;

			List<FundsTransactionModel> imprestCodes = new List<FundsTransactionModel>();

			dynamic fundsTransactionCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestTransactionCodes());
			foreach (var fundsTransactionCode in fundsTransactionCodes)
			{
				FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
				FundsTransactionObj.TransactionCode = fundsTransactionCode.TransactionCode;
				FundsTransactionObj.TransactionDescription = fundsTransactionCode.TransactionDescription;

				imprestCodes.Add(FundsTransactionObj);
			}
		}
		private void LoadImprestCategories()
		{
			 imprestCategoryList = new List<ImprestCategory>(); 

			dynamic imprestCategoryCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestCategories());
			foreach (var imprestCategoryCode in imprestCategoryCodes)
			{
				ImprestCategory imprestCategoryObj = new ImprestCategory();
				imprestCategoryObj.Code = imprestCategoryCode.Code;
				imprestCategoryObj.Description = imprestCategoryCode.Description;

				imprestCategoryList.Add(imprestCategoryObj);
			}
		}
		public JsonResult GetImprestElements(string imprestCategory)
		{
			List<ImprestElements> imprestElementList = new List<ImprestElements>();

			dynamic imprestElementCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestElements(imprestCategory));
			foreach (var imprestElementCode in imprestElementCodes)
			{
				ImprestElements imprestElementObj = new ImprestElements();
				imprestElementObj.Code = imprestElementCode.Code;
				imprestElementObj.Description = imprestElementCode.Description;

				imprestElementList.Add(imprestElementObj);
			}

			return Json(imprestElementList.ToList(), JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetEmployeeSalaryScale() 
		{
			string salaryScale = "";
			salaryScale = dynamicsNAVSOAPServices.fundsManagementWS.GetEmployeeSRCScale(employeeNo); 
			return Json(new { SalaryScale = salaryScale }, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetSalaryScales()
		{
			List<SalaryScales> srcList = new List<SalaryScales>();

			dynamic srcCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetSalaryScales());
			foreach (var srcCode in srcCodes)
			{
				SalaryScales srcObj = new SalaryScales();
				srcObj.Code = srcCode.Code;
				srcObj.Name = srcCode.Name;

				srcList.Add(srcObj);
			}

			return Json(srcList.ToList(), JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetSRCLocalDestinations()
		{
			List<SRCLocalDestinations> srcLocalDestinationList = new List<SRCLocalDestinations>();

			dynamic srcLocalDestinationCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetLocalSRCDestionations());
			foreach (var srcLocalDestinationCode in srcLocalDestinationCodes)
			{
				SRCLocalDestinations srcLocalDestinationObj = new SRCLocalDestinations();
				srcLocalDestinationObj.Code = srcLocalDestinationCode.Code;
				srcLocalDestinationObj.Name = srcLocalDestinationCode.Name;

				srcLocalDestinationList.Add(srcLocalDestinationObj);
			}

			return Json(srcLocalDestinationList.ToList(), JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetSRCInternationalDestinations()  
		{
			List<SRCInternationalDestinations> srcInternationalDestinationList = new List<SRCInternationalDestinations>();

			dynamic srcInternationalDestinationCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetInternationalSRCDestionations());
			foreach (var srcInternationalDestinationCode in srcInternationalDestinationCodes)
			{
				SRCInternationalDestinations srcInternationalDestinationObj = new SRCInternationalDestinations();
				srcInternationalDestinationObj.Code = srcInternationalDestinationCode.Code;
				srcInternationalDestinationObj.Name = srcInternationalDestinationCode.Name;

				srcInternationalDestinationList.Add(srcInternationalDestinationObj);
			}

			return Json(srcInternationalDestinationList.ToList(), JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetSRCAmount(string SRCScale,string Destination,decimal Qty)
		{
			decimal srcAmount = 0;
			srcAmount = dynamicsNAVSOAPServices.fundsManagementWS.GetSRCAmount(SRCScale, Destination);

            if (Qty > 0)
            {
				srcAmount= srcAmount*Qty;

			}

			return Json(new { LineAmount = srcAmount }, JsonRequestBehavior.AllowGet); 
		}
		public JsonResult GetSRCAmountCopy(string salaryScale, string clusterDestination) 
		{
			decimal srcAmount = 0;
			srcAmount = dynamicsNAVSOAPServices.fundsManagementWS.GetSRCAmount(salaryScale, clusterDestination);
			return Json(new { LineAmount = srcAmount }, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetImprestAmountPerCategory(string transactionType, string imprestCategory,string imprestElement,string kTNAJobGroup,string destination, string unit)
		{
			decimal lineAmount = 0;
			unit = unit != "" ? unit : "1";

			//lineAmount = dynamicsNAVSOAPServices.fundsManagementWS.GetImprestAmountPerCategory(transactionType, imprestCategory,imprestElement,kTNAJobGroup,destination,Convert.ToDecimal(unit));
			return Json(new { LineAmount = lineAmount }, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetImprestAmount(string DocumentNo)
		{
			decimal imprestAmount = 0;
			imprestAmount = dynamicsNAVSOAPServices.fundsManagementWS.GetImprestAmount(DocumentNo);
			return Json(new { Amount = imprestAmount }, JsonRequestBehavior.AllowGet);
		}
		private static void UploadFileToSharePoint(string SiteUrl, string DocLibrary, string ClientSubFolder, string FileName, string Login, string Password)
		{
			try
			{
				#region ConnectToSharePoint
				var securePassword = new SecureString();
				foreach (char c in Password)
				{ securePassword.AppendChar(c); }
				var onlineCredentials = new SharePointOnlineCredentials(Login, securePassword);
				#endregion
				#region Insert the data
				using (ClientContext CContext = new ClientContext(SiteUrl))
				{
					CContext.Credentials = onlineCredentials;
					Web web = CContext.Web;
					FileCreationInformation newFile = new FileCreationInformation();
					byte[] FileContent = System.IO.File.ReadAllBytes(FileName);
					newFile.ContentStream = new MemoryStream(FileContent);
					newFile.Url = Path.GetFileName(FileName);
					List DocumentLibrary = web.Lists.GetByTitle(DocLibrary);
					Folder Clientfolder = null;
					if (ClientSubFolder == "")
					{
						Clientfolder = DocumentLibrary.RootFolder;
					}
					else
					{
						Clientfolder = DocumentLibrary.RootFolder.Folders.Add(ClientSubFolder);
						Clientfolder.Update();
					}
					Microsoft.SharePoint.Client.File uploadFile = Clientfolder.Files.Add(newFile);
					CContext.Load(DocumentLibrary);
					CContext.Load(uploadFile);
					CContext.ExecuteQuery();
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("The File has been uploaded" + Environment.NewLine + "FileUrl -->" + SiteUrl + "/" + DocLibrary + "/" + ClientSubFolder + "/" + Path.GetFileName(FileName));
				}
				#endregion
			}
			catch (Exception exp)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(exp.Message + Environment.NewLine + exp.StackTrace);
			}
			finally
			{
				Console.ReadLine();
			}
		}
		//private static void UploadFileToSharePointCopy(string SiteUrl, string DocLibrary, string ClientSubFolder, string FileName, string Login, string Password)
		//{
		//	using (SP.ClientContext cnx = new SP.ClientContext("https://test.sharepoint.com/sites/test01"))
		//	{
		//		//string password = "xxx";
		//		//string account = "someone@mail.com";
		//		var secret = new SecureString();
		//		//string docLibrary = "Contact";
		//		//string folderName = "demo";
		//		//string filename = "RockN'Roll.pdf";
		//		foreach (char c in Password)
		//		{
		//			secret.AppendChar(c);
		//		}
		//		cnx.Credentials = new SP.SharePointOnlineCredentials(Login, secret);
		//		SP.Web web = cnx.Web;
		//		cnx.Load(web, website => website.Lists, website => website.ServerRelativeUrl);
		//		cnx.ExecuteQuery();
		//		SP.List list = web.Lists.GetByTitle(DocLibrary);
		//		cnx.Load(list, i => i.RootFolder.Folders, i => i.RootFolder);
		//		cnx.ExecuteQuery();
		//		var folderToBindTo = list.RootFolder.Folders;
		//		var folderToUpload = folderToBindTo.Where(i => i.Name == ClientSubFolder).First();

		//		SP.FileCreationInformation newFile = new SP.FileCreationInformation();
		//		newFile.Content = System.IO.File.ReadAllBytes(FileName);
		//		newFile.Url = list.RootFolder.ServerRelativeUrl + "/" + ClientSubFolder + "/" + FileName;
		//		newFile.Overwrite = true;

		//		SP.File uploadFile = folderToUpload.Files.Add(newFile);
		//		cnx.Load(uploadFile);
		//		cnx.ExecuteQuery();
		//		Console.WriteLine("done");
		//	};
		//}
		public void UploadDocumentContentStream(string siteUrl,string userName, string password)
		{
			try

			{

				OfficeDevPnP.Core.AuthenticationManager authMgr = new OfficeDevPnP.Core.AuthenticationManager();



				using (var ctx = authMgr.GetSharePointOnlineAuthenticatedContextTenant(siteUrl, userName, password))

				{

					Web web = ctx.Web;

					ctx.Load(web);

					ctx.Load(web.Lists);

					ctx.ExecuteQueryRetry();

					List list = web.Lists.GetByTitle("D1");

					ctx.Load(list);

					ctx.ExecuteQueryRetry();

					Folder folder = list.RootFolder.EnsureFolder("Folder1");

					ctx.Load(folder);

					ctx.ExecuteQueryRetry();



					Folder folderToUpload = web.GetFolderByServerRelativeUrl(folder.ServerRelativeUrl);

					folderToUpload.UploadFile("LargeFile.txt", @"D:\LargeFile.txt", true);

					folderToUpload.Update();

					ctx.Load(folder);

					ctx.ExecuteQueryRetry();

					folderToUpload.EnsureProperty(f => f.ServerRelativeUrl);

					var serverRelativeUrl = folderToUpload.ServerRelativeUrl.TrimEnd('/') + '/' + "LargeFile.txt";

				}

			}

			catch (Exception ex)
			{

				Console.WriteLine("Exception occurred : " + ex.Message);

				Console.ReadLine();

			}
		}
		#endregion Helper Functions

		//public ActionResult CancelRequest(string no, string url)
		//{
		//	try
		//	{
		//		dynamicsNAVSOAPServices.StaffAdvance.CancelApproval(no);
		//		return Redirect(url);
		//	}
		//	catch (Exception e)
		//	{
		//		return errorResponse.ApplicationExceptionError(e);
		//	}
		//}
    }
}