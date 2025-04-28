using System.Collections;
using UnityEngine;

public class HoldNote : MonoBehaviour
{
    public float moveSpeed;
    public int lane;

    private static float judgementLineX = 2.932941f;
    [SerializeField]
    private float hitWindow = 0.5f;

    private SpriteRenderer backgroundRenderer;
    private SpriteRenderer foregroundRenderer;
    private float missTimer = 0f;
    private bool scheduledDestroy = false;

    private bool isHolding = false;
    private bool startedJudging = false;
    private bool missed = false;

    private Vector3 originalBackgroundScale;
    private float holdTotalWidth;

    void Start()
    {
        backgroundRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
        foregroundRenderer = transform.Find("Foreground").GetComponent<SpriteRenderer>();

        if (backgroundRenderer == null || foregroundRenderer == null)
        {
            Debug.LogError("HoldNote: 找不到 Background 或 Foreground！");
        }

        originalBackgroundScale = backgroundRenderer.transform.localScale;
        holdTotalWidth = backgroundRenderer.bounds.size.x;
    }

    void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (!startedJudging)
        {
            float rightEdge = transform.position.x + holdTotalWidth / 2;
            float distance = Mathf.Abs(rightEdge - judgementLineX);

            if (distance <= hitWindow)
            {
                startedJudging = true;
            }
        }

        if (startedJudging && !missed)
        {
            if (!isHolding)
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

    public void OnHold(int inputLane)
    {
        if (!startedJudging) return;
        if (missed) return;
        if (inputLane != lane) return;

        isHolding = true;

        float rightEdge = transform.position.x + holdTotalWidth / 2;
        if (rightEdge <= judgementLineX)
        {
            CompleteHold();
        }
        else
        {
            float cutRatio = (rightEdge - judgementLineX) / holdTotalWidth;
            if (cutRatio < 0) cutRatio = 0;

            backgroundRenderer.transform.localScale = new Vector3(originalBackgroundScale.x * cutRatio, originalBackgroundScale.y, originalBackgroundScale.z);
        }
    }

    public void OnRelease(int inputLane)
    {
        if (!startedJudging) return;
        if (missed) return;
        if (inputLane != lane) return;

        isHolding = false;
    }

    private void CompleteHold()
    {
        if (missed) return;

        startedJudging = false;
        isHolding = false;

        backgroundRenderer.color = new Color(1f, 1f, 1f, 0f);
        foregroundRenderer.color = new Color(1f, 1f, 1f, 0f);

        ScoreManager.Instance.AddScore(3000);

        StartCoroutine(HitSequence());
    }

    private void Miss()
    {
        missed = true;
        isHolding = false;

        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = new Color(1f, 1f, 1f, 0.25f);
        }
        if (foregroundRenderer != null)
        {
            foregroundRenderer.color = new Color(1f, 1f, 1f, 0.25f);
        }

        ScoreManager.Instance.SubtractScore(3000);
    }

    private IEnumerator HitSequence()
    {
        yield return new WaitForSeconds(0.15f);
        Destroy(gameObject);
    }
}
