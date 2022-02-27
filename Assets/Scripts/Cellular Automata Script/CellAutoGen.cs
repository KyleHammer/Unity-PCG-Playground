using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Cellular_Automata_Script
{
    public class CellAutoGen : MonoBehaviour
    {
        [Header("PLAY MODE ONLY BUTTONS")]
        [SerializeField] private bool regenerateMapButton = false;
        [SerializeField] private bool doSimulationStepButton = false;
        
        [Space] [Header("Cell Map Parameters")]
        [SerializeField] private float aliveChance = 0.45f; // How dense the initial grid is with living cells
        [SerializeField] private int width = 50;
        [SerializeField] private int height = 50;
        [SerializeField] private int birthLimit = 4; // The number of neighbours that cause a dead cell to become alive
        [SerializeField] private int deathLimit = 3; // The lower neighbour limit at which cells start dying
        [SerializeField] private int overpopLimit = 6; // The upper neighbour limit at which cells start dying
        [SerializeField] private int simulationStepCount = 2; // The number of times we perform the simulation step

        [Space] [Header("Assign in inspector")] 
        [SerializeField] private Tilemap tilemapForeground;
        [SerializeField] private Tilemap tilemapBackground;
        [SerializeField] private Tile deadTile;
        [SerializeField] private Tile aliveTile;
        
        // Used as a cache for if doSimulationStep is used in run time
        private Dictionary<Vector2Int, CellState.State> generatedMap = new Dictionary<Vector2Int, CellState.State>();
        
        private void Start()
        {
            GenerateMap();
        }
        
        
        
        private void Update()
        {
            if (regenerateMapButton)
            {
                ClearMap();
                GenerateMap();
                regenerateMapButton = false;
            }
            
            if (doSimulationStepButton)
            {
                ClearMap();
                generatedMap = DoSimulationStep(generatedMap);
                InstantiateMap(generatedMap);
                doSimulationStepButton = false;
            }
        }
        
        
        
        private void GenerateMap()
        {
            // Create a new map
            Dictionary<Vector2Int, CellState.State> cellMap = new Dictionary<Vector2Int, CellState.State>();
            
            // Set up the map with random values
            InitializeMap(cellMap);
            
            // Run the simulation for a number of steps
            for (int i = 0; i < simulationStepCount; i++)
            {
                cellMap = DoSimulationStep(cellMap);
            }
            
            InstantiateMap(cellMap);

            generatedMap = cellMap;
        }
        
        
        
        private void InitializeMap(Dictionary<Vector2Int, CellState.State> map)
        {
            // Loop over X and Y to get the total number of cells
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    AddCell(map, new Vector2Int(x, y));
                }
            }
        }
        
        
        
        private void ClearMap()
        {
            tilemapForeground.ClearAllTiles();
            tilemapBackground.ClearAllTiles();
        }
        
        
        
        // Instantiates tiles based on the cellMap data
        private void InstantiateMap(Dictionary<Vector2Int, CellState.State> map)
        {
            foreach (var cell in map)
            {
                Vector3Int cellPosition = new Vector3Int(cell.Key.x, cell.Key.y, 0);
                    
                if (cell.Value == CellState.State.ALIVE)
                    tilemapForeground.SetTile(cellPosition, aliveTile);
                else
                    tilemapBackground.SetTile(cellPosition, deadTile);
            }
        }
        
        
        // Responsible for adding cell data to the list
        private void AddCell(Dictionary<Vector2Int, CellState.State> map, Vector2Int position)
        {
            CellState.State initialCellState;

            // Determine if starting dead or alive initially
            if (Random.Range(0.0f, 1.0f) < aliveChance)
                initialCellState = CellState.State.ALIVE;
            else
                initialCellState = CellState.State.DEAD;

            map.Add(position, initialCellState);
        }
        
        
        
        private Dictionary<Vector2Int, CellState.State> DoSimulationStep(Dictionary<Vector2Int, CellState.State> oldMap)
        {
            Dictionary<Vector2Int, CellState.State> newCellMap = new Dictionary<Vector2Int, CellState.State>();
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int neighbourCount = CountAliveNeighbours(oldMap, x, y);
                    
                    // The new value is based on our simulation rules
                    // First, if a cell is alive...
                    if(GetCellState(oldMap, x, y) == CellState.State.ALIVE)
                    {
                        // But has too few neighbours OR is at/over the population limit, kill it.
                        if(neighbourCount < deathLimit || neighbourCount >= overpopLimit)
                            newCellMap.Add(new Vector2Int(x, y), CellState.State.DEAD);
                        else
                            newCellMap.Add(new Vector2Int(x, y), CellState.State.ALIVE);
                    }
                    // Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                    else
                    {
                        if(neighbourCount > birthLimit)
                            newCellMap.Add(new Vector2Int(x, y), CellState.State.ALIVE);
                        else
                            newCellMap.Add(new Vector2Int(x, y), CellState.State.DEAD);
                    }
                }
            }

            return newCellMap;
        }



        private int CountAliveNeighbours(Dictionary<Vector2Int, CellState.State> map, int x, int y)
        {
            int count = 0;
            
            // The i and j starting and ending values are used to get the neighbours around a cell
            // X X X
            // X O X
            // X X X
            for(int i=-1; i<2; i++)
            {
                for(int j=-1; j<2; j++)
                {
                    int neighbourX = x+i;
                    int neighbourY = y+j;
                    
                    //If we're looking at the middle point
                    if(i == 0 && j == 0)
                    {
                        //Do nothing, we don't want to add ourselves in!
                        continue;
                    }
                    
                    //In case the index we're looking at it off the edge of the map
                    if(neighbourX < 0 || neighbourY < 0 || neighbourX >= width || neighbourY >= height)
                    {
                        count = count + 1;
                    }
                    //Otherwise, a normal check of the neighbour
                    else
                    {
                        if(GetCellState(map, neighbourX, neighbourY) == CellState.State.ALIVE)
                            count = count + 1;
                    }
                }
            }

            return count;
        }
        
        
        
        private CellState.State GetCellState(Dictionary<Vector2Int, CellState.State> map, int positionX, int positionY)
        {
            map.TryGetValue(new Vector2Int(positionX, positionY), out var cellState);
            
            return cellState;
        }
    }
}
