using Microsoft.Xna.Framework;

namespace SuperStomper.GameScripts.Engine
{
    internal class Movement
    {
        public Vector2 position;
        public float deltaX;
        public float deltaY;

        public Movement(Vector2 position)
        {
            this.position = position;
            deltaX = 0;
            deltaY = 0;
        }

        public void Update(float deltaTime)
        {
            position += new Vector2(deltaX, deltaY);
            deltaX = 0;
            deltaY = 0;
        }
    }
}
