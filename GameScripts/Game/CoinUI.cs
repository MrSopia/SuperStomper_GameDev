using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SuperStomper.GameScripts.Engine;

namespace SuperStomper.GameScripts.Game
{
    internal class CoinUI
    {
        private Sprite sprite;
        private Animation animation;
        private SpriteFont font;
        private Windowbox windowbox;

        private const int coinWidth = 16;
        private const int coinHeight = 16;

        public CoinUI(ContentManager content, Windowbox windowbox, SpriteFont font)
        {
            this.font = font;
            this.windowbox = windowbox;

            sprite = new Sprite(content.Load<Texture2D>(@"Spritesheets\Objects\Items"), new Rectangle(0, coinHeight * 2, coinWidth, coinHeight), Vector2.Zero);
            animation = new Animation(sprite.texture, 0.3f, true, new int[] { 10, 11, 12, 13 }, sprite.sourceRect.Size);
        }

        public void Update(float deltaTime)
        {
            animation.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch, int numberOfCoins, Vector2? position = null)
        {
            sprite.sourceRect = animation.GetCurrentFrame();
            sprite.Draw(spriteBatch, position == null ? -GameManager.cameraPosition + new Vector2(windowbox.GetScaledRect().Width - coinWidth - 20, 3) : (Vector2)position - new Vector2(coinWidth, -2));

            spriteBatch.DrawString(font, "x" + numberOfCoins.ToString(), position == null ? -GameManager.cameraPosition + new Vector2(windowbox.GetScaledRect().Width - 20, 1) : (Vector2)position, Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
        }
    }
}
