using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

namespace GridSystem
{
    public class Grid<T> : IDisposable
    {
        protected int rows;
        protected int columns;
        protected int rowsPerChunk;
        protected int columnsPerChunk;
        protected int verticalChunks;
        protected int horizontalChunks;
        protected float width;
        protected float height;
        protected float tileSize;
        protected Vector3 origin;
        protected Tile<T>[,] tiles;
        protected Chunk<T>[,] chunks;
        protected Queue<Chunk<T>> modifiedChunks;
        
        public event Action<Vector3> OriginChanged;
        public event Action InitializationFinished;

        public int Rows => rows;
        public int Columns => columns;
        public int RowsPerChunk => rowsPerChunk;
        public int ColumnsPerChunk => columnsPerChunk;
        public int VerticalChunks => verticalChunks;
        public int HorizontalChunks => horizontalChunks;
        public float Width  => width;
        public float Height => height;
        public float TileSize => tileSize;
        public Vector3 Origin
        {
            get => origin;
            set
            {
                origin = value;
                OnOriginChanged(value);
            }
        }
        public Tile<T>[,] Tiles => tiles;
        public Chunk<T>[,] Chunks => chunks;
        public Queue<Chunk<T>> ModifiedChunks => modifiedChunks;

        private void OnOriginChanged(Vector3 newOrigin)
        {
            OriginChanged?.Invoke(newOrigin);
        }

        public Grid(Vector3 origin, int horizontalChunks, int verticalChunks, int rowsPerChunk, int columnsPerChunk, float tileSize, Func<Tile<T>, T> instantiationFunc)
        {
            this.origin   = origin;
            this.tileSize = tileSize;
            this.rows     = horizontalChunks * rowsPerChunk;
            this.columns  = verticalChunks * columnsPerChunk;
            this.rowsPerChunk     = rowsPerChunk;
            this.columnsPerChunk  = columnsPerChunk;
            this.horizontalChunks = horizontalChunks;
            this.verticalChunks   = verticalChunks;

            this.width  = columns * tileSize;
            this.height = rows * tileSize;
            
            modifiedChunks = new Queue<Chunk<T>>();
            
            InitializeChunks(instantiationFunc);
            LinkTileNeighbors();
            GenerateChunksMesh();
            
            InitializationFinished?.Invoke();
        }

        private void InitializeChunks(Func<Tile<T>, T> instantiationFunc)
        {
            chunks = new Chunk<T>[verticalChunks, horizontalChunks];
            tiles  = new Tile<T>[columns, rows];

            for (int i = 0; i < verticalChunks; i++)
            {
                for (int j = 0; j < horizontalChunks; j++)
                {
                    Vector3 chunkOrigin = new Vector3(origin.x + (i * tileSize * columnsPerChunk), 
                                                      origin.y + (j * tileSize * rowsPerChunk), 
                                                      0);

                    chunks[i, j] = new Chunk<T>(this, chunkOrigin, rowsPerChunk, columnsPerChunk, tileSize);

                    for (int c = 0; c < columnsPerChunk; c++)
                    {
                        for (int r = 0; r < rowsPerChunk; r++)
                        {
                            int columnIndex = c + i * columnsPerChunk;
                            int rowIndex    = r + j * rowsPerChunk;

                            Vector2Int coordinate      = new Vector2Int(columnIndex, rowIndex);
                            Vector2Int localCoordinate = new Vector2Int(c, r);
                            
                            tiles[columnIndex, rowIndex] = new Tile<T>(chunks[i, j], coordinate, localCoordinate, instantiationFunc);
                        }
                    }
                }
            }
        }

        private void GenerateChunksMesh()
        {
            NativeList<JobHandle> handles = new NativeList<JobHandle>(chunks.Length, Allocator.Temp);

            for (int i = 0; i < verticalChunks; i++)
                for (int j = 0; j < horizontalChunks; j++)
                    handles.Add(chunks[i, j].ChunkMeshJobHandle());

            JobHandle.CompleteAll(handles);
            
            for (int i = 0; i < verticalChunks; i++) 
                for (int j = 0; j < horizontalChunks; j++)
                    chunks[i, j].UpdateMesh();

            handles.Dispose();
        }

        private void LinkTileNeighbors()
        {
            for (int i = 0; i < columns; i++) 
                for (int j = 0; j < rows; j++)
                    tiles[i, j].LinkNeighbors();
        }
        
        public Tile<T> TryGetTileAtPosition(Vector3 worldPosition)
        {
            return TryGetTileAtPosition(worldPosition.x, worldPosition.y);
        }
        
        public Tile<T> TryGetTileAtPosition(float worldPositionX, float worldPositionY)
        {
            if (worldPositionX < origin.x || worldPositionX > width)
                return default(Tile<T>);

            if (worldPositionY < origin.y || worldPositionY > height)
                return default(Tile<T>);

            float fractX = Mathf.Abs(worldPositionX - origin.x) / tileSize;
            float fractY = Mathf.Abs(worldPositionY - origin.y) / tileSize;

            int i = fractX == 0 ? 0 : Mathf.CeilToInt(fractX) - 1;
            int j = fractY == 0 ? 0 : Mathf.CeilToInt(fractY) - 1;

            return tiles[i, j];
        }
        
        public Tile<T> TryGetTileAtCoordinate(Vector2Int coordinate)
        {
            return TryGetTileAtCoordinate(new Vector2Int(coordinate.x, coordinate.y));
        }
        
        public Tile<T> TryGetTileAtCoordinate(int coordinateX, int coordinateY)
        {
            if (coordinateX < 0 || coordinateX >= columns)
                return default(Tile<T>);
            
            if (coordinateY < 0 || coordinateY >= rows)
                return default(Tile<T>);

            return tiles[coordinateX, coordinateY];
        }

        public void Update()
        {
            while (modifiedChunks.Any())
                modifiedChunks.Dequeue().UpdateMesh();
        }

        public void Dispose()
        {
            for (int i = 0; i < verticalChunks; i++)
                for (int j = 0; j < horizontalChunks; j++)
                    chunks[i, j].Dispose();
        }
    }
}
