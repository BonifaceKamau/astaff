using DynamicsNAV365_StaffPortal.Controllers.FinanceServices;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using DynamicsNAV365_StaffPortal.Models.Procurement.PurchaseRequistion;
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
using System.Web;
using System.Web.Mvc;
using static DynamicsNAV365_StaffPortal.Models.Procurement.PurchaseRequistion.PurchaseRequisitionLineModel;

namespace DynamicsNAV365_StaffPortal.Controllers.ProcurementServices
{
	public class PurchaseRequisitionController : Controller
	{
		string companyName = ServiceConnection.CompanyName;
		static string companyURL = "";

		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
		DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
		SuccessResponseController successResponse = new SuccessResponseController();
		InfoResponseController infoResponse = new InfoResponseController();
		ErrorResponseController errorResponse = new ErrorResponseController();
		FinanceHomeController financeHomeController = new FinanceHomeController();

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
		IEnumerable<SelectListItem> purchaseRequisitionTypes = null;
		List<SelectListItem> purchaseRequisitionCodes = null;
        List<SelectListItem> ProcurementPlans = null;
        IEnumerable<SelectListItem> ProcurementPlansList = null;
        IEnumerable<SelectListItem> purchaseRequisitionCodesList = null;
        IEnumerable<SelectListItem> sharedtypes = null;
        IEnumerable<SelectListItem> procurementPlans = null;

        AccountController accountController = new AccountController();
		string employeeNo = "";

		public PurchaseRequisitionController()
		{
			employeeNo = AccountController.GetEmployeeNo();
			//LoadPurchaseRequisitionTypes();
		}

		#region New Purchase Requisition
	
