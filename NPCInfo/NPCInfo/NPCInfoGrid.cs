using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPCInfo.NPCInfo
{
    public class NPCInfoGrid
    {
        private List<List<GridItem>> rows = new List<List<GridItem>>();

        public void AddRow(params GridItem[] items)
        {
            rows.Add(items.ToList());
        }

        public void Draw(SpriteBatch spriteBatch, CustomNPC npc)
        {

            Vector2 drawPosition = npc.GetLabelTopPosition();

            for (int i = rows.Count - 1; i >= 0; i--)
            {
                var row = rows[i];
                float rowHeight = 0f;
                
                var totalRowLength = 0f;
                var maxColLength = 0f;
                foreach (var item in row)
                {
                    totalRowLength += item.Width;
                    maxColLength = Math.Max(maxColLength, item.Height);
                }
                var startX = drawPosition.X - totalRowLength / 2f;
                Vector2 columnPosition = new Vector2(startX, drawPosition.Y - maxColLength);
                foreach (var item in row)
                {
                    item.Draw(spriteBatch, columnPosition);
                    columnPosition.X += item.Width + 5;  // Adjust spacing as needed
                    rowHeight = Math.Max(rowHeight, item.Height);
                }

                drawPosition.Y -= rowHeight - 5; // Adjust row spacing as needed
            }
        }
    }

    public abstract class GridItem
    {
        public abstract void Draw(SpriteBatch spriteBatch, Vector2 position);
        public abstract float Width { get; }
        public abstract float Height { get; }
    }

    public class TextItem : GridItem
    {
        private string text;
        private Color color;
        SpriteFont font;

        public TextItem(string text, Color color, SpriteFont font)
        {
            this.text = text;
            this.color = color;
            this.font = font;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Vector2 shadowOffset = new Vector2(2, 2);           
            spriteBatch.DrawString(font, text, position + shadowOffset, Color.Black); // Shadow
            shadowOffset = new Vector2(1, 1);
            spriteBatch.DrawString(font, text, position + shadowOffset, Color.White);
            spriteBatch.DrawString(font, text, position, color);

        }

        public override float Width => font.MeasureString(text).X;
        public override float Height => font.MeasureString(text).Y;
    }

    public class TextureItem : GridItem
    {
        private Texture2D texture;

        public TextureItem(Texture2D texture)
        {
            this.texture = texture;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public override float Width => texture.Width;
        public override float Height => texture.Height;
    }

}
