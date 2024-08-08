using System.Collections.Generic;
using System.Web.Mvc;
using OdataRef;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource
{
    public class TrainingSurveyDto:Training_Survey_Card
    {
        public IEnumerable<SelectListItem> Department_Code_Select{ get; set; }
        public IEnumerable<SelectListItem> Training_No_Select { get; set; }
        public IEnumerable<SelectListItem> Training_Objective_Select { get; set; }
        public IEnumerable<SelectListItem> Helpful_in_Productivity_Select { get; set; }
        public IEnumerable<SelectListItem> Did_Trainer_Show_Experience_Select { get; set; }
        public IEnumerable<SelectListItem> Did_Trainer_Deliver_Promise_Select { get; set; }
        public IEnumerable<SelectListItem> Trainer_answer_questions_Select { get; set; }
        public IEnumerable<SelectListItem> Trainer_Recommendation_Select { get; set; }
    }
}