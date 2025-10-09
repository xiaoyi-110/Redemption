using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{


    void Update()
    {
        if (ArrowManager.Instance.IsInWave() == false)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            ArrowManager.Instance.TypeArrow(KeyCode.UpArrow);

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            ArrowManager.Instance.TypeArrow(KeyCode.DownArrow);

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            ArrowManager.Instance.TypeArrow(KeyCode.LeftArrow);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ArrowManager.Instance.TypeArrow(KeyCode.RightArrow);

        // 如果玩家按下回车键，表示结束当前关卡
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ArrowManager.Instance.ForceFinish();   // 完成关卡

        }
    }
}