using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSetting.asset")]
public class MapSetting : ScriptableObject
{
    [SerializeField] [ListDrawerSettings(DraggableItems = false)]
    private List<TerrainSetting> settings;

    [Button]
    public void Sort()
    {
        settings.Sort((a, b) =>
        {
            if (a.Height < b.Height)
                return -1;
            else if (a.Height > b.Height)
                return 1;

            return 0;
        });
    }

    public Terrain Evaluate(float value)
    {
        foreach (var setting in settings)
        {
            if (value < setting.Height)
                return setting.TargetTerrain;
        }

        return null;
    }
}

[Serializable]
public class TerrainSetting
{
    public Terrain TargetTerrain;
    public float Height;
}
