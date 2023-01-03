using Microsoft.Xna.Framework;

namespace SuperStomper.GameScripts.Engine
{
    internal class Physics
    {
        public Vector2 velocity;
        public Vector2 desiredVelocity;
        public bool affectedByGravity;
        public float risingGravityScale; // gravity scale when entity is rising
        public float fallingGravityScale; // gravity scale when entity is falling
        public float dragScale; // drag scale when entity is moving horizontally

        private const float gravityConstant = 10;

        public Physics()
        {
            velocity = Vector2.Zero;
            desiredVelocity = Vector2.Zero;
            affectedByGravity = true;
            risingGravityScale = 1;
            fallingGravityScale = 1;
            dragScale = 1;
        }

        public void Update(float deltaTime)
        {
            desiredVelocity.X -= desiredVelocity.X * dragScale;
            desiredVelocity.X = desiredVelocity.X >= 0 ? (float)System.Math.Floor(desiredVelocity.X) : (float)System.Math.Ceiling(desiredVelocity.X);
            desiredVelocity.Y += gravityConstant * (desiredVelocity.Y >= 0 ? fallingGravityScale : risingGravityScale);
        }

        // Steps:
        // 1- if we moved, will there be a collsion?
        // 2- if there is a collision, then we shoot 4 rays
        // 3- two in the X direction (from top and bottom of player myHitbox)
        // 4- other two in the Y direction (from left and right of player myHitbox)
        // 5- if we hit something in X or Y direction, we only move the adequate
        // distance so that we stop at the collider
        public int CollisionHandling(Movement movement, Hitbox myHitbox, Hitbox otherHitbox, bool justDetection = false)
        {
            // there is a collision
            int collisionType = 0; // 0: no collision, 1: horizontal, 2: vertical, 3: horizontal & vertical
            float originalDeltaX = movement.deltaX;
            float originalDeltaY = movement.deltaY;
            Rectangle originalRect = myHitbox.rectangle;
            myHitbox.rectangle.Offset(movement.deltaX < 0 ? (int)System.Math.Floor(movement.deltaX) : (int)System.Math.Ceiling(movement.deltaX), movement.deltaY < 0 ? (int)System.Math.Floor(movement.deltaY) : (int)System.Math.Ceiling(movement.deltaY));
            Rectangle offsetedRectangle = myHitbox.rectangle;

            if (myHitbox.IsTouching(otherHitbox))
            {
                myHitbox.rectangle = originalRect;

                Ray ray1 = new Ray(new Vector3(new Vector2(movement.deltaX > 0 ? myHitbox.rectangle.Right : myHitbox.rectangle.Left, myHitbox.rectangle.Top), 0), new Vector3(movement.deltaX, 0, 0));
                Ray ray2 = new Ray(new Vector3(new Vector2(movement.deltaX > 0 ? myHitbox.rectangle.Right : myHitbox.rectangle.Left, myHitbox.rectangle.Bottom), 0), new Vector3(movement.deltaX, 0, 0));

                Ray ray3 = new Ray(new Vector3(new Vector2(myHitbox.rectangle.Left, movement.deltaY > 0 ? myHitbox.rectangle.Bottom : myHitbox.rectangle.Top), 0), new Vector3(0, movement.deltaY, 0));
                Ray ray4 = new Ray(new Vector3(new Vector2(myHitbox.rectangle.Right, movement.deltaY > 0 ? myHitbox.rectangle.Bottom : myHitbox.rectangle.Top), 0), new Vector3(0, movement.deltaY, 0));

                BoundingBox boundingBoxV = movement.deltaX > 0 ? GetLeftBoundingBox(otherHitbox.rectangle) : GetRightBoundingBox(otherHitbox.rectangle);
                BoundingBox boundingBoxH = movement.deltaY > 0 ? GetTopBoundingBox(otherHitbox.rectangle) : GetBottomBoundingBox(otherHitbox.rectangle);

                float? vert1 = ray1.Intersects(boundingBoxV);
                float? vert2 = ray2.Intersects(boundingBoxV);

                float? horiz1 = ray3.Intersects(boundingBoxH);
                float? horiz2 = ray4.Intersects(boundingBoxH);

                // collided from left or right first
                if ((vert1 != null || vert2 != null) && movement.deltaX != 0)
                {
                    collisionType += 1;

                    if (!justDetection)
                    {
                        movement.deltaX = movement.deltaX > 0 ? otherHitbox.rectangle.Left - myHitbox.rectangle.Right : otherHitbox.rectangle.Right - myHitbox.rectangle.Left;

                        movement.position = new Vector2((int)(movement.position.X), movement.position.Y);
                        desiredVelocity.X = 0;
                    }
                }
                if ((horiz1 != null || horiz2 != null) && movement.deltaY != 0)
                {
                    collisionType += 2;

                    if (!justDetection)
                    {
                        movement.deltaY = movement.deltaY > 0 ? otherHitbox.rectangle.Top - myHitbox.rectangle.Bottom : otherHitbox.rectangle.Bottom - myHitbox.rectangle.Top;

                        movement.position = new Vector2(movement.position.X, (int)movement.position.Y);

                        if (System.Math.Sign(originalDeltaY) == System.Math.Sign(desiredVelocity.Y))
                            desiredVelocity.Y = 0;
                    }
                }
            }
            else
                myHitbox.rectangle = originalRect;

            myHitbox.Update(movement.position);

            return collisionType;
        }

        private BoundingBox GetLeftBoundingBox(Rectangle rect) => new BoundingBox(new Vector3(rect.Left, rect.Top, 0), new Vector3(rect.Left, rect.Bottom, 0));
        private BoundingBox GetRightBoundingBox(Rectangle rect) => new BoundingBox(new Vector3(rect.Right, rect.Top, 0), new Vector3(rect.Right, rect.Bottom, 0));
        private BoundingBox GetTopBoundingBox(Rectangle rect) => new BoundingBox(new Vector3(rect.Left, rect.Top, 0), new Vector3(rect.Right, rect.Top, 0));
        private BoundingBox GetBottomBoundingBox(Rectangle rect) => new BoundingBox(new Vector3(rect.Left, rect.Bottom, 0), new Vector3(rect.Right, rect.Bottom, 0));
    }
}
