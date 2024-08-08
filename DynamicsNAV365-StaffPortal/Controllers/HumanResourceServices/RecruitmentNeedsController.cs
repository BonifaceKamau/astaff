using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DynamicsNAV365_StaffPortal.CodeHelpers;
using DynamicsNAV365_StaffPortal.Controllers.ResponseServices;
using DynamicsNAV365_StaffPortal.Models.HumanResource;
using DynamicsNAV365_StaffPortal.Models.HumanResource.TimeSheets;
using OdataRef;
using AcademicRequirements = OdataRef.AcademicRequirements;
using DimensionValues = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.DimensionValues;
using EmployeeLeaveTypes = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.EmployeeLeaveTypes;
using Employees = DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference.Employees;

namespace DynamicsNAV365_StaffPortal.Controllers.HumanResourceServices
{
    [NoCache]
    public class RecruitmentNeedsController : Controller
    {
        static string companyName = ServiceConnection.CompanyName;
        static string companyURL = "";

        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        BCODATAServices _bcodataServices = new BCODATAServices(companyURL);

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

        IQueryable<Employees> employees = null;
        IQueryable<EmployeeLeaveTypes> employeeLeaveTypes = null;
        IQueryable<DimensionValues> globalDimension1Values = null;
        IQueryable<DimensionValues> globalDimension2Values = null;
        IQueryable<DimensionValues> shortcutDimension3Values = null;
        IQueryable<DimensionValues> shortcutDimension4Values = null;
        IQueryable<DimensionValues> shortcutDimension5Values = null;
        IQueryable<DimensionValues> shortcutDimension6Values = null;
        IQueryable<DimensionValues> shortcutDimension7Values = null;

        IQueryable<DimensionValues> shortcutDimension8Values = null;
        //IQueryable<ResponsibilityCenters> responsibilityCenters = null;

        AccountController accountController = new AccountController();
        string employeeNo = "";

        public RecruitmentNeedsController()
        {
            employeeNo = AccountController.GetEmployeeNo();
        }

        public ActionResult RecruitmentNeeds()
        {
            var nav = _bcodataServices.BCOData;
            var userId = nav.UserSetupQuery.Where(c => c.Employee_No == employeeNo).FirstOrDefault()?.User_ID;
            var recruitmentNeedsList = nav.RecruitmentNeeds.AsEnumerable().Where(c=>c.Employee_No == employeeNo || c.Requested_By == userId).ToList();
            return View(recruitmentNeedsList);
        }

        public ActionResult NewRecruitmentNeed()
        {
            var nav = _bcodataServices.BCOData;
            var userId = nav.UserSetupQuery.Where(c => c.Employee_No == employeeNo).FirstOrDefault()?.User_ID;
            var recruitmentNeed = new OdataRef.RecruitmentNeeds
            {
                No = "",
                Employee_No = employeeNo,
                Requested_By = userId
                //DateTimeAdded = DateTime.Now
            };
            //var df= OdataRef.RecruitmentNeeds.CreateRecruitmentNeeds("");
            nav.AddToRecruitmentNeeds(recruitmentNeed);
            nav.SaveChanges();
            return RedirectToAction("ViewRecruitmentNeed", new {id = recruitmentNeed.No});
        }

