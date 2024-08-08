using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.Procurement.BidAnalysis;
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

namespace DynamicsNAV365_StaffPortal.Controllers.ProcurementServices
{
    public class BidAnalysisController : Controller
    {
        string companyName = ServiceConnection.CompanyName;
        static string companyURL = "";

        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        SuccessResponseController successResponse = new SuccessResponseController();
        InfoResponseController infoResponse = new InfoResponseController();
        ErrorResponseController errorResponse = new ErrorResponseController();
        AccountController accountController = new AccountController();
        string employeeNo = "";

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

        public BidAnalysisController()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        #region BidAnalysis Lines

        [ChildActionOnly]
        [Authorize]
        public ActionResult _ViewBidAnalysisLine(string BidAnalysisNo)
        {
            return PartialView(new BidAnalysisLineModel());
        }

        [Authorize]
        public JsonResult GetBidAnalysisLines(string DocumentNo)
        {
            List<BidAnalysisLineModel> purchaseRequisitionLinesList = new List<BidAnalysisLineModel>();
            string imprestlines = "BidAnalysisLines?$filter=Header_No eq '" + DocumentNo + "' &$format=json";

            HttpWebResponse httpResponseDestForeign = Models.Credentials.GetOdataData(imprestlines);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    BidAnalysisLineModel imprestLineObj = new BidAnalysisLineModel();
                    imprestLineObj.ResponseID = (string)config1["Response_Id"];
                    imprestLineObj.VendorNo = (string)config1["Vendor_No"];
                    imprestLineObj.VendorName = (string)config1["Vendor_Name"];
                    imprestLineObj.TotalQuotedAmount = (string)config1["Total_Quoted_Amount"];
                    imprestLineObj.RFQNo = (string)config1["RFQ_No"];
                    //imprestLineObj.Status = (string)config1["Status"];
                    purchaseRequisitionLinesList.Add(imprestLineObj);
                }
            }
            //List<PurchaseRequisitionLineModel> purchaseRequisitionLinesList = JsonConvert.DeserializeObject<List<PurchaseRequisitionLineModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitionLines(DocumentNo));
            return Json(purchaseRequisitionLinesList, JsonRequestBehavior.AllowGet);
            //return Json(JsonConvert.DeserializeObject<List<BidAnalysisLineModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetBidAnalysisLines(DocumentNo)), JsonRequestBehavior.AllowGet);
        }

        #endregion End BidAnalysis Lines

        #region BidAnalysis Approval

        [Authorize]
        public ActionResult BidAnalysisApproval(string BidAnalysisNo)
        {
            try
            {
                if (BidAnalysisNo.Equals(""))
                {
                    return RedirectToAction("OpenEntries", "Approval");
                }

                return View(JsonConvert.DeserializeObject<BidAnalysisHeaderModel>(dynamicsNAVSOAPServices.procurementManagementWS.GetBidAnalysisByNo(BidAnalysisNo)));

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BidAnalysisApproval(BidAnalysisHeaderModel PurchaseRequisitionObj, string Command)
        {
            try
            {
                if (PurchaseRequisitionObj.No.Equals(""))
                {
                    return RedirectToAction("OpenEntries", "Approval");
                }
                if (Command == "Approve")
                {
                    PurchaseRequisitionObj.Comments = PurchaseRequisitionObj.Comments != null ? PurchaseRequisitionObj.Comments : "";
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveBidAnalysis(employeeNo, PurchaseRequisitionObj.No, AccountController.GetEmployeeNo()))
                    {
                        responseHeader = "Success";
                        responseMessage = "" + PurchaseRequisitionObj.No + " approved successfully.";
                        detailedResponseMessage = "" + PurchaseRequisitionObj.No + " approved successfully.";
                        button1ControllerName = "Approval";
                        button1ActionName = "OpenEntries";
                        button1Name = "Ok";
                        button1Parameters = "";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";


                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        PurchaseRequisitionObj.ErrorStatus = true;
                        PurchaseRequisitionObj.ErrorMessage = "Unable to process the Purchase Requisition approve action.  " + ServiceConnection.contactICTDepartment + "";
                        return View(PurchaseRequisitionObj);
                    }
                }
                else if (Command == "Reject")
                {
                    PurchaseRequisitionObj.Comments = PurchaseRequisitionObj.Comments != null ? PurchaseRequisitionObj.Comments : "";
                    if (PurchaseRequisitionObj.Comments.Equals(""))
                    {
                        PurchaseRequisitionObj = JsonConvert.DeserializeObject<BidAnalysisHeaderModel>(dynamicsNAVSOAPServices.procurementManagementWS.GetBidAnalysisByNo(PurchaseRequisitionObj.No));
                        PurchaseRequisitionObj.ErrorStatus = true;
                        PurchaseRequisitionObj.ErrorMessage = "Indicate the reason for declining this document.";
                        return View(PurchaseRequisitionObj);
                    }
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectBidAnalysis(employeeNo, PurchaseRequisitionObj.No, PurchaseRequisitionObj.Comments))
                    {
                        responseHeader = "Success";
                        responseMessage = "" + PurchaseRequisitionObj.No + " rejected successfully.";
                        detailedResponseMessage = "" + PurchaseRequisitionObj.No + " rejected successfully.";
                        button1ControllerName = "Approval";
                        button1ActionName = "OpenEntries";
                        button1Name = "Ok";
                        button1Parameters = "";

                        button2ControllerName = "";
                        button2ActionName = "";
                        button2HasParameters = false;
                        button2Parameters = "";
                        button2Name = "";


                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        PurchaseRequisitionObj.ErrorStatus = true;
                        PurchaseRequisitionObj.ErrorMessage = "Unable to process the Purchase Order reject action.  " + ServiceConnection.contactICTDepartment + "";
                        return View(PurchaseRequisitionObj);
                    }
                }
                else if (Command == "View Attachment")
                {

                    string fileURL = dynamicsNAVSOAPServices.fundsManagementWS.GetAttachment(PurchaseRequisitionObj.No);
                    byte[] bytes = Convert.FromBase64String(fileURL);
                    return File(bytes, "application/pdf");

                }
                else
                {
                    PurchaseRequisitionObj.ErrorStatus = true;
                    PurchaseRequisitionObj.ErrorMessage = "Unable to process the approve/reject action.  " + ServiceConnection.contactICTDepartment + "";
                    return View(PurchaseRequisitionObj);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        #endregion BidAnalysis Approval
    }
}