using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.ProjectManagement
{
    public class ProjectManagementModel
    {
        public string Code { get; set; }
        public string ActivityCode { get; set; }
        public string StrategyCode { get; set; }
        public string ObjectiveCode { get; set; }

        public string ProjectManagementTeam { get; set; }
        public string Status { get; set; }

        [Display(Name = "Project Code")]
        public string ProjectCode { get; set; }

        [Display(Name = "Project Description")]
        public string ProjectDescription { get; set; }

        [Display(Name = "Project Start Date")]
        public string ProjectStartDate { get; set; }

        [Display(Name = "Project End Date")]
        public string ProjectEndDate { get; set; }

        [Display(Name = "Approval Status")]
        public string ApprovalStatus { get; set; }

        [Display(Name = "List of my Projects")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public string ProjectListItem { get; set; }
        public IEnumerable<SelectListItem> ProjectsListed { get; set; }



        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        [Display(Name = "Cost")]
        public decimal Cost { get; set; }


        [Display(Name = "Activity")]
        public string Activity { get; set; }

        [Display(Name = "Activity Start Date")]
        public string ActivityStartDate { get; set; }

        [Display(Name = "Activity End Date")]
        public string ActivityEndDate { get; set; }


        [Display(Name = "Activity Status")]
        public string ActivityStatus { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        [Display(Name = "Strategic Objective")]
        public string StrategicObjective { get; set; }

        //Project Team Members
        public string TeamMemberCode { get; set; }

        [Display(Name = "Team Member's Name")]
        public string TeamMemberName { get; set; }

        [Display(Name = "Team Member Type")]
        public string TeamMemberType { get; set; }


        [Display(Name = "Team Member Role")]
        public string TeamMemberRole { get; set; }


        [Display(Name = "Role Description")]
        public string RoleDescription { get; set; }

        public string MemberRole { get; set; }



    }
}