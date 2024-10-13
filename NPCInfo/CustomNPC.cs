using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace NPCInfo
{
    internal class CustomNPC
    {
        public NPC character;

        public string Name => character.Name;

        public CustomNPC(NPC character)
        {
            this.character = character;
        }

        public int GetFriendshipPoints()
        {
            if (!Game1.player.friendshipData.ContainsKey(character.Name)) return 0;

            var friendshipData = Game1.player.friendshipData[character.Name];
            return friendshipData.Points;
        }

        public int GetMaxPoints()
        {
            if (!Game1.player.friendshipData.ContainsKey(character.Name)) return 0;

            bool isDating = Game1.player.friendshipData[character.Name].IsDating();

            return character.isMarried() ? 3500 :
                character.datable.Value && !isDating ? NPC.maxFriendshipPoints - 500 :
                NPC.maxFriendshipPoints;
        }

        public bool ShouldGift()
        {
            if (GetFriendshipPoints() >= GetMaxPoints()) return false;
            if (!Game1.player.friendshipData.ContainsKey(character.Name)) return false;

            var friendshipData = Game1.player.friendshipData[character.Name];
            return friendshipData.GiftsThisWeek < 2 && friendshipData.GiftsToday == 0;
        }

        public bool ShouldSpeak()
        {
            if (!Game1.player.friendshipData.ContainsKey(character.Name)) return false;

            return !Game1.player.friendshipData[character.Name].TalkedToToday;
        }

        public string GetDisplayName()
        {
            string displayName = character.displayName;

            if (character.isBirthday()) return $"!!! {displayName} !!!";

            if (ShouldGift()) displayName = $"[G] {displayName}";
            if (ShouldSpeak()) displayName = $"[S] {displayName}";

            return displayName;
        }

        public string GetTodoList()
        {
            List<string> tasks = new List<string>();

            if (ShouldGift()) tasks.Add("Gift");
            if (ShouldSpeak()) tasks.Add("Speak");

            return tasks.Count > 0 ? "ToDo: " + string.Join(", ", tasks) : string.Empty;
        }

        public string GetLovedItems()
        {
            return string.Join(", ", Game1.player.Items
                .Where(i => i != null && character.getGiftTasteForThisItem(i) == 0)
                .Select(i => i.Name));
        }
    }
}
