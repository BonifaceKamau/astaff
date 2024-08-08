using System;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.Finance.ClaimsRefund;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using DynamicsNAV365_StaffPortal.Models.Finance.Imprest;
using DynamicsNAV365_StaffPortal.Models.Finance.StaffAdvance;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json.Linq;

namespace DynamicsNAV365_StaffPortal.Controllers.FinanceServices
{
    [Authorize]
    public class StaffAdvanceController : Controller
    {
        AccountController accountController = new AccountController();
        string employeeNo = "";
        List<RequestHeader> requestHeaders = new List<RequestHeader>();
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

        IQueryable<Currencies> currencyCodes = null;
        IQueryable<DimensionValues> globalDimension1Codes = null;
        IQueryable<DimensionValues> globalDimension2Codes = null;
        IQueryable<DimensionValues> shortcutDimension3Codes = null;
        IQueryable<DimensionValues> shortcutDimension4Codes = null;
        IQueryable<DimensionValues> shortcutDimension5Codes = null;
        IQueryable<DimensionValues> shortcutDimension6Codes = null;
        IQueryable<DimensionValues> shortcutDimension7Codes = null;
        IQueryable<DimensionValues> shortcutDimension8Codes = null;

        public StaffAdvanceController()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        static string companyURL = "";
        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

        public List<RequestHeader> RequestHeaders(string empNo, string staffadvanceNo)
        {
            string filterQuery = string.Empty;
            if (!string.IsNullOrEmpty(empNo))
            {
                filterQuery += $"Employee_No eq '{empNo}'";
            }

            if (!string.IsNullOrEmpty(staffadvanceNo))
            {
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    filterQuery += " and ";
                }

                filterQuery += $"No eq '{staffadvanceNo}'";
            }

