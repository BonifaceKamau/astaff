using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.PerformanceManagement
{
    public class CoreCompetenciesModel
    {
		[Display(Name = "Description")]
		public string Description { get; set; }

		[Display(Name = "Score")]
		public decimal Score { get; set; }

		[Display(Name = "Appraisee Comments")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please use letters only")]
        public string AppraiseeComments { get; set; }


		
		public string AppraisalNo { get; set; }
		public string Code { get; set; }
		
	}
}