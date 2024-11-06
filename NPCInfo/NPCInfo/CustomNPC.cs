using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.Quests;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NPCInfo
{
    public class GiftItem
    {
        public Item Item { get; }
        public Color GiftColor { get; }

        public GiftItem(Item item, Color giftColor)
        {
            Item = item;
            GiftColor = giftColor;
        }
    }

    public class CustomNPC
    {
        public NPC character;
        
        Color LighterGreen = new Color(200, 255, 200);  // Very light green
        Color LighterCyan = new Color(200, 255, 255);   // Very light cyan
        Color LighterPink = new Color(255, 200, 200);
        public string DisplayName => character.displayName;
        public string Name => character.Name;

        private int previousGiftsToday;
        private Friendship friendshipData;

        public CustomNPC(NPC character)
        {
            this.character = character;
            if (Game1.player.friendshipData.ContainsKey(character.Name))
            {
                friendshipData = Game1.player.friendshipData[character.Name];
                previousGiftsToday = friendshipData.GiftsToday;
            }
        }

        public int GetFriendshipPoints()
        {
            if (friendshipData == null) return 0;
            return friendshipData.Points; 
        }

        public int GetMaxPoints()
        {
            if (friendshipData == null) return 0;

            bool isDating = friendshipData.IsDating();

            return character.isMarried() ? 3500 :
                character.datable.Value && !isDating ? NPC.maxFriendshipPoints - 500 :
                NPC.maxFriendshipPoints;
        }

        public bool ShouldGift()
        {
            if (friendshipData == null) return character.CanSocialize;
            if (GetFriendshipPoints() >= GetMaxPoints()) return false;
            if (character.isBirthday()) return friendshipData.GiftsToday == 0;
            return friendshipData.GiftsThisWeek < 2 && friendshipData.GiftsToday == 0;
        }

        public bool ShouldSpeak()
        {
            if (friendshipData == null) return character.CanSocialize;
            if (GetFriendshipPoints() >= GetMaxPoints()) return false;
            return !friendshipData.TalkedToToday;
        }

        public bool IsQuestNPC()
        {
            foreach (var quest in Game1.player.questLog)
            {
                switch (quest)
                {
                    case ItemDeliveryQuest deliveryQuest when deliveryQuest.target.ToString() == this.Name:
                    case ResourceCollectionQuest collectionQuest when collectionQuest.target.ToString() == this.Name:
                        return true;
                }
            }
            return false;
        }

        public string GetDisplayName()
        {
            string displayName = character.displayName;

            if (character.isBirthday()) return $"! {displayName} !";

            return displayName;
        }

        public GiftItem GetLastGift()
        {
            string lastGiftId = LastGiftData.Instance.LastGifts.ContainsKey(Name) ? LastGiftData.Instance.LastGifts[Name] : "";
            if (string.IsNullOrEmpty(lastGiftId))
                return null;

            Item lastGift = ItemRegistry.Create(lastGiftId);
            Color giftColor = GetGiftTasteColor(lastGift);
            return new GiftItem(lastGift, giftColor);
        }

        private Color GetGiftTasteColor(Item lastGift)
        {
            if (lastGift == null) return Color.White;
            int taste = character.getGiftTasteForThisItem(lastGift);
            return taste switch
            {
                0 => Color.Green,     // Loved gift
                2 => Color.Blue, // Liked 
                4 => Color.Orange, // Disliked
                6 => Color.Red , //Hated
                8 => Color.Gray,   // Neutral
                _ => Color.White
            };
        }

        public bool CheckGiftsTodayChanged()
        {
            if (!Game1.player.friendshipData.ContainsKey(character.Name)) return false;
            friendshipData = Game1.player.friendshipData[character.Name];
            if (friendshipData == null) return false;
            if (friendshipData.GiftsToday > previousGiftsToday)
            {
                previousGiftsToday = friendshipData.GiftsToday;
                return true;
            }
            return false;
        }

        public Vector2 GetLabelTopPosition()
        {
            //Determine general location for the text / icons
            float x = character.Position.X + character.Sprite.SpriteWidth * 2;
            float y = character.Position.Y - character.Sprite.SpriteHeight - 20;
            Vector2 drawPosition = Game1.GlobalToLocal(new Vector2(x, y));
            return drawPosition;
        }
    }
}
