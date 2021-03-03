using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    /// <summary>
    /// מחלקה המבצעת חישובים סטטיסטיים
    /// </summary>
    class StatisticsCalculations
    {
        /// <summary>
        /// זמן ההמתנה המשוער בהתחשב בנתונים ההסטוריים ובנתוני זמן אמת 
        ///היחס בין מספר אנשי הצוות לבין הממתינים הינו הקובע את משך זמן ההמתנה המשוער
        /// </summary>
        public static int CalcEstimatedWaitingTime(SeasonInformation historyInfo)
        {
            try
            {
                int currentStaffNum = BLManager.CurrentStatusDepartmentList
                     .Find(d => d.DepartmentCode == historyInfo.DepartmentCode).StaffNum;
                int currentWaitersNum = BLManager.CurrentStatusDepartmentList
                     .Find(d => d.DepartmentCode == historyInfo.DepartmentCode).DepartmentQueue.Count();
                //היחס בין מספר אנשי הצוות לבין הממתינים הינו הקובע את משך זמן ההמתנה המשוער
                return (currentWaitersNum / currentStaffNum) * historyInfo.Ratio;
            }
            catch (Exception )
            {
                return 0;
            }
        }


        //בדיקת השערות
        //public static bool HypothesisTesting(SeasonInformation nonHistoricalInfo)
        //{
        //    //Get last historical info for the same department and same season in orther to check the difference.
        //    SeasonInformation historicalInfo = new Hospital_DBEntities2().SeasonInformation.ToList().LastOrDefault(i=>i.SeasonCode==nonHistoricalInfo.SeasonCode&&i.Ishistory==true);
        //    //todo Implement function. do you have to check or just change once in 5 years or 52 weeks or any other amount

        //    return true;
        //}
    }
}
