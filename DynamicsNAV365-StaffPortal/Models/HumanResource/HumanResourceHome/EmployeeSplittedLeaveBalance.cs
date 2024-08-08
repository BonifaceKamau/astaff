using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.HumanResourceHome
{
    public class EmployeeSplittedLeaveBalance
    {
        public string DaysEntitlement { get; set; }
        public string DaysEarned { get; set; }
        public string DaysTakenToDate { get; set; }
        public string DaysAvailable { get; set; }
        public string AnnualLeaveDaysCarriedForward { get; set; } 
        public string CurrentLeavePeriod { get; set; } 
    }
}