using DynamicsNAV365_StaffPortal.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.InventoryServices
{
    public class InventoryHomeController : Controller
    {
        public InventoryHomeController()
        {

        }

        [Authorize]
        public ActionResult InventoryInfo()
        {
            return View();
        }

        #region Helper Views
        [ChildActionOnly]
        public ActionResult _InventorySidebar()
        {
            EmployeeProfileModel employeeProfileModel = new EmployeeProfileModel();
            employeeProfileModel.PassportAttached = false;
            return PartialView(employeeProfileModel);
        }
        #endregion Helper Views

    }
}