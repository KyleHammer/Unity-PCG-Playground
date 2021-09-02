using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spelunky_Gen_Scripts
{
    public class SpelunkyRoomGen : MonoBehaviour
    {
        [Header("Tools")]
        [SerializeField] private bool refreshTiles = false;
        
        [Header("Maps")]
        [SerializeField] private List<GameObject> maps = new List<GameObject>();
        private GameObject currentMap;

        private void Start()
        {
            GenerateMap();
        }

        private void Update()
        {
            if (refreshTiles)
            {
                GenerateMap();
                refreshTiles = false;
            }
        }

        private void GenerateMap()
        {
            if(currentMap != null)
                Destroy(currentMap);
            
            currentMap = Instantiate(maps[Random.Range(0, maps.Count)], gameObject.transform);
        }
    }
}
