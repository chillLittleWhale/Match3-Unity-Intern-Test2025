using UnityEngine;
using UnityEngine.UI;

namespace NewGame
{
    public class Gui_GameMode_Timer : MonoBehaviour, IGui
    {
        [SerializeField] private Button btn_Pause;
        [SerializeField] private Text txt_Timer;

        public void Setup()
        {
            btn_Pause.onClick.RemoveAllListeners();

            btn_Pause.onClick.AddListener(() => GameManager.Instance.SetState(eGameState.PAUSE) );

            GameManager.Instance.TimerChangeEvent += OnTimerChange;
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

        private void OnTimerChange(int time) => txt_Timer.text = (GameConfig.TIME_LIMIT - time).ToString();
    }
}
