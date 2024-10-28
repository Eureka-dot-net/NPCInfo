using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

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
                DrawAllNPCInfo(e);
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

        private void DrawAllNPCInfo(RenderedWorldEventArgs e)
        {
            foreach (NPC character in Game1.currentLocation.characters)
            {
                if (character.CanSocialize)
                {
                    CustomNPC npc = new CustomNPC(character,giftData);

                    if (npc.ShouldGift())
                        npcToGift.Add(npc);

                    DrawNPCInfo(
                        e.SpriteBatch,
                        npc
                    );
                }
            }
        }

        private void DrawNPCInfo(SpriteBatch spriteBatch, CustomNPC npc)
        {
            string name = npc.GetDisplayName();
            bool shouldGift = npc.ShouldGift();
            bool shouldSpeak = npc.ShouldSpeak();
            bool isBirthday = npc.character.isBirthday();
            SpriteFont font = Game1.smallFont;

            //Determine general location for the text / icons
            float x = npc.character.Position.X + 32f;
            float y = npc.character.Position.Y - 64f * 1.25f;
            Vector2 drawPosition = Game1.GlobalToLocal(new Vector2(x, y));

            // Determine the position for the name
            float nameY = drawPosition.Y;
            GiftItem lastGiftItem = npc.GetLastGift();
          
            //Draw last gift
            if (lastGiftItem != null && shouldGift)
            {
                Item lastGift = lastGiftItem.Item;

                Vector2 giftPosition = new Vector2(npc.character.Position.X, npc.character.Position.Y);

                // Draw last gift info
                spriteBatch.DrawString(font, lastGift.Name, new Vector2(drawPosition.X, nameY), lastGiftItem.GiftColor, 0f, font.MeasureString(lastGift.Name) / 2f, 1f, SpriteEffects.None, 1f);
               
                nameY -= 30;
            }

            // Draw NPC name
            Vector2 textSize = font.MeasureString(name);
            spriteBatch.DrawString(font, name, new Vector2(drawPosition.X, nameY), Color.White, 0f, textSize / 2f, 1f, SpriteEffects.None, 1f);

            // Draw icons to the side
            float iconX = drawPosition.X + textSize.X / 2 + 5;

            if (shouldGift)
            {
                var giftPos = new Vector2(drawPosition.X - 45 - (textSize.X / 2f), nameY - 20);

                spriteBatch.Draw(isBirthday ? birthdayIcon : giftIcon, giftPos, Color.White);
            }
            if (shouldSpeak)
            {
                spriteBatch.Draw(speakIcon, new Vector2(iconX, nameY - 20), Color.White);
            }
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
    }
}
