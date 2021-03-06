﻿using System;
using System.Collections.Generic;
using System.Linq;
using MayBe;
using UnityEngine;

namespace GridSystem
{
    public abstract class BaseTile<T> : ITickable where T : BaseTile<T>
    {
        protected List<Maybe<T>> neighbors = new List<Maybe<T>>(8);

        protected readonly Chunk ownerChunk;
        protected readonly Grid<T> ownerGrid;
        protected readonly Vector3 position;
        protected readonly Vector2Int coordinate;
        protected readonly Vector2Int localCoordinate;
        
        private bool isActiveTickable; // cached state for optimization

        public Maybe<T> EastNeighbor  => neighbors[0];
        public Maybe<T> WestNeighbor  => neighbors[1];
        public Maybe<T> SouthNeighbor => neighbors[2];
        public Maybe<T> NorthNeighbor => neighbors[3];
        public Maybe<T> SouthEastNeighbor => neighbors[4];
        public Maybe<T> SouthWestNeighbor => neighbors[5];
        public Maybe<T> NorthEastNeighbor => neighbors[6];
        public Maybe<T> NorthWestNeighbor => neighbors[7];

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

        public Rect3D GetVertexRect() => ownerChunk.GetTileVertexRectAt(localCoordinate).ValueOr(Rect3D.Zero);
        
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

        public void ForEachCardinalNeighbor(Action<T> action)
        {
            NorthNeighbor.MatchSome(n => action(n));
            EastNeighbor.MatchSome(n => action(n));
            SouthNeighbor.MatchSome(n => action(n));
            WestNeighbor.MatchSome(n => action(n));
        }
        
        public void ForEachIntercardinalNeighbor(Action<T> action)
        {
            NorthEastNeighbor.MatchSome(n => action(n));
            SouthEastNeighbor.MatchSome(n => action(n));
            SouthWestNeighbor.MatchSome(n => action(n));
            NorthWestNeighbor.MatchSome(n => action(n));
        }
        
        public bool AnyNeighbor(Predicate<T> predicate)
        {
            return neighbors.FirstOrDefault(optional => optional.Filter(neighbor => predicate(neighbor)).HasValue) != null;
        }

        public Maybe<T> TraverseEast(int steps, Action<T> action)
        {
            int i = 0;

            var currentTile = EastNeighbor;

            while (currentTile.HasValue && i < steps)
            {
                currentTile.MatchSome(tile => action(tile));
                currentTile = currentTile.FlatMap(tile => tile.EastNeighbor);
                i++;
            }

            return currentTile.FlatMap(tile => tile.WestNeighbor);
        }
        
        public Maybe<T> TraverseWest(int steps, Action<T> action)
        {
            int i = 0;

            var currentTile = WestNeighbor;

            while (currentTile.HasValue && i < steps)
            {
                currentTile.MatchSome(tile => action(tile));
                currentTile = currentTile.FlatMap(tile => tile.WestNeighbor);
                i++;
            }
            
            return currentTile.FlatMap(tile => tile.EastNeighbor);;
        }
        
        public Maybe<T> TraverseNorth(int steps, Action<T> action)
        {
            int i = 0;

            var currentTile = NorthNeighbor;

            while (currentTile.HasValue && i < steps)
            {
                currentTile.MatchSome(tile => action(tile));
                currentTile = currentTile.FlatMap(tile => tile.NorthNeighbor);
                i++;
            }

            return currentTile.FlatMap(tile => tile.SouthNeighbor);;
        }
        
        public Maybe<T> TraverseSouth(int steps, Action<T> action)
        {
            int i = 0;

            var currentTile = SouthNeighbor;

            while (currentTile.HasValue && i < steps)
            {
                currentTile.MatchSome(tile => action(tile));
                currentTile = currentTile.FlatMap(tile => tile.SouthNeighbor);
                i++;
            }
            
            return currentTile.FlatMap(tile => tile.NorthNeighbor);;
        }
        
        public virtual bool SameTileCategory(T otherTile) => false;
        public virtual void Tick(long ticks) { }

        public abstract bool IsOccupied { get; }
        public abstract bool IsCollidable { get; }
    }
}