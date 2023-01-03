using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SuperStomper.GameScripts.Engine;

namespace SuperStomper.GameScripts.Game
{
    internal class Castle
    {
        public Hitbox hitbox;

        private Sprite sprite;

        private const int castleWidth = 80;
        private const int castleHeight = 80;

        public Castle(ContentManager content, Vector2 position)
        {
            sprite = new Sprite(content.Load<Texture2D>(@"Spritesheets\Environment\Castle"), new Rectangle(0, 0, castleWidth, castleHeight), Vector2.Zero, position);

            hitbox = new Hitbox(new Rectangle((int)position.X + (2 * castleWidth / 5), (int)position.Y + (4 * castleHeight / 5), castleWidth / 5, castleHeight / 5), Vector2.Zero);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, sprite.position);
        }
    }
}
