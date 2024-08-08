using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Web.Mvc;
using AutoMapper;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models;
using DynamicsNAV365_StaffPortal.Models.Account;
using DynamicsNAV365_StaffPortal.Models.Finance;
using DynamicsNAV365_StaffPortal.Models.HumanResource.FleetMgt;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OdataRef;

namespace DynamicsNAV365_StaffPortal.Controllers.Administration
{
    public class AdministrationController : Controller
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

        public AdministrationController()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        public ActionResult AdministrationInfo()
        {
            return View();
        }

        public ActionResult _AdministrationSidebar()
        {
            var employeeProfileModel = new EmployeeProfileModel();
            employeeProfileModel.PassportAttached = false;
            return PartialView(employeeProfileModel);
        }

        //public ActionResult FixedAssetPool()
        //{
        //    var fixedAssetsPools = _dcodataServices.BCOData.Fixed_Assets_Pool.Execute();
        //    return View(fixedAssetsPools);
        //}

        //[HttpGet]
        //public ActionResult ViewFixedAssetPooling(string docno)
        //{
        //    var fixedAssetsPools =
        //        _dcodataServices.BCOData.Fixed_Asset_Card.Execute().FirstOrDefault(c => c.No == docno);
        //    var fixedAsset = JsonConvert.DeserializeObject<FixedAsset>(JsonConvert.SerializeObject(fixedAssetsPools));
        //    fixedAsset.FA_Class_Code_Select = _dcodataServices.BCOData.FA_Class.Select(c =>
        //        new SelectListItem
        //        {
        //            Value = c.Code,
        //            Text = $"{c.Code}:{c.Name}",
        //            Selected = fixedAsset.FA_Class_Code == c.Code
        //        });
        //    fixedAsset.FA_Subclass_Code_Select = _dcodataServices.BCOData.FA_Subclass.Select(c =>
        //        new SelectListItem
        //        {
        //            Value = c.Code,
        //            Text = $"{c.Code}:{c.Name}",
        //            Selected = fixedAsset.FA_Subclass_Code == c.Code
        //        });
        //    fixedAsset.FA_Location_Code_Select = _dcodataServices.BCOData.Location.Select(c =>
        //        new SelectListItem
        //        {
        //            Value = c.Code,
        //            Text = $"{c.Code}:{c.Name}",
        //            Selected = fixedAsset.FA_Location_Code == c.Code
        //        });
        //    fixedAsset.Global_Dimension_2_Code_Select = _dcodataServices.BCOData.DimensionValues
        //        .Where(c => c.Global_Dimension_No == 2).Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.Code,
        //                Text = $"{c.Code}:{c.Name}",
        //                Selected = fixedAsset.Global_Dimension_2_Code == c.Code
        //            });
        //    fixedAsset.Component_of_Main_Asset_Select = new Dictionary<int, string>
        //        {{0, ""}, {1, "Main Asset"}, {2, "Component"}}.Select(c =>
        //        new SelectListItem
        //        {
        //            Value = c.Key.ToString(),
        //            Text = $"{c.Value}",
        //            Selected = fixedAsset.Component_of_Main_Asset == c.Value
        //        });
        //    fixedAsset.Vendor_No_Select = _dcodataServices.BCOData.ObjVendors.Select(c =>
        //        new SelectListItem
        //        {
        //            Value = c.No,
        //            Text = $"{c.No}:{c.Name}",
        //            Selected = fixedAsset.Vendor_No == c.No
        //        });
        //    fixedAsset.Maintenance_Vendor_No_Select = _dcodataServices.BCOData.ObjVendors.Select(c =>
        //        new SelectListItem
        //        {
        //            Value = c.No,
        //            Text = $"{c.No}:{c.Name}",
        //            Selected = fixedAsset.Maintenance_Vendor_No == c.No
        //        });
        //    fixedAsset.Responsible_Employee_Select = _dcodataServices.BCOData.Employees.Select(c =>
        //        new SelectListItem
        //        {
        //            Value = c.No,
        //            Text = $"{c.No}:{c.Full_Name}",
        //            Selected = fixedAsset.Responsible_Employee == c.No
        //        });
        //    return View(fixedAsset);
        //}

