using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPCInfo.Config
{
    public sealed class ModConfig
    {
        public bool Enabled { get; set; }
        public bool ShowName { get; set; }

        public bool ShowShouldGift { get; set; }

        public bool ShowShouldSpeak { get; set; }

        public bool ShowLastGift { get; set; }

        public int NumGiftsPerWeek { get; set; }

        public ModConfig()
        {
            Enabled = true;
            ShowName = true;
            ShowShouldGift = true;
            ShowShouldSpeak = true;
            ShowLastGift = true;
            NumGiftsPerWeek = 2;
        }
    }
}
