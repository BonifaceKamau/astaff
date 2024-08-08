using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.DocumentMgmt
{
	public class DocumentMgmtModel
	{
		public string LineNo { get; set; } 
		public string DocumentNo { get; set; }

		[Display(Name = "Document Code")]
		public string DocumentCode { get; set; }

		[Display(Name= "Document Description")]
		public string DocumentDescription { get; set; }

		public bool DocumentAttached { get; set; }
		public string FileName { get; set; } 
		public bool ErrorStatus { get; set; }
		public string ErrorMessage { get; set; }

        public int TabelID { get; set; }
        public string No { get; set; }
        public string FileExt { get; set; }
        public int ID { get; set; }
        public string DocType { get; set; }
    }
    public class DocumentAttachmentList
    {
        public string Status { get; set; }
        public List<DocumentMgmtModel> DocList { get; set; }
    }
}