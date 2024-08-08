using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using DynamicsNAV365_StaffPortal.Models.Finance.ImprestSurrender;
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
using DynamicsNAV365_StaffPortal.Models;
using DynamicsNAV365_StaffPortal.Models.Finance.StaffAdvance;

namespace DynamicsNAV365_StaffPortal.Controllers.FinanceServices
{
	[NoCache]
    public class ImprestSurrenderController : Controller
    {
		string companyName = ServiceConnection.CompanyName;
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

		List<UnsurrenderedImprestCode> unsurrenderedImprestCodes = null;
		List<FundsTransactionModel> imprestSurrenderCodes = null;
		List<ReceiptList> receiptLineList = null;

		AccountController accountController = new AccountController();
		string employeeNo = "";

		IQueryable<DimensionValues> globalDimension1Codes = null;
		IQueryable<DimensionValues> globalDimension2Codes = null;
		IQueryable<DimensionValues> shortcutDimension3Codes = null;
		IQueryable<DimensionValues> shortcutDimension4Codes = null;
		IQueryable<DimensionValues> shortcutDimension5Codes = null;
		IQueryable<DimensionValues> shortcutDimension6Codes = null;
		IQueryable<DimensionValues> shortcutDimension7Codes = null;
		IQueryable<DimensionValues> shortcutDimension8Codes = null;

		public ImprestSurrenderController()
		{
			employeeNo = AccountController.GetEmployeeNo();
		}

		#region New Imprest Surrender
	
		[Authorize]
		public ActionResult NewImprestSurrender()
		{
			string imprestSurrenderNo = "";
			ImprestSurrenderHeaderModel imprestSurrenderObj = new ImprestSurrenderHeaderModel();

			LoadUnsurrenderedImprestCodes();
			imprestSurrenderObj.UnsurrenderedImprests = new SelectList(unsurrenderedImprestCodes, "No", "Description");

			try
			{
				//Check open imprest surrender
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckOpenImprestSurrenderExists(employeeNo))
				{
					responseHeader = "Open Imprest Surrender";
					responseMessage = "An open imprest surrender exists for employee no. " + employeeNo + ", finalize on this imprest surrender before creating a new one.";
					detailedResponseMessage = "An open imprest surrender exists for employee no. " + employeeNo + ", finalize on this imprest surrender before creating a new one.";

					button1ControllerName = "ImprestSurrender";
					button1ActionName = "ImprestSurrenderHistory";
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
				//End check open imprest surrender

				//Create a new imprest surrender
				imprestSurrenderNo = dynamicsNAVSOAPServices.fundsManagementWS.CreateImprestSurrenderHeader(employeeNo);
				return RedirectToAction("EditImprestSurrender", new {ImprestSurrenderNo = imprestSurrenderNo});
				//End create imprest surrender				
				/*imprestSurrenderObj.No = imprestSurrenderNo;
				imprestSurrenderObj.EmployeeNo = employeeNo;
				var DimensionValues = JsonConvert.DeserializeObject<List<ImprestDimensionValue>>(dynamicsNAVSOAPServices.fundsManagementWS.GetDimensionValue());
				imprestSurrenderObj.GlobalDimension2CodeSelect =  DimensionValues?.Select(c=> new  SelectListItem
				{
					Value = c.Code,
					Text = $"{c.Code} - {c.Name}",
					Selected = c.Code.Equals(imprestSurrenderObj.GlobalDimension2Code,StringComparison.OrdinalIgnoreCase)
				}).ToList();
				imprestSurrenderObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

				//Hard Coding
				imprestSurrenderObj.ImprestDate = "01-01-0001";


				return View(imprestSurrenderObj);*/
			}
			catch (Exception ex)
			{
				//return errorResponse.ApplicationExceptionError(ex);
				imprestSurrenderObj.ErrorStatus = true;
				imprestSurrenderObj.ErrorMessage = ex.Message.ToString();
				return View(imprestSurrenderObj);
			}
		}

