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

    private static float judgementLineX = 2.932941f;
    private static float perfectWindow = 0.15f;
    private static float goodWindow = 0.3f;     

    private SpriteRenderer sr;
    private Animator animator;
    private float missTimer = 0f;

    public Sprite emptySprite;
    private bool canBePressed = false;
    
    // Cache references to avoid repeated lookups
    private NoteSpawner spawner;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // Only look up once in Start
        spawner = Object.FindFirstObjectByType<NoteSpawner>();
    }

    void Update()
    {
        // Move note rightward (toward judgment line)
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (!judged)
        {
            float distance = Mathf.Abs(transform.position.x - judgementLineX);
            
            // Enable pressing when note is within good window
            if (distance <= goodWindow)
            {
                canBePressed = true;
            }
            
            // Auto-miss if note passes beyond judgment line + good window
            // FIX: Only miss if the note is PAST the judgment line
            if (!autoMissed && transform.position.x > judgementLineX + goodWindow)
            {
                autoMissed = true;
                Miss();
            }
        }
        
        // Destroy missed notes after timer
        if (missed)
        {
            missTimer += Time.deltaTime;
            if (missTimer >= 2f)
            {
                Destroy(gameObject);
            }
        }
    }
    
    public void TryJudge()
    {
        if (judged) return;

        float distance = Mathf.Abs(transform.position.x - judgementLineX);
        
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
            // FIX: Don't automatically miss if the note hasn't reached the judgment area yet
            // Only miss if it's past the judgment line
            if (transform.position.x > judgementLineX)
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
    }

    private void GoodHit()
    {
        judged = true;
        canBePressed = false;

        sr.sprite = emptySprite;
        animator.Play("Hit");

        ScoreManager.Instance.AddScore(1500);
        StartCoroutine(HitSequence());
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
    }
}