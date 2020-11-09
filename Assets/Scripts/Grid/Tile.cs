using System;
using UnityEngine;

namespace GridSystem
{
    public class Tile<T>
    {
        protected readonly Chunk<T> ownerChunk;
        protected readonly Grid<T>  ownerGrid;
        protected readonly Vector2Int coordinate;
        protected readonly Vector2Int localCoordinate;

        protected T element;
        protected Vector3 position;

        public float Size => ownerChunk.TileSize;
        public T Element => element;
        public Vector3 Position => position;
        public Vector2Int Coordinate => coordinate;
        public Vector2Int LocalCoordinate => localCoordinate;

        public Tile<T> WestNeighbor  { get; private set; } = null;
        public Tile<T> EastNeighbor  { get; private set; } = null;
        public Tile<T> NorthNeighbor { get; private set; } = null;
        public Tile<T> SouthNeighbor { get; private set; } = null;
        public Tile<T> NorthWestNeighbor { get; private set; } = null;
        public Tile<T> NorthEastNeighbor { get; private set; } = null;
        public Tile<T> SouthWestNeighbor { get; private set; } = null;
        public Tile<T> SouthEastNeighbor { get; private set; } = null;

        public Tile(Chunk<T> ownerChunk, Vector2Int coordinate, Vector2Int localCoordinate, Func<Tile<T>, T> instantiationFunc)
        {
            this.ownerChunk = ownerChunk;
            this.coordinate = coordinate;
            this.ownerGrid  = ownerChunk.OwnerGrid;
            this.element    = instantiationFunc(this);
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
        
        public void LinkNeighbors()
        {
            WestNeighbor  = ownerGrid.TryGetTileAtCoordinate(coordinate.x - 1, coordinate.y);
            EastNeighbor  = ownerGrid.TryGetTileAtCoordinate(coordinate.x + 1, coordinate.y);
            NorthNeighbor = ownerGrid.TryGetTileAtCoordinate(coordinate.x, coordinate.y + 1);
            SouthNeighbor = ownerGrid.TryGetTileAtCoordinate(coordinate.x, coordinate.y - 1);
            NorthWestNeighbor = ownerGrid.TryGetTileAtCoordinate(coordinate.x - 1, coordinate.y + 1);
            NorthEastNeighbor = ownerGrid.TryGetTileAtCoordinate(coordinate.x + 1, coordinate.y + 1);
            SouthWestNeighbor = ownerGrid.TryGetTileAtCoordinate(coordinate.x - 1, coordinate.y - 1);
            SouthEastNeighbor = ownerGrid.TryGetTileAtCoordinate(coordinate.x + 1, coordinate.y - 1);
        }
    }
}
