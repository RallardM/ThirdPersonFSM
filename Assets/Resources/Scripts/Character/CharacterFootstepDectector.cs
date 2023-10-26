using UnityEngine;

public class CharacterFootstepDectector : MonoBehaviour
{
    [SerializeField]
    CharacterAudioController m_audioController;

    private void OnTriggerEnter(Collider other)
    {
        // If the current CharacterState is Freestate
        

        // Layer 7 (floor)
        if (other.gameObject.layer == 7)
        {
            m_audioController.PlaySound(ESoundType.Footstep);
            Debug.Log("CharacterFootstepDectector : OnTriggerEnter() : Footstep");
        }
    }
}
