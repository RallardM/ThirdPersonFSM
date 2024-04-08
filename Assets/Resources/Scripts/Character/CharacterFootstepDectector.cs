using UnityEngine;

public class CharacterFootstepDectector : MonoBehaviour
{
    [SerializeField]
    private CharacterAudioController m_audioController;

    [SerializeField]
    private CharacterControllerStateMachine m_characterStateMachine;

    private void OnTriggerEnter(Collider other)
    {
        if (m_characterStateMachine == null)
        {
            // Is not the player
            PlayFloorLayerFootstepSound(other);
            return;
        }

        // If the current state is not freestate returns
        // permits the landing sound to be heard instead
        // f the footstep sounds.
        if (m_characterStateMachine.IsInContactWithFloor() == false)
        {
            return;
        }

        PlayFloorLayerFootstepSound(other);
    }

    private void PlayFloorLayerFootstepSound(Collider other)
    {
        // Layer 7 (floor)
        if (other.gameObject.layer == 7)
        {
            m_audioController.PlaySound(ESoundType.Footstep);
        }
    }
}
