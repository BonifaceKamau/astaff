using System.Collections.Generic;
using System.Web.Mvc;
using OdataRef;

namespace DynamicsNAV365_StaffPortal.Models
{
    public class AssetTransferHeader:Asset_Transfer_Header_Receive
    {
        public bool ErrorStatus { get; set; }
        public string errorMessage { get; set; }
        public IEnumerable<SelectListItem> Department_Select { get; set; }
    }
}