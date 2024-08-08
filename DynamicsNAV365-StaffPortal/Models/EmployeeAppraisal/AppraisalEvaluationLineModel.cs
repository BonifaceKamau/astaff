using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.EmployeeAppraisal
{
	public class AppraisalEvaluationLineModel
	{
		public int LineNo { get; set; }
		public string EvaluationNo { get; set; }
		public string AppraisalNo { get; set; }

		[Display(Name = "Appraisal Period")]
		public string AppraisalPeriod { get; set; }
		public SelectList AppraisalPeriods { get; set; }

		[Display(Name = "Appraisal Objective")]
		public string AppraisalObjective { get; set; }
		public SelectList AppraisalObjectives { get; set; }

		[Display(Name = "Organization- Activity Description")]
		public string OrganizationActivityCode { get; set; }
		public SelectList OrganizationActivityCodes { get; set; }

		[Display(Name = "Department- Activity Description")]
		public string DepartmentActivityCode { get; set; }
		public SelectList DepartmentActivityCodes { get; set; }

		[Display(Name = "Individual- Activity Description")]
		public string ActivityDescription { get; set; }
		public string ActivityCode { get; set; }

		[Display(Name ="Base Unit of Measure")]
		public string BUM { get; set; }
		public SelectList BUMs { get; set; }

		[Display(Name = "Target Value")]
		public decimal TargetValue { get; set; }

		[Display(Name = "Actual Value")]
		public decimal ActualValue { get; set; }
		public string LineErrorMessage { get; set; }
		public bool LineErrorStatus { get; set; }
	}
}