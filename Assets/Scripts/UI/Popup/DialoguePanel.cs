using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class DialoguePanel : UIBase
{
    public override UIType UIType => UIType.Popup; // 弹窗层
    public override string PanelName => "DialoguePanel";

    [SerializeField] private Text dialogueText;
    [SerializeField] private Text npcNameText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Transform optionHolder;
    [SerializeField] private GameObject optionButtonPrefab;

    private string[] currentDialogue;
    private int currentLineIndex = 0;

    private void Awake()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.RegisterEvent<OnDialogueCompleteEventArgs>(OnDialogueCompleteHandler);
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.UnRegisterEvent<OnDialogueCompleteEventArgs>(OnDialogueCompleteHandler);
        }
    }
    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);
        nextButton.gameObject.SetActive(true);
        if (args.Length == 2 && args[0] is string[] dialogue && args[1] is string npcName)
        {
            currentDialogue = dialogue;
            npcNameText.text = npcName;
            currentLineIndex = 0;

            ShowNextLine();
        }
        else
        {
            Debug.LogError("DialoguePanel.OnOpen: args is wrong。");
            UIManager.Instance.ClosePanel(PanelName);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        ClearOptions();
        currentDialogue = null;
    }

    private void OnDialogueCompleteHandler(object sender,OnDialogueCompleteEventArgs args)
    {
        DialogManager.Instance.OnDialogueComplete();
    }
    private async UniTaskVoid RefreshLayoutAsync()
    {
        await UniTask.Yield();
        // 强制刷新布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(optionHolder as RectTransform);
        foreach (var fitter in optionHolder.GetComponentsInChildren<ContentSizeFitter>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(fitter.GetComponent<RectTransform>());
        }
    }


    public void OnNextButtonClicked()
    {
        currentLineIndex++;
        if (currentLineIndex < currentDialogue.Length)
        {
            ShowNextLine();
        }
        else
        {
            EventManager.Instance?.TriggerEvent(this, OnDialogueCompleteEventArgs.Create());
        }
    }

    private void ShowNextLine()
    {
        dialogueText.text = currentDialogue[currentLineIndex];
    }

    public void ShowOptions(string[] options)
    {
        ClearOptions();
        nextButton.gameObject.SetActive(false);

        for (int i = 0; i < options.Length; i++)
        {
            GameObject optionButton = Instantiate(optionButtonPrefab, optionHolder);
            Button button = optionButton.GetComponent<Button>();
            button.gameObject.SetActive(true);
            Text buttonText = button.GetComponentInChildren<Text>();

            if (buttonText != null)
            {
                buttonText.text = options[i];
            }

            int optionIndex = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                EventManager.Instance.TriggerEvent(this, OnOptionSelectedEventArgs.Create(optionIndex));
                ClearOptions();
                nextButton.gameObject.SetActive(true);
            });
        }
        RefreshLayoutAsync().Forget();
    }

    private void ClearOptions()
    {
        foreach (Transform child in optionHolder)
        {
            Destroy(child.gameObject);
        }
    }
}