using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudioController : MonoBehaviour
{
    [SerializeField] 
    private AudioSource m_audioSource;
    [SerializeField]
    private AudioClip m_jumpAudioClip;
    [SerializeField]
    private AudioClip m_landAudioClip;

    public void PlaySound(ESoundType soundType)
    {
        switch (soundType)
        {
            case ESoundType.Jump:
                m_audioSource.clip = m_jumpAudioClip;
                break;
            case ESoundType.Land:
                m_audioSource.clip = m_landAudioClip;
                break;
            case ESoundType.Count:
                Debug.LogWarning("CharacterAudioController : PlaySound() : Sound type not implemented");
                break;
        }

        m_audioSource.Play();
    }
}

public enum ESoundType
{
    Jump,
    Land,
    Count
}