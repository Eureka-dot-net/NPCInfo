using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NPCInfo.Config;
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
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            // Load icons
            speakIcon = Helper.ModContent.Load<Texture2D>("assets/speakIcon.png");
            giftIcon = Helper.ModContent.Load<Texture2D>("assets/giftIcon.png");
            birthdayIcon = Helper.ModContent.Load<Texture2D>("assets/birthdayIcon.png");

            // Register events
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            CustomNPCManager.Instance.GiftsTodayIncreased += OnGiftsTodayIncreased;
            npcInfoUI = new NPCInfoUI(helper);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enabled",
                tooltip: () => "Enable / Disable all displays.",
                getValue: () => this.Config.Enabled,
                setValue: value => this.Config.Enabled = value
            );

            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Individual Display Options"
                );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show name",
                tooltip: () => "Should the name of the NPC be displayed",
                getValue: () => this.Config.ShowName,
                setValue: value => this.Config.ShowName = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show should gift",
                tooltip: () => "Displays a gift icon if we should give the NPC a gift",
                getValue: () => this.Config.ShowShouldGift,
                setValue: value => this.Config.ShowShouldGift = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show should speak",
                tooltip: () => "Displays a speech bubble if we haven't spoken to the NPC and we need friendship",
                getValue: () => this.Config.ShowShouldSpeak,
                setValue: value => this.Config.ShowShouldSpeak = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show last gift",
                tooltip: () => "Disiplays the last gift that was given to the NPC",
                getValue: () => this.Config.ShowLastGift,
                setValue: value => this.Config.ShowLastGift = value
            );
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
            CustomNPCManager.Instance.CheckGiftsTodayChanged();
            if (!Game1.eventUp && Config.Enabled)
            {
                npcInfoUI.RenderNPCs(e.SpriteBatch, Config);
            }
            SetActiveItem();

        }

        private void SetActiveItem() => activeItem = Game1.player.CurrentItem;

    }
}
