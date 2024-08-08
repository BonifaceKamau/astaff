using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.PayrollModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.PayrollServices
{
    public class EmployeePayslipController : Controller
    {
		string companyName = ServiceConnection.CompanyName;
		static string companyURL = "";

		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
		DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        BCODATAServices _bcodataServices = new BCODATAServices(companyURL);

        SuccessResponseController successResponse = new SuccessResponseController();
		InfoResponseController infoResponse = new InfoResponseController();
		ErrorResponseController errorResponse = new ErrorResponseController();

		AccountController accountController = new AccountController();
		string employeeNo = "";

		public EmployeePayslipController()
		{
			employeeNo = AccountController.GetEmployeeNo();
		}

        #region Employee Payslip  

        [Authorize]
        public ActionResult PrintEmployeePayslip()
        {
            try
            {
                EmployeePayslipModel employeePayslipObj = new EmployeePayslipModel();

                //var payrollPeriods = from payrollPeriodQuery in dynamicsNAVODataServices.dynamicsNAVOData.PayrollPeriods
                //  where payrollPeriodQuery.Payroll_Group_Code.Equals(dynamicsNAVSOAPServices.payrollManagementWS.GetEmployeePayrollGroup(AccountController.GetEmployeeNo()))
                //                     select payrollPeriodQuery;

                //List<PayrollPeriodModel> substitutes = new List<PayrollPeriodModel>();

                //string substituteurl = "PayrollPeriods?$format=json";

                //HttpWebResponse httpsubstituteurl = Models.Credentials.GetOdataData(substituteurl);
                //using (var streamReader2 = new StreamReader(httpsubstituteurl.GetResponseStream()))
                //{
                //    var result2 = streamReader2.ReadToEnd();

                //    var details2 = JObject.Parse(result2);

                //    foreach (JObject config2 in details2["value"])
                //    {
                //        PayrollPeriodModel EList = new PayrollPeriodModel();
                //        EList.StartingDate = (string)config2["Starting_Date"];
                //        EList.Name = (string)config2["Name"];
                //        substitutes.Add(EList);
                //    }
                //}
                //List<PayrollPeriodModel> substitutes = new List<PayrollPeriodModel>();
                //string substituteurl = "PayrollPeriods?$format=json";

                //HttpWebResponse httpsubstituteurl = Models.Credentials.GetOdataData(substituteurl);
                //using (var streamReader2 = new StreamReader(httpsubstituteurl.GetResponseStream()))
                //{
                //    var result2 = streamReader2.ReadToEnd();

                //    var details2 = JObject.Parse(result2);

                //    foreach (JObject config2 in details2["value"])
                //    {
                //        PayrollPeriodModel EList = new PayrollPeriodModel();
                //        EList.StartingDate = (string)config2["Starting_Date"];
                //        EList.Name = (string)config2["Name"];
                //        substitutes.Add(EList);
                //    }
                //}

                //employeePayslipObj = new EmployeePayslipModel
                //{
                //    ListofPeriods = substitutes.Select(x =>
                //                          new SelectListItem()
                //                          {
                //                              Text = x.Name,
                //                              Value = x.Starting_Date
                //                          }).OrderBy(x => x.Text).ToList(),

                //};

                var employee = _bcodataServices.BCOData.HR_Employee.Execute().ToList();
                var userCompany = employee.Where(c => c.No == employeeNo).FirstOrDefault()?.Company;


                employeePayslipObj = new EmployeePayslipModel
                {
                    ListofPeriods = _bcodataServices.BCOData.PayrollPeriods.Where(c => c.Company_SBU == userCompany && c.ProcessingStatus == "Approved")
                    .Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Name,
                                              Value = x.Name

                                          }).ToList(),

                };
                //employeePayslipObj.EmployeeNo = AccountController.GetEmployeeNo();
                //employeePayslipObj.PayrollPeriods = new SelectList(substitutes, "Starting_Date", "Name");
                return View(employeePayslipObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PrintEmployeePayslip(EmployeePayslipModel EmployeePayslipObj)
        {
            bool payslipPrinted = false;
            string filePath = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var payrollPeriods = from payrollPeriodQuery in dynamicsNAVODataServices.dynamicsNAVOData.PayrollPeriods
                                        // where payrollPeriodQuery.Payroll_Group_Code.Equals(dynamicsNAVSOAPServices.payrollManagementWS.GetEmployeePayrollGroup(AccountController.GetEmployeeNo()))
                                         select payrollPeriodQuery;

                    EmployeePayslipObj.PayrollPeriods = new SelectList(payrollPeriods, "Starting_Date", "Name");
                    EmployeePayslipObj.EmployeeNo = AccountController.GetEmployeeNo();
                    string period = EmployeePayslipObj.PayrollPeriod;
                    DateTime selectedperiod = DateTime.Parse(EmployeePayslipObj.StartingDate);
                    //  payslipPrinted = dynamicsNAVSOAPServices.payrollManagementWS.PrintEmployeePayslip(EmployeePayslipObj.EmployeeNo, EmployeePayslipObj.PayrollPeriod);
                    filePath = dynamicsNAVSOAPServices.payrollManagementWS.GeneratePayslip(EmployeePayslipObj.EmployeeNo,selectedperiod);

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
        public JsonResult GeneratePayslip(string Period)
        {
            try
            {
                string employeeNo = "";
                string filename = "";
                string filenane = "";
                employeeNo = AccountController.GetEmployeeNo();
                filename = "Payslip_" + Period + "_" + employeeNo + ".pdf";
                var err = GetBaseUrl();
               
                filenane = dynamicsNAVSOAPServices.payrollManagementWS.GeneratePayslipPortal(employeeNo, Period, filename);
                return Json(new { message = $"{GetBaseUrl()}/reports/" + filename, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public async Task<ActionResult> GeneratePayslipFile(string Period)
        {
            try
            {
                string employeeNo = "";
                string filename = "";
                string filenane = "";
                employeeNo = AccountController.GetEmployeeNo();
                filename = "Payslip_" + Period + "_" + employeeNo + ".pdf";
                var err = GetBaseUrl();
               
                var base64String = dynamicsNAVSOAPServices.payrollManagementWS.GeneratePayslipPortal(employeeNo, Period, filename);
                //filename = dynamicsNAVSOAPServices.payrollManagementWS.VehicleFleetlogRecordReport(filename, startDateString, endDateString, LogNo??"",RegNumber??"", EmpNo??"",Program??"", format);
                /*if (filename.Equals("")) return errorResponse.ApplicationExceptionError(new Exception("Unable to print the VehicleFleetlogRecordReport. " + ServiceConnection.contactICTDepartment + " "));
                using (var wc = new WebClient())
                {
                    var byteArr = await wc.DownloadDataTaskAsync(filename);
                    return File(byteArr, MimeMapping.GetMimeMapping(filename));
                }*/
                var fileBytes = Convert.FromBase64String(base64String);
                return File(fileBytes, MimeMapping.GetMimeMapping(filename));
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

        [Authorize]
        public PartialViewResult _PreviewPayslipPDF()
        {
            return PartialView();
        }

        #endregion Employee Payslip

    }
}