using System;
using System.Linq;
using System.Collections.Generic;
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
        
        protected readonly List<Chunk> activeChunks  = new List<Chunk>();
        protected readonly List<Chunk> visibleChunks = new List<Chunk>();
        protected readonly Queue<Chunk> modifiedChunks = new Queue<Chunk>();

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
        public List<Chunk> ActiveChunks => activeChunks;
        public List<Chunk> VisibleChunks => visibleChunks;
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

            InitializeChunks();
            GenerateChunksMesh();
            MarkAllChunksActive();
            MarkAllChunksVisible();
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

        public void MarkAllChunksActive()
        {
            ForEachChunk(chunk => chunk.MarkActive());
        }
        
        public void MarkAllChunksVisible()
        {
            ForEachChunk(chunk => chunk.MarkVisible());
        }
        
        public void SetUVs(in Rect2D uvRect)
        {
            for (int i = 0; i < verticalChunks; i++) 
                for (int j = 0; j < horizontalChunks; j++)
                    chunks[i, j].SetUVs(in uvRect);
        }

        public void SetTileUVsAt(Vector2Int tileCoordinate, in Rect2D uvRect)
        {
            int rowIndex    = tileCoordinate.y / rowsPerChunk;
            int columnIndex = tileCoordinate.x / columnsPerChunk;

            Vector2Int tileLocalCoordinate = new Vector2Int(tileCoordinate.x % columnsPerChunk, 
                                                            tileCoordinate.y % rowsPerChunk);
            
            chunks[columnIndex, rowIndex].SetTileUVsAt(tileLocalCoordinate, in uvRect);
        }

        public void SetTileVerticesAt(Vector2Int tileCoordinate, in Rect3D vertexRect)
        {
            int rowIndex    = tileCoordinate.y / rowsPerChunk;
            int columnIndex = tileCoordinate.x / columnsPerChunk;

            Vector2Int tileLocalCoordinate = new Vector2Int(tileCoordinate.x % columnsPerChunk, 
                                                            tileCoordinate.y % rowsPerChunk);
            
            chunks[columnIndex, rowIndex].SetTileVerticesAt(tileLocalCoordinate, in vertexRect);
        }

        public Rect3D GetTileVertexRectAt(Vector2Int tileCoordinate)
        {
            int rowIndex    = tileCoordinate.y / rowsPerChunk;
            int columnIndex = tileCoordinate.x / columnsPerChunk;

            Vector2Int tileLocalCoordinate = new Vector2Int(tileCoordinate.x % columnsPerChunk, 
                                                            tileCoordinate.y % rowsPerChunk);

            return chunks[columnIndex, rowIndex].GetTileVertexRectAt(tileLocalCoordinate);
        }

        public Chunk TryGetChunkAt(Vector3 worldPosition)
        {
            return TryGetChunkAt(worldPosition.x, worldPosition.y);
        }
        
        public Chunk TryGetChunkAt(float worldPositionX, float worldPositionY)
        {
            if (worldPositionX < origin.x || worldPositionX > width)
                return null;

            if (worldPositionY < origin.y || worldPositionY > height)
                return null;

            float fractX = Mathf.Abs(worldPositionX - origin.x) / tileSize;
            float fractY = Mathf.Abs(worldPositionY - origin.y) / tileSize;

            int i = fractX == 0 ? 0 : Mathf.CeilToInt(fractX) - 1;
            int j = fractY == 0 ? 0 : Mathf.CeilToInt(fractY) - 1;

            return chunks[i / columnsPerChunk, j / rowsPerChunk];
        }

        public void UpdateMesh()
        {
            while (modifiedChunks.Any())
                modifiedChunks.Dequeue().UpdateMesh();
        }

        public void Draw(Material material)
        {
            if (material == null)
                return;
            
            visibleChunks.ForEach(chunk => Graphics.DrawMesh(chunk.Mesh, origin, Quaternion.identity, material, 0));
        }

        public void Tick(long ticks)
        {
            activeChunks.ForEach(chunk => chunk.Tick(ticks));
        }

        public void Dispose()
        {
            ForEachChunk(chunk => chunk.Dispose());
        }

        public void ForEachChunk(Action<Chunk> action)
        {
            for (int j = 0; j < horizontalChunks; j++)
                for (int i = 0; i < verticalChunks; i++)
                    action(chunks[i, j]);
        }
        
        public void ForEachCoordinate(Action<Vector2Int> action)
        {
            for (int j = 0; j < rows; j++)
                for (int i = 0; i < columns; i++)
                    action(new Vector2Int(i, j));
        }
    }
    
    public class Grid<T> : Grid where T : BaseTile<T>
    {
        protected T[,] tiles;
        public T[,] Tiles => tiles;

        public Grid(Vector3 origin, int horizontalChunks, int verticalChunks, int rowsPerChunk, int columnsPerChunk, float tileSize, Func<Grid<T>, Vector2Int, T> instantiationFunc)
        : base(origin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize)
        {
            InitializeTiles(instantiationFunc);
            UpdateTileNeighbors();
        }

        private void UpdateTileNeighbors()
        {
            for (int j = 0; j < rows; j++)
                for (int i = 0; i < columns; i++)
                    tiles[i, j].UpdateNeighbors();
        }

        private void InitializeTiles(Func<Grid<T>, Vector2Int, T> instantiationFunc)
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

                            Vector2Int tileCoordinate = new Vector2Int(columnIndex, rowIndex);

                            tiles[columnIndex, rowIndex] = instantiationFunc(this, tileCoordinate);
                        }
                    }
                }
            }
        }

        public T TryGetTileAt(Vector3 worldPosition)
        {
            return TryGetTileAt(worldPosition.x, worldPosition.y);
        }
        
        public T TryGetTileAt(float worldPositionX, float worldPositionY)
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
        
        public T TryGetTileAt(Vector2Int coordinate)
        {
            return TryGetTileAt(coordinate.x, coordinate.y);
        }
        
        public T TryGetTileAt(int coordinateX, int coordinateY)
        {
            if (coordinateX < 0 || coordinateX >= columns)
                return default(T);
            
            if (coordinateY < 0 || coordinateY >= rows)
                return default(T);

            return tiles[coordinateX, coordinateY];
        }

        public void ForEachTile(Action<T> action)
        {
            for (int j = 0; j < rows; j++) 
                for (int i = 0; i < columns; i++) 
                    action(tiles[i, j]);
        }
    }
}
