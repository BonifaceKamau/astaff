using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.Finance.StaffAdvance;

namespace DynamicsNAV365_StaffPortal.Controllers.Administration
{
    public class FleetManagementController:Controller
    {
        AccountController accountController = new AccountController();
        string employeeNo = "";
        List<RequestHeader> requestHeaders = new List<RequestHeader>();
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

        IQueryable<Currencies> currencyCodes = null;
        IQueryable<DimensionValues> globalDimension1Codes = null;
        IQueryable<DimensionValues> globalDimension2Codes = null;
        IQueryable<DimensionValues> shortcutDimension3Codes = null;
        IQueryable<DimensionValues> shortcutDimension4Codes = null;
        IQueryable<DimensionValues> shortcutDimension5Codes = null;
        IQueryable<DimensionValues> shortcutDimension6Codes = null;
        IQueryable<DimensionValues> shortcutDimension7Codes = null;
        IQueryable<DimensionValues> shortcutDimension8Codes = null;

        public FleetManagementController()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        public ActionResult FleetLogs()
        {
            
            return View();
        }
    }
}