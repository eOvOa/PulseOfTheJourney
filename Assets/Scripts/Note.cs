using System.Collections;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float moveSpeed;
    public int lane;
    
    [HideInInspector]
    public bool judged = false; // Make this public but hide in inspector
    
    private bool missed = false;
    private bool autoMissed = false;

    private static float judgementLineX = 2.932941f;
    private static float perfectWindow = 0.15f;   // 判定容错调整在这
    private static float goodWindow = 0.3f;     

    private SpriteRenderer sr;
    private Animator animator;
    private float missTimer = 0f;

    public Sprite emptySprite;
    private bool canBePressed = false;
    
    // Cache the spawner reference
    private NoteSpawner spawner;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        spawner = Object.FindFirstObjectByType<NoteSpawner>();
    }

    void Update()
    {
        // Move the note
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        // Only process non-judged notes
        if (!judged)
        {
            float distance = Mathf.Abs(transform.position.x - judgementLineX);
            
            // Update pressable state
            if (distance <= goodWindow)
            {
                canBePressed = true;
            }
            
            // Auto-miss if note passes judgment line by too much
            if (!autoMissed && transform.position.x > judgementLineX + goodWindow)
            {
                autoMissed = true; // Prevent double processing
                Miss();
                
                // Debug log for auto-miss
                Debug.Log($"Note in lane {lane} auto-missed at x={transform.position.x:F2}, judgmentLine={judgementLineX:F2}");
            }
        }
        
        // Handle fading out of missed notes
        if (missed)
        {
            missTimer += Time.deltaTime;
            if (missTimer >= 2f)
            {
                Destroy(gameObject);
            }
        }
    }
    
    // This is called by InputManager when the player tries to hit this note
    public void TryJudge()
    {
        // Prevent double judgment
        if (judged) 
        {
            Debug.Log($"Attempted to judge an already judged note in lane {lane}");
            return;
        }

        float distance = Mathf.Abs(transform.position.x - judgementLineX);
        
        // Only judge if in press range
        if (canBePressed)
        {
            if (distance <= perfectWindow)
            {
                PerfectHit();
                Debug.Log($"Perfect hit in lane {lane} at distance {distance:F3}");
            }
            else if (distance <= goodWindow)
            {
                GoodHit();
                Debug.Log($"Good hit in lane {lane} at distance {distance:F3}");
            }
            else
            {
                Miss();
                Debug.Log($"Miss in lane {lane} (out of window) at distance {distance:F3}");
            }
        }
        else
        {
            Miss();
            Debug.Log($"Miss in lane {lane} (not pressable) at position {transform.position.x:F2}");
        }
    }

    private void PerfectHit()
    {
        // Mark as judged first thing
        judged = true;
        canBePressed = false;

        // Visual feedback
        sr.sprite = emptySprite;
        animator.Play("Hit");

        // Score
        ScoreManager.Instance.AddScore(3000);
        
        // Start destruction sequence
        StartCoroutine(HitSequence());
    }

    private void GoodHit()
    {
        // Mark as judged first thing
        judged = true;
        canBePressed = false;

        // Visual feedback
        sr.sprite = emptySprite;
        animator.Play("Hit");

        // Score
        ScoreManager.Instance.AddScore(1500);
        
        // Start destruction sequence
        StartCoroutine(HitSequence());
    }

    private void Miss()
    {
        // Mark as judged first thing
        judged = true;
        missed = true;
        canBePressed = false;

        // Visual feedback - fade
        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f);
        }

        // Score penalty
        ScoreManager.Instance.SubtractScore(2000);
    }

    private IEnumerator HitSequence()
    {
        yield return new WaitForSeconds(0.15f);
        Destroy(gameObject);
    }
    
    void OnDestroy()
    {
        // Make sure we're removed from the spawner when destroyed
        if (spawner != null)
        {
            spawner.RemoveTapNote(lane, gameObject);
        }
    }
}