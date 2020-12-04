using System;
using GridSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Grid = GridSystem.Grid;
using Random = System.Random;

namespace DefaultNamespace
{
    public class EnvironmentGenerator : MonoBehaviour
    {
        [Header("Grids")] 
        [SerializeField] private TerrainGridBehaviour basegroundGridBehaviour;
        [SerializeField] private TerrainGridBehaviour backgroundGridBehaviour;
        [SerializeField] private TerrainGridBehaviour foregroundGridBehaviour;
        [SerializeField] private Dummy plantGridBehaviour;

        private Grid<TerrainTile> baseGroundGrid;
        private Grid<TerrainTile> backgroundGrid;
        private Grid<TerrainTile> foreGroundGrid;
        private Grid plantGrid;

        [SerializeField] private Sprite tree;
        [SerializeField] private Terrain empty;

        private Random random = new Random(0);
        private Rect2D treeRect2D;

        private void Start()
        {
            plantGrid = plantGridBehaviour.Grid;
            baseGroundGrid = basegroundGridBehaviour.Grid;
            backgroundGrid = backgroundGridBehaviour.Grid;
            foreGroundGrid = foregroundGridBehaviour.Grid;
            
            treeRect2D = Extension.GetUVRect(tree);
        }

        private Terrain RaycastTerrain(Vector2Int coordinate)
        {
            var tile = foreGroundGrid.TryGetTileAtCoordinate(coordinate);
            
            if (tile.Terrain != empty)
            {
                return tile.Terrain;
            }
            
            tile = backgroundGrid.TryGetTileAtCoordinate(coordinate);
            
            if (tile.Terrain != empty)
            {
                return tile.Terrain;
            }

            return null;
        }

        private void TryGrowTreeAt(Vector2Int coordinate, Terrain terrain)
        {
            float growChance = (random.Next() % 100) * terrain.Fertility;

            if (growChance > 80)
            {
                plantGrid.SetTileUV(in treeRect2D, coordinate);
            }
                
        }

        [Button]
        public void Grow()
        {
            plantGrid.SetUV(Rect2D.Empty);
            
            plantGrid.ForEachCoordinate(coordinate =>
            {
                Tree(coordinate);
            });
        }

        private void Tree(Vector2Int coordinate)
        {
            var terrain = RaycastTerrain(coordinate);
            
            if (terrain != null)
                TryGrowTreeAt(coordinate, terrain);
        }
    }
}