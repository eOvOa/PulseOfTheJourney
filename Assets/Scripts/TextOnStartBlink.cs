using UnityEngine;
using TMPro;

public class TextOnStartBlink : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float blinkSpeed = 1f;

    private bool shouldBlink = true;
    private float timer;

    void Start()
    {
        SetAlpha(1f); // 默认可见
    }

    void Update()
    {
        if (shouldBlink)
        {
            timer += Time.deltaTime;
            float alpha = (Mathf.Sin(timer * Mathf.PI / blinkSpeed) + 1f) / 2f;
            SetAlpha(alpha);

            if (Input.anyKeyDown)
            {
                shouldBlink = false;
                SetAlpha(0f);
                enabled = false;
            }
        }
    }

    private void SetAlpha(float alpha)
    {
        if (text != null)
        {
            Color c = text.color;
            text.color = new Color(c.r, c.g, c.b, alpha);
        }
    }
}