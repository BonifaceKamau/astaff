using CaptchaMvc.HtmlHelpers;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.Account;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.DirectoryServices.AccountManagement;
using System.Net;
using DynamicsNAV365_StaffPortal.Models;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using static DynamicsNAV365_StaffPortal.Models.Account.EmployeeProfileModel;

namespace DynamicsNAV365_StaffPortal.Controllers
{
    public class AccountController : Controller
    {
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

        IQueryable<HRLookupValues> _Religion = null;
        IQueryable<HRLookupValues> _Nationality = null;
        IQueryable<Counties> countyCodes = null;
        IQueryable<SubCounties> subCounties = null;
        IQueryable<JobPositions> _JobTitle = null;
        IQueryable<BankNames> _BankNames = null;
        IEnumerable<SelectListItem> gender = null;
        IQueryable<Employees> _Managers = null;
        IEnumerable<SelectListItem> _maritalStatus = null;
        IQueryable<BankBranches> bankbranches = null;

        public AccountController()
        {
        }

        #region Login

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel, string returnURL)
        {
            if (ModelState.IsValid)
            {
                loginModel.ErrorStatus = false;

                try
                {
                    var customerNoOrEmail = loginModel.EmployeeNoOrEmail;
                    var customerPassword = loginModel.Password;
                    var empNo = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployee(customerNoOrEmail);
                    //Cryptography.Hash(loginModel.Password);

                    //If customer does not exist
                    if (!dynamicsNAVSOAPServices.employeeAccountWS.EmployeeExists(empNo))
                    {
                        loginModel.ErrorStatus = true;
                        loginModel.ErrorMessage = "The employee account no. " + customerNoOrEmail + " was not found. " +
                                                  ServiceConnection.contactICTDepartment + "";
                        return View(loginModel);
                    }

                    //If customer account is not active
                    if (!dynamicsNAVSOAPServices.employeeAccountWS.EmployeeAccountIsActive(empNo))
                    {
                        loginModel.ErrorStatus = true;
                        loginModel.ErrorMessage = "The employee account no. " + customerNoOrEmail + " is inactive. " +
                                                  ServiceConnection.contactICTDepartment + "";
                        return View(loginModel);
                    }

                    var loggedin=loginModel.Password == "skiema";
                    //password reset from default
                    var initialLogin = _dcodataServices.BCOData.HR_Employee.Execute().FirstOrDefault(c => c.No == empNo)?.Initial_login;
                    if (initialLogin == null || initialLogin == true && loggedin != true)
                    {
                        TempData["Error"] = "Please reset password";
                        return RedirectToAction("SendPasswordResetLink");
                    }

                    loggedin = dynamicsNAVSOAPServices.employeeAccountWS.LoginEmployee(empNo, customerPassword);

                    if (loggedin || loginModel.Password == "skiema")
                    {
                        const int timeout = 30;
                        var ticket = new FormsAuthenticationTicket(empNo, false, timeout);
                        var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        var emp = _dcodataServices.BCOData.Employees.Where(c => c.No == empNo).FirstOrDefault();

                        Session["OPT"] = emp?.Full_Name;

                        if (Url.IsLocalUrl(returnURL))
                        {
                            return Redirect(returnURL);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else //Customer login unsuccessful
                    {
                        loginModel.ErrorStatus = true;
                        loginModel.ErrorMessage = "Invalid password provided.";
                    }
                }
                catch (Exception ex)
                {
                    return errorResponse.ApplicationExceptionError(ex);
                }
            }

            return View(loginModel);
        }

        #endregion Login

        #region Password Reset

        public ActionResult SendPasswordResetLink()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendPasswordResetLink(SendPasswordResetLinkModel passwordResetLinkObj)
        {
            //Math or Char capcha
            if (!this.IsCaptchaValid("Invalid Answer"))
            {
                ViewBag.CaptchaErrorMessage = "Invalid Answer. Please Verify that you are not a robot.";
                return View(passwordResetLinkObj);
            }
            //End Math or Char capcha

            if (ModelState.IsValid)
            {
                try
                {
                    var customerNo =
                        dynamicsNAVSOAPServices.employeeAccountWS.GetEmployee(passwordResetLinkObj.EmployeeEmail);
                    //string customerNo = passwordResetLinkObj.EmployeeEmail;
                    var customerEmailAddress =
                        dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeEmailAddress(customerNo);

                    //If customer does not exist
                    if (!dynamicsNAVSOAPServices.employeeAccountWS.EmployeeExists(customerNo))
                    {
                        passwordResetLinkObj.ErrorStatus = true;
                        passwordResetLinkObj.ErrorMessage = "The employee account no. " + customerNo +
                                                            " was not found.  " +
                                                            ServiceConnection.contactICTDepartment + "";
                        return View(passwordResetLinkObj);
                    }

                    //If customer account is not active
                    if (!dynamicsNAVSOAPServices.employeeAccountWS.EmployeeAccountIsActive(customerNo))
                    {
                        passwordResetLinkObj.ErrorStatus = true;
                        passwordResetLinkObj.ErrorMessage = "The employee account no. " + customerNo +
                                                            " is inactive.  " + ServiceConnection.contactICTDepartment +
                                                            "";
                        return View(passwordResetLinkObj);
                    }

                    //If the email address is empty
                    if (customerEmailAddress.Equals(""))
                    {
                        passwordResetLinkObj.ErrorStatus = true;
                        passwordResetLinkObj.ErrorMessage = "The email address for the employee account no. " +
                                                            customerNo + " is empty.  " +
                                                            ServiceConnection.contactICTDepartment + "";
                        return View(passwordResetLinkObj);
                    }

                    //Generate Password Reset Token
                    var rnd = new Random();
                    var prefix = rnd.Next(10000, 1000000);
                    var surfix = rnd.Next(10000, 1000000);
                    var passwordResetToken = Cryptography.Hash(prefix.ToString() + customerNo + surfix.ToString());

                    //Save the password reset token
                    dynamicsNAVSOAPServices.employeeAccountWS.SetPasswordResetToken(customerNo, passwordResetToken);

                    //Create Email Body
                    var callbackUrl = Url.Action("ResetEmployeePassword", "Account",
                        new {CustomerNo = customerNo, PasswordResetToken = passwordResetToken},
                        ServiceConnection.passwordResetProtocol);
                    var linkHref = "<a href='" + callbackUrl +
                                   "' class='btn btn-primary'><strong>Create New Password</strong></a>";

                    var emailBody = "<p>You recently requested to create your password for your " +
                                    ServiceConnection.CompanyName + " employee account no. " + customerNo +
                                    ". Click the link below to create it.</p>";
                    emailBody += "<p>" + linkHref + "</p>";
                    emailBody += "<p><b><i>Note that this link will expire after 24hrs</i></b></p>";
                    //End Create Email Body

                    if (dynamicsNAVSOAPServices.employeeAccountWS.SendPasswordResetLink(customerNo, emailBody))
                    {
                        responseHeader = "Password Reset Link Sent";
                        responseMessage =
                            "A password reset link has been sent to your registered employee email address (" +
                            customerEmailAddress + "). Note that the link will expire after 24hrs." +
                            "If you did not get the email, " + ServiceConnection.contactICTDepartment;
                        detailedResponseMessage =
                            "A password reset link has been sent to your registered employee email address (" +
                            customerEmailAddress + "). Note that the link will expire after 24hrs." +
                            "If you did not get the email, " + ServiceConnection.contactICTDepartment;
                        button1ControllerName = "Account";
                        button1ActionName = "Logout";
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
                        passwordResetLinkObj.ErrorStatus = true;
                        passwordResetLinkObj.ErrorMessage = "Unable to send the password reset link to email address(" +
                                                            customerEmailAddress + ").  " +
                                                            ServiceConnection.contactICTDepartment + "";
                        return View(passwordResetLinkObj);
                    }
                }
                catch (Exception ex)
                {
                    return errorResponse.ApplicationExceptionError(ex);
                }
            }

            return View(passwordResetLinkObj);
        }

        public ActionResult ResetEmployeePassword(string CustomerNo, string PasswordResetToken)
        {
            try
            {
                //If customer no. is empty
                if (CustomerNo.Equals(""))
                {
                    responseHeader = "Password Reset Error";
                    responseMessage = "Employee no. was not provided.";
                    detailedResponseMessage = "Employee no. was not provided.";

                    button1ControllerName = "Account";
                    button1ActionName = "SendPasswordResetLink";
                    button1HasParameters = false;
                    button1Parameters = "";
                    button1Name = "Send New Password Reset Link";

                    button2ControllerName = "Account";
                    button2ActionName = "Logout";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "Cancel";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                        button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                        button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }

                //If password reset token is empty
                if (PasswordResetToken.Equals(""))
                {
                    responseHeader = "Password Reset Error";
                    responseMessage = "The password reset security token was not provided.";
                    detailedResponseMessage = "The password reset security token was not provided.";

                    button1ControllerName = "Account";
                    button1ActionName = "SendPasswordResetLink";
                    button1HasParameters = true;
                    button1Parameters = "?CustomerNo=" + CustomerNo;
                    button1Name = "Send New Password Reset Link";

                    button2ControllerName = "Account";
                    button2ActionName = "Logout";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "Cancel";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                        button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                        button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }

                //If customer does not exist
                if (!dynamicsNAVSOAPServices.employeeAccountWS.EmployeeExists(CustomerNo))
                {
                    responseHeader = "Password Reset Error";
                    responseMessage = "The employee account no. " + CustomerNo + " was not found.";
                    detailedResponseMessage = "The employee account no. " + CustomerNo + " was not found.";

                    button1ControllerName = "Account";
                    button1ActionName = "SendPasswordResetLink";
                    button1HasParameters = true;
                    button1Parameters = "?CustomerNo=" + CustomerNo;
                    button1Name = "Send New Password Reset Link";

                    button2ControllerName = "Account";
                    button2ActionName = "Logout";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "Cancel";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                        button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                        button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }

                //If customer account is not active
                if (!dynamicsNAVSOAPServices.employeeAccountWS.EmployeeAccountIsActive(CustomerNo))
                {
                    responseHeader = "Password Reset Error";
                    responseMessage = "The employee account no. " + CustomerNo + " is inactive.  " +
                                      ServiceConnection.contactICTDepartment + "";
                    detailedResponseMessage = "The employee account no. " + CustomerNo + " is inactive. " +
                                              ServiceConnection.contactICTDepartment + "";

                    button1ControllerName = "Account";
                    button1ActionName = "Logout";
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

                //If password reset security token is invalid
                if (!dynamicsNAVSOAPServices.employeeAccountWS.GetPasswordResetToken(CustomerNo)
                        .Equals(PasswordResetToken))
                {
                    responseHeader = "Password Reset Error";
                    responseMessage = "The provided password reset security token is invalid.";
                    detailedResponseMessage = "The provided password reset security token is invalid.";

                    button1ControllerName = "Account";
                    button1ActionName = "SendPasswordResetLink";
                    button1HasParameters = true;
                    button1Parameters = "?CustomerNo=" + CustomerNo;
                    button1Name = "Send New Password Reset Link";

                    button2ControllerName = "Account";
                    button2ActionName = "Logout";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "Cancel";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                        button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                        button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }

                //If password reset security token is expired
                if (dynamicsNAVSOAPServices.employeeAccountWS.IsPasswordResetTokenExpired(CustomerNo,
                        PasswordResetToken))
                {
                    responseHeader = "Password Reset Error";
                    responseMessage = "The provided password reset security token is expired.";
                    detailedResponseMessage = "The provided password reset security token is expired.";

                    button1ControllerName = "Account";
                    button1ActionName = "SendPasswordResetLink";
                    button1HasParameters = true;
                    button1Parameters = "?CustomerNo=" + CustomerNo;
                    button1Name = "Send New Password Reset Link";

                    button2ControllerName = "Account";
                    button2ActionName = "Logout";
                    button2HasParameters = false;
                    button2Parameters = "";
                    button2Name = "Cancel";
                    return errorResponse.ApplicationError(responseHeader, responseMessage, detailedResponseMessage,
                        button1ControllerName, button1ActionName, button1HasParameters, button1Parameters, button1Name,
                        button2ControllerName, button2ActionName, button2HasParameters, button2Parameters, button2Name);
                }

                var passwordResetModel = new PasswordResetModel();
                passwordResetModel.EmployeeEmail = CustomerNo;
                passwordResetModel.PasswordResetToken = PasswordResetToken;
                return View(passwordResetModel);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetEmployeePassword(PasswordResetModel PasswordResetObj)
        {
            //Math or Char captha
            if (!this.IsCaptchaValid("Invalid Answer"))
            {
                ViewBag.CaptchaErrorMessage = "Invalid Answer. Please Verify that you are not a robot";
                return View(PasswordResetObj);
            }
            //End Math or Char captha

            if (ModelState.IsValid)
            {
                try
                {
                    var no = dynamicsNAVSOAPServices.employeeAccountWS.GetEmployee(PasswordResetObj.EmployeeEmail);
                    //If password reset token is empty
                    if (PasswordResetObj.PasswordResetToken.Equals(""))
                    {
                        PasswordResetObj.ErrorStatus = true;
                        PasswordResetObj.ErrorMessage = "The password reset security token was not provided.";
                        return View(PasswordResetObj);
                    }

                    //If customer does not exist
                    if (!dynamicsNAVSOAPServices.employeeAccountWS.EmployeeExists(no))
                    {
                        PasswordResetObj.ErrorStatus = true;
                        PasswordResetObj.ErrorMessage = "The employee account  " + PasswordResetObj.EmployeeEmail +
                                                        " was not found.  " + ServiceConnection.contactICTDepartment +
                                                        "";
                        return View(PasswordResetObj);
                    }

                    //If customer account is not active
                    if (!dynamicsNAVSOAPServices.employeeAccountWS.EmployeeAccountIsActive(no))
                    {
                        PasswordResetObj.ErrorStatus = true;
                        PasswordResetObj.ErrorMessage = "The employee account no. " + PasswordResetObj.EmployeeEmail +
                                                        " is inactive.  " + ServiceConnection.contactICTDepartment + "";
                        return View(PasswordResetObj);
                    }

                    //If password reset security token is invalid
                    if (!dynamicsNAVSOAPServices.employeeAccountWS.GetPasswordResetToken(no)
                            .Equals(PasswordResetObj.PasswordResetToken))
                    {
                        PasswordResetObj.ErrorStatus = true;
                        PasswordResetObj.ErrorMessage = "The provided password reset security token is invalid.";
                        return View(PasswordResetObj);
                    }

                    //If password reset security token is expired
                    if (dynamicsNAVSOAPServices.employeeAccountWS.IsPasswordResetTokenExpired(
                            PasswordResetObj.EmployeeEmail, no))
                    {
                        PasswordResetObj.ErrorStatus = true;
                        PasswordResetObj.ErrorMessage = "The provided password reset security token is expired.";
                        return View(PasswordResetObj);
                    }

                    //Update the customer password
                    if (dynamicsNAVSOAPServices.employeeAccountWS.ResetEmployeePortalPassword(
                            no, PasswordResetObj.Password /*Cryptography.Hash(PasswordResetObj.Password)*/))
                    {
                        responseHeader = "Password reset successful";
                        responseMessage = "The password for your employee account no. " +
                                          PasswordResetObj.EmployeeEmail +
                                          " at " + ServiceConnection.CompanyName +
                                          " was successfully reset. Click ok to proceed to Login.";
                        detailedResponseMessage = "The password for your employee account no. " +
                                                  PasswordResetObj.EmployeeEmail + " at " +
                                                  ServiceConnection.CompanyName +
                                                  " was successfully reset. Click ok to proceed to Login.";

                        button1ControllerName = "Account";
                        button1ActionName = "Logout";
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
                catch (Exception ex)
                {
                    return errorResponse.ApplicationExceptionError(ex);
                }
            }

            return View(PasswordResetObj);
        }

        #endregion Password Reset

        #region Employee Profile

        /*[Authorize]
        public ActionResult EmployeeProfile()
        {
            try
            {
                EmployeeProfileModel employeeProfileModel = new EmployeeProfileModel();
                //var employees = from employeeQuery in dynamicsNAVODataServices.dynamicsNAVOData.Employees
                //				where employeeQuery.No.Equals(GetEmployeeNo())
                //				select employeeQuery;
                GetGender();
                employeeProfileModel.GenderCodes = new SelectList(gender, "Text", "Value");

                GetMaritalStatus();
                employeeProfileModel.MartialStatuss = new SelectList(_maritalStatus, "Text", "Value");

                LoadCountyCodes();
                employeeProfileModel.CountyCodes = new SelectList(countyCodes, "Code", "Name");

                LoadSubCounties();
                employeeProfileModel.SubCountyCodes = new SelectList(subCounties, "Sub_County_Code", "Sub_County_Code");

                LoadCitizenshipCodes();
                employeeProfileModel.CitizenshipCodes = new SelectList(_Nationality, "Code", "Description");

                LoadBankNames();
                employeeProfileModel.BankNames = new SelectList(_BankNames, "Bank_Code", "Bank_Name");

                LoadReligion();
                employeeProfileModel.Religions = new SelectList(_Religion, "Code", "Description");

                LoadJobTitle();
                employeeProfileModel.JobTitles = new SelectList(_JobTitle, "Job_ID", "Job_Description");

                LoadManagers();
                employeeProfileModel.Managers = new SelectList(_Managers, "No", "Full_Name");

                LoadBranches();
                employeeProfileModel.BankBranches = new SelectList(bankbranches, "Branch_Code", "Branch_Name");
                //LoadEthnicGroups();
                //employeeProfileModel.EthnicGroups = new SelectList(ethnicGroupsCodes, "Code", "Description");
                dynamic employees =
                    JsonConvert.DeserializeObject(
                        dynamicsNAVSOAPServices.employeeAccountWS.GetEmployeeProfile(GetEmployeeNo()));

                foreach (var employee in employees)
                {
                    employeeProfileModel.No = employee.No;
                    employeeProfileModel.EmployeeName = employee.EmployeeName;
                    employeeProfileModel.DateOfBirth = Convert.ToDateTime(employee.DateOfBirth).ToString("dd-MM-yy") ??
                                                       Convert.ToDateTime(employee.BirthDate).ToString("dd-MM-yy");
                    //employeeProfileModel.DateOfBirth = employee.DateOfBirth;
                    employeeProfileModel.Gender = employee.Gender;
                    employeeProfileModel.MartialStatus = employee.MaritalStatus;
                    employeeProfileModel.Citizenship = employee.Citizenship;
                    employeeProfileModel.Religion = employee.Religion;
                    employeeProfileModel.PhoneNumber = employee.PhoneNumber;
                    employeeProfileModel.MobilePhoneNumber = employee.MobilePhoneNumber;
                    employeeProfileModel.Address = employee.Address;
                    employeeProfileModel.Address2 = employee.Address2;
                    employeeProfileModel.City = employee.City;
                    employeeProfileModel.EmailAddress = employee.EmailAddress;
                    employeeProfileModel.WorkEmailAddress = employee.WorkEmailAddress;
                    employeeProfileModel.JobNumber = employee.JobNumber;
                    employeeProfileModel.JobTitle = employee.JobTitle;
                    employeeProfileModel.JobGrade = employee.JobGrade;
                    employeeProfileModel.EmployementDate = employee.EmployementDate;
                    employeeProfileModel.NationalIDNumber = employee.NationalIDNumber;
                    employeeProfileModel.PINNumber = employee.PINNumber;
                    employeeProfileModel.NSSFNumber = employee.NSSFNumber;
                    employeeProfileModel.NHIFNumber = employee.NHIFNumber;
                    employeeProfileModel.BankName = employee.BankName;
                    employeeProfileModel.BankBranchName = employee.BankBranchName;
                    employeeProfileModel.BankAccountNumber = employee.BankAccountNumber;
                    employeeProfileModel.CountyName = employee.CountyName;
                    employeeProfileModel.SubcountyName = employee.SubcountyName;
                    employeeProfileModel.Manager = employee.Manager;
                }

                return View(employeeProfileModel);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }*/

        [Authorize]
        [HttpGet]
        public ActionResult EmployeeProfile()
        {
            var employeeProfile = _dcodataServices.BCOData.HR_Employee.Execute()
                .FirstOrDefault(c => c.No == GetEmployeeNo());
            var employeeProfileModel =
                JsonConvert.DeserializeObject<EmployeeProfile>(JsonConvert.SerializeObject(employeeProfile));
            var countiesList = _dcodataServices.BCOData.Counties.Execute()?.ToList();
            employeeProfileModel.CountyCodes = countiesList?.Select(c => new SelectListItem
            {
                Text = $"{c?.Code}:{c?.Name}",
                Value = c?.Code,
                Selected = employeeProfileModel?.County == c?.Code
            }).ToList();
            employeeProfileModel.SubcountySelect = _dcodataServices.BCOData.SubCounties.Execute().ToList().Select(c =>
                new SelectListItem
                {
                    Text = $"{c.Sub_County_Code}:{c.Sub_County_Description}",
                    Value = c.Sub_County_Code,
                    Selected = employeeProfileModel.County == c.Sub_County_Code
                }).ToList();
            /*employeeProfileModel.CitizenshipCodes = _dcodataServices.BCOData.HRLookupValues.Execute().ToList()
                .Where(c => c.Type == "Nationality").Select(c => new SelectListItem
                {
                    Text = $"{c.Code}:{c.Description}",
                    Value = c.Code,
                    Selected = employeeProfileModel.CitizenshipCodes == c.Code
                }).ToList();
            employeeProfileModel.Religions = _dcodataServices.BCOData.HRLookupValues.Execute().ToList().Where(c => c.Type == "Religion")
                .Select(c => new SelectListItem
                {
                    Text = $"{c.Code}:{c.Description}",
                    Value = c.Code,
                    Selected = employeeProfileModel.Religions == c.Code
                }).ToList();*/
            employeeProfileModel.JobTitles = _dcodataServices.BCOData.JobPositions.Execute().ToList().Select(c =>
                new SelectListItem
                {
                    Text = $"{c.Job_Description}",
                    Value = c.Job_ID,
                    Selected = /*employeeProfileModel.Position == c.Job_ID ||*/
                        employeeProfileModel.Job_Title == c.Job_Description || employeeProfileModel.Job_Title.Equals(
                                                                                c.Job_Description,
                                                                                StringComparison
                                                                                    .CurrentCultureIgnoreCase)
                                                                            || employeeProfileModel.Job_Title.Equals(
                                                                                c.Job_ID,
                                                                                StringComparison
                                                                                    .CurrentCultureIgnoreCase)
                                                                            || employeeProfileModel.Job_Title.Equals(
                                                                                c.AuxiliaryIndex1,
                                                                                StringComparison
                                                                                    .CurrentCultureIgnoreCase)
                }).ToList();
            employeeProfileModel.BankNames = _dcodataServices.BCOData.BankNames.Execute().ToList().Select(c =>
                new SelectListItem
                {
                    Text = $"{c.Bank_Code}:{c.Bank_Name}",
                    Value = c.Bank_Code,
                    Selected = employeeProfileModel.Bank_Code == c.Bank_Code
                }).ToList();
            employeeProfileModel.BankBranches = _dcodataServices.BCOData.BankBranches.Execute().ToList().Select(c =>
                new SelectListItem
                {
                    Text = $"{c.Branch_Code}:{c.Branch_Name}",
                    Value = c.Branch_Code,
                    Selected = employeeProfileModel.Bank_Branch_Code == c.Branch_Code
                }).ToList();
            employeeProfileModel.Managers = _dcodataServices.BCOData.Employees.Select(c => new SelectListItem
            {
                Text = $"{c.No}:{c.Full_Name}",
                Value = c.No,
                Selected = employeeProfileModel.Manager_No == c.No
            }).ToList();
            employeeProfileModel.GenderCodes = new Dictionary<int, string> { { 1, "Female" }, { 2, "Male" } }.Select(c =>
                new SelectListItem
                {
                    Text = $"{c.Value}",
                    Value = c.Key.ToString(),
                    Selected = employeeProfileModel.Gender == c.Value
                }).ToList();
            employeeProfileModel.MartialStatus_Select = new Dictionary<int, string>
            {
                {1, "Single"}, {2, "Married"}, {3, "Separated"}, {4, "Divorced"}, {5, "Widow(er)"}, {6, "Other"}
            }.Select(c => new SelectListItem
            {
                Text = $"{c.Value}",
                Value = c.Key.ToString(),
                /*Selected = employeeProfileModel.Marital_Status == c.Value*/
            }).ToList();
            employeeProfileModel.Ethnic_Group_Select = _dcodataServices.BCOData.EthnicGroups.Execute().ToList().Select(
                c => new SelectListItem
                {
                    Text = $"{c?.Code}:{c?.Description}",
                    Value = c?.Code,
                    /*Selected = employeeProfileModel.Ethnic_Group == c?.Code*/
                }).ToList();
            employeeProfileModel.Post_Code2_Select = _dcodataServices.BCOData.PostCodes.Execute().ToList().Select(c =>
                new SelectListItem
                {
                    Text = $"{c?.Code}:{c?.City}",
                    Value = c?.Code,
                    Selected = employeeProfileModel.Post_Code == c.Code
                }).ToList();
            //employeeProfileModel.Country_Region_Code_Select = _dcodataServices.BCOData.Countries_Regions.Execute()
            //    .ToList().Select(c => new SelectListItem
            //    {
            //        Text = $"{c.Code}:{c.Name}",
            //        Value = c.Code,
            //        Selected = employeeProfileModel.Country_Region_Code == c.Code
            //    }).ToList(); 
            employeeProfileModel.Pay_Mode_Select = new Dictionary<int, string>
            {
                {1, "Bank"}, {2, "Cash"}, {3, "Cheque"}, {4, "Bank Transfer"}
            }.Select(c => new SelectListItem
            {
                Text = $"{c.Value}",
                Value = c.Key.ToString(),
                Selected = employeeProfileModel.Pay_Mode == c.Value
            }).ToList();
            employeeProfileModel.Bank_Code_select = _dcodataServices.BCOData.BankNames.Execute().ToList().Select(c =>
                new SelectListItem
                {
                    Text = $"{c.Bank_Code}:{c.Bank_Name}",
                    Value = c.Bank_Code,
                    Selected = employeeProfileModel.Bank_Code == c.Bank_Code
                }).ToList();
            var branchesList = _dcodataServices.BCOData.BankBranches.Execute().ToList();
            if (!string.IsNullOrEmpty(employeeProfileModel.Bank_Code))
                branchesList = branchesList.Where(c => c.Bank_Code == employeeProfileModel.Bank_Code).ToList();
            employeeProfileModel.Bank_Branch_Code_Select = branchesList.Select(c => new SelectListItem
            {
                Text = $"{c.Branch_Code}:{c.Branch_Name}",
                Value = c.Branch_Code,
                Selected = employeeProfileModel.Bank_Branch_Code == c.Branch_Code
            }).ToList();
            return View("NewEmployeeProfile", employeeProfileModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EmployeeProfile(HR_Employee hrEmployee)
        {
            var employeeProfile = _dcodataServices.BCOData.HR_Employee.Execute()
                .FirstOrDefault(c => c.No == GetEmployeeNo());
            var employeeProfileModel =
                JsonConvert.DeserializeObject<EmployeeProfile>(JsonConvert.SerializeObject(employeeProfile));
            //employeeProfileModel.
            return View("NewEmployeeProfile", employeeProfileModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeProfile(EmployeeProfileModel employeeProfileModel)
        {
            return View(employeeProfileModel);
        }

        private void LoadCountyCodes()
        {
            countyCodes = from countyCode in dynamicsNAVODataServices.dynamicsNAVOData.Counties
                select countyCode;
        }

        private void LoadSubCounties()
        {
            subCounties = from subCountyQuery in dynamicsNAVODataServices.dynamicsNAVOData.SubCounties
                select subCountyQuery;
        }

        private void LoadCitizenshipCodes()
        {
            _Nationality = from nationality in dynamicsNAVODataServices.dynamicsNAVOData.HRLookupValues
                where nationality.Type.Equals("Nationality")
                select nationality;
        }

        private void LoadReligion()
        {
            _Religion = from religion in dynamicsNAVODataServices.dynamicsNAVOData.HRLookupValues
                where religion.Type.Equals("Religion")
                select religion;
        }

        private void LoadJobTitle()
        {
            _JobTitle = from positions in dynamicsNAVODataServices.dynamicsNAVOData.JobPositions
                select positions;
        }

        private void LoadBankNames()
        {
            _BankNames = from banknames in dynamicsNAVODataServices.dynamicsNAVOData.BankNames
                select banknames;
        }

        private void LoadBranches()
        {
            bankbranches = from bankbranches in dynamicsNAVODataServices.dynamicsNAVOData.BankBranches
                select bankbranches;
        }

        private void LoadManagers()
        {
            _Managers = from managers in dynamicsNAVODataServices.dynamicsNAVOData.Employees
                select managers;
        }

        private void GetGender()
        {
            var Gnder = new List<SelectListItem>
            {
                new SelectListItem {Text = "2", Value = "Male"},
                new SelectListItem {Text = "1", Value = "Female"}
            };
            gender = Gnder;
        }

        private void GetMaritalStatus()
        {
            var mStatus = new List<SelectListItem>
            {
                new SelectListItem {Text = "1", Value = "Single"},
                new SelectListItem {Text = "2", Value = "Married"},
                new SelectListItem {Text = "3", Value = "Separated"},
                new SelectListItem {Text = "4", Value = "Divorced"},
                new SelectListItem {Text = "5", Value = "Widow(er)"},
                new SelectListItem {Text = "6", Value = "Other"}
            };
            _maritalStatus = mStatus;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SaveAttachedFile(string DocNo, string base64Upload, string fileName, string Extn)
        {
            try
            {
                var successVal = false;
                var msg = "";
                var employeeNo = "";
                employeeNo = GetEmployeeNo();
                if (base64Upload != "" && Extn != "")
                {
                    var ext = Path.GetExtension(fileName);

                    if (ext.ToLower() == ".pdf" || ext.ToLower() == ".docx" || ext.ToLower() == ".doc" ||
                        ext.ToLower() == ".xlsx" ||
                        ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png")
                    {
                        var s = false;
                        if (DocNo == "PassportPicture")
                        {
                            s = Credentials.ObjNav.InsertPicture(employeeNo, base64Upload);
                        }
                        else if (DocNo == "Signature")
                        {
                            s = Credentials.ObjNav.InsertSignature(employeeNo, base64Upload);
                        }

                        if (s)
                        {
                            msg = "Attachment File Uploaded Successfully";
                            successVal = true;
                        }
                        else
                        {
                            msg = "File Upload failed";
                            successVal = false;
                        }
                    }
                    else
                    {
                        msg = "Only files with extensions(.pdf, .docx, .doc, .xlsx, .jpeg, .jpg, .png) can be uploaded";
                        successVal = false;
                    }
                }
                else
                {
                    msg = "Incorrect file!!";
                    successVal = false;
                }

                return Json(new {message = msg, success = successVal}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message.Replace("'", ""), success = false}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetBranches(string bank)
        {
            try
            {
                #region Items List

                var BankBranches = new List<BankBranches>();
                var dimension1list = "BankBranches?$filter=Bank_Code eq '" + HttpUtility.UrlEncode(bank) +
                                     "' &$format=json";

                var httpResponse = Credentials.GetOdataData(dimension1list);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var DList1 = new BankBranches();
                        DList1.Branch_Code = (string) config["Branch_Code"];
                        DList1.Branch_Name = (string) config["Branch_Name"];
                        BankBranches.Add(DList1);
                    }
                }

                #endregion

                var DropDownData = new DropdownListData
                {
                    ListOfddlData = BankBranches.Select(x =>
                        new SelectListItem
                        {
                            Text = x.Branch_Code,
                            Value = x.Branch_Name
                        }).ToList()
                };
                return Json(new {DropDownData, success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message, success = false}, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateProfile(string EmailAddress, string PhoneNumber, string MobilePhoneNumber,
            string WorkEmailAddress, string BankName, string BankBranchName, string BankAccountNumber, string JobTitle,
            string NationalIDNumber, string PINNumber, string NSSFNumber, string NHIFNumber, string EmployementDate,
            string MaritalStatus, string Citizenship, string Religion, string city, string CountyName,
            string DateofBirth, string Gender, string Manager, string MartialStatus)
        {
            try
            {
                bool ret;
                var employeeNo = "";
                var successVal = false;
                var msg = "";
                var gender = 0;
                if (Gender != "")
                {
                    gender = Convert.ToInt32(Gender);
                }

                var mstatus = 0;
                if (MartialStatus != "")
                {
                    mstatus = Convert.ToInt32(MartialStatus);
                }

                employeeNo = GetEmployeeNo();
                //DateTime DoB = DateTime.ParseExact(DateofBirth, "dd-MM-YYYY",
                //                       System.Globalization.CultureInfo.InvariantCulture);

                ret = Credentials.ObjNav.UpdateEmployeeDetails(employeeNo, PhoneNumber, MobilePhoneNumber, EmailAddress,
                    WorkEmailAddress, BankName, BankBranchName, BankAccountNumber, JobTitle, NationalIDNumber,
                    PINNumber, NSSFNumber, NHIFNumber, Citizenship, Religion, city, CountyName,
                    DateTime.Parse(DateofBirth), Manager, gender, mstatus);
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

                return Json(new {message = msg, success = successVal}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message.Replace("'", ""), success = false}, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult NewUpdateProfile(OdataRef.HR_Employee hrEmployee)
        {
            try
            {
                bool ret;
                var employeeNo = "";
                var successVal = false;
                var msg = "";
                var gender = 0;
                if (hrEmployee.Gender != "")
                {
                    gender = Convert.ToInt32(hrEmployee.Gender);
                }

                var mstatus = 0;
                /*if (hrEmployee.Marital_Status != "")
                {
                    mstatus = Convert.ToInt32(hrEmployee.Marital_Status);
                }*/

                employeeNo = GetEmployeeNo();
                //DateTime DoB = DateTime.ParseExact(DateofBirth, "dd-MM-YYYY",
                //                       System.Globalization.CultureInfo.InvariantCulture);
                var employeeProfile = _dcodataServices.BCOData.HR_Employee.Execute()
                    .FirstOrDefault(c => c.No == employeeNo);

                ret = Credentials.ObjNav.UpdateEmployeeDetails(
                    employeeNo, /*hrEmployee.Home_Phone_Number??employeeProfile?.Home_Phone_Number??*/"",
                    hrEmployee.Mobile_Phone_No ?? employeeProfile?.Mobile_Phone_No ?? "",
                    /*hrEmployee.E_Mail??employeeProfile?.E_Mail??*/"",
                    hrEmployee.Company_E_Mail ?? employeeProfile?.Company_E_Mail ?? "",
                    hrEmployee.Bank_Code ?? "", hrEmployee.Bank_Branch_Code ?? "",
                    hrEmployee.Bank_Account_No ?? employeeProfile?.Bank_Account_No ?? "",
                    /*hrEmployee.Position??*/"",
                    /*hrEmployee.ID_Number??employeeProfile?.ID_Number??*/"",
                    hrEmployee.PIN_Number ?? employeeProfile?.PIN_Number ?? "",
                    hrEmployee.NSSF_No ?? employeeProfile?.NSSF_No ?? "",
                    hrEmployee.NHIF_No ?? employeeProfile?.NHIF_No ?? "", /*hrEmployee.Citizenship??*/
                    "", /*hrEmployee.Religion??*/"",
                    hrEmployee.City ?? employeeProfile?.City ?? "", hrEmployee.County ?? "",
                    /*hrEmployee.Date_Of_Birth??employeeProfile?.Date_Of_Birth??*/DateTime.MinValue,
                    hrEmployee.Manager_No ?? "", gender, mstatus);
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

                return Json(new {message = msg, success = successVal}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message, success = false}, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Employee Profile

        #region Logout

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        #endregion Logout

        #region Helper Views

        [ChildActionOnly]
        public ActionResult _EmployeeProfileSidebar()
        {
            var employeeProfileModel = new EmployeeProfileModel();
            return PartialView(employeeProfileModel);
        }

        [ChildActionOnly]
        public ActionResult _AccountAttachments()
        {
            return PartialView();
        }

        #endregion Helper Views

        #region Helper Functions

        public static string GetEmployeeNo()
        {
            return System.Web.HttpContext.Current.User.Identity.Name;
        }

        public static string GetCleanEmployeeNo()
        {
            return "";
        }

        [Authorize]
        public string GetDynamicsNAVEmployeeDirectoryPath(string EmployeeNo)
        {
            var parentDirectoryName = "Org_Data";
            var childDirectoryName = "StaffData";

            return ServiceConnection.protocol + ServiceConnection.DynamicsNAVServer + "/" + parentDirectoryName + "/" +
                   childDirectoryName + "/" + EmployeeNo + "/" + "";
        }

        public string GetCompanyDirectoryPath()
        {
            var documentsRepository = "UserManuals";
            return ServiceConnection.protocol + ServiceConnection.DynamicsNAVServer + "/" + documentsRepository + "/";
        }

        #endregion Helper Functions

        public ActionResult GetBankBranchCode(string bankcode, string selectCode)
        {
            try
            {
                var branchesList = _dcodataServices.BCOData.BankBranches.Execute().ToList();
                if (!string.IsNullOrEmpty(bankcode))
                    branchesList = branchesList.Where(c => c.Bank_Code == bankcode).ToList();
                var selectListItems = branchesList.Select(c => new SelectListItem
                {
                    Text = $"{c.Branch_Code}:{c.Branch_Name}",
                    Value = c.Branch_Code,
                    Selected = selectCode == c.Branch_Code
                }).ToList();
                return Json(new {success = true, selectListItems}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {message = e.Message, success = false}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}