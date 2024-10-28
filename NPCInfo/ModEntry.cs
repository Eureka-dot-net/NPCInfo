using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace NPCInfo
{
    public class ModEntry : Mod
    {
        private List<CustomNPC> npcToGift;
        private Item activeItem;
        private Texture2D speakIcon;
        private Texture2D giftIcon;
        private Texture2D birthdayIcon;
        private NPCInfoUI npcInfoUI;

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

            npcInfoUI = new NPCInfoUI(helper);
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Load or initialize last gift data

            LastGiftData.Instance.LoadData(this.Helper);
            Monitor.Log("Last gift data loaded", LogLevel.Debug);
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            // Save last gift data
            LastGiftData.Instance.SaveData(Helper);
            Monitor.Log("Last gift data saved", LogLevel.Debug);
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!Game1.eventUp)
            {
                UpdateLastGifts();
                ResetNpcToGiftList();
                SetActiveItem();
                npcInfoUI.RenderNPCs(e.SpriteBatch);
            }
        }

        private void UpdateLastGifts()
        {
            if (npcToGift == null) return;

            foreach (CustomNPC npc in npcToGift)
            {
                if (!npc.ShouldGift()) // Add or update the last gift for eligible NPCs
                {
                    LastGiftData.Instance.LastGifts[npc.character.Name] = activeItem.QualifiedItemId;
                    break;
                }
            }
        }

        private void ResetNpcToGiftList() => npcToGift = new List<CustomNPC>();

        private void SetActiveItem() => activeItem = Game1.player.CurrentItem;

    }
}
