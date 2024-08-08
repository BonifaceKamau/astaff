using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.Account;
using DynamicsNAV365_StaffPortal.Models.DocumentMgmt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.DocumentRepositoryServices
{
    public class DocumentsController : Controller
    {
        string companyName = ServiceConnection.CompanyName;
        static string companyURL = "";

        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

        SuccessResponseController successResponse = new SuccessResponseController();
        InfoResponseController infoResponse = new InfoResponseController();
        ErrorResponseController errorResponse = new ErrorResponseController();

        string employeeNo = "";

        AccountController accountController = new AccountController();
        public DocumentsController()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        #region Documents Repository
      
        [Authorize]
        public PartialViewResult _PreviewDocument()
        {
            return PartialView();
        }

        [Authorize]
        public ActionResult ViewDocuments()
        {
            List<UserManualModel> documentTypeList= new List<UserManualModel>();
          
            dynamic documentTypes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetERPPUserManuals());
            foreach (var documentType in documentTypes)
            {
                UserManualModel userManualsObj = new UserManualModel();
                userManualsObj.Code = documentType.Code;
                userManualsObj.Name = documentType.Name;

                documentTypeList.Add(userManualsObj);
            }

            DocumentRepositoryModel documentRepository = new DocumentRepositoryModel();
            documentRepository.EmployeeNo = employeeNo;
            documentRepository.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

            documentRepository.DocumentTypes = new SelectList(documentTypeList, "Code", "Name");

            return View(documentRepository);
        }

       [Authorize]
        [HttpPost]
        public async Task<ActionResult> ViewDocuments(DocumentRepositoryModel documentRepository)
        {
            try
            {
                List<UserManualModel> documentTypeList = new List<UserManualModel>();

                dynamic documentTypes = JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.documentMgmt.GetERPPUserManuals());
                foreach (var documentType in documentTypes)
                {
                    UserManualModel userManualsObj = new UserManualModel();
                    userManualsObj.Code = documentType.Code;
                    userManualsObj.Name = documentType.Name;

                    documentTypeList.Add(userManualsObj);
                }

                documentRepository.EmployeeNo = employeeNo;
                documentRepository.EmployeeName = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeName(employeeNo);

                documentRepository.DocumentTypes = new SelectList(documentTypeList, "Code", "Name");

                if (ModelState.IsValid)
                {
                    switch (documentRepository.DocumentType)
                    {
                        case "PORTAL MANUAL":
                            using (WebClient wc = new WebClient())
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(accountController.GetCompanyDirectoryPath() + documentRepository.DocumentType + ".pdf");
                                return File(byteArr, "application/pdf");
                            }

                        case "FINANCE MANUAL":
                            using (WebClient wc = new WebClient())
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(accountController.GetCompanyDirectoryPath() + documentRepository.DocumentType + ".pdf");
                                return File(byteArr, "application/pdf");
                            }

                        case "FUNDS MANUAL":
                            using (WebClient wc = new WebClient())
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(accountController.GetCompanyDirectoryPath() + documentRepository.DocumentType + ".pdf");
                                return File(byteArr, "application/pdf");
                            }

                        case "PROCUREMENT MANUAL":
                            using (WebClient wc = new WebClient())
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(accountController.GetCompanyDirectoryPath() + documentRepository.DocumentType + ".pdf");
                                return File(byteArr, "application/pdf");
                            }

                        case "PROPERTY MANUAL":
                            using (WebClient wc = new WebClient())
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(accountController.GetCompanyDirectoryPath() + documentRepository.DocumentType + ".pdf");
                                return File(byteArr, "application/pdf");
                            }

                        case "CRM MANUAL":
                            using (WebClient wc = new WebClient())
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(accountController.GetCompanyDirectoryPath() + documentRepository.DocumentType + ".pdf");
                                return File(byteArr, "application/pdf");
                            }

                        case "PROJECT MANUAL":
                            using (WebClient wc = new WebClient())
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(accountController.GetCompanyDirectoryPath() + documentRepository.DocumentType + ".pdf");
                                return File(byteArr, "application/pdf");
                            }

                        case "STORES MANUAL":
                            using (WebClient wc = new WebClient())
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(accountController.GetCompanyDirectoryPath() + documentRepository.DocumentType + ".pdf");
                                return File(byteArr, "application/pdf");
                            }

                        case "HRM MANUAL":
                            using (WebClient wc = new WebClient())
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(accountController.GetCompanyDirectoryPath() + documentRepository.DocumentType + ".pdf");
                                return File(byteArr, "application/pdf");
                            }

                        case "OTHER":
                            using (WebClient wc = new WebClient())
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(accountController.GetCompanyDirectoryPath() + "Staff Portal Manual.pdf");
                                return File(byteArr, "application/pdf");
                            }

                        default:
                            using (WebClient wc = new WebClient())
                            {
                                var byteArr = await wc.DownloadDataTaskAsync(accountController.GetCompanyDirectoryPath() + "Staff Portal Manual.pdf");
                                return File(byteArr, "application/pdf");
                            }
                    }
                }
                else
                {
                    return View(documentRepository);
                }
            }

            catch (Exception ex)
            {
                documentRepository.ErrorStatus = true;
                documentRepository.ErrorMessage = "" + documentRepository.DocumentType + " document is missing in the directory. Contact ICT team to create the document in the shared folder.";
                return View(documentRepository);
            }
        }
        #endregion Documents Repository

        #region Helper Views
        [ChildActionOnly]
        public ActionResult _DocumentSidebar()
        {
            EmployeeProfileModel employeeProfileObj = new EmployeeProfileModel();
            employeeProfileObj.PassportAttached = false;
            return PartialView(employeeProfileObj);
        }

        [Authorize]
        public ActionResult DocumentInfo()
        {
            return View();
        }
        #endregion Helper Views
    }
}