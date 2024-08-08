using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.HumanResource.PerformanceTargetModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    public class CompleteAppraisalController : Controller
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

		public CompleteAppraisalController()
        {
			employeeNo = AccountController.GetEmployeeNo();
        }

		#region Complete Appraisal history

		[Authorize]
		public ActionResult CompleteAppraiseeAppraisal() 
		{
			try
			{
				List<EmployeePerformanceTargetHeaderModel> performanceTargetList = new List<EmployeePerformanceTargetHeaderModel>();

				dynamic performanceTargets = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetClosedAppraiseeEvaluations("", employeeNo));

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

		#endregion End Complete Appraisal history

		#region Complete Appraisal history

		[Authorize]
		public ActionResult CompleteAppraisalHistory()
		{
			try
			{
				List<EmployeePerformanceTargetHeaderModel> performanceTargetList = new List<EmployeePerformanceTargetHeaderModel>();

				dynamic performanceTargets = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetClosedAppraiserEvaluations("", employeeNo));

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

		#endregion End Complete Appraisal history

		#region View Complete Appraisal

		[Authorize]
		public ActionResult ViewCompleteAppraisal(string AppraisalNo)
		{
			try
			{
				if (AppraisalNo.Equals(""))
				{
					return RedirectToAction("CompleteAppraisalHistory", "CompleteAppraisal");
				}
				if (dynamicsNAVSOAPServices.hrManagementWS.CheckPerformanceTargetExists(AppraisalNo, AccountController.GetEmployeeNo()))
				{
					EmployeePerformanceTargetHeaderModel performanceTargetObj = new EmployeePerformanceTargetHeaderModel();
					dynamic performanceTarges = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetClosedAppraiseeEvaluations(AppraisalNo, ""));
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
					responseHeader = "Complete AppraisalNotFound";
					responseMessage = "Complete Appraisal No." + AppraisalNo + " was not found under employee no." + AccountController.GetEmployeeNo();
					detailedResponseMessage = "Complete Appraisal No." + AppraisalNo + " was not found under employee no." + AccountController.GetEmployeeNo();

					button1ControllerName = "CompleteAppraisal";
					button1ActionName = "CompleteAppraisalHistory";
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

		#endregion End View Complete Appraisal

		#region Complete Appraisal Lines

		[Authorize]
		[ChildActionOnly]
		public ActionResult _CompleteAppraisalLine(string HeaderNo)
		{
			EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj = new EmployeePerformanceTargetLineModel();
			return PartialView(employeePerformanceTargetLineObj);
		}

		[Authorize]
		public JsonResult GetCompleteAppraisalLines(string HeaderNo)
		{
			List<EmployeePerformanceTargetLineModel> employeePerformanceTargetList = new List<EmployeePerformanceTargetLineModel>();
			dynamic employeePerformanceTargetLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetClosedEvaluationLines(HeaderNo));
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
				employeePerformanceTargetLineObj.SupervisorsScore = employeePerformanceTargetLine.SupervisorsScore;
				employeePerformanceTargetLineObj.SupervisorComments = employeePerformanceTargetLine.SupervisorComments;
				employeePerformanceTargetLineObj.SelfAssessment = employeePerformanceTargetLine.SelfAssessment;
				employeePerformanceTargetLineObj.SupervisorAssessment = employeePerformanceTargetLine.SupervisorAssessment;

				employeePerformanceTargetList.Add(employeePerformanceTargetLineObj);
			}
			return Json(employeePerformanceTargetList.ToList(), JsonRequestBehavior.AllowGet);
		}

		[Authorize]
		public JsonResult GetCompleteAppraisalByLineNo(int LineNo, string HeaderNo)
		{
			EmployeePerformanceTargetLineModel employeePerformanceTargetLineObj = new EmployeePerformanceTargetLineModel();
			dynamic employeePerformanceTargetLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetAppraiserEvaluationByLineNo(LineNo, HeaderNo));
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
				employeePerformanceTargetLineObj.SupervisorsScore = employeePerformanceTargetLine.SupervisorsScore;
				employeePerformanceTargetLineObj.SupervisorComments = employeePerformanceTargetLine.SupervisorComments;
				employeePerformanceTargetLineObj.SelfAssessment = employeePerformanceTargetLine.SelfAssessment;
				employeePerformanceTargetLineObj.SupervisorAssessment = employeePerformanceTargetLine.SupervisorAssessment;
			}

			return Json(employeePerformanceTargetLineObj, JsonRequestBehavior.AllowGet);
		}

		#endregion End Appraiser Evaluation Lines
	}
}