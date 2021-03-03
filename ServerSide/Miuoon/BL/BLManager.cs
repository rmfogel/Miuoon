using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BL
{//לשנות סתם!!!
    /// <summary>
    /// המחלקה מנהלת את השירות ומכילה פונקציות ומשתנים סטטיים שונים לשימוש כל המחלקות
    /// </summary>
    public class BLManager
    {
        //רשימת הנתונים העכשיויים במחלקות
        public static List<CurrentStatusDepartment> CurrentStatusDepartmentList { get; set; }
        //העונה שכוללת את התאריך הלועזי הנוכחי 
        public static Seasons CurrentDate { get; set; }
        //העונה שמייצגת את היום בשבוע הנוכחי 
        public static Seasons CurrentDayOfWeek { get; set; }
        //העונה שכוללת את התאריך העברי הנוכחי 
        public static Seasons CurrentHeberewDate { get; set; }
        //הערך הבולאני המייצג את הריצה הראשונית
        private static bool isFirstRun;
        // מופע של מחלקה פנימית המייצרת טריגר יומי
        DailyTrigger trigger; // every day at 00:00

        public BLManager()
        {
           
            Hospital_DBEntities2 DB = new Hospital_DBEntities2();
            CurrentStatusDepartmentList = new List<CurrentStatusDepartment>();
            isFirstRun = true;
            DB.Departments.ToList().ForEach(i => CurrentStatusDepartmentList.Add(new CurrentStatusDepartment { DepartmentCode = i.DepartmentCode }));
            trigger = new DailyTrigger(11, 58);
            trigger.OnTimeTriggered += saveDataByTrigger;

        }



        /// <summary>
        /// טריגר המעדכן במסד הנתונים את הנתונים הסטטיסטיים של היום הקודם 
        /// ומעדכן את  העונות הנוכחיות לתאריכים של היום-עברי, לועזי ויום בשבוע 
        /// </summary>
        public void saveDataByTrigger()
        {


            //If first run then no need to set season in DB
            if (isFirstRun == false)
                CurrentStatusDepartmentList.ForEach(i => { i.SetSeasonInfo(); });
            else
                isFirstRun = false;
            CurrentDate = GetCurrentDate(true).FirstOrDefault();
            CurrentHeberewDate = GetCurrentHeberewDate(true).FirstOrDefault();
            CurrentDayOfWeek = GetCurrentDayOfWeek(true).FirstOrDefault();


        }

        /// <summary>
        /// פונקציה המעדכנת ממוצעים יומיים של אנשי צוות וממתינים
        /// </summary>
        public void AvgCalcTimer()
        {
            TimeSpan startTimeSpan = TimeSpan.Zero;
            TimeSpan periodTimeSpan = TimeSpan.FromHours(1);

            var timer = new System.Threading.Timer((e) =>
            {
                CurrentStatusDepartmentList.ForEach(i => { i.CalcStaffAvg(); i.CalcWaitersAvg(); });
            }, null, startTimeSpan, periodTimeSpan);
        }

        /// <summary>
        /// שליפת העונות המכילות את התאריך הלועזי בתאריך שנשלח 
        /// </summary>
        /// <param name="isDefined"></param>
        /// <param name="tempDate"></param>
        /// <returns> null מחזיר את הרשימה שנשלפה. אם לא קיים מחזיר   </returns>
        public static List<Seasons> GetCurrentDate(bool isDefined, DateTime? tempDate = null)
        {
            if (tempDate == null) tempDate = DateTime.Today;
            DateTime date = (DateTime)tempDate;
            Hospital_DBEntities2 db = new Hospital_DBEntities2();
            List<Seasons> seasonList = db.Seasons.Where(s => s.Date != null && s.IsDefined == isDefined).ToList();
            List<Seasons> seasonNewList = new List<Seasons>();
            for (int i = 0; i < db.Seasons.Count(); i++)
            {
                try
                {
                    if ((seasonList[i].Date.Value <= new DateTime(seasonList[i].Date.Value.Year, date.Month, date.Day)) && (
                    seasonList[i].Date.Value.AddDays(seasonList[i].NumOfDays) >= new DateTime(seasonList[i].Date.Value.Year, date.Month, date.Day)))
                        seasonNewList.Add(seasonList[i]);

                }
                catch (ArgumentOutOfRangeException)
                {
                    continue;
                }
            }

            return seasonList.Where(s =>
             (s.Date.Value <= new DateTime(s.Date.Value.Year, date.Month, date.Day) &&
      s.Date.Value.AddDays(s.NumOfDays) >= new DateTime(s.Date.Value.Year, date.Month, date.Day))).ToList();

        }

        /// <summary>
        /// שליפת העונות המכילות את התאריך העברי בתאריך שנשלח 
        /// </summary>
        /// <param name="isDefined"></param>
        /// <param name="tempDate"></param>
        /// <returns> null מחזיר את הרשימה שנשלפה. אם לא קיים מחזיר   </returns>
        public static List<Seasons> GetCurrentHeberewDate(bool isDefined, DateTime? tempDate = null)
        {
            if (tempDate == null) tempDate = DateTime.Today;
            DateTime date = (DateTime)tempDate;
            Hospital_DBEntities2 db = new Hospital_DBEntities2();
            List<Seasons> seasonList = db.Seasons.Where(s => s.HebrewDate != null && s.IsDefined == isDefined).ToList();
            return seasonList.Where(s => date.IsInRange(s.HebrewDate.Value, s.NumOfDays) == true).ToList();

        }

        /// <summary>
        /// שליפת העונות המכילות את היום בשבוע בתאריך שנשלח 
        /// </summary>
        /// <param name="isDefined"></param>
        /// <param name="tempDate"></param>
        /// <returns> null מחזיר את הרשימה שנשלפה. אם לא קיים מחזיר   </returns>
        public static List<Seasons> GetCurrentDayOfWeek(bool isDefined)
        {
            Hospital_DBEntities2 db = new Hospital_DBEntities2();
            return db.Seasons.Where(i => i.DayOfWeek.Value == (int)DateTime.Today.DayOfWeek && i.IsDefined == isDefined).ToList();


        }

        /// <summary>
        /// שולף רצף של עונות מתאריך לועזי מסוים לאחור
        /// </summary>
        /// <returns>מחזיר </returns>
        public static Seasons GetLongDateSeason()
        {
            DateTime date = DateTime.Today;
            /*there is only one defined seasons*/
            return (GetCurrentDate(true, date.AddDays(-1)).FirstOrDefault());

        }

        /// <summary>
        /// שולף רצף של עונות מתאריך לועזי מסוים לאחור
        /// </summary>
        /// <returns>מחזיר </returns>
        public static Seasons GetLongHeberewDateSeason()
        {
            DateTime date = DateTime.Today;
            /*there is only one defined seasons*/
            return GetCurrentHeberewDate(true, date.AddDays(-1)).FirstOrDefault();

        }

        /// <param name="seasonCode">העונה המבוקשת</param>
        /// <param name="departmentCode">המחלקה המבוקשת</param>
        /// <param name="isHistory">האם המידע היסטורי או לא</param>
        /// <returns>מחזיר מידע לתקופה מבוקשת</returns>
        public static SeasonInformation GetSeasonInformation(int seasonCode, int departmentCode, bool isHistory)
        {
            return new Hospital_DBEntities2().SeasonInformation.FirstOrDefault(s => s.SeasonCode == seasonCode && s.Ishistory == isHistory && s.DepartmentCode == departmentCode);
        }
    }
    /// <summary>
    /// מחלקה המייצרת טריגר יומי
    /// </summary>
    public class DailyTrigger
    {
        //התאריך האחרון בו הופעל הטריגר
        public DateTime LastDateInvoke { get; set; } = DateTime.Now.AddDays(-1);
        //השעה בה אמור לפעול הטריגר
        readonly TimeSpan triggerHour;

        public DailyTrigger(int hour, int minute = 0, int second = 0)
        {
#if DEBUG
            minute = DateTime.Now.Minute + 1;
#endif 

            triggerHour = new TimeSpan(hour, minute, second);

            InitiateAsync();

        }
       /// <summary>
       /// פונקציה אסינכרונית הבודקת האם הגיע הזמן להפעיל את הטריגר    
       /// </summary>
       /// <returns></returns>
        async Task InitiateAsync()
        {
            while (true)
            {
                if (LastDateInvoke.Date != DateTime.Now.Date &&
                     DateTime.Now.Hour == triggerHour.Hours && DateTime.Now.Minute == triggerHour.Minutes)
                {
                    LastDateInvoke = DateTime.Now.Date;
                    await Task.Delay(200);
                    OnTimeTriggered?.Invoke();
                }
                await Task.Delay(200);

            }
        }
        //האירוע דרכו מופעל הטריגר
        public event Action OnTimeTriggered;
    }
}
