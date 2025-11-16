using UnityEngine;
using UnityEngine.UI;

namespace NewGame
{
    public class Gui_Pause : MonoBehaviour, IGui
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnMenu;

        public void Setup()
        {
            btnClose.onClick.RemoveAllListeners();
            btnMenu.onClick.RemoveAllListeners();

            btnClose.onClick.AddListener(() => GameManager.Instance.SetState(eGameState.GAME_STARTED));
            btnMenu.onClick.AddListener(()  => GameManager.Instance.ReturnToMainMenu());
        }

        public void Hide() => gameObject.SetActive(false);
        public void Show() => gameObject.SetActive(true);
    }
}
