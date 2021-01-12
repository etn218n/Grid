using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Collections;
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

            tile.EastNeighbor.Filter(n => tile.IsRightEdge || tile.SameTileCategory(n))
                             .Match(some: (n) => ruleMask |= RuleEnum.HasEastNeighbor, 
                                    none: ()  => ruleMask |= RuleEnum.NotHasEastNeighbor);
            
            tile.WestNeighbor.Filter(n => tile.IsLeftEdge || tile.SameTileCategory(n))
                             .Match(some: (n) => ruleMask |= RuleEnum.HasWestNeighbor, 
                                    none: ()  => ruleMask |= RuleEnum.NotHasWestNeighbor);
            
            tile.SouthNeighbor.Filter(n => tile.IsBottomEdge || tile.SameTileCategory(n))
                              .Match(some: (n) => ruleMask |= RuleEnum.HasSouthNeighbor, 
                                     none: ()  => ruleMask |= RuleEnum.NotHasSouthNeighbor);
            
            tile.NorthNeighbor.Filter(n => tile.IsTopEdge || tile.SameTileCategory(n))
                              .Match(some: (n) => ruleMask |= RuleEnum.HasNorthNeighbor, 
                                     none: ()  => ruleMask |= RuleEnum.NotHasNorthNeighbor);
            
            tile.SouthEastNeighbor.Filter(n => tile.IsBottomEdge || tile.IsRightEdge || tile.SameTileCategory(n))
                                  .Match(some: (n) => ruleMask |= RuleEnum.HasSouthEastNeighbor, 
                                         none: ()  => ruleMask |= RuleEnum.NotHasSouthEastNeighbor);
            
            tile.SouthWestNeighbor.Filter(n => tile.IsBottomEdge || tile.IsLeftEdge || tile.SameTileCategory(n))
                                  .Match(some: (n) => ruleMask |= RuleEnum.HasSouthWestNeighbor, 
                                         none: ()  => ruleMask |= RuleEnum.NotHasSouthWestNeighbor);
            
            tile.NorthEastNeighbor.Filter(n => tile.IsTopEdge || tile.IsRightEdge || tile.SameTileCategory(n))
                                  .Match(some: (n) => ruleMask |= RuleEnum.HasNorthEastNeighbor, 
                                         none: ()  => ruleMask |= RuleEnum.NotHasNorthEastNeighbor);
            
            tile.NorthWestNeighbor.Filter(n => tile.IsTopEdge || tile.IsLeftEdge || tile.SameTileCategory(n))
                                  .Match(some: (n) => ruleMask |= RuleEnum.HasNorthWestNeighbor, 
                                         none: ()  => ruleMask |= RuleEnum.NotHasNorthWestNeighbor);

            return ruleMask;
        }

        public Option<TileRule> MatchedRule<T>(T tile) where T : BaseTile<T>
        {
            var ruleMask = TileNeighborsToMask(tile);
            
            return rules.FirstOrNone(rule => rule.Match(ruleMask));
        }
    }
}