		[Authorize]
		public ActionResult NewPurchaseRequisition()
		{
			string purchaseRequisitionNo = "";
			try
			{
				PurchaseRequisitionHeaderModel purchaseRequisitionObj = new PurchaseRequisitionHeaderModel();
			//	Check open purchase requisition
				if (dynamicsNAVSOAPServices.procurementManagementWS.CheckOpenPurchaseRequisitionExists(employeeNo))
				{
					responseHeader = "Open Purchase Requisition";
					responseMessage = "An open purchase requisition exists for employee no. " + employeeNo + ", finalize on this purchase requisition before creating a new one.";
					detailedResponseMessage = "An open purchase requisition exists for employee no. " + employeeNo + ", finalize on this purchase requisition before creating a new one.";

					button1ControllerName = "PurchaseRequisition";
					button1ActionName = "PurchaseRequisitionHistory";
					button1Name = "Ok";
					button1Parameters = "";

					button2ControllerName = "";
					button2ActionName = "";
					button2HasParameters = false;
					button2Parameters = "";
					button2Name = "";

					return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
														  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
														  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
				}
			//	End check open purchase requisition

				//Create a new purchase requisition
				purchaseRequisitionNo = dynamicsNAVSOAPServices.procurementManagementWS.CreatePurchaseRequisition(employeeNo);
				//End create purchase requisition

				purchaseRequisitionObj.No = purchaseRequisitionNo;
				purchaseRequisitionObj.EmployeeNo = employeeNo;
				purchaseRequisitionObj.GlobalDimension1Code = "";

				LoadCurrencies();
				LoadDimensions(purchaseRequisitionObj.GlobalDimension1Code);

                GetSharedTypes();
                purchaseRequisitionObj.SharedTypes = new SelectList(sharedtypes, "Text", "Value");
                //LoadResponsibilityCenters("");
                //LoadLocationCodes();
                purchaseRequisitionObj.ProcurementPlans = new SelectList(Enumerable.Empty<SelectListItem>());
                purchaseRequisitionObj.ProcurementPlanItems = new SelectList(Enumerable.Empty<SelectListItem>());
                purchaseRequisitionObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
				purchaseRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
				purchaseRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
				purchaseRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
				purchaseRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Code");
				purchaseRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Code");
				purchaseRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Code");
				purchaseRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Code");
				purchaseRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Code");
				//purchaseRequisitionObj.ResponsibilityCenters = new SelectList(responsibilityCenters, "Code", "Code");
				//purchaseRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Code");

				//	purchaseRequisitionObj.DocumentDate 

				return View(purchaseRequisitionObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult> NewPurchaseRequisition(PurchaseRequisitionHeaderModel PurchaseRequisitionObj, string Command)
		{
			bool purchaseRequisitionModified = false;
			bool approvalWorkflowExist = false;

			try
			{
				LoadCurrencies();
				LoadDimensions(PurchaseRequisitionObj.GlobalDimension1Code);
				//	LoadResponsibilityCenters("");
				//	LoadLocationCodes();

				PurchaseRequisitionObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
				PurchaseRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
				PurchaseRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Code");
				//PurchaseRequisitionObj.ResponsibilityCenters = new SelectList(responsibilityCenters, "Code", "Code");
				//PurchaseRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Code");
				if (Command.Equals("Submit For Approval"))
				{
					if (ModelState.IsValid)
					{
						if (dynamicsNAVSOAPServices.procurementManagementWS.CheckPurchaseRequisitionExists(PurchaseRequisitionObj.No, AccountController.GetEmployeeNo()))
						{
							//Check purchase requisition lines
							if (!dynamicsNAVSOAPServices.procurementManagementWS.CheckPurchaseRequisitionLinesExist(PurchaseRequisitionObj.No))
							{
								PurchaseRequisitionObj.ErrorStatus = true;
								PurchaseRequisitionObj.ErrorMessage = "Purchase requisition lines missing, the purchase requisition must contain a minimum of one line, add a purchase requisition line to continue.";
								return View(PurchaseRequisitionObj);
							}

							//Validate purchase requisition lines
							string purchaseRequisitionLineError = "";
							purchaseRequisitionLineError = dynamicsNAVSOAPServices.procurementManagementWS.ValidatePurchaseRequisitionLines(PurchaseRequisitionObj.No);
							if (!purchaseRequisitionLineError.Equals(""))
							{
								PurchaseRequisitionObj.ErrorStatus = true;
								PurchaseRequisitionObj.ErrorMessage = purchaseRequisitionLineError;
								return View(PurchaseRequisitionObj);
							}

							//Check Budget Availability
							dynamicsNAVSOAPServices.procurementManagementWS.CheckBudgetPurchaseRequisition(PurchaseRequisitionObj.No, DateTime.Parse(PurchaseRequisitionObj.RequestedReceiptDate));
							//End Check Budget Availability

							//Modify purchase requisition 
							PurchaseRequisitionObj.CurrencyCode = PurchaseRequisitionObj.CurrencyCode != null ? PurchaseRequisitionObj.CurrencyCode : "";
							PurchaseRequisitionObj.GlobalDimension1Code = PurchaseRequisitionObj.GlobalDimension1Code != null ? PurchaseRequisitionObj.GlobalDimension1Code : "";
							PurchaseRequisitionObj.GlobalDimension2Code = PurchaseRequisitionObj.GlobalDimension2Code != null ? PurchaseRequisitionObj.GlobalDimension2Code : "";

							purchaseRequisitionModified = dynamicsNAVSOAPServices.procurementManagementWS.ModifyPurchaseRequisition(PurchaseRequisitionObj.No, PurchaseRequisitionObj.EmployeeNo, DateTime.ParseExact(PurchaseRequisitionObj.RequestedReceiptDate, "dd-MM-yy", CultureInfo.InvariantCulture),
																													 PurchaseRequisitionObj.Description);

							if (!purchaseRequisitionModified)
							{
								PurchaseRequisitionObj.ErrorStatus = true;
								PurchaseRequisitionObj.ErrorMessage = "An error was experienced while trying to modify purchase requisition no." + PurchaseRequisitionObj.No + ", the server might be offline, try again after a while.";
								return View(PurchaseRequisitionObj);
							}

							//Send purchase requisition for approval
							approvalWorkflowExist = dynamicsNAVSOAPServices.procurementManagementWS.CheckPurchaseRequisitionApprovalWorkflowEnabled(PurchaseRequisitionObj.No);
							if (!approvalWorkflowExist)
							{
								PurchaseRequisitionObj.ErrorStatus = true;
								PurchaseRequisitionObj.ErrorMessage = "An error was experienced while trying to send an approval request for purchase requisition no." + PurchaseRequisitionObj.No + ", the approval workflow was not found. Contact the " + companyName + " ICT department for assistance.";
								return View(PurchaseRequisitionObj);
							}

							if (dynamicsNAVSOAPServices.procurementManagementWS.SendPurchaseRequisitionApprovalRequest(PurchaseRequisitionObj.No))
							{
								responseHeader = "Success";
								responseMessage = "Purchase requisition no." + PurchaseRequisitionObj.No + " was successfully sent for approval. Check with the " + companyName + " procurement department for approval status.";
								detailedResponseMessage = "PurchaseRequisition no." + PurchaseRequisitionObj.No + " was successfully sent for approval. Check with the " + companyName + " procurement department for approval status.";
								button1ControllerName = "PurchaseRequisition";
								button1ActionName = "PurchaseRequisitionHistory";
								button1Name = "Ok";
								button1Parameters = "";

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
								PurchaseRequisitionObj.ErrorStatus = true;
								PurchaseRequisitionObj.ErrorMessage = "An error was experienced while trying to send an approval request for purchase requisition no." + PurchaseRequisitionObj.No + ". Contact the " + companyName + " ICT division for assistance.";
								return View(PurchaseRequisitionObj);
							}
						}
						else
						{
							responseHeader = "Purchase Requisition NotFound";
							responseMessage = "The purchase requisition no." + PurchaseRequisitionObj.No + " was not found for employee no." + AccountController.GetEmployeeNo();
							detailedResponseMessage = "The purchase requisition no." + PurchaseRequisitionObj.No + " was not found for employee no." + AccountController.GetEmployeeNo();
							button1ControllerName = "PurchaseRequisition";
							button1ActionName = "PurchaseRequisitionHistory";
							button1Name = "Ok";
							button1Parameters = "";

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
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(PurchaseRequisitionObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                    //string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(PurchaseRequisitionObj.No);

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
					//	return View(PurchaseRequisitionObj);
					//}
				}
				else
				{
					return View(PurchaseRequisitionObj);
				}
			}
			catch (Exception ex)
			{
				PurchaseRequisitionObj.ErrorStatus = true;
				PurchaseRequisitionObj.ErrorMessage = ex.Message.ToString();
				return View(PurchaseRequisitionObj);
			}
		}
		#endregion New Purchase Requisition

		#region Edit Purchase Requisition
		[Authorize]
		public ActionResult OnBeforeEdit(string PurchaseRequisitionNo)
		{
			try
			{
				if (PurchaseRequisitionNo.Equals(""))
				{
					return RedirectToAction("PurchaseRequisitionHistory", "PurchaseRequisition");
				}
				if (dynamicsNAVSOAPServices.procurementManagementWS.CheckPurchaseRequisitionExists(PurchaseRequisitionNo, AccountController.GetEmployeeNo()))
				{
					string purchaseRequisitionStatus = GetPurchaseRequisitionStatus(PurchaseRequisitionNo);
					//if purchase requisition is open
					if (purchaseRequisitionStatus.Equals("Open"))
					{
						return RedirectToAction("EditPurchaseRequisition", "PurchaseRequisition", new { PurchaseRequisitionNo = PurchaseRequisitionNo });
					}
					////if purchase requisition is pending approval
					//if (purchaseRequisitionStatus.Equals("Pending Approval"))
					//{
					//	responseHeader = "Purchase Requisition Pending Approval";
					//	responseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
					//	detailedResponseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
					//	returnControllerName = "PurchaseRequisition";
					//	returnActionName = "EditPurchaseRequisition";
					//	returnLinkName = "Yes";
					//	hasParameters = true;
					//	parameters = "?PurchaseRequisitionNo=" + PurchaseRequisitionNo;
					//	cancelControllerName = "PurchaseRequisition";
					//	cancelActionName = "PurchaseRequisitionHistory";
					//	cancelLinkName = "No";
					//	return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
					//										   returnControllerName, returnActionName, returnLinkName,
					//										   hasParameters, parameters, cancelControllerName,
					//										   cancelActionName, cancelLinkName);
					//}

					//if purchase requisition is pending approval
					if (purchaseRequisitionStatus.Equals("Pending Approval"))
					{
						responseHeader = "Purchase Requisition Pending Approval";
						responseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " is already submitted for approval. Editing not allowed.";
						detailedResponseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " is already submitted for approval. Editing not allowed.";

						button1ControllerName = "PurchaseRequisition";
						button1ActionName = "PurchaseRequisitionHistory";
						button1Name = "Ok";
						button1Parameters = "";

						button2ControllerName = "";
						button2ActionName = "";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}

					//if purchase requisition is released
					if (purchaseRequisitionStatus.Equals("Released"))
					{
						responseHeader = "Purchase Requisition Approved";
						responseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " is already approved. Editing not allowed.";
						detailedResponseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " is already approved. Editing not allowed.";

						button1ControllerName = "PurchaseRequisition";
						button1ActionName = "PurchaseRequisitionHistory";
						button1Name = "Ok";
						button1Parameters = "";

						button2ControllerName = "";
						button2ActionName = "";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";


						return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}
					//if purchase requisition is rejected
					if (purchaseRequisitionStatus.Equals("Rejected"))
					{
						responseHeader = "Purchase Requisition Rejected";
						responseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " was rejected. Editing will reopen the document. Do you want to continue?";
						detailedResponseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " was rejected. Editing will reopen the document. Do you want to continue?";

						button1ControllerName = "PurchaseRequisition";
						button1ActionName = "EditPurchaseRequisition";
						button1Name = "Yes";
						button1HasParameters = true;
						button1Parameters = "?PurchaseRequisitionNo=" + PurchaseRequisitionNo;

						button2ActionName = "PurchaseRequisition";
						button2ActionName = "PurchaseRequisitionHistory";
						button2Name = "No";
						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}
					//if purchase requisition is posted/reversed
					if (purchaseRequisitionStatus.Equals("Posted") || purchaseRequisitionStatus.Equals("Reversed"))
					{
						responseHeader = "Purchase Requisition Posted";
						responseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " is already posted. Editing not allowed.";
						detailedResponseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " is already posted. Editing not allowed.";

						button1ControllerName = "PurchaseRequisition";
						button1ActionName = "PurchaseRequisitionHistory";
						button1Name = "Ok";
						button1Parameters = "";

						button2ControllerName = "";
						button2ActionName = "";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";


						return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}
					return RedirectToAction("PurchaseRequisitionHistory", "PurchaseRequisition");
				}
				else
				{
					responseHeader = "Purchase Requisition NotFound";
					responseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					button1ControllerName = "PurchaseRequisition";
					button1ActionName = "PurchaseRequisitionHistory";
					button1Name = "Ok";
					button1Parameters = "";

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
		public ActionResult EditPurchaseRequisition(string PurchaseRequisitionNo)
		{
			try
			{
				if (PurchaseRequisitionNo.Equals(""))
				{
					return RedirectToAction("PurchaseRequisitionHistory", "PurchaseRequisition");
				}
				if (dynamicsNAVSOAPServices.procurementManagementWS.CheckPurchaseRequisitionExists(PurchaseRequisitionNo, AccountController.GetEmployeeNo()))
				{
					string purchaseRequisitionStatus = dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitionStatus(PurchaseRequisitionNo);

					//if purchase requisition is pending approval, cancel approval request
					if (purchaseRequisitionStatus.Equals("Pending Approval"))
					{
						dynamicsNAVSOAPServices.procurementManagementWS.CancelPurchaseRequisitionApprovalRequest(PurchaseRequisitionNo);
					}
					//if purchase requisition is released, reopen and uncommit from budget
					if (purchaseRequisitionStatus.Equals("Released"))
					{
						//dynamicsNAVSOAPServices.procurementManagementWS.ReopenPurchaseRequisition(PurchaseRequisitionNo);
						dynamicsNAVSOAPServices.procurementManagementWS.CancelBudgetCommitmentPurchaseRequisition(PurchaseRequisitionNo);
					}
					//if purchase requisition is rejected, reopen document
					if (purchaseRequisitionStatus.Equals("Rejected"))
					{
						dynamicsNAVSOAPServices.procurementManagementWS.ReopenPurchaseRequisition(PurchaseRequisitionNo);
					}

					PurchaseRequisitionHeaderModel purchaseRequisitionObj = new PurchaseRequisitionHeaderModel();
					//var purchaseRequisitions = from purchaseRequisitionsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PurchaseRequisitions
					//						   where purchaseRequisitionsQuery.No.Equals(PurchaseRequisitionNo)
					//						   select purchaseRequisitionsQuery;
					dynamic purchaseRequisitions = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitions(PurchaseRequisitionNo, ""));
					foreach (var purchaseRequisition in purchaseRequisitions)
					{
						purchaseRequisitionObj.No = purchaseRequisition.No;
						purchaseRequisitionObj.DocumentDate = purchaseRequisition.DocumentDate;
						purchaseRequisitionObj.RequestedReceiptDate = purchaseRequisition.RequestedReceiptDate.ToString("dd-MM-yy");
						purchaseRequisitionObj.EmployeeNo = purchaseRequisition.EmployeeNo;
						purchaseRequisitionObj.CurrencyCode = purchaseRequisition.CurrencyCode;
						purchaseRequisitionObj.Description = purchaseRequisition.Description;
						purchaseRequisitionObj.GlobalDimension1Code = purchaseRequisition.GlobalDimension1Code;
						purchaseRequisitionObj.GlobalDimension2Code = purchaseRequisition.GlobalDimension2Code;
						purchaseRequisitionObj.ShortcutDimension3Code = purchaseRequisition.ShortcutDimension3Code;
						purchaseRequisitionObj.ShortcutDimension4Code = purchaseRequisition.ShortcutDimension4Code;
						purchaseRequisitionObj.ShortcutDimension5Code = purchaseRequisition.ShortcutDimension5Code;
						purchaseRequisitionObj.ShortcutDimension6Code = purchaseRequisition.ShortcutDimension6Code;
						purchaseRequisitionObj.ShortcutDimension7Code = purchaseRequisition.ShortcutDimension7Code;
						purchaseRequisitionObj.ShortcutDimension8Code = purchaseRequisition.ShortcutDimension8Code;
						purchaseRequisitionObj.Amount = purchaseRequisition.Amount;
						purchaseRequisitionObj.Status = purchaseRequisition.Status;
					}

					LoadCurrencies();
					LoadDimensions(purchaseRequisitionObj.GlobalDimension1Code);
                    //	LoadResponsibilityCenters("");
                    //	LoadLocationCodes();

                    GetSharedTypes();
                    purchaseRequisitionObj.SharedTypes = new SelectList(sharedtypes, "Text", "Value");

                    //GetProcurementPlan();
                    //purchaseRequisitionObj.ProcurementPlans = new SelectList(procurementPlans, "Code", "Description");
                    purchaseRequisitionObj.ProcurementPlans = new SelectList(Enumerable.Empty<SelectListItem>());
                    purchaseRequisitionObj.ProcurementPlanItems = new SelectList(Enumerable.Empty<SelectListItem>());
                    purchaseRequisitionObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
					purchaseRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
					purchaseRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");
				//	purchaseRequisitionObj.ResponsibilityCenters = new SelectList(responsibilityCenters, "Code", "Code");
				//	purchaseRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Code");

					return View(purchaseRequisitionObj);
				}
				else
				{
					responseHeader = "Purchase Requisition NotFound";
					responseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " was not found for employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " was not found for employee no." + AccountController.GetEmployeeNo();
					button1ControllerName = "PurchaseRequisition";
					button1ActionName = "PurchaseRequisitionHistory";
					button1Name = "Ok";
					button1Parameters = "";

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

        private void GetSharedTypes()
        {
            List<SelectListItem> Gnder = new List<SelectListItem>
                                                    {
                                                        new SelectListItem { Text = "Not Shared", Value = "Not Shared" },
                                                        new SelectListItem { Text = "Shared", Value = "Shared" }
                                                    };
            sharedtypes = Gnder;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateHeader(string Description, string SharedType, string ProcurementPlan, string ProcurementPlanItem, string Amount, string DocNo)
        {
            try
            {
                bool ret;
                bool successVal = false;
                string msg = "";
                
                ret = Credentials.ObjNav.UpdatePurchaseRequestHeader(DocNo, Description, SharedType);
                if (ret)
                {
                    msg = "Lines Updated Successfully";
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

        [Authorize]
		[HttpPost]
		public async Task<ActionResult> EditPurchaseRequisition(PurchaseRequisitionHeaderModel PurchaseRequisitionObj, string Command)
		{
			bool purchaseRequisitionModified = false;
			bool approvalWorkflowExist = false;

			try
			{
				LoadCurrencies();
				LoadDimensions(PurchaseRequisitionObj.GlobalDimension1Code);
                //	LoadResponsibilityCenters("");
                //	LoadLocationCodes();
                GetSharedTypes();
                PurchaseRequisitionObj.SharedTypes = new SelectList(sharedtypes, "Text", "Value");

                PurchaseRequisitionObj.ProcurementPlans = new SelectList(Enumerable.Empty<SelectListItem>());
                PurchaseRequisitionObj.ProcurementPlanItems = new SelectList(Enumerable.Empty<SelectListItem>());
                PurchaseRequisitionObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
				PurchaseRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
				PurchaseRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Code");
				PurchaseRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Code");
				//PurchaseRequisitionObj.ResponsibilityCenters = new SelectList(responsibilityCenters, "Code", "Code");
				//PurchaseRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Code");
				if (Command.Equals("Submit For Approval"))
                {
					if (ModelState.IsValid)
					{
						if (dynamicsNAVSOAPServices.procurementManagementWS.CheckPurchaseRequisitionExists(PurchaseRequisitionObj.No, AccountController.GetEmployeeNo()))
						{
							//Check purchase requisition lines
							if (!dynamicsNAVSOAPServices.procurementManagementWS.CheckPurchaseRequisitionLinesExist(PurchaseRequisitionObj.No))
							{
								PurchaseRequisitionObj.ErrorStatus = true;
								PurchaseRequisitionObj.ErrorMessage = "Purchase requisition lines missing, the purchase requisition must contain a minimum of one line, add a purchase requisition line to continue.";
								return View(PurchaseRequisitionObj);
							}

							//Validate purchase requisition lines
							string purchaseRequisitionLineError = "";
							purchaseRequisitionLineError = dynamicsNAVSOAPServices.procurementManagementWS.ValidatePurchaseRequisitionLines(PurchaseRequisitionObj.No);
							if (!purchaseRequisitionLineError.Equals(""))
							{
								PurchaseRequisitionObj.ErrorStatus = true;
								PurchaseRequisitionObj.ErrorMessage = purchaseRequisitionLineError;
								return View(PurchaseRequisitionObj);
							}

							//Check Budget Availability
							dynamicsNAVSOAPServices.procurementManagementWS.CheckBudgetPurchaseRequisition(PurchaseRequisitionObj.No, DateTime.Parse(PurchaseRequisitionObj.RequestedReceiptDate));
							//End Check Budget Availability

							//Modify purchase requisition 
							PurchaseRequisitionObj.CurrencyCode = PurchaseRequisitionObj.CurrencyCode != null ? PurchaseRequisitionObj.CurrencyCode : "";
							PurchaseRequisitionObj.GlobalDimension1Code = PurchaseRequisitionObj.GlobalDimension1Code != null ? PurchaseRequisitionObj.GlobalDimension1Code : "";
							PurchaseRequisitionObj.GlobalDimension2Code = PurchaseRequisitionObj.GlobalDimension2Code != null ? PurchaseRequisitionObj.GlobalDimension2Code : "";

							purchaseRequisitionModified = dynamicsNAVSOAPServices.procurementManagementWS.ModifyPurchaseRequisition(PurchaseRequisitionObj.No, PurchaseRequisitionObj.EmployeeNo, DateTime.Parse(PurchaseRequisitionObj.RequestedReceiptDate),
																													 PurchaseRequisitionObj.Description);

							if (!purchaseRequisitionModified)
							{
								PurchaseRequisitionObj.ErrorStatus = true;
								PurchaseRequisitionObj.ErrorMessage = "An error was experienced while trying to modify purchase requisition no." + PurchaseRequisitionObj.No + ", the server might be offline, try again after a while.";
								return View(PurchaseRequisitionObj);
							}

							//Send purchase requisition for approval
							approvalWorkflowExist = dynamicsNAVSOAPServices.procurementManagementWS.CheckPurchaseRequisitionApprovalWorkflowEnabled(PurchaseRequisitionObj.No);
							if (!approvalWorkflowExist)
							{
								PurchaseRequisitionObj.ErrorStatus = true;
								PurchaseRequisitionObj.ErrorMessage = "An error was experienced while trying to send an approval request for purchase requisition no." + PurchaseRequisitionObj.No + ", the approval workflow was not found. Contact the " + companyName + " ICT department for assistance.";
								return View(PurchaseRequisitionObj);
							}

							if (dynamicsNAVSOAPServices.procurementManagementWS.SendPurchaseRequisitionApprovalRequest(PurchaseRequisitionObj.No))
							{
								responseHeader = "Success";
								responseMessage = "Purchase requisition no." + PurchaseRequisitionObj.No + " was successfully sent for approval. Check with the " + companyName + " procurement department for approval status.";
								detailedResponseMessage = "PurchaseRequisition no." + PurchaseRequisitionObj.No + " was successfully sent for approval. Check with the " + companyName + " procurement department for approval status.";
								button1ControllerName = "PurchaseRequisition";
								button1ActionName = "PurchaseRequisitionHistory";
								button1Name = "Ok";
								button1Parameters = "";

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
								PurchaseRequisitionObj.ErrorStatus = true;
								PurchaseRequisitionObj.ErrorMessage = "An error was experienced while trying to send an approval request for purchase requisition no." + PurchaseRequisitionObj.No + ". Contact the " + companyName + " ICT division for assistance.";
								return View(PurchaseRequisitionObj);
							}
						}
						else
						{
							responseHeader = "Purchase Requisition NotFound";
							responseMessage = "The purchase requisition no." + PurchaseRequisitionObj.No + " was not found for employee no." + AccountController.GetEmployeeNo();
							detailedResponseMessage = "The purchase requisition no." + PurchaseRequisitionObj.No + " was not found for employee no." + AccountController.GetEmployeeNo();
							button1ControllerName = "PurchaseRequisition";
							button1ActionName = "PurchaseRequisitionHistory";
							button1Name = "Ok";
							button1Parameters = "";

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
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(PurchaseRequisitionObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                    //	string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(PurchaseRequisitionObj.No);

                    //	string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

                    //	if (!fileURL.Equals(""))
                    //	{
                    //		using (WebClient wc = new WebClient())
                    //		{
                    //			if (ext.Equals(".pdf"))
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "application/pdf");
                    //			}

                    //			else if (ext.Equals(".doc"))
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "application/msword");
                    //			}

                    //			else if (ext.Equals(".docx"))
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                    //			}

                    //			else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "image/jpeg");
                    //			}

                    //			else if (ext.Equals(".json"))
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "application/json");
                    //			}

                    //			else if (ext.Equals(".ppt"))
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "application/vnd.ms-powerpoint");
                    //			}

                    //			else if (ext.Equals(".png"))
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "image/png");
                    //			}

                    //			else if (ext.Equals(".pptx"))
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
                    //			}

                    //			else if (ext.Equals(".rar"))
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "application/vnd.rar");
                    //			}

                    //			else if (ext.Equals(".xls"))
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "application/vnd.ms-excel");
                    //			}

                    //			else if (ext.Equals(".xlsx"))
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    //			}

                    //			else
                    //			{
                    //				var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                    //				return File(byteArr, "text/plain");
                    //			}
                    //		}
                    //	}


                    //	else
                    //	{
                    //		return View(PurchaseRequisitionObj);
                    //	}
                }
                else
                {
                    return View(PurchaseRequisitionObj);
                }

            }
			catch (Exception ex)
			{
				PurchaseRequisitionObj.ErrorStatus = true;
				PurchaseRequisitionObj.ErrorMessage = ex.Message.ToString();
				return View(PurchaseRequisitionObj);
			}
		}
		#endregion Edit Purchase Requisition

		#region View Purchase Requisition
	
		[Authorize]
		public ActionResult ViewPurchaseRequisition(string PurchaseRequisitionNo)
		{
			try
			{
				if (PurchaseRequisitionNo.Equals(""))
				{
					return RedirectToAction("PurchaseRequisitionHistory", "PurchaseRequisition");
				}
				if (dynamicsNAVSOAPServices.procurementManagementWS.CheckPurchaseRequisitionExists(PurchaseRequisitionNo, AccountController.GetEmployeeNo()))
				{
					PurchaseRequisitionHeaderModel purchaseRequisitionObj = new PurchaseRequisitionHeaderModel();
					//var purchaseRequisitions = from purchaseRequisitionsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PurchaseRequisitions
					//						   where purchaseRequisitionsQuery.No.Equals(PurchaseRequisitionNo)
					//						   select purchaseRequisitionsQuery;
					dynamic purchaseRequisitions = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitions(PurchaseRequisitionNo, ""));
					foreach (var purchaseRequisition in purchaseRequisitions)
					{
						purchaseRequisitionObj.No = purchaseRequisition.No;
						purchaseRequisitionObj.DocumentDate = purchaseRequisition.DocumentDate;
						purchaseRequisitionObj.RequestedReceiptDate = purchaseRequisition.RequestedReceiptDate.ToString("dd-MM-yy");
						purchaseRequisitionObj.EmployeeNo = purchaseRequisition.EmployeeNo;
						purchaseRequisitionObj.CurrencyCode = purchaseRequisition.CurrencyCode;
						purchaseRequisitionObj.Description = purchaseRequisition.Description;
						purchaseRequisitionObj.GlobalDimension1Code = purchaseRequisition.GlobalDimension1Code;
						purchaseRequisitionObj.GlobalDimension2Code = purchaseRequisition.GlobalDimension2Code;
						purchaseRequisitionObj.ShortcutDimension3Code = purchaseRequisition.ShortcutDimension3Code;
						purchaseRequisitionObj.ShortcutDimension4Code = purchaseRequisition.ShortcutDimension4Code;
						purchaseRequisitionObj.ShortcutDimension5Code = purchaseRequisition.ShortcutDimension5Code;
						purchaseRequisitionObj.ShortcutDimension6Code = purchaseRequisition.ShortcutDimension6Code;
						purchaseRequisitionObj.ShortcutDimension7Code = purchaseRequisition.ShortcutDimension7Code;
						purchaseRequisitionObj.ShortcutDimension8Code = purchaseRequisition.ShortcutDimension8Code;
						purchaseRequisitionObj.Amount = purchaseRequisition.Amount;
						purchaseRequisitionObj.Status = purchaseRequisition.Status;
					}

					LoadCurrencies();
					LoadDimensions(purchaseRequisitionObj.GlobalDimension1Code);
				//	LoadResponsibilityCenters("");
				//	LoadLocationCodes();

					purchaseRequisitionObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
					purchaseRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
					purchaseRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
					purchaseRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");
				//	purchaseRequisitionObj.ResponsibilityCenters = new SelectList(responsibilityCenters, "Code", "Code");
				//	purchaseRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Code");

					return View(purchaseRequisitionObj);
				}
				else
				{
					responseHeader = "Purchase Requisition NotFound";
					responseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " was not found for employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The purchase requisition no." + PurchaseRequisitionNo + " was not found for employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "PurchaseRequisition";
					button1ActionName = "PurchaseRequisitionHistory";
					button1Name = "Ok";
					button1Parameters = "";

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
		public async Task<ActionResult> ViewPurchaseRequisition(PurchaseRequisitionHeaderModel purchaseRequisitionObj, string Command)
		{
			try
			{
				if (purchaseRequisitionObj.No.Equals(""))
				{
					return RedirectToAction("PurchaseRequisitionHistory", "PurchaseRequisition");
				}
				if (Command.Equals("View Attachment"))
				{
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(purchaseRequisitionObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
					//string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(purchaseRequisitionObj.No);

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
					//	return View(purchaseRequisitionObj);
					//}
				}
				else
				{
					purchaseRequisitionObj.ErrorStatus = true;
					//leaveApplicationObj2.ErrorMessage = "Unable to process the approve/reject action. Contact the " + companyName + " for assistance.";
					return View(purchaseRequisitionObj);
				}
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		#endregion View Purchase Requisition

		#region Purchase Requisition Line

		[ChildActionOnly]
		[Authorize]
		public ActionResult _PurchaseRequisitionLine(string PurchaseRequisitionNo)
		{
			PurchaseRequisitionLineModel purchaseRequisitionLineObj = new PurchaseRequisitionLineModel();

			string globalDimension1Code = dynamicsNAVSOAPServices.procurementManagementWS.GetGlobalDimension1Code(PurchaseRequisitionNo);

			LoadDimensions(globalDimension1Code);
			//LoadLocationCodes();
			//LoadItemUOMs();

			LoadPurchaseRequisitionTypes();
            //LoadProcurementPlan();
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

            purchaseRequisitionLineObj.RequisitionTypes = new SelectList(purchaseRequisitionTypes, "Value", "Text");
			purchaseRequisitionLineObj.RequisitionCodes = new SelectList(Enumerable.Empty<SelectListItem>());
            purchaseRequisitionLineObj.ProcurementPlansLns = new SelectList(Enumerable.Empty<SelectListItem>());
            purchaseRequisitionLineObj.ProcurementPlanItemsLns = new SelectList(Enumerable.Empty<SelectListItem>());
            purchaseRequisitionLineObj.Dimension1s = new SelectList(DimensionValues, "Code", "Name");
            purchaseRequisitionLineObj.Dimension2s = new SelectList(Enumerable.Empty<SelectListItem>());
            purchaseRequisitionLineObj.Dimension3s = new SelectList(Enumerable.Empty<SelectListItem>());
            purchaseRequisitionLineObj.Dimension4s = new SelectList(Enumerable.Empty<SelectListItem>());
            purchaseRequisitionLineObj.Dimension5s = new SelectList(Enumerable.Empty<SelectListItem>());
            purchaseRequisitionLineObj.Dimension6s = new SelectList(Enumerable.Empty<SelectListItem>());
            purchaseRequisitionLineObj.Dimension7s = new SelectList(Enumerable.Empty<SelectListItem>());
            //purchaseRequisitionLineObj.LineLocationCodes = new SelectList(locations, "Code", "Code");
            purchaseRequisitionLineObj.UOMs = new SelectList(Enumerable.Empty<SelectListItem>());
			purchaseRequisitionLineObj.LineGlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineGlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

			return PartialView(purchaseRequisitionLineObj);
		}
        public JsonResult GetDimension2(string Dimension1)
        {
            try
            {
                #region Items List
                List<DimensionValues> DimensionValues = new List<DimensionValues>();
                string dimension1list = "DimesnionCombinations?$filter=Dimension_1_Value_Code eq '" + HttpUtility.UrlEncode(Dimension1) + "' &$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(dimension1list);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues DList1 = new DimensionValues();
                        DList1.Code = (string)config["Dimension_2_Code"];
                        DList1.Name = (string)config["Dimension_2_Value_Code"];
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
        public JsonResult GetDimension3(string Dimension2)
        {
            try
            {
                #region Items List
                List<DimensionValues> DimensionValues = new List<DimensionValues>();
                string dimension1list = "DimesnionCombinations?$filter=Dimension_1_Value_Code eq '" + HttpUtility.UrlEncode(Dimension2) + "' &$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(dimension1list);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues DList1 = new DimensionValues();
                        DList1.Code = (string)config["Dimension_2_Code"];
                        DList1.Name = (string)config["Dimension_2_Value_Code"];
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
        public JsonResult GetDimension4(string Dimension3)
        {
            try
            {
                #region Items List
                List<DimensionValues> DimensionValues = new List<DimensionValues>();
                string dimension1list = "DimesnionCombinations?$filter=Dimension_1_Value_Code eq '" + HttpUtility.UrlEncode(Dimension3) + "' &$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(dimension1list);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues DList1 = new DimensionValues();
                        DList1.Code = (string)config["Dimension_2_Code"];
                        DList1.Name = (string)config["Dimension_2_Value_Code"];
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
        public JsonResult GetDimension5(string Dimension4)
        {
            try
            {
                #region Items List
                List<DimensionValues> DimensionValues = new List<DimensionValues>();
                string dimension1list = "DimesnionCombinations?$filter=Dimension_1_Value_Code eq '" + HttpUtility.UrlEncode(Dimension4) + "' &$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(dimension1list);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues DList1 = new DimensionValues();
                        DList1.Code = (string)config["Dimension_2_Code"];
                        DList1.Name = (string)config["Dimension_2_Value_Code"];
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
        public JsonResult GetDimension6(string Dimension5)
        {
            try
            {
                #region Items List
                List<DimensionValues> DimensionValues = new List<DimensionValues>();
                string dimension1list = "DimesnionCombinations?$filter=Dimension_1_Value_Code eq '" + HttpUtility.UrlEncode(Dimension5) + "' &$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(dimension1list);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues DList1 = new DimensionValues();
                        DList1.Code = (string)config["Dimension_2_Code"];
                        DList1.Name = (string)config["Dimension_2_Value_Code"];
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
        public JsonResult GetDimension7(string Dimension6)
        {
            try
            {
                #region Items List
                List<DimensionValues> DimensionValues = new List<DimensionValues>();
                string dimension1list = "DimesnionCombinations?$filter=Dimension_1_Value_Code eq '" + HttpUtility.UrlEncode(Dimension6) + "' &$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(dimension1list);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues DList1 = new DimensionValues();
                        DList1.Code = (string)config["Dimension_2_Code"];
                        DList1.Name = (string)config["Dimension_2_Value_Code"];
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
        public JsonResult GetProcurementPlanItemsre(PurchaseRequisitionLineModel PurchaseRequisitionLineObj)
        {
            try
            {
                #region Items List
                List<ProcurementPlanItems> ProcurementPlanItems = new List<ProcurementPlanItems>();
                string dimension1list = "ProcurementPlanItemss?$filter=Plan_Year eq '" + HttpUtility.UrlEncode(PurchaseRequisitionLineObj.ProcurementPlan) + "' and Global_Dimension_1_Code eq '"+ PurchaseRequisitionLineObj .LineGlobalDimension1Code+ "' and Global_Dimension_2_Code eq '"+ PurchaseRequisitionLineObj .LineGlobalDimension2Code+ "' &$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(dimension1list);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        ProcurementPlanItems Plist = new ProcurementPlanItems();
                        Plist.Code = (string)config["Plan_Item_No"];
                        Plist.Description = (string)config["Item_Description"];
                        ProcurementPlanItems.Add(Plist);
                    }
                }
                #endregion
                DropdownListData DropDownData = new DropdownListData
                {
                    ListOfddlData = ProcurementPlanItems.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Code,
                                         Value = x.Description
                                     }).ToList()
                };
                return Json(new { DropDownData, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [ChildActionOnly]
		[Authorize]
		public ActionResult _ViewPurchaseRequisitionLine(string PurchaseRequisitionNo)
		{
			PurchaseRequisitionLineModel purchaseRequisitionLineObj = new PurchaseRequisitionLineModel();

			string globalDimension1Code = dynamicsNAVSOAPServices.procurementManagementWS.GetGlobalDimension1Code(PurchaseRequisitionNo);

            //LoadDimensions("");
            LoadDimensions(globalDimension1Code);
            //LoadLocationCodes();
            //LoadItemUOMs();

            LoadPurchaseRequisitionTypes();
			purchaseRequisitionLineObj.RequisitionTypes = new SelectList(purchaseRequisitionTypes, "Value", "Text");
			purchaseRequisitionLineObj.RequisitionCodes = new SelectList(Enumerable.Empty<SelectListItem>());
            purchaseRequisitionLineObj.ProcurementPlans = new SelectList(Enumerable.Empty<SelectListItem>());
            purchaseRequisitionLineObj.Dimension1s = new SelectList(Enumerable.Empty<SelectListItem>());
            //purchaseRequisitionLineObj.LineLocationCodes = new SelectList(locations, "Code", "Code");
            purchaseRequisitionLineObj.UOMs = new SelectList(Enumerable.Empty<SelectListItem>());
			purchaseRequisitionLineObj.LineGlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineGlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
			purchaseRequisitionLineObj.LineShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");
			return PartialView(purchaseRequisitionLineObj);
		}

		[Authorize]
		public JsonResult GetPurchaseRequisitionLines(string DocumentNo)
		{
            List<PurchaseRequisitionLineModel> purchaseRequisitionLinesList = new List<PurchaseRequisitionLineModel>();
            string imprestlines = "RequisitionLine?$filter=Requisition_No eq '" + DocumentNo + "' &$format=json";

            HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(imprestlines);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    PurchaseRequisitionLineModel imprestLineObj = new PurchaseRequisitionLineModel();
                    imprestLineObj.LineNo = (string)config1["Line_No"];
                    imprestLineObj.DocumentNo = (string)config1["Requisition_No"];
                    imprestLineObj.RequisitionType = (string)config1["Type"];
                    imprestLineObj.ProcurementPlan = (string)config1["Procurement_Plan"];
                    imprestLineObj.ProcurementPlanItem = (string)config1["Procurement_Plan_Item"];
                    imprestLineObj.No = (string)config1["No"];
                    imprestLineObj.LineDescription = (string)config1["Description_2"];
                    imprestLineObj.UOM = (string)config1["Unit_of_Measure"];
                    imprestLineObj.UnitCost = (string)config1["Unit_Price"];
                    imprestLineObj.Quantity = (string)config1["Quantity"];
                    imprestLineObj.TotalLineCost = (string)config1["Amount"];
                    imprestLineObj.Dimension1 = (string)config1["Global_Dimension_1_Code"];
                    imprestLineObj.Dimension2 = (string)config1["Global_Dimension_2_Code"];
                    imprestLineObj.Dimension3 = (string)config1["ShortcutDimCode3"];
                    imprestLineObj.Dimension4 = (string)config1["ShortcutDimCode4"];
                    imprestLineObj.Dimension5 = (string)config1["ShortcutDimCode5"];
                    imprestLineObj.Dimension6 = (string)config1["ShortcutDimCode6"];
                    imprestLineObj.Dimension7 = (string)config1["ShortcutDimCode7"];
                    imprestLineObj.Status = (string)config1["Status"];
                    purchaseRequisitionLinesList.Add(imprestLineObj);
                }
            }
            //List<PurchaseRequisitionLineModel> purchaseRequisitionLinesList = JsonConvert.DeserializeObject<List<PurchaseRequisitionLineModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitionLines(DocumentNo));
            return Json(purchaseRequisitionLinesList, JsonRequestBehavior.AllowGet);
        }

		[Authorize]
		public ActionResult GetPurchaseRequisitionLine(string LineNo, string DocumentNo)
		{
			try
			{
                //List<PurchaseRequisitionLineModel> purchaseRequisitionLinesList = new List<PurchaseRequisitionLineModel>();
                PurchaseRequisitionLineModel purchaseRequisitionLinesList = new PurchaseRequisitionLineModel();
                string imprestlines = "RequisitionLine?$filter=Requisition_No eq '" + DocumentNo + "'and Line_No eq "+ Convert.ToInt32(LineNo) + "  &$format=json";

                HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(imprestlines);
                using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                {
                    var result1 = streamReader1.ReadToEnd();

                    var details1 = JObject.Parse(result1);

                    foreach (JObject config1 in details1["value"])
                    {
                        PurchaseRequisitionLineModel imprestLineObj = new PurchaseRequisitionLineModel();
                        imprestLineObj.LineNo = (string)config1["Line_No"];
                        imprestLineObj.DocumentNo = (string)config1["Requisition_No"];
                        imprestLineObj.RequisitionType = (string)config1["Type"];
                        imprestLineObj.ProcurementPlan = (string)config1["Procurement_Plan"];
                        imprestLineObj.ProcurementPlanItem = (string)config1["Procurement_Plan_Item"];
                        imprestLineObj.No = (string)config1["No"];
                        imprestLineObj.LineDescription = (string)config1["Description_2"];
                        imprestLineObj.UOM = (string)config1["Unit_of_Measure"];
                        imprestLineObj.UnitCost = (string)config1["Unit_Price"];
                        imprestLineObj.Quantity = (string)config1["Quantity"];
                        imprestLineObj.TotalLineCost = (string)config1["Amount"];
                        imprestLineObj.Dimension1 = (string)config1["Global_Dimension_1_Code"];
                        imprestLineObj.Dimension2 = (string)config1["Global_Dimension_2_Code"];
                        imprestLineObj.Dimension3 = (string)config1["ShortcutDimCode3"];
                        imprestLineObj.Dimension4 = (string)config1["ShortcutDimCode4"];
                        imprestLineObj.Dimension5 = (string)config1["ShortcutDimCode5"];
                        imprestLineObj.Dimension6 = (string)config1["ShortcutDimCode6"];
                        imprestLineObj.Dimension7 = (string)config1["ShortcutDimCode7"];
                        imprestLineObj.Status = (string)config1["Status"];
                        //purchaseRequisitionLinesList.Add(imprestLineObj);
                        purchaseRequisitionLinesList=imprestLineObj;
                    }
                }
                //List<PurchaseRequisitionLineModel> purchaseRequisitionLinesList = JsonConvert.DeserializeObject<List<PurchaseRequisitionLineModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitionLines(DocumentNo));
                return Json(purchaseRequisitionLinesList, JsonRequestBehavior.AllowGet);

                //PurchaseRequisitionLineModel purchaseRequisitionLineObj = JsonConvert.DeserializeObject<PurchaseRequisitionLineModel>(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitionByLineNo(Convert.ToInt32(LineNo), DocumentNo));
				//return Json(purchaseRequisitionLineObj, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		public JsonResult CreatePurchaseRequisitionLine(PurchaseRequisitionLineModel PurchaseRequisitionLineObj)
		{
            try
            {
                bool purchaseRequisitionLineCreated = false;

                PurchaseRequisitionLineObj.LineGlobalDimension1Code = PurchaseRequisitionLineObj.LineGlobalDimension1Code != null ? PurchaseRequisitionLineObj.LineGlobalDimension1Code : "";

                purchaseRequisitionLineCreated = dynamicsNAVSOAPServices.procurementManagementWS.CreatePurchaseRequisitionLine(PurchaseRequisitionLineObj.DocumentNo,
                                                PurchaseRequisitionLineObj.LineGlobalDimension1Code ?? "", PurchaseRequisitionLineObj.LineGlobalDimension2Code ?? "", PurchaseRequisitionLineObj.LineShortcutDimension3Code ?? "", PurchaseRequisitionLineObj.LineShortcutDimension4Code ?? "",
                                                PurchaseRequisitionLineObj.LineShortcutDimension5Code ?? "", PurchaseRequisitionLineObj.LineShortcutDimension6Code ?? "", PurchaseRequisitionLineObj.LineShortcutDimension7Code ?? "",
                                                Convert.ToInt32(PurchaseRequisitionLineObj.Quantity), Convert.ToDecimal(PurchaseRequisitionLineObj.UnitCost),
                                                PurchaseRequisitionLineObj.ProcurementPlan, PurchaseRequisitionLineObj.ProcurementPlanItem, PurchaseRequisitionLineObj.LineDescription);


                return Json(new { PurchaseRequisitionLineCreated = purchaseRequisitionLineCreated, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false, view = false }, JsonRequestBehavior.AllowGet); ;
            }
		}

		[Authorize]
		public JsonResult ModifyPurchaseRequisitionLine(PurchaseRequisitionLineModel PurchaseRequisitionLineObj)
		{
			bool purchaseRequisitionLineModified = false;

			PurchaseRequisitionLineObj.LineGlobalDimension1Code = PurchaseRequisitionLineObj.LineGlobalDimension1Code != null ? PurchaseRequisitionLineObj.LineGlobalDimension1Code : "";
			PurchaseRequisitionLineObj.LineGlobalDimension2Code = PurchaseRequisitionLineObj.LineGlobalDimension2Code != null ? PurchaseRequisitionLineObj.LineGlobalDimension2Code : "";

			purchaseRequisitionLineModified = dynamicsNAVSOAPServices.procurementManagementWS.ModifyPurchaseRequisitionLine(Convert.ToInt32(PurchaseRequisitionLineObj.LineNo), PurchaseRequisitionLineObj.DocumentNo, Convert.ToInt32(PurchaseRequisitionLineObj.Quantity), Convert.ToDecimal(PurchaseRequisitionLineObj.UnitCost), PurchaseRequisitionLineObj.LineDescription??"",
                PurchaseRequisitionLineObj.Dimension1??"", PurchaseRequisitionLineObj.Dimension2??"", PurchaseRequisitionLineObj.Dimension3??"", PurchaseRequisitionLineObj.Dimension4??"", PurchaseRequisitionLineObj.Dimension5??"", PurchaseRequisitionLineObj.Dimension6??"", PurchaseRequisitionLineObj.Dimension7??"", PurchaseRequisitionLineObj.ProcurementPlan??"",Convert.ToInt16( PurchaseRequisitionLineObj.ProcurementPlanItem??"0"));

            return Json(new { PurchaseRequisitionLineModified = purchaseRequisitionLineModified }, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult DeletePurchaseRequisitionLine(int LineNo, string DocumentNo)
		{
			bool purchaseRequisitionLineDeleted = false;

			purchaseRequisitionLineDeleted = dynamicsNAVSOAPServices.procurementManagementWS.DeletePurchaseRequisitionLine(LineNo, DocumentNo);

			return Json(new { PurchaseRequisitionLineDeleted = purchaseRequisitionLineDeleted, success=true }, JsonRequestBehavior.AllowGet);
		}
		#endregion Purchase Requisition Line

		#region Purchase requisitions history

		[Authorize]
		public ActionResult PurchaseRequisitionHistory()
		{
			try
			{
				return View(JsonConvert.DeserializeObject<List<PurchaseRequisitionHeaderModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitions("", employeeNo)));
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		public JsonResult GetPurchaseRequisitions()
		{
			List<PurchaseRequisitionHeaderModel> purchaseRequisitionsList = new List<PurchaseRequisitionHeaderModel>();
			FinanceHomeController financeHomeController = new FinanceHomeController();

			//var purchaseRequisitions = from purchaseRequisitionsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PurchaseRequisitions
			//						   where purchaseRequisitionsQuery.Employee_No.Equals(employeeNo)
			//						   select purchaseRequisitionsQuery;

			dynamic purchaseRequisitions = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitions("", employeeNo));

			foreach (var purchaseRequisition in purchaseRequisitions)
			{
				PurchaseRequisitionHeaderModel purchaseRequisitionObj = new PurchaseRequisitionHeaderModel();
				purchaseRequisitionObj.No = purchaseRequisition.No;
				purchaseRequisitionObj.DocumentDate = purchaseRequisition.DocumentDate;
				purchaseRequisitionObj.RequestedReceiptDate = purchaseRequisition.RequestedReceiptDate.ToString("dd-MM-yy");
				purchaseRequisitionObj.EmployeeNo = purchaseRequisition.EmployeeNo;
				purchaseRequisitionObj.CurrencyCode = purchaseRequisition.CurrencyCode;
				purchaseRequisitionObj.Description = purchaseRequisition.Description;
				purchaseRequisitionObj.GlobalDimension1Code = purchaseRequisition.GlobalDimension1Code;
				purchaseRequisitionObj.GlobalDimension2Code = purchaseRequisition.GlobalDimension2Code;
				purchaseRequisitionObj.ShortcutDimension3Code = purchaseRequisition.ShortcutDimension3Code;
				purchaseRequisitionObj.ShortcutDimension4Code = purchaseRequisition.ShortcutDimension4Code;
				purchaseRequisitionObj.ShortcutDimension5Code = purchaseRequisition.ShortcutDimension5Code;
				purchaseRequisitionObj.ShortcutDimension6Code = purchaseRequisition.ShortcutDimension6Code;
				purchaseRequisitionObj.ShortcutDimension7Code = purchaseRequisition.ShortcutDimension7Code;
				purchaseRequisitionObj.ShortcutDimension8Code = purchaseRequisition.ShortcutDimension8Code;
				purchaseRequisitionObj.Amount = purchaseRequisition.Amount;
				purchaseRequisitionObj.Status = purchaseRequisition.Status;
				purchaseRequisitionsList.Add(purchaseRequisitionObj);
			}
			return Json(purchaseRequisitionsList, JsonRequestBehavior.AllowGet);
		}

		#endregion Purchase requisitions history

		#region Purchase Requisition Approval
	
		[Authorize]
		public ActionResult PurchaseRequisitionApproval(string PurchaseRequisitionNo)
		{
			try
			{
				if (PurchaseRequisitionNo.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}

				PurchaseRequisitionHeaderModel purchaseRequisitionObj = new PurchaseRequisitionHeaderModel();
				//var purchaseRequisitions = from purchaseRequisitionsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PurchaseRequisitions
				//						   where purchaseRequisitionsQuery.No.Equals(PurchaseRequisitionNo)
				//						   select purchaseRequisitionsQuery;
				dynamic purchaseRequisitions = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitions(PurchaseRequisitionNo, ""));
				foreach (var purchaseRequisition in purchaseRequisitions)
				{
					purchaseRequisitionObj.No = purchaseRequisition.No;
					purchaseRequisitionObj.DocumentDate = purchaseRequisition.DocumentDate;
					purchaseRequisitionObj.RequestedReceiptDate = purchaseRequisition.RequestedReceiptDate.ToString("dd-MM-yy");
					purchaseRequisitionObj.EmployeeNo = purchaseRequisition.EmployeeNo;
					purchaseRequisitionObj.CurrencyCode = purchaseRequisition.CurrencyCode;
					purchaseRequisitionObj.Description = purchaseRequisition.Description;
					purchaseRequisitionObj.GlobalDimension1Code = purchaseRequisition.GlobalDimension1Code;
					purchaseRequisitionObj.GlobalDimension2Code = purchaseRequisition.GlobalDimension2Code;
					purchaseRequisitionObj.ShortcutDimension3Code = purchaseRequisition.ShortcutDimension3Code;
					purchaseRequisitionObj.ShortcutDimension4Code = purchaseRequisition.ShortcutDimension4Code;
					purchaseRequisitionObj.ShortcutDimension5Code = purchaseRequisition.ShortcutDimension5Code;
					purchaseRequisitionObj.ShortcutDimension6Code = purchaseRequisition.ShortcutDimension6Code;
					purchaseRequisitionObj.ShortcutDimension7Code = purchaseRequisition.ShortcutDimension7Code;
					purchaseRequisitionObj.ShortcutDimension8Code = purchaseRequisition.ShortcutDimension8Code;
					purchaseRequisitionObj.Amount = purchaseRequisition.Amount;
					purchaseRequisitionObj.Status = purchaseRequisition.Status;
				}

				LoadCurrencies();
				LoadDimensions(purchaseRequisitionObj.GlobalDimension1Code);
				//LoadResponsibilityCenters("");
				//LoadLocationCodes();

				purchaseRequisitionObj.CurrencyCodes = new SelectList(currencyCodes, "Code", "Code");
				purchaseRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
				purchaseRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
				purchaseRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
				purchaseRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
				purchaseRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
				purchaseRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
				purchaseRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
				purchaseRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");
				//purchaseRequisitionObj.ResponsibilityCenters = new SelectList(responsibilityCenters, "Code", "Code");
				//purchaseRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Code");

				return View(purchaseRequisitionObj);

			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> PurchaseRequisitionApproval(PurchaseRequisitionHeaderModel PurchaseRequisitionObj, string Command)
		{
			try
			{
				if (PurchaseRequisitionObj.No.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}
				if (Command == "Approve")
				{
					PurchaseRequisitionObj.Comments = PurchaseRequisitionObj.Comments != null ? PurchaseRequisitionObj.Comments : "";
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveRequisitionCopy(employeeNo, PurchaseRequisitionObj.No, PurchaseRequisitionObj.Comments, AccountController.GetEmployeeNo()))
					{
						responseHeader = "Success";
						responseMessage = "Purchase Requisition no." + PurchaseRequisitionObj.No + " was successfully approved.";
						detailedResponseMessage = "Purchase Requisition no." + PurchaseRequisitionObj.No + " was successfully approved.";
						button1ControllerName = "Approval";
						button1ActionName = "OpenEntries";
						button1Name = "Ok";
						button1Parameters = "";

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
						PurchaseRequisitionObj.ErrorStatus = true;
						PurchaseRequisitionObj.ErrorMessage = "Unable to process the Purchase Requisition approve action.  " + ServiceConnection.contactICTDepartment + "";
						return View(PurchaseRequisitionObj);
					}
				}
				else if (Command == "Reject")
				{
					PurchaseRequisitionObj.Comments = PurchaseRequisitionObj.Comments != null ? PurchaseRequisitionObj.Comments : "";
					if (PurchaseRequisitionObj.Comments.Equals(""))
					{
						dynamic purchaseRequisitions = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitions(PurchaseRequisitionObj.No, ""));
						foreach (var purchaseRequisition in purchaseRequisitions)
						{
							PurchaseRequisitionObj.No = purchaseRequisition.No;
							PurchaseRequisitionObj.DocumentDate = purchaseRequisition.DocumentDate;
							PurchaseRequisitionObj.RequestedReceiptDate = purchaseRequisition.RequestedReceiptDate;
							PurchaseRequisitionObj.EmployeeNo = purchaseRequisition.EmployeeNo;
							PurchaseRequisitionObj.CurrencyCode = purchaseRequisition.CurrencyCode;
							PurchaseRequisitionObj.Description = purchaseRequisition.Description;
							PurchaseRequisitionObj.GlobalDimension1Code = purchaseRequisition.GlobalDimension1Code;
							PurchaseRequisitionObj.GlobalDimension2Code = purchaseRequisition.GlobalDimension2Code;
							PurchaseRequisitionObj.ShortcutDimension3Code = purchaseRequisition.ShortcutDimension3Code;
							PurchaseRequisitionObj.ShortcutDimension4Code = purchaseRequisition.ShortcutDimension4Code;
							PurchaseRequisitionObj.ShortcutDimension5Code = purchaseRequisition.ShortcutDimension5Code;
							PurchaseRequisitionObj.ShortcutDimension6Code = purchaseRequisition.ShortcutDimension6Code;
							PurchaseRequisitionObj.ShortcutDimension7Code = purchaseRequisition.ShortcutDimension7Code;
							PurchaseRequisitionObj.ShortcutDimension8Code = purchaseRequisition.ShortcutDimension8Code;
							PurchaseRequisitionObj.Amount = purchaseRequisition.Amount;
							PurchaseRequisitionObj.Status = purchaseRequisition.Status;
						}

						PurchaseRequisitionObj.ErrorStatus = true;
						PurchaseRequisitionObj.ErrorMessage = "Kindly provide reason (s) for declining/rejecting this document.";
						return View(PurchaseRequisitionObj);
					}
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectRequisition(employeeNo, PurchaseRequisitionObj.No, PurchaseRequisitionObj.Comments))
					{
						responseHeader = "Success";
						responseMessage = "Purchase Requisition no." + PurchaseRequisitionObj.No + " was successfully rejected.";
						detailedResponseMessage = "Purchase Requisition no." + PurchaseRequisitionObj.No + " was successfully rejected.";
						button1ControllerName = "Approval";
						button1ActionName = "OpenEntries";
						button1Name = "Ok";
						button1Parameters = "";

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
						PurchaseRequisitionObj.ErrorStatus = true;
						PurchaseRequisitionObj.ErrorMessage = "Unable to process the Purchase Requisition reject action.  " + ServiceConnection.contactICTDepartment + "";
						return View(PurchaseRequisitionObj);
					}
				}
				else if (Command == "View Attachment")
				{
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(PurchaseRequisitionObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                    //string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(PurchaseRequisitionObj.No);

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
                    //			PurchaseRequisitionObj.ErrorStatus = true;
                    //			PurchaseRequisitionObj.ErrorMessage = "No file attached. This is because purchase requisition's document attachment is not mandatory. ";
                    //			return View(PurchaseRequisitionObj);
                    //		}
                    //	}
                    //}


                    //else
                    //{
                    //	PurchaseRequisitionObj.ErrorStatus = true;
                    //	PurchaseRequisitionObj.ErrorMessage="There was not file attached for this documnt No - "+ PurchaseRequisitionObj.No+" because attachments for purchase requisitions are optional.";
                    //	return View(PurchaseRequisitionObj);
                    //}
                }
				else
				{
					PurchaseRequisitionObj.ErrorStatus = true;
					PurchaseRequisitionObj.ErrorMessage = "Unable to process the approve/reject action.  " + ServiceConnection.contactICTDepartment + "";
					return View(PurchaseRequisitionObj);
				}
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

        #endregion Purchase Requisition Approval

        #region Documents Management

        [ChildActionOnly]
        [Authorize]
        public ActionResult _PurchaseRequisitionDocument(string DocumentNo)
        {
            DocumentMgmtModel portalDocumentObj = new DocumentMgmtModel();
            return PartialView(portalDocumentObj);
        }

        [ChildActionOnly]
        [Authorize]
        public ActionResult _ViewPurchaseRequisitionDocument(string DocumentNo)
        {
            DocumentMgmtModel portalDocumentObj = new DocumentMgmtModel();
            return PartialView(portalDocumentObj);
        }

        [Authorize]
        public JsonResult LoadPurchaseRequisitionDocuments(string DocumentNo)
        {
            List<DocumentMgmtModel> portalDocumentsList = new List<DocumentMgmtModel>();

            dynamic portalDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(DocumentNo));

            foreach (var portalDocument in portalDocuments)
            {
                DocumentMgmtModel documentManagementObj = new DocumentMgmtModel();
                documentManagementObj.DocumentNo = portalDocument.DocumentNo;
                documentManagementObj.DocumentCode = portalDocument.DocumentCode;
                documentManagementObj.DocumentAttached = portalDocument.DocumentAttached;
                documentManagementObj.DocumentDescription = portalDocument.DocumentDescription;
                documentManagementObj.FileName = portalDocument.FileName;

                portalDocumentsList.Add(documentManagementObj);
            }

            return Json(portalDocumentsList, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult DocumentAttachments(string DocNo, int TableID, string Status)
        {
            #region Document Attachment
            List<DocumentMgmtModel> DocAttachment = new List<DocumentMgmtModel>();
            string page = "DocAttachments?$filter=Table_ID eq " + TableID + " and No eq '" + DocNo + "'&format=json";

            HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);

                foreach (JObject config in details["value"])
                {
                    DocumentMgmtModel docAttList = new DocumentMgmtModel();
                    docAttList.TabelID = (int)config["Table_ID"];
                    docAttList.No = (string)config["No"];
                    docAttList.FileName = (string)config["File_Name"];
                    docAttList.FileExt = (string)config["File_Extension"];
                    docAttList.ID = (int)config["ID"];
                    docAttList.LineNo = (string)config["Line_No"];
                    docAttList.DocType = (string)config["Document_Type"];
                    DocAttachment.Add(docAttList);
                }
            }
            #endregion
            DocumentAttachmentList DocumentList = new DocumentAttachmentList
            {
                Status = Status,
                DocList = DocAttachment
            };
            return PartialView("~/Views/Shared/ImportantDocs.cshtml", DocumentList);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DocumentAttachmentview(int tblID, string No, int ID, string fileName, string ext)
        {
            try
            {
                bool success = false, view = false;
                string msg = "";
                string Attachment = Credentials.GetDocumentAttachmet(tblID, No, ID);

                string fName = fileName + "." + ext;
                Byte[] bytes = Convert.FromBase64String(Attachment);
                string path = Server.MapPath("~/StaffData/" + fName);
                Credentials.DownloadAttachment(path, bytes);
                msg = fName;
                view = false;
                success = true;

                return Json(new { message = msg, success, view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false, view = false }, JsonRequestBehavior.AllowGet); ;
            }
        }
        [HttpGet]
        public virtual ActionResult AttachmentDownload(string fileName)
        {
            string fullPath = Server.MapPath("~/StaffData/" + fileName);
            return File(fullPath, "application/octet-stream", fileName);
        }

        [Authorize]
        public ActionResult GetPurchaseRequisitionDocument(string DocumentNo, string DocumentCode)
        {
            try
            {
                DocumentMgmtModel portalDocumentObj = new DocumentMgmtModel();

				dynamic portalDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetLeaveApplicationDocument(DocumentNo, DocumentCode));

				foreach (var portalDocument in portalDocuments)
				{
					portalDocumentObj.DocumentNo = portalDocument.DocumentNo;
					portalDocumentObj.DocumentCode = portalDocument.DocumentCode;
					portalDocumentObj.DocumentAttached = portalDocument.DocumentAttached;
					portalDocumentObj.DocumentDescription = portalDocument.DocumentDescription;
					portalDocumentObj.FileName = portalDocument.FileName;
				}
				return Json(portalDocumentObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
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
                filename = "PurchaseRequisition_" + employeeNo + "_" + DocNo + ".pdf";
                filenane = Credentials.ObjNav.GeneratePurchaseRequestReport(filename, DocNo);
                return Json(new { message = "https://ess.cihebkenya.org/reports/" + filename, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteAttachedDocument(string DocNo, string DocID, string TableID)
        {
            try
            {
                bool successVal = false;
                string msg = "";

                Credentials.ObjNav.DeleteDocumentAttachment(Convert.ToInt32(TableID), DocNo, Convert.ToInt32(DocID));
                msg = "Attachment File Deleted Successfully";
                successVal = true;
                return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult UploadPurchaseRequsitionAttachments(string DocumentNo, string DocumentCode, string DocumentDescription)
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
                    string fileName = DocumentNo + "_" + DocumentDescription + fileExt;
                    string path = Path.Combine(HttpContext.Server.MapPath(root), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    if (fileExt == ".pdf" || fileExt ==".eml" || fileExt == ".xlsx" || fileExt == ".csv" || fileExt == ".rtf" 
                        || fileExt == ".doc" || fileExt ==".docx" || fileExt == ".jpg" || fileExt == ".jpeg" || fileExt == ".png"
                        || fileExt == ".msg"|| fileExt == ".msg")
                    {
                        file.SaveAs(path);

                        if (System.IO.File.Exists(path))
                        {
                            bool ret = false;
                            ret = dynamicsNAVSOAPServices.documentMgmt.InsertImprestAttachment(DocumentNo, path, 51525398, "Purchase Requisition");
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

                    //if (!fileExt.Equals(".pdf"))
                    //{
                    //	return Json(new { success = false, message = "Only pdf formats are allowed to be uploaded. Please convert your " + fileExt + " file to pdf for uploading" }, JsonRequestBehavior.AllowGet);
                    //}

                   

                    // dynamicsNAVSOAPServices.documentMgmt.ModifySystemFileURL(DocumentNo, DocumentCode, fileName);

                    //string username = ServiceConnection.sharePointUser;
                    //string Password = ServiceConnection.sharePointUserPassword;

                    //var securePassword = new SecureString();
                    //foreach (char c in Password)
                    //{
                    //	securePassword.AppendChar(c);
                    //}

                    //using (var ctx = new ClientContext(ServiceConnection.sharePointSiteUrl))
                    //{
                    //	ctx.Credentials = new Microsoft.SharePoint.Client.SharePointOnlineCredentials(username, securePassword);
                    //	Web web = ctx.Web;
                    //	ctx.Load(web);

                    //	//Ssl3: Secure Socket Layer (SSL) 3.0 security protocol.
                    //	//Tls: Transport Layer Security (TLS) 1.0 security protocol
                    //	ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                    //				   SecurityProtocolType.Tls11 |
                    //				   SecurityProtocolType.Tls12;

                    //	ctx.ExecuteQuery();

                    //	FileCreationInformation newFile = new FileCreationInformation();
                    //	//newFile.Content = System.IO.File.ReadAllBytes(path);
                    //	newFile.ContentStream = new MemoryStream(System.IO.File.ReadAllBytes(path));
                    //	newFile.Url = Path.GetFileName(path);
                    //	newFile.Overwrite = true;

                    //	List byTitle = ctx.Web.Lists.GetByTitle(ServiceConnection.SupplyChainFolderTitle);
                    //	Folder folder = byTitle.RootFolder.Folders.GetByUrl(ServiceConnection.RequisitionsFolder);
                    //	ctx.Load(folder);
                    //	ctx.ExecuteQuery();

                    //	Microsoft.SharePoint.Client.File uploadFile = folder.Files.Add(newFile);
                    //	ctx.Load(byTitle);
                    //	ctx.Load(uploadFile);
                    //	ctx.ExecuteQuery();

                    //	string SharePointUrl = ServiceConnection.sharePointSiteUrl + "/" + ServiceConnection.FinanceFolderTitle + "/" + ServiceConnection.ImprestFolder + "/" + Path.GetFileName(path);

                    //	dynamicsNAVSOAPServices.documentMgmt.UploadFileToSharePointAndNAV(DocumentNo, DocumentCode, fileName, SharePointUrl);
                    //}


                    //return Json(new { success = true, message = DocumentDescription + " uploaded successfully" }, JsonRequestBehavior.AllowGet);
                    //              }
                    //              else
                    //              {
                    //                  return Json(new { success = false, message = DocumentDescription + " was not uploaded. Try Again." }, JsonRequestBehavior.AllowGet);
                    //              }
                }
                return Json(new { success = false, message = "Please select a pdf file!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Documents Management

        #region Helper Functions
        public JsonResult GetPurchaseRequisitionAmount(string DocumentNo)
		{
			decimal purchaseRequisitionAmount = 0;
			purchaseRequisitionAmount = dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitionAmount(DocumentNo);
			return Json(new { Amount = purchaseRequisitionAmount }, JsonRequestBehavior.AllowGet);
		}
		public string GetPurchaseRequisitionStatus(string DocumentNo)
		{
			return dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitionStatus(DocumentNo);
		}
		private void LoadCurrencies()
		{
			currencyCodes = from currenciesQuery in dynamicsNAVODataServices.dynamicsNAVOData.Currencies
							select currenciesQuery;
		}
		private void LoadPurchaseRequisitionTypes()
		{
			List<SelectListItem> requisitionTypes = new List<SelectListItem>
													{
														new SelectListItem { Text = "Service", Value = "Service" },
														new SelectListItem { Text = "Item", Value = "Item" },
														new SelectListItem { Text = "Fixed Asset", Value = "Fixed Asset" }
													};

			purchaseRequisitionTypes = requisitionTypes;
		}
        private void loadprocurementplan()
        {
            globalDimension1Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                                    where dimensionValuesQuery.Global_Dimension_No.Equals(1) && dimensionValuesQuery.Blocked.Equals(false)
                                    select dimensionValuesQuery;
        }
        private void LoadDimensions(string GlobalDimension1Code)
		{
			globalDimension1Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
									where dimensionValuesQuery.Global_Dimension_No.Equals(1) && dimensionValuesQuery.Blocked.Equals(false)
									select dimensionValuesQuery;
			globalDimension2Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
									where dimensionValuesQuery.Global_Dimension_No.Equals(2) && dimensionValuesQuery.Blocked.Equals(false)
									select dimensionValuesQuery;
			shortcutDimension3Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
									  where dimensionValuesQuery.Global_Dimension_No.Equals(3) && dimensionValuesQuery.Blocked.Equals(false)
									  select dimensionValuesQuery;
			shortcutDimension4Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
									  where dimensionValuesQuery.Global_Dimension_No.Equals(4) && dimensionValuesQuery.Blocked.Equals(false)
									  select dimensionValuesQuery;
			shortcutDimension5Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
									  where dimensionValuesQuery.Global_Dimension_No.Equals(5) && dimensionValuesQuery.Blocked.Equals(false)
									  select dimensionValuesQuery;
			shortcutDimension6Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
									  where dimensionValuesQuery.Global_Dimension_No.Equals(6) && dimensionValuesQuery.Blocked.Equals(false)
									  select dimensionValuesQuery;
			shortcutDimension7Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
									  where dimensionValuesQuery.Global_Dimension_No.Equals(7) && dimensionValuesQuery.Blocked.Equals(false)
									  select dimensionValuesQuery;
			shortcutDimension8Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
									  where dimensionValuesQuery.Global_Dimension_No.Equals(8) && dimensionValuesQuery.Blocked.Equals(false)
									  select dimensionValuesQuery;
		}
		public JsonResult GetAvailableInventory(string ItemNo)
		{
			decimal availableInventory = 0;
            var getstore = dynamicsNAVSOAPServices.procurementManagementWS.GetStoreRequisitionLocation(ItemNo);
            availableInventory = dynamicsNAVSOAPServices.inventoryManagementWS.GetAvailableInventory(ItemNo,getstore);
			return Json(new { AvailableInventory = availableInventory }, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetPurchaseRequisitionUOMs(string RequisitionType)
		{
		    if (!RequisitionType.Equals(""))
			{
				List<PurchaseRequisitionUOMs> PurchaseRequisitionUOMList = new List<PurchaseRequisitionUOMs>();

				dynamic purchaseRequisitionUOMs = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitionUOMs(RequisitionType));

				foreach (var purchaseRequisitionUOM in purchaseRequisitionUOMs)
				{
					PurchaseRequisitionUOMs PurchaseRequisitionUOMObj = new PurchaseRequisitionUOMs();
					PurchaseRequisitionUOMObj.Code = purchaseRequisitionUOM.Code;
					PurchaseRequisitionUOMObj.Description = purchaseRequisitionUOM.Description;

					PurchaseRequisitionUOMList.Add(PurchaseRequisitionUOMObj);
				}

				return Json(PurchaseRequisitionUOMList.ToList(), JsonRequestBehavior.AllowGet);
			}
			else
			{
				return Json(new List<string>(), JsonRequestBehavior.AllowGet);
			}
		}
        public JsonResult GetProcurementPlan()
        {
            List<ItemsModel> procplanlist = JsonConvert.DeserializeObject<List<ItemsModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetProcurementPlan());

            return Json(procplanlist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProcurementPlanItems()
		{
			List<ItemsModel> itemsList = JsonConvert.DeserializeObject<List<ItemsModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetProcurementPlanItems());
			
			return Json(itemsList, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetProcurementPlanFixedAssets()
		{
			List<FixedAssetModel> fixedAssetList = JsonConvert.DeserializeObject<List<FixedAssetModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetProcurementPlanFixedAssets());

			return Json(fixedAssetList, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetProcurementPlanServices()
		{
			List<ServicesModel> servicesList = JsonConvert.DeserializeObject<List<ServicesModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetProcurementPlanServices());

			return Json(servicesList, JsonRequestBehavior.AllowGet); 
		}
		#endregion Helper Functions
	}
}