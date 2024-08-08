using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.Account;
using DynamicsNAV365_StaffPortal.Models.HumanResource.HumanResourceHome;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    public class HumanResourceHomeController : Controller
    {
		static string companyURL = "";
		static string employeeNo = "";

		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
		BCODATAServices dynamicsNAVODataServices = new BCODATAServices(companyURL);

		public HumanResourceHomeController()
		{
			employeeNo = AccountController.GetEmployeeNo();
		}
		//Human Resource Service Summary
		[Authorize]
		public ActionResult HumanResourceInfo()
		{
			return View();
		}

		#region Helper Views
	
		[Authorize]
		[ChildActionOnly]
		public ActionResult _HumanResourceSidebar()
		{
			EmployeeProfileModel employeeProfileModel = new EmployeeProfileModel();
			employeeProfileModel.PassportAttached = false;
			return PartialView(employeeProfileModel);
		}

		[Authorize]
		[ChildActionOnly]
		public ActionResult _EmployeeLeaveBalanceWidget()
		{
			List<EmployeeLeaveBalanceModel> employeeLeaveBalances = new List<EmployeeLeaveBalanceModel>();
			
			//var employeeLeaveTypes = from employeeLeaveTypeList in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeLeaveTypes
			//						 where employeeLeaveTypeList.Employee_No.Equals(AccountController.GetEmployeeNo())
			//						 select employeeLeaveTypeList;

			dynamic employeeLeaveTypes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.GetEmployeeLeaveTypes(employeeNo));

			foreach (var employeeLeaveType in employeeLeaveTypes)
			{
				EmployeeLeaveBalanceModel employeeLeaveBalance = new EmployeeLeaveBalanceModel();
				employeeLeaveBalance.LeaveType = employeeLeaveType.LeaveType;
			
				if (employeeLeaveBalance.LeaveType == "ANNUAL")
                {
					employeeLeaveBalance.LeaveBalanceStr = dynamicsNAVSOAPServices.hrManagementWS.GetAvailableAnnaulLeave(employeeNo).ToString();
				}
                else
                {
					employeeLeaveBalance.LeaveBalanceStr = employeeLeaveType.LeaveBalanceStr;
				}

				employeeLeaveBalances.Add(employeeLeaveBalance);
			}
			return PartialView(employeeLeaveBalances);
		}

		#endregion Helper Views

		public ActionResult _DisciplinaryStatusWidget()
		{
			var cases = dynamicsNAVODataServices.BCOData.Disciplinary_Cases_Card.Where(c => c.Employee_No == employeeNo).ToList();
			return PartialView(cases);
		}
    }
}