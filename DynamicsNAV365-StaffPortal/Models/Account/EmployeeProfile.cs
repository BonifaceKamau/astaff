using System.Collections.Generic;
using System.Web.Mvc;
using OdataRef;

namespace DynamicsNAV365_StaffPortal.Models.Account
{
    public class EmployeeProfile: HR_Employee
    {
        public bool ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
        public IEnumerable<SelectListItem> GenderCodes { get; set; }
        public IEnumerable<SelectListItem> MartialStatus_Select { get; set; }
        public IEnumerable<SelectListItem> CitizenshipCodes { get; set; }
        public IEnumerable<SelectListItem> Religions { get; set; }
        public IEnumerable<SelectListItem> CountyCodes { get; set; }
        public IEnumerable<SelectListItem> BankNames { get; set; }
        public IEnumerable<SelectListItem> BankBranches { get; set; }
        public IEnumerable<SelectListItem> JobTitles { get; set; }
        public IEnumerable<SelectListItem> Managers { get; set; }
        public List<SelectListItem> SubcountySelect { get; set; }
        public List<SelectListItem> Ethnic_Group_Select { get; set; }
        public IEnumerable<SelectListItem> Post_Code2_Select { get; set; }
        public IEnumerable<SelectListItem> Country_Region_Code_Select { get; set; }
        public IEnumerable<SelectListItem> Pay_Mode_Select { get; set; }
        public IEnumerable<SelectListItem> Bank_Code_select { get; set; }
        public IEnumerable<SelectListItem> Bank_Branch_Code_Select { get; set; }
    }
}