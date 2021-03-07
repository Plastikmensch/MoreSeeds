using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;
namespace MoreSeeds
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;

        private int lastQuality;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (!Game1.IsMultiplayer)
            {
                foreach (KeyValuePair<Vector2,SObject> pair in Game1.player.currentLocation.Objects.Pairs)
                {
                    if (IsSeedMaker(pair.Value.Name))
                    {
                        if (IsCrop(Game1.player.CurrentItem))
                        {
                            //Monitor.Log("Player holds Crop");
                            lastQuality = (Game1.player.CurrentItem as SObject).Quality;
                        }
                        if (pair.Value.heldObject.Value != null)
                        {
                            /*
                            Monitor.Log("Seed Maker has item");
                            foreach (var prop in pair.Value.heldObject.Value.GetType().GetProperties())
                            {
                                Monitor.Log($"Item: {prop.Name}: {prop.GetValue(pair.Value.heldObject.Value, null)}");
                            }
                            foreach (var prop in pair.Value.GetType().GetProperties())
                            {
                                Monitor.Log($"Seed Maker: {prop.Name}: {prop.GetValue(pair.Value, null)}");
                            }
                            */
                            if (pair.Value.heldObject.Value.Stack < 4) 
                            {
                                Monitor.Log("adding to stack");
                                pair.Value.heldObject.Value.Stack += Config.baseBonus;
                                Monitor.Log($"lastQuality: {lastQuality}");
                                //gold = 2, silver = 1, iridium = 4
                                switch (lastQuality)
                                {
                                    case 1:
                                        Monitor.Log("adding silver bonus");
                                        pair.Value.heldObject.Value.Stack += Config.silverBonus;
                                        break;
                                    case 2:
                                        Monitor.Log("adding gold bonus");
                                        pair.Value.heldObject.Value.Stack += Config.goldBonus;
                                        break;
                                    case 4:
                                        Monitor.Log("adding iridium bonus");
                                        pair.Value.heldObject.Value.Stack += Config.iridiumBonus;
                                        break;
                                    default:
                                        Monitor.Log("no bonus for quality");
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsSeedMaker(string name)
        {
            if(name.Contains("Seed Maker")) 
            {
                return true;
            }
            return false;
        }

        private bool IsCrop(Item item)
        {
            // exclude Fiber and Coffee Bean
            if (item == null || item.parentSheetIndex == 771 || item.parentSheetIndex == 433)
            {
                return false;
            }

            Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Crops");

            foreach (KeyValuePair<int, string> pair in dictionary)
            {
                if (Convert.ToInt32(pair.Value.Split('/')[3]) == item.parentSheetIndex)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
