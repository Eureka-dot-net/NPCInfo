using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPCInfo
{
    public class LastGiftData
    {
        public Dictionary<string, string> LastGifts { get; set; } = new Dictionary<string, string>();

        public void LoadData(IModHelper helper)
        {
            // Load or initialize last gift data
            var loadedData = helper.Data.ReadSaveData<LastGiftData>("LastGiftData");
            LastGifts = loadedData?.LastGifts ?? new Dictionary<string, string>();
        }

        public void SaveData(IModHelper helper)
        {
            // Save last gift data
            helper.Data.WriteSaveData("LastGiftData", this);
        }

        public void UpdateLastGift(string npcName, string itemId)
        {
            LastGifts[npcName] = itemId;
        }

        private LastGiftData() { }

        private static LastGiftData instance;
        public static LastGiftData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LastGiftData();
                }
                return instance;
            }
        }
    }
}
