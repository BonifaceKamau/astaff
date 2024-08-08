using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal;
using DynamicsNAV365_StaffPortal.Controllers;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.PerformanceManagement;
using System.Globalization;

namespace DynamicsNAV365_StaffPortal.Controllers.PerformanceManagement
{
   
    public class WorkPlansController : Controller
    {
        private string companyName = ServiceConnection.CompanyName;

        private static string companyURL = "";

        private DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);

        private DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

        private SuccessResponseController successResponse = new SuccessResponseController();

        private InfoResponseController infoResponse = new InfoResponseController();

        private ErrorResponseController errorResponse = new ErrorResponseController();

        //Queries 
        IQueryable<CorporateWorkplan> _corporateWorkplans = null;
        IQueryable<WorkplanPerspectives> _workplanPerspectives = null;
        //[Authorize]
        // Get Lines from Corporate work Plans Table
        public JsonResult GetLines(string WorkplanCode)
        {
            List<CEOWorkPlanModel> WorkplansObjList = new List<CEOWorkPlanModel>();

            var Workplanlines = from WorkQuery in dynamicsNAVODataServices.dynamicsNAVOData.CorporateWorkplan
                                         where WorkQuery.No.Equals(WorkplanCode)
                                         select WorkQuery;
           

            foreach (CorporateWorkplan Lines in Workplanlines)
            {
                
                CEOWorkPlanModel WorkplansObj = new CEOWorkPlanModel(); 
                WorkplansObj.Activity = Lines.Activity;
                WorkplansObj.PMI = Lines.Performance_Measure_Indicator;
                WorkplansObjList.Add(WorkplansObj);                
            }

            return Json(WorkplansObjList, JsonRequestBehavior.AllowGet);
        }
        //Insert CEO Workplan Lines s
        [HttpPost]
        public JsonResult InsertworkplanLines(string UserId, string Perspective, string Designation ,string Directorate, string PerformanceObjective,string Activity, string PMI, string PerformanceOutcome,decimal WeightTotal, string Project, DateTime CompletionDate)
        {
            //WeightTotal = 5;
            var HeaderNumber = "";
            var DirworkplanHeader = from WPQuery in dynamicsNAVODataServices.dynamicsNAVOData.DirectorateWorkplanHeader
                                    where WPQuery.User.Equals(UserId) && WPQuery.Designation.Equals(Designation)
                                  select WPQuery;
            var workphdata = DirworkplanHeader.FirstOrDefault();
            if (workphdata != null)
            {
                HeaderNumber = workphdata.No;
            }
            else
            {
                dynamicsNAVSOAPServices.performanceManagement.CreateCEOWorkPlanHeader(Designation,UserId);
                var HeaderNum = from NumQuery in dynamicsNAVODataServices.dynamicsNAVOData.DirectorateWorkplanHeader
                                        where NumQuery.User.Equals(UserId) && NumQuery.Designation.Equals(Designation)
                                        select NumQuery;
                var newheaderno = DirworkplanHeader.FirstOrDefault();
                HeaderNumber = newheaderno.No;
            }

            try
            {
                dynamicsNAVSOAPServices.performanceManagement.InsertCeoWorkplanLines(Directorate, PerformanceObjective, Activity, PMI,Designation,Perspective, Project,PerformanceOutcome,CompletionDate,WeightTotal,UserId,HeaderNumber);

                //return true;
                return Json(new { success = true, message ="Inserted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {

                //return fa
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
           
        }
        //Get CEO  Inserted WorkPlan Lines from Directorate Workplan Lines Table
        public JsonResult GetCEOWorkplanLines(string Designation)
        {
            List<CEOWorkPlanModel> CEOWorkplansObjList = new List<CEOWorkPlanModel>();
            // CEOWorkPlanModel CEOWorkplansLinesObj = new CEOWorkPlanModel();

            var DirectorateWorkplanlines = from LinesQuery in dynamicsNAVODataServices.dynamicsNAVOData.DirectorateWorkLines
                                           where LinesQuery.Designation.Equals("CEO")
                                select LinesQuery;


            foreach (DirectorateWorkLines Lines in DirectorateWorkplanlines)
            {

                CEOWorkPlanModel CEOWorkplansLinesObj = new CEOWorkPlanModel();
                CEOWorkplansLinesObj.Activity = Lines.Activity;
                CEOWorkplansLinesObj.PMI = Lines.Performance_Measure_Indicator;
                CEOWorkplansLinesObj.Directorate = Lines.Directorate;
                CEOWorkplansLinesObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                //CEOWorkplansLinesObj.CompletionDate = Lines.Completion_Date.Value;
                CEOWorkplansLinesObj.PerformanceObjective = Lines.Performance_Objective;
                CEOWorkplansLinesObj.PerformanceOutcome = Lines.Performance_Outcome;
                CEOWorkplansLinesObj.Project = Lines.Project;
                CEOWorkplansLinesObj.WeightTotal = Lines.Weight_Total ?? 0; 
                CEOWorkplansLinesObj.Perspective = Lines.Perspective;
                CEOWorkplansLinesObj.Code = Lines.Code;
                CEOWorkplansObjList.Add(CEOWorkplansLinesObj);
            }
           // return Json(CEOWorkplansLinesObj, JsonRequestBehavior.AllowGet);
            return Json(CEOWorkplansObjList, JsonRequestBehavior.AllowGet);
        }
        // Get CEO Line For Edit
        public JsonResult GetCEOLineForEdit(string Code)
        {
            CEOWorkPlanModel CEOWorkplansEditLinesObj = new CEOWorkPlanModel();

            var DirectorateWorkplanlinesByCode = from CodeQuery in dynamicsNAVODataServices.dynamicsNAVOData.DirectorateWorkLines
                                           where CodeQuery.Code.Equals(Code)
                                           select CodeQuery;


            foreach (DirectorateWorkLines Lines in DirectorateWorkplanlinesByCode)
            {

                CEOWorkplansEditLinesObj.No = Lines.No;
                CEOWorkplansEditLinesObj.Activity = Lines.Activity;
                CEOWorkplansEditLinesObj.PMI = Lines.Performance_Measure_Indicator;
                CEOWorkplansEditLinesObj.Directorate = Lines.Directorate;
                CEOWorkplansEditLinesObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                //CEOWorkplansEditLinesObj.CompletionDate = Lines.Completion_Date.Value;
                CEOWorkplansEditLinesObj.PerformanceObjective = Lines.Performance_Objective;
                CEOWorkplansEditLinesObj.PerformanceOutcome = Lines.Performance_Outcome;
                CEOWorkplansEditLinesObj.Project = Lines.Project;
                CEOWorkplansEditLinesObj.WeightTotal = Lines.Weight_Total ?? 0;
                CEOWorkplansEditLinesObj.Perspective = Lines.Perspective;
                CEOWorkplansEditLinesObj.Code = Lines.Code;
                CEOWorkplansEditLinesObj.Designation = Lines.Designation;
               // CEOWorkplansEditObjList.Add(CEOWorkplansEditLinesObj);
            }          
            return Json(CEOWorkplansEditLinesObj, JsonRequestBehavior.AllowGet);
        }

        //Delete CEO Workplan Line
        public JsonResult  DeleteCEOWorkPlanLine(string Code)
        {
            bool lineDeleted = false;
            lineDeleted = dynamicsNAVSOAPServices.performanceManagement.DeleteCEOWorkPlanLine(Code);
            return Json(lineDeleted, JsonRequestBehavior.AllowGet);
        }

       // Modify CEO Work  Plan Line

         public JsonResult ModifyCEOWorkPlanLine(string UserCode, string Perspective, string Designation, string PerformanceObjective, string Directorate, string Project, string Activity, string PMI, string PerformanceOutcome, decimal WeightTotal,DateTime CompletionDate)
        {
            //WeightTotal = 5;
            try
            {
             dynamicsNAVSOAPServices.performanceManagement.ModifyCEOWPL(Directorate,PerformanceObjective,Activity,PMI,Designation,Perspective,Project,PerformanceOutcome,CompletionDate,WeightTotal,UserCode);

            }
            catch (Exception ex)
            {
                return Json(new { success = false}, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        // GET: WorkPlans- Load Workplans Pages Based on Designations
        [Authorize]
        public ActionResult _FillWorkplan()  
        {
            //CEO Section - Filling in Workplans Using the Corporate Workplans Table
            CEOWorkPlanModel CEOWorkplansLinesObj = new CEOWorkPlanModel();
           
                var employeeNumber = AccountController.GetEmployeeNo(); 

                var EmployeeNoQuery = from EmpQuery in dynamicsNAVODataServices.dynamicsNAVOData.EmployeeQueryTwo
                                      where EmpQuery.No.Equals(employeeNumber)
                                      select EmpQuery;
                var userdata = EmployeeNoQuery.FirstOrDefault();
                var EmployeeDesignationQuery = from DesQuery in dynamicsNAVODataServices.dynamicsNAVOData.UserSetupQuery
                                               where DesQuery.Employee_No.Equals(employeeNumber)
                                               select DesQuery;
                var des = EmployeeDesignationQuery.FirstOrDefault();            
                LoadCorporateWorkplans();
                CEOWorkplansLinesObj.PerformanceObjectiveCodes = new SelectList(_corporateWorkplans, "No", "Performance_Objective");
                LoadWorkplanPerspectives();
                CEOWorkplansLinesObj.PerspectiveCodes = new SelectList(_workplanPerspectives, "Code", "Description");
                CEOWorkplansLinesObj.Directorate = userdata.Global_Dimension_1_Code;
                CEOWorkplansLinesObj.Designation = des.Designation;
                CEOWorkplansLinesObj.UserId = userdata.User_ID;



            //Loading Directors View
            CEOWorkPlanModel DirectorsWorkplansLinesObj = new CEOWorkPlanModel();
            LoadCorporateWorkplans();
            DirectorsWorkplansLinesObj.PerformanceObjectiveCodes = new SelectList(_corporateWorkplans, "No", "Performance_Objective");
            LoadWorkplanPerspectives();
            DirectorsWorkplansLinesObj.PerspectiveCodes = new SelectList(_workplanPerspectives, "Code", "Description");
            DirectorsWorkplansLinesObj.Directorate = userdata.Global_Dimension_1_Code;
            DirectorsWorkplansLinesObj.Department = userdata.Global_Dimension_2_Code;
            DirectorsWorkplansLinesObj.Designation= des.Designation;
            DirectorsWorkplansLinesObj.UserId= userdata.User_ID;


            //Loading Managers View
            CEOWorkPlanModel ManagersWorkplansLinesObj = new CEOWorkPlanModel();
            LoadCorporateWorkplans();
            ManagersWorkplansLinesObj.PerformanceObjectiveCodes = new SelectList(_corporateWorkplans, "No", "Performance_Objective");
            LoadWorkplanPerspectives();
            ManagersWorkplansLinesObj.PerspectiveCodes = new SelectList(_workplanPerspectives, "Code", "Description");
            ManagersWorkplansLinesObj.Directorate = userdata.Global_Dimension_1_Code;
            ManagersWorkplansLinesObj.Department = userdata.Global_Dimension_2_Code;
            ManagersWorkplansLinesObj.Designation = des.Designation;
            ManagersWorkplansLinesObj.UserId = userdata.User_ID;


            //Loading Assistant Managers View
            CEOWorkPlanModel AssitantManagersWorkplansLinesObj = new CEOWorkPlanModel();
            LoadCorporateWorkplans();
            AssitantManagersWorkplansLinesObj.PerformanceObjectiveCodes = new SelectList(_corporateWorkplans, "No", "Performance_Objective");
            LoadWorkplanPerspectives();
            AssitantManagersWorkplansLinesObj.PerspectiveCodes = new SelectList(_workplanPerspectives, "Code", "Description");
            AssitantManagersWorkplansLinesObj.Directorate = userdata.Global_Dimension_1_Code;
            AssitantManagersWorkplansLinesObj.Department = userdata.Global_Dimension_2_Code;
            AssitantManagersWorkplansLinesObj.Designation = des.Designation;
            AssitantManagersWorkplansLinesObj.UserId = userdata.User_ID;

            //Loading Officers View
            CEOWorkPlanModel OfficersWorkplansLinesObj = new CEOWorkPlanModel();
            LoadCorporateWorkplans();
            OfficersWorkplansLinesObj.PerformanceObjectiveCodes = new SelectList(_corporateWorkplans, "No", "Performance_Objective");
            LoadWorkplanPerspectives();
            OfficersWorkplansLinesObj.PerspectiveCodes = new SelectList(_workplanPerspectives, "Code", "Description");
            OfficersWorkplansLinesObj.Directorate = userdata.Global_Dimension_1_Code;
            OfficersWorkplansLinesObj.Department = userdata.Global_Dimension_2_Code;
            OfficersWorkplansLinesObj.Designation = des.Designation;
            OfficersWorkplansLinesObj.UserId = userdata.User_ID;           

            if (des.Designation == "CEO")
                return PartialView("_FillWorkplan", CEOWorkplansLinesObj);

            if (des.Designation == "Director")
               
                return PartialView("_DirectorsWorkPlans", DirectorsWorkplansLinesObj); 

            if (des.Designation == "Manager")
                return PartialView("_ManagersWorkPlans", ManagersWorkplansLinesObj); 

            if (des.Designation == "Asst Manager")
                return PartialView("_AsstManagersWorkPlans", AssitantManagersWorkplansLinesObj);
            else                
            return PartialView("_OfficersWorkPlans", OfficersWorkplansLinesObj);          


        }
        #region Main Directors Region
        
        //Create Directors Workplan Header
        public JsonResult CreateDirectorsWPHeader(string Designation, string UserId, string Directorate)
        {
            try
            {

                var CheckCeoRecord = (from CEOQuery in dynamicsNAVODataServices.dynamicsNAVOData.DirectorateWorkplanHeader
                                       where CEOQuery.Designation.Equals("CEO")
                                       select CEOQuery).ToList();
                if (CheckCeoRecord.Count < 1)
                {
                    return Json(new { message = "ceonill" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var DirectorsHeader = from DQuery in dynamicsNAVODataServices.dynamicsNAVOData.DirectorateWorkplanHeader
                                          where DQuery.Designation.Equals(Designation) && DQuery.Directorate.Equals(Directorate)
                                          select DQuery;
                    var directordata = DirectorsHeader.FirstOrDefault();
                    if (directordata == null)
                    {
                        dynamicsNAVSOAPServices.performanceManagement.CreateDirectorWorkPlanHeader(UserId, Designation, Directorate);
                        return Json(new { message = "succeeded" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (directordata != null && directordata.User == UserId)
                    {
                        return Json(new { message = "youexist" }, JsonRequestBehavior.AllowGet);

                    }
                    else if (directordata != null)
                    {
                        return Json(new { message = "someoneexists" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
                    }
                }
              
            }
            catch (Exception ex)
            {
                //return false
                return Json(new { message = "failed"}, JsonRequestBehavior.AllowGet);
            }
          
        }
        //Insert Director Workplan Lines
        [HttpPost]
        public JsonResult InsertDirectorWorkplanLines(string UserId, string Perspective, string Designation, string Directorate, string PerformanceObjective, string Activity, string PMI, string PerformanceOutcome, decimal WeightTotal, string Project, DateTime CompletionDate)
        {
            //WeightTotal = 5;
            try
            {
            var DirectorHeaderNumber = "";
            var DirectorworkplanHeader = from DWPQuery in dynamicsNAVODataServices.dynamicsNAVOData.DirectorateWorkplanHeader
                                    where DWPQuery.User.Equals(UserId) && DWPQuery.Designation.Equals(Designation)
                                    select DWPQuery;
            var direcdata = DirectorworkplanHeader.FirstOrDefault();

            if (direcdata==null)
            {
                return Json(new { message = "noheader" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                DirectorHeaderNumber = direcdata.No;
             
                    dynamicsNAVSOAPServices.performanceManagement.InsertDirectorWorkplanLines(Directorate, PerformanceObjective, Activity, PMI, Designation, Perspective, Project, PerformanceOutcome, CompletionDate, WeightTotal, UserId, DirectorHeaderNumber);

                    //return true;
                    return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);                
                
            }

            }
                catch (Exception ex)
            {
                //return false 
                return Json(new { message = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Get Directors WorkPlan Lines from Directorate Workplan Lines Table
        [HttpPost]
        public JsonResult LoadDirectorWorkplanLines(string Mydirectorate)
        {
            
            //var des = Designation;
            var dir = Mydirectorate;
            List<CEOWorkPlanModel> DirectorObjList = new List<CEOWorkPlanModel>();

            var DirectorsWL = from DirQuery in dynamicsNAVODataServices.dynamicsNAVOData.DirectorateWorkLines
                              where DirQuery.Designation.Equals("Director") && DirQuery.Directorate.Equals(Mydirectorate)
                                           select DirQuery;

            foreach (DirectorateWorkLines Lines in DirectorsWL)
            {

                CEOWorkPlanModel DirectorWorkplansLinesObj = new CEOWorkPlanModel();
                DirectorWorkplansLinesObj.Activity = Lines.Activity;
                DirectorWorkplansLinesObj.PMI = Lines.Performance_Measure_Indicator;
                DirectorWorkplansLinesObj.Directorate = Lines.Directorate;
                //DirectorWorkplansLinesObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                DirectorWorkplansLinesObj.CompletionDate = Lines.Completion_Date.Value.ToShortDateString();
                DirectorWorkplansLinesObj.PerformanceObjective = Lines.Performance_Objective;
                DirectorWorkplansLinesObj.PerformanceOutcome = Lines.Performance_Outcome;
                DirectorWorkplansLinesObj.Project = Lines.Project;
                DirectorWorkplansLinesObj.WeightTotal = Lines.Weight_Total ?? 0;
                DirectorWorkplansLinesObj.Perspective = Lines.Perspective;
                DirectorWorkplansLinesObj.Code = Lines.Code;
                DirectorWorkplansLinesObj.Designation = Lines.Designation;
                DirectorObjList.Add(DirectorWorkplansLinesObj);
            }            
            return Json(DirectorObjList, JsonRequestBehavior.AllowGet);
        }

        // Get Director Line For Edit
        [Authorize]
        [HttpPost]
        public JsonResult GetDirectorLineForEdit(string Code)
        {
            CEOWorkPlanModel DirWorkplansEditLinesObj = new CEOWorkPlanModel();

            var DirectorateWorkplanlinesByCode = from CodeQuery in dynamicsNAVODataServices.dynamicsNAVOData.DirectorateWorkLines
                                                 where CodeQuery.Code.Equals(Code)
                                                 select CodeQuery;


            foreach (DirectorateWorkLines Lines in DirectorateWorkplanlinesByCode)
            {

                DirWorkplansEditLinesObj.No = Lines.No;
                DirWorkplansEditLinesObj.Activity = Lines.Activity;
                DirWorkplansEditLinesObj.PMI = Lines.Performance_Measure_Indicator;
                DirWorkplansEditLinesObj.Directorate = Lines.Directorate;
                DirWorkplansEditLinesObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                //DirWorkplansEditLinesObj.CompletionDate = Lines.Completion_Date.Value;

                DirWorkplansEditLinesObj.PerformanceObjective = Lines.Performance_Objective;
                DirWorkplansEditLinesObj.PerformanceOutcome = Lines.Performance_Outcome;
                DirWorkplansEditLinesObj.Project = Lines.Project;
                DirWorkplansEditLinesObj.WeightTotal = Lines.Weight_Total ?? 0;
                DirWorkplansEditLinesObj.Perspective = Lines.Perspective;
                DirWorkplansEditLinesObj.Code = Lines.Code;
                DirWorkplansEditLinesObj.HeaderNo= Lines.Header_No;
                DirWorkplansEditLinesObj.Designation = Lines.Designation;
                // CEOWorkplansEditObjList.Add(CEOWorkplansEditLinesObj);
            }
            return Json(DirWorkplansEditLinesObj, JsonRequestBehavior.AllowGet);
        }

              
        // Modify Director Work  Plan Line

        public JsonResult ModifyDirectorWorkPlanLine(string Directorate,string Perspective,string UserCode,string PerformanceOutcome, decimal WeightTotal, DateTime CompletionDate, string Project, string PerformanceObjective,string HeaderNo)
        {
            //WeightTotal = 5;
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.ModifyDirectorWorkplanLine(CompletionDate,UserCode,WeightTotal,PerformanceOutcome,Project,PerformanceObjective,HeaderNo,Perspective,Directorate);

            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        //Delete Directors' Workplan Line
        public JsonResult DeleteDirectorWorkPlanLine(string Code)
        {
            bool lineDeleted = false;
            lineDeleted = dynamicsNAVSOAPServices.performanceManagement.DeleteDirectorsWorkPlanLine(Code); 
            return Json(lineDeleted, JsonRequestBehavior.AllowGet);
        }
        #endregion End  of Main Directors Region

        #region Managers Main Region

        //Get Managers WorkPlan Lines from Directorate Workplan Lines Table
        [HttpPost]
        public JsonResult LoadManagerWorkplanLines(string Department)
        {
           var dep = Department;
            List<CEOWorkPlanModel> ManagerObjList = new List<CEOWorkPlanModel>();

            var ManagersWL = from ManagerQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalWorkLines
                              where ManagerQuery.Designation.Equals("Manager") && ManagerQuery.Department.Equals(Department)
                             select ManagerQuery;
            //&& ManagerQuery.Directorate.Equals()

            foreach (DepartmentalWorkLines Lines in ManagersWL)
            {

                CEOWorkPlanModel ManagersWorkplansLinesObj = new CEOWorkPlanModel();
                ManagersWorkplansLinesObj.Activity = Lines.Activity;
                ManagersWorkplansLinesObj.PMI = Lines.Performance_Measure_Indicator;
                ManagersWorkplansLinesObj.Directorate = Lines.Directorate;
                ManagersWorkplansLinesObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                //ManagersWorkplansLinesObj.CompletionDate = Lines.Completion_Date.Value;
                ManagersWorkplansLinesObj.PerformanceObjective = Lines.Performance_Objective;
                ManagersWorkplansLinesObj.PerformanceOutcome = Lines.Performance_Outcome;
                ManagersWorkplansLinesObj.Project = Lines.Project;
                ManagersWorkplansLinesObj.WeightTotal = Lines.Weight_Total ?? 0;
                ManagersWorkplansLinesObj.Perspective = Lines.Perspective;
                ManagersWorkplansLinesObj.Code = Lines.Code;
                ManagersWorkplansLinesObj.Designation = Lines.Designation;
                ManagersWorkplansLinesObj.DepartmentalObjective = Lines.Departmental_Objective;
                ManagerObjList.Add(ManagersWorkplansLinesObj);
            }
            return Json(ManagerObjList, JsonRequestBehavior.AllowGet);
        }


       //Create Managers Workplan Header
        public JsonResult CreateManagersWPHeader(string Designation, string UserId, string Directorate, string Department)
        {
            try
            {
                var CheckDireRecord = (from DirQuery in dynamicsNAVODataServices.dynamicsNAVOData.DirectorateWorkplanHeader
                                          where DirQuery.Designation.Equals("Director") && DirQuery.Directorate.Equals(Directorate)
                                          select DirQuery).ToList();
                if (CheckDireRecord.Count < 1)
                {
                    return Json(new { message = "managernill" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var DirectorHeaderNo = "";
                    var ManagersHeader = from MQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalWorkPlanHeader
                                         where MQuery.Designation.Equals(Designation) && MQuery.Directorate.Equals(Directorate)
                                         select MQuery;
                    var managerdata = ManagersHeader.FirstOrDefault();
                    if (managerdata == null)
                    {
                        dynamicsNAVSOAPServices.performanceManagement.CreateManagerWorkPlanHeader(UserId, Designation, Department, Directorate);
                        return Json(new { message = "succeeded" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (managerdata != null && managerdata.User == UserId)
                    {
                        return Json(new { message = "youexist" }, JsonRequestBehavior.AllowGet);

                    }
                    else if (managerdata != null)
                    {
                        return Json(new { message = "someoneexists" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
                    }
                }
             
            }
            catch (Exception ex)
            {
                //return false
                return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }

        // Get Manager Line For Edit
        [Authorize]
        [HttpPost]
        public JsonResult GetManagerLineForEdit(string Code)
        {
            CEOWorkPlanModel ManagerPlansEditLinesObj = new CEOWorkPlanModel();

            var MangerLinesByCode = from ManagerQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalWorkLines
                                                 where ManagerQuery.Code.Equals(Code)
                                                 select ManagerQuery;


            foreach (DepartmentalWorkLines Lines in MangerLinesByCode)
            {

                ManagerPlansEditLinesObj.No = Lines.No;
                ManagerPlansEditLinesObj.Activity = Lines.Activity;
                ManagerPlansEditLinesObj.PMI = Lines.Performance_Measure_Indicator;
                ManagerPlansEditLinesObj.Directorate = Lines.Directorate;
                ManagerPlansEditLinesObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                //ManagerPlansEditLinesObj.CompletionDate = Lines.Completion_Date.Value;
                ManagerPlansEditLinesObj.PerformanceObjective = Lines.Performance_Objective;
                ManagerPlansEditLinesObj.PerformanceOutcome = Lines.Performance_Outcome;
                ManagerPlansEditLinesObj.Project = Lines.Project;
                ManagerPlansEditLinesObj.WeightTotal = Lines.Weight_Total ?? 0;
                ManagerPlansEditLinesObj.Perspective = Lines.Perspective;
                ManagerPlansEditLinesObj.Code = Lines.Code;
                ManagerPlansEditLinesObj.HeaderNo = Lines.Header_No;
                ManagerPlansEditLinesObj.Designation = Lines.Designation;
                ManagerPlansEditLinesObj.DepartmentalObjective = Lines.Departmental_Objective;
                ManagerPlansEditLinesObj.Department = Lines.Department;
                // CEOWorkplansEditObjList.Add(CEOWorkplansEditLinesObj);
            }
            return Json(ManagerPlansEditLinesObj, JsonRequestBehavior.AllowGet);
        }

        // Modify Manager Work  Plan Line
        public JsonResult ModifyManagerWorkPlanLine(string Directorate, string Perspective, string UserCode, string PerformanceOutcome, decimal WeightTotal, DateTime CompletionDate, string Project, string PerformanceObjective, string HeaderNo, string DeptObjective, string Department)
        {
            //WeightTotal = 5;
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.ModifyManagerWorkplanLine(CompletionDate, UserCode, WeightTotal, PerformanceOutcome, Project, PerformanceObjective, HeaderNo, Perspective, Directorate,DeptObjective,Department);

            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        //Delete Managers' Workplan Line
        public JsonResult DeleteManagerWorkPlanLine(string Code)
        {
            bool lineDeleted = false;
            lineDeleted = dynamicsNAVSOAPServices.performanceManagement.DeleteManagerWorkplanLine(Code);
            return Json(lineDeleted, JsonRequestBehavior.AllowGet);
        }
        #endregion End of Managers Main Region
        #region  Asst Managers Main Region

        //Load Assistant Manager Workplan Lines from Departmental Workplan Lines Table (Inherits from Managers')
        public JsonResult LoadAsstManagerWorkplanLines(string Department)
        {
            var dep = Department;
            List<CEOWorkPlanModel> AsstManagerObjList = new List<CEOWorkPlanModel>();
            var DesignationNew = "";
            var AsstManagersWL = from AsstManagerQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalWorkLines
                             where AsstManagerQuery.Department.Equals(Department) && AsstManagerQuery.Designation.Equals("Asst Manager")
                             select AsstManagerQuery;        

            foreach (DepartmentalWorkLines Lines in AsstManagersWL)
            {

                CEOWorkPlanModel ManagersWorkplansLinesObj = new CEOWorkPlanModel();
                ManagersWorkplansLinesObj.Activity = Lines.Activity;
                ManagersWorkplansLinesObj.PMI = Lines.Performance_Measure_Indicator;
                ManagersWorkplansLinesObj.Directorate = Lines.Directorate;
                ManagersWorkplansLinesObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                //ManagersWorkplansLinesObj.CompletionDate = Lines.Completion_Date.Value;
                ManagersWorkplansLinesObj.PerformanceObjective = Lines.Performance_Objective;
                ManagersWorkplansLinesObj.PerformanceOutcome = Lines.Performance_Outcome;
                ManagersWorkplansLinesObj.Project = Lines.Project;
                ManagersWorkplansLinesObj.WeightTotal = Lines.Weight_Total ?? 0;
                ManagersWorkplansLinesObj.Perspective = Lines.Perspective;
                ManagersWorkplansLinesObj.Code = Lines.Code;
                ManagersWorkplansLinesObj.Designation = Lines.Designation;
                ManagersWorkplansLinesObj.DepartmentalObjective = Lines.Departmental_Objective;
                AsstManagerObjList.Add(ManagersWorkplansLinesObj);
            }
            return Json(AsstManagerObjList, JsonRequestBehavior.AllowGet);
        }


        //Create Asst Managers Workplan Header
        public JsonResult CreateAsstManagersWPHeader(string Designation, string UserId, string Directorate, string Department)
        {
            var des = Designation;
            var user = UserId;
            var dir = Directorate;
            var dep = Department;
            try
            {
                var CheckManagerRecord = (from RecordQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalWorkPlanHeader
                                       where RecordQuery.Designation.Equals("Manager") && RecordQuery.Department.Equals(Department)
                                       select RecordQuery).ToList();
                if (CheckManagerRecord.Count<1)
                {
                    return Json(new { message = "managernill" }, JsonRequestBehavior.AllowGet);
                }
                { 
                var DirectorHeaderNo = "";
                var AsstManagersHeader = from AsstMQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalWorkPlanHeader
                                         where AsstMQuery.Designation.Equals(Designation) && AsstMQuery.Department.Equals(Department)
                                         select AsstMQuery;
                var Asstmanagerdata = AsstManagersHeader.FirstOrDefault();
                if (Asstmanagerdata == null)
                {
                    dynamicsNAVSOAPServices.performanceManagement.CreateAsstManagerWorkPlanHeader(UserId, Department, Directorate);
                    return Json(new { message = "succeeded" }, JsonRequestBehavior.AllowGet);
                }
                else if (Asstmanagerdata != null && Asstmanagerdata.User == UserId)
                {
                    return Json(new { message = "youexist" }, JsonRequestBehavior.AllowGet);

                }
                else if (Asstmanagerdata != null)
                {
                    return Json(new { message = "someoneexists" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
                }
            }
               
            }
            catch (Exception ex)
            {
                //return false
                return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }
        //Delete Asst Managers' Workplan Line 
        public JsonResult DeleteAsstManagerWorkPlanLine(string Code)
        {
            bool lineDeleted = false;
            lineDeleted = dynamicsNAVSOAPServices.performanceManagement.DeleteAsstManagerWorkplanLine(Code);
            return Json(lineDeleted, JsonRequestBehavior.AllowGet);
        }
        // Get Asst Manager Line For Edit
        [Authorize]
        [HttpPost]
        public JsonResult GetAsstManagerLineForEdit(string Code)
        {
            CEOWorkPlanModel AsstManagerPlansEditLinesObj = new CEOWorkPlanModel();

            var MangerLinesByCode = from ManagerQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalWorkLines
                                    where ManagerQuery.Code.Equals(Code)
                                    select ManagerQuery;


            foreach (DepartmentalWorkLines Lines in MangerLinesByCode)
            {

                AsstManagerPlansEditLinesObj.No = Lines.No;
                AsstManagerPlansEditLinesObj.Activity = Lines.Activity;
                AsstManagerPlansEditLinesObj.PMI = Lines.Performance_Measure_Indicator;
                AsstManagerPlansEditLinesObj.Directorate = Lines.Directorate;
                AsstManagerPlansEditLinesObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                //AsstManagerPlansEditLinesObj.CompletionDate = Lines.Completion_Date.Value;
                AsstManagerPlansEditLinesObj.PerformanceObjective = Lines.Performance_Objective;
                AsstManagerPlansEditLinesObj.PerformanceOutcome = Lines.Performance_Outcome;
                AsstManagerPlansEditLinesObj.Project = Lines.Project;
                AsstManagerPlansEditLinesObj.WeightTotal = Lines.Weight_Total ?? 0;
                AsstManagerPlansEditLinesObj.Perspective = Lines.Perspective;
                AsstManagerPlansEditLinesObj.Code = Lines.Code;
                AsstManagerPlansEditLinesObj.HeaderNo = Lines.Header_No;
                AsstManagerPlansEditLinesObj.Designation = Lines.Designation;
                AsstManagerPlansEditLinesObj.DepartmentalObjective = Lines.Departmental_Objective;
                AsstManagerPlansEditLinesObj.Department = Lines.Department;
                // CEOWorkplansEditObjList.Add(CEOWorkplansEditLinesObj);
            }
            return Json(AsstManagerPlansEditLinesObj, JsonRequestBehavior.AllowGet);
        }

        // Modify Manager Work  Plan Line
        public JsonResult ModifyAsstManagerWorkPlanLine(string Directorate, string Perspective, string UserCode, string PerformanceOutcome, decimal WeightTotal, DateTime CompletionDate, string Project, string PerformanceObjective, string HeaderNo, string DeptObjective, string Department)
        {
            //WeightTotal = 5;
            try
            {
                dynamicsNAVSOAPServices.performanceManagement.ModifyAsstManagerWorkplanLine(CompletionDate, UserCode, WeightTotal, PerformanceOutcome, Project, PerformanceObjective, HeaderNo, Perspective, Directorate, DeptObjective, Department);

            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion End of Asst Managers Main Region

        #region Officers Main Region
        //Load Officers Workplan Lines (Inherits from Assistant Managers)
        [HttpPost]
        public JsonResult LoadOfficersWorkplanLines(string Department)
        {
            var Dep = Department;
            List<CEOWorkPlanModel> OfficersObjList = new List<CEOWorkPlanModel>();

            var OfficersWL = from OfficerQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalWorkLines
                                 where OfficerQuery.Department.Equals(Department) && OfficerQuery.Designation.Equals("Officer")
                                 select OfficerQuery;
            //&& ManagerQuery.Directorate.Equals()

            foreach (DepartmentalWorkLines Lines in OfficersWL)
            {

                CEOWorkPlanModel OfficersWorkplansLinesObj = new CEOWorkPlanModel();
                OfficersWorkplansLinesObj.Activity = Lines.Activity;
                OfficersWorkplansLinesObj.PMI = Lines.Performance_Measure_Indicator;
                OfficersWorkplansLinesObj.Directorate = Lines.Directorate;
                OfficersWorkplansLinesObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                //OfficersWorkplansLinesObj.CompletionDate = Lines.Completion_Date.Value;
                OfficersWorkplansLinesObj.PerformanceObjective = Lines.Performance_Objective;
                OfficersWorkplansLinesObj.PerformanceOutcome = Lines.Performance_Outcome;
                OfficersWorkplansLinesObj.Project = Lines.Project;
                OfficersWorkplansLinesObj.WeightTotal = Lines.Weight_Total ?? 0;
                OfficersWorkplansLinesObj.Perspective = Lines.Perspective;
                OfficersWorkplansLinesObj.Code = Lines.Code;
                OfficersWorkplansLinesObj.Designation = Lines.Designation;
                OfficersWorkplansLinesObj.DepartmentalObjective = Lines.Departmental_Objective;
                OfficersObjList.Add(OfficersWorkplansLinesObj);
            }
            return Json(OfficersObjList, JsonRequestBehavior.AllowGet);
        }

        //Create Officers Workplan Header
        public JsonResult CreateOfficersWPHeader(string Designation, string UserId, string Directorate, string Department)
        {
            try
            {

                var DirectorHeaderNo = "";
                var des = Designation;
                var User = UserId;
                var Dire = Directorate;
                var dep = Department;
                var OfficersHeader = from OffQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalWorkPlanHeader
                                         where OffQuery.Designation.Equals(Designation) && OffQuery.Department.Equals(Department)
                                         select OffQuery;
                var Officerdata = OfficersHeader.FirstOrDefault();

                var CheckManagerRec= (from RecQuery in dynamicsNAVODataServices.dynamicsNAVOData.DepartmentalWorkPlanHeader
                                     where RecQuery.Designation.Equals("Asst Manager") && RecQuery.Department.Equals(Department)
                                     select RecQuery).ToList();
                if (CheckManagerRec.Count < 1)
                {
                    return Json(new { message = "managernill" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (Officerdata == null)
                    {
                        dynamicsNAVSOAPServices.performanceManagement.CreateOfficerWorkPlanHeader(UserId, Department, Directorate);
                        return Json(new { message = "succeeded" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (Officerdata != null && Officerdata.User == UserId)
                    {
                        return Json(new { message = "youexist" }, JsonRequestBehavior.AllowGet);

                    }
                    else if (Officerdata != null)
                    {
                        return Json(new { message = "someoneexists" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
                    }
                }

               // return Json(new { message = "succeeded" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //return false
                return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion End of Officers Main Region


        #region Helper Functions
        [Authorize]
        private void GetEmployeeDesignation()
        {

        }
        private void LoadCorporateWorkplans()
        {
            try
            {
                _corporateWorkplans = from _Corporates in dynamicsNAVODataServices.dynamicsNAVOData.CorporateWorkplan
                                          //where _HrJoblookupValue.Option.Equals("Qualification")  
                                      select _Corporates;
            }
            catch (Exception ex) {
            }

        }
        private void LoadWorkplanPerspectives()
        {
            _workplanPerspectives = from perspectives in dynamicsNAVODataServices.dynamicsNAVOData.WorkplanPerspectives
                               where perspectives.Type.Equals("Appraisal Perspective")
                               select perspectives;
        }
        #endregion Helper functions End
    }
}