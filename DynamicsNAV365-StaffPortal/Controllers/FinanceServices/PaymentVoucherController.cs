using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using DynamicsNAV365_StaffPortal.Models.Finance.PaymentVoucher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.FinanceServices
{
    public class PaymentVoucherController : Controller
    {
        static string companyURL = "";

        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

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

        AccountController accountController = new AccountController();
        string employeeNo = "";

        [Authorize]
        public ActionResult PaymentVoucherApproval(string PvNo)
        {
            try
            {
                if (PvNo.Equals(""))
                {
                    return RedirectToAction("OpenEntries", "Approval");
                }


                PVHeaderModel imprestRequestObj = new PVHeaderModel();

                //var imprestRequests = from imprestRequestsQuery in dynamicsNAVODataServices.dynamicsNAVOData.ImprestRequests
                //					  where imprestRequestsQuery.No.Equals(ImprestNo)
                //					  select imprestRequestsQuery;

                dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPVRequests(PvNo, ""));

                foreach (var imprestRequest in imprestRequests)
                {
                    imprestRequestObj.No = imprestRequest.No;
                    imprestRequestObj.DocumentDate = imprestRequest.DocumentDate;
                    imprestRequestObj.PostingDate = imprestRequest.PostingDate;
                    imprestRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
                    imprestRequestObj.BankAccountName = imprestRequest.BankAccountName;
                    imprestRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
                    imprestRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
                    imprestRequestObj.EmployeeName = imprestRequest.EmployeeName;
                    imprestRequestObj.ImprestType = imprestRequest.Type;
                    imprestRequestObj.CurrencyCode = imprestRequest.ImprestType;
                    //imprestRequestObj.Surrendered = imprestRequest.Surrendered ?? false;
                    imprestRequestObj.Description = imprestRequest.Description;
                    imprestRequestObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
                    imprestRequestObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
                    imprestRequestObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
                    imprestRequestObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
                    imprestRequestObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
                    imprestRequestObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
                    imprestRequestObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
                    imprestRequestObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
                    imprestRequestObj.Amount = imprestRequest.Amount;
                    imprestRequestObj.Status = imprestRequest.Status;
                }


                return View(imprestRequestObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }


        [ChildActionOnly]
        [Authorize]
        public ActionResult _ViewPVLine(string DocumentNo)
        {
            PVLineModel imprestLineObj = new PVLineModel();

            List<FundsTransactionModel> imprestCodes = new List<FundsTransactionModel>();

            dynamic fundsTransactionCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetImprestTransactionCodes());
            foreach (var fundsTransactionCode in fundsTransactionCodes)
            {
                FundsTransactionModel FundsTransactionObj = new FundsTransactionModel();
                FundsTransactionObj.TransactionCode = fundsTransactionCode.TransactionCode;
                FundsTransactionObj.TransactionDescription = fundsTransactionCode.TransactionDescription;

                imprestCodes.Add(FundsTransactionObj);
            }

            imprestLineObj.TransactionTypeS = new SelectList(imprestCodes, "TransactionCode", "TransactionDescription");
            //LoadImprestCategories();
            //imprestLineObj.ImprestCategories = new SelectList(imprestCategoryList, "Code", "Description");
            imprestLineObj.ImprestElements = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.SRCDestinations = new SelectList(Enumerable.Empty<SelectListItem>());
            imprestLineObj.SalaryScales = new SelectList(Enumerable.Empty<SelectListItem>());

            return PartialView(imprestLineObj);
        }

        [Authorize]
        public JsonResult GetPVLinesAjax(string DocumentNo)
        {
            List<PVLineModel> imprestLinesList = new List<PVLineModel>();
            string imprestlines = "PVLines?$filter=PV_No eq '" + DocumentNo + "' &$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(imprestlines);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    PVLineModel imprestLineObj = new PVLineModel();
                    imprestLineObj.LineNo = (string)config1["Line_No"];
                    imprestLineObj.DocumentNo = (string)config1["Document_No"];
                    imprestLineObj.TransactionType = (string)config1["Transaction_Type"];
                    imprestLineObj.ImprestCategory = (string)config1["Imprest_Category"];
                    imprestLineObj.ImprestElement = (string)config1["Imprest_Element"];
                    imprestLineObj.SRCDestination = (string)config1["Destination"];
                    imprestLineObj.SalaryScale = (string)config1["Salary_Scale"];
                    imprestLineObj.AccountType = (string)config1["Account_Type"];
                    imprestLineObj.AccountNo = (string)config1["Account_No"];
                    imprestLineObj.AccountName = (string)config1["Transaction_Type"];
                    imprestLineObj.LineDescription = (string)config1["Description"];
                    imprestLineObj.LineAmount = (string)config1["Amount"];
                    imprestLineObj.Days = (string)config1["Quantity"];
                    imprestLineObj.Dimension1 = (string)config1["Shortcut_Dimension_1_Code"];
                    imprestLineObj.Dimension2 = (string)config1["Shortcut_Dimension_2_Code"];
                    imprestLineObj.Dimension3 = (string)config1["ShortcutDimCode3"];
                    imprestLineObj.Dimension4 = (string)config1["ShortcutDimCode4"];
                    imprestLineObj.Dimension5 = (string)config1["ShortcutDimCode5"];
                    imprestLineObj.Dimension6 = (string)config1["ShortcutDimCode6"];
                    imprestLineObj.Dimension7 = (string)config1["ShortcutDimCode7"];
                    imprestLinesList.Add(imprestLineObj);
                }
            }
            
            return Json(imprestLinesList, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PaymentVoucherApproval(PVHeaderModel ImprestRequestObj, string Command)
        {
            try
            {
                if (ImprestRequestObj.No.Equals(""))
                {
                    return RedirectToAction("OpenEntries", "Approval");
                }
                if (Command == "Approve")
                {
                    ImprestRequestObj.Comments = ImprestRequestObj.Comments ?? "";
                    dynamicsNAVSOAPServices.ApprovalsMgmt.ApprovePaymentVoucher(ImprestRequestObj.No,
                        AccountController.GetEmployeeNo());
                    /*if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApprovePaymentVoucher(ImprestRequestObj.No, AccountController.GetEmployeeNo()))
                    {*/
                        responseHeader = "Success";
                        responseMessage = "Payment Voucher no." + ImprestRequestObj.No + " was successfully approved.";
                        detailedResponseMessage = "Payment Voucher no." + ImprestRequestObj.No + " was successfully approved.";

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

                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    /*}

                    ImprestRequestObj.ErrorStatus = true;
                    ImprestRequestObj.ErrorMessage = "Unable to process the imprest request approve action. " + ServiceConnection.contactICTDepartment + "";
                    return View(ImprestRequestObj);*/
                }

                if (Command == "Reject")
                {
                    ImprestRequestObj.Comments = ImprestRequestObj.Comments != null ? ImprestRequestObj.Comments : "";
                    if (ImprestRequestObj.Comments.Equals(""))
                    {
                        dynamic imprestRequests = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.fundsManagementWS.GetPVRequests(ImprestRequestObj.No, ""));

                        foreach (var imprestRequest in imprestRequests)
                        {
                            ImprestRequestObj.No = imprestRequest.No;
                            ImprestRequestObj.DocumentDate = imprestRequest.DocumentDate;
                            ImprestRequestObj.PostingDate = imprestRequest.PostingDate;
                            ImprestRequestObj.BankAccountNo = imprestRequest.BankAccountNo;
                            ImprestRequestObj.BankAccountName = imprestRequest.BankAccountName;
                            ImprestRequestObj.ReferenceNo = imprestRequest.ReferenceNo;
                            ImprestRequestObj.EmployeeNo = imprestRequest.EmployeeNo;
                            ImprestRequestObj.EmployeeName = imprestRequest.EmployeeName;
                            ImprestRequestObj.DateFrom = imprestRequest.DateFrom.ToString("dd-MM-yy");
                            ImprestRequestObj.DateTo = imprestRequest.DateTo.ToString("dd-MM-yy");
                            ImprestRequestObj.ImprestType = imprestRequest.Type;
                            ImprestRequestObj.CurrencyCode = imprestRequest.ImprestType;
                            ImprestRequestObj.Description = imprestRequest.Description;
                            ImprestRequestObj.GlobalDimension1Code = imprestRequest.GlobalDimension1Code;
                            ImprestRequestObj.GlobalDimension2Code = imprestRequest.GlobalDimension2Code;
                            ImprestRequestObj.ShortcutDimension3Code = imprestRequest.ShortcutDimension3Code;
                            ImprestRequestObj.ShortcutDimension4Code = imprestRequest.ShortcutDimension4Code;
                            ImprestRequestObj.ShortcutDimension5Code = imprestRequest.ShortcutDimension5Code;
                            ImprestRequestObj.ShortcutDimension6Code = imprestRequest.ShortcutDimension6Code;
                            ImprestRequestObj.ShortcutDimension7Code = imprestRequest.ShortcutDimension7Code;
                            ImprestRequestObj.ShortcutDimension8Code = imprestRequest.ShortcutDimension8Code;
                            ImprestRequestObj.Amount = imprestRequest.Amount;

                        }

                        ImprestRequestObj.ErrorStatus = true;
                        ImprestRequestObj.ErrorMessage = "Kindly provide reason (s) for declining/rejecting this document.";
                        return View(ImprestRequestObj);
                    }

                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectImprestRequest(employeeNo, ImprestRequestObj.No, ImprestRequestObj.Comments, AccountController.GetEmployeeNo()))
                    {
                        responseHeader = "Success";
                        responseMessage = "Imprest request no." + ImprestRequestObj.No + " was successfully rejected.";
                        detailedResponseMessage = "Imprest request no." + ImprestRequestObj.No + " was successfully rejected.";

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

                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }

                    ImprestRequestObj.ErrorStatus = true;
                    ImprestRequestObj.ErrorMessage = "Unable to process the imprest request reject action. " + ServiceConnection.contactICTDepartment + "";
                    return View(ImprestRequestObj);
                }

                if (Command == "View Attachment")
                {

                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(ImprestRequestObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");
                                       
                }

                ImprestRequestObj.ErrorStatus = true;
                ImprestRequestObj.ErrorMessage = "Unable to process the approve/reject action. " + ServiceConnection.contactICTDepartment + "";
                return View(ImprestRequestObj);
            }
            catch (Exception ex)
            {
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
                
                responseHeader = "Approval Error";
                responseMessage = $"Approval Error: {ex.Message}";
                detailedResponseMessage = ex.Message;
                return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
            }
        }

    }
}