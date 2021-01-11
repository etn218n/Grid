using UnityEngine;

namespace GridSystem
{
    public readonly struct Rect3D
    {
        private readonly Vector3 bottomLeft;
        private readonly Vector3 bottomRight;
        private readonly Vector3 topLeft;
        private readonly Vector3 topRight;

        public Vector3 BottomLeft  => bottomLeft;
        public Vector3 BottomRight => bottomRight;
        public Vector3 TopLeft  => topLeft;
        public Vector3 TopRight => topRight;
        
        public static readonly Rect3D Zero = new Rect3D(); 

        public Rect3D(Vector3 bottomLeft, Vector3 topLeft, Vector3 bottomRight, Vector3 topRight)
        {
            this.bottomLeft  = bottomLeft;
            this.bottomRight = bottomRight;
            this.topLeft     = topLeft;
            this.topRight    = topRight;
        }
    }
}