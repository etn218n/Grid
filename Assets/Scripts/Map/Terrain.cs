using System;
using GridSystem;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Terrain.asset", menuName = "Map/Terrain")]
public class Terrain : ScriptableObject, IHaveMovementCost
{
    [Space(10)]
    [SerializeField] [PreviewField(ObjectFieldAlignment.Left)] [LabelWidth(70)]
    private Sprite mainSprite;
    public  Sprite MainSprite => mainSprite;

    [Space(20)]
    [SerializeField] [LabelWidth(100)]
    private short movementCost;
    public  short MovementCost => movementCost;
    
    [SerializeField] [LabelWidth(100)] [Range(0, 10)]
    private float fertility;
    public  float Fertility => fertility;

    [SerializeField] [LabelWidth(100)]
    private int elevation;
    public int Elevation => elevation;

    [Space(20)]
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
    
