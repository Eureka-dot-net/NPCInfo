using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private Texture2D speakIcon;
        private Texture2D giftIcon;
        private Texture2D birthdayIcon;

        public override void Entry(IModHelper helper)
        {
            // Load icons
            speakIcon = Helper.ModContent.Load<Texture2D>("assets/speakIcon.png");
            giftIcon = Helper.ModContent.Load<Texture2D>("assets/giftIcon.png");
            birthdayIcon = Helper.ModContent.Load<Texture2D>("assets/birthdayIcon.png");

            // Register events
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Load or initialize last gift data
            giftData = Helper.Data.ReadSaveData<LastGiftData>("LastGiftData") ?? new LastGiftData();
            Monitor.Log("Last gift data loaded", LogLevel.Debug);
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            // Save last gift data
            Helper.Data.WriteSaveData("LastGiftData", giftData);
            Monitor.Log("Last gift data saved", LogLevel.Debug);
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!Game1.eventUp)
            {
                UpdateLastGifts();
                ResetNpcToGiftList();
                SetActiveItem();
                DrawNPCInfo(e);
            }
        }

        private void UpdateLastGifts()
        {
            if (npcToGift == null) return;

            foreach (CustomNPC npc in npcToGift)
            {
                if (!npc.ShouldGift()) // Add or update the last gift for eligible NPCs
                {
                    giftData.LastGifts[npc.character.Name] = activeItem.QualifiedItemId;
                    break;
                }
            }
        }

        private void ResetNpcToGiftList() => npcToGift = new List<CustomNPC>();

        private void SetActiveItem() => activeItem = Game1.player.CurrentItem;

        private void DrawNPCInfo(RenderedWorldEventArgs e)
        {
            SpriteFont font = Game1.smallFont;

            foreach (NPC character in Game1.currentLocation.characters)
            {
                if (character.CanSocialize)
                {
                    CustomNPC npc = new CustomNPC(character);

                    if (npc.ShouldGift()) npcToGift.Add(npc);

                    DrawNameAndIcons(
                        e.SpriteBatch,
                        npc.GetDisplayName(),
                        character.Position,
                        font,
                        1.25f,
                        Color.White,
                        npc.ShouldGift(),
                        npc.ShouldSpeak(),
                        character.isBirthday()
                    );

                    DrawLastGiftInfo(e, npc, character, font);
                }
            }
        }

        private void DrawNameAndIcons(SpriteBatch spriteBatch, string name, Vector2 position, SpriteFont font, float heightMultiplier, Color color, bool shouldGift, bool shouldSpeak, bool isBirthday)
        {
            Vector2 textSize = font.MeasureString(name);
            float x = position.X + 32f;
            float y = position.Y - 64f * heightMultiplier;

            Vector2 drawPosition = Game1.GlobalToLocal(new Vector2(x, y - 30));

            // Draw birthday icon on the left of the name
            if (isBirthday)
            {
                spriteBatch.Draw(birthdayIcon, new Vector2(drawPosition.X - birthdayIcon.Width - 40, drawPosition.Y - 25), Color.White);
            }

            // Draw NPC name
            spriteBatch.DrawString(font, name, drawPosition, color, 0f, textSize / 2f, 1f, SpriteEffects.None, 1f);

            // Draw icons to the right of the name
            float iconX = drawPosition.X + textSize.X / 2 + 5;

            if (shouldGift)
            {
                spriteBatch.Draw(giftIcon, new Vector2(iconX, drawPosition.Y - 20), Color.White);
                iconX += giftIcon.Width;
            }

            if (shouldSpeak)
            {
                spriteBatch.Draw(speakIcon, new Vector2(iconX, drawPosition.Y - 20), Color.White);
            }
        }

        private void DrawLastGiftInfo(RenderedWorldEventArgs e, CustomNPC npc, NPC character, SpriteFont font)
        {
            Item lastGift = GetLastGiftForNPC(npc.character.Name);
            if (lastGift == null || !npc.ShouldGift()) return;

            Color color = GetGiftTasteColor(npc.character.getGiftTasteForThisItem(lastGift));
            Vector2 giftPosition = new Vector2(character.Position.X, character.Position.Y);

            DrawText(e.SpriteBatch, lastGift.Name, giftPosition, font, 1.25f, color);
        }

        private Color GetGiftTasteColor(int giftTaste)
        {
            return giftTaste switch
            {
                0 => Color.LightGreen,     // Loved gift
                1 or 2 => Color.LightCyan, // Liked or neutral gift
                _ => Color.LightPink       // Disliked gift
            };
        }

        private void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, SpriteFont font, float heightMultiplier, Color color)
        {
            Vector2 textSize = font.MeasureString(text);
            float x = position.X + 32f;
            float y = position.Y - 64f * heightMultiplier;

            Vector2 drawPosition = Game1.GlobalToLocal(new Vector2(x, y));
            spriteBatch.DrawString(font, text, drawPosition, color, 0f, textSize / 2f, 1f, SpriteEffects.None, 1f);
        }

        public Item GetLastGiftForNPC(string npcName)
        {
            giftData ??= new LastGiftData();
            string lastGiftId = giftData.LastGifts.ContainsKey(npcName) ? giftData.LastGifts[npcName] : "";
            return !string.IsNullOrEmpty(lastGiftId) ? ItemRegistry.Create(lastGiftId) : null;
        }
    }
}