        //[HttpPost]
        //public ActionResult ViewFixedAssetPooling(Fixed_Asset_Card assetCard)
        //{
        //    try
        //    {
        //        dynamicsNAVSOAPServices.FleetMgmt.EditFixedAsset(assetCard.No, assetCard.Description??"",
        //            assetCard.FA_Class_Code, assetCard.FA_Subclass_Code,assetCard.FA_Location_Code, assetCard.Global_Dimension_2_Code,
        //            assetCard.Budgeted_Asset??false,assetCard.Serial_No??"",assetCard.Asset_Tag_No??"",Convert.ToInt32(assetCard.Main_Asset_Component??"0"),assetCard.Inactive??true,
        //            assetCard.Blocked??true,assetCard.Acquired??true,assetCard.Vendor_No??"",assetCard.Maintenance_Vendor_No??"",assetCard.Under_Maintenance??false,
        //            assetCard.Next_Service_Date??DateTime.MinValue,assetCard.Warranty_Date??DateTime.MinValue,assetCard.Insured??false,assetCard.Responsible_Employee??"");
                
        //        /*var fixedAssetsPools = _dcodataServices.BCOData.Fixed_Asset_Card.Execute()
        //            .FirstOrDefault(c => c.No == assetCard.No);
        //        var fixedAsset =
        //            JsonConvert.DeserializeObject<FixedAsset>(JsonConvert.SerializeObject(fixedAssetsPools));
        //        fixedAsset.FA_Class_Code_Select = _dcodataServices.BCOData.FA_Class.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.Code,
        //                Text = $"{c.Code}:{c.Name}",
        //                Selected = fixedAsset.FA_Class_Code == c.Code
        //            });
        //        fixedAsset.FA_Subclass_Code_Select = _dcodataServices.BCOData.FA_Subclass.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.Code,
        //                Text = $"{c.Code}:{c.Name}",
        //                Selected = fixedAsset.FA_Subclass_Code == c.Code
        //            });
        //        fixedAsset.FA_Location_Code_Select = _dcodataServices.BCOData.Location.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.Code,
        //                Text = $"{c.Code}:{c.Name}",
        //                Selected = fixedAsset.FA_Location_Code == c.Code
        //            });
        //        fixedAsset.Global_Dimension_2_Code_Select = _dcodataServices.BCOData.DimensionValues
        //            .Where(c => c.Global_Dimension_No == 2).Select(c =>
        //                new SelectListItem
        //                {
        //                    Value = c.Code,
        //                    Text = $"{c.Code}:{c.Name}",
        //                    Selected = fixedAsset.Global_Dimension_2_Code == c.Code
        //                });
        //        fixedAsset.Component_of_Main_Asset_Select = new Dictionary<int, string>
        //            {{0, ""}, {1, "Main Asset"}, {2, "Component"}}.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.Key.ToString(),
        //                Text = $"{c.Value}",
        //                Selected = fixedAsset.Component_of_Main_Asset == c.Value
        //            });
        //        fixedAsset.Vendor_No_Select = _dcodataServices.BCOData.ObjVendors.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.No,
        //                Text = $"{c.No}:{c.Name}",
        //                Selected = fixedAsset.Vendor_No == c.No
        //            });
        //        fixedAsset.Maintenance_Vendor_No_Select = _dcodataServices.BCOData.ObjVendors.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.No,
        //                Text = $"{c.No}:{c.Name}",
        //                Selected = fixedAsset.Maintenance_Vendor_No == c.No
        //            });*/
        //        return RedirectToAction("ViewFixedAssetPooling", new {docno = assetCard.No});
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        //public ActionResult DepreciationBook(string docno)
        //{
        //    ViewBag.no = docno;
        //    var depreciationBook = _bcodatav4Services.BCOData.Fixed_Asset_CardDepreciationBook.Execute()
        //        .FirstOrDefault(c => c.FA_No == docno);
            
