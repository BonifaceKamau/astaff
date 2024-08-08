using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
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
    public class AppraiseeEvaluationController : Controller
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

		public AppraiseeEvaluationController()
        {
			employeeNo = AccountController.GetEmployeeNo();
        }

		#region Appraisee Evaluation history

		[Authorize]
		public ActionResult AppraiseeEvaluationHistory()
		{
			try
			{
				List<EmployeePerformanceTargetHeaderModel> performanceTargetList = new List<EmployeePerformanceTargetHeaderModel>();

				dynamic performanceTargets = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetAppraiseeEvaluations("", employeeNo));

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

		#endregion End Appraisee Evaluation history

		#region View Appraisee Evaluation

		[Authorize]
		public ActionResult ViewAppraiseeEvaluation(string AppraisalNo) 
		{
			try
			{
				if (AppraisalNo.Equals(""))
				{
					return RedirectToAction("AppraiseeEvaluationHistory", "AppraiseeEvaluation");
				}
				if (dynamicsNAVSOAPServices.hrManagementWS.CheckPerformanceTargetExists(AppraisalNo, AccountController.GetEmployeeNo()))
				{
					EmployeePerformanceTargetHeaderModel performanceTargetObj = new EmployeePerformanceTargetHeaderModel();
					dynamic performanceTarges = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetAppraiseeEvaluations(AppraisalNo, ""));
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
					responseHeader = "Appraisee Evaluation NotFound";
					responseMessage = "The appraisee evaluation No." + AppraisalNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "The appraisee evaluation No." + AppraisalNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "AppraiseeEvaluation";
					button1ActionName = "AppraiseeEvaluationHistory";
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
		public async Task<ActionResult> ViewAppraiseeEvaluation(EmployeePerformanceTargetHeaderModel performanceTargetObj, string Command)
		{
			try
			{
				if (performanceTargetObj.No.Equals(""))
				{
					return RedirectToAction("AppraiseeEvaluationHistory", "AppraiseeEvaluation");
				}
				if (Command == "Submit")
				{
					if (dynamicsNAVSOAPServices.hrManagementWS.SubmitAppraiseeEvaluationToAppraiser(performanceTargetObj.No))
					{
						responseHeader = "Success";
						responseMessage = "Appraisee evaluation submitted to appraiser successfully.";
						detailedResponseMessage = "Appraisee evaluation submitted to appraiser successfully.";

						button1ControllerName = "AppraiseeEvaluation";
						button1ActionName = "AppraiseeEvaluationHistory";
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
						responseMessage = "Appraisal evaluation returned to appraisee successfully.";
						detailedResponseMessage = "Appraisal evaluation returned to appraisee successfully.";

						button1ControllerName = "AppraiseeEvaluation";
						button1ActionName = "AppraiseeEvaluationHistory";
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

		#endregion End View Appraisee Evaluation

		#region Appraisee Evaluation Lines

		[Authorize]
		[ChildActionOnly] 
		public ActionResult _AppraiseeEvaluationLine(string HeaderNo)
		{
			EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj = new EmployeePerformanceTargetLineModel();
			return PartialView(employeePerformanceTargetLineObj);
		}

		[Authorize]
		public JsonResult GetAppraiseeEvaluationLines(string HeaderNo)
		{
			List<EmployeePerformanceTargetLineModel> employeePerformanceTargetList = new List<EmployeePerformanceTargetLineModel>();
			dynamic employeePerformanceTargetLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetAppraiseeEvaluationLines(HeaderNo));
			foreach (var employeePerformanceTargetLine in employeePerformanceTargetLines)
			{
				EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj = new EmployeePerformanceTargetLineModel();

				employeePerformanceTargetLineObj.LineNo = employeePerformanceTargetLine.LineNo;
				employeePerformanceTargetLineObj.AppraisalNo = employeePerformanceTargetLine.AppraisalNo;
				employeePerformanceTargetLineObj.AgreedPerformanceTargets = employeePerformanceTargetLine.AgreedPerformanceTargets;
				employeePerformanceTargetLineObj.AgreedScore = employeePerformanceTargetLine.AgreedScore;
				employeePerformanceTargetLineObj.KeyPerformanceIndicator = employeePerformanceTargetLine.KeyPerformanceIndicator;
				employeePerformanceTargetLineObj.KeyResultAreasOutput = employeePerformanceTargetLine.KeyResultAreasOutput;
				employeePerformanceTargetLineObj.SelfScore = employeePerformanceTargetLine.SelfScore;
				employeePerformanceTargetLineObj.AppraiseeComments = employeePerformanceTargetLine.AppraiseeComments;
				employeePerformanceTargetLineObj.SelfAssessment = employeePerformanceTargetLine.SelfAssessment;
				employeePerformanceTargetLineObj.SupervisorAssessment = employeePerformanceTargetLine.SupervisorAssessment;

				employeePerformanceTargetList.Add(employeePerformanceTargetLineObj);
			}
			return Json(employeePerformanceTargetList.ToList(), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult GetAppraiseeEvaluationByLineNo(int LineNo, string HeaderNo)
		{
			EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj = new EmployeePerformanceTargetLineModel();
			dynamic employeePerformanceTargetLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetAppraiseeEvaluationByLineNo(LineNo, HeaderNo));
			foreach (var employeePerformanceTargetLine in employeePerformanceTargetLines)
			{
				employeePerformanceTargetLineObj.LineNo = employeePerformanceTargetLine.LineNo;
				employeePerformanceTargetLineObj.AppraisalNo = employeePerformanceTargetLine.AppraisalNo;
				employeePerformanceTargetLineObj.AgreedPerformanceTargets = employeePerformanceTargetLine.AgreedPerformanceTargets;
				employeePerformanceTargetLineObj.AgreedScore = employeePerformanceTargetLine.AgreedScore;
				employeePerformanceTargetLineObj.KeyPerformanceIndicator = employeePerformanceTargetLine.KeyPerformanceIndicator;
				employeePerformanceTargetLineObj.KeyResultAreasOutput = employeePerformanceTargetLine.KeyResultAreasOutput;
				employeePerformanceTargetLineObj.SelfScore = employeePerformanceTargetLine.SelfScore;
				employeePerformanceTargetLineObj.AppraiseeComments = employeePerformanceTargetLine.AppraiseeComments;
				employeePerformanceTargetLineObj.SelfAssessment = employeePerformanceTargetLine.SelfAssessment;
				employeePerformanceTargetLineObj.SupervisorAssessment = employeePerformanceTargetLine.SupervisorAssessment;
			}

			return Json(employeePerformanceTargetLineObj, JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult ModifyAppraiseeEvaluationLine(EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj)
		{
			bool targetLineModified = false;

			targetLineModified = dynamicsNAVSOAPServices.hrManagementWS.ModifyAppraiseeEvaluationLine(Convert.ToInt32(employeePerformanceTargetLineObj.LineNo),
																									employeePerformanceTargetLineObj.AppraisalNo,
																									Convert.ToDecimal(employeePerformanceTargetLineObj.SelfAssessment),
																									employeePerformanceTargetLineObj.AppraiseeComments);

			return Json(new { TargetLineModified = targetLineModified }, JsonRequestBehavior.AllowGet);
		}

		#endregion End Appraisee Evaluation Lines
	}
}