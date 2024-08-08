using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.Responses
{
	public class InfoResponseModel
	{
		public string ResponseType { get; set; }
		public string ResponseHeader { get; set; }
		public string ResponseMessage { get; set; }
		public string DetailedResponseMessage { get; set; }

		public string Button1ControllerName { get; set; }
		public string Button1ActionName { get; set; }
		public bool Button1HasParameters { get; set; }
		public string Button1Parameters { get; set; }
		public string Button1Name { get; set; }

		public string Button2ControllerName { get; set; }
		public string Button2ActionName { get; set; }
		public bool Button2HasParameters { get; set; }
		public string Button2Parameters { get; set; }
		public string Button2Name { get; set; }
	}
}