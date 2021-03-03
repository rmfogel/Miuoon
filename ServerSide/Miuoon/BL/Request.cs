using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using Entity;
namespace BL
{
    /// <summary>
    /// מייצג בקשת שירות
    /// </summary>
    public class Request
    {
        public List<CalculateHospital> CalculateHospitalList { get; set; }

        public Request()
        {
            CalculateHospitalList = new List<CalculateHospital>();
        }


        /// <summary>
        /// ממלא רשימה של בתי החולים ונתוניהם החישוביים
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="preferredhospitalCode"></param>
        /// <returns> מחזיר את הרשימה ממוינת מהמהיר לפחות</returns>
       public List<CalculateHospital> CalcDifference(string startPoint,int preferredhospitalCode,
                                                     int preferredDepartmentCode,int preferredWingCode)
        {
            int departmentType=1, wingType=7;
            Hospital_DBEntities2 db = new Hospital_DBEntities2();
            if(preferredDepartmentCode!=-1)
               departmentType = db.Departments.Where(d => d.DepartmentCode == preferredDepartmentCode).FirstOrDefault().DepartmentTypeCode;
            if (preferredWingCode != -1)
                wingType = db.Departments.Where(d => d.DepartmentCode == preferredWingCode).FirstOrDefault().DepartmentTypeCode;
            db.Hospitals.ToList()
                .ForEach(i => CalculateHospitalList
                .Add(new CalculateHospital{
                 HospitalCode = i.HospitalCode,
                    HospitalName = i.HospitalName,
                    HospitalAdrress=i.Street+" "+i.HouseNum+" "+i.City+" " + i.Country,
                    IsPreferred = (i.HospitalCode == preferredhospitalCode) }));
            CalculateHospitalList.ForEach(i => {
                Departments dtc = db.Departments.FirstOrDefault(d => d.HospitalCode == i.HospitalCode && d.DepartmentTypeCode == departmentType);
                if (dtc == null)
                    dtc = db.Departments.FirstOrDefault(d => d.HospitalCode == i.HospitalCode && d.DepartmentTypeCode == 1);
                i.CalcWaitingTime(dtc.DepartmentCode);
                 dtc = db.Departments.FirstOrDefault(d => d.HospitalCode == i.HospitalCode && d.DepartmentTypeCode == wingType);
                if (dtc == null)
                    dtc = db.Departments.FirstOrDefault(d => d.HospitalCode == i.HospitalCode && d.DepartmentTypeCode == 7);
                i.CalcWaitingTime(dtc.DepartmentCode);
                i.GooglePlaces(startPoint);
            });
            CalculateHospitalList = CalculateHospitalList.Where(h => h.WaitingTime > 0 && h.TravelingTime > 0).ToList();
            CalculateHospitalList= CalculateHospitalList.OrderBy(i => i.TravelingTime + i.WaitingTime).ToList();//מיון על פי חישוב משך הזמן הכולל
            return CalculateHospitalList;
       }
     

 
       //יכול להתבצע בclient

        ///// <summary>
        ///// מחזיר הפרש בין בית חולים מועדף לבין בית החולים המהיר ביותר
        ///// </summary>
        ///// <param name="preferredhospitalCode"></param>
        ///// <returns>
        ///// מחזיר -1 אם לא בחר
        ///// 0 אם הכי מהיר
        ///// אחרת את ההפרש
        ///// </returns>
        //public int DifferencePreferredToFast(int preferredhospitalCode)
        //{    
        //    var db = new Hospital_DBEntities2();                      
        //    int differenceBetweenPreferredAndFast=-1;                   
        //    if (preferredhospitalCode != -1)                          
        //    {
        //            int preferredHospitalTime = CalculateHospitalList.Where(i=>i.HospitalCode==preferredhospitalCode).FirstOrDefault().TravelingTime+ CalculateHospitalList.Where(i => i.HospitalCode == preferredhospitalCode).FirstOrDefault().WaitingTime;
        //            differenceBetweenPreferredAndFast = preferredHospitalTime - (CalculateHospitalList.FirstOrDefault().TravelingTime + CalculateHospitalList.FirstOrDefault().WaitingTime);
                   
        //    }
        //    return differenceBetweenPreferredAndFast;
        //}
    }
}
