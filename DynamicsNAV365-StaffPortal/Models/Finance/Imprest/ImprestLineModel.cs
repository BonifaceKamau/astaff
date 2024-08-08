using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.Finance.Imprest
{
	public class ImprestLineModel
	{
		public string LineAmountLCY;
		public string LineNo { get; set; }

		public string DocumentNo { get; set; }

        public string Status { get; set; }

        [Display(Name = "Transaction Type")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Transaction Type Required")]
		public string TransactionType { get; set; }
		public SelectList TransactionTypeS { get; set; } 

		[Display(Name = "Imprest Category")]
		public string ImprestCategory { get; set; } 
		public SelectList ImprestCategories { get; set; } 

		[Display(Name = "SRC Destination")]
		public string SRCDestination { get; set; } 
		public SelectList SRCDestinations { get; set; } 

		[Display(Name = "Imprest Element")]
		public string ImprestElement { get; set; }
		public SelectList ImprestElements { get; set; } 
		public string AccountType { get; set; }
		public string AccountNo { get; set; }
		public string AccountName { get; set; }

		[Display(Name = "Quantity")]
		public string Days { get; set; }

		[Display(Name = "Description")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Description Required")]
		public string LineDescription { get; set; }

		[Display(Name = Dimensions.GlobalDimension1Code)]
		// [Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.GlobalDimension1Code+" Required")]
		public string LineGlobalDimension1Code { get; set; }
		public SelectList LineGlobalDimension1Codes { get; set; }

		[Display(Name = Dimensions.GlobalDimension2Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.GlobalDimension2Code+" Required")]
		public string LineGlobalDimension2Code { get; set; }
		public SelectList LineGlobalDimension2Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension3Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension3Code+" Required")]
		public string LineShortcutDimension3Code { get; set; }
		public SelectList LineShortcutDimension3Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension4Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension4Code+" Required")]
		public string LineShortcutDimension4Code { get; set; }
		public SelectList LineShortcutDimension4Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension5Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension5Code+" Required")]
		public string LineShortcutDimension5Code { get; set; }
		public SelectList LineShortcutDimension5Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension6Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension6Code+" Required")]
		public string LineShortcutDimension6Code { get; set; }
		public SelectList LineShortcutDimension6Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension7Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension7Code+" Required")]
		public string LineShortcutDimension7Code { get; set; }
		public SelectList LineShortcutDimension7Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension8Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension8Code+" Required")]
		public string LineShortcutDimension8Code { get; set; }
		public SelectList LineShortcutDimension8Codes { get; set; }

		[Display(Name = "KTNA Job Group")]
		public string SalaryScale { get; set; } 
		public SelectList SalaryScales { get; set; }

		[Display(Name = "Amount")]
	//	[Required(ErrorMessage = "Amount Required")]
		public string LineAmount { get; set; }
		public bool LineErrorStatus { get; set; }
		public string LineErrorMessage { get; set; }

            //Added
        [Display(Name = Dimensions.GlobalDimension1Code)]
        [Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.GlobalDimension1Code + " Required")]
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

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string HeaderDescripton { get; set; }
        public string CurrencyCode { get; set; }
        public string UnitOfMeasure { get; set; }
        public IEnumerable<SelectListItem> CurrencyCodeSelect { get; set; }
        public IEnumerable<SelectListItem> UnitOfMeasureSelect { get; set; }
        public string UnitPrice { get; set; }
        public string Quantity { get; set; }
	}
}