using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    public Sprite[] ArrowSprites;
    private Image m_image;

    [HideInInspector]
    public int arrowDir;

    public Color FinishColor;  
    public Color ErrorColor;
    public Color DefaultColor = Color.white;

    void Awake()
    {
        m_image = GetComponent<Image>();
    }

    public void Setup(int dir)
    {
        ResetState();
        arrowDir = dir;
        if (ArrowSprites != null && dir >= 0 && dir < ArrowSprites.Length)
        {
            m_image.sprite = ArrowSprites[dir];
            m_image.SetNativeSize();
        }
    }

    private void ResetState()
    {
        m_image.color = DefaultColor;

        m_image.sprite = null;
    }
    public void SetToFinishState()
    {
        m_image.color = FinishColor;
    }

    public void SetToErrorState()
    {
        m_image.color = ErrorColor;
    }
}