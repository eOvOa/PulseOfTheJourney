using System.Collections;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float moveSpeed;
    public int lane;
    private bool judged = false;
    private bool missed = false;

    private static float judgementLineX = 2.932941f;
    private static float perfectWindow = 0.15f;  
    private static float goodWindow = 0.3f;     

    private SpriteRenderer sr;
    private Animator animator;
    private float missTimer = 0f;
    private bool scheduledDestroy = false;

    public Sprite emptySprite;
    private bool canBePressed = false;
    

    public bool IsJudged { get { return judged; } }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (!judged)
        {
            float distance = Mathf.Abs(transform.position.x - judgementLineX);

            if (distance <= goodWindow)
            {
                canBePressed = true;
            }

            if (transform.position.x > judgementLineX + goodWindow)
            {
                if (!judged)
                {
                    Miss();
                }
            }
        }

        if (missed)
        {
            missTimer += Time.deltaTime;
            if (missTimer >= 2f && !scheduledDestroy)
            {
                scheduledDestroy = true;
                Destroy(gameObject);
            }
        }
    }

    public void TryJudge()
    {
        if (judged) return;

        if (canBePressed)
        {
            float distance = Mathf.Abs(transform.position.x - judgementLineX);

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
            Miss();
        }
    }

    private void PerfectHit()
    {
        judged = true;
        canBePressed = false;

        sr.sprite = emptySprite; 
        animator.Play("Hit");

        ScoreManager.Instance.AddScore(3000); 
        
       
        NoteSpawner spawner = Object.FindFirstObjectByType<NoteSpawner>();
        if (spawner != null)
        {
            spawner.RemoveTapNote(lane, gameObject);
        }
        
        StartCoroutine(HitSequence());
    }

    private void GoodHit()
    {
        judged = true;
        canBePressed = false;

        sr.sprite = emptySprite;
        animator.Play("Hit");

        ScoreManager.Instance.AddScore(1500); 
        
       
        NoteSpawner spawner = Object.FindFirstObjectByType<NoteSpawner>();
        if (spawner != null)
        {
            spawner.RemoveTapNote(lane, gameObject);
        }
        
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
        

        NoteSpawner spawner = Object.FindFirstObjectByType<NoteSpawner>();
        if (spawner != null)
        {
            spawner.RemoveTapNote(lane, gameObject);
        }
    }

    private IEnumerator HitSequence()
    {
        yield return new WaitForSeconds(0.15f);
        Destroy(gameObject);
    }
}