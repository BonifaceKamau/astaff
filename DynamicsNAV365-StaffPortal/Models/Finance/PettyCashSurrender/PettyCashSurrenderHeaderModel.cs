using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.Finance.PettyCashSurrender
{
    public class PettyCashSurrenderHeaderModel
    {
		public string No { get; set; }

		public string DocumentDate { get; set; }

		public string PostingDate { get; set; }

        [Display(Name = "Petty Cash No.")]
        public string BankAccountNo { get; set; }
		public SelectList BankAccountNos { get; set; }

		public string BankAccountName { get; set; }

		public string ReferenceNo { get; set; }

		[Display(Name = "Employee No.")]
		public string EmployeeNo { get; set; }

		public string EmployeeName { get; set; }

		public bool Surrendered { get; set; }

        public string PettyCashType { get; set; }

        [Display(Name = "Currency")]
		public string CurrencyCode { get; set; }
		public SelectList CurrencyCodes { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Description Required")]
		[StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
		public string Description { get; set; }

		[Display(Name = Dimensions.GlobalDimension1Code)]
		public string GlobalDimension1Code { get; set; }
		public SelectList GlobalDimension1Codes { get; set; }

		[Display(Name = Dimensions.GlobalDimension2Code)]
		public string GlobalDimension2Code { get; set; }
		public SelectList GlobalDimension2Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension3Code)]
		public string ShortcutDimension3Code { get; set; }
		public SelectList ShortcutDimension3Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension4Code)]
		public string ShortcutDimension4Code { get; set; }
		public SelectList ShortcutDimension4Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension5Code)]
		public string ShortcutDimension5Code { get; set; }
		public SelectList ShortcutDimension5Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension6Code)]
		public string ShortcutDimension6Code { get; set; }
		public SelectList ShortcutDimension6Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension7Code)]
		public string ShortcutDimension7Code { get; set; }
		public SelectList ShortcutDimension7Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension8Code)]
		public string ShortcutDimension8Code { get; set; }
		public SelectList ShortcutDimension8Codes { get; set; }

		[Display(Name = "Comments")]
		public string Comments { get; set; }

		[Display(Name = "Amount")]
		public string Amount { get; set; }

		[Display(Name = "Status")]
		public string Status { get; set; }
		public bool ErrorStatus { get; set; }
		public string ErrorMessage { get; set; }

        [Display(Name = "Unsurrendered PettyCash")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select the petty to surrender.")]
        public string UnsurrenderedPettyCash { get; set; }
        public SelectList UnsurrenderedPettyCashs { get; set; }
    }
}