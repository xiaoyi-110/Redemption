using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : UIBase
{
    public override UIType UIType => UIType.Overlay; 
    public override string PanelName => "SettingsPanel";

    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button closeButton;

    [SerializeField] private Button showTipButton;
    [SerializeField] private Button backToMenuButton;

    private void Awake()
    {
        bgmToggle.onValueChanged.AddListener(OnBGMToggle);
        volumeSlider.onValueChanged.AddListener(OnVolumeChange);
        closeButton?.onClick.AddListener(() => UIManager.Instance.ClosePanel(PanelName));
        showTipButton?.onClick.AddListener(()=>ShowTutorialTip().Forget());
        backToMenuButton?.onClick.AddListener(BackToMainMenu);
    }

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);
        Time.timeScale = 0;
    }

    public override void OnClose()
    {
        base.OnClose();
        Time.timeScale = 1; 
    }

    private void OnBGMToggle(bool isOn)
    {
        AudioManager.Instance.SetBGMState(isOn);
    }

    private void OnVolumeChange(float volume)
    {
        AudioManager.Instance.SetVolume(volume);
    }
    private async UniTaskVoid ShowTutorialTip()
    {
        // 确保先关闭设置面板，再显示提示面板，避免UI层级问题
        UIManager.Instance.ClosePanel(PanelName);
        await UIManager.Instance.OpenPanel("TipPanel", "按下Q键将物品收回背包，在背包可选择物品按下F键进行装备。");
    }
    private void BackToMainMenu()
    {
        UIManager.Instance.OnBackToMenu();
    }
    public void ToggleSettings()
    {
        if (gameObject.activeSelf)
        {
            UIManager.Instance.ClosePanel(PanelName);
        }
        else
        {
            UIManager.Instance.OpenPanel(PanelName).Forget();
        }
    }
}