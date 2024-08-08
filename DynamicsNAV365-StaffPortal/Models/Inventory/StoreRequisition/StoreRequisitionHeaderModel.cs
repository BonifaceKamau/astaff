using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DynamicsNAV365_StaffPortal.Models.Inventory.StoreRequisition
{
	public class StoreRequisitionHeaderModel
	{
		public IEnumerable<SelectListItem> facilitySelect { get; set; }
		public string No { get; set; }

		public string DocumentDate { get; set; }

		public string PostingDate { get; set; }

		[Display(Name = "Location Code")]
		// [Required(AllowEmptyStrings = false, ErrorMessage = "Location Code Required")]
		public string LocationCode { get; set; }
		public SelectList LocationCodes { get; set; }

		[Display(Name = "Required Date")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Date Required")]
		public string RequiredDate { get; set; }

		[Display(Name = "Requester ID")]
		public string RequesterID { get; set; }

		[Display(Name = "Amount")]
		public string Amount { get; set; }

		[Display(Name = "Description")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Description Required")]
		public string Description { get; set; }

		[Display(Name = Dimensions.GlobalDimension1Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.GlobalDimension1Code + " Required")]
		public string GlobalDimension1Code { get; set; }
		public SelectList GlobalDimension1Codes { get; set; }

		[Display(Name = Dimensions.GlobalDimension2Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.GlobalDimension2Code + " Required")]
		public string GlobalDimension2Code { get; set; }
		public SelectList GlobalDimension2Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension3Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension3Code + " Required")]
		public string ShortcutDimension3Code { get; set; }
		public SelectList ShortcutDimension3Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension4Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension4Code + " Required")]
		public string ShortcutDimension4Code { get; set; }
		public SelectList ShortcutDimension4Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension5Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension5Code + " Required")]
		public string ShortcutDimension5Code { get; set; }
		public SelectList ShortcutDimension5Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension6Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension6Code + " Required")]
		public string ShortcutDimension6Code { get; set; }
		public SelectList ShortcutDimension6Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension7Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension7Code + " Required")]
		public string ShortcutDimension7Code { get; set; }
		public SelectList ShortcutDimension7Codes { get; set; }

		[Display(Name = Dimensions.ShortcutDimension8Code)]
		//[Required(AllowEmptyStrings = false, ErrorMessage = Dimensions.ShortcutDimension8Code + " Required")]
		public string ShortcutDimension8Code { get; set; }
		public SelectList ShortcutDimension8Codes { get; set; }
		public string ResponsibilityCenter { get; set; }
		public string Comments { get; set; }

		[Display(Name = "Status")]
		public string Status { get; set; }
		public string EmployeeNo { get; set; }
		public bool ErrorStatus { get; set; }
		public string ErrorMessage { get; set; }

		public virtual IList<StoreRequisitionLineModel> StoreRequisitionLines { get; set; }
		[Display(Name = "Facility No")]
		public string facilityNo { get; set; }
	}
}