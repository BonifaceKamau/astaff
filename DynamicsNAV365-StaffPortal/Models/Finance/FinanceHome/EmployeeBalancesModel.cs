using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome
{
	public class EmployeeBalancesModel
	{
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public string AmountStr { get; set; }
	}
}