        //    var config = new MapperConfiguration(cfg =>
        //    {
        //        cfg.CreateMap<Fixed_Asset_CardDepreciationBook, FixedAssetDepreciationBook>().ReverseMap()
        //            .ForMember(dest => dest.Depreciation_Ending_Date, opt => opt.Ignore())
        //            .ForMember(dest => dest.Depreciation_Starting_Date, opt => opt.Ignore())
        //            .ForMember(dest => dest.Projected_Disposal_Date, opt => opt.Ignore())
        //            .ForMember(dest => dest.Temp_Ending_Date, opt => opt.Ignore())
        //            .ForMember(dest => dest.First_User_Defined_Depr_Date, opt => opt.Ignore());
        //            //.ForMember(dest => dest.Depreciation_Starting_Date, opt => opt.MapFrom(src => src.Depreciation_Starting_Date))
        //            //.ForMember(dest => dest.Depreciation_Ending_Date, opt => opt.MapFrom(src => src.Depreciation_Ending_Date));
        //    });
        //    var mapper = config.CreateMapper();
        //    /*try
        //    {
        //        var fixedAssetDepreciationBook2 = mapper.Map<FixedAssetDepreciationBook>(depreciationBook);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }*/
        //    var settings = new JsonSerializerSettings
        //    {
        //        //DateFormatString = "yyyy-MM-dd",
        //        //Converters = new List<JsonConverter> { new ODataDateConverter() }
        //    };
        //    //JsonConvert.DefaultSettings = () => settings;
        //    settings.Converters.Add(new ODataDateConverter());
            
        //    var serializeObject = JsonConvert.SerializeObject(depreciationBook,settings);
        //   // var ser = new DataContractJsonSerializer(typeof(Fixed_Asset_CardDepreciationBook));

        //    var fixedAssetDepreciationBook = JsonConvert.DeserializeObject<FixedAssetDepreciationBook>(serializeObject,settings);
        //    fixedAssetDepreciationBook = fixedAssetDepreciationBook ?? new FixedAssetDepreciationBook
        //    {
        //        FA_No = docno
        //    };
        //    fixedAssetDepreciationBook.Depreciation_Book_Code_Select =
        //        _dcodataServices.BCOData.Depreciation_Book.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.Code,
        //                Text = $"{c.Code}",
        //                Selected = fixedAssetDepreciationBook.Depreciation_Book_Code == c.Code
        //            });
        //    fixedAssetDepreciationBook.Depreciation_Table_Code_Select =
        //        _dcodataServices.BCOData.Depreciation_Table_Header.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.Code,
        //                Text = $"{c.Code}",
        //                Selected = fixedAssetDepreciationBook.Depreciation_Book_Code == c.Code
        //            });
        //    fixedAssetDepreciationBook.FA_Posting_Group_Select = _dcodataServices.BCOData.FA_Posting_Group.Select(c =>
        //        new SelectListItem
        //        {
        //            Value = c.Code,
        //            Text = $"{c.Code}",
        //            Selected = fixedAssetDepreciationBook.FA_Posting_Group == c.Code
        //        });
        //    fixedAssetDepreciationBook.Depreciation_Method_Select = new Dictionary<int, string>()
        //    {
        //        {0, "Straight-Line"},
        //        {1, "Declining-Balance 1"},
        //        {2, "Declining-Balance 2"},
        //        {3, "DB1/SL"},
        //        {4, "DB2/SL"},
        //        {5, "User-Defined"},
        //        {6, "Manual"}
        //    }.Select(c =>
        //        new SelectListItem
        //        {
        //            Value = c.Key.ToString(),
        //            Text = $"{c.Value}",
        //            Selected = fixedAssetDepreciationBook.Depreciation_Method == c.Value
        //        });
        //    return PartialView(fixedAssetDepreciationBook);
        //}

