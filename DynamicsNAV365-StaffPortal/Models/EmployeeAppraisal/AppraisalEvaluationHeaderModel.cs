using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.EmployeeAppraisal
{
	public class AppraisalEvaluationHeaderModel
	{
		[Display(Name = "Appraisal Evaluation No.")]
		public string No { get; set; }

		[Display(Name = "Appraisal No.")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Appraisal No. cannot be empty")]
		public string ApprovedAppraisal { get; set; }
		public SelectList AppraisalNos { get; set; }

		public string EmployeeNo { get; set; }
		public string EmployeeName { get; set; }

		[Display(Name = "Appraisal Evaluation Stage")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Appraisal Evaluation Stage cannot be empty.")]
		public string EvaluationStage { get; set; }
		public SelectList EvaluationStages { get; set; }

		public string ErrorMessage { get; set; }
		public bool ErrorStatus { get; set; }
		public string Status { get; set; }
	}
}