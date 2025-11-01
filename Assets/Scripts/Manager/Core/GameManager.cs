using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public bool isGameStarted = false;
    public bool isGameWon = false;
    public bool isGameOver = false;
    public int rescuedNPC = 0;
    public int maxNPC;    
    public GameMessages Messages { get; set; }
    void Start()
    {
       
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    EndGame();
        //}
    }
    public void StartGame()
    {
        UIManager.Instance.OpenPanel("IntroPanelUI", Messages.StartMessage).Forget();
        UIManager.Instance.ClosePanel("StartPanel");
        isGameStarted = true;
        isGameWon = false;
        Time.timeScale = 1;

        Timer.Instance.ResetTimer();
        UIManager.Instance.OpenPanel("EquipmentUI").Forget();
    }

    public void EndGame()
    {
        isGameOver = true;
        isGameStarted = false;
        string message = isGameWon ? Messages.WonMessage : Messages.FailMessage;
        UIManager.Instance.OpenPanel("ResultPanelUI", message).Forget();
        //Time.timeScale = 0;
    }

    public void ResetGameState()
    {
        isGameStarted = false;
        isGameWon = false;
        isGameOver = false;
        rescuedNPC = 0;
        Debug.Log("”Œœ∑◊¥Ã¨“—÷ÿ÷√°£");
    }
}