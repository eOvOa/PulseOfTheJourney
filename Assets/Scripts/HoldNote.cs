using UnityEngine;

public class HoldNode : MonoBehaviour
{
    public float moveSpeed;

    private bool started = false;
    private bool allowedRelease = false;
    private bool isHolding = false;
    private bool finished = false;
    private bool missed = false;
    private bool releasedEarly = false;

    private SpriteRenderer sr;
    private float width;
    private static float judgementLineX = 3f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        width = sr.bounds.size.x;
    }

    void Update()
    {
        if (finished) return;

        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        float rightEdge = transform.position.x + width / 2f;
        float leftEdge = transform.position.x - width / 2f;

        if (!started && rightEdge >= judgementLineX)
        {
            started = true;
            if (!isHolding)
            {
                Miss();
            }
        }

        if (!allowedRelease && leftEdge >= judgementLineX)
        {
            allowedRelease = true;
        }

        if (transform.position.x > judgementLineX + 5f)
        {
            Destroy(gameObject);
        }
    }

    public void PlayerPress()
    {
        if (missed || finished) return;
        if (started) isHolding = true;
    }

    public void PlayerRelease()
    {
        if (missed || finished) return;
        if (allowedRelease)
            FinishHold();
        else
            EarlyRelease();
    }

    private void Miss()
    {
        missed = true;
        if (sr != null) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f);
    }

    private void EarlyRelease()
    {
        releasedEarly = true;
        if (sr != null) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f);
    }

    private void FinishHold()
    {
        finished = true;
        Destroy(gameObject, 0.2f);
    }
}