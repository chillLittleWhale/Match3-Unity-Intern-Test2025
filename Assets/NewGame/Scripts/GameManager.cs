using System;
using System.Collections.Generic;
using DG.Tweening;
using NTWUtils;
using UnityEngine;

namespace NewGame
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        [Header("Configs")]
        public int bottomSlot = 5;
        public int itemTypeCount = 5; // số lượng loại item sẽ được tạo trong màn chơi
        [SerializeField] LevelController level;
        [SerializeField] LevelConfig levelConfig;
        [SerializeField] List<TypeSpriteEntry> itemTypeSprites = new();

        [Header("Prefabs")]
        public GameObject cellPrefab;
        public GameObject bottomCellPrefab;
        public GameObject itemPrefab;
        public GameObject boardPrefab;


        public event Action<eGameState> StateChangeEvent = delegate { };
        public event Action<int> TimerChangeEvent = delegate { };

        private float levelPlayedTime = 0;
        private int lastPlayedTimeSend = -1;

        public bool IsGameWin { get; set; }
        public GameModeBase CurrentGameMode;
        private eGameState m_state;
        public eGameState State
        {
            get { return m_state; }
            private set
            {
                m_state = value;

                StateChangeEvent?.Invoke(m_state);
            }
        }

        public void SetState(eGameState state)
        {
            State = state;

            if (State == eGameState.PAUSE)
            {
                DOTween.PauseAll();
            }
            else
            {
                DOTween.PlayAll();
            }
        }

        public Sprite GetItemSprite(ItemType itemType) => itemTypeSprites.Find(x => x.type == itemType).sprite;

        public void EndGame(bool isWin)
        {
            IsGameWin = isWin;

            SetState(eGameState.GAME_OVER);

            DOTween.PauseAll();

            if (isWin)
            {
                Debug.Log("---Win---");
            }
            else
            {
                Debug.Log("---Lose---");
            }
        }
        protected override void Awake()
        {
            base.Awake();
            SetState(eGameState.SETUP);
        }

        void Start()
        {
            SetState(eGameState.MAIN_MENU);
        }

        void Update()
        {
            if (State == eGameState.GAME_STARTED)
            {
                levelPlayedTime += Time.deltaTime;

                if (lastPlayedTimeSend != (int)levelPlayedTime)
                {
                    lastPlayedTimeSend = (int)levelPlayedTime;
                    TimerChangeEvent?.Invoke((int)levelPlayedTime);

                    if (levelPlayedTime >= GameConfig.TIME_LIMIT)
                    {
                        CurrentGameMode.OnTimeRunout();
                    }
                }
            }
        }

        public void EnterLevel(eGameMode mode)
        {
            levelPlayedTime = 0;

            if (mode == eGameMode.Normal)
            {
                CurrentGameMode = new GameMode_Normal();
            }
            else if (mode == eGameMode.Timer)
            {
                CurrentGameMode = new GameMode_Timer();
            }

            level.Generate(levelConfig);
            SetState(eGameState.GAME_STARTED);
        }

        public void ReturnToMainMenu() => SetState(eGameState.MAIN_MENU);

        public void PutItemToBottomBar(Item item, Action callback = null)
        {
            level?.PutItemToBottomBar(item, callback);
        }

        public void ReturnItemToBoard(Item item, Action callback = null)
        {
            level?.ReturnItemToBoard(item, callback);
        }

        public bool ItemCanBePutBack() => CurrentGameMode.CanReturnItemToBoard();

        public int GetNumberOfItemRemain() => level.GetNumberOfItemRemain();

    }

    [Serializable]
    public struct TypeSpriteEntry
    {
        public ItemType type;
        public Sprite sprite;
    }

    public enum eGameMode
    {
        Normal,
        Timer
    }

    public enum eGameState
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        GAME_OVER,
    }
}
