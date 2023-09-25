using UnityEngine;

public class CharacterFloorTrigger : MonoBehaviour
{
    [field: SerializeField]
    private CharacterControllerStateMachine m_stateMachine { get; set; }
    public bool IsOnFloor {  get; private set; }
    
    private void OnTriggerStay(Collider other)
    {
        if (!IsOnFloor)
        {
            //Debug.Log("Touched the ground");
        }
        m_stateMachine.SetTouchGround(true);
        IsOnFloor = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Left the ground");
        m_stateMachine.SetTouchGround(false);
        IsOnFloor = false; 
    }
}
