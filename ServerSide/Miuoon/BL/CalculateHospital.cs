using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BL
{
    /// <summary>
    /// חישובים הקשורים לבתי חולים
    /// </summary>
   public class CalculateHospital
    {
        public int HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public int TravelingTime { get; set; }
        public int WaitingTime { get; set; }
        public string HospitalAdrress { get; set; }
        public bool IsPreferred { get; set; }
       

        /// <summary>
        /// מחשבת את זמן ההמתנה המשוער במחלקה ובאגף המיון יחד
        /// עפ"י התאריך-העברי הלועזי והיום בשבוע והמצב הנוכחי במחלקה 
        /// </summary>
        /// <param name="departmentCode"></param>
        public void CalcWaitingTime(int departmentCode)
        {
            Hospital_DBEntities2 db = new Hospital_DBEntities2();
            SeasonInformation dayOfWeekAvg=null;
            SeasonInformation dateAvg=null;
            SeasonInformation heberewDateAvg=null;

            if (BLManager.CurrentDayOfWeek != null)
            dayOfWeekAvg = BLManager.GetSeasonInformation(BLManager.CurrentDayOfWeek.SeasonCode, departmentCode, true);
            //בגלל שיש שוני בין השנים והמועדים הלועזיים תלויים ביום בחודש ובחודש לכן היחס ליום הינו אילו היה חל בשנה בו נשמר המידע
            if (BLManager.CurrentDate != null)
                dateAvg = BLManager.GetSeasonInformation(BLManager.CurrentDate.SeasonCode, departmentCode, true);
            if (BLManager.CurrentHeberewDate != null)
                heberewDateAvg = BLManager.GetSeasonInformation(BLManager.CurrentHeberewDate.SeasonCode, departmentCode, true);
            if (dayOfWeekAvg == null && heberewDateAvg == null && dateAvg == null)
                WaitingTime += StatisticsCalculations.CalcEstimatedWaitingTime(db.Departments.FirstOrDefault(d=>d.DepartmentCode==departmentCode).SeasonInformation.FirstOrDefault(si => si.SeasonCode == db.Seasons.FirstOrDefault(s => s.HebrewDate == null && s.Date.Value == null && s.DayOfWeek == null).SeasonCode));
            else if (dayOfWeekAvg != null && heberewDateAvg == null && dateAvg == null)
                WaitingTime += StatisticsCalculations.CalcEstimatedWaitingTime(dayOfWeekAvg);
            else if (heberewDateAvg != null && dateAvg == null)
                WaitingTime += StatisticsCalculations.CalcEstimatedWaitingTime(heberewDateAvg);
            else if (heberewDateAvg == null && dateAvg != null)
                WaitingTime += StatisticsCalculations.CalcEstimatedWaitingTime(dateAvg);
            else if(heberewDateAvg != null && dateAvg != null)//אם יום מסויים הינו גם תאריך עברי מיוחד וגם תאריך לועזי מיוחד משוקלל הממוצע של שניהם
            {
                WaitingTime += StatisticsCalculations.CalcEstimatedWaitingTime(heberewDateAvg);
                WaitingTime += StatisticsCalculations.CalcEstimatedWaitingTime(dateAvg);
                WaitingTime -= StatisticsCalculations.CalcEstimatedWaitingTime(BLManager.GetSeasonInformation(db.Seasons.FirstOrDefault(s => s.NumOfDays == 365).SeasonCode, departmentCode, true));
            }
            else //אין אף תאריך מיוחד סימן הזיהוי של המידע הכללי הינו אורך של 365 ימים
            {
                WaitingTime += StatisticsCalculations.CalcEstimatedWaitingTime(BLManager.GetSeasonInformation(db.Seasons.FirstOrDefault(s => s.NumOfDays == 365).SeasonCode,departmentCode,true));
            }


        }
        /// <summary>
        /// חישוב זמן נסיעה בין נקודת התחלה לבית חולים מסוים
        /// </summary>
        /// <param name="startPoint"></param>
        public void GooglePlaces(string startPoint)
        {

           string uri = "https://maps.googleapis.com/maps/api/distancematrix/xml?key=AIzaSyA5L81_-5d2Hy7hHsNVhodk1zS90Qu-aP8&origins="
                         + startPoint + "&destinations=" + HospitalAdrress + "&mode=driving&units=imperial&sensor=false";
           WebClient wc = new WebClient();
           try
           {
             string geoCodeInfo = wc.DownloadString(uri);
             XmlDocument xmlDoc = new XmlDocument();
             xmlDoc.LoadXml(geoCodeInfo);
           
             string duration = xmlDoc.DocumentElement.SelectSingleNode("/DistanceMatrixResponse/row/element/duration/value").InnerText;
             TravelingTime= Convert.ToInt32(duration)/60;
           }
           catch(Exception )
           {
             TravelingTime= -1;
           }
        }


    }
}
