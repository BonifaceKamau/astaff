using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.HumanResourceHome
{
	public class EmployeeLeaveBalanceModel
	{
		public string LeaveType { get; set; }
		public decimal? LeaveBalance { get; set; }
		public string LeaveBalanceStr { get; set; }
	}
}