            string url = $"StaffAdvance?$filter={filterQuery}&$format=json";
            var httpResponseDestForeign = Credentials.GetOdataData(url);
            httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();
                var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<RequestHeader>>(result1);
                return StaffAdvanceList?.ListValues;
            }
        }
        
        public ActionResult OpenStaffAdvance()
        {
            try
            {
                var httpResponseDestForeign =
                    Credentials.GetOdataData("StaffAdvance?$filter=Employee_No eq '" + employeeNo + "' &$format=json");
                httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
                using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                {
                    var result1 = streamReader1.ReadToEnd();
                    var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<RequestHeader>>(result1);
                    requestHeaders = StaffAdvanceList?.ListValues;
                    return View(StaffAdvanceList?.ListValues);
                }
                //var claimsRefundLists = JsonConvert.DeserializeObject<List<ClaimsRefundHeaderModel>>(dynamicsNAVSOAPServices.fundsManagementWS.GetOpenStaffAdvance("", employeeNo));
                //return View(claimsRefundLists);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        [HttpGet]
        public ActionResult NewStaffAdvance()
        {
            string staffadvanceNo = "";
            string openPettyCashNo = "";
            try
            {
                var staffAdvanceRequestHeader = new RequestHeader();
                //Check open imprest request
                var httpResponse =
                    Credentials.GetOdataData("StaffAdvance?$filter=Employee_No eq '" + employeeNo + "' &$format=json");
                using (var streamReader1 = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result1 = streamReader1.ReadToEnd();
                    var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<RequestHeader>>(result1);
                    openPettyCashNo = StaffAdvanceList?.ListValues?.FirstOrDefault(c => c?.Status == "Open")?.No;
                    //openPettyCashNo = dynamicsNAVSOAPServices.StaffAdvance.CheckOpenStaffAdvanceExists(employeeNo);
                }

                if (openPettyCashNo != null && !openPettyCashNo.Equals(""))
                {
                    responseHeader = "Open Staff Advance";
                    responseMessage = "An open Staff Advance No. (" + openPettyCashNo +
                                      ") exists under your account. You have to edit this document and submit for approval.";
                    detailedResponseMessage = "An open Staff Advance No. (" + openPettyCashNo +
                                              ") exists under your account. You have to edit this document and submit for approval.";

                    button1ControllerName = "StaffAdvance";
                    button1ActionName = "OpenStaffAdvance";
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
                //End check open staffadvance request

                //Create a new staffadvance request
                staffadvanceNo = dynamicsNAVSOAPServices.StaffAdvance.CreateImprestHeader(employeeNo);
                //End create imprest request

                staffAdvanceRequestHeader.No = staffadvanceNo;
                var httpResponseDestForeign = Credentials.GetOdataData(
                    $"StaffAdvance?$filter=Employee_No eq '{employeeNo}' and No eq '{staffadvanceNo}' &$format=json");
                using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                {
                    var result1 = streamReader1.ReadToEnd();
                    var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<RequestHeader>>(result1);
                    var requestHeader = StaffAdvanceList?.ListValues?.FirstOrDefault();
                    List<DimensionValues> DimensionValues = new List<DimensionValues>();
                    string dimension1list =
                        "DimensionValues?$filter=Global_Dimension_No eq 2 and Blocked eq false &$format=json";

                    HttpWebResponse webResponsedimension = Credentials.GetOdataData(dimension1list);
                    using (var webResponsedimension1list = new StreamReader(webResponsedimension.GetResponseStream()))
                    {
                        var dimension1result1 = webResponsedimension1list.ReadToEnd();

                        var details1 = JObject.Parse(dimension1result1);

                        foreach (JObject config1 in details1["value"])
                        {
                            DimensionValues DList1 = new DimensionValues();
                            DList1.Code = (string) config1["Code"];
                            DList1.Name = (string) config1["Name"];
                            DimensionValues.Add(DList1);
                        }
                    }

                    if (requestHeader != null)
                    {
                        requestHeader.GlobalDimension2CodesSelect = new SelectList(DimensionValues, "Code", "Code");
                    }

                    return View(requestHeader);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [HttpPost]
        public ActionResult NewStaffAdvance(RequestHeader requestHeader)
        {
            bool imprestRequestModified = false;
            bool approvalWorkflowExist = false;
            List<DimensionValues> DimensionValues = new List<DimensionValues>();
            string dimension1list =
                "DimensionValues?$filter=Global_Dimension_No eq 2 and Blocked eq false &$format=json";

            HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(dimension1list);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    DimensionValues DList1 = new DimensionValues();
                    DList1.Code = (string) config1["Code"];
                    DList1.Name = (string) config1["Name"];
                    DimensionValues.Add(DList1);
                }
            }
            requestHeader.GlobalDimension2CodesSelect = new SelectList(DimensionValues, "Code", "Code",requestHeader.GlobalDimension2Code);


            try
            {
                //LoadCurrencies();
                //LoadImprestRequestDimensions(ImprestRequestObj.GlobalDimension1Code);

                if (ModelState.IsValid)
                {
                    if (dynamicsNAVSOAPServices.StaffAdvance.CheckImprestRequestExists(requestHeader.No,
                        AccountController.GetEmployeeNo()))
                    {
                        //Check advance lines
                        if (!dynamicsNAVSOAPServices.StaffAdvance.CheckImprestLinesExist(requestHeader.No))
                        {
                            requestHeader.ErrorStatus = true;
                            requestHeader.ErrorMessage =
                                "staff advance lines missing, the staff advance must contain a minimum of one staff advance line, add an staff advance line to continue.";
                            return View(requestHeader);
                        }

                        //Validate advance lines
                        string imprestLineError = "";
                        imprestLineError = dynamicsNAVSOAPServices.StaffAdvance.ValidateImprestLines(requestHeader.No);
                        if (!imprestLineError.Equals(""))
                        {
                            requestHeader.ErrorStatus = true;
                            requestHeader.ErrorMessage = imprestLineError;
                            return View(requestHeader);
                        }

                        //Modify advance request
                        //requestHeader.Destination = requestHeader.Destination != null ? requestHeader.Destination : "";
                        //requestHeader.DateFrom = requestHeader.DateFrom != "01/01/0001" ? ImprestRequestObj.DateFrom : DateTime.Now.ToString();
                        //requestHeader.DateTo = requestHeader.DateTo != "01/01/0001" ? ImprestRequestObj.DateTo : DateTime.Now.ToString();

                        //imprestRequestModified = dynamicsNAVSOAPServices.StaffAdvance.ModifyImprestHeader(
                        //    requestHeader.No, requestHeader.EmployeeNo, requestHeader.PurposeOfImprest,
                        //    requestHeader.CreatePV,requestHeader.GlobalDimension2Code);
                        //if (!imprestRequestModified)
                        //{
                        //    requestHeader.ErrorStatus = true;
                        //    requestHeader.ErrorMessage =
                        //        "An error was experienced while trying to modify staff advance no." + requestHeader.No +
                        //        ", the server might be offline, try again after a while.";
                        //    return View(requestHeader);
                        //}

                        //Send imprest for approval
                        approvalWorkflowExist =
                            dynamicsNAVSOAPServices.StaffAdvance.CheckImprestApprovalWorkflowEnabled(requestHeader.No);
                        if (!approvalWorkflowExist)
                        {
                            requestHeader.ErrorStatus = true;
                            requestHeader.ErrorMessage =
                                "An error was experienced while trying to send an approval request for staff advance no." +
                                requestHeader.No + ", the approval workflow was not found. " +
                                ServiceConnection.contactICTDepartment + "";
                            return View(requestHeader);
                        }

                        if (dynamicsNAVSOAPServices.StaffAdvance.SendImprestApprovalRequest(requestHeader.No))
                        {
                            responseHeader = "Success";
                            //responseMessage = "Imprest no." + ImprestRequestObj.No + " was successfully sent for approval. Check with your HOD for approval for approval status.";
                            //	detailedResponseMessage = "Imprest no." + ImprestRequestObj.No + " was successfully sent for approval. Check with your HOD for approval for approval status.";
                            responseMessage = "staff advance no." + requestHeader.No +
                                              " was successfully sent for approval.";
                            detailedResponseMessage = "staff advance no." + requestHeader.No +
                                                      " was successfully sent for approval.";

                            button1ControllerName = "OpenStaffAdvance";
                            button1ActionName = "StaffAdvance";
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
                            requestHeader.ErrorStatus = true;
                            requestHeader.ErrorMessage =
                                "An error was experienced while trying to send an approval request for imprest no." +
                                requestHeader.No + ". " + ServiceConnection.contactICTDepartment + "";
                            return View(requestHeader);
                        }
                    }
                    else
                    {
                        responseHeader = "staff advance NotFound";
                        responseMessage = "The staff advance no." + requestHeader.No +
                                          " was not found under employee no." + AccountController.GetEmployeeNo();
                        detailedResponseMessage = "The staff advance no." + requestHeader.No +
                                                  " was not found under employee no." +
                                                  AccountController.GetEmployeeNo();

                        button1ControllerName = "StaffAdvance";
                        button1ActionName = "OpenStaffAdvance";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                            button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                            button1Name,
                            button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                            button2Name);
                    }
                }
                else
                {
                    return View(requestHeader);
                }
            }
            catch (Exception ex)
            {
                requestHeader.ErrorStatus = true;
                requestHeader.ErrorMessage = ex.Message.ToString();
                return View(requestHeader);
            }
        }

        [HttpGet]
        public ActionResult OnBeforeEdit(string documentno)
        {
            try
            {
                if (documentno.Equals(""))
                {
                    return RedirectToAction("OpenStaffAdvance");
                }

                if (dynamicsNAVSOAPServices.StaffAdvance.CheckImprestRequestExists(documentno,
                    AccountController.GetEmployeeNo()))
                {
                    string imprestStatus = GetImprestStatus(documentno);
                    //if imprest is open
                    if (imprestStatus.Equals("Open"))
                    {
                        return RedirectToAction("EditAdvanceRequest", new {documentno = documentno});
                    }

                    //if imprest is pending approval
                    if (imprestStatus.Equals("Pending Approval"))
                    {
                        responseHeader = "Advance Request Pending Approval";
                        responseMessage = "The Advance no." + documentno +
                                          " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
                        detailedResponseMessage = "The Advance no." + documentno +
                                                  " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";

                        button1ControllerName = "StaffAdvance";
                        button1ActionName = "EditAdvanceRequest";
                        button1HasParameters = true;
                        button1Parameters = "?documentno=" + documentno;
                        button1Name = "Yes";

                        button2ControllerName = "staffadvance";
                        button2ActionName = "openstaffadvance";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                            button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                            button1Name,
                            button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                            button2Name);
                    }

                    //if imprest is released
                    if (imprestStatus.Equals("Released"))
                    {
                        responseHeader = "advance Approved";
                        responseMessage = "The advance no." + documentno + " is already approved. Editing not allowed.";
                        detailedResponseMessage =
                            "The advance no." + documentno + " is already approved. Editing not allowed.";

                        button1ControllerName = "StaffAdvance";
                        button1ActionName = "OpenStaffAdvance";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                            button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                            button1Name,
                            button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                            button2Name);
                    }

                    //if imprest is rejected
                    if (imprestStatus.Equals("Rejected"))
                    {
                        responseHeader = "advance Rejected";
                        responseMessage = "The advance no." + documentno +
                                          " was rejected. Editing will reopen the document. Do you want to continue?";
                        detailedResponseMessage = "The Advance no." + documentno +
                                                  " was rejected. Editing will reopen the document. Do you want to continue?";

                        button1ControllerName = "staffadvance";
                        button1ActionName = "EditAdvanceRequest";
                        button1HasParameters = true;
                        button1Parameters = "?documentno=" + documentno;
                        button1Name = "Yes";

                        button2ControllerName = "StaffAdvance";
                        button2ActionName = "OpenStaffAdvance";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                            button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                            button1Name,
                            button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                            button2Name);
                    }

                    //if imprest is posted/reversed
                    if (imprestStatus.Equals("Posted") || imprestStatus.Equals("Reversed"))
                    {
                        responseHeader = "advance Posted";
                        responseMessage = "The advance no." + documentno + " is already posted. Editing not allowed.";
                        detailedResponseMessage =
                            "The advance no." + documentno + " is already posted. Editing not allowed.";

                        button1ControllerName = "StaffAdvance";
                        button1ActionName = "OpenStaffAdvance";
                        button1HasParameters = false;
                        button1Parameters = "";
                        button1Name = "Ok";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";

                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                            button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                            button1Name,
                            button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                            button2Name);
                    }

                    return RedirectToAction("OpenStaffAdvance");
                }
                else
                {
                    responseHeader = "Advance NotFound";
                    responseMessage = "The advance no." + documentno + " was not found under employee no." +
                                      AccountController.GetEmployeeNo();
                    detailedResponseMessage = "The advance no." + documentno + " was not found under employee no." +
                                              AccountController.GetEmployeeNo();

                    button1ControllerName = "StaffAdvance";
                    button1ActionName = "OpenStaffAdvance";
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

        public string GetImprestStatus(string DocumentNo)
        {
            return dynamicsNAVSOAPServices.StaffAdvance.GetImprestStatus(DocumentNo);
        }

        public ActionResult _AdvanceDocumentLine(string documentno)
        {
            DocumentMgmtModel documentManagementoBJ = new DocumentMgmtModel();

            var imprestUploadedDocuments =
                from imprestDocumentsQuery in dynamicsNAVODataServices.dynamicsNAVOData.PortalDocuments
                where imprestDocumentsQuery.DocumentNo.Equals(documentno)
                select imprestDocumentsQuery;

            dynamic imprestDocuments =
                JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(documentno));

            foreach (var imprestDocument in imprestDocuments)
            {
                documentManagementoBJ.LineNo = imprestDocument.LineNo;
                documentManagementoBJ.DocumentNo = imprestDocument.DocumentNo;
                documentManagementoBJ.DocumentCode = imprestDocument.DocumentCode;
                documentManagementoBJ.DocumentDescription = imprestDocument.DocumentDescription;
                documentManagementoBJ.DocumentAttached = imprestDocument.DocumentAttached ?? false;
                documentManagementoBJ.FileName = imprestDocument.FileName;
            }

            return PartialView(documentManagementoBJ);
        }

        public ActionResult _AdvanceLine(string imprestno)
        {
            ImprestLineModel imprestLineObj = new ImprestLineModel();

            List<FundsTransactionModel> imprestCodes = new List<FundsTransactionModel>();

            dynamic fundsTransactionCodes =
                JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.StaffAdvance.GetImprestTransactionCodes());
            foreach (var fundsTransactionCode in fundsTransactionCodes)
            {
                FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
                FundsTransactionObj.TransactionCode = fundsTransactionCode.TransactionCode;
                FundsTransactionObj.TransactionDescription = fundsTransactionCode.TransactionDescription;

                imprestCodes.Add(FundsTransactionObj);
            }

            #region Dimension 1 List

            List<DimensionValues> DimensionValues = new List<DimensionValues>();
            string dimension1list =
                "DimensionValues?$filter=Global_Dimension_No eq 1 and Blocked eq false &$format=json";

            HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(dimension1list);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    DimensionValues DList1 = new DimensionValues();
                    DList1.Code = (string) config1["Code"];
                    DList1.Name = (string) config1["Name"];
                    DimensionValues.Add(DList1);
                }
            }

            #endregion

            imprestLineObj.TransactionTypeS = new SelectList(imprestCodes, "TransactionCode", "TransactionDescription");
            imprestLineObj.Dimension1s = new SelectList(DimensionValues, "Code", "Name");
            imprestLineObj.Dimension2s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.Dimension3s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.Dimension4s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.Dimension5s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.Dimension6s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.Dimension7s = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.CurrencyCodeSelect = _bcodataServices.BCOData.currency.ToList().Select(c =>
                new SelectListItem
                {
                    Value = c.Code,
                    Text = $"{c.Code} :  {c.Description}"
                });

            return PartialView(imprestLineObj);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateHeader(string Description, bool CreatePV, string GlobalDimension2Code, string DocNo)
        {
            try
            {
                bool ret;
                bool successVal = false;
                string msg = "";

                //ret = dynamicsNAVSOAPServices.StaffAdvance.ModifyImprestHeader(DocNo, AccountController.GetEmployeeNo(), Description, CreatePV,GlobalDimension2Code);
                //if (ret)
                //{
                //    msg = "Updated Successfully";
                //    successVal = true;
                //}
                //else
                //{
                //    msg = "Details Failed to Update";
                //    successVal = true;
                //}

                return Json(new {message = msg, success = successVal}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message.Replace("'", ""), success = false}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditAdvanceRequest(string documentno)
        {
            try
            {
                if (documentno.Equals(""))
                {
                    return RedirectToAction("OpenStaffAdvance");
                }

                if (dynamicsNAVSOAPServices.StaffAdvance.CheckImprestRequestExists(documentno,
                    AccountController.GetEmployeeNo()))
                {
                    string imprestStatus = dynamicsNAVSOAPServices.StaffAdvance.GetImprestStatus(documentno);

                    //if imprest is pending approval, cancel approval request
                    if (imprestStatus.Equals("Pending Approval"))
                    {
                        dynamicsNAVSOAPServices.StaffAdvance.CancelImprestApprovalRequest(documentno);
                    }

                    //if imprest is released, reopen and uncommit from budget
                    if (imprestStatus.Equals("Released"))
                    {
                        dynamicsNAVSOAPServices.StaffAdvance.ReopenImprestRequest(documentno);
                        dynamicsNAVSOAPServices.StaffAdvance.CancelImprestBudgetCommitment(documentno);
                    }

                    //if imprest is rejected, reopen document
                    if (imprestStatus.Equals("Rejected"))
                    {
                        dynamicsNAVSOAPServices.StaffAdvance.ReopenImprestRequest(documentno);
                    }

                    var imprestRequestObj = RequestHeaders("", documentno).FirstOrDefault();

                    List<DimensionValues> DimensionValues = new List<DimensionValues>();
                    string dimension1list =
                        "DimensionValues?$filter=Global_Dimension_No eq 2 and Blocked eq false &$format=json";

                    HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(dimension1list);
                    using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                    {
                        var result1 = streamReader1.ReadToEnd();

                        var details1 = JObject.Parse(result1);

                        foreach (JObject config1 in details1["value"])
                        {
                            DimensionValues DList1 = new DimensionValues();
                            DList1.Code = (string) config1["Code"];
                            DList1.Name = (string) config1["Name"];
                            DimensionValues.Add(DList1);
                        }
                    }

                    imprestRequestObj.GlobalDimension2CodesSelect = new SelectList(DimensionValues, "Code", "Code",imprestRequestObj.GlobalDimension2Code);

                    return View(imprestRequestObj);
                }
                else
                {
                    responseHeader = "advance NotFound";
                    responseMessage = "The advance no." + documentno + " was not found under employee no." +
                                      AccountController.GetEmployeeNo();
                    detailedResponseMessage = "The advance no." + documentno + " was not found under employee no." +
                                              AccountController.GetEmployeeNo();

                    button1ControllerName = "StaffAdvance";
                    button1ActionName = "OpenStaffAdvance";
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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> EditAdvanceRequest(RequestHeader ImprestRequestObj, string Command)
        {
            bool imprestRequestModified = false;
            bool approvalWorkflowExist = false;
            List<DimensionValues> DimensionValues = new List<DimensionValues>();
            string dimension1list =
                "DimensionValues?$filter=Global_Dimension_No eq 2 and Blocked eq false &$format=json";

            HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(dimension1list);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    DimensionValues DList1 = new DimensionValues();
                    DList1.Code = (string) config1["Code"];
                    DList1.Name = (string) config1["Name"];
                    DimensionValues.Add(DList1);
                }
            }
            ImprestRequestObj.GlobalDimension2CodesSelect = new SelectList(DimensionValues, "Code", "Code",ImprestRequestObj.GlobalDimension2Code);

            try
            {
                if (Command.Equals("Submit For Approval"))
                {
                    if (/*ModelState.IsValid*/true)
                    {
                        if (dynamicsNAVSOAPServices.StaffAdvance.CheckImprestRequestExists(ImprestRequestObj.No,
                            AccountController.GetEmployeeNo()))
                        {
                            //Check imprest lines
                            if (!dynamicsNAVSOAPServices.StaffAdvance.CheckImprestLinesExist(ImprestRequestObj.No))
                            {
                                ImprestRequestObj.ErrorStatus = true;
                                ImprestRequestObj.ErrorMessage =
                                    "Staff Advance lines missing, the Staff Advance must contain a minimum of one Staff Advance line, add an Staff Advance line to continue.";
                                return View(ImprestRequestObj);
                            }

                            //Validate imprest lines
                            string imprestLineError = "";
                            imprestLineError =
                                dynamicsNAVSOAPServices.StaffAdvance.ValidateImprestLines(ImprestRequestObj.No);
                            if (!imprestLineError.Equals(""))
                            {
                                ImprestRequestObj.ErrorStatus = true;
                                ImprestRequestObj.ErrorMessage = imprestLineError;
                                return View(ImprestRequestObj);
                            }

                            //ImprestRequestObj.Destination = ImprestRequestObj.Destination != null ? ImprestRequestObj.Destination : "";
                            //ImprestRequestObj.DateFrom = ImprestRequestObj.DateFrom != "01/01/0001" ? ImprestRequestObj.DateFrom : DateTime.Now.ToString();
                            //ImprestRequestObj.DateTo = ImprestRequestObj.DateTo != "01/01/0001" ? ImprestRequestObj.DateTo : DateTime.Now.ToString();
                            //DateTime datefrom = DateTime.ParseExact(ImprestRequestObj.DateFrom.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //DateTime dateto = DateTime.ParseExact(ImprestRequestObj.DateTo, "MM/dd/yyyy", null);

                            //imprestRequestModified = dynamicsNAVSOAPServices.StaffAdvance.ModifyImprestHeader(
                            //    ImprestRequestObj.No, ImprestRequestObj.EmployeeNo, ImprestRequestObj.PurposeOfImprest??ImprestRequestObj.Description??"",
                            //    ImprestRequestObj.CreatePV,ImprestRequestObj.GlobalDimension2Code);

                            //if (!imprestRequestModified)
                            //{
                            //    ImprestRequestObj.ErrorStatus = true;
                            //    ImprestRequestObj.ErrorMessage =
                            //        "An error was experienced while trying to modify Staff Advance no." +
                            //        ImprestRequestObj.No + ", the server might be offline, try again after a while.";
                            //    return View(ImprestRequestObj);
                            //}

                            //Send imprest for approval
                            approvalWorkflowExist =
                                dynamicsNAVSOAPServices.StaffAdvance.CheckImprestApprovalWorkflowEnabled(
                                    ImprestRequestObj.No);

                            if (!approvalWorkflowExist)
                            {
                                ImprestRequestObj.ErrorStatus = true;
                                ImprestRequestObj.ErrorMessage =
                                    "An error was experienced while trying to send an approval request for advance no." +
                                    ImprestRequestObj.No + ", the approval workflow was not found. " +
                                    ServiceConnection.contactICTDepartment + "";
                                return View(ImprestRequestObj);
                            }

                            if (dynamicsNAVSOAPServices.StaffAdvance.SendImprestApprovalRequest(ImprestRequestObj.No))
                            {
                                responseHeader = "Success";
                                responseMessage = "Staff Advance no." + ImprestRequestObj.No +
                                                  " was successfully sent for approval.";
                                detailedResponseMessage = "Staff Advance no." + ImprestRequestObj.No +
                                                          " was successfully sent for approval.";

                                button1ControllerName = "StaffAdvance";
                                button1ActionName = "OpenStaffAdvance";
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
                                ImprestRequestObj.ErrorStatus = true;
                                ImprestRequestObj.ErrorMessage =
                                    "An error was experienced while trying to send an approval request for Staff Advance no." +
                                    ImprestRequestObj.No + ". " + ServiceConnection.contactICTDepartment + "";
                                return View(ImprestRequestObj);
                            }
                        }
                        else
                        {
                            responseHeader = "Imprest NotFound";
                            responseMessage = "The imprest no." + ImprestRequestObj.No +
                                              " was not found under employee no." + AccountController.GetEmployeeNo();
                            detailedResponseMessage = "The imprest no." + ImprestRequestObj.No +
                                                      " was not found under employee no." +
                                                      AccountController.GetEmployeeNo();

                            button1ControllerName = "Imprest";
                            button1ActionName = "ImprestRequestHistory";
                            button1HasParameters = false;
                            button1Parameters = "";
                            button1Name = "Ok";

                            button2ControllerName = "";
                            button2ActionName = "";
                            button2HasParameters = false;
                            button2Parameters = "";
                            button2Name = "";

                            return errorResponse.ApplicationError(responseHeader, responseMessage,
                                detailedResponseMessage,
                                button1ControllerName, button1ActionName, button1HasParameters, button1Parameters,
                                button1Name,
                                button2ControllerName, button2ActionName, button2HasParameters, button2Parameters,
                                button2Name);
                        }
                    }
                }

                if (Command.Equals("View Attachment"))
                {
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(ImprestRequestObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                }
                else
                {
                    return View(ImprestRequestObj);
                }
            }
            catch (Exception ex)
            {
                ImprestRequestObj.ErrorStatus = true;
                ImprestRequestObj.ErrorMessage = ex.Message.ToString();
                return View(ImprestRequestObj);
            }
        }

        [Authorize]
        public ActionResult ViewStaffAdvance(string documentno)
        {
            try
            {
                if (documentno.Equals(""))
                {
                    return RedirectToAction("ImprestRequestHistory", "Imprest");
                }

                if (dynamicsNAVSOAPServices.StaffAdvance.CheckImprestRequestExists(documentno,
                    AccountController.GetEmployeeNo()))
                {
                    var imprestRequestObj = RequestHeaders("", documentno).FirstOrDefault();

                    return View(imprestRequestObj);
                }
                else
                {
                    responseHeader = "Staff advance Not Found";
                    responseMessage = "The Staff advance no." + documentno + " was not found under employee no." +
                                      AccountController.GetEmployeeNo();
                    detailedResponseMessage = "The Staff advance no." + documentno +
                                              " was not found under employee no." + AccountController.GetEmployeeNo();

                    button1ControllerName = "Staffadvance";
                    button1ActionName = "OpenStaffadvance";
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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ViewStaffAdvance(RequestHeader pettyCashRequestObj, string Command)
        {
            try
            {
                if (pettyCashRequestObj.No.Equals(""))
                {
                    return RedirectToAction("OpenStaffAdvance");
                }

                if (Command.Equals("View Attachment"))
                {
                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(pettyCashRequestObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                }
                else
                {
                    pettyCashRequestObj.ErrorStatus = true;
                    //leaveApplicationObj2.ErrorMessage = "Unable to process the approve/reject action. Contact the " + companyName + " for assistance.";
                    return View(pettyCashRequestObj);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [HttpGet]
        public virtual ActionResult Download()
        {
            ClientContext ctx = new ClientContext(ServiceConnection.sharePointSiteUrl);
            SecureString passWord = new SecureString();
            foreach (char c in ServiceConnection.sharePointUserPassword.ToCharArray()) passWord.AppendChar(c);
            ctx.Credentials = new SharePointOnlineCredentials(ServiceConnection.sharePointUser, passWord);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(ctx, "/sites/Test/testdoc.docx");
            return File(fileInfo.Stream, "application/octet-stream", "testdoc.docx");
        }
    }
}