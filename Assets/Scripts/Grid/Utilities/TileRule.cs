using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace GridSystem
{
    [Flags]
    public enum RuleEnum
    {
        None   = 0,
        HasAll = 255,
        
        HasNorthEastNeighbor = 1,
        HasSouthEastNeighbor = 2,
        HasSouthWestNeighbor = 4,
        HasNorthWestNeighbor = 8,
        HasNorthNeighbor = 16,
        HasEastNeighbor  = 32, 
        HasSouthNeighbor = 64,
        HasWestNeighbor  = 128,
        
        NotHasNorthEastNeighbor = 256,
        NotHasSouthEastNeighbor = 512,
        NotHasSouthWestNeighbor = 1024,
        NotHasNorthWestNeighbor = 2048,
        NotHasNorthNeighbor = 4096,
        NotHasEastNeighbor  = 8192, 
        NotHasSouthNeighbor = 16384,
        NotHasWestNeighbor  = 32768,
    }
    
    [Serializable]
    public class TileRule
    {
        [SerializeField] private Sprite outputSprite;
        [SerializeField] private RuleEnum ruleMask;

        [NonSerialized] private Rect2D outputUVRect;
        [NonSerialized] private bool uvCalculated;

        public Sprite OutputSprite
        {
            get => outputSprite;
            set
            {
                outputSprite = value;
                OnOutputSpriteChanged();
            }
        }

        public RuleEnum RuleMask
        {
            get => ruleMask;
            set
            {
                ruleMask = value;
            }
        }
        
        public ref readonly Rect2D OutputUVRect
        {
            get
            {
                if (!uvCalculated)
                {
                    this.outputUVRect = Extension.GetUVRect(outputSprite);
                    uvCalculated = true;
                }
                
                return ref outputUVRect;
            }
        }

        private void OnOutputSpriteChanged()
        {
            if (outputSprite != null)
                outputUVRect = Extension.GetUVRect(outputSprite);
        }

        public TileRule(RuleEnum ruleMask, Sprite outputSprite)
        {
            this.ruleMask = ruleMask;
            this.outputSprite = outputSprite;
            this.outputUVRect = Extension.GetUVRect(outputSprite);
            this.uvCalculated = true;
        }

        public bool Match(RuleEnum ruleMask)
        {
            if ((this.ruleMask & ruleMask) == this.ruleMask)
                return true;

            return false;
        }

        public ref readonly Rect2D Output(RuleEnum ruleMask)
        {
            if ((this.ruleMask & ruleMask) != this.ruleMask)
                return ref Rect2D.Zero;

            return ref outputUVRect;
        }
    }
    
#if UNITY_EDITOR
    public class TileRuleAttributeProcessor : OdinAttributeProcessor<TileRule>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            TileRule rule = (TileRule)parentProperty.ValueEntry.WeakSmartValue;
            
            if (member.Name == "outputSprite")
            {
                attributes.Add(new HideLabelAttribute());
                attributes.Add(new HorizontalGroupAttribute("Row 1", 60));
                attributes.Add(new PreviewFieldAttribute(ObjectFieldAlignment.Left));
            }
            else if (member.Name == "ruleMask")
            {
                attributes.Add(new HideLabelAttribute());
                attributes.Add(new HorizontalGroupAttribute("Row 1"));
            }
        }
    }
#endif
}