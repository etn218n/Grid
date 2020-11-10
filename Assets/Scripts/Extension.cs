using UnityEngine;

public static class Extension
{
    public static bool IsBetween(this float thisValue, float value1, float value2, bool inclusive = true)
    {
        if (inclusive)
            return thisValue >= Mathf.Min(value1, value2) && thisValue <= Mathf.Max(value1, value2);

        return thisValue > Mathf.Min(value1, value2) && thisValue < Mathf.Max(value1, value2);
    }

    public static bool IsBetween(this int thisValue, int value1, int value2, bool inclusive = true)
    {
        if (inclusive)
            return thisValue >= Mathf.Min(value1, value2) && thisValue <= Mathf.Max(value1, value2);

        return thisValue > Mathf.Min(value1, value2) && thisValue < Mathf.Max(value1, value2);
    }
    
    public static Vector2[] GetUVs(Sprite sprite)
    {
        var rect = sprite.rect;
        
        Vector2[] uv = new[]
        {
            new Vector2(rect.x / sprite.texture.width, (sprite.rect.y + sprite.rect.height) / sprite.texture.height), //bottom left 
            new Vector2(rect.x / sprite.texture.width, rect.y / sprite.texture.height), //top left
            new Vector2((rect.x + rect.width) / sprite.texture.width, (sprite.rect.y + sprite.rect.height) / sprite.texture.height), 
            new Vector2((rect.x + rect.width) / sprite.texture.width, rect.y / sprite.texture.height), //top right
        };
        
        return uv;
    }
}