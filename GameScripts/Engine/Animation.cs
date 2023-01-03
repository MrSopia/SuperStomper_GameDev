using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperStomper.GameScripts.Engine
{
    internal class Animation
    {
        public bool looping;
        public bool finished { private set; get; }
        public int currentFrame { private set; get; }

        private Texture2D texture;
        private float timer;
        private float timeBetweenFrames;
        private int[] frames;
        private Point frameSize;

        public Animation(Texture2D texture, float timeBetweenFrames, bool looping, int[] frames, Point frameSize)
        {
            this.texture = texture;
            this.timeBetweenFrames = timeBetweenFrames;
            this.looping = looping;
            this.frames = frames;
            this.frameSize = frameSize;

            finished = false;
            currentFrame = 0;
        }

        public void Update(float deltaTime)
        {
            if (frames.Length <= 1)
                return;

            timer += deltaTime;
            int passedAFrame = (int)(timer / timeBetweenFrames);
            timer -= passedAFrame * timeBetweenFrames;
            currentFrame += passedAFrame;

            // animation is at the end
            // keep the animation alive if it should loop
            if (currentFrame >= frames.Length)
            {
                finished = !looping;
                currentFrame = looping ? 0 : currentFrame - 1;
            }

            if (finished)
                return;
        }

        public void Reset()
        {
            currentFrame = 0;
            finished = false;
            timer = 0;
        }

        public Rectangle GetCurrentFrame() => new Rectangle((frames[currentFrame] * frameSize.X) % texture.Width, ((frames[currentFrame] * frameSize.X) / texture.Width) * frameSize.Y, frameSize.X, frameSize.Y);
    }
}
