using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_explosionAudioSource;

    [SerializeField]
    private AudioClip m_explosionAudioClip;

    // The explosion only plays at the end of the intro animation or bty pressing X
    private void Start()
    {
        m_explosionAudioSource.clip = m_explosionAudioClip;
        m_explosionAudioSource.Play();
    }
}
