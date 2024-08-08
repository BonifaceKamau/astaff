using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.HumanResource.HumanResourceHome;
using DynamicsNAV365_StaffPortal.Models.HumanResource.PerformanceTargetModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    public class PerformanceTargetController : Controller
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

		string employeeNo = "";

		AccountController accountController = new AccountController();
		public PerformanceTargetController()
        {
			employeeNo = AccountController.GetEmployeeNo();
		}

		#region Global Appraisal Perspectives

		[Authorize]
		[ChildActionOnly]
		public ActionResult _ViewGlobalAppraisalPerspectives()
        {
			GlobalAppraisalPerspectiveModel globalAppraisalPerspectiveObj = new GlobalAppraisalPerspectiveModel();
			return PartialView(globalAppraisalPerspectiveObj);
        }

		[Authorize]
		public JsonResult GetGlobalAppraisalPerspectives()
        {
			List<GlobalAppraisalPerspectiveModel> globalAppraisalObjectiveList = new List<GlobalAppraisalPerspectiveModel>();
			dynamic globalAppraisalPerspectives = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetGlobalAppraisalPerspectives());

            foreach (var globalAppraisalPerspective in globalAppraisalPerspectives)
            {
				GlobalAppraisalPerspectiveModel globalAppraisalPerspectiveObj = new GlobalAppraisalPerspectiveModel();
				globalAppraisalPerspectiveObj.PerspectiveType = globalAppraisalPerspective.PerspectiveType;
				globalAppraisalPerspectiveObj.Code = globalAppraisalPerspective.Code;
				globalAppraisalPerspectiveObj.Description = globalAppraisalPerspective.Description;

				globalAppraisalObjectiveList.Add(globalAppraisalPerspectiveObj);
			}
			return Json(globalAppraisalObjectiveList.ToList(), JsonRequestBehavior.AllowGet);
        }
		#endregion End Global Appraisal Perspectives

		#region Business Values & Competencies

		[Authorize]
		[ChildActionOnly]
		public ActionResult _ViewBusinessCoreValuesAndCompetencies()
		{
			BusinessCoreValuesAndCompetenciesModel businessCoreValuesAndCompetenciesObj = new BusinessCoreValuesAndCompetenciesModel();
			return PartialView(businessCoreValuesAndCompetenciesObj);
		}

		[Authorize]
		public JsonResult GetBusinessCoreValuesAndCompetencies() 
		{
			List<BusinessCoreValuesAndCompetenciesModel> businessCoreValuesAndCompetenciesList = new List<BusinessCoreValuesAndCompetenciesModel>();
			dynamic businessCoreValuesAndCompetencies = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetBusinessCoreValueCompetencies());

			foreach (var businessCoreValuesAndCompetency in businessCoreValuesAndCompetencies)
			{
				BusinessCoreValuesAndCompetenciesModel businessCoreValuesAndCompetenciesObj = new BusinessCoreValuesAndCompetenciesModel();
				businessCoreValuesAndCompetenciesObj.Category = businessCoreValuesAndCompetency.Category;
				businessCoreValuesAndCompetenciesObj.Code = businessCoreValuesAndCompetency.Code;
				businessCoreValuesAndCompetenciesObj.Description = businessCoreValuesAndCompetency.Description;
				businessCoreValuesAndCompetenciesObj.Description2 = businessCoreValuesAndCompetency.Description2;

				businessCoreValuesAndCompetenciesList.Add(businessCoreValuesAndCompetenciesObj);
			}
			return Json(businessCoreValuesAndCompetenciesList.ToList(), JsonRequestBehavior.AllowGet);
		}
		#endregion End Business Values & Competencies

		#region Get Job Performance Targets

		[Authorize]
		[ChildActionOnly]
		public ActionResult _ViewJobPerformanceTarget()
        {
			JobPerformanceTargetModel jobPerformanceTargetObj = new JobPerformanceTargetModel();
			return PartialView(jobPerformanceTargetObj);
		}

		[Authorize]
		public JsonResult GetJobPerformanceTargets()  
		{
			List<JobPerformanceTargetModel> jobPerformanceTargetList = new List<JobPerformanceTargetModel>();

			dynamic jobPerformanceTargets = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetJobPerformanceTargets(employeeNo));

			foreach (var jobPerformanceTarget in jobPerformanceTargets)
			{
				JobPerformanceTargetModel jobPerformanceTargetObj = new JobPerformanceTargetModel();
				jobPerformanceTargetObj.AppraisalPeriod = jobPerformanceTarget.AppraisalPeriod;
				jobPerformanceTargetObj.ObjectiveCode = jobPerformanceTarget.ObjectiveCode;
				jobPerformanceTargetObj.ObjectiveDescription = jobPerformanceTarget.ObjectiveDescription;
				jobPerformanceTargetObj.PerspectiveType = jobPerformanceTarget.PerspectiveType;
				jobPerformanceTargetObj.DepartmentName = jobPerformanceTarget.DepartmentName;
				jobPerformanceTargetObj.PerspectiveDescription = jobPerformanceTarget.PerspectiveDescription;

				jobPerformanceTargetList.Add(jobPerformanceTargetObj);
			}

			return Json(jobPerformanceTargetList.ToList(), JsonRequestBehavior.AllowGet);
		}

		#endregion End Get Job  Performance Targets

		#region Employee Performance Target history

		[Authorize]
		public ActionResult EmployeePerformanceTargetHistory()
		{
			try
			{
				List<EmployeePerformanceTargetHeaderModel> performanceTargetList = new List<EmployeePerformanceTargetHeaderModel>();

				dynamic performanceTargets = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetEmployeePerformanceTargets("", employeeNo));

				foreach (var performanceTarget in performanceTargets)
				{
					EmployeePerformanceTargetHeaderModel performanceTargetObj = new EmployeePerformanceTargetHeaderModel();
					performanceTargetObj.No = performanceTarget.No;
					performanceTargetObj.EmployeeNo = performanceTarget.EmployeeNo;
					performanceTargetObj.EmployeeName = performanceTarget.EmployeeName;
					performanceTargetObj.SupervisorNo = performanceTarget.SupervisorNo;
					performanceTargetObj.SupervisorName = performanceTarget.SupervisorName;
					performanceTargetObj.SupervisorUserID = performanceTarget.SupervisorUserID;
					performanceTargetObj.AppraisalType = performanceTarget.AppraisalType;
					performanceTargetObj.AppraisalStage = performanceTarget.AppraisalStage;
					performanceTargetObj.AppraisalPeriod = performanceTarget.AppraisalPeriod;
					performanceTargetObj.UserID = performanceTarget.UserID;
					performanceTargetObj.DateofFirstAppointment = performanceTarget.DateofFirstAppointment;
					performanceTargetObj.Designation = performanceTarget.Designation;
					performanceTargetObj.DepartmentCode = performanceTarget.DepartmentCode;
					performanceTargetObj.DepartmentName = performanceTarget.DepartmentName;
					performanceTargetObj.CommentsAppraisee = performanceTarget.CommentsAppraisee;
					performanceTargetObj.DocumentDate = performanceTarget.DocumentDate;
					performanceTargetObj.EvaluationPeriodStartDate = performanceTarget.EvaluationPeriodStartDate;
					performanceTargetObj.EvaluationPeriodEndDate = performanceTarget.EvaluationPeriodEndDate;
					performanceTargetObj.TargetType = performanceTarget.TargetType;
					performanceTargetObj.FinalScores = performanceTarget.FinalScores;
					performanceTargetObj.TotalScores = performanceTarget.TotalScores;
					performanceTargetObj.RatingRemarks = performanceTarget.RatingRemarks;
					performanceTargetObj.Email = performanceTarget.Email;
					performanceTargetObj.AppraisalStatus = performanceTarget.AppraisalStatus;

					performanceTargetList.Add(performanceTargetObj);
				}
				return View(performanceTargetList);
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		#endregion End Employee Performance Target history

		#region New Employee Performance Target

		[Authorize]
		public ActionResult NewEmployeePerformanceTarget()
        {
			string AppraisalNo = "";

			try
			{
				//Check Open Performance Target
				AppraisalNo = dynamicsNAVSOAPServices.hrManagementWS.CheckOpenPerformanceTargetExists(employeeNo);

				if (!AppraisalNo.Equals(""))
                {
					responseHeader = "Open Performance Target Exists";
					responseMessage = "You are not allowed to create a new performance target until you submit the existing one. The unsubmitted performance target No. "+AppraisalNo+". ";
					detailedResponseMessage = "You are not allowed to create a new performance target until you submit the existing one. The unsubmitted performance target No. " + AppraisalNo + ". ";

					button1ControllerName = "PerformanceTarget";
					button1ActionName = "EmployeePerformanceTargetHistory";
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

				//Create Performance Target
				AppraisalNo = dynamicsNAVSOAPServices.hrManagementWS.CreateEmployeePerformanceTarget(employeeNo);

				if (dynamicsNAVSOAPServices.hrManagementWS.CheckPerformanceTargetExists(AppraisalNo, AccountController.GetEmployeeNo()))
				{
					EmployeePerformanceTargetHeaderModel performanceTargetObj = new EmployeePerformanceTargetHeaderModel();
					dynamic performanceTarges = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetEmployeePerformanceTargets(AppraisalNo, ""));
					foreach (var performanceTarget in performanceTarges)
					{
						performanceTargetObj.No = performanceTarget.No;
						performanceTargetObj.EmployeeNo = performanceTarget.EmployeeNo;
						performanceTargetObj.EmployeeName = performanceTarget.EmployeeName;
						performanceTargetObj.SupervisorNo = performanceTarget.SupervisorNo;
						performanceTargetObj.SupervisorName = performanceTarget.SupervisorName;
						performanceTargetObj.SupervisorUserID = performanceTarget.SupervisorUserID;
						performanceTargetObj.AppraisalType = performanceTarget.AppraisalType;
						performanceTargetObj.AppraisalStage = performanceTarget.AppraisalStage;
						performanceTargetObj.AppraisalPeriod = performanceTarget.AppraisalPeriod;
						performanceTargetObj.UserID = performanceTarget.UserID;
						performanceTargetObj.DateofFirstAppointment = performanceTarget.DateofFirstAppointment;
						performanceTargetObj.Designation = performanceTarget.Designation;
						performanceTargetObj.DepartmentCode = performanceTarget.DepartmentCode;
						performanceTargetObj.DepartmentName = performanceTarget.DepartmentName;
						performanceTargetObj.CommentsAppraisee = performanceTarget.CommentsAppraisee;
						performanceTargetObj.DocumentDate = performanceTarget.DocumentDate;
						performanceTargetObj.EvaluationPeriodStartDate = performanceTarget.EvaluationPeriodStartDate;
						performanceTargetObj.EvaluationPeriodEndDate = performanceTarget.EvaluationPeriodEndDate;
						performanceTargetObj.TargetType = performanceTarget.TargetType;
						performanceTargetObj.FinalScores = performanceTarget.FinalScores;
						performanceTargetObj.TotalScores = performanceTarget.TotalScores;
						performanceTargetObj.RatingRemarks = performanceTarget.RatingRemarks;
						performanceTargetObj.Email = performanceTarget.Email;
						performanceTargetObj.AppraisalStatus = performanceTarget.AppraisalStatus;

					}
					return View(performanceTargetObj);
				}
				else
				{
					responseHeader = "Performance Targets NotFound";
					responseMessage = "The Performance Target No." + AppraisalNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The Performance Target No." + AppraisalNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "PerformanceTarget";
					button1ActionName = "EmployeePerformanceTargetHistory";
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

		#endregion End New Employee Performance Target

		#region View Employee Performance Target

		[Authorize]
		public ActionResult ViewEmployeePerformanceTarget(string AppraisalNo) 
		{
			try
			{
				if (AppraisalNo.Equals(""))
				{
					return RedirectToAction("EmployeePerformanceTargetHistory", "PerformanceTarget"); 
				}
				if (dynamicsNAVSOAPServices.hrManagementWS.CheckPerformanceTargetExists(AppraisalNo, AccountController.GetEmployeeNo()))
				{
					EmployeePerformanceTargetHeaderModel performanceTargetObj = new EmployeePerformanceTargetHeaderModel();
					dynamic performanceTarges = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetEmployeePerformanceTargets(AppraisalNo, ""));
					foreach (var performanceTarget in performanceTarges) 
					{
						performanceTargetObj.No = performanceTarget.No;
						performanceTargetObj.EmployeeNo = performanceTarget.EmployeeNo;
						performanceTargetObj.EmployeeName = performanceTarget.EmployeeName;
						performanceTargetObj.SupervisorNo = performanceTarget.SupervisorNo;
						performanceTargetObj.SupervisorName = performanceTarget.SupervisorName;
						performanceTargetObj.SupervisorUserID = performanceTarget.SupervisorUserID;
						performanceTargetObj.AppraisalType = performanceTarget.AppraisalType;
						performanceTargetObj.AppraisalStage = performanceTarget.AppraisalStage;
						performanceTargetObj.AppraisalPeriod = performanceTarget.AppraisalPeriod;
						performanceTargetObj.UserID = performanceTarget.UserID;
						performanceTargetObj.DateofFirstAppointment = performanceTarget.DateofFirstAppointment;
						performanceTargetObj.Designation = performanceTarget.Designation;
						performanceTargetObj.DepartmentCode = performanceTarget.DepartmentCode;
						performanceTargetObj.DepartmentName = performanceTarget.DepartmentName;
						performanceTargetObj.CommentsAppraisee = performanceTarget.CommentsAppraisee;
						performanceTargetObj.DocumentDate = performanceTarget.DocumentDate;
						performanceTargetObj.EvaluationPeriodStartDate = performanceTarget.EvaluationPeriodStartDate;
						performanceTargetObj.EvaluationPeriodEndDate = performanceTarget.EvaluationPeriodEndDate;
						performanceTargetObj.TargetType = performanceTarget.TargetType;
						performanceTargetObj.FinalScores = performanceTarget.FinalScores;
						performanceTargetObj.TotalScores = performanceTarget.TotalScores;
						performanceTargetObj.RatingRemarks = performanceTarget.RatingRemarks;
						performanceTargetObj.Email = performanceTarget.Email;
						performanceTargetObj.AppraisalStatus = performanceTarget.AppraisalStatus;

					}
					return View(performanceTargetObj);
				}
				else
				{
					responseHeader = "Performance Targets NotFound";
					responseMessage = "The Performance Target No." + AppraisalNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The Performance Target No." + AppraisalNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "PerformanceTarget";
					button1ActionName = "EmployeePerformanceTargetHistory";
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
		public async Task<ActionResult> ViewEmployeePerformanceTarget(EmployeePerformanceTargetHeaderModel performanceTargetObj, string Command)
		{
			try
			{
				if (performanceTargetObj.No.Equals(""))
				{
					return RedirectToAction("EmployeePerformanceTargetHistory", "PerformanceTarget");
				}
				if (Command == "Submit")
				{
					if (dynamicsNAVSOAPServices.hrManagementWS.SubmitTargetsForApproval(performanceTargetObj.No))
					{
						responseHeader = "Success";
						responseMessage = "Performance targets successfully submitted for approval";
						detailedResponseMessage = "Performance targets successfully submitted for approval";

						button1ControllerName = "PerformanceTarget";
						button1ActionName = "EmployeePerformanceTargetHistory";
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
						performanceTargetObj.ErrorStatus = false;
						performanceTargetObj.ErrorMessage = "Unable submit evaluation to appraiser.";
						return View(performanceTargetObj);
					}
				}
				else if (Command == "Backward")
				{
					//Calculate Moderated Appraisal Score
					if (dynamicsNAVSOAPServices.hrManagementWS.ReturnEvaluationToAppraisee(performanceTargetObj.No))
					{
						responseHeader = "Success";
						responseMessage = "Returned to appraisee successfully.";
						detailedResponseMessage = "Returned to appraisee successfully.";

						button1ControllerName = "ModeratedAppraisal";
						button1ActionName = "ModeratedAppraisalHistory";
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
						performanceTargetObj.ErrorStatus = false;
						performanceTargetObj.ErrorMessage = "You cannot return evaluation to appraisee because you have already approved.";
						return View(performanceTargetObj);
					}
				}
				else if (Command == "Calculate")
				{
					//Calculate Moderated Appraisal Score
					if (dynamicsNAVSOAPServices.hrManagementWS.CalculateWeights(performanceTargetObj.No))
					{
						performanceTargetObj.ErrorStatus = false;
						performanceTargetObj.ErrorMessage = "";
						return View(performanceTargetObj);
					}

					else
					{
						performanceTargetObj.ErrorStatus = false;
						performanceTargetObj.ErrorMessage = "Unable to calculate weights. Please try again...";
						return View(performanceTargetObj);
					}
				}

				else if (Command == "Report")
				{
					string fileURL = dynamicsNAVSOAPServices.hrManagementWS.GenerateEvaluationReport(performanceTargetObj.No);

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
						return View(performanceTargetObj);
					}
				}

				else
				{
					performanceTargetObj.ErrorStatus = false;
					performanceTargetObj.ErrorMessage = "Unable to calculate weights action. Please try again...";
					return View(performanceTargetObj);
				}

			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		#endregion End View Employee Performance Target

		#region Employee Performance Target Lines

		[Authorize]
		[ChildActionOnly]
		public ActionResult _EmployeePerformanceTargetLine(string HeaderNo)
        {
			EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj = new EmployeePerformanceTargetLineModel();
			return PartialView(employeePerformanceTargetLineObj);
        }

		[Authorize]
		[ChildActionOnly]
		public ActionResult _ViewEmployeePerformanceTargetLine(string HeaderNo) 
		{
			EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj = new EmployeePerformanceTargetLineModel();
			return PartialView(employeePerformanceTargetLineObj);
		}

		[Authorize] 
		public JsonResult GetEmployeePerformanceTargetLines(string HeaderNo)
        {
			List<EmployeePerformanceTargetLineModel> employeePerformanceTargetList = new List<EmployeePerformanceTargetLineModel>();
			dynamic employeePerformanceTargetLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetEmployeePerformanceTargetsLines(HeaderNo));
			foreach (var employeePerformanceTargetLine in employeePerformanceTargetLines)
            {
				EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj = new EmployeePerformanceTargetLineModel();

				employeePerformanceTargetLineObj.LineNo = employeePerformanceTargetLine.LineNo;
				employeePerformanceTargetLineObj.AppraisalNo = employeePerformanceTargetLine.AppraisalNo;
				employeePerformanceTargetLineObj.AgreedPerformanceTargets = employeePerformanceTargetLine.AgreedPerformanceTargets;
				employeePerformanceTargetLineObj.AgreedScore = employeePerformanceTargetLine.AgreedScore;
				employeePerformanceTargetLineObj.KeyPerformanceIndicator = employeePerformanceTargetLine.KeyPerformanceIndicator;
				employeePerformanceTargetLineObj.KeyResultAreasOutput = employeePerformanceTargetLine.KeyResultAreasOutput;

				employeePerformanceTargetList.Add(employeePerformanceTargetLineObj);
			}
			return Json(employeePerformanceTargetList.ToList(), JsonRequestBehavior.AllowGet);
        }

		[Authorize]
		public JsonResult GetEmployeePerformanceTargetByLineNo(int LineNo, string HeaderNo) 
		{
			EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj = new EmployeePerformanceTargetLineModel();
			dynamic employeePerformanceTargetLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetEmployeePerformanceTargetsByLineNo(LineNo,HeaderNo)); 
			foreach (var employeePerformanceTargetLine in employeePerformanceTargetLines)
			{
				employeePerformanceTargetLineObj.LineNo = employeePerformanceTargetLine.LineNo;
				employeePerformanceTargetLineObj.AppraisalNo = employeePerformanceTargetLine.AppraisalNo;
				employeePerformanceTargetLineObj.AgreedPerformanceTargets = employeePerformanceTargetLine.AgreedPerformanceTargets;
				employeePerformanceTargetLineObj.AgreedScore = employeePerformanceTargetLine.AgreedScore;
				employeePerformanceTargetLineObj.KeyPerformanceIndicator = employeePerformanceTargetLine.KeyPerformanceIndicator;
				employeePerformanceTargetLineObj.KeyResultAreasOutput = employeePerformanceTargetLine.KeyResultAreasOutput;
			}

			return Json(employeePerformanceTargetLineObj, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult CreateEmployeePerformanceTargetLine(EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj)
		{
			bool targetLineCreated = false;

			targetLineCreated = dynamicsNAVSOAPServices.hrManagementWS.CreateEmployeePerformanceTargetLine(employeePerformanceTargetLineObj.AppraisalNo, employeePerformanceTargetLineObj.AgreedPerformanceTargets, employeePerformanceTargetLineObj.KeyPerformanceIndicator);
			
			return Json(new { TargetLineCreated = targetLineCreated }, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult ModifyEmployeePerformanceTargetLine(EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj)
		{
			bool targetLineModified = false;

			targetLineModified = dynamicsNAVSOAPServices.hrManagementWS.ModifyEmployeePerformanceTargetLine(Convert.ToInt32(employeePerformanceTargetLineObj.LineNo), 
				                                                                                            employeePerformanceTargetLineObj.AppraisalNo, 
																											employeePerformanceTargetLineObj.AgreedPerformanceTargets, 
																											employeePerformanceTargetLineObj.KeyPerformanceIndicator);

			return Json(new { TargetLineModified = targetLineModified }, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult DeleteEmployeePerformanceTargetLine(int LineNo, string HeaderNo) 
		{
			bool targetLineDeleted = false;

			targetLineDeleted = dynamicsNAVSOAPServices.hrManagementWS.DeleteEmployeePerformanceTargetLine(LineNo, HeaderNo);

			return Json(new { targetLineDeleted = targetLineDeleted }, JsonRequestBehavior.AllowGet);
		}
		#endregion End Employee Performance Target Lines

	}
}