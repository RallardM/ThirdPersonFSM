// Source : https://www.youtube.com/watch?v=luSVUPU6bK0
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera m_virtualCamera;
    private float m_shakeIntensity = 6.0f;
    private float m_shakeTime = 0.2f;
    private float m_timer;
    private CinemachineBasicMultiChannelPerlin m_noise;

    private void Awake()
    {
        m_virtualCamera = GetComponent<CinemachineVirtualCamera>();
        m_noise = m_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        StopShake();
    }

    public void ShakeCamera(float shakeIntensity)
    {
        m_noise.m_AmplitudeGain = shakeIntensity;
        m_timer = m_shakeTime;
    }

    void StopShake()
    {
        m_noise.m_AmplitudeGain = 0.0f;
        m_timer = 0.0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShakeCamera(m_shakeIntensity);
        }

        if (m_timer > 0.0f)
        {
            m_timer -= Time.deltaTime;
            if (m_timer <= 0.0f)
            {
                StopShake();
            }
        }
    }
}
