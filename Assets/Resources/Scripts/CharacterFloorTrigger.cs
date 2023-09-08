using UnityEngine;

public class CharacterFloorTrigger : MonoBehaviour
{
    public bool IsOnFloor {  get; private set; }
    
    private void OnTriggerStay(Collider other)
    {
        if (!IsOnFloor)
        {
            //Debug.Log("Touched the ground");
        }
        
        IsOnFloor = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Left the ground");
        IsOnFloor = false; 
    }
}
