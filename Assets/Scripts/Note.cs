using System.Collections;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float moveSpeed;
    private bool judged = false;
    private bool missed = false;

    private static float judgementLineX = 3f; 
    private static float hitWindow = 0.5f;    

    private SpriteRenderer sr;
    private float missTimer = 0f;
    private bool scheduledDestroy = false;

    public Sprite emptySprite; 
    private Animator animator;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
       
        //transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (!judged || missed)
        {
            // move this line so the key stops at wherr you hit it and plays Hit animation
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime); 
        
            if (transform.position.x > judgementLineX) // Immediately change key transparency if passed the judgement line (+2f removed)
            {
                Miss();
            }
        }

        if (missed)
        {
            missTimer += Time.deltaTime;
            if (missTimer >= 2f && !scheduledDestroy)
            {
                Destroy(gameObject);
                scheduledDestroy = true;
            }
        }
    }

    public void TryJudge()
    {
        if (judged) return;

        float distance = Mathf.Abs(transform.position.x - judgementLineX);
        if (distance <= hitWindow)
        {
            Hit();
        }
        else
        {
            Miss();
        }
    }

    private void Hit()
    {
        judged = true;
        sr.sprite = emptySprite; // Fake key disappear
        animator.Play("Hit");

        StartCoroutine(HitSequence());

        IEnumerator HitSequence() // Destroy game object after the animation played
        {
            yield return new WaitForSeconds(0.15f);
            Destroy(gameObject);
        }
    }

    private void Miss()
    {
        judged = true;
        missed = true;

        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f); 
        }
    }
}