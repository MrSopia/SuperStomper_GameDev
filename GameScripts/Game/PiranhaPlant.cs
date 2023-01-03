using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SuperStomper.GameScripts.Engine;

namespace SuperStomper.GameScripts.Game
{
    internal class PiranhaPlant
    {
        public Hitbox hitbox;

        private Sprite sprite;
        private Animation animation;
        private Movement movement;
        private int direction;
        private float distanceMoved;
        private float timer;
        private Vector2 initialPosition;

        private const int piranhaWidth = 32;
        private const int piranhaHeight = 32;
        private const float speed = 20;
        private const float showAndHideDuration = 1f;

        public PiranhaPlant(ContentManager content, Vector2 position)
        {
            initialPosition = position;

            sprite = new Sprite(content.Load<Texture2D>(@"Spritesheets\Enemies\Enemies"), new Rectangle(7 * piranhaWidth, 0, piranhaWidth, piranhaHeight), Vector2.Zero, position);
            animation = new Animation(sprite.texture, 0.25f, true, new int[] { 7, 8 }, sprite.sourceRect.Size);
            movement = new Movement(position);
            hitbox = new Hitbox(new Rectangle(8, 9, piranhaWidth - 8 * 2, piranhaHeight - 9), Vector2.Zero);

            direction = 1;
            distanceMoved = 0;
            timer = 0;
        }

        public void Update(float deltaTime)
        {
            if (timer == 0)
                distanceMoved += direction * speed * deltaTime;

            movement.position.Y = initialPosition.Y + distanceMoved;

            if (distanceMoved <= 0 || distanceMoved >= hitbox.rectangle.Height + 3)
                timer += deltaTime;

            if (timer >= showAndHideDuration)
            {
                timer = 0;
                direction *= -1;
            }

            movement.Update(deltaTime);
            animation.Update(deltaTime);
            hitbox.Update(movement.position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.sourceRect = animation.GetCurrentFrame();
            sprite.Draw(spriteBatch, movement.position);
        }
    }
}
