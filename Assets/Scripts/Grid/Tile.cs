using UnityEngine;

namespace GridSystem
{
    public class Tile
    {
        protected readonly Chunk ownerChunk;
        protected readonly Grid  ownerGrid;
        protected readonly Vector3 position;
        protected readonly Vector2Int coordinate;
        protected readonly Vector2Int localCoordinate;
        
        public Vector3 Position => position;
        public Vector2Int Coordinate => coordinate;
        public Vector2Int LocalCoordinate => localCoordinate;

        public Tile(Chunk ownerChunk, Vector2Int coordinate, Vector2Int localCoordinate)
        {
            this.ownerChunk = ownerChunk;
            this.coordinate = coordinate;
            this.ownerGrid  = ownerChunk.OwnerGrid;
            this.localCoordinate = localCoordinate;
            
            this.position = new Vector3(ownerGrid.Origin.x + (coordinate.x * ownerGrid.TileSize) + (ownerGrid.TileSize / 2), 
                                        ownerGrid.Origin.y + (coordinate.y * ownerGrid.TileSize) + (ownerGrid.TileSize / 2),
                                        0);
        }

        public bool IsBottomLeftCorner  => (coordinate.x == 0) && (coordinate.y == 0);
        public bool IsBottomRightCorner => (coordinate.x == ownerGrid.Columns - 1) && (coordinate.y == 0);
        public bool IsTopLeftCorner     => (coordinate.x == 0) && (coordinate.y == ownerGrid.Rows - 1);
        public bool IsTopRightCorner    => (coordinate.x == ownerGrid.Columns - 1) && (coordinate.y == ownerGrid.Rows - 1);
        
        public bool IsRightEdge  => (coordinate.x == ownerGrid.Columns - 1) && (coordinate.y != 0) && (coordinate.y != ownerGrid.Rows - 1);
        public bool IsBottomEdge => (coordinate.y == 0) && (coordinate.x != 0) && (coordinate.x != ownerGrid.Columns - 1);
        public bool IsTopEdge    => (coordinate.y == ownerGrid.Rows - 1) && (coordinate.x != 0) && (coordinate.x != ownerGrid.Columns - 1);
        public bool IsLeftEdge   => (coordinate.x == 0) && (coordinate.y != 0) && (coordinate.y != ownerGrid.Rows - 1);
        
        public bool IsEdge   => IsLeftEdge || IsRightEdge || IsTopEdge || IsBottomEdge;
        public bool IsCorner => IsBottomLeftCorner || IsBottomRightCorner || IsTopLeftCorner || IsTopRightCorner;

        public void SetUVs(ref Vector2[] uvs) => ownerChunk.SetTileUVs(this.localCoordinate, ref uvs);
    }
}
