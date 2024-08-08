using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.PayrollModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.PayrollServices
{
    public class EmployeeP9Controller : Controller
    {
        string companyName = ServiceConnection.CompanyName;
        static string companyURL = "";

        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        BCODATAServices BCODATAServices = new BCODATAServices(companyURL);

        SuccessResponseController successResponse = new SuccessResponseController();
        InfoResponseController infoResponse = new InfoResponseController();
        ErrorResponseController errorResponse = new ErrorResponseController();

        AccountController accountController = new AccountController();
        string employeeNo = "";

        public EmployeeP9Controller()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        #region  Employee P9

        [Authorize]
        public ActionResult PrintEmployeeP9()
        {
            try
            {
                EmployeeP9Model employeePayslipObj = new EmployeeP9Model();

                var payrollPeriods = from payrollPeriodQuery in BCODATAServices.BCOData.PayrollPeriods
                                     select payrollPeriodQuery;

                employeePayslipObj.EmployeeNo = AccountController.GetEmployeeNo();
                employeePayslipObj.PayrollPeriods = new SelectList(payrollPeriods, "Starting_Date", "Starting_Date");

                return View(employeePayslipObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PrintEmployeeP9(EmployeeP9Model EmployeePayslipObj)
        {
            string filePath = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var payrollPeriods = from payrollPeriodQuery in dynamicsNAVODataServices.dynamicsNAVOData.PayrollPeriods
                                         select payrollPeriodQuery;

                    EmployeePayslipObj.PayrollPeriods = new SelectList(payrollPeriods, "Starting_Date", "Starting_Date");
                    EmployeePayslipObj.EmployeeNo = AccountController.GetEmployeeNo();

                    filePath = dynamicsNAVSOAPServices.payrollManagementWS.GenerateP9(employeeNo, DateTime.Parse(EmployeePayslipObj.StartDate), DateTime.Parse(EmployeePayslipObj.EndDate), "");

                    if (!filePath.Equals(""))
                    {
                        using (WebClient wc = new WebClient())
                        {
                            var byteArr = await wc.DownloadDataTaskAsync(filePath);
                            return File(byteArr, "application/pdf");
                        }
                    }
                    else
                    {
                        EmployeePayslipObj.ErrorStatus = true;
                        EmployeePayslipObj.ErrorMessage = "Unable to print the payslip. " + ServiceConnection.contactICTDepartment + " ";

                        return View(EmployeePayslipObj);
                    }
                }
                catch (Exception ex)
                {
                    return errorResponse.ApplicationExceptionError(ex);
                }
            }
            else
            {
                return View(EmployeePayslipObj);
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GenerateP9(string StartDate, string EndDate)
        {
            try
            {
                string employeeNo = "";
                string filename = "";
                string filenane = "";
                employeeNo = AccountController.GetEmployeeNo();
                filename = "P9_" + employeeNo + ".pdf";

                filenane = dynamicsNAVSOAPServices.payrollManagementWS.GenerateP9(employeeNo, DateTime.Parse(StartDate), DateTime.Parse(EndDate), filename);
                return Json(new { message = $"{GetBaseUrl()}/reports/" + filename, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public string GetBaseUrl()
        {
            var request = HttpContext.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/") 
                appUrl = "/" + appUrl;

            var baseUrl = $"{request.Url?.Scheme}://{request.Url?.Authority}{appUrl}";

            return baseUrl;
        }


        #endregion End Employee P9

    }
}