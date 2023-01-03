using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperStomper.GameScripts.Engine;

namespace SuperStomper.GameScripts.Game
{
    internal class Mario
    {
        public bool canJump;
        public bool immune;
        public Hitbox hitbox;
        public Physics physics;
        public Movement movement;
        public int lives;
        public bool gameOver;

        private float immunityTimer;
        private float flickerTimer;
        private float deathTimer;

        private Animation animation;
        private Sprite sprite;
        private Sprite heartSprite;

        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation deathAnimation;

        private Vector2 prevVelocity;

        private const int marioWidth = 32;
        private const int marioHeight = 32;
        private const int heartWidth = 16;
        private const int heartHeight = 16;
        private const int speed = 12;
        private const int deathPushUpForce = 300;
        private const int jumpForce = 170;
        private const int maxSpeed = 150;
        private const int startingLives = 3;
        private const int immunityDuration = 2; // 2 seconds
        private const int flickerRate = 2;
        private const int dieAfter = 2;

        public Mario(ContentManager content, Vector2 position)
        {
            gameOver = false;
            canJump = false;
            lives = startingLives;
            immune = false;
            immunityTimer = 0;
            flickerTimer = 0;
            deathTimer = 0;

            heartSprite = new Sprite(content.Load<Texture2D>(@"Spritesheets\Mario\heart"), new Rectangle(0, 0, heartWidth, heartHeight), Vector2.Zero, new Vector2(3, 3));

            movement = new Movement(position);
            sprite = new Sprite(content.Load<Texture2D>(@"Spritesheets\Mario\Mario"), new Rectangle(0, 0, marioWidth, marioHeight), Vector2.Zero, position);
            hitbox = new Hitbox(new Rectangle(9, 15, marioWidth - 9 * 2, marioHeight - 15), Vector2.Zero);
            physics = new Physics() { affectedByGravity = true, dragScale = 0.05f, fallingGravityScale = 1.5f};

            idleAnimation = new Animation(sprite.texture, 0.1f, false, new int[] { 0 }, sprite.sourceRect.Size);
            runAnimation = new Animation(sprite.texture, 0.1f, true, new int[] { 1, 2, 3, 4 }, sprite.sourceRect.Size);
            jumpAnimation = new Animation(sprite.texture, 0.1f, false, new int[] { 5 }, sprite.sourceRect.Size);
            deathAnimation = new Animation(sprite.texture, 0.1f, false, new int[] { 6 }, sprite.sourceRect.Size);

            animation = idleAnimation;
            prevVelocity = physics.velocity;
        }

        public void DamageMario()
        {
            if (immune)
                return;

            lives = System.Math.Clamp(lives - 1, 0, lives);
            immune = true;
        }

        public void Update(float deltaTime)
        {
            if (lives <= 0)
            {
                deathTimer += deltaTime;

                if (deathTimer >= dieAfter)
                    gameOver = true;
            }

            if (immune)
                immunityTimer += deltaTime;

            if (immunityTimer > immunityDuration)
            {
                immune = false;
                immunityTimer = 0;
            }

            // Check if mario fell to a pit
            if (movement.position.Y >= Main.designedResolutionHeight)
                lives = 0;

            // We have to update movement and physics here in order to avoid
            // moving inside an object
            movement.Update(deltaTime);
            physics.Update(deltaTime);

            // Update Mario's components
            animation.Update(deltaTime);
            hitbox.Update(movement.position);

            // Mario is dead :(
            if (lives == 0)
            {
                // Apply force once
                if (animation != deathAnimation)
                {
                    animation = deathAnimation;
                    physics.desiredVelocity = -Vector2.UnitY * deathPushUpForce;
                }

                movement.deltaY += physics.desiredVelocity.Y * deltaTime;

                return;
            }

            // Mario's movement
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D)) // Move right
            {
                physics.desiredVelocity.X += speed;

                sprite.spriteEffects = SpriteEffects.None;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A)) // Move left
            {
                physics.desiredVelocity.X -= speed;

                sprite.spriteEffects = SpriteEffects.FlipHorizontally;
            }

            // Mario's jump
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (canJump)
                {
                    animation = jumpAnimation;
                    physics.desiredVelocity.Y -= jumpForce;
                    canJump = false;
                }
            }

            physics.desiredVelocity.X = System.Math.Clamp(physics.desiredVelocity.X, -maxSpeed, maxSpeed);

            // effect of velocity on position 
            movement.deltaX += physics.desiredVelocity.X * deltaTime;
            movement.deltaY += physics.desiredVelocity.Y * deltaTime;

            // If Mario is jumping or falling, shift to jump animation
            if (physics.velocity.Y != 0 || prevVelocity.Y != physics.velocity.Y)
            {
                animation = jumpAnimation;
                canJump = false;
            }
            else if (physics.velocity == Vector2.Zero) // Mario is static
            {
                animation = idleAnimation;
                canJump = true;
            }
            else
            {
                animation = runAnimation;
                canJump = true;
            }

            prevVelocity = physics.velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (flickerTimer == flickerRate)
            {
                sprite.sourceRect =  animation.GetCurrentFrame();
                sprite.Draw(spriteBatch, movement.position);
                flickerTimer = 0;
            }

            if (immunityTimer == 0)
                flickerTimer = flickerRate;
            else
                flickerTimer++;

            for (int i = 0; i < lives; i++)
                heartSprite.Draw(spriteBatch, heartSprite.position - GameManager.cameraPosition + (heartWidth + 3) * Vector2.UnitX * i);
        }
    }
}
