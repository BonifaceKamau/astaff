using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using DynamicsNAV365_StaffPortal.Models.StaffRequisition;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    public class StaffRequisitionController : Controller
    {
		static string companyName = ServiceConnection.CompanyName;
		static string companyURL = "";

		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
		DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

		SuccessResponseController successResponse = new SuccessResponseController();
		InfoResponseController infoResponse = new InfoResponseController();
		ErrorResponseController errorResponse = new ErrorResponseController();

		List<RequisitionType> requisitionTypes= null;
		List<PositionType> positionTypes = null;
		List<Establishment> establishments = null;
        List<Chapter6Requirments> chapter6Requirements = null;
        List<EmploymentContract> employementContracts = null;
        List<ProfessionalQualifications> professionalQualificationList = null; 
        List<AcademicRequirements> academicRequirementList= null;

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

		public StaffRequisitionController()
        {
			employeeNo = AccountController.GetEmployeeNo();
		}

		#region New Staff Requisition

		[Authorize]
		public ActionResult NewStaffRequisition()
		{
			try
			{
				//Check if can initialize requisition
				if (!dynamicsNAVSOAPServices.hrManagementWS.CheckIfCanInitiateStaffRequisition(employeeNo))
				{
					responseHeader = "Staff Requisition Exist";
					responseMessage = "You do not have permissions to create Staff Requisition Form.";
					detailedResponseMessage = "You do not have permissions to create Staff Requisition Form.";

					button1ControllerName = "StaffRequisition";
					button1ActionName = "StaffRequisitionHistory";
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
				//End Check if can initialize requisition

				//Check open requisition
				if (dynamicsNAVSOAPServices.hrManagementWS.CheckOpenStaffRequsitionExists(employeeNo))
				{
					responseHeader = "Staff Requisition Exist";
					responseMessage = "An open staff requisition exists for employee no. " + employeeNo + ", finalize on this staff requisitionbefore creating a new one.";
					detailedResponseMessage = "An open staff requisition exists for employee no. " + employeeNo + ", finalize on this staff requisitionbefore creating a new one.";

					button1ControllerName = "StaffRequisition";
					button1ActionName = "StaffRequisitionHistory";
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
				//End check open requisition

				StaffRequisition staffRequisitionObj = new StaffRequisition();

				LoadEmploymentContracts();
				LoadJobEstablishments();
				LoadRequisitionTypes();
				LoadPositionTypes();

				staffRequisitionObj.No = dynamicsNAVSOAPServices.hrManagementWS.CreateStaffRequsition(employeeNo);
				
				staffRequisitionObj.AppointmentTypes = new SelectList(employementContracts, "Code", "Description");
				staffRequisitionObj.Establishments = new SelectList(establishments, "Code", "Description");
				staffRequisitionObj.RequisitionTypes = new SelectList(requisitionTypes, "Code", "Description");
				staffRequisitionObj.PositionTypes = new SelectList(positionTypes, "Code", "Description");

				return View(staffRequisitionObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult> NewStaffRequisition(StaffRequisition StaffRequisitionObj, string Command) 
		{
			try
			{
				LoadEmploymentContracts();
				LoadJobEstablishments();
				LoadRequisitionTypes();
				LoadPositionTypes();

				StaffRequisitionObj.AppointmentTypes = new SelectList(employementContracts, "Code", "Description");
				StaffRequisitionObj.Establishments = new SelectList(establishments, "Code", "Description");
				StaffRequisitionObj.RequisitionTypes = new SelectList(requisitionTypes, "Code", "Description");
				StaffRequisitionObj.PositionTypes = new SelectList(positionTypes, "Code", "Description");

				if (ModelState.IsValid)
				{
					if (StaffRequisitionObj.No.Equals(""))
					{
						return RedirectToAction("StaffRequisitionHistory", "StaffRequisition");
					}

					//DateTime.ParseExact(LeaveApplicationObj.LeaveStartDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture)


					if (Command.Equals("Submit For Approval"))
					{

						bool leaveApplicationModified = dynamicsNAVSOAPServices.hrManagementWS.ModifyStaffRequsition(StaffRequisitionObj.No, StaffRequisitionObj.JobNo, StaffRequisitionObj.PositionType, StaffRequisitionObj.RequisitionType,
																												Convert.ToInt32(StaffRequisitionObj.Positions), DateTime.Parse(StaffRequisitionObj.ExpectedReportingDate),
																												StaffRequisitionObj.AppointmentType, StaffRequisitionObj.Description);

						if (!dynamicsNAVSOAPServices.hrManagementWS.CheckStaffRequsitionApprovalWorkflowEnabled(StaffRequisitionObj.No))
						{
							StaffRequisitionObj.ErrorStatus = true;
							StaffRequisitionObj.ErrorMessage = "No approval record enabled for Employee Requisition.";
							return View(StaffRequisitionObj);
						}

						if (dynamicsNAVSOAPServices.hrManagementWS.SendStaffRequsitionApprovalRequest(StaffRequisitionObj.No))
						{
							responseHeader = "Success";
							responseMessage = "Staff Requisition submitted successfully for approval.";
							detailedResponseMessage = "Staff Requisition submitted successfully for approval.";

							button1ControllerName = "StaffRequisition";
							button1ActionName = "StaffRequisitionHistory"; 
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

					}
					if (Command.Equals("View Attachment"))
					{
						string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(StaffRequisitionObj.No);

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
							return View(StaffRequisitionObj);
						}
					}
					else
					{
						return View(StaffRequisitionObj);
					}
				}
				else
				{
					return View(StaffRequisitionObj);
				}
			}
			catch (Exception ex)
			{
				StaffRequisitionObj.ErrorStatus = true;
				StaffRequisitionObj.ErrorMessage = ex.Message;
				return View(StaffRequisitionObj);
			}
		}
		#endregion New Staff Requisition

		#region Edit Staff Requisition
		[Authorize]
		public ActionResult OnBeforeEdit(string StaffRequisitionNo)
		{
			try
			{
				if (StaffRequisitionNo.Equals(""))
				{
					return RedirectToAction("StaffRequisitionHistory", "StaffRequisition");
				}
				if (dynamicsNAVSOAPServices.hrManagementWS.CheckStaffRequsitionExists(StaffRequisitionNo, AccountController.GetEmployeeNo()))
				{
					string leaveApplicationStatus = dynamicsNAVSOAPServices.hrManagementWS.GetStaffRequsitionStatus(StaffRequisitionNo);
					//if leave application is open
					if (leaveApplicationStatus.Equals("Open") || leaveApplicationStatus.Equals("Declined with amendments"))
					{
						return RedirectToAction("EditStaffRequisition", "StaffRequisition", new { StaffRequisitionNo = StaffRequisitionNo });
					}

					//if leave application is pending approval
					if (leaveApplicationStatus.Equals("Pending Approval"))
					{
						responseHeader = "Staff Requisition Pending Approval";
						responseMessage = "The Staff Requisition No." + StaffRequisitionNo + " is already submitted for approval. Editing not allowed.";
						detailedResponseMessage = "The Staff Requisition No." + StaffRequisitionNo + "is already submitted for approval. Editing not allowed.";

						button1ControllerName = "StaffRequisition";
						button1ActionName = "StaffRequisitionHistory";
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

					//if leave application is rejected
					if (leaveApplicationStatus.Equals("Rejected") || leaveApplicationStatus == "Declined with amendments")
					{
						responseHeader = "Staff Requisition Rejected";
						responseMessage = "The Staff Requisition No." + StaffRequisitionNo + " was rejected. Editing will reopen the document. Do you want to continue?";
						detailedResponseMessage = "The Staff Requisition No." + StaffRequisitionNo + " was rejected. Editing will reopen the document. Do you want to continue?";

						button1ControllerName = "StaffRequisition";
						button1ActionName = "EditStaffRequisition";
						button1HasParameters = true;
						button1Parameters = "?StaffRequisitionNo=" + StaffRequisitionNo;
						button1Name = "Ok";

						button2ControllerName = "StaffRequisition";
						button2ActionName = "StaffRequisitionHistory";
						button2HasParameters = false;
						button2Parameters = "";
						button2Name = "";

						return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
															  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
															  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
					}
					//if leave application is posted/reversed
					if (leaveApplicationStatus.Equals("Posted") || leaveApplicationStatus.Equals("Reversed") || leaveApplicationStatus.Equals("Released"))
					{
						responseHeader = "Staff Requisition Posted";
						responseMessage = "The Staff Requisition No." + StaffRequisitionNo + " is already posted. Editing not allowed.";
						detailedResponseMessage = "The Staff Requisition No." + StaffRequisitionNo + " is already posted. Editing not allowed.";

						button1ControllerName = "StaffRequisition";
						button1ActionName = "StaffRequisitionHistory";
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
					return RedirectToAction("StaffRequisitionHistory", "StaffRequisition");
				}
				else
				{
					responseHeader = "Staff Requisition NotFound";
					responseMessage = "The Staff Requisition No." + StaffRequisitionNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The Staff Requisition No." + StaffRequisitionNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "StaffRequisition";
					button1ActionName = "StaffRequisitionHistory";
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
		public ActionResult EditStaffRequisition(string StaffRequisitionNo)
		{
			try
			{
				if (StaffRequisitionNo.Equals(""))
				{
					return RedirectToAction("StaffRequisitionHistory", "StaffRequisition");
				}
				if (dynamicsNAVSOAPServices.hrManagementWS.CheckStaffRequsitionExists(StaffRequisitionNo, AccountController.GetEmployeeNo()))
				{
					StaffRequisition StaffRequisitionObj = JsonConvert.DeserializeObject<StaffRequisition>(dynamicsNAVSOAPServices.hrManagementWS.GetStaffRequisitionByNo(StaffRequisitionNo));

					LoadEmploymentContracts();
					LoadJobEstablishments();
					LoadRequisitionTypes();
					LoadPositionTypes();

					StaffRequisitionObj.AppointmentTypes = new SelectList(employementContracts, "Code", "Description");
					StaffRequisitionObj.Establishments = new SelectList(establishments, "Code", "Description");
					StaffRequisitionObj.RequisitionTypes = new SelectList(requisitionTypes, "Code", "Description");
					StaffRequisitionObj.PositionTypes = new SelectList(positionTypes, "Code", "Description");

					return View(StaffRequisitionObj);
				}
				else
				{
					responseHeader = "Staff Requisition NotFound";
					responseMessage = "The Staff Requisition No." + StaffRequisitionNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The Staff Requisition No." + StaffRequisitionNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "StaffRequisition";
					button1ActionName = "StaffRequisitionHistory";
					button1HasParameters = false;
					button1Parameters = "";
					button1Name = "Ok";

					button2ControllerName = "";
					button2ActionName = "";
					button2HasParameters = false;
					button2Parameters = "";
					button2Name = "";
					return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage, button1ControllerName, button1ActionName, button1HasParameters,
														  button1Parameters, button1Name, button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

				}
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult> EditStaffRequisition(StaffRequisition StaffRequisitionObj, string Command) 
		{
			try
			{
				LoadEmploymentContracts();
				LoadJobEstablishments();
				LoadRequisitionTypes();
				LoadPositionTypes();

				StaffRequisitionObj.AppointmentTypes = new SelectList(employementContracts, "Code", "Description");
				StaffRequisitionObj.Establishments = new SelectList(establishments, "Code", "Description");
				StaffRequisitionObj.RequisitionTypes = new SelectList(requisitionTypes, "Code", "Description");
				StaffRequisitionObj.PositionTypes = new SelectList(positionTypes, "Code", "Description");

				if (ModelState.IsValid)
				{
					if (StaffRequisitionObj.No.Equals(""))
					{
						return RedirectToAction("StaffRequisitionHistory", "StaffRequisition");
					}

					//DateTime.ParseExact(LeaveApplicationObj.LeaveStartDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture)


					if (Command.Equals("Submit For Approval"))
					{

						bool leaveApplicationModified = dynamicsNAVSOAPServices.hrManagementWS.ModifyStaffRequsition(StaffRequisitionObj.No, StaffRequisitionObj.JobNo, StaffRequisitionObj.PositionType, StaffRequisitionObj.RequisitionType,
																												Convert.ToInt32(StaffRequisitionObj.Positions), DateTime.Parse(StaffRequisitionObj.ExpectedReportingDate),
																												StaffRequisitionObj.AppointmentType, StaffRequisitionObj.Description);

						if (!dynamicsNAVSOAPServices.hrManagementWS.CheckStaffRequsitionApprovalWorkflowEnabled(StaffRequisitionObj.No))
						{
							StaffRequisitionObj.ErrorStatus = true;
							StaffRequisitionObj.ErrorMessage = "No approval record enabled for Employee Requisition.";
							return View(StaffRequisitionObj);
						}

						if (dynamicsNAVSOAPServices.hrManagementWS.SendStaffRequsitionApprovalRequest(StaffRequisitionObj.No))
						{
							responseHeader = "Success";
							responseMessage = "Staff Requisition submitted successfully for approval.";
							detailedResponseMessage = "Staff Requisition submitted successfully for approval.";

							button1ControllerName = "StaffRequisition";
							button1ActionName = "StaffRequisitionHistory";
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

					}
					if (Command.Equals("View Attachment"))
					{
						string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(StaffRequisitionObj.No);

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
							return View(StaffRequisitionObj);
						}
					}
					else
					{
						return View(StaffRequisitionObj);
					}
				}
				else
				{
					return View(StaffRequisitionObj);
				}
			}
			catch (Exception ex)
			{
				StaffRequisitionObj.ErrorStatus = true;
				StaffRequisitionObj.ErrorMessage = ex.Message;
				return View(StaffRequisitionObj);
			}
		}

		#endregion Edit Staff Requisition

		#region View Staff Requisition
		
		[Authorize]
		public ActionResult ViewStaffRequisition(string StaffRequisitionNo) 
		{
			try
			{
				if (StaffRequisitionNo.Equals(""))
				{
					return RedirectToAction("StaffRequisitionHistory", "StaffRequisition"); 
				}
				if (dynamicsNAVSOAPServices.hrManagementWS.CheckStaffRequsitionExists(StaffRequisitionNo, AccountController.GetEmployeeNo()))
				{
					StaffRequisition StaffRequisitionObj = JsonConvert.DeserializeObject<StaffRequisition>(dynamicsNAVSOAPServices.hrManagementWS.GetStaffRequisitionByNo(StaffRequisitionNo));

                    LoadEmploymentContracts();
					LoadJobEstablishments();
					LoadRequisitionTypes();
					LoadPositionTypes();

					StaffRequisitionObj.AppointmentTypes = new SelectList(employementContracts, "Code", "Description");
					StaffRequisitionObj.Establishments = new SelectList(establishments, "Code", "Description");
					StaffRequisitionObj.RequisitionTypes = new SelectList(requisitionTypes, "Code", "Description");
					StaffRequisitionObj.PositionTypes = new SelectList(positionTypes, "Code", "Description");

					return View(StaffRequisitionObj);
				}
				else
				{
					responseHeader = "Staff Requisition NotFound";
					responseMessage = "The Staff Requisition No." + StaffRequisitionNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The Staff Requisition No." + StaffRequisitionNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "StaffRequisition";
					button1ActionName = "StaffRequisitionHistory"; 
					button1HasParameters = false;
					button1Parameters = "";
					button1Name = "Ok";

					button2ControllerName = "";
					button2ActionName = "";
					button2HasParameters = false;
					button2Parameters = "";
					button2Name = "";
					return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage, button1ControllerName, button1ActionName, button1HasParameters,
														  button1Parameters, button1Name, button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);

				}
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> ViewStaffRequisition(StaffRequisition StaffRequisitionObj, string Command)
		{
			try
			{
				if (StaffRequisitionObj.No.Equals(""))
				{
					return RedirectToAction("ViewStaffRequisition", "StaffRequisition");
				}
				if (Command.Equals("View Attachment"))
				{
					string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(StaffRequisitionObj.No);

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
						return View(StaffRequisitionObj);
					}
				}
				else
				{
					StaffRequisitionObj.ErrorStatus = true;
					return View(StaffRequisitionObj);
				}
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		#endregion View Staff Requisition

		#region Staff Requisition History

		[Authorize]
		public ActionResult StaffRequisitionHistory() 
		{
			try
			{
				return View(JsonConvert.DeserializeObject<List<StaffRequisition>>(dynamicsNAVSOAPServices.hrManagementWS.GetStaffRequisitions(employeeNo))); 
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		#endregion End Staff Requisition History

		#region View Staff Requisition

		[Authorize]
		public ActionResult ViewStaffRequisitionApproval(string StaffRequisitionNo)
		{
			try
			{
				if (StaffRequisitionNo.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}

				StaffRequisition StaffRequisitionObj = new StaffRequisition();

				JsonConvert.DeserializeObject<StaffRequisition>(dynamicsNAVSOAPServices.hrManagementWS.GetStaffRequisitionByNo(StaffRequisitionNo));

				LoadEmploymentContracts();
				LoadJobEstablishments();
				LoadRequisitionTypes();
				LoadPositionTypes();

				StaffRequisitionObj.AppointmentTypes = new SelectList(employementContracts, "Code", "Description");
				StaffRequisitionObj.Establishments = new SelectList(establishments, "Code", "Description");
				StaffRequisitionObj.RequisitionTypes = new SelectList(requisitionTypes, "Code", "Description");
				StaffRequisitionObj.PositionTypes = new SelectList(positionTypes, "Code", "Description");

				return View(StaffRequisitionObj);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult> ViewStaffRequisitionApproval(StaffRequisition StaffRequisitionObj, string Command)
		{
			try
			{
				if (StaffRequisitionObj.No.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}
				if (Command == "Approve")
				{
					// test field
					StaffRequisitionObj.RejectionComments = StaffRequisitionObj.RejectionComments != null ? StaffRequisitionObj.RejectionComments : "";
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveStaffRequisition(employeeNo, StaffRequisitionObj.No, AccountController.GetEmployeeNo()))
					{
						responseHeader = "Success";
						responseMessage = "Staff Requistion No." + StaffRequisitionObj.No + " approved successfully.";
						detailedResponseMessage = "Staff Requistion No." + StaffRequisitionObj.No + " approved successfully.";

						button1ControllerName = "Approval";
						button1ActionName = "OpenEntries";
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
						StaffRequisitionObj.ErrorStatus = true;
						StaffRequisitionObj.ErrorMessage = "Unable to process the Staff Requisition approve action. Contact the " + companyName + " for assistance.";
						return View(StaffRequisitionObj);
					}
				}
				else if (Command == "Reject")
				{
					StaffRequisitionObj.RejectionComments = StaffRequisitionObj.RejectionComments != null ? StaffRequisitionObj.RejectionComments : "";
					if (StaffRequisitionObj.RejectionComments.Equals(""))
					{
						StaffRequisitionObj.ErrorStatus = true;
						StaffRequisitionObj.ErrorMessage = "Provide reasons for declining this requisition.";
						return View(JsonConvert.DeserializeObject<StaffRequisition>(dynamicsNAVSOAPServices.hrManagementWS.GetStaffRequisitionByNo(StaffRequisitionObj.No)));
					}

					if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectStaffRequisition(employeeNo, StaffRequisitionObj.No, StaffRequisitionObj.RejectionComments))
					{
						responseHeader = "Success";
						responseMessage = "Staff Requisition rejected successfully.";
						detailedResponseMessage = "Staff Requisition rejected successfully.";

						button1ControllerName = "Approval";
						button1ActionName = "OpenEntries";
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
						StaffRequisitionObj.ErrorStatus = true;
						StaffRequisitionObj.ErrorMessage = "Unable to process the Staff Requisition reject action. Contact the " + companyName + " for assistance.";
						return View(StaffRequisitionObj);
					}
				}
				else if (Command == "View Attachment")
				{
					string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(StaffRequisitionObj.No);

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
						return View(StaffRequisitionObj);
					}
				}

				else
				{
					StaffRequisitionObj.ErrorStatus = true;
					StaffRequisitionObj.ErrorMessage = "Unable to process the approve/reject action. Contact the " + companyName + " for assistance.";
					return View(StaffRequisitionObj);
				}
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		#endregion Staff Requisition Approval

		#region Document Management

		[ChildActionOnly]
		[Authorize]
		public ActionResult _JobDescriptionDocument(string DocumentNo)  
		{
			DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

			return PartialView(documentManagementoBJ);
		}

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewJobDescriptionDocument(string DocumentNo)
		{
			DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

			return PartialView(documentManagementoBJ);
		}

		[Authorize]
		public JsonResult GetJobDescriptionDocuments(string DocumentNo)
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
		public JsonResult UploadDetailedJobDescription(string DocumentNo, string DocumentCode, string DocumentDescription)
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
						dynamicsNAVSOAPServices.documentMgmt.ModifySystemFileURL(DocumentNo, DocumentCode, fileName);

						return Json(new { success = true, message = DocumentDescription + " uploaded successfully" }, JsonRequestBehavior.AllowGet);
					}
					else
					{
						return Json(new { success = false, message = DocumentDescription + " was not uploaded. Try Again." }, JsonRequestBehavior.AllowGet);
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
		public ActionResult GetJobDescriptionDocument(string LineNo, string DocumentNo, string DocumentCode)
		{
			try
			{
				DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

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

        #region Chapter 6 Requirements
        [ChildActionOnly]
        [Authorize]
        public ActionResult _AddChapter6Requirement(string DocumentNo)
        {
            Chapter6Requirments chapter6Requirments = new Chapter6Requirments();
            LoadChapter6Requiremenst();
            chapter6Requirments.Codes = new SelectList(chapter6Requirements, "Code", "Description5");
            return PartialView(chapter6Requirments);
        }

        [Authorize]
        public JsonResult CreateChapter6Requirement(Chapter6Requirments chapter6Requirments)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.CreateJobChapter6Requirements(chapter6Requirments.DocumentNo, chapter6Requirments.Code);

            if (entryCreated)
            {
                return Json(new { success = true, message = "Entry created successfully." }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { success = false, message = "Failed to create chapter 6 attachment" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult ModifyChapter6Requirement(Chapter6Requirments chapter6Requirments)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.ModifyJobChapter6Requirements(chapter6Requirments.LineNo, chapter6Requirments.DocumentNo, chapter6Requirments.Code);
            if (entryCreated)
            {
                return Json(new { success = true, message = "chapter6Requirments" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize]
        public JsonResult DeleteChapter6Requirement(int LineNo, string DocumentNo)
        {
            bool entryDeleted = false;

            entryDeleted = dynamicsNAVSOAPServices.hrManagementWS.DeleteJobChapter6Requirements(LineNo, DocumentNo);

            return Json(new { EntryDeleted = entryDeleted }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetJobChapter6Requirements(string DocumentNo)
        {
            List<Chapter6Requirments> chapter6RequirementList = new List<Chapter6Requirments>();

            var chapter6Requirements = from _chapter6Requirement in dynamicsNAVODataServices.dynamicsNAVOData.JobChapter6Requirements
                                       where _chapter6Requirement.Document_No.Equals(DocumentNo)
                                       select _chapter6Requirement;

            foreach (var chapter6Requirement in chapter6Requirements)
            {
                Chapter6Requirments chapter6RequirementObj = new Chapter6Requirments();
                chapter6RequirementObj.LineNo = chapter6Requirement.Line_No;
                chapter6RequirementObj.DocumentNo = chapter6Requirement.Document_No;
                chapter6RequirementObj.Code = chapter6Requirement.Code;
                chapter6RequirementObj.Description5 = chapter6Requirement.Description;

                chapter6RequirementList.Add(chapter6RequirementObj);
            }
            return Json(chapter6RequirementList.ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion End 

        #region Job Academic Qualifications

        [ChildActionOnly]
        [Authorize]
        public ActionResult _AddJobAcademicQualifications(string DocumentNo)
        {
            AcademicRequirements academicRequirements = new AcademicRequirements();

            LoadAcademicEducationLevels();
            academicRequirements.AcademicLevels = new SelectList(academicRequirementList, "Code", "Description1");

            return PartialView(academicRequirements);
        }

        [Authorize]
        public JsonResult CreateJobAcademicQualification(AcademicRequirements academicRequirements)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.CreateJobAcademicRequirement(academicRequirements.DocumentNo, Convert.ToInt32(academicRequirements.Code), academicRequirements.Description1);

            if (entryCreated)
            {
                return Json(new { success = true, message = "Entry created successfully." }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { success = false, message = "Failed to create chapter 6 attachment" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult ModifyJobAcademicQualification(AcademicRequirements academicRequirements) 
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.ModifyJobAcademicRequirement(academicRequirements.LineNo, academicRequirements.DocumentNo, Convert.ToInt32(academicRequirements.Code), academicRequirements.Description1);
            if (entryCreated)
            {
                return Json(new { success = true, message = "chapter6Requirments" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult DeleteJobAcademicQualification(int LineNo, string DocumentNo) 
        {
            bool entryDeleted = false;

            entryDeleted = dynamicsNAVSOAPServices.hrManagementWS.DeleteJobAcademicRequirement(LineNo, DocumentNo);

            return Json(new { EntryDeleted = entryDeleted }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetJobAcademicRequirements(string DocumentNo)
        {
            List<AcademicRequirements> academicRequirementList = new List<AcademicRequirements>();

            var jAcademicQualifications = from jAcademicQualificationCode in dynamicsNAVODataServices.dynamicsNAVOData.JobAcademicRequirements
                                          where jAcademicQualificationCode.Document_No.Equals(DocumentNo)
                                          select jAcademicQualificationCode;

            foreach (var jAcademicQualification in jAcademicQualifications)
            {
                AcademicRequirements academicRequirementsObj = new AcademicRequirements();
                academicRequirementsObj.LineNo = jAcademicQualification.Line_No;
                academicRequirementsObj.DocumentNo = jAcademicQualification.Document_No;
                academicRequirementsObj.Code = jAcademicQualification.Code.ToString();
                academicRequirementsObj.Description1 = jAcademicQualification.Description;

                academicRequirementList.Add(academicRequirementsObj);
            }
            return Json(academicRequirementList.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Professional Qualification

        [ChildActionOnly]
        [Authorize]
        public ActionResult _AddJobProfessionalQualifications()
        {
            ProfessionalQualifications professionalQualifications = new ProfessionalQualifications();

            LoadProfessionalQualifications();
            professionalQualifications.ProfessionalLevels = new SelectList(professionalQualificationList,"Code","Description2");

            return PartialView(professionalQualifications);
        }

        [Authorize]
        public JsonResult CreateJobProfessionalQualification(ProfessionalQualifications professionalQualifications)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.CreateJobQualificationRequirement(professionalQualifications.DocumentNo, Convert.ToInt32(professionalQualifications.Code),professionalQualifications.Description2);

            if (entryCreated)
            {
                return Json(new { success = true, message = "Entry created successfully." }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { success = false, message = "Failed to create chapter 6 attachment" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult ModifyJobProfessionalQualification(ProfessionalQualifications professionalQualifications)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.ModifyJobQualificationRequirement(professionalQualifications.LineNo, professionalQualifications.DocumentNo, Convert.ToInt32(professionalQualifications.Code), professionalQualifications.Description2);
            if (entryCreated)
            {
                return Json(new { success = true, message = "chapter6Requirments" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult DeleteJobProfessionalQualification(int LineNo, string DocumentNo)
        {
            bool entryDeleted = false;

            entryDeleted = dynamicsNAVSOAPServices.hrManagementWS.DeleteJobProfessionalBodiesReq(LineNo, DocumentNo);

            return Json(new { EntryDeleted = entryDeleted }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetJobProfessionalQualifications(string DocumentNo)
        {
            List<ProfessionalQualifications> professionalQualificationList = new List<ProfessionalQualifications>();

            var professionalQualificationRequirements = from qualificationRequirement in dynamicsNAVODataServices.dynamicsNAVOData.JobQualificationRequirements
                                                        where qualificationRequirement.Document_No.Equals(DocumentNo)
                                                        select qualificationRequirement;

            foreach (var professionalQualificationRequirement in professionalQualificationRequirements)
            {
                ProfessionalQualifications professionalQualificationReqObj = new ProfessionalQualifications();
                professionalQualificationReqObj.LineNo = professionalQualificationRequirement.Line_No;
                professionalQualificationReqObj.DocumentNo = professionalQualificationRequirement.Document_No;
                professionalQualificationReqObj.Code = professionalQualificationRequirement.Code.ToString();
                professionalQualificationReqObj.Description2 = professionalQualificationRequirement.Description;

                professionalQualificationList.Add(professionalQualificationReqObj);
            }
            return Json(professionalQualificationList.ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion End 

        #region Professional Bodies

        [ChildActionOnly]
        [Authorize]
        public ActionResult _AddJobProfessionalBodies(string DocumentNo)
        {
            ProfessionalBodies professionalBodies = new ProfessionalBodies();
            return PartialView(professionalBodies);
        }

        [Authorize]
        public JsonResult CreateJobProfessionalBody(ProfessionalBodies professionalBodies)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.CreateJobProfessionalBodiesReq(professionalBodies.DocumentNo, professionalBodies.Description3);

            if (entryCreated)
            {
                return Json(new { success = true, message = "Entry created successfully." }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { success = false, message = "Failed to create chapter 6 attachment" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult ModifyJobProfessionalBody(ProfessionalBodies professionalBodies)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.ModifyJobProfessionalBodiesReq(professionalBodies.LineNo, professionalBodies.DocumentNo, professionalBodies.Description3);
            if (entryCreated)
            {
                return Json(new { success = true, message = "chapter6Requirments" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult DeleteJobProfessionalBody(int LineNo, string DocumentNo)
        {
            bool entryDeleted = false;

            entryDeleted = dynamicsNAVSOAPServices.hrManagementWS.DeleteJobProfessionalBodiesReq(LineNo, DocumentNo);

            return Json(new { EntryDeleted = entryDeleted }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetJobProfessionalBodiesRequirements(string DocumentNo)
        {
            List<ProfessionalBodies> professionalBodiesRequirementList = new List<ProfessionalBodies>();

            var professionalBodiesRequirements = from professionalBodiesRequirementCode in dynamicsNAVODataServices.dynamicsNAVOData.JobProfessionalBodiesRequirements
                                                 where professionalBodiesRequirementCode.Document_No.Equals(DocumentNo)
                                                 select professionalBodiesRequirementCode;

            foreach (var professionalBodiesRequirement in professionalBodiesRequirements)
            {
                ProfessionalBodies professionalBodiesRequirementObj = new ProfessionalBodies();
                professionalBodiesRequirementObj.LineNo = professionalBodiesRequirement.Line_No;
                professionalBodiesRequirementObj.DocumentNo = professionalBodiesRequirement.Document_No;
                professionalBodiesRequirementObj.Description3 = professionalBodiesRequirement.Description;

                professionalBodiesRequirementList.Add(professionalBodiesRequirementObj);
            }
            return Json(professionalBodiesRequirementList.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion End

        #region Job Responsibilities

        [ChildActionOnly]
        [Authorize]
        public ActionResult _AddJobResponsibilities(string DocumentNo)
        {
            JobResponsibilities jobResponsibilities = new JobResponsibilities();
            return PartialView(jobResponsibilities);
        }

        [Authorize]
        public JsonResult CreateJobRequirementResponsibility(JobResponsibilities jobResponsibilities)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.CreateJobRequirementResponsibility(jobResponsibilities.DocumentNo, jobResponsibilities.Description7);

            if (entryCreated)
            {
                return Json(new { success = true, message = "Entry created successfully." }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { success = false, message = "Failed to create chapter 6 attachment" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult ModifyJobRequirementResponsibility(JobResponsibilities jobResponsibilities)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.ModifyJobRequirementResponsibility(jobResponsibilities.LineNo, jobResponsibilities.DocumentNo, jobResponsibilities.Description7);
            if (entryCreated)
            {
                return Json(new { success = true, message = "chapter6Requirments" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult DeleteJobRequirementResponsibility(int LineNo, string DocumentNo)
        {
            bool entryDeleted = false;

            entryDeleted = dynamicsNAVSOAPServices.hrManagementWS.DeleteJobRequirementResponsibility(LineNo, DocumentNo);

            return Json(new { EntryDeleted = entryDeleted }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetJobResponsibilities(string DocumentNo)
        {
            List<JobResponsibilities> jobResponsibilitiesList = new List<JobResponsibilities>();

            var jobResponsibilities = from _jobResponsibility in dynamicsNAVODataServices.dynamicsNAVOData.JobRequirementResponsibilities
                                      where _jobResponsibility.Document_No.Equals(DocumentNo)
                                      select _jobResponsibility;

            foreach (var jobResponsibility in jobResponsibilities)
            {
                JobResponsibilities jobResponsibilitiesObj = new JobResponsibilities();
                jobResponsibilitiesObj.LineNo = jobResponsibility.Line_No;
                jobResponsibilitiesObj.DocumentNo = jobResponsibility.Document_No;
                jobResponsibilitiesObj.Description7 = jobResponsibility.Description;

                jobResponsibilitiesList.Add(jobResponsibilitiesObj);
            }
            return Json(jobResponsibilitiesList.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion End 

        #region Experience

        [ChildActionOnly]
        [Authorize]
        public ActionResult _JobExperience(string DocumentNo)
        {
            JobExperience jobExperience = new JobExperience();
            return PartialView(jobExperience);
        }

        [Authorize]
        public JsonResult CreateJobExperienceRequirement(JobExperience jobExperience)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.CreateJobExperienceRequirement(jobExperience.DocumentNo, jobExperience.Years, jobExperience.Description4);

            if (entryCreated)
            {
                return Json(new { success = true, message = "Entry created successfully." }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { success = false, message = "Failed to create chapter 6 attachment" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult ModifyJobExperienceRequirement(JobExperience jobExperience)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.ModifyJobExperienceRequirement(jobExperience.LineNo, jobExperience.DocumentNo, jobExperience.Years, jobExperience.Description4);
            if (entryCreated)
            {
                return Json(new { success = true, message = "chapter6Requirments" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult DeleteJobExperienceRequirement(int LineNo, string DocumentNo)
        {
            bool entryDeleted = false;

            entryDeleted = dynamicsNAVSOAPServices.hrManagementWS.DeleteJobExperienceRequirement(LineNo, DocumentNo);

            return Json(new { EntryDeleted = entryDeleted }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetJobExperienceRequirements(string DocumentNo)
        {
            List<JobExperience> jobExperienceList = new List<JobExperience>();

            var jobExperiences = from _jobExperience in dynamicsNAVODataServices.dynamicsNAVOData.JobExperienceRequirements
                                 where _jobExperience.Document_No.Equals(DocumentNo)
                                 select _jobExperience;

            foreach (var jobExperience in jobExperiences)
            {
                JobExperience jobExperienceObj = new JobExperience();
                jobExperienceObj.LineNo = jobExperience.Line_No;
                jobExperienceObj.DocumentNo = jobExperience.Document_No;
                jobExperienceObj.Description4 = jobExperience.Description;
                jobExperienceObj.Years = jobExperience.Years ?? 0;

                jobExperienceList.Add(jobExperienceObj);
            }
            return Json(jobExperienceList.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion End

        #region Other

        [ChildActionOnly]
        [Authorize]
        public ActionResult _AddOtherRequirements(string DocumentNo)
        {
            OtherRequirements otherRequirements = new OtherRequirements();
            return PartialView(otherRequirements);
        }

        [Authorize]
        public JsonResult CreateJobOtherRequirement(OtherRequirements otherRequirements)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.CreateJobOtherRequirement(otherRequirements.DocumentNo, otherRequirements.Description6);

            if (entryCreated)
            {
                return Json(new { success = true, message = "Entry created successfully." }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { success = false, message = "Failed to create chapter 6 attachment" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult ModifyJobOtherRequirement(OtherRequirements otherRequirements)
        {
            bool entryCreated = false;

            entryCreated = dynamicsNAVSOAPServices.hrManagementWS.ModifyJobOtherRequirement(otherRequirements.LineNo, otherRequirements.DocumentNo, otherRequirements.Description6);
            if (entryCreated)
            {
                return Json(new { success = true, message = "chapter6Requirments" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult DeleteJobOtherRequirement(int LineNo, string DocumentNo) 
        {
            bool entryDeleted = false;

            entryDeleted = dynamicsNAVSOAPServices.hrManagementWS.DeleteJobOtherRequirement(LineNo, DocumentNo);

            return Json(new { EntryDeleted = entryDeleted }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetOtherRequirements(string DocumentNo) 
        {
            List<OtherRequirements> otherRequirementsList = new List<OtherRequirements>();

            var otherRequirements = from _otherRequirement in dynamicsNAVODataServices.dynamicsNAVOData.JobOtherRequirement
                                 where _otherRequirement.Document_No.Equals(DocumentNo)
                                 select _otherRequirement;

            foreach (var otherRequirement in otherRequirements)
            {
                OtherRequirements otherRequirementsObj = new OtherRequirements();
                otherRequirementsObj.LineNo = otherRequirement.Line_No;
                otherRequirementsObj.DocumentNo = otherRequirement.Document_No;
                otherRequirementsObj.Description6 = otherRequirement.Description;

                otherRequirementsList.Add(otherRequirementsObj);
            }
            return Json(otherRequirementsList.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion End Other

        #region Helper Functions

        private void LoadRequisitionTypes()
		{
			requisitionTypes = new List<RequisitionType> { 
				               new RequisitionType { Code = "Internal", Description = "Internal" }, 
				               new RequisitionType { Code = "Open", Description = "Open" } 
			                   };

		}

		private void LoadPositionTypes()
		{
			positionTypes = new List<PositionType> {
							   new PositionType { Code = "New Position", Description = "New Position" },
							   new PositionType { Code = "Existing Position", Description = "Existing Position" } 
							   };

		}

		private void LoadJobEstablishments() 
		{
			establishments= JsonConvert.DeserializeObject<List<Establishment>>(dynamicsNAVSOAPServices.hrManagementWS.GetJobEstablishments());
		}

		private void LoadEmploymentContracts()  
		{
			employementContracts = new List<EmploymentContract>();
			 
			dynamic employmentContractCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetEmploymentContracts()); 
			foreach (var employmentContractCode in employmentContractCodes)
			{
				EmploymentContract employmentContractObj = new EmploymentContract();
				employmentContractObj.Code = employmentContractCode.Code;
				employmentContractObj.Description = employmentContractCode.Description;

				employementContracts.Add(employmentContractObj);
			}
		}

        private void LoadChapter6Requiremenst()
        {
            chapter6Requirements = JsonConvert.DeserializeObject<List<Chapter6Requirments>>(dynamicsNAVSOAPServices.hrManagementWS.GetChapter6Requirements());
        }

        private void LoadAcademicEducationLevels()
        {
            academicRequirementList = JsonConvert.DeserializeObject<List<AcademicRequirements>>(dynamicsNAVSOAPServices.hrManagementWS.GetAcademicEducationLevels());
        }

        private void LoadProfessionalQualifications()
        {
            professionalQualificationList = JsonConvert.DeserializeObject<List<ProfessionalQualifications>>(dynamicsNAVSOAPServices.hrManagementWS.GetProfessionalLevels());
        }
        #endregion End Helper Functions
    }
}