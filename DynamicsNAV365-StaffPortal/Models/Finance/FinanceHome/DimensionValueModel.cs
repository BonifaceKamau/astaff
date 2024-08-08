using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome
{
	public class DimensionValueModel
	{
		public string DimensionCode { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string DimensionValueType { get; set; }
		public bool Blocked { get; set; }
		public int GlobalDimensionNo { get; set; }
		public int SequenceNo { get; set; }
		public string GlobalDimension1Code { get; set; }
	}
}