using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NPCInfo.NPCInfo;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            NPCInfoGrid grid = new NPCInfoGrid();
            List<GridItem> nameRow = new List<GridItem>();
            if (npc.ShouldGift())
            {
                nameRow.Add(new TextureItem(this.giftIcon));
            }
            nameRow.Add(new TextItem(npc.DisplayName, Color.White, Game1.smallFont));

            if (npc.ShouldSpeak())
            {
                nameRow.Add(new TextureItem(this.speakIcon));
            }
            grid.AddRow(nameRow.ToArray());

            GiftItem lastGiftItem = npc.GetLastGift();
            if (lastGiftItem != null && npc.ShouldGift())
            {
                Item lastGift = lastGiftItem.Item;
                grid.AddRow(new TextItem(lastGift.DisplayName, lastGiftItem.GiftColor, Game1.smallFont));
            }

            grid.Draw(spriteBatch,npc);

        }
    }
}
