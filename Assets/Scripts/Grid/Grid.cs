using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

namespace GridSystem
{
    public class Grid : IDisposable
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
        protected Chunk[,] chunks;
        protected Queue<Chunk> modifiedChunks;
        
        public int Rows => rows;
        public int Columns => columns;
        public int RowsPerChunk => rowsPerChunk;
        public int ColumnsPerChunk => columnsPerChunk;
        public int VerticalChunks => verticalChunks;
        public int HorizontalChunks => horizontalChunks;
        public float Width => width;
        public float Height => height;
        public float TileSize => tileSize;
        public Vector3 Origin => origin;
        public Chunk[,] Chunks => chunks;
        public Queue<Chunk> ModifiedChunks => modifiedChunks;

        public Grid(Vector3 origin, int horizontalChunks, int verticalChunks, int rowsPerChunk, int columnsPerChunk, float tileSize)
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
            
            modifiedChunks = new Queue<Chunk>();
            
            InitializeChunks();
            GenerateChunksMesh();
        }
        
        private void InitializeChunks()
        {
            chunks = new Chunk[verticalChunks, horizontalChunks];
            
            for (int i = 0; i < verticalChunks; i++)
            {
                for (int j = 0; j < horizontalChunks; j++)
                {
                    Vector3 chunkOrigin = new Vector3(origin.x + (i * tileSize * columnsPerChunk), 
                                                      origin.y + (j * tileSize * rowsPerChunk), 
                                                      0);

                    chunks[i, j] = new Chunk(this, chunkOrigin, rowsPerChunk, columnsPerChunk, tileSize);
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
    
    public class Grid<T> : Grid where T : BaseTile<T>
    {
        protected T[,] tiles;
        public T[,] Tiles => tiles;

        public Grid(Vector3 origin, int horizontalChunks, int verticalChunks, int rowsPerChunk, int columnsPerChunk, float tileSize, Func<Grid<T>, Chunk, Vector2Int, Vector2Int, T> instantiationFunc)
        : base(origin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize)
        {
            InitializeTiles(instantiationFunc);
            UpdateTileNeighbors();
        }

        private void UpdateTileNeighbors()
        {
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    tiles[i, j].UpdateNeighbors();
        }

        private void InitializeTiles(Func<Grid<T>, Chunk, Vector2Int, Vector2Int, T> instantiationFunc)
        {
            tiles = new T[columns, rows];
            
            for (int i = 0; i < verticalChunks; i++)
            {
                for (int j = 0; j < horizontalChunks; j++)
                {
                    for (int c = 0; c < columnsPerChunk; c++)
                    {
                        for (int r = 0; r < rowsPerChunk; r++)
                        {
                            int columnIndex = c + i * columnsPerChunk;
                            int rowIndex    = r + j * rowsPerChunk;

                            Vector2Int coordinate      = new Vector2Int(columnIndex, rowIndex);
                            Vector2Int localCoordinate = new Vector2Int(c, r);

                            tiles[columnIndex, rowIndex] = instantiationFunc(this, chunks[i, j], coordinate, localCoordinate);
                        }
                    }
                }
            }
        }

        public T TryGetTileAtPosition(Vector3 worldPosition)
        {
            return TryGetTileAtPosition(worldPosition.x, worldPosition.y);
        }
        
        public T TryGetTileAtPosition(float worldPositionX, float worldPositionY)
        {
            if (worldPositionX < origin.x || worldPositionX > width)
                return default(T);

            if (worldPositionY < origin.y || worldPositionY > height)
                return default(T);

            float fractX = Mathf.Abs(worldPositionX - origin.x) / tileSize;
            float fractY = Mathf.Abs(worldPositionY - origin.y) / tileSize;

            int i = fractX == 0 ? 0 : Mathf.CeilToInt(fractX) - 1;
            int j = fractY == 0 ? 0 : Mathf.CeilToInt(fractY) - 1;

            return tiles[i, j];
        }
        
        public T TryGetTileAtCoordinate(Vector2Int coordinate)
        {
            return TryGetTileAtCoordinate(coordinate.x, coordinate.y);
        }
        
        public T TryGetTileAtCoordinate(int coordinateX, int coordinateY)
        {
            if (coordinateX < 0 || coordinateX >= columns)
                return default(T);
            
            if (coordinateY < 0 || coordinateY >= rows)
                return default(T);

            return tiles[coordinateX, coordinateY];
        }

        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    action(tiles[i, j]);
        }
    }
}
