using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.Approvals
{
	public class ApprovalEntryModel
	{
		public string EntryNo { get; set; }
		public string TableID { get; set; }
		public string DocumentType { get; set; }
		public string DocumentNo { get; set; }
		public string Description { get; set; }
		public string SequenceNo { get; set; }
		public string ApprovalCode { get; set; }
		public string SenderEmployeeNo { get; set; }
		public string SenderEmployeeName { get; set; }
		public string ApproverEmployeeNo { get; set; }
		public string ApproverEmployeeName { get; set; }
		public string SenderID { get; set; }
		public string ApproverID { get; set; }
		public string Status { get; set; }
		public string DateTimeSentforApproval { get; set; }
		public string Comment { get; set; }
		public string DueDate { get; set; }
		public string Amount { get; set; }
		public string CurrencyCode { get; set; }
		public bool ErrorStatus { get; set; }
		public string ErrorMessage { get; set; }
        public string ImprestType { get; set; }
    }
}