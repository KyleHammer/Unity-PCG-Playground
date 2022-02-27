using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Cellular_Automata_Script
{
    public class CellAutoGen : MonoBehaviour
    {
        [Header("PLAY MODE ONLY BUTTONS")]
        [SerializeField] private bool regenerateMapButton = false;
        
        [Space] [Header("Cell Map Parameters")]
        [SerializeField] private float aliveChance = 0.45f;
        [SerializeField] private int width = 50;
        [SerializeField] private int height = 50;

        [Space] [Header("Assign in inspector")] 
        [SerializeField] private Tilemap tilemapForeground;
        [SerializeField] private Tilemap tilemapBackground;
        [SerializeField] private Tile deadTile;
        [SerializeField] private Tile aliveTile;

        private List<Cell> cellMap = new List<Cell>();
        
        private void Start()
        {
            InitializeMap();
        }
        
        
        
        private void Update()
        {
            if (regenerateMapButton)
            {
                ClearMap();
                InitializeMap();
                regenerateMapButton = false;
            }
        }
        
        
        
        private void ClearMap()
        {
            cellMap.Clear();
            tilemapForeground.ClearAllTiles();
            tilemapBackground.ClearAllTiles();
        }
        
        
        
        private void InitializeMap()
        {
            // Loop over X and Y to get the total number of cells
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    AddCell(new Vector2Int(x, y));
                }
            }

            InstantiateMap();
        }
        
        
        
        // Responsible for adding cell data to the list
        private void AddCell(Vector2Int position)
        {
            CellState.State initialCellState;

            // Determine if starting dead or alive initially
            if (Random.Range(0.0f, 1.0f) < aliveChance)
                initialCellState = CellState.State.ALIVE;
            else
                initialCellState = CellState.State.DEAD;

            cellMap.Add(new Cell(initialCellState, position));
        }
        
        
        
        // Instantiates tiles based on the cellMap data
        private void InstantiateMap()
        {
            foreach (var cell in cellMap)
            {
                Vector3Int cellPosition = new Vector3Int(cell.position.x, cell.position.y, 0);
                    
                if (cell.cellState == CellState.State.ALIVE)
                    tilemapForeground.SetTile(cellPosition, aliveTile);
                else
                    tilemapBackground.SetTile(cellPosition, deadTile);
            }
        }
    }
}
