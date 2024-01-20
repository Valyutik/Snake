using UnityEngine;

namespace PlayForge_Team.Snake.Runtime.Snakes
{
    public sealed class SnakePart : MonoBehaviour
    {
        private Vector2Int _cellId;

        public void SetCellPosition(Vector2Int cellId, Vector2 position)
        {
            _cellId = cellId;
            transform.position = position;
        }

        public Vector2Int GetCellId()
        {
            return _cellId;
        }
    }
}