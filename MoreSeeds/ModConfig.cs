using System;
namespace MoreSeeds
{
    public class ModConfig
    {
        // the base amount of seeds
        public int baseBonus { get; set; } = 5;
        // additional bonus for silver crops
        public int silverBonus { get; set; } = 1;
        // additional bonus for gold crops
        public int goldBonus { get; set; } = 2;
        // additional bonus for iridium crops
        public int iridiumBonus { get; set; } = 3;
    }
}
