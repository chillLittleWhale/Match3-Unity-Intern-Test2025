using UnityEngine;

namespace NewGame
{
    public class Board : MonoBehaviour
    {
        [Header("Config & References")]
        [SerializeField] Grid grid;
        [SerializeField] Transform root;
        public Transform cellContainer;
        public Transform itemContainer;

        public int layer;
        public Vector2Int size;
        public Cell[,] cells;


        public void Create(BoardConfig config, int layer)
        {
            grid.cellSize = new Vector2(GameConfig.CELL_SIZE_X, GameConfig.CELL_SIZE_Y);

            this.layer = layer;
            this.size = config.Size;
            this.cells = new Cell[size.x, size.y];
            root.position = config.Origin;

            root.position = new Vector3(root.position.x, root.position.y, -layer); // unity raycast ưu tiên z thấp

            SpawnGrid();
        }

        public void Clear()
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    cells[x, y].gameObject.Despawn();
                }
            }

            for (int i = itemContainer.childCount - 1; i >= 0; i--)
            {
                var child = itemContainer.GetChild(i);
                PoolManager.Instance.Despawn(child.gameObject);
            }
        }

        public int GetNumberOfItemRemain()
        {
            int count = 0;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    if (cells[x, y].Item is not null)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void SpawnGrid()
        {
            GameObject cellPrefab = GameManager.Instance.cellPrefab;
            if (cellPrefab is null)
            {
                Debug.LogError("Cell Prefab chưa được gán!");
                return;
            }

            // Xóa  cell cũ
            foreach (Transform child in cellContainer)
            {
                Destroy(child.gameObject);
            }

            // Tạo grid cell mới
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    // Tính toán vị trí cell, offset 50% để lấy giá trị trung tâm của cell
                    Vector3 position = grid.transform.position.Add(x: (x + 0.5f) * GameConfig.CELL_SIZE_X, y: (y + 0.5f) * GameConfig.CELL_SIZE_Y);

                    // Spawn grid cell
                    // GameObject newCellGO = Instantiate(cellPrefab, position, Quaternion.identity, cellContainer);
                    GameObject newCellGO = PoolManager.Instance.Spawn(cellPrefab, position, cellContainer);
                    newCellGO.name = $"Cell_{x}_{y}";

                    Cell newCell = newCellGO.GetComponent<Cell>();
                    newCell.Init(x, y, layer, root.position);
                    cells[x, y] = newCell;
                }
            }

            //set neighbours
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    if (x + 1 < size.x)
                    {
                        cells[x, y].NeighboursDict[Dir.Right] = cells[x + 1, y];
                    }

                    if (x > 0)
                    {
                        cells[x, y].NeighboursDict[Dir.Left] = cells[x - 1, y];
                    }

                    if (y + 1 < size.y)
                    {
                        cells[x, y].NeighboursDict[Dir.Up] = cells[x, y + 1];

                        if (x + 1 < size.x)
                        {
                            cells[x, y].NeighboursDict[Dir.UpRight] = cells[x + 1, y + 1];
                        }

                        if (x > 0)
                        {
                            cells[x, y].NeighboursDict[Dir.UpLeft] = cells[x - 1, y + 1];
                        }
                    }

                    if (y > 0)
                    {
                        cells[x, y].NeighboursDict[Dir.Down] = cells[x, y - 1];

                        if (x + 1 < size.x)
                        {
                            cells[x, y].NeighboursDict[Dir.DownRight] = cells[x + 1, y - 1];
                        }

                        if (x > 0)
                        {
                            cells[x, y].NeighboursDict[Dir.DownLeft] = cells[x - 1, y - 1];
                        }
                    }
                }
            }
        }
    }
}