        [HttpGet]
        public ActionResult ViewRecruitmentNeed(string id)
        {
            var nav = _bcodataServices.BCOData;
            var recruitmentNeeds = nav.RecruitmentNeeds
                .Where(c => c.No == id).FirstOrDefault();
            var staffRecruitmentNeeds = MapperHelper.Map<StaffRecruitmentNeeds>(recruitmentNeeds);
            staffRecruitmentNeeds.Job_ID_Select = (nav.CompanyJobList.AsEnumerable() ?? Array.Empty<CompanyJobList>()).Select(c =>
                new SelectListItem()
                {
                    Value = c.Job_ID,
                    Text = $"{c.Job_ID}:{c.Job_Description}",
                    Selected = staffRecruitmentNeeds.Job_ID == c.Job_ID
                });
            staffRecruitmentNeeds.Appointment_Type_Select =
                (nav.EmploymentContracts.AsEnumerable() ?? Array.Empty<EmploymentContracts>()).Select(c =>
                    new SelectListItem()
                    {
                        Value = c.Code,
                        Text = c.Description,
                        Selected = staffRecruitmentNeeds.Appointment_Type == c.Code
                    });
            staffRecruitmentNeeds.Location_Select = (nav.Locations.AsEnumerable() ?? Array.Empty<Locations>()).Select(
                c => new SelectListItem()
                {
                    Value = c.Code,
                    Text = c.Name,
                    Selected = staffRecruitmentNeeds.Appointment_Type == c.Code
                });
            staffRecruitmentNeeds.Reporting_To_Select = (nav.CompanyJobList.AsEnumerable() ?? Array.Empty<CompanyJobList>()).Select(
                c => new SelectListItem()
                {
                    Value = c.Job_ID,
                    Text = $"{c.Job_ID}:{c.Job_Description}",
                    Selected = staffRecruitmentNeeds.Reporting_To == c.Job_ID
                });
            staffRecruitmentNeeds.Requisition_Type_Select =
                new Dictionary<string, string> {{"Internal", "Internal"}, {"Open", "Open"}, {"External", "External"}}.Select(c =>
                    new SelectListItem()
                    {
                        Value = c.Key.ToString(),
                        Text = c.Value,
                        Selected = staffRecruitmentNeeds.Appointment_Type == c.Value
                    });
            staffRecruitmentNeeds.Reason_for_Recruitment_Select = new Dictionary<string, string>
                {{"", ""}, {"New Position", "New Position"}, {"Existing Position", "Existing Position"}}.Select(c => new SelectListItem()
            {
                Value = c.Key.ToString(),
                Text = c.Value,
                Selected = staffRecruitmentNeeds.Appointment_Type == c.Value
            });
            return View(staffRecruitmentNeeds);
        }

        [HttpPost]
        public ActionResult ViewRecruitmentNeed(OdataRef.RecruitmentNeeds RecruitmentNeeds)
        {
            try
            {
                var nav = _bcodataServices.BCOData;
                var recruitmentNeeds = nav.RecruitmentNeeds.Where(c => c.No == RecruitmentNeeds.No).FirstOrDefault();
                if (recruitmentNeeds != null)
                {
                    recruitmentNeeds.Job_ID = RecruitmentNeeds.Job_ID;
                    recruitmentNeeds.Requisition_Type = RecruitmentNeeds.Requisition_Type;
                    recruitmentNeeds.Location = RecruitmentNeeds.Location;
                    recruitmentNeeds.Reason_for_Recruitment = RecruitmentNeeds.Reason_for_Recruitment;
                    recruitmentNeeds.Appointment_Type = RecruitmentNeeds.Appointment_Type;
                    recruitmentNeeds.Reporting_To = RecruitmentNeeds.Reporting_To;
                    recruitmentNeeds.NoOfPositions = RecruitmentNeeds.NoOfPositions;
                    nav.UpdateObject(recruitmentNeeds);
                }

                nav.SaveChanges();
                TempData["Success"] = "Saved successfully";
                return RedirectToAction("RecruitmentNeeds");
            }
            catch (Exception e)
            {
                TempData["Error"] = e.InnerException?.Message??e.Message;
                return RedirectToAction("ViewRecruitmentNeed", new {id=RecruitmentNeeds.No});
            }
        }
        

        public ActionResult AcademicRequirements(string Id)
        {
            var nav = _bcodataServices.BCOData;
            ViewBag.Need_Id = Id;
            var requirementsEnumerable = nav.AcademicRequirements.Where(c => c.Need_Id == Id);
            return PartialView(requirementsEnumerable);
        }

