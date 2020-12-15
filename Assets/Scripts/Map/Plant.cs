using GridSystem;
using UnityEngine;

public class Plant
{
    private int width;
    private int height;
    private float maturity;
    private Rect2D spriteRectUV;

    public int Width => width;
    public int Height => height;
    public float Maturity => maturity;
    public Rect2D SpriteRectUV => spriteRectUV;

    public Plant(int width, int height, Rect2D spriteRectUV)
    {
        this.width  = width;
        this.height = height;
        this.spriteRectUV = spriteRectUV;
    }

    public void Grow(float deltaGrowth)
    {
        maturity += deltaGrowth;
        maturity = Mathf.Clamp01(maturity);
    }
}