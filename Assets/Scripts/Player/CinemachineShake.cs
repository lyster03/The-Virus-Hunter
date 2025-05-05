using System.Collections;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;

    private float initialAmplitude;
    private float initialFrequency;
    private int defaultSeed;

    private void Awake()
    {
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        noise = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise != null)
        {
            initialAmplitude = noise.m_AmplitudeGain;
            initialFrequency = noise.m_FrequencyGain;
            defaultSeed = noise.m_NoiseProfile != null ? noise.m_NoiseProfile.GetHashCode() : 0;
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        if (noise == null) return;

        noise.m_AmplitudeGain = intensity;
        noise.m_FrequencyGain = 1.0f;

        StopAllCoroutines();
        StartCoroutine(ResetNoiseAfter(time));
    }

    private IEnumerator ResetNoiseAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;

            
            var profile = noise.m_NoiseProfile;
            noise.m_NoiseProfile = null; 
            yield return null;           
            noise.m_NoiseProfile = profile; 
        }
    }

}
