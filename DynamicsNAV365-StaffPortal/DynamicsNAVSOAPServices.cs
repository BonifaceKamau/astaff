using DynamicsNAV365_StaffPortal.ApprovalManagementWebServiceReference;
using DynamicsNAV365_StaffPortal.DocumentManagementWebServiceRef;
using DynamicsNAV365_StaffPortal.EmployeeAccountWebServiceReference;
using DynamicsNAV365_StaffPortal.EmployeeAppraisalManagementWebServiceRef;
using DynamicsNAV365_StaffPortal.EmployeeTrainingManagementWebServiceRef;
using DynamicsNAV365_StaffPortal.FundsClaimManagementWebServiceReference;
using DynamicsNAV365_StaffPortal.FundsManagementWebServiceReference;
using DynamicsNAV365_StaffPortal.HumanResourceManagmentWebServiceReference;
using DynamicsNAV365_StaffPortal.InventoryManagementWebServiceReference;
using DynamicsNAV365_StaffPortal.LoanManagementWebServiceReference;
using DynamicsNAV365_StaffPortal.MemoSalaryAdvanceWebServiceReference;
using DynamicsNAV365_StaffPortal.PayrollManagementWebServiceReference;
using DynamicsNAV365_StaffPortal.ProcurementManagementWebServiceReference;
using DynamicsNAV365_StaffPortal.PerformanceManagementWebServiceReference;
using DynamicsNAV365_StaffPortal.ProjectManagementWebServiceReference;
using DynamicsNAV365_StaffPortal.ActivityRequestWs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using DynamicsNAV365_StaffPortal.NativeApprovalsMgmtWebReference;
using DynamicsNAV365_StaffPortal.Property_Card;
using DynamicsNAV365_StaffPortal.TimeSheet;

namespace DynamicsNAV365_StaffPortal
{
	public class DynamicsNAVSOAPServices
	{
        public EmployeeAccountWebService employeeAccountWS = new EmployeeAccountWebService();
        public HumanResourceManagementWS hrManagementWS = new HumanResourceManagementWS();
	//	public InventoryManagementWS inventoryManagementWS = new InventoryManagementWS();
		public ProcurementManagementWS inventoryManagementWS = new ProcurementManagementWS();
		public ProcurementManagementWS procurementManagementWS = new ProcurementManagementWS();
		public FundsManagementWebService fundsManagementWS = new FundsManagementWebService();
        public ActivityRequest activityRequestWS = new ActivityRequest();
        public FundsClaimManagementWebService fundsClaimManagementWS = new FundsClaimManagementWebService();
		public PortalApprovalManager ApprovalsMgmt = new PortalApprovalManager();
		public Approvals_Mgmt ApprovalsMgmtWebReference = new Approvals_Mgmt();
		public PayrollManagementWS payrollManagementWS = new PayrollManagementWS();
		public DocumentMgmt documentMgmt = new DocumentMgmt();
        public Property_Card_Service PropertyCardService = new Property_Card_Service();
        public ProjectPortalUpdate.ProjectPortalUpdate ProjectPortalUpdate = new ProjectPortalUpdate.ProjectPortalUpdate();
        //public EmployeeAppraisalManagementWS employeeAppraisalManagementWS = new EmployeeAppraisalManagementWS();
        public EmployeeTrainingManagementWS employeeTrainingManagementWS = new EmployeeTrainingManagementWS();
		public LoanManagementWebService loanManagementWS = new LoanManagementWebService();
		public MemoSalaryAdvanceWebService salaryAdvanceWS = new MemoSalaryAdvanceWebService();
        public PerformanceManagement performanceManagement = new PerformanceManagement();
        public ProjectManagementWS projectManagement = new ProjectManagementWS();
        public TimeSheetWS FleetMgmt = new TimeSheetWS();
        public DynamicsNAV365_StaffPortal.StaffAdvanceWebServiceReference.StaffAdvance StaffAdvance  = new DynamicsNAV365_StaffPortal.StaffAdvanceWebServiceReference.StaffAdvance();

        static string ConnPassword = "Nav365@";
        static string ConnUsername = "erp2";
        static string domain = "";
        static NetworkCredential netCred = new NetworkCredential(ConnUsername, ConnPassword, domain);

        public DynamicsNAVSOAPServices(string companyURLName)
		{
			//Employee Account WS
			employeeAccountWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("EmployeeAccountWebService", companyURLName);
            employeeAccountWS.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
           // employeeAccountWS.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //employeeAccountWS.Credentials = ServiceConnection.getConnectionCredentials(employeeAccountWS.Url);

            //HR Management WS
            hrManagementWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("HumanResourceManagementWS", companyURLName);
            hrManagementWS.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain); ;
           // hrManagementWS.Credentials = ServiceConnection.getConnectionCredentials(hrManagementWS.Url);