        public ActionResult OtherRequirements(string Id)
        {
            var nav = _bcodataServices.BCOData;
            ViewBag.Id = Id;
            var requirementsEnumerable = nav.RecruitmentNeedsOther_Qualifications.AsEnumerable().Where(c => c.Code == Id).ToList();
            return PartialView(requirementsEnumerable);
        }

        public ActionResult Skills(string Id)
        {
            var nav = _bcodataServices.BCOData;
            ViewBag.Id = Id;
            var requirementsEnumerable = nav.RecruitmentNeedsSkills.Where(c => c.Recruitment_Need_Code == Id).AsEnumerable();
            return PartialView(requirementsEnumerable);
        }

        public ActionResult MandatoryDocuments(string Id)
        {
            var nav = _bcodataServices.BCOData;
            var requirementsEnumerable = nav.Recruitment_RequestMandatory_Documents.Where(c => c.Document_No == Id);
            return PartialView(requirementsEnumerable);
        }

        public ActionResult JobResponsibility(string Id)
        {
            var nav = _bcodataServices.BCOData;
            ViewBag.Id = Id;
            var requirementsEnumerable = nav.RecruitmentNeedsKPA.Where(c => c.Job_ID == Id);
            return PartialView(requirementsEnumerable);
        }
        //[HttpGet]
        public ActionResult AddAcademicQualification(string id, int? lineNo)
        {
            var nav = _bcodataServices.BCOData;
            var jobAcademicRequirement = nav.AcademicRequirements.AsEnumerable().Where(c => c.Need_Id == id && c.Line_No == lineNo).FirstOrDefault();
            var qualifications = MapperHelper.Map<AcademicQualifications>(jobAcademicRequirement??new AcademicRequirements());
            qualifications.Education_Level_Id_Select = nav.AcademicEducationLevel.AsEnumerable().Select(c =>
                new SelectListItem
                {
                    Text = c.Description,
                    Value = c.EducationLevelID.ToString(),
                    Selected = jobAcademicRequirement?.Education_Level_Id == c.EducationLevelID
                }).ToList();
            qualifications.Course_Id_Select = nav.Academic_Certificates.AsEnumerable().Select(c =>
                new SelectListItem
                {
                    Text = c.CertificateName,
                    Value = c.s.ToString(),
                    Selected = jobAcademicRequirement?.Course_Id == c.s
                }).ToList();
            qualifications.Need_Id = id;
            return PartialView(qualifications);
        }

