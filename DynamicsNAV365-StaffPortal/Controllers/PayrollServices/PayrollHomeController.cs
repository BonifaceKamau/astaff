using DynamicsNAV365_StaffPortal.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.PayrollServices
{
    public class PayrollHomeController : Controller
    {
		public PayrollHomeController()
		{

		}

		[Authorize]
		public ActionResult PayrollInfo()
		{
			return View();
		}

		#region Helper Views
		[ChildActionOnly]
		public ActionResult _PayrollSidebar()
		{
			EmployeeProfileModel employeeProfileObj = new EmployeeProfileModel();
			employeeProfileObj.PassportAttached = false;
			return PartialView(employeeProfileObj);
		}
		#endregion Helper Views
	}
}