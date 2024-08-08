using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.Account;
using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using Microsoft.Graph;

namespace DynamicsNAV365_StaffPortal.Controllers.FinanceServices
{
    public class FinanceHomeController : Controller
    {
		static string companyName = ServiceConnection.CompanyName;
		static string companyURL = "";

		DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
		DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
		ErrorResponseController errorResponse = new ErrorResponseController();
		string employeeNo = AccountController.GetEmployeeNo();

		public FinanceHomeController()
		{

		}

		[Authorize]
		public ActionResult FinanceInfo()
		{
			return View();
		}

		#region Helper Views
		[Authorize]
		[ChildActionOnly]
		public ActionResult _FinanceSidebar()
		{
			var employeeProfileModel = new EmployeeProfileModel();
			employeeProfileModel.PassportAttached = false;
			return PartialView(employeeProfileModel);
		}

		[Authorize]
		[ChildActionOnly]
		public ActionResult _EmployeeBalanceWidget()
		{
			decimal imprestBalance = 0;
			imprestBalance = dynamicsNAVSOAPServices.fundsManagementWS.GetEmployeeImprestBalance(AccountController.GetEmployeeNo());
			var employeeBalancesModel = new EmployeeBalancesModel();
			employeeBalancesModel.Amount = imprestBalance;
			employeeBalancesModel.AmountStr = imprestBalance.ToString("n");
			return PartialView(employeeBalancesModel);
		}
        #endregion Helper Views

