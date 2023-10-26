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
    [SerializeField]
    private AudioClip m_footstepAudioClip;

    public void PlaySound(ESoundType soundType)
    {
        switch (soundType)
        {
            case ESoundType.Jump:
                Debug.Log("CharacterAudioController : PlaySound() : Jump");
                m_audioSource.clip = m_jumpAudioClip;
                break;
            case ESoundType.Land:
                Debug.Log("CharacterAudioController : PlaySound() : Land");
                m_audioSource.clip = m_landAudioClip;
                break;
            case ESoundType.Footstep:
                Debug.Log("CharacterAudioController : PlaySound() : Footstep");
                m_audioSource.clip = m_footstepAudioClip;
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
    Footstep,
    Count
}