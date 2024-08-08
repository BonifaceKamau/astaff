using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.Models.ProjectManagement;
using System.Threading.Tasks;
using System.Globalization;

namespace DynamicsNAV365_StaffPortal.Controllers.ProjectManagement
{
    public class ProjectsController : Controller
    {
        private string companyName = ServiceConnection.CompanyName;

        private static string companyURL = "";

        private DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);

        private DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);

        private SuccessResponseController successResponse = new SuccessResponseController();

        private InfoResponseController infoResponse = new InfoResponseController();

        private ErrorResponseController errorResponse = new ErrorResponseController();

        //Queries
        IQueryable<ProjectTeamMembers> _projectTeamMembers = null;
        IQueryable<ProjectHeader> _myownProjects = null;

        IQueryable<StrategicPlan> _strategicPlans = null;
        IQueryable<StrategicObjectives> _strategicObjectives = null;
        // GET: Projects for Each Project Manager
        public ActionResult _ManagerProjects()
        {

            var StaffNo = AccountController.GetEmployeeNo();       

            var ProjectTeamPeople = from ProjectQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                                    where ProjectQuery.No.Equals(StaffNo) && ProjectQuery.Role.Equals("PM")
                                    select ProjectQuery;

            var NonPMPeople = from OthersQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                              where OthersQuery.No.Equals(StaffNo) && OthersQuery.Role != "PM"
                              select OthersQuery;
            int countOne = 0;
            int countTwo = 0;

            //Project Managers
            List<ProjectManagementModel> ProjectLinesObjList = new List<ProjectManagementModel>();

            foreach (ProjectTeamMembers Lines in ProjectTeamPeople)
            {
                var PmCode = Lines.PM_Code;
                var ProjectHeader = from PHeaderQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader
                                    where PHeaderQuery.Project_Management_Team.Equals(PmCode)
                                    select PHeaderQuery;
                countOne = countOne + 1;
                foreach (ProjectHeader LinesTwo in ProjectHeader)
                {
                    ProjectManagementModel ProjectLinesObj = new ProjectManagementModel();
                    ProjectLinesObj.Code = LinesTwo.Code;
                    ProjectLinesObj.Description = LinesTwo.Description;
                    ProjectLinesObj.Cost = LinesTwo.Cost ?? 0;
                    ProjectLinesObj.StartDate = LinesTwo.Start_Date.Value.ToString("dd/MM/yyyy");
                    ProjectLinesObj.EndDate = LinesTwo.End_Date.Value.ToString("dd/MM/yyyy");
                    ProjectLinesObj.Status = LinesTwo.Status;
                    ProjectLinesObj.ProjectManagementTeam = LinesTwo.Project_Management_Team;

                    ProjectLinesObjList.Add(ProjectLinesObj);
                }
            }

            // Project Team members
            List<ProjectManagementModel> MemberLinesObjList = new List<ProjectManagementModel>();

            foreach (ProjectTeamMembers Lines in NonPMPeople)
            {
                var PmmCode = Lines.PM_Code;
                var memberRole = Lines.Role_Description;
                countTwo = countTwo + 1;

                var MemberProjectHeader = from PHeaderQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader
                                          where PHeaderQuery.Project_Management_Team.Equals(PmmCode)
                                          select PHeaderQuery;
                foreach (ProjectHeader LinesThree in MemberProjectHeader)
                {
                    ProjectManagementModel MemberLinesObj = new ProjectManagementModel();
                    MemberLinesObj.MemberRole = memberRole;
                    MemberLinesObj.Code = LinesThree.Code;
                    MemberLinesObj.Description = LinesThree.Description;
                    MemberLinesObj.Cost = LinesThree.Cost ?? 0;
                    MemberLinesObj.StartDate = LinesThree.Start_Date.Value.ToString("dd/MM/yyyy");
                    MemberLinesObj.EndDate = LinesThree.End_Date.Value.ToString("dd/MM/yyyy");
                    MemberLinesObj.Status = LinesThree.Status;
                    MemberLinesObj.ProjectManagementTeam = LinesThree.Project_Management_Team;

                    MemberLinesObjList.Add(MemberLinesObj);
                }
            }

            var ProjectManagerCount = countOne;
            var NonProjectManagerCount = countTwo;

            if (ProjectManagerCount > 0)
                return PartialView("_ManagerProjects", ProjectLinesObjList);

            else if (NonProjectManagerCount > 0)
                return PartialView("_MemberProjects", MemberLinesObjList);
            else
                return PartialView("_ProjectsHome");

        }
        //Get  Project Details/Activities for a project
        [Authorize]
        public ActionResult ViewProjectDetails(string ProjectNo)
        {           
            try
            {
                if (ProjectNo.Equals(""))

                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                }
                var ProjectActivitiesPA = from ActivityQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectActivities
                                          where ActivityQuery.Code.Equals(ProjectNo)
                                          select ActivityQuery;

                List<ProjectManagementModel> ActivitiesObjList = new List<ProjectManagementModel>(); 

                foreach (ProjectActivities Lines in ProjectActivitiesPA)
                {
                    ProjectManagementModel ActivitiesObj = new ProjectManagementModel();
                    ActivitiesObj.StrategyCode = Lines.Strategy;
                    ActivitiesObj.ObjectiveCode = Lines.Objectives;
                    ActivitiesObj.Code = Lines.Code;
                    ActivitiesObj.Activity = Lines.Activity;
                    ActivitiesObj.ActivityStartDate = Lines.Start_Date.Value.ToString("dd/MM/yyyy");
                    ActivitiesObj.ActivityEndDate = Lines.End_Date.Value.ToString("dd/MM/yyyy");
                    ActivitiesObj.ActivityStatus = Lines.Status;
                    ActivitiesObj.AssignedTo = Lines.Assigned_To;
                    ActivitiesObj.StrategicObjective = Lines.Strategic_Objective;
                    ActivitiesObjList.Add(ActivitiesObj);
                }

                return View(ActivitiesObjList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Get  Project Team Members for a project
        [Authorize]
        public ActionResult ViewTeamMembers(string ProjectManagementTeam)
        {
            try
            {
                if (ProjectManagementTeam.Equals(""))

                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                }
                var ProjectTeam = from MembersQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                                  where MembersQuery.PM_Code.Equals(ProjectManagementTeam)
                                  select MembersQuery;
                List<ProjectManagementModel> MembersObjList = new List<ProjectManagementModel>();

                foreach (ProjectTeamMembers Lines in ProjectTeam)
                {
                    ProjectManagementModel MembersObj = new ProjectManagementModel();
                    MembersObj.TeamMemberCode = Lines.No;
                    MembersObj.TeamMemberName = Lines.Name;
                    MembersObj.TeamMemberType = Lines.Type;
                    MembersObj.TeamMemberRole = Lines.Role;
                    MembersObj.RoleDescription = Lines.Role_Description;
                    MembersObjList.Add(MembersObj);
                }

                return View(MembersObjList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Get  Project Benefits for a project
        [Authorize]
        public ActionResult ProjectBenefits(string ProjectNo)
        {
            try
            {
                if (ProjectNo.Equals(""))

            {
                return RedirectToAction("ProjectInfo", "ProjectHome");
            }
            var StaffNo = AccountController.GetEmployeeNo();
            var MyRoleCheck = (from RoleQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                               where RoleQuery.No.Equals(StaffNo) && RoleQuery.Role.Equals("PM")
                               select RoleQuery).ToList();

            if (MyRoleCheck.Count < 1)
            {
                return RedirectToAction("ProjectInfo", "ProjectHome");
                //return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = ProjectNo });
            }
            else
            {
                var projectNumber = ProjectNo;
             
                    var ProjectBenefits = from BenefitsQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectBenefitsRegister
                                          where BenefitsQuery.Project_No.Equals(ProjectNo)
                                          select BenefitsQuery;
                    List<ProjectBenefitsModel> BenefitsObjList = new List<ProjectBenefitsModel>();
                    ViewBag.ProjectNumber = projectNumber;

                    foreach (ProjectBenefitsRegister Lines in ProjectBenefits)
                    {
                        ProjectBenefitsModel BenefitsObj = new ProjectBenefitsModel();

                        BenefitsObj.ProjectNo = Lines.Project_No;
                        BenefitsObj.BenefitText = Lines.Benefit;
                        BenefitsObj.ObjectiveSupported = Lines.Objective_Supported;
                        BenefitsObj.BenefitOwner = Lines.Benefit_Owner;
                        BenefitsObj.BeneficiariesText = Lines.Beneficiaries;
                        BenefitsObj.KPIText = Lines.KPI;
                        BenefitsObj.MeasureText = Lines.Measure;
                        BenefitsObj.FrequencyText = Lines.Frequency;
                        BenefitsObj.AssumptionsText = Lines.Benefit_Assumptions;
                        BenefitsObj.BenefitRisks = Lines.Benefit_Risks;
                        BenefitsObj.NotesText = Lines.Notes;
                        BenefitsObj.LineNo = Lines.Line_No;
                        BenefitsObjList.Add(BenefitsObj);
                    }

                    return View(BenefitsObjList);
               
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Get  Project Risks for a project
        [Authorize]
        public ActionResult ProjectRisks(string ProjectNo)
        {
            try
            {

                if (ProjectNo.Equals(""))

            {
                return RedirectToAction("ProjectInfo", "ProjectHome");
            }
            var StaffNo = AccountController.GetEmployeeNo();
            var MyRoleCheck = (from RoleQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                               where RoleQuery.No.Equals(StaffNo) && RoleQuery.Role.Equals("PM")
                               select RoleQuery).ToList();

            if (MyRoleCheck.Count < 1)
            {
                return RedirectToAction("ProjectInfo", "ProjectHome");
                //return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = ProjectNo });
            }
            else
            {
                 var ProjectRisks = from RisksQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectRiskRegister
                                       where RisksQuery.Project_Code.Equals(ProjectNo)
                                       select RisksQuery;
                    List<ProjectRisksModel> RisksObjList = new List<ProjectRisksModel>();
                    ViewBag.ProjectNumber = ProjectNo;
                    foreach (ProjectRiskRegister Lines in ProjectRisks)
                    {
                        ProjectRisksModel RisksObj = new ProjectRisksModel();

                        RisksObj.ProjectCode = Lines.Project_Code;
                        RisksObj.RiskDescription = Lines.Risk_Description;
                        RisksObj.ImpactDescription = Lines.Impact_Description;
                        RisksObj.ImpactLevel = Lines.Impact_Level ?? 0;
                        RisksObj.PriorityLevel = Lines.Priority_Level ?? 0;
                        RisksObj.ProbabilityLevel = Lines.Probability_Level ?? 0;
                        RisksObj.MitigationNotes = Lines.Mitigation_Notes;
                        RisksObj.OwnerCode = Lines.Owner;
                        RisksObj.OwnerName = Lines.Owner_Name;
                        RisksObj.ObjectiveRelated = Lines.Objective_Related;
                        //RisksObj.RiskStartDate = Lines.Risk_Start.Value.ToString("dd/MM/yyyy");
                        RisksObj.RiskStartDate = Lines.Risk_Start.Value;
                        //RisksObj.RiskEndDate = Lines.Risk_End.Value.ToString("dd/MM/yyyy");
                        RisksObj.RiskEndDate = Lines.Risk_End.Value;
                        RisksObj.LineNo = Lines.Line_No;
                        RisksObjList.Add(RisksObj);
                    }

                    return View(RisksObjList);
                
             }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Project Manager Workplan
        [Authorize]
        public ActionResult ProjectPlan(string ProjectNo)
        {
            try
            {

                if (ProjectNo.Equals(""))

                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                var MyRoleCheck = (from RoleQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                                   where RoleQuery.No.Equals(StaffNo) && RoleQuery.Role.Equals("PM")
                                   select RoleQuery).ToList();

                if (MyRoleCheck.Count < 1)
                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                    //return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = ProjectNo });
                }
                else
                {
                    var ProjectPlan = (from PlanQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectPlanHeader
                                       where PlanQuery.Project_No.Equals(ProjectNo)
                                       select PlanQuery).ToList();
                    List<ProjectPlanModel> PlanObjList = new List<ProjectPlanModel>();
                    if (ProjectPlan.Count > 0)
                    {
                     
                        ViewBag.ProjectNumber = ProjectNo;
                        foreach (ProjectPlanHeader Lines in ProjectPlan)
                        {
                            ProjectPlanModel PlanObj = new ProjectPlanModel();
                            PlanObj.ProjectName = Lines.Project_Name;
                            PlanObj.Description = Lines.Description;
                            PlanObj.StartDate = Lines.Start_Date.Value;
                            PlanObj.EndDate = Lines.End_Date.Value;
                            PlanObj.Status = Lines.Status;
                            PlanObj.ProjectNumber = Lines.Project_No;
                            PlanObj.ApprovalStatus = Lines.Approval_Status;

                            PlanObjList.Add(PlanObj);
                        }

                        return View(PlanObjList);
                    }
                    else
                    {
                        ViewBag.Nulldata = "There is no project plan for this project. Please create One";
                        ViewBag.ProjectNumber = ProjectNo;
                        //TempData["null"] = "There is no project plan for this project. Please create One";
                        return View(PlanObjList);
                    }
                }
              
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Project Manager Workplan
        [Authorize]
        public ActionResult NewProjectPlan(string ProjectNo)
        {
            try
            {

                if (ProjectNo.Equals(""))

                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                var MyRoleCheck = (from RoleQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                                   where RoleQuery.No.Equals(StaffNo) && RoleQuery.Role.Equals("PM")
                                   select RoleQuery).ToList();

                if (MyRoleCheck.Count < 1)
                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                    //return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = ProjectNo });
                }
                else
                {
                    var ProjectData = from PdataQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader
                                       where PdataQuery.Code.Equals(ProjectNo)
                                       select PdataQuery;
                    var Details = ProjectData.FirstOrDefault();

                    List<ProjectPlanModel> PlanObjList = new List<ProjectPlanModel>();

                    ProjectPlanModel PlanObj = new ProjectPlanModel();
                    PlanObj.ProjectNumber = ProjectNo;
                    PlanObj.ProjectName = Details.Description;
                    PlanObj.Status = Details.Status;

                    return View(PlanObj);
                   
                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Post Project Plan Header
        [Authorize]
        [HttpPost]
        public ActionResult NewProjectPlan(ProjectPlanModel PlansObj)
        {
            try
            {
                var ProjectPlanCheck = (from PCdataQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectPlanHeader
                                  where PCdataQuery.Project_No.Equals(PlansObj.ProjectNumber)
                                  select PCdataQuery).ToList();
                if (ProjectPlanCheck.Count > 0) 
                {
                    TempData["error"] = "Failed, a project plan for this project already exists";
                    return RedirectToAction("ProjectPlan", "Projects", new { ProjectNo = PlansObj.ProjectNumber });
                    //return View(PlansObj);                    
                }
                else
                {
                    dynamicsNAVSOAPServices.projectManagement.CreateProjectPlanHeader(PlansObj.Description, Convert.ToDateTime(PlansObj.StartDate), Convert.ToDateTime(PlansObj.EndDate), PlansObj.Status, PlansObj.ProjectNumber);
                    TempData["success"] = "Record Created successfully.";
                    return RedirectToAction("ProjectPlan", "Projects", new { ProjectNo = PlansObj.ProjectNumber });
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Project Details
        [Authorize]
        public ActionResult ProjectPlanDetails(string ProjectNo)
        {
            try
            {

                if (ProjectNo.Equals(""))

                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                var MyRoleCheck = (from RoleQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                                   where RoleQuery.No.Equals(StaffNo) && RoleQuery.Role.Equals("PM")
                                   select RoleQuery).ToList();

                if (MyRoleCheck.Count < 1)
                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                    //return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = ProjectNo });
                }
                else
                {
                    var PlanHeader = from PhQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectPlanHeader
                                      where PhQuery.Project_No.Equals(ProjectNo)
                                      select PhQuery;
                    var Details = PlanHeader.FirstOrDefault();

                    List<ProjectPlanModel> PlanObjList = new List<ProjectPlanModel>();

                    ProjectPlanModel PlanObj = new ProjectPlanModel();
                    PlanObj.Code = Details.Code;
                    PlanObj.ProjectNumber = ProjectNo;
                    PlanObj.ProjectName = Details.Project_Name;
                    PlanObj.StartDate = Details.Start_Date;
                    PlanObj.Description = Details.Description;
                    PlanObj.EndDate = Details.End_Date;
                    PlanObj.Status = Details.Status;
                    PlanObj.ApprovalStatus = Details.Approval_Status;          

                    return View(PlanObj);

                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Partial View for Project Plan activities
        [Authorize]
        public ActionResult _ProjectPlanActivities(string ProjectNo)
        {
            try
            {

                if (ProjectNo.Equals(""))

                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                var MyRoleCheck = (from RoleQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                                   where RoleQuery.No.Equals(StaffNo) && RoleQuery.Role.Equals("PM")
                                   select RoleQuery).ToList();

                if (MyRoleCheck.Count < 1)
                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                    //return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = ProjectNo });
                }
                else
                {

                    var PlanActivities = from ActivitiesQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectPlanActivities
                                     where ActivitiesQuery.Project_Number.Equals(ProjectNo)
                                     select ActivitiesQuery; 
                    

                    List<PlanActivitiesModel> PlanActivitiesObjList = new List<PlanActivitiesModel>();
                    foreach (ProjectPlanActivities Lines in PlanActivities)
                    {
                        PlanActivitiesModel PlanActivitiesObj = new PlanActivitiesModel();
                      
                        PlanActivitiesObj.Code = Lines.Code;
                        PlanActivitiesObj.LineNo = Lines.Line_No;
                        PlanActivitiesObj.Activity = Lines.Activity;
                        PlanActivitiesObj.StartDate = Lines.Start_Date;
                        PlanActivitiesObj.EndDate = Lines.End_Date;
                        PlanActivitiesObj.Status = Lines.Status;
                        //PlanActivitiesObj.StrategicPlan = Lines.Pla ;
                        PlanActivitiesObj.StrategicObjective = Lines.Strategic_Objective;
                        PlanActivitiesObj.ProjectNumber = Lines.Project_Number;
                        PlanActivitiesObjList.Add(PlanActivitiesObj);


                    }
                    ViewBag.ProjectNumber = ProjectNo;
                    return PartialView(PlanActivitiesObjList);


                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Add Project Plan Activity
        [Authorize]
        public ActionResult AddPlanActivity(string ProjectNo)
        {
            try
            {

                if (ProjectNo.Equals(""))

                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                }
                var StaffNo = AccountController.GetEmployeeNo();
                var MyRoleCheck = (from RoleQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                                   where RoleQuery.No.Equals(StaffNo) && RoleQuery.Role.Equals("PM")
                                   select RoleQuery).ToList();

                if (MyRoleCheck.Count < 1)
                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                    //return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = ProjectNo });
                }
                else
                {
                  
                    PlanActivitiesModel PlanActivitiesObj = new PlanActivitiesModel();
                    PlanActivitiesObj.ProjectNumber = ProjectNo;
                    LoadStrategicPlans();
                    PlanActivitiesObj.StrategicPlanCodes = new SelectList(_strategicPlans, "Code", "Description");
                    LoadStrategicObjectives();
                    PlanActivitiesObj.StrategicObjectiveCodes=new SelectList(_strategicObjectives,"Objective_Code","Objective_Description");
                    return View(PlanActivitiesObj);

                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Add Project Plan Activity - Post
        [Authorize]
        [HttpPost]
        public ActionResult AddPlanActivity(PlanActivitiesModel PlansObj)
        {
            try
            {
               
                var StaffNo = AccountController.GetEmployeeNo();
                var MyRoleCheck = (from RoleQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                                   where RoleQuery.No.Equals(StaffNo) && RoleQuery.Role.Equals("PM")
                                   select RoleQuery).ToList();

                if (MyRoleCheck.Count < 1)
                {
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                    //return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = ProjectNo });
                }
                else
                {
                    PlansObj.StrategicPlan= PlansObj.StrategicPlan != null ? PlansObj.StrategicPlan : "";
                    PlansObj.StrategicObjective = PlansObj.StrategicObjective != null ? PlansObj.StrategicObjective : "";

                    dynamicsNAVSOAPServices.projectManagement.AddPlanActivity(PlansObj.Activity, Convert.ToDateTime(PlansObj.StartDate), Convert.ToDateTime(PlansObj.EndDate), PlansObj.StrategicObjective, PlansObj.ProjectNumber, PlansObj.Status,PlansObj.StrategicPlan);
                    TempData["success"] = "Activity Added Successfully.";
                    return RedirectToAction("ProjectPlanDetails", "Projects", new { ProjectNo = PlansObj.ProjectNumber});

                }

            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Get Project Plan Activity for edit
        [Authorize]
        public ActionResult EditPlanActivity(int Line) 
        {
            try
            {
                if (Line.Equals("")) 

                {                  
                    return RedirectToAction("ProjectInfo", "ProjectHome");
                }

                int myline = Line;
                var GetActivity = from GetActivityQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectPlanActivities
                                        where GetActivityQuery.Line_No.Equals(Line)
                                        select GetActivityQuery;

                PlanActivitiesModel ActivityObj = new PlanActivitiesModel();
                LoadStrategicPlans();
                ActivityObj.StrategicPlanCodes = new SelectList(_strategicPlans, "Code", "Description");
                LoadStrategicObjectives();
                ActivityObj.StrategicObjectiveCodes = new SelectList(_strategicObjectives, "Objective_Code", "Objective_Description");
                foreach (ProjectPlanActivities Lines in GetActivity)
                {

                    ActivityObj.LineNo = Lines.Line_No;
                    ActivityObj.Activity = Lines.Activity;
                    ActivityObj.StartDate = Lines.Start_Date;
                    ActivityObj.EndDate = Lines.End_Date;
                    ActivityObj.StrategicObjective = Lines.Objective;
                    ActivityObj.StrategicPlan = Lines.Strategic_Plan;
                    ActivityObj.Status = Lines.Status;
                    ActivityObj.ProjectNumber = Lines.Project_Number;                    
                }

                return View(ActivityObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Get Project Plan Activity for edit
        [Authorize]
        [HttpPost]
        public ActionResult EditPlanActivity(PlanActivitiesModel  PlansObj)
        {
            try
            {
                var PlanHeader = from PhQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectPlanHeader
                                 where PhQuery.Project_No.Equals(PlansObj.ProjectNumber)
                                 select PhQuery;
                var Details = PlanHeader.FirstOrDefault();
                var approval = Details.Approval_Status;
                if (approval == "Approved")
                {
                    TempData["failed"] = "Failed, This plan has been approved and activities cannot be edited";
                    return RedirectToAction("ProjectPlanDetails", "Projects", new { ProjectNo = PlansObj.ProjectNumber });
                }
                else
                {

                    dynamicsNAVSOAPServices.projectManagement.ModifyProjectPlanActivity(PlansObj.LineNo, PlansObj.Activity, Convert.ToDateTime(PlansObj.StartDate), Convert.ToDateTime(PlansObj.EndDate), PlansObj.StrategicObjective, PlansObj.StrategicPlan, PlansObj.Status);
                    TempData["success"] = "Changes Saved Successfully";
                    return RedirectToAction("ProjectPlanDetails", "Projects", new { ProjectNo = PlansObj.ProjectNumber });
                }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }


        //Delete Activity Line
        [HttpPost]
        public JsonResult DeleteActivityLine(int LineNo,string ProjectNumber)
        {
            var PlanHeader = from PhQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectPlanHeader
                             where PhQuery.Project_No.Equals(ProjectNumber)
                             select PhQuery;
            var Details = PlanHeader.FirstOrDefault();
            var approval = Details.Approval_Status;
            try { 
            if (approval == "Approved")
            {
                return Json(new { message = "approved" }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                dynamicsNAVSOAPServices.projectManagement.DeletePlanActivity(LineNo);
                return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
            }            
            

            }
            catch (Exception ex)
            {
                return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }

        //Send Project Plan Approval Request
        [HttpPost]
        public JsonResult SendPlanApproval(string ProjectNumber)
        {
            var PlanHeader = from PhQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectPlanHeader
                             where PhQuery.Project_No.Equals(ProjectNumber)
                             select PhQuery;
            var Details = PlanHeader.FirstOrDefault();
            var approval = Details.Approval_Status;
            try
            {
                if (approval == "Approved")
                {
                    return Json(new { message = "approved" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    dynamicsNAVSOAPServices.projectManagement.SendProjectPlanApproval(ProjectNumber);
                    return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }
        //Get  Project Tasks for a Project member
        [Authorize]
        public ActionResult ProjectMemberTasks(string ProjectNo)
        {
            try
            {
                if (ProjectNo.Equals(""))

            {
                return RedirectToAction("ProjectInfo", "ProjectHome");
            }
            var staffNo = AccountController.GetEmployeeNo();
            
                var MemberProjectTasks = from TasksQuery in dynamicsNAVODataServices.dynamicsNAVOData.ResourcesTasksLines
                                         where TasksQuery.Project_No.Equals(ProjectNo) && TasksQuery.Resource_No.Equals(staffNo)
                                         select TasksQuery;
                var ProjectTitleCheck = from PTQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader
                                        where PTQuery.Code.Equals(ProjectNo)
                                        select PTQuery;
                var QueryData = ProjectTitleCheck.FirstOrDefault();

                List<ProjectTasksModel> MemberTasksObjList = new List<ProjectTasksModel>();

                foreach (ResourcesTasksLines Lines in MemberProjectTasks)
                {
                    ProjectTasksModel MemberTasksObj = new ProjectTasksModel();
                    ViewBag.ProjectTitle = QueryData.Description;
                    MemberTasksObj.ResourceNo = Lines.Resource_No;
                    MemberTasksObj.ProjectNo = Lines.Project_No;
                    MemberTasksObj.TaskDescription = Lines.Task_Description;
                    MemberTasksObj.LineNo = Lines.Line_No;
                    MemberTasksObj.AssignDate = Lines.Assign_Date.Value.ToString("dd/MM/yyyy");
                    MemberTasksObj.ExpectedCompletionDate = Lines.Expected_Completion.Value;
                    MemberTasksObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                    MemberTasksObj.PMRemarks = Lines.PM_Remarks;
                    MemberTasksObj.DurationTaken = Lines.Duration_Taken;
                    MemberTasksObj.MarkCompleted = Lines.Mark_Completed ?? false;
                    MemberTasksObj.ConfirmCompleted = Lines.Confirm_Completed ?? false;
                    MemberTasksObj.TaskSent = Lines.Task_Sent ?? false;
                    MemberTasksObjList.Add(MemberTasksObj);
                }

                return View(MemberTasksObjList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Get Project Benefit Line for edit
        [Authorize]
        public ActionResult EditProjectBenefit(int Line)
        {
            try
            {
                if (Line.Equals(""))

            {
                //return RedirectToAction("ProjectBenefits", "Projects", new { ProjectNo = BenefitsObj.ProjectNo });
                return RedirectToAction("ProjectInfo", "ProjectHome");
            }
           
                int myline = Line;
                var GetProjectBenefit = from GetBenefitQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectBenefitsRegister
                                        where GetBenefitQuery.Line_No.Equals(Line)
                                        select GetBenefitQuery;
                ProjectBenefitsModel BenefitsObj = new ProjectBenefitsModel();
                foreach (ProjectBenefitsRegister Lines in GetProjectBenefit)
                {

                    BenefitsObj.ProjectNo = Lines.Project_No;
                    BenefitsObj.BenefitText = Lines.Benefit;
                    BenefitsObj.ObjectiveSupported = Lines.Objective_Supported;
                    BenefitsObj.BenefitOwner = Lines.Benefit_Owner;
                    BenefitsObj.BeneficiariesText = Lines.Beneficiaries;
                    BenefitsObj.KPIText = Lines.KPI;
                    BenefitsObj.MeasureText = Lines.Measure;
                    BenefitsObj.FrequencyText = Lines.Frequency;
                    BenefitsObj.AssumptionsText = Lines.Benefit_Assumptions;
                    BenefitsObj.BenefitRisks = Lines.Benefit_Risks;
                    BenefitsObj.NotesText = Lines.Notes;
                    BenefitsObj.BaselineDate = Lines.Baseline_Date.Value;
                    BenefitsObj.RealisationStatus = Lines.Realisation_Status;
                    BenefitsObj.ActualRealisationDate = Lines.Actual_Realisation_Date.Value;
                    BenefitsObj.TargetValue = Lines.Target_Value;
                    BenefitsObj.AssumptionsText = Lines.Benefit_Assumptions;
                    BenefitsObj.BenefitRisks = Lines.Benefit_Risks;
                    BenefitsObj.LineNo = Lines.Line_No;
                }

                return View(BenefitsObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Edit Project Benefit POST
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProjectBenefit(ProjectBenefitsModel BenefitsObj, string Command)
        {
            try {
                if (Command.Equals("Save Changes"))
                {
                    BenefitsObj.AssumptionsText = BenefitsObj.AssumptionsText != null ? BenefitsObj.AssumptionsText : "";
                    BenefitsObj.BenefitRisks = BenefitsObj.BenefitRisks != null ? BenefitsObj.BenefitRisks : "";
                    BenefitsObj.NotesText = BenefitsObj.NotesText != null ? BenefitsObj.NotesText : "";
                    BenefitsObj.KPIText = BenefitsObj.KPIText != null ? BenefitsObj.KPIText : "";
                    BenefitsObj.MeasureText = BenefitsObj.MeasureText != null ? BenefitsObj.MeasureText : "";
                    BenefitsObj.FrequencyText = BenefitsObj.FrequencyText != null ? BenefitsObj.FrequencyText : "";
                    dynamicsNAVSOAPServices.projectManagement.ModifyProjectBenefit(BenefitsObj.ProjectNo, BenefitsObj.BenefitText, BenefitsObj.ObjectiveSupported, BenefitsObj.BenefitOwner, BenefitsObj.BeneficiariesText, BenefitsObj.KPIText, BenefitsObj.MeasureText, BenefitsObj.FrequencyText, BenefitsObj.LineNo, Convert.ToDateTime(BenefitsObj.BaselineDate), BenefitsObj.RealisationStatus, Convert.ToDateTime(BenefitsObj.ActualRealisationDate), BenefitsObj.TargetValue, BenefitsObj.AssumptionsText, BenefitsObj.BenefitRisks, BenefitsObj.NotesText);
                    TempData["success"] = "Changes Saved Successfully";
                    return RedirectToAction("ProjectBenefits", "Projects", new { ProjectNo = BenefitsObj.ProjectNo });
                }
                else
                {
                  
                    return View(BenefitsObj);
                }
            }
            catch(Exception ex)
            {
                TempData["failed"] = ex;
                return View(BenefitsObj);
            }
          
        }

        //Delete A Benefit  Line
        public JsonResult DeleteBenefitLine(int LineNo)
        {
            bool lineDeleted = false;
            lineDeleted = dynamicsNAVSOAPServices.projectManagement.DeleteBenefitLine(LineNo);
            return Json(lineDeleted, JsonRequestBehavior.AllowGet);
        }

        //Add Project Benefit 
        [Authorize]
        public ActionResult AddProjectBenefit(string ProjectNo)
        {
            try
            {
                if (ProjectNo.Equals(""))

            {
                return RedirectToAction("ProjectBenefits", "Projects", new { ProjectNo = ProjectNo });
                //return RedirectToAction("ProjectInfo", "ProjectHome");
            }
            
                ProjectBenefitsModel BenefitsObj = new ProjectBenefitsModel();
                BenefitsObj.ProjectNo = ProjectNo;
                return View(BenefitsObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Add Project Risk
        [Authorize]
        public ActionResult AddProjectRisk(string ProjectNo)
        {
            try
            {
                if (ProjectNo.Equals(""))

            {
                return RedirectToAction("ProjectRisks", "Projects", new { ProjectNo = ProjectNo });
                //return RedirectToAction("ProjectInfo", "ProjectHome");
            }
          
                ProjectRisksModel RiskObj = new ProjectRisksModel();
                RiskObj.ProjectCode = ProjectNo;
                return View(RiskObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Insert Project Benefit
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProjectBenefit(ProjectBenefitsModel AddBenefitsObj, string Command)
        {
            try
            {
                if (Command.Equals("Save"))
                {
                    AddBenefitsObj.AssumptionsText = AddBenefitsObj.AssumptionsText != null ? AddBenefitsObj.AssumptionsText: "";
                    AddBenefitsObj.BenefitRisks = AddBenefitsObj.BenefitRisks != null ? AddBenefitsObj.BenefitRisks : "";
                    AddBenefitsObj.NotesText = AddBenefitsObj.NotesText != null ? AddBenefitsObj.NotesText : "";
                    AddBenefitsObj.KPIText = AddBenefitsObj.KPIText != null ? AddBenefitsObj.KPIText : "";
                    AddBenefitsObj.MeasureText = AddBenefitsObj.MeasureText != null ? AddBenefitsObj.MeasureText : "";
                    AddBenefitsObj.FrequencyText = AddBenefitsObj.FrequencyText != null ? AddBenefitsObj.FrequencyText : "";

                    //AddBenefitsObj.ActualRealisationDate = AddBenefitsObj.ActualRealisationDate != null ? AddBenefitsObj.ActualRealisationDate : "";


                    dynamicsNAVSOAPServices.projectManagement.AddProjectBenefit(AddBenefitsObj.ProjectNo, AddBenefitsObj.BenefitText, AddBenefitsObj.ObjectiveSupported
                    , AddBenefitsObj.BenefitOwner, AddBenefitsObj.BeneficiariesText, AddBenefitsObj.KPIText, AddBenefitsObj.MeasureText, AddBenefitsObj.FrequencyText,
                    Convert.ToDateTime(AddBenefitsObj.BaselineDate), AddBenefitsObj.RealisationStatus, Convert.ToDateTime(AddBenefitsObj.ActualRealisationDate),
                    AddBenefitsObj.TargetValue, AddBenefitsObj.AssumptionsText, AddBenefitsObj.BenefitRisks, AddBenefitsObj.NotesText);

                    TempData["success"] = "Record added Successfully";
                    return RedirectToAction("ProjectBenefits", "Projects", new { ProjectNo = AddBenefitsObj.ProjectNo });
                }
                else
                {
                    return View(AddBenefitsObj);
                }
            }
            catch (Exception ex)
            {
                TempData["failed"] = ex;
                return View(AddBenefitsObj);
            }
        }

        //Insert Project Risk
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProjectRisk(ProjectRisksModel AddRiskObj, string Command)
        {
            try {
                if (Command.Equals("Save"))
                {
                    dynamicsNAVSOAPServices.projectManagement.AddProjectRisk(AddRiskObj.ProjectCode, AddRiskObj.RiskDescription, AddRiskObj.ImpactDescription,
                        AddRiskObj.ImpactLevel, AddRiskObj.ProbabilityLevel, AddRiskObj.MitigationNotes, AddRiskObj.OwnerName, AddRiskObj.ObjectiveRelated,
                        Convert.ToDateTime(AddRiskObj.RiskStartDate), Convert.ToDateTime(AddRiskObj.RiskEndDate));
                    TempData["success"] = "Changes Saved Successfully";
                    return RedirectToAction("ProjectRisks", "Projects", new { ProjectNo = AddRiskObj.ProjectCode });
                }
                else
                {
                    return View(AddRiskObj);
                }
            }
            catch(Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //return View(AddRiskObj);
            }
        }
        //Delete A Risk  Line
        public JsonResult DeleteRiskLine(int LineNo)
        {
            bool lineDeleted = false;
            lineDeleted = dynamicsNAVSOAPServices.projectManagement.DeleteRiskLine(LineNo);
            return Json(lineDeleted, JsonRequestBehavior.AllowGet);
        }


        //Get Project Risk Line for edit
        [Authorize]
        public ActionResult EditProjectRisk(int Line)
        {
            try
            {
                if (Line.Equals(""))

            {
                //return RedirectToAction("ProjectRisks", "Projects", new { ProjectNo = ProjectNo });
                return RedirectToAction("ProjectInfo", "ProjectHome");
            }
           
                int myline = Line;
                var GetProjectRisk = from GetRiskQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectRiskRegister
                                     where GetRiskQuery.Line_No.Equals(Line)
                                     select GetRiskQuery;
                ProjectRisksModel RisksObj = new ProjectRisksModel();
                foreach (ProjectRiskRegister Lines in GetProjectRisk)
                {
                    RisksObj.LineNo = Lines.Line_No;
                    RisksObj.ProjectCode = Lines.Project_Code;
                    RisksObj.RiskDescription = Lines.Risk_Description;
                    RisksObj.ImpactDescription = Lines.Impact_Description;
                    RisksObj.ImpactLevel = Lines.Impact_Level ?? 0;
                    RisksObj.ProbabilityLevel = Lines.Probability_Level ?? 0;
                    RisksObj.PriorityLevel = Lines.Probability_Level ?? 0;
                    RisksObj.MitigationNotes = Lines.Mitigation_Notes;
                    RisksObj.OwnerName = Lines.Owner_Name;
                    RisksObj.ObjectiveRelated = Lines.Objective_Related;
                    //RisksObj.RiskStartDate = Lines.Risk_Start.Value.ToString("dd/MM/yyy;
                    RisksObj.RiskStartDate = Lines.Risk_Start.Value;
                    //RisksObj.RiskEndDate = Lines.Risk_End.Value.ToString("dd/MM/yyyy");
                    RisksObj.RiskEndDate = Lines.Risk_End.Value;

                }

                return View(RisksObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Edit Project Risk POST
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProjectRisk(ProjectRisksModel RisksObj, string Command)
        {
            try {
                if (Command.Equals("Save"))
                {

                    //string RiskStartDate = RisksObj.RiskStartDate.ToString();          
                    //DateTime test= DateTime.ParseExact(RiskStartDate, "MM/dd/yyy", System.Globalization.CultureInfo.InvariantCulture);                  


                    dynamicsNAVSOAPServices.projectManagement.ModifyProjectRisk(RisksObj.LineNo, RisksObj.ProjectCode, RisksObj.RiskDescription, RisksObj.ImpactDescription, RisksObj.ImpactLevel, RisksObj.ProbabilityLevel, RisksObj.MitigationNotes, RisksObj.OwnerName, RisksObj.ObjectiveRelated,Convert.ToDateTime(RisksObj.RiskStartDate),Convert.ToDateTime(RisksObj.RiskEndDate));
                    TempData["success"] = "Changes Saved Successfully";
                    return RedirectToAction("ProjectRisks", "Projects", new { ProjectNo = RisksObj.ProjectCode});
                }
                else
                {
                    return View(RisksObj);
                }
            }
            catch(Exception ex)
            {
                TempData["failed"] = ex;
                return View(RisksObj);
            }
         
        }

        //Get Project Tasks to Manage Page
        [Authorize]
        public ActionResult ManageProject(string ProjectNo)
        {
            try
            {
                if (ProjectNo.Equals(""))

            {
                //return RedirectToAction("ProjectRisks", "Projects", new { ProjectNo = ProjectNo });
                return RedirectToAction("ProjectInfo", "ProjectHome");
            }
            var StaffNo = AccountController.GetEmployeeNo();
            var TestRole = "PM";

            var MyRoleCheck = (from RoleQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                               where RoleQuery.No.Equals(StaffNo) && RoleQuery.Role.Equals(TestRole)
                              select RoleQuery).ToList();

            if (MyRoleCheck.Count<1)
            {
                return RedirectToAction("ProjectInfo", "ProjectHome");
                //return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = ProjectNo });
            }
            
            else { 
              
                    string myline = ProjectNo;
                    //Get Project Status
                    var GetProjectStatus = from StatusQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader
                                           where StatusQuery.Code.Equals(ProjectNo)
                                           select StatusQuery;

                    var Data = GetProjectStatus.FirstOrDefault();
                    var ProjectStatus = Data.Status;

                    var GetProjectTasks = from TaskQuery in dynamicsNAVODataServices.dynamicsNAVOData.ResourcesTasksLines
                                          where TaskQuery.Project_No.Equals(ProjectNo)
                                          select TaskQuery;
                    List<ProjectTasksModel> ManagerTasksObjList = new List<ProjectTasksModel>();
                    ViewBag.ProjectStatus = ProjectStatus;
                    TempData["ManagerProjectNo"] = ProjectNo;
                    ViewBag.ManagerProjectNumber = ProjectNo;
                    foreach (ResourcesTasksLines Lines in GetProjectTasks)
                    {
                        ProjectTasksModel TasksObj = new ProjectTasksModel();
                        TasksObj.ResourceNo = Lines.Resource_No;
                        TasksObj.ProjectNo = Lines.Project_No;
                        TasksObj.TaskDescription = Lines.Task_Description;
                        TasksObj.LineNo = Lines.Line_No;
                        TasksObj.AssignDate = Lines.Assign_Date.Value.ToString("dd/MM/yyyy");
                        TasksObj.ExpectedCompletionDate = Lines.Expected_Completion.Value.Date;
                        TasksObj.CompletionDate = Lines.Completion_Date.Value.ToString("dd/MM/yyyy");
                        TasksObj.PMRemarks = Lines.PM_Remarks;
                        TasksObj.DurationTaken = Lines.Duration_Taken;
                        TasksObj.OnlineFile = Lines.Online_File;
                        TasksObj.FileName = Lines.File_Name;
                        TasksObj.FileAttached = Lines.File_Attached ?? false;
                        TasksObj.MarkCompleted = Lines.Mark_Completed ?? false;
                        TasksObj.ConfirmCompleted = Lines.Confirm_Completed ?? false;
                        TasksObj.TaskSent = Lines.Task_Sent ?? false;
                        ManagerTasksObjList.Add(TasksObj);

                    }

                    return View(ManagerTasksObjList);                
               
              }
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Manager Viewing Tasks File
        public ActionResult DownloadTaskFile(string FileName)
        {
            try
            {
                string filename = FileName;
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "/ProjectManagementFiles/" + filename;
                byte[] filedata = System.IO.File.ReadAllBytes(filepath);
                string contentType = MimeMapping.GetMimeMapping(filepath);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = filename,
                    Inline = true,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(filedata, contentType);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
                //return Content("an error occurred");
            }
        }
        //Add Project Task, Load View
        [Authorize]
        [HttpGet]
        public ActionResult AddProjectTask(string ProjectNo)
        {
            try
            {
                if (ProjectNo.Equals(""))

            {
                return RedirectToAction("ManageProject", "Projects", new { ProjectNo = ProjectNo });
                //return RedirectToAction("ProjectInfo", "ProjectHome");
            }
            var StaffNo = AccountController.GetEmployeeNo();
            var TestRole = "PM";

            var MyRoleCheck = (from RoleQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                               where RoleQuery.No.Equals(StaffNo) && RoleQuery.Role.Equals(TestRole)
                               select RoleQuery).ToList();

            if (MyRoleCheck.Count < 1)
            {
                return RedirectToAction("ProjectInfo", "ProjectHome");
                //return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = ProjectNo });
            }

          
                ProjectTasksModel ProjectTaskObj = new ProjectTasksModel();
                LoadProjectTeamMembers();
                ProjectTaskObj.TeamMembers = new SelectList(_projectTeamMembers, "No", "Name");


                ProjectTaskObj.ProjectNo = ProjectNo;
                return View(ProjectTaskObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Add Project Task POST
        [Authorize]
        [HttpPost]
        public ActionResult AddProjectTask(ProjectTasksModel ProjectTasksObj, string Command)
        {
            LoadProjectTeamMembers();
            ProjectTasksObj.TeamMembers = new SelectList(_projectTeamMembers, "No", "Name", ProjectTasksObj.AssignedTo);
            if (Command == "Save")
            {
                try
                {          
                    dynamicsNAVSOAPServices.projectManagement.AddProjectTask(ProjectTasksObj.TaskDescription, ProjectTasksObj.AssignedTo, ProjectTasksObj.ProjectNo,ProjectTasksObj.ExpectedCompletionDate);
                   
                    TempData["saved"] = "Entry Saved Successfully";
                    return RedirectToAction("ManageProject", "Projects", new { ProjectNo = ProjectTasksObj.ProjectNo });
                    //return View(ProjectTasksObj);
                }
                catch (Exception ex)
                {

                    return View(ProjectTasksObj);
                }

            }
            return View(ProjectTasksObj);
        
    }

        #region Helper Functions


        [HttpPost]
        public ActionResult GetTeamMembers(string ProjectNo)
        {
            var GetProjectTeamNo = from PMQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader
                                   where PMQuery.Code.Equals(ProjectNo)
                                   select PMQuery;
            var Data = GetProjectTeamNo.FirstOrDefault();
            var PMCode = Data.Project_Management_Team;

            //int statId;

            LoadProjectTeamMembers();
            List<SelectListItem> PTeamMembers = new List<SelectListItem>();
            List<ProjectTeamMembers> team_ = _projectTeamMembers.Where(x => x.PM_Code == PMCode).ToList();
            team_.ForEach(x =>
            {
                PTeamMembers.Add(new SelectListItem { Text = x.Name, Value = x.No.ToString() });
            });
            return Json(PTeamMembers, JsonRequestBehavior.AllowGet);
           
        }
        //Get Manager Task-Edit View
        [Authorize]
        public ActionResult EditTaskManager(int LineNo)
        {
            try
            {
                int myline = LineNo;
                var GetTaskLines = from GetTM in dynamicsNAVODataServices.dynamicsNAVOData.ResourcesTasksLines
                                     where GetTM.Line_No.Equals(LineNo)
                                     select GetTM;
                ProjectTasksModel TaskLineObj = new ProjectTasksModel();
                foreach (ResourcesTasksLines Lines in GetTaskLines)
                {
                    TaskLineObj.TaskDescription = Lines.Task_Description;
                    TaskLineObj.ConfirmCompleted = Lines.Confirm_Completed ?? false;
                    TaskLineObj.LineNo = Lines.Line_No;
                    TaskLineObj.PMRemarks = Lines.PM_Remarks;
                    TaskLineObj.ProjectNo = Lines.Project_No;
                }               
                
                return View(TaskLineObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Post Manager Task-Edit View
        [Authorize]
        [HttpPost]
        public ActionResult EditTaskManager(ProjectTasksModel TasksObjectEdit)
        {            
            //var line = TasksObjectEdit.LineNo;
            //var checkCompletion = from ComQuery in dynamicsNAVODataServices.dynamicsNAVOData.ResourcesTasksLines
            //                      where ComQuery.Line_No.Equals(line)
            //                      select ComQuery;
            //var outputData = checkCompletion.First();
            //var completed = outputData.Mark_Completed;
            try
            {                
                //if (completed == true)
                //{

                    dynamicsNAVSOAPServices.projectManagement.ManagerEditTaskLines(TasksObjectEdit.LineNo, TasksObjectEdit.ConfirmCompleted, TasksObjectEdit.PMRemarks, TasksObjectEdit.TaskDescription);
                    TempData["success"] = "Changes Saved Successfully";
                    return RedirectToAction("ManageProject", "Projects", new { ProjectNo = TasksObjectEdit.ProjectNo });
                //}
                //else
                //{
                //    TempData["failed"] = "Failed!, This task has not been marked as complete by the person assigned";
                //    return RedirectToAction("ManageProject", "Projects", new { ProjectNo = TasksObjectEdit.ProjectNo });
                //}
            }
            catch (Exception ex)
            {
                TempData["failed"] = "Failed!, This task has not been marked as complete by the person assigned";
                return RedirectToAction("ManageProject", "Projects", new { ProjectNo = TasksObjectEdit.ProjectNo });
                //return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Delete A Task on Managers' view
        public JsonResult DeleteTaskManagerLine(int LineNo)
        {
            bool lineDeleted = false;
            lineDeleted = dynamicsNAVSOAPServices.projectManagement.DeleteTaskManagerLine(LineNo);
            return Json(lineDeleted, JsonRequestBehavior.AllowGet);
        }


        //Get Member Task Line for Edit 
        [Authorize]
        public ActionResult EditMemberTasks(int LineNo)
        {
            try
            {
                if (LineNo.Equals(""))

            {
                //return RedirectToAction("ManageProject", "Projects", new { ProjectNo = TasksObjectEdit.ProjectNo });
                return RedirectToAction("ProjectInfo", "ProjectHome");
            }
          
                int myline = LineNo;
                var GetMemberTaskLines = from GetMember in dynamicsNAVODataServices.dynamicsNAVOData.ResourcesTasksLines
                                   where GetMember.Line_No.Equals(LineNo)
                                   select GetMember;
                ProjectTasksModel MemberTaskLineObj = new ProjectTasksModel();
                foreach (ResourcesTasksLines Lines in GetMemberTaskLines)
                {
                    MemberTaskLineObj.TaskDescription = Lines.Task_Description;
                    MemberTaskLineObj.MarkCompleted = Lines.Mark_Completed ?? false;
                    MemberTaskLineObj.LineNo = Lines.Line_No;
                    MemberTaskLineObj.ProjectNo = Lines.Project_No;
                }

                return View(MemberTaskLineObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Get Attached Links for Member Tasks
        [Authorize]
        [HttpPost]
        public JsonResult GetAttachedLinks(int LineNo)
        {
            try
            {
                List<ProjectTasksModel> MemberTaskLineObjList = new List<ProjectTasksModel>();
                int myline = LineNo;
                var GetMemberTaskLines = from GetMember in dynamicsNAVODataServices.dynamicsNAVOData.ResourcesTasksLines
                                         where GetMember.Line_No.Equals(LineNo)
                                         select GetMember;
                ProjectTasksModel MemberTaskLineObj = new ProjectTasksModel();
                foreach (ResourcesTasksLines Lines in GetMemberTaskLines)
                {
                 
                    MemberTaskLineObj.FileAttached = Lines.File_Attached?? false;
                    MemberTaskLineObj.LineNo = Lines.Line_No;
                    MemberTaskLineObjList.Add(MemberTaskLineObj);
                }
                return Json(MemberTaskLineObjList, JsonRequestBehavior.AllowGet);
                //return View(MemberTaskLineObj);
            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                //return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Get Attached Links to Attached links Form
        [Authorize]
        [HttpPost]
        public JsonResult GetAttachedLinksTwo(int LineNo)
        {
          
            try
            {              
                int myline = LineNo;
                var GetMemberTaskLines = from GetMember in dynamicsNAVODataServices.dynamicsNAVOData.ResourcesTasksLines
                                         where GetMember.Line_No.Equals(LineNo)
                                         select GetMember;
                ProjectTasksModel MemberTaskLineObj = new ProjectTasksModel();
                foreach (ResourcesTasksLines Lines in GetMemberTaskLines)
                {

                    MemberTaskLineObj.FileAttached = Lines.File_Attached ?? false;
                    MemberTaskLineObj.LineNo = Lines.Line_No;                 
                }
                return Json(MemberTaskLineObj, JsonRequestBehavior.AllowGet);
                //return View(MemberTaskLineObj);
            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                //return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //[Authorize]
        [HttpPost]
        public JsonResult UploadAttachedLink(int LineNo)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var root = "~/ProjectManagementFiles/";
                    bool folderpath = System.IO.Directory.Exists(HttpContext.Server.MapPath(root));

                    if (!folderpath)
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Server.MapPath(root));
                    }
                    var file = Request.Files[0];
                    string fileExt = System.IO.Path.GetExtension(file.FileName).ToLower();
                    string random = Guid.NewGuid().ToString();
                    random = random.Remove(1, 6);
                    string fileName =LineNo + random + fileExt;
                    string path = System.IO.Path.Combine(HttpContext.Server.MapPath(root), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    file.SaveAs(path);

                    if (System.IO.File.Exists(path))
                    {
                        dynamicsNAVSOAPServices.projectManagement.UploadTasksDocument(LineNo,path,fileName);

                        return Json(new { message="uploaded" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { message = "uploaded" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //Delete Attached Link
        [HttpPost]
        public JsonResult DeleteAttachedLink(int LineNo)   
        {
            bool _AttachedLinksDeleted = false;         

            _AttachedLinksDeleted = dynamicsNAVSOAPServices.projectManagement.DeleteAttachedLinks(LineNo);

            return Json(new { AttachedLinkDeleted = _AttachedLinksDeleted }, JsonRequestBehavior.AllowGet);
        }

        //Post Member Task-Edit View
        [Authorize]
        [HttpPost]
        public ActionResult EditMemberTasks(ProjectTasksModel MemberTasksObjectEdit)
        {          
            try
            {
                //if (completed == true)
                //{

                dynamicsNAVSOAPServices.projectManagement.MemberEditTaskLines(MemberTasksObjectEdit.LineNo, MemberTasksObjectEdit.MarkCompleted);
                TempData["success"] = "Changes Saved Successfully";
                return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = MemberTasksObjectEdit.ProjectNo });               
            }
            catch (Exception ex)
            {
                //TempData["failed"] = "Failed!, This task has not been marked as complete by the person assigned";
                //return RedirectToAction("ManageProject", "Projects", new { ProjectNo = MemberTasksObjectEdit.ProjectNo });
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        
        //Action for Begin Project
        public JsonResult BeginProject(string ProjectNo)
        {

            var StaffNo = AccountController.GetEmployeeNo();

            var UserIs = from IdQuery in dynamicsNAVODataServices.dynamicsNAVOData.UserSetupQuery
                         where IdQuery.Employee_No.Equals(StaffNo)
                         select IdQuery;
            var Result = UserIs.FirstOrDefault();
            var User = Result.User_ID;
            try
            {
                var ProjectHeader = from PHQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader
                                      where PHQuery.Code.Equals(ProjectNo)
                                      select PHQuery;

                var DataP = ProjectHeader.FirstOrDefault();
                var approval = DataP.Approval_Status;
                var start = DataP.Status;

                if (approval!="Approved")
                {                   
                    return Json(new { message = "Notapproved" }, JsonRequestBehavior.AllowGet);
                }
                else 
                {
                   if (start == "Ongoing")
                    {
                        return Json(new { message = "Ongoing" }, JsonRequestBehavior.AllowGet);
                    }
                   else if(start == "Cancelled")
                    {
                        return Json(new { message = "Cancelled" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (start == "Completed")
                    {
                        return Json(new { message = "Completed" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (start == "Not Started")
                    {
                        dynamicsNAVSOAPServices.projectManagement.BeginProject(ProjectNo, User);
                        return Json(new { message = "Success" }, JsonRequestBehavior.AllowGet);
                       
                    }                  

                }
                return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {              
                return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }
        
        //View for CLose Project
        [Authorize]
        public ActionResult ProjectsClosure()
        {
            var StaffNo = AccountController.GetEmployeeNo();
            List<ProjectManagementModel> ProjectClosureList = new List<ProjectManagementModel>();
            try
            {
                var Myteam = from Myteamlist in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                             where Myteamlist.No.Equals(StaffNo) && Myteamlist.Role.Equals("PM")
                             select Myteamlist;


                foreach (ProjectTeamMembers Lines in Myteam)
                {
                    var ProjectTeamCode = Lines.PM_Code;


                    var ProjectHeader = from PHeaderQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader
                                        where PHeaderQuery.Project_Management_Team.Equals(ProjectTeamCode)
                                        select PHeaderQuery;
                    foreach (ProjectHeader ProjectLines in ProjectHeader)
                    {
                        var ProjectNumber = ProjectLines.Code;
                        var ClosureListQuery = from ClosureListData in dynamicsNAVODataServices.dynamicsNAVOData.ProjectsClosure
                                               where ClosureListData.Project_Code.Equals(ProjectNumber)
                                               select ClosureListData;

                        foreach (ProjectsClosure ClosureLines in ClosureListQuery)
                        {
                            ProjectManagementModel ProjectClosureLinesObj = new ProjectManagementModel();
                            ProjectClosureLinesObj.ProjectCode = ClosureLines.Project_Code;
                            ProjectClosureLinesObj.ProjectDescription = ClosureLines.Project_Description;
                            ProjectClosureLinesObj.ProjectStartDate = ClosureLines.Project_Start.Value.ToString("dd/MM/yyyy");
                            ProjectClosureLinesObj.ProjectEndDate = ClosureLines.Project_End.Value.ToString("dd/MM/yyyy");
                            ProjectClosureLinesObj.ApprovalStatus = ClosureLines.Approval_Status;
                            ProjectClosureList.Add(ProjectClosureLinesObj);
                        }

                    }                   
                }
                return View(ProjectClosureList);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Add Project to Closure List View
        [Authorize]       
        public ActionResult AddClosureProject()
        {          
            try
            {
                ProjectManagementModel MyProjectsListObj = new ProjectManagementModel();
                LoadMyProjects();
                MyProjectsListObj.ProjectsListed= new SelectList(_myownProjects, "Code", "Description");
                return View(MyProjectsListObj);
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }
        //Add Project to Closure List POST
        [Authorize]
        [HttpPost]
        public ActionResult AddClosureProject(ProjectManagementModel ClosureObj)
        {
           // LoadMyProjects();
           // ClosureObj.ProjectsListed = new SelectList(_myownProjects, "Code", "Description", ClosureObj.);
            try
            {
                dynamicsNAVSOAPServices.projectManagement.AddClosureProject(ClosureObj.ProjectListItem);
                //TempData["success"] = "Changes Saved Successfully";
                //return RedirectToAction("ProjectMemberTasks", "Projects", new { ProjectNo = MemberTasksObjectEdit.ProjectNo });
                //ProjectManagementModel MyProjectsListObj = new ProjectManagementModel();
                //LoadMyProjects();
                // MyProjectsListObj.ProjectsListed = new SelectList(_myownProjects, "Code", "Description");
                return View();
            }
            catch (Exception ex)
            {
                return errorResponse.ApplicationExceptionError(ex);
            }
        }

        //Action for Complete Project
        [Authorize]
        [HttpPost]
        public JsonResult CompleteProject(string ProjectNoComplete)
        {

            var StaffNo = AccountController.GetEmployeeNo();

            var UserIs = from IdQuery in dynamicsNAVODataServices.dynamicsNAVOData.UserSetupQuery
                         where IdQuery.Employee_No.Equals(StaffNo)
                         select IdQuery;
            var Result = UserIs.FirstOrDefault();
            var User = Result.User_ID;
            try
            {
                var ProjectHeader = from PHQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader
                                    where PHQuery.Code.Equals(ProjectNoComplete)
                                    select PHQuery;

                var DataP = ProjectHeader.FirstOrDefault();
                var approval = DataP.Approval_Status;
                var start = DataP.Status;

                if (approval != "Approved")
                {
                    return Json(new { message = "Notapproved" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (start == "Ongoing")
                    {
                        dynamicsNAVSOAPServices.projectManagement.CompleteProject(ProjectNoComplete, User);
                        return Json(new { message = "Success" }, JsonRequestBehavior.AllowGet);
                        
                    }
                    else if (start == "Cancelled")
                    {
                        return Json(new { message = "Cancelled" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (start == "Completed")
                    {
                        return Json(new { message = "Completed" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (start == "Not Started")
                    {
                        return Json(new { message = "NotStarted" }, JsonRequestBehavior.AllowGet);
                    }

                }
                return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }


        //Action for Cancel Project
        public JsonResult CancelProject(string ProjectNo)
        {

            var StaffNo = AccountController.GetEmployeeNo();

            var UserIs = from IdQuery in dynamicsNAVODataServices.dynamicsNAVOData.UserSetupQuery
                         where IdQuery.Employee_No.Equals(StaffNo)
                         select IdQuery;
            var Result = UserIs.FirstOrDefault();
            var User = Result.User_ID;
            try
            {
                var ProjectHeader = from PHQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader
                                    where PHQuery.Code.Equals(ProjectNo)
                                    select PHQuery;

                var DataP = ProjectHeader.FirstOrDefault();
                //var approval = DataP.Approval_Status;
                var start = DataP.Status;               
             
                    if (start == "Cancelled")
                    {
                        return Json(new { message = "Cancelled" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (start == "Completed")
                    {
                        return Json(new { message = "Completed" }, JsonRequestBehavior.AllowGet);
                    }

                     else
                    {                      
                  
                        dynamicsNAVSOAPServices.projectManagement.CancelProject(ProjectNo, User);
                        return Json(new { message = "Success" }, JsonRequestBehavior.AllowGet);

                    }

            }
            catch (Exception ex)
            {
                return Json(new { message = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult GetObjectives(string StrategicCode)
        {
            //int statId;
            LoadStrategicObjectives();
            List<SelectListItem> Objectives = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(StrategicCode))
            {
                //statId = Convert.ToInt32(Qualificationcode);
                List<StrategicObjectives> objs_ = _strategicObjectives.Where(x => x.Plan_Code == StrategicCode).ToList();
                objs_.ForEach(x =>
                {
                    Objectives.Add(new SelectListItem { Text = x.Objective_Description, Value = x.Objective_Code.ToString() });
                });
            }
            return Json(Objectives, JsonRequestBehavior.AllowGet);
        }

        //Load Team Members for a particular Project

        private void LoadProjectTeamMembers()
        {
            var StaffNo = AccountController.GetEmployeeNo();
            _projectTeamMembers = from TeamQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                                  where TeamQuery.No !=StaffNo          
                                    select TeamQuery;
        }
        //Load Projects of a Project Manager      
        private void LoadMyProjects()
        {
            //_myownProjects = from PHeaderQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader                              
            //                   select PHeaderQuery;  

            var StaffNo = AccountController.GetEmployeeNo();
            var Myteam = from Myteamlist in dynamicsNAVODataServices.dynamicsNAVOData.ProjectTeamMembers
                         where Myteamlist.No.Equals(StaffNo) && Myteamlist.Role.Equals("PM")
                         select Myteamlist;

            foreach (ProjectTeamMembers Lines in Myteam)
            {
                var ProjectTeamCode = Lines.PM_Code;

                _myownProjects = from PHeaderQuery in dynamicsNAVODataServices.dynamicsNAVOData.ProjectHeader
                                 where PHeaderQuery.Project_Management_Team.Equals(ProjectTeamCode)
                                 select PHeaderQuery;
            }


        }

        //Load Strategic Plans
        private void LoadStrategicPlans()
        {

            _strategicPlans = from plansQuery in dynamicsNAVODataServices.dynamicsNAVOData.StrategicPlan
                                  select plansQuery;
        }
        //Load Strategic Objectives
        private void LoadStrategicObjectives()
        {

            _strategicObjectives = from objectivesQuery in dynamicsNAVODataServices.dynamicsNAVOData.StrategicObjectives
                              select objectivesQuery;
        }

        #endregion End Helper Functions
    }
}