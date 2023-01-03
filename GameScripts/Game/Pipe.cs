using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SuperStomper.GameScripts.Engine;

namespace SuperStomper.GameScripts.Game
{
    internal class Pipe
    {
        private Sprite sprite;

        private const int pipeWidth = 32;
        private const int pipeHeight = 32;

        public Pipe(ContentManager content, Vector2 position)
        {
            sprite = new Sprite(content.Load<Texture2D>(@"Spritesheets\Environment\OverWorld"), new Rectangle(3 * pipeWidth, 0, pipeWidth, pipeHeight), Vector2.Zero, position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, sprite.position);
        }
    }
}