        //[HttpPost]
        //public ActionResult DepreciationBook(Fixed_Asset_CardDepreciationBook assetCardDepreciationBook)
        //{
        //    try
        //    {
        //        ViewBag.no = assetCardDepreciationBook.FA_No;
        //        dynamicsNAVSOAPServices.FleetMgmt.EditDepreciationBook(assetCardDepreciationBook.FA_No, assetCardDepreciationBook.FA_Posting_Group??"",
        //            Convert.ToInt32(assetCardDepreciationBook.Depreciation_Method??"0"),assetCardDepreciationBook.Depreciation_Starting_Date??DateTime.MinValue,assetCardDepreciationBook.No_of_Depreciation_Years??Decimal.Zero,
        //            assetCardDepreciationBook.Depreciation_Ending_Date??DateTime.MinValue,assetCardDepreciationBook.Depreciation_Table_Code??"",assetCardDepreciationBook.Use_Half_Year_Convention??false);
        //        /*var depreciationBook = _bcodatav4Services.BCOData.Fixed_Asset_CardDepreciationBook.Execute().FirstOrDefault(c => c.FA_No == assetCardDepreciationBook.FA_No);
        //        var fixedAssetDepreciationBook = JsonConvert.DeserializeObject<FixedAssetDepreciationBook>(JsonConvert.SerializeObject(depreciationBook));
        //        fixedAssetDepreciationBook = fixedAssetDepreciationBook ?? new FixedAssetDepreciationBook
        //        {
        //            FA_No = assetCardDepreciationBook.FA_No
        //        };
        //        fixedAssetDepreciationBook.Depreciation_Book_Code_Select = _dcodataServices.BCOData.Depreciation_Book.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.Code,
        //                Text = $"{c.Code}",
        //                Selected = fixedAssetDepreciationBook.Depreciation_Book_Code == c.Code
        //            });
        //        fixedAssetDepreciationBook.Depreciation_Table_Code_Select = _dcodataServices.BCOData.Depreciation_Table_Header.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.Code,
        //                Text = $"{c.Code}",
        //                Selected = fixedAssetDepreciationBook.Depreciation_Book_Code == c.Code
        //            });
        //        fixedAssetDepreciationBook.FA_Posting_Group_Select = _dcodataServices.BCOData.FA_Posting_Group.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.Code,
        //                Text = $"{c.Code}",
        //                Selected = fixedAssetDepreciationBook.FA_Posting_Group == c.Code
        //            });
        //        fixedAssetDepreciationBook.Depreciation_Method_Select = new Dictionary<int, string>()
        //        {
        //            { 0, "Straight-Line" },
        //            { 1, "Declining-Balance 1" },
        //            { 2, "Declining-Balance 2" },
        //            { 3, "DB1/SL" },
        //            { 4, "DB2/SL" },
        //            { 5, "User-Defined" },
        //            { 6, "Manual" }
        //        }.Select(c =>
        //            new SelectListItem
        //            {
        //                Value = c.Key.ToString(),
        //                Text = $"{c.Value}",
        //                Selected = fixedAssetDepreciationBook.Depreciation_Method == c.Value
        //            });*/
        //        return RedirectToAction("ViewFixedAssetPooling", new {docno = assetCardDepreciationBook.FA_No});
        //    }
        //    catch (Exception e)
        //    {
        //        return errorResponse.ApplicationExceptionError(e);
        //    }
        //}

