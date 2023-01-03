using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SuperStomper.GameScripts.Engine;

namespace SuperStomper.GameScripts.Game
{
    internal class Gomba
    {
        public int direction;
        public Hitbox hitbox;
        public Physics physics;
        public Movement movement;
        public bool stompedOn;
        public bool shouldBeRemoved;

        private float deadSince;
        private Sprite sprite;
        private Animation animation;

        public const int stompRepulsionForce = 150;

        private const int gombaWidth = 32;
        private const int gombaHeight = 32;
        private const int speed = 30;
        private const float vanishTimeAfterStompedOn = 0.6f;

        public Gomba(ContentManager content, Vector2 position)
        {
            direction = -1;
            stompedOn = false;
            shouldBeRemoved = false;
            deadSince = 0;

            movement = new Movement(position);
            physics = new Physics() { dragScale = 0, velocity = direction * speed * Vector2.UnitX};
            sprite = new Sprite(content.Load<Texture2D>(@"Spritesheets\Enemies\Enemies"), new Rectangle(0, 0, gombaWidth, gombaHeight), Vector2.Zero, position);
            animation = new Animation(sprite.texture, 0.25f, true, new int[] { 0, 1 }, sprite.sourceRect.Size);
            hitbox = new Hitbox(new Rectangle(7, 15, gombaWidth - 7 * 2, gombaHeight - 15), Vector2.Zero);
        }

        public void Update(float deltaTime)
        {
            movement.Update(deltaTime);
            physics.Update(deltaTime);
            animation.Update(deltaTime);
            hitbox.Update(movement.position);

            if (!stompedOn)
                physics.desiredVelocity.X = speed * direction;
            else
                deadSince += deltaTime;

            if (deadSince > vanishTimeAfterStompedOn)
                shouldBeRemoved = true;

            // effect of velocity on position 
            movement.deltaX += physics.desiredVelocity.X * deltaTime;
            movement.deltaY += physics.desiredVelocity.Y * deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.sourceRect = stompedOn? new Rectangle(2 * gombaWidth, 0, gombaWidth, gombaHeight) : animation.GetCurrentFrame();
            sprite.Draw(spriteBatch, movement.position);
        }
    }
}
