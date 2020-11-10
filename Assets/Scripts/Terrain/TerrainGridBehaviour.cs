public class TerrainGridBehaviour : GridBehaviour<TerrainTile>
{
    private void Awake()
    {
        Create((tile) => new TerrainTile());
    }

    public override string ToString()
    {
        return "Terrain Grid";
    }
}
