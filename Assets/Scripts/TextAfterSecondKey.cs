using UnityEngine;
using TMPro;
using System.Collections;

public class TextOnAnyKeyBlink : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float blinkSpeed = 1f;
    public float delayBeforeBlink = 1f; // 延迟开始闪烁

    private bool shouldBlink = false;
    private float timer = 0f;
    private bool triggered = false;

    void Start()
    {
        if (text == null)
        {
            Debug.LogError("[TextOnAnyKeyBlink] Text not assigned!");
            return;
        }

        text.gameObject.SetActive(true);
        SetAlpha(0f); // 初始隐藏
    }

    void Update()
    {
        if (text == null) return;

        if (!triggered && Input.anyKeyDown)
        {
            triggered = true;
            Debug.Log("[TextBlink] Key pressed — will start blinking in " + delayBeforeBlink + "s");
            StartCoroutine(StartBlinkAfterDelay());
        }

        if (shouldBlink)
        {
            timer += Time.deltaTime;
            float alpha = (Mathf.Sin(timer * Mathf.PI / blinkSpeed) + 1f) / 2f;
            SetAlpha(alpha);
        }
    }

    private IEnumerator StartBlinkAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeBlink);
        shouldBlink = true;
        timer = 0f;
        Debug.Log("[TextBlink] Blinking started.");
    }

    private void SetAlpha(float alpha)
    {
        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, alpha);
    }
}