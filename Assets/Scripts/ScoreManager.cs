using UnityEngine;
using TMPro; // ✅ 记得改用 TextMeshPro命名空间

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public TextMeshProUGUI scoreText; // ✅ 用TextMeshProUGUI替代老的Text

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void SubtractScore(int amount)
    {
        score -= amount;
        if (score < 0) score = 0;
    }
}