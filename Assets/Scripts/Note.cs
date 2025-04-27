using System.Collections;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float moveSpeed;
    private bool judged = false;
    private bool missed = false;

    private static float judgementLineX = 2.932941f; 
    [SerializeField]
    private float hitWindow = 0.5f; // 容错窗口

    private SpriteRenderer sr;
    private float missTimer = 0f;
    private bool scheduledDestroy = false;

    public Sprite emptySprite;
    private Animator animator;

    private bool canBePressed = false;

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

            if (distance <= hitWindow)
            {
                canBePressed = true;
            }

            if (transform.position.x > judgementLineX + hitWindow)
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
                Destroy(gameObject);
                scheduledDestroy = true;
            }
        }
    }

    public void TryJudge()
    {
        if (judged) return;

        if (canBePressed)
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
        canBePressed = false;

        sr.sprite = emptySprite; // 假装消失
        animator.Play("Hit");

      
        ScoreManager.Instance.AddScore(2000);

        StartCoroutine(HitSequence());

        IEnumerator HitSequence()
        {
            yield return new WaitForSeconds(0.15f);
            Destroy(gameObject);
        }
    }

    private void Miss()
    {
        judged = true;
        missed = true;
        canBePressed = false;

        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f); // 变灰
        }

       
        ScoreManager.Instance.SubtractScore(2000);
    }
}
