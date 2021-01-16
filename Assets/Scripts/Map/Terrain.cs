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
    [SerializeField] [LabelWidth(100)] [Range(0, 10)]
    private int movementCost;
    public  int MovementCost => movementCost;
    
    public bool IsWalkable => movementCost < Movement.MaxCost;
    
    [SerializeField] [LabelWidth(100)] [Range(0, 10)]
    private float fertility;
    public  float Fertility => fertility;

    [SerializeField] [LabelWidth(100)]
    private int elevation;
    public  int Elevation => elevation;

    [SerializeField] [LabelWidth(100)]
    private bool isCollidable;
    public  bool IsCollidable => isCollidable;
    

    [Space(20)]
    [SerializeField]
    private TileRuleResolver ruleResolver = new TileRuleResolver();
    public  TileRuleResolver RuleResolver => ruleResolver; 

    [NonSerialized]
    private bool uvCalculated;

    private Rect2D spriteRect2D = Rect2D.Zero;

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
    