        #region Helper Functions
        public JsonResult GetGlobalDimension1Codes()
        {
            var dimensionValueList = new List<DimensionValueModel>();
            var dimensionValues = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
                                  where dimensionValuesQuery.Global_Dimension_No.Equals(1) && dimensionValuesQuery.Blocked.Equals(false)
                                  select dimensionValuesQuery;
            foreach (var dimensionValue in dimensionValues)
            {
                var dimensionValueObj = new DimensionValueModel();
                dimensionValueObj.DimensionCode = dimensionValue.Dimension_Code;
                dimensionValueObj.Code = dimensionValue.Code;
                dimensionValueObj.Name = dimensionValue.Name;
                dimensionValueObj.DimensionValueType = dimensionValue.Dimension_Value_Type;
                dimensionValueObj.Blocked = dimensionValue.Blocked ?? false;
                dimensionValueObj.GlobalDimensionNo = dimensionValue.Global_Dimension_No ?? 0;
                // dimensionValueObj.SequenceNo = dimensionValue.Sequence_No ?? 0;
                dimensionValueObj.GlobalDimension1Code = dimensionValue.Dimension_Code;
                dimensionValueList.Add(dimensionValueObj);
            }
            return Json(dimensionValues.ToList(), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetGlobalDimension2Codes(string GlobalDimension1Code)
        //{
        //	dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        //	List<DimensionValueModel> dimensionValueList = new List<DimensionValueModel>();
        //	var dimensionValues = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
        //						  where dimensionValuesQuery.Global_Dimension_No.Equals(2) && dimensionValuesQuery.Dimension_Code.Equals(GlobalDimension1Code) &&
        //								dimensionValuesQuery.Blocked.Equals(false)
        //						  select dimensionValuesQuery;
        //	foreach (DimensionValues dimensionValue in dimensionValues)
        //	{
        //		DimensionValueModel dimensionValueObj = new DimensionValueModel();
        //		dimensionValueObj.DimensionCode = dimensionValue.Dimension_Code;
        //		dimensionValueObj.Code = dimensionValue.Code;
        //		dimensionValueObj.Name = dimensionValue.Name;
        //		dimensionValueObj.DimensionValueType = dimensionValue.Dimension_Value_Type;
        //		dimensionValueObj.Blocked = dimensionValue.Blocked ?? false;
        //		dimensionValueObj.GlobalDimensionNo = dimensionValue.Global_Dimension_No ?? 0;
        //		//dimensionValueObj.SequenceNo = dimensionValue.Sequence_No ?? 0;
        //		dimensionValueObj.GlobalDimension1Code = dimensionValue.Global_Dimension_1_Code;
        //		dimensionValueList.Add(dimensionValueObj);
        //	}
        //	return Json(dimensionValueList, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetShortcutDimension3Codes(string GlobalDimension1Code)
        //{
        //	List<DimensionValueModel> dimensionValueList = new List<DimensionValueModel>();
        //	var dimensionValues = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
        //						  where dimensionValuesQuery.Global_Dimension_No.Equals(3) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) &&
        //								dimensionValuesQuery.Blocked.Equals(false)
        //						  select dimensionValuesQuery;
        //	foreach (DimensionValues dimensionValue in dimensionValues)
        //	{
        //		DimensionValueModel dimensionValueObj = new DimensionValueModel();
        //		dimensionValueObj.DimensionCode = dimensionValue.Dimension_Code;
        //		dimensionValueObj.Code = dimensionValue.Code;
        //		dimensionValueObj.Name = dimensionValue.Name;
        //		dimensionValueObj.DimensionValueType = dimensionValue.Dimension_Value_Type;
        //		dimensionValueObj.Blocked = dimensionValue.Blocked ?? false;
        //		dimensionValueObj.GlobalDimensionNo = dimensionValue.Global_Dimension_No ?? 0;
        //		// dimensionValueObj.SequenceNo = dimensionValue.Sequence_No ?? 0;
        //		dimensionValueObj.GlobalDimension1Code = dimensionValue.Global_Dimension_1_Code;
        //		dimensionValueList.Add(dimensionValueObj);
        //	}
        //	return Json(dimensionValueList, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetShortcutDimension4Codes(string GlobalDimension1Code)
        //{
        //	List<DimensionValueModel> dimensionValueList = new List<DimensionValueModel>();
        //	var dimensionValues = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
        //						  where dimensionValuesQuery.Global_Dimension_No.Equals(4) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) &&
        //								dimensionValuesQuery.Blocked.Equals(false)
        //						  select dimensionValuesQuery;
        //	foreach (DimensionValues dimensionValue in dimensionValues)
        //	{
        //		DimensionValueModel dimensionValueObj = new DimensionValueModel();
        //		dimensionValueObj.DimensionCode = dimensionValue.Dimension_Code;
        //		dimensionValueObj.Code = dimensionValue.Code;
        //		dimensionValueObj.Name = dimensionValue.Name;
        //		dimensionValueObj.DimensionValueType = dimensionValue.Dimension_Value_Type;
        //		dimensionValueObj.Blocked = dimensionValue.Blocked ?? false;
        //		dimensionValueObj.GlobalDimensionNo = dimensionValue.Global_Dimension_No ?? 0;
        //		// dimensionValueObj.SequenceNo = dimensionValue.Sequence_No ?? 0;
        //		dimensionValueObj.GlobalDimension1Code = dimensionValue.Global_Dimension_1_Code;
        //		dimensionValueList.Add(dimensionValueObj);
        //	}
        //	return Json(dimensionValueList, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetShortcutDimension5Codes(string GlobalDimension1Code)
        //{
        //	List<DimensionValueModel> dimensionValueList = new List<DimensionValueModel>();
        //	var dimensionValues = from dimensionValuesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
        //						  where dimensionValuesQuery.Global_Dimension_No.Equals(5) && dimensionValuesQuery.Global_Dimension_1_Code.Equals(GlobalDimension1Code) &&
        //						  dimensionValuesQuery.Blocked.Equals(false)
        //						  select dimensionValuesQuery;
        //	foreach (DimensionValues dimensionValue in dimensionValues)
        //	{
        //		DimensionValueModel dimensionValueObj = new DimensionValueModel();
        //		dimensionValueObj.DimensionCode = dimensionValue.Dimension_Code;
        //		dimensionValueObj.Code = dimensionValue.Code;
        //		dimensionValueObj.Name = dimensionValue.Name;
        //		dimensionValueObj.DimensionValueType = dimensionValue.Dimension_Value_Type;
        //		dimensionValueObj.Blocked = dimensionValue.Blocked ?? false;
        //		dimensionValueObj.GlobalDimensionNo = dimensionValue.Global_Dimension_No ?? 0;
        //		// dimensionValueObj.SequenceNo = dimensionValue.Sequence_No ?? 0;
        //		dimensionValueObj.GlobalDimension1Code = dimensionValue.Global_Dimension_1_Code;
        //		dimensionValueList.Add(dimensionValueObj);
        //	}
        //	return Json(dimensionValueList, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetShortcutDimension6Codes(string GlobalDimension1Code)
        //{
        //	List<DimensionValueModel> dimensionValueList = new List<DimensionValueModel>();
        //	var dimensionValues = from dimensionValuesList in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
        //						  where dimensionValuesList.Global_Dimension_No.Equals(6) && dimensionValuesList.Global_Dimension_1_Code.Equals(GlobalDimension1Code) &&
        //						  dimensionValuesList.Blocked.Equals(false)
        //						  select dimensionValuesList;
        //	foreach (DimensionValues dimensionValue in dimensionValues)
        //	{
        //		DimensionValueModel dimensionValueModel = new DimensionValueModel();
        //		dimensionValueModel.DimensionCode = dimensionValue.Dimension_Code;
        //		dimensionValueModel.Code = dimensionValue.Code;
        //		dimensionValueModel.Name = dimensionValue.Name;
        //		dimensionValueModel.DimensionValueType = dimensionValue.Dimension_Value_Type;
        //		dimensionValueModel.Blocked = dimensionValue.Blocked ?? false;
        //		dimensionValueModel.GlobalDimensionNo = dimensionValue.Global_Dimension_No ?? 0;
        //		//  dimensionValueModel.SequenceNo = dimensionValue.Sequence_No ?? 0;
        //		dimensionValueModel.GlobalDimension1Code = dimensionValue.Global_Dimension_1_Code;
        //		dimensionValueList.Add(dimensionValueModel);
        //	}
        //	return Json(dimensionValueList, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetShortcutDimension7Codes(string GlobalDimension1Code)
        //{
        //	List<DimensionValueModel> dimensionValueList = new List<DimensionValueModel>();
        //	var dimensionValues = from dimensionValuesList in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
        //						  where dimensionValuesList.Global_Dimension_No.Equals(7) && dimensionValuesList.Global_Dimension_1_Code.Equals(GlobalDimension1Code) &&
        //						  dimensionValuesList.Blocked.Equals(false)
        //						  select dimensionValuesList;
        //	foreach (DimensionValues dimensionValue in dimensionValues)
        //	{
        //		DimensionValueModel dimensionValueModel = new DimensionValueModel();
        //		dimensionValueModel.DimensionCode = dimensionValue.Dimension_Code;
        //		dimensionValueModel.Code = dimensionValue.Code;
        //		dimensionValueModel.Name = dimensionValue.Name;
        //		dimensionValueModel.DimensionValueType = dimensionValue.Dimension_Value_Type;
        //		dimensionValueModel.Blocked = dimensionValue.Blocked ?? false;
        //		dimensionValueModel.GlobalDimensionNo = dimensionValue.Global_Dimension_No ?? 0;
        //		//dimensionValueModel.SequenceNo = dimensionValue.Sequence_No ?? 0;
        //		dimensionValueModel.GlobalDimension1Code = dimensionValue.Global_Dimension_1_Code;
        //		dimensionValueList.Add(dimensionValueModel);
        //	}
        //	return Json(dimensionValueList, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetShortcutDimension8Codes(string GlobalDimension1Code)
        //{
        //	List<DimensionValueModel> dimensionValueList = new List<DimensionValueModel>();
        //	var dimensionValues = from dimensionValuesList in dynamicsNAVODataServices.dynamicsNAVOData.DimensionValues
        //						  where dimensionValuesList.Global_Dimension_No.Equals(8) && dimensionValuesList.Global_Dimension_1_Code.Equals(GlobalDimension1Code) &&
        //						  dimensionValuesList.Blocked.Equals(false)
        //						  select dimensionValuesList;
        //	foreach (DimensionValues dimensionValue in dimensionValues)
        //	{
        //		DimensionValueModel dimensionValueModel = new DimensionValueModel();
        //		dimensionValueModel.DimensionCode = dimensionValue.Dimension_Code;
        //		dimensionValueModel.Code = dimensionValue.Code;
        //		dimensionValueModel.Name = dimensionValue.Name;
        //		dimensionValueModel.DimensionValueType = dimensionValue.Dimension_Value_Type;
        //		dimensionValueModel.Blocked = dimensionValue.Blocked ?? false;
        //		dimensionValueModel.GlobalDimensionNo = dimensionValue.Global_Dimension_No ?? 0;
        //		//  dimensionValueModel.SequenceNo = dimensionValue.Sequence_No ?? 0;
        //		dimensionValueModel.GlobalDimension1Code = dimensionValue.Global_Dimension_1_Code;
        //		dimensionValueList.Add(dimensionValueModel);
        //	}
        //	return Json(dimensionValueList, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetCurrencies()
        //{
        //	List<Currency> currencyList = new List<Currency>();
        //	var currencies = from currenciesQuery in dynamicsNAVODataServices.dynamicsNAVOData.Currencies
        //					 select currenciesQuery;
        //	foreach (Currencies currency in currencies)
        //	{
        //		Currency currencyObj = new Currency();
        //		currencyObj.Code = currency.Code;
        //		currencyObj.Description = currency.Description;
        //		currencyList.Add(currencyObj);
        //	}
        //	return Json(currencyList, JsonRequestBehavior.AllowGet);
        //}
        public string GetLocalCurrencyCode()
		{
			return dynamicsNAVSOAPServices.fundsManagementWS.GetLocalCurrencyCode();
		}
		#endregion Helper Functions

		public async Task<ActionResult> CustomerBalance(DateTime expDate)
		{
			try
			{
				var employeeNo = "";
				var filename = "";
				var filenane = "";
				employeeNo = AccountController.GetEmployeeNo();
				filename = "CustomerBalance" + employeeNo + "_" + employeeNo + ".pdf";

				filename = dynamicsNAVSOAPServices.payrollManagementWS.GenerateCustomerBaleanceToDate(employeeNo, filename, expDate);
				if (filename.Equals("")) return errorResponse.ApplicationExceptionError(new Exception("Unable to print the CustomerBalance. " + ServiceConnection.contactICTDepartment + " "));
				using (var wc = new WebClient())
				{
					var byteArr = await wc.DownloadDataTaskAsync(filename);
					return File(byteArr, MimeMapping.GetMimeMapping(filename));
				}
			}
			catch (Exception ex)
			{
				return errorResponse.ApplicationExceptionError(ex);
				//return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
			}
		}
    }
}