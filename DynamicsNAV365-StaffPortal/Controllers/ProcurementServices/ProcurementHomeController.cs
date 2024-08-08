using DynamicsNAV365_StaffPortal.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.ProcurementServices
{
    public class ProcurementHomeController : Controller
    {
		public ProcurementHomeController()
		{

		}

		[Authorize]
		public ActionResult ProcurementInfo()
		{
			return View();
		}

		#region Helper Views
		[ChildActionOnly]
		public ActionResult _ProcurementSidebar()
		{
			EmployeeProfileModel employeeProfileModel = new EmployeeProfileModel();
			employeeProfileModel.PassportAttached = false;
			return PartialView(employeeProfileModel);
		}
		#endregion Helper Views
	}
}