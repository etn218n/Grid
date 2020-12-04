using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GridSystem
{
    [Serializable]
    public class TileRuleResolver
    {
        [SerializeField] [ListDrawerSettings(NumberOfItemsPerPage = 8)]
        private List<TileRule> rules = new List<TileRule>();

        public void AddRule(TileRule rule)
        {
            rules.Add(rule);
        }
        
        public void AddRule(RuleEnum rulemask, Sprite outputSprite)
        {
            rules.Add(new TileRule(rulemask, outputSprite));
        }

        public RuleEnum TileNeighborsToMask<T>(T tile) where T : BaseTile<T>
        {
            RuleEnum ruleMask = 0;

            if (tile.IsRightEdge || (tile.HasEastNeighbor && tile.ShareAttributeWith(tile.EastNeighbor)))
                ruleMask |= RuleEnum.HasEastNeighbor;
            else
                ruleMask |= RuleEnum.NotHasEastNeighbor;
            
            
            if (tile.IsLeftEdge || (tile.HasWestNeighbor && tile.ShareAttributeWith(tile.WestNeighbor)))
                ruleMask |= RuleEnum.HasWestNeighbor;
            else
                ruleMask |= RuleEnum.NotHasWestNeighbor;
            
            
            if (tile.IsBottomEdge || (tile.HasSouthNeighbor && tile.ShareAttributeWith(tile.SouthNeighbor)))
                ruleMask |= RuleEnum.HasSouthNeighbor;
            else
                ruleMask |= RuleEnum.NotHasSouthNeighbor;
            
            
            if (tile.IsTopEdge || (tile.HasNorthNeighbor && tile.ShareAttributeWith(tile.NorthNeighbor)))
                ruleMask |= RuleEnum.HasNorthNeighbor;
            else
                ruleMask |= RuleEnum.NotHasNorthNeighbor;
            
            
            if (tile.IsBottomEdge || tile.IsRightEdge || (tile.HasSouthEastNeighbor && tile.ShareAttributeWith(tile.SouthEastNeighbor)))
                ruleMask |= RuleEnum.HasSouthEastNeighbor;
            else
                ruleMask |= RuleEnum.NotHasSouthEastNeighbor;
            
            
            if (tile.IsBottomEdge || tile.IsLeftEdge || (tile.HasSouthWestNeighbor && tile.ShareAttributeWith(tile.SouthWestNeighbor)))
                ruleMask |= RuleEnum.HasSouthWestNeighbor;
            else
                ruleMask |= RuleEnum.NotHasSouthWestNeighbor;
            
            
            if (tile.IsTopEdge || tile.IsRightEdge || (tile.HasNorthEastNeighbor && tile.ShareAttributeWith(tile.NorthEastNeighbor)))
                ruleMask |= RuleEnum.HasNorthEastNeighbor;
            else
                ruleMask |= RuleEnum.NotHasNorthEastNeighbor;
            
            
            if (tile.IsTopEdge || tile.IsLeftEdge || (tile.HasNorthWestNeighbor && tile.ShareAttributeWith(tile.NorthWestNeighbor)))
                ruleMask |= RuleEnum.HasNorthWestNeighbor;
            else
                ruleMask |= RuleEnum.NotHasNorthWestNeighbor;
            

            return ruleMask;
        }

        public ref readonly Rect2D Output<T>(T tile) where T : BaseTile<T>
        {
            var mask = TileNeighborsToMask(tile);
            
            foreach (var rule in rules)
            {
                if (rule.Check(mask) == true)
                {
                    return ref rule.OutputUVRect;
                }
            }
            
            return ref Rect2D.Empty;
        }
    }
}