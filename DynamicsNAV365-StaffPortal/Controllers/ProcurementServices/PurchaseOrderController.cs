using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.Procurement.Purchases;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.ProcurementServices
{
    public class PurchaseOrderController : Controller
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

        public PurchaseOrderController() 
        {
			employeeNo = AccountController.GetEmployeeNo();

		}

		#region Purchase Order Lines

		[ChildActionOnly]
		[Authorize]
		public ActionResult _ViewPurchaseOrderLine(string RFXNo)
		{
			return PartialView(new PurchaseLineModel());
		}

		[Authorize]
		public JsonResult GetPurchaseOrderLines(string DocumentNo) 
		{
			return Json(JsonConvert.DeserializeObject<List<PurchaseLineModel>>(dynamicsNAVSOAPServices.procurementManagementWS.GetPOLines(DocumentNo)), JsonRequestBehavior.AllowGet);
		}

		#endregion End Purchase Order Lines

		#region Purchase Order Approval

		[Authorize]
		public ActionResult PurchaseOrderApproval(string PurchaseOrderNo)
		{
			try
			{
				if (PurchaseOrderNo.Equals(""))
				{
					return RedirectToAction("OpenEntries", "Approval");
				}

				return View(JsonConvert.DeserializeObject<PurchaseHeaderModel>(dynamicsNAVSOAPServices.procurementManagementWS.GetPOByNo(PurchaseOrderNo)));

			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
			}
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> PurchaseOrderApproval(PurchaseHeaderModel PurchaseRequisitionObj, string Command) 
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
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.ApprovePurchaseOrder(employeeNo, PurchaseRequisitionObj.No, AccountController.GetEmployeeNo()))
					{
						responseHeader = "Success";
						responseMessage = "Purchase Order no." + PurchaseRequisitionObj.No + " approved successfully.";
						detailedResponseMessage = "Purchase Order no." + PurchaseRequisitionObj.No + " approved successfully.";
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
						PurchaseRequisitionObj=JsonConvert.DeserializeObject<PurchaseHeaderModel>(dynamicsNAVSOAPServices.procurementManagementWS.GetPOByNo(PurchaseRequisitionObj.No));
						PurchaseRequisitionObj.ErrorStatus = true;
						PurchaseRequisitionObj.ErrorMessage = "Indicate the reason for declining this document.";
						return View(PurchaseRequisitionObj);
					}
					if (dynamicsNAVSOAPServices.ApprovalsMgmt.RejectPurchaseOrder(employeeNo, PurchaseRequisitionObj.No, PurchaseRequisitionObj.Comments))
					{
						responseHeader = "Success";
						responseMessage = "Purchase Order no." + PurchaseRequisitionObj.No + " rejected.";
						detailedResponseMessage = "Purchase Order no." + PurchaseRequisitionObj.No + " rejected.";
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
					//string fileURL = dynamicsNAVSOAPServices.documentMgmt.GenerateSupportingDocumentLink(PurchaseRequisitionObj.No);

					//string ext = Path.GetExtension(fileURL); // getting the file extension of uploaded file  

					//if (!fileURL.Equals(""))
					//{
					//	using (WebClient wc = new WebClient())
					//	{
					//		if (ext.Equals(".pdf"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/pdf");
					//		}

					//		else if (ext.Equals(".doc"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/msword");
					//		}

					//		else if (ext.Equals(".docx"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
					//		}

					//		else if ((ext.Equals(".jpeg")) || (ext.Equals(".jpg")))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "image/jpeg");
					//		}

					//		else if (ext.Equals(".json"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/json");
					//		}

					//		else if (ext.Equals(".ppt"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.ms-powerpoint");
					//		}

					//		else if (ext.Equals(".png"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "image/png");
					//		}

					//		else if (ext.Equals(".pptx"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
					//		}

					//		else if (ext.Equals(".rar"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.rar");
					//		}

					//		else if (ext.Equals(".xls"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.ms-excel");
					//		}

					//		else if (ext.Equals(".xlsx"))
					//		{
					//			var byteArr = await wc.DownloadDataTaskAsync(fileURL);
					//			return File(byteArr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
					//		}

						//	else
						//	{
						//		PurchaseRequisitionObj.ErrorStatus = true;
						//		PurchaseRequisitionObj.ErrorMessage = "No file attached. This is because purchase requisition's document attachment is not mandatory. ";
						//		return View(PurchaseRequisitionObj);
						//	}
						//}
					//}


					//else
					//{
					//	PurchaseRequisitionObj.ErrorStatus = true;
					//	PurchaseRequisitionObj.ErrorMessage = "There was not file attached for this documnt No - " + PurchaseRequisitionObj.No + " because attachments for purchase requisitions are optional.";
					//	return View(PurchaseRequisitionObj);
					//}
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

		#endregion Purchase Order Approval
	}
}