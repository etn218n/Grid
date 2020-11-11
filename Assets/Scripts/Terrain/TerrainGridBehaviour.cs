public class TerrainGridBehaviour : GridBehaviour<TerrainTile>
{
    private void Awake()
    {
        Create((chunk, coordinate, localCoordinate) => new TerrainTile(chunk, coordinate, localCoordinate));
    }

    public override string ToString()
    {
        return "Terrain Grid";
    }
}
