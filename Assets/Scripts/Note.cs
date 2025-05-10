using System.Collections;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float moveSpeed;
    public int lane;
    
    [HideInInspector]
    public bool judged = false;
    
    [HideInInspector]
    public bool missed = false;
    private bool autoMissed = false;

    public static float JudgementLineX = 2.932941f;
    private static float perfectWindow = 0.15f;
    private static float goodWindow = 0.3f;     

    private SpriteRenderer sr;
    private Animator animator;
    private float missTimer = 0f;

    public Sprite emptySprite;
    private bool canBePressed = false;
    
    private NoteSpawner spawner;
    private InputManager inputManager;
    
    private Transform myTransform;
    private float currentX;

    void Awake()
    {
        myTransform = transform;
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        spawner = FindObjectOfType<NoteSpawner>();
        inputManager = FindObjectOfType<InputManager>();
    }

    void Update()
    {
        currentX = myTransform.position.x;
        
        myTransform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (!judged)
        {
            float distance = Mathf.Abs(currentX - JudgementLineX);
            
            if (!canBePressed && distance <= goodWindow)
            {
                canBePressed = true;
                if (inputManager != null)
                {
                    inputManager.AddPressableNote(lane, gameObject);
                }
            }
            
            if (!autoMissed && currentX > JudgementLineX + goodWindow)
            {
                autoMissed = true;
                Miss();
            }
        }
        
        if (missed)
        {
            missTimer += Time.deltaTime;
            if (missTimer >= 1f)
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
        
        if (inputManager != null)
        {
            inputManager.RemovePressableNote(lane, gameObject);
        }
    }
}