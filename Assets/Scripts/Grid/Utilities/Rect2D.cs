using UnityEngine;

namespace GridSystem
{
    public readonly struct Rect2D
    {
        private readonly Vector2 bottomLeft;
        private readonly Vector2 bottomRight;
        private readonly Vector2 topLeft;
        private readonly Vector2 topRight;

        public Vector2 BottomLeft  => bottomLeft;
        public Vector2 BottomRight => bottomRight;
        public Vector2 TopLeft  => topLeft;
        public Vector2 TopRight => topRight;
        
        public static readonly Rect2D Zero = new Rect2D(); 

        public Rect2D(Vector2 bottomLeft, Vector2 topLeft, Vector2 bottomRight, Vector2 topRight)
        {
            this.bottomLeft  = bottomLeft;
            this.bottomRight = bottomRight;
            this.topLeft     = topLeft;
            this.topRight    = topRight;
        }
    }
}