using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    public Sprite[] ArrowSprites;
    private Image _image;

    [HideInInspector]
    public int ArrowDir;

    public Color FinishColor;  
    public Color ErrorColor;
    public Color DefaultColor = Color.white;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Setup(int dir)
    {
        ResetState();
        ArrowDir = dir;
        if (ArrowSprites != null && dir >= 0 && dir < ArrowSprites.Length)
        {
            _image.sprite = ArrowSprites[dir];
            _image.SetNativeSize();
        }
    }

    private void ResetState()
    {
        _image.color = DefaultColor;

        _image.sprite = null;
    }
    public void SetToFinishState()
    {
        _image.color = FinishColor;
    }

    public void SetToErrorState()
    {
        _image.color = ErrorColor;
    }
}