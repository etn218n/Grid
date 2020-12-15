using System;
using UnityEngine;

namespace GridSystem
{
    public class BaseTile<T> where T : BaseTile<T>
    {
        protected int id;
        protected T[] neighbors = new T[8];
        
        protected readonly Chunk ownerChunk;
        protected readonly Grid<T> ownerGrid;
        protected readonly Vector3 position;
        protected readonly Vector2Int coordinate;
        protected readonly Vector2Int localCoordinate;
        
        public int ID => id;
        public T EastNeighbor  => neighbors[0];
        public T WestNeighbor  => neighbors[1];
        public T SouthNeighbor => neighbors[2];
        public T NorthNeighbor => neighbors[3];
        public T SouthEastNeighbor => neighbors[4];
        public T SouthWestNeighbor => neighbors[5];
        public T NorthEastNeighbor => neighbors[6];
        public T NorthWestNeighbor => neighbors[7];

        public Vector3 Position => position;
        public Vector2Int Coordinate => coordinate;
        public Vector2Int LocalCoordinate => localCoordinate;

        public BaseTile(Grid<T> ownerGrid, Chunk ownerChunk, Vector2Int coordinate, Vector2Int localCoordinate)
        {
            this.id = 0;
            this.ownerChunk = ownerChunk;
            this.coordinate = coordinate;
            this.ownerGrid  = ownerGrid;
            this.localCoordinate = localCoordinate;
            
            this.position = new Vector3(ownerGrid.Origin.x + (coordinate.x * ownerGrid.TileSize) + (ownerGrid.TileSize / 2), 
                                        ownerGrid.Origin.y + (coordinate.y * ownerGrid.TileSize) + (ownerGrid.TileSize / 2),
                                        0);
        }

        public void UpdateNeighbors()
        {
            neighbors[0] = ownerGrid.TryGetTileAtCoordinate(coordinate.x + 1, coordinate.y);
            neighbors[1] = ownerGrid.TryGetTileAtCoordinate(coordinate.x - 1, coordinate.y);
            neighbors[2] = ownerGrid.TryGetTileAtCoordinate(coordinate.x, coordinate.y - 1);
            neighbors[3] = ownerGrid.TryGetTileAtCoordinate(coordinate.x, coordinate.y + 1);
            neighbors[4] = ownerGrid.TryGetTileAtCoordinate(coordinate.x + 1, coordinate.y - 1);
            neighbors[5] = ownerGrid.TryGetTileAtCoordinate(coordinate.x - 1, coordinate.y - 1);
            neighbors[6] = ownerGrid.TryGetTileAtCoordinate(coordinate.x + 1, coordinate.y + 1);
            neighbors[7] = ownerGrid.TryGetTileAtCoordinate(coordinate.x - 1, coordinate.y + 1);
        }

        public bool HasEastNeighbor  => neighbors[0] != null;
        public bool HasWestNeighbor  => neighbors[1] != null;
        public bool HasSouthNeighbor => neighbors[2] != null;
        public bool HasNorthNeighbor => neighbors[3] != null;
        public bool HasSouthEastNeighbor => neighbors[4] != null;
        public bool HasSouthWestNeighbor => neighbors[5] != null;
        public bool HasNorthEastNeighbor => neighbors[6] != null;
        public bool HasNorthWestNeighbor => neighbors[7] != null;
        
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

        public Rect3D GetVertexRect() => ownerChunk.GetTileRect(localCoordinate);
        
        public void SetUVs(in Rect2D rect2D) => ownerChunk.SetTileUVs(localCoordinate, in rect2D);
        public void SetVertices(in Rect3D rect3D) => ownerChunk.SetTileVertices(localCoordinate, in rect3D);
        public void SetColor(Color color) => ownerChunk.SetTileColor(localCoordinate, color);

        public void ForEachNeighbor(Action<T> action)
        {
            foreach (var neighbor in neighbors)
            {
                if (neighbor != null)
                    action(neighbor);
            }
        }
        
        public virtual bool ShareAttributeWith(T otherTile) => this.id == otherTile.id;
    }
}