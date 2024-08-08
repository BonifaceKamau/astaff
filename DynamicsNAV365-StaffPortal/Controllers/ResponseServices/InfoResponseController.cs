using DynamicsNAV365_StaffPortal.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.ResponseServices
{
    public class InfoResponseController : Controller
    {
		string companyName = ServiceConnection.CompanyName;

		#region Application Info
		public ActionResult ApplicationInfo(string ResponseHeader, string ResponseMessage, string DetailedResponseMessage, string Button1ControllerName, string Button1ActionName, bool Button1HasParameters, string Button1Parameters, string Button1Name, string Button2ControllerName, string Button2ActionName, bool Button2HasParameters, string Button2Parameters, string Button2Name)
		{
			InfoResponseModel infoResponseModel = new InfoResponseModel();

			infoResponseModel.ResponseType = "INFO";
			infoResponseModel.ResponseHeader = ResponseHeader;
			infoResponseModel.ResponseMessage = ResponseMessage;
			infoResponseModel.DetailedResponseMessage = DetailedResponseMessage;

			infoResponseModel.Button1ControllerName = Button1ControllerName;
			infoResponseModel.Button1ActionName = Button1ActionName;
			infoResponseModel.Button1HasParameters = Button1HasParameters;
			infoResponseModel.Button1Parameters = Button1Parameters;
			infoResponseModel.Button1Name = Button1Name;

			infoResponseModel.Button2ControllerName = Button2ControllerName;
			infoResponseModel.Button2ActionName = Button2ActionName;
			infoResponseModel.Button2HasParameters = Button2HasParameters;
			infoResponseModel.Button2Parameters = Button2Parameters;
			infoResponseModel.Button2Name = Button2Name;

			return View("InfoResponse", infoResponseModel);
		}
		#endregion Application Info

		#region Application Confirm Dialog
		public ActionResult ApplicationConfirm(string ResponseHeader, string ResponseMessage, string DetailedResponseMessage, string Button1ControllerName, string Button1ActionName, bool Button1HasParameters, string Button1Parameters, string Button1Name, string Button2ControllerName, string Button2ActionName, bool Button2HasParameters, string Button2Parameters, string Button2Name)
		{
			InfoResponseModel infoResponseModel = new InfoResponseModel();

			infoResponseModel.ResponseType = "CONFIRM";
			infoResponseModel.ResponseHeader = ResponseHeader;
			infoResponseModel.ResponseMessage = ResponseMessage;
			infoResponseModel.DetailedResponseMessage = DetailedResponseMessage;

			infoResponseModel.Button1ControllerName = Button1ControllerName;
			infoResponseModel.Button1ActionName = Button1ActionName;
			infoResponseModel.Button1HasParameters = Button1HasParameters;
			infoResponseModel.Button1Parameters = Button1Parameters;
			infoResponseModel.Button1Name = Button1Name;

			infoResponseModel.Button2ControllerName = Button2ControllerName;
			infoResponseModel.Button2ActionName = Button2ActionName;
			infoResponseModel.Button2HasParameters = Button2HasParameters;
			infoResponseModel.Button2Parameters = Button2Parameters;
			infoResponseModel.Button2Name = Button2Name;

			return View("ConfirmResponse", infoResponseModel);
		}

		#endregion Application Confirm Dialog
	}
}