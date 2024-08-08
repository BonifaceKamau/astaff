using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using DynamicsNAV365_StaffPortal.CodeHelpers;
using DynamicsNAV365_StaffPortal.Models.APIModels;
using OdataRef;

namespace DynamicsNAV365_StaffPortal.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomerApiController:ApiController
    {
        
        static string companyURL = "";
        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        BCODATAServices _bcodataServices = new BCODATAServices(companyURL);
        
        [Route("AddCustomers")]
        [HttpPost]
        //[ApiKeyAuthorize]
        public IHttpActionResult AddCustomer(CustomerInfo CustomerInfo)
        {
            try
            {
                /*// Retrieve the API key from the request header
                if (!Request.Headers.Contains("ApiKey"))
                {
                    return Content(HttpStatusCode.Unauthorized, "API key is missing.");
                }
                var apiKey = Request.Headers.GetValues("ApiKey").FirstOrDefault();

                // Validate the API key
                if (!IsValidApiKey(apiKey))
                {
                    return Content(HttpStatusCode.Unauthorized, "Invalid API key.");
                }*/
                var nav = _bcodataServices.BCOData;
                var memberCard = new TempMemberCard
                {
                    Passport_No = CustomerInfo.passport_no,
                    Name = CustomerInfo.customer_name,
                    Primary_Email = CustomerInfo.primary_email,
                    Phone_No = CustomerInfo.phone,
                    Address = CustomerInfo.address,
                    Date_of_Birth = CustomerInfo.dob,
                    Gender = CustomerInfo.gender,
                    Marital_Status = CustomerInfo.marital_status,
                    National_ID = CustomerInfo.national_id,
                    PIN_No = CustomerInfo.kra_pin,
                    Customer_Type = CustomerInfo.customer_type,
                    Alternative_Email = CustomerInfo.alternative_email,
                    PortalNo = CustomerInfo.incomoing_lead_id,
                    Lead_Source_Data = CustomerInfo.lead_source,
                    Registration_No = CustomerInfo.company_reg_no,
                };
                
                nav.AddToTempMemberCard(memberCard);
                nav.SaveChanges();
                /*var TempMemberCard = nav.TempMemberCard.Where(c=>c.National_ID == CustomerInfo.national_id).FirstOrDefault();  */
                if (memberCard.Number == 0 || CustomerInfo.GROUP_MEMBERS.All(c => string.IsNullOrEmpty(c.phone)) ||
                    CustomerInfo.GROUP_MEMBERS.Count <= 0) return Ok(true);
                foreach (var groupMemberTempLine in CustomerInfo.GROUP_MEMBERS.Select(groupMember => new TempMemberCardGroup_Member_Temp_Line
                         {
                             Group_Code =memberCard.Number,
                             Member_Name = groupMember.name,
                             Phone_No = groupMember.phone
                         }))
                {
                    nav.AddToTempMemberCardGroup_Member_Temp_Line(groupMemberTempLine);
                }

                nav.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex); 
                return Ok(ex.InnerException?.Message??ex.Message);
            }
        }
        private bool IsValidApiKey(string apiKey)
        {
            return string.Equals(apiKey, "integration", StringComparison.OrdinalIgnoreCase);
        }
    }
}