using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NewGame
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] List<Board> boards = new();
        [SerializeField] BottomBar bottomBar;

        public void Generate(LevelConfig levelConfig)
        {
            // 0.Reset
            Reset();

            // 1. Create bottom bar
            bottomBar.Create();

            // 2.Khởi tạo các board trắng
            for (int i = 0; i < levelConfig.BoardConfigs.Count; i++)
            {
                var boardPrefab = GameManager.Instance.boardPrefab;
                var boardInstance = PoolManager.Instance.Spawn(boardPrefab, levelConfig.BoardConfigs[i].Origin, transform, PoolManager.PoolCategory.Others);
                Board newBoard = boardInstance.GetComponent<Board>();
                newBoard.Create(levelConfig.BoardConfigs[i], i);
                boards.Add(newBoard);
            }

            // 3.Fill boards
            int neededItemCount = 0;
            for (int i = 0; i < levelConfig.BoardConfigs.Count; i++)
            {
                neededItemCount += levelConfig.BoardConfigs[i].Size.x * levelConfig.BoardConfigs[i].Size.y;
            }
            neededItemCount -= neededItemCount % GameConfig.CLEAR_NUM; // đảm bảo số lượng tile chia hết cho 3

            int matchCount = neededItemCount / GameConfig.CLEAR_NUM; // số lượng match 3

            int itemTypeCount = Math.Min(matchCount, GameManager.Instance.itemTypeCount);
            List<ItemType> allItemTypes = Enum.GetValues(typeof(NormalItem.eNormalType)).Cast<ItemType>().ToList();
            List<ItemType> selectedItemTypes = (List<ItemType>)allItemTypes.GetRandoms(itemTypeCount).Shuffle();

            // Chia đều số lượng item theo các loại
            List<ItemType> itemTypePool = new();
            List<int> chunks = GetDivinedChunk(itemTypeCount, neededItemCount, GameConfig.CLEAR_NUM);

            for (int i = 0; i < chunks.Count; i++)
            {
                for (int j = 0; j < chunks[i]; j++)
                {
                    itemTypePool.Add(selectedItemTypes[i]);
                }
            }

            itemTypePool.Shuffle();
            FillBoards(itemTypePool);

            // 4. Create overlap map

            for (int i = 0; i < boards.Count - 1; i++) // không cần tạo overlap map cho board cao nhất
            {
                var board = boards[i];
                // xét từng cell trong board hiện tại
                for (int x = 0; x < board.size.x; x++)
                {
                    for (int y = 0; y < board.size.y; y++)
                    {
                        var cell = board.cells[x, y];
                        for (int j = i + 1; j < boards.Count; j++) // xét từng board phía trên board hiện tại
                        {
                            var upperBoard = boards[j];
                            cell.AssignOverlapMap(upperBoard.cells);
                        }
                    }
                }
            }

            // 5. Refesh lock state for all cell
            for (int i = 0; i < boards.Count; i++)
            {
                var board = boards[i];
                for (int x = 0; x < board.size.x; x++)
                {
                    for (int y = 0; y < board.size.y; y++)
                    {
                        board.cells[x, y].RefreshLockState();
                    }
                }
            }
        }

        public void Reset()
        {
            // 1.Reset bottom bar
            bottomBar.Reset();

            for (int i = boards.Count - 1; i >= 0; i--)
            {
                boards[i].Clear();
                PoolManager.Instance.Despawn(boards[i].gameObject);
            }


            boards.Clear();
        }

        public int GetNumberOfItemRemain()
        {
            int count = 0;
            for (int i = 0; i < boards.Count; i++)
            {
                count += boards[i].GetNumberOfItemRemain();
            }
            return count;
        }

        private void FillBoards(List<ItemType> itemTypePool)
        {
            int count = itemTypePool.Count;

            for (int i = 0; i < boards.Count; i++)
            {
                var board = boards[i];
                for (int x = 0; x < board.size.x; x++)
                {
                    for (int y = 0; y < board.size.y; y++)
                    {
                        if (count > 0) // có thể sẽ không fill hết board cell do miss config nên cần có phần check này
                        {
                            var itemGo = PoolManager.Instance.Spawn(GameManager.Instance.itemPrefab, parent: board.itemContainer);
                            var item = itemGo.GetComponent<Item>();
                            if (item is not null)
                            {
                                item.Init(itemTypePool[count - 1]);

                                board.cells[x, y].AssignItem(item);
                                // item.transform.position = board.cells[x, y].transform.position;
                            }
                            count--;
                        }
                        else break;
                    }
                }
            }
        }

        private List<int> GetDivinedChunk(int chunkCount, int number, int chunkMultiple)
        {
            // vd: chunkCount = 3, number = 12, chunkMultiple = 3
            List<int> chunks = new List<int>();

            // Tính giá trị cơ bản mỗi chunk sẽ có
            int baseValue = number / chunkCount; // baseValue = 4
            baseValue -= baseValue % chunkMultiple; // baseValue = 4 - 1 = 3

            // Tính phần dư còn lại
            int remainder = number - baseValue * chunkCount; // remainder = 12 - 3 * 3 = 3
            int chunkBoostTime = remainder / chunkMultiple; // số lần thêm 1 bộ thừa vào chunks, chunkBoostTime = 1, thừa 1 bộ


            // Tạo các chunk
            for (int i = 0; i < chunkCount; i++)
            {
                if (chunkBoostTime > 0)
                {
                    chunks.Add(baseValue + chunkMultiple);  // Phần dư sẽ cộng thêm 1 bộ 
                    chunkBoostTime--;
                }
                else
                {
                    chunks.Add(baseValue);  // Phần còn lại giữ giá trị cơ bản
                }
            }

            return chunks;
        }

        public void PutItemToBottomBar(Item item, Action callback = null)
        {
            bottomBar.TryAddItem(item, callback);
        }

        public void ReturnItemToBoard(Item item, Action callback = null)
        {
            bottomBar.TryRemoveItem(item, callback);
        }
    }
}
