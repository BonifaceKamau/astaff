﻿namespace DynamicsNAV365_StaffPortal.Models.Procurement.RFX
{
    public class RFXLineModel
    {
        public string LineNo { get; set; }
        public string DocumentNo { get; set; } 
        public string RequisitionNo { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string UOM { get; set; }
        public string UnitPrice { get; set; }
        public string Amount { get; set; }
        public string ProcurementPlan { get; set; }
        public string ProcurementPlanItem { get; set; }
        public string BudgetLine { get; set; }
        public string CurrentBudget { get; set; }
        public string AttachedtoPR { get; set; }
        public string TORFileName { get; set; }
        public string GlobalDimension1Code { get; set; }
        public string GlobalDimension2Code { get; set; }
    }
}