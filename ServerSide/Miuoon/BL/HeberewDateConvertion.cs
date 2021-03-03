using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{/// <summary>
/// מחלקה המטפלת בהמרה וחישוב של תאריכים עבריים
/// </summary>
   public static class HeberewDateConvertion
    {
        public static int HeberewMonth(this DateTime value)
        {
            HebrewCalendar hd = new HebrewCalendar();
            return  hd.GetMonth(value); 
        }

        public static int HeberewDayOfMonth(this DateTime value)
        {
            HebrewCalendar hd = new HebrewCalendar();
            return hd.GetDayOfMonth(value);
        }

        /// <summary>
        /// מחשב האם התאריך בטווח התאריכים בהתחשב בנתוני לוח עברי : שנה מעוברת ,שנים מלאות ,חסרות וכסדרן
        /// </summary>
        /// <param name="value">date to check(today)</param>
        /// <param name="date">date of event</param>
        /// <param name="length">how meny days is event</param>
        /// <returns></returns>
        public static bool IsInRange(this DateTime value, DateTime date, int length)
        {
            length--;
            HebrewCalendar hd = new HebrewCalendar();
            //case 1: The same type of year
            if (hd.GetDaysInYear(hd.GetYear(value)) == hd.GetDaysInYear(hd.GetYear(date)))
            {
               //לא נכון למקרה הזה יכול להיות שימושי במקרים אחרים
                //if (hd.GetDayOfYear(date) + length > hd.GetDaysInYear(hd.GetYear(date)))
                //    length += hd.GetDaysInYear(hd.GetYear(date));
                if (hd.GetDayOfYear(value) >= hd.GetDayOfYear(date) && (hd.GetDayOfYear(value) <= (hd.GetDayOfYear(date) + length)))
                    return true;
                return false;
            }
            
            int month = hd.GetMonth(value);
            // all false options: different months(not Adar in leep year) or an erlier day in the month - same length of month 
            if (month < hd.GetMonth(date)&&hd.IsLeapYear(hd.GetYear(date)) == false)
                    return false;
                //both are ether leap or not, The differencse is by the months
            if (Math.Abs(hd.GetDaysInYear(hd.GetYear(value)) -hd.GetDaysInYear(hd.GetYear(date)))<=2)
               return CheckRange(hd.GetYear(date), month, hd.GetDayOfMonth(value),value,date,length);
            
            else if(hd.IsLeapYear(hd.GetYear(value)) ==true)
            {
                
                if (month > 6)
                    month -= 1;
                if (hd.GetMonth(value) != 6)
                 return   CheckRange(hd.GetYear(date), month, hd.GetDayOfMonth(value),value,date,length);
                else return false;
            }
            //date is a leap year
            else
            {
                if (month >= 6)
                    month += 1;
                if (hd.GetMonth(date) == 6 || (hd.GetMonth(date) < 6 && (hd.GetMonth(hd.AddDays(date, length)) >= 6)))
                    return false;
              return  CheckRange(hd.GetYear(date), month, hd.GetDayOfMonth(value),value,date,length);

            }

        }

        /// <summary>
        /// חלק מחישוב האם התאריך ברצף תאריכים
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="value"></param>
        /// <param name="date"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool CheckRange(int year,int month,int day,DateTime value,DateTime date,int length)
        {  
            HebrewCalendar hd = new HebrewCalendar();
            try
            {
                DateTime tempDate = hd.ToDateTime(year, month, day, 0, 0, 0, 0);
                if (hd.GetDayOfYear(value) >= hd.GetDayOfYear(date) && (hd.GetDayOfYear(value) <= (hd.GetDayOfYear(date) + length)))
                    return true;
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
               
            }
        }
    }
}
