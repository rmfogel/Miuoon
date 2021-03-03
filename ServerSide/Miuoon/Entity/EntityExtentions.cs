using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entity
{
    class EntityExtentions
    {
    }
        public partial class Seasons
        {
            //private int heberewMonth ;
            public int HeberewMonth
        {
           get
            
               { return new HebrewCalendar().GetMonth(HebrewDate.Value); }
        }

        //private int heberewDayInMonth;
        public int HeberewDayInMonth
        {
            get { return new HebrewCalendar().GetDayOfMonth(HebrewDate.Value); }
        }

        
    }

    public partial class SeasonInformation
    {
      //todo How to call it?
        public int Ratio
        {
            get
            {
               return WaitingTimeAvg / (WaitersAvg / StaffAvg);
            }

        }

    }

    public partial class Departments
    {
        //todo How to call it?
        public string FullName
        {
            get
            {
                if (this.BaseDepartmentCode == null)
                    return this.DepartmentTypes.DepartmentTypeName;
                return this.DepartmentTypes.DepartmentTypeName+" " + new Hospital_DBEntities2().Departments.Where(d=>d.DepartmentCode==this.BaseDepartmentCode).FirstOrDefault().DepartmentTypes.DepartmentTypeName;

            }

        }

    }

}
