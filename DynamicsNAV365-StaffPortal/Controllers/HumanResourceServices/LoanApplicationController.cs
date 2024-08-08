using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.HumanResource.LoanApplicationModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    [NoCache]
    public class LoanApplicationController : Controller
    {
		static string companyName = ServiceConnection.CompanyName;
		static string companyURL = "";

		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
		DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

		SuccessResponseController successResponse = new SuccessResponseController();
		InfoResponseController infoResponse = new InfoResponseController();
		ErrorResponseController errorResponse = new ErrorResponseController();

		List<LoanProductsModel> loanProductsObjList = null;

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

		string employeeNo = "";

		public LoanApplicationController()
        {
			employeeNo = AccountController.GetEmployeeNo();
        }

		#region New Loan Application

		[Authorize]
		public ActionResult NewLoanApplication() 
		{
			string LoanApplicationNo = "";
			string OpenLoanApplicationNo = "";
			try
			{
				//Check open loan application
				OpenLoanApplicationNo = dynamicsNAVSOAPServices.loanManagementWS.OpenLoanApplicationNoExists(employeeNo);
				
				if (!OpenLoanApplicationNo.Equals(""))
				{
					responseHeader = "Loan Application Exist";
					responseMessage = "You have an open document No. "+ OpenLoanApplicationNo + " which needs to be used before for loan application.Click edit button for you to proceed.";
					detailedResponseMessage = "You have an open document No. " + OpenLoanApplicationNo + " which needs to be used before for loan application. Click edit button for you to proceed.";

					button1ControllerName = "LoanApplication";
					button1ActionName = "LoanApplicationHistory";
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
				//End check open loan application

				LoanApplicationModel loanApplicationObj = new LoanApplicationModel();

				//Create loan Application
				LoanApplicationNo = dynamicsNAVSOAPServices.loanManagementWS.CreateLoanApplication(employeeNo);

				loanApplicationObj.No = LoanApplicationNo;
				loanApplicationObj.EmployeeNo = AccountController.GetEmployeeNo();
				loanApplicationObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

				LoadLoanProducts();
				loanApplicationObj.LoanProductTypes = new SelectList(loanProductsObjList, "Code", "Description");

				return View(loanApplicationObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult NewLoanApplication(LoanApplicationModel loanApplicationObj) 
		{
			try
			{
				LoadLoanProducts();

				if (ModelState.IsValid)
				{
					loanApplicationObj.LoanProductTypes = new SelectList(loanProductsObjList, "Code", "Description");

					//Modify Loan Application
					bool loanApplicationModified = dynamicsNAVSOAPServices.loanManagementWS.ModifyLoanApplication(loanApplicationObj.No, loanApplicationObj.LoanProductType, Convert.ToDecimal(loanApplicationObj.Amount), loanApplicationObj.Description);

					if (!dynamicsNAVSOAPServices.loanManagementWS.CheckLoanApplicationApprovalWorkflowEnabled(loanApplicationObj.No))
					{
						loanApplicationObj.ErrorStatus = true;
						loanApplicationObj.ErrorMessage = "An error was experienced when sending your loan application no." + loanApplicationObj.No + " for approval. Try again or contact the " + companyName + " ICT department.";
						return View(loanApplicationObj);
					}

					if (dynamicsNAVSOAPServices.loanManagementWS.SendLoanApplicationApprovalRequest(loanApplicationObj.No))
					{
						responseHeader = "Success";
						responseMessage = "Your loan application was successfully sent for approval.";
						detailedResponseMessage = "Your loan application was successfully sent for approval.";

						button1ControllerName = "LoanApplication";
						button1ActionName = "LoanApplicationHistory";
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
						loanApplicationObj.ErrorStatus = true;
						loanApplicationObj.ErrorMessage = "An error was experienced when sending your loan application for approval. Try again or contact the " + companyName + " ICT department.";
						return View(loanApplicationObj);
					}

				}
				else
				{
					return View(loanApplicationObj);
				}
			}
			catch (Exception ex)
			{
				loanApplicationObj.ErrorStatus = true;
				loanApplicationObj.ErrorMessage = ex.Message;
				return View(loanApplicationObj);
			}
		}
		#endregion New Leave Application

		#region Edit Loan Application

		[Authorize]
		public ActionResult OnBeforeEdit(string LoanApplicationNo)
		{
			try
			{
				if (LoanApplicationNo.Equals(""))
				{
					return RedirectToAction("LoanApplicationHistory", "LoanApplication");
				}
				if (dynamicsNAVSOAPServices.loanManagementWS.CheckLoanApplicationExists(LoanApplicationNo, employeeNo))
				{
					string loanApplicationStatus = dynamicsNAVSOAPServices.loanManagementWS.GetLoanApplicationStatus(LoanApplicationNo);

					//if Loan application is open
					if (loanApplicationStatus.Equals("Open")|| loanApplicationStatus.Equals("Application"))
					{
						return RedirectToAction("EditLoanApplication", "LoanApplication", new { LoanApplicationNo = LoanApplicationNo });
					}

					//if Loan application is pending approval
					if (loanApplicationStatus.Equals("Pending Approval"))
					{
						responseHeader = "Loan Application Pending Approval";
						responseMessage = "The Loan application no." + LoanApplicationNo + " is already submitted for approval. Editing not allowed.";
						detailedResponseMessage = "The Loan application no." + LoanApplicationNo + "is already submitted for approval. Editing not allowed.";

						button1ControllerName = "LoanApplication";
						button1ActionName = "LoanApplicationHistory";
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

					//if Loan application is released
					if (loanApplicationStatus.Equals("Released"))
					{
						responseHeader = "Loan Application Approved";
						responseMessage = "The Loan application no." + LoanApplicationNo + " is approved. Editing will reopen the document. Do you want to continue?";
						detailedResponseMessage = "The Loan application no." + LoanApplicationNo + " is approved. Editing will reopen the document. Do you want to continue?";

						button1ControllerName = "LoanApplication";
						button1ActionName = "EditLoanApplication";
						button1HasParameters = true;
						button1Parameters = "?LoanApplicationNo=" + LoanApplicationNo;
						button1Name = "Ok";

						button2ControllerName = "LoanApplication";
						button2ActionName = "LoanApplicationHistory";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}
					//if Loan application is rejected
					if (loanApplicationStatus.Equals("Rejected"))
					{
						responseHeader = "Loan Application Rejected";
						responseMessage = "The Loan application no." + LoanApplicationNo + " was rejected. Editing will reopen the document. Do you want to continue?";
						detailedResponseMessage = "The Loan application no." + LoanApplicationNo + " was rejected. Editing will reopen the document. Do you want to continue?";

						button1ControllerName = "LoanApplication";
						button1ActionName = "EditLoanApplication";
						button1HasParameters = true;
						button1Parameters = "?LoanApplicationNo=" + LoanApplicationNo;
						button1Name = "Ok";

						button2ControllerName = "LoanApplication";
						button2ActionName = "LoanApplicationHistory";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}

					//if Loan application is posted/reversed
					if (loanApplicationStatus.Equals("Posted") || loanApplicationStatus.Equals("Reversed"))
					{
						responseHeader = "Loan Application Posted";
						responseMessage = "The Loan application no." + LoanApplicationNo + " is already posted. Editing not allowed.";
						detailedResponseMessage = "The Loan application no." + LoanApplicationNo + " is already posted. Editing not allowed.";

						button1ControllerName = "LoanApplication";
						button1ActionName = "LoanApplicationHistory";
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

					return RedirectToAction("LoanApplicationHistory", "LoanApplication");
				}
				else
				{
					responseHeader = "Loan Application NotFound";
					responseMessage = "The Loan application no." + LoanApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The Loan application no." + LoanApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "LoanApplication";
					button1ActionName = "LoanApplicationHistory";
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
		public ActionResult EditLoanApplication(string LoanApplicationNo)
		{
			try
			{
				if (LoanApplicationNo.Equals(""))
				{
					return RedirectToAction("LoanApplicationHistory", "LoanApplication");
				}
				if (dynamicsNAVSOAPServices.loanManagementWS.CheckLoanApplicationExists(LoanApplicationNo, employeeNo))
				{
					LoanApplicationModel loanApplicationObj = new LoanApplicationModel();
					dynamic loanApplications = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.loanManagementWS.GetLoanApplications(LoanApplicationNo, ""));
					foreach (var loanApplication in loanApplications)
					{
						loanApplicationObj.No = loanApplication.No;
						loanApplicationObj.EmployeeNo = loanApplication.EmployeeNo;
						loanApplicationObj.EmployeeName = loanApplication.EmployeeName;
						loanApplicationObj.LoanProductType = loanApplication.LoanProductType;
						loanApplicationObj.Amount = loanApplication.Amount;
						loanApplicationObj.Description = loanApplication.Description;
						loanApplicationObj.Status = loanApplication.Status;

					}

					LoadLoanProducts();
					loanApplicationObj.LoanProductTypes = new SelectList(loanProductsObjList, "Code", "Description");

					return View(loanApplicationObj);
				}
				else
				{
					responseHeader = "Loan Application NotFound";
					responseMessage = "The loan application no." + LoanApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The loan application no." + LoanApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "LoanApplication";
					button1ActionName = "LoanApplicationHistory";
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
		[ValidateAntiForgeryToken]
		public ActionResult EditLoanApplication(LoanApplicationModel loanApplicationObj) 
		{
			try
			{
				LoadLoanProducts();

				if (ModelState.IsValid)
				{
					loanApplicationObj.LoanProductTypes = new SelectList(loanProductsObjList, "Code", "Description");

					//Modify Loan Application
					bool loanApplicationModified = dynamicsNAVSOAPServices.loanManagementWS.ModifyLoanApplication(loanApplicationObj.No, loanApplicationObj.LoanProductType, Convert.ToDecimal(loanApplicationObj.Amount), loanApplicationObj.Description);

					if (!dynamicsNAVSOAPServices.loanManagementWS.CheckLoanApplicationApprovalWorkflowEnabled(loanApplicationObj.No))
					{
						loanApplicationObj.ErrorStatus = true;
						loanApplicationObj.ErrorMessage = "An error was experienced when sending your loan application no." + loanApplicationObj.No + " for approval. Try again or contact the " + companyName + " ICT department.";
						return View(loanApplicationObj);
					}

					if (dynamicsNAVSOAPServices.loanManagementWS.SendLoanApplicationApprovalRequest(loanApplicationObj.No))
					{
						responseHeader = "Success";
						responseMessage = "Your loan application was successfully sent for approval.";
						detailedResponseMessage = "Your loan application was successfully sent for approval.";

						button1ControllerName = "LoanApplication";
						button1ActionName = "LoanApplicationHistory";
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
						loanApplicationObj.ErrorStatus = true;
						loanApplicationObj.ErrorMessage = "An error was experienced when sending your loan application for approval. Try again or contact the " + companyName + " ICT department.";
						return View(loanApplicationObj);
					}

				}
				else
				{
					return View(loanApplicationObj);
				}
			}
			catch (Exception ex)
			{
				loanApplicationObj.ErrorStatus = true;
				loanApplicationObj.ErrorMessage = ex.Message;
				return View(loanApplicationObj);
			}
		}
		#endregion Edit Loan Application

		#region View Loan Application

		[Authorize]
		public ActionResult ViewLoanApplication(string LoanApplicationNo)
		{
			try
			{
				if (LoanApplicationNo.Equals(""))
				{
					return RedirectToAction("LoanApplicationHistory", "LoanApplication");
				}
				if (dynamicsNAVSOAPServices.loanManagementWS.CheckLoanApplicationExists(LoanApplicationNo, AccountController.GetEmployeeNo()))
				{
					LoanApplicationModel loanApplicationObj = new LoanApplicationModel();
					dynamic loanApplications = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.loanManagementWS.GetLoanApplications(LoanApplicationNo, ""));
					foreach (var loanApplication in loanApplications)
					{
						loanApplicationObj.No = loanApplication.No;
						loanApplicationObj.EmployeeNo = loanApplication.EmployeeNo;
						loanApplicationObj.EmployeeName = loanApplication.EmployeeName;
						loanApplicationObj.LoanProductType = loanApplication.LoanProductType;
						loanApplicationObj.Amount = loanApplication.Amount;
						loanApplicationObj.Description = loanApplication.Description;
						loanApplicationObj.Status = loanApplication.Status;

					}

					LoadLoanProducts();
					loanApplicationObj.LoanProductTypes = new SelectList(loanProductsObjList, "Code", "Description");

					return View(loanApplicationObj);
				}
				else
				{
					responseHeader = "Loan Application NotFound";
					responseMessage = "The loan application no." + LoanApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The loan application no." + LoanApplicationNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "LoanApplication";
					button1ActionName = "LoanApplicationHistory";
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

		#endregion View Loan Application

		#region Loan Application History

		[Authorize]
		public ActionResult LoanApplicationHistory()
        {
            try
            {
				List<LoanApplicationModel> loanApplicationList = new List<LoanApplicationModel>();
				dynamic loanApplications = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.loanManagementWS.GetLoanApplications("", employeeNo));
				foreach (var loanApplication in loanApplications)
				{
					LoanApplicationModel loanApplicationObj = new LoanApplicationModel();
					loanApplicationObj.No = loanApplication.No;
					loanApplicationObj.EmployeeNo = loanApplication.EmployeeNo;
					loanApplicationObj.EmployeeName = loanApplication.EmployeeName;
					loanApplicationObj.LoanProductType = loanApplication.LoanProductType;
					loanApplicationObj.Amount = loanApplication.Amount;
					loanApplicationObj.Description = loanApplication.Description;
					loanApplicationObj.Status = loanApplication.Status;

					loanApplicationList.Add(loanApplicationObj);

				}
				return View(loanApplicationList);
			}

            catch(Exception ex)
            {
				return errorResponse.ApplicationExceptionError(ex);
            }
        }
		#endregion Loan Application History

		#region Helper Functions
		public void LoadLoanProducts()
        {
			 loanProductsObjList = new List<LoanProductsModel>();

			dynamic loanProducts = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.loanManagementWS.GetLoanProducts());

			foreach (var loanProduct in loanProducts)
			{
				LoanProductsModel loanProductsObj = new LoanProductsModel();
				loanProductsObj.Code = loanProduct.Code;
				loanProductsObj.Description = loanProduct.Description;

				loanProductsObjList.Add(loanProductsObj);
			}
		}
		#endregion End Helper Functions
	}
}