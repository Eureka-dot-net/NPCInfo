using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPCInfo
{
    public class NPCInfoRenderer
    {
        private readonly Texture2D speakIcon;
        private readonly Texture2D giftIcon;
        private readonly Texture2D birthdayIcon;

        public NPCInfoRenderer(Texture2D speakIcon, Texture2D giftIcon, Texture2D birthdayIcon)
        {
            this.speakIcon = speakIcon;
            this.giftIcon = giftIcon;
            this.birthdayIcon = birthdayIcon;
        }

        public void DrawNPCInfo(SpriteBatch spriteBatch, CustomNPC npc)
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
    }
}
