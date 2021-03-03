using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;
using Json.NET;
using Newtonsoft.Json;

namespace BL
{
    /// <summary>
    /// מחלקה המחזירה את המידע הראשוני הנצרך בעת הפעלת השירות
    /// </summary>
    public class StartData
    {
        /// <returns>של המחלקות ובתי החולים Jason  מחזיר תבנית</returns>
        public static string GetDepartmentList()
        {
            Hospital_DBEntities2 DB = new Hospital_DBEntities2();
            string departments = JsonConvert.SerializeObject(new Hospital_DBEntities2().Departments.ToList()
                                .Select(i=>new {i.HospitalCode,i.Hospitals.HospitalName
                                ,i.DepartmentCode,i.BaseDepartmentCode,i.DepartmentTypes.DepartmentTypeName })); ;
            return departments;
        }

        public static List<string> GetHospitalNames()
        {
            List<string> hospitals = new List<string>();
            new Hospital_DBEntities2().Hospitals.ToList().
                ForEach(h => { hospitals.Add(h.HospitalCode.ToString());
                              hospitals.Add(h.HospitalName); });
            return hospitals;
        }

        public static Dictionary<int,string> GetDepartmentsForHospital(int code)
        {
            Dictionary<int, string> departments = new Dictionary<int, string>();
            new Hospital_DBEntities2().Departments.Where(d => d.HospitalCode == code).ToList().ForEach(  i =>
            {
                    departments.Add(i.DepartmentCode, i.FullName);

            });
            return departments;
        }

     
    }
}
