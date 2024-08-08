using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using DynamicsNAV365_StaffPortal.Models.Inventory.StoreRequisition;
using DynamicsNAV365_StaffPortal.Models.Procurement.PurchaseRequistion;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.Models.Finance.StaffAdvance;

namespace DynamicsNAV365_StaffPortal.Controllers.InventoryServices
{
	public class StoreRequisitionController : Controller
    {
        string companyName = ServiceConnection.CompanyName;
        static string companyURL = "";

        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        BCODATAServices _dcodataServices = new BCODATAServices(companyURL);
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

        // IQueryable<Locations> locations = null;
        IQueryable<DimensionValues> globalDimension1Codes = null;
        IQueryable<DimensionValues> globalDimension2Codes = null;
        IQueryable<DimensionValues> shortcutDimension3Codes = null;
        IQueryable<DimensionValues> shortcutDimension4Codes = null;
        IQueryable<DimensionValues> shortcutDimension5Codes = null;
        IQueryable<DimensionValues> shortcutDimension6Codes = null;
        IQueryable<DimensionValues> shortcutDimension7Codes = null;
        IQueryable<DimensionValues> shortcutDimension8Codes = null;

        List<Items> items = null;
        List<ItemUOMModel> itemUOMs = null;
        //IQueryable<Items> items = null;
       // IQueryable<ItemUOMs> itemUOMs = null;
        List<LocationCodes> locations = null;
        
        AccountController accountController = new AccountController();
        string employeeNo = "";

        public StoreRequisitionController()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        #region New Store Requisition
      
        [Authorize]
        public ActionResult NewStoreRequisition()
        {
            string storeRequisitionNo = "";
            try
            {
                StoreRequisitionHeaderModel storeRequisitionObj = new StoreRequisitionHeaderModel();
                //Check open store requisition
                if (dynamicsNAVSOAPServices.inventoryManagementWS.CheckOpenStoreRequisitionExists(employeeNo))
                {
                    responseHeader = "Open Store Requisition";
                    responseMessage = "An open store requisition exists for employee no. " + employeeNo + ", finalize on this store requisition before creating a new one.";
                    detailedResponseMessage = "An open store requisition exists for employee no. " + employeeNo + ", finalize on this store requisition before creating a new one.";

                    button1ControllerName = "StoreRequisition";
                    button1ActionName = "StoreRequisitionHistory";
                    button1Name = "Ok";

                    button1HasParameters = false;
                    button1Parameters = "";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }
                //End check open store requisition

                //Create a new store requisition
                storeRequisitionNo = dynamicsNAVSOAPServices.inventoryManagementWS.CreateStoreRequisition(employeeNo);
                //End create store requisition

                return RedirectToAction("EditStoreRequisition", new {StoreRequisitionNo = storeRequisitionNo});

                /*storeRequisitionObj.No = storeRequisitionNo;
                storeRequisitionObj.EmployeeNo = employeeNo;
                storeRequisitionObj.GlobalDimension1Code = "";

                LoadLocationCodes();
                LoadDimensions(storeRequisitionObj.GlobalDimension1Code);
                storeRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Name");
                storeRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
                storeRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
                storeRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
                storeRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Code");
                storeRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Code");
                storeRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Code");
                storeRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Code");
                storeRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Code");
                return View(storeRequisitionObj);*/
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [Authorize]
        [HttpPost]
       	[ValidateAntiForgeryToken]
        public async Task<ActionResult> NewStoreRequisition(StoreRequisitionHeaderModel StoreRequisitionObj,string Command)
        {
            bool storeRequisitionModified = false;
            bool approvalWorkflowExist = false;

            LoadLocationCodes();
            LoadDimensions(StoreRequisitionObj.GlobalDimension1Code);
            StoreRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Code");
            StoreRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
            StoreRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Code");

            if (Command.Equals("Submit For Approval"))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (dynamicsNAVSOAPServices.inventoryManagementWS.CheckStoreRequisitionExists(StoreRequisitionObj.No, AccountController.GetEmployeeNo()))
                        {
                            //Check store requisition lines
                            if (!dynamicsNAVSOAPServices.inventoryManagementWS.CheckStoreRequisitionLinesExist(StoreRequisitionObj.No))
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = "Store requisition lines missing, the storeRequisition must contain a minimum of one storeRequisition line, add an storeRequisition line to continue.";
                                return View(StoreRequisitionObj);
                            }
                            //Validate store requisition lines
                            string storeRequisitionLineError = "";
                            storeRequisitionLineError = dynamicsNAVSOAPServices.inventoryManagementWS.ValidateStoreRequisitionLines(StoreRequisitionObj.No);
                            if (!storeRequisitionLineError.Equals(""))
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = storeRequisitionLineError;
                                return View(StoreRequisitionObj);
                            }
                            //Modify store requisition
                            StoreRequisitionObj.RequesterID = StoreRequisitionObj.RequesterID ?? "";

                            storeRequisitionModified = dynamicsNAVSOAPServices.inventoryManagementWS.ModifyStoreRequisition(StoreRequisitionObj.No, employeeNo, DateTime.Parse(StoreRequisitionObj.RequiredDate), StoreRequisitionObj.Description, StoreRequisitionObj.LocationCode, StoreRequisitionObj.GlobalDimension2Code);

                            if (!storeRequisitionModified)
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = "An error was experienced while trying to modify store requisition no." + StoreRequisitionObj.No + ", the server might be offline, try again after a while.";
                                return View(StoreRequisitionObj);
                            }

                            //Send store requisition for approval
                            approvalWorkflowExist = dynamicsNAVSOAPServices.inventoryManagementWS.CheckStoreRequisitionApprovalWorkflowEnabled(StoreRequisitionObj.No);
                            if (!approvalWorkflowExist)
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = "An error was experienced while trying to send an approval request for store requisition no." + StoreRequisitionObj.No + ", the approval workflow was not found. Contact the " + companyName + " ICT department for assistance.";
                                return View(StoreRequisitionObj);
                            }

