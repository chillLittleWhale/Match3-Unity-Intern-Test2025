using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace NewGame
{
    public class BottomBar : MonoBehaviour
    {
        public List<BottomCell> slots = new();
        Dictionary<ItemType, int> typeCount = new();

        public void Create()
        {
            Reset();

            float slotSize_X = GameConfig.CLEAR_SLOT * GameConfig.CELL_SIZE_X;
            for (int i = 0; i < GameConfig.CLEAR_SLOT; i++)
            {
                GameObject newCellGO = Instantiate(GameManager.Instance.bottomCellPrefab);
                newCellGO.name = $"SlotCell_{i}";

                newCellGO.transform.parent = transform;
                newCellGO.transform.position = new Vector3(transform.position.x - slotSize_X / 2 + GameConfig.CELL_SIZE_X * (i + 0.5f), transform.position.y, 0);

                slots.Add(newCellGO.GetComponent<BottomCell>());
            }
        }

        public bool TryAddItem(Item item, Action callback = null)
        {
            BottomCell emptyCell;
            if (HasEmptySlot(out emptyCell))
            {
                emptyCell.Assign(item);
                typeCount[item.Type]++;

                callback?.Invoke();
                CheckCollapMatchItem();
                CheckBottomBarFull();
                return true;
            }
            return false;
        }

        public bool TryRemoveItem(Item item, Action callback = null)
        {
            if (item == null) return false;

            for (int i = slots.Count - 1; i >= 0; i--)
            {
                if (slots[i].item == item)
                {
                    slots[i].Assign(null);
                    item.SetBottomCell(null);
                    typeCount[item.Type]--;
                    item.Cell.AssignItem(item);
                    callback?.Invoke();
                    break;
                }
            }

            return true;
        }

        public void Reset()
        {
            typeCount = new();
            foreach (var type in Enum.GetValues(typeof(ItemType)))
            {
                typeCount.Add((ItemType)type, 0);
            }

            for (int i = slots.Count - 1; i >= 0; i--)
            {
                var item = slots[i].item;
                if (item != null)
                {
                    slots[i].item = null;
                    PoolManager.Instance.Despawn(item.gameObject);
                }

                PoolManager.Instance.Despawn(slots[i].gameObject);
            }

            slots.Clear();
        }

        private bool HasEmptySlot(out BottomCell emptyCell)
        {
            emptyCell = null;
            foreach (var cell in slots)
            {
                if (cell.item == null)
                {
                    emptyCell = cell;
                    return true;
                }
            }
            return false;
        }

        private void CheckCollapMatchItem()
        {
            foreach (var kvp in typeCount)
            {
                if (kvp.Value >= GameConfig.CLEAR_NUM)
                {
                    typeCount[kvp.Key] = 0;

                    var typeToClear = kvp.Key;
                    foreach (var cell in slots)
                    {
                        Item item = cell.item;
                        if (item != null && item.Type == typeToClear)
                        {
                            cell.item = null; // clear item reference ngay lập tức để tránh click lại

                            // Animation scale về 0
                            item.transform.DOScale(Vector3.zero, 0.25f)
                                .SetEase(Ease.InBack)
                                .OnComplete(() =>
                                {
                                    PoolManager.Instance.Despawn(item.gameObject);
                                });
                        }
                    }

                    var itemRemain = GameManager.Instance.GetNumberOfItemRemain();
                    if (itemRemain == 0)
                    {
                        GameManager.Instance.EndGame(true);
                    }

                    Debug.Log($"Match 3 cleared: {kvp.Key}");
                    break;
                }
            }
        }

        private void CheckBottomBarFull()
        {
            foreach (var cell in slots)
            {
                if (cell.item == null)
                {
                    return;
                }
            }

            GameManager.Instance.CurrentGameMode.OnBottomBarFull();
        }
    }
}