using System;
using System.Collections.Generic;
using UnityEngine;

namespace NewGame
{
    [Serializable]
    public class Cell : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] SpriteRenderer lockSr;
        public int Layer { get; private set; }
        public Item Item
        {
            get => item;
            set
            {
                item = value;
                spriteRenderer.enabled = value != null;
            }
        }
        public bool IsBlocked;
        public Vector2Int BoardIndex { get; private set; }
        public Vector2 BoardOrigin { get; private set; }
        public Dictionary<Dir, Cell> NeighboursDict { get; set; } = new Dictionary<Dir, Cell>();
        public List<Cell> BlockedByCells { get; set; } = new List<Cell>(); // list of cells that block this cell
        public List<Cell> BlockingCells { get; set; } = new List<Cell>(); // list of cells that this cell blocks

        private Item item;


        public void Init(int x, int y, int layer, Vector2 origin)
        {
            BlockedByCells.Clear();
            BlockingCells.Clear();
            NeighboursDict.Clear();
            item = null;

            spriteRenderer.enabled = false;
            lockSr.enabled = false;

            BoardIndex = new Vector2Int(x, y);
            BoardOrigin = origin;
            Layer = layer;
            spriteRenderer.sortingOrder = layer * 10;
            lockSr.sortingOrder = layer * 10 + 2;
        }

        public void AssignItem(Item item)
        {
            Item = item;
            item.transform.position = transform.position;
            item.SetRenderOrder(Layer * 10 + 1);
            item.SetCell(this);
        }

        public void FreeCell()
        {
            item.IsOnBoardCell = false;
            Item = null;
            spriteRenderer.enabled = false;
            lockSr.enabled = false;

            foreach (var cell in BlockingCells)
            {
                cell.RefreshLockState();
            }
        }

        public bool IsOverlapWith(Cell other)
        {
            // return Math.Abs(transform.position.x - other.transform.position.x) < GameConfig.CELL_SIZE_X
            // && Math.Abs(transform.position.y - other.transform.position.y) < GameConfig.CELL_SIZE_Y;

            Vector2 cellPos = BoardOrigin + new Vector2(BoardIndex.x * GameConfig.CELL_SIZE_X, BoardIndex.y * GameConfig.CELL_SIZE_Y);
            Vector2 otherPos = other.BoardOrigin + new Vector2(other.BoardIndex.x * GameConfig.CELL_SIZE_X, other.BoardIndex.y * GameConfig.CELL_SIZE_Y);
            
            return Math.Abs(cellPos.x - otherPos.x) < GameConfig.CELL_SIZE_X
            && Math.Abs(cellPos.y - otherPos.y) < GameConfig.CELL_SIZE_Y;
        }

        public List<Cell> GetAllOverlapCellsWith(Cell other)
        {
            if (!IsOverlapWith(other)) return null;

            List<Cell> result = new() { other };

            foreach (var neighbour in other.NeighboursDict.Values)
            {
                if (IsOverlapWith(neighbour))
                {
                    result.Add(neighbour);
                }
            }

            return result;
        }

        public void AssignOverlapMap(Cell[,] upperCells)
        {
            if (upperCells == null) return;

            // duyệt từng cell trong upperCells, tìm ra cell đầu tiên overlap với this cell
            Cell firstOverlapCell = null;
            for (int x = 0; x < upperCells.GetLength(0); x++)
            {
                for (int y = 0; y < upperCells.GetLength(1); y++)
                {
                    var curUpperCell = upperCells[x, y];
                    if (curUpperCell.Item != null && IsOverlapWith(curUpperCell))
                    {
                        firstOverlapCell = curUpperCell;
                        break;
                    }
                }
            }

            if (firstOverlapCell != null)
            {
                var overlapCells = GetAllOverlapCellsWith(firstOverlapCell);

                BlockedByCells.AddRange(overlapCells);

                foreach (var cell in overlapCells)
                {
                    cell.BlockingCells.Add(this);
                }
            }
        }

        public void RefreshLockState()
        {
            bool isBlocked = false;
            foreach (var cell in BlockedByCells)
            {
                if (cell.Item != null)
                {
                    isBlocked = true;
                    break;
                }
            }

            IsBlocked = isBlocked;

            if (Item != null) lockSr.enabled = IsBlocked;
        }

        // public void OnMouseDown()
        // {
        //     Debug.Log("OnMouseDown");
        //     if (Item == null) return;
            
        //     var item = Item;
        //     Item = null;
        //     GameManager.Instance.OnItemClicked(item);
        // }
    }

    public enum Dir
    {
        Up,
        Down,
        Left,
        Right,
        UpRight,
        UpLeft,
        DownRight,
        DownLeft
    }
}
