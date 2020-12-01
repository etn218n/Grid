using System;
using UnityEngine;

namespace GridSystem
{
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
                new Vector2(rect.x / sprite.texture.width, rect.y / sprite.texture.height), //top left
                new Vector2(rect.x / sprite.texture.width, (sprite.rect.y + sprite.rect.height) / sprite.texture.height), //bottom left 
                new Vector2((rect.x + rect.width) / sprite.texture.width, rect.y / sprite.texture.height), //top right
                new Vector2((rect.x + rect.width) / sprite.texture.width, (sprite.rect.y + sprite.rect.height) / sprite.texture.height),
            };

            return uv;
        }
    
        public static UVRect GetUVRect(Sprite sprite)
        {
            var rect = sprite.rect;

            Vector2 topLeft     = new Vector2(rect.x / sprite.texture.width, (sprite.rect.y + sprite.rect.height) / sprite.texture.height);
            Vector2 bottomLeft  = new Vector2(rect.x / sprite.texture.width, rect.y / sprite.texture.height);
            Vector2 topRight    = new Vector2((rect.x + rect.width) / sprite.texture.width, (sprite.rect.y + sprite.rect.height) / sprite.texture.height);
            Vector2 bottomRight = new Vector2((rect.x + rect.width) / sprite.texture.width, rect.y / sprite.texture.height);
            
            return new UVRect(bottomLeft, topLeft, bottomRight, topRight);
        }
        
        public static void LogVector(Vector3 v)
        {
            Debug.Log(String.Format("({0:F5}, {1:F5}, {2:F5})", v.x, v.y, v.z));
        }
        
        public static void LogVector(Vector2 v)
        {
            Debug.Log(String.Format("({0:F5}, {1:F5})", v.x, v.y));
        }
    }
}