		[Authorize]
		[HttpPost]
		public ActionResult NewImprestSurrender(ImprestSurrenderHeaderModel ImprestSurrenderObj)
		{
			bool imprestSurrenderModified = false;
			bool approvalWorkflowExist = false;

			LoadUnsurrenderedImprestCodes();
			ImprestSurrenderObj.UnsurrenderedImprests = new SelectList(unsurrenderedImprestCodes, "No", "Description");

			if (ModelState.IsValid)
			{
				try
				{

					if (dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestSurrenderExists(ImprestSurrenderObj.No, AccountController.GetEmployeeNo()))
					{
						//get imprest surrender No
						ImprestSurrenderObj.No = dynamicsNAVSOAPServices.fundsManagementWS.GetOpenImprestSurrender(employeeNo);
						//end imprest surrender No

						//Modify imprest surrender
						//imprestSurrenderModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyImprestSurrenderHeader(ImprestSurrenderObj.No, ImprestSurrenderObj.EmployeeNo, ImprestSurrenderObj.UnsurrenderedImprest, ImprestSurrenderObj.Description, ImprestSurrenderObj.GlobalDimension2Code);

						if (!imprestSurrenderModified)
						{
							ImprestSurrenderObj.ErrorStatus = true;
							ImprestSurrenderObj.ErrorMessage = "An error was experienced while trying to modify imprest surrender no." + ImprestSurrenderObj.No + ", the server might be offline, try again after a while.";
							return View(ImprestSurrenderObj);
						}

						//Send imprest surrender for approval
						approvalWorkflowExist = dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestSurrenderApprovalWorkflowEnabled(ImprestSurrenderObj.No);
						if (!approvalWorkflowExist)
						{
							ImprestSurrenderObj.ErrorStatus = true;
							ImprestSurrenderObj.ErrorMessage = "An error was experienced while trying to send an approval request for imprest surrender no." + ImprestSurrenderObj.No + ", the approval workflow was not found. Contact the " + companyName + " ICT department for assistance.";
							return View(ImprestSurrenderObj);
						}

						if (dynamicsNAVSOAPServices.fundsManagementWS.SendImprestSurrenderApprovalRequest(ImprestSurrenderObj.No))
						{
							responseHeader = "Success";
							responseMessage = "Imprest surrender no." + ImprestSurrenderObj.No + " was successfully sent for approval. Check with the " + companyName + " finance department for approval status.";
							detailedResponseMessage = "Imprest surrender no." + ImprestSurrenderObj.No + " was successfully sent for approval. Check with the " + companyName + " finance department for approval status.";

							button1ControllerName = "ImprestSurrender";
							button1ActionName = "ImprestSurrenderHistory";
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
							ImprestSurrenderObj.ErrorStatus = true;
							ImprestSurrenderObj.ErrorMessage = "An error was experienced while trying to send an approval request for imprest surrender no." + ImprestSurrenderObj.No + ". Contact the " + companyName + " ICT department for assistance.";
							return View(ImprestSurrenderObj);
						}
					}
					else
					{
						responseHeader = "Imprest Surrender NotFound";
						responseMessage = "The imprest surrender no." + ImprestSurrenderObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
						detailedResponseMessage = "The imprest surrender no." + ImprestSurrenderObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

						button1ControllerName = "ImprestSurrender";
						button1ActionName = "ImprestSurrenderHistory";
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
					ImprestSurrenderObj.ErrorStatus = true;
					ImprestSurrenderObj.ErrorMessage = ex.Message.ToString();
					return View(ImprestSurrenderObj);
				}
			}
			else
			{
				return View(ImprestSurrenderObj);
			}
		}
	
		#endregion New Imprest Request

		#region Edit Imprest Surrender
	
		[Authorize]
		public ActionResult OnBeforeEdit(string ImprestSurrenderNo)
		{
			ImprestSurrenderHeaderModel imprestSurrenderObj = new ImprestSurrenderHeaderModel();

			try
			{
				imprestSurrenderObj.EmployeeNo = employeeNo;
				imprestSurrenderObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

				if (ImprestSurrenderNo.Equals(""))
				{
					return RedirectToAction("ImprestSurrenderHistory", "ImprestSurrender");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestSurrenderExists(ImprestSurrenderNo, AccountController.GetEmployeeNo()))
				{
					string imprestSurrenderStatus = GetImprestSurrenderStatus(ImprestSurrenderNo);
					//if imprest surrender is open
					if (imprestSurrenderStatus.Equals("Open"))
					{
						return RedirectToAction("EditImprestSurrender", "ImprestSurrender", new { ImprestSurrenderNo = ImprestSurrenderNo });
					}

					//if imprest surrender is pending approval
					if (imprestSurrenderStatus.Equals("Pending Approval"))
					{
						responseHeader = "Imprest Surrender Pending Approval";
						responseMessage = "The imprest surrender no." + ImprestSurrenderNo + " is already submitted for approval. Editing not allowed.";
						detailedResponseMessage = "The imprest surrender no." + ImprestSurrenderNo + " is already submitted for approval. Editing not allowed.";

						button1ControllerName = "ImprestSurrender";
						button1ActionName = "ImprestSurrenderHistory";
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

					//if imprest surrender is released
					if (imprestSurrenderStatus.Equals("Released"))
					{
						responseHeader = "Imprest Surrender Approved";
						responseMessage = "The imprest surrender no." + ImprestSurrenderNo + " is already approved. Editing not allowed.";
						detailedResponseMessage = "The imprest surrender no." + ImprestSurrenderNo + " is already approved. Editing not allowed.";

						button1ControllerName = "ImprestSurrender";
						button1ActionName = "ImprestSurrenderHistory";
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
					//if imprest surrender is rejected
					if (imprestSurrenderStatus.Equals("Rejected"))
					{
						responseHeader = "Imprest Surrender Rejected";
						responseMessage = "The imprest surrender no." + ImprestSurrenderNo + " was rejected. Editing will reopen the document. Do you want to continue?";
						detailedResponseMessage = "The imprest surrender no." + ImprestSurrenderNo + " was rejected. Editing will reopen the document. Do you want to continue?";

						button1ControllerName = "ImprestSurrender";
						button1ActionName = "ImprestSurrenderHistory";
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

					//if imprest surrender is posted/reversed
					if (imprestSurrenderStatus.Equals("Posted") || imprestSurrenderStatus.Equals("Reversed"))
					{
						responseHeader = "Imprest Surrender Posted";
						responseMessage = "The imprest surrender no." + ImprestSurrenderNo + " is already posted. Editing not allowed.";
						detailedResponseMessage = "The imprest surrender no." + ImprestSurrenderNo + " is already posted. Editing not allowed.";

						button1ControllerName = "ImprestSurrender";
						button1ActionName = "ImprestSurrenderHistory";
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
					return RedirectToAction("ImprestSurrenderHistory", "ImprestSurrender");
				}
				else
				{
					responseHeader = "Imprest Surrender NotFound";
					responseMessage = "The imprest surrender no." + ImprestSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The imprest surrender no." + ImprestSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "ImprestSurrender";
					button1ActionName = "ImprestSurrenderHistory";
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
		public ActionResult EditImprestSurrender(string ImprestSurrenderNo)
		{
			try
			{
				if (ImprestSurrenderNo.Equals(""))
				{
					return RedirectToAction("ImprestSurrenderHistory", "ImprestSurrender");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestSurrenderExists(ImprestSurrenderNo, AccountController.GetEmployeeNo()))
				{
					string imprestSurrenderStatus = dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenderStatus(ImprestSurrenderNo);

					//if imprest surrender is pending approval, cancel approval request
					if (imprestSurrenderStatus.Equals("Pending Approval"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.CancelImprestSurrenderApprovalRequest(ImprestSurrenderNo);
					}
					//if imprest surrender is released, reopen and uncommit from budget

					if (imprestSurrenderStatus.Equals("Released"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.CancelImprestSurrenderBudgetCommitment(ImprestSurrenderNo);
					}
					//if imprest Surrender is rejected, reopen document
					if (imprestSurrenderStatus.Equals("Rejected"))
					{
						dynamicsNAVSOAPServices.fundsManagementWS.ReopenImprestSurrender(ImprestSurrenderNo);
					}

					ImprestSurrenderHeaderModel imprestSurrenderObj = new ImprestSurrenderHeaderModel();
					//var imprestSurrenders = from imprestSurrendersQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestSurrenders
					//						where imprestSurrendersQuery.No.Equals(ImprestSurrenderNo)
					//						select imprestSurrendersQuery;
					dynamic imprestSurrenders = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenders(ImprestSurrenderNo, ""));
					foreach (var imprestSurrender in imprestSurrenders)
					{
						imprestSurrenderObj.No = imprestSurrender.No;
						imprestSurrenderObj.DocumentDate = imprestSurrender.DocumentDate;
						imprestSurrenderObj.PostingDate = imprestSurrender.PostingDate;
						imprestSurrenderObj.EmployeeNo = imprestSurrender.EmployeeNo;
						imprestSurrenderObj.EmployeeName = imprestSurrender.EmployeeName;
						imprestSurrenderObj.CurrencyCode = imprestSurrender.ImprestType;
						imprestSurrenderObj.Description = imprestSurrender.Description;
                        imprestSurrenderObj.GlobalDimension1Code = imprestSurrender.GlobalDimension1Code;
                        imprestSurrenderObj.GlobalDimension2Code = imprestSurrender.GlobalDimension2Code;
                        imprestSurrenderObj.ShortcutDimension3Code = imprestSurrender.ShortcutDimension3Code;
                        imprestSurrenderObj.ShortcutDimension4Code = imprestSurrender.ShortcutDimension4Code;
                        imprestSurrenderObj.ShortcutDimension5Code = imprestSurrender.ShortcutDimension5Code;
                        imprestSurrenderObj.ShortcutDimension6Code = imprestSurrender.ShortcutDimension6Code;
                        imprestSurrenderObj.ShortcutDimension7Code = imprestSurrender.ShortcutDimension7Code;
                        imprestSurrenderObj.ShortcutDimension8Code = imprestSurrender.ShortcutDimension8Code;
                        imprestSurrenderObj.Amount = imprestSurrender.Amount;
						imprestSurrenderObj.ActualSpent = imprestSurrender.ActualSpent;
						imprestSurrenderObj.Status = imprestSurrender.Status;
						/*var DimensionValues = JsonConvert.DeserializeObject<List<ImprestDimensionValue>>(dynamicsNAVSOAPServices.fundsManagementWS.GetDimensionValue());
						imprestSurrenderObj.GlobalDimension2CodeSelect =  DimensionValues?.Select(c=> new  SelectListItem
						{
							Value = c.Code,
							Text = $"{c.Code} - {c.Name}",
							Selected = c.Code.Equals(imprestSurrenderObj.GlobalDimension2Code,StringComparison.OrdinalIgnoreCase)
						}).ToList();*/
					}

					LoadUnsurrenderedImprestCodes();
					imprestSurrenderObj.UnsurrenderedImprests = new SelectList(unsurrenderedImprestCodes, "No", "Description");

					imprestSurrenderObj.EmployeeNo = employeeNo;
					imprestSurrenderObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

					return View(imprestSurrenderObj);
				}
				else
				{
					responseHeader = "Imprest Surrender NotFound";
					responseMessage = "The imprest surrender no." + ImprestSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The imprest surrender no." + ImprestSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "ImprestSurrender";
					button1ActionName = "ImprestSurrenderHistory";
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
		public async Task<ActionResult> EditImprestSurrender(ImprestSurrenderHeaderModel ImprestSurrenderObj, string Command)
		{
			bool imprestSurrenderModified = false;
			bool approvalWorkflowExist = false;
			//bool imprestDocumentsAttached = false;

			if (Command.Equals("Submit For Approval"))
            {
				if (ModelState.IsValid)
				{
					LoadUnsurrenderedImprestCodes();
					ImprestSurrenderObj.UnsurrenderedImprests = new SelectList(unsurrenderedImprestCodes, "No", "Description");

					try
					{

						if (dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestSurrenderExists(ImprestSurrenderObj.No, AccountController.GetEmployeeNo()))
						{

                            //imprestSurrenderModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyImprestSurrenderHeader(ImprestSurrenderObj.No, ImprestSurrenderObj.EmployeeNo, ImprestSurrenderObj.UnsurrenderedImprest, ImprestSurrenderObj.Description, ImprestSurrenderObj.GlobalDimension2Code);

							if (!imprestSurrenderModified)
							{
								ImprestSurrenderObj.ErrorStatus = true;
								ImprestSurrenderObj.ErrorMessage = "An error was experienced while trying to modify imprest surrender no." + ImprestSurrenderObj.No + ", the server might be offline, try again after a while.";
								return View(ImprestSurrenderObj);
							}

							//Send imprest surrender for approval
							approvalWorkflowExist = dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestSurrenderApprovalWorkflowEnabled(ImprestSurrenderObj.No);
							if (!approvalWorkflowExist)
							{
								ImprestSurrenderObj.ErrorStatus = true;
								ImprestSurrenderObj.ErrorMessage = "An error was experienced while trying to send an approval request for imprest surrender no." + ImprestSurrenderObj.No + ", the approval workflow was not found. Contact the " + companyName + " ICT department for assistance.";
								return View(ImprestSurrenderObj);
							}

							if (dynamicsNAVSOAPServices.fundsManagementWS.SendImprestSurrenderApprovalRequest(ImprestSurrenderObj.No))
							{
								responseHeader = "Success";
								responseMessage = "Imprest surrender no." + ImprestSurrenderObj.No + " was successfully sent for approval. Check with the " + companyName + " finance department for approval status.";
								detailedResponseMessage = "Imprest surrender no." + ImprestSurrenderObj.No + " was successfully sent for approval. Check with the " + companyName + " finance department for approval status.";

								button1ControllerName = "ImprestSurrender";
								button1ActionName = "ImprestSurrenderHistory";
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
								ImprestSurrenderObj.ErrorStatus = true;
								ImprestSurrenderObj.ErrorMessage = "An error was experienced while trying to send an approval request for imprest no." + ImprestSurrenderObj.No + ". Contact the " + companyName + " ICT department for assistance.";
								return View(ImprestSurrenderObj);
							}
						}
						else
						{
							responseHeader = "Imprest Surrender NotFound";
							responseMessage = "The imprest surrender no." + ImprestSurrenderObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
							detailedResponseMessage = "The imprest surrender no." + ImprestSurrenderObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();

							button1ControllerName = "ImprestSurrender";
							button1ActionName = "ImprestSurrenderHistory";
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
						ImprestSurrenderObj.ErrorStatus = true;
						ImprestSurrenderObj.ErrorMessage = ex.Message.ToString();
						return View(ImprestSurrenderObj);
					}
				}
			}
			if (Command.Equals("View Attachment"))
			{
				string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(ImprestSurrenderObj.No);

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
					return View(ImprestSurrenderObj);
				}
			}

			else
			{
				return View(ImprestSurrenderObj);
			}
		}

		#endregion Edit Imprest Surrender

		#region View Imprest Request
	
		[Authorize]
		public ActionResult ViewImprestSurrender(string ImprestSurrenderNo)
		{
			try
			{
				if (ImprestSurrenderNo.Equals(""))
				{
					return RedirectToAction("ImprestSurrenderHistory", "ImprestSurrender");
				}
				if (dynamicsNAVSOAPServices.fundsManagementWS.CheckImprestSurrenderExists(ImprestSurrenderNo, AccountController.GetEmployeeNo()))
				{
					ImprestSurrenderHeaderModel imprestSurrenderObj = new ImprestSurrenderHeaderModel();
					//var imprestSurrenders = from imprestSurrendersQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestSurrenders
					//						where imprestSurrendersQuery.No.Equals(ImprestSurrenderNo)
					//						select imprestSurrendersQuery;
					//var DimensionValues = JsonConvert.DeserializeObject<List<ImprestDimensionValue>>(dynamicsNAVSOAPServices.fundsManagementWS.GetDimensionValue());

					dynamic imprestSurrenders = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenders(ImprestSurrenderNo,""));
					foreach (var imprestSurrender in imprestSurrenders)
					{
						imprestSurrenderObj.No = imprestSurrender.No;
						imprestSurrenderObj.DocumentDate = imprestSurrender.DocumentDate;
						imprestSurrenderObj.PostingDate = imprestSurrender.PostingDate;
						imprestSurrenderObj.EmployeeNo = imprestSurrender.EmployeeNo;
						imprestSurrenderObj.EmployeeName = imprestSurrender.EmployeeName;
						imprestSurrenderObj.CurrencyCode = imprestSurrender.ImprestType;
						imprestSurrenderObj.Description = imprestSurrender.Description;
                        imprestSurrenderObj.GlobalDimension1Code = imprestSurrender.GlobalDimension1Code;
                        imprestSurrenderObj.GlobalDimension2Code = imprestSurrender.GlobalDimension2Code;
                        imprestSurrenderObj.ShortcutDimension3Code = imprestSurrender.ShortcutDimension3Code;
                        imprestSurrenderObj.ShortcutDimension4Code = imprestSurrender.ShortcutDimension4Code;
                        imprestSurrenderObj.ShortcutDimension5Code = imprestSurrender.ShortcutDimension5Code;
                        imprestSurrenderObj.ShortcutDimension6Code = imprestSurrender.ShortcutDimension6Code;
                        imprestSurrenderObj.ShortcutDimension7Code = imprestSurrender.ShortcutDimension7Code;
                        imprestSurrenderObj.ShortcutDimension8Code = imprestSurrender.ShortcutDimension8Code;
                        imprestSurrenderObj.Amount = imprestSurrender.Amount;
						imprestSurrenderObj.ActualSpent = imprestSurrender.ActualSpent;
						imprestSurrenderObj.Status = imprestSurrender.Status;
						//imprestSurrenderObj.GlobalDimension2CodeSelect =  DimensionValues?.Select(c=> new  SelectListItem
						//{
						//	Value = c.Code,
						//	Text = $"{c.Code} - {c.Name}",
						//	Selected = c.Code.Equals(imprestSurrenderObj.GlobalDimension2Code,StringComparison.OrdinalIgnoreCase)
						//}).ToList();
					}

					//var globalDimension1Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
					//							where dimensionValuesQuery.Global_Dimension_No.Equals(1)
					//							select dimensionValuesQuery;
					//var globalDimension2Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
					//							where dimensionValuesQuery.Global_Dimension_No.Equals(2) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
					//							select dimensionValuesQuery;
					//var shortcutDimension3Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
					//							  where dimensionValuesQuery.Global_Dimension_No.Equals(3) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
					//							  select dimensionValuesQuery;
					//var shortcutDimension4Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
					//							  where dimensionValuesQuery.Global_Dimension_No.Equals(4) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
					//							  select dimensionValuesQuery;
					//var shortcutDimension5Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
					//							  where dimensionValuesQuery.Global_Dimension_No.Equals(5) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
					//							  select dimensionValuesQuery;
					//var shortcutDimension6Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
					//							  where dimensionValuesQuery.Global_Dimension_No.Equals(6) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
					//							  select dimensionValuesQuery;
					//var shortcutDimension7Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
					//							  where dimensionValuesQuery.Global_Dimension_No.Equals(7) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
					//							  select dimensionValuesQuery;
					//var shortcutDimension8Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
					//							  where dimensionValuesQuery.Global_Dimension_No.Equals(8) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
					//							  select dimensionValuesQuery;

					//imprestSurrenderObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
					//imprestSurrenderObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
					//imprestSurrenderObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
					//imprestSurrenderObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
					//imprestSurrenderObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
					//imprestSurrenderObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
					//imprestSurrenderObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
					//imprestSurrenderObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

					LoadUnsurrenderedImprestCodes();
					imprestSurrenderObj.UnsurrenderedImprests = new SelectList(unsurrenderedImprestCodes, "No", "Description");

					return View(imprestSurrenderObj);
				}
				else
				{
					responseHeader = "Imprest Surrender NotFound";
					responseMessage = "The imprest surrender no." + ImprestSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The imprest surrender no." + ImprestSurrenderNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "ImprestSurrender";
					button1ActionName = "ImprestSurrenderHistory";
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
		public async Task<ActionResult> ViewImprestSurrender(ImprestSurrenderHeaderModel pettyCashRequestObj, string Command)
		{
			try
			{
				if (pettyCashRequestObj.No.Equals(""))
				{
					return RedirectToAction("ImprestSurrenderHistory", "ImprestSurrender");
				}
				if (Command.Equals("View Attachment"))
				{
					string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(pettyCashRequestObj.No);

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
						return View(pettyCashRequestObj);
					}
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
		#endregion View Imprest Request

		#region Imprest Surrenders history

		[Authorize]
		public ActionResult ImprestSurrenderHistory()
		{
			try
			{
				List<ImprestSurrenderHeaderModel> imprestSurrendersList = new List<ImprestSurrenderHeaderModel>();
				FinanceHomeController financeHomeController = new FinanceHomeController();

				//var imprestSurrenders = from imprestSurrendersQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestSurrenders
				//						where imprestSurrendersQuery.HR_Employee_No.Equals(employeeNo)
				//						select imprestSurrendersQuery;

				dynamic imprestSurrenders = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenders("", employeeNo));

				foreach (var imprestSurrender in imprestSurrenders)
				{
					ImprestSurrenderHeaderModel imprestSurrenderObj = new ImprestSurrenderHeaderModel();
					imprestSurrenderObj.No = imprestSurrender.No;
					imprestSurrenderObj.DocumentDate = imprestSurrender.DocumentDate;
					imprestSurrenderObj.PostingDate = imprestSurrender.PostingDate;
					imprestSurrenderObj.EmployeeNo = imprestSurrender.EmployeeNo;
					imprestSurrenderObj.EmployeeName = imprestSurrender.EmployeeName;
					imprestSurrenderObj.CurrencyCode = imprestSurrender.ImprestType;
					imprestSurrenderObj.Description = imprestSurrender.Description;
                    imprestSurrenderObj.GlobalDimension1Code = imprestSurrender.GlobalDimension1Code;
                    imprestSurrenderObj.GlobalDimension2Code = imprestSurrender.GlobalDimension2Code;
                    imprestSurrenderObj.ShortcutDimension3Code = imprestSurrender.ShortcutDimension3Code;
                    imprestSurrenderObj.ShortcutDimension4Code = imprestSurrender.ShortcutDimension4Code;
                    imprestSurrenderObj.ShortcutDimension5Code = imprestSurrender.ShortcutDimension5Code;
                    imprestSurrenderObj.ShortcutDimension6Code = imprestSurrender.ShortcutDimension6Code;
                    imprestSurrenderObj.ShortcutDimension7Code = imprestSurrender.ShortcutDimension7Code;
                    imprestSurrenderObj.ShortcutDimension8Code = imprestSurrender.ShortcutDimension8Code;
                    imprestSurrenderObj.Amount = imprestSurrender.Amount;
					imprestSurrenderObj.ActualSpent = imprestSurrender.ActualSpent;
					imprestSurrenderObj.Status = imprestSurrender.Status;
					imprestSurrendersList.Add(imprestSurrenderObj);
				}
				return View(imprestSurrendersList);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		public JsonResult GetImprestSurrenders()
		{
			List<ImprestSurrenderHeaderModel> imprestSurrendersList = new List<ImprestSurrenderHeaderModel>();
			FinanceHomeController financeHomeController = new FinanceHomeController();

			//var imprestSurrenders = from imprestSurrendersQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestSurrenders
			//						where imprestSurrendersQuery.HR_Employee_No.Equals(employeeNo)
			//						select imprestSurrendersQuery;

			dynamic imprestSurrenders = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenders("", employeeNo));

			foreach (var imprestSurrender in imprestSurrenders)
			{
				ImprestSurrenderHeaderModel imprestSurrenderObj = new ImprestSurrenderHeaderModel();
				imprestSurrenderObj.No = imprestSurrender.No;
				imprestSurrenderObj.DocumentDate = imprestSurrender.DocumentDate;
				imprestSurrenderObj.PostingDate = imprestSurrender.PostingDate;
				imprestSurrenderObj.EmployeeNo = imprestSurrender.EmployeeNo;
				imprestSurrenderObj.EmployeeName = imprestSurrender.EmployeeName;
				imprestSurrenderObj.CurrencyCode = imprestSurrender.ImprestType;
				imprestSurrenderObj.Description = imprestSurrender.Description;
                imprestSurrenderObj.GlobalDimension1Code = imprestSurrender.GlobalDimension1Code;
                imprestSurrenderObj.GlobalDimension2Code = imprestSurrender.GlobalDimension2Code;
                imprestSurrenderObj.ShortcutDimension3Code = imprestSurrender.ShortcutDimension3Code;
                imprestSurrenderObj.ShortcutDimension4Code = imprestSurrender.ShortcutDimension4Code;
                imprestSurrenderObj.ShortcutDimension5Code = imprestSurrender.ShortcutDimension5Code;
                imprestSurrenderObj.ShortcutDimension6Code = imprestSurrender.ShortcutDimension6Code;
                imprestSurrenderObj.ShortcutDimension7Code = imprestSurrender.ShortcutDimension7Code;
                imprestSurrenderObj.ShortcutDimension8Code = imprestSurrender.ShortcutDimension8Code;
                imprestSurrenderObj.Amount = imprestSurrender.Amount;
				imprestSurrenderObj.ActualSpent = imprestSurrender.ActualSpent;
				imprestSurrenderObj.Status = imprestSurrender.Status;
				imprestSurrendersList.Add(imprestSurrenderObj);
			}
			return Json(imprestSurrendersList, JsonRequestBehavior.AllowGet);
		}

		#endregion Imprest surrender history

		#region Imprest Surrender Approval
	
		[Authorize]
		public ActionResult ImprestSurrenderApproval(string ImprestSurrenderNo)
		{
			try
			{
				if (ImprestSurrenderNo.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}

				ImprestSurrenderHeaderModel imprestSurrenderObj = new ImprestSurrenderHeaderModel();
				//var imprestSurrenders = from imprestSurrendersQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestSurrenders
				//						where imprestSurrendersQuery.No.Equals(ImprestSurrenderNo)
				//						select imprestSurrendersQuery;
				dynamic imprestSurrenders = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenders(ImprestSurrenderNo, ""));
				foreach (var imprestSurrender in imprestSurrenders)
				{
					imprestSurrenderObj.No = imprestSurrender.No;
					imprestSurrenderObj.DocumentDate = imprestSurrender.DocumentDate;
					imprestSurrenderObj.PostingDate = imprestSurrender.PostingDate;
					imprestSurrenderObj.EmployeeNo = imprestSurrender.EmployeeNo;
					imprestSurrenderObj.EmployeeName = imprestSurrender.EmployeeName;
					imprestSurrenderObj.CurrencyCode = imprestSurrender.ImprestType;
					imprestSurrenderObj.Description = imprestSurrender.Description;
					imprestSurrenderObj.GlobalDimension1Code = imprestSurrender.GlobalDimension1Code;
					imprestSurrenderObj.GlobalDimension2Code = imprestSurrender.GlobalDimension2Code;
					imprestSurrenderObj.ShortcutDimension3Code = imprestSurrender.ShortcutDimension3Code;
					imprestSurrenderObj.ShortcutDimension4Code = imprestSurrender.ShortcutDimension4Code;
					imprestSurrenderObj.ShortcutDimension5Code = imprestSurrender.ShortcutDimension5Code;
					imprestSurrenderObj.ShortcutDimension6Code = imprestSurrender.ShortcutDimension6Code;
					imprestSurrenderObj.ShortcutDimension7Code = imprestSurrender.ShortcutDimension7Code;
					imprestSurrenderObj.ShortcutDimension8Code = imprestSurrender.ShortcutDimension8Code;
					imprestSurrenderObj.Amount = imprestSurrender.Amount;
					imprestSurrenderObj.ActualSpent = imprestSurrender.ActualSpent;
					imprestSurrenderObj.Status = imprestSurrender.Status;
				}

				//var globalDimension1Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
				//							where dimensionValuesQuery.Global_Dimension_No.Equals(1)
				//							select dimensionValuesQuery;
				//var globalDimension2Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
				//							where dimensionValuesQuery.Global_Dimension_No.Equals(2) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
				//							select dimensionValuesQuery;
				//var shortcutDimension3Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
				//							  where dimensionValuesQuery.Global_Dimension_No.Equals(3) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
				//							  select dimensionValuesQuery;
				//var shortcutDimension4Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
				//							  where dimensionValuesQuery.Global_Dimension_No.Equals(4) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
				//							  select dimensionValuesQuery;
				//var shortcutDimension5Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
				//							  where dimensionValuesQuery.Global_Dimension_No.Equals(5) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
				//							  select dimensionValuesQuery;
				//var shortcutDimension6Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
				//							  where dimensionValuesQuery.Global_Dimension_No.Equals(6) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
				//							  select dimensionValuesQuery;
				//var shortcutDimension7Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
				//							  where dimensionValuesQuery.Global_Dimension_No.Equals(7) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
				//							  select dimensionValuesQuery;
				//var shortcutDimension8Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
				//							  where dimensionValuesQuery.Global_Dimension_No.Equals(8) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(imprestSurrenderObj.GlobalDimension1Code)
				//							  select dimensionValuesQuery;

				//imprestSurrenderObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Name");
				//imprestSurrenderObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Name");
				//imprestSurrenderObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Name");
				//imprestSurrenderObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Name");
				//imprestSurrenderObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Name");
				//imprestSurrenderObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Name");
				//imprestSurrenderObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Name");
				//imprestSurrenderObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Name");

				LoadUnsurrenderedImprestCodes();
				imprestSurrenderObj.UnsurrenderedImprests = new SelectList(unsurrenderedImprestCodes, "No", "Description");

				return View(imprestSurrenderObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ImprestSurrenderApproval(ImprestSurrenderHeaderModel ImprestSurrenderObj, string Command)
		{
			try
			{
				if (ImprestSurrenderObj.No.Equals(""))
				{
					return RedirectToAction("RequestsToApprove", "Approval");
				}
				if (Command == "Approve")
				{
					ImprestSurrenderObj.Comments = ImprestSurrenderObj.Comments ?? "";
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveImprestRequest(employeeNo, ImprestSurrenderObj.No, AccountController.GetEmployeeNo()))
					{
                        TempData["Success"] = "Imprest Surrender no." + ImprestSurrenderObj.No + " was successfully approved.";
                        return RedirectToAction("OpenEntries", "Approval");
                        //responseHeader = "Success";
                        //responseMessage = "Imprest Surrender no." + ImprestSurrenderObj.No + " was successfully approved.";
                        //detailedResponseMessage = "Imprest Surrender no." + ImprestSurrenderObj.No + " was successfully approved.";

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
                        TempData["Error"] = "Unable to process the imprest surrender approve action. Contact the " + companyName + " for assistance.";
                        return View(ImprestSurrenderObj);
                        //ImprestSurrenderObj.ErrorStatus = true;
                        //ImprestSurrenderObj.ErrorMessage = "Unable to process the imprest surrender approve action. Contact the " + companyName + " for assistance.";
                        //return View(ImprestSurrenderObj);
                    }
				}
				else if (Command == "Reject")
				{
					ImprestSurrenderObj.Comments = ImprestSurrenderObj.Comments ?? "";
					if (ImprestSurrenderObj.Comments.Equals(""))
					{
						dynamic imprestSurrenders = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenders(ImprestSurrenderObj.No, ""));
						foreach (var imprestSurrender in imprestSurrenders)
						{
							ImprestSurrenderObj.No = imprestSurrender.No;
							ImprestSurrenderObj.DocumentDate = imprestSurrender.DocumentDate;
							ImprestSurrenderObj.PostingDate = imprestSurrender.PostingDate;
							ImprestSurrenderObj.EmployeeNo = imprestSurrender.EmployeeNo;
							ImprestSurrenderObj.EmployeeName = imprestSurrender.EmployeeName;
							ImprestSurrenderObj.CurrencyCode = imprestSurrender.ImprestType;
							ImprestSurrenderObj.Description = imprestSurrender.Description;
							ImprestSurrenderObj.GlobalDimension1Code = imprestSurrender.GlobalDimension1Code;
							ImprestSurrenderObj.GlobalDimension2Code = imprestSurrender.GlobalDimension2Code;
							ImprestSurrenderObj.ShortcutDimension3Code = imprestSurrender.ShortcutDimension3Code;
							ImprestSurrenderObj.ShortcutDimension4Code = imprestSurrender.ShortcutDimension4Code;
							ImprestSurrenderObj.ShortcutDimension5Code = imprestSurrender.ShortcutDimension5Code;
							ImprestSurrenderObj.ShortcutDimension6Code = imprestSurrender.ShortcutDimension6Code;
							ImprestSurrenderObj.ShortcutDimension7Code = imprestSurrender.ShortcutDimension7Code;
							ImprestSurrenderObj.ShortcutDimension8Code = imprestSurrender.ShortcutDimension8Code;
							ImprestSurrenderObj.Amount = imprestSurrender.Amount;
							ImprestSurrenderObj.ActualSpent = imprestSurrender.ActualSpent;
							ImprestSurrenderObj.Status = imprestSurrender.Status;
						}
                        TempData["Error"] = "Rejection comments is required.";
                        return View(ImprestSurrenderObj);
                        //ImprestSurrenderObj.ErrorStatus = true;
                        //ImprestSurrenderObj.ErrorMessage = "Rejection comments is required.";
                        //return View(ImprestSurrenderObj);
                    }
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectImprestRequest(employeeNo, ImprestSurrenderObj.No, ImprestSurrenderObj.Comments, AccountController.GetEmployeeNo()))
					{
                        TempData["Success"] = "Imprest Surrender no." + ImprestSurrenderObj.No + " was successfully rejected.";
                        return RedirectToAction("OpenEntries", "Approval");
                        //responseHeader = "Success";
                        //responseMessage = "Imprest Surrender no." + ImprestSurrenderObj.No + " was successfully rejected.";
                        //detailedResponseMessage = "Imprest Surrender no." + ImprestSurrenderObj.No + " was successfully rejected.";

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
                        TempData["Error"] = "Unable to process the imprest surrender reject action. Contact the " + companyName + " for assistance.";
                        return View(ImprestSurrenderObj);
                        //ImprestSurrenderObj.ErrorStatus = true;
                        //ImprestSurrenderObj.ErrorMessage = "Unable to process the imprest surrender reject action. Contact the " + companyName + " for assistance.";
                        //return View(ImprestSurrenderObj);
                    }
				}
				else if (Command == "View Attachment")
				{
					string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(ImprestSurrenderObj.No);

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
						return View(ImprestSurrenderObj);
					}
				}
				else
				{
                    TempData["Error"] = "Unable to process the approve/reject action. Contact the " + companyName + " for assistance.";
                    return View(ImprestSurrenderObj);
                    //ImprestSurrenderObj.ErrorStatus = true;
                    //ImprestSurrenderObj.ErrorMessage = "Unable to process the approve/reject action. Contact the " + companyName + " for assistance.";
                    //return View(ImprestSurrenderObj);
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
		public JsonResult GetImprestSurrenderPortalDocuments(string DocumentNo)
		{
			List<DocumentMgmtModel> documentssList = new List<DocumentMgmtModel>();

			//var imprestUploadedDocuments = from imprestDocumentsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
			//							   where imprestDocumentsQuery.DocumentNo.Equals(DocumentNo)
			//							   select imprestDocumentsQuery;

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

				documentssList.Add(documentManagementoBJ);
			}
			return Json(documentssList.ToList(), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		[HttpPost]
		public JsonResult UploadImprestSurrenderPortalDocument(string DocumentNo, string DocumentCode, string DocumentDescription)
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

			//var imprestUploadedDocuments = from imprestDocumentsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
			//							   where imprestDocumentsQuery.DocumentNo.Equals(DocumentNo)
			//							   select imprestDocumentsQuery;

		//	dynamic imprestDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(DocumentNo));

		//	foreach (var imprestDocument in imprestDocuments)
		//	{
		//		documentManagementoBJ.DocumentNo = imprestDocument.DocumentNo;
		//		documentManagementoBJ.DocumentCode = imprestDocument.DocumentCode;
		//		documentManagementoBJ.DocumentDescription = imprestDocument.DocumentDescription;
		//		documentManagementoBJ.DocumentAttached = imprestDocument.DocumentAttached ?? false;
		//		documentManagementoBJ.FileName = imprestDocument.FileName;
		//}
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
			//	documentManagementoBJ.DocumentNo = imprestDocument.DocumentNo;
			//	documentManagementoBJ.DocumentCode = imprestDocument.DocumentCode;
			//	documentManagementoBJ.DocumentDescription = imprestDocument.DocumentDescription;
			//	documentManagementoBJ.DocumentAttached = imprestDocument.DocumentAttached ?? false;
			//	documentManagementoBJ.FileName = imprestDocument.FileName;
			//}
			return PartialView(documentManagementoBJ);
		}

		[Authorize]
		public ActionResult GetImprestSurrenderPortalDocumentPerLineNo(string LineNo, string DocumentNo, string DocumentCode)
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

		#endregion Document Management

		#region Imprest Surrender Line
		
		[ChildActionOnly]
		[Authorize]
		public ActionResult _ImprestSurrenderLine(string DocumentNo)
		{
			ImprestSurrenderLineModel imprestLineObj = new ImprestSurrenderLineModel();

			//string globalDimension1Code = dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenderGlobalDimension1Code(DocumentNo);
		//	LoadImprestRequestDimensions(globalDimension1Code);
			LoadImprestSurrenderCodes();
			LoadReceiptLines();
			string imprestSurrenderlines = "ImprestLines?$filter=Document_No eq '" + DocumentNo + "' &$format=json";

			HttpWebResponse httpResponseDestForeign1 = Credentials.GetOdataData(imprestSurrenderlines);
			using (var streamReader1 = new StreamReader(httpResponseDestForeign1.GetResponseStream()))
			{
				var result1 = streamReader1.ReadToEnd();

				var details1 = JObject.Parse(result1);
				var imprestLinesList = new List<ImprestSurrenderLineModel>();

				foreach (JObject config1 in details1["value"])
				{
					var imprestSurrenderLineModel = new ImprestSurrenderLineModel();
					imprestLineObj.LineNo = (string)config1["Line_No"];
					imprestLineObj.DocumentNo = (string)config1["Document_No"];
					imprestLineObj.LineAmount = (string)config1["Amount"];
					imprestLineObj.Dimension1 = (string)config1["Shortcut_Dimension_1_Code"];
					imprestLineObj.Dimension2 = (string)config1["Shortcut_Dimension_2_Code"];
					imprestLineObj.Dimension3 = (string)config1["ShortcutDimCode3"];
					imprestLineObj.Dimension4 = (string)config1["ShortcutDimCode4"];
					imprestLineObj.Dimension5 = (string)config1["ShortcutDimCode5"];
					imprestLineObj.Dimension6 = (string)config1["ShortcutDimCode6"];
					imprestLineObj.Dimension7 = (string)config1["ShortcutDimCode7"];
					imprestLineObj.LineActualAmount = (string)config1["Actual_Spent"];
					//imprestLineObj. = (string)config1["Status"];
					imprestLinesList.Add(imprestSurrenderLineModel);
				}
			}

			imprestLineObj.ReceiptNos = new SelectList(receiptLineList, "ReceiptNo", "Description");
			imprestLineObj.ImprestSurrenderCodes = new SelectList(imprestSurrenderCodes, "TransactionCode", "TransactionDescription");
			return PartialView(imprestLineObj);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewImprestSurrenderLine(string DocumentNo)
		{
			ImprestSurrenderLineModel imprestLineObj = new ImprestSurrenderLineModel();

			string globalDimension1Code = dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenderGlobalDimension1Code(DocumentNo);
			//LoadImprestRequestDimensions(globalDimension1Code);
			LoadImprestSurrenderCodes();
			LoadReceiptLines();

			imprestLineObj.ReceiptNos = new SelectList(receiptLineList, "ReceiptNo", "Description");
			imprestLineObj.ImprestSurrenderCodes = new SelectList(imprestSurrenderCodes, "TransactionCode", "TransactionDescription");
			return PartialView(imprestLineObj);
		}

		[Authorize]
		public JsonResult GetImprestSurrenderLinesAjax(string DocumentNo)
		{
            List<ImprestSurrenderLineModel> pettyCashSurrenderLinesList = new List<ImprestSurrenderLineModel>();
            string imprestlines = "PettyCashSurrenderLines?$filter=Document_No eq '" + DocumentNo + "' &$format=json";

            HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(imprestlines);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    ImprestSurrenderLineModel imprestLineObj = new ImprestSurrenderLineModel();
                    imprestLineObj.LineNo = (string)config1["Line_No"];
                    imprestLineObj.DocumentNo = (string)config1["Document_No"];
                    imprestLineObj.ImprestSurrenderCode = (string)config1["Type"];

                    imprestLineObj.AccountType = (string)config1["Account_Type"];
                    imprestLineObj.AccountNo = (string)config1["Account_No"];
                    imprestLineObj.AccountName = (string)config1["Transaction_Type"];
                    imprestLineObj.LineSurrenderDescription = (string)config1["Description"];
                    imprestLineObj.LineAmount = (string)config1["Amount"];
                    imprestLineObj.LineActualAmount = (string)config1["Actual_Spent"];
                    imprestLineObj.ReceiptNo = (string)config1["ReceiptNo"];
                    imprestLineObj.ReceiptAmount = (string)config1["Actual_Spent"];
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
            //          List<ImprestSurrenderLineModel> imprestLinesList = new List<ImprestSurrenderLineModel>();

            //dynamic imprestRequestAjaxLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenderLines(DocumentNo));

            //foreach (var imprestLine in imprestRequestAjaxLines)
            //{
            //	ImprestSurrenderLineModel imprestLineObj = new ImprestSurrenderLineModel();
            //	imprestLineObj.LineNo = imprestLine.LineNo;
            //	imprestLineObj.DocumentNo = imprestLine.DocumentNo;
            //	imprestLineObj.ImprestSurrenderCode = imprestLine.ImprestSurrenderCode;
            //	imprestLineObj.AccountType = imprestLine.AccountType;
            //	imprestLineObj.AccountNo = imprestLine.AccountNo;
            //	imprestLineObj.AccountName = imprestLine.AccountName;
            //	imprestLineObj.LineSurrenderDescription = imprestLine.LineSurrenderDescription;
            //	imprestLineObj.LineAmount = imprestLine.LineAmount;
            //	imprestLineObj.ReceiptNo = imprestLine.ReceiptNo;
            //	imprestLineObj.ReceiptAmount = imprestLine.ReceiptAmount;
            //	imprestLineObj.LineActualAmount = imprestLine.LineActualAmount;
            //	imprestLinesList.Add(imprestLineObj);
            //}
            return Json(pettyCashSurrenderLinesList, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult GetImprestSurrenderLine(string LineNo, string DocumentNo)
		{
			/*ImprestSurrenderLineModel imprestLineObj = new ImprestSurrenderLineModel();

			dynamic imprestSurrenderLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestRequestByLine(Convert.ToInt32(LineNo), DocumentNo));

			foreach (var imprestLine in imprestSurrenderLines)
			{
				imprestLineObj.LineNo = imprestLine.LineNo;
				imprestLineObj.DocumentNo = imprestLine.DocumentNo;
				imprestLineObj.ImprestSurrenderCode = imprestLine.ImprestSurrenderCode;
				imprestLineObj.AccountType = imprestLine.AccountType;
				imprestLineObj.AccountNo = imprestLine.AccountNo;
				imprestLineObj.AccountName = imprestLine.AccountName;
				imprestLineObj.LineSurrenderDescription = imprestLine.LineSurrenderDescription;
				imprestLineObj.LineAmount = imprestLine.LineAmount;
				imprestLineObj.ReceiptNo = imprestLine.ReceiptNo;
				imprestLineObj.ReceiptAmount = imprestLine.ReceiptAmount;
				imprestLineObj.LineActualAmount = imprestLine.LineActualAmount;
				imprestLineObj.LineSurrenderDescription = imprestLine.LineDescription;
			}*/

			ImprestSurrenderLineModel imprestLineObj = new ImprestSurrenderLineModel();

			string globalDimension1Code = dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenderGlobalDimension1Code(DocumentNo);
			//	LoadImprestRequestDimensions(globalDimension1Code);
			LoadImprestSurrenderCodes();
			LoadReceiptLines();
			string imprestSurrenderlines = "ImprestLines?$filter=Line_No eq " + LineNo + " &$format=json";

			HttpWebResponse httpResponseDestForeign1 = Credentials.GetOdataData(imprestSurrenderlines);
			using (var streamReader1 = new StreamReader(httpResponseDestForeign1.GetResponseStream()))
			{
				var result1 = streamReader1.ReadToEnd();

				var details1 = JObject.Parse(result1);
				var imprestLinesList = new List<ImprestSurrenderLineModel>();

				foreach (JObject config1 in details1["value"])
				{
					var imprestSurrenderLineModel = new ImprestSurrenderLineModel();
					imprestLineObj.LineNo = (string)config1["Line_No"];
					imprestLineObj.DocumentNo = (string)config1["Document_No"];
					imprestLineObj.LineAmount = (string)config1["Amount"];
					imprestLineObj.Dimension1 = (string)config1["Shortcut_Dimension_1_Code"];
					imprestLineObj.Dimension2 = (string)config1["Shortcut_Dimension_2_Code"];
					imprestLineObj.Dimension3 = (string)config1["ShortcutDimCode3"];
					imprestLineObj.Dimension4 = (string)config1["ShortcutDimCode4"];
					imprestLineObj.Dimension5 = (string)config1["ShortcutDimCode5"];
					imprestLineObj.Dimension6 = (string)config1["ShortcutDimCode6"];
					imprestLineObj.Dimension7 = (string)config1["ShortcutDimCode7"];
					//imprestLineObj. = (string)config1["Status"];
					imprestLinesList.Add(imprestSurrenderLineModel);
				}
			}

			imprestLineObj.ReceiptNos = new SelectList(receiptLineList, "ReceiptNo", "Description");
			imprestLineObj.ImprestSurrenderCodes = new SelectList(imprestSurrenderCodes, "TransactionCode", "TransactionDescription");

			return Json(imprestLineObj, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult CreateImprestSurrenderLine(ImprestSurrenderLineModel ImprestSurrenderLineObj)
		{
			bool imprestSurrenderLineCreated = false;

			ImprestSurrenderLineObj.LineGlobalDimension1Code = ImprestSurrenderLineObj.LineGlobalDimension1Code ?? "";
			ImprestSurrenderLineObj.LineGlobalDimension2Code = ImprestSurrenderLineObj.LineGlobalDimension2Code ?? "";
			ImprestSurrenderLineObj.LineShortcutDimension3Code = ImprestSurrenderLineObj.LineShortcutDimension3Code ?? "";
			ImprestSurrenderLineObj.LineShortcutDimension4Code = ImprestSurrenderLineObj.LineShortcutDimension4Code ?? "";
			ImprestSurrenderLineObj.LineShortcutDimension5Code = ImprestSurrenderLineObj.LineShortcutDimension5Code ?? "";
			ImprestSurrenderLineObj.LineShortcutDimension6Code = ImprestSurrenderLineObj.LineShortcutDimension6Code ?? "";
			ImprestSurrenderLineObj.LineShortcutDimension7Code = ImprestSurrenderLineObj.LineShortcutDimension7Code ?? "";
			ImprestSurrenderLineObj.LineShortcutDimension8Code = ImprestSurrenderLineObj.LineShortcutDimension8Code ?? "";
			ImprestSurrenderLineObj.ReceiptNo = ImprestSurrenderLineObj.ReceiptNo ?? "";

			imprestSurrenderLineCreated = dynamicsNAVSOAPServices.fundsManagementWS.CreateImprestSurrenderLine(ImprestSurrenderLineObj.DocumentNo, ImprestSurrenderLineObj.ReceiptNo,
																											   Convert.ToDecimal(ImprestSurrenderLineObj.LineActualAmount), ImprestSurrenderLineObj.LineSurrenderDescription);

			return Json(new { ImprestSurrenderLineCreated = imprestSurrenderLineCreated }, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult ModifyImprestSurrenderLine(ImprestSurrenderLineModel ImprestSurrenderLineObj)
		{
			try
			{
				bool imprestSurrenderLineModified = false;

				ImprestSurrenderLineObj.LineGlobalDimension1Code = ImprestSurrenderLineObj.LineGlobalDimension1Code ?? "";
				ImprestSurrenderLineObj.LineGlobalDimension2Code = ImprestSurrenderLineObj.LineGlobalDimension2Code ?? "";
				ImprestSurrenderLineObj.LineShortcutDimension3Code = ImprestSurrenderLineObj.LineShortcutDimension3Code ?? "";
				ImprestSurrenderLineObj.LineShortcutDimension4Code = ImprestSurrenderLineObj.LineShortcutDimension4Code ?? "";
				ImprestSurrenderLineObj.LineShortcutDimension5Code = ImprestSurrenderLineObj.LineShortcutDimension5Code ?? "";
				ImprestSurrenderLineObj.LineShortcutDimension6Code = ImprestSurrenderLineObj.LineShortcutDimension6Code ?? "";
				ImprestSurrenderLineObj.LineShortcutDimension7Code = ImprestSurrenderLineObj.LineShortcutDimension7Code ?? "";
				ImprestSurrenderLineObj.LineShortcutDimension8Code = ImprestSurrenderLineObj.LineShortcutDimension8Code ?? "";
				ImprestSurrenderLineObj.ReceiptNo = ImprestSurrenderLineObj.ReceiptNo ?? "";

				imprestSurrenderLineModified = dynamicsNAVSOAPServices.fundsManagementWS.ModifyImprestSurrenderLine(Convert.ToInt32(ImprestSurrenderLineObj.LineNo), ImprestSurrenderLineObj.DocumentNo,
					ImprestSurrenderLineObj.ReceiptNo, Convert.ToDecimal(ImprestSurrenderLineObj.LineActualAmount), ImprestSurrenderLineObj.LineSurrenderDescription, ImprestSurrenderLineObj.LineGlobalDimension1Code, ImprestSurrenderLineObj.LineGlobalDimension2Code, ImprestSurrenderLineObj.LineShortcutDimension3Code,
					ImprestSurrenderLineObj.LineShortcutDimension4Code, ImprestSurrenderLineObj.LineShortcutDimension5Code, ImprestSurrenderLineObj.LineShortcutDimension6Code, ImprestSurrenderLineObj.LineShortcutDimension7Code);

				return Json(new {success = true, ImprestSurrenderLineModified = imprestSurrenderLineModified }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				return Json(new {success = false, message = e.Message, ImprestSurrenderLineModified = false }, JsonRequestBehavior.AllowGet);
			}
		}

		[Authorize]
		public JsonResult DeleteImprestSurrenderLine(int LineNo, string DocumentNo)
		{
			bool imprestLineDeleted = false;

			imprestLineDeleted = dynamicsNAVSOAPServices.fundsManagementWS.DeleteImprestSurrenderLine(LineNo, DocumentNo);

			return Json(new { ImprestLineDeleted = imprestLineDeleted }, JsonRequestBehavior.AllowGet);
		}
		#endregion Imprest Surrender Line

		#region Helper Functions
		//public JsonResult GetImprestAmount(string DocumentNo)
		//{
		//	decimal imprestAmount = 0;
		//	imprestAmount = dynamicsNAVSOAPServices.fundsManagementWS.GetImprestAmount(DocumentNo);
		//	return Json(new { Amount = imprestAmount }, JsonRequestBehavior.AllowGet);
		//}
		public JsonResult GetImprestSurrenderAmount(string DocumentNo)
		{
			decimal imprestAmount = 0;
			imprestAmount = dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenderAmount(DocumentNo);
			return Json(new { Amount = imprestAmount }, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetImprestBalance(string imprestNo, decimal actualSpent)
		{
			decimal imprestRemainingAmount = 0;
			imprestRemainingAmount = dynamicsNAVSOAPServices.fundsManagementWS.GetImprestRemainingAmount(imprestNo, actualSpent);
			return Json(new { Difference = imprestRemainingAmount }, JsonRequestBehavior.AllowGet);
		}
		public string GetImprestSurrenderStatus(string DocumentNo)
		{
			return dynamicsNAVSOAPServices.fundsManagementWS.GetImprestSurrenderStatus(DocumentNo);
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
		private void LoadReceiptLines()
		{
			receiptLineList = JsonConvert.DeserializeObject<List<ReceiptList>>(dynamicsNAVSOAPServices.fundsManagementWS.GetReceiptList(employeeNo));
		}
		private void LoadImprestSurrenderCodes()
		{
			imprestSurrenderCodes = new List<FundsTransactionModel>();

			dynamic fundsTransactionCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestTransactionCodes());
			foreach (var fundsTransactionCode in fundsTransactionCodes)
			{
				FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
				FundsTransactionObj.TransactionCode = fundsTransactionCode.TransactionCode;
				FundsTransactionObj.TransactionDescription = fundsTransactionCode.TransactionDescription;

				imprestSurrenderCodes.Add(FundsTransactionObj);
			}
		}
		[Authorize]
		[HttpGet]
		public JsonResult ValidateImprestSurrenderLines(string DocumentNo, string UnsurrenderedImprest)
		{
            try
            {
                return Json(dynamicsNAVSOAPServices.fundsManagementWS.InsertImprestSurrenderLines(DocumentNo, UnsurrenderedImprest), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
		#endregion Helper Functions

		public ActionResult Substitution(string imprestsurrenderno)
		{
			try
			{
				if (imprestsurrenderno.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}

				var requestHeader = RequestHeaders("", imprestsurrenderno)?.FirstOrDefault();
				return View(requestHeader);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		public List<RequestHeader> RequestHeaders(string empNo, string no)
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

			string url = $"Request_Headers?$filter={filterQuery}&$format=json";
			var httpResponseDestForeign = Credentials.GetOdataData(url);
			httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
			using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
			{
				var result1 = streamReader1.ReadToEnd();
				var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<RequestHeader>>(result1);
				return StaffAdvanceList?.ListValues;
			}
		}
    }

    public class ImprestDimensionValue
    {
	    public string Code { get; set; }
	    public string Name { get; set; }
    }
}