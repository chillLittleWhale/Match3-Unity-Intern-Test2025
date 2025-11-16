using UnityEngine;
using UnityEngine.UI;

namespace NewGame
{
    public class Gui_EndGame_Win : MonoBehaviour, IGui
    {
        [SerializeField] private Button btnExit;

        public void Setup()
        {
            btnExit.onClick.RemoveAllListeners();
            btnExit.onClick.AddListener(() => { GameManager.Instance.ReturnToMainMenu(); });
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}
