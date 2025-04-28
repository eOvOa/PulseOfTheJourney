using UnityEngine;

public class HoldNote : MonoBehaviour
{
    public float moveSpeed;

    private SpriteRenderer backgroundRenderer;
    private SpriteRenderer foregroundRenderer;

    private bool canBePressed = false;
    private bool started = false;
    private bool allowedRelease = false;
    private bool isHolding = false;
    private bool finished = false;
    private bool missed = false;
    private bool scheduledDestroy = false;
    private bool scoringActive = false;
    private bool hasHeldSuccessfully = false;

    private float width;
    private float originalWidth;
    private static float judgementLineX = 2.932941f;
    [SerializeField]
    private float hitWindow = 0.5f;

    private float missTimer = 0f;
    private float scoreTimer = 0f;

    void Start()
    {
        backgroundRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
        foregroundRenderer = transform.Find("Foreground").GetComponent<SpriteRenderer>();

        if (backgroundRenderer == null || foregroundRenderer == null)
        {
            Debug.LogError("HoldNote: 找不到 Background 或 Foreground！");
        }

        width = backgroundRenderer.bounds.size.x;
        originalWidth = width;
    }

    void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        float rightEdge = transform.position.x + width / 2f;
        float leftEdge = transform.position.x - width / 2f;

        if (!canBePressed && rightEdge >= judgementLineX - hitWindow && rightEdge <= judgementLineX + hitWindow)
        {
            canBePressed = true;
        }

        if (canBePressed && rightEdge > judgementLineX + hitWindow)
        {
            if (!started)
            {
                Miss();
            }
            canBePressed = false;
        }

        if (!allowedRelease && Mathf.Abs(leftEdge - judgementLineX) <= hitWindow)
        {
            allowedRelease = true;
        }

        if (!finished && !missed && scoringActive)
        {
            HandleScoreDuringHold();
        }

        if (isHolding && !finished)
        {
            EatHold();
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

        if (transform.position.x > judgementLineX + 10f)
        {
            scoringActive = false;
            Destroy(gameObject);
        }
    }

    public void PlayerPress()
    {
        if (missed || finished) return;

        if (canBePressed)
        {
            started = true;
            isHolding = true;
            scoringActive = true;
            hasHeldSuccessfully = true;
        }
    }

    public void PlayerRelease()
    {
        if (missed || finished) return;

        if (allowedRelease)
        {
            FinishHold();
        }
        else
        {
            EarlyRelease();
        }
    }

    private void EatHold()
    {
        float eatAmount = moveSpeed * Time.deltaTime;

        width -= eatAmount;
        if (width < 0) width = 0;

        transform.localScale = new Vector3(width / originalWidth, transform.localScale.y, transform.localScale.z);
        transform.position -= new Vector3(eatAmount / 2f, 0);
    }

    private void Miss()
    {
        missed = true;
        scoringActive = false;

        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = new Color(backgroundRenderer.color.r, backgroundRenderer.color.g, backgroundRenderer.color.b, 0.25f);
        }
        if (foregroundRenderer != null)
        {
            foregroundRenderer.color = new Color(foregroundRenderer.color.r, foregroundRenderer.color.g, foregroundRenderer.color.b, 0.25f);
        }

        if (!hasHeldSuccessfully)
        {
            ScoreManager.Instance.SubtractScore(3000);
        }
    }

    private void EarlyRelease()
    {
        scoringActive = false;
        isHolding = false;

        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = new Color(backgroundRenderer.color.r, backgroundRenderer.color.g, backgroundRenderer.color.b, 0.25f);
        }
        if (foregroundRenderer != null)
        {
            foregroundRenderer.color = new Color(foregroundRenderer.color.r, foregroundRenderer.color.g, foregroundRenderer.color.b, 0.25f);
        }
    }

    private void FinishHold()
    {
        finished = true;
        scoringActive = false;
        isHolding = false;
        Destroy(gameObject, 0.2f);
    }

    private void HandleScoreDuringHold()
    {
        scoreTimer += Time.deltaTime;

        if (scoreTimer >= 0.001f)
        {
            if (isHolding)
            {
                ScoreManager.Instance.AddScore(1);
            }
            scoreTimer = 0f;
        }
    }

    public bool CanBePressed()
    {
        return canBePressed;
    }
}
