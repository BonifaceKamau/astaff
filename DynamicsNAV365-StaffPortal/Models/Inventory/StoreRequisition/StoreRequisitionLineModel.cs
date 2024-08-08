using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Models.Inventory.StoreRequisition
{
	public class StoreRequisitionLineModel
	{
		public string LineNo { get; set; }

		public string DocumentNo { get; set; }

		[Display(Name = "Type")]
		public string Type { get; set; }

		[Display(Name = "Item")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Item Required")]
		public string ItemNo { get; set; }
		public SelectList Items { get; set; }

		[Display(Name = "Item Description")]
		public string ItemDescription { get; set; }

		[Display(Name = "Unit of Measure")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Unit of Measure Required")]
		public string UOM { get; set; }
		public SelectList UOMs { get; set; }

		[Display(Name = "Location Code")]
		public string LineLocationCode { get; set; }
		public SelectList LineLocationCodes { get; set; }

		public string Inventory { get; set; }

		[Display(Name = "Quantity")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Quantity Required")]
		public string Quantity { get; set; }

		[Display(Name = "Unit Cost")]
		public string UnitCost { get; set; }

		public string LineTotalCost { get; set; }

		public string GenBusPostingGroup { get; set; }

		public string GenProdPostingGroup { get; set; }


		[Display(Name = "Description")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Description Required")]
		public string LineDescription { get; set; }

		[Display(Name = Dimensions.GlobalDimension1Code)]
		//  [Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.GlobalDimension1Code + " Required")]
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
		public bool LineErrorStatus { get; set; }
		public string LineErrorMessage { get; set; }
	}
}