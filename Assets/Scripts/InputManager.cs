using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private JudgementLine judgementLine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            judgementLine.TryJudge(0); // 红

        if (Input.GetKeyDown(KeyCode.S))
            judgementLine.TryJudge(1); // 蓝

        if (Input.GetKeyDown(KeyCode.D))
            judgementLine.TryJudge(2); // 白

        if (Input.GetKeyDown(KeyCode.F))
            judgementLine.TryJudge(3); // 绿
    }
}