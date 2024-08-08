using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.Finance.PettyCashSurrender
{
    public class PettyCashSurrenderLineModel
    {
		public string LineNo { get; set; }

		public string DocumentNo { get; set; }

        public string Status { get; set; }

        [Display(Name = "PettyCash Transaction Type")]
		//[Required(AllowEmptyStrings = false, ErrorMessage = "PettyCash Transaction Type Required")]
		public string PettyCashTransactionType { get; set; }
		public SelectList PettyCashTransactionTypes { get; set; }

		[Display(Name = "Receipt Code")]
		//[Required(AllowEmptyStrings = false, ErrorMessage = "Imprest Surrender Code Required")]
		public string ReceiptNo { get; set; }
		public SelectList ReceiptNos { get; set; }
		public string AccountType { get; set; }
		public string AccountNo { get; set; }
		public string AccountName { get; set; }

        public string PettyCashType { get; set; }

        [Display(Name = "Description")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Description Required")]
		public string LineDescription { get; set; }

		[Display(Name = Dimensions.GlobalDimension1Code)]
		public string LineGlobalDimension1Code { get; set; }
		public SelectList LineGlobalDimension1Codes { get; set; }

		[Display(Name = Dimensions.GlobalDimension2Code)]
		public string LineGlobalDimension2Code { get; set; }
		public SelectList LineGlobalDimension2Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension3Code)]
		public string LineShortcutDimension3Code { get; set; }
		public SelectList LineShortcutDimension3Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension4Code)]
		public string LineShortcutDimension4Code { get; set; }
		public SelectList LineShortcutDimension4Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension5Code)]
		public string LineShortcutDimension5Code { get; set; }
		public SelectList LineShortcutDimension5Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension6Code)]
		public string LineShortcutDimension6Code { get; set; }
		public SelectList LineShortcutDimension6Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension7Code)]
		public string LineShortcutDimension7Code { get; set; }
		public SelectList LineShortcutDimension7Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension8Code)]
		public string LineShortcutDimension8Code { get; set; }
		public SelectList LineShortcutDimension8Codes { get; set; }

		[Display(Name = "Amount")]
		[Required(ErrorMessage = "Amount Required")]
		public string LineAmount { get; set; }

        [Display(Name = "Actual Amount")]
        [Required(ErrorMessage = "Actual Amount Required")]
        public string LineActualAmount { get; set; }
        

        public bool LineErrorStatus { get; set; }
		public string LineErrorMessage { get; set; }

        [Display(Name = Dimensions.GlobalDimension1Code)]
        public string Dimension1 { get; set; }
        public SelectList Dimension1s { get; set; }
        [Display(Name = Dimensions.GlobalDimension2Code)]
        public string Dimension2 { get; set; }
        public SelectList Dimension2s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension3Code)]
        public string Dimension3 { get; set; }
        public SelectList Dimension3s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension4Code)]
        public string Dimension4 { get; set; }
        public SelectList Dimension4s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension5Code)]
        public string Dimension5 { get; set; }
        public SelectList Dimension5s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension6Code)]
        public string Dimension6 { get; set; }
        public SelectList Dimension6s { get; set; }

        [Display(Name = Dimensions.ShortcutDimension7Code)]
        public string Dimension7 { get; set; }
        public SelectList Dimension7s { get; set; }

        public class DropdownListData
        {
            public List<SelectListItem> ListOfddlData { get; set; }
        }
    }
}