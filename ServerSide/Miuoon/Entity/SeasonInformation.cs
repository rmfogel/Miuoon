//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class SeasonInformation
    {
        public int InfoCode { get; set; }
        public int SeasonCode { get; set; }
        public int StaffAvg { get; set; }
        public int WaitingTimeAvg { get; set; }
        public int WaitersAvg { get; set; }
        public bool Ishistory { get; set; }
        public int DepartmentCode { get; set; }
        public int Whidnesstochange { get; set; }
        public Nullable<System.DateTime> AddingDate { get; set; }
    
        public virtual Departments Departments { get; set; }
        public virtual Seasons Seasons { get; set; }
    }
}
