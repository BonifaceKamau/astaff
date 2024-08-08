using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using DynamicsNAV365_StaffPortal.Models.HumanResource.HumanResourceHome;
using DynamicsNAV365_StaffPortal.Models.HumanResource.LeaveApplicationModels;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.Models.HumanResource;
using OdataRef;
using DimensionValues = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.DimensionValues;
using DocAttachments = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.DocAttachments;
using EmployeeLeaveTypes = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.EmployeeLeaveTypes;
using Employees = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.Employees;
using Job_List = OdataRef.Job_List;
using LeaveTypes = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.LeaveTypes;
using PortalDocuments = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.PortalDocuments;

namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    [NoCache]
    public class LeaveApplicationController : Controller
    {
        static string companyName = ServiceConnection.CompanyName;
        static string companyURL = "";

        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        BCODATAServices _bcodataServices = new BCODATAServices(companyURL);

        SuccessResponseController successResponse = new SuccessResponseController();
        InfoResponseController infoResponse = new InfoResponseController();
        ErrorResponseController errorResponse = new ErrorResponseController();

        private string responseHeader = "";
        private string responseMessage = "";
        private string detailedResponseMessage = "";

        private string button1ControllerName = "";
        private string button1ActionName = "";
        private bool button1HasParameters = false;
        private string button1Parameters = "";
        private string button1Name = "";

        private string button2ControllerName = "";
        private string button2ActionName = "";
        private bool button2HasParameters = false;
        private string button2Parameters = "";
        private string button2Name = "";

        IQueryable<Employees> employees = null;
        IQueryable<EmployeeLeaveTypes> employeeLeaveTypes = null;
        IQueryable<DimensionValues> globalDimension1Values = null;
        IQueryable<DimensionValues> globalDimension2Values = null;
        IQueryable<DimensionValues> shortcutDimension3Values = null;
        IQueryable<DimensionValues> shortcutDimension4Values = null;
        IQueryable<DimensionValues> shortcutDimension5Values = null;
        IQueryable<DimensionValues> shortcutDimension6Values = null;
        IQueryable<DimensionValues> shortcutDimension7Values = null;

        IQueryable<DimensionValues> shortcutDimension8Values = null;
        //IQueryable<ResponsibilityCenters> responsibilityCenters = null;

        AccountController accountController = new AccountController();
        string employeeNo = "";

        public LeaveApplicationController()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        #region New Leave Application

        [System.Web.Mvc.Authorize]
        public ActionResult NewLeaveApplication()
        {
            try
            {
                LeaveApplicationModel leaveApplicationObj = new LeaveApplicationModel();

                leaveApplicationObj.ApplicationNo = dynamicsNAVSOAPServices.hrManagementWS.GetOpenLeaveApplication(employeeNo);

                if (!string.IsNullOrEmpty(leaveApplicationObj.ApplicationNo))
                {
                    return RedirectToAction("EditLeaveApplication", "LeaveApplication"
                        ,new { LeaveApplicationNo = leaveApplicationObj.ApplicationNo });
                }

                else
                {
                    //Check open leave application
                    if (dynamicsNAVSOAPServices.hrManagementWS.CheckOpenLeaveApplicationExists(employeeNo))
                    {
                        responseHeader = "Leave Application Exist";
                        responseMessage = "An open leave application exists for employee no. " + employeeNo +
                                          ", finalize on this leave application before creating a new one.";
                        detailedResponseMessage = "An open leave application exists for employee no. " + employeeNo +
                                                  ", finalize on this leave application before creating a new one.";

                        button1ControllerName = "LeaveApplication";
                        button1ActionName = "LeaveApplicationHistory";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";
                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                            button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                            button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    //End check open leave application

                    //Create Leave Application
                    dynamicsNAVSOAPServices.hrManagementWS.CreateLeaveApplication(employeeNo);
                    leaveApplicationObj.No = dynamicsNAVSOAPServices.hrManagementWS.GetOpenLeaveApplication(employeeNo);
                    leaveApplicationObj.EmployeeNo = AccountController.GetEmployeeNo();
                    leaveApplicationObj.EmployeeName =
                        dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);
                    leaveApplicationObj.GlobalDimension1Code = "";

                    LoadDimensions(leaveApplicationObj.GlobalDimension1Code);
                    List<LeaveTypes> Leavetypes = new List<LeaveTypes>();
                    string dimension1list = "LeaveTypes?$filter=Show_in_Portal eq true";
                    //string dimension1list = "LeaveTypes";

                    //LoadEmployeeSubstitutes(leaveApplicationObj.EmployeeNo);
                    //LoadEmployeeLeaveTypes(leaveApplicationObj.EmployeeNo);

                    HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(dimension1list);
                    using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                    {
                        var result1 = streamReader1.ReadToEnd();

                        var details1 = JObject.Parse(result1);

                        foreach (JObject config1 in details1["value"])
                        {
                            LeaveTypes DList1 = new LeaveTypes();
                            DList1.Code = (string)config1["Code"];
                            DList1.Description = (string)config1["Description"];
                            Leavetypes.Add(DList1);
                        }
                    }


                    #endregion

                    //	LoadResponsibilityCenters();
                    var employee = _bcodataServices.BCOData.HR_Employee.Execute().ToList();
                    var userCompany = employee.Where(c => c.No == employeeNo).FirstOrDefault()?.Company;
                    var substitutes = employee.Where(c => c.Company == userCompany && c.No != employeeNo);

                    leaveApplicationObj.Employees = new SelectList(substitutes, "No", "Search_Name",
                        leaveApplicationObj.SubstituteEmployeeNo);


                    var useremployee = _bcodataServices.BCOData.HR_Employee.Execute().ToList();
                    var employeegender = useremployee.Where(c => c.No == employeeNo).FirstOrDefault()?.Gender;

                    //leaveApplicationObj.LeaveTypes = new SelectList(Leavetypes, "Code", "Description");
                    leaveApplicationObj.LeaveTypes = _bcodataServices.BCOData.LeaveTypes
                        .Where(c => c.Show_in_Portal == true && (c.Gender == employeegender || c.Gender == "Both")).Select(c => new SelectListItem()
                        {
                            Value = c.Code,
                            Text = c.Description,
                            Selected = leaveApplicationObj.LeaveType == c.Code
                        }).ToList();
                    leaveApplicationObj.GlobalDimension1Codes = new SelectList(globalDimension1Values, "Code", "Name");
                    leaveApplicationObj.GlobalDimension2Codes = new SelectList(globalDimension2Values, "Code", "Name");



                    return View(leaveApplicationObj);

                }
               
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewLeaveApplication(LeaveApplicationModel LeaveApplicationObj, string Command)
        {
            string leaveApplicationNo = "";
            try
            {
                //LoadEmployeeSubstitutes(LeaveApplicationObj.EmployeeNo);
                //LoadEmployeeLeaveTypes(LeaveApplicationObj.EmployeeNo);
                //LoadDimensions(LeaveApplicationObj.GlobalDimension1Code);
                ////LoadResponsibilityCenters();
                //List<LeaveTypes> leaveTypes = new List<LeaveTypes>();
                ////string dimension1list = "LeaveTypes";
                //string dimension1list = "LeaveTypes?$filter=Show_in_Portal eq true";

                //HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(dimension1list);
                //using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                //{
                //    var result1 = streamReader1.ReadToEnd();

                //    var details1 = JObject.Parse(result1);

                //    foreach (JObject config1 in details1["value"])
                //    {
                //        LeaveTypes DList1 = new LeaveTypes();
                //        DList1.Code = (string)config1["Code"];
                //        DList1.Description = (string)config1["Description"];
                //        leaveTypes.Add(DList1);
                //    }
                //}

                /*List<Employees> substitutes = new List<Employees>();
                string substituteurl = "Employees?$format=json";

                HttpWebResponse httpsubstituteurl = Models.Credentials.GetOdataData(substituteurl);
                using (var streamReader2 = new StreamReader(httpsubstituteurl.GetResponseStream()))
                {
                    var result2 = streamReader2.ReadToEnd();

                    var details2 = JObject.Parse(result2);

                    foreach (JObject config2 in details2["value"])
                    {
                        Employees EList = new Employees();
                        EList.No = (string)config2["No"];
                        EList.Full_Name = (string)config2["Full_Name"];
                        substitutes.Add(EList);
                    }
                }
                string companyFilter = "SPUR";
                substitutes = substitutes.Where(e => e.Company == companyFilter).ToList();*/
                //var employee = _bcodataServices.BCOData.HR_Employee.Execute().ToList();
                //var userCompany = employee.Where(c => c.No == employeeNo).FirstOrDefault()?.Company;
                //var substitutes = employee.Where(c => c.Company == userCompany);

                //LeaveApplicationObj.Employees = new SelectList(substitutes, "No", "Search_Name",
                //    LeaveApplicationObj.SubstituteEmployeeNo);
                ////LeaveApplicationObj.LeaveTypes = new SelectList(leaveTypes, "Code", "Description");
                //LeaveApplicationObj.LeaveTypes = _bcodataServices.BCOData.LeaveTypes
                //    .Where(c => c.Show_in_Portal == true).Select(c => new SelectListItem()
                //    {
                //        Value = c.Code,
                //        Text = c.Description,
                //        Selected = LeaveApplicationObj.LeaveType == c.Code
                //    }).ToList();
                //LeaveApplicationObj.GlobalDimension1Codes = new SelectList(globalDimension1Values, "Code", "Name");
                //LeaveApplicationObj.GlobalDimension2Codes = new SelectList(globalDimension2Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Values, "Code", "Name");
                //	LeaveApplicationObj.ResponsibilityCenters = new SelectList(responsibilityCenters, "Code", "Description");

                //if (ModelState.IsValid)
                //{
                //    LeaveApplicationObj.ReasonForLeave = LeaveApplicationObj.ReasonForLeave != null
                //        ? LeaveApplicationObj.ReasonForLeave
                //        : "";
                //    LeaveApplicationObj.SubstituteEmployeeNo = LeaveApplicationObj.SubstituteEmployeeNo != null
                //        ? LeaveApplicationObj.SubstituteEmployeeNo
                //        : "";

                //    leaveApplicationNo = LeaveApplicationObj.No;
                //    if (leaveApplicationNo.Equals(""))
                //    {
                //        return RedirectToAction("LeaveApplicationHistory", "LeaveApplication");
                //    }

                    //DateTime.ParseExact(LeaveApplicationObj.LeaveStartDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture)


                    if (Command.Equals("Submit For Approval"))
                    {
                        bool leaveApplicationModified = dynamicsNAVSOAPServices.hrManagementWS.ModifyLeaveApplication(
                            LeaveApplicationObj.No, LeaveApplicationObj.LeaveType,
                            DateTime.Parse(LeaveApplicationObj.LeaveStartDate),
                            Convert.ToDecimal(LeaveApplicationObj.DaysApplied), LeaveApplicationObj.ReasonForLeave
                          );

                        if (!dynamicsNAVSOAPServices.hrManagementWS.CheckLeaveApplicationApprovalWorkflowEnabled(
                                leaveApplicationNo))
                        {
                            LeaveApplicationObj.No = leaveApplicationNo;
                            LeaveApplicationObj.ErrorStatus = true;
                            LeaveApplicationObj.ErrorMessage =
                                "An error was experienced when sending your leave application no." +
                                leaveApplicationNo + " for approval. Try again or contact the " + companyName +
                                " ICT department.";
                            return View(LeaveApplicationObj);
                        }

                        if (dynamicsNAVSOAPServices.hrManagementWS.SendLeaveApplicationApprovalRequest(
                                leaveApplicationNo))
                        {
                            responseHeader = "Success";
                            responseMessage =
                                "Your leave application was successfully sent for approval. Once approved, you will receive an email containing your leave details.";
                            detailedResponseMessage =
                                "Your leave application was successfully sent for approval. Once approved, you will receive an email containing your leave details.";

                            button1ControllerName = "LeaveApplication";
                            button1ActionName = "LeaveApplicationHistory";
                            button1HasParameters = false;
                            button1Parameters = "";
                            button1Name = "Ok";

                            button2ControllerName = "";
                            button2ActionName = "";
                            button2HasParameters = false;
                            button2Parameters = "";
                            button2Name = "";

                            return successResponse.ApplicationSuccess(responseHeader, responseMessage,
                                detailedResponseMessage,
                                button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                                button1Name,
                                button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                                button2Name);
                        }
                    }

                    if (Command.Equals("View Attachment"))
                    {
                        string fileURL =
                            dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(LeaveApplicationObj.No);

                        string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

                        if (!fileURL.Equals(""))
                        {
                            using (WebClient wc = new WebClient())
                            {
                                if (ext.Equals(".pdf"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/pdf");
                                }

                                else if (ext.Equals(".doc"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/msword");
                                }

                                else if (ext.Equals(".docx"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr,
                                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                                }

                                else if (ext.Equals(".jpeg") || ext.Equals(".jpg"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "image/jpeg");
                                }

                                else if (ext.Equals(".json"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/json");
                                }

                                else if (ext.Equals(".ppt"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/vnd.ms-powerpoint");
                                }

                                else if (ext.Equals(".png"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "image/png");
                                }

                                else if (ext.Equals(".pptx"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr,
                                        "application/vnd.openxmlformats-officedocument.presentationml.presentation");
                                }

                                else if (ext.Equals(".rar"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/vnd.rar");
                                }

                                else if (ext.Equals(".xls"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/vnd.ms-excel");
                                }

                                else if (ext.Equals(".xlsx"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr,
                                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                                }

                                else
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "text/plain");
                                }
                            }
                        }


                        else
                        {
                            return View(LeaveApplicationObj);
                        }
                    //}
                    //else
                    //{
                    //    LeaveApplicationObj.No = leaveApplicationNo;
                    //    //LeaveApplicationObj.ErrorStatus = true;
                    //    //	LeaveApplicationObj.ErrorMessage = "An error was experienced when sending your leave application for approval. Try again or contact the " + companyName + " ICT department.";
                    //    return View(LeaveApplicationObj);
                    //}
                }
                else
                {
                    return View(LeaveApplicationObj);
                }
            }
            catch (Exception ex)
            {
                //return errorResponse.ApplicationExceptionError(ex);
                LeaveApplicationObj.No = leaveApplicationNo;
                LeaveApplicationObj.ErrorStatus = true;
                LeaveApplicationObj.ErrorMessage = ex.Message;
                return View(LeaveApplicationObj);
            }
        }


        #region Edit Leave Application

        [System.Web.Mvc.Authorize]
        public ActionResult OnBeforeEdit(string LeaveApplicationNo)
        {
            try
            {
                if (LeaveApplicationNo.Equals(""))
                {
                    return RedirectToAction("LeaveApplicationHistory", "LeaveApplication");
                }

                if (dynamicsNAVSOAPServices.hrManagementWS.CheckLeaveApplicationExists(LeaveApplicationNo,
                        AccountController.GetEmployeeNo()))
                {
                    string leaveApplicationStatus =
                        dynamicsNAVSOAPServices.hrManagementWS.GetLeaveApplicationStatus(LeaveApplicationNo);
                    //if leave application is open
                    if (leaveApplicationStatus.Equals("Open") ||
                        leaveApplicationStatus.Equals("Declined with amendments") ||
                        leaveApplicationStatus.Equals("Pending Approval"))
                    {
                        return RedirectToAction("EditLeaveApplication", "LeaveApplication",
                            new { LeaveApplicationNo = LeaveApplicationNo });
                    }

                    //if leave application is pending approval
                    //if (leaveApplicationStatus.Equals("Pending Approval"))
                    //{
                    //    responseHeader = "Leave Application Pending Approval";
                    //    responseMessage = "The leave application no." + LeaveApplicationNo +
                    //                      " is already submitted for approval. Editing not allowed.";
                    //    detailedResponseMessage = "The leave application no." + LeaveApplicationNo +
                    //                              "is already submitted for approval. Editing not allowed.";

                    //    button1ControllerName = "LeaveApplication";
                    //    button1ActionName = "LeaveApplicationHistory";
                    //    button1HasParameters = false;
                    //    button1Parameters = "";
                    //    button1Name = "Ok";

                    //    button2ControllerName = "";
                    //    button2ActionName = "";
                    //    button2HasParameters = false;
                    //    button2Parameters = "";
                    //    button2Name = "";
                    //    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                    //        button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                    //        button1Name,
                    //        button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                    //        button2Name);
                    //}

                    ////if leave application is released
                    ////if (leaveApplicationStatus.Equals("Released"))
                    ////{
                    ////	responseHeader = "Leave Application Approved";
                    ////	responseMessage = "The leave application no." + LeaveApplicationNo + " is approved. Editing will reopen the document. Do you want to continue?";
                    ////	detailedResponseMessage = "The leave application no." + LeaveApplicationNo + " is approved. Editing will reopen the document. Do you want to continue?";

                    ////	button1ControllerName = "LeaveApplication";
                    ////	button1ActionName = "EditLeaveApplication";
                    ////	button1HasParameters = true;
                    ////	button1Parameters = "?LeaveApplicationNo=" + LeaveApplicationNo;
                    ////	button1Name = "Ok";

                    ////	button2ControllerName = "LeaveApplication";
                    ////	button2ActionName = "LeaveApplicationHistory";
                    ////	button2HasParameters = false;
                    ////	button2Parameters = "";
                    ////	button2Name = "";

                    ////	return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                    ////										  button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                    ////										  button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    ////}
                    ////if leave application is rejected
                    //if (leaveApplicationStatus.Equals("Rejected") ||
                    //    leaveApplicationStatus == "Declined with amendments")
                    //{
                    //    responseHeader = "Leave Application Rejected";
                    //    responseMessage = "The leave application no." + LeaveApplicationNo +
                    //                      " was rejected. Editing will reopen the document. Do you want to continue?";
                    //    detailedResponseMessage = "The leave application no." + LeaveApplicationNo +
                    //                              " was rejected. Editing will reopen the document. Do you want to continue?";

                    //    button1ControllerName = "LeaveApplication";
                    //    button1ActionName = "EditLeaveApplication";
                    //    button1HasParameters = true;
                    //    button1Parameters = "?LeaveApplicationNo=" + LeaveApplicationNo;
                    //    button1Name = "Ok";

                    //    button2ControllerName = "LeaveApplication";
                    //    button2ActionName = "LeaveApplicationHistory";
                    //    button2HasParameters = false;
                    //    button2Parameters = "";
                    //    button2Name = "";

                    //    return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                    //        button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                    //        button1Name,
                    //        button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                    //        button2Name);
                    //}

                    ////if leave application is posted/reversed
                    //if (leaveApplicationStatus.Equals("Posted") || leaveApplicationStatus.Equals("Reversed") ||
                    //    leaveApplicationStatus.Equals("Released"))
                    //{
                    //    responseHeader = "Leave Application Posted";
                    //    responseMessage = "The leave application no." + LeaveApplicationNo +
                    //                      " is already posted. Editing not allowed.";
                    //    detailedResponseMessage = "The leave application no." + LeaveApplicationNo +
                    //                              " is already posted. Editing not allowed.";

                    //    button1ControllerName = "LeaveApplication";
                    //    button1ActionName = "LeaveApplicationHistory";
                    //    button1HasParameters = false;
                    //    button1Parameters = "";
                    //    button1Name = "Ok";

                    //    button2ControllerName = "";
                    //    button2ActionName = "";
                    //    button2HasParameters = false;
                    //    button2Parameters = "";
                    //    button2Name = "";
                    //    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                    //        button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                    //        button1Name,
                    //        button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                    //        button2Name);
                    //}

                    return RedirectToAction("LeaveApplicationHistory", "LeaveApplication");
                }
                else
                {
                    responseHeader = "Leave Application NotFound";
                    responseMessage = "The leave application no." + LeaveApplicationNo +
                                      " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "The leave application no." + LeaveApplicationNo +
                                              " was not found under employee no." + AccountController.GetEmployeeNo();

                    button1ControllerName = "LeaveApplication";
                    button1ActionName = "LeaveApplicationHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                        button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                        button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [System.Web.Mvc.Authorize]
        public ActionResult EditLeaveApplication(string LeaveApplicationNo)
        {
            try
            {
                if (LeaveApplicationNo.Equals(""))
                {
                    return RedirectToAction("LeaveApplicationHistory", "LeaveApplication");
                }

                if (dynamicsNAVSOAPServices.hrManagementWS.CheckLeaveApplicationExists(LeaveApplicationNo,
                        AccountController.GetEmployeeNo()))
                {
                    string leaveApplicationStatus =
                        dynamicsNAVSOAPServices.hrManagementWS.GetLeaveApplicationStatus(LeaveApplicationNo);

                    //if leave application is released, reopen document
                    //if (leaveApplicationStatus.Equals("Released"))
                    //{
                    //	dynamicsNAVSOAPServices.hrManagementWS.ReopenLeaveApplication(LeaveApplicationNo);
                    //}
                    //if leave application is rejected, reopen document
                    if (leaveApplicationStatus.Equals("Rejected") ||
                        leaveApplicationStatus.Equals("Declined with amendments"))
                    {
                        dynamicsNAVSOAPServices.hrManagementWS
                            .CancelLeaveApplicationApprovalRequest(LeaveApplicationNo);
                    }

                    LeaveApplicationModel leaveApplicationObj = new LeaveApplicationModel();
                    //var leaveApplications = from leaveApplicationsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRLeaveApplications
                    //						where leaveApplicationsQuery.No.Equals(LeaveApplicationNo)
                    //						select leaveApplicationsQuery;
                    dynamic leaveApplications =
                        JsonConvert.DeserializeObject(
                            dynamicsNAVSOAPServices.hrManagementWS.Getleaves(LeaveApplicationNo, ""));
                    foreach (var leaveApplication in leaveApplications)
                    {
                        leaveApplicationObj.No = leaveApplication.No;
                        leaveApplicationObj.EmployeeNo = leaveApplication.EmployeeNo;
                        leaveApplicationObj.EmployeeName = leaveApplication.EmployeeName;
                        leaveApplicationObj.LeaveType = leaveApplication.LeaveType;
                        leaveApplicationObj.LeaveBalance = leaveApplication.LeaveBalance;
                        leaveApplicationObj.DaysApplied = leaveApplication.DaysApplied;
                        leaveApplicationObj.DaysApproved = leaveApplication.DaysApproved;
                        //leaveApplicationObj.LeaveStartDate = leaveApplication.LeaveStartDate.ToString("dd/MM/yy");
                        //leaveApplicationObj.LeaveEndDate = leaveApplication.LeaveEndDate.ToString("dd/MM/yy");
                        //leaveApplicationObj.LeaveReturnDate = leaveApplication.LeaveReturnDate.ToString("dd/MM/yy");
                        leaveApplicationObj.LeaveStartDate =
                            Convert.ToDateTime(leaveApplication.LeaveStartDate).ToString("dd-MM-yy");
                        leaveApplicationObj.LeaveEndDate =
                            Convert.ToDateTime(leaveApplication.LeaveEndDate).ToString("dd-MM-yy");
                        leaveApplicationObj.LeaveReturnDate =
                            Convert.ToDateTime(leaveApplication.LeaveReturnDate).ToString("dd-MM-yy");
                        leaveApplicationObj.ReasonForLeave = leaveApplication.ReasonForLeave;
                        leaveApplicationObj.SentToReliever = leaveApplication.SentToReliever != "No";
                        leaveApplicationObj.RelieverAcknowledgement = leaveApplication.RelieverAcknowledgement != "No";
                        leaveApplicationObj.SubstituteEmployeeNo = leaveApplication.SubstituteEmployeeNo;
                        leaveApplicationObj.SubstituteEmployeeName = leaveApplication.SubstituteEmployeeName;
                        leaveApplicationObj.Status = leaveApplication.Status;
                    }

                    //LoadEmployeeSubstitutes(leaveApplicationObj.EmployeeNo);
                    //LoadEmployeeLeaveTypes(leaveApplicationObj.EmployeeNo);
                    LoadDimensions(leaveApplicationObj.GlobalDimension1Code);

                    //List<LeaveTypes> leaveTypes = new List<LeaveTypes>();
                    ////string dimension1list = "LeaveTypes";
                    //string dimension1list = "LeaveTypes?$filter=Show_in_Portal eq true";

                    //HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(dimension1list);
                    //using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                    //{
                    //    var result1 = streamReader1.ReadToEnd();

                    //    var details1 = JObject.Parse(result1);

                    //    foreach (JObject config1 in details1["value"])
                    //    {
                    //        LeaveTypes DList1 = new LeaveTypes();
                    //        DList1.Code = (string) config1["Code"];
                    //        DList1.Description = (string) config1["Description"];
                    //        leaveTypes.Add(DList1);
                    //    }
                    //}

                    /*List<Employees> substitutes = new List<Employees>();
                    string substituteurl = "Employees?$format=json";
                    //string substituteurl = $"Employees?$format=json&$filter=Company eq '{companyFilter}'";

                    HttpWebResponse httpsubstituteurl = Models.Credentials.GetOdataData(substituteurl);
                    using (var streamReader2 = new StreamReader(httpsubstituteurl.GetResponseStream()))
                    {
                        var result2 = streamReader2.ReadToEnd();

                        var details2 = JObject.Parse(result2);

                        foreach (JObject config2 in details2["value"])
                        {
                            Employees EList = new Employees();
                            EList.No = (string)config2["No"];
                            EList.Full_Name = (string)config2["Full_Name"];
                            substitutes.Add(EList);
                        }
                    }
                    string companyFilter = "SPUR";
                    substitutes = substitutes.Where(e => e.Company == companyFilter).ToList();*/
                    var employee = _bcodataServices.BCOData.HR_Employee.Execute().ToList();
                    var userCompany = employee.Where(c => c.No == employeeNo).FirstOrDefault()?.Company;
                    var substitutes = employee.Where(c => c.Company == userCompany);

                    leaveApplicationObj.Employees = new SelectList(substitutes, "No", "Search_Name",
                        leaveApplicationObj.SubstituteEmployeeNo);

                    var useremployee = _bcodataServices.BCOData.HR_Employee.Execute().ToList();
                    var employeegender = useremployee.Where(c => c.No == employeeNo).FirstOrDefault()?.Gender;
                       

                    leaveApplicationObj.LeaveTypes = _bcodataServices.BCOData.LeaveTypes
                        .Where(c => c.Show_in_Portal == true && (c.Gender == employeegender || c.Gender == "Both")).Select(c => new SelectListItem()
                        {
                            Value = c.Code,
                            Text = c.Description,
                            Selected = leaveApplicationObj.LeaveType == c.Code
                        }).ToList();

                    leaveApplicationObj.GlobalDimension1Codes = new SelectList(globalDimension1Values, "Code", "Name");
                    leaveApplicationObj.GlobalDimension2Codes = new SelectList(globalDimension2Values, "Code", "Name");
                    leaveApplicationObj.ShortcutDimension3Codes =
                        new SelectList(shortcutDimension3Values, "Code", "Name");
                    leaveApplicationObj.ShortcutDimension4Codes =
                        new SelectList(shortcutDimension4Values, "Code", "Name");
                    leaveApplicationObj.ShortcutDimension5Codes =
                        new SelectList(shortcutDimension5Values, "Code", "Name");
                    leaveApplicationObj.ShortcutDimension6Codes =
                        new SelectList(shortcutDimension6Values, "Code", "Name");
                    leaveApplicationObj.ShortcutDimension7Codes =
                        new SelectList(shortcutDimension7Values, "Code", "Name");
                    leaveApplicationObj.ShortcutDimension8Codes =
                        new SelectList(shortcutDimension8Values, "Code", "Name");
                    //leaveApplicationObj.ResponsibilityCenters = new SelectList(responsibilityCenters, "Code", "Description");

                    return View(leaveApplicationObj);
                }
                else
                {
                    responseHeader = "Leave Application NotFound";
                    responseMessage = "The leave application no." + LeaveApplicationNo +
                                      " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "The leave application no." + LeaveApplicationNo +
                                              " was not found under employee no." + AccountController.GetEmployeeNo();

                    button1ControllerName = "LeaveApplication";
                    button1ActionName = "LeaveApplicationHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                        button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                        button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditLeaveApplication(LeaveApplicationModel LeaveApplicationObj, string Command)
        {
            string leaveApplicationNo = "";
            try
            {
                // LoadEmployeeSubstitutes(LeaveApplicationObj.EmployeeNo);
                //LoadDimensions(LeaveApplicationObj.GlobalDimension1Code);
                //LoadEmployeeLeaveTypes(LeaveApplicationObj.EmployeeNo);
                //	LoadResponsibilityCenters();
                /*List<LeaveTypes> leaveTypes = new List<LeaveTypes>();
                string dimension1list = "LeaveTypes?$filter=Show_in_Portal eq true";

                HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(dimension1list);
                using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                {
                    var result1 = streamReader1.ReadToEnd();

                    var details1 = JObject.Parse(result1);

                    foreach (JObject config1 in details1["value"])
                    {
                        LeaveTypes DList1 = new LeaveTypes();
                        DList1.Code = (string)config1["Code"];
                        DList1.Description = (string)config1["Description"];
                        leaveTypes.Add(DList1);
                    }
                }*/
                /*var leaveTypes = _bcodataServices.BCOData.LeaveTypes.Where(c => c.Show_in_Portal == true).Select(c=> new SelectListItem()
                {
	                Value = c.Code,
	                Text = c.Description,
	                Selected = LeaveApplicationObj.LeaveType == c.Code
                }).ToList();*/

                /*List<Employees> substitutes = new List<Employees>();
                string substituteurl = "Employees?$format=json";

                HttpWebResponse httpsubstituteurl = Models.Credentials.GetOdataData(substituteurl);
                using (var streamReader2 = new StreamReader(httpsubstituteurl.GetResponseStream()))
                {
                    var result2 = streamReader2.ReadToEnd();

                    var details2 = JObject.Parse(result2);

                    foreach (JObject config2 in details2["value"])
                    {
                        Employees EList = new Employees();
                        EList.No = (string)config2["No"];
                        EList.Full_Name = (string)config2["Full_Name"];
                        substitutes.Add(EList);
                    }
                }
                string companyFilter = "SPUR";
                substitutes = substitutes.Where(e => e.Company == companyFilter).ToList();*/
                //var employee = _bcodataServices.BCOData.HR_Employee.Execute().ToList();

                //var userCompany = employee.Where(c => c.No == employeeNo).FirstOrDefault()?.Company;
                //var substitutes = employee.Where(c => c.Company == userCompany);

                //LeaveApplicationObj.Employees = new SelectList(substitutes, "No", "Search_Name",
                //    LeaveApplicationObj.SubstituteEmployeeNo);

                //LeaveApplicationObj.LeaveTypes = _bcodataServices.BCOData.LeaveTypes
                //    .Where(c => c.Show_in_Portal == true).Select(c => new SelectListItem()
                //    {
                //        Value = c.Code,
                //        Text = c.Description,
                //        Selected = LeaveApplicationObj.LeaveType == c.Code
                //    }).ToList();

                //LeaveApplicationObj.GlobalDimension1Codes = new SelectList(globalDimension1Values, "Code", "Name");
                //LeaveApplicationObj.GlobalDimension2Codes = new SelectList(globalDimension2Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Values, "Code", "Name");
                //LeaveApplicationObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Values, "Code", "Name");
                //LeaveApplicationObj.ResponsibilityCenters = new SelectList(responsibilityCenters, "Code", "Description");

                //if (ModelState.IsValid)
                //{
                    //LeaveApplicationObj.ReasonForLeave = LeaveApplicationObj.ReasonForLeave != null
                    //    ? LeaveApplicationObj.ReasonForLeave
                    //    : "";
                    //LeaveApplicationObj.SubstituteEmployeeNo = LeaveApplicationObj.SubstituteEmployeeNo != null
                    //    ? LeaveApplicationObj.SubstituteEmployeeNo
                    //    : "";

                    leaveApplicationNo = LeaveApplicationObj.No;

                    if (leaveApplicationNo.Equals(""))
                    {
                        return RedirectToAction("LeaveApplicationHistory", "LeaveApplication");
                    }

          
                    
                    var employee = _bcodataServices.BCOData.HR_Employee.Execute().ToList();
                    var userCompany = employee.Where(c => c.No == employeeNo).FirstOrDefault()?.Company;
                    var substitutes = employee.Where(c => c.Company == userCompany);
                    LeaveApplicationObj.Employees = new SelectList(substitutes, "No", "Search_Name",
                        LeaveApplicationObj.SubstituteEmployeeNo);

                    var useremployee = _bcodataServices.BCOData.HR_Employee.Execute().ToList();
                    var employeegender = useremployee.Where(c => c.No == employeeNo).FirstOrDefault()?.Gender;
                    var employeeuser = useremployee.Where(c => c.No == employeeNo).FirstOrDefault()?.User_ID;


                LeaveApplicationObj.LeaveTypes = _bcodataServices.BCOData.LeaveTypes
                      .Where(c => c.Show_in_Portal == true && (c.Gender == employeegender || c.Gender == "Both")).Select(c => new SelectListItem()
                      {
                          Value = c.Code,
                          Text = c.Description,
                          Selected = LeaveApplicationObj.LeaveType == c.Code
                      }).ToList();
                //DateTime.ParseExact(LeaveApplicationObj.LeaveStartDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture)


                if (Command.Equals("Submit For Approval"))
                    {
                        //bool leaveApplicationModified = dynamicsNAVSOAPServices.hrManagementWS.ModifyLeaveApplication(
                        //    LeaveApplicationObj.No, LeaveApplicationObj.LeaveType,
                        //    DateTime.Parse(LeaveApplicationObj.LeaveStartDate),
                        //    Convert.ToDecimal(LeaveApplicationObj.DaysApplied), LeaveApplicationObj.ReasonForLeave ?? ""
                        //);

                        if (!dynamicsNAVSOAPServices.hrManagementWS.CheckLeaveApplicationApprovalWorkflowEnabled(
                                leaveApplicationNo))
                        {
                            LeaveApplicationObj.No = leaveApplicationNo;
                            LeaveApplicationObj.ErrorStatus = true;
                            LeaveApplicationObj.ErrorMessage =
                                "An error was experienced when sending your leave application no." +
                                leaveApplicationNo + " for approval. Try again or contact the " + companyName +
                                " ICT department.";
                           // return View(LeaveApplicationObj);
                        }

                        if (dynamicsNAVSOAPServices.hrManagementWS.SendLeaveApplicationApprovalRequest(
                                leaveApplicationNo))
                        {
                            responseHeader = "Success";
                            responseMessage =
                                "Your leave application was successfully sent for approval. Once approved, you will receive an email containing your leave details.";
                            detailedResponseMessage =
                                "Your leave application was successfully sent for approval. Once approved, you will receive an email containing your leave details.";

                            button1ControllerName = "LeaveApplication";
                            button1ActionName = "LeaveApplicationHistory";
                            button1HasParameters = false;
                            button1Parameters = "";
                            button1Name = "Ok";

                            button2ControllerName = "";
                            button2ActionName = "";
                            button2HasParameters = false;
                            button2Parameters = "";
                            button2Name = "";

                            return successResponse.ApplicationSuccess(responseHeader, responseMessage,
                                detailedResponseMessage,
                                button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                                button1Name,
                                button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                                button2Name);
                        }
                    }

                    if (Command.Equals("View Attachment"))
                    {
                        string fileURL =
                            dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(LeaveApplicationObj.No);

                        string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

                        if (!fileURL.Equals(""))
                        {
                            using (WebClient wc = new WebClient())
                            {
                                if (ext.Equals(".pdf"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/pdf");
                                }

                                else if (ext.Equals(".doc"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/msword");
                                }

                                else if (ext.Equals(".docx"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr,
                                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                                }

                                else if (ext.Equals(".jpeg") || ext.Equals(".jpg"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "image/jpeg");
                                }

                                else if (ext.Equals(".json"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/json");
                                }

                                else if (ext.Equals(".ppt"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/vnd.ms-powerpoint");
                                }

                                else if (ext.Equals(".png"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "image/png");
                                }

                                else if (ext.Equals(".pptx"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr,
                                        "application/vnd.openxmlformats-officedocument.presentationml.presentation");
                                }

                                else if (ext.Equals(".rar"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/vnd.rar");
                                }

                                else if (ext.Equals(".xls"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "application/vnd.ms-excel");
                                }

                                else if (ext.Equals(".xlsx"))
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr,
                                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                                }

                                else
                                {
                                    var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                    return File(byteArr, "text/plain");
                                }
                            }
                        }


                        else
                        {
                            return View(LeaveApplicationObj);
                        }
                   // }
                   // else
                    //{
                    //    LeaveApplicationObj.No = leaveApplicationNo;
                    //    //LeaveApplicationObj.ErrorStatus = true;
                    //    //	LeaveApplicationObj.ErrorMessage = "An error was experienced when sending your leave application for approval. Try again or contact the " + companyName + " ICT department.";
                    //    return View(LeaveApplicationObj);
                    //}
                }
                else
                {
                    return View(LeaveApplicationObj);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //LeaveApplicationObj.No = leaveApplicationNo;
                //LeaveApplicationObj.ErrorStatus = true;
                //LeaveApplicationObj.ErrorMessage = ex.Message;
                //return View(LeaveApplicationObj);
            }
        }

        #endregion Edit Leave Application

        #region View Leave Application

        [System.Web.Mvc.Authorize]
        public ActionResult ViewLeaveApplication(string LeaveApplicationNo)
        {
            try
            {
                if (LeaveApplicationNo.Equals(""))
                {
                    return RedirectToAction("LeaveApplicationHistory", "LeaveApplication");
                }

                if (dynamicsNAVSOAPServices.hrManagementWS.CheckLeaveApplicationExists(LeaveApplicationNo,
                        AccountController.GetEmployeeNo()))
                {
                    LeaveApplicationModel leaveApplicationObj = new LeaveApplicationModel();
                    //var leaveApplications = from leaveApplicationsQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRLeaveApplications
                    //						where leaveApplicationsQuery.No.Equals(LeaveApplicationNo)
                    //						select leaveApplicationsQuery;
                    dynamic leaveApplications =
                        JsonConvert.DeserializeObject(
                            dynamicsNAVSOAPServices.hrManagementWS.Getleaves(LeaveApplicationNo, ""));
                    foreach (var leaveApplication in leaveApplications)
                    {
                        leaveApplicationObj.No = leaveApplication.No;
                        leaveApplicationObj.EmployeeNo = leaveApplication.EmployeeNo;
                        leaveApplicationObj.EmployeeName = leaveApplication.EmployeeName;
                        leaveApplicationObj.LeaveType = leaveApplication.LeaveType;
                        leaveApplicationObj.LeaveBalance = leaveApplication.LeaveBalance;
                        leaveApplicationObj.DaysApplied = leaveApplication.DaysApplied;
                        leaveApplicationObj.DaysApproved = leaveApplication.DaysApproved;
                        //leaveApplicationObj.LeaveStartDate = leaveApplication.LeaveStartDate;
                        //leaveApplicationObj.LeaveEndDate = leaveApplication.LeaveEndDate;
                        //leaveApplicationObj.LeaveReturnDate = leaveApplication.LeaveReturnDate;
                        leaveApplicationObj.LeaveStartDate =
                            Convert.ToDateTime(leaveApplication.LeaveStartDate).ToString("dd-MM-yy");
                        leaveApplicationObj.LeaveEndDate =
                            Convert.ToDateTime(leaveApplication.LeaveEndDate).ToString("dd-MM-yy");
                        leaveApplicationObj.LeaveReturnDate =
                            Convert.ToDateTime(leaveApplication.LeaveReturnDate).ToString("dd-MM-yy");
                        leaveApplicationObj.Comments = leaveApplication.RejectionComments;
                        leaveApplicationObj.ReasonForLeave = leaveApplication.ReasonForLeave;
                        leaveApplicationObj.SubstituteEmployeeNo = leaveApplication.SubstituteEmployeeNo;
                        leaveApplicationObj.RelieverAcknowledgement = leaveApplication.RelieverAcknowledgement != "No";
                        leaveApplicationObj.SentToReliever = leaveApplication.SentToReliever != "No";
                        leaveApplicationObj.SubstituteEmployeeName = leaveApplication.SubstituteEmployeeName;
                        leaveApplicationObj.Comments =
                            dynamicsNAVSOAPServices.ApprovalsMgmt.RejectionComments(LeaveApplicationNo);
                        leaveApplicationObj.Status = leaveApplication.Status;
                    }

                    //LoadEmployeeSubstitutes(leaveApplicationObj.EmployeeNo);
                    //LoadEmployeeLeaveTypes(leaveApplicationObj.EmployeeNo);
                    List<LeaveTypes> leaveTypes = new List<LeaveTypes>();
                    string dimension1list = "LeaveTypes?$filter=Show_in_Portal eq true";

                    HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(dimension1list);
                    using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                    {
                        var result1 = streamReader1.ReadToEnd();

                        var details1 = JObject.Parse(result1);

                        foreach (JObject config1 in details1["value"])
                        {
                            LeaveTypes DList1 = new LeaveTypes();
                            DList1.Code = (string)config1["Code"];
                            DList1.Description = (string)config1["Description"];
                            leaveTypes.Add(DList1);
                        }
                    }

                    /*List<Employees> substitutes = new List<Employees>();
                    string substituteurl = "Employees?$format=json";

                    HttpWebResponse httpsubstituteurl = Models.Credentials.GetOdataData(substituteurl);
                    using (var streamReader2 = new StreamReader(httpsubstituteurl.GetResponseStream()))
                    {
                        var result2 = streamReader2.ReadToEnd();

                        var details2 = JObject.Parse(result2);

                        foreach (JObject config2 in details2["value"])
                        {
                            Employees EList = new Employees();
                            EList.No = (string)config2["No"];
                            EList.Full_Name = (string)config2["Full_Name"];
                            substitutes.Add(EList);
                        }
                    }
                    string companyFilter = "SPUR";
                    substitutes = substitutes.Where(e => e.Company == companyFilter).ToList();*/
                    var employee = _bcodataServices.BCOData.HR_Employee.Execute().ToList();
                    var userCompany = employee.Where(c => c.No == employeeNo).FirstOrDefault()?.Company;
                    var substitutes = employee.Where(c => c.Company == userCompany);

                    leaveApplicationObj.Employees = new SelectList(substitutes, "No", "Search_Name");
                    leaveApplicationObj.EmployeeLeaveTypes = new SelectList(leaveTypes, "Code", "Description");

                    return View(leaveApplicationObj);
                }
                else
                {
                    responseHeader = "Leave Application NotFound";
                    responseMessage = "The leave application no." + LeaveApplicationNo +
                                      " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "The leave application no." + LeaveApplicationNo +
                                              " was not found under employee no." + AccountController.GetEmployeeNo();

                    button1ControllerName = "LeaveApplication";
                    button1ActionName = "LeaveApplicationHistory";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Ok";

                    button2ControllerName = "";
                    button2ActionName = "";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                        button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                        button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize]
        public async Task<ActionResult> ViewLeaveApplication(LeaveApplicationModel leaveApplicationObj2, string Command)
        {
            try
            {
                if (leaveApplicationObj2.No.Equals(""))
                {
                    return RedirectToAction("ViewLeaveApplication", "LeaveApplication");
                }

                if (Command.Equals("View Attachment"))
                {
                    string fileURL =
                        dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(leaveApplicationObj2.No);

                    string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

                    if (!fileURL.Equals(""))
                    {
                        using (WebClient wc = new WebClient())
                        {
                            if (ext.Equals(".pdf"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/pdf");
                            }

                            else if (ext.Equals(".doc"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/msword");
                            }

                            else if (ext.Equals(".docx"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr,
                                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                            }

                            else if (ext.Equals(".jpeg") || ext.Equals(".jpg"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "image/jpeg");
                            }

                            else if (ext.Equals(".json"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/json");
                            }

                            else if (ext.Equals(".ppt"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/vnd.ms-powerpoint");
                            }

                            else if (ext.Equals(".png"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "image/png");
                            }

                            else if (ext.Equals(".pptx"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr,
                                    "application/vnd.openxmlformats-officedocument.presentationml.presentation");
                            }

                            else if (ext.Equals(".rar"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/vnd.rar");
                            }

                            else if (ext.Equals(".xls"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/vnd.ms-excel");
                            }

                            else if (ext.Equals(".xlsx"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr,
                                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                            }

                            else
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "text/plain");
                            }
                        }
                    }


                    else
                    {
                        return View(leaveApplicationObj2);
                    }
                }
                else
                {
                    leaveApplicationObj2.ErrorStatus = true;
                    //leaveApplicationObj2.ErrorMessage = "Unable to process the approve/reject action. Contact the " + companyName + " for assistance.";
                    return View(leaveApplicationObj2);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        #endregion View Leave Application

        #region Leave Applications

        [System.Web.Mvc.Authorize]
        public ActionResult LeaveApplicationHistory()
        {
            bool enabled = true;
            if (!enabled)
            {
                responseHeader = "Leave Application Disabled";
                responseMessage = "The leave application service is not available at the moment. Contact the " +
                                  companyName + "ICT department for assistance.";
                detailedResponseMessage = "The leave application service is not available at the moment. Contact the " +
                                          companyName + "ICT department for assistance.";

                button1ControllerName = "LeaveApplication";
                button1ActionName = "LeaveApplicationHistory";
                button1HasParameters = false;
                button1Parameters = "";
                button1Name = "Ok";

                button2ControllerName = "";
                button2ActionName = "";
                button2HasParameters = false;
                button2Parameters = "";
                button2Name = "";
                return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
            }

            try
            {
                List<LeaveApplicationModel> leaveApplicationList = new List<LeaveApplicationModel>();

                dynamic leaveApplications =
                    JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.hrManagementWS.Getleaves("", employeeNo));

                foreach (var leaveApplication in leaveApplications)
                {
                    LeaveApplicationModel leaveApplicationObj = new LeaveApplicationModel();
                    leaveApplicationObj.No = leaveApplication.No;
                    leaveApplicationObj.EmployeeNo = leaveApplication.EmployeeNo;
                    leaveApplicationObj.EmployeeName = leaveApplication.EmployeeName;
                    leaveApplicationObj.LeaveType = leaveApplication.LeaveType;
                    leaveApplicationObj.LeaveBalance = leaveApplication.LeaveBalance;
                    leaveApplicationObj.DaysApplied = leaveApplication.DaysApplied;
                    leaveApplicationObj.DaysApproved = leaveApplication.DaysApproved;
                    leaveApplicationObj.LeaveStartDate = leaveApplication.LeaveStartDate.ToString("dd-MM-yy");
                    leaveApplicationObj.LeaveEndDate = leaveApplication.LeaveEndDate.ToString("dd-MM-yy");
                    leaveApplicationObj.LeaveReturnDate = leaveApplication.LeaveReturnDate.ToString("dd-MM-yy");
                    leaveApplicationObj.ReasonForLeave = leaveApplication.ReasonForLeave;
                    leaveApplicationObj.Comments = leaveApplication.RejectionComments;
                    leaveApplicationObj.RelieverAcknowledgement = leaveApplication.RelieverAcknowledgement != "No";
                    leaveApplicationObj.SentToReliever = leaveApplication.SentToReliever != "No";
                    leaveApplicationObj.SubstituteEmployeeNo = leaveApplication.SubstituteEmployeeNo;
                    leaveApplicationObj.SubstituteEmployeeName = leaveApplication.SubstituteEmployeeName;
                    //leaveApplicationObj.Comments =
                    //    dynamicsNAVSOAPServices.ApprovalsMgmt.RejectionComments(leaveApplicationObj.No);
                    leaveApplicationObj.Status = leaveApplication.Status;

                    if (leaveApplicationObj.Status == "Rejected")
                    {
                        leaveApplicationObj.Status = "Declined with amendments";
                    }
                    else if (leaveApplicationObj.Status == "Posted" || leaveApplicationObj.Status == "Released")
                    {
                        leaveApplicationObj.Status = "Approved";
                    }
                    else
                        leaveApplicationObj.Status = leaveApplication.Status;

                    leaveApplicationList.Add(leaveApplicationObj);
                }

                return View(leaveApplicationList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        #endregion Leave Applications

        #region View Leave Approval

        [System.Web.Mvc.Authorize]
        public ActionResult LeaveApplicationApproval(string LeaveApplicationNo)
        {
            try
            {
                if (LeaveApplicationNo.Equals(""))
                {
                    return RedirectToAction("OpenEntries", "Approval");
                }

                LeaveApplicationModel leaveApplicationObj = new LeaveApplicationModel();
                dynamic leaveApplications =
                    JsonConvert.DeserializeObject(
                        dynamicsNAVSOAPServices.hrManagementWS.Getleaves(LeaveApplicationNo, ""));
                foreach (var leaveApplication in leaveApplications)
                {
                    leaveApplicationObj.No = leaveApplication.No;
                    leaveApplicationObj.EmployeeNo = leaveApplication.EmployeeNo;
                    leaveApplicationObj.EmployeeName = leaveApplication.EmployeeName;
                    leaveApplicationObj.LeaveType = leaveApplication.LeaveType;
                    leaveApplicationObj.LeaveBalance = leaveApplication.LeaveBalance;
                    leaveApplicationObj.DaysApplied = leaveApplication.DaysApplied;
                    leaveApplicationObj.DaysApproved = leaveApplication.DaysApproved;
                    leaveApplicationObj.LeaveStartDate = leaveApplication.LeaveStartDate.ToString("dd-MM-yy");
                    leaveApplicationObj.LeaveEndDate = leaveApplication.LeaveEndDate.ToString("dd-MM-yy");
                    leaveApplicationObj.LeaveReturnDate = leaveApplication.LeaveReturnDate.ToString("dd-MM-yy");
                    leaveApplicationObj.ReasonForLeave = leaveApplication.ReasonForLeave;
                    leaveApplicationObj.RelieverAcknowledgement = leaveApplication.RelieverAcknowledgement != "No";
                    leaveApplicationObj.SentToReliever = leaveApplication.SentToReliever != "No";
                    leaveApplicationObj.SubstituteEmployeeNo = leaveApplication.SubstituteEmployeeNo;
                    leaveApplicationObj.SubstituteEmployeeName = leaveApplication.SubstituteEmployeeName;
                    leaveApplicationObj.Status = leaveApplication.Status;
                }

                //LoadEmployeeSubstitutes(leaveApplicationObj.EmployeeNo);
                //LoadEmployeeLeaveTypes(leaveApplicationObj.EmployeeNo);
                LoadDimensions(leaveApplicationObj.GlobalDimension1Code);
                List<LeaveTypes> leaveTypes = new List<LeaveTypes>();
                string dimension1list = "LeaveTypes?$filter=Show_in_Portal eq true";

                HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(dimension1list);
                using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                {
                    var result1 = streamReader1.ReadToEnd();

                    var details1 = JObject.Parse(result1);

                    foreach (JObject config1 in details1["value"])
                    {
                        LeaveTypes DList1 = new LeaveTypes();
                        DList1.Code = (string)config1["Code"];
                        DList1.Description = (string)config1["Description"];
                        leaveTypes.Add(DList1);
                    }
                }


                var employee = _bcodataServices.BCOData.HR_Employee.Execute().ToList();
                var userCompany = employee.Where(c => c.No == employeeNo).FirstOrDefault()?.Company;
                var substitutes = employee.Where(c => c.Company == userCompany);

                leaveApplicationObj.Employees = new SelectList(substitutes, "No", "Search_Name",
                    leaveApplicationObj.SubstituteEmployeeNo);
                //leaveApplicationObj.LeaveTypes = new SelectList(leaveTypes, "Code", "Description");
                leaveApplicationObj.LeaveTypes = _bcodataServices.BCOData.LeaveTypes
                    .Where(c => c.Show_in_Portal == true).Select(c => new SelectListItem()
                    {
                        Value = c.Code,
                        Text = c.Description,
                        Selected = leaveApplicationObj.LeaveType == c.Code
                    }).ToList();
                leaveApplicationObj.GlobalDimension1Codes = new SelectList(globalDimension1Values, "Code", "Name");
                leaveApplicationObj.GlobalDimension2Codes = new SelectList(globalDimension2Values, "Code", "Name");
                leaveApplicationObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Values, "Code", "Name");
                leaveApplicationObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Values, "Code", "Name");
                leaveApplicationObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Values, "Code", "Name");
                leaveApplicationObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Values, "Code", "Name");
                leaveApplicationObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Values, "Code", "Name");
                leaveApplicationObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Values, "Code", "Name");

                return View(leaveApplicationObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> LeaveApplicationApproval(LeaveApplicationModel LeaveApplicationObj,
            string Command)
        {
            try
            {
                if (LeaveApplicationObj.No.Equals(""))
                {
                    return RedirectToAction("OpenEntries", "Approval");
                }
                var useremployee = _bcodataServices.BCOData.UserSetupQuery.Execute().ToList();
                var employeeuser = useremployee.Where(c => c.Employee_No == employeeNo).FirstOrDefault()?.User_ID;

                if (Command == "Approve")
                {
                    // test field
                    LeaveApplicationObj.Comments =
                        LeaveApplicationObj.Comments != null ? LeaveApplicationObj.Comments : "";

                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveLeave(employeeuser, LeaveApplicationObj.No,
                            AccountController.GetEmployeeNo()))
                    {
                        responseHeader = "Success";
                        responseMessage = "Leave application no." + LeaveApplicationObj.No +
                                          " was successfully approved.";
                        detailedResponseMessage = "Leave application no." + LeaveApplicationObj.No +
                                                  " was successfully approved.";

                        button1ControllerName = "Approval";
                        button1ActionName = "OpenEntries";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";
                        return successResponse.ApplicationSuccess(responseHeader, responseMessage,
                            detailedResponseMessage,
                            button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                            button1Name,
                            button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                            button2Name);
                    }
                    else
                    {
                        LeaveApplicationObj.ErrorStatus = true;
                        LeaveApplicationObj.ErrorMessage =
                            "Unable to process the leave application approve action. Contact the " + companyName +
                            " for assistance.";
                        return View(LeaveApplicationObj);
                    }
                }
                else if (Command == "Reject")
                {
                    LeaveApplicationObj.Comments =
                        LeaveApplicationObj.Comments != null ? LeaveApplicationObj.Comments : "";
                    if (LeaveApplicationObj.Comments.Equals(""))
                    {
                        dynamic leaveApplications =
                            JsonConvert.DeserializeObject(
                                dynamicsNAVSOAPServices.hrManagementWS.Getleaves(LeaveApplicationObj.No, ""));
                        foreach (var leaveApplication in leaveApplications)
                        {
                            LeaveApplicationObj.No = leaveApplication.No;
                            LeaveApplicationObj.EmployeeNo = leaveApplication.EmployeeNo;
                            LeaveApplicationObj.EmployeeName = leaveApplication.EmployeeName;
                            LeaveApplicationObj.LeaveType = leaveApplication.LeaveType;
                            LeaveApplicationObj.LeaveBalance = leaveApplication.LeaveBalance;
                            LeaveApplicationObj.DaysApplied = leaveApplication.DaysApplied;
                            LeaveApplicationObj.DaysApproved = leaveApplication.DaysApproved;
                            LeaveApplicationObj.LeaveEndDate = leaveApplication.LeaveEndDate;
                            LeaveApplicationObj.LeaveReturnDate = leaveApplication.LeaveReturnDate;
                            LeaveApplicationObj.LeaveStartDate = leaveApplication.LeaveStartDate.ToString("dd-MM-yy");
                            LeaveApplicationObj.LeaveEndDate = leaveApplication.LeaveEndDate.ToString("dd-MM-yy");
                            LeaveApplicationObj.LeaveReturnDate = leaveApplication.LeaveReturnDate.ToString("dd-MM-yy");
                            LeaveApplicationObj.ReasonForLeave = leaveApplication.ReasonForLeave;
                            LeaveApplicationObj.RelieverAcknowledgement = leaveApplication.RelieverAcknowledgement != "No";
                            LeaveApplicationObj.SentToReliever = leaveApplication.SentToReliever != "No";
                            LeaveApplicationObj.SubstituteEmployeeNo = leaveApplication.SubstituteEmployeeNo;
                            LeaveApplicationObj.SubstituteEmployeeName = leaveApplication.SubstituteEmployeeName;
                            LeaveApplicationObj.Status = leaveApplication.Status;
                        }

                        LeaveApplicationObj.ErrorStatus = true;
                        LeaveApplicationObj.ErrorMessage =
                            "Kindly provide reason (s) for declining/rejecting this document.";
                        return View(LeaveApplicationObj);
                    }

                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectLeave(employeeuser, LeaveApplicationObj.No,
                            LeaveApplicationObj.Comments,employeeNo))
                    {
                        responseHeader = "Success";
                        responseMessage = "Leave application no." + LeaveApplicationObj.No +
                                          " was successfully rejected.";
                        detailedResponseMessage = "Leave application no." + LeaveApplicationObj.No +
                                                  " was successfully rejected.";

                        button1ControllerName = "Approval";
                        button1ActionName = "OpenEntries";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return successResponse.ApplicationSuccess(responseHeader, responseMessage,
                            detailedResponseMessage,
                            button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                            button1Name,
                            button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                            button2Name);
                    }
                    else
                    {
                        LeaveApplicationObj.ErrorStatus = true;
                        LeaveApplicationObj.ErrorMessage =
                            "Unable to process the leave application reject action. Contact the " + companyName +
                            " for assistance.";
                        return View(LeaveApplicationObj);
                    }
                }
                else if (Command == "View Attachment")
                {
                    string fileURL =
                        dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(LeaveApplicationObj.No);

                    string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

                    if (!fileURL.Equals(""))
                    {
                        using (WebClient wc = new WebClient())
                        {
                            if (ext.Equals(".pdf"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/pdf");
                            }

                            else if (ext.Equals(".doc"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/msword");
                            }

                            else if (ext.Equals(".docx"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr,
                                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                            }

                            else if (ext.Equals(".jpeg") || ext.Equals(".jpg"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "image/jpeg");
                            }

                            else if (ext.Equals(".json"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/json");
                            }

                            else if (ext.Equals(".ppt"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/vnd.ms-powerpoint");
                            }

                            else if (ext.Equals(".png"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "image/png");
                            }

                            else if (ext.Equals(".pptx"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr,
                                    "application/vnd.openxmlformats-officedocument.presentationml.presentation");
                            }

                            else if (ext.Equals(".rar"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/vnd.rar");
                            }

                            else if (ext.Equals(".xls"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "application/vnd.ms-excel");
                            }

                            else if (ext.Equals(".xlsx"))
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr,
                                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                            }

                            else
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(fileURL);
                                return File(byteArr, "text/plain");
                            }
                        }
                    }


                    else
                    {
                        return View(LeaveApplicationObj);
                    }
                }

                else
                {
                    LeaveApplicationObj.ErrorStatus = true;
                    LeaveApplicationObj.ErrorMessage = "Unable to process the approve/reject action. Contact the " +
                                                       companyName + " for assistance.";
                    return View(LeaveApplicationObj);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        #endregion Leave Application Approval

        #region Document Management

        [System.Web.Mvc.Authorize]
        public JsonResult GetLeaveApplicationDocuments(string DocumentNo)
        {
            try
            {
                List<DocumentMgmtModel> documentManagementList = new List<DocumentMgmtModel>();

                var PortalDocs = from DocQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
                                 where DocQuery.DocumentNo.Equals(DocumentNo)
                                 select DocQuery;
                foreach (PortalDocuments LinesTwo in PortalDocs)
                {
                    DocumentMgmtModel documentManagementObj = new DocumentMgmtModel();
                    documentManagementObj.DocumentNo = LinesTwo.DocumentNo;
                    documentManagementObj.DocumentCode = LinesTwo.Document_Code;
                    documentManagementObj.DocumentAttached = LinesTwo.Document_Attached ?? false;
                    documentManagementObj.DocumentDescription = LinesTwo.Document_Description;
                    documentManagementObj.FileName = LinesTwo.File_Name;
                    documentManagementList.Add(documentManagementObj);
                }

                return Json(documentManagementList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception )
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        public JsonResult UploadLeaveApplicationDocument(string DocumentNo, string DocumentCode,
            string DocumentDescription)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var root = "~/StaffData/";
                    bool folderpath = Directory.Exists(HttpContext.Server.MapPath(root));

                    if (!folderpath)
                    {
                        Directory.CreateDirectory(HttpContext.Server.MapPath(root));
                    }

                    var file = Request.Files[0];
                    string fileExt = Path.GetExtension(file.FileName).ToLower();
                    string fileName = "Leave Application-" + DocumentCode + fileExt;
                    string path = Path.Combine(HttpContext.Server.MapPath(root), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    if (fileExt == ".pdf" || fileExt == ".eml" || fileExt == ".xlsx" || fileExt == ".csv" ||
                        fileExt == ".rtf" || fileExt == ".doc" || fileExt == ".docx" || fileExt == ".jpg" ||
                        fileExt == ".jpeg" || fileExt == ".png" || fileExt == ".msg")
                    {
                        file.SaveAs(path);

                        if (System.IO.File.Exists(path))
                        {
                            bool ret = false;
                            ret = dynamicsNAVSOAPServices.documentMgmt.InsertImprestAttachment(DocumentNo, path,
                                51525209, "Leave Application");
                            //dynamicsNAVSOAPServices.documentMgmt.UploadFileToSharePointAndNAV(DocumentNo, DocumentCode, fileName, path);
                            if (ret)
                            {
                                return Json(
                                    new { success = true, message = DocumentDescription + " uploaded successfully" },
                                    JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { success = false, message = DocumentDescription + " was not uploaded" },
                                    JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { success = false, message = DocumentDescription + " was not uploaded" },
                                JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Unsupported file format" },
                            JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = false, message = "Please select a pdf file!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [System.Web.Mvc.Authorize]
        public ActionResult GetLeaveApplicationDocumentByLineNo(string DocumentNo, string DocumentCode)
        {
            try
            {
                DocumentMgmtModel documentManagementObj = new DocumentMgmtModel();
                dynamic portalDocuments = JsonConvert.DeserializeObject(
                    dynamicsNAVSOAPServices.documentMgmt.GetLeaveApplicationDocument(DocumentNo, DocumentCode));

                foreach (var portalDocument in portalDocuments)
                {
                    documentManagementObj.DocumentNo = portalDocument.DocumentNo;
                    documentManagementObj.DocumentCode = portalDocument.DocumentCode;
                    documentManagementObj.DocumentAttached = portalDocument.DocumentAttached;
                    documentManagementObj.DocumentDescription = portalDocument.DocumentDescription;
                    documentManagementObj.FileName = portalDocument.FileName;
                }

                return Json(documentManagementObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [ChildActionOnly]
        [System.Web.Mvc.Authorize]
        public ActionResult _LeaveApplicationDocument(string LeaveApplicationNo)
        {
            //check if attachment exists
            DocumentMgmtModel documentManagementObj = new DocumentMgmtModel();

            #region Attachments

            List<DocAttachments> DocAttachments = new List<DocAttachments>();
            string dimension1list = "DocAttachments?$filter=No eq '" + LeaveApplicationNo +
                                    "' and Table_ID eq 51525209 &$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(dimension1list);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    DocAttachments DList1 = new DocAttachments();
                    DList1.No = (string)config1["No"];
                    DList1.Doc_Attachment_Description = (string)config1["Doc_Attachment_Description"];
                    DocAttachments.Add(DList1);

                    documentManagementObj.DocumentNo = (string)config1["No"];
                    documentManagementObj.DocumentCode = (string)config1["No"];
                    documentManagementObj.DocumentDescription = (string)config1["Doc_Attachment_Description"];
                    documentManagementObj.DocumentAttached = true;
                }
            }

            #endregion


            return PartialView(documentManagementObj);
        }

        [ChildActionOnly]
        [System.Web.Mvc.Authorize]
        public ActionResult _ViewLeaveApplicationDocument(string DocumentNo)
        {
            DocumentMgmtModel documentManagementObj = new DocumentMgmtModel();
            return PartialView(documentManagementObj);
        }

        #endregion End Document Management

        #region Helper Functions

        [System.Web.Mvc.Authorize]
        public JsonResult InsertLeaveApplicationDocuments(string LeaveApplicationNo, string LeaveType)
        {
            bool leaveApplicationDocumentsInserted =
                dynamicsNAVSOAPServices.documentMgmt.InsertLeaveApplicationDocument(LeaveApplicationNo, LeaveType);
            return Json(leaveApplicationDocumentsInserted, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.Authorize]
        [ChildActionOnly]
        public ActionResult _employeeSplittedLeaveBalance()
        {
            EmployeeSplittedLeaveBalance employeeSplittedLeaveBalance = new EmployeeSplittedLeaveBalance();
            return PartialView(employeeSplittedLeaveBalance);
        }

        [System.Web.Mvc.Authorize]
        public JsonResult getEmployeeLeaveSplittedBalance(string leaveType)
        {
            List<EmployeeSplittedLeaveBalance> employeeLeaveSplittedBalances = new List<EmployeeSplittedLeaveBalance>();

            dynamic employeeSplittedLeaves = JsonConvert.DeserializeObject(
                dynamicsNAVSOAPServices.hrManagementWS.GetEmployeeSplittedLeaveBalances(employeeNo, leaveType));
            if (employeeSplittedLeaves != null)
            {
                foreach (var employeeSplittedLeave in employeeSplittedLeaves)
                {
                    EmployeeSplittedLeaveBalance employeeSplittedLeaveBalance = new EmployeeSplittedLeaveBalance();
                    employeeSplittedLeaveBalance.DaysEntitlement = employeeSplittedLeave.DaysEntitlement;
                    employeeSplittedLeaveBalance.DaysEarned = employeeSplittedLeave.DaysEarned;
                    employeeSplittedLeaveBalance.DaysTakenToDate = employeeSplittedLeave.DaysTakenToDate;
                    employeeSplittedLeaveBalance.DaysAvailable = employeeSplittedLeave.DaysAvailable;
                    employeeSplittedLeaveBalance.AnnualLeaveDaysCarriedForward =
                    employeeSplittedLeave.AnnualLeaveDaysCarriedForward;
                    employeeSplittedLeaveBalance.CurrentLeavePeriod =
                        dynamicsNAVSOAPServices.hrManagementWS.GetCurrentLeavePeriod();

                    employeeLeaveSplittedBalances.Add(employeeSplittedLeaveBalance);
                }
            }
                

            return Json(employeeLeaveSplittedBalances.ToList(), JsonRequestBehavior.AllowGet);
            //return PartialView(employeeLeaveSplittedBalances);
        }

        //[System.Web.Mvc.Authorize]
        //public PartialViewResult getEmployeeLeaveSplittedBalance(string leaveType = "PATERNITY")
        //{
        //    List<EmployeeSplittedLeaveBalance> employeeLeaveSplittedBalances = new List<EmployeeSplittedLeaveBalance>();

        //    dynamic employeeSplittedLeaves = JsonConvert.DeserializeObject(
        //        dynamicsNAVSOAPServices.hrManagementWS.GetEmployeeSplittedLeaveBalances(employeeNo, leaveType));

        //    foreach (var employeeSplittedLeave in employeeSplittedLeaves)
        //    {
        //        EmployeeSplittedLeaveBalance employeeSplittedLeaveBalance = new EmployeeSplittedLeaveBalance();
        //        employeeSplittedLeaveBalance.DaysEntitlement = employeeSplittedLeave.DaysEntitlement;
        //        employeeSplittedLeaveBalance.DaysEarned = employeeSplittedLeave.DaysEarned;
        //        employeeSplittedLeaveBalance.DaysTakenToDate = employeeSplittedLeave.DaysTakenToDate;
        //        employeeSplittedLeaveBalance.DaysAvailable = employeeSplittedLeave.DaysAvailable;
        //        employeeSplittedLeaveBalance.AnnualLeaveDaysCarriedForward =
        //            employeeSplittedLeave.AnnualLeaveDaysCarriedForward;
        //        employeeSplittedLeaveBalance.CurrentLeavePeriod =
        //            dynamicsNAVSOAPServices.hrManagementWS.GetCurrentLeavePeriod();

        //        employeeLeaveSplittedBalances.Add(employeeSplittedLeaveBalance);
        //    }

        //    return PartialView("_employeeSplittedLeaveBalance", employeeLeaveSplittedBalances);
        //}

        public JsonResult GetEmployeeLeaveTypes(string EmployeeNo)
        {
            var employeeLeaveTypes =
                from employeeLeaveTypeQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeLeaveTypes
                where employeeLeaveTypeQuery.Employee_No.Equals(EmployeeNo)
                select employeeLeaveTypeQuery;
            return Json(employeeLeaveTypes.ToList(), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetLeaveEndDate(string EmployeeNo, string LeaveType, string LeaveStartDate,
        //    Decimal DaysApplied)
        //{
        //    //DateTime leaveEndDate = dynamicsNAVSOAPServices.hrManagementWS.GetLeaveEndDate(EmployeeNo, LeaveType,
        //    //    DateTime.Parse(LeaveStartDate), DaysApplied);

        //    //return Json(leaveEndDate.ToString("dd-MM-yy"), JsonRequestBehavior.AllowGet);
        //    if (!string.IsNullOrWhiteSpace(LeaveStartDate))
        //    {
        //        DateTime leaveEndDate = dynamicsNAVSOAPServices.hrManagementWS.GetLeaveEndDate(EmployeeNo, LeaveType,
        //            DateTime.Parse(LeaveStartDate), DaysApplied);

        //        return Json(leaveEndDate.ToString("dd-MM-yy"), JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public JsonResult GetLeaveReturnDate(string leaveNo, string LeaveType, string LeaveStartDate,
        //    Decimal DaysApplied)
        //{

        //    if (!string.IsNullOrWhiteSpace(LeaveStartDate))
        //    {
        //        DateTime leaveReturnDate = dynamicsNAVSOAPServices.hrManagementWS.GetLeaveReturnDate(leaveNo,
        //                 DaysApplied, DateTime.Parse(LeaveStartDate));

        //        return Json(leaveReturnDate.ToString("dd-MM-yy"), JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public JsonResult GetLeaveEndDate(string EmployeeNo, string LeaveType, string LeaveStartDate,
            Decimal DaysApplied)
        {
            if (DaysApplied != 0 )
            {
                DateTime leaveEndDate = dynamicsNAVSOAPServices.hrManagementWS.GetLeaveEndDate(EmployeeNo, LeaveType,
                                  DateTime.Parse(LeaveStartDate), DaysApplied);

                return Json(leaveEndDate.ToString("dd-MM-yy"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetLeaveReturnDate(string leaveNo, string LeaveType, string LeaveStartDate,
            Decimal DaysApplied)
        {
                DateTime leaveReturnDate = dynamicsNAVSOAPServices.hrManagementWS.GetLeaveReturnDate(leaveNo,
                    DaysApplied,DateTime.Parse(LeaveStartDate));

                return Json(leaveReturnDate.ToString("dd-MM-yy"), JsonRequestBehavior.AllowGet);
        }


        //only for approvals validation
        public JsonResult GetApprovedLeaveEndDate(string EmployeeNo, string LeaveType, string LeaveStartDate,
          Decimal DaysApplied)
        {
            if (DaysApplied != 0)
            {
                DateTime leaveEndDate = dynamicsNAVSOAPServices.hrManagementWS.GetApprovedLeaveEndDate(EmployeeNo, LeaveType,
                                  DateTime.Parse(LeaveStartDate), DaysApplied);

                return Json(leaveEndDate.ToString("dd-MM-yy"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetApprovedLeaveReturnDate(string leaveNo, string LeaveStartDate,
            Decimal DaysApplied)
        {
            DateTime leaveReturnDate = dynamicsNAVSOAPServices.hrManagementWS.GetApprovedLeaveReturnDate(leaveNo,
                DaysApplied, DateTime.Parse(LeaveStartDate));

            return Json(leaveReturnDate.ToString("dd-MM-yy"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetStartDate(string LeaveAppNo, string LeaveType)
        {
            if (LeaveType != "")
            {
                DateTime LeaveStartDate = dynamicsNAVSOAPServices.hrManagementWS.ModifyLeaveHeader(LeaveAppNo, LeaveType);

                return Json(LeaveStartDate.ToString("dd-MM-yy"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDaysApplied(string LeaveAppNo, string LeaveType)
        {
            if (LeaveType != "")
            {
                Decimal DaysApplied = dynamicsNAVSOAPServices.hrManagementWS.GetDaysApplied(LeaveAppNo);

                return Json(DaysApplied, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ModifyNewLeave(string LeaveAppNo, string LeaveType)
        {
            try
            {
                dynamicsNAVSOAPServices.hrManagementWS.ModifyNewLeave(LeaveAppNo, LeaveType ?? "");

                // Return a success response
                return Json(new { success = true, message = "Leave modified successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                // Return an error response if an exception occurs
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        //private void LoadEmployeeLeaveTypes(string employeeNo)
        //{
        //    //employeeLeaveTypes = from employeeLeaveTypeQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeLeaveTypes
        //    //where employeeLeaveTypeQuery.Employee_No.Equals(employeeNo)
        //    //					 select employeeLeaveTypeQuery;
        //    List<LeaveTypes> employeeLeaveTypes = new List<LeaveTypes>();
        //    string dimension1list = "LeaveTypes?$filter=Show_in_Portal eq true";

        //    HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(dimension1list);
        //    using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
        //    {
        //        var result1 = streamReader1.ReadToEnd();

        //        var details1 = JObject.Parse(result1);

        //        foreach (JObject config1 in details1["value"])
        //        {
        //            LeaveTypes DList1 = new LeaveTypes();
        //            DList1.Code = (string) config1["Code"];
        //            DList1.Description = (string) config1["Description"];
        //            employeeLeaveTypes.Add(DList1);
        //        }
        //    }
        //}

        private void LoadEmployeeSubstitutes(string employeeNo)
        {
            employees = from employeeQuery in dynamicsNAVODataServices.dynamicsNAVOData.Employees
                        where employeeQuery.No != employeeNo
                        select employeeQuery;
        }

        //public ActionResult ModifyLeaveHeader(LeaveApplicationModel LeaveApplicationObj)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.hrManagementWS.ModifyLeaveHeader(LeaveApplicationObj.No,
        //                                                                  LeaveApplicationObj.LeaveType ?? "",
        //                                                                  LeaveApplicationObj.SubstituteEmployeeNo ?? "",
        //                                                                  LeaveApplicationObj.ReasonForLeave ?? "");

        //        // Return a success response if needed
        //        return Json(new { success = true, message = "Leave application modified successfully." }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        // Return an error response if an exception occurs
        //        return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult ModifyLeave(LeaveApplicationModel LeaveApplicationObj)
        {
            try
            {
                dynamicsNAVSOAPServices.hrManagementWS.ModifyLeave(LeaveApplicationObj.No,
                                                                          LeaveApplicationObj.LeaveType ?? "",
                                                                          LeaveApplicationObj.SubstituteEmployeeNo ?? "",
                                                                          LeaveApplicationObj.ReasonForLeave ?? "");

                // Return a success response if needed
                return Json(new { success = true, message = "Leave application modified successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                // Return an error response if an exception occurs
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
   
        public ActionResult TargetSettingToReliever(LeaveApplicationModel LeaveApplicationObj)
        {
            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            //    return Json(new { success = false, errors = errors });
            //}

            //if (string.IsNullOrWhiteSpace(LeaveApplicationObj.SubstituteEmployeeNo))
            //{
            //    return Json(new { success = false, errors = new List<string> { "Substitute Employee is required." } });
            //}
            //if (string.IsNullOrWhiteSpace(LeaveApplicationObj.ReasonForLeave))
            //{
            //    return Json(new { success = false, errors = new List<string> { "Reason for Leave is required." } });
            //}
            //if (!int.TryParse(LeaveApplicationObj.DaysApplied, out int daysApplied) || daysApplied <= 0)
            //{
            //    return Json(new { success = false, errors = new List<string> { "Days Applied must be a positive number." } });
            //}

            try
            {
                dynamicsNAVSOAPServices.hrManagementWS.NotifyLeaveReliever(LeaveApplicationObj.No);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                // Log the exception
                // logger.LogError(e, "Error in TargetSettingToReliever");
                return Json(new { success = false, errors = new List<string> { "There was an error processing your request. Please try again later." } });
            }
        }

        //public ActionResult TargetSettingToReliever(LeaveApplicationModel LeaveApplicationObj)
        //{

        //    try
        //    {
        //        // Perform the business logic
        //        dynamicsNAVSOAPServices.hrManagementWS.NotifyLeaveReliever(LeaveApplicationObj.No);
        //        TempData["Success"] = "Sent To Reliever Successfully!";

        //        // Return success response
        //        return Json(new { success = true });
        //    }
        //    catch (Exception e)
        //    {
        //        // Handle exceptions and return error response
        //        return Json(new { success = false, errors = new List<string> { "There was an error processing your request. Please try again later." } });
        //    }
        //}


        //private void ModifyLeaveHeader(LeaveApplicationModel LeaveApplicationObj)
        //{
        //    dynamicsNAVSOAPServices.hrManagementWS.ModifyLeaveHeader(LeaveApplicationObj.No,
        //               LeaveApplicationObj.LeaveType ?? "", LeaveApplicationObj.LeaveType ?? "");
        //}
        //public ActionResult ModifyLeaveHeader(LeaveApplicationModel LeaveApplicationObj)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.hrManagementWS.ModifyLeaveHeader(LeaveApplicationObj.No,
        //                                                                  LeaveApplicationObj.LeaveType ?? "",
        //                                                                  LeaveApplicationObj.SubstituteEmployeeNo ?? "");

        //        return Json(new { success = true, message = "Leave application modified successfully." }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        // Return an error response
        //        return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        private void LoadDimensions(string GlobalDimension1Code)
        {
            globalDimension1Values =
                from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                where dimensionValuesQuery.Global_Dimension_No.Equals(1) && dimensionValuesQuery.Blocked.Equals(false)
                select dimensionValuesQuery;
            globalDimension2Values =
                from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                where dimensionValuesQuery.Global_Dimension_No.Equals(2) &&
                      dimensionValuesQuery.Dimension_Code.Equals(GlobalDimension1Code) &&
                      dimensionValuesQuery.Blocked.Equals(false)
                select dimensionValuesQuery;
            shortcutDimension3Values =
                from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                where dimensionValuesQuery.Global_Dimension_No.Equals(3) &&
                      dimensionValuesQuery.Dimension_Code.Equals(GlobalDimension1Code) &&
                      dimensionValuesQuery.Blocked.Equals(false)
                select dimensionValuesQuery;
            shortcutDimension4Values =
                from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                where dimensionValuesQuery.Global_Dimension_No.Equals(4) &&
                      dimensionValuesQuery.Dimension_Code.Equals(GlobalDimension1Code) &&
                      dimensionValuesQuery.Blocked.Equals(false)
                select dimensionValuesQuery;
            shortcutDimension5Values =
                from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                where dimensionValuesQuery.Global_Dimension_No.Equals(5) &&
                      dimensionValuesQuery.Dimension_Code.Equals(GlobalDimension1Code) &&
                      dimensionValuesQuery.Blocked.Equals(false)
                select dimensionValuesQuery;
            shortcutDimension6Values =
                from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                where dimensionValuesQuery.Global_Dimension_No.Equals(6) &&
                      dimensionValuesQuery.Dimension_Code.Equals(GlobalDimension1Code) &&
                      dimensionValuesQuery.Blocked.Equals(false)
                select dimensionValuesQuery;
            shortcutDimension7Values =
                from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                where dimensionValuesQuery.Global_Dimension_No.Equals(7) &&
                      dimensionValuesQuery.Dimension_Code.Equals(GlobalDimension1Code) &&
                      dimensionValuesQuery.Blocked.Equals(false)
                select dimensionValuesQuery;
            shortcutDimension8Values =
                from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                where dimensionValuesQuery.Global_Dimension_No.Equals(8) &&
                      dimensionValuesQuery.Dimension_Code.Equals(GlobalDimension1Code) &&
                      dimensionValuesQuery.Blocked.Equals(false)
                select dimensionValuesQuery;
        }

        //private void LoadResponsibilityCenters()
        //{
        //	responsibilityCenters = from responsibilityCenterQuery in dynamicsNAVODataServices.dynamicsNAVOData.ResponsibilityCenters
        //							select responsibilityCenterQuery;
        //}

        #endregion Helper Functions

        public ActionResult LeaveAcknowledgment()
        {
            var leaveApplicationsEnumerable = _bcodataServices.BCOData.EmployeeLeaveApplications
                .Where(c => c.Duties_Taken_Over_By == employeeNo && c.Status != "Released" && c.RelieverAcknowledgement == false && c.SentToReliever == true).ToList();
            return View(leaveApplicationsEnumerable);
        }

        public ActionResult acknowledgeleave(string id, bool? acknowledgmentValue)
        {
            var nav = _bcodataServices.BCOData;
            var leaveApplication = nav.Leave_Acknowledge
                .Where(c => c.Application_No == id).FirstOrDefault();
            if (leaveApplication != null)
            {
                if(acknowledgmentValue == true)
                {
                    leaveApplication.RelieverAcknowledgement = true;
                    leaveApplication.RelieverAcknowledgementReject = false;
                    leaveApplication.RelieverAcknowledgementDate = DateTime.Now;
                    nav.UpdateObject(leaveApplication);
                    nav.SaveChanges();
                    dynamicsNAVSOAPServices.hrManagementWS.LeaveRelieverAcknowledgement(id);

                }
                else 
                if(acknowledgmentValue == false)
                {
                    leaveApplication.RelieverAcknowledgementReject = true;
                    leaveApplication.SentToReliever = false;
                    leaveApplication.RelieverAcknowledgementDate = DateTime.Now;
                    nav.UpdateObject(leaveApplication);
                    nav.SaveChanges();
                    dynamicsNAVSOAPServices.hrManagementWS.LeaveRelieverAcknowledgement(id);

                }


           
            }
            return RedirectToAction("LeaveAcknowledgment");
        }
    }
}