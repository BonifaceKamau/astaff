using System;
using System.Web.Http;
using DynamicsNAV365_StaffPortal.CodeHelpers;
using DynamicsNAV365_StaffPortal.Models.APIModels;
using OdataRef;

namespace DynamicsNAV365_StaffPortal.Controllers
{
    [RoutePrefix("api/leadFiles")]
    public class LeadFileApiController:ApiController
    {
        static string companyURL = "";
        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        BCODATAServices _bcodataServices = new BCODATAServices(companyURL);
        
        [Route("AddLeadFiles")]
        [HttpPost]
        //[IpAuthorizationFilter("192.168.1.100", "example.com")] 
        public IHttpActionResult AddLeadFiles(LeadFileInfo LeadFileInfo)
        {
            try
            {
                var nav = _bcodataServices.BCOData;
                var leadFileCard = new Lead_File_Card
                {
                    Member_No = LeadFileInfo.customer_id,
                    Booking_No = LeadFileInfo.Booking_id,
                    Plot_No = LeadFileInfo.Plot_number,
                    No_of_Installments = LeadFileInfo.NoOfInstallments,
                    Interest_Amount_Per_Month = LeadFileInfo.InterestAmount,
                    Booking_Date = LeadFileInfo.BookingDate,
                    Purchase_Type = LeadFileInfo.purchase_type,
                    Sales_Person_Code = LeadFileInfo.Marketer,
                    Booking_Type = "Land"
                };
                
                nav.AddToLead_File_Card(leadFileCard);
                nav.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex); 
                return Ok(ex.InnerException?.Message??ex.Message);
            }
        }
    }
}