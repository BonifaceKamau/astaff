using System;
using System.Net;

namespace DynamicsNAV365_StaffPortal
{
    public class ServiceConnection
	{
		public static string AppName = Properties.Settings.Default.AppName;
		public static string CompanyName = Properties.Settings.Default.CompanyName;
		public static string CompanyWebsite = Properties.Settings.Default.CompanyWebsite;
		public static string PoweredBy = Properties.Settings.Default.PoweredBy; 
		public static string contactICTDepartment = "Contact the " + CompanyName + " ICT Department for assistance.";
		public static string protocol = Properties.Settings.Default.protocol;
		public static string passwordResetProtocol = Properties.Settings.Default.passwordResetProtocol;
		public static string DynamicsNAVServer = "192.168.0.5";
		static string SOAPPortNumber = Properties.Settings.Default.SOAPPortNumber;
		static string ODATAPortNumber = Properties.Settings.Default.ODATAPortNumber;
		static string DynamicsNAVServiceName = Properties.Settings.Default.DynamicsNAVServiceName;
		static string DefaultCompanyURLName = Properties.Settings.Default.DefaultCompanyURLName;
        //static string ConnUsername = Properties.Settings.Default.ConnUsername;
        //static string ConnPassword = Properties.Settings.Default.ConnPassword;
        static string ConnPassword = "Nav365@";
        static string ConnUsername = "erp2";
        static string domain = "";
        static NetworkCredential netCred = new NetworkCredential(ConnUsername, ConnPassword,domain);
        

        //SharePoint Configurations
        public static string sharePointSiteUrl = "";
		public static string sharePointUser = "erpadmin@kentrade.go.ke";
		public static string sharePointUserPassword = "dWp@j$9JUB6A$cCF0z";

		//Funds Management
		public static string FinanceFolderTitle = "Finance Management";
		public static string ImprestFolder = "TRAVEL REQUEST";
		public static string PettyCashFolder = "PETTY CASH";
		public static string PettyCashSurrenderFolder = "PETTY CASH SURRENDER";
		public static string ImprestSurrenderFolder = "EXPENSES SURRENDER";
		public static string FundsClaimFolder = "CLAIMS";
		public static string GeneralImprestFolder = "GENERAL IMPREST";

		//Human Resources
		public static string HumanResourcesFolderTitle = "HUMAN RESOURCE MANAGEMENT";
		public static string LeaveApplicationFolder = "LEAVE APPLICATION";
		public static string LeaveReimbursementFolder = "LEAVE REIMBURSEMENT";
		public static string RecruitmentFolder = "ERECRUITMENT"; 
		public static string PayrollFolder = "PAYROLL DOCUMENTS";
		public static string PerformanceManagementFolder = "Performance Management";

		//Supply Chain & Logistics Management 
		public static string SupplyChainFolderTitle = "SUPPLY CHAIN MANAGEMENT";
		public static string GeneralFolderTitle = "GENERAL SCM";  
		public static string RequisitionsFolder = "REQUISITIONS";
		public static string SupplierPortalFolder = "SUPPLIER PORTAL";

		//static NetworkCredential netCred = new NetworkCredential(ConnUsername, ConnPassword);
		public static string googleReCaptchaKey = "6LdBZFkUAAAAAGhY2hZVzi2B9LB-SdHX83whjp1i";
		public static string GetDynamicsNAVSOAPURL(string ServiceName, string CompanyURLName)
		{
			if (CompanyURLName.Equals(""))
			{
				return protocol + DynamicsNAVServer + ":" + SOAPPortNumber + "/" + DynamicsNAVServiceName + "/WS/" + DefaultCompanyURLName + "/Codeunit/" + ServiceName;
			}
			else
			{
				return protocol + DynamicsNAVServer + ":" + SOAPPortNumber + "/" + DynamicsNAVServiceName + "/WS/" + CompanyURLName + "/Codeunit/" + ServiceName;
			}
		}
		public static string GetDynamicsNAVSOAPAGEPURL(string ServiceName, string CompanyURLName)
		{
			if (CompanyURLName.Equals(""))
			{
				return protocol + DynamicsNAVServer + ":" + SOAPPortNumber + "/" + DynamicsNAVServiceName + "/WS/" + DefaultCompanyURLName + "/Page/" + ServiceName;
			}
			else
			{
				return protocol + DynamicsNAVServer + ":" + SOAPPortNumber + "/" + DynamicsNAVServiceName + "/WS/" + CompanyURLName + "/Page/" + ServiceName;
			}
		}
		public static string GetDynamicsNAVODATAURL(string CompanyURLName)
		{
			if (CompanyURLName.Equals(""))
			{
				return protocol + DynamicsNAVServer + ":" + ODATAPortNumber + "/" + DynamicsNAVServiceName + "/OData/Company('" + DefaultCompanyURLName + "')";
			}
			else
			{
				return protocol + DynamicsNAVServer + ":" + ODATAPortNumber + "/" + DynamicsNAVServiceName + "/OData/Company('" + CompanyURLName + "')";
			}
		}
		public static CredentialCache getConnectionCredentials(string Url)
		{
			CredentialCache myCredentials = new CredentialCache();
			if (myCredentials.GetCredential(new Uri(Url), "Basic") == null)
			{
				myCredentials.Add(new Uri(Url), "Basic", netCred);
			}
			return myCredentials;
		}
        
    }
}