using UnityEngine;
using UnityEngine.UI;

namespace NewGame
{
    public class Gui_GameMode_Normal : MonoBehaviour, IGui
    {
        [SerializeField] private Button btn_Pause;

        public void Setup()
        {
            btn_Pause.onClick.RemoveAllListeners();

            btn_Pause.onClick.AddListener(() => GameManager.Instance.SetState(eGameState.PAUSE) );
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}
