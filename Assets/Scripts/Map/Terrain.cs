using System;
using GridSystem;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Terrain.asset", menuName = "Map/Terrain")]
public class Terrain : ScriptableObject
{
    [Space(10)]
    [SerializeField] [PreviewField(ObjectFieldAlignment.Left)] [LabelWidth(70)]
    private Sprite mainSprite;
    public  Sprite MainSprite => mainSprite;

    [Space(10)]
    [SerializeField] [LabelWidth(70)] [Range(0, 10)]
    private float fertility;
    public  float Fertility => fertility;

    [SerializeField] [LabelWidth(70)]
    private int elevation;
    public int Elevation => elevation;

    [SerializeField]
    private TileRuleResolver ruleResolver = new TileRuleResolver();
    public  TileRuleResolver RuleResolver => ruleResolver; 

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
    
