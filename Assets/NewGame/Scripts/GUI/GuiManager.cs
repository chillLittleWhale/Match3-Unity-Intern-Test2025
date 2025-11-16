using System.Collections;
using System.Collections.Generic;
using NTWUtils;
using UnityEngine;

namespace NewGame
{
    public class GuiManager : PersistentSingleton<GuiManager>
    {
        private IGui[] guiList;

        protected override void Awake()
        {
            base.Awake();

            guiList = GetComponentsInChildren<IGui>(true);
        }

        void Start()
        {
            GameManager.Instance.StateChangeEvent += OnGameStateChange;
            for (int i = 0; i < guiList.Length; i++)
            {
                guiList[i].Setup();
            }
        }

        private void OnGameStateChange(eGameState state)
        {
            switch (state)
            {
                case eGameState.MAIN_MENU:
                    HideAllPanel();
                    ShowPanel<Gui_Menu>();
                    break;
                case eGameState.GAME_STARTED:
                    HideAllPanel();
                    switch (GameManager.Instance.CurrentGameMode.ModeType)
                    {
                        case eGameMode.Normal:
                            ShowPanel<Gui_GameMode_Normal>();
                            break;
                        case eGameMode.Timer:
                            ShowPanel<Gui_GameMode_Timer>();
                            break;
                    }
                    break;
                case eGameState.PAUSE:
                    ShowPanel<Gui_Pause>();
                    break;
                case eGameState.GAME_OVER:
                    HideAllPanel();
                    switch (GameManager.Instance.IsGameWin)
                    {
                        case true:
                            ShowPanel<Gui_EndGame_Win>();
                            break;
                        case false:
                            ShowPanel<Gui_EndGame_Lose>();
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void HideAllPanel()
        {
            for (int i = 0; i < guiList.Length; i++)
            {
                guiList[i].Hide();
            }
        }

        private void ShowPanel<T>() where T : IGui
        {
            for (int i = 0; i < guiList.Length; i++)
            {
                if (guiList[i] is T)
                {
                    guiList[i].Show();
                    break;
                }
            }
        }

        private void HidePanel<T>() where T : IGui
        {
            for (int i = 0; i < guiList.Length; i++)
            {
                if (guiList[i] is T)
                {
                    guiList[i].Hide();
                    break;
                }
            }
        }
    }
}
