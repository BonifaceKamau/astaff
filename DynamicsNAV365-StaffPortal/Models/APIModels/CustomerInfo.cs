using System;
using System.Collections.Generic;

namespace DynamicsNAV365_StaffPortal.Models.APIModels
{
    public class CustomerInfo
    {
        public int incomoing_lead_id { get; set; }
        public string customer_name { get; set; }
        public string national_id { get; set; }
        public string passport_no { get; set; }
        public string kra_pin { get; set; }
        public DateTime? dob { get; set; }
        public string gender { get; set; }
        public string marital_status { get; set; }
        public string phone { get; set; }
        public string primary_email { get; set; }
        public string alternative_email { get; set; }
        public string address { get; set; }
        public string customer_type { get; set; }
        public string company_reg_no { get; set; }
        public string sub_cat_id { get; set; }
        public string marketer { get; set; }
        public string lead_source { get; set; }
        public List<GroupMember> GROUP_MEMBERS { get; set; }
    }

    public class GroupMember
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string relationship { get; set; }
    }
}