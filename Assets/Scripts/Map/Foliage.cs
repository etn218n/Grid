using System;
using GridSystem;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Foliage.asset", menuName = "Map/Foliage")]
public class Foliage : ScriptableObject
{
    [Space(10)]
    [SerializeField] [PreviewField(ObjectFieldAlignment.Left)] [LabelWidth(70)]
    private Sprite mainSprite;

    [NonSerialized] 
    private bool uvCalculated = false;

    private Rect2D spriteRect2D;

    public ref readonly Rect2D SpriteRect2D
    {
        get
        {
            if (!uvCalculated)
            {
                spriteRect2D = Extension.GetUVRect(mainSprite);
                uvCalculated = true;
            }

            return ref spriteRect2D;
        }
    }
}
