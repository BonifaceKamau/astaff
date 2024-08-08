using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal.Controllers
{
    [System.Web.Http.RoutePrefix("api/projects")]
    public class WepApiController : ApiController
    {
        static string companyName = ServiceConnection.CompanyName;
        static string companyURL = "";

        DynamicsNAVSOAPServices dynamicsNAVSOAPServices = new DynamicsNAVSOAPServices(companyURL);
        DynamicsNAVODATAServices dynamicsNAVODataServices = new DynamicsNAVODATAServices(companyURL);
        BCODATAServices _bcodataServices = new BCODATAServices(companyURL);

        [System.Web.Http.Route("GetProjects")]
        public JsonResult<List<OdataRef.Property_Card>> GetProjects()
        {
            var nav = _bcodataServices.BCOData;
            var data = new string[] {"value1", "value2"};
            return Json(nav.Property_Card.Execute().ToList());
        }

        [System.Web.Http.Route("UpdateProjects")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateProjects(string propertyNo, PropertyStatus status)
        {
            try
            {
                var nav = _bcodataServices.BCOData;
                var propertyCard = nav.Property_Card.Where(c => c.No == propertyNo).FirstOrDefault();
                if (propertyCard == null)
                {
                    return NotFound();
                }
                propertyCard.Portal_Status = status.ToString();
                nav.UpdateObject(propertyCard);
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

    public enum PropertyStatus
    {
        Open=0,
        Reserved=1,
        Sold=2
    }
}