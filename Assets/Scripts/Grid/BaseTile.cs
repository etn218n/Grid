using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

namespace GridSystem
{
    public abstract class BaseTile<T> : ITickable where T : BaseTile<T>
    {
        protected List<Option<T>> neighbors = new List<Option<T>>(8);

        protected readonly Chunk ownerChunk;
        protected readonly Grid<T> ownerGrid;
        protected readonly Vector3 position;
        protected readonly Vector2Int coordinate;
        protected readonly Vector2Int localCoordinate;
        
        private bool isActiveTickable; // cached state for optimization

        public Option<T> EastNeighbor  => neighbors[0];
        public Option<T> WestNeighbor  => neighbors[1];
        public Option<T> SouthNeighbor => neighbors[2];
        public Option<T> NorthNeighbor => neighbors[3];
        public Option<T> SouthEastNeighbor => neighbors[4];
        public Option<T> SouthWestNeighbor => neighbors[5];
        public Option<T> NorthEastNeighbor => neighbors[6];
        public Option<T> NorthWestNeighbor => neighbors[7];

        public Vector3 Position => position;
        public Vector2Int Coordinate => coordinate;
        public Vector2Int LocalCoordinate => localCoordinate;
        public bool IsActiveTickable => isActiveTickable;

        protected BaseTile(Grid<T> ownerGrid, Vector2Int coordinate)
        {
            this.ownerGrid  = ownerGrid;
            this.coordinate = coordinate;

            int rowIndex    = coordinate.y / ownerGrid.RowsPerChunk;
            int columnIndex = coordinate.x / ownerGrid.ColumnsPerChunk;

            this.ownerChunk = ownerGrid.Chunks[columnIndex, rowIndex];

            this.localCoordinate = new Vector2Int(coordinate.x % ownerGrid.ColumnsPerChunk, 
                                                  coordinate.y % ownerGrid.RowsPerChunk);
            
            this.position = new Vector3(ownerGrid.Origin.x + (coordinate.x * ownerGrid.TileSize) + (ownerGrid.TileSize / 2), 
                                        ownerGrid.Origin.y + (coordinate.y * ownerGrid.TileSize) + (ownerGrid.TileSize / 2),
                                        0);
        }

        public void UpdateNeighbors()
        {
            neighbors.Add(ownerGrid.GetTileAt(coordinate.x + 1, coordinate.y));
            neighbors.Add(ownerGrid.GetTileAt(coordinate.x - 1, coordinate.y));
            neighbors.Add(ownerGrid.GetTileAt(coordinate.x, coordinate.y - 1));
            neighbors.Add(ownerGrid.GetTileAt(coordinate.x, coordinate.y + 1));
            neighbors.Add(ownerGrid.GetTileAt(coordinate.x + 1, coordinate.y - 1));
            neighbors.Add(ownerGrid.GetTileAt(coordinate.x - 1, coordinate.y - 1));
            neighbors.Add(ownerGrid.GetTileAt(coordinate.x + 1, coordinate.y + 1));
            neighbors.Add(ownerGrid.GetTileAt(coordinate.x - 1, coordinate.y + 1));
        }

        public bool HasEastNeighbor  => neighbors[0].HasValue;
        public bool HasWestNeighbor  => neighbors[1].HasValue;
        public bool HasSouthNeighbor => neighbors[2].HasValue;
        public bool HasNorthNeighbor => neighbors[3].HasValue;
        public bool HasSouthEastNeighbor => neighbors[4].HasValue;
        public bool HasSouthWestNeighbor => neighbors[5].HasValue;
        public bool HasNorthEastNeighbor => neighbors[6].HasValue;
        public bool HasNorthWestNeighbor => neighbors[7].HasValue;
        
        public bool IsBottomLeftCorner  => (coordinate.x == 0) && (coordinate.y == 0);
        public bool IsTopLeftCorner     => (coordinate.x == 0) && (coordinate.y == ownerGrid.Rows - 1);
        public bool IsBottomRightCorner => (coordinate.x == ownerGrid.Columns - 1) && (coordinate.y == 0);
        public bool IsTopRightCorner    => (coordinate.x == ownerGrid.Columns - 1) && (coordinate.y == ownerGrid.Rows - 1);

        public bool IsBottomEdge => (coordinate.y == 0);
        public bool IsLeftEdge   => (coordinate.x == 0);
        public bool IsTopEdge    => (coordinate.y == ownerGrid.Rows - 1);
        public bool IsRightEdge  => (coordinate.x == ownerGrid.Columns - 1);

        public bool IsEdge   => IsLeftEdge || IsRightEdge || IsTopEdge || IsBottomEdge;
        public bool IsCorner => IsBottomLeftCorner || IsBottomRightCorner || IsTopLeftCorner || IsTopRightCorner;

        public Option<Rect3D> GetVertexRect() => ownerChunk.GetTileVertexRectAt(localCoordinate);
        
        public void SetUVs(in Rect2D uvRect) => ownerChunk.SetTileUVsAt(localCoordinate, in uvRect);
        public void SetColor(Color color) => ownerChunk.SetTileColorAt(localCoordinate, color);
        public void SetVertices(in Rect3D rect3D) => ownerChunk.SetTileVerticesAt(localCoordinate, in rect3D);

        public void MarkActive()
        {
            if (isActiveTickable)
                return;
            
            ownerChunk.ActiveTickables.Add(this);
        }
        
        public void MarkInactive()
        {
            if (!isActiveTickable)
                return;
            
            ownerChunk.ActiveTickables.Remove(this);
        }
        
        public void ForEachNeighbor(Action<T> action)
        {
            foreach (var neighbor in neighbors)
                neighbor.MatchSome(n => action(n));
        }
        
        public bool AnyNeighbor(Predicate<T> predicate)
        {
            return neighbors.FirstOrDefault(optional => optional.Filter(neighbor => predicate(neighbor)).HasValue) != null;
        }
        
        public virtual bool SameTileCategory(T otherTile) => false;
        public virtual void Tick(long ticks) { }

        public abstract bool IsOccupied { get; }
        public abstract bool IsCollidable { get; }
    }
}