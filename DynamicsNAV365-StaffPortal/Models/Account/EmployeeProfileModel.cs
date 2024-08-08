using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections.Generic;

namespace DynamicsNAV365_StaffPortal.Models.Account
{
	public class EmployeeProfileModel
	{
		[Display(Name = "Employee No.")]
		public string No { get; set; }

		[Display(Name = "Employee Name")]
		public string EmployeeName { get; set; }

		[Display(Name = "Date of Birth")]
		public string DateOfBirth { get; set; }

		[Display(Name = "Gender")]
		public string Gender { get; set; }

		[Display(Name = "Martial Status")]
		public string MartialStatus { get; set; }

        [Display(Name = "Citizenship")]
		public string Citizenship { get; set; }

		[Display(Name = "Religion")]
		public string Religion { get; set; }

		[Display(Name = "Phone No.")]
		public string PhoneNumber { get; set; }

		[Display(Name = "Mobile Phone No.")]
		public string MobilePhoneNumber { get; set; }

		[Display(Name = "Postal Address")]
		public string Address { get; set; }

		[Display(Name = "Address 2")]
		public string Address2 { get; set; }

		[Display(Name = "City")]
		public string City { get; set; }

		[Display(Name = "Personal Email Address")]
		public string EmailAddress { get; set; }

		[Display(Name = "Official Email Address")]
		public string WorkEmailAddress { get; set; }

		[Display(Name = "Job No.")]
		public string JobNumber { get; set; }

		[Display(Name = "Job Title")]
		public string JobTitle { get; set; }

        [Display(Name = "Reporting To")]
        public string Manager { get; set; }

        [Display(Name = "Job Grade")]
		public string JobGrade { get; set; }

		[Display(Name = "Employement Date")]
		public string EmployementDate { get; set; }

		[Display(Name = "National ID No.")]
		public string NationalIDNumber { get; set; }

		[Display(Name = "KRA PIN No.")]
		public string PINNumber { get; set; }

		[Display(Name = "NSSF No.")]
		public string NSSFNumber { get; set; }

		[Display(Name = "NHIF No.")]
		public string NHIFNumber { get; set; }

		[Display(Name = "Professional No.")]
		public string ProfessionalNumber { get; set; }

		[Display(Name = "Professional Licence Expiry Date")]
		public string ProfessionalLicenceExpiryDate { get; set; }

		[Display(Name = "Bank Name")]
		public string BankName { get; set; }

		[Display(Name = "Bank Branch Name")]
		public string BankBranchName { get; set; }

		[Display(Name = "Bank Account No.")]
		public string BankAccountNumber { get; set; }

		[Display(Name = "County")]
		public string CountyName { get; set; }

		[Display(Name = "Sub County")]
		public string SubcountyName { get; set; }

		public bool PassportAttached { get; set; }
        public bool IsHr { get; set; }
        public string EmployeePassportPath { get; set; }
		public bool ErrorStatus { get; set; }
		public string ErrorMessage { get; set; }

        public SelectList CitizenshipCodes { get; set; }

        public SelectList CountyCodes { get; set; }

        public SelectList SubCountyCodes { get; set; }
        public SelectList BankNames { get; set; }

        public SelectList Religions { get; set; }

        public SelectList JobTitles { get; set; }
        public SelectList GenderCodes { get; set; }

        public SelectList Managers { get; set; }

        public SelectList MartialStatuss { get; set; }
        public SelectList BankBranches { get; set; }

        public class DropdownListData
        {
            public List<SelectListItem> ListOfddlData { get; set; }
        }
    }

}