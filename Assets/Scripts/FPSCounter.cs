using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class FPSCounter : MonoBehaviour
{
    private const float fpsMeasurePeriod = 0.5f;

    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps = 0;
    private int m_PreviousFps = -1;

    [SerializeField] private TMP_Text m_Text;

    private void Awake()
    {
        if (m_Text == null)
            m_Text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        m_FpsNextPeriod = Time.unscaledTime + fpsMeasurePeriod;
    }

    private void Update()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        return;
#endif

        m_FpsAccumulator++;
        float currentTime = Time.unscaledTime;

        if (currentTime >= m_FpsNextPeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod = currentTime + fpsMeasurePeriod;

            if (m_CurrentFps != m_PreviousFps)
            {
                m_Text.text = m_CurrentFps + " FPS";
                m_PreviousFps = m_CurrentFps;
            }
        }
    }
}