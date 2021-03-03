using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/AddOneWaiter")]

    public class AddOneWaiterController : ApiController
    {
        [Route("{depCode}")]
        [HttpGet]
        public Boolean AddWaiter(string depCode)
        {
            try
            {
                BLManager.CurrentStatusDepartmentList.Where(cs => cs.DepartmentCode == Int32.Parse(depCode)).FirstOrDefault().AddWaiter();
                return true;
            }
            catch (Exception ) 
            {
                return false;
            }
        }

        //[Route("ExitW/{depCode}")]
        //[HttpGet]
        //public Boolean ExitWaiter(string depCode)
        //{
        //    try
        //    {
        //        BLManager.CurrentStatusDepartmentList.Where(cs => cs.DepartmentCode == Int32.Parse(depCode)).FirstOrDefault().AddWaiter();
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //}
        //[Route("Tew/{depCode:int}")]
        //[ResponseType(typeof(Boolean))]
        //public Boolean ExitWaiter([FromUri]int depCode)
        //{
        //    try
        //    {
        //        BLManager.CurrentStatusDepartmentList.Where(cs => cs.DepartmentCode == depCode).FirstOrDefault().ExitWaiter();
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        public void Options() { }

    }
}