        public ActionResult AcademicRequirementsModels(string Id)
        {
            var nav = _bcodataServices.BCOData;
            var jobAcademicRequirement = nav.AcademicRequirements.Where(c => c.Need_Id == Id);
            return PartialView("_academicRequirementsModels",jobAcademicRequirement);
        }
        [HttpPost]
        public ActionResult SaveAcademicQualifications(NeedsRequirements AcademicQualifications)
        {
            try
            {
                var nav = _bcodataServices.BCOData;
                AcademicQualifications.Qualification_Type = "Academic";
                if (AcademicQualifications.Line_No == null ||AcademicQualifications.Line_No ==0)
                {
                    nav.AddToNeedsRequirements(AcademicQualifications);
                    nav.SaveChanges();
                    return Json(new {success=true}, JsonRequestBehavior.AllowGet);
                }

                var needsRequirements = nav.NeedsRequirements.Where(c => c.Line_No == AcademicQualifications.Line_No).FirstOrDefault();
                if (needsRequirements != null)
                {
                    needsRequirements.Education_Level_Id = AcademicQualifications.Education_Level_Id;
                    needsRequirements.Course_Id = AcademicQualifications.Course_Id;
                    needsRequirements.Mandatory = AcademicQualifications.Mandatory;
                    nav.UpdateObject(needsRequirements);
                }

                nav.SaveChanges();
                return Json(new {success=true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success=false,message=e.InnerException?.Message??e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteAcademicQualification(int lineNo)
        {
            try
            {
                var nav = _bcodataServices.BCOData;
                var needsRequirement = nav.NeedsRequirements.Where(c => c.Line_No == lineNo).FirstOrDefault();
                if (needsRequirement == null)
                {
                    return Json(new {saved=false, messsage= "object not found"}, JsonRequestBehavior.AllowGet);
                }
                nav.DeleteObject(needsRequirement);
                nav.SaveChanges();
                return Json(new {success=true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success=false,messsage=e.InnerException?.Message??e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddOtherRequirement(string id, string qualification)
        {
            var nav = _bcodataServices.BCOData;
            var jobAcademicRequirement = nav.RecruitmentNeedsOther_Qualifications.AsEnumerable().Where(c => c.Code == id && c.Qualification == qualification).FirstOrDefault();
            if (jobAcademicRequirement == null)
            {
                return PartialView(new RecruitmentNeedsOther_Qualifications()
                {
                    Code = id
                });
            }

            ViewBag.Need_Id = id;
            jobAcademicRequirement.Code = id;
            return PartialView(jobAcademicRequirement);
        }

        public ActionResult DeleteOtherRequirement(string Id,string Qualification)
        {
            try
            {
                var nav = _bcodataServices.BCOData;
                var otherRequirement = nav.RecruitmentNeedsOther_Qualifications.Where(c => c.Code == Id && c.Qualification == Qualification).FirstOrDefault();
                if (otherRequirement == null)
                {
                    return Json(new {saved=false, messsage= "object not found"}, JsonRequestBehavior.AllowGet);
                }
                nav.DeleteObject(otherRequirement);
                nav.SaveChanges();
                return Json(new {success=true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success=false,messsage=e.InnerException?.Message??e.Message}, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SaveOtherRequirements(RecruitmentNeedsOther_Qualifications OtherRequirements,string OldQualification)
        {
            try
            {
                var nav = _bcodataServices.BCOData;

                var otherRequirement = nav.RecruitmentNeedsOther_Qualifications.AsEnumerable().Where(c => c.Code == OtherRequirements.Code && c.Qualification== OldQualification).FirstOrDefault();
                if (otherRequirement != null)
                {
                    otherRequirement.Qualification = OtherRequirements.Qualification;
                    nav.UpdateObject(otherRequirement);
                nav.SaveChanges();
                return Json(new {success=true}, JsonRequestBehavior.AllowGet);
                }
                nav.AddToRecruitmentNeedsOther_Qualifications(OtherRequirements);
                nav.SaveChanges();
                return Json(new {success=true}, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new {success=false,message=e.InnerException?.Message??e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult OtherRequirementsModels(string Id)
        {
            var nav= _bcodataServices.BCOData;
            var otherRequirements = nav.RecruitmentNeedsOther_Qualifications.AsEnumerable().Where(c => c.Code == Id).ToList();
            return PartialView("_OtherRequirementsModels",otherRequirements);
        }

        public ActionResult AddSkill(string id,int? lineNo)
        {
            var nav = _bcodataServices.BCOData;
            var needsSkills = nav.RecruitmentNeedsSkills.AsEnumerable().Where(c => c.Line_No == lineNo).FirstOrDefault();
            if (needsSkills == null)
            {
                return PartialView(new RecruitmentNeedsSkills()
                {
                    Recruitment_Need_Code = id
                });
            }

            ViewBag.Need_Id = id;
            needsSkills.Recruitment_Need_Code = id;
            return PartialView(needsSkills);
        }

        public ActionResult DeleteSkills(int Id)
        {
            try
            {
                var nav = _bcodataServices.BCOData;
                var needsSkills = nav.RecruitmentNeedsSkills.Where(c => c.Line_No == Id).FirstOrDefault();
                if (needsSkills == null)
                {
                    return Json(new {saved=false, messsage= "object not found"}, JsonRequestBehavior.AllowGet);
                }
                nav.DeleteObject(needsSkills);
                nav.SaveChanges();
                return Json(new {success=true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success=false,messsage=e.InnerException?.Message??e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveSkills(RecruitmentNeedsSkills Skills)
        {
            try
            {
                var nav = _bcodataServices.BCOData;

                var needsSkills = nav.RecruitmentNeedsSkills.Where(c => c.Line_No == Skills.Line_No).FirstOrDefault();
                if (needsSkills != null)
                {
                    needsSkills.Name = Skills.Name;
                    needsSkills.Description = Skills.Description;
                    needsSkills.Remarks = Skills.Remarks;
                    needsSkills.Mandatory = Skills.Mandatory;
                    nav.UpdateObject(needsSkills);
                nav.SaveChanges();
                return Json(new {success=true}, JsonRequestBehavior.AllowGet);
                }
                nav.AddToRecruitmentNeedsSkills(Skills);
                nav.SaveChanges();
                return Json(new {success=true}, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new {success=false,message=e.InnerException?.Message??e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SkillsModels(string Id)
        {
            var nav= _bcodataServices.BCOData;
            var skillsList = nav.RecruitmentNeedsSkills.AsEnumerable().Where(c => c.Recruitment_Need_Code == Id);
            return PartialView("_SkillModel",skillsList);
        }
        
        //job responsiblity
        public ActionResult AddJobResponsibility(int? lineNo,string jobId)
        {
            var nav = _bcodataServices.BCOData;
            var responsibility = nav.RecruitmentNeedsKPA.AsEnumerable().Where(c => c.Line_No == lineNo).FirstOrDefault();
            if (responsibility == null)
            {
                return PartialView(new RecruitmentNeedsKPA()
                {
                    Job_ID = jobId,
                });
            }

            ViewBag.Job_ID = jobId;
            responsibility.Job_ID = jobId;
            return PartialView(responsibility);
        }

        public ActionResult DeleteJobResponsibility(int Id)
        {
            try
            {
                var nav = _bcodataServices.BCOData;
                var needsSkills = nav.RecruitmentNeedsKPA.Where(c => c.Line_No == Id).FirstOrDefault();
                if (needsSkills == null)
                {
                    return Json(new {saved=false, messsage= "object not found"}, JsonRequestBehavior.AllowGet);
                }
                nav.DeleteObject(needsSkills);
                nav.SaveChanges();
                return Json(new {success=true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success=false,messsage=e.InnerException?.Message??e.Message}, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SaveJobResponsibility(RecruitmentNeedsKPA resp)
        {
            try
            {
                var nav = _bcodataServices.BCOData;

                var responsiblity = nav.RecruitmentNeedsKPA.Where(c => c.Line_No == resp.Line_No).FirstOrDefault();
                if (responsiblity != null)
                {
                    responsiblity.Job_ID = resp.Job_ID;
                    responsiblity.Responsibility = resp.Responsibility;
                    nav.UpdateObject(responsiblity);
                nav.SaveChanges();
                return Json(new {success=true}, JsonRequestBehavior.AllowGet);
                }
                nav.AddToRecruitmentNeedsKPA(resp);
                nav.SaveChanges();
                return Json(new {success=true}, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new {success=false,message=e.InnerException?.Message??e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult JobResponsibilityModels(string Id)
        {
            var nav= _bcodataServices.BCOData;
            var responsibilities = nav.RecruitmentNeedsKPA.AsEnumerable().Where(c => c.Job_ID == Id);
            return PartialView("_ResponsibilityModel",responsibilities);
        }

        public ActionResult RecruitmentNeedsSendApproval(string Id)
        {
            try
            {
                dynamicsNAVSOAPServices.ApprovalsMgmt.SendApprovalRequest(Id);
                    TempData["Success"] = "Document Sucsessfully sent for approval";
                return RedirectToAction("RecruitmentNeeds");
            }
            catch (Exception e)
            {
                TempData["Error"] =$"Error: {e.InnerException?.Message??e.Message}";
                return RedirectToAction("RecruitmentNeeds");
            }
        }
    }
}