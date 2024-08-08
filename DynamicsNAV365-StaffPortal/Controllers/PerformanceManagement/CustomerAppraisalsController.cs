using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.PerformanceManagement;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers.PerformanceManagement
{
    public class CustomerAppraisalsController : Controller
    {
        private string companyName = ServiceConnection.CompanyName;

        private static string companyURL = "";

        private DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);

        private DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

        private SuccessResponseController successResponse = new SuccessResponseController();

        private InfoResponseController infoResponse = new InfoResponseController();

        private ErrorResponseController errorResponse = new ErrorResponseController();
        // GET: CustomerAppraisals
        public ActionResult _CustomerAppraisals() 
        {

            //CustomerAppraisalsModel CustomerAppObj = new CustomerAppraisalsModel();

            //var employeeNumber = AccountController.GetEmployeeNo();

            //var EmployeeNoQuery = from EmpQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
            //                      where EmpQuery.No.Equals(employeeNumber)
            //                      select EmpQuery;
            //var userdata = EmployeeNoQuery.FirstOrDefault();
            //CustomerAppObj.UserId = userdata.User_ID;
            //return PartialView("_CustomerAppraisals", CustomerAppObj);

            //Load Customer Headers 
        //[Authorize]
        ////[HttpPost]
        //public JsonResult LoadCustomerHeader(string Designation)
        //{
            //var Des = Designation;
            var StaffNo = AccountController.GetEmployeeNo();
            List<CustomerAppraisalsModel> CustomerAppLinesObjList = new List<CustomerAppraisalsModel>();

            var CustomerAppLines = from CustQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalHeader
                                   where CustQuery.Assign_To_Customer.Equals(StaffNo)
                                   select CustQuery;

            foreach (HRAppraisalHeader Lines in CustomerAppLines)
            {

                CustomerAppraisalsModel CustomerAppLinesObj = new CustomerAppraisalsModel();

                CustomerAppLinesObj.AppraisalNo = Lines.No;
                CustomerAppLinesObj.EmployeeName = Lines.Employee_Name;
                CustomerAppLinesObj.EmployeeNo = Lines.Employee_No;
                CustomerAppLinesObj.Designation = Lines.Designation;
                CustomerAppLinesObj.Department = Lines.Department_Name;
                CustomerAppLinesObj.AppraisalPeriod = Lines.Appraisal_Period;
                CustomerAppLinesObj.AppraisalStage = Lines.Appraisal_Stage;
                CustomerAppLinesObj.ExternalSourceNo = Lines.Assign_To_Customer;
                CustomerAppLinesObj.AppraisalStatus = Lines.Appraisal_Status;
                CustomerAppLinesObj.Appeal = Lines.Appealed ?? false;

                CustomerAppLinesObjList.Add(CustomerAppLinesObj);
            }
            return PartialView(CustomerAppLinesObjList);
            //return Json(CustomerAppLinesObjList, JsonRequestBehavior.AllowGet);
        }
          

        //Get Customer Lines 
        [Authorize]       
        public ActionResult SingleCustomerForm(string AppraisalNo)
        {
            try
            {
                if (AppraisalNo.Equals(""))

                {
                    //return RedirectToAction("ManageProject", "Projects", new { ProjectNo = TasksObjectEdit.ProjectNo });
                    return RedirectToAction("CustomerAppraisalHome", "PerformanceHome");
                }
                var Ap = AppraisalNo;
                List<CustomerAppraisalsModel> CustomerAppObjList = new List<CustomerAppraisalsModel>();
                var CustomerAppLines = from CustAppQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLines
                                       where CustAppQuery.Appraisal_No.Equals(AppraisalNo) && CustAppQuery.Categorize_As.Equals("External Sources")
                                       select CustAppQuery;
                foreach (HRAppraisalLines Lines in CustomerAppLines)
                {
                    CustomerAppraisalsModel CustomerAppObj = new CustomerAppraisalsModel();

                    CustomerAppObj.LineNo = Lines.Line_No;
                    CustomerAppObj.ExtGoalsTargets = Lines.Perfomance_Goals_and_Targets;
                    CustomerAppObj.AppraisalNo = Lines.Appraisal_No;
                    CustomerAppObj.ExternalRating = Lines.External_Source_Rating ?? 0;
                    CustomerAppObj.ExtSourceComments = Lines.External_Source_Comments;
                    CustomerAppObjList.Add(CustomerAppObj);
                }
                return View(CustomerAppObjList);
            }
            catch (Exception ex)
            {
             return errorResponse.ApplicationExceptionError(ex);
            }
            //return Json(CustomerAppObjList, JsonRequestBehavior.AllowGet);
        }


        //Get A single Customer Line for Edit
 
        public ActionResult GetCustomerLineForedit(int LineNo)
        {
            try
            {
                if (LineNo.Equals(""))

                {
                    //return RedirectToAction("ManageProject", "Projects", new { ProjectNo = TasksObjectEdit.ProjectNo });
                    return RedirectToAction("CustomerAppraisalHome", "PerformanceHome");
                }
                var pcode = LineNo;
                CustomerAppraisalsModel CustomerLinesObj = new CustomerAppraisalsModel();

                var CustLineQuery = from CustQuery in dynamicsNAVODataServices.dynamicsNAVOData.HRAppraisalLines
                                    where CustQuery.Line_No.Equals(LineNo)
                                    select CustQuery;

                foreach (HRAppraisalLines Lines in CustLineQuery)
                {
                    CustomerLinesObj.ExtGoalsTargets = Lines.Perfomance_Goals_and_Targets;
                    CustomerLinesObj.ExternalRating = Lines.External_Source_Rating ?? 0;
                    CustomerLinesObj.ExtSourceComments = Lines.External_Source_Comments;
                    CustomerLinesObj.AppraisalNo = Lines.Appraisal_No;
                    CustomerLinesObj.LineNo = Lines.Line_No;
                }

                return View(CustomerLinesObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
            //return Json(CustomerLinesObj, JsonRequestBehavior.AllowGet);
        }
        //Modify Customer Line
        [Authorize]
        [HttpPost]
        public ActionResult GetCustomerLineForedit(CustomerAppraisalsModel customerObj)
        {
        //public JsonResult ModifyCustomerLine(int LineNo, decimal CustomerRating, string CustomerComments)  
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.ModifyCustomerLine(customerObj.LineNo, customerObj.ExternalRating, customerObj.ExtSourceComments);
                TempData["saved"] = "Changes saved";
                return RedirectToAction("SingleCustomerForm", "CustomerAppraisals", new { AppraisalNo = customerObj.AppraisalNo });
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }           
        }
    }
}