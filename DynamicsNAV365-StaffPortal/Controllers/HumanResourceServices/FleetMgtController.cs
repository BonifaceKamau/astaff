using DynamicsNAV365_StaffPortal.Models.HumanResource.FleetMgt;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Threading.Tasks;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models;
using Newtonsoft.Json;
using OdataRef;
using DimensionValues = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.DimensionValues;
using Employees = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.Employees;
using MVFixedAssets = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.MVFixedAssets;

namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    [Authorize]
    public class FleetMgtController : Controller
    {
        static string companyName = ServiceConnection.CompanyName;
        static string companyURL = "";

        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        BCODATAServices _dcodataServices = new BCODATAServices(companyURL);
        BCODATAV4Services _dcodataV4Services = new BCODATAV4Services(companyURL);

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

        public FleetMgtController()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        // GET: FleetMgt
        public ActionResult Index()
        {
            AccountController accountController = new AccountController();
            string employeeNo = "";

            employeeNo = AccountController.GetEmployeeNo();

            List<FleetHeaderModel> FleetLogsHeader = new List<FleetHeaderModel>();
            string pageData = "FleetLogHeader?$filter=Employee_No eq '" + employeeNo + "' &$format=json";

            HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(pageData);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    FleetHeaderModel DList1 = new FleetHeaderModel();
                    DList1.No = (string) config1["No"];
                    DList1.EmpName = (string) config1["Employee_Name"];
                    DList1.EmpNo = employeeNo;
                    DList1.Description = (string) config1["Description"];
                    DList1.Status = (string) config1["Status"];
                    DList1.CreatedOn = (string) config1["Created_On"];
                    FleetLogsHeader.Add(DList1);
                }
            }

            return View("~/Views/FleetMgt/FleetMgtList.cshtml", FleetLogsHeader);
        }

        public ActionResult MaintainanceAndRepair()
        {
            var maintananceAndRepairs = _dcodataServices.BCOData.Maintanance_and_repair
                .Where(c => c.Employee_No == employeeNo).ToList();
            return View(maintananceAndRepairs);
        }

        public ActionResult ViewFleetMgt(string DocNo)
        {
            AccountController accountController = new AccountController();
            string employeeNo = "";

            employeeNo = AccountController.GetEmployeeNo();

            FleetHeaderModel FleetLogsHeader = new FleetHeaderModel();
            string pageData = "FleetLogHeader?$filter=No eq '" + DocNo + "' and Employee_No eq '" + employeeNo +
                              "'&$format=json";

            HttpWebResponse httpResponseDestForeign = Credentials.GetOdataData(pageData);
            using (var streamReader1 = new StreamReader(httpResponseDestForeign.GetResponseStream()))
            {
                var result1 = streamReader1.ReadToEnd();

                var details1 = JObject.Parse(result1);

                foreach (JObject config1 in details1["value"])
                {
                    FleetLogsHeader.No = (string) config1["No"];
                    FleetLogsHeader.EmpName = (string) config1["Employee_Name"];
                    FleetLogsHeader.EmpNo = employeeNo;
                    FleetLogsHeader.Description = (string) config1["Description"];
                    FleetLogsHeader.Status = (string) config1["Status"];
                    FleetLogsHeader.CreatedOn = (string) config1["Created_On"];
                }
            }


            return View(FleetLogsHeader);
        }

        public PartialViewResult LogsLineView(string DocNo)
        {
            AccountController accountController = new AccountController();
            string employeeNo = "";
            
            //var employeesList = _dcodataServices.BCOData.Employees.ToList();

            employeeNo = AccountController.GetEmployeeNo();
            List<FleetLinesModel> ImpLines = new List<FleetLinesModel>();
            string pageLine = "FleetLogLines?$filter=No eq '" + DocNo + "' and Driver_Employee_No eq '" + employeeNo +
                              "' &$format=json";
            HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
            using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    FleetLinesModel ImLine = new FleetLinesModel();
                    ImLine.No = (string) config["No"];
                    ImLine.DriverEmpNo = (string) config["Driver_Employee_No"];
                    ImLine.EntryNo = (string) config["Entry_No"];
                    ImLine.FixedAssetNo = (string) config["Motor_Vehicle_No"];
                    ImLine.VehicleNo = (string) config["Motor_Vehicle_Reg"];
                    ImLine.StaffNo = (string) config["Staff_No"];
                    ImLine.StaffName = (string) config["Staff_Name"];
                    /*var staffNoSplit = ImLine.StaffNo?.Split(',').Select(c=>employeesList.FirstOrDefault(v=>v.No == c.Trim())?.Full_Name);
                    if (staffNoSplit != null)
                        ImLine.StaffName = string.Join(",", staffNoSplit);*/
                    ImLine.Date = Convert.ToDateTime((string) config["Date"]).ToString("dd-MM-yy");
                    ImLine.From = (string) config["From"];
                    ImLine.To = (string) config["Too"];
                    ImLine.StartTime = (string) config["Start_Time"];
                    ImLine.EndTime = (string) config["End_Time"];
                    ImLine.OdometerStart = (string) config["Odometer_Start"];
                    ImLine.OdometerEnd = (string) config["Odometer_End"];
                    ImLine.DistanceCovered = (string) config["Project_Distance_Covered_KM"];
                    ImLine.Litres = (string) config["Liters"];
                    ImLine.Amount = (string) config["Amount"];
                    ImLine.Status = (string) config["Status"];
                    ImLine.Dimension1 = (string) config["Shortcut_Dimension_1_Code"];
                    ImLine.Dimension2 = (string) config["Shortcut_Dimension_2_Code"];
                    ImLine.Dimension3 = (string) config["ShortcutDimCode3"];
                    ImLine.Dimension4 = (string) config["ShortcutDimCode4"];
                    ImLine.Dimension5 = (string) config["ShortcutDimCode5"];
                    ImLine.Dimension6 = (string) config["ShortcutDimCode6"];
                    ImLine.Dimension7 = (string) config["ShortcutDimCode7"];
                    ImLine.Purpose = (string) config["Purpose"];

                    ImpLines.Add(ImLine);
                }
            }

            var lines = new FleetLinesList
            {
                ListofFleetLines = ImpLines
            };
            return PartialView("~/Views/FleetMgt/FleetMgtLine.cshtml", lines);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteLine(string LnNo, string DocNo)
        {
            try
            {
                int ln = Convert.ToInt32(LnNo);
                AccountController accountController = new AccountController();
                string employeeNo = "";

                employeeNo = AccountController.GetEmployeeNo();
                Credentials.ObjNav.DeleteFleetLogLine(DocNo, ln, employeeNo);
                return Json(new {message = "Entry deleted successfully", success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message.Replace("'", ""), success = false}, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitApprovalRequest(string DocNo)
        {
            try
            {
                Credentials.ObjNav.SubmitFleetApprovalRequest(DocNo);
                return Json(new {message = "Approval request successfull", success = true},
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message.Replace("'", ""), success = false}, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult AddLog(string EntryNo, string DocNo)
        {
            try
            {
                ListOfddlData programList = new ListOfddlData();

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

                List<Employees> substitutes = new List<Employees>();
                string substituteurl = "Employees?$format=json";

                HttpWebResponse httpsubstituteurl = Credentials.GetOdataData(substituteurl);
                using (var streamReader2 = new StreamReader(httpsubstituteurl.GetResponseStream()))
                {
                    var result2 = streamReader2.ReadToEnd();

                    var details2 = JObject.Parse(result2);

                    foreach (JObject config2 in details2["value"])
                    {
                        Employees EList = new Employees();
                        EList.No = (string) config2["No"];
                        EList.Full_Name = (string) config2["Full_Name"];
                        substitutes.Add(EList);
                    }
                }

                List<MVFixedAssets> motors = new List<MVFixedAssets>();
                string motorsurl = "MVFixedAssets?$format=json";

                HttpWebResponse httpmotorsurl = Credentials.GetOdataData(motorsurl);
                using (var streamReader3 = new StreamReader(httpmotorsurl.GetResponseStream()))
                {
                    var result3 = streamReader3.ReadToEnd();

                    var details3 = JObject.Parse(result3);

                    foreach (JObject config3 in details3["value"])
                    {
                        MVFixedAssets MList = new MVFixedAssets();
                        MList.No = (string) config3["No"];
                        MList.MVl_Reg_No = (string) config3["MVl_Reg_No"];
                        motors.Add(MList);
                    }
                }

                programList = new ListOfddlData
                {
                    ListofStaff = substitutes.Select(x =>
                        new SelectListItem()
                        {
                            Text = x.Full_Name,
                            Value = x.No
                        }).OrderBy(x => x.Text).ToList(),
                    ListofFixedAssets = motors.Select(x =>
                        new SelectListItem()
                        {
                            Text = x.MVl_Reg_No,
                            Value = x.No
                        }).OrderBy(x => x.Text).ToList(),
                };
                programList.Dimension1s = new SelectList(DimensionValues, "Code", "Name");
                programList.Dimension2s = new SelectList(Enumerable.Empty<SelectListItem>());
                programList.Dimension3s = new SelectList(Enumerable.Empty<SelectListItem>());
                programList.Dimension4s = new SelectList(Enumerable.Empty<SelectListItem>());
                programList.Dimension5s = new SelectList(Enumerable.Empty<SelectListItem>());
                programList.Dimension6s = new SelectList(Enumerable.Empty<SelectListItem>());
                programList.Dimension7s = new SelectList(Enumerable.Empty<SelectListItem>());

                return PartialView("~/Views/FleetMgt/_FleetMgtAddLog.cshtml", programList);
            }
            catch (Exception ex)
            {
                return PartialView("~/Views/FleetMgt/_FleetMgtAddLog.cshtml", ex.Message);
            }
        }

        [Authorize]
        public JsonResult SubmitLogs(FleetLinesModel FleetLogs)
        {
            try
            {
                bool purchaseRequisitionLineCreated = false;
                AccountController accountController = new AccountController();
                string employeeNo = "";

                employeeNo = AccountController.GetEmployeeNo();
                DateTime startTime = DateTime.ParseExact(FleetLogs.StartTime, "HH:mm",
                    CultureInfo.InvariantCulture);
                DateTime EndTime = DateTime.ParseExact(FleetLogs.EndTime, "HH:mm",
                    CultureInfo.InvariantCulture);

                if (string.IsNullOrEmpty(FleetLogs.EntryNo) || FleetLogs.EntryNo =="0")
                {
                    purchaseRequisitionLineCreated = Credentials.ObjNav.InsertFleeLogLine(employeeNo, FleetLogs.No,
                        FleetLogs.StaffNo??"", FleetLogs.VehicleNo, startTime, EndTime, FleetLogs.From, FleetLogs.To,
                        Convert.ToDecimal(FleetLogs.OdometerStart), Convert.ToDecimal(FleetLogs.OdometerEnd),
                        FleetLogs.Dimension1 ?? "", FleetLogs.Dimension2 ?? "", FleetLogs.Dimension3 ?? "",
                        FleetLogs.Dimension4 ?? "",
                        FleetLogs.Dimension5 ?? "", FleetLogs.Dimension6 ?? "", FleetLogs.Dimension7 ?? "",
                        Convert.ToDecimal(FleetLogs.Litres),
                        Convert.ToDecimal(FleetLogs.Amount), DateTime.Parse(FleetLogs.Date), FleetLogs.Purpose??"");
                }
                else
                {
                    purchaseRequisitionLineCreated = Credentials.ObjNav.EditFleeLogLine(Convert.ToInt32(FleetLogs.EntryNo),employeeNo, FleetLogs.No,
                        FleetLogs.StaffNo??"", FleetLogs.VehicleNo, startTime, EndTime, FleetLogs.From, FleetLogs.To,
                        Convert.ToDecimal(FleetLogs.OdometerStart), Convert.ToDecimal(FleetLogs.OdometerEnd),
                        FleetLogs.Dimension1 ?? "", FleetLogs.Dimension2 ?? "", FleetLogs.Dimension3 ?? "",
                        FleetLogs.Dimension4 ?? "",
                        FleetLogs.Dimension5 ?? "", FleetLogs.Dimension6 ?? "", FleetLogs.Dimension7 ?? "",
                        Convert.ToDecimal(FleetLogs.Litres),
                        Convert.ToDecimal(FleetLogs.Amount), DateTime.Parse(FleetLogs.Date), FleetLogs.Purpose??"");
                }


                return Json(new {PurchaseRequisitionLineCreated = purchaseRequisitionLineCreated, success = true},
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message, success = false, view = false}, JsonRequestBehavior.AllowGet);
                ;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GenerateFleetReport(string DocNo)
        {
            try
            {
                string employeeNo = "";
                string filename = "";
                string filenane = "";
                employeeNo = AccountController.GetEmployeeNo();
                filename = "VehicleLog_" + DocNo + ".pdf";
                filenane = Credentials.ObjNav.GenerateFleetReport(filename, DocNo);
                return Json(new {message = "https://ess.cihebkenya.org/reports/" + filename, success = true},
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message.Replace("'", ""), success = false}, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult NewVehicleLogRecord(string DocNo)
        {
            try
            {
                string employeeNo = "";
                employeeNo = AccountController.GetEmployeeNo();

                Credentials.ObjNav.CreateNewFleetCard(employeeNo);
                return Json(new {message = "Success", success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {message = ex.Message.Replace("'", ""), success = false}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ViewMaintanaceAndRepair(string docno)
        {
            var maintananceAndRepairCard = _dcodataServices.BCOData.Maintanance_and_repair_Card.AsEnumerable()
                .FirstOrDefault(c => c.No == docno);
            var maintananceAndRepair  = JsonConvert.DeserializeObject<MaintananceAndRepair>(JsonConvert.SerializeObject(maintananceAndRepairCard));
            maintananceAndRepair.Service_Provider_Select = _dcodataServices.BCOData.ObjVendors.Select(c =>
                new SelectListItem()
                {
                    Value = c.No,
                    Text = $"{c.No}:{c.Name}",
                    Selected = maintananceAndRepair.Service_Provider == c.No
                });
            maintananceAndRepair.Service_Type_Select = _dcodataServices.BCOData.VehicleServiceTypes.Select(c =>
                new SelectListItem()
                {
                    Value = c.Service_Type,
                    Text = $"{c.Line_No}:{c.Service_Type}",
                    Selected = maintananceAndRepair.Service_Provider == c.Service_Type
                });
            maintananceAndRepair.Service_Interval_Type_Select = new Dictionary<int, string> { { 0, "" }, { 1, "Mileage" }, { 2, "Periodical" } }.Select(c =>
                new SelectListItem()
                {
                    Value = c.Key.ToString(),
                    Text = $"{c.Value}",
                    Selected = maintananceAndRepair.Service_Interval_Type == c.Value
                });
            maintananceAndRepair.Reg_No_Select = _dcodataServices.BCOData.MVFixedAssets.Execute().Select(c =>
                new SelectListItem()
                {
                    Value = c.No,
                    Text = $"{c.MVl_Reg_No}",
                    Selected = maintananceAndRepair.Reg_No == c.No
                });
            return View(maintananceAndRepair);
        }

        [HttpPost]
        public ActionResult ViewMaintanaceAndRepair(Maintanance_and_repair_Card repairCard)
        {
            try
            {
                var maintananceAndRepairCard = _dcodataServices.BCOData.Maintanance_and_repair_Card.AsEnumerable()
                    .FirstOrDefault(c => c.No == repairCard.No);
                dynamicsNAVSOAPServices.FleetMgmt.UpdateVehicleRepairsHeader(repairCard.No,
                    repairCard.Description ?? "",
                    repairCard.Service_Provider ?? "", repairCard.Service_Type ?? "", repairCard.Reg_No ?? "",
                    repairCard.Service_Period ?? "", repairCard.Amount ?? Decimal.Zero,
                    repairCard.Date_of_Service ?? DateTime.MinValue,
                    repairCard.Current_Odometer_Reading ?? Decimal.Zero, repairCard.Service_Mileage ?? Decimal.Zero);
                var maintananceAndRepair  = JsonConvert.DeserializeObject<MaintananceAndRepair>(JsonConvert.SerializeObject(maintananceAndRepairCard));
                maintananceAndRepair.Service_Provider_Select = _dcodataServices.BCOData.ObjVendors.Select(c =>
                    new SelectListItem()
                    {
                        Value = c.No,
                        Text = $"{c.No}:{c.Name}",
                        Selected = maintananceAndRepair.Service_Provider == c.No
                    });
                maintananceAndRepair.Service_Type_Select = _dcodataServices.BCOData.VehicleServiceTypes.Select(c =>
                    new SelectListItem()
                    {
                        Value = c.Service_Type,
                        Text = $"{c.Line_No}:{c.Service_Type}",
                        Selected = maintananceAndRepair.Service_Provider == c.Service_Type
                    });
                maintananceAndRepair.Service_Interval_Type_Select = new Dictionary<int, string> { { 0, "" }, { 1, "Mileage" }, { 2, "Periodical" } }.Select(c =>
                    new SelectListItem()
                    {
                        Value = c.Key.ToString(),
                        Text = $"{c.Value}",
                        Selected = maintananceAndRepair.Service_Interval_Type == c.Value
                    });
                maintananceAndRepair.Reg_No_Select = _dcodataServices.BCOData.MVFixedAssets.Execute().Select(c =>
                    new SelectListItem()
                    {
                        Value = c.No,
                        Text = $"{c.MVl_Reg_No}",
                        Selected = maintananceAndRepair.Reg_No == c.No
                    });
                return View(maintananceAndRepair);
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult MaintainaceLineView(string DocNo, bool editable = true)
        {
            ViewBag.no = DocNo;
            ViewBag.editable = editable;
            var repairLinesList = _dcodataServices.BCOData.Maintanance_and_repair_CardService_and_Repair_Lines
                .Where(c => c.Header_No == DocNo).ToList();
            return PartialView(repairLinesList);
        }

        public ActionResult NewMaitainanceAndRepair()
        {
            try
            {
                var no = dynamicsNAVSOAPServices.FleetMgmt.CreateNewVehicleRepair(employeeNo);
                return RedirectToAction("ViewMaintanaceAndRepair", new {docno = no});
                /*var repairCard = _dcodataServices.BCOData.Maintanance_and_repair_Card.AsEnumerable()
                .FirstOrDefault(c => c.No == no);
            var maintananceAndRepair  = JsonConvert.DeserializeObject<MaintananceAndRepair>(JsonConvert.SerializeObject(repairCard));
            maintananceAndRepair.Service_Provider_Select = _dcodataServices.BCOData.ObjVendors.Select(c =>
                new SelectListItem()
                {
                    Value = c.No,
                    Text = $"{c.No}:{c.Name}",
                    Selected = maintananceAndRepair.Service_Provider == c.No
                });
            maintananceAndRepair.Service_Type_Select = _dcodataServices.BCOData.VehicleServiceTypes.Select(c =>
                new SelectListItem()
                {
                    Value = c.Service_Type,
                    Text = $"{c.Line_No}:{c.Service_Type}",
                    Selected = maintananceAndRepair.Service_Provider == c.Service_Type
                });
            maintananceAndRepair.Service_Interval_Type_Select = new Dictionary<int, string> { { 0, "" }, { 1, "Mileage" }, { 2, "Periodical" } }.Select(c =>
                new SelectListItem()
                {
                    Value = c.Key.ToString(),
                    Text = $"{c.Value}",
                    Selected = maintananceAndRepair.Service_Interval_Type == c.Value
                });
            return View(maintananceAndRepair);*/
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult DeleteMaintainaceLine(string LnNo, string DocNo)
        {
            try
            {
                dynamicsNAVSOAPServices.FleetMgmt.DeleteMaintainaceLine(LnNo, DocNo);
                return Json(new {successful = true, message = "Saved"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {successful = false, message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadMaintainancelines(string DocNo)
        {
            var repairLinesList = _dcodataServices.BCOData.Maintanance_and_repair_CardService_and_Repair_Lines
                .Where(c => c.Header_No == DocNo).ToList();
            return Json(repairLinesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModifyMaintainanceLine(Maintanance_and_repair_CardService_and_Repair_Lines repairLines,
            string Command)
        {
            try
            {
                if (Command.Equals("create", StringComparison.CurrentCultureIgnoreCase) || repairLines.Line_No == 0)
                {
                    dynamicsNAVSOAPServices.FleetMgmt.VehicleRepairLine(repairLines.Header_No, repairLines.Description??"",
                        repairLines.Cost ?? decimal.Zero);
                }

                dynamicsNAVSOAPServices.FleetMgmt.EditVehicleRepairLine(repairLines.Header_No, repairLines.Line_No,
                    repairLines.Description??"", repairLines.Cost ?? decimal.Zero);
                return Json(new {success = true, message = "Saved successfully"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false, message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteMaintainanceLine(string No)
        {
            try
            {
                dynamicsNAVSOAPServices.FleetMgmt.DeleteMaintainanceLine(Convert.ToInt32(No));
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = true, message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RepairSendForApproval(string no)
        {
            try
            {
                dynamicsNAVSOAPServices.FleetMgmt.VehicleRepairsSendApproval(no);
                TempData["Success"] = "sent Successfully";
                return RedirectToAction("MaintainanceAndRepair");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult RepairCancelApproval(string no)
        {
            try
            {
                dynamicsNAVSOAPServices.FleetMgmt.VehicleRepairsCancelApproval(no);
                TempData["Success"] = "sent Successfully";
                return RedirectToAction("MaintainanceAndRepair");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult GetVehicleOdometer(string vehicleNo)
        {
            var odometer = dynamicsNAVSOAPServices.FleetMgmt.VehiclelastOdometer(vehicleNo);
            return Json(odometer,JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditLog(int entryNo, string DocNo)
        {
            ListOfddlData programList = new ListOfddlData();
            var logLine = _dcodataServices.BCOData.FleetLogLines.Execute().FirstOrDefault(c => c.Entry_No == entryNo);
            //var staffNoSplit = logLine?.Staff_No.Contains(",") == true? logLine?.Staff_No.Split(',') :null;
            var staffNoSplit = logLine?.Staff_No?.Split(',').ToList();
            /*programList.ListofStaff = _dcodataServices.BCOData.Employees.Execute().Select(c => new SelectListItem
            {
                Value = c.No,
                Text = c.Full_Name,
                Selected = staffNoSplit?.Any(v=>v.Trim().Equals(c.No,StringComparison.CurrentCultureIgnoreCase))== true
                           //c.Full_Name.Contains(logLine.Staff_Name) || c.No == logLine.Staff_Name
            }).ToList();*/
            programList.EntryNo = entryNo;
            programList.No = logLine?.No;
            programList.DocumentDate = logLine.Date.ToString();
            //programList.VehicleReg = logLine.Motor_Vehicle_Reg;
            programList.VehicleNo = logLine.Motor_Vehicle_No;
            programList.ListofFixedAssets = _dcodataServices.BCOData.MVFixedAssets.Execute().Select(c =>
                new SelectListItem
                {
                    Text = c.MVl_Reg_No,
                    Value = c.No,
                    Selected = c.No == logLine.Motor_Vehicle_No
                }).ToList();
            programList.From = logLine.From;
            programList.Too = logLine.Too;
            programList.StaffNo = logLine.Staff_No;
            programList.StartTime = Convert.ToDateTime(logLine.Start_Time);
            programList.EndTime = Convert.ToDateTime(logLine.End_Time);
            programList.OdometerStart = logLine.Odometer_Start.ToString();
            programList.OdometerEnd = logLine.Odometer_End.ToString();
            programList.Litres = logLine.Liters.ToString();
            programList.Amount = logLine.Amount.ToString();
            programList.Purpose =logLine.Purpose;
            programList.OdometerStart = logLine.Odometer_Start.ToString();
            var dimensionValuesList = _dcodataServices.BCOData.DimensionValues.Execute().ToList();
            programList.Dimension1s = dimensionValuesList.Where(c=>c.Global_Dimension_No==1).Select(c=> new SelectListItem
            {
                 Value= c.Code,
                 Text = $"{c.Code}:{c.Name}",
                Selected = logLine.Shortcut_Dimension_1_Code == c.Code
            });
            programList.Dimension2s = dimensionValuesList.Where(c=>c.Global_Dimension_No==2).Select(c=> new SelectListItem
            {
                Value= c.Code,
                Text = $"{c.Code}:{c.Name}",
                Selected = logLine.Shortcut_Dimension_2_Code == c.Code
            });
            programList.Dimension3s = dimensionValuesList.Where(c=>c.Global_Dimension_No==3).Select(c=> new SelectListItem
            {
                Value= c.Code,
                Text = $"{c.Code}:{c.Name}",
                Selected = logLine.ShortcutDimCode3 == c.Code
            });
            programList.Dimension4s = dimensionValuesList.Where(c=>c.Global_Dimension_No==4).Select(c=> new SelectListItem
            {
                Value= c.Code,
                Text = $"{c.Code}:{c.Name}",
                Selected = logLine.ShortcutDimCode4 == c.Code
            });
            programList.Dimension5s = dimensionValuesList.Where(c=>c.Global_Dimension_No==5).Select(c=> new SelectListItem
            {
                Value= c.Code,
                Text = $"{c.Code}:{c.Name}",
                Selected = logLine.ShortcutDimCode5 == c.Code
            });
            programList.Dimension6s = dimensionValuesList.Where(c=>c.Global_Dimension_No==6).Select(c=> new SelectListItem
            {
                Value= c.Code,
                Text = $"{c.Code}:{c.Name}",
                Selected = logLine.ShortcutDimCode6 == c.Code
            });
            programList.Dimension7s = dimensionValuesList.Where(c=>c.Global_Dimension_No==7).Select(c=> new SelectListItem
            {
                Value= c.Code,
                Text = $"{c.Code}:{c.Name}",
                Selected = logLine.ShortcutDimCode7 == c.Code
            });
            ViewBag.Edit = true;
            return PartialView("~/Views/FleetMgt/_FleetMgtAddLog.cshtml",programList);
        }

        public ActionResult GetVehicleLastToo(string vehicleNo)
        {
            var odometer = dynamicsNAVSOAPServices.FleetMgmt.VehicleLastToo(vehicleNo);
            return Json(odometer,JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateMaintainanceHeader(string DocNo, decimal amount)
        {
            try
            {
                var odometer = dynamicsNAVSOAPServices.FleetMgmt.UpdateMaintainanceHeaderAmount(DocNo, amount);
                return Json(new {success = true},JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false, message = e.Message},JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FleetLogreports()
        {
            return View();
        }

        public async Task<ActionResult> VehicleFleetlogRecordReport(string startDate, string endDate, string RegNumber, string LogNo, string EmpNo, string Program, string format)
        {
            try
            {
                var employeeNo = "";
                var filename = "";
                var filenane = "";
                employeeNo = AccountController.GetEmployeeNo();
                filename = "VehicleFleetlogRecordReport" + employeeNo + "_" + DateTime.Now.Day + "."+format;

                var startDateString = string.IsNullOrEmpty(startDate)
                    ? DateTime.MinValue.ToString("MM dd yy")
                    : Convert.ToDateTime(startDate).ToString("MM dd yy");
                
                var endDateString = string.IsNullOrEmpty(endDate)
                    ? DateTime.Now.ToString("MM dd yy")
                    : Convert.ToDateTime(endDate).ToString("MM dd yy");

                var base64String = dynamicsNAVSOAPServices.payrollManagementWS.VehicleFleetlogRecordReport(filename, startDateString, endDateString, LogNo??"",RegNumber??"", EmpNo??"",Program??"", format);
                //filename = dynamicsNAVSOAPServices.payrollManagementWS.VehicleFleetlogRecordReport(filename, startDateString, endDateString, LogNo??"",RegNumber??"", EmpNo??"",Program??"", format);
                /*if (filename.Equals("")) return errorResponse.ApplicationExceptionError(new Exception("Unable to print the VehicleFleetlogRecordReport. " + ServiceConnection.contactICTDepartment + " "));
                using (var wc = new WebClient())
                {
                    var byteArr = await wc.DownloadDataTaskAsync(filename);
                    return File(byteArr, MimeMapping.GetMimeMapping(filename));
                }*/
                var fileBytes = Convert.FromBase64String(base64String);
                return File(fileBytes, MimeMapping.GetMimeMapping(filename));

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        public async Task<ActionResult> VehicleFleetSummaryReport(string startDate, string endDate, string RegNumber, string LogNo, string EmpNo, string Program, string format)
        {
            try
            {
                var employeeNo = "";
                var filename = "";
                var filenane = "";
                employeeNo = AccountController.GetEmployeeNo();
                filename = "VehicleFleetSummaryReport" + employeeNo + "_" + DateTime.Now.Day + "."+format;
                
                var startDateString = string.IsNullOrEmpty(startDate)
                    ? DateTime.MinValue.ToString("MM dd yy")
                    : Convert.ToDateTime(startDate).ToString("MM dd yy");
                
                var endDateString = string.IsNullOrEmpty(endDate)
                    ? DateTime.Now.ToString("MM dd yy")
                    : Convert.ToDateTime(endDate).ToString("MM dd yy");

                filename = dynamicsNAVSOAPServices.payrollManagementWS.VehicleFleetSummaryReport(filename, startDateString, endDateString, LogNo??"",RegNumber??"", EmpNo??"",Program??"",format);
                if (filename.Equals("")) return errorResponse.ApplicationExceptionError(new Exception("Unable to print the VehicleFleetSummaryReport. " + ServiceConnection.contactICTDepartment + " "));
                using (var wc = new WebClient())
                {
                    var byteArr = await wc.DownloadDataTaskAsync(filename);
                    return File(byteArr, MimeMapping.GetMimeMapping(filename));
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
    }
}