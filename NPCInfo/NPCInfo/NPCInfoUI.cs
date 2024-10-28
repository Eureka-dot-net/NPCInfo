using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NPCInfo.NPCInfo;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPCInfo
{
    public class NPCInfoUI
    {
        private readonly NPCInfoRenderer npcInfoRenderer;

        public NPCInfoUI(IModHelper helper)
        {
            Texture2D speakIcon = helper.ModContent.Load<Texture2D>("assets/speakIcon.png");
            Texture2D giftIcon = helper.ModContent.Load<Texture2D>("assets/giftIcon.png");
            Texture2D birthdayIcon = helper.ModContent.Load<Texture2D>("assets/birthdayIcon.png");

            npcInfoRenderer = new NPCInfoRenderer(speakIcon, giftIcon, birthdayIcon);
        }

        public void RenderNPCs(SpriteBatch spriteBatch)
        {
            foreach (var character in Game1.currentLocation.characters)
            {
                if (character.CanSocialize)
                {
                    CustomNPC npc = CustomNPCManager.Instance.GetOrCreateCustomNPC(character);
                    npcInfoRenderer.DrawNPCInfo(spriteBatch, npc);
                }
            }
        }
    }
}