			//Inventory Management WS
			//inventoryManagementWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("InventoryManagementWS", companyURLName);
			//inventoryManagementWS.Credentials = ServiceConnection.getConnectionCredentials(inventoryManagementWS.Url);
			inventoryManagementWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("ProcurementManagementWS", companyURLName);
            inventoryManagementWS.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            // inventoryManagementWS.Credentials = ServiceConnection.getConnectionCredentials(inventoryManagementWS.Url);

            //Procurement Management WS
            procurementManagementWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("ProcurementManagementWS", companyURLName);
            procurementManagementWS.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            // procurementManagementWS.Credentials = ServiceConnection.getConnectionCredentials(procurementManagementWS.Url);

            //Funds Management WS
            fundsManagementWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("FundsManagementWebService", companyURLName);
            fundsManagementWS.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            //fundsManagementWS.Credentials = ServiceConnection.getConnectionCredentials(fundsManagementWS.Url);

            //Activity Request WS
            activityRequestWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("ActivityRequest", companyURLName);
            activityRequestWS.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);

            //Fundsclaim Management WS
            fundsClaimManagementWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("FundsClaimManagementWebService", companyURLName);
            fundsClaimManagementWS.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            //fundsClaimManagementWS.Credentials = ServiceConnection.getConnectionCredentials(fundsClaimManagementWS.Url);

            //Approvals Management WS
            ApprovalsMgmt.Url = ServiceConnection.GetDynamicsNAVSOAPURL("PortalApprovalManager", companyURLName);
            ApprovalsMgmt.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            //ApprovalsMgmt.Credentials = ServiceConnection.getConnectionCredentials(ApprovalsMgmt.Url);

            //Payroll Management WS
            payrollManagementWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("PayrollManagementWS", companyURLName);
            payrollManagementWS.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            //payrollManagementWS.Credentials = ServiceConnection.getConnectionCredentials(payrollManagementWS.Url);

            //Document Management WS
            documentMgmt.Url = ServiceConnection.GetDynamicsNAVSOAPURL("DocumentMgmt", companyURLName);
            documentMgmt.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            // documentMgmt.Credentials = ServiceConnection.getConnectionCredentials(documentMgmt.Url);
            
            //property card page WS
            PropertyCardService.Url = ServiceConnection.GetDynamicsNAVSOAPAGEPURL("Property_Card", companyURLName);
            PropertyCardService.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            
            //property update ws
            ProjectPortalUpdate.Url = ServiceConnection.GetDynamicsNAVSOAPURL("ProjectPortalUpdate", companyURLName);
            ProjectPortalUpdate.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);

            //Employee Appraisal Management WS
            //employeeAppraisalManagementWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("EmployeeAppraisalManagementWS", companyURLName);
            //employeeAppraisalManagementWS.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //employeeAppraisalManagementWS.Credentials = ServiceConnection.getConnectionCredentials(employeeAppraisalManagementWS.Url);

            //Employee Training Management WS
            employeeTrainingManagementWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("EmployeeTrainingManagementWS", companyURLName);
            employeeTrainingManagementWS.Credentials = CredentialCache.DefaultCredentials;
            employeeTrainingManagementWS.Credentials = ServiceConnection.getConnectionCredentials(employeeTrainingManagementWS.Url);

            //Loan Management Web Service
            loanManagementWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("LoanManagementWebService", companyURLName);
            loanManagementWS.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            //loanManagementWS.Credentials = ServiceConnection.getConnectionCredentials(loanManagementWS.Url);

            //Memo Salary Advance Web Service
            salaryAdvanceWS.Url = ServiceConnection.GetDynamicsNAVSOAPURL("MemoSalaryAdvanceWebService", companyURLName);
            salaryAdvanceWS.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            //salaryAdvanceWS.Credentials = ServiceConnection.getConnectionCredentials(salaryAdvanceWS.Url);

            //Performance Management
            performanceManagement.Url = ServiceConnection.GetDynamicsNAVSOAPURL("PerformanceManagement", companyURLName);
            performanceManagement.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            //this.performanceManagement.Credentials =ServiceConnection.getConnectionCredentials(this.performanceManagement.Url);

            //Project Management
            projectManagement.Url = ServiceConnection.GetDynamicsNAVSOAPURL("ProjectManagementWS",companyURLName);
            projectManagement.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            //this.projectManagement.Credentials = ServiceConnection.getConnectionCredentials(this.projectManagement.Url);
            
            //staff Advance
            StaffAdvance.Url = ServiceConnection.GetDynamicsNAVSOAPURL("StaffAdvance",companyURLName);
            StaffAdvance.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            
            //FleetMgmt
            FleetMgmt.Url = ServiceConnection.GetDynamicsNAVSOAPURL("TimeSheetWS",companyURLName);
            FleetMgmt.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
            
            //approval mgmt
            ApprovalsMgmtWebReference.Url = ServiceConnection.GetDynamicsNAVSOAPURL("Approvals_Mgmt",companyURLName);
            ApprovalsMgmtWebReference.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);

		}
    }
}