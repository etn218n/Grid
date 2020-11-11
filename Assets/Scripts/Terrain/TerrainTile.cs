using GridSystem;
using UnityEngine;

public enum TerrainType { None, Sand, Soil, Grass, Water }

public class TerrainTile : Tile
{
    public TerrainType Type = TerrainType.None;

    public TerrainTile(Chunk ownerChunk, Vector2Int coordinate, Vector2Int localCoordinate) : base(ownerChunk, coordinate, localCoordinate)
    {
    }
}
