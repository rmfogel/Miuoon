using Entity;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BL
{
    /// <summary>
    /// מחלקה המייצגת את המצב הנוכחי במחלקה
    /// </summary>
    public class CurrentStatusDepartment
    {
        public int DepartmentCode { get; set; }
        public int DayWaitingAvg { get; set; }
        public int DayWaitingSampleSpace { get; set; }
        public int StaffNum { get; set; }
        public int StaffAvg { get; set; }
        public int WaitersAvg { get; set; }
        /// <summary>
        /// staff and waiters sample space
        /// </summary>
        public int SampleSpace { get; set; }
        public Queue<DateTime> DepartmentQueue { get; set; } = new Queue<DateTime>();
        /// <summary>
         //todo  delete!!!(the values- ctr)
        /// </summary>
        public CurrentStatusDepartment()
        {
            //Random rnd = new Random();
            //DayWaitingAvg = rnd.Next(50,150);
            //DayWaitingSampleSpace = 2000;
            //StaffAvg = 30;
            //StaffNum = 30;
            //WaitersAvg = rnd.Next(100,200);
        }

        /// <summary>
        /// adds waiter to waiting queue of department
        /// </summary>
        public void AddWaiter()
        {
            DepartmentQueue.Enqueue(DateTime.Now);
        }

        /// <summary>
        /// removes waiter from waiting queue of department
        /// and updates the daily waiting time avg
        /// </summary>
        public void ExitWaiter()
        {
            int diff = (DateTime.Now - DepartmentQueue.First()).Minutes;
            DayWaitingAvg = (DayWaitingAvg * DayWaitingSampleSpace + diff) / ++DayWaitingSampleSpace;
        }

        /// <summary>
        /// adds staff 
        /// </summary>
        /// <param name="num"></param>
        public void AddStaff(int num)
        {
            StaffNum += num;
        }

        /// <summary>
        /// update daily staff avg
        /// </summary>
        public void CalcStaffAvg()
        {
            StaffAvg = (StaffAvg * SampleSpace + StaffNum) / ++SampleSpace;
        }

        /// <summary>
        /// Update daily waiters avg
        /// </summary>
        public void CalcWaitersAvg()
        {
            WaitersAvg = (WaitersAvg * (SampleSpace - 1) + DepartmentQueue.Count) / SampleSpace;
        }

        /// <summary>
        /// עידכון נתוני העונות במסד הנתונים
        /// </summary>
        public void SetSeasonInfo()
         
        {       //set date not historical season information
            if (BLManager.CurrentDate != null && BLManager.GetSeasonInformation(BLManager.CurrentDate.SeasonCode, DepartmentCode, false) != null)
            {
                SetSeasonInfoInDB(BLManager.GetSeasonInformation(BLManager.CurrentDate.SeasonCode, DepartmentCode, false));
            }
            //set heberew date not historical season information 
            if (BLManager.CurrentHeberewDate != null && BLManager.GetSeasonInformation(BLManager.CurrentHeberewDate.SeasonCode, DepartmentCode, false) != null)
            {
                SetSeasonInfoInDB(BLManager.GetSeasonInformation(BLManager.CurrentHeberewDate.SeasonCode, DepartmentCode, false));
            }

            //set day of week not historical  season information 
            if (BLManager.CurrentDayOfWeek != null && BLManager.GetSeasonInformation(BLManager.CurrentDayOfWeek.SeasonCode, DepartmentCode, false) != null)
            {
                SetSeasonInfoInDB(BLManager.GetSeasonInformation(BLManager.CurrentDayOfWeek.SeasonCode, DepartmentCode, false));
            }
            //Check if difining new season needed (if no specific info for dopartment for heberew date or date)
            if ((BLManager.CurrentDate == null || BLManager.GetSeasonInformation(BLManager.CurrentDate.SeasonCode, DepartmentCode, false) == null) && (BLManager.CurrentHeberewDate == null || BLManager.GetSeasonInformation(BLManager.CurrentHeberewDate.SeasonCode, DepartmentCode, false) == null))
            {
                //only day of week info is not strong enough
                //רוצים לבדוק האם הנתונים של היום שונים באופן משמעותי מהנתונים הכלליים
                //האם צריך לשמור את היום כתאריך מיוחד או לא
                //הנתונים הכלליים משמעותם: או המידע של כל השנה או המידע של היום הנוכחי בשבוע, תלוי בשאילה אם 
                //יש מידע על יום בשבוע או אין
                SeasonInformation infoToCheck;
                if (BLManager.CurrentDayOfWeek == null)
                    infoToCheck = BLManager.GetSeasonInformation(new Hospital_DBEntities2().Seasons.First(s => s.NumOfDays == 365).SeasonCode, DepartmentCode, true);
                else infoToCheck = BLManager.GetSeasonInformation(BLManager.CurrentDayOfWeek.SeasonCode, DepartmentCode, true);
                //בודק מה היחס שבין היחסים של התאריכים
                //אם היחס שונה משמעותית
                if (infoToCheck.Ratio / DayWaitingAvg / (WaitersAvg / StaffAvg) < 0.5 || infoToCheck.Ratio / (DayWaitingAvg / (WaitersAvg / StaffAvg)) > 1.5)
                    AddSeason();
            }
        }

        /// <summary>
        /// שומר/משכלל את הממוצעים היומיים במסד הנתונים
        /// </summary>
        /// <param name="information"></param>
        public  void SetSeasonInfoInDB(SeasonInformation information)
        {
            Hospital_DBEntities2 db = new Hospital_DBEntities2();
            information = db.SeasonInformation.Where(i=>i.InfoCode==information.InfoCode).First();
            information.WaitersAvg = (information.WaitersAvg * information.Whidnesstochange + WaitersAvg) / ++information.Whidnesstochange;
            information.StaffAvg = (information.StaffAvg * (information.Whidnesstochange - 1) + StaffAvg) / information.Whidnesstochange;
            information.WaitingTimeAvg = (int)(information.WaitingTimeAvg * (information.Whidnesstochange - 1) + DayWaitingAvg) / information.Whidnesstochange;
            db.SaveChanges();
            //האם להעביר מידע לתקופה להיסטורי לפי מרחב המדגם
            if ((information.Seasons.DayOfWeek != null && information.Whidnesstochange == 52/*(one year)*/) || (information.Seasons.DayOfWeek == null && information.Whidnesstochange == 5/*(5 years)*/))
            {
                    information.Ishistory = true;
                db.SaveChanges();
                    db.SeasonInformation.Add(new SeasonInformation {
                        Ishistory = false,
                        DepartmentCode = information.DepartmentCode,
                        SeasonCode = information.SeasonCode,
                        AddingDate = DateTime.Today
                    });
                db.SaveChanges();
            }
        }

        /// <summary>
        /// אם הממוצע היומי שונה משמעותית מהמידע הכללי ולא מופיע לתאריך זה מידע שהוגדר 
        /// נוצר מופע חדש של עונה לא מוגדרת המכילה את כל הנתונים- תאריך עברי לועזי ויום בשבוע לצורך בדיקה 
        /// </summary>
        public void AddSeason()
        {
            List<Seasons> seasonToCheck = new List<Seasons>(); 
            Hospital_DBEntities2 db = new Hospital_DBEntities2();
            if (BLManager.CurrentDate != null) seasonToCheck.Add( BLManager.CurrentDate);
            if (BLManager.CurrentHeberewDate != null) seasonToCheck.Add(BLManager.CurrentHeberewDate);
            else if (BLManager.CurrentDate == null)
            {
                seasonToCheck.Add(new Seasons { Date = DateTime.Today, DayOfWeek = ((int)DateTime.Today.DayOfWeek) + 1, HebrewDate = DateTime.Today, IsDefined = false, NumOfDays = 1 });
                db.Seasons.Add(seasonToCheck[0]);
                db.SaveChanges();
            }
            seasonToCheck.ForEach(s=>
           {
               db.SeasonInformation.Add(new SeasonInformation
               {
                   AddingDate = DateTime.Today,
                   DepartmentCode = DepartmentCode,
                   Ishistory = false,
                   StaffAvg = StaffAvg,
                   WaitersAvg = WaitersAvg,
                   WaitingTimeAvg = DayWaitingAvg,
                   Whidnesstochange = 1,
                   SeasonCode = s.SeasonCode
               });
           });
            db.SaveChanges();
            if (BLManager.GetCurrentDate(false).Count() == 10)
            {
                DefineSeasonInfo(1, 5);
            }

            if (BLManager.GetCurrentHeberewDate(false).Count() == 10)
            {
                DefineSeasonInfo(2, 5);
            }

            if (BLManager.GetCurrentDayOfWeek(false).Count() == 100)
            {
                DefineSeasonInfo(3, 52);
            }
        }

        /// <summary>
        /// משנה עונה לעונה מוגדרת כאשר יש מספיק מידע המוכיח את השוני המשמעותי בעונה
        /// </summary>
        /// <param name="seasonType"></param>
        /// <param name="num"></param>
        public static void DefineSeasonInfo(int seasonType, int num)
        {
            Hospital_DBEntities2 db = new Hospital_DBEntities2();
            List<Seasons> tempSeasons = new List<Seasons>();
            Seasons tempLongSeasons = new Seasons();

            switch (seasonType)
            {
                case 1: { db.Seasons.Add(new Seasons { Date = DateTime.Today.Date, IsDefined = true, NumOfDays = 1 }); db.SaveChanges(); tempSeasons = BLManager.GetCurrentDate(false); tempLongSeasons = BLManager.GetLongDateSeason(); BLManager.CurrentDate = db.Seasons.Last(); } break;
                case 2: { db.Seasons.Add(new Seasons { HebrewDate = DateTime.Today.Date, IsDefined = true, NumOfDays = 1 }); db.SaveChanges(); tempSeasons = BLManager.GetCurrentHeberewDate(false); tempLongSeasons = BLManager.GetLongHeberewDateSeason(); BLManager.CurrentHeberewDate = db.Seasons.Last(); } break;
                case 3: { db.Seasons.Add(new Seasons { DayOfWeek = (int)DateTime.Today.DayOfWeek, IsDefined = true, NumOfDays = 1 }); tempSeasons = BLManager.GetCurrentDayOfWeek(false); BLManager.CurrentDayOfWeek = db.Seasons.Last(); } break;

            }

            db.SaveChanges();
            List<SeasonInformation> seasonTempInfo = new List<SeasonInformation>();
            if (tempLongSeasons == null)
            {
                //מקבץ את כל המידע לתקופה של תקופת יום בשבוע מסויימת לא מוגדרות
                CombineSeasons(GetInfoForSeasonList(tempSeasons), num, db.Seasons.ToList().Last().SeasonCode);
            }
            else
            {
                if (seasonType == 1)
                    BLManager.CurrentDate = tempLongSeasons;
                else BLManager.CurrentHeberewDate = tempLongSeasons;
                CombineSeasons(GetInfoForSeasonList(tempSeasons), 5, tempLongSeasons.SeasonCode);
                db.Seasons.Remove(db.Seasons.Last());
            }

            db.SeasonInformation.ToList().ForEach(se => tempSeasons.ForEach(s => { if (se.SeasonCode == s.SeasonCode) seasonTempInfo.Add(se); }));
            db.SeasonInformation.RemoveRange(seasonTempInfo);
            db.SaveChanges();
            List<int> listCode = tempSeasons.Select(i => i.SeasonCode).ToList();
            db.Seasons.RemoveRange(db.Seasons.Where(item => listCode.Any(item2 => item2 == item.SeasonCode)));

        }

        /// <summary>
        ///צירוף מס' עונות לעונה אחת וקישור כל יחידות המידע של העונות לעונה החדשה שהוגדרה
        ///או כאשר רוצים ליצור עונה מוגדרת ארוכה מ-2 עונות מוגדרות רצופות)
        ///(או ליצור ממס' עונות לא מוגדרות של אותו תאריך עונה מוגדרת אחת
        /// </summary>
        /// <param name="seasonInfo">רשימת העונות שרוצים לצרף</param>
        /// <param name="num">המס' לפיו יוחלט האם המידע לעונה הוא היסטורי</param>
        public static void CombineSeasons(List<SeasonInformation> seasonInfo,int num,int seasonCode)
        {
           Hospital_DBEntities2 db = new Hospital_DBEntities2();
           seasonInfo
          .GroupBy(p => p.DepartmentCode)
          .Select(g => g.ToList())
          .ToList()
          .ForEach(i =>
          {
              db.SeasonInformation.Add(new SeasonInformation
              {
                  SeasonCode = seasonCode,
                  StaffAvg = (int)i.Average(s => s.StaffAvg),
                  WaitersAvg = (int)i.Average(s => s.WaitersAvg),
                  WaitingTimeAvg = (int)i.Average(s => s.WaitingTimeAvg),
                  Whidnesstochange = i.Count(),
                  DepartmentCode = i.First().DepartmentCode,
                  Ishistory = (i.Count >= num)

              });
              db.SaveChanges();
          });
        }

        /// <summary>
        ///מחזיר רשימה של מידע לעונה לפי רשימת עונות
        /// </summary>
        /// <param name="seasons"></param>
        /// <returns></returns>
        public static List<SeasonInformation> GetInfoForSeasonList(List<Seasons> seasons)
        {
            List<SeasonInformation> seasonInfo = new List<SeasonInformation>();

            new Hospital_DBEntities2().SeasonInformation.ToList().ForEach(se => seasons.ForEach(s => { if (se.SeasonCode == s.SeasonCode) seasonInfo.Add(se); }));
            return seasonInfo;

        }
    }
}
