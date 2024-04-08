using System.Collections.Generic;
using UnityEngine;

public class CharacterAudioController : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_feetAudioSource;

    [SerializeField]
    private AudioSource m_handAudioSource;

    [SerializeField]
    private AudioSource m_mouthAudioSource;

    [SerializeField]
    private AudioClip m_jumpAudioClip;

    [SerializeField]
    private AudioClip m_landAudioClip;

    [SerializeField]
    private List<AudioClip> m_footstepAudioClips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> m_slapAudioClips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> m_gruntAudioClips = new List<AudioClip>();

    public void PlaySound(ESoundType soundType)
    {
        EAudioSource source = EAudioSource.Count;
        AudioClip clip = null;
        int randomIndex = 0;
        switch (soundType)
        {
            case ESoundType.Jump:
                //Debug.Log("CharacterAudioController : PlaySound() : Jump");
                clip = m_jumpAudioClip;
                source = EAudioSource.Feet;
                break;
            case ESoundType.Land:
                //Debug.Log("CharacterAudioController : PlaySound() : Land");
                clip = m_landAudioClip;
                source = EAudioSource.Feet;
                break;
            case ESoundType.Footstep:
                //Debug.Log("CharacterAudioController : PlaySound() : Footstep");
                randomIndex = Random.Range(0, m_footstepAudioClips.Count);
                clip = m_footstepAudioClips[randomIndex];
                source = EAudioSource.Feet;
                break;
            case ESoundType.Slap:
                //Debug.Log("CharacterAudioController : PlaySound() : Slap");
                randomIndex = Random.Range(0, m_slapAudioClips.Count);
                clip = m_slapAudioClips[randomIndex];
                source = EAudioSource.Hand;
                break;
            case ESoundType.Grunt:
                //Debug.Log("CharacterAudioController : PlaySound() : Grunt");
                randomIndex = Random.Range(0, m_gruntAudioClips.Count);
                clip = m_gruntAudioClips[randomIndex];
                source = EAudioSource.Mouth;
                break;
            case ESoundType.Count:
                Debug.LogWarning("CharacterAudioController : PlaySound() : Sound type not implemented");
                break;
        }

        switch(source)
        {
            case EAudioSource.Feet:
                m_feetAudioSource.clip = clip;
                m_feetAudioSource.Play();
                break;
            case EAudioSource.Hand:
                m_handAudioSource.clip = clip;
                m_handAudioSource.Play();
                break;
            case EAudioSource.Mouth:
                m_mouthAudioSource.clip = clip;
                m_mouthAudioSource.Play();
                break;
            case EAudioSource.Count:
                Debug.LogWarning("CharacterAudioController : PlaySound() : Audio source not implemented");
                break;
        }
    }
}

public enum ESoundType
{
    Jump,
    Land,
    Footstep,
    Slap,
    Grunt,
    Count
}

public enum EAudioSource
{
    Feet,
    Hand,
    Mouth,
    Count
}