using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.Controllers
{
    public class ExitOneWaiterController : ApiController
    {

        [RoutePrefix("api/ExitOneWaiter")]

        public class AddWaiterController : ApiController
        {
            [Route("{depCode}")]
            [HttpGet]
            public Boolean ExitWaiter(string depCode)
            {
                try
                {
                    BLManager.CurrentStatusDepartmentList.Where(cs => cs.DepartmentCode == Int32.Parse(depCode)).FirstOrDefault().ExitWaiter();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public void Options() { }


        }
    }
}
