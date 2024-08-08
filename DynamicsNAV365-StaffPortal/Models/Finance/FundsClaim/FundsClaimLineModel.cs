using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.Finance.FundsClaim
{
	public class FundsClaimLineModel
	{
		public int LineNo { get; set; }
		public string DocumentNo { get; set; }

		[Display(Name = "Funds Claim Code")]
		public string ImprestCode { get; set; }
		public SelectList ImprestCodes { get; set; }

		[Display(Name = "Traveling From")]
		public string FromCity { get; set; }

		[Display(Name = "To")]
		public string ToCity { get; set; }
		public SelectList Cities { get; set; }

		[Display(Name = "Description")]
		public string LineDescription { get; set; }

		[Display(Name = "Amount (in Kenya Shillings)")]
		public decimal LineAmount { get; set; }
		public bool LineErrorStatus { get; set; }
		public string LineErrorMessage { get; set; }
	}
}