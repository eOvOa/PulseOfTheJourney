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
        if (text == null)
        {
            Debug.LogError("[TextOnStartBlink] Text not assigned.");
            return;
        }

        text.gameObject.SetActive(true);
        SetAlpha(1f);
    }

    void Update()
    {
        if (text == null || !shouldBlink) return;

        // 闪烁效果
        timer += Time.deltaTime;
        float alpha = (Mathf.Sin(timer * Mathf.PI / blinkSpeed) + 1f) / 2f;
        SetAlpha(alpha);

        // 任意按键（任何设备）立即停止
        if (Input.anyKeyDown)
        {
            shouldBlink = false;
            SetAlpha(0f);
            Debug.Log("[TextOnStartBlink] Any key detected, hiding text.");
            enabled = false;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, alpha);
    }
}