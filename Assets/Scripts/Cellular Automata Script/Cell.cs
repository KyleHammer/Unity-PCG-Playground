using UnityEngine;

namespace Cellular_Automata_Script
{
    public class Cell
    {
        public CellState.State cellState;
        public Vector2Int position;
        
        public Cell(CellState.State _cellState, Vector2Int _position)
        {
            cellState = _cellState;
            position = _position;
        }
    }
}
