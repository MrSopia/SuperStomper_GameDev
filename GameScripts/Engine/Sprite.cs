using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperStomper.GameScripts.Engine
{
    internal class Sprite
    {
        public float rotation;
        public float scale;
        public Color color;
        public SpriteEffects spriteEffects;
        public Rectangle sourceRect;
        public Vector2 position { private set; get; }
        public Texture2D texture { private set; get; }

        private Vector2 origin;

        public Sprite(Texture2D texture, Rectangle sourceRect, Vector2 origin, Vector2? position = null)
        {
            this.sourceRect = sourceRect;
            this.texture = texture;
            this.origin = origin;
            this.position = position != null ? (Vector2)position : Vector2.Zero;

            rotation = 0;
            scale = 1;
            color = Color.White;
            spriteEffects = SpriteEffects.None;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(texture, position, sourceRect, color, rotation, origin, scale, spriteEffects, 0);
        }
    }
}
