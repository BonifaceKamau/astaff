using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.EmployeeAppraisal
{
	public class EmployeeAppraisalLineModel
	{
		[Display(Name = "Appraisal No.")]
		public string DocumentNo { get; set; }

		[Display(Name = "Appraisal Period")]
		public string AppraisalPeriod { get; set; }
		public SelectList AppraisalPeriods { get; set; }

		[Display(Name = "Appraisal Objective")]
		public string AppraisalObjective{ get; set; }
		public SelectList AppraisalObjectives { get; set; } 

		[Display(Name = "Organization Activity code")]
		public string OrganizationActivityCode { get; set; }
		public SelectList OrganizationActivityCodes { get; set; } 

		[Display(Name = "Department Activity code")]
		public string DepartmentActivityCode { get; set; }
		public SelectList DepartmentActivityCodes { get; set; } 

		[Display(Name = "Activity code")]
		public string ActivityCode { get; set; }

		[Display(Name = "Individual Employee Activity Description")]
		public string ActivityDescription { get; set; }

		[Display(Name = "Objective weight")]
		public decimal ObjectiveWeight { get; set; }

		[Display(Name = "Activity weight")]
		public decimal ActivityWeight { get; set; }

		[Display(Name = "Target Value")]
		public decimal TargetValue { get; set; }

		[Display(Name = "Base unit of measure")]
		public string BUM{ get; set; }
		public SelectList BUMs { get; set; } 

		[Display(Name = "Quater 1 Actual Value")]
		public decimal Q1ActualValue { get; set; }

		[Display(Name = "Actual value")]
		public string ActualValue { get; set; }

		[Display(Name = "Actual agreed value")]
		public string ActualAgreedValue{ get; set; }

		[Display(Name = "Moderate value")]
		public string ModerateValue { get; set; }

		[Display(Name = "Parameter Type")]
		public string ParameterType { get; set; }
		public SelectList ParameterTypes { get; set; }

		[Display(Name = "Appraisal Score Type")]
		public string AppraisalScoreType { get; set; }
		public SelectList AppraisalScoreTypes { get; set; }

		public bool LineErrorStatus { get; set; }
		public string LineErrorMessage { get; set; }  
	}
}