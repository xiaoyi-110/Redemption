using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ProcedureLaunch : ProcedureBase
{
    private Dictionary<string, bool> m_LoadFlags;

    // Called once when this procedure is initialized
    public override void OnInit(FSM<ProcedureManager> fsm)
    {
        base.OnInit(fsm);
        m_LoadFlags = new Dictionary<string, bool>();
    }

    // Called when entering this procedure
    public override void OnEnter(FSM<ProcedureManager> fsm)
    {
        base.OnEnter(fsm);

        m_LoadFlags.Clear();
        UIManager.Instance.OpenPanel("StartPanel").Forget();
        //Debug.Log("[ProcedureLaunch] Start loading initial assets...");
        AssetManager.Instance.LoadInitialAssetsAsync(OnAssetsLoaded);
    }

    // Called every frame while this procedure is active
    public override void OnUpdate(FSM<ProcedureManager> fsm)
    {
        // No update logic needed
    }

    // Callback when initial assets are loaded
    private void OnAssetsLoaded()
    {
        bool allGood = true;
        SoundDatabase loadedDatabase = AssetManager.Instance.LoadedSoundDatabase;
        if (loadedDatabase != null)
        {
            AudioManager.Instance.SetSoundDatabase(loadedDatabase);
            //Debug.Log("[ProcedureLaunch] Assets loaded successfully. Switching to Menu Procedure.");
            UIManager.Instance.OpenPanel("StartPanel").Forget();
            fsm.ChangeState<ProcedureMenu>();
        }
        else
        {
            Debug.LogError("[ProcedureLaunch] Failed to load sound database!");
        allGood = false;
        }

        TextAsset gameMessagesJson = AssetManager.Instance.LoadedGameMessagesJson;
        if (gameMessagesJson != null)
        {
            // 反序列化JSON到GameMessages类
            GameMessages messages = JsonUtility.FromJson<GameMessages>(gameMessagesJson.text);
            if (messages != null)
            {
                GameManager.Instance.Messages = messages;
            }
            else
            {
                Debug.LogError("[ProcedureLaunch] Failed to parse GameMessages JSON!");
                allGood = false;
            }
        }
        else
        {
            Debug.LogError("[ProcedureLaunch] Failed to load GameMessages JSON!");
            allGood = false;
        }

        if (allGood)
        {
            DialogManager.Instance.LoadDialogueFromCSV("Assets/Resources/DataTable/dialog.csv");
            //UIManager.Instance.OpenPanel("StartPanel").Forget();
            fsm.ChangeState<ProcedureMenu>();
        }
        else
        {
            Debug.LogError("[ProcedureLaunch] Initialization failed, not switching state.");
        }
    }

    // Factory method
    public static ProcedureLaunch Create()
    {
        return new ProcedureLaunch();
    }
}
