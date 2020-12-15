using System;
using GridSystem;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Plant.asset", menuName = "Map/Plant")]
public class PlantBlueprint : ScriptableObject
{
    [Space(10)]
    [SerializeField] [PreviewField(ObjectFieldAlignment.Left)] [LabelWidth(70)]
    private Sprite mainSprite;

    [SerializeField] 
    private int width;
    public  int Width => width;
    
    [SerializeField] 
    private int height;
    public  int Height => height;

    [NonSerialized] 
    private bool uvCalculated = false;

    private Rect2D spriteRectUV;

    public ref readonly Rect2D SpriteRectUV
    {
        get
        {
            if (!uvCalculated)
            {
                spriteRectUV = Extension.GetUVRect(mainSprite);
                uvCalculated = true;
            }

            return ref spriteRectUV;
        }
    }

    public Plant CreatePlant()
    {
        return new Plant(width, height, SpriteRectUV);
    }
}
