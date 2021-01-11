using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace GridSystem
{
    [BurstCompile]
    public struct SetChunkMeshColorJob : IJob
    {
        public NativeArray<Color> Colors;

        public Color PaintColor;
        
        public int NumberOfCells;
        
        public void Execute()
        {
            for (int i = 0; i < NumberOfCells; i++)
            {
                int colorsIndex = i * 4;
            
                Colors[colorsIndex + 0] = PaintColor;
                Colors[colorsIndex + 1] = PaintColor;
                Colors[colorsIndex + 2] = PaintColor;
                Colors[colorsIndex + 3] = PaintColor;
            }
        }
    }
}