using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace GridSystem
{
    [BurstCompile]
    public struct GenerateChunkMeshJob : IJob
    {
        public NativeArray<Vector3> Vertices;
        public NativeArray<Vector2> UVs;
        public NativeArray<Color>   Colors;
        public NativeArray<int>     Indices;

        public int NumberOfCells;
        public int Rows;
        public int Columns;
        public float CellSize;
        public float OriginX;
        public float OriginY;
        public float OriginZ;
        
        public void Execute()
        {
            for (int i = 0; i < NumberOfCells; i++)
            {
                GenerateUVs(i);
                GenerateVertices(i);
                GenerateTriangles(i);
                GenerateColors(i);
            }
        }
        
        private void GenerateVertices(int cellIndex)
        {
            int rowIndex    = cellIndex / Columns;
            int columnIndex = cellIndex % Columns;

            Vector3 vertexPosition = new Vector3(OriginX + (columnIndex * CellSize) + (CellSize / 2),
                                                 OriginY + (rowIndex * CellSize) + (CellSize / 2),
                                                 OriginZ);
            
            int verticesIndex = cellIndex * 4;

            float margin = CellSize / 2;

            Vertices[verticesIndex + 0] = new Vector3(vertexPosition.x - margin, vertexPosition.y - margin, vertexPosition.z);
            Vertices[verticesIndex + 1] = new Vector3(vertexPosition.x - margin, vertexPosition.y + margin, vertexPosition.z);
            Vertices[verticesIndex + 2] = new Vector3(vertexPosition.x + margin, vertexPosition.y - margin, vertexPosition.z);
            Vertices[verticesIndex + 3] = new Vector3(vertexPosition.x + margin, vertexPosition.y + margin, vertexPosition.z);
        }
        
        private void GenerateUVs(int cellIndex)
        {
            int uvsIndex = cellIndex * 4;
            
            UVs[uvsIndex + 0] = new Vector2(0, 0);
            UVs[uvsIndex + 1] = new Vector2(0, 1);
            UVs[uvsIndex + 2] = new Vector2(1, 0);
            UVs[uvsIndex + 3] = new Vector2(1, 1);
        }
        
        private void GenerateTriangles(int cellIndex)
        {
            int indicesIndex = cellIndex * 6;

            Indices[indicesIndex + 0] = cellIndex * 4 + 0;
            Indices[indicesIndex + 1] = cellIndex * 4 + 1;
            Indices[indicesIndex + 2] = cellIndex * 4 + 2;
            Indices[indicesIndex + 3] = cellIndex * 4 + 2;
            Indices[indicesIndex + 4] = cellIndex * 4 + 1;
            Indices[indicesIndex + 5] = cellIndex * 4 + 3;
        }

        private void GenerateColors(int cellIndex)
        {
            int colorsIndex = cellIndex * 4;
            
            Colors[colorsIndex + 0] = Color.white;
            Colors[colorsIndex + 1] = Color.white;
            Colors[colorsIndex + 2] = Color.white;
            Colors[colorsIndex + 3] = Color.white;
        }
    }
}