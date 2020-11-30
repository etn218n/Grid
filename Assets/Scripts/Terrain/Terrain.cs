using System;
using GridSystem;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Terrain.asset")]
public class Terrain : ScriptableObject
{
    [Space(10)]
    [SerializeField] [PreviewField(ObjectFieldAlignment.Left)] [LabelWidth(70)]
    private Sprite mainSprite = null;
    public  Sprite MainSprite => mainSprite;

    [Space(10)]
    [SerializeField] [LabelWidth(70)]
    private int elevation;
    public int Elevation => elevation;

    [SerializeField]
    private TileRuleResolver ruleResolver = new TileRuleResolver();
    public  TileRuleResolver RuleResolver => ruleResolver; 

    [NonSerialized]
    private bool uvCalculated = false;

    private UVRect spriteUVRect;

    public ref readonly UVRect SpriteUVRect
    {
        get
        {
            if (!uvCalculated)
            {
                spriteUVRect = Extension.GetUVRect(mainSprite);
                uvCalculated = true;
            }

            return ref spriteUVRect;
        }
    }
}
    
