using GridSystem;
using UnityEngine;

public class PlantTile : BaseTile<PlantTile>, ITickable
{
    private Plant plant;
    private float fertility;
    private float growSpeed;
    
    public PlantTile(Grid<PlantTile> ownerGrid, Vector2Int coordinate) : base(ownerGrid, coordinate)
    {
    }

    public void Seed(Plant plant)
    {
        this.plant = plant;

        growSpeed = fertility * Random.Range(0.0001f, 0.001f);

        OnPlantSeeded();
    }

    public void Fertilize(float fertility)
    {
        this.fertility = Mathf.Clamp(fertility, 0, 10);
    }

    private void OnPlantSeeded()
    {
        ownerChunk.ActiveTickables.Add(this);
        AdjustTileDimension(plant.Width, plant.Height);
        SetUVs(plant.SpriteUVRect);
    }

    public void ShowPlantInfo()
    {
        if (plant == null)
            return;

        Debug.Log(plant.Maturity);
    }

    public void Tick(long ticks)
    {
        if (plant.IsFullyGrown)
            return;
        
        plant.Grow(growSpeed);
        AdjustPlantSize();
    }

    public void RemovePlant()
    {
        plant = null;

        OnPlantRemoved();
    }

    private void OnPlantRemoved()
    {
        ownerChunk.ActiveTickables.Remove(this);
        AdjustTileDimension(1, 1);
        SetUVs(Rect2D.Zero);
    }

    private void AdjustPlantSize()
    {
        Rect3D vertexRect = GetVertexRect();

        Vector3 bottomLeft  = vertexRect.BottomLeft;
        Vector3 topLeft     = vertexRect.TopLeft;
        Vector3 bottomRight = vertexRect.BottomRight;
        Vector3 topRight    = vertexRect.TopRight;
        
        float tileHeight = (plant.Height * ownerGrid.TileSize) * plant.Maturity;
        
        topLeft.y  = bottomLeft.y + tileHeight;
        topRight.y = bottomLeft.y + tileHeight;

        SetVertices(new Rect3D(bottomLeft, topLeft, bottomRight, topRight));
    }

    private void AdjustTileDimension(int width, int height)
    {
        Rect3D vertexRect = GetVertexRect();

        Vector3 bottomLeft  = vertexRect.BottomLeft;
        Vector3 topLeft     = vertexRect.TopLeft;
        Vector3 bottomRight = vertexRect.BottomRight;
        Vector3 topRight    = vertexRect.TopRight;

        float tileWidth  = width  * ownerGrid.TileSize;
        float tileHeight = height * ownerGrid.TileSize;

        topRight.x    = bottomLeft.x + tileWidth;
        bottomRight.x = bottomLeft.x + tileWidth;
        topLeft.y     = bottomLeft.y + tileHeight;
        topRight.y    = bottomLeft.y + tileHeight;
        
        float zValue  = (bottomLeft.y) * 0.0001f;
        topLeft.z     = zValue;
        topRight.z    = zValue;
        bottomLeft.z  = zValue;
        bottomRight.z = zValue;

        SetVertices(new Rect3D(bottomLeft, topLeft, bottomRight, topRight));
    }
}