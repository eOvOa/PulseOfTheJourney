using UnityEngine;

public class HoldNote : MonoBehaviour
{
    public float moveSpeed;

    private SpriteRenderer backgroundRenderer;

    private bool canBePressed = false;
    private bool started = false;
    private bool allowedRelease = false;
    private bool isHolding = false;
    private bool finished = false;
    private bool missed = false;
    private bool scheduledDestroy = false;

    private float width;
    private float originalWidth;
    private static float judgementLineX = 2.932941f;
    [SerializeField]
    private float hitWindow = 0.5f;

    private float missTimer = 0f;

    void Start()
    {
        Transform backgroundTransform = transform.Find("Background");
        if (backgroundTransform != null)
        {
            backgroundRenderer = backgroundTransform.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("Background not found under HoldNote prefab!");
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
            Destroy(gameObject);
        }

        //实时加分或者扣分
        if (isHolding && !finished)
        {
            int points = Mathf.RoundToInt(3 * Time.deltaTime * 1000);
            ScoreManager.Instance.AddScore(points);
        }
        else if (missed && !finished) 
        {
            int points = Mathf.RoundToInt(1 * Time.deltaTime * 1000);
            ScoreManager.Instance.SubtractScore(points);
        }
    }

    public void PlayerPress()
    {
        if (missed || finished) return;

        if (canBePressed)
        {
            started = true;
            isHolding = true;
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
        transform.position -= new Vector3(eatAmount / 2f, 0, 0);
    }

    private void Miss()
    {
        missed = true;
        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0.25f);
        }
    }

    private void EarlyRelease()
    {
        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0.25f);
        }
        isHolding = false;
        missed = true; 
    }

    private void FinishHold()
    {
        finished = true;
        Destroy(gameObject, 0.2f);
    }

    public bool CanBePressed()
    {
        return canBePressed;
    }
}
