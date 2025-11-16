using UnityEngine;
using UnityEngine.UI;

public class UIPanelMain : MonoBehaviour, IMenu
{
    [SerializeField] private Button btnTimerMode;

    [SerializeField] private Button btnNomalMode;

    private UIMainManager m_mngr;

    private void Awake()
    {
        btnNomalMode.onClick.AddListener(OnClickMoves);
        btnTimerMode.onClick.AddListener(OnClickTimer);
    }

    private void OnDestroy()
    {
        if (btnNomalMode) btnNomalMode.onClick.RemoveAllListeners();
        if (btnTimerMode) btnTimerMode.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    private void OnClickTimer()
    {
        m_mngr.LoadLevelTimer();
    }

    private void OnClickMoves()
    {
        m_mngr.LoadLevelMoves();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
