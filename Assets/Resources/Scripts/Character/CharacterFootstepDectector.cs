using UnityEngine;

public class CharacterFootstepDectector : MonoBehaviour
{
    [SerializeField]
    private CharacterAudioController m_audioController;

    [SerializeField]
    private CharacterControllerStateMachine m_characterStateMachine;

    private void OnTriggerEnter(Collider other)
    {
        // If the current state is not freestate returns
        if (m_characterStateMachine.IsInContactWithFloor() == false)
        {
            return;
        }

        // Layer 7 (floor)
        if (other.gameObject.layer == 7)
        {
            Debug.Log("CharacterFootstepDectector : OnTriggerEnter() : Layer 7");
            CharacterAudioController a = m_audioController;
            ESoundType b = ESoundType.Footstep;
            Debug.Log("CharacterFootstepDectector : OnTriggerEnter() : Enters PlaySound()");
            m_audioController.PlaySound(ESoundType.Footstep);
            Debug.Log("CharacterFootstepDectector : OnTriggerEnter() : PlaySound() Done");
        }
    }
}
