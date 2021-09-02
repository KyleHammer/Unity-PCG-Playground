using System.Collections.Generic;
using UnityEngine;

namespace Spelunky_Gen_Scripts
{
    public class SetPieceGen : MonoBehaviour
    {
        [Header("SetPieces")]
        [SerializeField] private List<GameObject> setPieces = new List<GameObject>();
        
        private void Start()
        {
            GenerateSetPiece();
        }

        private void GenerateSetPiece()
        {
            Instantiate(setPieces[Random.Range(0, setPieces.Count)], transform.position, Quaternion.identity, gameObject.transform.parent);
            Destroy(gameObject);
        }
    }
}
