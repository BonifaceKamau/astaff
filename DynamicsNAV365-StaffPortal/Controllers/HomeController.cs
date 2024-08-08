using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers
{
	public class HomeController : Controller
	{
		static string companyURL = "";

		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
		DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
		BCODATAServices _dcodataServices = new BCODATAServices(companyURL);

		SuccessResponseController successResponse = new SuccessResponseController();
		InfoResponseController infoResponse = new InfoResponseController();
		ErrorResponseController errorResponse = new ErrorResponseController();

		[Authorize]
		public ActionResult Index()
		{
			//var portalModulesEnumerable = _dcodataServices.BCOData.PortalModules.Execute();
			return View();
		}
	}
}