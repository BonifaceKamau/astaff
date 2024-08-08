using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.PerformanceManagement
{
    public class PerformanceHomeController : Controller
    {
        // GET: PerformanceInfo
        [Authorize]
        public ActionResult PerformanceInfo()
        {
            return View();
        }
        [Authorize]
        public ActionResult TargetsHome()   
        {

            return View();
        }
        [Authorize]
        public ActionResult AppraisalsHome()
        {
            return View();
        }
        [Authorize]
        public ActionResult SupervisorAppraisalHome()
        {
            return View();
        }
        [Authorize]
        public ActionResult PeerAppraisalHome()
        {
            return View();
        }
        [Authorize]
        public ActionResult CustomerAppraisalHome()
        {
            return View();
        }
        [Authorize]
        public ActionResult SubordinateAppraisalHome()
        {
            return View();
        }
        [Authorize]
        public ActionResult AppraisalResultsHome()
        {
            return View(); 
        }

        [Authorize]
        public ActionResult _PerformanceManagementSidebar()
        {
            return PartialView();
        }
    }
}