        public ActionResult CopyFixedAsset(string faNo, int? noOfCopies, string firstFANo, bool? useFaNumberSeries)
        {
            try
            {
                dynamicsNAVSOAPServices.FleetMgmt.CopyFixedAsset(faNo, noOfCopies ?? 1, firstFANo,
                    useFaNumberSeries ?? true);
                return RedirectToAction("FixedAssetPool");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult AssetTransferIssued()
        {
            var asset = _dcodataServices.BCOData.Asset_Transfer_List_Issue.Execute();
            return View(asset);
        }

        public ActionResult AssetTransferReceived()
        {
            var asset = _dcodataServices.BCOData.Asset_Transfer_List_Received.Execute();
            return View(asset);
        }
        [HttpGet]
        public ActionResult ViewAssetTransfer(string docno)
        {
            var asset = _dcodataServices.BCOData.Asset_Transfer_Header.Execute().FirstOrDefault(c=>c.No == docno);
            var assetTransferHeader = JsonConvert.DeserializeObject<AssetTransferHeader>(JsonConvert.SerializeObject(asset));
            assetTransferHeader.Department_Select = _dcodataServices.BCOData.DimensionValues.Execute().Where(c=>c.Global_Dimension_No==1).Select(c => new SelectListItem()
            {
                Value= c.Code,
                Text = $"{c.Code}:{c.Name}",
                Selected = asset.Department == c.Code
            });
            return View(assetTransferHeader);
        }
        [HttpPost]
        public ActionResult ViewAssetTransfer(Asset_Transfer_Header_Receive transferHeader)
        {
            try
            {
                dynamicsNAVSOAPServices.FleetMgmt.EditAssetTransfer(transferHeader.No,
                    transferHeader.Effective_Transfer_Date?.Date ?? DateTime.MinValue, transferHeader.Department??"");
                return RedirectToAction("ViewAssetTransfer", new { docno=  transferHeader.No});
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult _AssetTransferLine(string no)
        {
            ViewBag.no = no;
            var transferLines = _dcodataServices.BCOData.Asset_Transfer_Header_IssueAsset_Transfer_Line.Execute()
                .Where(c => c.Transfer_No == no);
            return PartialView(transferLines);
        }

        public ActionResult NewAssetTransfer()
        {
            try
            {
                var no = dynamicsNAVSOAPServices.FleetMgmt.NewAssetTransfer(employeeNo);
                return RedirectToAction("ViewAssetTransfer", new {docno = no});
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult LoadAssetTransferLines(string no)
        {
            var transferLines = _dcodataServices.BCOData.Asset_Transfer_Header_IssueAsset_Transfer_Line.Execute()
                .Where(c => c.Transfer_No == no);
            return Json(transferLines, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModifyAssetTransferLine(Asset_Transfer_Header_ReceiveAsset_Transfer_Line transferLine)
        {
            try
            {
                if (transferLine.Line_No == 0)
                {
                    dynamicsNAVSOAPServices.FleetMgmt.NewAssetTransferLine(transferLine.Transfer_No,transferLine.Asset_No,
                        transferLine.Current_Holder_No ?? "", transferLine.Employee_No?? "", transferLine.Remark??"");
                    return Json(new { success = true, message = "Saved successfully"}, JsonRequestBehavior.AllowGet);
                }

                dynamicsNAVSOAPServices.FleetMgmt.EditAssetTransferLine(transferLine.Line_No,transferLine.Asset_No,
                    transferLine.Current_Holder_No ?? "", transferLine.Employee_No?? "", transferLine.Remark??"");
                return Json(new { success = true, message = "Saved successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteAssetTransferLine(int no)
        {
            try
            {
                dynamicsNAVSOAPServices.FleetMgmt.DeleteAssetTransferLine(no);
                return Json(new { success = true, message = "Saved successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult IssueAssetTransfer(string no)
        {
            try
            {
                dynamicsNAVSOAPServices.FleetMgmt.IssueAssetTransfer(no);
                return RedirectToAction("AssetTransferIssued");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult RecieveAssetTransfer(string no)
        {
            try
            {
                dynamicsNAVSOAPServices.FleetMgmt.RecieveAssetTransfer(no);
                return RedirectToAction("AssetTransferReceived");
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }

        public ActionResult NewFixedAssetsPool()
        {
            try
            {
                var no = dynamicsNAVSOAPServices.FleetMgmt.NewFixedAsset(employeeNo);
                return RedirectToAction("ViewFixedAssetPooling", new {docno = no});
            }
            catch (Exception e)
            {
                return errorResponse.ApplicationExceptionError(e);
            }
        }
    }
    public class ODataDateConverter : JsonConverter
    {
        public const string TypePropertyName = "type";
        private bool _dormant = false;
        
        /*public override bool CanConvert(Type objectType)
        {
            //return objectType == typeof(OdataRef.Fixed_Asset_Card);
            return true;
        }*/

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null || reader.TokenType != JsonToken.StartObject)
            {
                // Handle the null reader case
                // You can return a default value or throw an exception, depending on your requirements
                return null; // Or throw an exception
            }

            var jsonObject = JObject.Load(reader);
            ConvertValueToDateTime(jsonObject, "Depr_Ending_Date_Custom_1");
            ConvertValueToDateTime(jsonObject, "Depr_Starting_Date_Custom_1");
            ConvertValueToDateTime(jsonObject, "Depreciation_Ending_Date");
            ConvertValueToDateTime(jsonObject, "Depreciation_Starting_Date");
            ConvertValueToDateTime(jsonObject, "First_User_Defined_Depr_Date");
            ConvertValueToDateTime(jsonObject, "Projected_Disposal_Date");
            ConvertValueToDateTime(jsonObject, "Temp_Ending_Date");

            //var result = jsonObject.ToObject(objectType, serializer);
            return jsonObject.ToObject(objectType);
        }

        public void ConvertValueToDateTime(JObject jsonObject, string valueName)
        {
            var value = jsonObject.GetValue(valueName);

            if (value != null && value.Type == JTokenType.Object)
            {
                var year = value.Value<int>("Year");
                var month = value.Value<int>("Month");
                var day = value.Value<int>("Day");
                var dateTime = new DateTime(year, month, day);

                var newJsonObject = new JObject(jsonObject); // Create a new JObject with the same properties
                newJsonObject[valueName] = dateTime.Date; // Update the value in the new JObject

                // Replace the properties in the original JObject
                jsonObject.RemoveAll();
                foreach (var property in newJsonObject.Properties())
                {
                    jsonObject.Add(property.Name, property.Value);
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            /*var jObject = new JObject();

            foreach (var property in value.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(value);

                if (property.PropertyType == typeof(Date?) && propertyValue != null)
                {
                    var df = (Date?) propertyValue;
                    var dateTime = new  DateTime(df.Value.Year, df.Value.Month, df.Value.Day);
                    //var dateTimeObject = JToken.FromObject(dateTime, serializer);
                    jObject.Add(property.Name, dateTime);
                }
                else
                {
                    //jObject.Add(property.Name, property.GetValue(odataDate));
                    if (propertyValue != null) jObject.Add(property.Name, JToken.FromObject(propertyValue, serializer));
                }
            }

            jObject.WriteTo(writer);*/
            
            //
            _dormant = true;
            var t = JToken.FromObject(value, serializer);
            if (t.Type == JTokenType.Object /*t.GetType() == typeof(Date?)*/)
            {
                var o = (JObject)t;
                //o.AddFirst(new JProperty(TypePropertyName, value.GetType().Name));
                if (o.TryGetValue("Year", out var yearToken) &&
                    o.TryGetValue("Month", out var monthToken) &&
                    o.TryGetValue("Day", out var dayToken))
                {
                    if (int.TryParse(yearToken.ToString(), out var year) &&
                        int.TryParse(monthToken.ToString(), out var month) &&
                        int.TryParse(dayToken.ToString(), out var day))
                    {
                        var date = new DateTime(year, month, day);
                        o.Remove("Year");
                        o.Remove("Month");
                        o.Remove("Day");
                        o.AddFirst(new JProperty("Date", date));
                        o.WriteTo(writer);
                    }
                }
                else
                {
                    o.WriteTo(writer);
                }
            }
            else
            {
                t.WriteTo(writer);
            }
        }
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            if (_dormant)
            {
                _dormant = false;
                return false;
            }
            return true;
        }
    }
}