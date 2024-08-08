using DynamicsNAV365_StaffPortal.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.ResponseServices
{
    public class ErrorResponseController : Controller
    {
		string companyName = ServiceConnection.CompanyName;

		#region Application Error
		public ActionResult ApplicationError(string responseHeader, string responseMessage, string detailedResponseMessage, string button1ControllerName, string button1ActionName, bool button1HasParameters, string button1Parameters, string button1Name, string button2ControllerName, string button2ActionName, bool button2HasParameters, string button2Parameters, string button2Name)
		{
			ErrorResponseModel errorResponseObj = new ErrorResponseModel();

			errorResponseObj.ResponseType = "ERROR";
			errorResponseObj.ResponseHeader = responseHeader;
			errorResponseObj.ResponseMessage = responseMessage;
			errorResponseObj.DetailedResponseMessage = detailedResponseMessage;

			errorResponseObj.Button1ControllerName = button1ControllerName;
			errorResponseObj.Button1ActionName = button1ActionName;
			errorResponseObj.Button1HasParameters = button1HasParameters;
			errorResponseObj.Button1Parameters = button1Parameters;
			errorResponseObj.Button1Name = button1Name;

			errorResponseObj.Button2ControllerName = button2ControllerName;
			errorResponseObj.Button2ActionName = button2ActionName;
			errorResponseObj.Button2HasParameters = button2HasParameters;
			errorResponseObj.Button2Parameters = button2Parameters;
			errorResponseObj.Button2Name = button2Name;

			return View("ErrorResponse", errorResponseObj);
		}

		public ActionResult ApplicationExceptionError(Exception ex,string returnUrl="")
		{
			ErrorResponseModel errorResponseObj = new ErrorResponseModel();

			errorResponseObj.ResponseType = "ERROR";
			errorResponseObj.ResponseHeader = "Application Exception Error";
			errorResponseObj.ResponseMessage = ex.Message;
			errorResponseObj.DetailedResponseMessage = ex.Message;

			errorResponseObj.Button1ControllerName = "Home";
			errorResponseObj.Button1ActionName = "Index";
			errorResponseObj.Button1HasParameters = false;
			errorResponseObj.Button1Parameters = "";
			errorResponseObj.Button1Name = "Close";

			errorResponseObj.Button2ControllerName = "";
			errorResponseObj.Button2ActionName = "";
			errorResponseObj.Button2HasParameters = false;
			errorResponseObj.Button2Parameters = "";
			errorResponseObj.Button2Name = "";
			ViewBag.returnUrl = returnUrl;

			return View("ErrorResponse", errorResponseObj);
		}

		#endregion Application Error

		#region Server Errors
		public ActionResult InternalServerError()
		{
			ErrorResponseModel errorResponseObj = new ErrorResponseModel();
			errorResponseObj.ResponseType = "ERROR";
			errorResponseObj.ResponseHeader = "500 Internal Server Error";
			errorResponseObj.ResponseMessage = "The staff portal is unable to connect to the server.<br />" +
											   "The server could be temporarily unavailable or too busy.Try again in a few minutes.";
			errorResponseObj.DetailedResponseMessage = "";

			errorResponseObj.Button1ControllerName = "Account";
			errorResponseObj.Button1ActionName = "Logout";
			errorResponseObj.Button1HasParameters = false;
			errorResponseObj.Button1Parameters = "";
			errorResponseObj.Button1Name = "Ok";

			errorResponseObj.Button2ControllerName = "";
			errorResponseObj.Button2ActionName = "";
			errorResponseObj.Button2HasParameters = false;
			errorResponseObj.Button2Parameters = "";
			errorResponseObj.Button2Name = "";

			return View(errorResponseObj);
		}
		public ActionResult GatewayTimeout()
		{
			ErrorResponseModel errorResponseObj = new ErrorResponseModel();
			errorResponseObj.ResponseType = "ERROR";
			errorResponseObj.ResponseHeader = "504 Gateway Timeout Error";
			errorResponseObj.ResponseMessage = "The staff portal is unable to connect to the server.<br />" +
											   "The server could be temporarily unavailable or too busy.Try again in a few minutes.";
			errorResponseObj.DetailedResponseMessage = "";

			errorResponseObj.Button1ControllerName = "Account";
			errorResponseObj.Button1ActionName = "Logout";
			errorResponseObj.Button1HasParameters = false;
			errorResponseObj.Button1Parameters = "";
			errorResponseObj.Button1Name = "Ok";

			errorResponseObj.Button2ControllerName = "";
			errorResponseObj.Button2ActionName = "";
			errorResponseObj.Button2HasParameters = false;
			errorResponseObj.Button2Parameters = "";
			errorResponseObj.Button2Name = "";

			return View("ErrorResponse", errorResponseObj);
		}
		#endregion Server Errors

	}
}