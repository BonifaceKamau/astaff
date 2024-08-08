using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.Account;
using DynamicsNAV365_StaffPortal.Models.Approvals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.ApprovalServices
{
    public class ApprovalController : Controller
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

		public ApprovalController()
		{
			employeeNo = AccountController.GetEmployeeNo();
		}

        [Authorize]
        public ActionResult Approvals(string DocumentNo)
        {
            try
            {
                List<ApprovalEntryModel> approvalEntriesList = new List<ApprovalEntryModel>();

                dynamic approvalEntries = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.ApprovalsMgmt.GetApprovers("", DocumentNo));

                foreach (var approvalEntry in approvalEntries)
                {
                    ApprovalEntryModel approvalEntryObj = new ApprovalEntryModel();
                    approvalEntryObj.EntryNo = approvalEntry.EntryNo;
                    approvalEntryObj.TableID = approvalEntry.TableID;
                    approvalEntryObj.DocumentType = approvalEntry.DocumentType;
                    approvalEntryObj.DocumentNo = approvalEntry.DocumentNo;
                    approvalEntryObj.Description = approvalEntry.Description;
                    approvalEntryObj.SequenceNo = approvalEntry.SequenceNo;
                    approvalEntryObj.ApprovalCode = approvalEntry.ApprovalCode;
                    approvalEntryObj.SenderID = approvalEntry.SenderID;
                    approvalEntryObj.ApproverID = approvalEntry.ApproverID;
                    approvalEntryObj.Status = approvalEntry.Status;
                    approvalEntryObj.DateTimeSentforApproval = approvalEntry.DateTimeSentforApproval;
                    approvalEntryObj.Amount = approvalEntry.Amount;
                    approvalEntryObj.CurrencyCode = approvalEntry.CurrencyCode;
                    approvalEntryObj.ApproverEmployeeNo = approvalEntry.ApproverEmployeeNo;
                    approvalEntryObj.ApproverEmployeeName = approvalEntry.ApproverEmployeeName;
                    approvalEntryObj.SenderEmployeeNo = approvalEntry.SenderEmployeeNo;
                    approvalEntryObj.SenderEmployeeName = approvalEntry.SenderEmployeeName;
                    approvalEntriesList.Add(approvalEntryObj);
                }
                return View(approvalEntriesList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [Authorize]
        public ActionResult OpenEntries()
        {
            try
            {
                List<ApprovalEntryModel> approvalEntriesList = new List<ApprovalEntryModel>();

                var useremployee = _bcodataServices.BCOData.UserSetupQuery.Execute().ToList();
                var employeeuser = useremployee.Where(c => c.Employee_No == employeeNo).FirstOrDefault()?.User_ID;

                //if (employeeuser != null)
                //{
                //    approvalEntryObj.ErrorStatus = true;
                //    approvalEntryObj.ErrorMessage =
                //        "Kindly provide reason (s) for declining/rejecting this document.";
                //}

                dynamic approvalEntries = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.ApprovalsMgmt.GetOpenApprovalEntries(employeeuser, ""));

                foreach (var approvalEntry in approvalEntries)
                {
                    ApprovalEntryModel approvalEntryObj = new ApprovalEntryModel();
                    approvalEntryObj.EntryNo = approvalEntry.EntryNo;
                    approvalEntryObj.TableID = approvalEntry.TableID;
                    approvalEntryObj.DocumentType = approvalEntry.DocumentType;
                    approvalEntryObj.DocumentNo = approvalEntry.DocumentNo;
                    approvalEntryObj.Description = approvalEntry.Description;
                    approvalEntryObj.SequenceNo = approvalEntry.SequenceNo;
                    approvalEntryObj.ApprovalCode = approvalEntry.ApprovalCode;
                    approvalEntryObj.SenderID = approvalEntry.SenderID;
                    approvalEntryObj.ApproverID = approvalEntry.ApproverID;
                    approvalEntryObj.Status = approvalEntry.Status;
                    approvalEntryObj.DateTimeSentforApproval = approvalEntry.DateTimeSentforApproval;
                    approvalEntryObj.Amount = approvalEntry.Amount;
                    approvalEntryObj.CurrencyCode = approvalEntry.CurrencyCode;
                    approvalEntryObj.ApproverEmployeeNo = approvalEntry.ApproverEmployeeNo;
                    approvalEntryObj.ApproverEmployeeName = approvalEntry.ApproverEmployeeName;
                    approvalEntryObj.SenderEmployeeNo = approvalEntry.SenderEmployeeNo;
                    approvalEntryObj.SenderEmployeeName = approvalEntry.SenderEmployeeName;
                    approvalEntryObj.ImprestType = approvalEntry.ImprestType;
                    approvalEntriesList.Add(approvalEntryObj);
                }

                return View(approvalEntriesList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        } 

        [Authorize]
        public ActionResult RejectedEntries() 
        {
            try
            {
                List<ApprovalEntryModel> approvalEntriesList = new List<ApprovalEntryModel>();

                dynamic approvalEntries = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.ApprovalsMgmt.GetRejectedApprovalEntries(employeeNo, ""));

                foreach (var approvalEntry in approvalEntries)
                {
                    ApprovalEntryModel approvalEntryObj = new ApprovalEntryModel();
                    approvalEntryObj.EntryNo = approvalEntry.EntryNo;
                    approvalEntryObj.TableID = approvalEntry.TableID;
                    approvalEntryObj.DocumentType = approvalEntry.DocumentType;
                    approvalEntryObj.DocumentNo = approvalEntry.DocumentNo;
                    approvalEntryObj.Description = approvalEntry.Description;
                    approvalEntryObj.SequenceNo = approvalEntry.SequenceNo;
                    approvalEntryObj.ApprovalCode = approvalEntry.ApprovalCode;
                    approvalEntryObj.SenderID = approvalEntry.SenderID;
                    approvalEntryObj.ApproverID = approvalEntry.ApproverID;
                    approvalEntryObj.Status = approvalEntry.Status;
                    approvalEntryObj.DateTimeSentforApproval = approvalEntry.DateTimeSentforApproval;
                    approvalEntryObj.Amount = approvalEntry.Amount;
                    approvalEntryObj.CurrencyCode = approvalEntry.CurrencyCode;
                    approvalEntryObj.ApproverEmployeeNo = approvalEntry.ApproverEmployeeNo;
                    approvalEntryObj.ApproverEmployeeName = approvalEntry.ApproverEmployeeName;
                    approvalEntryObj.SenderEmployeeNo = approvalEntry.SenderEmployeeNo;
                    approvalEntryObj.SenderEmployeeName = approvalEntry.SenderEmployeeName;
                    approvalEntriesList.Add(approvalEntryObj);
                }

                return View(approvalEntriesList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [Authorize]
        public ActionResult ApprovedEntries() 
        {
            try
            {
                List<ApprovalEntryModel> approvalEntriesList = new List<ApprovalEntryModel>();

                dynamic approvalEntries = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.ApprovalsMgmt.GetApprovedApprovalEntries(employeeNo, ""));

                foreach (var approvalEntry in approvalEntries)
                {
                    ApprovalEntryModel approvalEntryObj = new ApprovalEntryModel();
                    approvalEntryObj.EntryNo = approvalEntry.EntryNo;
                    approvalEntryObj.TableID = approvalEntry.TableID;
                    approvalEntryObj.DocumentType = approvalEntry.DocumentType;
                    approvalEntryObj.DocumentNo = approvalEntry.DocumentNo;
                    approvalEntryObj.Description = approvalEntry.Description;
                    approvalEntryObj.SequenceNo = approvalEntry.SequenceNo;
                    approvalEntryObj.ApprovalCode = approvalEntry.ApprovalCode;
                    approvalEntryObj.SenderID = approvalEntry.SenderID;
                    approvalEntryObj.ApproverID = approvalEntry.ApproverID;
                    approvalEntryObj.Status = approvalEntry.Status;
                    approvalEntryObj.DateTimeSentforApproval = approvalEntry.DateTimeSentforApproval;
                    approvalEntryObj.Amount = approvalEntry.Amount;
                    approvalEntryObj.CurrencyCode = approvalEntry.CurrencyCode;
                    approvalEntryObj.ApproverEmployeeNo = approvalEntry.ApproverEmployeeNo;
                    approvalEntryObj.ApproverEmployeeName = approvalEntry.ApproverEmployeeName;
                    approvalEntryObj.SenderEmployeeNo = approvalEntry.SenderEmployeeNo;
                    approvalEntryObj.SenderEmployeeName = approvalEntry.SenderEmployeeName;
                    approvalEntriesList.Add(approvalEntryObj);
                }

                return View(approvalEntriesList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        #region Helper Views
        [ChildActionOnly]
		public ActionResult _ApprovalSidebar()
		{
			EmployeeProfileModel employeeProfileModel = new EmployeeProfileModel();
			employeeProfileModel.PassportAttached = false;
			return PartialView(employeeProfileModel);
		}
		#endregion Helper Views
	}
}