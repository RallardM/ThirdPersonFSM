using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField]
    private EDamageType eDamageType = EDamageType.Count;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On trigger enter damage source");
        var charController = other.GetComponent<CharacterControllerStateMachine>();
        if (charController == null)
        {
            return;
        }

        charController.ReceiveDamage(eDamageType);
        Debug.Log(other.name + " receives damage of type: " + eDamageType.ToString());
    }
}

public enum EDamageType
{
    Normal,
    Stunning,
    Count
}