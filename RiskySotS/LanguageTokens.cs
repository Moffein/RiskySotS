using R2API;

namespace RiskySotS
{
    internal class LanguageTokens
    {
        public LanguageTokens()
        {
            LanguageAPI.Add("SHRINE_COLOSSUS_RISKYSOTS_NAME", "Shrine of the Colossus");
            LanguageAPI.Add("SHRINE_COLOSSUS_RISKYSOTS_CONTEXT", "Pray to Shrine of the Colossus");
            LanguageAPI.Add("SHRINE_COLOSSUS_RISKYSOTS_DESCRIPTION", "A Shrine that marks the path towards the Meridian.\n\nActivate the Shrine on 2 consecutive stages to guarantee a Halcyon Shrine on the 3rd stage.");
            LanguageAPI.Add("SHRINE_COLOSSUS_RISKYSOTS_LORE", "LORE HERE");

            LanguageAPI.Add("SHRINE_COLOSSUS_RISKYSOTS_ACTIVATION_1", "<style=cWorldEvent>Lightning flashes in the distance.</style>");
            LanguageAPI.Add("SHRINE_COLOSSUS_RISKYSOTS_TP_FINISH", "<style=cWorldEvent>The storm draws closer.</style>");

            //Your language here
            //LanguageAPI.Add("SHRINE_COLOSSUS_RISKYSOTS_NAME", "", "ru");
            //LanguageAPI.Add("SHRINE_COLOSSUS_RISKYSOTS_CONTEXT", "", "ru");
            //LanguageAPI.Add("SHRINE_COLOSSUS_RISKYSOTS_DESCRIPTION", "", "ru");
            //LanguageAPI.Add("SHRINE_COLOSSUS_RISKYSOTS_LORE", "", "ru");
            //etc.
        }
    }
}
