using System.Collections;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float moveSpeed;
    public int lane;
    
    [HideInInspector]
    public bool judged = false;
    
    private bool missed = false;
    private bool autoMissed = false;

    // 使其成为静态公共属性，方便访问
    public static float JudgementLineX = 2.932941f;
    private static float perfectWindow = 0.15f;
    private static float goodWindow = 0.3f;     

    private SpriteRenderer sr;
    private Animator animator;
    private float missTimer = 0f;

    public Sprite emptySprite;
    private bool canBePressed = false;
    
    // 缓存引用，避免重复查找
    private NoteSpawner spawner;
    private InputManager inputManager;
    
    // 缓存Transform以避免频繁访问
    private Transform myTransform;
    // 缓存当前位置x值以避免每帧都获取
    private float currentX;

    void Awake()
    {
        myTransform = transform;
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // 只在Start中查找一次
        spawner = FindObjectOfType<NoteSpawner>();
        inputManager = FindObjectOfType<InputManager>();
    }

    void Update()
    {
        // 缓存当前位置
        currentX = myTransform.position.x;
        
        // 使用缓存的Transform
        myTransform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (!judged)
        {
            float distance = Mathf.Abs(currentX - JudgementLineX);
            
            // 当音符进入可按范围时
            if (!canBePressed && distance <= goodWindow)
            {
                canBePressed = true;
                // 通知InputManager这个音符可以被按下
                if (inputManager != null)
                {
                    inputManager.AddPressableNote(lane, gameObject);
                }
            }
            
            // 音符超过判定线+判定窗口后，自动判定为Miss
            if (!autoMissed && currentX > JudgementLineX + goodWindow)
            {
                autoMissed = true;
                Miss();
            }
        }
        
        // 处理已错过的音符
        if (missed)
        {
            missTimer += Time.deltaTime;
            if (missTimer >= 1f) // 减少为1秒以减少对象数量
            {
                Destroy(gameObject);
            }
        }
    }
    
    public void TryJudge()
    {
        if (judged) return;

        float distance = Mathf.Abs(currentX - JudgementLineX);
        
        if (canBePressed)
        {
            if (distance <= perfectWindow)
            {
                PerfectHit();
            }
            else if (distance <= goodWindow)
            {
                GoodHit();
            }
            else
            {
                Miss();
            }
        }
        else
        {
            // 只有音符已过判定线才判定为Miss
            if (currentX > JudgementLineX)
            {
                Miss();
            }
        }
    }

    private void PerfectHit()
    {
        judged = true;
        canBePressed = false;

        sr.sprite = emptySprite;
        animator.Play("Hit");

        ScoreManager.Instance.AddScore(3000);
        StartCoroutine(HitSequence());
        
        // 从可按列表中移除
        if (inputManager != null)
        {
            inputManager.RemovePressableNote(lane, gameObject);
        }
    }

    private void GoodHit()
    {
        judged = true;
        canBePressed = false;

        sr.sprite = emptySprite;
        animator.Play("Hit");

        ScoreManager.Instance.AddScore(1500);
        StartCoroutine(HitSequence());
        
        // 从可按列表中移除
        if (inputManager != null)
        {
            inputManager.RemovePressableNote(lane, gameObject);
        }
    }

    private void Miss()
    {
        judged = true;
        missed = true;
        canBePressed = false;

        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f);
        }

        ScoreManager.Instance.SubtractScore(2000);
        
        // 从可按列表中移除
        if (inputManager != null)
        {
            inputManager.RemovePressableNote(lane, gameObject);
        }
    }

    private IEnumerator HitSequence()
    {
        yield return new WaitForSeconds(0.15f);
        Destroy(gameObject);
    }
    
    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.RemoveTapNote(lane, gameObject);
        }
        
        // 确保从可按列表中移除
        if (inputManager != null)
        {
            inputManager.RemovePressableNote(lane, gameObject);
        }
    }
}