using System;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

namespace GridSystem
{
    public class Chunk<T> : IDisposable
    {
        protected int rows;
        protected int columns;
        protected float width;
        protected float height;
        protected float tileSize;
        protected Vector3 origin;
        protected Mesh mesh;
        protected NativeArray<Vector3> vertices;
        protected NativeArray<Vector2> uvs;
        protected NativeArray<Color> colors;
        protected NativeArray<int> indices;
        protected readonly Grid<T> ownerGrid;

        protected bool uvsModified;
        protected bool verticesModified;
        protected bool colorsModified;
        protected bool indicesModified;

        public int Rows     => rows;
        public int Columns  => columns;
        public float Width  => width;
        public float Height => height;
        public float TileSize => tileSize;
        public Mesh Mesh => mesh;
        public Grid<T> OwnerGrid => ownerGrid;

        public Chunk(Grid<T> ownerGrid, Vector3 origin, int rows, int columns, float tileSize)
        {
            this.origin   = origin;
            this.tileSize = tileSize;
            this.rows     = rows;
            this.columns  = columns;
            this.width    = columns * tileSize;
            this.height   = rows * tileSize;
            
            this.ownerGrid = ownerGrid;
            
            mesh = new Mesh();
            mesh.MarkDynamic();

            int numberOfTiles = rows * columns;
            
            vertices  = new NativeArray<Vector3>(numberOfTiles * 4, Allocator.Persistent);
            uvs       = new NativeArray<Vector2>(numberOfTiles * 4, Allocator.Persistent);
            colors    = new NativeArray<Color>(numberOfTiles * 4, Allocator.Persistent);
            indices   = new NativeArray<int>(numberOfTiles * 6, Allocator.Persistent);
        }

        public void SetChunkUVs(ref Vector2[] uvs)
        {
            for (int i = 0; i < rows * columns * 4; i += 4)
            {
                this.uvs[i + 0] = uvs[0];
                this.uvs[i + 1] = uvs[1];
                this.uvs[i + 2] = uvs[2];
                this.uvs[i + 3] = uvs[3];
            }
            
            OnUVsModified();
        }
        
        public void SetTileUVs(Vector2Int localCoordinate, ref Vector2[] uv)
        {
            if (localCoordinate.x < 0 || localCoordinate.x >= columns)
            {
                Debug.Log("Cell Coordinate X can not be greater than Columns");
                return;
            }

            if (localCoordinate.y < 0 || localCoordinate.y >= rows)
            {
                Debug.Log("Cell Coordinate Y can not be greater than Rows");
                return;
            }
            
            int index = (localCoordinate.y * columns * 4) + (localCoordinate.x * 4);

            uvs[index + 0] = uv[0];
            uvs[index + 1] = uv[1];
            uvs[index + 2] = uv[2];
            uvs[index + 3] = uv[3];
            
            OnUVsModified();
        }

        private void OnUVsModified()
        {
            uvsModified = true;
            
            if (!ownerGrid.ModifiedChunks.Contains(this))
                ownerGrid.ModifiedChunks.Enqueue(this);
        }

        private void OnVerticesModified()
        {
            verticesModified = true;

            if (!ownerGrid.ModifiedChunks.Contains(this))
                ownerGrid.ModifiedChunks.Enqueue(this);
        }

        private void OnIndicesModified()
        {
            indicesModified = true;

            if (!ownerGrid.ModifiedChunks.Contains(this))
                ownerGrid.ModifiedChunks.Enqueue(this);
        }
        
        private void OnColorModified()
        {
            colorsModified = true;

            if (!ownerGrid.ModifiedChunks.Contains(this))
                ownerGrid.ModifiedChunks.Enqueue(this);
        }
        
        public void UpdateMesh()
        {
            UpdateVertices();
            UpdateUVs();
            UpdateColor();
            UpdateIndices();
        }

        public void UpdateVertices()
        {
            if (verticesModified)
            {
                mesh.SetVertices(vertices);
                verticesModified = false;
            }
        }
        
        public void UpdateColor()
        {
            if (colorsModified)
            {
                mesh.SetColors(colors);
                colorsModified = true;
            }
        }

        public void UpdateUVs()
        {
            if (uvsModified)
            {
                mesh.SetUVs(0, uvs);
                uvsModified = false;
            }
        }

        public void UpdateIndices()
        {
            if (indicesModified)
            {
                mesh.SetIndices(indices, MeshTopology.Triangles, 0);
                indicesModified = false;
            }
        }

        public void SetChunkColor(Color color)
        {
            for (int i = 0; i < rows * columns * 4; i++)
            {
                colors[i] = color;
            }
            
            OnColorModified();
        }
        
        public JobHandle SetColorJobHandle(Color color)
        {
            SetChunkMeshColorJob job = new SetChunkMeshColorJob()
            {
                Colors = colors,
                PaintColor = color,
                NumberOfCells = rows * columns
            };

            return job.Schedule();
        }
        
        public JobHandle ChunkMeshJobHandle()
        {
            GenerateChunkMeshJob job = new GenerateChunkMeshJob()
            {
                Vertices = vertices,
                UVs      = uvs,
                Indices  = indices,
                Colors   = colors,
                Rows     = rows,
                Columns  = columns,
                CellSize = tileSize,
                OriginX  = origin.x,
                OriginY  = origin.y,
                NumberOfCells = rows * columns
            };

            uvsModified      = true;
            colorsModified   = true;
            indicesModified  = true;
            verticesModified = true;

            return job.Schedule();
        }

        public void Dispose()
        {
            uvs.Dispose();
            colors.Dispose();
            indices.Dispose();
            vertices.Dispose();
        }
    }
}