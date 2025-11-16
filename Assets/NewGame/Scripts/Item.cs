using UnityEngine;

namespace NewGame
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        public ItemType Type { get; private set; }
        public Cell Cell { get; private set; }
        public BottomCell BottomCell { get; private set; }

        public bool IsOnBoardCell = true;

        public void Init(ItemType type)
        {
            Type = type;
            spriteRenderer.sprite = GameManager.Instance.GetItemSprite(type);
        }

        public void SetCell(Cell cell)
        {
            Cell = cell;
            if (cell != null)
            {
                IsOnBoardCell = true;
            }
            else
            {
                Debug.LogError("Item set null cell, this should not happen!");
            }
        }

        public void SetBottomCell(BottomCell bottomCell)
        {
            BottomCell = bottomCell;
            if (bottomCell != null)
            {
                IsOnBoardCell = false;
            }
        }

        public void OnMouseDown()
        {
            if (IsOnBoardCell)
            {
                if (Cell.IsBlocked) return;

                GameManager.Instance.PutItemToBottomBar(this, () => { Cell.FreeCell(); });
            }
            else // on bottom bar
            {
                if (GameManager.Instance.ItemCanBePutBack())
                {
                    GameManager.Instance.ReturnItemToBoard(this, () =>
                    {
                        Cell.RefreshLockState();
                        foreach (var cell in Cell.BlockingCells)
                        {
                            cell.RefreshLockState();
                        }
                    });
                }
            }
        }

        public void SetRenderOrder(int order) => spriteRenderer.sortingOrder = order;
    }

    public enum ItemType
    {
        Normal01,
        Normal02,
        Normal03,
        Normal04,
        Normal05,
        Normal06,
        Normal07
    }
}

