using GridSystem;
using UnityEngine;

public class Plant
{
    private int width;
    private int height;
    private float maturity;
    private Rect2D spriteUVRect;

    public int Width => width;
    public int Height => height;
    public float Maturity => maturity;
    public Rect2D SpriteUVRect => spriteUVRect;
    public bool IsFullyGrown => maturity >= 1;

    public Plant(int width, int height, Rect2D spriteUVRect)
    {
        this.width  = width;
        this.height = height;
        this.spriteUVRect = spriteUVRect;
    }

    public void Grow(float deltaGrowth)
    {
        maturity += deltaGrowth;
        maturity = Mathf.Clamp01(maturity);
    }
}