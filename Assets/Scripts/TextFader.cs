using UnityEngine;
using TMPro;

public class TextFader : MonoBehaviour
{
    public TextMeshProUGUI text; // 挂载要闪烁的文本
    public float fadeSpeed = 1.2f; // 控制闪烁速度

    private float alpha = 1f;
    private bool fadingOut = true;

    void Update()
    {
        if (fadingOut)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            if (alpha <= 0f)
            {
                alpha = 0f;
                fadingOut = false;
            }
        }
        else
        {
            alpha += Time.deltaTime * fadeSpeed;
            if (alpha >= 1f)
            {
                alpha = 1f;
                fadingOut = true;
            }
        }

        Color currentColor = text.color;
        currentColor.a = alpha;
        text.color = currentColor;
    }
}

