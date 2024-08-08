using DynamicsNAV365_StaffPortal.Models.Finance.FinanceHome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DynamicsNAV365_StaffPortal.Models.HumanResource.FleetMgt
{
    public class FleetHeaderModel
    {
        public string No { get; set; }
        public string Status { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string Description { get; set; }
        public string CreatedOn { get; set; }
        public Boolean ErrorStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class FleetLinesModel
    {
        public string No { get; set; }
        public string Status { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string Description { get; set; }
        public string DriverEmpNo { get; set; }
        public string EntryNo { get; set; }
        public string Date { get; set; }
        public string StaffNo { get; set; }
        public string StaffName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string VehicleNo { get; set; }
        public string FixedAssetNo { get; set; }
        public string OdometerStart { get; set; }
        public string OdometerEnd { get; set; }
        public string DistanceCovered { get; set; }
        public string Litres { get; set; }
        public string Amount { get; set; }
        [Display(Name = Dimensions.GlobalDimension1Code)]
        public string Dimension1 { get; set; }
        public SelectList Dimension1s { get; set; }
        [Display(Name = Dimensions.GlobalDimension2Code)]
        public string Dimension2 { get; set; }
        public SelectList Dimension2s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension3Code)]
        public string Dimension3 { get; set; }
        public SelectList Dimension3s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension4Code)]
        public string Dimension4 { get; set; }
        public SelectList Dimension4s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension5Code)]
        public string Dimension5 { get; set; }
        public SelectList Dimension5s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension6Code)]
        public string Dimension6 { get; set; }
        public SelectList Dimension6s { get; set; }

        [Display(Name = Dimensions.ShortcutDimension7Code)]
        public string Dimension7 { get; set; }
        public SelectList Dimension7s { get; set; }
        public string Purpose { get; set; }
    }
    public class FleetLinesList
    {
        public List<FleetLinesModel> ListofFleetLines { get; set; }
        
    }

    public class ListOfddlData
    {
        public string VehicleReg { get; set; }
        public string StaffNo { get; set; }
        public string DocumentDate { get; set; }

        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }
        public string OdometerStart { get; set; }
        public string OdometerEnd { get; set; }
        public string DistanceCovered { get; set; }
        public string Litres { get; set; }
        public string Amount { get; set; }

        [Display(Name = Dimensions.GlobalDimension1Code)]
        public string Dimension1 { get; set; }
        public IEnumerable<SelectListItem> Dimension1s { get; set; }

        [Display(Name = Dimensions.GlobalDimension2Code)]
        public string Dimension2 { get; set; }
        public IEnumerable<SelectListItem> Dimension2s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension3Code)]
        public string Dimension3 { get; set; }
        public IEnumerable<SelectListItem> Dimension3s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension4Code)]
        public string Dimension4 { get; set; }
        public IEnumerable<SelectListItem> Dimension4s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension5Code)]
        public string Dimension5 { get; set; }
        public IEnumerable<SelectListItem> Dimension5s { get; set; }
        [Display(Name = Dimensions.ShortcutDimension6Code)]
        public string Dimension6 { get; set; }
        public IEnumerable<SelectListItem> Dimension6s { get; set; }

        [Display(Name = Dimensions.ShortcutDimension7Code)]
        public string Dimension7 { get; set; }
        public IEnumerable<SelectListItem> Dimension7s { get; set; }
        public List<SelectListItem> ListofPrograms { get; set; }
        public List<SelectListItem> ListofStaff { get; set; }
        public List<SelectListItem> ListofFixedAssets { get; set; }
        public int EntryNo { get; set; }
        public string No { get; set; }
        public string VehicleNo { get; set; }
        public string From { get; set; }
        public string Too { get; set; }
        public string Purpose { get; set; }
    }
}