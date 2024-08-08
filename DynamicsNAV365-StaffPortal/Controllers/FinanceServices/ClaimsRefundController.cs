using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using DynamicsNAV365_StaffPortal.Models.Finance.ClaimsRefund;
using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
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
using DynamicsNAV365_StaffPortal.Models.Finance.ImprestSurrender;

namespace DynamicsNAV365_StaffPortal.Controllers.FinanceServices
{
    public class ClaimsRefundController : Controller
    {
		static string companyURL = "";

		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
		DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
		BCODATAServices _dcodataServices = new BCODATAServices(companyURL);

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
		List<UnsurrenderedImprestCode> unsurrenderedImprestCodes = null;
	//	List<FundsClaimTransactionCodes> claimTransactionCodes = null;

		AccountController accountController = new AccountController();
		string employeeNo = "";

		public ClaimsRefundController()
        {
			employeeNo = AccountController.GetEmployeeNo();
        }

		#region New Claims/Refund

		[Authorize]
		public ActionResult NewClaimsRefund()
		{
			string openClaimsRefundNo = "";
			string claimsRefundNo = "";

			try
			{
				ClaimsRefundHeaderModel claimsRefundObj = new ClaimsRefundHeaderModel();

				openClaimsRefundNo = dynamicsNAVSOAPServices.fundsManagementWS.CheckOpenClaimsRefundExists(employeeNo);

				//Check open claims/refund 
				if (!openClaimsRefundNo.Equals(""))
				{
					responseHeader = "Open Claims/Refund Request";
					responseMessage = "An open Claims/Refund No. " + openClaimsRefundNo + " exists under your employee no. " + employeeNo + ", finalize on this claims/refund before creating a new one.";
					detailedResponseMessage = "An open Claims/Refund No. " + openClaimsRefundNo + " exists under your employee no. " + employeeNo + ", finalize on this claims/refund before creating a new one.";

					button1ControllerName = "ClaimsRefund";
					button1ActionName = "ClaimsRefundHistory";
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
				//End check open claims/refund 

				//Create a new claims/refund 
				claimsRefundNo = dynamicsNAVSOAPServices.fundsManagementWS.CreateClaimsRefundHeader(employeeNo);
				//End create claims/refund

				claimsRefundObj.No = claimsRefundNo;
				claimsRefundObj.EmployeeNo = employeeNo;
				claimsRefundObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);
				claimsRefundObj.GlobalDimension1Code = "";
				claimsRefundObj.Status = "Open";

				//LoadCurrencies();
				//LoadImprestRequestDimensions(claimsRefundObj.GlobalDimension1Code);

				return View(claimsRefundObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}


		[Authorize]
		[HttpPost] 
		public async Task<ActionResult> NewClaimsRefund(ClaimsRefundHeaderModel claimsRefundObj, string Command)
		{
			bool claimsRefundModified = false;
			bool approvalWorkflowExist = false;
			try
			{
				if (Command.Equals("Submit For Approval"))
				{
					if (ModelState.IsValid)
					{
						if (dynamicsNAVSOAPServices.fundsManagementWS.CheckClaimsRefundExists(claimsRefundObj.No, AccountController.GetEmployeeNo()))
						{
							//Check claims/refund lines
							if (!dynamicsNAVSOAPServices.fundsManagementWS.CheckClaimsRefundLinesExist(claimsRefundObj.No))
							{
								claimsRefundObj.ErrorStatus = true;
								claimsRefundObj.ErrorMessage = "Claims/Refund lines missing, the claims/refund must contain a minimum of one line, add an claims/refund line to continue.";
								return View(claimsRefundObj);
							}

							claimsRefundModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyClaimsRefundHeader(claimsRefundObj.No, claimsRefundObj.EmployeeNo, claimsRefundObj.Description);

							//Send claims/refund for approval
							approvalWorkflowExist = dynamicsNAVSOAPServices.fundsManagementWS.CheckClaimsRefundApprovalWorkflowEnabled(claimsRefundObj.No);
							if (!approvalWorkflowExist)
							{
								claimsRefundObj.ErrorStatus = true;
								claimsRefundObj.ErrorMessage = "An error was experienced while trying to send an approval request for imprest/refund no." + claimsRefundObj.No + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
								return View(claimsRefundObj);
							}

							if (dynamicsNAVSOAPServices.fundsManagementWS.SendClaimsRefundApprovalRequest(claimsRefundObj.No))
							{
								responseHeader = "Success";
								responseMessage = "Claims/Refund no." + claimsRefundObj.No + " was successfully sent for approval.";
								detailedResponseMessage = "Claims/Refund no." + claimsRefundObj.No + " was successfully sent for approval.";

								button1ControllerName = "ClaimsRefund";
								button1ActionName = "ClaimsRefundHistory";
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
								claimsRefundObj.ErrorStatus = true;
								claimsRefundObj.ErrorMessage = "An error was experienced while trying to send an approval request for claims/refund no." + claimsRefundObj.No + ". " + ServiceConnection.contactICTDepartment + "";
								return View(claimsRefundObj);
							}
						}
						else
						{
							responseHeader = "Claims/Refund NotFound";
							responseMessage = "The Claims/Refund no." + claimsRefundObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
							detailedResponseMessage = "The Claims/Refund no." + claimsRefundObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

							button1ControllerName = "ClaimsRefund";
							button1ActionName = "ClaimsRefundHistory";
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
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(claimsRefundObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
     //               string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(claimsRefundObj.No);

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
					//	return View(claimsRefundObj);
					//}
				}
				else
				{
					return View(claimsRefundObj);
				}
			}
			catch (Exception ex)
			{
				claimsRefundObj.ErrorStatus = true;
				claimsRefundObj.ErrorMessage = ex.Message;
				return View(claimsRefundObj);
			}
		}

		#endregion New Claims/Refund

		#region Edit Claims/Refund

		[Authorize]
		public ActionResult OnBeforeEdit(string ClaimsRefundNo) 
		{
			try
			{
				if (ClaimsRefundNo.Equals(""))
				{
					return RedirectToAction("ClaimsRefundHistory", "ClaimsRefund"); 
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckClaimsRefundExists(ClaimsRefundNo, AccountController.GetEmployeeNo()))
				{
					string claimsRefundStatus = GetClaimsRefundStatus(ClaimsRefundNo);
				
					//if claims/refund is open
					if (claimsRefundStatus.Equals("Open"))
					{
						return RedirectToAction("EditClaimsRefund", "ClaimsRefund", new { ClaimsRefundNo = ClaimsRefundNo }); 
					}

					//if claims/refund is pending approval
					if (claimsRefundStatus.Equals("Pending Approval"))
					{
						responseHeader = "Claims/Refund Pending Approval";
						responseMessage = "The Claims/Refund no." + ClaimsRefundNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
						detailedResponseMessage = "The Claims/Refund no." + ClaimsRefundNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";

						button1ControllerName = "ClaimsRefund";
						button1ActionName = "EditClaimsRefund";
						button1HasParameters = true;
						button1Parameters = "?ClaimsRefundNo=" + ClaimsRefundNo;
						button1Name = "Yes";

						button2ControllerName = "ClaimsRefund";
						button2ActionName = "ClaimsRefundHistory";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}

					//if claims/refund is released
					if (claimsRefundStatus.Equals("Released"))
					{
						responseHeader = "Claims/Refund Approved";
						responseMessage = "The Claims/Refund no." + ClaimsRefundNo + " is already approved. Editing not allowed.";
						detailedResponseMessage = "The Claims/Refund no." + ClaimsRefundNo + " is already approved. Editing not allowed.";

						button1ControllerName = "ClaimsRefund";
						button1ActionName = "ClaimsRefundHistory";
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

					//if claim/refund is rejected
					if (claimsRefundStatus.Equals("Rejected"))
					{
						responseHeader = "Claims/Refund Rejected";
						responseMessage = "The Claims/Refund  no." + ClaimsRefundNo + " was rejected. Editing will reopen the document. Do you want to continue?";
						detailedResponseMessage = "The Claims/Refund  no." + ClaimsRefundNo + " was rejected. Editing will reopen the document. Do you want to continue?";

						button1ControllerName = "ClaimsRefund ";
						button1ActionName = "EditClaimsRefund";
						button1HasParameters = true;
						button1Parameters = "?ClaimsRefundNo=" + ClaimsRefundNo;
						button1Name = "Yes";

						button2ControllerName = "ClaimsRefund";
						button2ActionName = "ClaimsRefundHistory";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}

					//if claims/refund is posted/reversed
					if (claimsRefundStatus.Equals("Posted") || claimsRefundStatus.Equals("Reversed"))
					{
						responseHeader = "Claims/Refund Posted";
						responseMessage = "The claims/refund no." + ClaimsRefundNo + " is already posted. Editing not allowed.";
						detailedResponseMessage = "The claims/refund no." + ClaimsRefundNo + " is already posted. Editing not allowed.";

						button1ControllerName = "ClaimsRefund";
						button1ActionName = "ClaimsRefundHistory";
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
					responseHeader = "Claims/Refund NotFound";
					responseMessage = "The claims/refund no." + ClaimsRefundNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The claims/refund no." + ClaimsRefundNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "ClaimsRefund";
					button1ActionName = "ClaimsRefundHistory";
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
		public ActionResult EditClaimsRefund(string ClaimsRefundNo) 
		{
			try
			{
				if (ClaimsRefundNo.Equals(""))
				{
					return RedirectToAction("ClaimsRefundHistory", "ClaimsRefund");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckClaimsRefundExists(ClaimsRefundNo, AccountController.GetEmployeeNo()))
				{
					string imprestStatus = dynamicsNAVSOAPServices.fundsManagementWS.GetClaimsRefundStatus(ClaimsRefundNo);

					//if claims/refund is pending approval, cancel approval request
					if (imprestStatus.Equals("Pending Approval"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.CancelClaimsRefundApprovalRequest(ClaimsRefundNo);
					}
					//if claims/refund is released, reopen and uncommit from budget
					if (imprestStatus.Equals("Released"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.CancelImprestBudgetCommitment(ClaimsRefundNo);
					}
					//if claims/refund is rejected, reopen document
					if (imprestStatus.Equals("Rejected"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.CancelClaimsRefundApprovalRequest(ClaimsRefundNo);
					}
					 
					ClaimsRefundHeaderModel claimsRefundObj = new ClaimsRefundHeaderModel();
					LoadUnsurrenderedImprestCodes();
					dynamic claimsRefundValues = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetClaimsRefund(ClaimsRefundNo, ""));
					foreach (var claimsRefundValue in claimsRefundValues)
					{
						claimsRefundObj.No = claimsRefundValue.No;
						claimsRefundObj.DocumentDate = claimsRefundValue.DocumentDate;
						claimsRefundObj.PostingDate = claimsRefundValue.PostingDate;
						claimsRefundObj.BankAccountNo = claimsRefundValue.BankAccountNo;
						claimsRefundObj.BankAccountName = claimsRefundValue.BankAccountName;
						claimsRefundObj.ReferenceNo = claimsRefundValue.ReferenceNo;
						claimsRefundObj.EmployeeNo = claimsRefundValue.EmployeeNo;
						claimsRefundObj.EmployeeName = claimsRefundValue.EmployeeName;
						claimsRefundObj.Description = claimsRefundValue.Description;
						claimsRefundObj.GlobalDimension1Code = claimsRefundValue.GlobalDimension1Code;
						claimsRefundObj.GlobalDimension2Code = claimsRefundValue.GlobalDimension2Code;
						claimsRefundObj.ShortcutDimension3Code = claimsRefundValue.ShortcutDimension3Code;
						claimsRefundObj.ShortcutDimension4Code = claimsRefundValue.ShortcutDimension4Code;
						claimsRefundObj.ShortcutDimension5Code = claimsRefundValue.ShortcutDimension5Code;
						claimsRefundObj.ShortcutDimension6Code = claimsRefundValue.ShortcutDimension6Code;
						claimsRefundObj.ShortcutDimension7Code = claimsRefundValue.ShortcutDimension7Code;
						claimsRefundObj.ShortcutDimension8Code = claimsRefundValue.ShortcutDimension8Code;
						claimsRefundObj.Amount = claimsRefundValue.Amount;
						claimsRefundObj.Comments = dynamicsNAVSOAPServices.ApprovalsMgmt.RejectionComments(claimsRefundObj.No);
						claimsRefundObj.Status = claimsRefundValue.Status;
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
                    claimsRefundObj.GlobalDimension2Codes = new SelectList(DimensionValues, "Code", "Code");
					claimsRefundObj.UnsurrenderedImprests = new SelectList(unsurrenderedImprestCodes, "No", "Description");
						//LoadCurrencies();
                    //LoadImprestRequestDimensions(claimsRefundObj.GlobalDimension1Code);

                    //claimsRefundObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
                    //claimsRefundObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
                    //claimsRefundObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
                    //claimsRefundObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
                    //claimsRefundObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
                    //claimsRefundObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
                    //claimsRefundObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
                    //claimsRefundObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

                    return View(claimsRefundObj);
				}
				else
				{
					responseHeader = "Claims/Refund NotFound";
					responseMessage = "The Claims/Refund no." + ClaimsRefundNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The Claims/Refund no." + ClaimsRefundNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "ClaimsRefund";
					button1ActionName = "ClaimsRefundHistory";
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
		private void LoadUnsurrenderedImprestCodes() 
		{
			unsurrenderedImprestCodes = new List<UnsurrenderedImprestCode>();

			dynamic unsurrenderImprestList = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetUnsurrenderedImprests(employeeNo)); 
			foreach (var unsurrenderImprest in unsurrenderImprestList)
			{
				UnsurrenderedImprestCode unsurrenderedImprestObj = new UnsurrenderedImprestCode();
				unsurrenderedImprestObj.No = unsurrenderImprest.No;
				unsurrenderedImprestObj.Description = unsurrenderImprest.Description;

				unsurrenderedImprestCodes.Add(unsurrenderedImprestObj);
			}
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult> EditClaimsRefund(ClaimsRefundHeaderModel ClaimsRefundObj, string Command)
		{
			bool claimsRefundModified = false;
			bool approvalWorkflowExist = false;
			try
			{
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
				ClaimsRefundObj.GlobalDimension2Codes = new SelectList(DimensionValues, "Code", "Code");;
				if (Command.Equals("Submit For Approval"))
				{
					if (ModelState.IsValid)
					{
						if (dynamicsNAVSOAPServices.fundsManagementWS.CheckClaimsRefundExists(ClaimsRefundObj.No, AccountController.GetEmployeeNo()))
						{
							//Check claims/refund lines
							if (!dynamicsNAVSOAPServices.fundsManagementWS.CheckClaimsRefundLinesExist(ClaimsRefundObj.No))
							{
								ClaimsRefundObj.ErrorStatus = true;
								ClaimsRefundObj.ErrorMessage = "Claims/Refund lines missing, the claims/refund must contain a minimum of one line, add an claims/refund line to continue.";
								return View(ClaimsRefundObj);
							}

							claimsRefundModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyClaimsRefundHeader(ClaimsRefundObj.No, ClaimsRefundObj.EmployeeNo, ClaimsRefundObj.Description);

							//Send claims/refund for approval
							approvalWorkflowExist = dynamicsNAVSOAPServices.fundsManagementWS.CheckClaimsRefundApprovalWorkflowEnabled(ClaimsRefundObj.No);
							if (!approvalWorkflowExist)
							{
								ClaimsRefundObj.ErrorStatus = true;
								ClaimsRefundObj.ErrorMessage = "An error was experienced while trying to send an approval request for imprest/refund no." + ClaimsRefundObj.No + ", the approval workflow was not found. " + ServiceConnection.contactICTDepartment + "";
								return View(ClaimsRefundObj);
							}

							if (dynamicsNAVSOAPServices.fundsManagementWS.SendClaimsRefundApprovalRequest(ClaimsRefundObj.No))
							{
								responseHeader = "Success";
								responseMessage = "Claims/Refund no." + ClaimsRefundObj.No + " was successfully sent for approval.";
								detailedResponseMessage = "Claims/Refund no." + ClaimsRefundObj.No + " was successfully sent for approval.";

								button1ControllerName = "ClaimsRefund";
								button1ActionName = "ClaimsRefundHistory"; 
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
								ClaimsRefundObj.ErrorStatus = true;
								ClaimsRefundObj.ErrorMessage = "An error was experienced while trying to send an approval request for claims/refund no." + ClaimsRefundObj.No + ". " + ServiceConnection.contactICTDepartment + "";
								return View(ClaimsRefundObj);
							}
						}
						else
						{
							responseHeader = "Claims/Refund NotFound";
							responseMessage = "The Claims/Refund no." + ClaimsRefundObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
							detailedResponseMessage = "The Claims/Refund no." + ClaimsRefundObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

							button1ControllerName = "ClaimsRefund";
							button1ActionName = "ClaimsRefundHistory";
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
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(ClaimsRefundObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                    //string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(ClaimsRefundObj.No);

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
                    //	return View(ClaimsRefundObj);
                    //}
                }
				else
				{
					return View(ClaimsRefundObj);
				}
			}
			catch (Exception ex)
			{
				ClaimsRefundObj.ErrorStatus = true;
				ClaimsRefundObj.ErrorMessage = ex.Message;
				return View(ClaimsRefundObj);
			}
		}

		#endregion Edit Claims/Refund

		#region View Claims/Refund

		[Authorize]
		public ActionResult ViewClaimsRefund(string ClaimsRefundNo) 
		{
			try
			{
				if (ClaimsRefundNo.Equals(""))
				{
					return RedirectToAction("ClaimsRefundHistory", "ClaimsRefund");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckClaimsRefundExists(ClaimsRefundNo, AccountController.GetEmployeeNo()))
				{
					ClaimsRefundHeaderModel claimsRefundObj = new ClaimsRefundHeaderModel();
					dynamic claimsRefundValues = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetClaimsRefund(ClaimsRefundNo, ""));
					foreach (var claimsRefundValue in claimsRefundValues) 
					{
						claimsRefundObj.No = claimsRefundValue.No;
						claimsRefundObj.DocumentDate = claimsRefundValue.DocumentDate;
						claimsRefundObj.PostingDate = claimsRefundValue.PostingDate;
						claimsRefundObj.BankAccountNo = claimsRefundValue.BankAccountNo;
						claimsRefundObj.BankAccountName = claimsRefundValue.BankAccountName;
						claimsRefundObj.ReferenceNo = claimsRefundValue.ReferenceNo;
						claimsRefundObj.EmployeeNo = claimsRefundValue.EmployeeNo;
						claimsRefundObj.EmployeeName = claimsRefundValue.EmployeeName;
						claimsRefundObj.Description = claimsRefundValue.Description;
						claimsRefundObj.GlobalDimension1Code = claimsRefundValue.GlobalDimension1Code;
						claimsRefundObj.GlobalDimension2Code = claimsRefundValue.GlobalDimension2Code;
						claimsRefundObj.ShortcutDimension3Code = claimsRefundValue.ShortcutDimension3Code;
						claimsRefundObj.ShortcutDimension4Code = claimsRefundValue.ShortcutDimension4Code;
						claimsRefundObj.ShortcutDimension5Code = claimsRefundValue.ShortcutDimension5Code;
						claimsRefundObj.ShortcutDimension6Code = claimsRefundValue.ShortcutDimension6Code;
						claimsRefundObj.ShortcutDimension7Code = claimsRefundValue.ShortcutDimension7Code;
						claimsRefundObj.ShortcutDimension8Code = claimsRefundValue.ShortcutDimension8Code;
						claimsRefundObj.Amount = claimsRefundValue.Amount;
						claimsRefundObj.Comments = dynamicsNAVSOAPServices.ApprovalsMgmt.RejectionComments(claimsRefundObj.No);
						claimsRefundObj.Status = claimsRefundValue.Status;
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
                    claimsRefundObj.GlobalDimension2Codes = new SelectList(DimensionValues, "Code", "Code");
                    //LoadCurrencies();
                    //LoadCurrencies();
                    //LoadImprestRequestDimensions(claimsRefundObj.GlobalDimension1Code);

                    //claimsRefundObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
                    //claimsRefundObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
                    //claimsRefundObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
                    //claimsRefundObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
                    //claimsRefundObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
                    //claimsRefundObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
                    //claimsRefundObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
                    //claimsRefundObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

                    return View(claimsRefundObj);
				}
				else
				{
					responseHeader = "Claims/Refund Not Found";
					responseMessage = "The Claims/Refund No." + ClaimsRefundNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The Claims/Refund No." + ClaimsRefundNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "ClaimsRefund";
					button1ActionName = "ClaimsRefundHistory"; 
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
		public async Task<ActionResult> ViewClaimsRefund(ClaimsRefundHeaderModel claimsRefundObj, string Command) 
		{
			try
			{
				if (claimsRefundObj.No.Equals(""))
				{
					return RedirectToAction("ClaimsRefundHistory", "ClaimsRefund");
				}
				if (Command.Equals("View Attachment"))
				{
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(claimsRefundObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                    //string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(claimsRefundObj.No);

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
                    //	return View(claimsRefundObj);
                    //}
                }
				else
				{
					claimsRefundObj.ErrorStatus = true;
					//leaveApplicationObj2.ErrorMessage = "Unable to process the approve/reject action. Contact the " + companyName + " for assistance.";
					return View(claimsRefundObj);
				}
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		#endregion View Claims/Refund

		#region Claims/Refund history

		[Authorize]
		public ActionResult ClaimsRefundHistory() 
		{
			try
			{
				List<ClaimsRefundHeaderModel> claimsRefundList = new List<ClaimsRefundHeaderModel>();

				dynamic claimsRefundLists = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetClaimsRefund("", employeeNo));

				foreach (var claimsRefundValue in claimsRefundLists)
				{
					ClaimsRefundHeaderModel claimsRefundObj = new ClaimsRefundHeaderModel();
					claimsRefundObj.No = claimsRefundValue.No;
					claimsRefundObj.DocumentDate = claimsRefundValue.DocumentDate;
					claimsRefundObj.PostingDate = claimsRefundValue.PostingDate;
					claimsRefundObj.BankAccountNo = claimsRefundValue.BankAccountNo;
					claimsRefundObj.BankAccountName = claimsRefundValue.BankAccountName;
					claimsRefundObj.ReferenceNo = claimsRefundValue.ReferenceNo;
					claimsRefundObj.EmployeeNo = claimsRefundValue.EmployeeNo;
					claimsRefundObj.EmployeeName = claimsRefundValue.EmployeeName;
					claimsRefundObj.Description = claimsRefundValue.Description;
					claimsRefundObj.GlobalDimension1Code = claimsRefundValue.GlobalDimension1Code;
					claimsRefundObj.GlobalDimension2Code = claimsRefundValue.GlobalDimension2Code;
					claimsRefundObj.ShortcutDimension3Code = claimsRefundValue.ShortcutDimension3Code;
					claimsRefundObj.ShortcutDimension4Code = claimsRefundValue.ShortcutDimension4Code;
					claimsRefundObj.ShortcutDimension5Code = claimsRefundValue.ShortcutDimension5Code;
					claimsRefundObj.ShortcutDimension6Code = claimsRefundValue.ShortcutDimension6Code;
					claimsRefundObj.ShortcutDimension7Code = claimsRefundValue.ShortcutDimension7Code;
					claimsRefundObj.ShortcutDimension8Code = claimsRefundValue.ShortcutDimension8Code;
					claimsRefundObj.Amount = claimsRefundValue.Amount;
					claimsRefundObj.Comments = dynamicsNAVSOAPServices.ApprovalsMgmt.RejectionComments(claimsRefundObj.No);
					claimsRefundObj.Status = claimsRefundValue.Status;

					claimsRefundList.Add(claimsRefundObj);
				}
				return View(claimsRefundList);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		#endregion Claims/Refund history

		#region Claims/Refund Line

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ClaimsRefundLine(string DocumentNo)
		{
			ClaimsRefundLineModel ClaimsRefundLineObj = new ClaimsRefundLineModel();

			List<FundsTransactionModel> claimTransactionCodes = new List<FundsTransactionModel>();

			dynamic fundsTransactionCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestTransactionCodes());
			foreach (var fundsTransactionCode in fundsTransactionCodes)
			{
				FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
				FundsTransactionObj.TransactionCode = fundsTransactionCode.TransactionCode;
				FundsTransactionObj.TransactionDescription = fundsTransactionCode.TransactionDescription;

				claimTransactionCodes.Add(FundsTransactionObj);
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

            ClaimsRefundLineObj.TransactionTypes = new SelectList(claimTransactionCodes, "TransactionCode", "TransactionDescription");
            ClaimsRefundLineObj.Dimension1s = new SelectList(DimensionValues, "Code", "Name");
            ClaimsRefundLineObj.Dimension2s = new SelectList(Enumerable.Empty<SelectListItem>());
            ClaimsRefundLineObj.Dimension3s = new SelectList(Enumerable.Empty<SelectListItem>());
            ClaimsRefundLineObj.Dimension4s = new SelectList(Enumerable.Empty<SelectListItem>());
            ClaimsRefundLineObj.Dimension5s = new SelectList(Enumerable.Empty<SelectListItem>());
            ClaimsRefundLineObj.Dimension6s = new SelectList(Enumerable.Empty<SelectListItem>());
            ClaimsRefundLineObj.Dimension7s = new SelectList(Enumerable.Empty<SelectListItem>());

            return PartialView(ClaimsRefundLineObj);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewClaimsRefundLine(string DocumentNo) 
		{
			ClaimsRefundLineModel ClaimsRefundLineObj = new ClaimsRefundLineModel();

			List<FundsTransactionModel> claimTransactionCodes = new List<FundsTransactionModel>();

			dynamic fundsTransactionCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestTransactionCodes());
			foreach (var fundsTransactionCode in fundsTransactionCodes)
			{
				FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
				FundsTransactionObj.TransactionCode = fundsTransactionCode.TransactionCode;
				FundsTransactionObj.TransactionDescription = fundsTransactionCode.TransactionDescription;

				claimTransactionCodes.Add(FundsTransactionObj);
			}
			ClaimsRefundLineObj.TransactionTypes = new SelectList(claimTransactionCodes, "TransactionCode", "TransactionDescription");

			return PartialView(ClaimsRefundLineObj);
		}

		[Authorize]
        public JsonResult GetClaimsRefundLines(string DocumentNo)
        {
            List<ClaimsRefundLineModel> imprestLinesList = new List<ClaimsRefundLineModel>();
            var linesList = _dcodataServices.BCOData.ImprestLines.Where(c => c.Document_No == DocumentNo).ToList();
            /*string imprestlines = "ImprestLines?$filter=Document_No eq '" + DocumentNo + "' &$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(imprestlines);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    ClaimsRefundLineModel imprestLineObj = new ClaimsRefundLineModel();
                    imprestLineObj.LineNo = (string)config1["Line_No"];
                    imprestLineObj.DocumentNo = (string)config1["Document_No"];
                    imprestLineObj.TransactionType = (string)config1["Transaction_Type"];                   
                    imprestLineObj.LineDescription = (string)config1["Description"];
                    imprestLineObj.LineAmount = (string)config1["Amount"];                  
                    imprestLineObj.Dimension1 = (string)config1["Shortcut_Dimension_1_Code"];
                    imprestLineObj.Dimension2 = (string)config1["Shortcut_Dimension_2_Code"];
                    imprestLineObj.Dimension3 = (string)config1["ShortcutDimCode3"];
                    imprestLineObj.Dimension4 = (string)config1["ShortcutDimCode4"];
                    imprestLineObj.Dimension5 = (string)config1["ShortcutDimCode5"];
                    imprestLineObj.Dimension6 = (string)config1["ShortcutDimCode6"];
                    imprestLineObj.Dimension7 = (string)config1["ShortcutDimCode7"];
                    //imprestLineObj. = (string)config1["Status"];
                    imprestLinesList.Add(imprestLineObj);
                }
            }*/
            foreach (var config1 in linesList)
            {
	            ClaimsRefundLineModel imprestLineObj = new ClaimsRefundLineModel();
	            imprestLineObj.LineNo = config1.Line_No.ToString();
	            imprestLineObj.DocumentNo = config1.Document_No.ToString();
	            imprestLineObj.TransactionType = config1.Transaction_Type.ToString();                   
	            imprestLineObj.LineDescription = config1.Description.ToString();
	            imprestLineObj.LineAmount = config1.Amount.ToString();
	            imprestLineObj.ActualSpent = config1.Actual_Spent;
	            imprestLineObj.Dimension1 = config1.Shortcut_Dimension_1_Code;
	            imprestLineObj.Dimension2 = config1.Shortcut_Dimension_2_Code;
	            imprestLineObj.Dimension3 = config1.ShortcutDimCode3;
	            imprestLineObj.Dimension4 = config1.ShortcutDimCode4;
	            imprestLineObj.Dimension5 = config1.ShortcutDimCode5;
	            imprestLineObj.Dimension6 = config1.ShortcutDimCode6;
	            imprestLineObj.Dimension7 = config1.ShortcutDimCode7;
	            //imprestLineObj. = (string)config1["Status"];
	            imprestLinesList.Add(imprestLineObj);
            }
            return Json(imprestLinesList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetClaimsRefundLinesp(string DocumentNo)
		{
			List<ClaimsRefundLineModel> claimsRefundLinesList = new List<ClaimsRefundLineModel>();

			dynamic claimsRefundLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetClaimsRefundLines(DocumentNo));

			foreach (var claimsRefundLine in claimsRefundLines)
			{
				ClaimsRefundLineModel ClaimsRefundLineObj = new ClaimsRefundLineModel();
				ClaimsRefundLineObj.LineNo = claimsRefundLine.LineNo;
				ClaimsRefundLineObj.DocumentNo = claimsRefundLine.DocumentNo;
				ClaimsRefundLineObj.TransactionType = claimsRefundLine.ImprestCode;
				ClaimsRefundLineObj.LineDescription = claimsRefundLine.LineDescription;
				ClaimsRefundLineObj.LineAmount = claimsRefundLine.LineAmount;
				claimsRefundLinesList.Add(ClaimsRefundLineObj);
			}
			return Json(claimsRefundLinesList, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult GetClaimsRefundLine(string LineNo, string DocumentNo)
		{
			ClaimsRefundLineModel ClaimsRefundLineObj = new ClaimsRefundLineModel();
			var firstOrDefault = _dcodataServices.BCOData.ImprestLines.Where(c=>c.Document_No == DocumentNo).ToList().FirstOrDefault(c => c.Line_No.ToString() == LineNo);
			/*dynamic claimsRefundLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetClaimsRefundByLine(Convert.ToInt32(LineNo), DocumentNo));
			foreach (var claimsRefundLine in claimsRefundLines) 
			{
				ClaimsRefundLineObj.LineNo = claimsRefundLine.LineNo;
				ClaimsRefundLineObj.DocumentNo = claimsRefundLine.DocumentNo;
				ClaimsRefundLineObj.TransactionType = claimsRefundLine.ImprestCode;
				ClaimsRefundLineObj.LineDescription = claimsRefundLine.LineDescription;
				ClaimsRefundLineObj.LineAmount = claimsRefundLine.LineAmount;
				ClaimsRefundLineObj.ActualSpent = claimsRefundLine.ActualSpent;
			}*/
			ClaimsRefundLineObj.LineNo = firstOrDefault?.Line_No.ToString();
			ClaimsRefundLineObj.DocumentNo = firstOrDefault?.Document_No.ToString();
			ClaimsRefundLineObj.TransactionType = firstOrDefault?.Transaction_Type.ToString();                   
			ClaimsRefundLineObj.LineDescription = firstOrDefault?.Description.ToString();
			ClaimsRefundLineObj.LineAmount = firstOrDefault?.Amount.ToString();
			ClaimsRefundLineObj.ActualSpent = firstOrDefault?.Actual_Spent;
			ClaimsRefundLineObj.Dimension1 = firstOrDefault?.Shortcut_Dimension_1_Code;
			ClaimsRefundLineObj.Dimension2 = firstOrDefault?.Shortcut_Dimension_2_Code;
			ClaimsRefundLineObj.Dimension3 = firstOrDefault?.ShortcutDimCode3;
			ClaimsRefundLineObj.Dimension4 = firstOrDefault?.ShortcutDimCode4;
			ClaimsRefundLineObj.Dimension5 = firstOrDefault?.ShortcutDimCode5;
			ClaimsRefundLineObj.Dimension6 = firstOrDefault?.ShortcutDimCode6;
			ClaimsRefundLineObj.Dimension7 = firstOrDefault?.ShortcutDimCode7;

			return Json(ClaimsRefundLineObj, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult CreateClaimsRefundLine(ClaimsRefundLineModel ClaimsRefundLineObj) 
		{
			try
			{
				var claimsRefundLineCreated = false;

				if (Convert.ToDecimal(ClaimsRefundLineObj.LineAmount) < 0)
				{
					return Json(new { success = false, message = "The system cannot allow you to add/save zero " + (ClaimsRefundLineObj.LineAmount) + " amount. Please type amount or liaise with finance for assistance." }, JsonRequestBehavior.AllowGet);
				}
            
				ClaimsRefundLineObj.LineDescription = ClaimsRefundLineObj.LineDescription ?? "";          
				ClaimsRefundLineObj.Dimension1 = ClaimsRefundLineObj.Dimension1 ?? "";
				ClaimsRefundLineObj.Dimension2 = ClaimsRefundLineObj.Dimension2 ?? "";
				ClaimsRefundLineObj.Dimension3 = ClaimsRefundLineObj.Dimension3 ?? "";
				ClaimsRefundLineObj.Dimension4 = ClaimsRefundLineObj.Dimension4 ?? "";
				ClaimsRefundLineObj.Dimension5 = ClaimsRefundLineObj.Dimension5 ?? "";
				ClaimsRefundLineObj.Dimension6 = ClaimsRefundLineObj.Dimension6 ?? "";
				ClaimsRefundLineObj.Dimension7 = ClaimsRefundLineObj.Dimension7 ?? "";
       

				/*claimsRefundLineCreated = dynamicsNAVSOAPServices.fundsManagementWS.CreateClaimsRefundLine(ClaimsRefundLineObj.DocumentNo, ClaimsRefundLineObj.TransactionType, Convert.ToDecimal(ClaimsRefundLineObj.LineAmount),
					ClaimsRefundLineObj.LineDescription, ClaimsRefundLineObj.Dimension1, ClaimsRefundLineObj.Dimension2, ClaimsRefundLineObj.Dimension3, ClaimsRefundLineObj.Dimension4,
					ClaimsRefundLineObj.Dimension5, ClaimsRefundLineObj.Dimension6, ClaimsRefundLineObj.Dimension7, ClaimsRefundLineObj.ActualSpent??Decimal.Zero);*/
				return Json(claimsRefundLineCreated ? new { success = true, message = "claims/refund line added successfully." } : new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		[Authorize]
		public JsonResult ModifyClaimsRefundLine(ClaimsRefundLineModel ClaimsRefundLineObj)
		{
			try
			{
				bool claimsRefundLineModified = false;
				ClaimsRefundLineObj.LineDescription = ClaimsRefundLineObj.LineDescription ?? "";
				ClaimsRefundLineObj.Dimension1 = ClaimsRefundLineObj.Dimension1 ?? "";
				ClaimsRefundLineObj.Dimension2 = ClaimsRefundLineObj.Dimension2 ?? "";
				ClaimsRefundLineObj.Dimension3 = ClaimsRefundLineObj.Dimension3 ?? "";
				ClaimsRefundLineObj.Dimension4 = ClaimsRefundLineObj.Dimension4 ?? "";
				ClaimsRefundLineObj.Dimension5 = ClaimsRefundLineObj.Dimension5 ?? "";
				ClaimsRefundLineObj.Dimension6 = ClaimsRefundLineObj.Dimension6 ?? "";
				ClaimsRefundLineObj.Dimension7 = ClaimsRefundLineObj.Dimension7 ?? "";

				/*claimsRefundLineModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyClaimsRefundLine(Convert.ToInt32(ClaimsRefundLineObj.LineNo), ClaimsRefundLineObj.DocumentNo, ClaimsRefundLineObj.TransactionType,
					Convert.ToDecimal(ClaimsRefundLineObj.LineAmount), ClaimsRefundLineObj.LineDescription, ClaimsRefundLineObj.Dimension1, ClaimsRefundLineObj.Dimension2, ClaimsRefundLineObj.Dimension3, ClaimsRefundLineObj.Dimension4,
					ClaimsRefundLineObj.Dimension5, ClaimsRefundLineObj.Dimension6, ClaimsRefundLineObj.Dimension7, ClaimsRefundLineObj.ActualSpent??Decimal.One);*/

				return Json(new { success = true, ClaimsRefundLineModified = claimsRefundLineModified }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				return Json(new {success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		[Authorize]
		public JsonResult DeleteClaimsRefundLine(int LineNo, string DocumentNo) 
		{
			bool claimsRefundLineDeleted = false;

			claimsRefundLineDeleted = dynamicsNAVSOAPServices.fundsManagementWS.DeleteClaimsRefundLine(LineNo, DocumentNo);  

			return Json(new { ClaimsRefundLineDeleted = claimsRefundLineDeleted }, JsonRequestBehavior.AllowGet);
		}
	
		#endregion Claims/Refund Line

		#region Claims/Refund Approval

		[Authorize]
		public ActionResult ClaimsRefundApproval(string ClaimsRefundNo) 
		{
			try
			{
				if (ClaimsRefundNo.Equals(""))  
				{
					return RedirectToAction("OpenEntries", "Approval");
				}


				ClaimsRefundHeaderModel ClaimsHeaderObj = new ClaimsRefundHeaderModel();

				dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetClaimsRefund(ClaimsRefundNo, ""));

				foreach (var imprestRequest in imprestRequests)
				{
					ClaimsHeaderObj.No = imprestRequest.No;
					ClaimsHeaderObj.DocumentDate = imprestRequest.DocumentDate;
					ClaimsHeaderObj.PostingDate = imprestRequest.PostingDate;
					ClaimsHeaderObj.BankAccountNo = imprestRequest.BankAccountNo;
					ClaimsHeaderObj.BankAccountName = imprestRequest.BankAccountName;
					ClaimsHeaderObj.ReferenceNo = imprestRequest.ReferenceNo;
					ClaimsHeaderObj.EmployeeNo = imprestRequest.EmployeeNo;
					ClaimsHeaderObj.EmployeeName = imprestRequest.EmployeeName;
					ClaimsHeaderObj.Description = imprestRequest.Description;
					ClaimsHeaderObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
					ClaimsHeaderObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
					ClaimsHeaderObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
					ClaimsHeaderObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
					ClaimsHeaderObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
					ClaimsHeaderObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
					ClaimsHeaderObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
					ClaimsHeaderObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
					ClaimsHeaderObj.Amount = imprestRequest.Amount;
					ClaimsHeaderObj.Status = imprestRequest.Status;
				}

				//LoadCurrencies();
				//LoadImprestRequestDimensions(ClaimsHeaderObj.GlobalDimension1Code);

				//ClaimsHeaderObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
				//ClaimsHeaderObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
				//ClaimsHeaderObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
				//ClaimsHeaderObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
				//ClaimsHeaderObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
				//ClaimsHeaderObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
				//ClaimsHeaderObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
				//ClaimsHeaderObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

				return View(ClaimsHeaderObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ClaimsRefundApproval(ClaimsRefundHeaderModel ClaimsHeaderObj, string Command) 
		{
			try
			{
				if (ClaimsHeaderObj.No.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}
				if (Command == "Approve")
				{
					ClaimsHeaderObj.Comments = ClaimsHeaderObj.Comments ?? "";
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveImprestRequest(employeeNo, ClaimsHeaderObj.No, AccountController.GetEmployeeNo()))
					{
                        TempData["Success"] = "Claims/Refund no." + ClaimsHeaderObj.No + " was successfully approved.";
                        return RedirectToAction("OpenEntries", "Approval");
                        //responseHeader = "Success";
                        //responseMessage = "Claims/Refund no." + ClaimsHeaderObj.No + " was successfully approved.";
                        //detailedResponseMessage = "Claims/Refund no." + ClaimsHeaderObj.No + " was successfully approved.";

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
                        TempData["Error"] = "Unable to process the claims/refund approve action. " + ServiceConnection.contactICTDepartment + "";
                        return View(ClaimsHeaderObj);
                        //ClaimsHeaderObj.ErrorStatus = true;
                        //ClaimsHeaderObj.ErrorMessage = "Unable to process the claims/refund approve action. " + ServiceConnection.contactICTDepartment + "";
                        //return View(ClaimsHeaderObj);
                    }
				}

				if (Command == "Reject")
				{
					ClaimsHeaderObj.Comments = ClaimsHeaderObj.Comments ?? "";
					if (ClaimsHeaderObj.Comments.Equals(""))
					{
						dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetClaimsRefund(ClaimsHeaderObj.No, ""));

						foreach (var imprestRequest in imprestRequests)
						{
							ClaimsHeaderObj.No = imprestRequest.No;
							ClaimsHeaderObj.DocumentDate = imprestRequest.DocumentDate;
							ClaimsHeaderObj.PostingDate = imprestRequest.PostingDate;
							ClaimsHeaderObj.BankAccountNo = imprestRequest.BankAccountNo;
							ClaimsHeaderObj.BankAccountName = imprestRequest.BankAccountName;
							ClaimsHeaderObj.ReferenceNo = imprestRequest.ReferenceNo;
							ClaimsHeaderObj.EmployeeNo = imprestRequest.EmployeeNo;
							ClaimsHeaderObj.EmployeeName = imprestRequest.EmployeeName;
							ClaimsHeaderObj.Description = imprestRequest.Description;
							ClaimsHeaderObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
							ClaimsHeaderObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
							ClaimsHeaderObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
							ClaimsHeaderObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
							ClaimsHeaderObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
							ClaimsHeaderObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
							ClaimsHeaderObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
							ClaimsHeaderObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
							ClaimsHeaderObj.Amount = imprestRequest.Amount;

						}

                        TempData["Error"] = "Kindly provide reason (s) for declining/rejecting this document.";
                        return View(ClaimsHeaderObj);
                        //ClaimsHeaderObj.ErrorStatus = true;
                        //ClaimsHeaderObj.ErrorMessage = "Kindly provide reason (s) for declining/rejecting this document.";
                        //return View(ClaimsHeaderObj);
                    }

					if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectImprestRequest(employeeNo, ClaimsHeaderObj.No, ClaimsHeaderObj.Comments, AccountController.GetEmployeeNo()))
					{
                        TempData["Success"] = "Claims/Refund no." + ClaimsHeaderObj.No + " was successfully rejected.";
                        return RedirectToAction("OpenEntries", "Approval");
                        //responseHeader = "Success";
                        //responseMessage = "Claims/Refund no." + ClaimsHeaderObj.No + " was successfully rejected.";
                        //detailedResponseMessage = "Claims/Refund no." + ClaimsHeaderObj.No + " was successfully rejected.";

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
                        TempData["Error"] = "Unable to process the claims/refund reject action. " + ServiceConnection.contactICTDepartment + "";
                        return View(ClaimsHeaderObj);
                        //ClaimsHeaderObj.ErrorStatus = true;
                        //ClaimsHeaderObj.ErrorMessage = "Unable to process the claims/refund reject action. " + ServiceConnection.contactICTDepartment + "";
                        //return View(ClaimsHeaderObj);
                    }
				}
				else if (Command == "View Attachment")
				{
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(ClaimsHeaderObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                }
				else
				{
                    TempData["Error"] = "Unable to process the approve/refund reject action. " + ServiceConnection.contactICTDepartment + "";
                    return View(ClaimsHeaderObj);
                    //ClaimsHeaderObj.ErrorStatus = true;
                    //ClaimsHeaderObj.ErrorMessage = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                    //return View(ClaimsHeaderObj);
                }
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		#endregion Claims/Refund Approval

		#region Document Management

		[Authorize]
		public JsonResult GetClaimsRefundPortalDocuments(string DocumentNo) 
		{
			List<DocumentMgmtModel> documentsList = new List<DocumentMgmtModel>();

			dynamic claimsRefundDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(DocumentNo));

			foreach (var claimsRefundDocument in claimsRefundDocuments)
			{
				DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();
				documentManagementoBJ.LineNo = claimsRefundDocument.LineNo;
				documentManagementoBJ.DocumentNo = claimsRefundDocument.DocumentNo;
				documentManagementoBJ.DocumentCode = claimsRefundDocument.DocumentCode;
				documentManagementoBJ.DocumentDescription = claimsRefundDocument.DocumentDescription;
				documentManagementoBJ.DocumentAttached = claimsRefundDocument.DocumentAttached;
				documentManagementoBJ.FileName = claimsRefundDocument.FileName;

				documentsList.Add(documentManagementoBJ);
			}
			return Json(documentsList.ToList(), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		[HttpPost]
		public JsonResult UploadClaimsRefundDocumentLink(string DocumentNo, string DocumentCode, string DocumentDescription) 
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

					if (!fileExt.Equals(".pdf"))
					{
						return Json(new { success = false, message = "Only pdf formats are allowed to be uploaded. Please convert your " + fileExt + " file to pdf for uploading." }, JsonRequestBehavior.AllowGet);
					}

					file.SaveAs(path);
                    if (System.IO.File.Exists(path))
                    {
                        bool ret = false;
                        ret = dynamicsNAVSOAPServices.documentMgmt.InsertImprestAttachment(DocumentNo, path, 51525003, "Travel Claims");
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
				return Json(new { success = false, message = "Please select a pdf file!" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		[Authorize]
		public JsonResult DeleteClaimsRefundDocuments(string DocumentNo) 
		{
			bool imprestDocumentsDeleted = false;
			return Json(new { ImprestDocumentsDeleted = imprestDocumentsDeleted }, JsonRequestBehavior.AllowGet);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ClaimsRefundDocumentLine(string DocumentNo) 
		{
			DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

			return PartialView(documentManagementoBJ);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewClaimsRefundDocumentLine(string DocumentNo) 
		{
			DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();
			return PartialView(documentManagementoBJ);
		}

		[Authorize]
		public ActionResult GetClaimsRefundDocumentLink(string LineNo, string DocumentNo, string DocumentCode) 
		{
			try
			{
				DocumentMgmtModel documentManagementObj = new DocumentMgmtModel();

				dynamic claimsRefundDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocumentByDocCode(Convert.ToInt32(LineNo), DocumentNo, DocumentCode));

				foreach (var claimsRefundDocument in claimsRefundDocuments)
				{
					documentManagementObj.LineNo = claimsRefundDocument.LineNo;
					documentManagementObj.DocumentNo = claimsRefundDocument.DocumentNo;
					documentManagementObj.DocumentCode = claimsRefundDocument.DocumentCode;
					documentManagementObj.DocumentDescription = claimsRefundDocument.DocumentDescription;
					documentManagementObj.DocumentAttached = claimsRefundDocument.DocumentAttached;
					documentManagementObj.FileName = claimsRefundDocument.FileName;
				}

				return Json(documentManagementObj, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		#endregion Document Management

		#region Helper Functions
          [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateHeader(string Description,  string GlobalDimension2Code,  string DocNo)
        {
            try
            {
                bool ret;
                bool successVal = false;
                string msg = "";

                ret = Credentials.ObjNav.UpdateClaimsRequestHeader(DocNo, Description, GlobalDimension2Code);
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
		public string GetClaimsRefundStatus(string DocumentNo) 
		{
			return dynamicsNAVSOAPServices.fundsManagementWS.GetClaimsRefundStatus(DocumentNo); 
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
		private void LoadClaimsRefundCodes()
		{

			List<FundsTransactionModel> claimTransactionCodes = new List<FundsTransactionModel>();

			dynamic fundsTransactionCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestTransactionCodes());
			foreach (var fundsTransactionCode in fundsTransactionCodes)
			{
				FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
				FundsTransactionObj.TransactionCode = fundsTransactionCode.TransactionCode;
				FundsTransactionObj.TransactionDescription = fundsTransactionCode.TransactionDescription;

				claimTransactionCodes.Add(FundsTransactionObj);
			}
		}
		public JsonResult GetClaimsRefundAmount(string DocumentNo)
		{
			decimal claimsRefundAmount = 0;
			claimsRefundAmount = dynamicsNAVSOAPServices.fundsManagementWS.GetClaimsRefundAmount(DocumentNo);
			return Json(new { Amount = claimsRefundAmount }, JsonRequestBehavior.AllowGet);
		}
		#endregion Helper Functions
	}
}