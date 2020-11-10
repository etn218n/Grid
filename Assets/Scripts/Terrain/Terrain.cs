using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Terrain.asset")]
public class Terrain : ScriptableObject
{
    [Space(10)]
    [SerializeField] [PreviewField] [HorizontalGroup("Row 1", Width = 60)] [HideLabel]
    private Sprite northWestTile = null;
    
    [Space(10)]
    [SerializeField] [PreviewField] [HorizontalGroup("Row 1", Width = 60)] [HideLabel]
    private Sprite northTile = null;
    
    [Space(10)]
    [SerializeField] [PreviewField] [HorizontalGroup("Row 1", Width = 60)] [HideLabel]
    private Sprite northEastTile = null;
    
    [Space(10)]
    [SerializeField] [PreviewField] [HorizontalGroup("Row 2", Width = 60)] [HideLabel]
    private Sprite westTile = null;
    
    [Space(10)]
    [SerializeField] [PreviewField] [HorizontalGroup("Row 2", Width = 60)] [HideLabel]
    private Sprite centerTile = null;
    
    [Space(10)]
    [SerializeField] [PreviewField] [HorizontalGroup("Row 2", Width = 60)] [HideLabel]
    private Sprite eastTile = null;

    [Space(10)]
    [SerializeField] [PreviewField] [HorizontalGroup("Row 3", Width = 60)] [HideLabel]
    private Sprite southWestTile = null;
    
    [Space(10)]
    [SerializeField] [PreviewField] [HorizontalGroup("Row 3", Width = 60)] [HideLabel]
    private Sprite southTile = null;
    
    [Space(10)]
    [SerializeField] [PreviewField] [HorizontalGroup("Row 3", Width = 60)] [HideLabel]
    private Sprite southEastTile = null;
}
    
