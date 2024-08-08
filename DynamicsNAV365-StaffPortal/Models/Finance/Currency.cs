using Newtonsoft.Json;

namespace DynamicsNAV365_StaffPortal.Models.Finance
{
    public class Currency
    {
        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Last_Date_Modified")]
        public string LastDateModified { get; set; }

        [JsonProperty("Last_Date_Adjusted")]
        public string LastDateAdjusted { get; set; }

        [JsonProperty("ISO_Code")]
        public string IsoCode { get; set; }

        [JsonProperty("ISO_Numeric_Code")]
        public string IsoNumericCode { get; set; }

        [JsonProperty("Unrealized_Gains_Acc")]
        public string UnrealizedGainsAcc { get; set; }

        [JsonProperty("Realized_Gains_Acc")]
        public string RealizedGainsAcc { get; set; }

        [JsonProperty("Unrealized_Losses_Acc")]
        public string UnrealizedLossesAcc { get; set; }

        [JsonProperty("Realized_Losses_Acc")]
        public string RealizedLossesAcc { get; set; }
    }
}