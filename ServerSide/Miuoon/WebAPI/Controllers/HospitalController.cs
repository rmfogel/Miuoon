using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BL;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/hospital")]
    public class HospitalController : ApiController
    {
        //מחזיר מידע ראשוני על בתי החולים והמחלקות
        [HttpGet]
        public string GetHospitals()
        {
            return  StartData.GetDepartmentList();
        }

        //מעביר את בקשת השירות לשרת וחמזירה את רשימת בתי החולים הממויינת
        [HttpGet]
        [ResponseType(typeof(string))]
        [Route("suitable/{StartingPoint}/{hospitalCode:int}/{departmentCode:int}/{wingCode:int}")]
        public IHttpActionResult GetSuitableHospitals([FromUri]string StartingPoint, 
            [FromUri]int hospitalCode, [FromUri]int departmentCode, [FromUri]int wingCode)
        {
            Request request = new Request();
            return Ok(JsonConvert.SerializeObject(request.CalcDifference(StartingPoint, hospitalCode, departmentCode, wingCode)));
        }


        public void Options() { }

    }


}   

