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
        SetAlpha(0f); // 默认隐藏
    }

    void Update()
    {
        if (!shouldBlink)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key) && IsRealKey(key))
                {
                    pressedKeys.Add(key);
                    if (pressedKeys.Count >= 2)
                    {
                        shouldBlink = true;
                    }
                }
            }
        }
        else
        {
            timer += Time.deltaTime;
            float alpha = (Mathf.Sin(timer * Mathf.PI / blinkSpeed) + 1f) / 2f;
            SetAlpha(alpha);
        }
    }

    private bool IsRealKey(KeyCode key)
    {
        // 可根据项目调整范围
        return ((int)key >= (int)KeyCode.A && (int)key <= (int)KeyCode.Z) ||
               ((int)key >= (int)KeyCode.Alpha0 && (int)key <= (int)KeyCode.Alpha9);
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