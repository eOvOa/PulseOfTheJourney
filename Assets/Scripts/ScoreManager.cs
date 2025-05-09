using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public TextMeshProUGUI scoreText;
    
    // 添加用于显示的游戏对象
    public GameObject missSprite;    // Miss提示的游戏对象
    public GameObject niceSprite;    // Nice提示的游戏对象
    public GameObject comboSprite;   // Combo提示的游戏对象
    
    // 连击相关变量
    private int currentCombo = 0;  // 总连击数
    private int holdCount = 0;     // Hold音符的击中数量
    private float lastHitTime = 0f;
    private float comboResetTime = 2.0f; // 如果超过2秒没有击中，combo重置
    
    // 所有提示显示的固定持续时间
    private float feedbackDisplayTime = 0.5f;
    
    // 跟踪上一次检查的时间，用于超时检测
    private float lastComboCheckTime = 0f;
    private float comboCheckInterval = 0.1f; // 每0.1秒检查一次连击状态
    
    // 跟踪活跃的提示协程
    private Coroutine activeFeedbackCoroutine = null;

    private void Awake()
    {
        Instance = this;
        
        // 确保提示初始时是隐藏的
        HideAllFeedback();
        
        lastComboCheckTime = Time.time;
    }

    private void Update()
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {score}";
        }
        
        // 定期检查连击状态，防止超时
        if (Time.time - lastComboCheckTime > comboCheckInterval)
        {
            lastComboCheckTime = Time.time;
            
            // 检查是否需要重置连击（超时未击中）
            if (currentCombo > 0 && Time.time - lastHitTime > comboResetTime)
            {
                // 显示Miss并重置连击
                RegisterMiss();
            }
        }
    }

    // 正确击中普通音符
    public void AddScore(int amount)
    {
        score += amount;
        currentCombo++;
        lastHitTime = Time.time;
        
        // 显示相应的连击提示
        ShowHitFeedback();
    }
    
    // 正确击中Hold音符
    public void AddHoldNoteScore(int amount)
    {
        score += amount;
        currentCombo++;
        holdCount++; // 特别记录Hold音符的击中次数
        lastHitTime = Time.time;
        
        // 显示相应的连击提示
        ShowHitFeedback();
    }
    
    // Hold音符持续时加分，但不增加连击数
    public void AddHoldScore(int amount)
    {
        // 仅增加分数，不增加连击，不显示新提示
        score += amount;
        lastHitTime = Time.time;
    }

    // 未击中时调用此方法
    public void SubtractScore(int amount)
    {
        score -= amount;
        if (score < 0) score = 0;
        
        // 显示Miss提示
        ShowMissFeedback();
        
        // 重置连击
        ResetCombo();
    }
    
    // 重置连击数
    private void ResetCombo()
    {
        currentCombo = 0;
        holdCount = 0;
    }
    
    // 显示击中后的连击提示
    private void ShowHitFeedback()
    {
        // 停止所有正在运行的显示协程
        if (activeFeedbackCoroutine != null)
        {
            StopCoroutine(activeFeedbackCoroutine);
            activeFeedbackCoroutine = null;
        }
        
        // 隐藏所有提示
        HideAllFeedback();
        
        // 根据连击数选择要显示的提示
        if (currentCombo >= 5) // 总连击数达到5（Hold和Tap的组合）
        {
            // 五连击及以上显示Combo
            activeFeedbackCoroutine = StartCoroutine(ShowTemporaryFeedback(comboSprite));
        }
        else if (holdCount >= 3) // 如果Hold音符达到3个
        {
            // 三个Hold显示Nice
            activeFeedbackCoroutine = StartCoroutine(ShowTemporaryFeedback(niceSprite));
        }
    }
    
    // 显示Miss提示（无论连击状态如何）
    private void ShowMissFeedback()
    {
        // 停止所有正在运行的显示协程
        if (activeFeedbackCoroutine != null)
        {
            StopCoroutine(activeFeedbackCoroutine);
            activeFeedbackCoroutine = null;
        }
        
        // 隐藏所有提示
        HideAllFeedback();
        
        // 显示Miss提示
        activeFeedbackCoroutine = StartCoroutine(ShowTemporaryFeedback(missSprite));
    }
    
    // 隐藏所有提示
    private void HideAllFeedback()
    {
        if (missSprite != null) missSprite.SetActive(false);
        if (niceSprite != null) niceSprite.SetActive(false);
        if (comboSprite != null) comboSprite.SetActive(false);
    }
    
    // 临时显示提示然后隐藏 - 固定显示0.5秒
    private IEnumerator ShowTemporaryFeedback(GameObject feedbackObject)
    {
        if (feedbackObject == null) yield break;
        
        // 显示提示
        feedbackObject.SetActive(true);
        
        // 等待固定的0.5秒
        yield return new WaitForSeconds(feedbackDisplayTime);
        
        // 隐藏提示
        feedbackObject.SetActive(false);
        
        // 重置协程引用
        activeFeedbackCoroutine = null;
    }
    
    // 公共方法处理Miss情况（无论是主动miss还是超时miss）
    public void RegisterMiss()
    {
        // 显示Miss提示
        ShowMissFeedback();
        
        // 重置连击数
        ResetCombo();
    }
}