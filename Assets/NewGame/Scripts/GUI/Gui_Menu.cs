using UnityEngine;
using UnityEngine.UI;

namespace NewGame
{
    public class Gui_Menu : MonoBehaviour, IGui
    {
        [SerializeField] private Button btnMode_Normal;
        [SerializeField] private Button btnMode_Timer;

        public void Setup()
        {
            btnMode_Normal.onClick.RemoveAllListeners();
            btnMode_Timer.onClick.RemoveAllListeners();

            btnMode_Normal.onClick.AddListener(() => GameManager.Instance.EnterLevel(eGameMode.Normal) );
            btnMode_Timer.onClick.AddListener(()  => GameManager.Instance.EnterLevel(eGameMode.Timer) );
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}
