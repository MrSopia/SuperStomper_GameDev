using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SuperStomper.GameScripts.Engine;

namespace SuperStomper.GameScripts.Game
{
    internal class Coin
    {
        public Hitbox hitbox;

        private Sprite sprite;
        private Animation animation;

        private const int coinWidth = 16;
        private const int coinHeight = 16;

        public Coin(ContentManager content, Vector2 position)
        {
            sprite = new Sprite(content.Load<Texture2D>(@"Spritesheets\Objects\Items"), new Rectangle(0, coinHeight, coinWidth, coinHeight), Vector2.Zero, position);
            animation = new Animation(sprite.texture, 0.25f, true, new int[] { 5, 6, 7, 8 }, sprite.sourceRect.Size);

            hitbox = new Hitbox(new Rectangle((int)position.X, (int)position.Y, coinWidth, coinHeight), Vector2.Zero);
        }

        public void Update(float deltaTime)
        {
            animation.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.sourceRect = animation.GetCurrentFrame();
            sprite.Draw(spriteBatch, sprite.position);
        }
    }
}