                            if (dynamicsNAVSOAPServices.inventoryManagementWS.SendStoreRequisitionApprovalRequest(StoreRequisitionObj.No))
                            {
                                responseHeader = "Success";
                                responseMessage = "Store requisition no." + StoreRequisitionObj.No + " was successfully sent for approval. Check with the " + companyName + " inventory department for approval status.";
                                detailedResponseMessage = "Store requisition no." + StoreRequisitionObj.No + " was successfully sent for approval. Check with the " + companyName + " inventory department for approval status.";
                                button1ControllerName = "StoreRequisition";
                                button1ActionName = "StoreRequisitionHistory";
                                button1Name = "Ok";
                                button1HasParameters = false;
                                button1Parameters = "";
                                return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                      button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                      button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                            }
                            else
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = "An error was experienced while trying to send an approval request for store requisition no." + StoreRequisitionObj.No + ". Contact the " + companyName + " ICT department for assistance.";
                                return View(StoreRequisitionObj);
                            }
                        }
                        else
                        {
                            responseHeader = "Store Requisition NotFound";
                            responseMessage = "The store requisition no." + StoreRequisitionObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
                            detailedResponseMessage = "The store requisition no." + StoreRequisitionObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
                            button1ControllerName = "StoreRequisition";
                            button1ActionName = "StoreRequisitionHistory";
                            button1Name = "Ok";
                            button1HasParameters = false;
                            button1Parameters = "";
                            return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        StoreRequisitionObj.ErrorStatus = true;
                        StoreRequisitionObj.ErrorMessage = ex.Message.ToString();
                        return View(StoreRequisitionObj);
                    }
                }
            }
            if (Command.Equals("View Attachment"))
            {
                string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(StoreRequisitionObj.No);

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
                            return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                        }

                        else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
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
                            return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
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
                            return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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
                    return View(StoreRequisitionObj);
                }
            }
            else
            {
                return View(StoreRequisitionObj);
            }
        }
        #endregion New Store Requisition

        #region Edit Store Requisition
    
        [Authorize]
        public ActionResult OnBeforeEdit(string StoreRequisitionNo)
        {
            try
            {
                if (StoreRequisitionNo.Equals(""))
                {
                    return RedirectToAction("StoreRequisitionHistory", "StoreRequisition");
                }
                if (dynamicsNAVSOAPServices.inventoryManagementWS.CheckStoreRequisitionExists(StoreRequisitionNo, AccountController.GetEmployeeNo()))
                {
                    string storeRequisitionStatus = GetStoreRequisitionStatus(StoreRequisitionNo);
                    //if store requisition is open
                    if (storeRequisitionStatus.Equals("Open"))
                    {
                        return RedirectToAction("EditStoreRequisition", "StoreRequisition", new { StoreRequisitionNo = StoreRequisitionNo });
                    }
                    ////if store requisition is pending approval
                    //if (storeRequisitionStatus.Equals("Pending Approval"))
                    //{
                    //	responseHeader = "Store Requisition Pending Approval";
                    //	responseMessage = "The stores requisition no." + StoreRequisitionNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
                    //	detailedResponseMessage = "The stores requisition no." + StoreRequisitionNo + " is pending approval. Editing will cancel the approval request and uncommit the document from the budget. Do you want to continue?";
                    //	returnControllerName = "StoreRequisition";
                    //	returnActionName = "EditStoreRequisition";
                    //	returnLinkName = "Yes";
                    //	hasParameters = true;
                    //	parameters = "?StoreRequisitionNo=" + StoreRequisitionNo;
                    //	cancelControllerName = "StoreRequisition";
                    //	cancelActionName = "StoreRequisitionHistory";
                    //	cancelLinkName = "No";
                    //	return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                    //										   returnControllerName, returnActionName, returnLinkName,
                    //										   hasParameters, parameters, cancelControllerName,
                    //										   cancelActionName, cancelLinkName);
                    //}

                    //if store requisition is pending approval
                    if (storeRequisitionStatus.Equals("Pending Approval"))
                    {
                        responseHeader = "Store Requisition Pending Approval";
                        responseMessage = "The stores requisition no." + StoreRequisitionNo + " is already submitted for approval. Editing not allowed.";
                        detailedResponseMessage = "The stores requisition no." + StoreRequisitionNo + " is already submitted for approval. Editing not allowed.";
                        button1ControllerName = "StoreRequisition";
                        button1ActionName = "StoreRequisitionHistory";
                        button1Name = "Ok";
                        button1HasParameters = false;
                        button1Parameters = "";
                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }

                    //if store requisition is released
                    if (storeRequisitionStatus.Equals("Released"))
                    {
                        responseHeader = "Store Requisition Approved";
                        responseMessage = "The store requisition no." + StoreRequisitionNo + " is already approved. Editing not allowed.";
                        detailedResponseMessage = "The store requisition no." + StoreRequisitionNo + " is already approved. Editing not allowed.";
                        button1ControllerName = "StoreRequisition";
                        button1ActionName = "StoreRequisitionHistory";
                        button1Name = "Ok";
                        button1HasParameters = false;
                        button1Parameters = "";
                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    //if store requisition is rejected
                    if (storeRequisitionStatus.Equals("Rejected"))
                    {
                        responseHeader = "Store Requisition Rejected";
                        responseMessage = "The store requisition no." + StoreRequisitionNo + " was rejected. Editing will reopen the document. Do you want to continue?";
                        detailedResponseMessage = "The store requisition no." + StoreRequisitionNo + " was rejected. Editing will reopen the document. Do you want to continue?";
                        button1ControllerName = "StoreRequisition";
                        button1ActionName = "EditStoreRequisition";
                        button1Name = "Yes";
                        button1HasParameters = true;
                        button1Parameters = "?StoreRequisitionNo=" + StoreRequisitionNo;
                        button2ControllerName = "StoreRequisition";
                        button2ActionName = "StoreRequisitionHistory";
                        button2Name = "No";
                        return infoResponse.ApplicationConfirm(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    //if store requisition is posted/reversed
                    if (storeRequisitionStatus.Equals("Posted") || storeRequisitionStatus.Equals("Reversed"))
                    {
                        responseHeader = "Store Requisition Posted";
                        responseMessage = "The store requisition no." + StoreRequisitionNo + " is already posted. Editing not allowed.";
                        detailedResponseMessage = "The store requisition no." + StoreRequisitionNo + " is already posted. Editing not allowed.";
                        button1ControllerName = "StoreRequisition";
                        button1ActionName = "StoreRequisitionHistory";
                        button1Name = "Ok";
                        button1HasParameters = false;
                        button1Parameters = "";
                        return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                          button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                          button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    return RedirectToAction("StoreRequisitionHistory", "StoreRequisition");
                }
                else
                {
                    responseHeader = "Store Requisition NotFound";
                    responseMessage = "The store requisition no." + StoreRequisitionNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "The store requisition no." + StoreRequisitionNo + " was not found under employee no." + AccountController.GetEmployeeNo();
                    button1ControllerName = "StoreRequisition";
                    button1ActionName = "StoreRequisitionHistory";
                    button1Name = "Ok";
                    button1HasParameters = false;
                    button1Parameters = "";
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
        public ActionResult EditStoreRequisition(string StoreRequisitionNo)
        {
            try
            {
                if (StoreRequisitionNo.Equals(""))
                {
                    return RedirectToAction("StoreRequisitionHistory", "StoreRequisition");
                }
                if (dynamicsNAVSOAPServices.inventoryManagementWS.CheckStoreRequisitionExists(StoreRequisitionNo, AccountController.GetEmployeeNo()))
                {
                    string storeRequisitionStatus = dynamicsNAVSOAPServices.inventoryManagementWS.GetStoreRequisitionStatus(StoreRequisitionNo);

                    //if store requisition is pending approval, cancel approval request
                    if (storeRequisitionStatus.Equals("Pending Approval"))
                    {
                        //dynamicsNAVSOAPServices.inventoryManagementWS.CancelStoreRequisitionApprovalRequest(StoreRequisitionNo);
                        dynamicsNAVSOAPServices.inventoryManagementWS.CancelStoreRequisitionBudgetCommitment(StoreRequisitionNo);
                    }
                    //if store requisition is released, reopen and uncommit from budget
                    if (storeRequisitionStatus.Equals("Released"))
                    {
                        dynamicsNAVSOAPServices.inventoryManagementWS.ReopenStoreRequisition(StoreRequisitionNo);
                        dynamicsNAVSOAPServices.inventoryManagementWS.CancelStoreRequisitionBudgetCommitment(StoreRequisitionNo);
                    }
                    //if store requisition is rejected, reopen document
                    if (storeRequisitionStatus.Equals("Rejected"))
                    {
                        dynamicsNAVSOAPServices.inventoryManagementWS.ReopenStoreRequisition(StoreRequisitionNo);
                    }

                    StoreRequisitionHeaderModel storeRequisitionObj = new StoreRequisitionHeaderModel();
                    //var storeRequisitions = from storeRequisitionsQuery in dynamicsNAVODataServices.dynamicsNAVOData.StoreRequisitions
                    //                        where storeRequisitionsQuery.No.Equals(StoreRequisitionNo)
                    //                        select storeRequisitionsQuery;

                    dynamic storeRequisitions = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.inventoryManagementWS.GetStoreRequisitions(StoreRequisitionNo, ""));
                    foreach (var storeRequisition in storeRequisitions)
                    {
                        //storeRequisitionObj.No = storeRequisition.No;
                        //storeRequisitionObj.EmployeeNo = storeRequisition.Employee_No;
                        //storeRequisitionObj.DocumentDate = storeRequisition.Document_Date != null ? storeRequisition.Document_Date.Value.ToShortDateString() : "n/a";
                        //storeRequisitionObj.RequiredDate = storeRequisition.Required_Date != null ? storeRequisition.Required_Date.Value.ToShortDateString() : "n/a";
                        //storeRequisitionObj.RequesterID = storeRequisition.Requester_ID;
                        //storeRequisitionObj.Amount = (storeRequisition.Amount ?? 0).ToString("N");
                        //storeRequisitionObj.Description = storeRequisition.Description;
                        //storeRequisitionObj.Status = storeRequisition.Status;
                        //storeRequisitionObj.LocationCode = storeRequisition.Location_Code;
                        //storeRequisitionObj.GlobalDimension1Code = storeRequisition.Global_Dimension_1_Code;
                        //storeRequisitionObj.GlobalDimension2Code = storeRequisition.Global_Dimension_2_Code;
                        //storeRequisitionObj.ShortcutDimension3Code = storeRequisition.Shortcut_Dimension_3_Code;
                        //storeRequisitionObj.ShortcutDimension4Code = storeRequisition.Shortcut_Dimension_4_Code;
                        //storeRequisitionObj.ShortcutDimension5Code = storeRequisition.Shortcut_Dimension_5_Code;
                        //storeRequisitionObj.ShortcutDimension6Code = storeRequisition.Shortcut_Dimension_6_Code;
                        //storeRequisitionObj.ShortcutDimension7Code = storeRequisition.Shortcut_Dimension_7_Code;
                        //storeRequisitionObj.ShortcutDimension8Code = storeRequisition.Shortcut_Dimension_8_Code;

                        storeRequisitionObj.No = storeRequisition.No;
                        storeRequisitionObj.DocumentDate = storeRequisition.DocumentDate;
                        storeRequisitionObj.RequiredDate = storeRequisition.RequestedReceiptDate.ToString("dd-MM-yy");
                        storeRequisitionObj.RequesterID = storeRequisition.RequesterID;
                        storeRequisitionObj.EmployeeNo = storeRequisition.EmployeeNo;
                        storeRequisitionObj.LocationCode = storeRequisition.LocationCode;
                        storeRequisitionObj.Description = storeRequisition.Description;
                        storeRequisitionObj.GlobalDimension1Code = storeRequisition.GlobalDimension1Code;
                        storeRequisitionObj.GlobalDimension2Code = storeRequisition.GlobalDimension2Code;
                        storeRequisitionObj.ShortcutDimension3Code = storeRequisition.ShortcutDimension3Code;
                        storeRequisitionObj.ShortcutDimension4Code = storeRequisition.ShortcutDimension4Code;
                        storeRequisitionObj.ShortcutDimension5Code = storeRequisition.ShortcutDimension5Code;
                        storeRequisitionObj.ShortcutDimension6Code = storeRequisition.ShortcutDimension6Code;
                        storeRequisitionObj.ShortcutDimension7Code = storeRequisition.ShortcutDimension7Code;
                        storeRequisitionObj.ShortcutDimension8Code = storeRequisition.ShortcutDimension8Code;
                        storeRequisitionObj.Amount = storeRequisition.Amount;
                        storeRequisitionObj.Status = storeRequisition.Status;
                        storeRequisitionObj.facilityNo = storeRequisition.facilityNo;
                    }

                    LoadLocationCodes();
                    LoadDimensions(storeRequisitionObj.GlobalDimension1Code);
                    storeRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Code");
                    storeRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
                    storeRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Code");
                    storeRequisitionObj.facilitySelect = _dcodataServices.BCOData.Facility_List.Execute().Select(c=> new SelectListItem
                    {
                        Text = $"{c.No}:{c.Facility_Name}",
                        Value = c.No,
                        Selected = c.No == storeRequisitionObj.facilityNo
                    });

                    return View(storeRequisitionObj);
                }

                responseHeader = "Store Requisition NotFound";
                responseMessage = "The store requisition no." + StoreRequisitionNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                detailedResponseMessage = "The store requisition no." + StoreRequisitionNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                button1ControllerName = "StoreRequisition";
                button1ActionName = "StoreRequisitionHistory";
                button1Name = "Ok";
                button1HasParameters = false;
                button1Parameters = "";
                return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditStoreRequisition(StoreRequisitionHeaderModel StoreRequisitionObj, string Command) 
        {
            bool storeRequisitionModified = false;
            bool approvalWorkflowExist = false;

            LoadLocationCodes();
            LoadDimensions(StoreRequisitionObj.GlobalDimension1Code);
            StoreRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Code");
            StoreRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
            StoreRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Code");
            StoreRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Code");
            StoreRequisitionObj.facilitySelect = _dcodataServices.BCOData.Facility_List.Execute().Select(c=> new SelectListItem
            {
                Text = $"{c.No}:{c.Facility_Name}",
                Value = c.No,
                Selected = c.No == StoreRequisitionObj.facilityNo
            });

            if (Command.Equals("Submit For Approval"))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (dynamicsNAVSOAPServices.inventoryManagementWS.CheckStoreRequisitionExists(StoreRequisitionObj.No, AccountController.GetEmployeeNo()))
                        {
                            //Check store requisition lines
                            if (!dynamicsNAVSOAPServices.inventoryManagementWS.CheckStoreRequisitionLinesExist(StoreRequisitionObj.No))
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = "Store requisition lines missing, the storeRequisition must contain a minimum of one storeRequisition line, add an storeRequisition line to continue.";
                                return View(StoreRequisitionObj);
                            }
                            //Validate store requisition lines
                            string storeRequisitionLineError = "";
                            storeRequisitionLineError = dynamicsNAVSOAPServices.inventoryManagementWS.ValidateStoreRequisitionLines(StoreRequisitionObj.No);
                            if (!storeRequisitionLineError.Equals(""))
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = storeRequisitionLineError;
                                return View(StoreRequisitionObj);
                            }
                            //Modify store requisition
                            StoreRequisitionObj.RequesterID = StoreRequisitionObj.RequesterID ?? "";
                            const string format = "MM/dd/yy";
                            DateTime result;
                            var tryParseExact =DateTime.TryParseExact(StoreRequisitionObj.RequiredDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

                            if (!tryParseExact)
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = "A  date convert error was experienced while trying to modify store requisition no." + StoreRequisitionObj.No + ", the server might be offline, try again after a while.";
                                return View(StoreRequisitionObj);
                            }
                            storeRequisitionModified = dynamicsNAVSOAPServices.inventoryManagementWS.ModifyStoreRequisition(StoreRequisitionObj.No, employeeNo, result/*DateTime.Parse(StoreRequisitionObj.RequiredDate)*/, StoreRequisitionObj.Description, StoreRequisitionObj.LocationCode, StoreRequisitionObj.GlobalDimension2Code);

                            if (!storeRequisitionModified)
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = "An error was experienced while trying to modify store requisition no." + StoreRequisitionObj.No + ", the server might be offline, try again after a while.";
                                return View(StoreRequisitionObj);
                            }

                            //Send store requisition for approval
                            approvalWorkflowExist = dynamicsNAVSOAPServices.inventoryManagementWS.CheckStoreRequisitionApprovalWorkflowEnabled(StoreRequisitionObj.No);
                            if (!approvalWorkflowExist)
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = "An error was experienced while trying to send an approval request for store requisition no." + StoreRequisitionObj.No + ", the approval workflow was not found. Contact the " + companyName + " ICT department for assistance.";
                                return View(StoreRequisitionObj);
                            }

                            if (dynamicsNAVSOAPServices.inventoryManagementWS.SendStoreRequisitionApprovalRequest(StoreRequisitionObj.No))
                            {
                                responseHeader = "Success";
                                responseMessage = "Store requisition no." + StoreRequisitionObj.No + " was successfully sent for approval. Check with the " + companyName + " inventory department for approval status.";
                                detailedResponseMessage = "Store requisition no." + StoreRequisitionObj.No + " was successfully sent for approval. Check with the " + companyName + " inventory department for approval status.";
                                button1ControllerName = "StoreRequisition";
                                button1ActionName = "StoreRequisitionHistory";
                                button1Name = "Ok";
                                button1HasParameters = false;
                                button1Parameters = "";
                                return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                                      button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                                      button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                            }
                            else
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = "An error was experienced while trying to send an approval request for store requisition no." + StoreRequisitionObj.No + ". Contact the " + companyName + " ICT department for assistance.";
                                return View(StoreRequisitionObj);
                            }
                        }
                        else
                        {
                            responseHeader = "Store Requisition NotFound";
                            responseMessage = "The store requisition no." + StoreRequisitionObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
                            detailedResponseMessage = "The store requisition no." + StoreRequisitionObj.No + " was not found under employee no." + AccountController.GetEmployeeNo();
                            button1ControllerName = "StoreRequisition";
                            button1ActionName = "StoreRequisitionHistory";
                            button1Name = "Ok";
                            button1HasParameters = false;
                            button1Parameters = "";
                            return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                                                              button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                              button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        StoreRequisitionObj.ErrorStatus = true;
                        StoreRequisitionObj.ErrorMessage = ex.Message.ToString();
                        return View(StoreRequisitionObj);
                    }
                }
            }
            if (Command.Equals("View Attachment"))
            {
                string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(StoreRequisitionObj.No);

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
                            return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                        }

                        else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
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
                            return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
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
                            return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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
                    return View(StoreRequisitionObj);
                }
            }
            else
            {
                return View(StoreRequisitionObj);
            }
        }
        #endregion Edit Store Requisition

        #region View Store Requisition

        [Authorize]
        public ActionResult ViewStoreRequisition(string StoreRequisitionNo)
        {
            try
            {
                if (StoreRequisitionNo.Equals(""))
                {
                    return RedirectToAction("StoreRequisitionHistory", "StoreRequisition");
                }
                if (dynamicsNAVSOAPServices.inventoryManagementWS.CheckStoreRequisitionExists(StoreRequisitionNo, AccountController.GetEmployeeNo()))
                {
                    StoreRequisitionHeaderModel storeRequisitionObj = new StoreRequisitionHeaderModel();
                    //var storeRequisitions = from storeRequisitionsQuery in dynamicsNAVODataServices.dynamicsNAVOData.StoreRequisitions
                    //                        where storeRequisitionsQuery.No.Equals(StoreRequisitionNo)
                    //                        select storeRequisitionsQuery;

                    dynamic storeRequisitions = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.inventoryManagementWS.GetStoreRequisitions(StoreRequisitionNo, ""));
                    foreach (var storeRequisition in storeRequisitions)
                    {
                        //storeRequisitionObj.No = storeRequisition.No;
                        //storeRequisitionObj.EmployeeNo = storeRequisition.Employee_No;
                        //storeRequisitionObj.DocumentDate = storeRequisition.Document_Date != null ? storeRequisition.Document_Date.Value.ToShortDateString() : "n/a";
                        //storeRequisitionObj.RequiredDate = storeRequisition.Required_Date != null ? storeRequisition.Required_Date.Value.ToShortDateString() : "n/a";
                        //storeRequisitionObj.RequesterID = storeRequisition.Requester_ID;
                        //storeRequisitionObj.Amount = (storeRequisition.Amount ?? 0).ToString("N");
                        //storeRequisitionObj.Description = storeRequisition.Description;
                        //storeRequisitionObj.Status = storeRequisition.Status;
                        //storeRequisitionObj.LocationCode = storeRequisition.Location_Code;
                        //storeRequisitionObj.GlobalDimension1Code = storeRequisition.Global_Dimension_1_Code;
                        //storeRequisitionObj.GlobalDimension2Code = storeRequisition.Global_Dimension_2_Code;
                        //storeRequisitionObj.ShortcutDimension3Code = storeRequisition.Shortcut_Dimension_3_Code;
                        //storeRequisitionObj.ShortcutDimension4Code = storeRequisition.Shortcut_Dimension_4_Code;
                        //storeRequisitionObj.ShortcutDimension5Code = storeRequisition.Shortcut_Dimension_5_Code;
                        //storeRequisitionObj.ShortcutDimension6Code = storeRequisition.Shortcut_Dimension_6_Code;
                        //storeRequisitionObj.ShortcutDimension7Code = storeRequisition.Shortcut_Dimension_7_Code;
                        //storeRequisitionObj.ShortcutDimension8Code = storeRequisition.Shortcut_Dimension_8_Code;

                        storeRequisitionObj.No = storeRequisition.No;
                        storeRequisitionObj.DocumentDate = storeRequisition.DocumentDate;
                        storeRequisitionObj.RequiredDate = storeRequisition.RequestedReceiptDate.ToString("dd-MM-yy");
                        storeRequisitionObj.RequesterID = storeRequisition.RequesterID;
                        storeRequisitionObj.EmployeeNo = storeRequisition.EmployeeNo;
                        storeRequisitionObj.LocationCode = storeRequisition.LocationCode;
                        storeRequisitionObj.Description = storeRequisition.Description;
                        storeRequisitionObj.GlobalDimension1Code = storeRequisition.GlobalDimension1Code;
                        storeRequisitionObj.GlobalDimension2Code = storeRequisition.GlobalDimension2Code;
                        storeRequisitionObj.ShortcutDimension3Code = storeRequisition.ShortcutDimension3Code;
                        storeRequisitionObj.ShortcutDimension4Code = storeRequisition.ShortcutDimension4Code;
                        storeRequisitionObj.ShortcutDimension5Code = storeRequisition.ShortcutDimension5Code;
                        storeRequisitionObj.ShortcutDimension6Code = storeRequisition.ShortcutDimension6Code;
                        storeRequisitionObj.ShortcutDimension7Code = storeRequisition.ShortcutDimension7Code;
                        storeRequisitionObj.ShortcutDimension8Code = storeRequisition.ShortcutDimension8Code;
                        storeRequisitionObj.Amount = storeRequisition.Amount;
                        storeRequisitionObj.Status = storeRequisition.Status;
                        storeRequisitionObj.Comments = storeRequisition.Comment;
                        storeRequisitionObj.facilityNo = storeRequisition.facilityNo;
                    }
                    string globalDimension1Code = dynamicsNAVSOAPServices.procurementManagementWS.GetGlobalDimension1Code(StoreRequisitionNo);

                    //LoadDimensions("");
                    LoadDimensions(globalDimension1Code);
                    LoadLocationCodes();
                    storeRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Code");
                    storeRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
                    storeRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Code");
                    storeRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Code");
                    //LoadLocationCodes();
                    //LoadStoreRequisitionDimensions(storeRequisitionObj.GlobalDimension1Code);
                    //storeRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Name");
                    //storeRequisitionObj.GlobalDimension1Codes = new SelectList(globalDimension1Values, "Code", "Code");
                    //storeRequisitionObj.GlobalDimension2Codes = new SelectList(globalDimension2Values, "Code", "Code");
                    //storeRequisitionObj.ShortcutDimension3Codes = new SelectList(shortcutDimension3Values, "Code", "Code");
                    //storeRequisitionObj.ShortcutDimension4Codes = new SelectList(shortcutDimension4Values, "Code", "Code");
                    //storeRequisitionObj.ShortcutDimension5Codes = new SelectList(shortcutDimension5Values, "Code", "Code");
                    //storeRequisitionObj.ShortcutDimension6Codes = new SelectList(shortcutDimension6Values, "Code", "Code");
                    //storeRequisitionObj.ShortcutDimension7Codes = new SelectList(shortcutDimension7Values, "Code", "Code");
                    //storeRequisitionObj.ShortcutDimension8Codes = new SelectList(shortcutDimension8Values, "Code", "Code");

                    return View(storeRequisitionObj);
                }
                else
                {
                    responseHeader = "Store Requisition NotFound";
                    responseMessage = "The store requisition no." + StoreRequisitionNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    detailedResponseMessage = "The store requisition no." + StoreRequisitionNo + " was not found for employee no." + AccountController.GetEmployeeNo();
                    button1ControllerName = "StoreRequisition";
                    button1ActionName = "StoreRequisitionHistory";
                    button1Name = "Ok";
                    button1HasParameters = false;
                    button1Parameters = "";
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
        public async Task<ActionResult> ViewPurchaseRequisition(StoreRequisitionHeaderModel storeRequisitionObj, string Command)
        {
            try
            {
                if (storeRequisitionObj.No.Equals(""))
                {
                    return RedirectToAction("StoreRequisitionHistory", "StoreRequisition");
                }
                if (Command.Equals("View Attachment"))
                {
                    string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(storeRequisitionObj.No);

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
                                return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                            }

                            else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
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
                                return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
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
                                return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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
                        return View(storeRequisitionObj); 
                    }
                }
                else
                {
                    storeRequisitionObj.ErrorStatus = true;
                    //leaveApplicationObj2.ErrorMessage = "Unable to process the approve/reject action. Contact the " + companyName + " for assistance.";
                    return View(storeRequisitionObj);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        #endregion View Store Requisition

        #region Store Requisition Line

        [ChildActionOnly]
        [Authorize]
        public ActionResult _StoreRequisitionLine(string StoreRequisitionNo)
        {
            StoreRequisitionLineModel storeRequisitionLineObj = new StoreRequisitionLineModel();

            //string globalDimension1Code = dynamicsNAVSOAPServices.inventoryManagementWS.GetGlobalDimension1Code(StoreRequisitionNo);
            //LoadStoreRequisitionDimensions(globalDimension1Code);
            Session["storereqno"] = StoreRequisitionNo;
            LoadItems();
            LoadItemUOMs();
            LoadLocationCodes();

            //List<ItemsModel> items = new List<ItemsModel>();

            //dynamic itemCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.procurementManagementWS.GetItems());

            //foreach (var itemCode in itemCodes)
            //{
            //    ItemsModel itemObj = new ItemsModel();
            //    itemObj.Code = itemCode.Code;
            //    itemObj.Description = itemCode.Description;

            //    items.Add(itemObj);
            //}

            storeRequisitionLineObj.Items = new SelectList(items, "No", "Description");
            storeRequisitionLineObj.LineLocationCodes = new SelectList(locations, "Code", "Code");
            storeRequisitionLineObj.UOMs = new SelectList(itemUOMs, "Code", "Code");
            string globalDimension1Code = dynamicsNAVSOAPServices.procurementManagementWS.GetGlobalDimension1Code(StoreRequisitionNo);

            //LoadDimensions("");
            LoadDimensions(globalDimension1Code);
            LoadLocationCodes();
            storeRequisitionLineObj.LineLocationCodes = new SelectList(locations, "Code", "Code");
            storeRequisitionLineObj.LineGlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
            storeRequisitionLineObj.LineGlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Code");
            //   storeRequisitionLineObj.UOMs = new SelectList(Enumerable.Empty<SelectListItem>());
            return PartialView(storeRequisitionLineObj);
        }

        [ChildActionOnly]
        [Authorize]
        public ActionResult _ViewStoreRequisitionLine(string StoreRequisitionNo)
        {
            StoreRequisitionLineModel storeRequisitionLineObj = new StoreRequisitionLineModel();

            //string globalDimension1Code = dynamicsNAVSOAPServices.inventoryManagementWS.GetGlobalDimension1Code(StoreRequisitionNo);
            //LoadStoreRequisitionDimensions(globalDimension1Code);

            LoadItems();
            LoadItemUOMs();
            LoadLocationCodes();

         //   List<ItemsModel> items = new List<ItemsModel>();

            //dynamic itemCodes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.procurementManagementWS.GetItems());

            //foreach (var itemCode in itemCodes)
            //{
            //    ItemsModel itemObj = new ItemsModel();
            //    itemObj.Code = itemCode.Code;
            //    itemObj.Description = itemCode.Description;

            //    items.Add(itemObj);
            //}


            storeRequisitionLineObj.Items = new SelectList(items, "No", "Description");
            storeRequisitionLineObj.LineLocationCodes = new SelectList(locations, "Code", "Code");
            storeRequisitionLineObj.UOMs = new SelectList(itemUOMs, "Code", "Code");
            string globalDimension1Code = dynamicsNAVSOAPServices.procurementManagementWS.GetGlobalDimension1Code(StoreRequisitionNo);

            //LoadDimensions("");
            LoadDimensions(globalDimension1Code);
            LoadLocationCodes();
            storeRequisitionLineObj.LineLocationCodes = new SelectList(locations, "Code", "Code");
            storeRequisitionLineObj.LineGlobalDimension1Codes = new SelectList(globalDimension1Codes, "Code", "Code");
            storeRequisitionLineObj.LineGlobalDimension2Codes = new SelectList(globalDimension2Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension3Codes = new SelectList(shortcutDimension3Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension4Codes = new SelectList(shortcutDimension4Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension5Codes = new SelectList(shortcutDimension5Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension6Codes = new SelectList(shortcutDimension6Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension7Codes = new SelectList(shortcutDimension7Codes, "Code", "Code");
            storeRequisitionLineObj.LineShortcutDimension8Codes = new SelectList(shortcutDimension8Codes, "Code", "Code");
            return PartialView(storeRequisitionLineObj);
        }

        [Authorize]
        [Authorize]
        public ActionResult GetStoreRequisitionLines(string DocumentNo)
        {
            try
            {
                List<StoreRequisitionLineModel> storeRequisitionLines = new List<StoreRequisitionLineModel>();
                string imprestlines = "RequisitionLine?$filter=Requisition_No eq '" + DocumentNo + "'&format=json";

                HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(imprestlines);
                using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                {
                    var result1 = streamReader1.ReadToEnd();

                    var details1 = JObject.Parse(result1);

                    foreach (JObject config1 in details1["value"])
                    {
                        StoreRequisitionLineModel storeRequisitionLineObj = new StoreRequisitionLineModel();
                        storeRequisitionLineObj.LineNo = (string)config1["Line_No"];
                        storeRequisitionLineObj.DocumentNo = (string)config1["Requisition_No"];
                        storeRequisitionLineObj.Type = (string)config1["Type"];
                        storeRequisitionLineObj.ItemNo = (string)config1["No"];
                        storeRequisitionLineObj.ItemDescription = (string)config1["Description"];
                        storeRequisitionLineObj.Inventory = (string)config1["Quantity_in_Store"];
                        storeRequisitionLineObj.LineLocationCode = (string)config1["Location_Code"];
                        storeRequisitionLineObj.UOM = (string)config1["Unit_of_Measure"];
                        storeRequisitionLineObj.UnitCost = (string)config1["Unit_Price"];
                        storeRequisitionLineObj.Quantity = (string)config1["Quantity"];
                        storeRequisitionLineObj.LineGlobalDimension1Code = (string)config1["Global_Dimension_1_Code"];
                        storeRequisitionLineObj.LineGlobalDimension2Code = (string)config1["Global_Dimension_2_Code"];
                        storeRequisitionLineObj.LineShortcutDimension3Code = (string)config1["ShortcutDimCode3"];
                        storeRequisitionLineObj.LineShortcutDimension4Code = (string)config1["ShortcutDimCode4"];
                        storeRequisitionLineObj.LineShortcutDimension5Code = (string)config1["ShortcutDimCode5"];
                        storeRequisitionLineObj.LineShortcutDimension6Code = (string)config1["ShortcutDimCode6"];
                        storeRequisitionLineObj.LineShortcutDimension7Code = (string)config1["ShortcutDimCode7"];
                        storeRequisitionLineObj.LineShortcutDimension8Code = (string)config1["ShortcutDimCode8"];
                        storeRequisitionLines.Add(storeRequisitionLineObj);
                    }
                }

                return Json(storeRequisitionLines, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        public JsonResult GetStoreRequisitionLines1(string DocumentNo)
        {
            List<StoreRequisitionLineModel> storeRequisitionLinesList = new List<StoreRequisitionLineModel>();

            //var storeRequisitionLines = from storeRequisitionLinesQuery in dynamicsNAVODataServices.dynamicsNAVOData.StoreRequisitionLines
            //                            where storeRequisitionLinesQuery.Document_No.Equals(DocumentNo)
            //                            select storeRequisitionLinesQuery;
            
            dynamic storeRequisitionLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.inventoryManagementWS.GetStoreRequisitionLines(DocumentNo));

            foreach (var storeRequisitionLine in storeRequisitionLines)
            {
                StoreRequisitionLineModel storeRequisitionLineObj = new StoreRequisitionLineModel();
                storeRequisitionLineObj.LineNo = storeRequisitionLine.LineNo;
                storeRequisitionLineObj.DocumentNo = storeRequisitionLine.DocumentNo;
                storeRequisitionLineObj.ItemNo = storeRequisitionLine.ItemNo;
                storeRequisitionLineObj.ItemDescription = storeRequisitionLine.ItemDescription;
                storeRequisitionLineObj.LineLocationCode = storeRequisitionLine.LineLocationCode;
                storeRequisitionLineObj.Quantity = storeRequisitionLine.Quantity;
                storeRequisitionLineObj.Inventory = storeRequisitionLine.Inventory;
                storeRequisitionLineObj.UnitCost = storeRequisitionLine.UnitCost;
                storeRequisitionLineObj.UOM = storeRequisitionLine.UOM;
                storeRequisitionLineObj.LineDescription = storeRequisitionLine.LineDescription;
                storeRequisitionLineObj.LineGlobalDimension1Code = storeRequisitionLine.LineGlobalDimension1Code;
                storeRequisitionLineObj.LineGlobalDimension2Code = storeRequisitionLine.LineGlobalDimension2Code;
                storeRequisitionLineObj.LineShortcutDimension3Code = storeRequisitionLine.LineShortcutDimension3Code;
                storeRequisitionLineObj.LineShortcutDimension4Code = storeRequisitionLine.LineShortcutDimension4Code;
                storeRequisitionLineObj.LineShortcutDimension5Code = storeRequisitionLine.LineShortcutDimension5Code;
                storeRequisitionLineObj.LineShortcutDimension6Code = storeRequisitionLine.LineShortcutDimension6Code;
                storeRequisitionLineObj.LineShortcutDimension7Code = storeRequisitionLine.LineShortcutDimension7Code;
                storeRequisitionLineObj.LineShortcutDimension8Code = storeRequisitionLine.LineShortcutDimension8Code;
                storeRequisitionLinesList.Add(storeRequisitionLineObj);
            }
            return Json(storeRequisitionLinesList, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult GetStoreRequisitionLine(string LineNo, string DocumentNo)
        {
            try
            {  
                List<StoreRequisitionLineModel> purchaseRequisitionLinesList = new List<StoreRequisitionLineModel>();
                string imprestlines = "RequisitionLine?$filter=Requisition_No eq '" + DocumentNo + "'and Line_No eq " + Convert.ToInt32(LineNo) + "  &$format=json";

                HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(imprestlines);
                using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                {
                    var result1 = streamReader1.ReadToEnd();

                    var details1 = JObject.Parse(result1);

                    foreach (JObject config1 in details1["value"])
                    {
                        StoreRequisitionLineModel imprestLineObj = new StoreRequisitionLineModel();
                        imprestLineObj.LineNo = (string)config1["Line_No"];
                        imprestLineObj.DocumentNo = (string)config1["Requisition_No"];
                        imprestLineObj.Type = (string)config1["Type"];                       
                        imprestLineObj.ItemNo = (string)config1["No"];
                        imprestLineObj.LineLocationCode = (string)config1["Location_Code"];
                        imprestLineObj.ItemDescription = (string)config1["Description"];
                        imprestLineObj.Inventory = (string)config1["Quantity_in_Store"];                      
                        imprestLineObj.UOM = (string)config1["Unit_of_Measure"];
                        imprestLineObj.UnitCost = (string)config1["Unit_Price"];
                        imprestLineObj.Quantity = (string)config1["Quantity"];                       
                        imprestLineObj.LineGlobalDimension1Code = (string)config1["Global_Dimension_1_Code"];
                        imprestLineObj.LineGlobalDimension2Code = (string)config1["Global_Dimension_2_Code"];
                        imprestLineObj.LineShortcutDimension3Code = (string)config1["ShortcutDimCode3"];
                        imprestLineObj.LineShortcutDimension4Code = (string)config1["ShortcutDimCode4"];
                        imprestLineObj.LineShortcutDimension5Code = (string)config1["ShortcutDimCode5"];
                        imprestLineObj.LineShortcutDimension6Code = (string)config1["ShortcutDimCode6"];
                        imprestLineObj.LineShortcutDimension7Code = (string)config1["ShortcutDimCode7"];
                        imprestLineObj.LineShortcutDimension8Code = (string)config1["ShortcutDimCode8"];
                        purchaseRequisitionLinesList.Add(imprestLineObj);
                    }
                }
                //List<PurchaseRequisitionLineModel> purchaseRequisitionLinesList = JsonConvert.DeserializeObject<List<PurchaseRequisitionLineModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitionLines(DocumentNo));
                return Json(purchaseRequisitionLinesList.FirstOrDefault(), JsonRequestBehavior.AllowGet);

                //PurchaseRequisitionLineModel purchaseRequisitionLineObj = JsonConvert.DeserializeObject<PurchaseRequisitionLineModel>(dynamicsNAVSOAPServices.procurementManagementWS.GetPurchaseRequisitionByLineNo(Convert.ToInt32(LineNo), DocumentNo));
                //return Json(purchaseRequisitionLineObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        [Authorize]
        public JsonResult GetStoreRequisitionLine1(string LineNo, string DocumentNo)
        {
            StoreRequisitionLineModel storeRequisitionLineObj = new StoreRequisitionLineModel();

            //var storeRequisitionLines = from storeRequisitionLinesQuery in dynamicsNAVODataServices.dynamicsNAVOData.StoreRequisitionLines
            //                            where storeRequisitionLinesQuery.Line_No.Equals(LineNo) && storeRequisitionLinesQuery.Document_No.Equals(DocumentNo)
            //                            select storeRequisitionLinesQuery;
         
            dynamic storeRequisitionLines = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.inventoryManagementWS.GetStoreRequisitionByLineNo(Convert.ToInt32(LineNo), DocumentNo));

            foreach (var storeRequisitionLine in storeRequisitionLines)
            {
                //storeRequisitionLineObj.LineNo = storeRequisitionLine.Line_No;
                //storeRequisitionLineObj.DocumentNo = storeRequisitionLine.Document_No;
                //storeRequisitionLineObj.ItemNo = storeRequisitionLine.Item_No;
                //storeRequisitionLineObj.ItemDescription = storeRequisitionLine.Item_Description;
                //storeRequisitionLineObj.LineLocationCode = storeRequisitionLine.Location_Code;
                //storeRequisitionLineObj.UOM = storeRequisitionLine.Unit_of_Measure_Code;
                //storeRequisitionLineObj.Inventory = storeRequisitionLine.Inventory ?? 0;
                //storeRequisitionLineObj.Quantity = storeRequisitionLine.Quantity ?? 0;
                //storeRequisitionLineObj.LineDescription = storeRequisitionLine.Description;
                //storeRequisitionLineObj.LineGlobalDimension1Code = storeRequisitionLine.Global_Dimension_1_Code;
                //storeRequisitionLineObj.LineGlobalDimension2Code = storeRequisitionLine.Global_Dimension_2_Code;
                //storeRequisitionLineObj.LineShortcutDimension3Code = storeRequisitionLine.Shortcut_Dimension_3_Code;
                //storeRequisitionLineObj.LineShortcutDimension4Code = storeRequisitionLine.Shortcut_Dimension_4_Code;
                //storeRequisitionLineObj.LineShortcutDimension5Code = storeRequisitionLine.Shortcut_Dimension_5_Code;
                //storeRequisitionLineObj.LineShortcutDimension6Code = storeRequisitionLine.Shortcut_Dimension_6_Code;
                //storeRequisitionLineObj.LineShortcutDimension7Code = storeRequisitionLine.Shortcut_Dimension_7_Code;
                //storeRequisitionLineObj.LineShortcutDimension8Code = storeRequisitionLine.Shortcut_Dimension_8_Code;

                storeRequisitionLineObj.LineNo = storeRequisitionLine.LineNo;
                storeRequisitionLineObj.DocumentNo = storeRequisitionLine.DocumentNo;
                storeRequisitionLineObj.ItemNo = storeRequisitionLine.ItemNo;
                storeRequisitionLineObj.ItemDescription = storeRequisitionLine.ItemDescription;
                storeRequisitionLineObj.LineLocationCode = storeRequisitionLine.LineLocationCode;
                storeRequisitionLineObj.Quantity = storeRequisitionLine.Quantity;
                storeRequisitionLineObj.Inventory = storeRequisitionLine.Inventory;
                storeRequisitionLineObj.UnitCost = storeRequisitionLine.UnitCost;
                storeRequisitionLineObj.UOM = storeRequisitionLine.UOM;
                storeRequisitionLineObj.LineDescription = storeRequisitionLine.LineDescription;
            }

            return Json(storeRequisitionLineObj, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult CreateStoreRequisitionLine(StoreRequisitionLineModel StoreRequisitionLineObj)
        {
            try
            {
                bool storeRequisitionLineCreated = false;

                StoreRequisitionLineObj.UOM = StoreRequisitionLineObj.UOM ?? "";
                StoreRequisitionLineObj.ItemDescription = StoreRequisitionLineObj.ItemDescription ?? "";
                StoreRequisitionLineObj.LineDescription = StoreRequisitionLineObj.LineDescription ?? "";
                StoreRequisitionLineObj.LineLocationCode = StoreRequisitionLineObj.LineLocationCode ?? "";
                StoreRequisitionLineObj.LineGlobalDimension1Code = StoreRequisitionLineObj.LineGlobalDimension1Code ?? "";
                StoreRequisitionLineObj.LineGlobalDimension2Code = StoreRequisitionLineObj.LineGlobalDimension2Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension3Code = StoreRequisitionLineObj.LineShortcutDimension3Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension4Code = StoreRequisitionLineObj.LineShortcutDimension4Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension5Code = StoreRequisitionLineObj.LineShortcutDimension5Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension6Code = StoreRequisitionLineObj.LineShortcutDimension6Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension7Code = StoreRequisitionLineObj.LineShortcutDimension7Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension8Code = StoreRequisitionLineObj.LineShortcutDimension8Code ?? "";
                //var getstore = dynamicsNAVSOAPServices.procurementManagementWS.GetStoreRequisitionLocation(StoreRequisitionLineObj.DocumentNo);
                var getstore = _dcodataServices.BCOData.HR_Employee.Where(c=>c.No == employeeNo).FirstOrDefault()?.Location_Code;
                //decimal availableInventory = dynamicsNAVSOAPServices.inventoryManagementWS.GetAvailableInventory(StoreRequisitionLineObj.ItemNo, getstore);
                decimal? availableInventory = 0;
                //availableInventory = dynamicsNAVSOAPServices.inventoryManagementWS.GetAvailableInventory(ItemNo,getstore);
                var httpResponseDestForeign =
                    Credentials.GetOdataData("ItemsList?$filter=No eq '" + StoreRequisitionLineObj.ItemNo + "' &$format=json");
                httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
                using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
                {
                    var result1 = streamReader1.ReadToEnd();
                    var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<Items>>(result1);
                    availableInventory = StaffAdvanceList?.ListValues?.FirstOrDefault()?.Inventory;
                }
                /*if(Convert.ToDecimal(StoreRequisitionLineObj.Quantity)> availableInventory)
                {
                    return Json(new { success = false, message = "The quantity requested cannot be more than the available inventory" }, JsonRequestBehavior.AllowGet);
                }*/
                StoreRequisitionLineObj.LineLocationCode = getstore;
                
                storeRequisitionLineCreated = dynamicsNAVSOAPServices.inventoryManagementWS.CreateStoreRequisitionLine(StoreRequisitionLineObj.DocumentNo,
                    StoreRequisitionLineObj.ItemNo, StoreRequisitionLineObj.LineLocationCode,
                    Convert.ToDecimal(StoreRequisitionLineObj.Quantity), StoreRequisitionLineObj.UOM, StoreRequisitionLineObj.LineDescription,
                    StoreRequisitionLineObj.LineGlobalDimension1Code, StoreRequisitionLineObj.LineGlobalDimension2Code,
                    StoreRequisitionLineObj.LineShortcutDimension3Code,
                    StoreRequisitionLineObj.LineShortcutDimension4Code,
                    StoreRequisitionLineObj.LineShortcutDimension5Code,
                    StoreRequisitionLineObj.LineShortcutDimension6Code,
                    StoreRequisitionLineObj.LineShortcutDimension7Code,Convert.ToDecimal(StoreRequisitionLineObj.Inventory??"0.00"));

                return Json(new { success = storeRequisitionLineCreated }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false ,message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult ModifyStoreRequisitionLine(StoreRequisitionLineModel StoreRequisitionLineObj)
        {
            try
            {
                bool storeRequisitionLineModified = false;

                StoreRequisitionLineObj.UOM = StoreRequisitionLineObj.UOM ?? "";
                StoreRequisitionLineObj.ItemDescription = StoreRequisitionLineObj.ItemDescription ?? "";
                StoreRequisitionLineObj.LineDescription = StoreRequisitionLineObj.LineDescription ?? "";
                StoreRequisitionLineObj.LineLocationCode = StoreRequisitionLineObj.LineLocationCode ?? "";
                StoreRequisitionLineObj.LineGlobalDimension1Code = StoreRequisitionLineObj.LineGlobalDimension1Code ?? "";
                StoreRequisitionLineObj.LineGlobalDimension2Code = StoreRequisitionLineObj.LineGlobalDimension2Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension3Code = StoreRequisitionLineObj.LineShortcutDimension3Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension4Code = StoreRequisitionLineObj.LineShortcutDimension4Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension5Code = StoreRequisitionLineObj.LineShortcutDimension5Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension6Code = StoreRequisitionLineObj.LineShortcutDimension6Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension7Code = StoreRequisitionLineObj.LineShortcutDimension7Code ?? "";
                StoreRequisitionLineObj.LineShortcutDimension8Code = StoreRequisitionLineObj.LineShortcutDimension8Code ?? "";

                storeRequisitionLineModified = dynamicsNAVSOAPServices.inventoryManagementWS.ModifyStoreRequisitionLine(Convert.ToInt32(StoreRequisitionLineObj.LineNo),
                    StoreRequisitionLineObj.DocumentNo, StoreRequisitionLineObj.ItemNo, Convert.ToDecimal(StoreRequisitionLineObj.Quantity),
                    StoreRequisitionLineObj.UOM, StoreRequisitionLineObj.ItemDescription, StoreRequisitionLineObj.LineLocationCode, StoreRequisitionLineObj.LineGlobalDimension1Code, StoreRequisitionLineObj.LineGlobalDimension2Code, StoreRequisitionLineObj.LineShortcutDimension3Code);
                return Json(new { success = storeRequisitionLineModified }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message=e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult DeleteStoreRequisitionLine(string LineNo, string DocumentNo)
        {
            var storeRequisitionLineDeleted = false;

            storeRequisitionLineDeleted = dynamicsNAVSOAPServices.inventoryManagementWS.DeleteStoreRequisitionLine(Convert.ToInt32(LineNo), DocumentNo);

            return Json(new { StoreRequisitionLineDeleted = storeRequisitionLineDeleted }, JsonRequestBehavior.AllowGet);
        }
       
        #endregion Store Requisition Line

        #region Store requisitions history
       
        [Authorize]
        public ActionResult StoreRequisitionHistory()
        {
            try
            {
                var storeRequisitions = dynamicsNAVSOAPServices.procurementManagementWS.GetStoreRequisitions("", employeeNo);
                return View(JsonConvert.DeserializeObject<List<StoreRequisitionHeaderModel>>(storeRequisitions));
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        #endregion Store requisitions history

        #region Store Requisition Approval
      
        [Authorize]
        [HttpGet]
        public ActionResult StoreRequisitionApproval(string StoreRequisitionNo)
        {
            try
            {
                if (StoreRequisitionNo.Equals(""))
                {
                    return RedirectToAction("OpenEntries", "Approval");
                }
                StoreRequisitionHeaderModel StoreRequisitionObj = new StoreRequisitionHeaderModel();

                //var storeRequisitions = from storeRequisitionsQuery in dynamicsNAVODataServices.dynamicsNAVOData.StoreRequisitions
                //                        where storeRequisitionsQuery.No.Equals(StoreRequisitionNo)
                //                        select storeRequisitionsQuery;

                dynamic storeRequisitions = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.inventoryManagementWS.GetStoreRequisitions(StoreRequisitionNo, ""));

                foreach (var storeRequisition in storeRequisitions)
                {
                    StoreRequisitionObj.No = storeRequisition.No;
                    StoreRequisitionObj.DocumentDate = storeRequisition.DocumentDate;
                    StoreRequisitionObj.RequesterID = storeRequisition.RequesterID;
                    StoreRequisitionObj.RequiredDate = storeRequisition.RequestedReceiptDate.ToString("dd-MM-yy");
                    StoreRequisitionObj.EmployeeNo = storeRequisition.EmployeeNo;
                    StoreRequisitionObj.LocationCode = storeRequisition.LocationCode;
                    StoreRequisitionObj.Description = storeRequisition.Description;
                    StoreRequisitionObj.GlobalDimension1Code = storeRequisition.GlobalDimension1Code;
                    StoreRequisitionObj.GlobalDimension2Code = storeRequisition.GlobalDimension2Code;
                    StoreRequisitionObj.ShortcutDimension3Code = storeRequisition.ShortcutDimension3Code;
                    StoreRequisitionObj.ShortcutDimension4Code = storeRequisition.ShortcutDimension4Code;
                    StoreRequisitionObj.ShortcutDimension5Code = storeRequisition.ShortcutDimension5Code;
                    StoreRequisitionObj.ShortcutDimension6Code = storeRequisition.ShortcutDimension6Code;
                    StoreRequisitionObj.ShortcutDimension7Code = storeRequisition.ShortcutDimension7Code;
                    StoreRequisitionObj.ShortcutDimension8Code = storeRequisition.ShortcutDimension8Code;
                    StoreRequisitionObj.Amount = storeRequisition.Amount;
                    StoreRequisitionObj.Status = storeRequisition.Status;
                }

                LoadLocationCodes();
                StoreRequisitionObj.LocationCodes = new SelectList(locations, "Code", "Code");

                return View(StoreRequisitionObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StoreRequisitionApproval(StoreRequisitionHeaderModel StoreRequisitionObj, string Command)
        {
            try
            {
                if (StoreRequisitionObj.No.Equals(""))
                {
                    return RedirectToAction("OpenEntries", "Approval");
                }
                if (Command == "Approve")
                {
                    StoreRequisitionObj.Comments = StoreRequisitionObj.Comments ?? "";
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApproveRequisition(employeeNo, StoreRequisitionObj.No, AccountController.GetEmployeeNo()))
                    {
                        responseHeader = "Success";
                        responseMessage = "Store Requisition no." + StoreRequisitionObj.No + " was successfully approved.";
                        detailedResponseMessage = "Store Requisition no." + StoreRequisitionObj.No + " was successfully approved.";
                        button1ControllerName = "Approval";
                        button1ActionName = "OpenEntries";
                        button1Name = "Ok";
                        button1HasParameters = false;
                        button1Parameters = "";
                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        StoreRequisitionObj.ErrorStatus = true;
                        StoreRequisitionObj.ErrorMessage = "Unable to process the Store Requisition approve action. " + ServiceConnection.contactICTDepartment + "";
						return View(StoreRequisitionObj);
                    }
                }
                else if (Command == "Reject")
                {
                    StoreRequisitionObj.Comments = StoreRequisitionObj.Comments ?? "";
                    if (StoreRequisitionObj.Comments.Equals(""))
                    {
                        dynamic storeRequisitions = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.inventoryManagementWS.GetStoreRequisitions(StoreRequisitionObj.No, ""));

                        foreach (var storeRequisition in storeRequisitions)
                        {
                            StoreRequisitionObj.No = storeRequisition.No;
                            StoreRequisitionObj.DocumentDate = storeRequisition.DocumentDate;
                            StoreRequisitionObj.RequesterID = storeRequisition.RequesterID;
                            StoreRequisitionObj.RequiredDate = storeRequisition.RequestedReceiptDate.ToString("dd-MM-yy");
                            StoreRequisitionObj.EmployeeNo = storeRequisition.EmployeeNo;
                            StoreRequisitionObj.LocationCode = storeRequisition.LocationCode;
                            StoreRequisitionObj.Description = storeRequisition.Description;
                            StoreRequisitionObj.GlobalDimension1Code = storeRequisition.GlobalDimension1Code;
                            StoreRequisitionObj.GlobalDimension2Code = storeRequisition.GlobalDimension2Code;
                            StoreRequisitionObj.ShortcutDimension3Code = storeRequisition.ShortcutDimension3Code;
                            StoreRequisitionObj.ShortcutDimension4Code = storeRequisition.ShortcutDimension4Code;
                            StoreRequisitionObj.ShortcutDimension5Code = storeRequisition.ShortcutDimension5Code;
                            StoreRequisitionObj.ShortcutDimension6Code = storeRequisition.ShortcutDimension6Code;
                            StoreRequisitionObj.ShortcutDimension7Code = storeRequisition.ShortcutDimension7Code;
                            StoreRequisitionObj.ShortcutDimension8Code = storeRequisition.ShortcutDimension8Code;
                            StoreRequisitionObj.Amount = storeRequisition.Amount;
                            StoreRequisitionObj.Status = storeRequisition.Status;
                        }

                        StoreRequisitionObj.ErrorStatus = true;
                        StoreRequisitionObj.ErrorMessage = "Kindly provide reason (s) for declining/rejecting this document.";
                        return View(StoreRequisitionObj);
                    }
                    if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectRequisition(employeeNo, StoreRequisitionObj.No, StoreRequisitionObj.Comments))
                    {
                        responseHeader = "Success";
                        responseMessage = "Store Requisition no." + StoreRequisitionObj.No + " was successfully rejected.";
                        detailedResponseMessage = "Store Requisition no." + StoreRequisitionObj.No + " was successfully rejected.";
                        button1ControllerName = "Approval";
                        button1ActionName = "OpenEntries";
                        button1Name = "Ok";
                        button1HasParameters = false;
                        button1Parameters = "";
                        return successResponse.ApplicationSuccess(responseHeader, responseMessage, detailedResponseMessage,
                                                    button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                                                    button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                    }
                    else
                    {
                        StoreRequisitionObj.ErrorStatus = true;
                        StoreRequisitionObj.ErrorMessage = "Unable to process the Store Requisition reject action. " + ServiceConnection.contactICTDepartment + "";
						return View(StoreRequisitionObj);
                    }
                }
                else if (Command == "View Attachment")
                {
                    string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(StoreRequisitionObj.No);

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
                                return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                            }

                            else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
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
                                return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
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
                                return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                            }

                            else
                            {
                                StoreRequisitionObj.ErrorStatus = true;
                                StoreRequisitionObj.ErrorMessage = "No file attached. This is because store's document attachment is not mandatory. ";
                                return View(StoreRequisitionObj);
                            }
                        }
                    }


                    else
                    {
                        return View(StoreRequisitionObj);
                    }
                }
                else
                {
                    StoreRequisitionObj.ErrorStatus = true;
                    StoreRequisitionObj.ErrorMessage = "Unable to process the approve/reject action. "+ ServiceConnection.contactICTDepartment + "";

                    return View(StoreRequisitionObj);
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        #endregion Store Requisition ApprovalERPUpgrade55

        #region Document Management 
     
        [ChildActionOnly]
        [Authorize]
        public ActionResult _StoreRequisitionDocument(string DocumentNo)
        {
            DocumentMgmtModel portalDocumentObj = new DocumentMgmtModel();
            return PartialView(portalDocumentObj);
        }

        [ChildActionOnly]
        [Authorize]
        public ActionResult _ViewStoreRequisitionDocument(string DocumentNo)
        {
            DocumentMgmtModel portalDocumentObj = new DocumentMgmtModel();
            return PartialView(portalDocumentObj);
        }

        [Authorize]
        public JsonResult LoadStoreRequisitionDocuments(string DocumentNo)
        {
            List<DocumentMgmtModel> portalDocumentsList = new List<DocumentMgmtModel>();

            dynamic portalDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetPortalDocuments(DocumentNo));

            foreach (var portalDocument in portalDocuments)
            {
                DocumentMgmtModel documentManagementObj = new DocumentMgmtModel();
                documentManagementObj.DocumentNo = portalDocument.DocumentNo;
                documentManagementObj.DocumentCode = portalDocument.DocumentCode;
                documentManagementObj.DocumentAttached = portalDocument.DocumentAttached;
                documentManagementObj.DocumentDescription = portalDocument.DocumentDescription;
                documentManagementObj.FileName = portalDocument.FileName;

                portalDocumentsList.Add(documentManagementObj);
            }

            return Json(portalDocumentsList, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult GetStoreRequisitionDocument(string DocumentNo, string DocumentCode)
        {
            try
            {
                DocumentMgmtModel portalDocumentObj = new DocumentMgmtModel();

                dynamic portalDocuments = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetLeaveApplicationDocument(DocumentNo, DocumentCode));

                foreach (var portalDocument in portalDocuments)
                {
                    portalDocumentObj.DocumentNo = portalDocument.DocumentNo;
                    portalDocumentObj.DocumentCode = portalDocument.DocumentCode;
                    portalDocumentObj.DocumentAttached = portalDocument.DocumentAttached;
                    portalDocumentObj.DocumentDescription = portalDocument.DocumentDescription;
                    portalDocumentObj.FileName = portalDocument.FileName;
                }

                return Json(portalDocumentObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        /*[Authorize]
        [HttpPost]
        public JsonResult UploadStoreRequisitionAttachments(string DocumentNo, string DocumentCode, string DocumentDescription)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var root = "~/StaffData/";
                    bool folderpath = System.IO.Directory.Exists(HttpContext.Server.MapPath(root));

                    if (!folderpath)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Server.MapPath(root));
                    }

                    var file = Request.Files[0];
                    string fileExt = System.IO.Path.GetExtension(file.FileName).ToLower();
                    string fileName = DocumentNo + "_" + DocumentDescription + fileExt;
                    string path = System.IO.Path.Combine(HttpContext.Server.MapPath(root), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    if (!fileExt.Equals(".pdf"))
                    {
                        return Json(new { success = false, message = "Only pdf formats are allowed to be uploaded. Please convert your " + fileExt + " file to pdf for uploading" }, JsonRequestBehavior.AllowGet);
                    }

                    file.SaveAs(path);

                    if (System.IO.File.Exists(path))
                    {
                      //  dynamicsNAVSOAPServices.documentMgmt.ModifySystemFileURL(DocumentNo, DocumentCode, fileName);

                        string username = ServiceConnection.sharePointUser;
                        string Password = ServiceConnection.sharePointUserPassword;

                        var securePassword = new SecureString();
                        foreach (char c in Password)
                        {
                            securePassword.AppendChar(c);
                        }

                        using (var ctx = new ClientContext(ServiceConnection.sharePointSiteUrl))
                        {
                            ctx.Credentials = new Microsoft.SharePoint.Client.SharePointOnlineCredentials(username, securePassword);
                            Web web = ctx.Web;
                            ctx.Load(web);

                            //Ssl3: Secure Socket Layer (SSL) 3.0 security protocol.
                            //Tls: Transport Layer Security (TLS) 1.0 security protocol
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                           SecurityProtocolType.Tls11 |
                                           SecurityProtocolType.Tls12;

                            ctx.ExecuteQuery();

                            FileCreationInformation newFile = new FileCreationInformation();
                            //newFile.Content = System.IO.File.ReadAllBytes(path);
                            newFile.ContentStream = new MemoryStream(System.IO.File.ReadAllBytes(path));
                            newFile.Url = Path.GetFileName(path);
                            newFile.Overwrite = true;

                            List byTitle = ctx.Web.Lists.GetByTitle(ServiceConnection.SupplyChainFolderTitle);
                            Folder folder = byTitle.RootFolder.Folders.GetByUrl(ServiceConnection.RequisitionsFolder);
                            ctx.Load(folder);
                            ctx.ExecuteQuery();

                            Microsoft.SharePoint.Client.File uploadFile = folder.Files.Add(newFile);
                            ctx.Load(byTitle);
                            ctx.Load(uploadFile);
                            ctx.ExecuteQuery();

                            string SharePointUrl = ServiceConnection.sharePointSiteUrl + "/" + ServiceConnection.FinanceFolderTitle + "/" + ServiceConnection.ImprestFolder + "/" + Path.GetFileName(path);

                            dynamicsNAVSOAPServices.documentMgmt.UploadFileToSharePointAndNAV(DocumentNo, DocumentCode, fileName, SharePointUrl);
                        }

                        return Json(new { success = true, message = DocumentDescription + " uploaded successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = DocumentDescription + " was not uploaded. Try Again." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "Please select a pdf file!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }*/
        [Authorize]
        [HttpPost]
        public JsonResult UploadStoreRequisitionAttachments(string DocumentNo, string DocumentCode, string DocumentDescription)
        {
            try
            {
                if (Request.Files.Count <= 0)
                    return Json(new {success = false, message = "Please select a pdf file!"},
                        JsonRequestBehavior.AllowGet);
                const string root = "~/StaffData/";
                var folderpath = Directory.Exists(HttpContext.Server.MapPath(root));

                if (!folderpath)
                {
                    Directory.CreateDirectory(HttpContext.Server.MapPath(root));
                }

                var file = Request.Files[0];
                var fileExt = Path.GetExtension(file.FileName).ToLower();
                var fileName = DocumentNo + "_" + DocumentDescription + fileExt;
                var path = Path.Combine(HttpContext.Server.MapPath(root), fileName);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                if (fileExt != ".pdf" || fileExt != ".eml" || fileExt != ".xlsx" || fileExt != ".csv" ||
                    fileExt != ".rtf" || fileExt != ".doc" || fileExt != ".docx" || fileExt != ".jpg" ||
                    fileExt != ".jpeg" || fileExt != ".png" || fileExt != ".msg")
                    return Json(new {success = false, message = "Unsupported file format"},
                        JsonRequestBehavior.AllowGet);
                file.SaveAs(path);

                if (!System.IO.File.Exists(path))
                    return Json(new {success = false, message = DocumentDescription + " was not uploaded"},
                        JsonRequestBehavior.AllowGet);
                var ret = dynamicsNAVSOAPServices.documentMgmt.InsertImprestAttachment(DocumentNo, path, 51525398, DocumentDescription);
                return Json(ret ? new { success = true, message = DocumentDescription + " uploaded successfully" } : new { success = false, message = DocumentDescription + " was not uploaded" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Document Management 

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateHeader(string No, string Description, string GlobalDimension2Code, string LocationCode, string facilityNo)
        {
            try
            {
                bool ret;
                bool successVal = false;
                string msg = "";

                ret = dynamicsNAVSOAPServices.procurementManagementWS.UpdateHeader(No, Description, LocationCode, GlobalDimension2Code, facilityNo??"");
                if (ret)
                {
                    msg = "Details Updated Successfully";
                    successVal = true;
                }
                else
                {
                    msg = "Details Failed to Update";
                    successVal = true;
                }

                return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Helper Functions
        public JsonResult GetStoreRequisitionAmount(string DocumentNo)
        {
            decimal storeRequisitionAmount = 0;
            storeRequisitionAmount = dynamicsNAVSOAPServices.inventoryManagementWS.GetStoreRequisitionAmount(DocumentNo);
            return Json(new { Amount = storeRequisitionAmount }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAvailableInventory(string ItemNo)
        {
            decimal availableInventory = 0;
           var docno = Session["storereqno"].ToString();
            //var getstore = dynamicsNAVSOAPServices.procurementManagementWS.GetStoreRequisitionLocation(docno);
            var getstore = _dcodataServices.BCOData.HR_Employee.Where(c=>c.No == employeeNo).FirstOrDefault()?.Location_Code;
            availableInventory = dynamicsNAVSOAPServices.procurementManagementWS.GetAvailableInventory(ItemNo, getstore);
            return Json(new { AvailableInventory = availableInventory }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAvailableInventorys(string ItemNo,string docno)
        {
            decimal availableInventory;
            docno = Session["storereqno"].ToString();
            //var getstore = dynamicsNAVSOAPServices.procurementManagementWS.GetStoreRequisitionLocation(docno);
            var getstore = _dcodataServices.BCOData.HR_Employee.Where(c=>c.No == employeeNo).FirstOrDefault()?.Location_Code;
            var df =dynamicsNAVSOAPServices.procurementManagementWS.GetAvailableInventory(ItemNo, "");
            var dfbd = dynamicsNAVSOAPServices.procurementManagementWS.GetAvailableInventory(ItemNo, getstore);
            availableInventory = dynamicsNAVSOAPServices.procurementManagementWS.GetAvailableInventory(ItemNo, getstore);
            var httpResponseDestForeign =
                Credentials.GetOdataData("ItemsList?$filter=No eq '" + ItemNo + "' &$format=json");
            httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();
                var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<Items>>(result1);
                var inventory = StaffAdvanceList?.ListValues?.FirstOrDefault()?.Inventory;
                return Json(new { AvailableInventory = inventory }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { AvailableInventory = availableInventory }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ValidateQuantityRequested(string ItemNo,decimal Quantity)
        {
            decimal? availableInventory = 0;
            //var getstore = dynamicsNAVSOAPServices.procurementManagementWS.GetStoreRequisitionLocation(Session["storereqno"].ToString());
            var getstore = _dcodataServices.BCOData.HR_Employee.Where(c=>c.No == employeeNo).FirstOrDefault()?.Location_Code;
            //availableInventory = dynamicsNAVSOAPServices.inventoryManagementWS.GetAvailableInventory(ItemNo,getstore);
            var httpResponseDestForeign =
                Credentials.GetOdataData("ItemsList?$filter=No eq '" + ItemNo + "' &$format=json");
            httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();
                var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<Items>>(result1);
                availableInventory = StaffAdvanceList?.ListValues?.FirstOrDefault()?.Inventory;
            }

            if (Quantity < 1)
            {
                return Json(new { success = false, message = "The quantity requested cannot be more than the current inventory for item no. " + ItemNo + "." }, JsonRequestBehavior.AllowGet);
            }

            /*if (Quantity > availableInventory)
            {
                return Json(new { success = false, message = "The quantity requested cannot be more than the available inventory" }, JsonRequestBehavior.AllowGet);
            }
            else*/
            {
                return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public string GetStoreRequisitionStatus(string DocumentNo)
        {
            return dynamicsNAVSOAPServices.inventoryManagementWS.GetStoreRequisitionStatus(DocumentNo);
        }
        private void LoadLocationCodes()
        {
             locations = JsonConvert.DeserializeObject<List<LocationCodes>>(dynamicsNAVSOAPServices.procurementManagementWS.GetLocationCodes());
        }
        //public JsonResult LoadItems()
        //{
        //    List<ItemsModel> procplanlist = JsonConvert.DeserializeObject<List<ItemsModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetInventoryItems());

        //    return Json(procplanlist, JsonRequestBehavior.AllowGet);
        //}
        private void LoadItems()
        {
            items = dynamicsNAVODataServices.dynamicsNAVOData.Items.Where(m => m.Inventory > 0 ).ToList();//JsonConvert.DeserializeObject<List<ItemsModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetInventoryItems());
        }

        private void LoadItemUOMs()
        {

            itemUOMs = JsonConvert.DeserializeObject<List<ItemUOMModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetItemUOMs());
        }
        private void LoadDimensions(string GlobalDimension1Code)
        {
            globalDimension1Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                                    where dimensionValuesQuery.Global_Dimension_No.Equals(1) && dimensionValuesQuery.Blocked.Equals(false)
                                    select dimensionValuesQuery;
            globalDimension2Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                                    where dimensionValuesQuery.Global_Dimension_No.Equals(2) && dimensionValuesQuery.Blocked.Equals(false)
                                    select dimensionValuesQuery;
            shortcutDimension3Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                                      where dimensionValuesQuery.Global_Dimension_No.Equals(3) && dimensionValuesQuery.Blocked.Equals(false)
                                      select dimensionValuesQuery;
            shortcutDimension4Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                                      where dimensionValuesQuery.Global_Dimension_No.Equals(4) && dimensionValuesQuery.Blocked.Equals(false)
                                      select dimensionValuesQuery;
            shortcutDimension5Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                                      where dimensionValuesQuery.Global_Dimension_No.Equals(5) && dimensionValuesQuery.Blocked.Equals(false)
                                      select dimensionValuesQuery;
            shortcutDimension6Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                                      where dimensionValuesQuery.Global_Dimension_No.Equals(6) && dimensionValuesQuery.Blocked.Equals(false)
                                      select dimensionValuesQuery;
            shortcutDimension7Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                                      where dimensionValuesQuery.Global_Dimension_No.Equals(7) && dimensionValuesQuery.Blocked.Equals(false)
                                      select dimensionValuesQuery;
            shortcutDimension8Codes = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                                      where dimensionValuesQuery.Global_Dimension_No.Equals(8) && dimensionValuesQuery.Blocked.Equals(false)
                                      select dimensionValuesQuery;
        }
        #endregion Helper Functions
    }
}