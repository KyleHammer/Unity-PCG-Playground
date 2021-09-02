using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spelunky_Gen_Scripts
{
    // TODO: If you were to continue using this PCG formula consider the following
    /// <summary>
    /// - Use less expensive operations than GetComponent in loops
    /// - Make GetTileFromCoordinate more efficient (i.e. use a dictionary)
    /// - Use Unity editor tools instead of the update loop
    /// </summary>
    
    public class SpelunkyMapGen : MonoBehaviour
    {
        private List<GameObject> tiles = new List<GameObject>();
        private GameObject startTile;
        private GameObject goalTile;

        private List<GameObject> tilesOnPath = new List<GameObject>();

        [Header("Tools")]
        [SerializeField] private bool refreshTiles = false;
        
        [Header("Materials")]
        [SerializeField] private Material basicMaterial;
        [SerializeField] private Material greenMaterial;
        [SerializeField] private Material yellowMaterial;
        
        [Header("Map Generation Size")]
        [SerializeField] private int rowCount = 4;
        [SerializeField] private int columnCount = 4;
        
        [Header("Tile Prefab")]
        [SerializeField] private GameObject spelunkyTile;

        private void Start()
        {
            GenerateTiles();
        }

        #region Inpector Tools

        private void Update()
        {
            if (refreshTiles)
            {
                GenerateTiles();
                refreshTiles = false;
            }
        }

        #endregion

        #region Tile Generation and Assignment

        private void GenerateTiles()
        {
            GenerateBasicTiles();
            AssignStartTiles();
            GeneratePath();
        }

        private void GenerateBasicTiles()
        {
            // Clear out previous tiles
            foreach (var tile in tiles)
            {
                Destroy(tile);
            }
            tiles.Clear();
            
            // Generate new tiles
            for (int x = 0; x < columnCount; x++)
            {
                for (int y = 0; y < rowCount; y++)
                {
                    tiles.Add(Instantiate(spelunkyTile, new Vector3(x, y), Quaternion.identity));
                }
            }
        }

        private void AssignStartTiles()
        {
            // Reset materials if goal and start have already been assigned
            if (startTile != null || goalTile != null)
            {
                startTile.GetComponent<Renderer>().material = basicMaterial;
                goalTile.GetComponent<Renderer>().material = basicMaterial;
            }
            
            // Get top row tiles
            List<GameObject> topRowTiles = new List<GameObject>();

            foreach (var tile in tiles)
            {
                if( (int) tile.transform.position.y == rowCount - 1)
                    topRowTiles.Add(tile);
            }

            // Assign start and goal tiles randomly
            startTile = topRowTiles[Random.Range(0, topRowTiles.Count)];
            startTile.GetComponent<Renderer>().material = greenMaterial;
        }
        
        #endregion

        #region Path Generation

        private void GeneratePath()
        {
            tilesOnPath.Clear();

            tilesOnPath.Add(startTile);
            
            GameObject pathTile = GenerateNextTile(startTile);
            
            while (pathTile != null)
            {
                tilesOnPath.Add(pathTile);
                pathTile.GetComponent<Renderer>().material = yellowMaterial;

                pathTile = GenerateNextTile(pathTile);
            }

            goalTile = tilesOnPath[tilesOnPath.Count - 1];
            goalTile.GetComponent<Renderer>().material = greenMaterial;
        }

        private GameObject GenerateNextTile(GameObject currentTile)
        {
            int generationDecision = Random.Range(0, 3);

            switch (generationDecision)
            {
                case 0:
                    return GenerateLeft(currentTile);
                case 1:
                    return GenerateRight(currentTile);
                case 2:
                    return GenerateDown(currentTile);
                default:
                    Debug.LogError("Path generation uses random number "+generationDecision+" which is out of bounds");
                    return null;
            }
        }

        private GameObject GenerateLeft(GameObject tile)
        {
            GameObject nextTile = GetTileFromCoordinate(tile.transform.position + Vector3.left);

            if (tilesOnPath.Contains(nextTile))
                GenerateRight(tile);
            else if (nextTile != null)
                return nextTile;
            
            return GenerateDown(tile);
        }
        private GameObject GenerateRight(GameObject tile)
        {
            GameObject nextTile = GetTileFromCoordinate(tile.transform.position + Vector3.right);

            if (tilesOnPath.Contains(nextTile))
                GenerateLeft(tile);
            else if (nextTile != null)
                return nextTile;
            
            return GenerateDown(tile);
        }
        
        private GameObject GenerateDown(GameObject tile)
        {
            GameObject nextTile = GetTileFromCoordinate(tile.transform.position + Vector3.down);

            if (nextTile != null)
                return nextTile;
            
            return null;
        }

        /// <summary>
        /// Takes in a coordinate and returns the tile at the coordinate
        /// </summary>
        /// <param name="tileCoordinate"></param>
        /// <returns> GameObject Tile if it exists, null if it does not exist</returns>
        private GameObject GetTileFromCoordinate(Vector3 tileCoordinate)
        {
            // TODO: Make this more efficient (i.e. use a dictionary)
            foreach (var tile in tiles)
            {
                if (tile.transform.position == tileCoordinate)
                    return tile;
            }

            return null;
        }

        #endregion
    }
}
