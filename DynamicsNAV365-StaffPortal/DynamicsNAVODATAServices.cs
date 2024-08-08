using DynamicsNAV365_StaffPortal.DynamicsNAVODataServiceReference;
using System;
using System.Net;

namespace DynamicsNAV365_StaffPortal
{
	public class DynamicsNAVODATAServices
	{
		public NAV dynamicsNAVOData = null;
        static string ConnPassword = "Port@l0103";
        static string ConnUsername = "Portaluser";
        static string domain = "";
        public DynamicsNAVODATAServices(string companyURLName)
		{
            

            dynamicsNAVOData = new NAV(new Uri(ServiceConnection.GetDynamicsNAVODATAURL(companyURLName)));
            //dynamicsNAVOData.Credentials = ServiceConnection.getConnectionCredentials(ServiceConnection.GetDynamicsNAVODATAURL(companyURLName));
            // dynamicsNAVOData.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //System.Net.CredentialCache.DefaultCredentials;
            dynamicsNAVOData.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
        }
    }

    public class BCODATAServices
    {
        public OdataRef.NAV BCOData = null;
        static string ConnPassword = "Port@l0103";
        static string ConnUsername = "Portaluser";
        static string domain = "";
        public BCODATAServices(string companyURLName)
        {
            BCOData = new OdataRef.NAV(new Uri(ServiceConnection.GetDynamicsNAVODATAURL(companyURLName)));
            BCOData.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
        }
    }
    public class BCODATAV4Services
    {
        public OdataRefV4.NAV.NAV BCOData = null;
        static string ConnPassword = "Nav365@";
        static string ConnUsername = "erp2";
        static string domain = "";
        public BCODATAV4Services(string companyURLName)
        {
            var urlName = string.IsNullOrEmpty(companyURLName) ? "Optiven R.E" : companyURLName;
            BCOData = new OdataRefV4.NAV.NAV(new Uri($"http://192.168.0.5:8048/Optiven/ODataV4/Company('" + urlName + "')"));
            BCOData.Credentials = new NetworkCredential(ConnUsername, ConnPassword, domain);
        }
    }
}