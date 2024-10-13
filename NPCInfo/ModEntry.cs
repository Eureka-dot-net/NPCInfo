using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AchtuurCore.Utility;
using HoverLabels;
using System.Collections.Generic;

namespace NPCInfo
{
    public class LastGiftData
    {
        public Dictionary<string, string> LastGifts { get; set; } = new Dictionary<string, string>();
    }

    public class ModEntry : Mod
    {
        private LastGiftData giftData;
        private List<CustomNPC> npcToGift;
        private Item activeItem;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Load saved gift data (or create new if none exists)
            giftData = Helper.Data.ReadSaveData<LastGiftData>("LastGiftData") ?? new LastGiftData();
            Monitor.Log("Last gift data loaded", LogLevel.Debug);
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            // Save current gift data
            Helper.Data.WriteSaveData("LastGiftData", giftData);
            Monitor.Log("Last gift data saved", LogLevel.Debug);
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            Helper.Data.WriteSaveData("LastGiftData", giftData);
            DrawNames(e);
        }

        public Item GetLastGiftForNPC(string npcName)
        {
            if (giftData == null) giftData = new LastGiftData();

            // Retrieve the last gift item by its ID
            string lastGiftId = giftData.LastGifts.ContainsKey(npcName) ? giftData.LastGifts[npcName] : "";
            return !string.IsNullOrEmpty(lastGiftId) ? ItemRegistry.Create(lastGiftId) : null;
        }

        private void DrawNames(RenderedWorldEventArgs e)
        {
            giftData = Helper.Data.ReadSaveData<LastGiftData>("LastGiftData") ;
            if (Game1.eventUp) return; // Exit early if an event is active

            UpdateLastGifts();
            ResetNpcToGiftList();
            SetActiveItem();

            SpriteFont smallFont = Game1.smallFont;

            // Draw NPC names and last gift information
            foreach (NPC character in Game1.currentLocation.characters)
            {
                if (!character.CanSocialize) continue;

                CustomNPC npc = new CustomNPC(character);

                if (npc.ShouldGift()) npcToGift.Add(npc); // Add to NPC list if eligible for a gift

                DrawText(e.SpriteBatch, npc.GetDisplayName(), character.Position, smallFont, 1.25f, Color.White);

                DrawLastGiftInfo(e, npc, character, smallFont);
            }
        }

        private void UpdateLastGifts()
        {
            if (npcToGift == null) return;

            foreach (CustomNPC npc in npcToGift)
            {
                if (!npc.ShouldGift()) // Update or add the last gift for each NPC
                {
                    string npcName = npc.character.Name;
                    string giftId = activeItem.QualifiedItemId;

                    giftData.LastGifts[npcName] = giftId;
                    break; // Update only one NPC's last gift per draw cycle
                }
            }
        }

        private void ResetNpcToGiftList()
        {
            npcToGift = new List<CustomNPC>(); // Reset the list of NPCs to gift
        }

        private void SetActiveItem()
        {
            // Set the current item the player is holding as the active item
            activeItem = Game1.player.CurrentItem;
        }

        private void DrawLastGiftInfo(RenderedWorldEventArgs e, CustomNPC npc, NPC character, SpriteFont font)
        {
            Item lastGift = GetLastGiftForNPC(npc.character.Name);
            if (lastGift == null || !npc.ShouldGift()) return;

            Color color = GetGiftTasteColor(npc.character.getGiftTasteForThisItem(lastGift));

            Vector2 giftPosition = new Vector2(character.Position.X, character.Position.Y + 30);
            DrawText(e.SpriteBatch, lastGift.Name, giftPosition, font, 1.25f, color);
        }

        private Color GetGiftTasteColor(int giftTaste)
        {
            return giftTaste switch
            {
                0 => Color.LightGreen,    // Loved gift
                1 or 2 => Color.LightCyan, // Liked or neutral gift
                _ => Color.LightPink,     // Disliked gift
            };
        }

        private void DrawText(SpriteBatch spriteBatch, string name, Vector2 position, SpriteFont font, float heightMultiplier, Color color)
        {
            Vector2 textSize = font.MeasureString(name);
            float x = position.X + 32f;
            float y = position.Y - 64f * heightMultiplier;

            Vector2 drawPosition = Game1.GlobalToLocal(new Vector2(x, y));
            spriteBatch.DrawString(font, name, drawPosition, color, 0f, textSize / 2f, 1f, SpriteEffects.None, 1f);
        }
    }
}
