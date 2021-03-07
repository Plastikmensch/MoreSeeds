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
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
        }

        // Doesn't update fast enough, can be removed
        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (e.IsLocalPlayer)
            {
                Monitor.Log("Inventory changed");
                foreach (Item item in e.Removed)
                {
                    Monitor.Log($"removed: {item.Name}");
                    if (IsCrop(item))
                    {
                        Monitor.Log("Item is Crop");
                        //lastQuality = (item as SObject).Quality;
                    }
                    Monitor.Log($"removed category: {item.Category} {item.getCategoryName()}");
                    Monitor.Log($"removed index: {item.ParentSheetIndex}");
                    Monitor.Log($"removed quality: {(item as SObject).Quality}");
                }
                foreach (ItemStackSizeChange item in e.QuantityChanged)
                {
                    Monitor.Log("Stack size changed");
                    if (item.OldSize > item.NewSize)
                    {
                        Monitor.Log($"removed: {item.Item.Name}");
                        if (IsCrop(item.Item))
                        {
                            Monitor.Log("Item is Crop");
                            //lastQuality = (item.Item as SObject).Quality;
                        }
                        Monitor.Log($"removed category: {item.Item.Category} {item.Item.getCategoryName()}");
                        Monitor.Log($"removed index: {item.Item.ParentSheetIndex}");
                        Monitor.Log($"removed quality: {(item.Item as SObject).Quality}");
                    }
                }
            }
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
                            Monitor.Log("Player holds Crop");
                            lastQuality = (Game1.player.CurrentItem as SObject).Quality;
                        }
                        if (pair.Value.heldObject.Value != null)
                        {
                            Monitor.Log("Seed Maker has item");
                            foreach (var prop in pair.Value.heldObject.Value.GetType().GetProperties())
                            {
                                Monitor.Log($"Item: {prop.Name}: {prop.GetValue(pair.Value.heldObject.Value, null)}");
                            }
                            foreach (var prop in pair.Value.GetType().GetProperties())
                            {
                                Monitor.Log($"Seed Maker: {prop.Name}: {prop.GetValue(pair.Value, null)}");
                            }
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
