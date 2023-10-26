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
            m_audioController.PlaySound(ESoundType.Footstep);
        }
    }
}
