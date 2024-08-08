using System.Collections.Generic;
using System.Web.Mvc;
using OdataRef;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource
{
    public class PeerAppraisal:Peer_Appraisal_Header
    {
        public bool? ErrorStatus { get; set; }
        public string errorMessage { get; set; }
        public IEnumerable<SelectListItem> PeriodSelect { get; set; }
        public IEnumerable<SelectListItem> Peer_Appraiser_1Select { get; set; }
        public IEnumerable<SelectListItem> Peer_Appraiser_2Select { get; set; }
        public IEnumerable<SelectListItem> Peer_Appraiser_3Select { get; set; }
    }
}