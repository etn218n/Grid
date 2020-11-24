public class TerrainGridBehaviour : GridBehaviour<TerrainTile>
{
    private void Awake()
    {
        Create((grid, chunk, coordinate, localCoordinate) => new TerrainTile(grid, chunk, coordinate, localCoordinate));
    }

    public override string ToString()
    {
        return "Terrain Grid";
    }
}
