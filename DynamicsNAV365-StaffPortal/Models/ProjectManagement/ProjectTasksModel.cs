using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.ProjectManagement
{
    public class ProjectTasksModel
    {
        public string ResourceNo { get; set; }

        [Display (Name="Project Number")]
        public string ProjectNo { get; set; }

        public int LineNo { get; set; }

        [Display(Name = "Project Title")]
        public string ProjectTitle { get; set; }

        [Display(Name = "Task")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Task Description is  Required")]
        public string TaskDescription { get; set; }

        [Display(Name = "Assigned On")]
        public string AssignDate { get; set; }        

        [Display(Name = "Expected Completion Date")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{MM/dd/yyyy}")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is  Required")]
        public DateTime ExpectedCompletionDate { get; set; }

        [Display(Name = "Completion Date")]
        public string CompletionDate { get; set; }

        [Display(Name = "PM Remarks")]
        public string PMRemarks { get; set; }

        [Display(Name = "Duration")]
        public string DurationTaken { get; set; }

        public string OnlineFile { get; set; }

        [Display(Name = "Assigned To")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public string AssignedTo { get; set; }
        public IEnumerable<SelectListItem> TeamMembers { get; set; }

        [Display(Name = "Mark Completed")]
        public bool MarkCompleted { get; set; }

        [Display(Name = "Confirm Completed")]
        public bool ConfirmCompleted { get; set; }

        [Display(Name = "Task Sent")]
        public bool TaskSent { get; set; }

        [Display(Name = "File Attached")]
        public bool FileAttached { get; set; }

        public string AttachedFileName { get; set; }
        public string FileName { get; set; }

    }
    }