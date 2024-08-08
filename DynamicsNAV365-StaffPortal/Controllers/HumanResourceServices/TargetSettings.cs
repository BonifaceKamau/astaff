using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models;
using DynamicsNAV365_StaffPortal.Models.Account;
using DynamicsNAV365_StaffPortal.Models.Finance.StaffAdvance;
using DynamicsNAV365_StaffPortal.Models.HumanResource;
//using Microsoft.Graph;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using OdataRef;
using OdataRefV4.NAV;
using CorporateWorkplan = DynamicsNAV365_StaffPortal.Models.HumanResource.CorporateWorkplan;
using DirectorateWorkplanHeader = DynamicsNAV365_StaffPortal.Models.HumanResource.DirectorateWorkplanHeader;
using HR_Appraisal_Periods = OdataRef.HR_Appraisal_Periods;
using MidYear_Appraisal_Lines = OdataRef.MidYear_Appraisal_Lines;
using MidYearAppraisal = DynamicsNAV365_StaffPortal.Models.MidYearAppraisal;
using Peer_Appraisal_Header = OdataRef.Peer_Appraisal_Header;
using Staff_Target_Lines = OdataRef.Staff_Target_Lines;
using StaffTargetObjectives = DynamicsNAV365_StaffPortal.Models.HumanResource.StaffTargetObjectives;

namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    [Authorize]
    public class TargetSettingsController : Controller
    {
        static string companyName = ServiceConnection.CompanyName;
        static string companyURL = "";

        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        BCODATAServices _dcodataServices = new BCODATAServices(companyURL);
        BCODATAV4Services _bcodatav4Services = new BCODATAV4Services(companyURL);

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

        string employeeNo = "";

        public List<StaffTargetObjectives> StaffTargetObjectives(string empNo, string No)
        {
            var filterQuery = string.Empty;
            if (!string.IsNullOrEmpty(empNo))
            {
                filterQuery += $"Staff_No eq '{empNo}'";
            }

            if (!string.IsNullOrEmpty(No))
            {
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    filterQuery += " and ";
                }

                filterQuery += $"No eq '{No}'";
            }

            var url = $"StaffTargetObjectives?$filter={filterQuery}&$format=json";
            var httpResponseDestForeign = Credentials.GetOdataData(url);
            httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();
                var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<StaffTargetObjectives>>(result1);
                return StaffAdvanceList?.ListValues;
            }
        }

        public List<WorkplanHeaderModel> TargetSettings(string empNo, string No, bool? approved = false)
        {
            var filterQuery = string.Empty;
            if (!string.IsNullOrEmpty(empNo))
            {
                filterQuery += $"Staff_No eq '{empNo}'";
            }

            if (!string.IsNullOrEmpty(No))
            {
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    filterQuery += " and ";
                }

                filterQuery += $"No eq '{No}'";
            }

            if (approved != null && approved == true)
            {
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    filterQuery += " and ";
                }

                filterQuery += $"Status eq 'Released'";
            }

            var url = string.IsNullOrEmpty(filterQuery)
                ? "StaffTargetObjectives?$format=json"
                : $"StaffTargetObjectives?$filter={filterQuery}&$format=json";
            var httpResponseDestForeign = Credentials.GetOdataData(url);
            httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();
                var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<WorkplanHeaderModel>>(result1);
                return StaffAdvanceList?.ListValues;
            }
        }

        public List<CorporateWorkplan> CorporateWorkplan(string empNo, string No)
        {
            var filterQuery = string.Empty;
            if (!string.IsNullOrEmpty(empNo))
            {
                filterQuery += $"Staff_No eq '{empNo}'";
            }

            if (!string.IsNullOrEmpty(No))
            {
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    filterQuery += " and ";
                }

                filterQuery += $"No eq '{No}'";
            }

            var url = string.IsNullOrEmpty(filterQuery)
                ? "CorporateWorkplan?$format=json"
                : $"CorporateWorkplan?$filter={filterQuery}&$format=json";
            var httpResponseDestForeign = Credentials.GetOdataData(url);
            httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();
                var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<CorporateWorkplan>>(result1);
                return StaffAdvanceList?.ListValues;
            }
        }

        public List<WorkplanLine> TargetSettingLines(string No)
        {
            var filterQuery = string.Empty;
            if (!string.IsNullOrEmpty(No))
            {
                filterQuery += $"No eq '{No}'";
            }

            var url = string.IsNullOrEmpty(filterQuery)
                ? "Staff_Target_Lines?$format=json"
                : $"Staff_Target_Lines?$filter={filterQuery}&$format=json";
            var httpResponseDestForeign = Credentials.GetOdataData(url);
            httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();
                var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<WorkplanLine>>(result1);
                return StaffAdvanceList?.ListValues;
            }
        }

        public List<DirectorateWorkplanHeader> DirectorateWorkplanList(string empNo, string No)
        {
            var filterQuery = string.Empty;
            if (!string.IsNullOrEmpty(empNo))
            {
                filterQuery += $"Staff_No eq '{empNo}'";
            }

            if (!string.IsNullOrEmpty(No))
            {
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    filterQuery += " and ";
                }

                filterQuery += $"No eq '{No}'";
            }

            var url = string.IsNullOrEmpty(filterQuery)
                ? "DirectorateWorkplanHeader?$format=json"
                : $"DirectorateWorkplanHeader?$filter={filterQuery}&$format=json";
            var httpResponseDestForeign = Credentials.GetOdataData(url);
            httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();
                var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<DirectorateWorkplanHeader>>(result1);
                return StaffAdvanceList?.ListValues;
            }
        }

        public List<DepartmentalWorkplanHeader> DepartmentWorkplansList(string empNo, string No)
        {
            var filterQuery = string.Empty;
            if (!string.IsNullOrEmpty(empNo))
            {
                filterQuery += $"Staff_No eq '{empNo}'";
            }

            if (!string.IsNullOrEmpty(No))
            {
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    filterQuery += " and ";
                }

                filterQuery += $"No eq '{No}'";
            }

            var url = string.IsNullOrEmpty(filterQuery)
                ? "DepartmentalWorkPlanHeader?$format=json"
                : $"DepartmentalWorkPlanHeader?$filter={filterQuery}&$format=json";
            var httpResponseDestForeign = Credentials.GetOdataData(url);
            httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();
                var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<DepartmentalWorkplanHeader>>(result1);
                return StaffAdvanceList?.ListValues;
            }
        }

        public List<MidYearAppraisal> ApprovedMidYearAppraisalsList(string empNo, string No)
        {
            var filterQuery = string.Empty;
            if (!string.IsNullOrEmpty(empNo))
            {
                filterQuery += $"Staff_No eq '{empNo}'";
            }

            if (!string.IsNullOrEmpty(No))
            {
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    filterQuery += " and ";
                }

                filterQuery += $"No eq '{No}'";
            }

            var url = string.IsNullOrEmpty(filterQuery)
                ? "MidYearAppraisal?$format=json"
                : $"MidYearAppraisal?$filter={filterQuery}&$format=json";
            var httpResponseDestForeign = Credentials.GetOdataData(url);
            httpResponseDestForeign.Headers.Add("Cache-Control", "no-cache");
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();
                var StaffAdvanceList = JsonConvert.DeserializeObject<Odata<MidYearAppraisal>>(result1);
                return StaffAdvanceList?.ListValues;
            }
        }

        public TargetSettingsController()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        // GET
        public ActionResult StaffTargetObjectivesList()
        {
            try
            {
                return View(StaffTargetObjectives(employeeNo, ""));
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult TargetSetting(string ApprovedTargetSetting, bool? Editable = true)
        {
            try
            {
                ViewBag.Editable = Editable;
                ViewBag.ActiveTab = "TargetSetting";

                if (!string.IsNullOrEmpty(ApprovedTargetSetting))
                    ViewBag.ActiveTab = "ApprovedTargetSetting";
                
                bool isHR = dynamicsNAVSOAPServices.StaffAdvance.IsHr(employeeNo);

                if (isHR)
                {
                    var targetObjectivesList = _dcodataServices.BCOData.StaffTargetObjectives.Where(c => c.Employee_Category == "Marketer")
                   .ToList();

                    return View(targetObjectivesList);
                } else
                {
                    var targetObjectivesList = _dcodataServices.BCOData.StaffTargetObjectives
                  .Where(c => c.Supervisor == employeeNo && c.Employee_Category == "Marketer" && c.Sent_to_Supervisor == true).ToList();

                    return View(targetObjectivesList);
                }
               
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }
        public ActionResult PillarsTargetSettings(string ApprovedTargetSetting, bool? Editable = true)
        {
            try
            {
                ViewBag.Editable = Editable;
                ViewBag.ActiveTab = "TargetSetting";

                if (!string.IsNullOrEmpty(ApprovedTargetSetting))
                    ViewBag.ActiveTab = "ApprovedTargetSetting";

                bool isHR = dynamicsNAVSOAPServices.StaffAdvance.IsHr(employeeNo);

                if (isHR)
                {
                    var targetObjectivesList = _dcodataServices.BCOData.StaffTargetObjectives.Where(c => c.Employee_Category == "Normal" )
                   .ToList();

                    return View(targetObjectivesList);
                }
                else
                {
                    var targetObjectivesList = _dcodataServices.BCOData.StaffTargetObjectives
                  .Where(c => c.Supervisor == employeeNo && c.Employee_Category == "Normal" && c.Sent_to_Supervisor == true).ToList();

                    return View(targetObjectivesList);
                }

            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult SupervisorEmpApprovedTargetSetting(string ApprovedTargetSetting, bool? Editable = true)
        {
            try
            {
                ViewBag.Editable = Editable;
                ViewBag.ActiveTab = "SupervisorEmpApprovedTargetSetting";
                if (!string.IsNullOrEmpty(ApprovedTargetSetting))
                    ViewBag.ActiveTab = "ApprovedTargetSetting";
                bool isHR = dynamicsNAVSOAPServices.StaffAdvance.IsHr(employeeNo);

                var targetObjectivesList = _dcodataServices.BCOData.StaffTargetObjectives.Where(c =>
                        c.Approval_Status == "Approved" && c.Accepted_By_Staff == true &&
                        c.Accepted_by_Supervisor == true)
                    .ToList();
                return View(targetObjectivesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult SupervisorEmpPendingTargetSetting(string ApprovedTargetSetting, bool? Editable = true)
        {
            try
            {
                ViewBag.Editable = Editable;
                ViewBag.ActiveTab = "SupervisorEmpPendingTargetSetting";
                if (!string.IsNullOrEmpty(ApprovedTargetSetting))
                    ViewBag.ActiveTab = "ApprovedTargetSetting";
                bool isHR = dynamicsNAVSOAPServices.StaffAdvance.IsHr(employeeNo);

                var targetObjectivesList = _dcodataServices.BCOData.StaffTargetObjectives.Where(c =>
                        c.Approval_Status == "Approved" && c.Accepted_By_Staff == true &&
                        c.Accepted_by_Supervisor == false)
                    .ToList();
                return View(targetObjectivesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult HREAcceptedTargetSetting(string ApprovedTargetSetting, bool? Editable = true)
        {
            try
            {
                ViewBag.Editable = Editable;
                ViewBag.ActiveTab = "HREAcceptedTargetSetting";
                if (!string.IsNullOrEmpty(ApprovedTargetSetting))
                    ViewBag.ActiveTab = "ApprovedTargetSetting";

                var targetObjectivesList = _dcodataServices.BCOData.StaffTargetObjectives
                    .Where(c => c.Approval_Status == "Approved" && c.Accepted_By_Staff == true).ToList();
                return View(targetObjectivesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult HREPendingTargetSetting(string ApprovedTargetSetting, bool? Editable = true)
        {
            try
            {
                ViewBag.Editable = Editable;
                ViewBag.ActiveTab = "HREPendingTargetSetting";
                if (!string.IsNullOrEmpty(ApprovedTargetSetting))
                    ViewBag.ActiveTab = "ApprovedTargetSetting";
                bool isHR = dynamicsNAVSOAPServices.StaffAdvance.IsHr(employeeNo);

                var targetObjectivesList = _dcodataServices.BCOData.StaffTargetObjectives
                    .Where(c => c.Approval_Status == "Approved" && c.Accepted_By_Staff == false).ToList();
                return View(targetObjectivesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult AppraiseeTargets(string ApprovedTargetSetting, bool? Editable = true)
        {
            try
            {
                ViewBag.Editable = Editable;
                ViewBag.ActiveTab = "AppraiseeTargets";
                if (!string.IsNullOrEmpty(ApprovedTargetSetting))
                    ViewBag.ActiveTab = "ApprovedTargetSetting";

                var targetObjectivesList = _dcodataServices.BCOData.StaffTargetObjectives
                    .Where(c => c.Staff_No == employeeNo && c.HR_Approval_Status == "Approved").ToList();
                return View(targetObjectivesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult MIBstatics(string ApprovedTargetSetting, bool? Editable = true)
        {
            try
            {
                ViewBag.Editable = Editable;
                ViewBag.ActiveTab = "MIBstatics";
                if (!string.IsNullOrEmpty(ApprovedTargetSetting))
                    ViewBag.ActiveTab = "ApprovedTargetSetting";
                var targetObjectivesList =
                    _dcodataServices.BCOData.MIBstatistics.Where(c => c.Code == employeeNo).ToList();
                return View(targetObjectivesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult Salesstatics(string ApprovedTargetSetting, bool? Editable = true)
        {
            try
            {
                ViewBag.Editable = Editable;
                ViewBag.ActiveTab = "Salesstatics";
                if (!string.IsNullOrEmpty(ApprovedTargetSetting))
                    ViewBag.ActiveTab = "ApprovedTargetSetting";
                var targetObjectivesList =
                    _dcodataServices.BCOData.SalesAccrual.Where(c => c.Code == employeeNo).ToList();
                return View(targetObjectivesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult SalesInstallment(bool? Editable = true)
        {
            try
            {
                ViewBag.Editable = Editable;
                ViewBag.ActiveTab = "SalesInstallment";
                var targetObjectivesList =
                    _dcodataServices.BCOData.SalesInstallment.Where(c => c.Code == employeeNo).ToList();
                return View(targetObjectivesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult SignedSales(bool? Editable = true)
        {
            try
            {
                ViewBag.Editable = Editable;
                ViewBag.ActiveTab = "SignedSales";
                var targetObjectivesList =
                    _dcodataServices.BCOData.SignedSales.Where(c => c.Code == employeeNo).ToList();
                return View(targetObjectivesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult SupervisorTargetSetting()
        {
            try
            {
                var targetObjectivesList = _dcodataServices.BCOData.StaffTargetObjectives.Where(c =>
                    c.Supervisor == employeeNo && c.Sent_to_Supervisor == true
                                               && c.Accepted_By_Staff == false);
                return View(targetObjectivesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult SuperVisorViewTargetSettings(string no)
        {
            try
            {
                //var headerModel = TargetSettings("", no)?.FirstOrDefault();
                var headerModel = _dcodataServices.BCOData.StaffTargetObjectives.ToList()
                    .FirstOrDefault(c => c.No == no);
                var staffTargetObjectives =
                    JsonConvert.DeserializeObject<StaffTargetObjectives>(JsonConvert.SerializeObject(headerModel));
                staffTargetObjectives.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
                    new SelectListItem
                    {
                        Text = c.Code,
                        Value = c.Code,
                        Selected = staffTargetObjectives.Period == c.Code
                    }).ToList();

                return View(staffTargetObjectives);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult CorporateWorkplanList()
        {
            try
            {
                return View(CorporateWorkplan("", ""));
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult DirectorateWorkplan()
        {
            try
            {
                return View(DirectorateWorkplanList("", ""));
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }


        public ActionResult ViewStaffTargetObjective(string number)
        {
            return errorResponse.ApplicationExceptionError(new NotImplementedException());
        }

        public ActionResult ViewTargetSettings(string no)
        {
            try
            {
                var headerModel = _dcodataServices.BCOData.StaffTargetObjectives.ToList().FirstOrDefault(c => c.No == no);

                var staffTargetObjectives = JsonConvert.DeserializeObject<StaffTargetObjectives>(JsonConvert.SerializeObject(headerModel));

                staffTargetObjectives.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
                    new SelectListItem
                    {
                        Text = c.Code,
                        Value = c.Code,
                        Selected = staffTargetObjectives.Period == c.Code

                    }).ToList();

                bool isHR = dynamicsNAVSOAPServices.StaffAdvance.IsHr(employeeNo);
                staffTargetObjectives.isHr = isHR;

                var CurrentEmployee = AccountController.GetEmployeeNo();
                ViewBag.CurrentEmployee = CurrentEmployee;

                staffTargetObjectives.EmpManagers = LoadManagerEmployees(staffTargetObjectives.Staff);

                //staffTargetObjectives.TypeSelect = dictionary.Select(c =>
                //    new SelectListItem
                //    {
                //        Text = c.Value,
                //        Value = c.Key.ToString(),
                //        Selected = staffTargetObjectives.Type.Equals(c.Value, StringComparison.CurrentCultureIgnoreCase)
                //    }).ToList();
                //staffTargetObjectives.Type = dictionary
                //    .FirstOrDefault(c => c.Value.Equals(staffTargetObjectives.Type, StringComparison.CurrentCultureIgnoreCase)).Key
                //    .ToString();

                return View(staffTargetObjectives);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        [Authorize]
        public ActionResult ApprovalsTargetSettings(string no)
        {
            try
            {
                var headerModel = _dcodataServices.BCOData.StaffTargetObjectives.ToList()
                    .FirstOrDefault(c => c.No == no);
                var staffTargetObjectives =
                    JsonConvert.DeserializeObject<StaffTargetObjectives>(JsonConvert.SerializeObject(headerModel));


                var CurrentEmployee = AccountController.GetEmployeeNo();
                ViewBag.CurrentEmployee = CurrentEmployee;


                return View(staffTargetObjectives);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult ViewAprraiseTargetSettings(string no)
        {
            try
            {
                //var headerModel = TargetSettings("", no)?.FirstOrDefault();
                //var dictionary = new Dictionary<int, string> {{0, "Annual"}, {1, "Probation"}};
                var headerModel = _dcodataServices.BCOData.StaffTargetObjectives.ToList()
                    .FirstOrDefault(c => c.No == no);
                var staffTargetObjectives =
                    JsonConvert.DeserializeObject<StaffTargetObjectives>(JsonConvert.SerializeObject(headerModel));
                staffTargetObjectives.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
                    new SelectListItem
                    {
                        Text = c.Code,
                        Value = c.Code,
                        Selected = staffTargetObjectives.Period == c.Code
                    }).ToList();


                var CurrentEmployee = AccountController.GetEmployeeNo();
                ViewBag.CurrentEmployee = CurrentEmployee;

                staffTargetObjectives.EmpManagers = LoadManagerEmployees(staffTargetObjectives.Staff);

                //staffTargetObjectives.TypeSelect = dictionary.Select(c =>
                //    new SelectListItem
                //    {
                //        Text = c.Value,
                //        Value = c.Key.ToString(),
                //        Selected = staffTargetObjectives.Type.Equals(c.Value, StringComparison.CurrentCultureIgnoreCase)
                //    }).ToList();
                //staffTargetObjectives.Type = dictionary
                //    .FirstOrDefault(c => c.Value.Equals(staffTargetObjectives.Type, StringComparison.CurrentCultureIgnoreCase)).Key
                //    .ToString();

                return View(staffTargetObjectives);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult HRViewAprraiseTargetSettings(string no)
        {
            try
            {
                //var headerModel = TargetSettings("", no)?.FirstOrDefault();
                //var dictionary = new Dictionary<int, string> {{0, "Annual"}, {1, "Probation"}};
                var headerModel = _dcodataServices.BCOData.StaffTargetObjectives.ToList()
                    .FirstOrDefault(c => c.No == no);
                var staffTargetObjectives =
                    JsonConvert.DeserializeObject<StaffTargetObjectives>(JsonConvert.SerializeObject(headerModel));
                staffTargetObjectives.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
                    new SelectListItem
                    {
                        Text = c.Code,
                        Value = c.Code,
                        Selected = staffTargetObjectives.Period == c.Code
                    }).ToList();


                var CurrentEmployee = AccountController.GetEmployeeNo();
                ViewBag.CurrentEmployee = CurrentEmployee;

                staffTargetObjectives.EmpManagers = LoadManagerEmployees(staffTargetObjectives.Staff);

                //staffTargetObjectives.TypeSelect = dictionary.Select(c =>
                //    new SelectListItem
                //    {
                //        Text = c.Value,
                //        Value = c.Key.ToString(),
                //        Selected = staffTargetObjectives.Type.Equals(c.Value, StringComparison.CurrentCultureIgnoreCase)
                //    }).ToList();
                //staffTargetObjectives.Type = dictionary
                //    .FirstOrDefault(c => c.Value.Equals(staffTargetObjectives.Type, StringComparison.CurrentCultureIgnoreCase)).Key
                //    .ToString();

                return View(staffTargetObjectives);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult ViewCorporateWorkplan(string number)
        {
            return errorResponse.ApplicationExceptionError(new NotImplementedException());
        }

        public ActionResult ViewDirectorateWorkplan(string number)
        {
            return errorResponse.ApplicationExceptionError(new NotImplementedException());
        }

        public ActionResult DepartmentWorkplans()
        {
            try
            {
                return View(DepartmentWorkplansList("", ""));
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult ApprovedTargetSetting()
        {
            try
            {
                /*var StaffTargetObjectives = _dcodataServices.BCOData.StaffTargetObjectives.Where(c => c.Approved_By_Supervisor == true)
                   .ToString();*/
                ViewBag.Editable = false;
                ViewBag.ActiveTab = "ApprovedTargetSetting";
                return RedirectToAction("TargetSetting",
                    new {Editable = false, ApprovedTargetSetting = "ApprovedTargetSetting"});
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult ApprovedMidYearAppraisals()
        {
            try
            {
                var midYearAppraisals = _dcodataServices.BCOData.MidYearAppraisal
                    .Where(c => c.Staff_No == employeeNo && c.Approved_By_Supervisor == true).ToList();
                //return View(ApprovedMidYearAppraisalsList("", ""));
                return View(midYearAppraisals);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult ViewMidYearAppraisal(string number)
        {
            //var midYearAppraisal = ApprovedMidYearAppraisalsList("",number).FirstOrDefault();
            var yearAppraisal = _dcodataServices.BCOData.MidYearAppraisal.ToList().FirstOrDefault(c => c.No == number);
            var midYearAppraisal =
                JsonConvert.DeserializeObject<MidYearAppraisal>(JsonConvert.SerializeObject(yearAppraisal));
            var dictionary = new Dictionary<int, string> {{0, "Annual"}, {1, "Probation"}};
            if (midYearAppraisal != null)
            {
                midYearAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
                    new SelectListItem
                    {
                        Text = c.Code,
                        Value = c.Code,
                        Selected = midYearAppraisal.Period == c.Code
                    }).ToList();
                midYearAppraisal.TypeSelect = dictionary.Select(c =>
                    new SelectListItem
                    {
                        Text = c.Value,
                        Value = c.Key.ToString(),
                        Selected = midYearAppraisal.Type.Equals(c.Value, StringComparison.CurrentCultureIgnoreCase)
                    }).ToList();
                midYearAppraisal.Type = dictionary
                    .FirstOrDefault(c =>
                        c.Value.Equals(midYearAppraisal.Type, StringComparison.CurrentCultureIgnoreCase)).Key
                    .ToString();
                return View(midYearAppraisal);
            }

            return HttpNotFound();
        }

        public ActionResult SupervisorViewMidYearAppraisal(string number)
        {
            //var midYearAppraisal = ApprovedMidYearAppraisalsList("",number).FirstOrDefault();
            var yearAppraisal = _dcodataServices.BCOData.MidYearAppraisal.ToList().FirstOrDefault(c => c.No == number);
            var midYearAppraisal =
                JsonConvert.DeserializeObject<MidYearAppraisal>(JsonConvert.SerializeObject(yearAppraisal));
            if (midYearAppraisal != null)
            {
                midYearAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
                    new SelectListItem
                    {
                        Text = c.Code,
                        Value = c.Code,
                        Selected = midYearAppraisal.Period == c.Code
                    }).ToList();
                return View(midYearAppraisal);
            }

            return HttpNotFound();
        }

        //public ActionResult ViewApprovedMidYearAppraisal(string number)
        //{
        //    //var midYearAppraisal = ApprovedMidYearAppraisalsList("",number).FirstOrDefault();
        //    var yearAppraisal = _dcodataServices.BCOData.MidYearAppraisal.ToList().FirstOrDefault(c => c.No == number);
        //    var midYearAppraisal = JsonConvert.DeserializeObject<MidYearAppraisal>(JsonConvert.SerializeObject(yearAppraisal));
        //    if (midYearAppraisal != null)
        //    {
        //        midYearAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.Code,
        //                Value = c.Code,
        //                Selected = midYearAppraisal.Period == c.Code
        //            }).ToList();
        //        return View(midYearAppraisal);
        //    }

        //    return HttpNotFound();
        //}

        //[HttpPost]
        //public ActionResult SupervisorViewMidYearAppraisal(MidYearAppraisal yearMidAppraisal)
        //{
        //    try
        //    {
        //        //var midYearAppraisal = ApprovedMidYearAppraisalsList("",number).FirstOrDefault();
        //        DateTime DateConv;
        //        const string format = "yyyy-mm-dd";
        //        //DateTime.TryParseExact(midYearAppraisal.Date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateConv);
        //        DateTime.TryParse(yearMidAppraisal.Date.ToString(), out DateConv);
        //        var saved = dynamicsNAVSOAPServices.StaffAdvance.EditTMiddleYearHeader(yearMidAppraisal.No, DateConv.Date,
        //            yearMidAppraisal.Employee_Comments ?? "", yearMidAppraisal.Supervisor_Comments ?? "", yearMidAppraisal.Supervisor ?? "", yearMidAppraisal.Period ?? "",Convert.ToInt32(yearMidAppraisal.Type));
        //        if (saved)
        //        {
        //            return RedirectToAction("SupervisorViewMidYearAppraisal", "TargetSettings", new { number = yearMidAppraisal.No });
        //        }

        //        var yearAppraisal = _dcodataServices.BCOData.MidYearAppraisal.ToList().FirstOrDefault(c => c.No == yearMidAppraisal.No);
        //        var midYearAppraisal = JsonConvert.DeserializeObject<MidYearAppraisal>(JsonConvert.SerializeObject(yearAppraisal));
        //        if (midYearAppraisal != null)
        //        {
        //            midYearAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
        //                new SelectListItem
        //                {
        //                    Text = c.Code,
        //                    Value = c.Code,
        //                    Selected = midYearAppraisal.Period == c.Code
        //                }).ToList();
        //            return View(midYearAppraisal);
        //        }

        //        return HttpNotFound();
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}
        //public ActionResult ViewApprovedMidYearAppraisal(MidYearAppraisal yearMidAppraisal)
        //{
        //    try
        //    {
        //        //var midYearAppraisal = ApprovedMidYearAppraisalsList("",number).FirstOrDefault();
        //        DateTime DateConv;
        //        const string format = "yyyy-mm-dd";
        //        //DateTime.TryParseExact(midYearAppraisal.Date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateConv);
        //        DateTime.TryParse(yearMidAppraisal.Date.ToString(), out DateConv);
        //        var saved = dynamicsNAVSOAPServices.StaffAdvance.EditTMiddleYearHeader(yearMidAppraisal.No, DateConv.Date,
        //            yearMidAppraisal.Employee_Comments ?? "", yearMidAppraisal.Supervisor_Comments ?? "", yearMidAppraisal.Supervisor ?? "", yearMidAppraisal.Period ?? "", Convert.ToInt32(yearMidAppraisal.Type));
        //        if (saved)
        //        {
        //            return RedirectToAction("ViewApprovedMidYearAppraisal", "TargetSettings", new { number = yearMidAppraisal.No });
        //        }

        //        var yearAppraisal = _dcodataServices.BCOData.MidYearAppraisal.ToList().FirstOrDefault(c => c.No == yearMidAppraisal.No);
        //        var midYearAppraisal = JsonConvert.DeserializeObject<MidYearAppraisal>(JsonConvert.SerializeObject(yearAppraisal));
        //        if (midYearAppraisal != null)
        //        {
        //            midYearAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
        //                new SelectListItem
        //                {
        //                    Text = c.Code,
        //                    Value = c.Code,
        //                    Selected = midYearAppraisal.Period == c.Code
        //                }).ToList();
        //            return View(midYearAppraisal);
        //        }

        //        return HttpNotFound();
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        public ActionResult EditWorkplanHeader()
        {
            throw new NotImplementedException();
        }

        public ActionResult _TargetSettingLine(string no, string period,string staff, string status, bool editable = true)
        {
            try
            {
                ViewBag.workplanno = no;
                ViewBag.Period = period;
                ViewBag.editable = editable;
                ViewBag.Staff = staff;
                ViewBag.Status = status;

                var CurrentEmployee = AccountController.GetEmployeeNo();
                ViewBag.CurrentEmployee = CurrentEmployee;

                bool isHR = dynamicsNAVSOAPServices.StaffAdvance.IsHr(employeeNo);
                ViewBag.IsHr = isHR;

                var staffTargetLinesList =
                    _dcodataServices.BCOData.Staff_Target_Lines.Where(c => c.Doc_No == no).ToList();
                //if (editable)
                //    staffTargetLinesList = staffTargetLinesList.Where(c => c.Supervisor == employeeNo).ToList();
                foreach (var staffTargetLines in staffTargetLinesList)
                {
                    try
                    {
                        staffTargetLines.Specific_Action_Plan =
                            dynamicsNAVSOAPServices.StaffAdvance.StaffSettingLineSpecific_Action_Plan(
                                staffTargetLines.Doc_No, staffTargetLines.No);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    //try
                    //{
                    //    staffTargetLines.Success_Measure = dynamicsNAVSOAPServices.StaffAdvance.StaffSettingLineSuccess_Measure(staffTargetLines.Doc_No, staffTargetLines.No);
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e.Message);
                    //}
                    //if (staffTargetLines.Due_Date != null) staffTargetLines.Due_Date = Convert.ToDateTime(staffTargetLines.Due_Date.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
                }

                /*if (!string.IsNullOrEmpty(period))
                    staffTargetLinesList = staffTargetLinesList.Where(c => c.Period == period).ToList();*/
                //return PartialView(TargetSettingLines(workplanno));
                return PartialView(staffTargetLinesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult _AppraiseeTargetSettingLine(string no, string period, bool editable = true)
        {
            try
            {
                ViewBag.workplanno = no;
                ViewBag.Period = period;
                ViewBag.editable = editable;
                var CurrentEmployee = AccountController.GetEmployeeNo();
                ViewBag.CurrentEmployee = CurrentEmployee;

                var staffTargetLinesList =
                    _dcodataServices.BCOData.Staff_Target_Lines.Where(c => c.Doc_No == no).ToList();
                if (editable)
                    staffTargetLinesList = staffTargetLinesList.Where(c => c.Staff_No == employeeNo).ToList();
                foreach (var staffTargetLines in staffTargetLinesList)
                {
                    try
                    {
                        staffTargetLines.Specific_Action_Plan =
                            dynamicsNAVSOAPServices.StaffAdvance.StaffSettingLineSpecific_Action_Plan(
                                staffTargetLines.Doc_No, staffTargetLines.No);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                return PartialView(staffTargetLinesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        //public ActionResult UpdateTargetSettingLine(WorkplanLine WorkplanLine)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(WorkplanLine.TargetLineNumber))
        //        {
        //            dynamicsNAVSOAPServices.StaffAdvance.NewTargetSettingLine(WorkplanLine?.Number, WorkplanLine.DepartmentalObjective, WorkplanLine.Project,
        //                WorkplanLine.Activity,
        //                WorkplanLine.PerformanceMeasure,
        //                WorkplanLine.CompletionDate,
        //                WorkplanLine.Targets,
        //                WorkplanLine.TargetScore,
        //                WorkplanLine.WeightTotal,
        //                WorkplanLine.Directorate,
        //                WorkplanLine.Department,
        //                WorkplanLine.Perspective);
        //        }
        //        else
        //            dynamicsNAVSOAPServices.StaffAdvance.EditTargetSettingLine(WorkplanLine?.TargetLineNumber, WorkplanLine?.Number, WorkplanLine.DepartmentalObjective, WorkplanLine.Project,
        //                WorkplanLine.Activity,
        //                WorkplanLine.PerformanceMeasure,
        //                WorkplanLine.CompletionDate,
        //                WorkplanLine.Targets,
        //                WorkplanLine.TargetScore,
        //                WorkplanLine.WeightTotal,
        //                WorkplanLine.Directorate,
        //                WorkplanLine.Department,
        //                WorkplanLine.Perspective);

        //        return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult PerformanceInfo()
        {
            return View();
        }

        public ActionResult _PerformanceSidebar()
        {
            EmployeeProfileModel employeeProfileModel = new EmployeeProfileModel();
            employeeProfileModel.PassportAttached = false;
            bool isHR = dynamicsNAVSOAPServices.StaffAdvance.IsHr(employeeNo);
            employeeProfileModel.IsHr = isHR;
            return PartialView(employeeProfileModel);
        }

        [HttpGet]
        public ActionResult NewTargetSetting()
        {
            try
            {
                var no = dynamicsNAVSOAPServices.StaffAdvance.NewTargetSettingHeader(employeeNo);
                var headerTargets = StaffTargetObjectives("", no).FirstOrDefault();
                if (headerTargets != null)
                {
                    /*headerTargets.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c => new SelectListItem()
                    {
                        Text = c.Code,
                        Value = c.Code
                    }).ToList();
                    return View(headerTargets);*/
                    return RedirectToAction("ViewTargetSettings", new {no});
                }

                TempData["Error"] = "Error creating new  target";
                return RedirectToAction("TargetSetting", "TargetSettings");
                //return errorResponse.ApplicationExceptionError(new Exception("Error creating new  target"));
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message.ToString();
                return RedirectToAction("TargetSetting", "TargetSettings");
                //return errorResponse.ApplicationExceptionError(e);
            }
        }

        [HttpPost]
        public ActionResult NewTargetSetting(StaffTargetObjectives targetObjectives)
        {
            try
            {
                var saved = dynamicsNAVSOAPServices.StaffAdvance.EditTargetSettingHeader(targetObjectives.Number,
                    targetObjectives.Staff ?? "");
                if (saved)
                {
                    TempData["Success"] = "Header updated successfully";
                    return RedirectToAction("ViewTargetSettings", "TargetSettings", new {no = targetObjectives.Number});
                }

                targetObjectives.errorMessage = "An error occured while updating please try again";
                targetObjectives.ErrorStatus = true;
                return errorResponse.ApplicationExceptionError(
                    new Exception("An error occured while updating please try again"));
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }
        [HttpPost]
        public ActionResult SaveHeader(string Number, string Staff)
        {
            try
            {
                var saved = dynamicsNAVSOAPServices.StaffAdvance.EditTargetSettingHeader(Number, Staff ?? "");
                if (saved)
                {
                    TempData["Success"] = "Header updated successfully";
                    return Json(new { success = true, message = "Header updated successfully" });
                }

                return Json(new { success = false, message = "An error occurred while updating. Please try again." });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An exception occurred: " + e.Message });
            }
        }

        public ActionResult MidYearAppraisals()
        {
            var midYearAppraisals =
                _dcodataServices.BCOData.MidYearAppraisal.Where(c => c.Staff_No == employeeNo).ToList();
            return View(midYearAppraisals);
        }

        public ActionResult ProbationAppraisals()
        {
            var midYearAppraisals =
                _dcodataServices.BCOData.MidYearAppraisal.Where(c => c.Staff_No == employeeNo).ToList();
            return View(midYearAppraisals);
        }

        public ActionResult SupervisorMidYearAppraisals()
        {
            var midYearAppraisals = _dcodataServices.BCOData.MidYearAppraisal.Where(c => c.Supervisor == employeeNo
                && c.Sent_to_Supervisor == true
                && c.Approved_By_Supervisor == false).ToList();
            return View(midYearAppraisals);
        }

        public ActionResult PeerAppraisals()
        {
            /*_bcodatav4Services.BCOData.AddToItems(new Items()
            {
                Description = "tr"
            });
            _bcodatav4Services.BCOData.SaveChanges();*/
            var appraisalHeaders = _dcodataServices.BCOData.Peer_Appraisal_Header.Execute()
                .Where(c => c.Supervisor == employeeNo).ToList();
            return View(appraisalHeaders);
        }

        //public ActionResult Appraisals()
        //{
        //    var appraisalHeaders = _dcodataServices.BCOData.Staff_Appraisal_Header.Where(c => c.Staff_No == employeeNo).ToList();
        //    return View(appraisalHeaders);
        //}

        //public ActionResult ApprovedAppraisals()
        //{
        //    var appraisalHeaders = _dcodataServices.BCOData.Staff_Appraisal_Header.Where(c => c.Staff_No == employeeNo
        //                                                                                      && c.Approved_By_Supervisor == true).ToList();
        //    ViewBag.ApprovedAppraisals = true;
        //    return View("Appraisals", appraisalHeaders);
        //}
        public ActionResult Appraisals(string ApprovedAppraisals)
        {
            ViewBag.ActiveTab = "Appraisals";
            //if (!string.IsNullOrEmpty(ApprovedAppraisals))
            //    ViewBag.ActiveTab = "ApprovedAppraisals";
            var appraisalHeaders = _dcodataServices.BCOData.Staff_Appraisal_Header
                .Where(c => c.Staff_No == employeeNo)
                .ToList();
            return View("Appraisals", appraisalHeaders);
        }

        public ActionResult ApprovedAppraisals()
        {
            var appraisalHeaders = _dcodataServices.BCOData.Staff_Appraisal_Header
                .Where(c => c.Staff_No == employeeNo && c.Approved_By_Supervisor == true)
                .ToList();

            ViewBag.ApprovedAppraisals = true;
            ViewBag.ActiveTab = "ApprovedAppraisals";
            return View("Appraisals", appraisalHeaders);
        }

        public ActionResult SupervisorAppraisals()
        {
            var appraisalHeaders = _dcodataServices.BCOData.Staff_Appraisal_Header.Where(c => c.Supervisor == employeeNo
                && c.Sent_to_Supervisor == true && c.Approved_By_Supervisor == false).ToList();
            ViewBag.ApprovedAppraisals = true;
            return View(appraisalHeaders);
        }

        public ActionResult SupervisorApprovedMidAppraisals()
        {
            var midYearAppraisals = _dcodataServices.BCOData.MidYearAppraisal.Where(c => c.Supervisor == employeeNo
                && c.Sent_to_Supervisor == true
                && c.Approved_By_Supervisor == true).ToList();
            return View(midYearAppraisals);
        }

        public ActionResult SupervisorApprovedTargetSetting()
        {
            try
            {

                var targetObjectivesList = _dcodataServices.BCOData.StaffTargetObjectives.Where(c =>
                    c.Supervisor == employeeNo && c.Sent_to_Supervisor == true
                                               && c.Accepted_By_Staff == true);
                return View(targetObjectivesList);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult SupervisorApprovedAppraisals()
        {
            var appraisalHeaders = _dcodataServices.BCOData.Staff_Appraisal_Header.Where(c => c.Supervisor == employeeNo
                && c.Sent_to_Supervisor == true && c.Approved_By_Supervisor == true).ToList();
            ViewBag.ApprovedAppraisals = true;
            return View(appraisalHeaders);
        }
        //[HttpGet]
        //public ActionResult NewAppraisal()
        //{
        //    try
        //    {
        //        var no = dynamicsNAVSOAPServices.StaffAdvance.NewStaffAppraisalHeader(employeeNo);
        //        return RedirectToAction("ViewAppraisal", new {no });
        //        /*var appraisalHeader = _dcodataServices.BCOData.Staff_Appraisal_Header.AsEnumerable().FirstOrDefault(c => c.No == no);
        //        var appraisal = JsonConvert.DeserializeObject<Appraisal>(JsonConvert.SerializeObject(appraisalHeader));
        //        appraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.Code,
        //                Value = c.Code,
        //                Selected = appraisal.Period == c.Code
        //            }).ToList();
        //        appraisal.TypeSelect = new Dictionary<int, string> { { 0, "General" }, { 1, "Probation" } }.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.Value,
        //                Value = c.Key.ToString(),
        //                Selected = appraisal.Type.Equals(c.Value, StringComparison.CurrentCultureIgnoreCase)
        //            }).ToList();
        //        return View(appraisal);*/
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        //[HttpPost]
        //public ActionResult NewAppraisal(Staff_Appraisal_Header staffAppraisalHeader)
        //{
        //    try
        //    {
        //        var saved = dynamicsNAVSOAPServices.StaffAdvance.EditStaffAppraisalHeader(staffAppraisalHeader.No, staffAppraisalHeader.Period, Convert.ToInt32(staffAppraisalHeader.Type));
        //        if (saved)
        //        {
        //            return RedirectToAction("ViewAppraisal", "TargetSettings", new { no = staffAppraisalHeader.No });
        //        }

        //        var appraisalHeader = _dcodataServices.BCOData.Staff_Appraisal_Header.AsEnumerable().FirstOrDefault(c => c.No == staffAppraisalHeader.No);
        //        var appraisal = JsonConvert.DeserializeObject<Appraisal>(JsonConvert.SerializeObject(appraisalHeader));
        //        appraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.Code,
        //                Value = c.Code,
        //                Selected = appraisal.Period == c.Code
        //            }).ToList();
        //        appraisal.TypeSelect = new Dictionary<int, string> { { 0, "General" }, { 1, "Probation" } }.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.Value,
        //                Value = c.Key.ToString(),
        //                Selected = appraisal.Type.Equals(c.Value, StringComparison.CurrentCultureIgnoreCase)
        //            }).ToList();
        //        return View();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //}

        [HttpGet]
        public ActionResult ViewAppraisal(string no, bool ediatable = true)
        {
            ViewBag.ediatable = ediatable;
            var appraisalHeader = _dcodataServices.BCOData.Staff_Appraisal_Header.AsEnumerable()
                .FirstOrDefault(c => c.No == no);
            var appraisal = JsonConvert.DeserializeObject<Appraisal>(JsonConvert.SerializeObject(appraisalHeader));
            appraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
                new SelectListItem
                {
                    Text = c.Code,
                    Value = c.Code,
                    Selected = appraisal.Period == c.Code
                }).ToList();
            appraisal.TypeSelect = new Dictionary<int, string> {{0, "General"}, {1, "Probation"}}.Select(c =>
                new SelectListItem
                {
                    Text = c.Value,
                    Value = c.Key.ToString(),
                    Selected = appraisal.Type.Equals(c.Value, StringComparison.CurrentCultureIgnoreCase)
                }).ToList();
            return View(appraisal);
        }

        //[HttpPost]
        //public ActionResult ViewAppraisal(Staff_Appraisal_Header staffAppraisalHeader)
        //{
        //    try
        //    {
        //        var saved = dynamicsNAVSOAPServices.StaffAdvance.EditStaffAppraisalHeader(staffAppraisalHeader.No, staffAppraisalHeader.Period, Convert.ToInt32(staffAppraisalHeader.Type));

        //        var appraisalHeader = _dcodataServices.BCOData.Staff_Appraisal_Header.AsEnumerable().FirstOrDefault(c => c.No == staffAppraisalHeader.No);
        //        var appraisal = JsonConvert.DeserializeObject<Appraisal>(JsonConvert.SerializeObject(appraisalHeader));
        //        appraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.Code,
        //                Value = c.Code,
        //                Selected = appraisal.Period == c.Code
        //            }).ToList();
        //        appraisal.TypeSelect = new Dictionary<int, string> { { 0, "General" }, { 1, "Probation" } }.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.Value,
        //                Value = c.Key.ToString(),
        //                Selected = appraisal.Type.Equals(c.Value, StringComparison.CurrentCultureIgnoreCase)
        //            }).ToList();
        //        return View(appraisal);
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        [HttpGet]
        public ActionResult SupervisorViewAppraisal(string no, bool ediatable = true)
        {
            ViewBag.ediatable = ediatable;
            var appraisalHeader = _dcodataServices.BCOData.Staff_Appraisal_Header.AsEnumerable()
                .FirstOrDefault(c => c.No == no);
            var appraisal = JsonConvert.DeserializeObject<Appraisal>(JsonConvert.SerializeObject(appraisalHeader));
            appraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
                new SelectListItem
                {
                    Text = c.Code,
                    Value = c.Code,
                    Selected = appraisal.Period == c.Code
                }).ToList();
            appraisal.TypeSelect = new Dictionary<int, string> {{0, "General"}, {1, "Probation"}}.Select(c =>
                new SelectListItem
                {
                    Text = c.Value,
                    Value = c.Key.ToString(),
                    Selected = appraisal.Type.Equals(c.Value, StringComparison.CurrentCultureIgnoreCase)
                }).ToList();
            return View(appraisal);
        }

        //[HttpGet]
        //public ActionResult NewPeerAppraisal()
        //{
        //    try
        //    {
        //        var no = dynamicsNAVSOAPServices.StaffAdvance.NewPeerAppraisalHeader(employeeNo);
        //        var appraisalHeader = _dcodataServices.BCOData.Peer_Appraisal_Header.AsEnumerable().FirstOrDefault(c => c.No == no);
        //        var peerAppraisal = JsonConvert.DeserializeObject<PeerAppraisal>(JsonConvert.SerializeObject(appraisalHeader));
        //        peerAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.Code,
        //                Value = c.Code,
        //            }).ToList();
        //        peerAppraisal.Peer_Appraiser_1Select = _dcodataServices.BCOData.Employees.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.No,
        //                Value = $"{c.No}:{c.Full_Name}",
        //                Selected = peerAppraisal.Peer_Appraiser_1 == c.No
        //            }).ToList();
        //        peerAppraisal.Peer_Appraiser_2Select = _dcodataServices.BCOData.Employees.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.No,
        //                Value = $"{c.No}:{c.Full_Name}",
        //                Selected = peerAppraisal.Peer_Appraiser_2 == c.No
        //            }).ToList();
        //        peerAppraisal.Peer_Appraiser_3Select = _dcodataServices.BCOData.Employees.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.No,
        //                Value = $"{c.No}:{c.Full_Name}",
        //                Selected = peerAppraisal.Peer_Appraiser_3 == c.No
        //            }).ToList();
        //        return View(peerAppraisal);
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        [HttpGet]
        public ActionResult ViewPeerAppraisal(string no)
        {
            var appraisalHeader = _dcodataServices.BCOData.Peer_Appraisal_Header.AsEnumerable()
                .FirstOrDefault(c => c.No == no);
            var peerAppraisal =
                JsonConvert.DeserializeObject<PeerAppraisal>(JsonConvert.SerializeObject(appraisalHeader));
            peerAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
                new SelectListItem
                {
                    Text = c.Code,
                    Value = c.Code,
                    Selected = peerAppraisal.Period == c.Code
                }).ToList();
            var employeesEnumerable = _dcodataServices.BCOData.Employees.Execute().ToList();
            peerAppraisal.Peer_Appraiser_1Select = employeesEnumerable.Select(c =>
                new SelectListItem
                {
                    Value = c.No,
                    Text = $"{c.No}:{c.Full_Name}",
                    Selected = peerAppraisal.Peer_Appraiser_1 == c.No
                }).ToList();
            peerAppraisal.Peer_Appraiser_2Select = employeesEnumerable.Select(c =>
                new SelectListItem
                {
                    Value = c.No,
                    Text = $"{c.No}:{c.Full_Name}",
                    Selected = peerAppraisal.Peer_Appraiser_2 == c.No
                }).ToList();
            peerAppraisal.Peer_Appraiser_3Select = employeesEnumerable.Select(c =>
                new SelectListItem
                {
                    Value = c.No,
                    Text = $"{c.No}:{c.Full_Name}",
                    Selected = peerAppraisal.Peer_Appraiser_3 == c.No
                }).ToList();
            return View(peerAppraisal);
        }

        //[HttpPost]
        //public ActionResult ViewPeerAppraisal(Peer_Appraisal_Header appraisalHeader)
        //{
        //    try
        //    {
        //        var peerAppraisalHeader = _dcodataServices.BCOData.Peer_Appraisal_Header.AsEnumerable().FirstOrDefault(c => c.No == appraisalHeader.No);
        //        //var peerAppraisalHeader2 = _bcodatav4Services.BCOData.Peer_Appraisal_Header.AsEnumerable().FirstOrDefault(c => c.No == appraisalHeader.No);

        //        var peerAppraisal = JsonConvert.DeserializeObject<PeerAppraisal>(JsonConvert.SerializeObject(peerAppraisalHeader));
        //        var employeesEnumerable = _dcodataServices.BCOData.Employees.Execute().ToList();
        //        peerAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
        //            new SelectListItem
        //            {
        //                Text = c.Code,
        //                Value = c.Code,
        //                Selected = peerAppraisal.Period == c.Code
        //            }).ToList();
        //        peerAppraisal.Peer_Appraiser_1Select = employeesEnumerable.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.No,
        //                Text = $"{c.No}:{c.Full_Name}",
        //                Selected = peerAppraisal.Peer_Appraiser_1 == c.No
        //            }).ToList();
        //        peerAppraisal.Peer_Appraiser_2Select = employeesEnumerable.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.No,
        //                Text = $"{c.No}:{c.Full_Name}",
        //                Selected = peerAppraisal.Peer_Appraiser_2 == c.No
        //            }).ToList();
        //        peerAppraisal.Peer_Appraiser_3Select = employeesEnumerable.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.No,
        //                Text = $"{c.No}:{c.Full_Name}",
        //                Selected = peerAppraisal.Peer_Appraiser_3 == c.No
        //            }).ToList();
        //        var modified = dynamicsNAVSOAPServices.StaffAdvance.EditPeerAppraisalHeader(appraisalHeader.No, appraisalHeader.Peer_Appraiser_1, appraisalHeader.Peer_Appraiser_2
        //            , appraisalHeader.Peer_Appraiser_3, appraisalHeader.Period ?? "", appraisalHeader.Description ?? "", appraisalHeader.Supervisor ?? "");
        //        if (modified)
        //        {
        //            return RedirectToAction("PeerAppraisals", "TargetSettings");
        //        }

        //        return View(peerAppraisal);
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        //[HttpGet]
        //public ActionResult NewMidYearAppraisal()
        //{
        //    try
        //    {
        //        var no = dynamicsNAVSOAPServices.StaffAdvance.NewMiddleYearHeader(employeeNo);
        //        return RedirectToAction("ViewMidYearAppraisal", new { number= no});
        //        /*var midYearAppraisal = ApprovedMidYearAppraisalsList("", no).FirstOrDefault();
        //        if (midYearAppraisal != null)
        //        {
        //            midYearAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c => new SelectListItem()
        //            {
        //                Text = c.Code,
        //                Value = c.Code
        //            }).ToList();
        //            return View(midYearAppraisal);
        //        }

        //        return errorResponse.ApplicationExceptionError(new Exception("Error creating new  MidYear Appraisal"));*/
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["Error"] = e.Message.ToString();
        //        return RedirectToAction("TargetSetting", "TargetSettings");
        //        //return errorResponse.ApplicationExceptionError(e);
        //    }
        //}
        //[HttpGet]
        //public ActionResult NewProbationAppraisal()
        //{
        //    try
        //    {
        //        var no = dynamicsNAVSOAPServices.StaffAdvance.NewMiddleYearHeader(employeeNo);
        //        return RedirectToAction("ViewProbationAppraisal", new { no });
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["Error"] = e.Message.ToString();
        //        return RedirectToAction("TargetSetting", "TargetSettings");
        //        //return errorResponse.ApplicationExceptionError(e);
        //    }
        //}
        //[HttpPost]
        //public ActionResult ViewProbationAppraisal(MidYearAppraisal midYearAppraisal)
        //{
        //    try
        //    {
        //        DateTime DateConv;
        //        const string format = "yyyy-mm-dd";
        //        //DateTime.TryParseExact(midYearAppraisal.Date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateConv);
        //        DateTime.TryParse(midYearAppraisal.Date.ToString(), out DateConv);
        //        var saved = dynamicsNAVSOAPServices.StaffAdvance.EditTMiddleYearHeader(midYearAppraisal.No, DateConv.Date,
        //            midYearAppraisal.Employee_Comments ?? "", midYearAppraisal.Supervisor_Comments ?? "", midYearAppraisal.Supervisor ?? "", midYearAppraisal.Period ?? "", Convert.ToInt32(midYearAppraisal.Type));
        //        if (saved)
        //        {
        //            return RedirectToAction("ViewProbationAppraisal", "TargetSettings", new { no = midYearAppraisal.No });
        //        }

        //        midYearAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c => new SelectListItem()
        //        {
        //            Text = c.Code,
        //            Value = c.Code
        //        }).ToList();
        //        return errorResponse.ApplicationExceptionError(new Exception("Error creating new  MidYear Appraisal"));
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        [HttpGet]
        public ActionResult ViewProbationAppraisal(string no)
        {
            try
            {
                var midYearAppraisal = ApprovedMidYearAppraisalsList("", no).FirstOrDefault();
                if (midYearAppraisal != null)
                {
                    midYearAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c =>
                        new SelectListItem()
                        {
                            Text = c.Code,
                            Value = c.Code
                        }).ToList();
                    return View(midYearAppraisal);
                }

                return errorResponse.ApplicationExceptionError(new Exception("Error creating new  MidYear Appraisal"));
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        //[HttpPost]
        //public ActionResult NewMidYearAppraisal(MidYearAppraisal midYearAppraisal)
        //{
        //    try
        //    {
        //        DateTime DateConv;
        //        const string format = "yyyy-mm-dd";
        //        //DateTime.TryParseExact(midYearAppraisal.Date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateConv);
        //        DateTime.TryParse(midYearAppraisal.Date.ToString(), out DateConv);
        //        var saved = dynamicsNAVSOAPServices.StaffAdvance.EditTMiddleYearHeader(midYearAppraisal.No, DateConv.Date,
        //            midYearAppraisal.Employee_Comments ?? "", midYearAppraisal.Supervisor_Comments ?? "", midYearAppraisal.Supervisor ?? "", midYearAppraisal.Period ?? "", Convert.ToInt32(midYearAppraisal.Type));
        //        if (saved)
        //        {
        //            return RedirectToAction("ViewMidYearAppraisal", "TargetSettings", new { number = midYearAppraisal.No });
        //        }

        //        midYearAppraisal.PeriodSelect = _dcodataServices.BCOData.HR_Appraisal_Periods.Select(c => new SelectListItem()
        //        {
        //            Text = c.Code,
        //            Value = c.Code
        //        }).ToList();
        //        return errorResponse.ApplicationExceptionError(new Exception("Error creating new  MidYear Appraisal"));
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        //public ActionResult _MidYearLines(string no, bool editable = true, bool isSupervisor = false)
        //{
        //    ViewBag.no = no;
        //    ViewBag.editable = editable;
        //    ViewBag.isSupervisor = isSupervisor;
        //    var linesList = _dcodataServices.BCOData.MidYear_Appraisal_Lines.Where(c => c.Doc_No == no && c.Type == "Checkin Agenda").ToList();
        //    foreach (var midYearAppraisalLines in linesList)
        //    {
        //        midYearAppraisalLines.Success_Measure = dynamicsNAVSOAPServices.StaffAdvance.MidYearLineSuccess_Measure(midYearAppraisalLines.Doc_No, midYearAppraisalLines.Entry_No.ToString());
        //        midYearAppraisalLines.Items_Description = dynamicsNAVSOAPServices.StaffAdvance.MidYearLineDescription(midYearAppraisalLines.Doc_No, midYearAppraisalLines.Entry_No.ToString());
        //    }
        //    return PartialView(linesList);
        //}

        public ActionResult _MidYearConcernsLines(string no, bool editable = true, bool isSupervisor = false)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            ViewBag.isSupervisor = isSupervisor;
            var linesList = _dcodataServices.BCOData.MidYear_Appraisal_Lines
                .Where(c => c.Doc_No == no && c.Type == "Concerns").ToList();
            return PartialView(linesList);
        }

        public ActionResult _MidYearAgreedActionsLines(string no, bool editable = true, bool isSupervisor = false)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            ViewBag.isSupervisor = isSupervisor;
            var linesList = _dcodataServices.BCOData.MidYear_Appraisal_Lines
                .Where(c => c.Doc_No == no && c.Type == "Agreed Actions").ToList();
            return PartialView(linesList);
        }

        /*public ActionResult SaveMidYearLine(MidYear_Appraisal_Lines yearAppraisalLines)
        {
            try
            {
                var saved = dynamicsNAVSOAPServices.StaffAdvance.NewMidYearLine(yearAppraisalLines.Doc_No,yearAppraisalLines.Items_Description,
                    yearAppraisalLines.Success_Measure,yearAppraisalLines.Staff_Comments,yearAppraisalLines.Supervisor_Comments);
                return Json(saved ? new {success=true, message="Saved successfully"} : new {success=false, message="Error on Saved data"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success=false, message=e.Message},JsonRequestBehavior.AllowGet);
            }
        }*/

        //public ActionResult ModifyMidYearLine(MidYear_Appraisal_Lines yearAppraisalLines)
        //{
        //    try
        //    {
        //        if (yearAppraisalLines.Entry_No == 0)
        //        {
        //            var yearLineEntry = dynamicsNAVSOAPServices.StaffAdvance.NewMidYearLine(yearAppraisalLines.Doc_No, yearAppraisalLines.Items_Description,
        //                yearAppraisalLines.Success_Measure ?? "", yearAppraisalLines.Staff_Comments ?? "", yearAppraisalLines.Supervisor_Comments ?? "", int.Parse(yearAppraisalLines.Type));
        //            return Json(yearLineEntry != 0 ? new { success = true, message = "Saved successfully", entryNo = yearLineEntry } : new { success = false, message = "Error on Saved data", entryNo = 0 }, JsonRequestBehavior.AllowGet);
        //        }

        //        var saved = dynamicsNAVSOAPServices.StaffAdvance.EditTMidYearLine(yearAppraisalLines.Entry_No.ToString(), yearAppraisalLines.Items_Description,
        //            yearAppraisalLines.Success_Measure ?? "", yearAppraisalLines.Staff_Comments ?? "", yearAppraisalLines.Supervisor_Comments ?? "");
        //        return Json(saved ? new { success = true, message = "Saved successfully" } : new { success = false, message = "Error on Saved data" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult ModifyTargetLine(Staff_Target_Lines staffTargetLines, string Command)
        //{
        //    try
        //    {
        //        /*if(!_dcodataServices.BCOData.Staff_Target_Lines.AsEnumerable().Any(c=>c.No ==staffTargetLines.No && c.Objective_Code == staffTargetLines.Objective_Code &&
        //            c.Objective == staffTargetLines.Objective&& c.Success_Measure == staffTargetLines.Success_Measure && c.Doc_No ==staffTargetLines.Doc_No
        //                                                                         && c.Specific_Action_Plan ==staffTargetLines.Specific_Action_Plan&& c.Due_Date_Description == staffTargetLines.Due_Date_Description))*/
        //        //if (staffTargetLines.No == "0")
        //        if (Command.Equals("create", StringComparison.CurrentCultureIgnoreCase))
        //        {
        //            var LineEntry = dynamicsNAVSOAPServices.StaffAdvance.NewStaffTargetLines(staffTargetLines.No, staffTargetLines.Objective_Code,
        //                staffTargetLines.Objective, staffTargetLines.Success_Measure, staffTargetLines.Due_Date ?? DateTime.Now, employeeNo, staffTargetLines.Doc_No
        //                , staffTargetLines.Specific_Action_Plan, staffTargetLines.Due_Date_Description, staffTargetLines.Period ?? "");
        //            return Json(LineEntry != "0" ? new { success = true, message = "Saved successfully", No = LineEntry } : new { success = false, message = "Error on Saved data", No = "0" }, JsonRequestBehavior.AllowGet);
        //        }

        //        var saved = dynamicsNAVSOAPServices.StaffAdvance.EditStaffTargetLines(staffTargetLines.No, staffTargetLines.Objective_Code,
        //            staffTargetLines.Objective, staffTargetLines.Success_Measure, staffTargetLines.Due_Date ?? DateTime.Now, employeeNo, staffTargetLines.Doc_No
        //            , staffTargetLines.Specific_Action_Plan, staffTargetLines.Due_Date_Description);
        //        return Json(saved ? new { success = true, message = "Saved successfully" } : new { success = false, message = "Error on Saved data" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult ModifyTargetLine(Staff_Target_Lines staffTargetLines, string Command,
            string[] Specific_Action_Plan)
        {
            try
            {
                if (Command.Equals("create", StringComparison.CurrentCultureIgnoreCase))
                {
                    // Create logic
                    var LineEntry = "";
                    if (Specific_Action_Plan.Length > 0)
                    {
                        foreach (var item in Specific_Action_Plan)
                        {
                            LineEntry = dynamicsNAVSOAPServices.StaffAdvance.NewStaffTargetLines(
                                staffTargetLines.Objective, employeeNo,
                                item, staffTargetLines.Marks ?? decimal.Zero, staffTargetLines.Doc_No, staffTargetLines.Target_Amount ?? decimal.Zero,staffTargetLines.KPIDescription ?? "");
                        }
                    }
                    else
                    {
                        LineEntry = dynamicsNAVSOAPServices.StaffAdvance.NewStaffTargetLines(staffTargetLines.Objective,
                            employeeNo,
                            staffTargetLines.Specific_Action_Plan, staffTargetLines.Marks ?? decimal.Zero,
                            staffTargetLines.Doc_No, staffTargetLines.Target_Amount ?? decimal.Zero, staffTargetLines.KPIDescription ?? "");
                    }

                    return Json(
                        LineEntry != "0"
                            ? new {success = true, message = "Saved successfully", No = LineEntry}
                            : new {success = false, message = "Error on Saved data", No = "0"},
                        JsonRequestBehavior.AllowGet);
                }

                var saved = dynamicsNAVSOAPServices.StaffAdvance.EditStaffTargetLines(staffTargetLines.No,
                    staffTargetLines.Doc_No, staffTargetLines.Objective, employeeNo,
                    staffTargetLines.Specific_Action_Plan,
                    staffTargetLines.Marks ?? decimal.Zero, staffTargetLines.Target_Amount ?? decimal.Zero, staffTargetLines.KPIDescription ??"");
                return Json(
                    saved
                        ? new {success = true, message = "Updated successfully"}
                        : new {success = false, message = "Error on Update"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false, message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifyMarksLine(Staff_Target_Lines staffTargetLines)
        {
            try
            {
                var saved = dynamicsNAVSOAPServices.StaffAdvance.EditStaffTargetMarksLines(
                    staffTargetLines.Doc_No,
                    staffTargetLines.Objective,
                    staffTargetLines.Marks ?? decimal.Zero,
                    staffTargetLines.StaffMarks ?? decimal.Zero,
                    staffTargetLines.SupervisorMarks ?? decimal.Zero,
                    staffTargetLines.Staff_Remark ?? "",
                    staffTargetLines.SupervisorComment ?? "",
                    staffTargetLines.Target_Amount ?? decimal.Zero,
                    staffTargetLines.KPIDescription ??""
                );

                return Json(
                    saved
                        ? new {success = true, message = "Updated successfully"}
                        : new {success = false, message = "Error on Update"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false, message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }


        //public ActionResult LoadMidYearlines(string DocumentNo)
        //{
        //    var linesList = _dcodataServices.BCOData.MidYear_Appraisal_Lines.Where(c => c.Doc_No == DocumentNo && c.Type == "Checkin Agenda").ToList();
        //    foreach (var midYearAppraisalLines in linesList)
        //    {
        //        midYearAppraisalLines.Success_Measure = dynamicsNAVSOAPServices.StaffAdvance.MidYearLineSuccess_Measure(midYearAppraisalLines.Doc_No, midYearAppraisalLines.Entry_No.ToString());
        //        midYearAppraisalLines.Items_Description = dynamicsNAVSOAPServices.StaffAdvance.MidYearLineDescription(midYearAppraisalLines.Doc_No, midYearAppraisalLines.Entry_No.ToString());
        //    }
        //    return Json(linesList, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult LoadMidYearConcernlines(string DocumentNo)
        {
            var linesList = _dcodataServices.BCOData.MidYear_Appraisal_Lines
                .Where(c => c.Doc_No == DocumentNo && c.Type == "Concerns").ToList();
            return Json(linesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadMidYearAgreedActionslines(string DocumentNo)
        {
            var linesList = _dcodataServices.BCOData.MidYear_Appraisal_Lines
                .Where(c => c.Doc_No == DocumentNo && c.Type == "Agreed Actions").ToList();
            return Json(linesList, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult DeleteMidYearLine(string entryNo)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.StaffAdvance.DeleteTMidYearLine(entryNo);
        //        return Json(new { success = true, message = "Saved successfully" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult DeleteTargetLine(string No, string DocNo)
        {
            try
            {
                dynamicsNAVSOAPServices.StaffAdvance.DeleteTargetLine(No, DocNo, employeeNo);
                return Json(new {success = true, message = "Saved successfully"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false, message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadStaffTargetlines(string No, string period)
        {
            var linesList = _dcodataServices.BCOData.Staff_Target_Lines
                .Where(c => c.Doc_No == No && c.Supervisor == employeeNo).ToList();
            /*if (!string.IsNullOrEmpty(period))
                linesList = linesList.Where(c => c.Period == period).ToList();*/
            foreach (var staffTargetLines in linesList)
            {
                try
                {
                    staffTargetLines.Specific_Action_Plan =
                        dynamicsNAVSOAPServices.StaffAdvance.StaffSettingLineSpecific_Action_Plan(
                            staffTargetLines.Doc_No, staffTargetLines.No);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                //try
                //{
                //    staffTargetLines.Success_Measure = dynamicsNAVSOAPServices.StaffAdvance.StaffSettingLineSuccess_Measure(staffTargetLines.Doc_No, staffTargetLines.No);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.Message);
                //}
                //if (staffTargetLines.Due_Date != null) staffTargetLines.Due_Date = Convert.ToDateTime(staffTargetLines.Due_Date.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
            }

            return Json(JsonConvert.SerializeObject(linesList, Formatting.Indented), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> TargetSettingPerformancePlanning(string no)
        {
            try
            {
                string employeeNo = "";
                string filename = "";
                //string filenane = "";
                employeeNo = AccountController.GetEmployeeNo();
                filename = "performance_planning" + no + "_" + employeeNo + ".pdf";

                filename = dynamicsNAVSOAPServices.payrollManagementWS.GeneratePerformancePlaningPortal(no, filename);
                if (filename.Equals(""))
                    return errorResponse.ApplicationExceptionError(new Exception(
                        "Unable to print the performance planning. " + ServiceConnection.contactICTDepartment + " "));
                using (var wc = new WebClient())
                {
                    var byteArr = await wc.DownloadDataTaskAsync(filename);
                    return File(byteArr, MimeMapping.GetMimeMapping(filename));
                }
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message.Replace("'", ""), success = false}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TargetSettingToSuperVisor(string no, string staff)
        {
            try
            {
                //var saved = dynamicsNAVSOAPServices.StaffAdvance.TargetSettingToSuperVisor(no, staff);
                //return Json(saved ? new { success = true, message = "Updated successfully" } : new { success = false, message = "Error on Update" }, JsonRequestBehavior.AllowGet);
                dynamicsNAVSOAPServices.StaffAdvance.TargetSettingToSuperVisor(no, staff);
                TempData["Success"] = "Sent To HR Successfully!";
                return RedirectToAction("TargetSetting", "TargetSettings");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }
        public ActionResult TargetSettingSupervisorApprove(string no)
        {
            try
            {
                //var saved = dynamicsNAVSOAPServices.StaffAdvance.TargetSettingToSuperVisor(no, staff);
                //return Json(saved ? new { success = true, message = "Updated successfully" } : new { success = false, message = "Error on Update" }, JsonRequestBehavior.AllowGet);
               var approved = dynamicsNAVSOAPServices.StaffAdvance.TargetSettingSupervisorApprove(no);
                //  return Json(approved ? new { success = true, message = "Approved successfully" } : new { success = false, message = "Error on Update" }, JsonRequestBehavior.AllowGet);
                TempData["Success"] = "Approved Successfully!";
                return RedirectToAction("TargetSetting", "TargetSettings");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }


        public ActionResult TargetSettingApprove(string no)
        {
            try
            {
                //var saved = dynamicsNAVSOAPServices.StaffAdvance.TargetSettingToSuperVisor(no, staff);
                //return Json(saved ? new { success = true, message = "Updated successfully" } : new { success = false, message = "Error on Update" }, JsonRequestBehavior.AllowGet);
                dynamicsNAVSOAPServices.StaffAdvance.TargetSettingApprove(no, employeeNo);
                TempData["Success"] = "Approved Successfully!";
                return RedirectToAction("TargetSetting", "TargetSettings");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult TargetSettingReject(string no)
        {
            try
            {
                //var saved = dynamicsNAVSOAPServices.StaffAdvance.TargetSettingToSuperVisor(no, staff);
                //return Json(saved ? new { success = true, message = "Updated successfully" } : new { success = false, message = "Error on Update" }, JsonRequestBehavior.AllowGet);
                dynamicsNAVSOAPServices.StaffAdvance.TargetSettingReject(no, employeeNo);
                TempData["Success"] = "Rejected Successfully!";
                return RedirectToAction("TargetSetting", "TargetSettings");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult TargetSettingAccept(string no)
        {
            try
            {
                dynamicsNAVSOAPServices.StaffAdvance.TargetSettingAccept(no);
                TempData["Success"] = "Accepted Successfully";
                return RedirectToAction("AppraiseeTargets", "TargetSettings");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult TargetSettingToEmpSupervisor(string no, string staff)
        {
            try
            {
                dynamicsNAVSOAPServices.StaffAdvance.TargetSettingToEmpSupervisor(no, staff);
                TempData["Success"] = "Accepted Successfully";
                return RedirectToAction("AppraiseeTargets", "TargetSettings");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult TargetSettingApprovetargerts(string no, string staff)
        {
            try
            {
                dynamicsNAVSOAPServices.StaffAdvance.TargetSettingApproveTargerts(no, staff);
                TempData["Success"] = "Successfully Approved";
                return RedirectToAction("TargetSetting", "TargetSettings");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }
        public ActionResult TargetSettingHRApprovetargerts(string no)
        {
            try
            {
                dynamicsNAVSOAPServices.StaffAdvance.TargetSettingHRApproveTargerts(no);
                TempData["Success"] = "Successfully Approved";
                return RedirectToAction("TargetSetting", "TargetSettings");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }
        public ActionResult TargetSettingHRRejecttargerts(string no)
        {
            try
            {
                dynamicsNAVSOAPServices.StaffAdvance.TargetSettingHRRejectTargerts(no);
                TempData["Success"] = "Successfully Rejected";
                return RedirectToAction("TargetSetting", "TargetSettings");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult TargetSettingRejecttargerts(string no, string staff)
        {
            try
            {
                dynamicsNAVSOAPServices.StaffAdvance.TargetSettingRejectTargerts(no, staff);
                TempData["Success"] = "Successfully Rejected";
                return RedirectToAction("TargetSetting", "TargetSettings");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public async Task<ActionResult> MidYearCheckinForm(string no)
        {
            try
            {
                string employeeNo = "";
                string filename = "";
                // string filenane = "";
                employeeNo = AccountController.GetEmployeeNo();
                filename = "MidYearCheckinForm" + no + "_" + employeeNo + ".pdf";

                filename = dynamicsNAVSOAPServices.payrollManagementWS.GenerateMidYearCheckinFormPortal(no, filename);
                if (filename.Equals(""))
                    return errorResponse.ApplicationExceptionError(new Exception(
                        "Unable to print the MidYearCheckinForm. " + ServiceConnection.contactICTDepartment + " "));
                using (var wc = new WebClient())
                {
                    var byteArr = await wc.DownloadDataTaskAsync(filename);
                    return File(byteArr, MimeMapping.GetMimeMapping(filename));
                }
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message.Replace("'", ""), success = false}, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult MidYearToSuperVisor(string no)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.StaffAdvance.MidYearToSuperVisor(no);
        //        TempData["Success"] = "Sent To Supervisor Successfully";
        //        return RedirectToAction("MidYearAppraisals", "TargetSettings");
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}
        //public ActionResult ProbationToSuperVisor(string no)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.StaffAdvance.MidYearToSuperVisor(no);
        //        TempData["Success"] = "Sent To Supervisor Successfully";
        //        return RedirectToAction("ProbationAppraisals", "TargetSettings");
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        //public ActionResult MidYearReject(string no, string rejectionComments)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.StaffAdvance.MidYearReject(no, rejectionComments ?? "");
        //        TempData["Success"] = "rejected Successfully";
        //        return RedirectToAction("SupervisorMidYearAppraisals", "TargetSettings");
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        //public ActionResult MidYearApprove(string no)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.StaffAdvance.MidYearApprove(no);
        //        TempData["Success"] = "Successfully";
        //        return RedirectToAction("SupervisorMidYearAppraisals", "TargetSettings");
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        //public ActionResult PeerAppraisalSendToReview(string no)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.StaffAdvance.SendForReviewPeerAppraisalHeader(no);
        //        return RedirectToAction("PeerAppraisals", "TargetSettings");
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        public ActionResult _AppraisalLines(string no, bool editable = true, bool isSupervisor = false)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            ViewBag.isSupervisor = isSupervisor;
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines
                .Where(c => c.Doc_No == no && c.Staff_No == employeeNo && c.Type == "Objectives").ToList();
            return PartialView(linesList);
        }

        public ActionResult _AppraisalLineManagementReviews(string no, bool editable = true, bool isSupervisor = false)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            ViewBag.isSupervisor = isSupervisor;
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines.Where(c =>
                c.Doc_No == no && c.Staff_No == employeeNo && c.Type == "Management Leadership").ToList();
            return PartialView(linesList);
        }

        public ActionResult _AppraisalLineJobKnowledgeReviews(string no, bool editable = true,
            bool isSupervisor = false)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            ViewBag.isSupervisor = isSupervisor;
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines
                .Where(c => c.Doc_No == no && c.Staff_No == employeeNo && c.Type == "Job Knowledge").ToList();
            return PartialView(linesList);
        }

        public ActionResult _AppraisalLineProblemSolvings(string no, bool editable = true, bool isSupervisor = false)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            ViewBag.isSupervisor = isSupervisor;
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines
                .Where(c => c.Doc_No == no && c.Staff_No == employeeNo && c.Type == "Problem Solving").ToList();
            return PartialView(linesList);
        }

        public ActionResult _AppraisalLineCommunicationAndTeamWork(string no, bool editable = true,
            bool isSupervisor = false)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            ViewBag.isSupervisor = isSupervisor;
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines.Where(c =>
                c.Doc_No == no && c.Staff_No == employeeNo && c.Type == "Communication and Teamwork").ToList();
            return PartialView(linesList);
        }

        public ActionResult _AppraisalLineLearningGoals(string no, bool editable = true, bool isSupervisor = false)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            ViewBag.isSupervisor = isSupervisor;
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines
                .Where(c => c.Doc_No == no && c.Staff_No == employeeNo && c.Type == "Learning Goals").ToList();
            return PartialView(linesList);
        }

        public ActionResult _AppraisalLineEmployeeComments(string no, bool editable = true, bool isSupervisor = false)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            ViewBag.isSupervisor = isSupervisor;
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines.Where(c =>
                c.Doc_No == no && c.Staff_No == employeeNo && c.Type == "Employee Comments").ToList();
            return PartialView(linesList);
        }

        public ActionResult _AppraisalLineSuperVisorComments(string no, bool editable = true, bool isSupervisor = false)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            ViewBag.isSupervisor = isSupervisor;
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines.Where(c =>
                c.Doc_No == no && c.Staff_No == employeeNo && c.Type == "Supervisor Comments").ToList();
            return PartialView(linesList);
        }

        //public ActionResult ModifyAppraisalLine(Staff_Appraisal_Lines appraisalLines, string Command)
        //{
        //    try
        //    {
        //        //if (Command.Equals("create", StringComparison.CurrentCultureIgnoreCase))
        //        //{
        //        //    var entryNo = dynamicsNAVSOAPServices.StaffAdvance.NewStaffAppraisalLine(appraisalLines.Doc_No, employeeNo,
        //        //        Convert.ToInt32(appraisalLines.Type), appraisalLines.Objective_Code ?? "", appraisalLines.Objective ?? "", appraisalLines.Success_Measure ?? "",
        //        //        appraisalLines.Due_Date ?? Date.Now, appraisalLines.Remark_Id ?? 0, appraisalLines.Remarks ?? "", (int)(appraisalLines.Rate ?? 0), appraisalLines.Supervisor_Remarks ?? "",
        //        //        (int)(appraisalLines.Supervisor_Rate ?? 0), (int)(appraisalLines.Section_Rating ?? 0), appraisalLines.Appraisee_Remarks ?? "", appraisalLines.Employees_Remarks ?? "");
        //        //    return Json(new { success = true, message = "Saved successfully", No = entryNo }, JsonRequestBehavior.AllowGet);
        //        //}

        //        //dynamicsNAVSOAPServices.StaffAdvance.EditStaffAppraisalLine(appraisalLines.Doc_No, employeeNo,
        //        //    Convert.ToInt32(appraisalLines.Type), appraisalLines.Entry_No, appraisalLines.Objective_Code ?? "", appraisalLines.Objective ?? "", appraisalLines.Success_Measure ?? "",
        //        //    appraisalLines.Due_Date ?? Date.Now, appraisalLines.Remark_Id ?? 0, appraisalLines.Remarks ?? "", (int)(appraisalLines.Rate ?? 0), appraisalLines.Supervisor_Remarks ?? "",
        //        //    (int)(appraisalLines.Supervisor_Rate ?? 0), (int)(appraisalLines.Section_Rating ?? 0), appraisalLines.Appraisee_Remarks ?? "", appraisalLines.Employees_Remarks ?? "");
        //        //return Json(new { success = true, message = "Saved successfully", No = appraisalLines.Entry_No }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public ActionResult ModifyPeerAppraisalLine(Peer_Appraisal_Lines peerAppraisalLine, string Command)
        //{
        //    try
        //    {
        //        if (Command.Equals("create", StringComparison.CurrentCultureIgnoreCase))
        //        {
        //            var entryNo = dynamicsNAVSOAPServices.StaffAdvance.NewPeerAppraisalLine(peerAppraisalLine.Doc_No, employeeNo,
        //                Convert.ToInt32(peerAppraisalLine.Type), peerAppraisalLine.Remarks);
        //            return Json(new { success = true, message = "Saved successfully", No = entryNo }, JsonRequestBehavior.AllowGet);
        //        }

        //        dynamicsNAVSOAPServices.StaffAdvance.EditPeerAppraisalLine((int)peerAppraisalLine.Entry_No, peerAppraisalLine.Remarks);
        //        return Json(new { success = true, message = "Saved successfully", No = peerAppraisalLine.Entry_No }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult DeleteAppraisalLine(int no, string DocNo)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.StaffAdvance.DeleteStaffAppraisalLine(no, DocNo);
        //        return Json(new { success = true, message = "Saved successfully" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult LoadAppraisalLines(string No)
        {
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines
                .Where(c => c.Doc_No == No && c.Staff_No == employeeNo && c.Type == "Objectives").ToList();
            return Json(JsonConvert.SerializeObject(linesList, Formatting.Indented), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadSuperVisorCommentlines(string No)
        {
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines.Where(c =>
                c.Doc_No == No && c.Staff_No == employeeNo && c.Type == "Supervisor Comments").ToList();
            return Json(linesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadProblemSolvingslines(string No)
        {
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines
                .Where(c => c.Doc_No == No && c.Staff_No == employeeNo && c.Type == "Problem Solving").ToList();
            return Json(linesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadManagementReviewsLines(string No)
        {
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines.Where(c =>
                c.Doc_No == No && c.Staff_No == employeeNo && c.Type == "Management Leadership").ToList();
            return Json(linesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadLearningGoalsLines(string No)
        {
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines
                .Where(c => c.Doc_No == No && c.Staff_No == employeeNo && c.Type == "Learning Goals").ToList();
            return Json(linesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadEmployeeCommentsLines(string No)
        {
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines.Where(c =>
                c.Doc_No == No && c.Staff_No == employeeNo && c.Type == "Employee Comments").ToList();
            return Json(linesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadCommunicationAndTeamWorkLines(string No)
        {
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines.Where(c =>
                c.Doc_No == No && c.Staff_No == employeeNo && c.Type == "Communication and Teamwork").ToList();
            return Json(linesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadJobKnowledgeReviewLines(string No)
        {
            var linesList = _dcodataServices.BCOData.Staff_Appraisal_Lines
                .Where(c => c.Doc_No == No && c.Staff_No == employeeNo && c.Type == "Job Knowledge").ToList();
            return Json(linesList, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AppraisalReport(string no)
        {
            try
            {
                string employeeNo = "";
                string filename = "";
                //string filenane = "";
                employeeNo = AccountController.GetEmployeeNo();
                filename = "AppraisalReport" + no + "_" + employeeNo + ".pdf";

                filename = dynamicsNAVSOAPServices.payrollManagementWS.GenerateAppraisalReportPortal(no, filename);
                if (filename.Equals(""))
                    return errorResponse.ApplicationExceptionError(new Exception(
                        "Unable to print the AppraisalReport. " + ServiceConnection.contactICTDepartment + " "));
                using (var wc = new WebClient())
                {
                    var byteArr = await wc.DownloadDataTaskAsync(filename);
                    return File(byteArr, MimeMapping.GetMimeMapping(filename));
                }
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message.Replace("'", ""), success = false}, JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult AppraisalSendToSupervisor(string no)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.StaffAdvance.AppraisalSendToSupervisor(no);
        //        TempData["Success"] = "Success";
        //        return RedirectToAction("Appraisals");
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["Error"] = e.Message;
        //        return RedirectToAction("ViewAppraisal", new {no});
        //    }
        //}

        public ActionResult EmployeePeerAppraisal()
        {
            var appraisalHeaders = _dcodataServices.BCOData.Peer_Appraisal_Header.Execute().Where(c =>
                c.Peer_Appraiser_1 == employeeNo ||
                c.Peer_Appraiser_2 == employeeNo || c.Peer_Appraiser_3 == employeeNo).ToList();
            return View(appraisalHeaders);
        }

        public ActionResult ViewEmployeePeerAppraisal(string no, bool ediatable = true)
        {
            ViewBag.ediatable = ediatable;
            var peerAppraisalHeader =
                _dcodataServices.BCOData.Peer_Appraisal_Header.Execute().FirstOrDefault(c => c.No == no);
            return View(peerAppraisalHeader);
        }
        //public ActionResult SendBackToSupervisorEmployeePeerAppraisal(string no)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.StaffAdvance.SendBackToSupervisorEmployeePeerAppraisal(no);
        //        TempData["Success"] = "Submitted Successfully";
        //        return RedirectToAction("EmployeePeerAppraisal");
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["Error"] = e.Message;
        //        return RedirectToAction("ViewEmployeePeerAppraisal");
        //    }
        //}

        public ActionResult _PeerAppraisalManagmentReview(string no, bool editable = true)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            var peerAppraisalLinesList = _dcodataServices.BCOData.Peer_Appraisal_Lines.Execute().Where(c =>
                c.Doc_No == no
                && c.Type == "Management Leadership").ToList();
            foreach (var peerAppraisalLines in peerAppraisalLinesList.Where(peerAppraisalLines =>
                         peerAppraisalLines.Entry_No != null))
            {
                //peerAppraisalLines.Remarks = dynamicsNAVSOAPServices.StaffAdvance.PeerAppraisalLineRemarks((int)peerAppraisalLines.Entry_No, peerAppraisalLines.Doc_No);
            }

            return PartialView(peerAppraisalLinesList);
        }

        public ActionResult _PeerAppraisalAknowLedgementReview(string no, bool editable = true)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            var peerAppraisalLinesList = _dcodataServices.BCOData.Peer_Appraisal_Lines.Execute().Where(c =>
                c.Doc_No == no
                && c.Type == "Job Knowledge").ToList();
            foreach (var peerAppraisalLines in peerAppraisalLinesList.Where(peerAppraisalLines =>
                         peerAppraisalLines.Entry_No != null))
            {
                //peerAppraisalLines.Remarks = dynamicsNAVSOAPServices.StaffAdvance.PeerAppraisalLineRemarks((int)peerAppraisalLines.Entry_No, peerAppraisalLines.Doc_No);
            }

            return PartialView(peerAppraisalLinesList);
        }

        public ActionResult _PeerAppraisalProblemSolvingReview(string no, bool editable = true)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            var peerAppraisalLinesList = _dcodataServices.BCOData.Peer_Appraisal_Lines.Execute().Where(c =>
                c.Doc_No == no
                && c.Type == "Problem Solving").ToList();
            foreach (var peerAppraisalLines in peerAppraisalLinesList.Where(peerAppraisalLines =>
                         peerAppraisalLines.Entry_No != null))
            {
                //peerAppraisalLines.Remarks = dynamicsNAVSOAPServices.StaffAdvance.PeerAppraisalLineRemarks((int)peerAppraisalLines.Entry_No, peerAppraisalLines.Doc_No);
            }

            return PartialView(peerAppraisalLinesList);
        }

        public ActionResult _PeerAppraisalCommunicationReview(string no, bool editable = true)
        {
            ViewBag.no = no;
            ViewBag.editable = editable;
            var peerAppraisalLinesList = _dcodataServices.BCOData.Peer_Appraisal_Lines.Execute().Where(c =>
                c.Doc_No == no
                && c.Type == "Communication and Teamwork").ToList();
            foreach (var peerAppraisalLines in peerAppraisalLinesList.Where(peerAppraisalLines =>
                         peerAppraisalLines.Entry_No != null))
            {
                //peerAppraisalLines.Remarks = dynamicsNAVSOAPServices.StaffAdvance.PeerAppraisalLineRemarks((int)peerAppraisalLines.Entry_No, peerAppraisalLines.Doc_No);
            }

            return PartialView(peerAppraisalLinesList);
        }

        public ActionResult LoadPeerAppraisalLines(string DocumentNo, string type)
        {
            ViewBag.no = DocumentNo;
            var peerAppraisalLinesList = _dcodataServices.BCOData.Peer_Appraisal_Lines.Execute().Where(c =>
                c.Doc_No == DocumentNo
                && c.Type.Equals(type, StringComparison.CurrentCultureIgnoreCase)).ToList();
            foreach (var peerAppraisalLines in peerAppraisalLinesList.Where(peerAppraisalLines =>
                         peerAppraisalLines.Entry_No != null))
            {
                //peerAppraisalLines.Remarks = dynamicsNAVSOAPServices.StaffAdvance.PeerAppraisalLineRemarks((int)peerAppraisalLines.Entry_No, peerAppraisalLines.Doc_No);
            }

            return Json(peerAppraisalLinesList, JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> LoadManagerEmployees(string selectedEmpManager)
        {
            var empManagers = new List<SelectListItem>();

            dynamic fundsTransactionCodes =
                JsonConvert.DeserializeObject(dynamicsNAVSOAPServices.StaffAdvance.GetManagerEmployees(employeeNo));
            foreach (var fundsTransactionCode in fundsTransactionCodes)
            {
                var item = new SelectListItem
                {
                    Value = fundsTransactionCode.No.ToString(),
                    Text = fundsTransactionCode.Name,
                    Selected = fundsTransactionCode.No.ToString() == selectedEmpManager
                };

                empManagers.Add(item);
            }

            return empManagers;
        }

        //public ActionResult DeletePeerAppraisalLine(int no, string DocNo)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.StaffAdvance.DeletePeerAppraisalLine(no, DocNo);
        //        return Json(new { success = true, message = "Saved successfully" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult TargetSettingLinesTemplateExport()
        {
            try
            {
                using (var package = new XLWorkbook())
                {
                    var worksheet = package.Worksheets.Add("KPIs Import Template");

                    // Set the header row
                    worksheet.Cell(1, 1).Value = "What";
                    worksheet.Cell(1, 2).Value = "How";


                    // Format the header row
                    var range = worksheet.Range("A1:B1");
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = XLFillPatternValues.Solid;
                    range.Style.Fill.BackgroundColor = XLColor.Gray;

                    // Autofit the columns
                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        package.SaveAs(stream);
                        var content = stream.ToArray();
                        TempData["Success"] = "Template was successfully downloaded.";
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "KPIs Import Template.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        public ActionResult ImportLines(TargetImportViewModel model)
        {
            if (model.UploadedFile != null && model.UploadedFile.ContentLength > 0)
            {
                try
                {
                    string path = Server.MapPath("~/UploadedFiles/");
                    string filename = Path.GetFileName(model.UploadedFile.FileName);
                    string fullPath = Path.Combine(path, filename);
                    var LineEntry = "";

                    // Ensure the directory exists
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    model.UploadedFile.SaveAs(fullPath);

                    // Read the Excel file
                    using (var workbook = new XLWorkbook(fullPath))
                    {
                        var worksheet = workbook.Worksheet(1); // Assuming you want to read the first worksheet
                        var rows = worksheet.RangeUsed().RowsUsed();
                        var cnt = 0;
                        foreach (var row in rows.Skip(1))
                        {
                            // Read data from each cell in the row
                            var what = row.Cell(1).GetValue<string>();
                            var how = row.Cell(2).GetValue<string>();
                            
                            LineEntry = dynamicsNAVSOAPServices.StaffAdvance.NewStaffTargetLines(
                                what, employeeNo,
                                how, decimal.Zero, model.Doc_No, decimal.Zero,model.KPIdesc ??"");
                            cnt += 1;
                        }
                    }

                    // Optionally, delete the uploaded file after processing
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    ViewBag.Message = "File uploaded successfully";
                    TempData["Success"] = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    return errorResponse.ApplicationExceptionError(ex);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select a file to upload.");
            }

            return RedirectToAction("ViewTargetSettings", new {no=model.Doc_No});
        }
    }
}