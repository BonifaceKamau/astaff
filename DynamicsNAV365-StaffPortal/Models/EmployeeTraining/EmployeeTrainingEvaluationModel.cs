using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.EmployeeTraining
{
	public class EmployeeTrainingEvaluationModel
	{
        [Display(Name = "Training Evaluation No.")]
        public string TrainingEvaluationNo { get; set; }

        [Display(Name = "Training to Evaluate")]
        public string ApplicationNo { get; set; }
        public SelectList Applications { get; set; }

        public string EmployeeNo { get; set; }

        public string EmployeeName { get; set; }

        [Display(Name = "Training Calender Year")]
        public string CalenderYear { get; set; }
        public SelectList YearCodes { get; set; }

        [Display(Name = "Development Need")]
        public string DevelopmentNeed { get; set; }

        [Display(Name = "Training Provider.")]
        public string TrainingProvider { get; set; }

        [Display(Name = "Training Venue/Location")]
        public string TrainingLocation { get; set; }

        public string Objectives { get; set; }

        [Display(Name = "Brief Comment about the Training")]
        public string Comments { get; set; }

        [Display(Name = "Start Date")]
        public string TrainingStartDate { get; set; }

        [Display(Name = "End Date.")]
        public string TrainingEndDate { get; set; }

        [Display(Name = "Objective of the Training was Clearly Defined")]
        public string ObjectiveMet { get; set; }
        public SelectList ObjectivesMet { get; set; }

        [Display(Name = "Participation & Interraction Strongly Encouraged")]
        public string ParticipationEncouraged { get; set; }
        public SelectList Participations { get; set; }

        [Display(Name = "Topics Covered were Relevant")]
        public string TopicsCovered { get; set; }
        public SelectList Topics { get; set; }

        [Display(Name = "Content was organised and easy to follow")]
        public string ContentOrganised { get; set; }
        public SelectList Contents { get; set; }

        [Display(Name = "Material Distributed was Helpful")]
        public string MaterialDistributed { get; set; }
        public SelectList Materials { get; set; }

        [Display(Name = "Training Experience will be Useful in my work")]
        public string TrainingExperience { get; set; }
        public SelectList TrainingExperiences { get; set; } 

        [Display(Name = "Trainer was Knowledgeable about the training topics")]
        public string TrainerKnowledgeable { get; set; }
        public SelectList Trainers { get; set; }

        [Display(Name = "Trainer was well Prepared")]
        public string TrainerWellPrepared { get; set; }
        public SelectList TrainerPreparedness { get; set; }

        [Display(Name = "Training Objectives")]
        public string TrainingObjective { get; set; }
        public SelectList TrainingObjectives { get; set; } 

        [Display(Name = "How do you rate the overal Training")]
        public string Rate { get; set; }
        public SelectList Ratings { get; set; }

        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public bool ErrorStatus { get; set; }
    }
}