using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TextAfterSecondKeyBlink : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float blinkSpeed = 1f;

    private bool shouldBlink = false;
    private float timer;
    private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();

    void Start()
    {
        if (text == null)
        {
            Debug.LogError("[TextAfterSecondKeyBlink] Text not assigned!");
            return;
        }

        text.gameObject.SetActive(true); // 强制激活 Text 对象
        SetAlpha(0f); // 初始隐藏
        Debug.Log("[TextAfterSecondKeyBlink] Initialized. Waiting for input...");
    }

    void Update()
    {
        if (text == null) return;

        if (shouldBlink)
        {
            timer += Time.deltaTime;
            float alpha = (Mathf.Sin(timer * Mathf.PI / blinkSpeed) + 1f) / 2f;
            SetAlpha(alpha);
        }
        else
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key) && IsRealKey(key))
                {
                    if (!pressedKeys.Contains(key))
                    {
                        pressedKeys.Add(key);
                        Debug.Log("[TextAfterSecondKeyBlink] Key pressed: " + key);
                    }

                    if (pressedKeys.Count >= 2)
                    {
                        Debug.Log("[TextAfterSecondKeyBlink] Two distinct keys detected. Starting blink.");
                        shouldBlink = true;
                        timer = 0;
                        SetAlpha(1f);
                        break;
                    }
                }
            }

            // 临时调试热键：按 M 直接触发
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("[TextAfterSecondKeyBlink] Manual trigger by M key.");
                shouldBlink = true;
                timer = 0;
                SetAlpha(1f);
            }
        }
    }

    private bool IsRealKey(KeyCode key)
    {
        // 仅接受字母与数字键
        return (key >= KeyCode.A && key <= KeyCode.Z) ||
               (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9);
    }

    private void SetAlpha(float alpha)
    {
        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, alpha);
    }
}
