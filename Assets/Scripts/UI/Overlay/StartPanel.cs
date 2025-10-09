using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : UIBase
{
    public override UIType UIType => UIType.Overlay;
    public override string PanelName => "StartPanel";

    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;

    [SerializeField] private GameObject creditsPanel;

    private void Awake()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        settingsButton.onClick.AddListener(() => OnSettingsButtonClicked().Forget());
        creditsButton.onClick.AddListener(OnCreditsButtonClicked);
    }

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);
        Time.timeScale = 0;

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        Time.timeScale = 1;
    }

    private void OnStartButtonClicked()
    {
        GameManager.Instance.StartGame();
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();
    }

    private async UniTaskVoid OnSettingsButtonClicked()
    {
        await UIManager.Instance.OpenPanel("SettingsPanel");
    }

    private void OnCreditsButtonClicked()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(!creditsPanel.activeSelf);
        }
    }
}