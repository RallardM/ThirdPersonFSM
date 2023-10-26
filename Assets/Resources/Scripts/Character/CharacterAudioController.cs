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
    private List<AudioClip> m_footstepAudioClips = new List<AudioClip>();

    public void PlaySound(ESoundType soundType)
    {
        switch (soundType)
        {
            case ESoundType.Jump:
                //Debug.Log("CharacterAudioController : PlaySound() : Jump");
                m_audioSource.clip = m_jumpAudioClip;
                break;
            case ESoundType.Land:
                //Debug.Log("CharacterAudioController : PlaySound() : Land");
                m_audioSource.clip = m_landAudioClip;
                break;
            case ESoundType.Footstep:
                //Debug.Log("CharacterAudioController : PlaySound() : Footstep");
                int randomIndex = Random.Range(0, m_footstepAudioClips.Count);
                m_audioSource.clip = m_footstepAudioClips[randomIndex];
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