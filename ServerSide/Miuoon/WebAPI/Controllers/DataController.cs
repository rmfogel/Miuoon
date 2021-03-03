using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/Data")]
    public class DataController : ApiController
    {
        //מחזיר רשימת בתי חולים
        [HttpGet]
        public List<string> GetHospitalNames()
        {
            return StartData.GetHospitalNames();
        }

        //מחזיר רשימת מחלקות לבית חולים מבוקש
        [Route("{code}")]
        [HttpGet]
        public Dictionary<int,string> GetDepartmentsForHospital(string code)
        {
            return StartData.GetDepartmentsForHospital(Int32.Parse(code));
        }

        //כניסת ממתין 
        [Route("{depCode}")]
        [HttpPost]
        public Boolean AddWaiter(string depCode)
        {
            try
            {
                BLManager.CurrentStatusDepartmentList.Where(cs => cs.DepartmentCode == Int32.Parse(depCode)).FirstOrDefault().AddWaiter();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            }
        //יציאת ממתין 
        [HttpPost]
        public void ExitWaiter(int depCode)
        {
            BLManager.CurrentStatusDepartmentList.Where(cs => cs.DepartmentCode == depCode).FirstOrDefault().ExitWaiter();
        }

        //כניסת איש צוות
        [HttpPost]
        [Route("{depCode:int}/{num:int}")]
        public bool AddStaff(int depCode,int num)
        {
            BLManager.CurrentStatusDepartmentList.Where(cs => cs.DepartmentCode == depCode).FirstOrDefault().AddStaff(num);
            return true;
        }

        public void Options() { }
    }
}
