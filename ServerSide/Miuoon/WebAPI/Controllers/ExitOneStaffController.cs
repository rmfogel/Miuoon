using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/ExitOneStaff")]

    public class ExitOneStaffController : ApiController
    {

        [HttpGet]
        [Route("{depCode}")]
        public bool AddStaff(string depCode)
        {
            try
            {
                BLManager.CurrentStatusDepartmentList.Where(cs => cs.DepartmentCode == Int32.Parse(depCode)).FirstOrDefault().AddStaff(-1);
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
