using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NPCInfo.NPCInfo;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Characters;

namespace NPCInfo
{
    public class ModEntry : Mod
    {
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
            CustomNPCManager.Instance.GiftsTodayIncreased += OnGiftsTodayIncreased;
            npcInfoUI = new NPCInfoUI(helper);
        }

        private void OnGiftsTodayIncreased(CustomNPC npc)
        {
            LastGiftData.Instance.LastGifts[npc.character.Name] = activeItem.QualifiedItemId;
            Monitor.Log($"Player gave {activeItem.Name} to {npc.DisplayName}", LogLevel.Info);
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
                //UpdateLastGifts();
                //ResetNpcToGiftList();
                CustomNPCManager.Instance.CheckGiftsTodayChanged();
                npcInfoUI.RenderNPCs(e.SpriteBatch);
                SetActiveItem();
            }
        }

        private void SetActiveItem() => activeItem = Game1.player.CurrentItem;

    }
}
