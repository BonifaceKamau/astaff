using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.ProjectManagement
{
    public class ProjectHomeController : Controller
    {
        // GET: ProjectHome
        [Authorize]
        public ActionResult ProjectInfo()
        {
            return View();
        }
        [Authorize]
        public ActionResult _ProjectManagementSidebar()
        {
            return PartialView();
        }
    }
}