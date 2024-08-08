using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.HumanResource.SalaryAdvanceModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    public class SalaryAdvanceController : Controller
    {
		static string companyName = ServiceConnection.CompanyName;
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

		string employeeNo = "";
		public SalaryAdvanceController()
        {
			employeeNo = AccountController.GetEmployeeNo();
		}

		#region New Salary Advance
		public ActionResult NewSalaryAdvance()
        {
			string SalaryAdvanceNo = "";
			string OpenSalaryAdvance = ""; 
			try
			{
				//Check open Salary Advance Memo
				OpenSalaryAdvance = dynamicsNAVSOAPServices.salaryAdvanceWS.OpenMemoSalaryAdvanceNoExists(employeeNo);

				if (!OpenSalaryAdvance.Equals(""))
				{
					responseHeader = "Salary Advance Memo Exist";
					responseMessage = "You have an open Salary Advance Memo No. " + OpenSalaryAdvance + " which needs to be used before for Salary Advance Memo.Click edit button for you to proceed.";
					detailedResponseMessage = "You have an open Salary Advance Memo No. " + OpenSalaryAdvance + " which needs to be used before for Salary Advance Memo.Click edit button for you to proceed.";

					button1ControllerName = "SalaryAdvance";
					button1ActionName = "SalaryAdvanceHistory";
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
				//End check open Salary Advance Memo

				SalaryAdvanceHeaderModel salaryAdvanceObj = new SalaryAdvanceHeaderModel();

				//Create Salary Advance Memo
				SalaryAdvanceNo = dynamicsNAVSOAPServices.salaryAdvanceWS.CreateMemoSalaryAdvance(employeeNo);

				salaryAdvanceObj.No = SalaryAdvanceNo;
				salaryAdvanceObj.EmployeeNo = AccountController.GetEmployeeNo();
				salaryAdvanceObj.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

				return View(salaryAdvanceObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult NewSalaryAdvance(SalaryAdvanceHeaderModel salaryAdvanceObj)
		{
			try
			{
				if (ModelState.IsValid)
				{
					//Modify Salary Advance
					bool salaryAdvanceModified = dynamicsNAVSOAPServices.salaryAdvanceWS.ModifyMemoSalaryAdvance(salaryAdvanceObj.No,Convert.ToDecimal(salaryAdvanceObj.Amount), salaryAdvanceObj.Description);
					 
					if (!dynamicsNAVSOAPServices.salaryAdvanceWS.CheckMemoSalaryAdvanceApprovalWorkflowEnabled(salaryAdvanceObj.No))
					{
						salaryAdvanceObj.ErrorStatus = true;
						salaryAdvanceObj.ErrorMessage = "An error was experienced when sending your Salary Advance Memo " + salaryAdvanceObj.No + " for approval. Try again or contact the " + companyName + " ICT department.";
						return View(salaryAdvanceObj);
					}

					if (dynamicsNAVSOAPServices.salaryAdvanceWS.SendMemoSalaryAdvanceApprovalRequest(salaryAdvanceObj.No))
					{
						responseHeader = "Salary Advance Success";
						responseMessage = "Your Salary Advance application was successfully sent for approval.";
						detailedResponseMessage = "Your Salary Advance application was successfully sent for approval.";

						button1ControllerName = "SalaryAdvance";
						button1ActionName = "SalaryAdvanceHistory";
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
						salaryAdvanceObj.ErrorStatus = true;
						salaryAdvanceObj.ErrorMessage = "An error was experienced when sending your Salary Advance application for approval. Try again or contact the " + companyName + " ICT department.";
						return View(salaryAdvanceObj);
					}

				}
				else
				{
					return View(salaryAdvanceObj);
				}
			}
			catch (Exception ex)
			{
				salaryAdvanceObj.ErrorStatus = true;
				salaryAdvanceObj.ErrorMessage = ex.Message;
				return View(salaryAdvanceObj);
			}
		}
		#endregion End Salary Advance

		#region Edit Salary Advance

		[Authorize]
		public ActionResult OnBeforeEdit(string MemoNo)
		{
			try
			{
				if (MemoNo.Equals(""))
				{
					return RedirectToAction("SalaryAdvanceHistory", "SalaryAdvance");
				}
				if (dynamicsNAVSOAPServices.salaryAdvanceWS.CheckMemoSalaryAdvanceExists(MemoNo, AccountController.GetEmployeeNo()))
				{
					string salaryAdvanceStatus = dynamicsNAVSOAPServices.salaryAdvanceWS.GetMemoSalaryAdvanceStatus(MemoNo);

					//if salary Advance is open
					if (salaryAdvanceStatus.Equals("Open"))
					{
						return RedirectToAction("EditSalaryAdvance", "SalaryAdvance", new { MemoNo = MemoNo });
					}

					//if salary Advance is pending approval
					if (salaryAdvanceStatus.Equals("Pending Approval"))
					{
						responseHeader = "Salary Advance Pending Approval";
						responseMessage = "The Salary Advance Memo No." + MemoNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
						detailedResponseMessage = "The Salary Advance Memo No." + MemoNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";

						button1ControllerName = "SalaryAdvance";
						button1ActionName = "EditSalaryAdvance";
						button1HasParameters = true;
						button1Parameters = "?MemoNo=" + MemoNo;
						button1Name = "Yes";

						button2ControllerName = "SalaryAdvance";
						button2ActionName = "SalaryAdvanceHistory";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}

					//if salary Advance is released
					if (salaryAdvanceStatus.Equals("Released"))
					{
						responseHeader = "Salary Advance Approved";
						responseMessage = "The Salary Advance Memo No." + MemoNo + " is already approved. Editing not allowed.";
						detailedResponseMessage = "The Salary Advance Memo No." + MemoNo + " is already approved. Editing not allowed.";

						button1ControllerName = "SalaryAdvance";
						button1ActionName = "SalaryAdvanceHistory";
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

					//if Salary Advance is rejected
					if (salaryAdvanceStatus.Equals("Rejected"))
					{
						responseHeader = "Salary Advance Rejected";
						responseMessage = "The Salary Advance Memo No." + MemoNo  + " was rejected. Editing will reopen the document. Do you want to continue?";
						detailedResponseMessage = "The Salary Advance Memo No." + MemoNo + " was rejected. Editing will reopen the document. Do you want to continue?";

						button1ControllerName = "SalaryAdvance";
						button1ActionName = "EditSalaryAdvance";
						button1HasParameters = true;
						button1Parameters = "?MemoNo=" + MemoNo;
						button1Name = "Yes";

						button2ControllerName = "SalaryAdvance";
						button2ActionName = "SalaryAdvanceHistory";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}

					//if Salary Advance is posted/reversed
					if (salaryAdvanceStatus.Equals("Posted") || salaryAdvanceStatus.Equals("Reversed"))
					{
						responseHeader = "Salary Advance Posted";
						responseMessage = "The Salary Advance Memo No." + MemoNo + " is already posted. Editing not allowed.";
						detailedResponseMessage = "The Salary Advance Memo No." + MemoNo + " is already posted. Editing not allowed.";

						button1ControllerName = "SalaryAdvance";
						button1ActionName = "SalaryAdvanceHistory";
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
					return RedirectToAction("SalaryAdvanceHistory", "SalaryAdvance");
				}
				else
				{
					responseHeader = "Salary Advance NotFound";
					responseMessage = "The Salary Advance Memo No." + MemoNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The Salary Advance Memo No." + MemoNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "SalaryAdvance";
					button1ActionName = "SalaryAdvanceHistory";
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
		public ActionResult EditSalaryAdvance(string MemoNo) 
		{
			try
			{
				if (MemoNo.Equals(""))
				{
					return RedirectToAction("SalaryAdvanceHistory", "SalaryAdvance");
				}
				if (dynamicsNAVSOAPServices.salaryAdvanceWS.CheckMemoSalaryAdvanceExists(MemoNo, AccountController.GetEmployeeNo()))
				{
					SalaryAdvanceHeaderModel salaryAdvanceObj = new SalaryAdvanceHeaderModel();
					dynamic salaryAdvances = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.salaryAdvanceWS.GetMemoSalaryAdvance(MemoNo, ""));
					foreach (var salaryAdvance in salaryAdvances)
					{
						salaryAdvanceObj.No = salaryAdvance.No;
						salaryAdvanceObj.EmployeeNo = salaryAdvance.EmployeeNo;
						salaryAdvanceObj.EmployeeName = salaryAdvance.EmployeeName;
						salaryAdvanceObj.Amount = salaryAdvance.Amount;
						salaryAdvanceObj.Description = salaryAdvance.Description;
						salaryAdvanceObj.Status = salaryAdvance.Status;

					}

					return View(salaryAdvanceObj);
				}
				else
				{
					responseHeader = "Salary Advance Memo NotFound";
					responseMessage = "The Salary Advance Memo" + MemoNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The Salary Advance Memo" + MemoNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "SalaryAdvance";
					button1ActionName = "SalaryAdvanceHistory";
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
		public ActionResult EditSalaryAdvance(SalaryAdvanceHeaderModel salaryAdvanceObj)
		{
			try
			{
				if (ModelState.IsValid)
				{
					//Modify Salary Advance
					bool salaryAdvanceModified = dynamicsNAVSOAPServices.salaryAdvanceWS.ModifyMemoSalaryAdvance(salaryAdvanceObj.No, Convert.ToDecimal(salaryAdvanceObj.Amount), salaryAdvanceObj.Description);

					if (!dynamicsNAVSOAPServices.salaryAdvanceWS.CheckMemoSalaryAdvanceApprovalWorkflowEnabled(salaryAdvanceObj.No))
					{
						salaryAdvanceObj.ErrorStatus = true;
						salaryAdvanceObj.ErrorMessage = "An error was experienced when sending your Salary Advance Memo " + salaryAdvanceObj.No + " for approval. Try again or contact the " + companyName + " ICT department.";
						return View(salaryAdvanceObj);
					}

					if (dynamicsNAVSOAPServices.salaryAdvanceWS.SendMemoSalaryAdvanceApprovalRequest(salaryAdvanceObj.No))
					{
						responseHeader = "Salary Advance Success";
						responseMessage = "Your Salary Advance application was successfully sent for approval.";
						detailedResponseMessage = "Your Salary Advance application was successfully sent for approval.";

						button1ControllerName = "SalaryAdvance";
						button1ActionName = "SalaryAdvanceHistory";
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
						salaryAdvanceObj.ErrorStatus = true;
						salaryAdvanceObj.ErrorMessage = "An error was experienced when sending your Salary Advance application for approval. Try again or contact the " + companyName + " ICT department.";
						return View(salaryAdvanceObj);
					}

				}
				else
				{
					return View(salaryAdvanceObj);
				}
			}
			catch (Exception ex)
			{
				salaryAdvanceObj.ErrorStatus = true;
				salaryAdvanceObj.ErrorMessage = ex.Message;
				return View(salaryAdvanceObj);
			}
		}
		#endregion End Edit Salary Advance

		#region View Salary Advance

		[Authorize]
		public ActionResult ViewSalaryAdvance (string MemoNo) 
		{
			try
			{
				if (MemoNo.Equals(""))
				{
					return RedirectToAction("SalaryAdvanceHistory", "SalaryAdvance");
				}
				if (dynamicsNAVSOAPServices.salaryAdvanceWS.CheckMemoSalaryAdvanceExists(MemoNo, AccountController.GetEmployeeNo()))
				{
					SalaryAdvanceHeaderModel salaryAdvanceObj = new SalaryAdvanceHeaderModel();
					dynamic salaryAdvances = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.salaryAdvanceWS.GetMemoSalaryAdvance(MemoNo, ""));
					foreach (var salaryAdvance in salaryAdvances)
					{
						salaryAdvanceObj.No = salaryAdvance.No;
						salaryAdvanceObj.EmployeeNo = salaryAdvance.EmployeeNo;
						salaryAdvanceObj.EmployeeName = salaryAdvance.EmployeeName;
						salaryAdvanceObj.Amount = salaryAdvance.Amount;
						salaryAdvanceObj.Description = salaryAdvance.Description;
						salaryAdvanceObj.Status = salaryAdvance.Status;

					}

					return View(salaryAdvanceObj);
				}
				else
				{
					responseHeader = "Salary Advance Memo NotFound";
					responseMessage = "The Salary Advance Memo" + MemoNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The Salary Advance Memo" + MemoNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "SalaryAdvance";
					button1ActionName = "SalaryAdvanceHistory";
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

		#endregion End View Salary Advance

		#region Salary Advance History

		[Authorize]
		public ActionResult SalaryAdvanceHistory() 
		{
			try
			{
				List<SalaryAdvanceHeaderModel> salaryAdvanceList = new List<SalaryAdvanceHeaderModel>();
				dynamic salaryAdvances = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.salaryAdvanceWS.GetMemoSalaryAdvance("",employeeNo));
				foreach (var salaryAdvance in salaryAdvances)
				{
					SalaryAdvanceHeaderModel salaryAdvanceObj = new SalaryAdvanceHeaderModel();
					salaryAdvanceObj.No = salaryAdvance.No;
					salaryAdvanceObj.EmployeeNo = salaryAdvance.EmployeeNo;
					salaryAdvanceObj.EmployeeName = salaryAdvance.EmployeeName;
					salaryAdvanceObj.Amount = salaryAdvance.Amount;
					salaryAdvanceObj.Description = salaryAdvance.Description;
					salaryAdvanceObj.Status = salaryAdvance.Status;

					salaryAdvanceList.Add(salaryAdvanceObj);

				}
				return View(salaryAdvanceList);
			}

			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}
		#endregion End Salary Advance History

		#region Salary Advance Line

		[ChildActionOnly]
		[Authorize]
		public ActionResult _SalaryAdvanceLine(string DocumentNo)
		{
			SalaryAdvanceLineModel salaryAdvanceLineObj = new SalaryAdvanceLineModel();

			return PartialView(salaryAdvanceLineObj);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewSalaryAdvanceLine(string DocumentNo)
		{
			SalaryAdvanceLineModel salaryAdvanceLineObj = new SalaryAdvanceLineModel(); 

			return PartialView(salaryAdvanceLineObj);
		}

		[Authorize]
		public JsonResult GetSalaryAdvanceLines(string DocumentNo)
		{
			List<SalaryAdvanceLineModel> salaryAdvanceList = new List<SalaryAdvanceLineModel>();

			dynamic salaryAdvanceLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.salaryAdvanceWS.GetMemoSalaryAdvanceLines(DocumentNo));

			foreach (var salaryAdvanceLine in salaryAdvanceLines) 
			{
				SalaryAdvanceLineModel salaryAdvanceLineObj = new SalaryAdvanceLineModel();
				salaryAdvanceLineObj.LineNo = salaryAdvanceLine.LineNo;
				salaryAdvanceLineObj.DocumentNo = salaryAdvanceLine.No;
				salaryAdvanceLineObj.LineAmount = salaryAdvanceLine.Amount;

				salaryAdvanceList.Add(salaryAdvanceLineObj);
			}
			return Json(salaryAdvanceList, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult GetSalaryAdvanceLine(string LineNo, string DocumentNo)
		{
			SalaryAdvanceLineModel salaryAdvanceLineObj = new SalaryAdvanceLineModel();
			dynamic salaryAdvanceLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.salaryAdvanceWS.GetMemoSalaryAdvanceByLineNo(Convert.ToInt32(LineNo), DocumentNo));
			foreach (var salaryAdvanceLine in salaryAdvanceLines)
			{
				salaryAdvanceLineObj.LineNo = salaryAdvanceLine.LineNo;
				salaryAdvanceLineObj.DocumentNo = salaryAdvanceLine.No;
				salaryAdvanceLineObj.LineAmount = salaryAdvanceLine.Amount;
			}

			return Json(salaryAdvanceLineObj, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult CreateSalaryAdvanceLine(SalaryAdvanceLineModel SalaryAdvanceLineObj)
		{
			bool salaryAdvanceLineCreated = false;

			salaryAdvanceLineCreated = dynamicsNAVSOAPServices.salaryAdvanceWS.CreateMemoSalaryAdvanceLine(SalaryAdvanceLineObj.DocumentNo, Convert.ToDecimal(SalaryAdvanceLineObj.LineAmount));

			return Json(new { SalaryAdvanceLineCreated = salaryAdvanceLineCreated }, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult ModifySalaryAdvanceLine(SalaryAdvanceLineModel SalaryAdvanceLineObj)
		{
			bool salaryAdvanceLineModified = false;

			salaryAdvanceLineModified = dynamicsNAVSOAPServices.salaryAdvanceWS.ModifyMemoSalaryAdvanceLine(Convert.ToInt32(SalaryAdvanceLineObj.LineNo), SalaryAdvanceLineObj.DocumentNo, Convert.ToDecimal(SalaryAdvanceLineObj.LineAmount));

			return Json(new { SalaryAdvanceLineModified = salaryAdvanceLineModified }, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult DeleteSalaryAdvanceLine(string LineNo, string DocumentNo)
		{
			bool salaryAdvanceLineDeleted = false;

			salaryAdvanceLineDeleted = dynamicsNAVSOAPServices.salaryAdvanceWS.DeleteMemoSalaryAdvanceLine(Convert.ToInt32(LineNo), DocumentNo);

			return Json(new { SalaryAdvanceDeleted = salaryAdvanceLineDeleted }, JsonRequestBehavior.AllowGet);
		}
		#endregion Salary Advance Line

		#region Helper Functions
		public JsonResult GetSalaryAdvanceAmount(string DocumentNo)
		{
			decimal salaryAdvanceAmount = 0;
			salaryAdvanceAmount = dynamicsNAVSOAPServices.salaryAdvanceWS.GetSalaryAdvanceAmount(DocumentNo);
			return Json(new { Amount = salaryAdvanceAmount }, JsonRequestBehavior.AllowGet);
		}
		#endregion End Helper Functions
	}
}