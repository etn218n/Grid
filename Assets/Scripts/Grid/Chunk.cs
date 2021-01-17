using System;
using System.Collections.Generic;
using MayBe;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

namespace GridSystem
{
    public class Chunk : IDisposable
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
        
        protected readonly Grid ownerGrid;
        protected readonly List<ITickable> activeTickables = new List<ITickable>();

        protected bool uvsModified;
        protected bool verticesModified;
        protected bool colorsModified;
        protected bool indicesModified;

        public int Rows => rows;
        public int Columns => columns;
        public float Width => width;
        public float Height => height;
        public float TileSize => tileSize;
        public Vector3 Origin => origin;
        public Mesh Mesh => mesh;
        public Grid OwnerGrid => ownerGrid;
        public List<ITickable> ActiveTickables => activeTickables;

        public Chunk(Grid ownerGrid, Vector3 origin, int rows, int columns, float tileSize)
        {
            this.origin    = origin;
            this.tileSize  = tileSize;
            this.rows      = rows;
            this.columns   = columns;
            this.width     = columns * tileSize;
            this.height    = rows * tileSize;
            this.ownerGrid = ownerGrid;
            
            mesh = new Mesh();
            mesh.MarkDynamic();

            int numberOfTiles = rows * columns;
            vertices = new NativeArray<Vector3>(numberOfTiles * 4, Allocator.Persistent);
            uvs      = new NativeArray<Vector2>(numberOfTiles * 4, Allocator.Persistent);
            colors   = new NativeArray<Color>(numberOfTiles * 4, Allocator.Persistent);
            indices  = new NativeArray<int>(numberOfTiles * 6, Allocator.Persistent);
        }

        public void Tick(long ticks)
        {
            activeTickables.ForEach(t => t.Tick(ticks));
        }

        public void MarkActive()
        {
            ownerGrid.ActiveChunks.Add(this);
        }

        public void MarkInactive()
        {
            ownerGrid.ActiveChunks.Remove(this);
        }

        public void MarkVisible()
        {
            ownerGrid.VisibleChunks.Add(this);
        }
        
        public void MarkInvisible()
        {
            ownerGrid.VisibleChunks.Remove(this);
        }

        public void ToggleActive()
        {
            if (ownerGrid.ActiveChunks.Contains(this))
                MarkInactive();
            else
                MarkActive();
        }
        
        public void ToggleVisible()
        {
            if (ownerGrid.VisibleChunks.Contains(this))
                MarkInvisible();
            else
                MarkVisible();
        }
        
        public void SetUVs(in Rect2D uvRect)
        {
            for (int i = 0; i < rows * columns * 4; i += 4)
            {
                uvs[i + 0] = uvRect.BottomLeft;
                uvs[i + 1] = uvRect.TopLeft;
                uvs[i + 2] = uvRect.BottomRight;
                uvs[i + 3] = uvRect.TopRight;
            }

            OnUVsModified();
        }

        public void SetTileUVsAt(Vector2Int tileLocalCoordinate, in Rect2D uvRect)
        {
            if (tileLocalCoordinate.x < 0 || tileLocalCoordinate.x >= columns)
            {
                Debug.Log("Cell Coordinate X can not be greater than Columns");
                return;
            }

            if (tileLocalCoordinate.y < 0 || tileLocalCoordinate.y >= rows)
            {
                Debug.Log("Cell Coordinate Y can not be greater than Rows");
                return;
            }
            
            int index = (tileLocalCoordinate.y * columns * 4) + (tileLocalCoordinate.x * 4);

            uvs[index + 0] = uvRect.BottomLeft;
            uvs[index + 1] = uvRect.TopLeft;
            uvs[index + 2] = uvRect.BottomRight;
            uvs[index + 3] = uvRect.TopRight;
            
            OnUVsModified();
        }

        public void SetTileVerticesAt(Vector2Int tileLocalCoordinate, in Rect3D vertexRect)
        {
            int index = (tileLocalCoordinate.y * columns * 4) + (tileLocalCoordinate.x * 4);
            
            vertices[index + 0] = vertexRect.BottomLeft;
            vertices[index + 1] = vertexRect.TopLeft;
            vertices[index + 2] = vertexRect.BottomRight;
            vertices[index + 3] = vertexRect.TopRight;
            
            OnVerticesModified();
        }
        
        public void SetColor(Color color)
        {
            for (int i = 0; i < rows * columns * 4; i++)
                colors[i] = color;

            OnColorModified();
        }

        public void SetTileColorAt(Vector2Int tileLocalCoordinate, Color color)
        {
            int index = (tileLocalCoordinate.y * columns * 4) + (tileLocalCoordinate.x * 4);
            
            colors[index + 0] = color;
            colors[index + 1] = color;
            colors[index + 2] = color;
            colors[index + 3] = color;
            
            OnColorModified();
        }
        
        public Maybe<Rect3D> GetTileVertexRectAt(Vector2Int tileLocalCoordinate)
        {
            if (tileLocalCoordinate.x < 0 || tileLocalCoordinate.x > columns)
                return Maybe.None<Rect3D>();
            
            if (tileLocalCoordinate.y < 0 || tileLocalCoordinate.y > rows)
                return Maybe.None<Rect3D>();
            
            int index = (tileLocalCoordinate.y * columns * 4) + (tileLocalCoordinate.x * 4);

            return Maybe.Some(new Rect3D(vertices[index + 0], 
                                         vertices[index + 1], 
                                         vertices[index + 2], 
                                         vertices[index + 3]));
        }
        
        private void OnUVsModified()
        {
            if (uvsModified == false)
                ownerGrid.ModifiedChunks.Enqueue(this);
            
            uvsModified = true;
        }

        private void OnVerticesModified()
        {
            if (verticesModified == false)
                ownerGrid.ModifiedChunks.Enqueue(this);
            
            verticesModified = true;
        }

        private void OnIndicesModified()
        {
            if (indicesModified == false)
                ownerGrid.ModifiedChunks.Enqueue(this);
            
            indicesModified = true;
        }
        
        private void OnColorModified()
        {
            if (colorsModified == false)
                ownerGrid.ModifiedChunks.Enqueue(this);
            
            colorsModified = true;
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
                OriginZ  = origin.z,
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