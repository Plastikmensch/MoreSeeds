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
        
        /// <inheritdoc/>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            //TODO: Multiplayer support
            if (!Context.IsWorldReady || Game1.IsMultiplayer) return;

            foreach (KeyValuePair<Vector2,SObject> pair in Game1.player.currentLocation.Objects.Pairs)
            {
                if (IsSeedMaker(pair.Value) && IsPlayerInRange(pair.Value, Game1.player))
                {
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

                        // Apply bonus if Stack is lower than default values
                        //TODO: Find a better way to keep track whether a bonus was already applied
                        if (pair.Value.heldObject.Value.Stack < 4) 
                        {
                            Monitor.Log("adding to stack");
                            pair.Value.heldObject.Value.Stack += Config.baseBonus;

                            //gold = 2, silver = 1, iridium = 4
                            switch(pair.Value.lastInputItem.Value.Quality)
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

        /// <summary>
        /// Checks whether obj is a Seed Maker
        /// </summary>
        /// <param name="obj">Stardew Valley object to check</param>
        private bool IsSeedMaker(SObject obj)
        {
            return obj != null && obj.Name.Contains("Seed Maker");
        }

        /// <summary>
        /// Check whether player is in range of given object,
        /// or if a Hopper is attached, in the range of the Hopper.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// Thrown when obj or who is null
        /// </exception>
        /// <param name="obj">Object to test for</param>
        /// <param name="who">Farmer to test</param>
        /// <param name="withHopper"></param>
        /// <param name="range">Numerical range of position</param>
        private bool IsPlayerInRange(SObject obj, Farmer who, int range = 1)
        {
            Vector2 farmerPosition = who.Tile;
            Vector2 position = obj.TileLocation;

            // "Borrows" the logic in Utility.tileWithinRadiusOfPlayer, adjusted to also check Hopper position.
            // Hopper x coordinate is same as position x, so no need to check x position twice.
            // However y coordinate is different as Hopper is above seed maker. 
            if (Math.Abs(position.X - farmerPosition.X) <= range)
            {
                // Whether player is 1 tile away from position or from hopper above
                return Math.Abs(position.Y - farmerPosition.Y) <= range || (IsHopperAttached(obj) && Math.Abs(position.Y - 1 - farmerPosition.Y) <= range);
            }
            return false;
        }

        /// <summary>
        /// Checks whether a Hopper is attached to object
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// Thrown when obj is null
        /// </exception>
        /// <param name="obj">Object to check</param>
        private bool IsHopperAttached(SObject obj)
        {
            Vector2 hopperPosition = new(obj.TileLocation.X, obj.TileLocation.Y - 1);
            return obj.Location.Objects.Pairs.Any(o => o.Key == hopperPosition && o.Value.Name.Equals("Hopper"));
        }
    }
}
